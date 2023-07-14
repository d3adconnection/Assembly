﻿using System.Collections.Generic;
using System.IO;
using Blamite.Blam.Localization;
using Blamite.Blam.Util;
using Blamite.Serialization;
using Blamite.IO;
using Blamite.Util;

namespace Blamite.Blam.ThirdGen.Localization
{
	public class ThirdGenLanguage
	{
		private readonly AESKey _encryptionKey;
		private readonly StructureLayout _pointerLayout;
		private readonly int _sizeAlign;
		private readonly bool _hashes;

		public ThirdGenLanguage(GameLanguage language, StructureValueCollection values, FileSegmenter segmenter,
			FileSegmentGroup localeArea, EngineDescription buildInfo)
		{
			Language = language;
			_pointerLayout = buildInfo.Layouts.GetLayout("locale index table element");
			_encryptionKey = buildInfo.LocaleKey;
			_hashes = buildInfo.UsesStringHashes;
			_sizeAlign = (_encryptionKey != null) ? AES.BlockSize : 1;
			Load(values, segmenter, localeArea);
		}

		public GameLanguage Language { get; private set; }
		public int StringCount { get; private set; }
		public FileSegment LocaleIndexTable { get; private set; }
		public FileSegment LocaleData { get; private set; }
		public SegmentPointer LocaleIndexTableLocation { get; set; }
		public SegmentPointer LocaleDataLocation { get; set; }
		public byte[] IndexTableHash { get; private set; }
		public byte[] StringDataHash { get; private set; }

		public StructureValueCollection Serialize()
		{
			var result = new StructureValueCollection();
			result.SetInteger("string count", (uint) StringCount);
			result.SetInteger("locale table size", LocaleData != null ? (uint) LocaleData.Size : 0);

			if (LocaleIndexTableLocation != null)
				result.SetInteger("locale index table offset", (uint)LocaleIndexTableLocation.AsPointer());
			if (LocaleDataLocation != null)
				result.SetInteger("locale data index offset", (uint)LocaleDataLocation.AsPointer());

			if (IndexTableHash != null)
				result.SetRaw("index table hash", IndexTableHash);
			if (StringDataHash != null)
				result.SetRaw("string data hash", StringDataHash);

			return result;
		}

		private void Load(StructureValueCollection values, FileSegmenter segmenter, FileSegmentGroup localeArea)
		{
			StringCount = (int) values.GetInteger("string count");
			if (StringCount > 0)
			{
				// Index table offset, segment, and pointer
				uint localeIndexTableOffset = localeArea.PointerToOffset((uint)values.GetInteger("locale index table offset"));
				LocaleIndexTable = segmenter.WrapSegment(localeIndexTableOffset, (uint)StringCount*8, 8, SegmentResizeOrigin.End);
				LocaleIndexTableLocation = localeArea.AddSegment(LocaleIndexTable);

				// Data offset, segment, and pointer
				uint localeDataOffset = localeArea.PointerToOffset((uint)values.GetInteger("locale data index offset"));
				var localeDataSize = (uint) values.GetInteger("locale table size");
				LocaleData = segmenter.WrapSegment(localeDataOffset, localeDataSize, _sizeAlign, SegmentResizeOrigin.End);
				LocaleDataLocation = localeArea.AddSegment(LocaleData);

				// Load hashes if they exist
				if (values.HasRaw("index table hash"))
					IndexTableHash = values.GetRaw("index table hash");
				if (values.HasRaw("string data hash"))
					StringDataHash = values.GetRaw("string data hash");
			}
		}

		public List<LocalizedString> LoadStrings(IReader reader)
		{
			var result = new List<LocalizedString>();
			if (StringCount == 0)
				return result;

			byte[] stringData = ReadLocaleData(reader);
			using (var stringReader = new EndianReader(new MemoryStream(stringData), reader.Endianness))
			{
				reader.SeekTo(LocaleIndexTableLocation.AsOffset());

				// Read each locale
				for (int i = 0; i < StringCount; i++)
				{
					// Read the offset and stringID
					StringID id;
					int offset;
					ReadLocalePointer(reader, out id, out offset);

					if (offset >= stringReader.Length)
						break; // Bad table - bail out so we don't end up in a huge memory-hogging loop

					stringReader.SeekTo(offset);
					string locale = stringReader.ReadUTF8();
					result.Add(new LocalizedString(id, locale));
				}
			}
			return result;
		}

		public void SaveStrings(List<LocalizedString> locales, IStream stream)
		{
			if (LocaleData == null || LocaleIndexTable == null)
				return;

			using (var offsetData = new MemoryStream())
			using (var stringData = new MemoryStream())
			using (var offsetWriter = new EndianWriter(offsetData, stream.Endianness))
			using (var stringWriter = new EndianWriter(stringData, stream.Endianness))
			{
				// Write the string and offset data to buffers
				foreach (LocalizedString locale in locales)
				{
					WriteLocalePointer(offsetWriter, locale.Key, (int) stringWriter.Position);
					stringWriter.WriteUTF8(locale.Value);
				}

				// Round the size of the string data up
				var dataSize = (int) ((stringData.Position + _sizeAlign - 1) & ~(_sizeAlign - 1));
				stringData.SetLength(dataSize);

				// Update the two locale data hashes if we need to
				// (the hash arrays are set to null if the build doesn't need them)
				if (IndexTableHash != null)
				{
					if (_hashes)
						IndexTableHash = SHA1.Transform(offsetData.ToArray(), 0, (int)offsetData.Length);
					else
						IndexTableHash = new byte[20];
				}
				if (StringDataHash != null)
				{
					if (_hashes)
						StringDataHash = SHA1.Transform(stringData.ToArray(), 0, dataSize);
					else
						StringDataHash = new byte[20];
				}

				// Make sure there's free space for the offset table and then write it to the file
				LocaleIndexTable.Resize((uint) offsetData.Length, stream);
				stream.SeekTo(LocaleIndexTableLocation.AsOffset());
				stream.WriteBlock(offsetData.ToArray(), 0, (int) offsetData.Length);

				// Encrypt the string data if necessary
				byte[] strings = stringData.ToArray();
				if (_encryptionKey != null)
					strings = AES.Encrypt(strings, 0, dataSize, _encryptionKey.Key, _encryptionKey.IV);

				// Make sure there's free space for the string data and then write it to the file
				LocaleData.Resize((uint)dataSize, stream);
				stream.SeekTo(LocaleDataLocation.AsOffset());
				stream.WriteBlock(strings, 0, dataSize);

				// Update the string count and recalculate the language table offsets
				StringCount = locales.Count;
			}
		}

		private void ReadLocalePointer(IReader reader, out StringID id, out int offset)
		{
			StructureValueCollection values = StructureReader.ReadStructure(reader, _pointerLayout);
			id = new StringID(values.GetInteger("stringid"));
			offset = (int) values.GetInteger("offset");
		}

		private void WriteLocalePointer(IWriter writer, StringID id, int offset)
		{
			var values = new StructureValueCollection();
			values.SetInteger("stringid", id.Value);
			values.SetInteger("offset", (uint) offset);
			StructureWriter.WriteStructure(values, _pointerLayout, writer);
		}

		private byte[] ReadLocaleData(IReader reader)
		{
			// Read the string data
			reader.SeekTo(LocaleDataLocation.AsOffset());
			byte[] stringData = reader.ReadBlock((int)LocaleData.Size);

			// Decrypt it if necessary
			if (_encryptionKey != null)
				stringData = AES.Decrypt(stringData, _encryptionKey.Key, _encryptionKey.IV);

			return stringData;
		}
	}
}
