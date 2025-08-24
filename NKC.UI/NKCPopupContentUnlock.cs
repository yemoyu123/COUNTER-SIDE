using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupContentUnlock : NKCUIBase
{
	public delegate void OnClose();

	private const string PREFAB_ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_unlock";

	private const string TEXTURE_ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_POPUP_CONTENT_UNLOCK_TEXTURE";

	private const string ICON_ASSET_BUNDLE_NAME = "AB_INVEN_ICON_ITEM_MISC";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_CONTENT_UNLOCK";

	private const string UI_CC_TEXTURE_ASSET_NAME = "NKM_UI_CONTENTS_UNLOCK_COUNTERCASE";

	private const string STR_CC_TITLE_KEY = "";

	private const string STR_CC_DESC_KEY = "";

	private static NKCPopupContentUnlock m_Instance;

	public GameObject m_objBG;

	public Animator m_Ani;

	public Image m_imgMain;

	public Text m_lbTitle;

	public Text m_lbDesc;

	public GameObject m_objCounterCaseParent;

	public Image m_imgCounterCaseUnit;

	public GameObject m_objEpisodeParent;

	public Text m_lbEpisodeTitle;

	public Text m_lbEpisodeName;

	public GameObject m_objActParent;

	public Text m_lbActNum;

	public GameObject m_objNormalIconParent;

	public GameObject m_objSpecialIconParent;

	public Image m_imgSpecialIcon;

	public float m_fAutoCloseTime = 3f;

	private OnClose dOnClose;

	private float m_fDeltaTime;

	private bool m_bUserInput;

	private bool m_bInitComplete;

	public static NKCPopupContentUnlock instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupContentUnlock>("ab_ui_nkm_ui_unlock", "NKM_UI_POPUP_CONTENT_UNLOCK", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupContentUnlock>();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_fDeltaTime = 0f;
		dOnClose?.Invoke();
	}

	private void Init()
	{
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback = new EventTrigger.TriggerEvent();
		entry.callback.AddListener(OnUserInputEvent);
		EventTrigger eventTrigger = m_objBG.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = m_objBG.AddComponent<EventTrigger>();
		}
		eventTrigger.triggers.Clear();
		eventTrigger.triggers.Add(entry);
		m_bInitComplete = true;
	}

	public void Open(NKCContentManager.NKCUnlockableContent unlockableContent, OnClose onClose)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		dOnClose = onClose;
		if (unlockableContent == null)
		{
			ForceClose();
			return;
		}
		NKCUtil.SetGameobjectActive(m_objCounterCaseParent, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEpisodeParent, bValue: false);
		NKCUtil.SetGameobjectActive(m_objActParent, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNormalIconParent, string.IsNullOrEmpty(unlockableContent.m_PopupIconName));
		NKCUtil.SetGameobjectActive(m_objSpecialIconParent, !string.IsNullOrEmpty(unlockableContent.m_PopupIconName));
		if (m_objSpecialIconParent.activeSelf)
		{
			NKCUtil.SetImageSprite(m_imgSpecialIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>(unlockableContent.m_PopupIconAssetBundleName, unlockableContent.m_PopupIconName));
		}
		switch (unlockableContent.m_eContentsType)
		{
		case ContentsType.COUNTERCASE_NEW_CHARACTER:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unlockableContent.m_ContentsValue);
			if (unitTempletBase == null || !unitTempletBase.CollectionEnableByTag)
			{
				ForceClose();
				return;
			}
			NKCUtil.SetGameobjectActive(m_objCounterCaseParent, bValue: true);
			NKCUtil.SetImageSprite(m_imgCounterCaseUnit, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase));
			NKCUtil.SetImageSprite(m_imgMain, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_CONTENT_UNLOCK_TEXTURE", "NKM_UI_CONTENTS_UNLOCK_COUNTERCASE"), bDisableIfSpriteNull: true);
			foreach (NKMContentUnlockTemplet value in NKMTempletContainer<NKMContentUnlockTemplet>.Values)
			{
				if (value.m_eContentsType == ContentsType.COUNTERCASE)
				{
					NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString(value.m_strPopupTitle));
					NKCUtil.SetLabelText(m_lbDesc, string.Format(NKCStringTable.GetString(value.m_strPopupDesc), unitTempletBase.Name));
					break;
				}
			}
			break;
		}
		case ContentsType.EPISODE:
		{
			NKMStageTempletV2 nKMStageTempletV3 = NKMStageTempletV2.Find(unlockableContent.m_ContentsValue);
			if (nKMStageTempletV3 == null || !nKMStageTempletV3.EnableByTag)
			{
				ForceClose();
				return;
			}
			NKMEpisodeTempletV2 nKMEpisodeTempletV3 = NKMEpisodeTempletV2.Find(nKMStageTempletV3.EpisodeId, nKMStageTempletV3.m_Difficulty);
			if (nKMEpisodeTempletV3 == null || !nKMEpisodeTempletV3.EnableByTag)
			{
				ForceClose();
				return;
			}
			NKCUtil.SetGameobjectActive(m_objEpisodeParent, bValue: true);
			NKCUtil.SetImageSprite(m_imgMain, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_CONTENT_UNLOCK_TEXTURE", unlockableContent.m_PopupImageName), bDisableIfSpriteNull: true);
			NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString(unlockableContent.m_PopupTitle));
			NKCUtil.SetLabelText(m_lbDesc, NKCStringTable.GetString(unlockableContent.m_PopupDesc));
			NKCUtil.SetLabelText(m_lbEpisodeTitle, nKMEpisodeTempletV3.GetEpisodeTitle());
			NKCUtil.SetLabelText(m_lbEpisodeName, nKMEpisodeTempletV3.GetEpisodeName());
			break;
		}
		case ContentsType.ACT:
		{
			NKMStageTempletV2 nKMStageTempletV2 = NKMStageTempletV2.Find(unlockableContent.m_ContentsValue);
			if (nKMStageTempletV2 == null || !nKMStageTempletV2.EnableByTag)
			{
				ForceClose();
				return;
			}
			NKMEpisodeTempletV2 nKMEpisodeTempletV2 = NKMEpisodeTempletV2.Find(nKMStageTempletV2.EpisodeId, nKMStageTempletV2.m_Difficulty);
			if (nKMEpisodeTempletV2 == null || !nKMEpisodeTempletV2.EnableByTag)
			{
				ForceClose();
				return;
			}
			NKCUtil.SetGameobjectActive(m_objEpisodeParent, bValue: true);
			NKCUtil.SetGameobjectActive(m_objActParent, bValue: true);
			NKCUtil.SetImageSprite(m_imgMain, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_CONTENT_UNLOCK_TEXTURE", unlockableContent.m_PopupImageName), bDisableIfSpriteNull: true);
			NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString(unlockableContent.m_PopupTitle));
			NKCUtil.SetLabelText(m_lbDesc, NKCStringTable.GetString(unlockableContent.m_PopupDesc));
			NKCUtil.SetLabelText(m_lbEpisodeTitle, nKMEpisodeTempletV2.GetEpisodeTitle());
			NKCUtil.SetLabelText(m_lbEpisodeName, nKMEpisodeTempletV2.GetEpisodeName());
			NKCUtil.SetLabelText(m_lbActNum, string.Format("{0}{1}", NKCStringTable.GetString("SI_DP_CONTENTS_UNLOCK_ACT_DISPLAY"), nKMStageTempletV2.ActId));
			break;
		}
		case ContentsType.DUNGEON:
		{
			NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(unlockableContent.m_ContentsValue);
			if (nKMStageTempletV == null || !nKMStageTempletV.EnableByTag)
			{
				ForceClose();
				return;
			}
			NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(nKMStageTempletV.EpisodeId, nKMStageTempletV.m_Difficulty);
			if (nKMEpisodeTempletV == null || !nKMEpisodeTempletV.EnableByTag)
			{
				ForceClose();
				return;
			}
			NKCUtil.SetGameobjectActive(m_objEpisodeParent, bValue: true);
			NKCUtil.SetImageSprite(m_imgMain, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_CONTENT_UNLOCK_TEXTURE", unlockableContent.m_PopupImageName), bDisableIfSpriteNull: true);
			NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString(unlockableContent.m_PopupTitle));
			NKCUtil.SetLabelText(m_lbDesc, NKCStringTable.GetString(unlockableContent.m_PopupDesc));
			NKCUtil.SetLabelText(m_lbEpisodeTitle, nKMEpisodeTempletV.GetEpisodeTitle());
			NKCUtil.SetLabelText(m_lbEpisodeName, nKMEpisodeTempletV.GetEpisodeName());
			break;
		}
		default:
			NKCUtil.SetImageSprite(m_imgMain, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_CONTENT_UNLOCK_TEXTURE", unlockableContent.m_PopupImageName), bDisableIfSpriteNull: true);
			NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString(unlockableContent.m_PopupTitle));
			NKCUtil.SetLabelText(m_lbDesc, NKCStringTable.GetString(unlockableContent.m_PopupDesc));
			break;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_bUserInput = false;
		m_fDeltaTime = 0f;
		UIOpened();
	}

	private void ForceClose()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_fDeltaTime = 0f;
		dOnClose?.Invoke();
	}

	public void Update()
	{
		if (base.gameObject.activeInHierarchy)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_bUserInput)
			{
				Close();
			}
			if (m_fDeltaTime > m_fAutoCloseTime)
			{
				Close();
			}
		}
	}

	public void OnUserInputEvent(BaseEventData eventData)
	{
	}
}
