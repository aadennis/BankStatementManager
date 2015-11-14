using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using CommonTestUtilities;
using CommonTestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wells.StatementManagement;
using Moq;
using System.Xml.Linq;

namespace CommonTestUtilities {
	[TestClass]
	public class StatementManagerTest {
		private Mock<IDateTime> _mockDateTime;

		[TestInitialize]
		public void Initialize() {
			_mockDateTime = new Mock<IDateTime>(MockBehavior.Strict);
			_mockDateTime.Setup(x => x.GetCurrentDateTime()).Returns(new DateTime(2013, 1, 1));
			
		}

		[TestMethod]
		public void TidiedSmileStatementAsXmlConvertsToTransactionsAsObjectGraph() {
			var xmlToConvertToArray = XDocument.Parse(StatementManagerTestHelper.GetStringFromResource(@"ExpectedTidiedTestStatement.htm"));
			var actualArray = new SmileStatementManager().ConvertStatementFromTidiedHtmlToSmileStatement(xmlToConvertToArray);
			var count = 0;
			foreach (var smileStatementTransaction in actualArray) {
				ValidateSmileTransaction(smileStatementTransaction, count++);
			}
		}

		[TestMethod]
		public void SmileStatementTransactionsConvertToOfxTransactions() {
			var xmlWithHeader = XDocument.Parse(StatementManagerTestHelper.GetStringFromResource(@"ExpectedTidiedTestStatement.htm"));
			var actualArray = new SmileStatementManager().ConvertSmileStatementObjectToOfxStatementObject(ExpectedSmileStatementTransactions, xmlWithHeader); 
			var count = 0;
			foreach (var ofxTransaction in actualArray) {
				ValidateOfxTransaction(ofxTransaction, count++);
			}
		}

		[TestMethod]
		public void OfxStatementAsObjectConvertstoOfxAsXml() {
			var tidiedStatementXml = XDocument.Parse(StatementManagerTestHelper.GetStringFromResource(@"ExpectedTidiedTestStatement.htm"));
			var expectedOfxStatementXml = XDocument.Parse(StatementManagerTestHelper.GetStringFromResource(@"ExpectedOfxStatement.xml"));

			var statementHeader = new SmileStatementHeader(tidiedStatementXml, _mockDateTime.Object).ConvertSmileToOfxStatementHeader();
			var ofxStatementBody = new OfxStatement(ExpectedOfxStatement, statementHeader);
			var actualArray = OfxManager.OutputOfxElements(ofxStatementBody, statementHeader);

			_mockDateTime.VerifyAll();

			Assert.IsTrue(XNode.DeepEquals(expectedOfxStatementXml, actualArray));
		}

		[TestMethod]
		public void NullUntidyHtmlParameterThrowsException() {
			var mockParser = new Mock<IHtmlParser>(MockBehavior.Loose);

			AssertException.Throws<ArgumentNullException>(
				() => new SmileStatementManager(null, mockParser.Object, null),
				e => Assert.AreEqual(
					"No source statement file specified\r\nParameter name: sourceStatementFile", e.Message));
		}

		[TestMethod]
		public void EmptySourceStatementThrowsException() {
			var mockParser = new Mock<IHtmlParser>(MockBehavior.Loose);

			AssertException.Throws<ArgumentNullException>(
				() => new SmileStatementManager(string.Empty, mockParser.Object, null),
				e => Assert.AreEqual(
					"No source statement file specified\r\nParameter name: sourceStatementFile", e.Message));
		}

		[TestMethod]
		public void NullParserParameterThrowsException() {
			new Mock<IHtmlParser>(MockBehavior.Loose);

			AssertException.Throws<ArgumentNullException>(
				() => new SmileStatementManager("UntidyHtml", null, null),
				e => Assert.AreEqual(
					"parser is null\r\nParameter name: parser", e.Message));
		}

		//todo - complete test - requires MUT to be refactored
		[TestMethod]
		public void TidyReturnsWellFormedHtml() {
			const string untidyStatement = "untidy html content";
			var tidiedStatement = StatementManagerTestHelper.GetStringFromResource(@"ExpectedTidiedTestStatement.htm");
			var mockParser = new Mock<IHtmlParser>(MockBehavior.Strict);
			mockParser.Setup(x => x.TidyStatement(It.IsAny<string>())).Returns(tidiedStatement);

			var sm = new SmileStatementManager(untidyStatement, mockParser.Object, null);
			sm.GetStatementAsOfxObject();
			mockParser.VerifyAll();
		}

		[TestMethod]
		public void TidyConvertsSourceHtmlToWellFormedHtml() {
			const string resourceName = "TestStatement.htm";
			var targetFolder = Environment.CurrentDirectory;
			var srcFilePathAndName = targetFolder + @"\" + resourceName;
			WriteResourceToFile(resourceName);

			var smileHtmlStatementParser = new SmileHtmlStatementParser() as IHtmlParser;

			var smileStatementManager = new SmileStatementManager(srcFilePathAndName, smileHtmlStatementParser, null);
			smileStatementManager.GetStatementAsOfxObject();
			var tidiedHtml = smileStatementManager.TidiedHtml;
			var expectedStatement = StatementManagerTestHelper.GetStringFromResource(@"ExpectedTidiedTestStatement.htm");

			Assert.AreEqual(expectedStatement, tidiedHtml);
		}

		private static void WriteResourceToFile(string resourceName) {
			const string resourceFolder = @"StatementManagerTests.Resources";
			const string readEncoding = "iso-8859-1";
			var targetFolder = Environment.CurrentDirectory;
			var srcFilePathAndName = targetFolder + @"\" + resourceName;

			//var resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

			var srcStatementStream =
				Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format(@"{0}.{1}", resourceFolder, resourceName));
			if (srcStatementStream == null) throw new ArgumentNullException(string.Format("[{0}] not found", resourceName));
			var ss = new StreamReader(srcStatementStream, Encoding.GetEncoding(readEncoding));

