using System.Collections.Generic;
using UnityEngine;

namespace Funzilla
{
	internal static class LevelManager
	{
		private static readonly List<string> List = new ();

		internal static List<string> Levels
		{
			get
			{
				if (List is not {Count: > 0})
				{
					LoadLevels();
				}
				return List;
			}
		}

		internal static void LoadLevels()
		{
			LoadCsv("Levels");
		}

		private static void LoadCsv(string csvFile)
		{
			List.Clear();
			var index = 0;
			CsvReader.LoadFromResource(csvFile, 
				columns => index = columns["Level"],
				cells => List.Add(cells[index])
			);
		}
	}
}