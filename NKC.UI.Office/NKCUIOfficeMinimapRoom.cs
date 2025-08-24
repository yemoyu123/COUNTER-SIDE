using System.Collections.Generic;
using ClientPacket.Office;
using NKM;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Office;

public class NKCUIOfficeMinimapRoom : MonoBehaviour, IOfficeMinimap
{
	public enum ScrollRectExpanded
	{
		Normal,
		Expanded
	}

	public delegate void OnScroll(Vector2 value);

	public ScrollRect m_scrollRectMapRoom;

	private Dictionary<int, NKCUIComOfficeMapTileRoom> m_dicMapTileRoom = new Dictionary<int, NKCUIComOfficeMapTileRoom>();

	private ScrollRectExpanded m_eScrollRectExpanded;

	private float m_fOriginalScrollRectContentPreferredWidth;

	private bool m_bFirstOpen;

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
		SortTilesAlongYaxis();
		m_dicMapTileRoom.Clear();
		NKCUIComOfficeMapTileRoom[] array = m_scrollRectMapRoom?.content?.GetComponentsInChildren<NKCUIComOfficeMapTileRoom>();
		if (array != null)
		{
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				if (NKMOfficeRoomTemplet.Find(array[i].m_iRoomId) != null)
				{
					array[i].Init();
					if (m_dicMapTileRoom.ContainsKey(array[i].m_iRoomId))
					{
						Debug.LogError($"Same Room Key Exist in MINIMAP_ROOM Prefab, RoomId: {array[i].m_iRoomId}");
					}
					else
					{
						m_dicMapTileRoom.Add(array[i].m_iRoomId, array[i]);
					}
				}
			}
		}
		if (m_scrollRectMapRoom != null)
		{
			LayoutElement component = m_scrollRectMapRoom.content.GetComponent<LayoutElement>();
			if (component != null)
			{
				m_fOriginalScrollRectContentPreferredWidth = component.preferredWidth;
			}
			m_scrollRectMapRoom.content.pivot = new Vector2(0.5f, 0.5f);
			m_scrollRectMapRoom.onValueChanged.RemoveAllListeners();
			m_scrollRectMapRoom.onValueChanged.AddListener(OnScrollRectValueChanged);
			m_eScrollRectExpanded = ScrollRectExpanded.Normal;
		}
		m_bFirstOpen = true;
		UpdateRedDotAll();
		base.gameObject.SetActive(value: false);
	}

	public Transform GetScrollTargetTileTransform(int sectionId)
	{
		List<Transform> list = new List<Transform>();
		foreach (KeyValuePair<int, NKCUIComOfficeMapTileRoom> item in m_dicMapTileRoom)
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
		if (m_dicMapTileRoom.ContainsKey(roomId))
		{
			return m_dicMapTileRoom[roomId].GetComponent<RectTransform>();
		}
		return null;
	}

	public Transform GetRightEndTileTransform()
	{
		List<Transform> list = new List<Transform>();
		foreach (KeyValuePair<int, NKCUIComOfficeMapTileRoom> item in m_dicMapTileRoom)
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
			if (m_bFirstOpen)
			{
				Vector2 normalizedPosition = m_scrollRectMapRoom.normalizedPosition;
				normalizedPosition.x = 0f;
				m_scrollRectMapRoom.normalizedPosition = normalizedPosition;
				m_bFirstOpen = false;
			}
			UpdateCameraPosition();
			UpdateRedDotAll();
		}
	}

	public void UpdateRoomStateAll()
	{
		foreach (KeyValuePair<int, NKCUIComOfficeMapTileRoom> item in m_dicMapTileRoom)
		{
			item.Value.UpdateRoomState();
		}
	}

	public void UpdateRoomState(NKMOfficeRoomTemplet.RoomType roomType)
	{
		foreach (KeyValuePair<int, NKCUIComOfficeMapTileRoom> item in m_dicMapTileRoom)
		{
			if (item.Value.RoomType == roomType)
			{
				item.Value.UpdateRoomState();
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
			if (m_dicMapTileRoom.ContainsKey(room.Key))
			{
				m_dicMapTileRoom[room.Key].UpdateRoomState();
			}
		}
		NKCUIOfficeMapFront.GetInstance()?.OfficeUpsideMenu?.UpdateMinimapRoomInfo();
	}

	public void UpdateRoomInfo(NKMOfficeRoom officeRoom)
	{
		if (m_dicMapTileRoom.ContainsKey(officeRoom.id))
		{
			m_dicMapTileRoom[officeRoom.id].UpdateRoomInfo(officeRoom);
		}
	}

	public void UpdatePurchasedRoom(NKMOfficeRoom officeRoom)
	{
		if (m_dicMapTileRoom.ContainsKey(officeRoom.id))
		{
			m_dicMapTileRoom[officeRoom.id].UpdateRoomState();
		}
		NKCUIOfficeMapFront.GetInstance()?.OfficeUpsideMenu?.UpdateMinimapRoomInfo();
	}

	public void LockRoomsInSection(int sectionId)
	{
		foreach (KeyValuePair<int, NKCUIComOfficeMapTileRoom> item in m_dicMapTileRoom)
		{
			if (item.Value.m_iSectionId == sectionId)
			{
				item.Value.LockRoom();
			}
		}
	}

	public void ExpandScrollRectRange()
	{
		if (m_scrollRectMapRoom == null || m_eScrollRectExpanded == ScrollRectExpanded.Expanded)
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
		m_scrollRectMapRoom.content.GetComponent<LayoutElement>().preferredWidth += num3;
		m_eScrollRectExpanded = ScrollRectExpanded.Expanded;
	}

	public void RevertScrollRectRange()
	{
		if (!(m_scrollRectMapRoom == null) && m_eScrollRectExpanded != ScrollRectExpanded.Normal)
		{
			LayoutElement component = m_scrollRectMapRoom.content.GetComponent<LayoutElement>();
			if (component != null)
			{
				component.preferredWidth = m_fOriginalScrollRectContentPreferredWidth;
			}
			m_eScrollRectExpanded = ScrollRectExpanded.Normal;
		}
	}

	public bool IsRedDotOn()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			return NKCAlarmManager.CheckOfficeDormNotify(nKMUserData);
		}
		return false;
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

	public void UpdateRoomFxAll()
	{
		foreach (KeyValuePair<int, NKCUIComOfficeMapTileRoom> item in m_dicMapTileRoom)
		{
			item.Value.UpdateFxState();
		}
	}

	public void UpdateRedDotAll()
	{
		foreach (KeyValuePair<int, NKCUIComOfficeMapTileRoom> item in m_dicMapTileRoom)
		{
			item.Value.UpdateRedDot();
		}
	}

	private void SortTilesAlongYaxis()
	{
		if (!(m_scrollRectMapRoom?.content != null))
		{
			return;
		}
		List<Transform> list = new List<Transform>();
		int childCount = m_scrollRectMapRoom.content.childCount;
		for (int i = 0; i < childCount; i++)
		{
			list.Add(m_scrollRectMapRoom.content.GetChild(i));
		}
		list.Sort(delegate(Transform e1, Transform e2)
		{
			if (e1.position.y < e2.position.y)
			{
				return 1;
			}
			return (e1.position.y > e2.position.y) ? (-1) : 0;
		});
		for (int num = 0; num < childCount; num++)
		{
			list[num].SetSiblingIndex(num);
		}
	}

	private void OnScrollRectValueChanged(Vector2 value)
	{
		if (m_scrollRectMapRoom == null)
		{
			return;
		}
		if (m_dOnScrollCamMove != null && m_scrollRectMapRoom.content.sizeDelta.x > 0f)
		{
			m_dOnScrollCamMove(value);
		}
		if (NKCUIPopupOfficeMemberEdit.IsInstanceOpen)
		{
			if (value.x > 0.5f && m_eScrollRectExpanded == ScrollRectExpanded.Normal)
			{
				ExpandScrollRectRange();
			}
			else if (value.x < 0.5f && m_eScrollRectExpanded == ScrollRectExpanded.Expanded)
			{
				RevertScrollRectRange();
			}
		}
	}

	private void OnDestroy()
	{
		if (m_dicMapTileRoom != null)
		{
			m_dicMapTileRoom.Clear();
			m_dicMapTileRoom = null;
		}
	}
}
