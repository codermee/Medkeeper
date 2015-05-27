using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using Coding4Fun.Toolkit.Controls;
using Medkeeper.Classes;
using Medkeeper.Resources;
using Microsoft.Phone.Shell;

namespace Medkeeper.Views
{
	public partial class EditPage
	{
		private Item CurrentItem { get; set; }

		public EditPage()
		{
			InitializeComponent();
			SetupApplicationBar();
		}

		private void SetupApplicationBar()
		{
			var saveIcon = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
			saveIcon.Text = AppResources.Save;

			var cancelIcon = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
			cancelIcon.Text = AppResources.Cancel;
		}

		private string GetFromQuerystring()
		{
			string querystring;
			NavigationContext.QueryString.TryGetValue("guid", out querystring);
			NavigationContext.QueryString.Clear();
			return querystring;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			CurrentItem = new Item();
			var guid = GetFromQuerystring();
			if (!String.IsNullOrEmpty(guid))
			{
				PageTitle.Text = AppResources.Edit;
				CurrentItem = (from i in App.ViewModel.ItemList where i.Guid == guid select i).FirstOrDefault();
			}

			DataContext = CurrentItem;
		}

		private void OnAppBarSaveIconClick(object sender, EventArgs e)
		{
			DescTextBox.Focus();
			if (CurrentItem.Name != null)
			{
				if (CurrentItem.Guid == null || CurrentItem.Guid == "00000000-0000-0000-0000-000000000000")
				{
					CurrentItem.Guid = Guid.NewGuid().ToString();
				}
				App.ViewModel.Save(CurrentItem);
				TileUtility.Instance.UpdateTile(CurrentItem);
				Common.Instance.NavigateToUrl(Globals.MainPageUri);
			}
			else
			{
				var toast = new ToastPrompt
					{
						Title = AppResources.MissingItemName,
						TextWrapping = TextWrapping.Wrap,
						MillisecondsUntilHidden = 2000,
						Height = 150
					};
				toast.Show();
			}
		}

		private void OnAppBarCancelIconClick(object sender, EventArgs e)
		{
			var common = Common.Instance;
			common.NavigateToUrl(Globals.MainPageUri);
		}

		//private void OnDatePickerValueChanged(object sender, DateTimeValueChangedEventArgs e)
		//{
		//	throw new NotImplementedException();
		//	if (DatePicker.Value.HasValue)
		//	{
				
		//	}
		//}

		//private void OnTimePickerValueChanged(object sender, DateTimeValueChangedEventArgs e)
		//{
		//	throw new NotImplementedException();
		//	if (TimePicker.Value.HasValue)
		//	{
				
		//	}
		//}
	}
}