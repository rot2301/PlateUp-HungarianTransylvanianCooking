using HungarianTransylvanianCooking.Utils;
using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.References;
using KitchenLib.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Custom topping ingredients that the base game doesn't have:
//   Garlic (provider)  --chop-->  Garlic Paste
//   Sour Cream (provider)
//   Smoked Ham (provider)  --chop-->  Chopped Smoked Ham
//
// UniqueNameIDs are hashed into save data, so never rename them after release.
namespace HungarianTransylvanianCooking.Langos
{
	public class Garlic : CustomItem
	{
		public override string UniqueNameID => "Garlic";
		public override ItemStorage ItemStorageFlags => ItemStorage.StackableFood;
		public override ItemCategory ItemCategory => ItemCategory.Generic;
		public override Appliance DedicatedProvider => GetCastedGDO<Appliance, GarlicProvider>();

		public override List<Item.ItemProcess> Processes => new()
		{
			new()
			{
				Process = GetGDO<Process>(ProcessReferences.Chop),
				Duration = 1.5f,
				Result = GetCastedGDO<Item, GarlicPaste>()
			}
		};

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosModels.GarlicBulb("Garlic");
	}

	public class GarlicPaste : CustomItem
	{
		public override string UniqueNameID => "Garlic Paste";
		public override ItemStorage ItemStorageFlags => ItemStorage.StackableFood;
		public override ItemCategory ItemCategory => ItemCategory.Generic;

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosModels.GarlicPaste("Garlic Paste");
	}

	public class SourCream : CustomItem
	{
		public override string UniqueNameID => "Sour Cream";
		public override ItemStorage ItemStorageFlags => ItemStorage.StackableFood;
		public override ItemCategory ItemCategory => ItemCategory.Generic;
		public override Appliance DedicatedProvider => GetCastedGDO<Appliance, SourCreamProvider>();

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosModels.SourCreamTub("Sour Cream");
	}

	public class SmokedHam : CustomItem
	{
		public override string UniqueNameID => "Smoked Ham";
		public override ItemStorage ItemStorageFlags => ItemStorage.StackableFood;
		public override ItemCategory ItemCategory => ItemCategory.Generic;
		public override Appliance DedicatedProvider => GetCastedGDO<Appliance, SmokedHamProvider>();

		public override List<Item.ItemProcess> Processes => new()
		{
			new()
			{
				Process = GetGDO<Process>(ProcessReferences.Chop),
				Duration = 1f,
				Result = GetCastedGDO<Item, ChoppedSmokedHam>()
			}
		};

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosModels.SmokedHam("Smoked Ham");
	}

	public class ChoppedSmokedHam : CustomItem
	{
		public override string UniqueNameID => "Chopped Smoked Ham";
		public override ItemStorage ItemStorageFlags => ItemStorage.StackableFood;
		public override ItemCategory ItemCategory => ItemCategory.Generic;

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosModels.ChoppedHam("Chopped Smoked Ham");
	}

	public class GarlicProvider : LangosProvider<Garlic>
	{
		public override string UniqueNameID => "Garlic Provider";
		protected override string ProviderName => "Garlic";
		protected override string ProviderDescription => "Provides garlic";
		protected override int VanillaLook => ApplianceReferences.SourceOnion;
	}

	public class SourCreamProvider : LangosProvider<SourCream>
	{
		public override string UniqueNameID => "Sour Cream Provider";
		protected override string ProviderName => "Sour Cream";
		protected override string ProviderDescription => "Provides sour cream";
		protected override int VanillaLook => ApplianceReferences.SourceMilk;
	}

	public class SmokedHamProvider : LangosProvider<SmokedHam>
	{
		public override string UniqueNameID => "Smoked Ham Provider";
		protected override string ProviderName => "Smoked Ham";
		protected override string ProviderDescription => "Provides Hungarian smoked ham";
		protected override int VanillaLook => ApplianceReferences.SourceHam;
	}

	/// <summary>
	/// An unlimited provider for <typeparamref name="T"/>. Its model and shop behaviour are copied from a similar vanilla
	/// provider, with a model of the provided item on top so the two can be told apart.
	/// </summary>
	public abstract class LangosProvider<T> : CustomAppliance where T : CustomItem
	{
		protected abstract int VanillaLook { get; }
		protected abstract string ProviderName { get; }
		protected abstract string ProviderDescription { get; }
		private Appliance Vanilla => GetGDO<Appliance>(VanillaLook);

		public override List<IApplianceProperty> Properties => new()
		{
			KitchenPropertiesUtils.GetUnlimitedCItemProvider(GetCustomGameDataObject<T>().ID)
		};

		public override List<(Locale, ApplianceInfo)> InfoList => new()
		{
			(Locale.English, CreateApplianceInfo(ProviderName, ProviderDescription, new(), new()))
		};

		public override bool IsPurchasable => true;
		public override PriceTier PriceTier => Vanilla?.PriceTier ?? PriceTier.Medium;
		public override ShoppingTags ShoppingTags => Vanilla?.ShoppingTags ?? (ShoppingTags.Basic | ShoppingTags.Cooking);
		public override RarityTier RarityTier => Vanilla?.RarityTier ?? RarityTier.Common;
		public override ShopRequirementFilter ShopRequirementFilter => Vanilla?.ShopRequirementFilter ?? ShopRequirementFilter.None;
		public override List<Item> RequiresIngredientForShop => new() { GetCastedGDO<Item, T>() };

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= BuildPrefab();

		private GameObject BuildPrefab()
		{
			var prefab = Prefabs.Create(UniqueNameID);
			var look = Vanilla?.Prefab;
			if (look == null)
			{
				LogWarning($"{UniqueNameID}: vanilla provider model not found, using a plain box");
				Prefabs.AddPart(prefab, "Box", PrimitiveType.Cube, new Vector3(0f, 0.45f, 0f), new Vector3(0.9f, 0.9f, 0.9f), Prefabs.Material("Wood", 0x9C6B3E));
			}
			else
			{
				Object.Instantiate(look, prefab.transform, false).name = "Provider";
			}

			// The item's own model as a sign on top of the crate.
			var item = GetCastedGDO<Item, T>()?.Prefab;
			if (item != null)
			{
				var bounds = Prefabs.LocalBounds(prefab, prefab);
				var sign = Object.Instantiate(item, prefab.transform, false);
				sign.name = "Sign";
				sign.transform.localPosition = new Vector3(0f, bounds.max.y, 0f);
				LogInfo($"{UniqueNameID}: provider bounds {bounds.min} to {bounds.max}");
			}
			return prefab;
		}
	}
}
