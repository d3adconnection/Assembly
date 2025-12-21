// All code credits go to palesius (original Assembly fork: https://github.com/palesius/Assembly)
// A gigantic thank you for making this possible in Assembly.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Media;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Assembly.Helpers;
using Assembly.Metro.Controls.PageTemplates.Games.Components;
using Assembly.Metro.Controls.PageTemplates.Games.Components.Editors;
using Assembly.Metro.Controls.PageTemplates.Games.Components.MetaData;
using Assembly.Metro.Dialogs;
using Assembly.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using Blamite.Blam;
using Blamite.Blam.Localization;
using Blamite.Blam.Resources;
using Blamite.Blam.Scripting;
using Blamite.Serialization;
using Blamite.Injection;
using Blamite.IO;
using Blamite.Plugins;
using Blamite.RTE;
//using Blamite.RTE.H2Vista;
using Blamite.Util;
using CloseableTabItemDemo;
using Microsoft.Win32;
using Newtonsoft.Json;
//using XBDMCommunicator;
using Blamite.Blam.ThirdGen;
//using Blamite.RTE.MCC;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Resources;
using System.Windows.Threading;
using Assembly.Helpers.Native;
using Assembly.Helpers.Net;
using Assembly.Metro.Controls.PageTemplates;
using Assembly.Helpers.Plugins;
using Assembly.Metro.Controls.PageTemplates.Games;
using Assembly.Metro.Controls.PageTemplates.Tools;
using Assembly.Metro.Controls.PageTemplates.Tools.Halo4;
using XboxChaos.Models;
using Xceed.Wpf.AvalonDock.Controls;
using Assembly.Helpers.Net.Sockets;
using Blamite.Blam.ThirdGen.Structures;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Media;

namespace Assembly.Windows
{
    public static class ExtensionMethods
    {
        private static readonly Action EmptyDelegate = delegate { };
        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }

    public partial class BatchTagGroup
    {
        public String name;
        public Dictionary<String, BatchTagEntry> entries = new Dictionary<String, BatchTagEntry>();

        public BatchTagGroup(String _name)
        {
            name = _name;
        }
    }

    public partial class BatchTagEntry
    {
        public String name;
        public Dictionary<uint, BatchTagField> fields = new Dictionary<uint, BatchTagField>();

        public BatchTagEntry(String _name)
        {
            name = _name;
        }

        public void addField(String def, bool mapLevel)
        {
            BatchTagField f = new BatchTagField(def);
            if (mapLevel && fields.ContainsKey(f.line)) fields.Remove(f.line);
            fields.Add(f.line, f);
        }

        public void cloneFields(BatchTagEntry src)
        {
            foreach (BatchTagField tf in src.fields.Values)
            {
                fields.Add(tf.line, tf);
            }
        }
    }

    public class BatchTagField
    {
        public int hits = 0;
        public tfType fldType = tfType.unknown;
        public uint line;
        public string name;
        public float valFloat;
        public float valFloatMin;
        public float valFloatMax;
        public float valFloatx;
        public float valFloaty;
        public float valFloatz;
        public float valFloatw;
        public int valInt;
        public short valInt16;
        public short valInt16Min;
        public short valInt16Max;
        public byte valUInt8;
        public UInt16 valUInt16;
        public UInt32 valUInt;
        public UInt64 valUInt64;
        public Int32 valInt32;
        public Int64 valInt64;
        public bool valFlagType;
        public string valString;
        public string valTagGroup;
        public string valTagPath;
        
        public enum tfType : int
        {
            unknown = -1,
            float32 = 0,
            flags32 = 1,
            int8 = 2,
            int16 = 3,
            enum8 = 4,
            rangeFloat32 = 5,
            ranged = 6,
            stringid = 7,
            degree = 8,
            vector3 = 9,
            uint32 = 10,
            uint8 = 11,
            int32 = 12,
            enum16 = 13,
            colorf = 14,
            vector2 = 15,
            vector4 = 16,
            point2 = 17,
            point3 = 18,
            degree2 = 19,
            degree3 = 20,
            uint16 = 21,
            int64 = 22,
            uint64 = 23,
            rangeint16 = 24,
            tagref = 25
        }

