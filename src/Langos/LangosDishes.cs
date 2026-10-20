using KitchenData;
using KitchenLib.Customs;
using KitchenLib.References;
using System.Collections.Generic;
using UnityEngine;

namespace HungarianTransylvanianCooking.Langos
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
			"Piata Lángos"
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
			GetGDO<Item>(ItemReferences.FryingPan),
			GetGDO<Item>(ItemReferences.Plate)
		};

		// Decides which appliances the starting kitchen gets. Kneading happens on plain counters.
		public override HashSet<Process> RequiredProcesses => new()
		{
			GetGDO<Process>(ProcessReferences.Cook)
		};

		private GameObject _display;
		public override GameObject DisplayPrefab => _display ??= LangosModels.Plated("Langos Display");
		private GameObject _icon;
		public override GameObject IconPrefab => _icon ??= LangosModels.Plated("Langos Icon");

		public override List<(Locale, UnlockInfo)> InfoList => new()
		{
			(Locale.English, CreateUnlockInfo("Lángos", "Adds lángos as a main: deep-fried dough, the Hungarian beach and market classic.", "Hot, crispy, and best eaten standing up."))
		};

		public override Dictionary<Locale, string> Recipe => new()
		{
			{ Locale.English, "Combine flour with oil, then knead it on a counter into a lángos. Put it in a frying pan and fry it on a hob - take it out before it burns! Plate and serve." }
		};
	}

	/// <summary>Main card: sweet lángos with chocolate syrup and/or chopped nuts, never mixed with savoury toppings.</summary>
	public class LangosSweetDish : CustomDish
	{
		public override string UniqueNameID => "Langos Sweet Dish";
		public override DishType Type => DishType.Main;
		public override UnlockGroup UnlockGroup => UnlockGroup.Dish;
		public override CardType CardType => CardType.Default;
		public override bool IsUnlockable => true;
		public override DishCustomerChange CustomerMultiplier => DishCustomerChange.SmallDecrease;
		public override Unlock.RewardLevel ExpReward => Unlock.RewardLevel.Medium;
		public override int Difficulty => 2;

		public override List<Unlock> HardcodedRequirements => new() { GetCastedGDO<Unlock, LangosDish>() };

		public override List<Dish.MenuItem> ResultingMenuItems => new()
		{
			new()
			{
				Item = GetCastedGDO<Item, PlatedSweetLangos>(),
				Phase = MenuPhase.Main,
				Weight = 1f
			}
		};

		// The syrup bottle brings its provider (the Sundaes chocolate syrup stand); customers get its servings.
		public override HashSet<Item> MinimumIngredients => new()
		{
			GetGDO<Item>(ItemReferences.SundaeSyrupChocolate),
			GetGDO<Item>(ItemReferences.NutsIngredient)
		};

		public override HashSet<Process> RequiredProcesses => new()
		{
			GetGDO<Process>(ProcessReferences.Chop)
		};

		private GameObject _display;
		public override GameObject DisplayPrefab => _display ??= LangosModels.Plated("Langos Sweet Display", LangosTopping.Chocolate, LangosTopping.Nuts);
		private GameObject _icon;
		public override GameObject IconPrefab => _icon ??= LangosModels.Plated("Langos Sweet Icon", LangosTopping.Chocolate, LangosTopping.Nuts);

		public override List<(Locale, UnlockInfo)> InfoList => new()
		{
			(Locale.English, CreateUnlockInfo("Édes Lángos", "Adds sweet lángos as a main, topped with chocolate syrup and/or chopped nuts.", "Nobody said lángos had to be savoury."))
		};

		public override Dictionary<Locale, string> Recipe => new()
		{
			{ Locale.English, "Drizzle chocolate syrup from the bottle and/or add chopped nuts to a plated lángos. Sweet and savoury toppings can't go on the same lángos." }
		};
	}

	public class LangosCheeseDish : LangosToppingCard
	{
		public override string UniqueNameID => "Langos Cheese Dish";
		protected override LangosTopping Part => LangosTopping.Cheese;
		protected override Item Topping => GetGDO<Item>(ItemReferences.CheeseGrated);
		public override HashSet<Item> MinimumIngredients => new() { GetGDO<Item>(ItemReferences.Cheese) };
		public override HashSet<Process> RequiredProcesses => new() { GetGDO<Process>(ProcessReferences.Chop) };
		protected override string Title => "Sajtos Lángos";
		protected override string Description => "Customers may ask for grated cheese on their lángos.";
		protected override string FlavourText => "More cheese is always the answer.";
		protected override string RecipeText => "Chop cheese to grate it, then add it to a plated lángos.";
	}

	public class LangosGarlicDish : LangosToppingCard
	{
		public override string UniqueNameID => "Langos Garlic Dish";
		protected override LangosTopping Part => LangosTopping.Garlic;
		// Garlic comes from IngredientLib: whole --chop--> peeled --chop--> minced.
		protected override Item Topping => GetCastedGDO<Item, IngredientLib.Ingredient.Items.MincedGarlic>();
		public override HashSet<Item> MinimumIngredients => new() { GetCastedGDO<Item, IngredientLib.Ingredient.Items.Garlic>() };
		public override HashSet<Process> RequiredProcesses => new() { GetGDO<Process>(ProcessReferences.Chop) };
		protected override string Title => "Fokhagymás Lángos";
		protected override string Description => "Customers may ask for minced garlic on their lángos.";
		protected override string FlavourText => "Keeps the vampires away.";
		protected override string RecipeText => "Chop garlic to peel it, chop it again to mince it, then add it to a plated lángos.";
	}

	public class LangosSourCreamDish : LangosToppingCard
	{
		public override string UniqueNameID => "Langos Sour Cream Dish";
		protected override LangosTopping Part => LangosTopping.SourCream;
		protected override Item Topping => GetCastedGDO<Item, SourCream>();
		public override HashSet<Item> MinimumIngredients => new() { GetCastedGDO<Item, SourCream>() };
		protected override string Title => "Tejfölös Lángos";
		protected override string Description => "Customers may ask for sour cream on their lángos.";
		protected override string FlavourText => "Half of every proper lángos.";
		protected override string RecipeText => "Add sour cream to a plated lángos.";
	}

	public class LangosOnionDish : LangosToppingCard
	{
		public override string UniqueNameID => "Langos Onion Dish";
		protected override LangosTopping Part => LangosTopping.Onion;
		protected override Item Topping => GetGDO<Item>(ItemReferences.OnionChopped);
		public override HashSet<Item> MinimumIngredients => new() { GetGDO<Item>(ItemReferences.Onion) };
		public override HashSet<Process> RequiredProcesses => new() { GetGDO<Process>(ProcessReferences.Chop) };
		protected override string Title => "Hagymás Lángos";
		protected override string Description => "Customers may ask for chopped onion on their lángos.";
		protected override string FlavourText => "Bring a mint.";
		protected override string RecipeText => "Chop an onion, then add it to a plated lángos.";
	}

	public class LangosPicklesDish : LangosToppingCard
	{
		public override string UniqueNameID => "Langos Pickles Dish";
		protected override LangosTopping Part => LangosTopping.Pickles;
		protected override Item Topping => GetGDO<Item>(ItemReferences.Pickle);
		public override HashSet<Item> MinimumIngredients => new() { GetGDO<Item>(ItemReferences.Pickle) };
		protected override string Title => "Savanyúságos Lángos";
		protected override string Description => "Customers may ask for pickles on their lángos.";
		protected override string FlavourText => "A little sour to cut through the grease.";
		protected override string RecipeText => "Add pickles to a plated lángos.";
	}

	public class LangosHamDish : LangosToppingCard
	{
		public override string UniqueNameID => "Langos Ham Dish";
		protected override LangosTopping Part => LangosTopping.Ham;
		protected override Item Topping => GetCastedGDO<Item, ChoppedSmokedHam>();
		public override HashSet<Item> MinimumIngredients => new() { GetCastedGDO<Item, SmokedHam>() };
		public override HashSet<Process> RequiredProcesses => new() { GetGDO<Process>(ProcessReferences.Chop) };
		protected override string Title => "Sonkás Lángos";
		protected override string Description => "Customers may ask for chopped smoked ham on their lángos.";
		protected override string FlavourText => "Smoked low and slow, chopped fast.";
		protected override string RecipeText => "Chop smoked ham, then add it to a plated lángos.";
	}

	/// <summary>
	/// A card that lets customers ask for one more savoury topping on their lángos. Savoury toppings combine freely
	/// (see PlatedLangos), so each card only has to say which topping it adds.
	/// </summary>
	public abstract class LangosToppingCard : CustomDish
	{
		protected abstract LangosTopping Part { get; }
		protected abstract Item Topping { get; }
		protected abstract string Title { get; }
		protected abstract string Description { get; }
		protected abstract string FlavourText { get; }
		protected abstract string RecipeText { get; }

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
				Ingredient = Topping
			}
		};

		private GameObject _display;
		public override GameObject DisplayPrefab => _display ??= LangosModels.Plated($"{UniqueNameID} Display", Part);
		private GameObject _icon;
		public override GameObject IconPrefab => _icon ??= LangosModels.Plated($"{UniqueNameID} Icon", Part);

		public override List<(Locale, UnlockInfo)> InfoList => new()
		{
			(Locale.English, CreateUnlockInfo(Title, Description, FlavourText))
		};

		public override Dictionary<Locale, string> Recipe => new()
		{
			{ Locale.English, RecipeText }
		};
	}
}
