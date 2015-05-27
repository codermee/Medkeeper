using System;
using System.Linq;
using System.Windows.Navigation;
using Medkeeper.Classes;

namespace Medkeeper.Views
{
	public partial class HistoryPage
	{
		private Item CurrentItem { get; set; }

		public HistoryPage()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			var guid = GetFromQuerystring();
			if (!String.IsNullOrEmpty(guid))
			{
				CurrentItem = (from v in App.ViewModel.ItemList where v.Guid == guid select v).FirstOrDefault();
				if (CurrentItem != null)
				{
					CurrentItem.HistoryFriendly.Reverse();
					DataContext = CurrentItem;
				}
			}
		}

		private string GetFromQuerystring()
		{
			string querystring;
			NavigationContext.QueryString.TryGetValue("guid", out querystring);
			return querystring;
		}
	}
}