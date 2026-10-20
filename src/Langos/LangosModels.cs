using HungarianTransylvanianCooking.Utils;
using KitchenData;
using KitchenLib.References;
using UnityEngine;

namespace HungarianTransylvanianCooking.Langos
{
	/// <summary>Placeholder primitive models for the lángos chain. Sizes are in world units (a counter is 1 wide).</summary>
	internal static class LangosModels
	{
		internal const float LangosDiameter = 0.44f;

		private static Material Dough => Prefabs.Flat("Lángos Dough", 0xF1DDB2);
		private static Material Fried => Prefabs.Flat("Lángos Fried", 0xE6B45C);
		private static Material FriedEdge => Prefabs.Flat("Lángos Fried Edge", 0xB06A26);
		private static Material Cheese => Prefabs.Flat("Grated Cheese", 0xF8DC6E);
		private static Material MincedGarlic => Prefabs.Flat("Minced Garlic", 0xEDE3BF);
		private static Material Cream => Prefabs.Flat("Sour Cream", 0xFBF8EE);
		private static Material Tub => Prefabs.Flat("Sour Cream Tub", 0x6FA8DC);
		private static Material Onion => Prefabs.Flat("Chopped Onion", 0xE7D3E8);
		private static Material Pickle => Prefabs.Flat("Pickle", 0x7E9A3A);
		private static Material Ham => Prefabs.Flat("Smoked Ham", 0xC96B63);
		private static Material HamRind => Prefabs.Flat("Smoked Ham Rind", 0x6B3A22);
		private static Material Chocolate => Prefabs.Flat("Chocolate Syrup", 0x4A2A18);
		private static Material Nuts => Prefabs.Flat("Chopped Nuts", 0xC9A06A);
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

		/// <summary>
		/// A plated lángos with a model part for each of <paramref name="toppings"/>, each named after its topping so an
		/// ItemGroupView can toggle it. Every topping sits in its own spots so any combination stays readable.
		/// </summary>
		public static GameObject Plated(string name, params LangosTopping[] toppings)
		{
			var prefab = Prefabs.Create(name);
			Prefabs.AddDisc(prefab, "Plate", 0f, 0.6f, 0.03f, Plate);
			AddLangos(prefab, "Langos", 0.03f, LangosStage.Fried);

			// The lángos' middle tops out around y=0.08 on the plate; spreads go first, bits on top.
			const float spread = 0.08f;
			const float bits = 0.095f;
			foreach (var topping in toppings)
			{
				var group = new GameObject(topping.ToString());
				group.transform.SetParent(prefab.transform, false);
				switch (topping)
				{
					case LangosTopping.SourCream:
						Prefabs.AddDisc(group, "Spread", spread, 0.24f, 0.012f, Cream);
						break;
					case LangosTopping.Chocolate:
						AddSpiralDrizzle(group, spread + 0.008f, Chocolate);
						break;
					case LangosTopping.Garlic:
						Scatter(group, PrimitiveType.Cube, 9, 20f, 0.03f, 0.11f, new Vector3(0.022f, 0.015f, 0.022f), bits, MincedGarlic);
						break;
					case LangosTopping.Cheese:
						Scatter(group, PrimitiveType.Cube, 6, 0f, 0f, 0.12f, new Vector3(0.09f, 0.015f, 0.025f), bits, Cheese);
						break;
					case LangosTopping.Onion:
						Scatter(group, PrimitiveType.Cube, 6, 35f, 0.04f, 0.11f, new Vector3(0.035f, 0.025f, 0.035f), bits, Onion);
						break;
					case LangosTopping.Pickles:
						for (int i = 0; i < 3; i++)
						{
							float angle = (100f + i * 120f) * Mathf.Deg2Rad;
							Prefabs.AddDisc(group, $"Slice {i}", bits - 0.006f, 0.075f, 0.012f, Pickle).transform.localPosition
								+= new Vector3(Mathf.Cos(angle) * 0.09f, 0f, Mathf.Sin(angle) * 0.09f);
						}
						break;
					case LangosTopping.Ham:
						Scatter(group, PrimitiveType.Cube, 5, 60f, 0.03f, 0.1f, new Vector3(0.045f, 0.03f, 0.045f), bits, Ham);
						break;
					case LangosTopping.Nuts:
						Scatter(group, PrimitiveType.Cube, 8, 15f, 0.02f, 0.11f, new Vector3(0.03f, 0.022f, 0.03f), bits, Nuts);
						break;
				}
			}
			return prefab;
		}

