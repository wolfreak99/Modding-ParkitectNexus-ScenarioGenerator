using System.Collections.Generic;
using MiniJSON;

namespace ScenarioGenerator
{
	public class ValueStore
	{
		private string previousKey = "";
		public bool hasValueStoreChange {
			get {
				var key = Serialize();
				if (this.previousKey != key) {
					previousKey = key;
					return true;
				}
				return false;
			}
		}

		private const string PLAIN_SCALE = "p";
		private const string MAX_HEIGHT = "m_h";
		private const string MAX_DEPTH = "m_d";
		private const string DITCH_RATIO = "d";
		private const string FLOOD_ROUND = "f";
		private const string ENTERANCE_CLERANCE = "e";
		private const string GENERATE_TERRAIN_TYPE = "g";
		private const string TERRAIN_SCALE = "t";
		private const string TREE_COUNT = "t_c";
		private const string SEED = "s";

		public string Seed { get; set; }
		public float DitchRatio { get; set; }
		public float EntranceClearance { get; set; }
		public float FloodRounds { get; set; }
		public bool GenerateTerrainType { get; set; }
		public float MaxDepth { get; set; }
		public float MaxHeight { get; set; }

		public float PlainScale { get; set; }
		public float TerrainScale { get; set; }
		public float TreeCount { get; set; }

		public ValueStore()
		{
			DitchRatio = .6f;
			EntranceClearance = 7;
			FloodRounds = 30;
			GenerateTerrainType = true;
			MaxDepth = 4f;
			MaxHeight = 3f;
			Seed = "test";
			PlainScale = .15f;
			TerrainScale = .5f;
			TreeCount = 100f;
		}

		public string Serialize()
		{
			Dictionary<string, object> value = new Dictionary<string, object>();
			value.Add(PLAIN_SCALE, PlainScale);
			value.Add(MAX_HEIGHT, MaxHeight);
			value.Add(MAX_DEPTH, MaxDepth);
			value.Add(DITCH_RATIO, DitchRatio);
			value.Add(FLOOD_ROUND, FloodRounds);
			value.Add(ENTERANCE_CLERANCE, EntranceClearance);
			value.Add(GENERATE_TERRAIN_TYPE, GenerateTerrainType);
			value.Add(TERRAIN_SCALE, TerrainScale);
			value.Add(TREE_COUNT, TreeCount);
			value.Add(SEED, Seed);
			return Json.Serialize(value);
		}

		public void DeSerialize(string data)
		{
			Dictionary<string, object> value = (Dictionary<string, object>)Json.Deserialize(data);
			PlainScale = (float)(double)value[PLAIN_SCALE];
			MaxHeight = (float)(double)value[MAX_HEIGHT];
			MaxDepth = (float)(double)value[MAX_DEPTH];
			DitchRatio = (float)(double)value[DITCH_RATIO];
			FloodRounds = (float)(double)value[FLOOD_ROUND];
			EntranceClearance = (float)(double)value[ENTERANCE_CLERANCE];
			GenerateTerrainType = (bool)value[GENERATE_TERRAIN_TYPE];
			TerrainScale = (float)(double)value[TERRAIN_SCALE];
			TreeCount = (float)(double)value[TREE_COUNT];
			Seed = (string)value[SEED];
		}
	}
}
