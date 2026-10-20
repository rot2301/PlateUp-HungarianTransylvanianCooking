global using static KitchenLib.Utils.GDOUtils;
global using static KitchenLib.Utils.LocalisationUtils;
global using static HungarianTransylvanianCooking.Main;
using KitchenData;
using KitchenLib;
using KitchenLib.Interfaces;
using KitchenMods;
using System.Reflection;
using UnityEngine;

namespace HungarianTransylvanianCooking
{
	// IAutoRegisterAll makes KitchenLib register every CustomGameDataObject in this assembly,
	// so new dishes/items only need a class - no list to keep in sync here.
	public class Main : BaseMod, IAutoRegisterAll
	{
		public const string MOD_GUID = "ruben.hungariantransylvaniancooking";
		public const string MOD_NAME = "Hungarian-Transylvanian Cooking";
		public const string MOD_AUTHOR = "Ruben";
		public const string MOD_VERSION = "0.1.0";
		public const string MOD_GAMEVERSION = ">=1.1.0";

		public Main() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

		protected override void OnPostActivate(Mod mod)
		{
			LogInfo($"v{MOD_VERSION} loaded");
#if DEBUG
			LogInfo("Debug build: dumping vanilla data and unlocking all lángos cards in lángos runs");
			Debugging.GameDataDump.Register();
#endif
		}

		public static T GetGDO<T>(int id) where T : GameDataObject => GetExistingGDO(id) as T;

		internal static void LogInfo(string msg) => Debug.Log($"[{MOD_GUID}] {msg}");
		internal static void LogWarning(string msg) => Debug.LogWarning($"[{MOD_GUID}] {msg}");
	}
}
