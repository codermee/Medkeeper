using System;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using Coding4Fun.Toolkit.Controls;
using Medkeeper.Classes;
using Medkeeper.Resources;
using Microsoft.Phone.Shell;

namespace Medkeeper.Views
{
	public partial class DetailsPage
	{
		private Item CurrentItem { get; set; }

		public DetailsPage()
		{
			InitializeComponent();
		}

		#region Private methods

		private void SetupApplicationBar()
		{
			var deleteIcon = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
			deleteIcon.Text = AppResources.Delete;

			var editIcon = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
			editIcon.Text = AppResources.Edit;

			var pinIcon = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
			pinIcon.Text = AppResources.PinToStart;

			var deleteIntakeMenuItem = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];
			deleteIntakeMenuItem.Text = AppResources.DeleteLatestIntake;

			//var editIntakeMenuItem = (ApplicationBarMenuItem) ApplicationBar.MenuItems[1];
			//editIntakeMenuItem.Text = AppResources.EditLatestIntake;

			var foundTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("guid=" + CurrentItem.Guid));
			if (foundTile != null)
			{
				pinIcon.IsEnabled = false;
			}
		}

		private string GetFromQuerystring()
		{
			string querystring;
			NavigationContext.QueryString.TryGetValue("guid", out querystring);
			return querystring;
		}

		#endregion

		#region Events

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			var guid = GetFromQuerystring();
			if (!String.IsNullOrEmpty(guid))
			{
				CurrentItem = (from v in App.ViewModel.ItemList where v.Guid == guid select v).FirstOrDefault();
				DataContext = CurrentItem;
			}
			SetupApplicationBar();
		}

		private void OnAppBarPinIconClick(object sender, EventArgs e)
		{
			var isLowMemoryDevice = (bool)IsolatedStorageSettings.ApplicationSettings[Globals.SettingIsLowMemoryDevice];
			if (isLowMemoryDevice)
			{
				var toast = new ToastPrompt
					{
						Title = AppResources.NoSupportForBackgroundAgents,
						TextWrapping = TextWrapping.Wrap,
						MillisecondsUntilHidden = 2000,
						Height = 150
					};
				toast.Show();
			}
			else
			{
				var tileUtility = TileUtility.Instance;
				tileUtility.CreateTile(CurrentItem);
			}
		}

		private void OnAppBarEditIconClick(object sender, EventArgs e)
		{
			var editUri = String.Format(Globals.EditAddPageUri, CurrentItem.Guid);
			Common.Instance.NavigateToUrl(editUri);
		}

		private void OnAppBarDeleteIconClick(object sender, EventArgs e)
		{
			var result = MessageBox.Show(AppResources.AreYouSure, AppResources.DeleteMessageBoxTitle, MessageBoxButton.OKCancel);
			if (result == MessageBoxResult.OK)
			{
				TileUtility.Instance.RemoveTile(CurrentItem);
				App.ViewModel.Delete(CurrentItem);
				Common.Instance.NavigateToUrl(Globals.MainPageUri);
			}
		}

		//private void OnAppBarEditIntakeMenuItemClick(object sender, EventArgs e)
		//{
		//	var editUri = String.Format(Globals.EditAddPageUri, CurrentItem.Guid);
		//	Common.Instance.NavigateToUrl(editUri);
		//}

		private void OnAppBarDeleteIntakeMenuItemClick(object sender, EventArgs e)
		{
			if (CurrentItem.LatestIntake.HasValue)
			{
				var result = MessageBox.Show(AppResources.AreYouSure, AppResources.DeleteLatestIntakeMessageBoxTitle, MessageBoxButton.OKCancel);
				if (result == MessageBoxResult.OK)
				{
					if (CurrentItem.History.Count > 0 && CurrentItem.HistoryFriendly.Count > 0)
					{
						var historyIntake = CurrentItem.History.FirstOrDefault();
						if (historyIntake.Month == DateTime.Now.Month && historyIntake.Day == DateTime.Now.Day)
						{
							var time = historyIntake.ToString(CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern);
							CurrentItem.UiDate = String.Format("{0} {1}", AppResources.Today, time);
							CurrentItem.LatestIntake = historyIntake;
							CurrentItem.History.RemoveAt(CurrentItem.History.Count - 1);
							CurrentItem.HistoryFriendly.RemoveAt(CurrentItem.HistoryFriendly.Count - 1);
						}
						else if (historyIntake.Month == DateTime.Now.Month && historyIntake.Day == DateTime.Now.AddDays(-1).Day)
						{
							var time = historyIntake.ToString(CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern);
							CurrentItem.UiDate = String.Format("{0} {1}", AppResources.Yesterday, time);
							CurrentItem.LatestIntake = historyIntake;
							CurrentItem.History.RemoveAt(CurrentItem.History.Count - 1);
							CurrentItem.HistoryFriendly.RemoveAt(CurrentItem.HistoryFriendly.Count - 1);
						}
						else
						{
							CurrentItem.UiDate = CurrentItem.HistoryFriendly.FirstOrDefault();
							CurrentItem.LatestIntake = historyIntake;
							CurrentItem.History.RemoveAt(CurrentItem.History.Count - 1);
							CurrentItem.HistoryFriendly.RemoveAt(CurrentItem.HistoryFriendly.Count - 1);
						}
					}
					else
					{
						CurrentItem.UiDate = String.Empty;
						CurrentItem.LatestIntake = null;
						CurrentItem.FriendlyDate = String.Empty;

					}
					App.ViewModel.Save(CurrentItem);
					TileUtility.Instance.UpdateTile(CurrentItem);
					var toast = new ToastPrompt
						{
							Title = AppResources.IntakeRemoved,
							TextWrapping = TextWrapping.Wrap,
							MillisecondsUntilHidden = 2000,
							Height = 150
						};
					toast.Show();
				}
			}
			else
			{
				var toast = new ToastPrompt
					{
						Title = AppResources.NoIntakesToRemove,
						TextWrapping = TextWrapping.Wrap,
						MillisecondsUntilHidden = 2000,
						Height = 150
					};
				toast.Show();
			}
		}

		private void OnMarkIntakeButtonClick(object sender, RoutedEventArgs e)
		{
			if (CurrentItem.LatestIntake.HasValue)
			{
				if (CurrentItem.History.Count == 20 && CurrentItem.HistoryFriendly.Count == 20)
				{
					CurrentItem.History.RemoveAt(0);
					CurrentItem.HistoryFriendly.RemoveAt(0);
				}
				CurrentItem.History.Add(CurrentItem.LatestIntake.Value);
				CurrentItem.HistoryFriendly.Add(CurrentItem.FriendlyDate);
			}
			CurrentItem.LatestIntake = DateTime.Now;
			var time = CurrentItem.LatestIntake.Value.ToString(CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern);
			CurrentItem.UiDate = String.Format("{0} {1}", AppResources.Today, time);
			CurrentItem.FriendlyDate = Common.Instance.GetFriendlyDate(CurrentItem.LatestIntake);
			App.ViewModel.Save(CurrentItem);
			TileUtility.Instance.UpdateTile(CurrentItem);
			var toast = new ToastPrompt
				{
					Title = AppResources.IntakeSaved,
					TextWrapping = TextWrapping.Wrap,
					MillisecondsUntilHidden = 2000,
					Height = 150
				};
			toast.Show();
		}

		#endregion
	}
}