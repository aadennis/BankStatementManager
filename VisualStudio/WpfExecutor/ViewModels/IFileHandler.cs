namespace WpfExecutor.ViewModels {
	/// <summary>
	/// For classes to implement filehandling - in practice, the reading of the raw statement and the writing of the 
	/// Ofx-compliant file
	/// </summary>
	public interface IFileHandler {

		/// <summary>
		/// Check that the specified disk location is valid - implementor should throw exception if not found
		/// </summary>
		/// <param name="location">full path to the named file</param>
		void ValidateFileLocation(string location);
	}
}
