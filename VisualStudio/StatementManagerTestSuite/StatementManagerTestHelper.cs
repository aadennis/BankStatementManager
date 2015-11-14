using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace CommonTestUtilities {
	static internal class StatementManagerTestHelper {

		private const string ReadEncoding = "iso-8859-1";
		const string ResourceFolder = "StatementManagerTests.Resources";

		internal static string GetStringFromResource(string resourceName) {
			var srcStatementStream =
				Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format(@"{0}.{1}", ResourceFolder, resourceName));
			if (srcStatementStream == null) throw new ArgumentNullException(string.Format("[{0}] not found", resourceName));
			return new StreamReader(srcStatementStream, Encoding.GetEncoding(ReadEncoding)).ReadToEnd();
		}
	}
}