			File.WriteAllText(srcFilePathAndName, ss.ReadToEnd(), Encoding.GetEncoding(readEncoding));
		}

		private static void ValidateSmileTransaction(SmileStatementTransaction smileStatementTransaction, int index) {
			Assert.AreEqual(ExpectedSmileStatementTransactions[index].AmountAsDebit, smileStatementTransaction.AmountAsDebit);
			Assert.AreEqual(ExpectedSmileStatementTransactions[index].TransactionDetail, smileStatementTransaction.TransactionDetail);
			Assert.AreEqual(ExpectedSmileStatementTransactions[index].AmountAsCredit, smileStatementTransaction.AmountAsCredit);
			Assert.AreEqual(ExpectedSmileStatementTransactions[index].RunningBalance, smileStatementTransaction.RunningBalance);
			Assert.AreEqual(ExpectedSmileStatementTransactions[index].TransactionDate, smileStatementTransaction.TransactionDate);
		}

		private static void ValidateOfxTransaction(OfxTransaction ofxTransaction, int index) {
			Assert.AreEqual(ExpectedOfxStatement[index].Amount, ofxTransaction.Amount);
			Assert.AreEqual(ExpectedOfxStatement[index].DatePosted, ofxTransaction.DatePosted);
			Assert.AreEqual(ExpectedOfxStatement[index].FitId, ofxTransaction.FitId);
			Assert.AreEqual(ExpectedOfxStatement[index].Name, ofxTransaction.Name);
			Assert.AreEqual(ExpectedOfxStatement[index].Memo, ofxTransaction.Memo);
			Assert.AreEqual(ExpectedOfxStatement[index].TrnType, ofxTransaction.TrnType);
		}

		private static readonly List<SmileStatementTransaction> ExpectedSmileStatementTransactions = new List<SmileStatementTransaction> {
			                    new SmileStatementTransaction {
			                                                    AmountAsDebit = "`10.01",
			                                                    TransactionDetail = "DENTAL PLAN 333",
			                                                    AmountAsCredit = "",
			                                                    RunningBalance = "`39.00+",
			                                                    TransactionDate = "01/03/2010"
			                                                    },
			                    new SmileStatementTransaction {
			                                                    AmountAsDebit = "`3.45",
			                                                    TransactionDetail = "1785778",
			                                                    AmountAsCredit = "",
			                                                    RunningBalance = "`35.55+",
			                                                    TransactionDate = "02/03/2010"
			                                                    },
			                    new SmileStatementTransaction {
			                                                    AmountAsDebit = "",
			                                                    TransactionDetail = "INTEREST TAX PAID",
			                                                    AmountAsCredit = "`0.46",
			                                                    RunningBalance = "`36.01+",
			                                                    TransactionDate = "21/03/2010"
			                                                    },
			                    new SmileStatementTransaction { 
			                                                    AmountAsDebit = "",
			                                                    TransactionDetail = "THE EMPLOYMENT",
			                                                    AmountAsCredit = "`200.02",
			                                                    RunningBalance = "`236.03+",
			                                                    TransactionDate = "22/03/2010"
			                                                    },
			                    new SmileStatementTransaction { 
			                                                    AmountAsDebit = "`30.00",
			                                                    TransactionDetail = "LINK 13:37APR04",
			                                                    AmountAsCredit = "",
			                                                    RunningBalance = "`206.03+",
			                                                    TransactionDate = "04/04/2010"
			                                                    }
		};


		private static readonly List<OfxTransaction> ExpectedOfxStatement =
			new List<OfxTransaction> {
			                         	new OfxTransaction {
			                         	                   	Amount = "-10.01",
			                         	                   	DatePosted = "20100301000000",
			                         	                   	FitId = "177-1",
			                         	                   	Name = "DENTAL PLAN 333",
															Memo = "DENTAL PLAN 333",
			                         	                   	RunningBalance = "",
															TrnType = TrnTypeEnum.Debit
			                         	                   },
			                         	new OfxTransaction {
			                         	                   	Amount = "-3.45",
			                         	                   	DatePosted = "20100302000000",
			                         	                   	FitId = "177-2",
			                         	                   	Name = "1785778",
															Memo = "1785778",
			                         	                   	RunningBalance = "",
															TrnType = TrnTypeEnum.Debit
			                         	                   },
			                         	new OfxTransaction {
			                         	                   	Amount = "+0.46",
			                         	                   	DatePosted = "20100321000000",
			                         	                   	FitId = "177-3",
															Name = "INTEREST TAX PAID",
															Memo = "INTEREST TAX PAID",
			                         	                   	RunningBalance = "",
															TrnType = TrnTypeEnum.Credit
			                         	                   },
			                         	new OfxTransaction {
			                         	                   	Amount = "+200.02",
			                         	                   	DatePosted = "20100322000000",
			                         	                   	FitId = "177-4",
															Name = "THE EMPLOYMENT",
															Memo = "THE EMPLOYMENT",
			                         	                   	RunningBalance = "",
															TrnType = TrnTypeEnum.Credit
			                         	                   },
			                         	new OfxTransaction {
			                         	                   	Amount = "-30.00",
			                         	                   	DatePosted = "20100404000000",
			                         	                   	FitId = "177-5",
															Name = "LINK 13:37APR04",
															Memo = "LINK 13:37APR04",
			                         	                   	RunningBalance = "",
															TrnType = TrnTypeEnum.Debit
			                         	                   },
		};
	}
}