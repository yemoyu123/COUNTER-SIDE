using System;
using System.Collections.Generic;
using ClientPacket.Office;
using DG.Tweening;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Office;

public class NKCUIOfficeMinimapFacility : MonoBehaviour, IOfficeMinimap
{
	[Serializable]
	public struct FacilityInfo
	{
		public string key;

		public string m_bgColor;

		public string m_glowColor;

		public string m_strokeColor;

		public string m_titleColor;

		public string m_npcColor;

		public string m_strIcon;
	}

	public delegate void OnScroll(Vector2 value);

	public ScrollRect m_scrollRectMapRoom;

	[Header("시설 상태 색상 정보")]
	public FacilityInfo m_LockInfo;

	public FacilityInfo[] m_facilityInfoArray;

	private Dictionary<string, FacilityInfo> m_dicFacilityInfo = new Dictionary<string, FacilityInfo>();

	private Dictionary<int, NKCUIComOfficeMapTileFacility> m_dicMapTileFacility = new Dictionary<int, NKCUIComOfficeMapTileFacility>();

	private float m_fOriginalScrollRectContentPreferredWidth;

	public OnScroll m_dOnScrollCamMove;

	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	public ScrollRect GetScrollRect()
	{
		return m_scrollRectMapRoom;
	}

	public float GetScrollRectContentOriginalWidth()
	{
		return m_fOriginalScrollRectContentPreferredWidth;
	}

	public void Init()
	{
		m_dicFacilityInfo.Clear();
		if (m_facilityInfoArray != null)
		{
			int num = m_facilityInfoArray.Length;
			for (int i = 0; i < num; i++)
			{
				if (!m_dicFacilityInfo.ContainsKey(m_facilityInfoArray[i].key))
				{
					m_dicFacilityInfo.Add(m_facilityInfoArray[i].key, m_facilityInfoArray[i]);
				}
				else
				{
					Debug.LogError("Same facility Info in NKCUIOfficeMinimapFacility Prefab");
				}
			}
		}
		m_dicMapTileFacility.Clear();
		NKCUIComOfficeMapTileFacility[] array = m_scrollRectMapRoom?.content?.GetComponentsInChildren<NKCUIComOfficeMapTileFacility>();
		if (array != null)
		{
			int num2 = array.Length;
			for (int j = 0; j < num2; j++)
			{
				if (NKMOfficeRoomTemplet.Find(array[j].m_iRoomId) != null)
				{
					if (m_dicMapTileFacility.ContainsKey(array[j].m_iRoomId))
					{
						Debug.LogError($"Same Room Key Exist in MINIMAP_FACILITY Prefab, RoomId: {array[j].m_iRoomId}");
					}
					else
					{
						m_dicMapTileFacility.Add(array[j].m_iRoomId, array[j]);
					}
				}
				array[j].Init();
			}
		}
		if (m_scrollRectMapRoom != null)
		{
			LayoutElement component = m_scrollRectMapRoom.content.GetComponent<LayoutElement>();
			if (component != null)
			{
				m_fOriginalScrollRectContentPreferredWidth = component.preferredWidth;
			}
			m_scrollRectMapRoom.onValueChanged.RemoveAllListeners();
			m_scrollRectMapRoom.onValueChanged.AddListener(OnScrollRectValueChanged);
		}
		UpdateRedDotAll();
		base.gameObject.SetActive(value: false);
	}

	public Transform GetScrollTargetTileTransform(int sectionId)
	{
		List<Transform> list = new List<Transform>();
		foreach (KeyValuePair<int, NKCUIComOfficeMapTileFacility> item in m_dicMapTileFacility)
		{
			NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(item.Key);
			if (nKMOfficeRoomTemplet != null && nKMOfficeRoomTemplet.SectionId == sectionId)
			{
				list.Add(item.Value.transform);
			}
		}
		if (list.Count <= 0)
		{
			return null;
		}
		list.Sort(delegate(Transform e1, Transform e2)
		{
			if (e1.position.x > e2.position.x)
			{
				return 1;
			}
			return (e1.position.x < e2.position.x) ? (-1) : 0;
		});
		return list[0];
	}

	public RectTransform GetTileRectTransform(int roomId)
	{
		if (m_dicMapTileFacility.ContainsKey(roomId))
		{
			return m_dicMapTileFacility[roomId].GetComponent<RectTransform>();
		}
		return null;
	}

	public Transform GetRightEndTileTransform()
	{
		List<Transform> list = new List<Transform>();
		foreach (KeyValuePair<int, NKCUIComOfficeMapTileFacility> item in m_dicMapTileFacility)
		{
			list.Add(item.Value.transform);
		}
		if (list.Count <= 0)
		{
			return null;
		}
		list.Sort(delegate(Transform e1, Transform e2)
		{
			if (e1.position.x < e2.position.x)
			{
				return 1;
			}
			return (e1.position.x > e2.position.x) ? (-1) : 0;
		});
		return list[0];
	}

	public void SetActive(bool value)
	{
		base.gameObject.SetActive(value);
		if (value)
		{
			UpdateCameraPosition();
		}
	}

