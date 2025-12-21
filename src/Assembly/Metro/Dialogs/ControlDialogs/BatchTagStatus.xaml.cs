using System.Windows;
using System.Windows.Threading;
using Assembly.Helpers.Native;

namespace Assembly.Metro.Dialogs.ControlDialogs
{
	/// <summary>
	///     Interaction logic for BatchTagStatus.xaml
	/// </summary>
	public partial class BatchTagStatus : Window
	{
		public BatchTagStatus()
		{
			InitializeComponent();
			DwmDropShadow.DropShadowToWindow(this);
		}

		/// <summary>
		/// Updates the tag processing status
		/// </summary>
		/// <param name="tagIdx">Current tag index (1-based)</param>
		/// <param name="tagCnt">Total number of tags</param>
		/// <param name="tagMsg">Status message to display</param>
		public void UpdateTagStatus(int tagIdx, int tagCnt, string tagMsg)
		{
			Dispatcher.Invoke(() =>
			{
				if (tagCnt > 0)
				{
					pbTagProgress.Maximum = tagCnt;
					pbTagProgress.Value = tagIdx;
				}
				
				if (!string.IsNullOrEmpty(tagMsg))
				{
					txtTagStatus.Text = tagMsg;
				}
			}, DispatcherPriority.Render);
		}
	}
}
