using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Office;
using NKC.Templet.Office;
using NKM;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.Office;

public class NKCOfficeBuilding : NKCOfficeBuildingBase
{
	[Header("가구 선택 모드 빈칸")]
	public Color m_colEmpty;

	[Header("선택된 가구의 칸")]
	public Color m_colSelect;

	[Header("다른 가구의 칸")]
	public Color m_colOccupied;

	[Header("겹친 칸")]
	public Color m_colProhibited;

	[Header("위 컬러들에 PMA 적용 필요한지")]
	public bool ApplyPremultifiledAlpha = true;

	[Header("선택 가구")]
	public Color m_colSelectFuniture;

	public float m_fSelectFunitureLoopTime;

	public float m_fSelectFunitureAlpha = 0.5f;

	private NKCOfficeRoomData m_currentRoomData;

	private List<NKCOfficeCharacterNPC> m_lstNPCCharacters = new List<NKCOfficeCharacterNPC>();

	private bool m_SelectionImpossible;

	private NKCOfficeFunitureData m_FunitureDataBeforeMove;

	private NKCOfficeFuniture m_funSelection;

	private Dictionary<long, NKCOfficeFuniture> m_dicFuniture = new Dictionary<long, NKCOfficeFuniture>();

	public NKCOfficeFunitureData m_SelectedFunitureData { get; private set; }

	public bool HasSelection
	{
		get
		{
			if (m_SelectedFunitureData != null && m_SelectedFunitureData.Templet != null)
			{
				return m_funSelection != null;
			}
			return false;
		}
	}

	public override void Init(NKCOfficeFuniture.OnClickFuniture onSelectFuniture)
	{
		base.Init(onSelectFuniture);
		dOnSelectFuniture = onSelectFuniture;
		if (ApplyPremultifiledAlpha)
		{
			PremultiflyAlpha(ref m_colEmpty);
			PremultiflyAlpha(ref m_colSelect);
			PremultiflyAlpha(ref m_colOccupied);
			PremultiflyAlpha(ref m_colProhibited);
		}
		m_Floor.Init(BuildingFloor.Floor, m_colEmpty, m_colSelect, m_colOccupied, m_colProhibited, OnBeginDrag, OnDrag, OnEndDrag);
		m_FloorTile.Init(BuildingFloor.Tile, m_colEmpty, m_colSelect, m_colOccupied, m_colProhibited, OnBeginDrag, OnDrag, OnEndDrag);
		m_LeftWall.Init(BuildingFloor.LeftWall, m_colEmpty, m_colSelect, m_colOccupied, m_colProhibited, OnBeginDrag, OnDrag, OnEndDrag);
		m_RightWall.Init(BuildingFloor.RightWall, m_colEmpty, m_colSelect, m_colOccupied, m_colProhibited, OnBeginDrag, OnDrag, OnEndDrag);
	}

	private void PremultiflyAlpha(ref Color col)
	{
		col.r *= col.a;
		col.g *= col.a;
		col.b *= col.a;
	}

	public bool SetRoomData(NKCOfficeRoomData roomData, List<NKMUserProfileData> lstFriends)
	{
		Debug.Log("SetRoomData");
		if (roomData == null)
		{
			return false;
		}
		NKMOfficeRoomTemplet templet = roomData.GetTemplet();
		if (templet == null)
		{
			return false;
		}
		CleanupFunitures();
		m_currentRoomData = roomData;
		SetRoomSize(templet.CellX, templet.CellY, templet.CellZ, m_fTileSize);
		m_LeftWall.bInvertRequired = false;
		m_RightWall.bInvertRequired = true;
		SetDecoration(roomData.m_FloorInteriorID, InteriorTarget.Floor);
		SetDecoration(roomData.m_WallInteriorID, InteriorTarget.Wall);
		SetDecoration(roomData.m_BackgroundID, InteriorTarget.Background);
		AddFuniture(roomData.m_dicFuniture.Values);
		SortFloorObjects();
		if (roomData != null && !roomData.IsMyOffice)
		{
			SetSDCharacters(roomData.m_lstUnitUID, roomData.m_OwnerUID);
		}
		else
		{
			SetSDCharacters(roomData?.m_lstUnitUID, lstFriends);
		}
		PlayInteractionOnEnter();
		return true;
	}

