using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommonTestUtilities {
	public static class AssertException {
		public static void Throws<TException>(Action action, Action<TException> exception) where TException : Exception {
			try {
				action.Invoke();
			} catch (TException e) {
				exception.Invoke(e);
				return;
			} catch (Exception e) {
				Assert.Fail(string.Format("Expected exception [{0}], actual [{1}]", typeof(TException), e.GetType()));
			}
			Assert.Fail(string.Format("Expected exception [{0}], but no exception was thrown", typeof(TException)));
		}
	}
}