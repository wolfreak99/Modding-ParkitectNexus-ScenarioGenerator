namespace ScenarioGenerator
{
	[System.Flags]
	public enum GenerateFlags
	{
		None = 0,
		Height = 1 << 0,
		TerrainType = 1 << 1,
		Water = 1 << 2,
		Trees = 1 << 3,
		All = Height | TerrainType | Water | Trees
	}
}
