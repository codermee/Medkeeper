using System;
using System.IO.IsolatedStorage;
using System.Linq;
using Medkeeper.Resources;
using Microsoft.Phone.Shell;

namespace Medkeeper.Classes
{
	public class TileUtility
	{

		#region Singleton

		private static TileUtility instance;
		public static TileUtility Instance
		{
			get { return instance ?? (instance = new TileUtility()); }
		}

		#endregion

		#region Constructor

		private TileUtility()
		{
			// Empty constructor
		}

		#endregion

		#region Public methods

		public bool GetLiveTileSetting()
		{
			bool isEnabled;
			var settings = IsolatedStorageSettings.ApplicationSettings;

			if (settings.Contains(Globals.SettingLiveTile))
			{
				isEnabled = (bool)settings[Globals.SettingLiveTile];
			}
			else
			{
				isEnabled = true;
			}

			return isEnabled;
		}

		public void RemoveTile(Item item)
		{
			var foundTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("guid=" + item.Guid));

			if (foundTile != null)
			{
				foundTile.Delete();
			}
		}

		public void CreateTile(Item item)
		{
			var foundTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("guid=" + item.Guid));

			if (foundTile == null)
			{
				//var liveTileUpdatesEnabled = GetLiveTileSetting();

				//if (liveTileUpdatesEnabled)
				//{
				//	CreatePeriodicTask(Globals.LiveTileUpdaterPeriodicTaskName);
				//}

				var tile = SetupTileData(item);

				ShellTile.Create(new Uri(String.Format(Globals.DetailsPageUri, item.Guid), UriKind.Relative), tile, true);
			}
		}

		public void UpdateTile(Item item)
		{
			var foundTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("guid=" + item.Guid));

			if (foundTile != null)
			{
				var tile = SetupTileData(item);

				foundTile.Update(tile);
			}
		}

		//public void CreatePeriodicTask(string name)
		//{
		//	var task = new PeriodicTask(name)
		//	{
		//		Description = "Live tile updater for Ontop",
		//		ExpirationTime = DateTime.Now.AddDays(14)
		//	};

		//	// If the agent is already registered, remove it and then add it again (further down)
		//	RemovePeriodicTask(task.Name);

		//	// Not supported in current version
		//	//task.BeginTime = DateTime.Now.AddSeconds(10);

		//	try
		//	{
		//		// Can only be called when application is running in foreground
		//		ScheduledActionService.Add(task);
		//	}
		//	catch (InvalidOperationException exception)
		//	{
		//		if (exception.Message.Contains("BNS Error: The action is disabled"))
		//		{
		//			MessageBox.Show(AppResources.BackgroundAgentsDisabled);
		//		}
		//		if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
		//		{
		//			// No user action required.
		//			// The system prompts the user when the hard limit of periodic tasks has been reached.
		//		}
		//	}

		//	//#if DEBUG
		//	//ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(60));
		//	//#endif
		//}

		//public void RemovePeriodicTask(string name)
		//{
		//	if (ScheduledActionService.Find(name) != null)
		//	{
		//		ScheduledActionService.Remove(name);
		//	}
		//}

		#endregion

		#region Private methods

		private static FlipTileData SetupTileData(Item item)
		{
			var tile = new FlipTileData
				{
					SmallBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileSmall.png", UriKind.Relative),
					WideBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileLarge.png", UriKind.Relative),
					BackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileMedium.png", UriKind.Relative),
					Title = AppResources.ApplicationTitleLiveTile,
					BackTitle = item.Name,
					BackContent = item.FriendlyDate != String.Empty ? AppResources.LatestIntake + item.FriendlyDate : AppResources.EmptyIntake,
					WideBackContent = item.FriendlyDate != String.Empty ? AppResources.LatestIntake + item.FriendlyDate : AppResources.EmptyIntake
				};
			return tile;
		}

		#endregion
	}
}