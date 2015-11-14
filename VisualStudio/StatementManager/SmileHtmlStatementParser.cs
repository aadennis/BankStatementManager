using System.IO;
using System.Text;
using TidyNet;
using Utilities.Properties;

namespace Wells.StatementManagement {
	public class SmileHtmlStatementParser : IHtmlParser{
		private const string ReadEncoding = "iso-8859-1";

		readonly Tidy _tidy;

		public SmileHtmlStatementParser() {
			_tidy = new Tidy();
			SetParameters(_tidy);
		}

		/// <summary>
		/// Run the input html through the HtmlTidy library, first replacing £ sign with `, as
		/// lib does not seem to support iso-8859-1, the encoding for the original statement.
		/// Also comment out the js script tags, as these fail parsing.
		/// </summary>
		/// <returns>A wellformed xml (bank) statement</returns>
		public string TidyStatement(string statement) {
			//task.factory.startnew
			var ss = new StreamReader(statement, Encoding.GetEncoding(ReadEncoding));
			var sourceStatementFileContent = ss.ReadToEnd();
			var tmc = new TidyMessageCollection();
			var input = new MemoryStream();
			var output = new MemoryStream();
			var bytes = Encoding.GetEncoding(ReadEncoding).GetBytes(sourceStatementFileContent.Replace('£', '`'));
			input.Write(bytes, 0, bytes.Length);
			input.Position = 0;
			_tidy.Parse(input, output, tmc);
			var outputResult = Encoding.GetEncoding(ReadEncoding).GetString(output.ToArray());
			outputResult = StringUtils.InsertStringInString(outputResult, @"<script", @"<!--", true);
			return StringUtils.InsertStringInString(outputResult, @"</script>", @"-->", false);
		}

		private static void SetParameters(Tidy tidy) {
			tidy.Options.DocType = DocType.Omit;
			tidy.Options.Xhtml = true;
			tidy.Options.XmlOut = true;
			tidy.Options.CharEncoding = CharEncoding.Latin1;
			tidy.Options.QuoteNbsp = false;
			//tidy.Options.DropFontTags = true;
			//tidy.Options.LogicalEmphasis = true;
			//tidy.Options.MakeClean = true;
			//tidy.Options.TidyMark = false;
			//tidy.Options.NumEntities = true;
			//tidy.Options.NumEntities = false;
		}
	}
}
