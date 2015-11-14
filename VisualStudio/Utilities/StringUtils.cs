using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace Utilities {

	public class StringUtils {

		public static string InsertStringInString(string OriginalString, string StringToFind, string ReplacementChars, bool ReplaceBeforeFoundString) {
			int index1 = OriginalString.IndexOf(StringToFind);
			if (index1 == -1) {
				return "";
			}

			var insertionPoint = ReplaceBeforeFoundString ? index1 : (index1 + StringToFind.Length);
			return OriginalString.Insert(insertionPoint, ReplacementChars);
		}

		public static string RemoveSpaces(string StringWithSpaces) {
		    return Regex.Replace(StringWithSpaces, "[ ]", "");
		}

		public static string RemoveNonMoneyChars(string StringWithNonMoneyChars) {
			return Regex.Replace(StringWithNonMoneyChars, "[^.0-9-+]", "");
		}

		public static string RemoveNonIntegerChars(string StringWithNonIntegerChars) {
			return Regex.Replace(StringWithNonIntegerChars, "[^0-9]", "");
		}
	}
}
