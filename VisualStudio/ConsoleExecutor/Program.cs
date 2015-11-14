using System;
using System.IO;
using System.Reflection;
using System.Text;
using ConsoleExecutor.Properties;
using Utilities;
using Wells.StatementManagement;

namespace ConsoleExecutor {
	// acks for use of the htmltidy.dll - http://sourceforge.net/projects/tidynet/

	internal static class Program {
		private const string ReadEncoding = "iso-8859-1";

		private static void Main() {
			const string destOfxFile = @"x.ofx";
			const string sourceStatementFile = @"ShortSmileStatementSource.htm";
			const string resourceFolder = "ConsoleExecutor.Resources";
			const string backSlash = @"\";
			string tempFolder = Environment.GetEnvironmentVariable("TEMP");

			// test purposes only...
			CopyResourceToFile(tempFolder, resourceFolder, sourceStatementFile);

			// todo - delete whole class
			IHtmlParser x = new SmileHtmlStatementParser();

			var statementManager = new StatementManager(tempFolder + backSlash + sourceStatementFile, x);
			statementManager.ProcessStatement();

			Console.WriteLine(Resources.ProgramMainComplete, destOfxFile);
			Console.ReadLine();
		}


		private static void CopyResourceToFile(string targetFolder, string resourceFolder, string resourceName) {
			Stream srcStatementStream =
				Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format(@"{0}.{1}", resourceFolder, resourceName));
			if (srcStatementStream == null) throw new ArgumentNullException(string.Format("[{0}] not found", resourceName));
			var ss = new StreamReader(srcStatementStream, Encoding.GetEncoding(ReadEncoding));
			string srcFilePathAndName = targetFolder + @"\" + resourceName;
			File.WriteAllText(srcFilePathAndName, ss.ReadToEnd(), Encoding.GetEncoding(ReadEncoding));
		}
	}
}