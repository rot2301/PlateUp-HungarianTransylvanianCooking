using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.References;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

// Lángos chain:
//   Flour + Oil  --combine-->  Lángos Dough  --knead (counter)-->  Raw Lángos
//   Pot + Oil (vanilla) + Raw Lángos  --combine-->  Frying Pot  --cook (hob)-->  Fried Pot  --(too long)-->  Burned Pot
//   Fried Pot  --take out-->  Fried Lángos (+ the oil pot stays behind for the next one)
//   Plate + Fried Lángos (+ Grated Cheese once the topping card is picked)  -->  Plated Lángos
//
// UniqueNameIDs are hashed into save data, so never rename them after release.
namespace HungarianTransylvanianCooking.Langos
{
	public class LangosDough : CustomItemGroup
	{
		public override string UniqueNameID => "Langos Dough";
		public override ItemStorage ItemStorageFlags => ItemStorage.StackableFood;
		public override ItemCategory ItemCategory => ItemCategory.Generic;
		public override bool AutoCollapsing => true;

		public override List<ItemGroup.ItemSet> Sets => new()
		{
			new()
			{
				Items = new()
				{
					GetGDO<Item>(ItemReferences.Flour),
					GetGDO<Item>(ItemReferences.Oil)
				},
				IsMandatory = true,
				Min = 2,
				Max = 2
			}
		};

		public override List<Item.ItemProcess> Processes => new()
		{
			new()
			{
				Process = GetGDO<Process>(ProcessReferences.Knead),
				Duration = 1.5f,
				Result = GetCastedGDO<Item, RawLangos>()
			}
		};

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosModels.DoughBall("Langos Dough");
	}

	public class RawLangos : CustomItem
	{
		public override string UniqueNameID => "Raw Langos";
		public override ItemStorage ItemStorageFlags => ItemStorage.StackableFood;
		public override ItemCategory ItemCategory => ItemCategory.Generic;

		// Only fries inside the oil pot (the "wrapper"), never directly on a hob.
		public override List<Item.ItemProcess> Processes => new()
		{
			new()
			{
				Process = GetGDO<Process>(ProcessReferences.Cook),
				RequiresWrapper = true,
				Result = GetCastedGDO<Item, FriedLangos>()
			}
		};

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosModels.Flat("Raw Langos");
	}

	public class FriedLangos : CustomItem
	{
		public override string UniqueNameID => "Fried Langos";
		public override ItemStorage ItemStorageFlags => ItemStorage.StackableFood;
		public override ItemCategory ItemCategory => ItemCategory.Generic;

		public override List<Item.ItemProcess> Processes => new()
		{
			new()
			{
				Process = GetGDO<Process>(ProcessReferences.Cook),
				RequiresWrapper = true,
				Result = GetCastedGDO<Item, BurnedLangos>()
			}
		};

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosModels.FriedFlat("Fried Langos");
	}

	public class BurnedLangos : CustomItem
	{
		public override string UniqueNameID => "Burned Langos";
		public override ItemCategory ItemCategory => ItemCategory.Generic;

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosModels.BurnedFlat("Burned Langos");
	}

	public class LangosFryingPot : CustomItemGroup<ItemGroupView>
	{
		public override string UniqueNameID => "Langos Pot - Frying";
		public override ItemStorage ItemStorageFlags => ItemStorage.None;
		public override ItemCategory ItemCategory => ItemCategory.Generic;
		public override Item DisposesTo => GetGDO<Item>(ItemReferences.Pot);

		public override List<ItemGroup.ItemSet> Sets => new()
		{
			new()
			{
				Items = new() { GetGDO<Item>(ItemReferences.PotWithOil) },
				IsMandatory = true,
				Min = 1,
				Max = 1
			},
			new()
			{
				Items = new() { GetCastedGDO<Item, RawLangos>() },
				Min = 1,
				Max = 1
			}
		};

		public override List<Item.ItemProcess> Processes => new()
		{
			new()
			{
				Process = GetGDO<Process>(ProcessReferences.Cook),
				Duration = 4f,
				Result = GetCastedGDO<Item, LangosFriedPot>()
			}
		};

		public override List<ItemGroupView.ColourBlindLabel> Labels => LangosPot.Labels;

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosPot.Build("Langos Pot - Frying");
		public override void SetupPrefab(GameObject prefab) => LangosPot.SetupView(prefab);
	}

	public class LangosFriedPot : CustomItemGroup<ItemGroupView>
	{
		public override string UniqueNameID => "Langos Pot - Fried";
		public override ItemStorage ItemStorageFlags => ItemStorage.None;
		public override ItemCategory ItemCategory => ItemCategory.Generic;
		public override Item DisposesTo => GetGDO<Item>(ItemReferences.Pot);

