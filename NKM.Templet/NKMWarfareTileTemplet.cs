using System.Collections.Generic;
using Cs.Logging;
using Cs.Math;
using NKM.Warfare;

namespace NKM.Templet;

public sealed class NKMWarfareTileTemplet
{
	public sealed class BattleConditionChange
	{
		public WARFARE_GAME_CONDITION condition;

		public int value;

		public string strId;

		internal NKMBattleConditionTemplet templet;

		public int BattleConditionId => templet.BattleCondID;

		public static BattleConditionChange Create(NKMLua lua)
		{
			if (!lua.OpenTable("BattleConditionChange"))
			{
				return null;
			}
			BattleConditionChange battleConditionChange = new BattleConditionChange();
			lua.GetData("Condition", ref battleConditionChange.condition);
			lua.GetData("Value", ref battleConditionChange.value);
			lua.GetData("StrID", ref battleConditionChange.strId);
			lua.CloseTable();
			return battleConditionChange;
		}

		public BattleConditionChange Clone()
		{
			return MemberwiseClone() as BattleConditionChange;
		}

		public bool BeSatisfied(int phase)
		{
			if (condition == WARFARE_GAME_CONDITION.WFC_PHASE)
			{
				return phase == value;
			}
			return false;
		}

		public void Validate(NKMWarfareMapTemplet mapTemplet)
		{
			if (condition == WARFARE_GAME_CONDITION.WFC_PHASE && value <= 0)
			{
				Log.ErrorAndExit($"[{mapTemplet.m_WarfareMapStrID}] phase 조건 변경시 value는 1보다 같거나 커야 합니다. value:{value}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.BattleConditionChange.cs", 43);
			}
		}
	}

	public sealed class DungeonChange
	{
		public WARFARE_GAME_CONDITION condition;

		public int value;

		public string strID;

		public NKM_WARFARE_ENEMY_ACTION_TYPE actionType;

		public bool flag;

		public bool target;

		public static DungeonChange Create(NKMLua lua)
		{
			if (!lua.OpenTable("DungeonChange"))
			{
				return null;
			}
			DungeonChange dungeonChange = new DungeonChange();
			lua.GetData("Condition", ref dungeonChange.condition);
			lua.GetData("Value", ref dungeonChange.value);
			lua.GetData("StrID", ref dungeonChange.strID);
			lua.GetData("ActionType", ref dungeonChange.actionType);
			lua.GetData("Flag", ref dungeonChange.flag);
			lua.GetData("Target", ref dungeonChange.target);
			lua.CloseTable();
			return dungeonChange;
		}

		public DungeonChange Clone()
		{
			return MemberwiseClone() as DungeonChange;
		}
	}

	public sealed class TileChange
	{
		public WARFARE_GAME_CONDITION condition;

		public int value;

		public NKM_WARFARE_MAP_TILE_TYPE tileType;

		public string summonDungeonStrID;

		public NKM_WARFARE_ENEMY_ACTION_TYPE summonDungeonActionType;

		public WARFARE_GAME_CONDITION winTileType;

		public WARFARE_GAME_CONDITION loseTileType;

		public int rate;

		public static TileChange Create(NKMLua lua)
		{
			if (!lua.OpenTable("TileChange"))
			{
				return null;
			}
			TileChange tileChange = new TileChange();
			lua.GetData("Condition", ref tileChange.condition);
			lua.GetData("Value", ref tileChange.value);
			lua.GetData("Type", ref tileChange.tileType);
			lua.GetData("Rate", ref tileChange.rate);
			lua.GetData("SummonDGStrID", ref tileChange.summonDungeonStrID);
			lua.GetData("SummonDGActionType", ref tileChange.summonDungeonActionType);
			lua.GetData("WinType", ref tileChange.winTileType);
			lua.GetData("LoseType", ref tileChange.loseTileType);
			lua.CloseTable();
			return tileChange;
		}

		public TileChange Clone()
		{
			return MemberwiseClone() as TileChange;
		}

		public bool BeSatisfied(int phase)
		{
			if (condition == WARFARE_GAME_CONDITION.WFC_PHASE && value == phase)
			{
				return RandomGenerator.Next(100) < rate;
			}
			return false;
		}

