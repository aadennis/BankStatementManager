using System;
using System.Collections.Generic;
using System.Linq;
using Wells.StatementManagement;
using System.Reflection;
using System.IO;
using System.Text;


namespace Wells.StatementManagement {

	// acks for use of the htmltidy.dll - http://sourceforge.net/projects/tidynet/

	class Program {

		private static readonly string readEncoding = "iso-8859-1";

		static void Main(string[] args) {
			//string srcStatementFile = @"D:\data\Dropbox\household\bank\Smile\Page77.htm";
			string destOfxFile = @"d:\\scratch\\x.ofx";

			var srcStatementStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(@"ConsoleExecutor.Artifacts.SmileStatementSource.htm");
			var ss = new StreamReader(srcStatementStream, Encoding.GetEncoding(readEncoding));
			string srcStatementFile = ss.ReadToEnd();

			var statementManager = new StatementManager(srcStatementFile, destOfxFile);
			statementManager.ProcessStatement();
		}
	}
}
