using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Office;

public class NKCUIComOfficeMapTileFacility : MonoBehaviour
{
	public enum FacilityType
	{
		None,
		Lab,
		Factory,
		Hangar,
		Boss,
		TeraBrain
	}

	public int m_iRoomId;

	public string roomInfoKey;

	[Space]
	public Text m_lbTitle;

	public Image m_imgBg;

	public Image m_imgGlow;

	public Image m_imgStroke;

	public Image m_imgFacilityColor;

	public Image m_imgIcon;

	public Image m_imgNpc1;

	public Image m_imgNpc2;

	[Header("시설 아이콘 Deco")]
	public GameObject m_objIconRoot;

	public GameObject m_objLock;

	public GameObject m_objOn;

	[Space]
	public GameObject m_objBgRoot;

	public GameObject m_objLockRoot;

	public GameObject m_objRedDot;

	public RectTransform m_rtBgShape;

	public NKCUIComStateButton m_csbtnTileButton;

	[Header("공방 제작 상태")]
	public GameObject m_objWorkshopRoot;

	public GameObject m_objProgress;

	public GameObject m_objComplete;

	private const string m_strSpriteBundleName = "ab_ui_office_sprite";

	private NKMOfficeRoomTemplet.RoomType m_eFacilityType;

	private ContentsType m_eContentsType;

	private bool m_bUnlocked;

	public NKMOfficeRoomTemplet.RoomType RoomType => m_eFacilityType;

	public bool IsRedDotOn => m_objRedDot.activeSelf;

	public RectTransform RectTransformTileShape => m_rtBgShape;

	public void Init()
	{
		m_eContentsType = ContentsType.None;
		NKCUtil.SetButtonClickDelegate(m_csbtnTileButton, OnBtnTile);
		RectTransform rectTransform = m_objBgRoot?.GetComponent<RectTransform>();
		RectTransform rectTransform2 = m_objLockRoot?.GetComponent<RectTransform>();
		if (rectTransform != null && rectTransform2 != null)
		{
			rectTransform2.sizeDelta = rectTransform.sizeDelta;
		}
	}

