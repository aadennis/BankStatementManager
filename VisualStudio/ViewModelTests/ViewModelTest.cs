using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfExecutor.ViewModels;
using Moq;
using Wells.StatementManagement;

namespace ViewModelTests {
	[TestClass]
	public class ViewModelTest {

		[TestMethod]
		public void ConvertButtonVisibilityIsFalseWhenSourceAndTargetFoldersAreEmpty() {
			var vm = new DialogVm();
			Assert.AreEqual(false, vm.IsSaveButtonVisible);
			vm.ConvertedOfxDestinationFolder = string.Empty;
			vm.StatementLocation = string.Empty;
			Assert.AreEqual(false, vm.IsSaveButtonVisible);
		}

		[TestMethod]
		public void ConvertButtonVisibilityIsFalseWhenSourceFoldersIsEmpty() {
			var vm = new DialogVm {
			                      	StatementLocation = string.Empty
			                      };
			Assert.AreEqual(false, vm.IsSaveButtonVisible);
		}

		[TestMethod]
		public void ConvertButtonVisibilityIsFalseWhenTargetFoldersIsEmpty() {
			var vm = new DialogVm {
				ConvertedOfxDestinationFolder = string.Empty
			};
			Assert.AreEqual(false, vm.IsSaveButtonVisible);
		}

		[TestMethod]
		public void ConvertButtonVisibilityIsTrueWhenWhenSourceAndTargetFoldersAreNotEmpty() {
// ReSharper disable UseObjectOrCollectionInitializer
			var vm = new DialogVm();
// ReSharper restore UseObjectOrCollectionInitializer
			vm.StatementLocation = "source";
			vm.ConvertedOfxDestinationFolder = "destination";
			Assert.AreEqual(true, vm.IsSaveButtonVisible);
		}

		[TestMethod]
		public void ConvertButtonVisibilityIsTrueWhenWhenTargetAndSourceFoldersAreNotEmpty() {
// ReSharper disable UseObjectOrCollectionInitializer
			var vm = new DialogVm();
// ReSharper restore UseObjectOrCollectionInitializer
			vm.ConvertedOfxDestinationFolder = "destination";
			vm.StatementLocation = "source";
			Assert.AreEqual(true, vm.IsSaveButtonVisible);
		}

		[TestMethod]
		public void IsStatementLocationPopulatedIsFalseWhenSourceFolderIsEmpty() {
			var vm = new DialogVm();
			Assert.AreEqual(false, vm.IsStatementLocationPopulated);
			Assert.AreEqual(true, vm.IsStatementLocationEmpty);

			vm.StatementLocation = string.Empty;

			Assert.AreEqual(false, vm.IsStatementLocationPopulated);
			Assert.AreEqual(true, vm.IsStatementLocationEmpty);
		}

		[TestMethod]
		public void IsStatementLocationPopulatedIsTrueWhenSourceFolderIsPopulated() {
			var vm = new DialogVm {StatementLocation = "The statement location"};

			Assert.AreEqual(true, vm.IsStatementLocationPopulated);
			Assert.AreEqual(false, vm.IsStatementLocationEmpty);
		}

		[TestMethod]
		public void ProgramExitsWhenStatementLocationIsEmptyString() {
			var fileHandler = new Mock<IFileHandler>(MockBehavior.Strict);
			fileHandler.Setup(x => x.ValidateFileLocation(It.IsAny<string>()));
			var parser = new Mock<IHtmlParser>(MockBehavior.Strict);
			parser.Setup(x => x.TidyStatement(It.IsAny<string>())).Returns(CopyResourceToString("ViewModelTests.Resources", "ExpectedTidiedTestStatement.htm"));

			var vm = new DialogVm(fileHandler.Object, parser.Object) { StatementLocation = string.Empty };
			vm.ConvertSmileToOfxStatementForDisplay();

			fileHandler.VerifyAll();
		}

		[TestMethod]
		public void SmileStatementIsConvertedToInMemoryOfxStatement() {
			var fileHandler = new Mock<IFileHandler>(MockBehavior.Strict);
			fileHandler.Setup(x => x.ValidateFileLocation(It.IsAny<string>()));
			var parser = new Mock<IHtmlParser>(MockBehavior.Strict);
			parser.Setup(x => x.TidyStatement(It.IsAny<string>())).Returns(CopyResourceToString("ViewModelTests.Resources", "ExpectedTidiedTestStatement.htm"));

			var vm = new DialogVm(fileHandler.Object, parser.Object) { StatementLocation = "The statement location" };
			vm.ConvertSmileToOfxStatementForDisplay();

			fileHandler.VerifyAll();
			Assert.AreEqual(5, vm.GetTransactions.Count);
			Assert.AreEqual("12344321", vm.AccountCode, "Account Code");
			Assert.AreEqual("084456", vm.SortCode, "Sort Code");
			Assert.AreEqual("04/04/2010", vm.StatementDate, "Statement Date");
		}

		// todo - currently have to copy this around, as using a common version does not work
		/// <summary>
		/// Copy a resource from the current project to a string
		/// </summary>
		/// <param name="resourceFolder">The folder name where the resource lives</param>
		/// <param name="resourceName">The resource name</param>
		/// <returns>The content of the resource, converted to a string</returns>
		private static string CopyResourceToString(string resourceFolder, string resourceName) {
			var srcStatementStream =
				Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format(@"{0}.{1}", resourceFolder, resourceName));
			if (srcStatementStream == null) throw new ArgumentNullException(string.Format("[{0}] not found", resourceName));
			return new StreamReader(srcStatementStream, Encoding.GetEncoding(Utilities.FileManagementUtils.ReadEncoding)).ReadToEnd();
		}
	}
}
