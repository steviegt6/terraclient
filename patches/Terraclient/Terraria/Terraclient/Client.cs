#region License
// Copyright (C) 2021 Tomat and Contributors
// GNU General Public License Version 3, 29 June 2007
#endregion

using System;
using Terraria.ModLoader;

namespace Terraria.Terraclient
{
	/// <summary>
	///		Terraclient stuff.
	/// </summary>
	public static class Client
	{
		public static Version TerraclientVersion => BuildInfo.TerraclientVersion;
	}
}