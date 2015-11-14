namespace Wells.StatementManagement {

	/// <summary>
	/// The set of singleton values found in a Ofx compliant statement
	/// </summary>
	public class OfxStatementHeader {
		/// <summary>
		/// Statement date
		/// </summary>
		public string StatementDate { get; set; }

		/// <summary>
		/// Account number of the current statement
		/// </summary>
		public string AccountNumber { get; set; }

		/// <summary>
		/// Sort code associated with the account number
		/// </summary>
		public string SortCode { get; set; }

		/// <summary>
		/// Time at which the Ofx conversion happened
		/// </summary>
		public string ServerDate { get; set; }
	}
}
