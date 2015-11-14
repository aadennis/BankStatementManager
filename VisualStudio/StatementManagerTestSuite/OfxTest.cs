using System;
using System.Collections.Generic;
using System.Linq;
using CommonTestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wells.StatementManagement;

namespace StatementManagerTests {
	[TestClass]
	public class OfxTest {
		readonly OfxFactory _ofxFactory = new OfxFactory();

		[TestMethod]
		public void OfxStatementWithNullPropertiesThrowsException() {

			CheckForNullProperties("Amount", "DatePosted", "FitId", "Name", "Memo",
			                       "RunningBalance");
		}

		[TestMethod]
		public void OfxStatementWithOkTransactionSetCanBeConstructed() {
			var transactions = _ofxFactory.GetOfxTransactions();
			var sumOfTransactions =
				transactions.Select(x => new {amountAsDecimal = Decimal.Parse(x.Amount)}).Sum(s => s.amountAsDecimal);

			Assert.AreEqual(549.76M, sumOfTransactions);
			Assert.AreEqual(2, transactions.Count());
		}

		private static void CheckForNullProperties(params string [] propertyNamesForOfxTransaction) {
			foreach (var propertyName in propertyNamesForOfxTransaction) {
				var name = propertyName;
				CheckForNullProperty(name);
			}
		}

		private static void CheckForNullProperty(string propertyName) {
			var ofxFactory = new OfxFactory();
			var statementHeader = new OfxStatementHeader();
			var transactions =
				new List<OfxTransaction> {
				                         	ofxFactory.GetOfxTransactionWithPassedPropertySetToNull(propertyName)
				                         };

			AssertException.Throws<NullReferenceException>(() => new OfxStatement(transactions, statementHeader),
														   e => Assert.AreEqual(string.Format("{0} must have a value", propertyName), e.Message));
		}

	}
}