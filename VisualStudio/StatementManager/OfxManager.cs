using System;
using System.Linq;
using System.Xml.Linq;

namespace Wells.StatementManagement {

	public static class OfxManager {

		public static XDocument OutputOfxElements(OfxStatement ofxStatement, OfxStatementHeader statementHeader) {

			var ofxTransactions = ofxStatement.GetTransactions();

			var firstPostingDate = ofxTransactions.First().DatePosted;
			var lastPostingDate = ofxTransactions.Last().DatePosted;
			var closingBalance = 8899.22; // todo - fix this
			var serverDate = statementHeader.ServerDate;

			var ofx =
			new XElement("OFX",
				new XElement("SIGNONMSGSRSV1",
					new XElement("SONRS",
						new XElement("STATUS",
							new XElement("CODE", "0"),
							new XElement("SEVERITY", "INFO")
						),
						new XElement("DTSERVER", serverDate),
						new XElement("LANGUAGE", "ENG")
					)
				),
				new XElement("BANKMSGSRSV1",
					new XElement("STMTTRNRS",
						new XElement("TRNUID", "1"),
						new XElement("STATUS",
							new XElement("CODE","0"),
							new XElement("SEVERITY","INFO")
						),
						new XElement("STMTRS",
							new XElement("CURDEF","GBP"),
							new XElement("BANKACCTFROM",
								new XElement("BANKID",statementHeader.SortCode),
								new XElement("ACCTID",statementHeader.AccountNumber),
								new XElement("ACCTTYPE","CHECKING")),
							new XElement("BANKTRANLIST",
								new XElement("DTSTART",firstPostingDate),
								new XElement("DTEND",lastPostingDate),
						from f in ofxTransactions select 
							 new XElement("STMTTRN",
									new XElement("TRNTYPE", f.TrnType),
									new XElement("DTPOSTED", f.DatePosted),
									new XElement("TRNAMT", f.Amount),
									new XElement("FITID", f.FitId),
									new XElement("NAME", f.Name),
									new XElement("MEMO", f.Memo)
									)
							),
							new XElement("LEDGERBAL",
								new XElement("BALAMT", closingBalance),
								new XElement("DTASOF", lastPostingDate)
							),
							new XElement("AVAILBAL",
								new XElement("BALAMT", closingBalance),
								new XElement("DTASOF", lastPostingDate)
							)
						)
					)
				)
			);

			//http://msdn.microsoft.com/en-gb/library/syste.xml.linq.xdeclaration.encoding.aspx
			return new XDocument(
				new XDeclaration("1.0", "US-ASCII", "yes"),
				new XProcessingInstruction("OFX",
										   @"OFXHEADER=""200"" VERSION=""200"" SECURITY=""NONE"" OLDFILEUID=""NONE"" NEWFILEUID=""NONE"""),
				ofx
				);

		}

	}
}
