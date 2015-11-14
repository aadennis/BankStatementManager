using System;

namespace Utilities {

	/// <summary>
	/// Get and set values on objects
	/// </summary>
	public static class ObjectUtilities {

		/// <summary>
		/// sets the value of the passed type property
		/// </summary>
		/// <param name="type">object type</param>
		/// <param name="propertyName">a property of the passed type</param>
		/// <param name="propertyValue">the value to which the property is to be set</param>
		public static void SetPropertyValue(object type, string propertyName, string propertyValue) {
			var propertyInfo = type.GetType().GetProperty(propertyName);
			if (propertyInfo == null) {
				throw new NullReferenceException(string.Format("Property [{0}] is not valid", propertyName));
			}
			propertyInfo.SetValue(type, propertyValue, null);
		}
	}
}
