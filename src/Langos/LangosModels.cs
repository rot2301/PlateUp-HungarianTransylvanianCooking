using HungarianTransylvanianCooking.Utils;
using UnityEngine;

namespace HungarianTransylvanianCooking.Langos
{
	/// <summary>Placeholder primitive models for the lángos chain. Sizes are in world units (a counter is 1 wide).</summary>
	internal static class LangosModels
	{
		private const float LangosDiameter = 0.44f;

		private static Material Dough => Prefabs.Flat("Lángos Dough", 0xF1DDB2);
		private static Material Fried => Prefabs.Flat("Lángos Fried", 0xD99B3C);
		private static Material Burned => Prefabs.Flat("Lángos Burned", 0x3A2A1F);
		private static Material Oil => Prefabs.Flat("Frying Oil", 0xE6C23F);
		private static Material Cheese => Prefabs.Flat("Grated Cheese", 0xF8DC6E);
		private static Material Metal => Prefabs.Material("Metal", 0x9AA0A6);
		private static Material Plate => Prefabs.Material("Plate", 0xF4F4F0);

		public static GameObject DoughBall(string name)
		{
			var prefab = Prefabs.Create(name);
			Prefabs.AddPart(prefab, "Dough", PrimitiveType.Sphere, new Vector3(0f, 0.08f, 0f), new Vector3(0.3f, 0.16f, 0.3f), Dough);
			return prefab;
		}

		public static GameObject Flat(string name) => Single(name, LangosStage.Raw);
		public static GameObject FriedFlat(string name) => Single(name, LangosStage.Fried);
		public static GameObject BurnedFlat(string name) => Single(name, LangosStage.Burned);

		private static GameObject Single(string name, LangosStage stage)
		{
			var prefab = Prefabs.Create(name);
			AddLangos(prefab, stage.ToString(), 0f, stage);
			return prefab;
		}

		/// <summary>A pot of oil holding one lángos in each possible state; the ItemGroupView toggles which one shows.</summary>
		public static GameObject FryingPot(string name, out GameObject raw, out GameObject fried, out GameObject burned)
		{
			var prefab = Prefabs.Create(name);
			Prefabs.AddDisc(prefab, "Pot", 0f, 0.56f, 0.24f, Metal);
			Prefabs.AddDisc(prefab, "Oil", 0.2f, 0.5f, 0.02f, Oil);
			raw = AddLangos(prefab, "Raw", 0.21f, LangosStage.Raw);
			fried = AddLangos(prefab, "Fried", 0.21f, LangosStage.Fried);
			burned = AddLangos(prefab, "Burned", 0.21f, LangosStage.Burned);
			return prefab;
		}

		public static GameObject Plated(string name, out GameObject langos, out GameObject cheese)
		{
			var prefab = Prefabs.Create(name);
			Prefabs.AddDisc(prefab, "Plate", 0f, 0.6f, 0.03f, Plate);
			langos = AddLangos(prefab, "Langos", 0.03f, LangosStage.Fried);

			cheese = new GameObject("Cheese");
			cheese.transform.SetParent(prefab.transform, false);
			// A loose scatter of shreds on top of the lángos.
			Vector2[] spots = { new(0f, 0f), new(0.08f, 0.05f), new(-0.07f, 0.07f), new(0.05f, -0.09f), new(-0.09f, -0.04f), new(0.12f, -0.02f) };
			for (int i = 0; i < spots.Length; i++)
			{
				var shred = Prefabs.AddPart(cheese, $"Shred {i}", PrimitiveType.Cube,
					new Vector3(spots[i].x, 0.09f, spots[i].y), new Vector3(0.09f, 0.015f, 0.025f), Cheese);
				shred.transform.localRotation = Quaternion.Euler(0f, i * 47f, 0f);
			}
			return prefab;
		}

		private enum LangosStage { Raw, Fried, Burned }

		private static GameObject AddLangos(GameObject parent, string name, float y, LangosStage stage)
		{
			var material = stage switch
			{
				LangosStage.Fried => Fried,
				LangosStage.Burned => Burned,
				_ => Dough
			};
			// Fried lángos puffs up, so it gets a domed sphere instead of a flat disc.
			if (stage == LangosStage.Raw)
				return Prefabs.AddDisc(parent, name, y, LangosDiameter, 0.03f, material);
			return Prefabs.AddPart(parent, name, PrimitiveType.Sphere, new Vector3(0f, y + 0.03f, 0f), new Vector3(LangosDiameter, 0.07f, LangosDiameter), material);
		}
	}
}
