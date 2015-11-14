using System;

namespace Wells.StatementManagement {

	public class StatementManagerDateTime : IDateTime {

		/// <summary>
		/// Return the (System) current date time
		/// </summary>
		/// <returns>current date time in UTC format</returns>
		public DateTime GetCurrentDateTime() {
			return DateTime.UtcNow;
		}
	}
}