		public void Validate(NKMWarfareMapTemplet mapTemplet)
		{
			if (tileType == NKM_WARFARE_MAP_TILE_TYPE.NWNTT_CHEST && (condition != WARFARE_GAME_CONDITION.WFC_PHASE || value != 0))
			{
				Log.ErrorAndExit($"[{mapTemplet.m_WarfareMapStrID}] 컨테이터타일 타입변경은 0페이즈 조건 만으로 제한합니다. condition:{condition} value:{value}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.TileChange.cs", 56);
			}
			if (condition == WARFARE_GAME_CONDITION.WFC_PHASE && value < 0)
			{
				Log.ErrorAndExit($"[{mapTemplet.m_WarfareMapStrID}] phase 조건 변경시 value는 0보다 같거나 커야 합니다. value:{value}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.TileChange.cs", 61);
			}
		}
	}

	public struct PhaseState
	{
		public NKM_WARFARE_MAP_TILE_TYPE TileType { get; internal set; }

		public int BattleConditionId { get; internal set; }

		public PhaseState(NKM_WARFARE_MAP_TILE_TYPE type, int battleConditionId)
		{
			TileType = type;
			BattleConditionId = battleConditionId;
		}
	}

	public const int IncrementalTileLimitPhase = 10;

	private readonly List<short> neighborTiles = new List<short>();

	private NKMDungeonTemplet dungeon;

	private NKMDungeonTemplet summonDungeon;

	public readonly string mapStringId;

	public TilePosition position;

	public int pointIndex;

	public NKM_WARFARE_MAP_TILE_TYPE m_TileType = NKM_WARFARE_MAP_TILE_TYPE.NWMTT_NORMAL;

	public WARFARE_GAME_CONDITION m_TileWinType;

	public WARFARE_GAME_CONDITION m_TileLoseType;

	public string m_DungeonStrID;

	public bool m_bFlagDungeon;

	public bool m_bTargetUnit;

	public NKM_WARFARE_ENEMY_ACTION_TYPE m_NKM_WARFARE_ENEMY_ACTION_TYPE;

	public int m_enemyActionValue;

	public NKM_WARFARE_SPAWN_POINT_TYPE m_NKM_WARFARE_SPAWN_POINT_TYPE;

	public WARFARE_SUMMON_CONDITION m_SummonCondition;

	public byte m_SummonConditionValue;

	public string m_SummonDungeonStrID;

	public string m_BattleConditionStrID;

	public DungeonChange dungeonChange;

	public TileChange tileChange;

	public BattleConditionChange battleConditionChange;

	public NKMBattleConditionTemplet BattleCondition { get; private set; }

	public IEnumerable<short> NeighborTiles => neighborTiles;

	public NKMWarfareTileTemplet(string mapStringId, List<short> neighbors)
	{
		this.mapStringId = mapStringId;
		if (neighbors != null)
		{
			neighborTiles = neighbors;
		}
	}

