namespace Wells.StatementManagement {

	/// <summary>
	/// A bank statement transaction implementation specific to the Smile Bank.
	/// </summary>
	public class SmileStatementTransaction {

		public SmileStatementTransaction() {
			TransactionDate = string.Empty;
			TransactionDetail = string.Empty;
			AmountAsCredit = string.Empty;
			AmountAsDebit = string.Empty;
			RunningBalance = string.Empty;
		}

		/// <summary>
		/// Transaction Date
		/// Todo - convert to a DateTime, validate
		/// </summary>
		public string TransactionDate { get;  set; }

		/// <summary>
		/// Transaction detail
		/// </summary>
		public string TransactionDetail { get;  set; }

		/// <summary>
		/// The unsigned amount of the credit - mutually exclusive with debit
		/// todo - have 1 property [Amount], signed, int, and 1 property [IsDebit] boolean
		/// as ever, this will be unable to automatically cope with changes made by Smile to their html
		/// </summary>
		public string AmountAsCredit { get;  set; }

		/// <summary>
		/// The unsigned amount of the debit - mutually exclusive with credit
		/// </summary>
		public string AmountAsDebit { get; set; }

		/// <summary>
		/// The signed running balance after the current transaction has been applied. 
		/// Note that the Smile statement contains a "Brought Forward" line which precedes true transactions,
		/// and which logically belongs to the statement, not the set of transactions.
		/// Todo - fix that.
		/// </summary>
		public string RunningBalance { get;  set; }
	}
}
