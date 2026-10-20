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
		public override string ColourBlindTag => "Ga";
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
		public override string ColourBlindTag => "G";
		public override ItemStorage ItemStorageFlags => ItemStorage.StackableFood;
		public override ItemCategory ItemCategory => ItemCategory.Generic;

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosModels.GarlicPaste("Garlic Paste");
	}

	public class SourCream : CustomItem
	{
		public override string UniqueNameID => "Sour Cream";
		public override string ColourBlindTag => "SC";
		public override ItemStorage ItemStorageFlags => ItemStorage.StackableFood;
		public override ItemCategory ItemCategory => ItemCategory.Generic;
		public override Appliance DedicatedProvider => GetCastedGDO<Appliance, SourCreamProvider>();

		private GameObject _prefab;
		public override GameObject Prefab => _prefab ??= LangosModels.SourCreamCup("Sour Cream");
	}

	public class SmokedHam : CustomItem
	{
		public override string UniqueNameID => "Smoked Ham";
		public override string ColourBlindTag => "Ha";
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
		public override string ColourBlindTag => "H";
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
		protected override string[] VanillaContents => new[] { "onion" };
	}

	public class SourCreamProvider : LangosProvider<SourCream>
	{
		public override string UniqueNameID => "Sour Cream Provider";
		protected override string ProviderName => "Sour Cream";
		protected override string ProviderDescription => "Provides sour cream";
		protected override int VanillaLook => ApplianceReferences.SourceMilk;
		protected override string[] VanillaContents => new[] { "milk", "bottle", "carton", "jug" };
	}

	public class SmokedHamProvider : LangosProvider<SmokedHam>
	{
		public override string UniqueNameID => "Smoked Ham Provider";
		protected override string ProviderName => "Smoked Ham";
		protected override string ProviderDescription => "Provides Hungarian smoked ham";
		protected override int VanillaLook => ApplianceReferences.SourceHam;
		// Five vanilla slices stacked on the board become one whole smoked ham.
		protected override string[] VanillaContents => new[] { "ham" };
		protected override bool ReplaceContentsAsOne => true;
	}

	/// <summary>
	/// An unlimited provider for <typeparamref name="T"/>. Its model and shop behaviour are copied from a similar vanilla
	/// provider. The vanilla food in that model is swapped for <typeparamref name="T"/>'s model where it can be found
	/// (see <see cref="VanillaContents"/>); otherwise the item sits on top as a sign.
	/// </summary>
	public abstract class LangosProvider<T> : CustomAppliance where T : CustomItem
	{
		protected abstract int VanillaLook { get; }
		protected abstract string ProviderName { get; }
		protected abstract string ProviderDescription { get; }

		/// <summary>
		/// Words that name the vanilla food parts in <see cref="VanillaLook"/>'s model (matched against object and mesh names).
		/// Those parts are hidden and refilled with this provider's item. Empty keeps the vanilla model whole.
		/// </summary>
		protected virtual string[] VanillaContents => new string[0];

		/// <summary>True puts one item where all the vanilla food parts were, instead of filling each part's space.</summary>
		protected virtual bool ReplaceContentsAsOne => false;

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

			var item = GetCastedGDO<Item, T>()?.Prefab;
			if (item == null)
				return prefab;

			if (ReplaceVanillaContents(prefab, item) == 0)
			{
				// Couldn't find the vanilla food: put the item on top as a sign instead.
				var bounds = Prefabs.LocalBounds(prefab, prefab);
				var sign = CopyItemModel(item, prefab, "Sign");
				sign.transform.localPosition = new Vector3(0f, bounds.max.y, 0f);
				if (VanillaContents.Length > 0)
					LogWarning($"{UniqueNameID}: found no vanilla parts matching [{string.Join(", ", VanillaContents)}], using a sign on top. Bounds {bounds.min} to {bounds.max}");
			}
			return prefab;
		}

		/// <summary>A copy of the item's model for decoration, without the colourblind label KitchenLib attaches to the item.</summary>
		private static GameObject CopyItemModel(GameObject item, GameObject prefab, string name)
		{
			var copy = Object.Instantiate(item, prefab.transform, false);
			copy.name = name;
			var label = copy.transform.Find("Colour Blind");
			if (label != null)
				Object.DestroyImmediate(label.gameObject);
			return copy;
		}

		/// <summary>Hides the vanilla food parts and fills the space each one took up with copies of <paramref name="item"/>.</summary>
		private int ReplaceVanillaContents(GameObject prefab, GameObject item)
		{
			if (VanillaContents.Length == 0)
				return 0;

			var parts = new List<(MeshRenderer Renderer, Bounds Bounds)>();
			foreach (var renderer in prefab.GetComponentsInChildren<MeshRenderer>(true))
			{
				var mesh = renderer.GetComponent<MeshFilter>()?.sharedMesh;
				string names = $"{renderer.name} {mesh?.name}".ToLowerInvariant();
				if (!VanillaContents.Any(names.Contains))
					continue;
				var bounds = Prefabs.LocalBounds(prefab, renderer.gameObject);
				// Anything this big is the crate or fridge itself, not the food in it.
				if (Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z) > 0.9f)
					continue;
				parts.Add((renderer, bounds));
			}
			if (parts.Count == 0)
				return 0;

			foreach (var part in parts)
				part.Renderer.enabled = false;
			LogInfo($"{UniqueNameID}: hid {parts.Count} vanilla part(s): {string.Join(", ", parts.Select(p => $"'{p.Renderer.name}' size {p.Bounds.size}"))}");

			var itemBounds = Prefabs.LocalBounds(item, item);
			var itemSize = Vector3.Max(itemBounds.size, Vector3.one * 0.05f);

			if (ReplaceContentsAsOne)
			{
				var all = parts[0].Bounds;
				foreach (var part in parts)
					all.Encapsulate(part.Bounds);
				CopyItemModel(item, prefab, "Contents").transform.localPosition = new Vector3(all.center.x, all.min.y - itemBounds.min.y, all.center.z);
				return 1;
			}

			int placed = 0;
			foreach (var (_, bounds) in parts)
			{
				// One part may be a whole row of bottles, or a tall bottle, so fill its space with as many items as fit:
				// a grid of up to 4 x 4, stacked up to 3 high, shrunk (not below 60%) when even one is too wide.
				int across = Mathf.Clamp(Mathf.FloorToInt(bounds.size.x / itemSize.x), 1, 4);
				int deep = Mathf.Clamp(Mathf.FloorToInt(bounds.size.z / itemSize.z), 1, 4);
				float scale = Mathf.Clamp(Mathf.Min(bounds.size.x / across / itemSize.x, bounds.size.z / deep / itemSize.z), 0.6f, 1f);
				int high = Mathf.Clamp(Mathf.FloorToInt(bounds.size.y / (itemSize.y * scale)), 1, 3);

				for (int x = 0; x < across; x++)
				for (int z = 0; z < deep; z++)
				for (int y = 0; y < high; y++)
				{
					var copy = CopyItemModel(item, prefab, $"Contents {placed++}");
					copy.transform.localScale = Vector3.one * scale;
					copy.transform.localPosition = new Vector3(
						bounds.min.x + bounds.size.x * (x + 0.5f) / across,
						bounds.min.y + (y * itemSize.y - itemBounds.min.y) * scale,
						bounds.min.z + bounds.size.z * (z + 0.5f) / deep);
				}
			}
			return placed;
		}
	}
}
