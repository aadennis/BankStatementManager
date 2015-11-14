using System.Collections.Generic;
using Wells.StatementManagement;
using Utilities;

namespace CommonTestUtilities {

	/// <summary>
	/// Factory to create Ofx objects for test purposes
	/// </summary>
	public class OfxFactory {
		private readonly List<OfxTransaction> _ofxTransactions;
		private readonly OfxTransaction _ofxTransaction;
		private readonly OfxTransaction _ofxTransaction2;

		/// <summary>
		/// Creates a set of test transactions, and a list containing those transactions
		/// </summary>
		public OfxFactory() {
			_ofxTransaction =
				new OfxTransaction {
				                   	Amount = "+402.99",
				                   	DatePosted = "20130307000000",
				                   	FitId = "001",
				                   	Name = "The Payee",
									Memo = "The Memo",
				                   	RunningBalance = "200.29",
				                   	TrnType = TrnTypeEnum.Credit
				                   };

			_ofxTransaction2 =
				new OfxTransaction {
				                   	Amount = "+146.77",
				                   	DatePosted = "20130308000000",
				                   	FitId = "001",
				                   	//todo - test for duplicate FitIds
									Name = "The Payee2",
									Memo = "The Memo2",
									RunningBalance = "600.29",
				                   	TrnType = TrnTypeEnum.Debit
				                   };


			_ofxTransactions = new List<OfxTransaction> {_ofxTransaction, _ofxTransaction2};
		}

		/// <summary>
		/// Return an arbitrary one of the test transactions, first setting one of the properties to null
		/// </summary>
		/// <param name="propertyName">the property to be set to null</param>
		/// <returns>a test transaction, with the required property set to null</returns>
		public OfxTransaction GetOfxTransactionWithPassedPropertySetToNull(string propertyName) {
			ObjectUtilities.SetPropertyValue(_ofxTransaction, propertyName, null);
			return _ofxTransaction;
		}

		/// <summary>
		/// Gets the set of test transactions
		/// </summary>
		/// <returns>the set of transactions</returns>
		public IEnumerable<OfxTransaction> GetOfxTransactions() {
			return _ofxTransactions;
		}
	}
}