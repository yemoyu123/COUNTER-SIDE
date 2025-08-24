using System;
using ClientPacket.Office;
using NKC.FX;
using NKC.Office;
using NKM;
using NKM.Templet;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Office;

public class NKCUIComOfficeMapTileRoom : MonoBehaviour
{
	[Serializable]
	public struct RoomObject
	{
		public GameObject m_objRoot;

		public Text m_lbRoomName;

		public Color m_colBg;

		public Color m_colGlow;

		public Color m_colStroke;
	}

	private enum RoomState
	{
		NORMAL,
		LOBBY,
		LOCK,
		CANNOT_PURCHASE,
		NEED_PURCHASE,
		NO_SIGNAL
	}

	public int m_iRoomId;

	public bool m_bIsLobby;

	[Header("룸 오브젝트")]
	public RoomObject m_roomInfo;

	public RoomObject m_lobbyInfo;

	public RoomObject m_roomLock;

	public RoomObject m_noSignal;

	[Header("룸 색상 이미지")]
	public Image m_imgBg;

	public Image m_imgGlow;

	public Image m_imgStroke;

	[Header("로비 유닛")]
	public Image[] m_imgUnitLobbyArray;

	public Text m_lbUnitLobbyCount;

	[Header("기숙사 유닛")]
	public Image[] m_imgUnitDormArray;

	public Text m_lbUnitDormCount;

	[Header("언락 필요 자원")]
	public GameObject m_objPriceRoot;

	public Image m_imgUnlockItemIcon;

	public Text m_lbUnlockPriceCount;

	[Space]
	public Animator m_animator;

	public RectTransform m_rtBgShape;

	public NKCUIComStateButton m_csbtnTileButton;

	public GameObject m_objRedDot;

	private RoomState m_eRoomState;

	private NKMOfficeRoomTemplet.RoomType m_eRoomType;

	private Image[] m_currentUnitArray;

	private Text m_currentUnitCountText;

	private Text m_currentRoomNameText;

	private Sprite m_unitBlackSprite;

	private bool IgnoreSuperUser = true;

	private static Animator m_selectedAnimator;

	public int m_iSectionId { get; private set; }

	public NKMOfficeRoomTemplet.RoomType RoomType => m_eRoomType;

	public RectTransform RectTransformTileShape => m_rtBgShape;

	public void Init()
	{
		m_eRoomType = NKMOfficeRoomTemplet.RoomType.Dorm;
		NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(m_iRoomId);
		if (nKMOfficeRoomTemplet != null)
		{
			m_eRoomType = nKMOfficeRoomTemplet.Type;
		}
		if (m_animator == null)
		{
			m_animator = GetComponent<Animator>();
		}
		m_animator?.SetInteger("State", 0);
		NKC_FX_PTC_OFFICE_MAP_TILE[] componentsInChildren = GetComponentsInChildren<NKC_FX_PTC_OFFICE_MAP_TILE>(includeInactive: true);
		if (componentsInChildren != null)
		{
			RectTransform component = GetComponent<RectTransform>();
			int num = componentsInChildren.Length;
			for (int i = 0; i < num; i++)
			{
				componentsInChildren[i].Reference = component;
			}
		}
		m_unitBlackSprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_office_sprite", "AB_UI_OFFICE_MEMBER_NONE");
		NKCUtil.SetButtonClickDelegate(m_csbtnTileButton, OnBtnTile);
		NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
	}

	public void UpdateRoomInfo(NKMOfficeRoom officeRoom)
	{
		NKMOfficeRoomTemplet roomTemplet = NKMOfficeRoomTemplet.Find(m_iRoomId);
		SetRoomName(officeRoom, roomTemplet);
		SetUnitAssignInfo(officeRoom, roomTemplet);
	}

	public void LockRoom()
	{
		if (m_eRoomState != RoomState.LOCK)
		{
			m_eRoomState = RoomState.LOCK;
			NKMOfficeRoom officeRoom = NKCUIOfficeMapFront.GetInstance()?.OfficeData.GetOfficeRoom(m_iRoomId);
			SetRoomInfo(m_eRoomState, officeRoom);
		}
	}

	public void UpdateFxState()
	{
		NKCUIOfficeMapFront instance = NKCUIOfficeMapFront.GetInstance();
		if (IsAccessableRoom() && instance.CurrentMinimapState == NKCUIOfficeMapFront.MinimapState.Edit)
		{
			m_animator?.SetInteger("State", 1);
		}
		else
		{
			m_animator?.SetInteger("State", 0);
		}
	}

