using System.Text.RegularExpressions;

namespace Utilities.Properties {
	public class StringUtils {

		public static string InsertStringInString(string originalString, string stringToFind, string replacementChars, bool replaceBeforeFoundString) {
			int index1 = originalString.IndexOf(stringToFind);
			if (index1 == -1) {
				return originalString;
			}

			var insertionPoint = replaceBeforeFoundString ? index1 : (index1 + stringToFind.Length);
			return originalString.Insert(insertionPoint, replacementChars);
		}

		public static string RemoveSpaces(string stringWithSpaces) {
		    return Regex.Replace(stringWithSpaces, "[ ]", "");
		}

		public static string RemoveNonMoneyChars(string stringWithNonMoneyChars) {
			return Regex.Replace(stringWithNonMoneyChars, "[^.0-9-+]", "");
		}

		public static string RemoveNonMoneyCharsIncSign(string stringWithNonMoneyChars) {
			return Regex.Replace(stringWithNonMoneyChars, "[^.0-9]", "");
		}

		public static string RemoveNonIntegerChars(string stringWithNonIntegerChars) {
			return Regex.Replace(stringWithNonIntegerChars, "[^0-9]", "");
		}
	}
}
