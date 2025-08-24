using System.Collections.Generic;
using UnityEngine;

namespace NKC;

public class NKCWarfareGameTileMgr
{
	private const int DEFAULT_TILE_COUNT = 70;

	private Transform m_parent;

	private List<NKCWarfareGameTile> m_tileList = new List<NKCWarfareGameTile>();

	public NKCWarfareGameTileMgr(Transform parent)
	{
		m_parent = parent;
	}

	public void Init(NKCWarfareGameTile.onClickPossibleArrivalTile onClickTile)
	{
		if (m_tileList.Count <= 0)
		{
			for (int i = 0; i < 70; i++)
			{
				NKCWarfareGameTile newInstance = NKCWarfareGameTile.GetNewInstance(i, m_parent, onClickTile);
				m_tileList.Add(newInstance);
			}
		}
	}

	public void Close()
	{
		for (int i = 0; i < m_tileList.Count; i++)
		{
			m_tileList[i].Close();
		}
		m_tileList.Clear();
	}

	public NKCWarfareGameTile GetTile(int index)
	{
		if (index < 0 || index >= m_tileList.Count)
		{
			return null;
		}
		return m_tileList[index];
	}

	public void SetTileActive(int tileCount)
	{
		for (int i = 0; i < m_tileList.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_tileList[i], i < tileCount);
		}
	}

	public void SetTileLayer0Type(int index, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE type)
	{
		NKCWarfareGameTile tile = GetTile(index);
		if (!(tile == null))
		{
			tile.SetTileLayer0Type(type);
		}
	}

	public bool IsTileLayer0Type(int index, NKCWarfareGameTile.WARFARE_TILE_LAYER_0_TYPE type)
	{
		NKCWarfareGameTile tile = GetTile(index);
		if (tile == null)
		{
			return false;
		}
		if (tile.Get_WARFARE_TILE_LAYER_0_TYPE() != type)
		{
			return false;
		}
		return true;
	}

	public bool IsTileLayer2Type(int index, NKCWarfareGameTile.WARFARE_TILE_LAYER_2_TYPE type)
	{
		NKCWarfareGameTile tile = GetTile(index);
		if (tile == null)
		{
			return false;
		}
		if (tile.Get_WARFARE_TILE_LAYER_2_TYPE() != type)
		{
			return false;
		}
		return true;
	}
}
