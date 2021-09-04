using Mono.Cecil;
using MonoMod;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.IO;
using System.Windows;
using static Terraria.ModLoader.Setup.Program;

namespace Terraria.ModLoader.Setup
{
	internal class HookGenTask : SetupOperation
	{
		private const string libsPath = "src/Terraclient/Terraria/Libraries";
		private const string binLibsPath = "src/Terraclient/Terraria/bin/Debug/net5.0/Libraries";
		private const string terraclientAssemblyPath = @"src/Terraclient/Terraria/bin/Debug/net5.0/Terraclient.dll";
		private const string installedNetRefs = @"\dotnet\packs\Microsoft.NETCore.App.Ref\5.0.0\ref\net5.0";

		public HookGenTask(ITaskInterface taskInterface) : base(taskInterface)
		{
		}

		public override void Run()
		{
			if (!File.Exists(terraclientAssemblyPath)) {
				MessageBox.Show($"\"{terraclientAssemblyPath}\" does not exist.", "Terraclient exe not found", MessageBoxButton.OK);
				taskInterface.SetStatus("Cancelled");
				return;
			}

			string outputPath = Path.Combine(libsPath, "Common", "TerrariaHooks.dll");

			if (File.Exists(outputPath))
				File.Delete(outputPath);

			taskInterface.SetStatus($"Hooking: Terraclient.dll -> TerrariaHooks.dll");

			HookGen(terraclientAssemblyPath, outputPath);

			File.Delete(Path.ChangeExtension(outputPath, "pdb"));

			MessageBox.Show("Success. Make sure you diff tModLoader after this");
		}

		public static void HookGen(string inputPath, string outputPath)
		{
			using var mm = new MonoModder {
				InputPath = inputPath,
				OutputPath = outputPath,
				ReadingMode = ReadingMode.Deferred,

				DependencyDirs = {
					Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + installedNetRefs
				},
				MissingDependencyThrow = false,
			};

			mm.DependencyDirs.AddRange(Directory.GetDirectories(binLibsPath, "*", SearchOption.AllDirectories));

			mm.Read();
			mm.MapDependencies();

			var gen = new HookGenerator(mm, "TerrariaHooks") {
				HookPrivate = true,
			};
			gen.Generate();
			RemoveModLoaderTypes(gen.OutputModule);
			gen.OutputModule.Write(outputPath);
		}

		private static void RemoveModLoaderTypes(ModuleDefinition module)
		{
			return;
			for (int i = module.Types.Count - 1; i >= 0; i--)
				if (module.Types[i].FullName.Contains("Terraria.ModLoader"))
					module.Types.RemoveAt(i);
		}
	}
}