        public static BatchTagField.tfType nameToEnum(String name)
        {
            switch (name)
            {
                case "float32": return tfType.float32;
                case "flags32": return tfType.flags32;
                case "int8": return tfType.int8;
                case "int16": return tfType.int16;
                case "int32": return tfType.int32;
                case "enum8": return tfType.enum8;
                case "enum16": return tfType.enum16;
                case "rangef": return tfType.rangeFloat32;
                case "ranged": return tfType.ranged;
                case "stringid": return tfType.stringid;
                case "degree": return tfType.degree;
                case "vector3": return tfType.vector3;
                case "uint8": return tfType.uint8;
                case "uint32": return tfType.uint32;
                case "colorf": return tfType.colorf;
                case "vector2": return tfType.vector2;
                case "vector4": return tfType.vector4;
                case "point2": return tfType.point2;
                case "point3": return tfType.point3;
                case "degree2": return tfType.degree2;
                case "degree3": return tfType.degree3;
                case "uint16": return tfType.uint16;
                case "int64": return tfType.int64;
                case "uint64": return tfType.uint64;
                case "rangeint16": return tfType.rangeint16;
                case "tagref": return tfType.tagref;
            }
            return tfType.unknown;
        }

        public static String enumToType(BatchTagField.tfType fldType)
        {
            switch (fldType)
            {
                case tfType.float32: return "Float32Data";
                case tfType.flags32: return "FlagData";
                case tfType.int8: return "Int8Data";
                case tfType.int16: return "Int16Data";
                case tfType.int32: return "Int32Data";
                case tfType.enum8: return "EnumData";
                case tfType.enum16: return "EnumData";
                case tfType.rangeFloat32: return "RangeFloat32Data";
                case tfType.ranged: return "RangeDegreeData";
                case tfType.stringid: return "StringIDData";
                case tfType.degree: return "DegreeData";
                case tfType.vector3: return "Vector3Data";
                case tfType.uint8: return "Uint8Data";
                case tfType.uint32: return "Uint32Data";
                case tfType.colorf: return "ColorFData";
                case tfType.vector2: return "Vector2Data";
                case tfType.vector4: return "Vector4Data";
                case tfType.point2: return "Point2Data";
                case tfType.point3: return "Point3Data";
                case tfType.degree2: return "Degree2Data";
                case tfType.degree3: return "Degree3Data";
                case tfType.uint16: return "Uint16Data";
                case tfType.int64: return "Int64Data";
                case tfType.uint64: return "Uint64Data";
                case tfType.rangeint16: return "RangeInt16Data";
                case tfType.tagref: return "TagRefData";
            }
            return null;
        }

