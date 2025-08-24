using System.Collections.Generic;
using NKM;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.Office;

public class NKCOfficeFacility : NKCOfficeBuildingBase
{
	[Header("NPC 캐릭터들")]
	public List<NKCOfficeCharacterNPC> m_lstNPCCharacters;

	[Header("배경")]
	public GameObject m_objFacilityBackground;

	private List<NKCOfficeFuniture> m_lstFunitures;

	private List<NKCOfficeFuniture> m_lstFloorFunitures;

	public static NKCOfficeFacility GetInstance(NKMOfficeRoomTemplet templet)
	{
		if (!templet.IsFacility)
		{
			Debug.LogError("Logic Error! tried open non-facility room as facility");
			return null;
		}
		NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(templet.FacilityPrefab, templet.FacilityPrefab);
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(nKMAssetName);
		if (nKCAssetInstanceData?.m_Instant == null)
		{
			Debug.LogError($"NKCUIOfficeFacility : {nKMAssetName} not found!");
			return null;
		}
		NKCOfficeFacility component = nKCAssetInstanceData.m_Instant.GetComponent<NKCOfficeFacility>();
		if (component == null)
		{
			Debug.LogError($"NKCUIOfficeFacility : {nKMAssetName} don't have NKCUIOfficeFacility component!");
			return null;
		}
		component.Init();
		return component;
	}

	public virtual void Init()
	{
		base.Init(null);
		dOnSelectFuniture = null;
		m_Floor?.Init(BuildingFloor.Floor, Color.white, Color.white, Color.white, Color.white, null, null, null);
		m_FloorTile?.Init(BuildingFloor.Tile, Color.white, Color.white, Color.white, Color.white, null, null, null);
		m_LeftWall?.Init(BuildingFloor.LeftWall, Color.white, Color.white, Color.white, Color.white, null, null, null);
		m_RightWall?.Init(BuildingFloor.RightWall, Color.white, Color.white, Color.white, Color.white, null, null, null);
		m_lstFunitures = new List<NKCOfficeFuniture>(base.gameObject.GetComponentsInChildren<NKCOfficeFuniture>(includeInactive: true));
		if (m_Floor?.m_rtFunitureRoot != null)
		{
			m_lstFloorFunitures = new List<NKCOfficeFuniture>(m_Floor.m_rtFunitureRoot.GetComponentsInChildren<NKCOfficeFuniture>(includeInactive: true));
		}
		else
		{
			m_lstFloorFunitures = new List<NKCOfficeFuniture>();
		}
		foreach (NKCOfficeFuniture lstFuniture in m_lstFunitures)
		{
			if (!(lstFuniture == null))
			{
				lstFuniture.Init();
				lstFuniture.SetShowTile(value: false);
				lstFuniture.dOnBeginDragFuniture = OnBeginDrag;
				lstFuniture.dOnDragFuniture = OnDrag;
				lstFuniture.dOnEndDragFuniture = OnEndDrag;
				if (lstFuniture.m_eFunitureType == InteriorTarget.Wall)
				{
					lstFuniture.SetFunitureBoxRaycast(value: false);
				}
			}
		}
		foreach (NKCOfficeCharacterNPC lstNPCCharacter in m_lstNPCCharacters)
		{
			lstNPCCharacter.Init(this);
			lstNPCCharacter.StartAI();
		}
		SetBackground();
		UpdateAlarm();
		UpdateFloorMap();
	}

	public override void CleanUp()
	{
		base.CleanUp();
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
		foreach (NKCOfficeFuniture lstFloorFuniture in m_lstFloorFunitures)
		{
			FloorRect floorRect = CalculateFloorRect(lstFloorFuniture.m_rtFloor);
			for (int k = floorRect.x; k < floorRect.x + floorRect.sizeX; k++)
			{
				for (int l = floorRect.y; l < floorRect.y + floorRect.sizeY; l++)
				{
					m_FloorMap[k, l] = 1L;
				}
			}
		}
	}

	public virtual void UpdateAlarm()
	{
	}

	private void SetBackground()
	{
		if (m_objFacilityBackground != null)
		{
			m_objBackground = m_objFacilityBackground;
			m_objBackground.transform.SetParent(m_rtBackgroundRoot);
			Transform transform = m_objBackground.transform.Find("Stretch/Background");
			if (transform != null)
			{
				m_rtBackground = transform.GetComponent<RectTransform>();
				EventTrigger eventTrigger = transform.GetComponent<EventTrigger>();
				if (eventTrigger == null)
				{
					eventTrigger = transform.gameObject.AddComponent<EventTrigger>();
				}
				eventTrigger.triggers.Clear();
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.BeginDrag;
				entry.callback.AddListener(delegate(BaseEventData eventData)
				{
					PointerEventData data = eventData as PointerEventData;
					OnBeginDrag(data);
				});
				eventTrigger.triggers.Add(entry);
				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.Drag;
				entry.callback.AddListener(delegate(BaseEventData eventData)
				{
					PointerEventData pointData = eventData as PointerEventData;
					OnDrag(pointData);
				});
				eventTrigger.triggers.Add(entry);
				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.EndDrag;
				entry.callback.AddListener(delegate(BaseEventData eventData)
				{
					PointerEventData data = eventData as PointerEventData;
					OnEndDrag(data);
				});
				eventTrigger.triggers.Add(entry);
				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.Scroll;
				entry.callback.AddListener(delegate(BaseEventData eventData)
				{
					PointerEventData eventData2 = eventData as PointerEventData;
					OnScroll(eventData2);
				});
				eventTrigger.triggers.Add(entry);
			}
		}
		SetBackgroundSize();
	}
}
