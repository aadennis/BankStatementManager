using Wells.StatementManagement;
using WpfExecutor.ViewModels;

namespace WpfExecutor.Views {

	public partial class MainWindow {
		private readonly DialogVm _dialogVm = new DialogVm(new SmileFileHandler(), new SmileHtmlStatementParser());

		public MainWindow() {
			InitializeComponent();
			DataContext = _dialogVm;
		}
	}
}
