namespace Medkeeper.Classes
{
	public class Globals
	{
		// URIs
		public static string SettingsPageUri = "/Views/SettingsPage.xaml";
		public static string AboutPageUri = "/Views/AboutPage.xaml";
		public static string AddPageUri = "/Views/EditPage.xaml";
		public static string EditAddPageUri = "/Views/EditPage.xaml?guid={0}";
		public static string DetailsPageUri = "/Views/DetailsPage.xaml?guid={0}";
		public static string MainPageUri = "/MainPage.xaml";
		public static string HistoryPageUri = "/Views/HistoryPage.xaml?guid={0}";

		// Other
		public static string LiveTileUpdaterPeriodicTaskName = "LiveTileUpdaterForMedkeep";
		public static string SettingPrefix = "Setting_";

		// Settings
		public static string SettingLiveTile = "Setting_LiveTile";
		public static string SettingIsLowMemoryDevice = "Setting_IsLowMemoryDevice";

		public static string Ddrops = "D-droppar";
		public static string DdropsDescription = "Tillskott av D-vitamin för barn under 2 år.";
		public static string DdropsDose = "5 droppar ges oralt varje dag tills barnet är 2 år gammalt (mörkhyade barn tills de är 5 år).";
		public static string Semper = "Semper magdroppar";
		public static string SemperDescription = "Droppar vid magbesvär/maginfluensa/kolik.";
		public static string SemperDose = "5 droppar ges oralt varje dag.";
	}
}