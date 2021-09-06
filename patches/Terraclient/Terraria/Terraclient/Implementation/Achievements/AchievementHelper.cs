using System.Linq;
using Terraria.Achievements;
using Terraria.Social;

namespace Terraria.Terraclient.Implementation.Achievements
{
	public static class AchievementHelper
	{
		public static void ForceUnlock(this Achievement achievement) {
			foreach (AchievementCondition condition in achievement._conditions.Values.Where(x => !x.IsCompleted))
				condition.Complete();

			SocialAPI.Achievements?.CompleteAchievement(achievement.Name);
			achievement.DoCompleted();
		}
	}
}