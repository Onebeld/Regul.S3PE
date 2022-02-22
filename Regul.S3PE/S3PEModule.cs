using System.Collections.Generic;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Regul.Base;
using Regul.ModuleSystem;
using Regul.ModuleSystem.Models;

namespace Regul.S3PE
{
	public class S3PEModule : IModule
	{
		public string Name { get; } = "S3PE";
		public string Creator { get; } = "Onebeld";
		public string Description { get; } = "(ALPHA) Editor for The Sims 3 package-files";
		public string Version { get; } = "0.2.0.0";
		public bool CorrectlyInitialized { get; set; }
		public bool ThereIsAnUpdate { get; set; }
		public IImage Icon { get; set; } = new Bitmap(AvaloniaLocator.Current.GetService<IAssetLoader>().Open(new Uri("avares://Regul.S3PE/icon.png")));

		private const string S3PEId = "Onebeld_Editor_S3PE_938fsWb42M";

		public void Execute()
		{
			Application.Current.Styles.Add(new StyleInclude(new Uri("resm:Style?assembly=Regul"))
			{
				Source = new Uri("avares://Regul.S3PE/S3PEStyles.axaml")
			});

			ModuleManager.Editors.Add(
				new Editor
				{
					Id = S3PEId,
					Name = "The Sims 3 Package Editor",
					CreatingAnEditor = () => new S3PE(),
					Icon = App.GetResource<PathGeometry>("TheSims3Icon"),
					DialogFilters = new List<FileDialogFilter>
					{
						new FileDialogFilter {Name = "The Sims 3 DBPF Packages", Extensions = {"package", "nhd"}}
					}
				});
		}

		public Language[] Languages { get; } = new Language[]
		{
			new Language("English", "en"),
			new Language("Russian", "ru"),
		};
		
		public IStyle LanguageStyle { get; set; }

		public string PathToLocalization { get; } = "Regul.S3PE/Localization/";
		public string LinkForCheckUpdates { get; } = "https://raw.githubusercontent.com/Onebeld/Regul.S3PE/main/module.txt";
	}
}