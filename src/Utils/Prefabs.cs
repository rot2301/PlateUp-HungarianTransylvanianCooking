using KitchenLib.Materials;
using System.Collections.Generic;
using UnityEngine;

namespace HungarianTransylvanianCooking.Utils
{
	/// <summary>
	/// Builds placeholder models out of Unity primitives so the mod is playable without a Unity asset bundle.
	/// Swap these for real models later by loading prefabs from a bundle instead.
	/// </summary>
	internal static class Prefabs
	{
		private static GameObject _container;
		private static readonly Dictionary<string, Material> _materials = new();

		// Prefabs live under an inactive root so the templates themselves never render in the world;
		// the game instantiates copies, which are active because each template is activeSelf.
		private static Transform Container
		{
			get
			{
				if (_container == null)
				{
					_container = new GameObject($"{MOD_NAME} Prefabs");
					_container.SetActive(false);
					Object.DontDestroyOnLoad(_container);
				}
				return _container.transform;
			}
		}

		public static GameObject Create(string name)
		{
			var prefab = new GameObject(name);
			prefab.transform.SetParent(Container, false);
			return prefab;
		}

		public static GameObject AddPart(GameObject parent, string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material)
		{
			var part = GameObject.CreatePrimitive(type);
			part.name = name;
			Object.DestroyImmediate(part.GetComponent<Collider>());
			part.transform.SetParent(parent.transform, false);
			part.transform.localPosition = position;
			part.transform.localScale = scale;
			part.GetComponent<MeshRenderer>().sharedMaterial = material;
			return part;
		}

		/// <summary>A flat disc (squashed cylinder). Unity's cylinder is 2 units tall, hence the halved height.</summary>
		public static GameObject AddDisc(GameObject parent, string name, float y, float diameter, float thickness, Material material)
			=> AddPart(parent, name, PrimitiveType.Cylinder, new Vector3(0f, y + thickness / 2f, 0f), new Vector3(diameter, thickness / 2f, diameter), material);

		/// <summary>Uses a vanilla material when it exists, otherwise a flat colour with PlateUp's own shader.</summary>
		public static Material Material(string vanillaName, int fallbackHex)
		{
			var vanilla = MaterialManager.GetMaterial(vanillaName, includeCustomMaterials: false);
			return vanilla != null ? vanilla : Flat(vanillaName, fallbackHex);
		}

		public static Material Flat(string name, int hex)
		{
			string key = $"HTC - {name}";
			if (_materials.TryGetValue(key, out var cached))
				return cached;

			var colour = new Color(((hex >> 16) & 0xFF) / 255f, ((hex >> 8) & 0xFF) / 255f, (hex & 0xFF) / 255f, 0f);
			var material = new Material(Shader.Find("Simple Flat")) { name = key };
			material.SetColor("_Color0", colour);
			material.SetFloat("_Shininess", 0f);
			material.SetFloat("_OverlayScale", 10f);
			_materials[key] = material;
			return material;
		}
	}
}
