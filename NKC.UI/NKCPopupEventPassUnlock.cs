using System.Collections;
using NKM;
using NKM.EventPass;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupEventPassUnlock : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_EVENT_PASS";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_EVENT_PASS_SPECIAL_UNLOCK";

	private static NKCPopupEventPassUnlock m_Instance;

	public EventTrigger m_eventTriggerBg;

	public Animator m_animator;

	public Text m_lbTitle;

	public Image m_imgUnitCard;

	public Image m_imgShipCard;

	public Image m_imgEquipCard;

	public float m_fCloseDelay;

	public static NKCPopupEventPassUnlock Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEventPassUnlock>("AB_UI_NKM_UI_EVENT_PASS", "NKM_UI_POPUP_EVENT_PASS_SPECIAL_UNLOCK", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupEventPassUnlock>();
				m_Instance?.Init();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => string.Empty;

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void Init()
	{
		base.gameObject.SetActive(value: false);
		if (m_eventTriggerBg != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				CheckInstanceAndClose();
			});
			m_eventTriggerBg.triggers.Add(entry);
		}
		NKCEventPassDataManager eventPassDataManager = NKCScenManager.GetScenManager().GetEventPassDataManager();
		if (eventPassDataManager != null)
		{
			InitData(NKMEventPassTemplet.Find(eventPassDataManager.EventPassId));
		}
	}

	public void InitData(NKMEventPassTemplet eventPassTemplet)
	{
		if (eventPassTemplet == null)
		{
			return;
		}
		NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString(eventPassTemplet.EventPassTitleStrId));
		switch (eventPassTemplet.EventPassMainRewardType)
		{
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_OPERATOR:
		{
			ActivateImageCard(m_imgUnitCard);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(eventPassTemplet.EventPassMainReward);
			if (unitTempletBase != null)
			{
				NKCUtil.SetImageSprite(m_imgUnitCard, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase), bDisableIfSpriteNull: true);
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_SHIP:
		{
			ActivateImageCard(m_imgShipCard);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(eventPassTemplet.EventPassMainReward);
			if (unitTempletBase != null)
			{
				NKCUtil.SetImageSprite(m_imgShipCard, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase), bDisableIfSpriteNull: true);
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_SKIN:
		{
			ActivateImageCard(m_imgUnitCard);
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(eventPassTemplet.EventPassMainReward);
			if (skinTemplet != null)
			{
				NKCUtil.SetImageSprite(m_imgUnitCard, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, skinTemplet), bDisableIfSpriteNull: true);
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_EQUIP:
		{
			ActivateImageCard(m_imgEquipCard);
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(eventPassTemplet.EventPassMainReward);
			if (equipTemplet != null)
			{
				NKCUtil.SetImageSprite(m_imgEquipCard, NKCResourceUtility.GetOrLoadEquipIcon(equipTemplet), bDisableIfSpriteNull: true);
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_MOLD:
		{
			ActivateImageCard(m_imgEquipCard);
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(eventPassTemplet.EventPassMainReward);
			if (itemMoldTempletByID != null)
			{
				NKCUtil.SetImageSprite(m_imgEquipCard, NKCResourceUtility.GetOrLoadMoldIcon(itemMoldTempletByID), bDisableIfSpriteNull: true);
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_MISC:
		{
			ActivateImageCard(m_imgEquipCard);
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(eventPassTemplet.EventPassMainReward);
			if (itemMiscTempletByID != null)
			{
				NKCUtil.SetImageSprite(m_imgEquipCard, NKCResourceUtility.GetOrLoadMiscItemIcon(itemMiscTempletByID), bDisableIfSpriteNull: true);
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_USER_EXP:
		case NKM_REWARD_TYPE.RT_BUFF:
		case NKM_REWARD_TYPE.RT_EMOTICON:
		case NKM_REWARD_TYPE.RT_MISSION_POINT:
		case NKM_REWARD_TYPE.RT_BINGO_TILE:
		case NKM_REWARD_TYPE.RT_PASS_EXP:
			break;
		}
	}

	public void Open()
	{
		base.gameObject.SetActive(value: true);
		StartCoroutine(ProcessTimer());
		UIOpened();
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	private IEnumerator ProcessTimer()
	{
		while (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			yield return null;
		}
		float delayTimer = 0f;
		while (delayTimer < m_fCloseDelay)
		{
			delayTimer += Time.deltaTime;
			yield return null;
		}
		Close();
	}

	private void ActivateImageCard(Image imageCard)
	{
		NKCUtil.SetGameobjectActive(m_imgUnitCard, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgShipCard, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgEquipCard, bValue: false);
		NKCUtil.SetGameobjectActive(imageCard, bValue: true);
	}
}
