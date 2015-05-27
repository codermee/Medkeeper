using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Medkeeper.Classes
{
	public class Item : INotifyPropertyChanged
	{
		public string Guid { get; set; }

		private List<DateTime> history;
		public List<DateTime> History
		{
			get { return history ?? (history = new List<DateTime>()); }
			set
			{
				history = value;
				NotifyPropertyChanged("History");
			}
		}

		private List<string> historyFriendly;
		public List<string> HistoryFriendly
		{
			get { return historyFriendly ?? (historyFriendly = new List<string>()); } 
			set
			{
				historyFriendly = value;
				NotifyPropertyChanged("HistoryFriendly");
			}
		}

		private bool markedDeleted;
		public bool MarkedDeleted
		{
			get { return markedDeleted; }
			set
			{
				markedDeleted = value;
				NotifyPropertyChanged("MarkedDeleted");
			}
		}

		private string name;
		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				NotifyPropertyChanged("Name");
			}
		}

		private string dose;
		public string Dose
		{
			get { return dose; }
			set
			{
				dose = value;
				NotifyPropertyChanged("Dose");
			}
		}

		private string description;
		public string Description
		{
			get { return description; }
			set
			{
				description = value;
				NotifyPropertyChanged("Description");
			}
		}

		private DateTime? latestIntake;
		public DateTime? LatestIntake
		{
			get { return latestIntake; }
			set
			{
				latestIntake = value;
				NotifyPropertyChanged("LatestIntake");
			}
		}

		private string friendlyDate;
		public string FriendlyDate
		{
			get { return friendlyDate; }
			set
			{
				friendlyDate = value;
				NotifyPropertyChanged("FriendlyDate");
			}
		}

		private string uiDate;
		public string UiDate
		{
			get { return uiDate; }
			set
			{
				uiDate = value;
				NotifyPropertyChanged("UiDate");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}