using System.Collections.Generic;
using ClientPacket.Office;
using NKM;
using NKM.Templet.Office;

namespace NKC.Office;

public class NKCOfficeRoomData
{
	public int ID;

	public string Name;

	public Dictionary<long, NKCOfficeFunitureData> m_dicFuniture = new Dictionary<long, NKCOfficeFunitureData>();

	public List<long> m_lstUnitUID = new List<long>();

	public int m_FloorInteriorID;

	public int m_WallInteriorID;

	public int m_BackgroundID;

	public long m_OwnerUID;

	public bool IsMyOffice => m_OwnerUID == 0;

	public NKCOfficeRoomData(NKMOfficeRoom room, long uid)
	{
		if (room == null)
		{
			ID = -1;
			Name = "";
			m_FloorInteriorID = 0;
			m_WallInteriorID = 0;
			m_BackgroundID = 0;
			m_OwnerUID = 0L;
			return;
		}
		ID = room.id;
		Name = room.name;
		m_dicFuniture = new Dictionary<long, NKCOfficeFunitureData>();
		foreach (NKMOfficeFurniture furniture in room.furnitures)
		{
			m_dicFuniture.Add(furniture.uid, new NKCOfficeFunitureData(furniture));
		}
		m_lstUnitUID.AddRange(room.unitUids);
		m_FloorInteriorID = room.floorInteriorId;
		m_WallInteriorID = room.wallInteriorId;
		m_BackgroundID = room.backgroundId;
		m_OwnerUID = uid;
	}

	public NKMOfficeRoomTemplet GetTemplet()
	{
		return NKMOfficeRoomTemplet.Find(ID);
	}

	public void AddFuniture(NKCOfficeFunitureData funitureData)
	{
		m_dicFuniture.Add(funitureData.uid, funitureData);
	}