        public BatchTagField(String src)
        {
            String[] toks = src.Split(':');
            fldType = BatchTagField.nameToEnum(toks[0]);
            line = uint.Parse(toks[1]);
            name = toks[2];
            switch (fldType)
            {
                case tfType.float32:
                    valFloat = float.Parse(toks[3]);
                    break;
                case tfType.rangeFloat32:
                    valFloatMin = float.Parse(toks[3]);
                    valFloatMax = float.Parse(toks[4]);
                    break;
                case tfType.ranged:
                    valFloatMin = float.Parse(toks[3]);
                    valFloatMax = float.Parse(toks[4]);
                    break;
                case tfType.int8:
                    valInt = int.Parse(toks[3]);
                    break;
                case tfType.int16:
                    valInt = int.Parse(toks[3]);
                    break;
                case tfType.flags32:
                    valInt = int.Parse(toks[3].Substring(1));
                    switch (toks[3].Substring(0, 1))
                    {
                        case "+": valFlagType = true; break;
                        case "-": valFlagType = false; break;
                        default: valFlagType = true; break;
                    }
                    break;
                case tfType.enum8:
                    valInt = int.Parse(toks[3]);
                    break;
                case tfType.stringid:
                    valString = toks[3];
                    break;
                case tfType.degree:
                    valFloat = float.Parse(toks[3]);
                    break;
                case tfType.vector3:
                    valFloatx = float.Parse(toks[3]);
                    valFloaty = float.Parse(toks[4]);
                    valFloatz = float.Parse(toks[5]);
                    break;
                case tfType.uint8:
                    valUInt8 = byte.Parse(toks[3]);
                    break;
                case tfType.uint32:
                    valUInt = UInt32.Parse(toks[3]);
                    break;
                case tfType.int32:
                    valInt32 = Int32.Parse(toks[3]);
                    break;
                case tfType.enum16:
                    valInt = int.Parse(toks[3]);
                    break;
                case tfType.colorf:
                    valString = toks[3];
                    break;
                case tfType.vector2:
                    valFloatx = float.Parse(toks[3]);
                    valFloaty = float.Parse(toks[4]);
                    break;
                case tfType.vector4:
                    valFloatx = float.Parse(toks[3]);
                    valFloaty = float.Parse(toks[4]);
                    valFloatz = float.Parse(toks[5]);
                    valFloatw = float.Parse(toks[6]);
                    break;
                case tfType.point2:
                    valFloatx = float.Parse(toks[3]);
                    valFloaty = float.Parse(toks[4]);
                    break;
                case tfType.point3:
                    valFloatx = float.Parse(toks[3]);
                    valFloaty = float.Parse(toks[4]);
                    valFloatz = float.Parse(toks[5]);
                    break;
                case tfType.degree2:
                    valFloatx = float.Parse(toks[3]);
                    valFloaty = float.Parse(toks[4]);
                    break;
                case tfType.degree3:
                    valFloatx = float.Parse(toks[3]);
                    valFloaty = float.Parse(toks[4]);
                    valFloatz = float.Parse(toks[5]);
                    break;
                case tfType.uint16:
                    valUInt16 = UInt16.Parse(toks[3]);
                    break;
                case tfType.int64:
                    valInt64 = Int64.Parse(toks[3]);
                    break;
                case tfType.uint64:
                    valUInt64 = UInt64.Parse(toks[3]);
                    break;
                case tfType.rangeint16:
                    valInt16Min = short.Parse(toks[3]);
                    valInt16Max = short.Parse(toks[4]);
                    break;
                case tfType.tagref:
                    valTagGroup = toks[3];
                    valTagPath = toks[4];
                    break;
                default:
                    break;
            }
        }

        public BatchTagField(BatchTagField tf)
        {
            fldType = tf.fldType;
            line = tf.line;
            name = tf.name;
            valFloat = tf.valFloat;
            valInt = tf.valInt;
            valFlagType = tf.valFlagType;
        }

        public String getFldTypeName()
        {
            return BatchTagField.enumToType(fldType);
        }
    }
    public partial class Home
    {
        public String genPath;
        public String genFile;

        List<String> lstIssues = new List<String>();

        private Dictionary<String, BatchTagGroup> loadBatchTagSettings(HaloMap map)
        {
            Dictionary<String, BatchTagGroup> tgDict = new Dictionary<string, BatchTagGroup>();

            String[] lines = System.IO.File.ReadAllLines(genFile);
            for (int i = 0; i < lines.Count(); i++)
            {
                String[] toks = lines[i].Split('|');
                BatchTagGroup tg = null;
                if (!tgDict.TryGetValue(toks[0], out tg))
                {
                    tg = new BatchTagGroup(toks[0]);
                    tgDict.Add(toks[0], tg);
                }
                BatchTagEntry te = null;
                if (!tg.entries.TryGetValue(toks[1], out te))
                {
                    te = new BatchTagEntry(toks[1]);
                    tg.entries.Add(toks[1], te);
                }
                if (toks[2].Substring(0, 1) == "*")
                {
                    BatchTagEntry src = null;
                    if (tg.entries.TryGetValue(toks[2], out src)) { te.cloneFields(tg.entries[(toks[2])]); }
                }
                else
                {
                    te.addField(toks[2], false);
                }
            }
            return tgDict;
        }

