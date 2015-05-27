using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Coding4Fun.Toolkit.Controls;
using Medkeeper.Classes;
using Medkeeper.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Medkeeper
{
	public partial class MainPage
	{
		public Item SelectedItem { get; set; }

		public MainPage()
		{
			InitializeComponent();
			SetupApplicationBar();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			App.ViewModel.LoadData();
			DataContext = App.ViewModel.ItemList;

			while (NavigationService.CanGoBack)
			{
				NavigationService.RemoveBackEntry();
			}
		}

		private void SetupApplicationBar()
		{
			var addIcon = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
			addIcon.Text = AppResources.Add;
			
			var aboutMenuItem = (ApplicationBarMenuItem)ApplicationBar.MenuItems[0];
			aboutMenuItem.Text = AppResources.About;
		}

		#region Events

		private void OnHistoryListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// If selected index is not -1 (no selection)
			if (HistoryListBox.SelectedIndex != -1)
			{
				var item = HistoryListBox.SelectedItem as Item;
				if (item != null)
				{
					var detailsUri = String.Format(Globals.HistoryPageUri, item.Guid);
					var common = Common.Instance;
					common.NavigateToUrl(detailsUri);
				}
			}

			// Reset selected index to -1 (no selection)
			ItemListBox.SelectedIndex = -1;
		}

		private void OnItemListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// If selected index is not -1 (no selection)
			if (ItemListBox.SelectedIndex != -1)
			{
				var item = ItemListBox.SelectedItem as Item;
				if (item != null)
				{
					var detailsUri = String.Format(Globals.DetailsPageUri, item.Guid);
					var common = Common.Instance;
					common.NavigateToUrl(detailsUri);
				}
			}

			// Reset selected index to -1 (no selection)
			ItemListBox.SelectedIndex = -1;
		}

		private void OnPinToStartMenuItemLoaded(object sender, RoutedEventArgs e)
		{
			var foundTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("guid=" + SelectedItem.Guid));
			if (foundTile != null)
			{
				((MenuItem)sender).IsEnabled = false;
			}
		}

		private void GestureListener_Hold(object sender, GestureEventArgs e)
		{
			// sender is the StackPanel in this example 
			var item = ((Grid)sender).DataContext;

			// item has the type of the model
			SelectedItem = item as Item;
		}

		#region App bar

		private void OnAppBarAboutMenuItemClick(object sender, EventArgs e)
		{
			var common = Common.Instance;
			common.NavigateToUrl(Globals.AboutPageUri);
		}

		private void OnAppBarAddItemClick(object sender, EventArgs e)
		{
			var common = Common.Instance;
			common.NavigateToUrl(Globals.AddPageUri);
		}

		#endregion

		#region Context menu

		private void OnContextMenuPinToStartClick(object sender, RoutedEventArgs e)
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
				tileUtility.CreateTile(SelectedItem);
			}
		}

		private void OnContextMenuEditClick(object sender, RoutedEventArgs e)
		{
			var common = Common.Instance;
			common.NavigateToUrl(String.Format(Globals.EditAddPageUri, SelectedItem.Guid));
		}

		private void OnContextMenuDeleteClick(object sender, RoutedEventArgs e)
		{
			var tileUtility = TileUtility.Instance;
			var common = Common.Instance;
			if (SelectedItem != null)
			{
				var item = (from i in App.ViewModel.ItemList where i.Guid == SelectedItem.Guid select i).FirstOrDefault();

				var result = MessageBox.Show(AppResources.AreYouSure, AppResources.DeleteMessageBoxTitle, MessageBoxButton.OKCancel);
				if (result == MessageBoxResult.OK)
				{
					tileUtility.RemoveTile(item);
					App.ViewModel.Delete(item);
				}

				App.ViewModel.LoadData();
			}
			else
			{
				common.ShowErrorMessage();
			}
		}

		#endregion

		#endregion
	}
}