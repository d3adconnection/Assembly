using System.Windows;
using Assembly.Metro.Dialogs.ControlDialogs;

namespace Assembly.Metro.Dialogs
{
	public static class MetroBatchTagStatus
	{
		/// <summary>
		///     Show the Batch Tag Status dialog
		/// </summary>
		/// <returns>The dialog instance for updating progress</returns>
		public static BatchTagStatus Show()
		{
			App.AssemblyStorage.AssemblySettings.HomeWindow.ShowMask();
			var statusDialog = new BatchTagStatus
			{
				Owner = App.AssemblyStorage.AssemblySettings.HomeWindow,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};
			statusDialog.Show();
			return statusDialog;
		}

		/// <summary>
		///     Close the Batch Tag Status dialog
		/// </summary>
		/// <param name="dialog">The dialog instance to close</param>
		public static void Close(BatchTagStatus dialog)
		{
			if (dialog != null)
			{
				dialog.Close();
				App.AssemblyStorage.AssemblySettings.HomeWindow.HideMask();
			}
		}
	}
}
