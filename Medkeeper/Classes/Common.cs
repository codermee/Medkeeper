using System;
using System.Globalization;
using System.Windows;
using Coding4Fun.Toolkit.Controls;
using Medkeeper.Resources;
using Microsoft.Phone.Controls;

namespace Medkeeper.Classes
{
	public class Common
	{

		#region Singleton

		private static Common instance;
		public static Common Instance
		{
			get { return instance ?? (instance = new Common()); }
		}

		#endregion

		#region Constructor

		private Common()
		{
			// Empty constructor
		}

		#endregion

		public void ShowErrorMessage()
		{
			var toast = new ToastPrompt
				{
					Title = AppResources.ErrorMessage,
					TextWrapping = TextWrapping.Wrap,
					MillisecondsUntilHidden = 2000,
					Height = 150
				};
			toast.Show();
		}

		public void NavigateToUrl(string url)
		{
			var uri = new Uri(url, UriKind.Relative);
			((PhoneApplicationFrame)Application.Current.RootVisual).Navigate(uri);
		}

		public string GetFriendlyDate(DateTime? date)
		{
			var format = CultureInfo.CurrentUICulture.DateTimeFormat;
			
			var retVal = String.Empty;
			if (date.HasValue)
			{
				var formattedDate = date.Value.ToString(format.MonthDayPattern).Replace("MMMM", "MMM");
				var formattedTime = date.Value.ToString(format.ShortTimePattern);
				retVal = String.Format("{0} {1}", formattedDate, formattedTime);
			}
			return retVal;
		}
	}
}