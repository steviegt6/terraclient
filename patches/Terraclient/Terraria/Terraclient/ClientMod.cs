#region License
// Copyright (C) 2021 Tomat and Contributors
// GNU General Public License Version 3, 29 June 2007
#endregion

using ReLogic.Content.Sources;
using System;
using System.Reflection;
using Terraria.ModLoader;
using Terraria.ModLoader.Assets;

namespace Terraria.Terraclient
{
	internal class ClientMod : Mod
	{
		public override string Name => "Terraclient";

		public override Version Version => Client.TerraclientVersion;

		internal ClientMod() {
			Side = ModSide.NoSync;
			DisplayName = "Terraclient";
			Code = Assembly.GetExecutingAssembly();

			BackgroundAutoloadingEnabled 
				= ContentAutoloadingEnabled
					= GoreAutoloadingEnabled
						= MusicAutoloadingEnabled
							= SoundAutoloadingEnabled = false;
		}

		public override IContentSource CreateDefaultContentSource() => new AssemblyResourcesContentSource(Assembly.GetExecutingAssembly(), "Terraria.Terraclient.");
	}
}