	public void UpdateRoomStateAll()
	{
		foreach (KeyValuePair<int, NKCUIComOfficeMapTileFacility> item in m_dicMapTileFacility)
		{
			item.Value.UpdateRoomState(m_dicFacilityInfo, m_LockInfo);
		}
	}

	public void UpdateRoomState(NKMOfficeRoomTemplet.RoomType roomType)
	{
		foreach (KeyValuePair<int, NKCUIComOfficeMapTileFacility> item in m_dicMapTileFacility)
		{
			if (item.Value.RoomType == roomType)
			{
				item.Value.UpdateRoomState(m_dicFacilityInfo, m_LockInfo);
				break;
			}
		}
	}

	public void UpdateRoomStateInSection(int sectionId)
	{
		NKMOfficeSectionTemplet nKMOfficeSectionTemplet = NKMOfficeSectionTemplet.Find(sectionId);
		if (nKMOfficeSectionTemplet == null)
		{
			return;
		}
		foreach (KeyValuePair<int, NKMOfficeRoomTemplet> room in nKMOfficeSectionTemplet.Rooms)
		{
			if (m_dicMapTileFacility.ContainsKey(room.Key))
			{
				m_dicMapTileFacility[room.Key].UpdateRoomState(m_dicFacilityInfo, m_LockInfo);
			}
		}
		NKCUIOfficeMapFront.GetInstance()?.OfficeUpsideMenu?.UpdateMinimapRoomInfo();
	}

	public void UpdateRoomInfo(NKMOfficeRoom officeRoom)
	{
	}

	public void UpdatePurchasedRoom(NKMOfficeRoom officeRoom)
	{
	}

	public void LockRoomsInSection(int sectionId)
	{
	}

	public void ExpandScrollRectRange()
	{
		if (m_scrollRectMapRoom == null)
		{
			return;
		}
		Vector3 position = GetRightEndTileTransform().position;
		float popupWidth = NKCUIPopupOfficeMemberEdit.Instance.PopupWidth;
		if (popupWidth > 0f)
		{
			float x = ((float)Screen.width - popupWidth) * 0.5f;
			if (NKCUIManager.FrontCanvas != null)
			{
				Vector3 vector = NKCUIManager.FrontCanvas.worldCamera.ScreenToWorldPoint(new Vector3(x, (float)Screen.height * 0.5f, 0f));
				position.x -= vector.x;
			}
		}
		Vector3 position2 = m_scrollRectMapRoom.content.position;
		Vector3 position3 = new Vector3(position2.x - position.x, position2.y, position2.z);
		Vector3 vector2 = NKCUIManager.FrontCanvas.worldCamera.WorldToScreenPoint(position3);
		float num = m_scrollRectMapRoom.content.rect.width * NKCUIManager.FrontCanvas.scaleFactor;
		_ = m_scrollRectMapRoom.content.pivot;
		float num2 = vector2.x + (1f - m_scrollRectMapRoom.content.pivot.x) * num;
		float num3 = ((float)Screen.width - num2) * 2f / NKCUIManager.FrontCanvas.scaleFactor;
		m_scrollRectMapRoom.content.GetComponent<LayoutElement>().preferredWidth = m_fOriginalScrollRectContentPreferredWidth + num3;
	}

	public void RevertScrollRectRange()
	{
		if (!(m_scrollRectMapRoom == null))
		{
			m_scrollRectMapRoom.content.DOKill();
			LayoutElement component = m_scrollRectMapRoom.content.GetComponent<LayoutElement>();
			if (component != null)
			{
				component.preferredWidth = m_fOriginalScrollRectContentPreferredWidth;
			}
		}
	}

	public bool IsRedDotOn()
	{
		foreach (KeyValuePair<int, NKCUIComOfficeMapTileFacility> item in m_dicMapTileFacility)
		{
			if (item.Value.IsRedDotOn)
			{
				return true;
			}
		}
		return false;
	}

	public void UpdateRedDotAll()
	{
		foreach (KeyValuePair<int, NKCUIComOfficeMapTileFacility> item in m_dicMapTileFacility)
		{
			item.Value.UpdateRedDot();
		}
	}

	public void UpdateCameraPosition()
	{
		if (m_scrollRectMapRoom.content.sizeDelta.x <= 0f)
		{
			m_scrollRectMapRoom.horizontalNormalizedPosition = 0f;
		}
		if (m_dOnScrollCamMove != null)
		{
			m_dOnScrollCamMove(m_scrollRectMapRoom.normalizedPosition);
		}
	}

	private void OnScrollRectValueChanged(Vector2 value)
	{
		if (!(m_scrollRectMapRoom == null) && m_dOnScrollCamMove != null && m_scrollRectMapRoom.content.sizeDelta.x > 0f)
		{
			m_dOnScrollCamMove(value);
		}
	}

	private void OnDestroy()
	{
		if (m_dicFacilityInfo != null)
		{
			m_dicFacilityInfo.Clear();
			m_dicFacilityInfo = null;
		}
		if (m_dicMapTileFacility != null)
		{
			m_dicMapTileFacility.Clear();
			m_dicMapTileFacility = null;
		}
	}
}