		// Components (the lángos) carry over from the frying pot and get cooked along with it.
		public override List<ItemGroup.ItemSet> Sets => new();
		public override bool ApplyProcessesToComponents => true;

		// Taking from the pot hands out the fried lángos and leaves the oil pot behind.
		public override bool PreventExplicitSplit => true;
		public override bool AllowSplitMerging => true;
		public override bool SplitByComponents => true;
		public override Item SplitByComponentsHolder => GetGDO<Item>(ItemReferences.PotWithOil);
		public override Item RefuseSplitWith => GetGDO<Item>(ItemReferences.PotWithOil);
		public override int SplitCount => 1;
		public override List<Item> SplitDepletedItems => new() { GetGDO<Item>(ItemReferences.PotWithOil) };

		public override List<Item.ItemProcess> Processes => new()
		{
			new()
			{
				Process = GetGDO<Process>(ProcessReferences.Cook),
				Duration = 6f,
				IsBad = true,
				Result = GetCastedGDO<Item, LangosBurnedPot>()
			}
		};

		public override List<ItemGroupView.ColourBlindLabel> Labels => LangosPot.Labels;

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosPot.Build("Langos Pot - Fried");
		public override void SetupPrefab(GameObject prefab) => LangosPot.SetupView(prefab);
	}

	public class LangosBurnedPot : CustomItemGroup<ItemGroupView>
	{
		public override string UniqueNameID => "Langos Pot - Burned";
		public override ItemStorage ItemStorageFlags => ItemStorage.None;
		public override ItemCategory ItemCategory => ItemCategory.Generic;
		public override Item DisposesTo => GetGDO<Item>(ItemReferences.Pot);
		public override List<ItemGroup.ItemSet> Sets => new();
		public override bool ApplyProcessesToComponents => true;

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosPot.Build("Langos Pot - Burned");
		public override void SetupPrefab(GameObject prefab) => LangosPot.SetupView(prefab);
	}

	/// <summary>Shared model + view wiring for the three oil-pot states.</summary>
	internal static class LangosPot
	{
		public static List<ItemGroupView.ColourBlindLabel> Labels => new()
		{
			new() { Item = GetCastedGDO<Item, RawLangos>(), Text = "L" },
			new() { Item = GetCastedGDO<Item, FriedLangos>(), Text = "FL" }
		};

		public static GameObject Build(string name) => LangosModels.FryingPot(name, out _, out _, out _);

		public static void SetupView(GameObject prefab)
		{
			var view = prefab.TryAddComponent<ItemGroupView>();
			view.ComponentGroups = new()
			{
				new() { Item = GetCastedGDO<Item, RawLangos>(), Objects = new() { prefab.GetChild("Raw") } },
				new() { Item = GetCastedGDO<Item, FriedLangos>(), Objects = new() { prefab.GetChild("Fried") } },
				new() { Item = GetCastedGDO<Item, BurnedLangos>(), Objects = new() { prefab.GetChild("Burned") } }
			};
		}
	}

	public class PlatedLangos : CustomItemGroup<ItemGroupView>
	{
		public override string UniqueNameID => "Plated Langos";
		public override ItemStorage ItemStorageFlags => ItemStorage.None;
		public override ItemCategory ItemCategory => ItemCategory.Generic;
		public override ItemValue ItemValue => ItemValue.Medium;
		public override Item DisposesTo => GetGDO<Item>(ItemReferences.Plate);
		public override Item DirtiesTo => GetGDO<Item>(ItemReferences.PlateDirty);

		public override List<ItemGroup.ItemSet> Sets => new()
		{
			new()
			{
				Items = new()
				{
					GetGDO<Item>(ItemReferences.Plate),
					GetCastedGDO<Item, FriedLangos>()
				},
				IsMandatory = true,
				Min = 2,
				Max = 2
			},
			// Toppings: customers only ask for these once their card (e.g. LangosCheeseDish) is unlocked.
			new()
			{
				Items = new()
				{
					GetGDO<Item>(ItemReferences.CheeseGrated)
				},
				RequiresUnlock = true,
				Min = 0,
				Max = 1
			}
		};

		public override List<ItemGroupView.ColourBlindLabel> Labels => new()
		{
			new() { Item = GetGDO<Item>(ItemReferences.CheeseGrated), Text = "Ch" }
		};

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosModels.Plated("Plated Langos", out _, out _);

		public override void SetupPrefab(GameObject prefab)
		{
			var view = prefab.TryAddComponent<ItemGroupView>();
			view.ComponentGroups = new()
			{
				new() { Item = GetCastedGDO<Item, FriedLangos>(), Objects = new() { prefab.GetChild("Langos") } },
				new() { Item = GetGDO<Item>(ItemReferences.CheeseGrated), Objects = new() { prefab.GetChild("Cheese") } }
			};
		}
	}
}
