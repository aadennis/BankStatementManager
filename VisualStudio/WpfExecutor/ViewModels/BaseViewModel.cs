using System;
using System.ComponentModel;
using System.Linq.Expressions;
using WpfExecutor.Annotations;

namespace WpfExecutor.ViewModels {

	/// <summary>
	/// Base view model to implement common needs, such as INPC handlers
	/// </summary>
	public class BaseViewModel : INotifyPropertyChanged   {

		/// <summary>
		/// Event handler delegate
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Validates that property name corresponds to a valid type, and passes through to the handler to alert subscribers
		/// </summary>
		/// <typeparam name="T">the return type of the property</typeparam>
		/// <param name="propertyName">the name of the property</param>
		protected void OnPropertyChanged<T>(Expression<Func<T>> propertyName) {
			var body = propertyName.Body as MemberExpression;
			if (body == null) {
				throw new NullReferenceException(string.Format("[{0}] is not a valid property name", propertyName));
			}
			OnPropertyChanged(body.Member.Name);
		}

		/// <summary>
		/// Alerts subscribers to the property change event
		/// </summary>
		/// <param name="propertyName">the name of the property</param>
		[NotifyPropertyChangedInvocator]
		protected void OnPropertyChanged(string propertyName) {
			var handler = PropertyChanged;
			if (handler != null) {
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
