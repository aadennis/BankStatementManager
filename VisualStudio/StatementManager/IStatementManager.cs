namespace Wells.StatementManagement {
	public interface IStatementManager {

		/// <summary>
		/// Convert the (probably) malformed html from the bank website to an Ofx statement in object form.
		/// Get the header information and transactions from the (single) statement,
		/// and save to an OFX-compliant XML doc.
		/// </summary>
		OfxStatement GetStatementAsOfxObject();

		/// <summary>
		/// Save the OFX file in a location which may be based on the passed name
		/// </summary>
		/// <param name="ofxList">The OfxStatement to be saved to file</param>
		/// <param name="targetOfxFolder">target ofx folder</param>
		/// <returns>the location of the saved OFX file for use by the caller</returns>
		string SaveOfxToFolder(OfxStatement ofxList, string targetOfxFolder);

	}
}
