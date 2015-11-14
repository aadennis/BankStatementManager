using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wells.StatementManagement {

	/// <summary>
	/// Represents a collection of Ofx transactions, ie a bank statement
	/// </summary>
	public class OfxStatement : IEnumerable<OfxTransaction> {

		private readonly List<OfxTransaction> _transactions;

		public readonly OfxStatementHeader StatementHeader;

		public OfxStatement(List<OfxTransaction> transactions, OfxStatementHeader statementHeader) {
			ValidateProperty(transactions, "Amount", "DatePosted", "FitId", "Name", "Memo", "RunningBalance");
			_transactions = transactions;

			StatementHeader = new OfxStatementHeader {
				StatementDate = statementHeader.StatementDate,
				AccountNumber = statementHeader.AccountNumber,
				SortCode = statementHeader.SortCode,
				ServerDate = statementHeader.ServerDate
			};
		}

		public List<OfxTransaction> GetTransactions() {
			return _transactions;
		}

		public OfxTransaction GetTransaction(int index) {
			return _transactions[index];
		}

		public IEnumerator<OfxTransaction> GetEnumerator() {
			return _transactions.GetEnumerator();
		}

		private static void ValidateProperty(IEnumerable<OfxTransaction> transactions, params string[] propertyNames) {
			// todo - throw exception if property name does not exist for this type

			foreach (var propertyName in propertyNames) {
				var name = propertyName;
				if (transactions.Any(t => t.GetType().GetProperty(name).GetValue(t, null) == null)) {
					throw new NullReferenceException(string.Format("{0} must have a value", name));
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
