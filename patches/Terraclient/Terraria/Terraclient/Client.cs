#region License
// Copyright (C) 2021 Tomat and Contributors
// GNU General Public License Version 3, 29 June 2007
#endregion

using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace Terraria.Terraclient
{
	/// <summary>
	///		Terraclient stuff.
	/// </summary>
	public static class Client
	{
		public static Version TerraclientVersion => BuildInfo.TerraclientVersion;

		public static bool ConnectToVanillaServers;
		public static bool PoseAsVanilla;

		internal static void SaveConfiguration() {
			Main.Configuration.Put(nameof(ConnectToVanillaServers), ConnectToVanillaServers);
			Main.Configuration.Put(nameof(PoseAsVanilla), PoseAsVanilla);
		}

		internal static void LoadConfiguration() {
			Main.Configuration.Get(nameof(ConnectToVanillaServers), ref ConnectToVanillaServers);
			Main.Configuration.Get(nameof(PoseAsVanilla), ref PoseAsVanilla);

			ModNet.AllowVanillaClients = ConnectToVanillaServers;
		}

		internal static void PostSaveConfiguration() {
			if (!ConnectToVanillaServers)
				return;

			// ModBiome doesn't matter.
			// Boss Bars are client-side and never checked by servers.
			// ModCommands don't matter.
			// ModDusts are client-side.
			// ModGores are client-side.
			// ModKeybinds are typically client-side.
			// ModMenus are client-side.
			// ModPlayers are permissible.
			// ModSceneEffect doesn't matter.
			// ModSound doesn't matter.
			// ModSystem doesn't matter.
			// TODO: Might need to check for tile entities.
			// TODO: This might as well always be true if ModNet disallowed vanilla clients due to ModLoaderMod.
			if (ItemLoader.ItemCount > ItemID.Count 
			    || NPCLoader.NPCCount > NPCID.Count 
			    || ProjectileLoader.ProjectileCount > ProjectileID.Count 
			    || WallLoader.WallCount > WallID.Count 
			    || BuffLoader.BuffCount > BuffID.Count
			    || MountLoader.MountCount > MountID.Count
			    || PrefixLoader.PrefixCount > PrefixID.Count
			    || RarityLoader.RarityCount > ItemRarityID.Count
			    || TileLoader.TileCount > TileID.Count) 
				ModLoader.ModLoader.Reload();
		}
	}
}