        private void menuBatchTag_File_Click(object sender, RoutedEventArgs e)
        {
            var OpenTemplateDialog = new OpenFileDialog
            {
                Title = "Select template file",
                Multiselect = false,
                Filter = "All files (*.*)|*.*"
            };

            if (!(bool)OpenTemplateDialog.ShowDialog(this)) return;
            
            genPath = string.Format(OpenTemplateDialog.FileName);
            var statusDialog = MetroBatchTagStatus.Show();
            StatusUpdater.Update("Applying batch tag template...");
            processTask(genPath, false, statusDialog);
        }

        private void menuBatchTag_Folder_Click(object sender, RoutedEventArgs e)
        {
            var OpenTemplateDialog = new CommonOpenFileDialog
            {
                Title = "Select folder with batch template files",
                Multiselect = false,
                IsFolderPicker = true,
                EnsurePathExists = true
            };

            if (!(OpenTemplateDialog.ShowDialog() == CommonFileDialogResult.Ok)) return;
            
            genPath = string.Format(OpenTemplateDialog.FileName);
            var statusDialog = MetroBatchTagStatus.Show();
            StatusUpdater.Update("Applying batch tag template...");
            processTask(genPath, true, statusDialog);
        }

        private async void processTask(string genPath, bool IsFolder, Metro.Dialogs.ControlDialogs.BatchTagStatus statusDialog)
        {
            List<String> lstIssues = new List<String>();

            int mapCnt = 0;
            int mapIdx = 0;

            foreach (LayoutDocument ld in documentManager.Children)
            {
                if (ld.Content.GetType().Name == "HaloMap") { mapCnt++; }
            }
            
            foreach (LayoutDocument ld in documentManager.Children)
            {
                if (ld.Content.GetType().Name == "HaloMap")
                {
                    HaloMap map = (HaloMap)ld.Content;
                    mapIdx++;
                    String mapPath = map.GetCacheLocation();

                    string[] files = new string[] { };
                    if (IsFolder) { files = Directory.GetFiles(genPath, "*.txt"); }
                    else { files = new string[] { genPath }; }

                    foreach (string file in files)
                    {
                        genFile = file;
                        if (System.IO.File.Exists(genFile))
                        {
                            Dictionary<String, BatchTagGroup> wsDict = loadBatchTagSettings(map);
                            
                            // Count total tags to process for this map
                            int totalTagsForMap = 0;
                            foreach (TagGroup tg in map.tvTagList.Items)
                            {
                                BatchTagGroup wstg = null;
                                if (wsDict.TryGetValue(tg.TagGroupMagic, out wstg))
                                {
                                    foreach (TagEntry te in tg.Children)
                                    {
                                        BatchTagEntry wste = null;
                                        if (wstg.entries.TryGetValue(te.TagFileName, out wste))
                                        {
                                            totalTagsForMap++;
                                        }
                                        else
                                        {
                                            // Check for wildcard matches
                                            int prefPos = 0;
                                            while (prefPos < te.TagFileName.Length && prefPos >= 0)
                                            {
                                                BatchTagEntry tryWste = null;
                                                if (wstg.entries.TryGetValue(te.TagFileName.Substring(0, prefPos) + "*", out tryWste))
                                                {
                                                    totalTagsForMap++;
                                                    break;
                                                }
                                                prefPos = te.TagFileName.IndexOf('\\', prefPos);
                                                if (prefPos > 0) prefPos++;
                                            }
                                        }
                                    }
                                }
                            }
                            
                            int processedTags = 0;
                            
                            statusDialog.UpdateTagStatus(0, totalTagsForMap, 
                                string.Format("Processing map {0}/{1}: {2}", mapIdx, mapCnt, map.GetCacheFile().InternalName));
                            
                            foreach (TagGroup tg in map.tvTagList.Items)
                            {
                                BatchTagGroup wstg = null;
                                if (wsDict.TryGetValue(tg.TagGroupMagic, out wstg))
                                {
                                    int tagCnt = tg.Children.Count;
                                    int tagIdx = 0;
                                    
                                    foreach (TagEntry te in tg.Children)
                                    {
                                        tagIdx++;
                                        BatchTagEntry wste = null;
                                        if (!wstg.entries.TryGetValue(te.TagFileName, out wste))
                                        {
                                            int prefPos = 0;
                                            while (prefPos < te.TagFileName.Length && prefPos >= 0)
                                            {
                                                BatchTagEntry tryWste = null;
                                                wstg.entries.TryGetValue(te.TagFileName.Substring(0, prefPos) + "*", out tryWste);
                                                if (tryWste != null)
                                                {
                                                    Debug.Print(te.TagFileName + "<=>" + te.TagFileName.Substring(0, prefPos) + "*");
                                                    wste = tryWste;
                                                }
                                                prefPos = te.TagFileName.IndexOf('\\', prefPos);
                                                if (prefPos > 0) prefPos++;
                                                await Dispatcher.Yield();
                                            }
                                        }
                                        if (wste != null)
                                        {
                                            processedTags++;
                                            String tagMsg = string.Format("({0}/{1}) {2}\n{3}.{4}", 
                                                mapIdx, mapCnt, map.GetCacheFile().InternalName, te.TagFileName, te.GroupName);
                                            statusDialog.UpdateTagStatus(processedTags, totalTagsForMap, tagMsg);
                                            processTagEntry(te, wste, map, lstIssues);
                                        }
                                        await Dispatcher.Yield();
                                    }

                                }
                            }
                            foreach (BatchTagGroup wstg in wsDict.Values)
                            {
                                foreach (BatchTagEntry wste in wstg.entries.Values)
                                {
                                    foreach (BatchTagField wstf in wste.fields.Values)
                                    {
                                        if (wstf.hits == 0)
                                        {
                                            lstIssues.Add(string.Format("No hits for [{0}]{1}: ({2}) \"{3}\"", wstg.name, wste.name, wstf.line, wstf.name));
                                        }
                                        await Dispatcher.Yield();
                                    }
                                    await Dispatcher.Yield();
                                }
                                await Dispatcher.Yield();
                            }
                            await Dispatcher.Yield();
                            wsDict.Clear();
                        }
                        await Dispatcher.Yield();
                    }
                }
                await Dispatcher.Yield();
            }
            await Dispatcher.Yield();
            MetroBatchTagStatus.Close(statusDialog);

            if (lstIssues.Count > 0)
            {
                System.Text.StringBuilder sbIssues = new System.Text.StringBuilder();
                foreach (String curIssue in lstIssues) { sbIssues.AppendLine(curIssue); }
                Clipboard.SetText(sbIssues.ToString());
                System.Media.SystemSounds.Beep.Play();
                StatusUpdater.Update("Some tags could not be updated. Results have been copied to clipboard.");
            }
            else
            {
                System.Media.SystemSounds.Beep.Play();
                StatusUpdater.Update("All tags have been updated successfully.");
            }
            lstIssues.Clear();
        }
        private void processTagEntry(TagEntry te, BatchTagEntry wste, HaloMap map, List<string> lstIssues)
        {
            map.CreateTag(te);
            CloseableTabItem cti = (CloseableTabItem)map.contentTabs.SelectedItem;
            MetaContainer mc = (MetaContainer)cti.Content;
            MetaEditor me = (MetaEditor)mc.tabMetaEditor.Content;
            AssemblyPluginVisitor tgp = me.GetPluginVisitor();
            bool dirty = false;
            foreach (MetaField mfouter in tgp.Values)
            {
                MetaField mf = mfouter;
                while (mf.GetType().Name == "WrappedTagBlockEntry")
                {
                    mf = ((WrappedTagBlockEntry)mf).WrappedField;
                }
                BatchTagField wstf = null;
                if (wste.fields.TryGetValue(mf.PluginLine, out wstf))
                {
                    if (wstf.getFldTypeName() == mf.GetType().Name)
                    {
                        switch (wstf.fldType)
                        {
                            case BatchTagField.tfType.float32:
                                Float32Data dFloat32 = (Float32Data)mf;
                                if (dFloat32.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dFloat32.Value != wstf.valFloat)
                                    {
                                        dirty = true;
                                        dFloat32.Value = wstf.valFloat;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.rangeFloat32:
                                RangeFloat32Data dRangeFloat32 = (RangeFloat32Data)mf;
                                if (dRangeFloat32.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dRangeFloat32.Min != wstf.valFloatMin || dRangeFloat32.Max != wstf.valFloatMax)
                                    {
                                        dirty = true;
                                        dRangeFloat32.Min = wstf.valFloatMin;
                                        dRangeFloat32.Max = wstf.valFloatMax;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.ranged:
                                RangeDegreeData dRangeDegree = (RangeDegreeData)mf;
                                if (dRangeDegree.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dRangeDegree.Min != wstf.valFloatMin || dRangeDegree.Max != wstf.valFloatMax)
                                    {
                                        dirty = true;
                                        dRangeDegree.Min = wstf.valFloatMin;
                                        dRangeDegree.Max = wstf.valFloatMax;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.flags32:
                                FlagData dFlag = (FlagData)mf;
                                if (dFlag.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dFlag.Bits.ElementAt(wstf.valInt).IsSet != wstf.valFlagType)
                                    {
                                        dirty = true;
                                        dFlag.Bits.ElementAt(wstf.valInt).IsSet = wstf.valFlagType;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.int8:
                                Int8Data dInt8 = (Int8Data)mf;
                                if (dInt8.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dInt8.Value != wstf.valInt)
                                    {
                                        dirty = true;
                                        dInt8.Value = (sbyte)wstf.valInt;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.int16:
                                Int16Data dInt16 = (Int16Data)mf;
                                if (dInt16.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dInt16.Value != wstf.valInt)
                                    {
                                        dirty = true;
                                        dInt16.Value = (short)wstf.valInt;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.enum8:
                                EnumData dEnum = (EnumData)mf;
                                if (dEnum.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dEnum.Value != wstf.valInt)
                                    {
                                        dirty = true;
                                        dEnum.Value = wstf.valInt;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.stringid:
                                StringIDData dStringID = (StringIDData)mf;
                                if (dStringID.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dStringID.Value != wstf.valString)
                                    {
                                        dirty = true;
                                        dStringID.Value = wstf.valString;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.degree:
                                DegreeData dDegree = (DegreeData)mf;
                                if (dDegree.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dDegree.Value != wstf.valFloat)
                                    {
                                        dirty = true;
                                        dDegree.Value = wstf.valFloat;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.vector3:
                                Vector3Data dVector3 = (Vector3Data)mf;
                                if (dVector3.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dVector3.A != wstf.valFloatx || dVector3.B != wstf.valFloaty || dVector3.C != wstf.valFloatz)
                                    {
                                        dirty = true;
                                        dVector3.A = wstf.valFloatx;
                                        dVector3.B = wstf.valFloaty;
                                        dVector3.C = wstf.valFloatz;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.uint8:
                                Uint8Data dUInt8 = (Uint8Data)mf;
                                if (dUInt8.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dUInt8.Value != wstf.valUInt8)
                                    {
                                        dirty = true;
                                        dUInt8.Value = wstf.valUInt8;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.uint32:
                                Uint32Data dUInt32 = (Uint32Data)mf;
                                if (dUInt32.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dUInt32.Value != wstf.valUInt)
                                    {
                                        dirty = true;
                                        dUInt32.Value = wstf.valUInt;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.int32:
                                Int32Data dInt32 = (Int32Data)mf;
                                if (dInt32.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dInt32.Value != wstf.valInt32)
                                    {
                                        dirty = true;
                                        dInt32.Value = wstf.valInt32;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.enum16:
                                EnumData dEnum16 = (EnumData)mf;
                                if (dEnum16.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dEnum16.Value != wstf.valInt)
                                    {
                                        dirty = true;
                                        dEnum16.Value = wstf.valInt;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.colorf:
                                ColorData dcolorf = (ColorData)mf;
                                if (dcolorf.Name == wstf.name)
                                {
                                    if (wstf.valString.StartsWith("#")) { wstf.valString = wstf.valString.Substring(1); }
                                    if (wstf.valString.Length != 6) throw new Exception("Color not valid");
                                    Color colorf = Color.FromArgb(255, byte.Parse(wstf.valString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(wstf.valString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber), byte.Parse(wstf.valString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber));
                                    if (dcolorf.Value != colorf)
                                    {
                                        wstf.hits += 1;
                                        dirty = true;
                                        dcolorf.Value = colorf;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.vector2:
                                Vector2Data dVector2 = (Vector2Data)mf;
                                if (dVector2.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dVector2.A != wstf.valFloatx || dVector2.B != wstf.valFloaty)
                                    {
                                        dirty = true;
                                        dVector2.A = wstf.valFloatx;
                                        dVector2.B = wstf.valFloaty;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.vector4:
                                Vector4Data dVector4 = (Vector4Data)mf;
                                if (dVector4.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dVector4.A != wstf.valFloatx || dVector4.B != wstf.valFloaty || dVector4.C != wstf.valFloatz || dVector4.D != wstf.valFloatw)
                                    {
                                        dirty = true;
                                        dVector4.A = wstf.valFloatx;
                                        dVector4.B = wstf.valFloaty;
                                        dVector4.C = wstf.valFloatz;
                                        dVector4.D = wstf.valFloatw;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.point2:
                                Point2Data dPoint2 = (Point2Data)mf;
                                if (dPoint2.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dPoint2.A != wstf.valFloatx || dPoint2.B != wstf.valFloaty)
                                    {
                                        dirty = true;
                                        dPoint2.A = wstf.valFloatx;
                                        dPoint2.B = wstf.valFloaty;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.point3:
                                Point3Data dPoint3 = (Point3Data)mf;
                                if (dPoint3.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dPoint3.A != wstf.valFloatx || dPoint3.B != wstf.valFloaty || dPoint3.C != wstf.valFloatz)
                                    {
                                        dirty = true;
                                        dPoint3.A = wstf.valFloatx;
                                        dPoint3.B = wstf.valFloaty;
                                        dPoint3.C = wstf.valFloatz;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.degree2:
                                Degree2Data dDegree2 = (Degree2Data)mf;
                                if (dDegree2.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dDegree2.A != wstf.valFloatx || dDegree2.B != wstf.valFloaty)
                                    {
                                        dirty = true;
                                        dDegree2.A = wstf.valFloatx;
                                        dDegree2.B = wstf.valFloaty;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.degree3:
                                Degree3Data dDegree3 = (Degree3Data)mf;
                                if (dDegree3.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dDegree3.A != wstf.valFloatx || dDegree3.B != wstf.valFloaty || dDegree3.C != wstf.valFloatz)
                                    {
                                        dirty = true;
                                        dDegree3.A = wstf.valFloatx;
                                        dDegree3.B = wstf.valFloaty;
                                        dDegree3.C = wstf.valFloatz;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.uint16:
                                Uint16Data dUInt16 = (Uint16Data)mf;
                                if (dUInt16.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dUInt16.Value != wstf.valUInt16)
                                    {
                                        dirty = true;
                                        dUInt16.Value = wstf.valUInt16;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.int64:
                                Int64Data dInt64 = (Int64Data)mf;
                                if (dInt64.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dInt64.Value != wstf.valInt64)
                                    {
                                        dirty = true;
                                        dInt64.Value = wstf.valInt64;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.uint64:
                                Uint64Data dUInt64 = (Uint64Data)mf;
                                if (dUInt64.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dUInt64.Value != wstf.valUInt64)
                                    {
                                        dirty = true;
                                        dUInt64.Value = wstf.valUInt64;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.rangeint16:
                                RangeInt16Data dRangeInt16 = (RangeInt16Data)mf;
                                if (dRangeInt16.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    if (dRangeInt16.Min != wstf.valInt16Min || dRangeInt16.Max != wstf.valInt16Max)
                                    {
                                        dirty = true;
                                        dRangeInt16.Min = wstf.valInt16Min;
                                        dRangeInt16.Max = wstf.valInt16Max;
                                    }
                                }
                                break;
                            case BatchTagField.tfType.tagref:
                                TagRefData dTagRef = (TagRefData)mf;
                                if (dTagRef.Name == wstf.name)
                                {
                                    wstf.hits += 1;
                                    TagEntry foundTag = null;
                                    
                                    // Try to find the tag by group and path
                                    if (!string.IsNullOrEmpty(wstf.valTagGroup) && !string.IsNullOrEmpty(wstf.valTagPath))
                                    {
                                        // Search for matching tag in hierarchy
                                        foreach (TagGroup tagGroup in dTagRef.Tags.Groups)
                                        {
                                            if (tagGroup.TagGroupMagic == wstf.valTagGroup)
                                            {
                                                foreach (TagEntry tagEntry in tagGroup.Children)
                                                {
                                                    if (tagEntry.TagFileName == wstf.valTagPath)
                                                    {
                                                        foundTag = tagEntry;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (foundTag != null) break;
                                        }
                                    }
                                    
                                    // Check if value needs updating
                                    bool needsUpdate = false;
                                    if (foundTag != null)
                                    {
                                        if (dTagRef.Value == null || dTagRef.Value.TagFileName != foundTag.TagFileName)
                                        {
                                            needsUpdate = true;
                                        }
                                    }
                                    else if (wstf.valTagPath == "null" && dTagRef.Value != null)
                                    {
                                        needsUpdate = true;
                                        foundTag = null;
                                    }
                                    
                                    if (needsUpdate)
                                    {
                                        dirty = true;
                                        dTagRef.Value = foundTag;
                                        if (foundTag != null && dTagRef.Tags.Groups != null)
                                        {
                                            dTagRef.Group = dTagRef.Tags.Groups.FirstOrDefault(g => g.TagGroupMagic == wstf.valTagGroup);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            if (dirty)
            {
                Debug.Print("Updated [" + te.GroupName + "]:" + te.TagFileName + ".");
                me.PublicSave();
            }
            RaiseEvent(new RoutedEventArgs(CloseableTabItem.CloseTabEvent, cti));
            map.contentTabs.Items.Remove(cti);
        }
    }
}

namespace Assembly.Metro.Controls.PageTemplates.Games
{
    public partial class HaloMap : INotifyPropertyChanged
    {
        public string GetCacheLocation()
        {
            return _cacheLocation;
        }
        public ICacheFile GetCacheFile()
        {
            return _cacheFile;
        }
        public EngineDescription GetBuildInfo()
        {
            return _buildInfo;
        }

        public void saveCacheFile()
        {
            using (IStream stream = _mapManager.OpenReadWrite())
                _cacheFile.SaveChanges(stream);
        }

        public void WSForceLoad(TagEntry tag)
        {
            var container = extractTags(new List<TagEntry>() { tag }, ExtractMode.Forceload, true, false);

            if (container == null)
                return;

            // Now take the info we just extracted and use it to forceload
            bool dirty = false;
            using (IStream stream = _mapManager.OpenReadWrite())
            {
                var _zonesets = _cacheFile.Resources.LoadZoneSets(stream);

                foreach (ExtractedTag et in container.Tags)
                {
                    if (!_zonesets.GlobalZoneSet.IsTagActive(et.OriginalIndex))
                    {
                        _zonesets.GlobalZoneSet.ActivateTag(et.OriginalIndex, true);
                        dirty = true;
                    }
                }

                foreach (ExtractedResourceInfo eri in container.Resources)
                {
                    if (!_zonesets.GlobalZoneSet.IsResourceActive(eri.OriginalIndex))
                    {
                        _zonesets.GlobalZoneSet.ActivateResource(eri.OriginalIndex, true);
                        dirty = true;
                    }
                }

                if (dirty)
                {
                    _zonesets.SaveChanges(stream);
                    _cacheFile.SaveChanges(stream);
                }
            }

            if (dirty)
            {
                LoadTags();
                System.Diagnostics.Debug.Print(String.Format("Forceloaded {0}", tag.TagFileName));
            }
            else
            {
                System.Diagnostics.Debug.Print(String.Format("Already forceloaded {0}", tag.TagFileName));
            }
        }
    }
}

namespace Assembly.Metro.Controls.PageTemplates.Games.Components
{
    public partial class MetaEditor : UserControl
    {
        public AssemblyPluginVisitor GetPluginVisitor()
        {
            return _pluginVisitor;
        }

        public void PublicSave()
        {
            UpdateMeta(MetaWriter.SaveType.File, false, false);
        }
    }
}
