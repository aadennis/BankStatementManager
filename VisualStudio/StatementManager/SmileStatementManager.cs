using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Utilities;
using StringUtils = Utilities.Properties.StringUtils;

namespace Wells.StatementManagement {

	public class SmileStatementManager : IStatementManager {
		private readonly string _sourceStatementFilePath = "";
		private XDocument _ofxDoc;
		private SmileStatementHeader _statementHeader;
		private OfxStatementHeader _ofxHeader;

		private readonly IHtmlParser _smileParser;
		private readonly IDateTime _statementManagerDateTime;

		public string TidiedHtml { get; private set; }

		public SmileStatementManager() {}

		/// <summary>
		/// constructor - ensure source statement is passed
		/// </summary>
		/// <param name="sourceStatementFile">source statement file name including path</param>
		/// <param name="parser">the particular tidy-tool</param>
		/// <param name="statementManagerDateTime">contains the server date to be used in the header</param>
		public SmileStatementManager(string sourceStatementFile, IHtmlParser parser, IDateTime statementManagerDateTime) {
			if (string.IsNullOrEmpty(sourceStatementFile)) {
				throw new ArgumentNullException("sourceStatementFile", @"No source statement file specified");
			}
			if (parser == null) {
				throw new ArgumentNullException("parser", @"parser is null");
			}
			_sourceStatementFilePath = sourceStatementFile;
			_smileParser = parser;
			_statementManagerDateTime = statementManagerDateTime;
		}

		public XDocument OfxDoc {
			get { return _ofxDoc; }
			private set {
				if (_ofxDoc == value) return;
				_ofxDoc = value;
			}
		}

		public SmileStatementHeader StatementHeader {
			get { return _statementHeader; }
			private set {
				if (_statementHeader == value) return;
				_statementHeader = value;
			}
		}

		public OfxStatementHeader OfxHeader {
			get { return _ofxHeader; }
			private set {
				if (_ofxHeader == value) return;
				_ofxHeader = value;
			}
		}

		/// <summary>
		/// Given a (Smile) statement on the disk, convert to an Ofx-compliant statement, including header information
		/// </summary>
		/// <returns>Ofx statement in object form</returns>
		public OfxStatement GetStatementAsOfxObject() {
			//task.factory.startnew

			TidiedHtml = _smileParser.TidyStatement(_sourceStatementFilePath);
			var tidiedStatementAsXml = XDocument.Parse(TidiedHtml);

			// todo - I want to test this, but in order to do that, need to make this public (or an accessor) for now...
			var sList = ConvertStatementFromTidiedHtmlToSmileStatement(tidiedStatementAsXml);

			return ConvertSmileStatementObjectToOfxStatementObject(sList, tidiedStatementAsXml);
		}

		public string SaveOfxToFolder(OfxStatement ofxList, string targetOfxFolder) {
			if (targetOfxFolder == null) {
				throw new Exception("No target OFX folder specified");
			}
			// todo - feels a bit smelly - this could be called before _statementHeader has been populated - how to 
			// design to avoid that?
			var ofxLocation = string.Format(@"{0}\StatementPage{1}.ofx", targetOfxFolder, StatementHeader. AccountPageNumber);
			OfxDoc = OfxManager.OutputOfxElements(ofxList, OfxHeader);
			OfxDoc.Save(ofxLocation);
			return ofxLocation;
		}

		public XDocument GetOfxStatement() {
			return OfxManager.OutputOfxElements(GetStatementAsOfxObject(),OfxHeader);
		}

		public List<SmileStatementTransaction> ConvertStatementFromTidiedHtmlToSmileStatement(XDocument xml) {
			SmileStatementTransaction s = null;
			var sList = new List<SmileStatementTransaction>();
			var currentColumn = 1;

			var inners = xml.Descendants("td").Where(x => x.HasAttributes && x.FirstAttribute.Value.Contains("summaryDetail"));
			foreach (var td in inners) {
				if (td.FirstAttribute.Value.Equals("summaryDetailC")) {
					s = new SmileStatementTransaction();
					currentColumn = 0; // Changed to index columns starting from 0
				}
				else {
					currentColumn++;
				}
				ProcessTransactionColumn(td, currentColumn, s, sList);
			}
			return sList;
		}

		/// <summary>
		/// Convert a Smile statement (both header and transactions) to Ofx format
		/// </summary>
		/// <param name="smileTransactions">List of Smile transactions</param>
		/// <param name="statementAsXml">the original Smile statement converted to xml, to allow the getting of header information</param>
		/// <param name="fitIdPrefix"></param>
		/// <returns></returns>
		public OfxStatement ConvertSmileStatementObjectToOfxStatementObject(IEnumerable<SmileStatementTransaction> smileTransactions, XDocument statementAsXml) {
			var transactionCount = 0;
			var ofxTransactions = new List<OfxTransaction>();
			StatementHeader = new SmileStatementHeader(statementAsXml, _statementManagerDateTime);

			foreach (var txn in smileTransactions) {
				var ofxTransaction = new OfxTransaction {
				                           	Amount =
				                           		StringUtils.RemoveNonMoneyChars(txn.AmountAsDebit.Contains(".")
				                           		                                	? "-" + txn.AmountAsDebit
				                           		                                	: "+" + txn.AmountAsCredit),
				                           	DatePosted =
				                           		DateTimeUtils.ConvertDateAddTrailingZeroes(txn.TransactionDate, "dd/MM/yyyy",
				                           		                                           "yyyyMMdd", 6),
				                           	FitId = string.Format("{0}-{1}", StatementHeader.AccountPageNumber, ++transactionCount),
				                           	Name = txn.TransactionDetail,
											Memo = txn.TransactionDetail,
				                           	RunningBalance = string.Empty,
				                           	TrnType = DebitOrCredit(txn.AmountAsCredit)
				};
				ofxTransactions.Add(ofxTransaction);
			}
			
			OfxHeader = StatementHeader.ConvertSmileToOfxStatementHeader();
			return new OfxStatement(ofxTransactions, OfxHeader);
		}

		private static TrnTypeEnum DebitOrCredit(string amountAsCredit) {
			return string.IsNullOrEmpty(amountAsCredit) ? TrnTypeEnum.Debit : TrnTypeEnum.Credit;
		}

		private static void ProcessTransactionColumn(XElement td, int currentColumn, SmileStatementTransaction s,
		                                             List<SmileStatementTransaction> sList) {

			Action<string>[] columnAssignments = {
				value => s.TransactionDate = value,
				value => s.TransactionDetail = value,
				value => s.AmountAsCredit = value,
				value => s.AmountAsDebit = value,
				value => s.RunningBalance = value
			};

			// No default - assumes that 4 is the highest column here (if not will throw an array out of bounds exception)
			columnAssignments[currentColumn](string.IsNullOrWhiteSpace(td.Value) ? string.Empty : td.Value);

			// Check against last index of column assignments (so adding another column does not change this code)
			if (currentColumn != columnAssignments.Length - 1) return;

			// TransactionDetail containing BROUGHT FORWARD is not a regular row
			if (!s.TransactionDetail.Contains("FORWARD")) {
				sList.Add(s);
			}
		}
	}
}