	public void SetTempFurniture(NKMOfficePreset preset)
	{
		List<NKCOfficeFunitureData> list = new List<NKCOfficeFunitureData>();
		long num = 0L;
		foreach (NKMOfficeFurniture furniture in preset.furnitures)
		{
			NKCOfficeFunitureData nKCOfficeFunitureData = new NKCOfficeFunitureData(furniture);
			nKCOfficeFunitureData.uid = num;
			list.Add(nKCOfficeFunitureData);
			num++;
		}
		Dictionary<int, long> dictionary = NKCOfficeManager.MakeRequiredFurnitureHaveCountDic(m_currentRoomData.ID, preset);
		CleanupFunitures();
		CleanupCharacters(bCleanupNPC: false);
		SetDecoration(preset.floorInteriorId, InteriorTarget.Floor);
		SetDecoration(preset.wallInteriorId, InteriorTarget.Wall);
		SetDecoration(preset.backgroundId, InteriorTarget.Background);
		AddFuniture(list);
		SortFloorObjects();
		foreach (NKCOfficeFuniture value2 in m_dicFuniture.Values)
		{
			int id = value2.Templet.Id;
			if (!dictionary.TryGetValue(id, out var value))
			{
				value = 0L;
			}
			if (value <= 0)
			{
				value2.SetColor(Color.red);
			}
			dictionary[id] = value - 1;
		}
	}

	public override void UpdateSDCharacters(List<long> lstUnitUID, List<NKMUserProfileData> lstFriends)
	{
		base.UpdateSDCharacters(lstUnitUID, lstFriends);
		m_currentRoomData.m_lstUnitUID = lstUnitUID;
	}

	protected override void Update()
	{
		base.Update();
	}

	public override void CleanUp()
	{
		base.CleanUp();
		CleanupFunitures();
		CleanupFloors();
	}

	protected override void CleanupCharacters(bool bCleanupNPC)
	{
		base.CleanupCharacters(bCleanupNPC);
		if (!bCleanupNPC)
		{
			return;
		}
		foreach (NKCOfficeCharacterNPC lstNPCCharacter in m_lstNPCCharacters)
		{
			lstNPCCharacter.Cleanup();
			UnityEngine.Object.Destroy(lstNPCCharacter.gameObject);
		}
		m_lstNPCCharacters.Clear();
	}

	private void CleanupFunitures()
	{
		ClearSelection();
		ClearAllFuniture();
	}

	private void CleanupFloors()
	{
		m_Floor.CleanUp();
		m_FloorTile.CleanUp();
		m_LeftWall.CleanUp();
		m_RightWall.CleanUp();
	}

	public void ClearSelection()
	{
		m_SelectedFunitureData = null;
		m_FunitureDataBeforeMove = null;
		foreach (NKCOfficeFloorBase officeFloor in base.OfficeFloors)
		{
			officeFloor.ShowSelectionTile(value: false);
		}
		if (m_funSelection != null)
		{
			m_funSelection.SetColor(Color.white);
			m_funSelection.CleanUp();
			m_funSelection = null;
		}
		SetAllFunitureAlpha(value: false, -1L);
	}

	private RectTransform GetTargetSelectionRoot(InteriorTarget type)
	{
		if ((uint)type <= 1u || type != InteriorTarget.Wall)
		{
			return m_Floor.m_rtSelectedFunitureRoot;
		}
		return m_LeftWall.m_rtSelectedFunitureRoot;
	}

