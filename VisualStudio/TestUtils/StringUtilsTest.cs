using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;



namespace TestUtils {
	[TestClass]
	public class StringUtilsTest {

		string OriginalString = "BeforeAndxxxxx7After";
		string  StringToFind = "Andxxxxx7";
		string ReplacementChars = "Middle";



		[TestMethod]
		public void StringInsertedInStringBeforeLookedForString() {
			var ReplaceBeforeFoundString = true;
			var replacedString = StringUtils.InsertStringInString(OriginalString, StringToFind, ReplacementChars, ReplaceBeforeFoundString);
			Assert.AreEqual("BeforeMiddleAndxxxxx7After", replacedString);

		}

		[TestMethod]
		public void StringInsertedInStringAfterLookedForString() {
			var ReplaceBeforeFoundString = false;
			var replacedString = StringUtils.InsertStringInString(OriginalString, StringToFind, ReplacementChars, ReplaceBeforeFoundString);
			Assert.AreEqual("BeforeAndxxxxx7MiddleAfter", replacedString);

		}

		[TestMethod]
		public void StringNotFoundInString() {
			var ReplaceBeforeFoundString = false;
			var replacedString = StringUtils.InsertStringInString(OriginalString, "youwillnotfindthis", ReplacementChars, ReplaceBeforeFoundString);
			Assert.AreEqual(0, replacedString.Length);

		}
	}
}


