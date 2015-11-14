using System;

namespace Wells.StatementManagement {

	/// <summary>
	/// Maintains significant date time values for the Statement Management namespace
	/// (for the moment...) just to allow testability when dealing with system clock dependent values
	/// </summary>
	public interface IDateTime {

		/// <summary>
		/// Get a version of the current date/time
		/// </summary>
		/// <returns>A datetime value</returns>
		DateTime GetCurrentDateTime();
	}
}