	private RectTransform GetTargetSelectionRoot(BuildingFloor target)
	{
		return GetFloorBase(target)?.m_rtSelectedFunitureRoot;
	}

	private NKCOfficeFloorBase GetFloorBase(BuildingFloor target)
	{
		return target switch
		{
			BuildingFloor.Tile => m_FloorTile, 
			BuildingFloor.LeftWall => m_LeftWall, 
			BuildingFloor.RightWall => m_RightWall, 
			_ => m_Floor, 
		};
	}

	public void TouchFurniture(long uid)
	{
		if (!m_dicFuniture.TryGetValue(uid, out var value))
		{
			return;
		}
		value.OnTouchReact();
		if (value.Templet == null || !value.Templet.HasInteraction)
		{
			return;
		}
		List<NKCOfficeCharacter> list = new List<NKCOfficeCharacter>();
		foreach (NKCOfficeCharacter value2 in m_dicCharacter.Values)
		{
			if (!value2.HasInteractionTarget())
			{
				Vector3 vector = value.transform.localPosition - value2.transform.localPosition;
				if (!(value.Templet.TargetRange * value.Templet.TargetRange < vector.sqrMagnitude))
				{
					list.Add(value2);
				}
			}
		}
		NKCOfficeManager.TryPlayReactionInteraction(value, list);
	}

	public NKM_ERROR_CODE MoveFunitureMode(long uid)
	{
		m_FunitureDataBeforeMove = m_currentRoomData.GetFuniture(uid);
		if (m_FunitureDataBeforeMove == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_FURNITURE_NOT_FOUND;
		}
		m_SelectedFunitureData = new NKCOfficeFunitureData(m_FunitureDataBeforeMove);
		GetFloorBase(m_SelectedFunitureData.eTarget);
		if (!m_dicFuniture.TryGetValue(m_SelectedFunitureData.uid, out m_funSelection))
		{
			ClearSelection();
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_FURNITURE_NOT_FOUND;
		}
		m_funSelection.SetShowTile(value: false);
		m_funSelection.dOnBeginDragFuniture = null;
		m_funSelection.dOnDragFuniture = OnDragSelectedFuniture;
		m_funSelection.dOnEndDragFuniture = null;
		if (m_funSelection.IsInteractionOngoing)
		{
			m_funSelection.InteractingCharacter.UnregisterInteraction();
		}
		SetAllFunitureAlpha(value: true, uid);
		ShowSelectionTile(m_SelectedFunitureData.Templet.Target);
		UpdateSelectedFuniturePos(bForceUpdateColor: true);
		return NKM_ERROR_CODE.NEC_OK;
	}

	public void CancelMoveFuniture()
	{
		if (m_funSelection != null && m_FunitureDataBeforeMove != null)
		{
			m_funSelection.SetShowTile(value: false);
			m_funSelection.SetColor(Color.white);
			m_funSelection.dOnBeginDragFuniture = OnBeginDrag;
			m_funSelection.dOnDragFuniture = OnDragUnSelectedFuniture;
			m_funSelection.dOnEndDragFuniture = OnEndDrag;
			SetFuniturePosition(m_funSelection, m_FunitureDataBeforeMove);
		}
		m_funSelection = null;
		ClearSelection();
	}

