using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonTestUtilities;
using Utilities;


namespace TestUtils {

	internal class TestClass {
		public string TestProperty { get; set;}
	}

	[TestClass]
	public class ObjectUtilitiesTest {
		private TestClass _testClass;
		
		[TestInitialize]
		public void Initialize() {
			_testClass = new TestClass {TestProperty = "A Value"};			
		}

		[TestMethod]
		public void PropertyIsSetToUpdatedValue() {
			Assert.AreEqual("A Value", _testClass.TestProperty);
			ObjectUtilities.SetPropertyValue(_testClass, "TestProperty", "Changed it");
			Assert.AreEqual("Changed it", _testClass.TestProperty);
		}

		[TestMethod]
		public void PropertyCannotBeSetToUpdatedValueBecausePropertyDoesNotExist() {
			AssertException.Throws<NullReferenceException>(
				() => ObjectUtilities.SetPropertyValue(_testClass, "TestPropertyDoesNotExist", "Changed it"),
				e => Assert.AreEqual("Property [TestPropertyDoesNotExist] is not valid", e.Message));
		}
	}
}