	public bool LoadFromLUA(NKMLua cNKMLua, int index)
	{
		if (!cNKMLua.OpenTable("NKMWarfareTile" + index))
		{
			return false;
		}
		cNKMLua.GetData("m_TileType", ref m_TileType);
		cNKMLua.GetData("m_TileWinType", ref m_TileWinType);
		cNKMLua.GetData("m_TileLoseType", ref m_TileLoseType);
		cNKMLua.GetData("m_DungeonStrID", ref m_DungeonStrID);
		cNKMLua.GetData("m_bFlagDungeon", ref m_bFlagDungeon);
		cNKMLua.GetData("m_bTargetUnit", ref m_bTargetUnit);
		cNKMLua.GetData("m_NKM_WARFARE_ENEMY_ACTION_TYPE", ref m_NKM_WARFARE_ENEMY_ACTION_TYPE);
		cNKMLua.GetData("m_enemyActionValue", ref m_enemyActionValue);
		cNKMLua.GetData("m_NKM_WARFARE_SPAWN_POINT_TYPE", ref m_NKM_WARFARE_SPAWN_POINT_TYPE);
		cNKMLua.GetData("m_SummonCondition", ref m_SummonCondition);
		cNKMLua.GetData("m_SummonConditionValue", ref m_SummonConditionValue);
		cNKMLua.GetData("m_SummonDungeonStrID", ref m_SummonDungeonStrID);
		cNKMLua.GetData("m_BattleConditionStrID", ref m_BattleConditionStrID);
		dungeonChange = DungeonChange.Create(cNKMLua);
		tileChange = TileChange.Create(cNKMLua);
		battleConditionChange = BattleConditionChange.Create(cNKMLua);
		cNKMLua.CloseTable();
		if (m_BattleConditionStrID != null)
		{
			BattleCondition = NKMBattleConditionManager.GetTempletByStrID(m_BattleConditionStrID);
			if (BattleCondition == null)
			{
				Log.ErrorAndExit("battle condition id가 유효하지 않음. strId:" + m_BattleConditionStrID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.cs", 80);
			}
		}
		return true;
	}

	public bool HasContainer()
	{
		if (m_TileType != NKM_WARFARE_MAP_TILE_TYPE.NWNTT_CHEST)
		{
			if (tileChange != null)
			{
				return tileChange.tileType == NKM_WARFARE_MAP_TILE_TYPE.NWNTT_CHEST;
			}
			return false;
		}
		return true;
	}

	public bool HasIncrType()
	{
		if (m_TileType != NKM_WARFARE_MAP_TILE_TYPE.NWMTT_INCR)
		{
			if (tileChange != null)
			{
				return tileChange.tileType == NKM_WARFARE_MAP_TILE_TYPE.NWMTT_INCR;
			}
			return false;
		}
		return true;
	}

	public bool NeedQuestionMark()
	{
		if (tileChange != null)
		{
			return tileChange.tileType == NKM_WARFARE_MAP_TILE_TYPE.NWNTT_CHEST;
		}
		return false;
	}

	public void DeepCopyFromSource(NKMWarfareTileTemplet source)
	{
		m_TileType = source.m_TileType;
		m_TileWinType = source.m_TileWinType;
		m_TileLoseType = source.m_TileLoseType;
		m_DungeonStrID = source.m_DungeonStrID;
		m_bFlagDungeon = source.m_bFlagDungeon;
		m_bTargetUnit = source.m_bTargetUnit;
		m_NKM_WARFARE_ENEMY_ACTION_TYPE = source.m_NKM_WARFARE_ENEMY_ACTION_TYPE;
		m_enemyActionValue = source.m_enemyActionValue;
		m_NKM_WARFARE_SPAWN_POINT_TYPE = source.m_NKM_WARFARE_SPAWN_POINT_TYPE;
		m_SummonDungeonStrID = source.m_SummonDungeonStrID;
		m_SummonCondition = source.m_SummonCondition;
		m_SummonConditionValue = source.m_SummonConditionValue;
		m_BattleConditionStrID = source.m_BattleConditionStrID;
		dungeonChange = source.dungeonChange?.Clone();
		tileChange = source.tileChange?.Clone();
		battleConditionChange = source.battleConditionChange?.Clone();
	}

	public PhaseState GetPhaseState(int phase)
	{
		PhaseState result = new PhaseState
		{
			BattleConditionId = (BattleCondition?.BattleCondID ?? 0),
			TileType = m_TileType
		};
		if (battleConditionChange != null && battleConditionChange.BeSatisfied(phase))
		{
			result.BattleConditionId = battleConditionChange.BattleConditionId;
		}
		if (tileChange != null && tileChange.BeSatisfied(phase) && tileChange.tileType != m_TileType)
		{
			result.TileType = tileChange.tileType;
		}
		return result;
	}

	public bool CheckTileTypeChange(int phase, out NKM_WARFARE_MAP_TILE_TYPE tileType)
	{
		if (tileChange == null || !tileChange.BeSatisfied(phase))
		{
			tileType = NKM_WARFARE_MAP_TILE_TYPE.NWMTT_DISABLE;
			return false;
		}
		tileType = tileChange.tileType;
		return true;
	}

	public bool CheckBattleConditionChange(int phase, out NKMBattleConditionTemplet templet)
	{
		if (battleConditionChange == null || !battleConditionChange.BeSatisfied(phase))
		{
			templet = null;
			return false;
		}
		templet = battleConditionChange.templet;
		return true;
	}

	public void JoinServerOnly()
	{
		if (battleConditionChange != null)
		{
			battleConditionChange.templet = NKMBattleConditionManager.GetTempletByStrID(battleConditionChange.strId);
			if (battleConditionChange.templet == null)
			{
				Log.ErrorAndExit("spawn battle condition id가 유효하지 않음. strId:" + battleConditionChange.strId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.cs", 178);
			}
		}
	}

	public void ValidateServerOnly(NKMWarfareMapTemplet mapTemplet)
	{
		tileChange?.Validate(mapTemplet);
		battleConditionChange?.Validate(mapTemplet);
		if (m_bTargetUnit && string.IsNullOrEmpty(m_DungeonStrID))
		{
			Log.ErrorAndExit("[" + mapStringId + "] 타겟이 지정된 타일은 던전 아이디를 입력해야 합니다. position:" + position, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.cs", 190);
		}
		if (HasIncrType())
		{
			if (string.IsNullOrEmpty(m_SummonDungeonStrID))
			{
				Log.ErrorAndExit("[" + mapStringId + "] 던전 아이디가 설정되지 않았습니다. position:" + position, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.cs", 197);
			}
			if (m_SummonCondition == WARFARE_SUMMON_CONDITION.PHASE && m_SummonConditionValue == 0)
			{
				Log.ErrorAndExit("[" + mapStringId + "] 소환조건 PHASE 타일은 값이 1보다 커야 합니다.position:" + position, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.cs", 202);
			}
		}
		if (m_TileType == NKM_WARFARE_MAP_TILE_TYPE.NWMTT_INCR && (m_bTargetUnit || m_bFlagDungeon))
		{
			Log.ErrorAndExit("[" + mapStringId + "] 증원타일은 flagship 혹은 target 설정이 불가합니다. position:" + position, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.cs", 209);
		}
		if (!string.IsNullOrEmpty(m_DungeonStrID))
		{
			dungeon = NKMDungeonManager.GetDungeonTemplet(m_DungeonStrID);
			if (dungeon == null)
			{
				Log.ErrorAndExit("NKMWarfareTile, Cannot find Dungeon. DungeonStrID:" + m_DungeonStrID + " MapStrID:" + mapStringId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.cs", 217);
			}
			else if (dungeon.m_DungeonTempletBase.StageTemplet != null)
			{
				Log.ErrorAndExit("[WarfareTile] dungeon cannot be a stage dungeon. SummonDungeonStrID:" + m_DungeonStrID + " MapStrID:" + mapStringId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.cs", 222);
			}
		}
		if (!string.IsNullOrEmpty(m_SummonDungeonStrID))
		{
			summonDungeon = NKMDungeonManager.GetDungeonTemplet(m_SummonDungeonStrID);
			if (summonDungeon == null)
			{
				Log.ErrorAndExit("NKMWarfareTile, Cannot find SummonDungeonStrID:" + m_SummonDungeonStrID + " MapStrID:" + mapStringId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.cs", 231);
			}
			else if (summonDungeon.m_DungeonTempletBase.StageTemplet != null)
			{
				Log.ErrorAndExit("[WarfareTile] summon dungeon cannot be a stage dungeon. SummonDungeonStrID:" + m_SummonDungeonStrID + " MapStrID:" + mapStringId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.cs", 236);
			}
		}
		if (m_TileType == NKM_WARFARE_MAP_TILE_TYPE.NWMTT_DISABLE && m_NKM_WARFARE_SPAWN_POINT_TYPE != NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_NONE)
		{
			Log.ErrorAndExit($"NKMWarfareTile, disable tile has spawn value:{m_NKM_WARFARE_SPAWN_POINT_TYPE} MapStrID:{mapStringId} position:{position.ToString()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.cs", 243);
		}
	}

	public NKMDungeonTemplet GetDungeonTemplet(bool summon)
	{
		if (!summon)
		{
			return dungeon;
		}
		return summonDungeon;
	}

	public NKM_ERROR_CODE CheckSpawnable(NKMUnitTempletBase shipTemplet)
	{
		if (m_TileType == NKM_WARFARE_MAP_TILE_TYPE.NWMTT_DISABLE)
		{
			return NKMError.Build(NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_SET_UNIT_ON_DISABLE_TILE, $"tileIndex:{position.Index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.cs", 253);
		}
		if (m_DungeonStrID != null)
		{
			return NKMError.Build(NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_POSITION_BY_DUNGEON, $"tileIndex:{position.Index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.cs", 258);
		}
		if (shipTemplet == null)
		{
			return NKMError.Build(NKM_ERROR_CODE.NEC_FAIL_WARFARE_SHIP_DATA_NOT_FOUND, $"tileIndex:{position.Index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.cs", 263);
		}
		if (m_NKM_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_NONE)
		{
			return NKMError.Build(NKM_ERROR_CODE.NEC_FAIL_WARFARE_TRYING_SPAWN_TO_INVLID_TILE, $"tileIndex:{position.Index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.cs", 268);
		}
		if (m_NKM_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT && shipTemplet.m_NKM_UNIT_STYLE_TYPE != NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT)
		{
			return NKMError.Build(NKM_ERROR_CODE.NEC_FAIL_WARFARE_GAME_CANNOT_ASSAULT_POSITION, $"tileIndex:{position.Index}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareTileTemplet.cs", 274);
		}
		return NKM_ERROR_CODE.NEC_OK;
	}
}