	public NKM_ERROR_CODE AddFunitureMode(int funitureID)
	{
		ClearSelection();
		NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMItemMiscTemplet.FindInterior(funitureID);
		if (nKMOfficeInteriorTemplet == null)
		{
			Debug.LogError($"Funiture {funitureID} not found!");
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_ROOM_INTERIOR_ID_NOT_FOUND;
		}
		NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(m_currentRoomData.ID);
		BuildingFloor target;
		int posX;
		int posY;
		switch (nKMOfficeInteriorTemplet.Target)
		{
		default:
			target = BuildingFloor.Floor;
			posX = nKMOfficeRoomTemplet.FloorX / 3;
			posY = nKMOfficeRoomTemplet.FloorY / 3;
			break;
		case InteriorTarget.Tile:
			target = BuildingFloor.Tile;
			posX = nKMOfficeRoomTemplet.FloorX / 3;
			posY = nKMOfficeRoomTemplet.FloorY / 3;
			break;
		case InteriorTarget.Wall:
			target = BuildingFloor.LeftWall;
			posX = nKMOfficeRoomTemplet.LeftWallX - nKMOfficeInteriorTemplet.CellX;
			posY = 0;
			break;
		}
		m_SelectedFunitureData = new NKCOfficeFunitureData(-1L, funitureID, target, posX, posY);
		m_funSelection = NKCOfficeFuniture.GetInstance(-1L, nKMOfficeInteriorTemplet, m_fTileSize, bInvert: false, GetTargetSelectionRoot(target));
		if (m_funSelection == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_OFFICE_FAIL;
		}
		m_funSelection.dOnBeginDragFuniture = null;
		m_funSelection.dOnDragFuniture = OnDragSelectedFuniture;
		m_funSelection.dOnEndDragFuniture = null;
		SetAllFunitureAlpha(value: true, -1L);
		ShowSelectionTile(nKMOfficeInteriorTemplet.Target);
		UpdateSelectedFuniturePos(bForceUpdateColor: true);
		return NKM_ERROR_CODE.NEC_OK;
	}

	private void ShowSelectionTile(InteriorTarget type)
	{
		m_Floor.ShowSelectionTile(type == InteriorTarget.Floor);
		m_FloorTile.ShowSelectionTile(type == InteriorTarget.Tile);
		m_LeftWall.ShowSelectionTile(type == InteriorTarget.Wall);
		m_RightWall.ShowSelectionTile(type == InteriorTarget.Wall);
		switch (type)
		{
		case InteriorTarget.Floor:
			m_Floor.UpdateSelectionTile(null, m_currentRoomData);
			break;
		case InteriorTarget.Tile:
			m_FloorTile.UpdateSelectionTile(null, m_currentRoomData);
			break;
		case InteriorTarget.Wall:
			m_LeftWall.UpdateSelectionTile(null, m_currentRoomData);
			m_RightWall.UpdateSelectionTile(null, m_currentRoomData);
			break;
		}
	}

	private void UpdateSelectedFuniturePos(bool bForceUpdateColor = false)
	{
		if (!HasSelection)
		{
			return;
		}
		NKCOfficeFloorBase floorBase = GetFloorBase(m_SelectedFunitureData.eTarget);
		if (floorBase == null)
		{
			return;
		}
		m_funSelection.transform.SetParent(GetTargetSelectionRoot(m_SelectedFunitureData.eTarget));
		m_funSelection.SetInvert(floorBase.GetFunitureInvert(m_SelectedFunitureData));
		Vector3 worldPos = floorBase.GetWorldPos(m_SelectedFunitureData.PosX, m_SelectedFunitureData.PosY, m_SelectedFunitureData.SizeX, m_SelectedFunitureData.SizeY);
		m_funSelection.transform.position = worldPos;
		bool flag = false;
		if (m_currentRoomData != null)
		{
			(int, int) size = NKMOfficeRoomTemplet.Find(m_currentRoomData.ID).GetSize(m_SelectedFunitureData.eTarget);
			flag = NKCOfficeManager.FunitureBoundaryCheck(size.Item1, size.Item2, m_SelectedFunitureData);
		}
		bool flag2 = floorBase.UpdateSelectionTile(m_SelectedFunitureData, m_currentRoomData);
		bool flag3 = !flag || !flag2;
		if (bForceUpdateColor || m_SelectionImpossible != flag3)
		{
			if (flag3)
			{
				m_funSelection.SetColor(Color.red);
			}
			else
			{
				m_funSelection.SetGlow(m_colSelectFuniture, m_fSelectFunitureLoopTime);
			}
			m_SelectionImpossible = flag3;
		}
		m_funSelection.InvalidateWorldRect();
	}

