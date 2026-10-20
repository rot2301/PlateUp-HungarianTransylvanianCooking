using KitchenData;
using KitchenLib.Customs;
using KitchenLib.References;
using System.Collections.Generic;
using UnityEngine;

namespace TransylvanianHungarianCooking.Langos
{
	/// <summary>Starting dish: plain fried lángos.</summary>
	public class LangosDish : CustomDish
	{
		public override string UniqueNameID => "Langos Dish";
		public override DishType Type => DishType.Base;
		public override UnlockGroup UnlockGroup => UnlockGroup.Dish;
		public override CardType CardType => CardType.Default;
		public override bool IsUnlockable => true;
		public override bool IsAvailableAsLobbyOption => true;
		public override DishCustomerChange CustomerMultiplier => DishCustomerChange.None;
		public override Unlock.RewardLevel ExpReward => Unlock.RewardLevel.Medium;
		public override int Difficulty => 2;
		public override Color ColourOverride => new(0.85f, 0.61f, 0.24f);

		public override List<string> StartingNameSet => new()
		{
			"Lángos Láng",
			"The Puszta Fryer",
			"Balaton Beach Bites",
			"Fokhagyma & Co.",
			"Piața Lángos"
		};

		public override List<Dish.MenuItem> ResultingMenuItems => new()
		{
			new()
			{
				Item = GetCastedGDO<Item, PlatedLangos>(),
				Phase = MenuPhase.Main,
				Weight = 1f
			}
		};

		// Decides which providers the starting kitchen gets.
		public override HashSet<Item> MinimumIngredients => new()
		{
			GetGDO<Item>(ItemReferences.Flour),
			GetGDO<Item>(ItemReferences.Oil),
			GetGDO<Item>(ItemReferences.Pot),
			GetGDO<Item>(ItemReferences.Plate)
		};

		// Decides which appliances the starting kitchen gets. Kneading happens on plain counters.
		public override HashSet<Process> RequiredProcesses => new()
		{
			GetGDO<Process>(ProcessReferences.Cook)
		};

		private GameObject _display;
		public override GameObject DisplayPrefab => _display ??= LangosModels.Plated("Langos Display", out _, out _);
		private GameObject _icon;
		public override GameObject IconPrefab => _icon ??= LangosModels.Plated("Langos Icon", out _, out _);

		public override List<(Locale, UnlockInfo)> InfoList => new()
		{
			(Locale.English, CreateUnlockInfo("Lángos", "Adds lángos as a main: deep-fried dough, the Hungarian beach and market classic.", "Hot, crispy, and best eaten standing up."))
		};

		public override Dictionary<Locale, string> Recipe => new()
		{
			{ Locale.English, "Combine flour and oil, then knead it on a counter into a lángos. Add oil to a pot, drop the lángos in and fry it on a hob — take it out before it burns! Plate and serve." }
		};
	}

	/// <summary>Topping card: customers start asking for grated cheese on their lángos.</summary>
	public class LangosCheeseDish : CustomDish
	{
		public override string UniqueNameID => "Langos Cheese Dish";
		public override DishType Type => DishType.Extra;
		public override UnlockGroup UnlockGroup => UnlockGroup.Dish;
		public override CardType CardType => CardType.Default;
		public override bool IsUnlockable => true;
		public override DishCustomerChange CustomerMultiplier => DishCustomerChange.SmallDecrease;
		public override Unlock.RewardLevel ExpReward => Unlock.RewardLevel.Small;
		public override int Difficulty => 1;

		public override List<Unlock> HardcodedRequirements => new() { GetCastedGDO<Unlock, LangosDish>() };

		public override HashSet<Dish.IngredientUnlock> IngredientsUnlocks => new()
		{
			new()
			{
				MenuItem = GetCastedGDO<ItemGroup, PlatedLangos>(),
				Ingredient = GetGDO<Item>(ItemReferences.CheeseGrated)
			}
		};

		public override HashSet<Item> MinimumIngredients => new()
		{
			GetGDO<Item>(ItemReferences.Cheese)
		};

		public override HashSet<Process> RequiredProcesses => new()
		{
			GetGDO<Process>(ProcessReferences.Chop)
		};

		private GameObject _display;
		public override GameObject DisplayPrefab => _display ??= LangosModels.Plated("Langos Cheese Display", out _, out _);
		private GameObject _icon;
		public override GameObject IconPrefab => _icon ??= LangosModels.Plated("Langos Cheese Icon", out _, out _);

		public override List<(Locale, UnlockInfo)> InfoList => new()
		{
			(Locale.English, CreateUnlockInfo("Sajtos Lángos", "Customers may ask for grated cheese on their lángos.", "More cheese is always the answer."))
		};

		public override Dictionary<Locale, string> Recipe => new()
		{
			{ Locale.English, "Chop cheese to grate it, then add it to a plated lángos." }
		};
	}
}
