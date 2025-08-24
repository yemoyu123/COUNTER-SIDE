using NKC.UI.Tooltip;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperatorPassiveSlot : MonoBehaviour
{
	public delegate void OnSelectSlot(int id, int lv);

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_operator_info";

	private const string UI_ASSET_NAME = "NKM_UI_OPERATOR_POPUP_SKILL_PASSIVE_SLOT";

	private NKCAssetInstanceData m_InstanceData;

	private const string ASSET_BUNDLE_NAME_BIG = "ab_ui_nkm_ui_unit_selection";

	private const string UI_ASSET_NAME_BIG = "NKM_UI_OPERATOR_SKILL_SLOT";

	public Image[] m_SkillIcon;

	public Text[] m_SkillName;

	public Text[] m_SkillDesc;

	public Text[] m_SkillLv;

	public NKCUIComStateButton m_SkillIBnt;

	public NKCUIComStateButton m_SkillSelect;

	public GameObject m_ObjSelect;

	public GameObject m_ObjNormal;

	private int m_curSkillID;

	private int m_curSkillLevel;

	private OnSelectSlot m_dOnClick;

	public int Key => m_curSkillID;

	public static NKCUIOperatorPassiveSlot GetResource(bool bBig = false)
	{
		string bundleName = (bBig ? "ab_ui_nkm_ui_unit_selection" : "ab_ui_nkm_ui_operator_info");
		string assetName = (bBig ? "NKM_UI_OPERATOR_SKILL_SLOT" : "NKM_UI_OPERATOR_POPUP_SKILL_PASSIVE_SLOT");
		return GetResource(bundleName, assetName);
	}

	public static NKCUIOperatorPassiveSlot GetResource(string bundleName, string assetName)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(bundleName, assetName);
		if (nKCAssetInstanceData == null)
		{
			return null;
		}
		NKCUIOperatorPassiveSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIOperatorPassiveSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIOperatorPassiveSlot Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		return component;
	}

	public void Init()
	{
		if (null != m_SkillIBnt)
		{
			m_SkillIBnt.PointerDown.RemoveAllListeners();
			m_SkillIBnt.PointerDown.AddListener(OnPointDownSkill);
		}
		NKCUtil.SetBindFunction(m_SkillSelect, OnClick);
		m_curSkillID = 0;
		m_curSkillLevel = 0;
	}

	public void SetData(Sprite icon, string name, int skillID, int skillLv)
	{
		Image[] skillIcon = m_SkillIcon;
		for (int i = 0; i < skillIcon.Length; i++)
		{
			NKCUtil.SetImageSprite(skillIcon[i], icon);
		}
		Text[] skillName = m_SkillName;
		for (int i = 0; i < skillName.Length; i++)
		{
			NKCUtil.SetLabelText(skillName[i], name);
		}
		skillName = m_SkillLv;
		for (int i = 0; i < skillName.Length; i++)
		{
			NKCUtil.SetLabelText(skillName[i], string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, skillLv));
		}
		m_curSkillID = skillID;
		m_curSkillLevel = skillLv;
		skillName = m_SkillDesc;
		foreach (Text label in skillName)
		{
			NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(skillID);
			if (skillTemplet != null)
			{
				NKCUtil.SetLabelText(label, NKCOperatorUtil.MakeOperatorSkillDesc(skillTemplet, skillLv));
			}
		}
	}

	private void OnPointDownSkill(PointerEventData eventData)
	{
		if (NKCOperatorUtil.GetSkillTemplet(m_curSkillID) != null)
		{
			NKCUITooltip.Instance.Open(m_curSkillID, m_curSkillLevel, eventData.position);
		}
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	public void SetCallBack(OnSelectSlot _callBack)
	{
		m_dOnClick = _callBack;
	}

	private void OnClick()
	{
		m_dOnClick?.Invoke(m_curSkillID, m_curSkillLevel);
	}

	public void OnSelect(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_ObjSelect, bActive);
		NKCUtil.SetGameobjectActive(m_ObjNormal, !bActive);
	}
}