	private void OnDragUnSelectedFuniture(PointerEventData eventData)
	{
		OnDrag(eventData);
	}

	private void OnDragSelectedFuniture(PointerEventData eventData)
	{
		if (!HasSelection)
		{
			return;
		}
		BuildingFloor buildingFloor = m_SelectedFunitureData.eTarget;
		bool flag = false;
		if (m_SelectedFunitureData.Templet.Target == InteriorTarget.Wall)
		{
			if (buildingFloor == BuildingFloor.LeftWall && m_RightWall.IsContainsScreenPoint(eventData.position))
			{
				buildingFloor = BuildingFloor.RightWall;
				flag = true;
			}
			else if (buildingFloor == BuildingFloor.RightWall && m_LeftWall.IsContainsScreenPoint(eventData.position))
			{
				buildingFloor = BuildingFloor.LeftWall;
				flag = true;
			}
		}
		NKCOfficeFloorBase floorBase = GetFloorBase(buildingFloor);
		if (floorBase == null)
		{
			return;
		}
		OfficeFloorPosition cellPosFromScreenPos = floorBase.GetCellPosFromScreenPos(eventData.position, m_SelectedFunitureData.SizeX, m_SelectedFunitureData.SizeY);
		m_SelectedFunitureData.SetPosition(buildingFloor, cellPosFromScreenPos.x, cellPosFromScreenPos.y);
		UpdateSelectedFuniturePos();
		if (flag)
		{
			switch (buildingFloor)
			{
			case BuildingFloor.RightWall:
				m_LeftWall.UpdateSelectionTile(m_SelectedFunitureData, m_currentRoomData);
				break;
			case BuildingFloor.LeftWall:
				m_RightWall.UpdateSelectionTile(m_SelectedFunitureData, m_currentRoomData);
				break;
			}
		}
	}

	public void InvertSelection()
	{
		m_SelectedFunitureData.bInvert = !m_SelectedFunitureData.bInvert;
		NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(m_currentRoomData.ID);
		(int, int) size = nKMOfficeRoomTemplet.GetSize(m_SelectedFunitureData.eTarget);
		if (nKMOfficeRoomTemplet != null)
		{
			if (m_SelectedFunitureData.PosX + m_SelectedFunitureData.SizeX > size.Item1)
			{
				m_SelectedFunitureData.SetPosition(size.Item1 - m_SelectedFunitureData.SizeX, m_SelectedFunitureData.PosY);
			}
			if (m_SelectedFunitureData.PosY + m_SelectedFunitureData.SizeY > size.Item2)
			{
				m_SelectedFunitureData.SetPosition(m_SelectedFunitureData.PosX, size.Item2 - m_SelectedFunitureData.SizeY);
			}
		}
		UpdateSelectedFuniturePos();
	}

	public void SetAllFunitureAlpha(bool value, long excludeUID = -1L)
	{
		foreach (KeyValuePair<long, NKCOfficeFuniture> item in m_dicFuniture)
		{
			if (item.Key == excludeUID)
			{
				item.Value.SetAlpha(1f);
			}
			else
			{
				item.Value.SetAlpha(value ? m_fSelectFunitureAlpha : 1f);
			}
		}
	}

	public void ClearAllFuniture()
	{
		foreach (InteriorTarget value in Enum.GetValues(typeof(InteriorTarget)))
		{
			_ = value;
			foreach (KeyValuePair<long, NKCOfficeFuniture> item in m_dicFuniture)
			{
				item.Value.CleanUp();
			}
			m_dicFuniture.Clear();
		}
	}

	public void AddFuniture(IEnumerable<NKCOfficeFunitureData> lstFunitures)
	{
		foreach (NKCOfficeFunitureData lstFuniture in lstFunitures)
		{
			AddFuniture(lstFuniture, bUpdateFloorMap: false);
		}
		UpdateFloorMap();
	}

