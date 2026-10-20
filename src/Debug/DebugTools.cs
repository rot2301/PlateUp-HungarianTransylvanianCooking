#if DEBUG
using HungarianTransylvanianCooking.Langos;
using Kitchen;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Event;
using KitchenLib.References;
using KitchenMods;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

// Development helpers. Only compiled into Debug builds, so Release builds (the ones uploaded to the Workshop) never
// contain any of this.
namespace HungarianTransylvanianCooking.Debugging
{
	/// <summary>
	/// Logs the vanilla items and providers our content copies or depends on: what each provider hands out, how the
	/// relevant items split, and the model hierarchy of each prefab. Look for "[DEBUG DUMP]" in Player.log.
	/// </summary>
	internal static class GameDataDump
	{
		private static readonly (string Name, int ID)[] Providers =
		{
			("SourceOnion", ApplianceReferences.SourceOnion),
			("SourceMilk", ApplianceReferences.SourceMilk),
			("SourceHam", ApplianceReferences.SourceHam),
			("SourcePickle", ApplianceReferences.SourcePickle),
			("SourceChocolate", ApplianceReferences.SourceChocolate),
			("SourceNuts", ApplianceReferences.SourceNuts),
			("SourceCheese", ApplianceReferences.SourceCheese),
			("SourceOil", ApplianceReferences.SourceOil)
		};

		private static readonly (string Name, int ID)[] Items =
		{
			("Pickle", ItemReferences.Pickle),
			("PickleJar", ItemReferences.PickleJar),
			("Oil", ItemReferences.Oil),
			("OilIngredient", ItemReferences.OilIngredient),
			("FryingPan", ItemReferences.FryingPan),
			("FryingPanBurned", ItemReferences.FryingPanBurned)
		};

		public static void Register() => Events.BuildGameDataPostViewInitEvent += (_, args) =>
		{
			if (args.firstBuild)
				Dump(args.gamedata);
		};

		private static void Dump(GameData data)
		{
			var log = new StringBuilder("[DEBUG DUMP] Vanilla data our content relies on\n");

			foreach (var (name, id) in Providers)
			{
				if (!data.TryGet<Appliance>(id, out var appliance))
				{
					log.AppendLine($"== Appliance {name} ({id}): NOT FOUND");
					continue;
				}
				log.AppendLine($"== Appliance {name} ({id}) '{appliance.name}'  price={appliance.PriceTier} tags={appliance.ShoppingTags} rarity={appliance.RarityTier} filter={appliance.ShopRequirementFilter} purchasable={appliance.IsPurchasable}");
				foreach (var provider in appliance.Properties.OfType<CItemProvider>())
					log.AppendLine($"   provides: {ItemName(data, provider.DefaultProvidedItem)}  available={provider.Available} max={provider.Maximum}");
				AppendHierarchy(log, appliance.Prefab);
			}

			foreach (var (name, id) in Items)
			{
				if (!data.TryGet<Item>(id, out var item))
				{
					log.AppendLine($"== Item {name} ({id}): NOT FOUND");
					continue;
				}
				log.AppendLine($"== Item {name} ({id}) '{item.name}'  type={item.GetType().Name}");
				log.AppendLine($"   dedicated provider: {(item.DedicatedProvider != null ? $"{item.DedicatedProvider.name} ({item.DedicatedProvider.ID})" : "none")}");
				log.AppendLine($"   split: sub item={ItemName(data, item.SplitSubItem?.ID ?? 0)} count={item.SplitCount} by copying={item.SplitByCopying} prevent explicit={item.PreventExplicitSplit} allow merging={item.AllowSplitMerging}");
				foreach (var process in item.DerivedProcesses ?? new List<Item.ItemProcess>())
					log.AppendLine($"   process: {process.Process?.name} {process.Duration}s -> {ItemName(data, process.Result?.ID ?? 0)}{(process.IsBad ? " (bad)" : "")}{(process.RequiresWrapper ? " (needs wrapper)" : "")}");
				if (item is ItemGroup group)
					foreach (var set in group.DerivedSets)
						log.AppendLine($"   set: [{string.Join(", ", set.Items.Select(i => i.name))}] min={set.Min} max={set.Max} mandatory={set.IsMandatory}");
				AppendHierarchy(log, item.Prefab);
			}

			// Our own providers, to check where the item sign ended up on the copied crate.
			foreach (var custom in new CustomAppliance[] { GetCustomGameDataObject<SourCreamProvider>() as CustomAppliance, GetCustomGameDataObject<SmokedHamProvider>() as CustomAppliance })
			{
				if (custom == null)
					continue;
				log.AppendLine($"== Custom appliance {custom.UniqueNameID} ({custom.ID})");
				AppendHierarchy(log, custom.Prefab);
			}

			Debug.Log(log.ToString());
		}

