using System;
using System.Xml.Linq;
using Utilities.Properties;

namespace Wells.StatementManagement {

	/// <summary>
	/// Extracts the  scalar values from the statement header
	/// </summary>
	 public class SmileStatementHeader {
		private readonly string _sortCode;
		private readonly string _accountCode;
		private readonly string _statementDate;
		private readonly string _serverDate;
		public readonly string AccountPageNumber;

	 	/// <summary>
	 	/// Populate the statement header values (by dumbly walking the TD elements).
	 	/// This is brittle: if eg Smile change sort code to even Sort Code, then this will break.
	 	/// And yet we don't want to make it case-insensitive, as that is guessing about the nature
	 	/// of the change - if ANYTHING changes re metadata, it should rightly break.
	 	/// The blocks of if/if below are because the label element is immediately followed by the 
	 	/// data element
	 	/// </summary>
	 	/// <param name="xml">the (now) well-formed XML document to be parsed for these header values</param>
	 	/// <param name="dateTime">Holds server/system date values for header purposes</param>
	 	public SmileStatementHeader(XDocument xml, IDateTime dateTime) {
	 		if (dateTime != null) {
	 			_serverDate = dateTime.GetCurrentDateTime().ToString("yyyyMMddHHmmss");
	 		}

	 	// painful way to get element after sortcode, etc...
			var sortcodeCandidates = xml.Descendants("td");
			var getSortCode = false;
			var getAccountCode = false;
			var getStatementDate = false;

			foreach (var td in sortcodeCandidates) {

				if (getSortCode) {
					_sortCode = td.Value.Trim();
					getSortCode = false;
				}
				if (td.Value.Equals("sort code")) {
					getSortCode = true;
				}

				if (getAccountCode) {
					_accountCode = td.Value.Trim();
					getAccountCode = false;
				}
				if (td.Value.Equals("account number")) {
					getAccountCode = true;
				}

				if (getStatementDate) {
					_statementDate = td.Value.Trim();
					getStatementDate = false;
				}
				if (td.Value.Equals("statement date")) {
					getStatementDate = true;
				}

				if (!td.Value.Contains("of your current account")) continue;
				var y = td.Value.IndexOf("page",StringComparison.InvariantCulture);
				var blockContainingPageNumber = td.Value.Substring(y, 30);
				AccountPageNumber = StringUtils.RemoveNonIntegerChars(blockContainingPageNumber);

			}
		}

		 public OfxStatementHeader ConvertSmileToOfxStatementHeader() {
			 return new OfxStatementHeader {
				 AccountNumber = _accountCode,
				 ServerDate = _serverDate,
				 SortCode = _sortCode,
				 StatementDate = _statementDate
			 };
		 }
	 }
}