	public void AddFuniture(NKCOfficeFunitureData funitureData, bool bUpdateFloorMap = true)
	{
		if (funitureData == null)
		{
			return;
		}
		NKMOfficeInteriorTemplet templet = funitureData.Templet;
		if (templet == null)
		{
			return;
		}
		NKCOfficeFloorBase floorBase = GetFloorBase(funitureData.eTarget);
		if (floorBase == null)
		{
			return;
		}
		NKCOfficeFuniture instance = NKCOfficeFuniture.GetInstance(funitureData.uid, templet, m_fTileSize, floorBase.GetFunitureInvert(funitureData), floorBase.m_rtFunitureRoot);
		if (!(instance == null))
		{
			m_dicFuniture.Add(funitureData.uid, instance);
			instance.transform.localPosition = floorBase.GetLocalPos(funitureData);
			instance.dOnClickFuniture = dOnSelectFuniture;
			instance.dOnBeginDragFuniture = OnBeginDrag;
			instance.dOnDragFuniture = OnDragUnSelectedFuniture;
			instance.dOnEndDragFuniture = OnEndDrag;
			instance.UpdateInteractionPos(m_Floor.m_rtFunitureRoot);
			if (bUpdateFloorMap)
			{
				UpdateFloorMap();
			}
		}
	}

	public void MoveFuniture(NKCOfficeFunitureData funitureData)
	{
		if (funitureData == null)
		{
			Debug.LogError("MoveFuniture : data null.");
			return;
		}
		if (!m_dicFuniture.TryGetValue(funitureData.uid, out var value))
		{
			Debug.LogError($"MoveFuniture : funiture uid {funitureData.uid} not found!!!");
			return;
		}
		SetFuniturePosition(value, funitureData);
		UpdateFloorMap();
	}

	public void RemoveFuniture(long uid)
	{
		if (!m_dicFuniture.TryGetValue(uid, out var value))
		{
			Debug.LogError($"RemoveFuniture : funiture uid {uid} not found!!!");
			return;
		}
		m_dicFuniture.Remove(uid);
		value.CleanUp();
		UpdateFloorMap();
	}

	public void ClearAllFunitures()
	{
		foreach (NKCOfficeFuniture value in m_dicFuniture.Values)
		{
			value.CleanUp();
		}
		m_dicFuniture.Clear();
		UpdateFloorMap();
	}

	private void SetFuniturePosition(NKCOfficeFuniture funiture, NKCOfficeFunitureData funitureData)
	{
		NKCOfficeFloorBase floorBase = GetFloorBase(funitureData.eTarget);
		funiture.transform.SetParent(floorBase.m_rtFunitureRoot);
		funiture.SetInvert(floorBase.GetFunitureInvert(funitureData));
		funiture.transform.localPosition = floorBase.GetLocalPos(funitureData);
		funiture.InvalidateWorldRect();
		funiture.UpdateInteractionPos(m_Floor.m_rtFunitureRoot);
	}

	public void SetDecoration(int id, InteriorTarget target)
	{
		NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMItemMiscTemplet.FindInterior(id);
		if (nKMOfficeInteriorTemplet != null)
		{
			SetDecoration(nKMOfficeInteriorTemplet);
			return;
		}
		switch (target)
		{
		case InteriorTarget.Background:
			SetDecoration(NKMCommonConst.Office.DefaultBackgroundItem);
			break;
		case InteriorTarget.Floor:
			SetDecoration(NKMCommonConst.Office.DefaultFloorItem);
			break;
		case InteriorTarget.Wall:
			SetDecoration(NKMCommonConst.Office.DefaultWallItem);
			break;
		case InteriorTarget.Tile:
			break;
		}
	}

