using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Utilities {
	public class FileManagementUtils {
		public const string ReadEncoding = "iso-8859-1";

		//todo - how to refer to an assembly resource from another assembly
		/// <summary>
		/// Copy a resource to a folder on the file system
		/// </summary>
		/// <param name="targetFolder">Location to save the copied resource</param>
		/// <param name="resourceFolder">VS folder containing the resource</param>
		/// <param name="resourceName">resource name</param>
		public static void CopyResourceToFile(string targetFolder, string resourceFolder, string resourceName) {
			// todo - refactor to use CopyResourceToString
			var srcStatementStream =
				Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format(@"{0}.{1}", resourceFolder, resourceName));
			if (srcStatementStream == null) throw new ArgumentNullException(string.Format("[{0}] not found", resourceName));
			var ss = new StreamReader(srcStatementStream, Encoding.GetEncoding(ReadEncoding));
			var srcFilePathAndName = targetFolder + @"\" + resourceName;
			File.WriteAllText(srcFilePathAndName, ss.ReadToEnd(), Encoding.GetEncoding(ReadEncoding));
		}

		/// <summary>
		/// Copy a resource from the current project to a string
		/// </summary>
		/// <param name="resourceFolder">The folder name where the resource lives</param>
		/// <param name="resourceName">The resource name</param>
		/// <returns>The content of the resource, converted to a string</returns>
		public static string CopyResourceToString(string resourceFolder, string resourceName) {
			var resourceNames =
			    Assembly.GetExecutingAssembly().GetManifestResourceNames();
			var srcStatementStream =
				Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format(@"{0}.{1}", resourceFolder, resourceName));
			if (srcStatementStream == null) throw new ArgumentNullException(string.Format("[{0}] not found", resourceName));
			return new StreamReader(srcStatementStream, Encoding.GetEncoding(ReadEncoding)).ReadToEnd();
		}
	}
}
