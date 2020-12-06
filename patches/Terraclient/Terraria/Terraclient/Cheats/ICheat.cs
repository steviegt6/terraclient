namespace Terraria.Terraclient.Cheats
{
	public interface ICheat
	{
		string Name();

		string Description();

		bool IsEnabled();
	}
}