		/// <summary>
		/// A snail-shell drizzle: one strand spiralling out from the centre, built from short capsules laid end to end so
		/// their rounded ends blend into a continuous line.
		/// </summary>
		private static void AddSpiralDrizzle(GameObject group, float y, Material material)
		{
			const float innerRadius = 0.015f;
			const float outerRadius = 0.135f;
			const float turns = 2.5f;
			const float width = 0.032f;
			const float thickness = 0.016f;
			const float segment = 0.03f;

			// Archimedean spiral: the radius grows evenly with the angle.
			float growth = (outerRadius - innerRadius) / (turns * Mathf.PI * 2f);
			Vector3 Point(float angle)
			{
				float radius = innerRadius + growth * angle;
				return new Vector3(Mathf.Cos(angle) * radius, y, Mathf.Sin(angle) * radius);
			}

			float theta = 0f;
			for (int i = 0; theta < turns * Mathf.PI * 2f; i++)
			{
				// Step by roughly equal lengths along the strand, so the tight middle doesn't bunch up.
				float radius = innerRadius + growth * theta;
				float next = theta + segment / Mathf.Max(radius, 0.02f);
				Vector3 from = Point(theta), to = Point(next);

				var piece = Prefabs.AddPart(group, $"Strand {i}", PrimitiveType.Capsule, (from + to) / 2f, Vector3.one, material);
				// Capsules stand along their local y axis (2 units tall); lay each one flat, pointing along the strand.
				piece.transform.localRotation = Quaternion.LookRotation(to - from, Vector3.up) * Quaternion.Euler(90f, 0f, 0f);
				piece.transform.localScale = new Vector3(width, (Vector3.Distance(from, to) + width) / 2f, thickness);
				theta = next;
			}
		}

		/// <summary>
		/// Pieces strewn loosely around the middle: uneven spacing, distance, size and tilt, so they read as tossed on rather
		/// than laid out. Seeded from the arguments, so every copy of a model gets the same scatter.
		/// </summary>
		private static void Scatter(GameObject group, PrimitiveType type, int count, float startAngle, float innerRadius, float outerRadius, Vector3 scale, float y, Material material)
		{
			var random = new System.Random(count * 7919 + (int)(startAngle * 31f) + (int)(outerRadius * 1000f));
			float Between(float min, float max) => min + (float)random.NextDouble() * (max - min);

			float step = 360f / count;
			for (int i = 0; i < count; i++)
			{
				float angle = (startAngle + i * step + Between(-0.4f, 0.4f) * step) * Mathf.Deg2Rad;
				// Square root spreads pieces evenly over the ring's area instead of bunching them at the inner edge.
				float radius = Mathf.Lerp(innerRadius, outerRadius, Mathf.Sqrt(Between(0f, 1f)));
				var size = scale * Between(0.8f, 1.2f);
				var piece = Prefabs.AddPart(group, $"Piece {i}", type, new Vector3(Mathf.Cos(angle) * radius, y + Between(-0.004f, 0.004f), Mathf.Sin(angle) * radius), size, material);
				piece.transform.localRotation = Quaternion.Euler(Between(-12f, 12f), Between(0f, 360f), Between(-12f, 12f));
			}
		}

		/// <summary>A tejföl cup: tapered towards the bottom, with the cream showing at the top.</summary>
		public static GameObject SourCreamCup(string name)
		{
			var prefab = Prefabs.Create(name);
			Prefabs.AddFrustum(prefab, "Cup", Vector3.zero, 0.14f, 0.2f, 0.13f, Tub);
			Prefabs.AddFrustum(prefab, "Rim", new Vector3(0f, 0.13f, 0f), 0.21f, 0.21f, 0.012f, Tub);
			Prefabs.AddDisc(prefab, "Cream", 0.128f, 0.19f, 0.01f, Cream);
			return prefab;
		}

