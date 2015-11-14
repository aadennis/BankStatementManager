using System;

namespace Utilities {
	public static class DateTimeUtils {

		public static string ConvertDateAddTrailingZeroes(string dateIn, string patternIn, string patternOut, int zeroCount) {
			return ParseDate(dateIn, patternIn, patternOut) + new string('0', zeroCount);
		}

		private static string ParseDate(string dateIn, string patternIn, string patternOut) {
			if (dateIn == null) throw new ArgumentNullException("dateIn");
			if (patternIn == null) throw new ArgumentNullException("patternIn");
			if (patternOut == null) throw new ArgumentNullException("patternOut");

			DateTime parsedDate;

			if (DateTime.TryParseExact(dateIn, patternIn, null, System.Globalization.DateTimeStyles.None, out parsedDate)) {
				return parsedDate.ToString(patternOut);
			} else {
				throw new Exception(string.Format("{0} is not a valid date format for a pattern {1}", dateIn, patternIn));
			}
		}
	}
}