	public void UpdateRoomState()
	{
		NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(m_iRoomId);
		NKCUIOfficeMapFront instance = NKCUIOfficeMapFront.GetInstance();
		if (instance == null)
		{
			return;
		}
		if (nKMOfficeRoomTemplet != null)
		{
			m_iSectionId = nKMOfficeRoomTemplet.SectionId;
		}
		bool visiting = NKCUIOfficeMapFront.GetInstance().Visiting;
		m_eRoomState = RoomState.NORMAL;
		if (!visiting && !NKCContentManager.IsContentsUnlocked(ContentsType.OFFICE))
		{
			m_eRoomState = RoomState.LOCK;
		}
		if (nKMOfficeRoomTemplet == null)
		{
			m_eRoomState = RoomState.LOCK;
		}
		if (m_eRoomState == RoomState.NORMAL && !instance.OfficeData.IsOpenedSection(m_iSectionId))
		{
			m_eRoomState = RoomState.LOCK;
		}
		if (m_eRoomState == RoomState.NORMAL && !instance.OfficeData.IsOpenedRoom(m_iRoomId))
		{
			if (!visiting)
			{
				m_eRoomState = RoomState.NEED_PURCHASE;
				if (nKMOfficeRoomTemplet.HasUnlockType)
				{
					UnlockInfo unlockInfo = new UnlockInfo(nKMOfficeRoomTemplet.UnlockReqType, nKMOfficeRoomTemplet.UnlockReqValue);
					if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in unlockInfo, IgnoreSuperUser))
					{
						m_eRoomState = RoomState.CANNOT_PURCHASE;
					}
				}
			}
			else
			{
				m_eRoomState = RoomState.LOCK;
			}
		}
		if (m_eRoomState == RoomState.LOCK && visiting)
		{
			m_eRoomState = RoomState.NO_SIGNAL;
		}
		if (m_eRoomState == RoomState.NORMAL && m_bIsLobby)
		{
			m_eRoomState = RoomState.LOBBY;
		}
		NKMOfficeRoom officeRoom = NKCUIOfficeMapFront.GetInstance()?.OfficeData.GetOfficeRoom(m_iRoomId);
		SetRoomInfo(m_eRoomState, officeRoom);
		UpdateFxState();
	}

	public void UpdateRedDot()
	{
		if (!IsAccessableRoom())
		{
			NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
		}
		else if (m_bIsLobby)
		{
			bool flag = false;
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				flag = NKCAlarmManager.CheckOfficeCommunityNotify(nKMUserData);
			}
			NKCUtil.SetGameobjectActive(m_objRedDot, LoyaltyFullUnitExist() || flag);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objRedDot, LoyaltyFullUnitExist());
		}
	}

	private bool LoyaltyFullUnitExist()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return false;
		}
		NKMOfficeRoom officeRoom = nKMUserData.OfficeData.GetOfficeRoom(m_iRoomId);
		if (officeRoom == null)
		{
			return false;
		}
		foreach (long unitUid in officeRoom.unitUids)
		{
			NKMUnitData unitFromUID = nKMUserData.m_ArmyData.GetUnitFromUID(unitUid);
			if (unitFromUID != null && unitFromUID.CheckOfficeRoomHeartFull())
			{
				return true;
			}
		}
		return false;
	}

	private bool IsAccessableRoom()
	{
		if (m_eRoomState != RoomState.LOBBY)
		{
			return m_eRoomState == RoomState.NORMAL;
		}
		return true;
	}

	private void HideRoomObjectRootAll()
	{
		NKCUtil.SetGameobjectActive(m_roomInfo.m_objRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_lobbyInfo.m_objRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_roomLock.m_objRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_noSignal.m_objRoot, bValue: false);
	}

	private void SetRoomInfo(RoomState roomState, NKMOfficeRoom officeRoom)
	{
		HideRoomObjectRootAll();
		m_currentRoomNameText = null;
		m_currentUnitArray = null;
		m_currentUnitCountText = null;
		NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(m_iRoomId);
		switch (roomState)
		{
		case RoomState.NORMAL:
			m_currentRoomNameText = m_roomInfo.m_lbRoomName;
			m_currentUnitArray = m_imgUnitDormArray;
			m_currentUnitCountText = m_lbUnitDormCount;
			SetTileColor(m_roomInfo);
			NKCUtil.SetGameobjectActive(m_roomInfo.m_objRoot, bValue: true);
			break;
		case RoomState.LOBBY:
			m_currentRoomNameText = m_lobbyInfo.m_lbRoomName;
			m_currentUnitArray = m_imgUnitLobbyArray;
			m_currentUnitCountText = m_lbUnitLobbyCount;
			SetTileColor(m_lobbyInfo);
			NKCUtil.SetGameobjectActive(m_lobbyInfo.m_objRoot, bValue: true);
			break;
		case RoomState.LOCK:
		case RoomState.CANNOT_PURCHASE:
		case RoomState.NEED_PURCHASE:
			m_currentRoomNameText = m_roomLock.m_lbRoomName;
			SetTileColor(m_roomLock);
			NKCUtil.SetGameobjectActive(m_roomLock.m_objRoot, bValue: true);
			if (nKMOfficeRoomTemplet != null && nKMOfficeRoomTemplet.PriceItem != null)
			{
				NKCUtil.SetImageSprite(m_imgUnlockItemIcon, NKCResourceUtility.GetOrLoadMiscItemIcon(nKMOfficeRoomTemplet.PriceItem.m_ItemMiscID));
				NKCUtil.SetLabelText(m_lbUnlockPriceCount, $"{nKMOfficeRoomTemplet.Price:#,##0}");
			}
			else
			{
				NKCUtil.SetLabelText(m_lbUnlockPriceCount, $"{0:#,##0}");
			}
			break;
		case RoomState.NO_SIGNAL:
			SetTileColor(m_noSignal);
			NKCUtil.SetGameobjectActive(m_noSignal.m_objRoot, bValue: true);
			break;
		}
		SetRoomName(officeRoom, nKMOfficeRoomTemplet);
		SetUnitAssignInfo(officeRoom, nKMOfficeRoomTemplet);
	}

	private void SetTileColor(RoomObject roomObject)
	{
		NKCUtil.SetImageColor(m_imgBg, roomObject.m_colBg);
		NKCUtil.SetImageColor(m_imgGlow, roomObject.m_colGlow);
		NKCUtil.SetImageColor(m_imgStroke, roomObject.m_colStroke);
	}

	private void SetRoomName(NKMOfficeRoom officeRoom, NKMOfficeRoomTemplet roomTemplet)
	{
		if (officeRoom != null && !string.IsNullOrEmpty(officeRoom.name))
		{
			NKCUtil.SetLabelText(m_currentRoomNameText, officeRoom.name);
		}
		else
		{
			NKCUtil.SetLabelText(m_currentRoomNameText, NKCUIOfficeMapFront.GetDefaultRoomName(m_iRoomId));
		}
	}

	private void SetUnitAssignInfo(NKMOfficeRoom officeRoom, NKMOfficeRoomTemplet roomTemplet)
	{
		if (m_currentUnitArray == null || officeRoom == null || roomTemplet == null)
		{
			return;
		}
		if (!NKCUIOfficeMapFront.GetInstance().Visiting)
		{
			int num = m_currentUnitArray.Length;
			for (int i = 0; i < num; i++)
			{
				bool flag = i < roomTemplet.UnitLimitCount;
				if (m_currentUnitArray[i] != null)
				{
					NKCUtil.SetGameobjectActive(m_currentUnitArray[i].gameObject, flag);
				}
				if (flag)
				{
					if (i < officeRoom.unitUids.Count)
					{
						NKCUtil.SetImageSprite(m_currentUnitArray[i], GetUnitFaceSprite(officeRoom.unitUids[i]));
					}
					else
					{
						NKCUtil.SetImageSprite(m_currentUnitArray[i], m_unitBlackSprite);
					}
				}
			}
		}
		else
		{
			int num2 = m_currentUnitArray.Length;
			for (int j = 0; j < num2; j++)
			{
				bool flag2 = j < roomTemplet.UnitLimitCount;
				if (m_currentUnitArray[j] != null)
				{
					NKCUtil.SetGameobjectActive(m_currentUnitArray[j].gameObject, flag2);
				}
				if (flag2)
				{
					if (j < officeRoom.unitUids.Count)
					{
						NKCUtil.SetImageSprite(m_currentUnitArray[j], GetFriendUnitFaceSprite(officeRoom.unitUids[j]));
					}
					else
					{
						NKCUtil.SetImageSprite(m_currentUnitArray[j], m_unitBlackSprite);
					}
				}
			}
		}
		NKCUtil.SetLabelText(m_currentUnitCountText, $"{officeRoom.unitUids.Count}/{roomTemplet.UnitLimitCount}");
	}

	private Sprite GetUnitFaceSprite(long unitUId)
	{
		NKMUnitData nKMUnitData = NKCScenManager.GetScenManager()?.GetMyUserData()?.m_ArmyData.GetUnitOrTrophyFromUID(unitUId);
		if (nKMUnitData != null)
		{
			Sprite orLoadMinimapFaceIcon = NKCResourceUtility.GetOrLoadMinimapFaceIcon(NKMUnitManager.GetUnitTempletBase(nKMUnitData.m_UnitID));
			if (orLoadMinimapFaceIcon != null)
			{
				return orLoadMinimapFaceIcon;
			}
		}
		return m_unitBlackSprite;
	}

	private Sprite GetFriendUnitFaceSprite(long unitUId)
	{
		NKCOfficeData nKCOfficeData = NKCScenManager.GetScenManager()?.GetMyUserData()?.OfficeData;
		if (nKCOfficeData != null)
		{
			Sprite orLoadMinimapFaceIcon = NKCResourceUtility.GetOrLoadMinimapFaceIcon(NKMUnitManager.GetUnitTempletBase(nKCOfficeData.GetFriendUnitId(unitUId)));
			if (orLoadMinimapFaceIcon != null)
			{
				return orLoadMinimapFaceIcon;
			}
		}
		return m_unitBlackSprite;
	}

	private void OnCloseMemberEdit()
	{
		m_selectedAnimator?.SetInteger("State", 1);
		m_selectedAnimator = null;
		NKCUIOfficeMapFront.GetInstance()?.RevertScrollRectRange();
	}

	private void OnBtnTile()
	{
		NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(m_iRoomId);
		if (nKMOfficeRoomTemplet == null)
		{
			return;
		}
		if (NKCUIOfficeMapFront.GetInstance().IgnoreRoomLockState)
		{
			m_eRoomState = (m_bIsLobby ? RoomState.LOBBY : RoomState.NORMAL);
		}
		if (m_eRoomState == RoomState.LOCK || m_eRoomState == RoomState.NO_SIGNAL)
		{
			string gET_STRING_OFFICE_ROOM_IN_LOCKED_SECTION = NKCUtilString.GET_STRING_OFFICE_ROOM_IN_LOCKED_SECTION;
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(gET_STRING_OFFICE_ROOM_IN_LOCKED_SECTION, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		if (m_eRoomState == RoomState.CANNOT_PURCHASE)
		{
			string message = "";
			if (nKMOfficeRoomTemplet.HasUnlockType)
			{
				message = NKCContentManager.MakeUnlockConditionString(new UnlockInfo(nKMOfficeRoomTemplet.UnlockReqType, nKMOfficeRoomTemplet.UnlockReqValue), bSimple: false);
			}
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(message, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			return;
		}
		if (m_eRoomState == RoomState.NEED_PURCHASE)
		{
			if (nKMOfficeRoomTemplet.PriceItem == null)
			{
				string content = string.Format(NKCUtilString.GET_STRING_OFFICE_PURCHASE_ROOM, m_currentRoomNameText.text);
				NKCPopupResourceConfirmBox.Instance.OpenForConfirm(NKCUtilString.GET_STRING_UNLOCK, content, delegate
				{
					NKCPacketSender.Send_NKMPacket_OFFICE_OPEN_ROOM_REQ(m_iRoomId);
				});
			}
			else
			{
				string content2 = string.Format(NKCUtilString.GET_STRING_OFFICE_PURCHASE_ROOM, m_currentRoomNameText.text);
				NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_UNLOCK, content2, nKMOfficeRoomTemplet.PriceItem.m_ItemMiscID, nKMOfficeRoomTemplet.Price, delegate
				{
					NKCPacketSender.Send_NKMPacket_OFFICE_OPEN_ROOM_REQ(m_iRoomId);
				});
			}
			return;
		}
		NKCUIOfficeMapFront instance = NKCUIOfficeMapFront.GetInstance();
		if (instance == null)
		{
			return;
		}
		switch (instance.CurrentMinimapState)
		{
		case NKCUIOfficeMapFront.MinimapState.Main:
			if (IsAccessableRoom())
			{
				NKCUIOffice.GetInstance()?.Open(m_iRoomId);
			}
			break;
		case NKCUIOfficeMapFront.MinimapState.Edit:
		{
			NKCUIPopupOfficeMemberEdit.Instance.Open(m_iRoomId, OnCloseMemberEdit);
			if (m_selectedAnimator != null)
			{
				m_selectedAnimator.SetInteger("State", 1);
			}
			m_selectedAnimator = m_animator;
			m_animator?.SetInteger("State", 2);
			float popupWidth = NKCUIPopupOfficeMemberEdit.Instance.PopupWidth;
			if (popupWidth > 0f)
			{
				float x = ((float)Screen.width - popupWidth) * 0.5f;
				Vector3 vector = NKCCamera.GetSubUICamera().ScreenToWorldPoint(new Vector3(x, (float)Screen.height * 0.5f, 0f));
				Vector3 position = base.transform.position;
				position.x -= vector.x;
				NKCUIOfficeMapFront.GetInstance()?.MoveMiniMap(position, restrictScroll: false);
			}
			break;
		}
		case NKCUIOfficeMapFront.MinimapState.Visit:
		{
			long currentFriendUid = instance.OfficeData.CurrentFriendUid;
			NKCUIOffice.GetInstance()?.Open(currentFriendUid, m_iRoomId);
			break;
		}
		}
	}

	private void OnDestroy()
	{
		m_currentRoomNameText = null;
		m_currentUnitArray = null;
		m_currentUnitCountText = null;
		m_unitBlackSprite = null;
		m_selectedAnimator = null;
	}
}