		public static GameObject SmokedHam(string name)
		{
			var prefab = Prefabs.Create(name);
			Prefabs.AddPart(prefab, "Ham", PrimitiveType.Cube, new Vector3(0f, 0.08f, 0f), new Vector3(0.3f, 0.16f, 0.2f), Ham);
			// Dark smoked rind on top and down the sides.
			Prefabs.AddPart(prefab, "Rind", PrimitiveType.Cube, new Vector3(0f, 0.165f, 0f), new Vector3(0.31f, 0.025f, 0.21f), HamRind);
			Prefabs.AddPart(prefab, "Rind Side", PrimitiveType.Cube, new Vector3(0f, 0.09f, 0.1f), new Vector3(0.31f, 0.15f, 0.015f), HamRind);
			return prefab;
		}

		public static GameObject ChoppedHam(string name)
		{
			var prefab = Prefabs.Create(name);
			// A loose heap: a wide bottom layer and a few pieces tumbled on top.
			Scatter(prefab, PrimitiveType.Cube, 7, 10f, 0.02f, 0.1f, new Vector3(0.06f, 0.045f, 0.06f), 0.023f, Ham);
			var top = new GameObject("Top");
			top.transform.SetParent(prefab.transform, false);
			Scatter(top, PrimitiveType.Cube, 3, 75f, 0f, 0.045f, new Vector3(0.055f, 0.045f, 0.055f), 0.06f, Ham);
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

			var rim = new GameObject("Rim");
			rim.transform.SetParent(langos.transform, false);
			for (int i = 0; i < LangosRim.Puffs; i++)
				Prefabs.AddPart(rim, $"Puff {i}", PrimitiveType.Sphere, Vector3.zero, Vector3.one, FriedEdge);

			// Every lángos shares this one lumpy shape; each spawned copy turns it to a random angle (see LangosRim.Awake).
			LangosRim.Shape(rim.transform, new System.Random(1957));
			rim.AddComponent<LangosRim>();
			return langos;
		}
	}

	/// <summary>
	/// Turns each fried lángos' rim to a random angle when the game spawns it. The shape itself stays fixed, because the
	/// pan, hand and plate each spawn their own copy and a reshaped rim would visibly jump between those steps.
	/// </summary>
	internal class LangosRim : MonoBehaviour
	{
		public const int Puffs = 14;
		private const float PuffSize = 0.11f;
		private const float PuffHeight = 0.07f;

		// Only runs on spawned copies: the prefab template sits under an inactive root.
		private void Awake() => transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

		/// <summary>Hand-stretched dough puffs up unevenly, so each puff gets its own size, height, spacing and reach.</summary>
		public static void Shape(Transform rim, System.Random random)
		{
			float Vary(float value, float amount) => value * (1f + ((float)random.NextDouble() * 2f - 1f) * amount);

			float rimRadius = (LangosModels.LangosDiameter - PuffSize) / 2f;
			for (int i = 0; i < rim.childCount && i < Puffs; i++)
			{
				float angle = (i + Vary(1f, 0.35f) - 1f) * Mathf.PI * 2f / Puffs;
				float radius = Vary(rimRadius, 0.08f);
				float width = Vary(PuffSize, 0.3f);
				float height = Vary(PuffHeight, 0.3f);

				var puff = rim.GetChild(i);
				puff.localPosition = new Vector3(Mathf.Cos(angle) * radius, height / 2f, Mathf.Sin(angle) * radius);
				// Stretched along the rim so neighbouring puffs blend into a bumpy ring.
				puff.localScale = new Vector3(width, height, Vary(width, 0.25f) * 1.3f);
				puff.localRotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg, 0f);
			}
		}
	}

	/// <summary>The model parts of a plated lángos. Names double as the child object names.</summary>
	public enum LangosTopping { SourCream, Garlic, Cheese, Onion, Pickles, Ham, Chocolate, Nuts }
}
