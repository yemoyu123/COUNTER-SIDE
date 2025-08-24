using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Warfare;

namespace NKM.Templet;

public sealed class NKMWarfareMapTemplet
{
	private readonly List<NKMWarfareTileTemplet> tiles = new List<NKMWarfareTileTemplet>();

	private IReadOnlyList<NKMWarfareTileTemplet> validTiles;

	private IReadOnlyList<NKMWarfareTileTemplet> diveTiles;

	private IReadOnlyList<NKMWarfareTileTemplet> assultTiles;

	public const int MAX_MAP_SIZE_X = 10;

	public const int MAX_MAP_SIZE_Y = 7;

	public string m_WarfareMapStrID = "";

	public int m_MapSizeX;

	public int m_MapSizeY;

	public IReadOnlyList<NKMWarfareTileTemplet> Tiles => tiles;

	public int TileCount => tiles.Count;

	public int GetPosXByIndex(int index)
	{
		return index % m_MapSizeX;
	}

	public int GetPosYByIndex(int index)
	{
		return index / m_MapSizeX;
	}

	public TilePosition GetTilePosition(int index)
	{
		return new TilePosition((byte)(index % m_MapSizeX), (byte)(index / m_MapSizeX), (byte)index);
	}

	public int GetSpawnPointCountByType(NKM_WARFARE_SPAWN_POINT_TYPE type)
	{
		return validTiles.Count((NKMWarfareTileTemplet tile) => tile.m_NKM_WARFARE_SPAWN_POINT_TYPE == type);
	}

	public int GetDivePointTileCount()
	{
		return diveTiles.Count;
	}

	public int GetDivePointTileIndex(int pointIndex)
	{
		if (pointIndex < 0 || pointIndex >= diveTiles.Count)
		{
			return -1;
		}
		return diveTiles[pointIndex].position.Index;
	}

	public int GetDivePointIndex(int tileIndex)
	{
		NKMWarfareTileTemplet tile = GetTile(tileIndex);
		if (tile == null || tile.m_NKM_WARFARE_SPAWN_POINT_TYPE != NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_DIVE)
		{
			return -1;
		}
		return tile.pointIndex;
	}

	public int GetAssultPointTileIndex(int pointIndex)
	{
		if (pointIndex < 0 || pointIndex >= assultTiles.Count)
		{
			return -1;
		}
		return assultTiles[pointIndex].position.Index;
	}

	public int GetAssultPointIndex(int tileIndex)
	{
		NKMWarfareTileTemplet tile = GetTile(tileIndex);
		if (tile == null || tile.m_NKM_WARFARE_SPAWN_POINT_TYPE != NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT)
		{
			return -1;
		}
		return tile.pointIndex;
	}

	public short GetIndexByPos(int posX, int posY)
	{
		if (posX < 0 || posX >= m_MapSizeX || posY < 0 || posY >= m_MapSizeY)
		{
			return -1;
		}
		return (short)(posX + posY * m_MapSizeX);
	}

	public NKMWarfareTileTemplet GetTile(int posX, int posY)
	{
		int indexByPos = GetIndexByPos(posX, posY);
		if (indexByPos == -1)
		{
			return null;
		}
		return tiles[indexByPos];
	}

	public NKMWarfareTileTemplet GetTile(int index)
	{
		if (index < 0 || index >= tiles.Count)
		{
			return null;
		}
		return tiles[index];
	}

	public IEnumerable<string> GetDungeonStrIDList()
	{
		return from tile in tiles
			where tile.m_DungeonStrID != null
			select tile.m_DungeonStrID;
	}

	public string GetFlagDungeonStrID()
	{
		NKMWarfareTileTemplet nKMWarfareTileTemplet = tiles.FirstOrDefault((NKMWarfareTileTemplet tile) => tile.m_DungeonStrID != null && tile.m_bFlagDungeon);
		if (nKMWarfareTileTemplet == null)
		{
			return string.Empty;
		}
		return nKMWarfareTileTemplet.m_DungeonStrID;
	}

	public short GetWinTileIndexByWinType(WARFARE_GAME_CONDITION winType)
	{
		if (winType == WARFARE_GAME_CONDITION.WFC_NONE)
		{
			return -1;
		}
		return tiles.FirstOrDefault((NKMWarfareTileTemplet tile) => tile.m_TileWinType == winType)?.position.Index ?? (-1);
	}

