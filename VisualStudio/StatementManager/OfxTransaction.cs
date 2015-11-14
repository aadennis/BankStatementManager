namespace Wells.StatementManagement {

	/// <summary>
		/// Set of allowed Transaction types
		/// </summary>
		public enum TrnTypeEnum {
			Debit,
			Credit
		} ;


	/// <summary>
	/// A bank statement transaction which conforms to the OFX standard. Example:
	/// <STMTTRN>
	///  <TRNTYPE>DEBIT</TRNTYPE>
	///  <DTPOSTED>20130307000000</DTPOSTED>
	///  <TRNAMT>-3.00</TRNAMT>
	///  <FITID>185-22</FITID>
	///  <NAME>WWW.GIFFGAFF.COM</NAME>
	///  <MEMO>WWW.GIFFGAFF.COM</MEMO>
	///</STMTTRN>
	/// </summary>
	public class OfxTransaction  {

	
		/// <summary>
		/// OFX name - TRNTYPE
		/// Transaction type - restricted to TrnTypeEnum
		/// </summary>
		public TrnTypeEnum TrnType { get; set; }

		/// <summary>
		/// OFX name - DTPOSTED
		/// Date of transaction posting in the format YYYYMMDD, with 6 trailing zeroes, which may
		/// be populated, but are not in this application eg  20130307000000
		/// </summary>
		public string DatePosted { get; set;}

		/// <summary>
		/// OFX name - TRNAMT
		/// signed amount of the transaction - minus sign represents a debit from the 
		/// perspective of the current account holder
		/// </summary>
		public string Amount { get; set;}

		/// <summary>
		/// OFX name - NAME
		/// The name of the payee
		/// todo - maybe need a class SmileOfxCoupler
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// OFX name - MEMO
		/// A memo that can be associated with the transaction - if no dedicated memo, duplicate the name value
		/// </summary>
		public string Memo { get; set; }

		/// <summary>
		/// OFX name - FITID
		/// The unique identifier of the transaction within a statement. A surrogate key, ie without "business" meaning
		/// </summary>
		public string FitId { get; set;}

		/// <summary>
		/// OFX name - n/a
		/// This is a convenience property for keeping track of the running balance, relative to other transactions in the
		/// statement - it is not a requirement of the OFX standard in the context of a single transaction, and should therefore
		/// be factored out to a OfxStatement class, where the starting and final balances are used for reconciliation.
		/// </summary>
		public string RunningBalance { get; set;}
	}
}
