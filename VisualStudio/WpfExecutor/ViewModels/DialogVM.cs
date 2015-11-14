using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Wells.StatementManagement;
using MessageBox = System.Windows.Forms.MessageBox;

namespace WpfExecutor.ViewModels {
	public sealed class DialogVm : BaseViewModel {
		private readonly Microsoft.Win32.OpenFileDialog _fileDialog;
		private FolderBrowserDialog _folderDialog;
		private string _statementLocation;
		private string _convertedOfxDestinationFolder;
		private string _confirmedDestinationFileLocation;
		private string _statementDate;
		private string _sortCode;
		private string _accountCode;

		private bool _isSaveButtonVisible;
		private SmileStatementManager _smileStatementManager;
		private readonly IFileHandler _fileHandler;
		private readonly IHtmlParser _parser; // = new SmileHtmlStatementParser() as IHtmlParser;
		private ObservableCollection<OfxTransaction> _ofxTransactions;
		public ICommand ShowSourceLocationCommand { get; set; }
		public ICommand ShowDestinationLocationCommand { get; set; }
		public ICommand RunConversionCommand { get; set; }
		public ICommand RunConversionIsVisibleCommand { get; set; }
		public ICommand SaveOfxStatementCommand { get; set; }

		public DialogVm(IFileHandler fileHandler, IHtmlParser parser) : this()  {
			_fileHandler = fileHandler;
			_parser = parser;
		}

		public DialogVm() {
			IsSaveButtonVisible = false;
			IsStatementLocationEmpty = true;
			IsConvertedOfxDestinationFolderEmpty = true;
			_fileDialog = new Microsoft.Win32.OpenFileDialog { DefaultExt = ".htm", Filter = "html documents (.htm)|*.htm; *.html" };
			ShowSourceLocationCommand = new DelegateCommand(GetFullPathOfStatementFileName);
			ShowDestinationLocationCommand = new DelegateCommand(GetFolderForOfxLocation);
			RunConversionCommand = new DelegateCommand(ConvertSmileToOfxStatementForDisplay);
			SaveOfxStatementCommand = new DelegateCommand(SaveOfxStatementToDisk);
		}

		public ObservableCollection<OfxTransaction> GetTransactions {
			get {
				return IsDesignTime ? GetTestOfxTransactions : _ofxTransactions;
			}
			set {
				if (_ofxTransactions == value) return;
				_ofxTransactions = value;
				OnPropertyChanged(() => GetTransactions);
			}
		}

		public static ObservableCollection<OfxTransaction> GetTestOfxTransactions {
			get {
				return new ObservableCollection<OfxTransaction> {
			            new OfxTransaction {
			                                Amount = "934.29",
			                                DatePosted = "20130307",
			                                FitId = "123-11",
			                                Memo = "The first memo",
			                                Name = "The first name",
			                                RunningBalance = "122.33",
			                                TrnType = TrnTypeEnum.Credit
			                                },
			            new OfxTransaction {
			                                Amount = "122.12",
			                                DatePosted = "20130308",
			                                FitId = "123-12",
			                                Memo = "The next memo",
			                                Name = "The next name",
			                                RunningBalance = "423.87",
			                                TrnType = TrnTypeEnum.Debit
			                                },
			            new OfxTransaction {
			                                Amount = "1000001.56",
			                                DatePosted = "20130309",
			                                FitId = "123-13",
			                                Memo = "The last memo",
			                                Name = "The last name",
			                                RunningBalance = "2842.72",
			                                TrnType = TrnTypeEnum.Credit
			                                },
			        };
			}
		}

		public OfxStatement OfxStatementSourcedFromSmile { get; private set; }

		public bool IsStatementLocationEmpty { get; private set; }

		public bool IsStatementLocationPopulated {
			get {
				if (IsDesignTime) return true;
				return !IsStatementLocationEmpty;
			}
		}

		public string StatementLocation {
			get { return _statementLocation; }
			set {
				if (_statementLocation == value) return;
				_statementLocation = value;
				IsStatementLocationEmpty = string.IsNullOrEmpty(_statementLocation);
				SourceAndTargetFolderPopulated();
				OnPropertyChanged(() => StatementLocation);
				OnPropertyChanged(() => IsSaveButtonVisible);
				OnPropertyChanged(() => IsStatementLocationPopulated);
				OnPropertyChanged(() => GetTransactions);
			}
		}

		public string ConvertedOfxDestinationFolder {
			get { return _convertedOfxDestinationFolder; }
			set {
				if (_convertedOfxDestinationFolder == value) return;
				_convertedOfxDestinationFolder = value;
				IsConvertedOfxDestinationFolderEmpty = string.IsNullOrEmpty(_convertedOfxDestinationFolder);
				SourceAndTargetFolderPopulated();
				OnPropertyChanged(() => ConvertedOfxDestinationFolder);
				OnPropertyChanged(() => IsSaveButtonVisible);
			}
		}

