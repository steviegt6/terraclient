using ReLogic.Content.Sources;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.Assets;
using Terraria.Terraclient.Cheats;
using Terraria.Terraclient.Loading;

namespace Terraria.Terraclient
{
	internal class ClientMod : TerraclientMod
	{
		public override string Name => "Terraclient";

		public override Version Version => Client.TerraclientVersion;

		public override Preferences CheatConfigFile { get; }

		internal ClientMod() {
			Side = ModSide.NoSync;
			DisplayName = "Terraclient";
			Code = Assembly.GetExecutingAssembly();

			BackgroundAutoloadingEnabled 
				= ContentAutoloadingEnabled
					= GoreAutoloadingEnabled
						= MusicAutoloadingEnabled
							= SoundAutoloadingEnabled = false;

			CheatConfigFile = new Preferences(Path.Combine(Main.SavePath, "Terraclient", "Terraclient"));
		}

		public override void Load() {
			base.Load();

			foreach (Type type in Code.GetTypes().Where(x => x.IsSubclassOf(typeof(ModCheatCategory)))) {
				if (type.IsAbstract || type.GetCustomAttribute<TerraclientAssociatedAttribute>() is null)
					continue;

				AddContent(Activator.CreateInstance(type) as ILoadable);
			}

			foreach (Type type in Code.GetTypes().Where(x => x.IsSubclassOf(typeof(ModCheat)))) {
				if (type.IsAbstract || type.GetCustomAttribute<TerraclientAssociatedAttribute>() is null)
					continue;

				AddContent(Activator.CreateInstance(type) as ILoadable);
			}
		}

		public override IContentSource CreateDefaultContentSource() => new AssemblyResourcesContentSource(Assembly.GetExecutingAssembly(), "Terraria.Terraclient.");
	}
}