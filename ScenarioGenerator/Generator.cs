using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SRandom = System.Random;

namespace ScenarioGenerator
{
	public class Generator
	{
		private Park park;

		private const float MIN_ROAD_WIDTH = 7f;
		private const float GROUND_HEIGHT = 3.0f;
		private const float WATTER_OFFSET = 0.05f;
		private const float PARK_FENCE_OFFSET = 2f;
		private const int DEFAULT_TERRAIN_TYPE = 0;
		private const float DEF_HEIGHT = 3;

		private SRandom _waterRandom;
		private SRandom _treeRandom;

		public Generator(Park park)
		{
			this.park = park;
		}

		public void Reset()
		{
			if (park == null) {
				return;
			}

			for (var x = 0; x < park.xSize; x++) {
				for (var z = 0; z < park.zSize; z++) {
					var patch = park.getTerrain(x, z);

					// Remove water if present.
					if (patch.hasWater()) {
						WaterFlooding.unflood(new Vector3(x, 0, z));
					}

					// Reset height.
					for (var i = 0; i < 4; i++) {
						var d = GROUND_HEIGHT - patch.h[i];
						if (d != 0) {
							patch.changeHeight(i, d, true);
						}
					}

					// Reset terrain type.
					patch.TerrainType = DEFAULT_TERRAIN_TYPE;
				}
			}
			
			foreach (var o in ConfigWindow.FindObjectsOfType<TreeEntity>()) {
				o.Kill();
			}
		}

		public void Generate(ValueStore keyStore)
		{
			Reset();

			if (park == null) {
				return;
			}

			var seed = 0;
			// Generate a random seed.
			foreach (char c in keyStore.Seed) {
				seed += (int)c;
			}

			_waterRandom = new SRandom(seed);
			_treeRandom = new SRandom(seed);
			
			for (var z = 0; z <= park.zSize; z++) {
				for (var x = 0; x <= park.xSize; x++) {
					// Calculate height of terrain patch based on perlin noise.
					var y = (Mathf.PerlinNoise(
						x / (park.xSize * keyStore.PlainScale) + seed,
						z / (park.zSize * keyStore.PlainScale) + seed
					) * (1 + keyStore.DitchRatio)) - (float)keyStore.DitchRatio;
					if (y < 0 && keyStore.DitchRatio != 0) {
						y /= keyStore.DitchRatio;
					}

					y = y * (y < 0 ? keyStore.MaxDepth : (float)keyStore.MaxHeight);
					y = y > 0 ? Mathf.FloorToInt(y) : Mathf.CeilToInt(y);

					// Generate terrain type for the terrain patch based on perlin noise.
					if (keyStore.GenerateTerrainType) {
						var patch = park.getTerrain(x, z);
						if (patch != null) {
							var types = ScriptableSingleton<AssetManager>.Instance.terrainTypes.Length;
							var terrainTypeIndex = Mathf.PerlinNoise(
								x / (park.xSize * keyStore.TerrainScale) + seed,
								z / (park.xSize * keyStore.TerrainScale) + seed
							);

							patch.TerrainType = Mathf.FloorToInt(Mathf.Abs(terrainTypeIndex - 0.5f) * types);
						}
					}

					// Limit heights near road.
					var roadWidth = Mathf.Max(MIN_ROAD_WIDTH, keyStore.EntranceClearance);
					if (x < roadWidth) {
						continue;
					}

					y = Mathf.Clamp(y, roadWidth - x, x - roadWidth);

					// Limit heights near entrance.
					var distanceToEntrance = Vector3.Distance(
						park.parkEntrances.First().transform.position,
						new Vector3(x, GROUND_HEIGHT, z)
					);

					if (distanceToEntrance < roadWidth) {
						continue;
					}

					// If this location should be raised, change the hight of the patch and the ones around.
					if (y != 0) {
						for (var cornerIndex = 0; cornerIndex < 4; cornerIndex++) {
							var ox = cornerIndex == 1 || cornerIndex == 2 ? 1 : 0;
							var oz = cornerIndex == 2 || cornerIndex == 3 ? 1 : 0;

							var patch = park.getTerrain(x - ox, z - oz);

							if (patch != null) {
								var current = patch.h[cornerIndex] - DEF_HEIGHT;
								patch.smoothChangeHeight(park, cornerIndex, y - current, true);
							}
						}
					}
				}
			}

			// Randomly flood the map.
			for (var i = 0; i < keyStore.FloodRounds; i++) {
				var x = Mathf.RoundToInt(_waterRandom.NextFloat(keyStore.EntranceClearance, park.xSize));
				var z = Mathf.RoundToInt(_waterRandom.NextFloat(0, park.zSize));

				var patch = park.getTerrain(x, z);
				if (patch == null || patch.hasWater() || patch.getLowestHeight() >= GROUND_HEIGHT) {
					continue;
				}

				WaterFlooding.flood(new Vector3(x, GROUND_HEIGHT - WATTER_OFFSET, z));
			}

			// Randomly spawn a forrest.
			for (var i = 0; i < keyStore.TreeCount; i++) {
				var x = Mathf.RoundToInt(_treeRandom.NextFloat(keyStore.EntranceClearance, park.xSize - PARK_FENCE_OFFSET));
				var z = Mathf.RoundToInt(_treeRandom.NextFloat(PARK_FENCE_OFFSET, park.zSize - PARK_FENCE_OFFSET));

				var patch = park.getTerrain(x, z);
				if (patch == null || patch.hasWater()) {
					continue;
				}

				var y = patch.getHighestHeight();
				if (y != patch.getLowestHeight()) {
					continue;
				}

				TreeEntity fir = null;
				foreach (var o in ScriptableSingleton<AssetManager>.Instance.getDecoObjects()) {
					if (o.getName().StartsWith("Fir") && o is TreeEntity) {
						fir = o as TreeEntity;
					}
				}

				if (fir != null) {
					var tree = ConfigWindow.Instantiate(fir);
					tree.transform.position = new Vector3(x, y, z);
					tree.transform.forward = Vector3.forward;
					tree.Initialize();
					tree.startAnimateSpawn(Vector3.zero);
				}
			}
		}
	}
}