	public NKM_ERROR_CODE CanAddFuniture(NKCOfficeFunitureData funitureData, bool ignoreCount = false)
	{
		if (funitureData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_FAIL;
		}
		if (funitureData.Templet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_INTERIOR_ID_NOT_FOUND;
		}
		if (!TypeMatch(funitureData.Templet.Target, funitureData.eTarget))
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_FURNITURE_TYPE_MISMATCH;
		}
		NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(ID);
		if (nKMOfficeRoomTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_NOT_FOUND;
		}
		if (NKCScenManager.CurrentUserData().OfficeData.GetFreeInteriorCount(funitureData.itemID) <= 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_FURNITURE_NOT_REMAINS;
		}
		(int, int) size = nKMOfficeRoomTemplet.GetSize(funitureData.eTarget);
		if (!NKCOfficeManager.FunitureBoundaryCheck(size.Item1, size.Item2, funitureData))
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_FURNITURE_OUT_OF_BOUND;
		}
		if (funitureData.Templet.Target == InteriorTarget.Floor)
		{
			int num = GetStuffedFloorCellCount() + funitureData.Templet.CellX * funitureData.Templet.CellY;
			int num2 = size.Item1 * size.Item2;
			if (num > num2 * 9 / 10)
			{
				return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_FURNITURE_ROOM_FULL;
			}
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = CheckFunitureAddOverlap(funitureData);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			return nKM_ERROR_CODE;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	private int GetStuffedFloorCellCount()
	{
		int num = 0;
		foreach (NKCOfficeFunitureData value in m_dicFuniture.Values)
		{
			if (value.eTarget == BuildingFloor.Floor)
			{
				num += value.Templet.CellX * value.Templet.CellY;
			}
		}
		return num;
	}

	public void MoveFuniture(long uid, BuildingFloor target, int posX, int posY, bool bInvert)
	{
		GetFuniture(uid).SetPosition(target, posX, posY, bInvert);
	}

	public NKM_ERROR_CODE CanMoveFuniture(long uid, BuildingFloor target, int posX, int posY, bool bInvert)
	{
		NKCOfficeFunitureData funiture = GetFuniture(uid);
		if (funiture == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_NOT_FOUND;
		}
		NKCOfficeFunitureData nKCOfficeFunitureData = new NKCOfficeFunitureData(funiture);
		nKCOfficeFunitureData.SetPosition(target, posX, posY, bInvert);
		nKCOfficeFunitureData.bInvert = bInvert;
		if (funiture == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_FAIL;
		}
		if (funiture.Templet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_INTERIOR_ID_NOT_FOUND;
		}
		if (!TypeMatch(funiture.Templet.Target, target))
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_FURNITURE_TYPE_MISMATCH;
		}
		NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(ID);
		if (nKMOfficeRoomTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_NOT_FOUND;
		}
		(int, int) size = nKMOfficeRoomTemplet.GetSize(target);
		if (!NKCOfficeManager.FunitureBoundaryCheck(size.Item1, size.Item2, nKCOfficeFunitureData))
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_FURNITURE_OUT_OF_BOUND;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = CheckFunitureMoveOverlap(nKCOfficeFunitureData);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			return nKM_ERROR_CODE;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public void RemoveFuniture(long uid)
	{
		m_dicFuniture.Remove(uid);
	}

	public NKM_ERROR_CODE CanRemoveFurniture(long uid)
	{
		if (!m_dicFuniture.ContainsKey(uid))
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_NOT_FOUND;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	private bool TypeMatch(InteriorTarget type, BuildingFloor target)
	{
		switch (target)
		{
		case BuildingFloor.Floor:
			return type == InteriorTarget.Floor;
		case BuildingFloor.Tile:
			return type == InteriorTarget.Tile;
		case BuildingFloor.LeftWall:
		case BuildingFloor.RightWall:
			return type == InteriorTarget.Wall;
		default:
			return false;
		}
	}

	public NKCOfficeFunitureData GetFuniture(long uid)
	{
		if (m_dicFuniture.TryGetValue(uid, out var value))
		{
			return value;
		}
		return null;
	}

	public NKM_ERROR_CODE CheckFunitureAddOverlap(NKCOfficeFunitureData newFuniture)
	{
		foreach (KeyValuePair<long, NKCOfficeFunitureData> item in m_dicFuniture)
		{
			if (item.Key == newFuniture.uid)
			{
				return NKM_ERROR_CODE.NEC_FAIL_OFFICE_DUPLICATE_FURNITURE_UID;
			}
			NKCOfficeFunitureData value = item.Value;
			if (item.Value.eTarget == newFuniture.eTarget && NKCOfficeManager.IsFunitureOverlaps(newFuniture, value))
			{
				return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_FURNITURE_OVERLAP;
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public NKM_ERROR_CODE CheckFunitureMoveOverlap(NKCOfficeFunitureData funitureMoveData)
	{
		foreach (KeyValuePair<long, NKCOfficeFunitureData> item in m_dicFuniture)
		{
			if (item.Key != funitureMoveData.uid && item.Value.eTarget == funitureMoveData.eTarget)
			{
				NKCOfficeFunitureData value = item.Value;
				if (NKCOfficeManager.IsFunitureOverlaps(funitureMoveData, value))
				{
					return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_FURNITURE_OVERLAP;
				}
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public void ClearAllFunitures()
	{
		m_dicFuniture.Clear();
	}

	public NKM_ERROR_CODE SetDecoration(int id, InteriorTarget target)
	{
		NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMItemMiscTemplet.FindInterior(id);
		if (nKMOfficeInteriorTemplet.InteriorCategory != InteriorCategory.DECO)
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_INTERIOR_NOT_DECO;
		}
		if (nKMOfficeInteriorTemplet.Target != target)
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_FURNITURE_TYPE_MISMATCH;
		}
		switch (target)
		{
		case InteriorTarget.Tile:
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_FURNITURE_TYPE_MISMATCH;
		case InteriorTarget.Wall:
			m_WallInteriorID = id;
			break;
		case InteriorTarget.Floor:
			m_FloorInteriorID = id;
			break;
		case InteriorTarget.Background:
			m_BackgroundID = id;
			break;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public NKMOfficePreset MakePresetFromRoom()
	{
		NKMOfficePreset nKMOfficePreset = new NKMOfficePreset();
		nKMOfficePreset.floorInteriorId = m_FloorInteriorID;
		nKMOfficePreset.wallInteriorId = m_WallInteriorID;
		nKMOfficePreset.backgroundId = m_BackgroundID;
		nKMOfficePreset.furnitures = new List<NKMOfficeFurniture>();
		foreach (NKCOfficeFunitureData value in m_dicFuniture.Values)
		{
			nKMOfficePreset.furnitures.Add(value.ToNKMOfficeFurniture());
		}
		return nKMOfficePreset;
	}
}
