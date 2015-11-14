using System.Windows.Forms;

namespace WpfExecutor.ViewModels {
	public class SmileFileHandler : IFileHandler {
		public void ValidateFileLocation(string location) {
			if (string.IsNullOrEmpty(location)) {
				MessageBox.Show(Properties.Resources.CompleteAllBoxesUIErrorMessage,
													 Properties.Resources.UIErrorTitle);
			}
		}
	}
}
