namespace Wells.StatementManagement {

	/// <summary>
	/// Parser of Html-based bank statements.
	/// Any implementing class must be persistence-ignorant 
	/// </summary>
	public interface IHtmlParser {

		/// <summary>
		/// Tidy an html bank statement, defined as returning well-formed html,
		/// that can then be parsed as an xml document
		/// </summary>
		/// <param name="statement">bank statement in (possibly) "malformed" html</param>
		/// <returns>bank statement with well-formed html</returns>
		string TidyStatement(string statement);
	}
}