		private static string ItemName(GameData data, int id)
		{
			if (id == 0)
				return "none";
			return data.TryGet<Item>(id, out var item) ? $"{item.name} ({id})" : $"unknown ({id})";
		}

		private static void AppendHierarchy(StringBuilder log, GameObject root)
		{
			if (root == null)
			{
				log.AppendLine("   prefab: none");
				return;
			}
			log.AppendLine("   prefab:");
			AppendNode(log, root.transform, 2);
		}

		private static void AppendNode(StringBuilder log, Transform node, int depth)
		{
			var indent = new string(' ', depth * 2);
			var components = node.GetComponents<Component>()
				.Where(c => c != null && c is not Transform)
				.Select(c => c switch
				{
					MeshFilter filter => $"MeshFilter({filter.sharedMesh?.name})",
					Renderer renderer => $"{renderer.GetType().Name}({string.Join(", ", renderer.sharedMaterials.Select(m => m?.name))})",
					_ => c.GetType().Name
				});
			log.AppendLine($"{indent}{node.name}{(node.gameObject.activeSelf ? "" : " [inactive]")}  pos={node.localPosition} rot={node.localEulerAngles} scale={node.localScale}  {string.Join(" ", components)}");
			if (depth > 12)
				return;
			foreach (Transform child in node)
				AppendNode(log, child, depth + 1);
		}
	}

	/// <summary>
	/// In a lángos run, unlocks Édes Lángos and every topping card right away, as if they had been picked from a card
	/// choice. The game then grants their providers like it would for a real pick.
	/// </summary>
	public class UnlockLangosCards : RestaurantSystem, IModSystem
	{
		private EntityQuery _unlocks;
		private EntityQuery _options;

		private static IEnumerable<int> Cards => new[]
		{
			GetCustomGameDataObject<LangosSweetDish>(),
			GetCustomGameDataObject<LangosCheeseDish>(),
			GetCustomGameDataObject<LangosGarlicDish>(),
			GetCustomGameDataObject<LangosSourCreamDish>(),
			GetCustomGameDataObject<LangosOnionDish>(),
			GetCustomGameDataObject<LangosPicklesDish>(),
			GetCustomGameDataObject<LangosHamDish>()
		}.Where(c => c != null).Select(c => c.ID);

		protected override void Initialise()
		{
			base.Initialise();
			_unlocks = GetEntityQuery(typeof(CProgressionUnlock));
			_options = GetEntityQuery(typeof(CProgressionOption));
		}

		protected override void OnUpdate()
		{
			// Only between days, and never while a card choice is open: an extra selected option could be read as the player's pick.
			if (!Has<SIsNightTime>() || !_options.IsEmpty)
				return;

			using var unlocks = _unlocks.ToComponentDataArray<CProgressionUnlock>(Allocator.Temp);
			var unlocked = new HashSet<int>(unlocks.Select(u => u.ID));
			if (!unlocked.Contains(GetCustomGameDataObject<LangosDish>().ID))
				return;

			foreach (int card in Cards.Where(c => !unlocked.Contains(c)))
			{
				// The same entity the card choice popup creates when a card is picked; DelegateUnlockSystem does the rest.
				var option = EntityManager.CreateEntity();
				EntityManager.AddComponentData(option, new CProgressionOption { ID = card });
				EntityManager.AddComponent<CProgressionOption.Selected>(option);
				// Seven recipe popups in a row would get in the way of testing.
				EntityManager.AddComponent<CSkipShowingRecipe>(option);
				LogInfo($"[DEBUG] Unlocking card {card}");
			}
		}
	}
}
#endif
