using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.References;
using KitchenLib.Utils;
using System.Collections.Generic;
using UnityEngine;

// Lángos chain:
//   Flour + Oil (from the bottle, which is kept)  --combine-->  Lángos Dough  --knead (counter)-->  Raw Lángos
//   Frying Pan (vanilla) + Raw Lángos  --combine-->  Lángos Pan  --cook (hob)-->  Fried Pan  --(too long)-->  Burned Frying Pan (vanilla, wash it)
//   Fried Pan  --take out-->  Fried Lángos (+ the empty pan stays behind for the next one)
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
					// The portion poured from the oil bottle; using ItemReferences.Oil would consume the bottle itself.
					GetGDO<Item>(ItemReferences.OilIngredient)
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

		// Only fries inside a frying pan (the "wrapper"), never directly on a hob.
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

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosModels.FriedFlat("Fried Langos");
	}

	public class LangosFryingPan : CustomItemGroup<ItemGroupView>
	{
		public override string UniqueNameID => "Langos Pan - Frying";
		public override ItemStorage ItemStorageFlags => ItemStorage.None;
		public override ItemCategory ItemCategory => ItemCategory.Generic;
		public override Item DisposesTo => GetGDO<Item>(ItemReferences.FryingPan);

		public override List<ItemGroup.ItemSet> Sets => new()
		{
			new()
			{
				Items = new() { GetGDO<Item>(ItemReferences.FryingPan) },
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
				Result = GetCastedGDO<Item, LangosFriedPan>()
			}
		};

		public override List<ItemGroupView.ColourBlindLabel> Labels => LangosPan.Labels;

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosPan.Build("Langos Pan - Frying");
		public override void SetupPrefab(GameObject prefab) => LangosPan.SetupView(prefab);
	}

	public class LangosFriedPan : CustomItemGroup<ItemGroupView>
	{
		public override string UniqueNameID => "Langos Pan - Fried";
		public override ItemStorage ItemStorageFlags => ItemStorage.None;
		public override ItemCategory ItemCategory => ItemCategory.Generic;
		public override Item DisposesTo => GetGDO<Item>(ItemReferences.FryingPan);

		// Components (the lángos) carry over from the frying pan and get cooked along with it.
		public override List<ItemGroup.ItemSet> Sets => new();
		public override bool ApplyProcessesToComponents => true;

		// Taking from the pan hands out the fried lángos and leaves the empty pan behind.
		public override bool PreventExplicitSplit => true;
		public override bool AllowSplitMerging => true;
		public override bool SplitByComponents => true;
		public override Item SplitByComponentsHolder => GetGDO<Item>(ItemReferences.FryingPan);
		public override Item RefuseSplitWith => GetGDO<Item>(ItemReferences.FryingPan);
		public override int SplitCount => 1;
		public override List<Item> SplitDepletedItems => new() { GetGDO<Item>(ItemReferences.FryingPan) };

		// Left too long it burns like a vanilla fajita pan: the lángos is lost and the pan needs washing.
		public override List<Item.ItemProcess> Processes => new()
		{
			new()
			{
				Process = GetGDO<Process>(ProcessReferences.Cook),
				Duration = 6f,
				IsBad = true,
				Result = GetGDO<Item>(ItemReferences.FryingPanBurned)
			}
		};

		public override List<ItemGroupView.ColourBlindLabel> Labels => LangosPan.Labels;

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosPan.Build("Langos Pan - Fried");
		public override void SetupPrefab(GameObject prefab) => LangosPan.SetupView(prefab);
	}

	/// <summary>Shared model + view wiring for the two frying-pan states.</summary>
	internal static class LangosPan
	{
		public static List<ItemGroupView.ColourBlindLabel> Labels => new()
		{
			new() { Item = GetCastedGDO<Item, RawLangos>(), Text = "L" },
			new() { Item = GetCastedGDO<Item, FriedLangos>(), Text = "FL" }
		};

		public static GameObject Build(string name) => LangosModels.FryingPan(name, out _, out _);

		public static void SetupView(GameObject prefab)
		{
			var view = prefab.TryAddComponent<ItemGroupView>();
			view.ComponentGroups = new()
			{
				new() { Item = GetCastedGDO<Item, RawLangos>(), Objects = new() { prefab.GetChild("Raw") } },
				new() { Item = GetCastedGDO<Item, FriedLangos>(), Objects = new() { prefab.GetChild("Fried") } }
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
