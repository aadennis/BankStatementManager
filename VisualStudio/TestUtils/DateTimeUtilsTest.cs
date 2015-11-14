using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities;

namespace TestUtils {
	[TestClass]
	public class DateTimeUtilsTest {
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ParseDateDateInArgumentIsNullThrowsException() {
			var dummy = DateTimeUtils.ConvertDateAddTrailingZeroes(null, "x", "x", 0);
			Assert.Fail("Failed to throw exception");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ParseDatePatternInArgumentIsNullThrowsException() {
			var dummy = DateTimeUtils.ConvertDateAddTrailingZeroes("x", null, "x", 0);
			Assert.Fail("Failed to throw exception");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ParseDatePatternOutArgumentIsNullThrowsException() {
			var dummy = DateTimeUtils.ConvertDateAddTrailingZeroes("x", "x", null, 0);
			Assert.Fail("Failed to throw exception");
		}

		[TestMethod]
		public void ConvertDateAdds5Zeroes() {
			const string inDate = "31/12/2013";
			const string inPattern = "dd/MM/yyyy";
			const string outPattern = "yyyyMMdd";
			var outDate = DateTimeUtils.ConvertDateAddTrailingZeroes(inDate, inPattern, outPattern, 5);
			Assert.AreEqual("2013123100000", outDate);
		}

		[TestMethod]
		[ExpectedException(typeof(Exception))]
		public void ConvertDateThrowsExceptionOnBadFormat() {
			const string inDate = "31/12/2013x";
			const string inPattern = "dd/MM/yyyy";
			const string outPattern = "yyyyMMdd";
			DateTimeUtils.ConvertDateAddTrailingZeroes(inDate, inPattern, outPattern, 5);
			Assert.Fail("Expected exception not thrown");
		}

	}
}
