namespace NKM;

public sealed class NKMBackgroundConst
{
	public sealed class BackgroundUnitConst
	{
		public int UnitType { get; private set; } = 2;

		public float UnitSize { get; private set; }

		public int UnitFace { get; private set; }

		public float UnitPosX { get; private set; }

		public float UnitPosY { get; private set; }

		public bool UnitBackImage { get; private set; }

		public int UnitSkinOption { get; private set; }

		public float UnitRotation { get; private set; }

		public bool UnitFlip { get; private set; }

		public float UnitAnimTime { get; private set; }

		public void Load(NKMLua lua, int index)
		{
			using (lua.OpenTable($"UnitSlot{index + 1}", $"[NKMBackgroundConst] open subTable 'UnitSlot{index + 1}' failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMBackgroundConst.cs", 49))
			{
				UnitPosX = lua.GetFloat("unitPosX");
				UnitPosY = lua.GetFloat("unitPosY");
				UnitSize = lua.GetFloat("unitSize");
			}
		}
	}

	private const int BackgroundUnitCount = 3;

	private BackgroundUnitConst[] unitSlot = new BackgroundUnitConst[3];

	public int DefaultBackgroundItem { get; private set; }

	public BackgroundUnitConst GetBackgroundUnitSlot(int index)
	{
		return unitSlot[index];
	}

	public void Load(NKMLua lua)
	{
		using (lua.OpenTable("NKMBackgroundUnitInfo", "[NKMBackgroundConst] loading BackgroundUnitInfo table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMBackgroundConst.cs", 18))
		{
			for (int i = 0; i < 3; i++)
			{
				BackgroundUnitConst backgroundUnitConst = new BackgroundUnitConst();
				backgroundUnitConst.Load(lua, i);
				unitSlot[i] = backgroundUnitConst;
			}
		}
		using (lua.OpenTable("LobbyDefaultBackground", "[NKMBackgroundConst] loading LobbyDefaultBackground table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMBackgroundConst.cs", 28))
		{
			DefaultBackgroundItem = lua.GetInt32("DefaultBackgroundItem");
		}
	}
}