	public NKMWarfareTileTemplet GetWinTileByWinType(WARFARE_GAME_CONDITION winType)
	{
		if (winType == WARFARE_GAME_CONDITION.WFC_NONE)
		{
			return null;
		}
		return tiles.FirstOrDefault((NKMWarfareTileTemplet tile) => tile.m_TileWinType == winType);
	}

	public List<short> GetLoseTileIndexList(WARFARE_GAME_CONDITION loseType)
	{
		if (loseType == WARFARE_GAME_CONDITION.WFC_NONE)
		{
			return null;
		}
		return (from tile in tiles
			where tile.m_TileLoseType == loseType
			select tile.position.Index).ToList();
	}

	public bool LoadFromLUA(NKMLua lua, string strID)
	{
		m_WarfareMapStrID = strID;
		lua.GetData("m_MapSizeX", ref m_MapSizeX);
		lua.GetData("m_MapSizeY", ref m_MapSizeY);
		if (m_MapSizeX <= 0)
		{
			Log.Error($"m_MapSizeX Invalid: {m_MapSizeX}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareMapTemplet.cs", 187);
			return false;
		}
		if (m_MapSizeY <= 0)
		{
			Log.Error($"m_MapSizeY Invalid: {m_MapSizeY}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareMapTemplet.cs", 192);
			return false;
		}
		tiles.Clear();
		int num = 0;
		for (byte b = 0; b < m_MapSizeY; b++)
		{
			for (byte b2 = 0; b2 < m_MapSizeX; b2++)
			{
				NKMWarfareTileTemplet nKMWarfareTileTemplet = CreateTileTemplet(b2, b);
				if (!nKMWarfareTileTemplet.LoadFromLUA(lua, num))
				{
					return false;
				}
				nKMWarfareTileTemplet.position = new TilePosition(b2, b, GetIndexByPos(b2, b));
				tiles.Add(nKMWarfareTileTemplet);
				num++;
			}
		}
		validTiles = tiles.Where((NKMWarfareTileTemplet tile) => tile.m_TileType != NKM_WARFARE_MAP_TILE_TYPE.NWMTT_DISABLE).ToList();
		diveTiles = validTiles.Where((NKMWarfareTileTemplet tile) => tile.m_NKM_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_DIVE).ToList();
		assultTiles = validTiles.Where((NKMWarfareTileTemplet tile) => tile.m_NKM_WARFARE_SPAWN_POINT_TYPE == NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_ASSAULT).ToList();
		for (int num2 = 0; num2 < diveTiles.Count; num2++)
		{
			diveTiles[num2].pointIndex = num2;
		}
		for (int num3 = 0; num3 < assultTiles.Count; num3++)
		{
			assultTiles[num3].pointIndex = num3;
		}
		return true;
	}

	public void JoinServerOnly()
	{
		foreach (NKMWarfareTileTemplet tile in tiles)
		{
			tile.JoinServerOnly();
		}
	}

	public void ValidateServerOnly()
	{
		foreach (NKMWarfareTileTemplet tile in tiles)
		{
			tile.ValidateServerOnly(this);
		}
		if (!validTiles.Any((NKMWarfareTileTemplet tile) => tile.m_NKM_WARFARE_SPAWN_POINT_TYPE != NKM_WARFARE_SPAWN_POINT_TYPE.NWSPT_NONE))
		{
			Log.ErrorAndExit("[" + m_WarfareMapStrID + "] 스폰 타일이 최소 한 곳 존재해야 합니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMWarfareMapTemplet.cs", 250);
		}
	}

	private NKMWarfareTileTemplet CreateTileTemplet(byte x, byte y)
	{
		List<short> list = new List<short>();
		for (sbyte b = (sbyte)(x - 1); b <= (sbyte)(x + 1); b++)
		{
			for (sbyte b2 = (sbyte)(y - 1); b2 <= (sbyte)(y + 1); b2++)
			{
				if (b >= 0 && b < m_MapSizeX && b2 >= 0 && b2 < m_MapSizeY)
				{
					list.Add(GetIndexByPos(b, b2));
				}
			}
		}
		return new NKMWarfareTileTemplet(m_WarfareMapStrID, list)
		{
			position = new TilePosition(x, y, GetIndexByPos(x, y))
		};
	}
}