		public string ConfirmedDestinationFileLocation {
			get { return _confirmedDestinationFileLocation; }
			set {
				if (_confirmedDestinationFileLocation == value) return;
				_confirmedDestinationFileLocation = value;
				OnPropertyChanged(() => ConfirmedDestinationFileLocation);
			}
		}

		public bool IsSaveButtonVisible {
			get { return _isSaveButtonVisible; }
			set {
				if (_isSaveButtonVisible == value) return;
				_isSaveButtonVisible = value;
				OnPropertyChanged(() => IsSaveButtonVisible);
			}
		}

		public string StatementDate {
			get { return _statementDate; }
			set {
				if (_statementDate == value) return;
				_statementDate = value;
			}
		}

		public string SortCode {
			get { return _sortCode; }
			set {
				if (_sortCode == value) return;
				_sortCode = value;
			}
		}

		public string AccountCode {
			get { return _accountCode; }
			set {
				if (_accountCode == value) return;
				_accountCode = value;
			}
		}

		public void ConvertSmileToOfxStatementForDisplay() {
			ConvertSmileToOfxStatementForDisplayWithCurrentDateTime(new StatementManagerDateTime());
		}

		public void SaveOfxStatementToDisk() {
			_fileHandler.ValidateFileLocation(ConvertedOfxDestinationFolder);
			var destinationFile = _smileStatementManager.SaveOfxToFolder(OfxStatementSourcedFromSmile, ConvertedOfxDestinationFolder);

			ConfirmedDestinationFileLocation = string.Format("{0}\t[{1}]", destinationFile, DateTime.Now).Replace(@"\\", @"\");
			_ofxTransactions = ConvertStatementToTransactionList(OfxStatementSourcedFromSmile);
			OnPropertyChanged(() => GetTransactions);

			MessageBox.Show(string.Format("{0}{1}",
				Properties.Resources.MainWindow_runConversionClick_Done___check__,
				ConfirmedDestinationFileLocation));
		}

		private void ConvertSmileToOfxStatementForDisplayWithCurrentDateTime(IDateTime statementManagerDateTime) {
			_fileHandler.ValidateFileLocation(StatementLocation);
			if (string.IsNullOrEmpty(StatementLocation)) return;
			_smileStatementManager = new SmileStatementManager(StatementLocation, _parser, statementManagerDateTime);
			OfxStatementSourcedFromSmile = _smileStatementManager.GetStatementAsOfxObject();
			GetTransactions = ConvertStatementToTransactionList(OfxStatementSourcedFromSmile);

			AccountCode = OfxStatementSourcedFromSmile.StatementHeader.AccountNumber;
			StatementDate = OfxStatementSourcedFromSmile.StatementHeader.StatementDate;
			SortCode = OfxStatementSourcedFromSmile.StatementHeader.SortCode;

			OnPropertyChanged(() => StatementDate);
			OnPropertyChanged(() => SortCode);
			OnPropertyChanged(() => AccountCode);
		}

		private ObservableCollection<OfxTransaction> ConvertStatementToTransactionList(IEnumerable<OfxTransaction> statement) {
			var transactions = new ObservableCollection<OfxTransaction>();
			foreach (var ofxTransaction in statement) {
				transactions.Add(ofxTransaction);
			}
			return transactions;
		}

		private void SourceAndTargetFolderPopulated() {
			IsSaveButtonVisible = !IsStatementLocationEmpty &&
									  !IsConvertedOfxDestinationFolderEmpty;
		}

		private bool IsConvertedOfxDestinationFolderEmpty { get; set; }

		private void GetFullPathOfStatementFileName() {
			StatementLocation = _fileDialog.ShowDialog() != true ? string.Empty : _fileDialog.FileName;
			ConvertSmileToOfxStatementForDisplay();
		}

		private void GetFolderForOfxLocation() {
			_folderDialog = new FolderBrowserDialog { SelectedPath = ConvertedOfxDestinationFolder };

			var result = _folderDialog.ShowDialog();
			ConvertedOfxDestinationFolder = result.ToString() == "OK" ? _folderDialog.SelectedPath : string.Empty;
			OnPropertyChanged(() => IsConvertedOfxDestinationFolderEmpty);
			OnPropertyChanged(() => IsSaveButtonVisible);
		}

		private static bool IsDesignTime {
			get { return DesignerProperties.GetIsInDesignMode(new DependencyObject()); }
		}
	}
}