	public void SetDecoration(NKMOfficeInteriorTemplet templet)
	{
		if (templet == null)
		{
			return;
		}
		if (templet.InteriorCategory != InteriorCategory.DECO)
		{
			Debug.LogError("tried SetDecoration with non-deco interior");
			return;
		}
		if (templet.Target == InteriorTarget.Tile)
		{
			Debug.LogError("Tile has no decoration!");
			return;
		}
		switch (templet.Target)
		{
		case InteriorTarget.Floor:
			m_Floor.SetDecoration(templet);
			break;
		case InteriorTarget.Wall:
			m_LeftWall.SetDecoration(templet);
			m_RightWall.SetDecoration(templet);
			break;
		case InteriorTarget.Background:
			SetBackground(templet);
			break;
		case InteriorTarget.Tile:
			break;
		}
	}

	protected override void UpdateFloorMap()
	{
		m_FloorMap = new long[m_SizeX, m_SizeY];
		for (int i = 0; i < m_SizeX; i++)
		{
			for (int j = 0; j < m_SizeY; j++)
			{
				m_FloorMap[i, j] = 0L;
			}
		}
		foreach (KeyValuePair<long, NKCOfficeFunitureData> item in m_currentRoomData.m_dicFuniture)
		{
			NKCOfficeFunitureData value = item.Value;
			if (value.eTarget != BuildingFloor.Floor)
			{
				continue;
			}
			for (int k = value.PosX; k < value.PosX + value.SizeX; k++)
			{
				for (int l = value.PosY; l < value.PosY + value.SizeY; l++)
				{
					m_FloorMap[k, l] = value.uid;
				}
			}
		}
	}

	public void AddNPC(string npcAssetName, string spineAssetName, string BTAssetName, Vector3 localPos)
	{
		NKCOfficeCharacterNPC nKCOfficeCharacterNPC = ((!string.IsNullOrEmpty(npcAssetName)) ? NKCOfficeCharacterNPC.GetNPCInstance(NKMAssetName.ParseBundleName(npcAssetName, npcAssetName)) : NKCOfficeCharacterNPC.GetNPCInstance());
		if (nKCOfficeCharacterNPC == null)
		{
			Debug.LogError("AddNPC Failed! " + npcAssetName);
			return;
		}
		nKCOfficeCharacterNPC.SpineAssetName = spineAssetName;
		nKCOfficeCharacterNPC.BTAssetName = BTAssetName;
		nKCOfficeCharacterNPC.Init(this);
		nKCOfficeCharacterNPC.transform.localPosition = localPos;
		nKCOfficeCharacterNPC.StartAI();
		m_lstNPCCharacters.Add(nKCOfficeCharacterNPC);
	}

	public override void OnCharacterBeginDrag(NKCOfficeCharacter character)
	{
		foreach (NKCOfficeFuniture value in m_dicFuniture.Values)
		{
			if (NKCOfficeManager.CanPlayInteraction(character, value))
			{
				value.SetHighlight(value: true);
			}
			else
			{
				value.SetHighlight(value: false);
			}
		}
	}

	public override void OnCharacterEndDrag(NKCOfficeCharacter character)
	{
		if (character == null)
		{
			return;
		}
		foreach (NKCOfficeFuniture value in m_dicFuniture.Values)
		{
			value.SetHighlight(value: false);
		}
		NKCOfficeFuniture furnitureFromPosition = GetFurnitureFromPosition(character.transform.localPosition);
		if (furnitureFromPosition != null && NKCOfficeManager.CanPlayInteraction(character, furnitureFromPosition))
		{
			NKCOfficeManager.PlayInteraction(character, furnitureFromPosition);
		}
		if (!NKCDefineManager.DEFINE_USE_CHEAT() || !NKCUtil.IsUsingSuperUserFunction())
		{
			return;
		}
		character.ResetUnitInteractionCooltime();
		foreach (NKCOfficeCharacter item in GetCharactersInRange(character.transform.position, 100f))
		{
			if (!(item == character))
			{
				item.ResetUnitInteractionCooltime();
				if (NKCOfficeManager.CanPlayInteraction(character, item))
				{
					Debug.Log("DEBUG : Force interaction play");
					NKCOfficeManager.PlayInteraction(character, item);
				}
			}
		}
	}

