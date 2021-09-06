namespace Terraria.Achievements
{
	partial class Achievement
	{
		/// <summary>
		///		Invokes <see cref="OnCompleted"/>.
		/// </summary>
		internal void DoCompleted() => OnCompleted?.Invoke(this);
	}
}