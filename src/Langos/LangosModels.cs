using HungarianTransylvanianCooking.Utils;
using KitchenData;
using KitchenLib.References;
using UnityEngine;

namespace HungarianTransylvanianCooking.Langos
{
	/// <summary>Placeholder primitive models for the lángos chain. Sizes are in world units (a counter is 1 wide).</summary>
	internal static class LangosModels
	{
		private const float LangosDiameter = 0.44f;

		private static Material Dough => Prefabs.Flat("Lángos Dough", 0xF1DDB2);
		private static Material Fried => Prefabs.Flat("Lángos Fried", 0xE6B45C);
		private static Material FriedEdge => Prefabs.Flat("Lángos Fried Edge", 0xB06A26);
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

		private static GameObject Single(string name, LangosStage stage)
		{
			var prefab = Prefabs.Create(name);
			AddLangos(prefab, stage.ToString(), 0f, stage);
			return prefab;
		}

		/// <summary>
		/// The vanilla frying pan with one lángos in each possible state on top; the ItemGroupView toggles which one shows.
		/// The pan itself is copied from the game so it looks the same whether or not something is frying in it.
		/// </summary>
		public static GameObject FryingPan(string name, out GameObject raw, out GameObject fried)
		{
			var prefab = Prefabs.Create(name);
			float y = 0.06f;

			var vanilla = GetGDO<Item>(ItemReferences.FryingPan)?.Prefab;
			if (vanilla != null)
			{
				var pan = Object.Instantiate(vanilla, prefab.transform, false);
				pan.name = "Pan";
				// Sit the lángos low inside the pan rather than on its rim or handle.
				var bounds = Prefabs.LocalBounds(prefab, pan);
				y = bounds.min.y + bounds.size.y * 0.35f;
				LogInfo($"Vanilla frying pan bounds {bounds.min} to {bounds.max}; lángos placed at y={y:0.###}");
			}
			else
			{
				LogWarning("Vanilla frying pan prefab not found, using a placeholder pan");
				Prefabs.AddDisc(prefab, "Pan", 0f, 0.62f, 0.06f, Metal);
				Prefabs.AddPart(prefab, "Handle", PrimitiveType.Cube, new Vector3(0.48f, 0.04f, 0f), new Vector3(0.36f, 0.04f, 0.07f), Metal);
			}

			raw = AddLangos(prefab, "Raw", y, LangosStage.Raw);
			fried = AddLangos(prefab, "Fried", y, LangosStage.Fried);
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

		private enum LangosStage { Raw, Fried }

		private static GameObject AddLangos(GameObject parent, string name, float y, LangosStage stage)
		{
			if (stage == LangosStage.Raw)
				return Prefabs.AddDisc(parent, name, y, LangosDiameter, 0.03f, Dough);

			// Fried lángos: a lighter, slightly domed middle inside a darker, puffed-up rim.
			var langos = new GameObject(name);
			langos.transform.SetParent(parent.transform, false);
			langos.transform.localPosition = new Vector3(0f, y, 0f);

			Prefabs.AddPart(langos, "Middle", PrimitiveType.Sphere, new Vector3(0f, 0.03f, 0f), new Vector3(0.34f, 0.04f, 0.34f), Fried);

			const int rimPuffs = 12;
			const float puffSize = 0.12f;
			float rimRadius = (LangosDiameter - puffSize) / 2f;
			for (int i = 0; i < rimPuffs; i++)
			{
				float angle = i * Mathf.PI * 2f / rimPuffs;
				Prefabs.AddPart(langos, $"Rim {i}", PrimitiveType.Sphere,
					new Vector3(Mathf.Cos(angle) * rimRadius, 0.035f, Mathf.Sin(angle) * rimRadius),
					new Vector3(puffSize, 0.07f, puffSize), FriedEdge);
			}
			return langos;
		}
	}
}