	private NKCOfficeFuniture GetFurnitureFromPosition(Vector3 localPos)
	{
		OfficeFloorPosition pos = CalculateFloorPosition(localPos);
		if (!m_Floor.IsInBound(pos))
		{
			return null;
		}
		long key = base.FloorMap[pos.x, pos.y];
		if (m_dicFuniture.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}

	public override NKCOfficeFuniture FindInteractableInterior(NKCOfficeCharacter character)
	{
		if (character == null)
		{
			return null;
		}
		List<NKCOfficeFuniture> list = new List<NKCOfficeFuniture>();
		Vector3 localPosition = character.transform.localPosition;
		foreach (NKCOfficeFuniture value in m_dicFuniture.Values)
		{
			if (value.Templet != null && !(value.Templet.TargetRange <= 0f))
			{
				float num = value.Templet.TargetRange * value.Templet.TargetRange;
				if (!((value.transform.localPosition - localPosition).sqrMagnitude > num) && NKCOfficeManager.CanPlayInteraction(character, value))
				{
					list.Add(value);
				}
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public override NKCOfficeCharacter FindInteractableCharacter(NKCOfficeCharacter character)
	{
		if (character == null)
		{
			return null;
		}
		List<NKCOfficeCharacter> list = new List<NKCOfficeCharacter>();
		foreach (NKCOfficeCharacter value in m_dicCharacter.Values)
		{
			if (!(character == value) && NKCOfficeManager.CanPlayInteraction(character, value))
			{
				list.Add(value);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	private void PlayInteractionOnEnter()
	{
		foreach (NKCOfficeFuniture value in m_dicFuniture.Values)
		{
			if (value.HasInteractionTarget() || UnityEngine.Random.Range(0, 100) >= NKMCommonConst.Office.OfficeInteraction.RoomEnterActRatePercent)
			{
				continue;
			}
			foreach (NKCOfficeCharacter value2 in m_dicCharacter.Values)
			{
				if (NKCOfficeManager.CanPlayInteraction(value2, value) && NKCOfficeManager.PlayInteraction(value2, value))
				{
					value2.transform.localPosition = value2.GetInteractionPosition();
					break;
				}
			}
		}
		foreach (NKCOfficeCharacter value3 in m_dicCharacter.Values)
		{
			if (value3.HasInteractionTarget() || UnityEngine.Random.Range(0, 100) >= NKMCommonConst.Office.OfficeInteraction.RoomEnterActRatePercent)
			{
				continue;
			}
			foreach (NKCOfficeCharacter value4 in m_dicCharacter.Values)
			{
				if (NKCOfficeManager.CanPlayInteraction(value3, value4, bIgnoreRange: true) && NKCOfficeManager.PlayInteraction(value3, value4, bIgnoreRange: true, bForceAlign: true))
				{
					value3.transform.localPosition = value3.GetInteractionPosition();
					value4.transform.localPosition = value4.GetInteractionPosition();
				}
			}
		}
		foreach (NKCOfficeCharacter value5 in m_dicCharacter.Values)
		{
			if (!value5.HasInteractionTarget() && UnityEngine.Random.Range(0, 100) < NKMCommonConst.Office.OfficeInteraction.RoomEnterActRatePercent && value5.SoloInteractionCache != null && value5.SoloInteractionCache.Count != 0)
			{
				NKCOfficeUnitInteractionTemplet soloTemplet = value5.SoloInteractionCache[UnityEngine.Random.Range(0, value5.SoloInteractionCache.Count)];
				value5.RegisterInteraction(soloTemplet);
			}
		}
	}
}
