using NKC.UI.Component;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIItemListTagSlot : MonoBehaviour
{
	public enum SPAC_TAG_TYPE
	{
		UNIT_LEVEL,
		UNIT_SKILL_LEVEL,
		UNIT_TACTIC,
		UNIT_REACTOR,
		UNIT_LOYALTY,
		OPER_LEVEL,
		OPER_MAIN_SKILL_LEVEL,
		OPER_SUB_SKILL_LEVEL,
		OPER_SUB_SKILL_CUSTOM,
		SHIP_LEVEL,
		EQUIP_POTEN_OPTION,
		EQUIP_POTEN_OPTION_MAX,
		EQUIP_SET_OPTION,
		EQUIP_STAT
	}

	public delegate void OnToggleOperSkillList(bool bSelect);

	public Image m_imgIcon;

	public NKCComTMPUIText m_lbTitle;

	public NKCComTMPUIText m_lbDescription;

	public NKCUIComToggle m_ctgl;

	public GameObject m_objDetail;

	private OnToggleOperSkillList m_callback;

	private void SetString(string strTitle, string strDesc = "")
	{
		NKCUtil.SetLabelText(m_lbTitle, strTitle);
		NKCUtil.SetLabelText(m_lbDescription, strDesc);
		NKCUtil.SetGameobjectActive(m_lbDescription.gameObject, !string.IsNullOrEmpty(strDesc));
	}

	public void SetData(SPAC_TAG_TYPE type, int val)
	{
		switch (type)
		{
		case SPAC_TAG_TYPE.UNIT_LEVEL:
		case SPAC_TAG_TYPE.OPER_LEVEL:
		case SPAC_TAG_TYPE.SHIP_LEVEL:
			NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_SELEC_TAG_LEVEL"));
			SetString(string.Format(NKCStringTable.GetString("SI_DP_SLOT_CHOICE_VIEWER_TAG_LEVEL"), val));
			break;
		case SPAC_TAG_TYPE.UNIT_SKILL_LEVEL:
			NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_SELEC_TAG_UNIT_SKILL_LEVEL"));
			SetString(NKCStringTable.GetString("SI_DP_SLOT_CHOICE_VIEWER_TAG_SKILL_LEVEL"));
			break;
		case SPAC_TAG_TYPE.UNIT_TACTIC:
			NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_SELEC_TAG_UNIT_TACTIC"));
			SetString(string.Format(NKCStringTable.GetString("SI_DP_SLOT_CHOICE_VIEWER_TAG_TACTIC_UPDATE"), val));
			break;
		case SPAC_TAG_TYPE.UNIT_REACTOR:
			NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_SELEC_TAG_UNIT_REACTOR"));
			SetString(string.Format(NKCStringTable.GetString("SI_DP_SLOT_CHOICE_VIEWER_TAG_REACTOR_1"), val), NKCStringTable.GetString("SI_DP_SLOT_CHOICE_VIEWER_TAG_REACTOR_2"));
			break;
		case SPAC_TAG_TYPE.UNIT_LOYALTY:
			NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_SELEC_TAG_UNIT_LOYALTY"));
			SetString(string.Format(NKCStringTable.GetString("SI_DP_SLOT_CHOICE_VIEWER_TAG_LOYALTY"), val));
			break;
		case SPAC_TAG_TYPE.OPER_MAIN_SKILL_LEVEL:
			NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_SELEC_TAG_OPR_SKILL_TACTICUPDATE"));
			SetString(string.Format(NKCStringTable.GetString("SI_DP_SLOT_CHOICE_VIEWER_TAG_OPR_SKILL_1"), val));
			break;
		case SPAC_TAG_TYPE.OPER_SUB_SKILL_LEVEL:
			NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_SELEC_TAG_OPR_SKILL_LEVEL"));
			SetString(string.Format(NKCStringTable.GetString("SI_DP_SLOT_CHOICE_VIEWER_TAG_OPR_PASSIVE_SKILL"), val));
			break;
		case SPAC_TAG_TYPE.OPER_SUB_SKILL_CUSTOM:
			m_ctgl?.Select(bSelect: false, bForce: true);
			NKCUtil.SetToggleValueChangedDelegate(m_ctgl, OnSelectToggle);
			NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_SELEC_TAG_OPR_SKILL_SELEC"));
			SetString(NKCStringTable.GetString("SI_DP_SLOT_CHOICE_VIEWER_TAG_OPR_PASSIVE_SKILL_SELECTION"));
			break;
		case SPAC_TAG_TYPE.EQUIP_POTEN_OPTION:
			NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_SELEC_TAG_MISC_POTEN"));
			SetString(NKCStringTable.GetString("SI_DP_SLOT_CHOICE_VIEWER_TAG_HIDDEN_OPTION_SELECTION"));
			break;
		case SPAC_TAG_TYPE.EQUIP_POTEN_OPTION_MAX:
			NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_SELEC_TAG_MISC_POTEN_MAX"));
			SetString(NKCStringTable.GetString("SI_DP_SLOT_CHOICE_VIEWER_TAG_HIDDEN_OPTION_MAX"));
			break;
		case SPAC_TAG_TYPE.EQUIP_SET_OPTION:
			NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_SELEC_TAG_MISC_SET"));
			SetString(NKCStringTable.GetString("SI_DP_SLOT_CHOICE_VIEWER_TAG_SET_OPTION_SELECTION"));
			break;
		case SPAC_TAG_TYPE.EQUIP_STAT:
			NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_SELEC_TAG_MISC_STAT"));
			SetString(NKCStringTable.GetString("SI_DP_SLOT_CHOICE_VIEWER_TAG_ADDITIONAL_OPTION_SELECTION"));
			break;
		default:
			Debug.Log($"[NKCUIItemListTagSlot::SetData]Not Support Data Type : {type}, value : {val}");
			break;
		}
		m_ctgl?.SetLock(type != SPAC_TAG_TYPE.OPER_SUB_SKILL_CUSTOM);
		NKCUtil.SetGameobjectActive(m_objDetail, type == SPAC_TAG_TYPE.OPER_SUB_SKILL_CUSTOM);
	}

	public void SetCallBack(OnToggleOperSkillList callback)
	{
		m_callback = callback;
	}

	private void OnSelectToggle(bool bSelect)
	{
		m_callback?.Invoke(bSelect);
	}
}
