using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Linq;
using Medkeeper.Classes;
using Medkeeper.Resources;

namespace Medkeeper.ViewModels
{
	public class MainViewModel : INotifyPropertyChanged
	{

		private ObservableCollection<Item> itemList;
		public ObservableCollection<Item> ItemList
		{
			get { return itemList; }
			set
			{
				itemList = value;
				NotifyPropertyChanged("ItemList");
			}
		}

		public MainViewModel()
		{
			ItemList = new ObservableCollection<Item>();
		}

		public void LoadData()
		{
			var settings = IsolatedStorageSettings.ApplicationSettings;

			var items = (from setting in settings
						 where settings.Any()
						 where !setting.Key.StartsWith(Globals.SettingPrefix)
						 select setting.Value as Item).ToList();

			if (ItemList.Count.Equals(0) && !items.Any() && CultureInfo.CurrentUICulture.ToString().Equals("sv-SE"))
			{
				var itemOne = new Item
					{
						Guid = Guid.NewGuid().ToString(),
						Name = Globals.Ddrops,
						Description = Globals.DdropsDescription,
						Dose = Globals.DdropsDose
					};
				ItemList.Add(itemOne);
				settings.Add(itemOne.Guid, itemOne);

				var itemTwo = new Item
					{
						Guid = Guid.NewGuid().ToString(),
						Name = Globals.Semper,
						Description = Globals.SemperDescription,
						Dose = Globals.SemperDose
					};
				ItemList.Add(itemTwo);
				settings.Add(itemTwo.Guid, itemTwo);
			}
			else
			{
				ItemList.Clear();
				foreach (var item in items.Where(item => !item.MarkedDeleted))
				{
					var latestIntake = item.LatestIntake.HasValue ? item.LatestIntake.Value : DateTime.MinValue;
					var time = latestIntake.ToString(CultureInfo.CurrentUICulture.DateTimeFormat.ShortTimePattern);

					if (latestIntake.Month == DateTime.Now.Month && latestIntake.Day == DateTime.Now.Day)
					{
						item.UiDate = String.Format("{0} {1}", AppResources.Today, time);
					}
					else if (latestIntake.Month == DateTime.Now.AddDays(-1).Month && latestIntake.Day == DateTime.Now.AddDays(-1).Day)
					{
						item.UiDate = String.Format("{0} {1}", AppResources.Yesterday, time);
					}
					else if (latestIntake != DateTime.MinValue)
					{
						item.UiDate = item.FriendlyDate;
					}

					ItemList.Add(item);
				}
			}
		}

		public void Save(Item item)
		{
			var settings = IsolatedStorageSettings.ApplicationSettings;
			if (settings.Contains(item.Guid))
			{
				settings[item.Guid] = item;
			}
			else
			{
				settings.Add(item.Guid, item);
			}
			settings.Save();
		}

		public void Delete(Item item)
		{
			var settings = IsolatedStorageSettings.ApplicationSettings;
			if (settings.Contains(item.Guid))
			{
				if (item.Name == Globals.Ddrops || item.Name == Globals.Semper)
				{
					item.MarkedDeleted = true;
				}
				else
				{
					settings.Remove(item.Guid);
				}
			}
			settings.Save();
		}

		public void RemoveIntake(Item item)
		{
			Delete(item);
			Save(item);
		}

		#region Eventhandlers

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

	}
}