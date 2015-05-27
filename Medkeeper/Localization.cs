using Medkeeper.Resources;

namespace Medkeeper
{
	public class Localization
	{
		private static readonly AppResources _localizedResources = new AppResources();
		public AppResources LocalizedResources { get { return _localizedResources; } }
	}
}