	public void UpdateRoomState(Dictionary<string, NKCUIOfficeMinimapFacility.FacilityInfo> dicFacilityInfo, NKCUIOfficeMinimapFacility.FacilityInfo lockInfo)
	{
		m_eFacilityType = NKMOfficeRoomTemplet.RoomType.Dorm;
		NKMOfficeRoomTemplet nKMOfficeRoomTemplet = NKMOfficeRoomTemplet.Find(m_iRoomId);
		if (nKMOfficeRoomTemplet != null)
		{
			m_eFacilityType = nKMOfficeRoomTemplet.Type;
			m_eContentsType = NKCUIOfficeMapFront.GetFacilityContentType(m_eFacilityType);
		}
		m_bUnlocked = false;
		if (nKMOfficeRoomTemplet != null)
		{
			m_bUnlocked = NKCContentManager.IsContentsUnlocked(m_eContentsType);
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnTileButton, OnBtnTile);
		NKCUtil.SetGameobjectActive(m_objOn, m_bUnlocked);
		NKCUtil.SetGameobjectActive(m_objLock, !m_bUnlocked);
		NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objIconRoot, nKMOfficeRoomTemplet != null);
		NKCUtil.SetGameobjectActive(m_objLockRoot, !m_bUnlocked);
		if (m_bUnlocked)
		{
			if (dicFacilityInfo.ContainsKey(roomInfoKey))
			{
				Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_office_sprite", dicFacilityInfo[roomInfoKey].m_strIcon);
				NKCUtil.SetGameobjectActive(m_objIconRoot, orLoadAssetResource != null);
				NKCUtil.SetImageSprite(m_imgIcon, orLoadAssetResource);
				ApplyTileColor(dicFacilityInfo[roomInfoKey]);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objIconRoot, bValue: false);
			}
		}
		else
		{
			ApplyTileColor(lockInfo);
		}
		UpdateCraftingState();
		UpdateRedDot();
	}

	public void UpdateRedDot()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && m_bUnlocked)
		{
			switch (m_eFacilityType)
			{
			case NKMOfficeRoomTemplet.RoomType.Forge:
				NKCUtil.SetGameobjectActive(m_objRedDot, NKCAlarmManager.CheckFactoryNotify(nKMUserData));
				break;
			case NKMOfficeRoomTemplet.RoomType.Hangar:
				NKCUtil.SetGameobjectActive(m_objRedDot, NKCAlarmManager.CheckHangarNotify(nKMUserData));
				break;
			case NKMOfficeRoomTemplet.RoomType.CEO:
				NKCUtil.SetGameobjectActive(m_objRedDot, NKCAlarmManager.CheckScoutNotify(nKMUserData));
				break;
			}
		}
	}

	private void ApplyTileColor(NKCUIOfficeMinimapFacility.FacilityInfo facilityInfo)
	{
		Color color;
		if (!string.IsNullOrEmpty(facilityInfo.m_bgColor))
		{
			ColorUtility.TryParseHtmlString(facilityInfo.m_bgColor, out color);
			NKCUtil.SetImageColor(m_imgBg, color);
		}
		if (!string.IsNullOrEmpty(facilityInfo.m_glowColor))
		{
			ColorUtility.TryParseHtmlString(facilityInfo.m_glowColor, out color);
			NKCUtil.SetImageColor(m_imgGlow, color);
		}
		if (!string.IsNullOrEmpty(facilityInfo.m_strokeColor))
		{
			ColorUtility.TryParseHtmlString(facilityInfo.m_strokeColor, out color);
			NKCUtil.SetImageColor(m_imgStroke, color);
			NKCUtil.SetImageColor(m_imgFacilityColor, color);
		}
		if (!string.IsNullOrEmpty(facilityInfo.m_titleColor))
		{
			ColorUtility.TryParseHtmlString(facilityInfo.m_titleColor, out color);
			NKCUtil.SetLabelTextColor(m_lbTitle, color);
		}
		if (!string.IsNullOrEmpty(facilityInfo.m_npcColor))
		{
			ColorUtility.TryParseHtmlString(facilityInfo.m_npcColor, out color);
			NKCUtil.SetImageColor(m_imgNpc1, color);
			NKCUtil.SetImageColor(m_imgNpc2, color);
		}
	}

	private void UpdateCraftingState()
	{
		if (m_eFacilityType != NKMOfficeRoomTemplet.RoomType.Forge || !m_bUnlocked)
		{
			NKCUtil.SetGameobjectActive(m_objWorkshopRoot, bValue: false);
			return;
		}
		NKMCraftData craftData = NKCScenManager.GetScenManager().GetMyUserData().m_CraftData;
		NKM_CRAFT_SLOT_STATE nKM_CRAFT_SLOT_STATE = NKM_CRAFT_SLOT_STATE.NECSS_EMPTY;
		int mAX_CRAFT_SLOT_DATA = NKMCraftData.MAX_CRAFT_SLOT_DATA;
		for (int i = 1; i <= mAX_CRAFT_SLOT_DATA; i++)
		{
			NKMCraftSlotData slotData = craftData.GetSlotData((byte)i);
			if (slotData != null)
			{
				if (slotData.GetState(NKCSynchronizedTime.GetServerUTCTime()) == NKM_CRAFT_SLOT_STATE.NECSS_CREATING_NOW)
				{
					nKM_CRAFT_SLOT_STATE = NKM_CRAFT_SLOT_STATE.NECSS_CREATING_NOW;
				}
				else if (slotData.GetState(NKCSynchronizedTime.GetServerUTCTime()) == NKM_CRAFT_SLOT_STATE.NECSS_COMPLETED && nKM_CRAFT_SLOT_STATE != NKM_CRAFT_SLOT_STATE.NECSS_CREATING_NOW)
				{
					nKM_CRAFT_SLOT_STATE = NKM_CRAFT_SLOT_STATE.NECSS_COMPLETED;
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_objWorkshopRoot, nKM_CRAFT_SLOT_STATE != NKM_CRAFT_SLOT_STATE.NECSS_EMPTY);
		NKCUtil.SetGameobjectActive(m_objProgress, nKM_CRAFT_SLOT_STATE == NKM_CRAFT_SLOT_STATE.NECSS_CREATING_NOW);
		NKCUtil.SetGameobjectActive(m_objComplete, nKM_CRAFT_SLOT_STATE == NKM_CRAFT_SLOT_STATE.NECSS_COMPLETED);
	}

	private void OnBtnTile()
	{
		if (!m_bUnlocked)
		{
			if (m_eContentsType == ContentsType.None)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_COMING_SOON_SYSTEM);
			}
			else
			{
				NKCContentManager.ShowLockedMessagePopup(m_eContentsType);
			}
		}
		else if (NKMOfficeRoomTemplet.Find(m_iRoomId) != null)
		{
			NKCUIOffice.GetInstance()?.Open(m_iRoomId);
		}
	}
}
