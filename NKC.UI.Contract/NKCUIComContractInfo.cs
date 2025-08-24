using NKC.UI.Collection;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Contract;

public class NKCUIComContractInfo : MonoBehaviour
{
	private const string SpriteBundleName = "BANNER_COMMON_PREFAB_Sprite";

	public string m_UnitStrID;

	public Image m_imgRankIcon;

	public GameObject m_objAwakenFX;

	public Text m_lbName;

	public Text m_lbSubName;

	public Image m_imgClass;

	public Text m_lbClass;

	public NKCUIComStateButton m_btnDetail;

	[Header("미획득 표기")]
	public GameObject m_objNotHave;

	private NKMUnitTempletBase m_UnitTempletBase;

	public void Awake()
	{
		if (m_btnDetail != null)
		{
			m_btnDetail.PointerClick.RemoveAllListeners();
			m_btnDetail.PointerClick.AddListener(OnClickDetail);
		}
		SetData();
	}

	private void OnEnable()
	{
		SetData();
	}

	public void SetData()
	{
		m_UnitTempletBase = NKMUnitTempletBase.Find(m_UnitStrID);
		if (m_UnitTempletBase == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetImageSprite(m_imgRankIcon, GetSpriteUnitGrade(m_UnitTempletBase.m_NKM_UNIT_GRADE));
		NKCUtil.SetGameobjectActive(m_objAwakenFX, m_UnitTempletBase.m_bAwaken);
		NKCUtil.SetLabelText(m_lbName, m_UnitTempletBase.GetUnitName());
		NKCUtil.SetImageSprite(m_imgClass, GetUnitRoleIconAssetName(m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE));
		NKCUtil.SetLabelText(m_lbClass, NKCUtilString.GetRoleText(m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE, bAwaken: false));
		NKCUtil.SetGameobjectActive(m_lbSubName, m_UnitTempletBase.m_bAwaken);
		NKCUtil.SetLabelText(m_lbSubName, m_UnitTempletBase.GetUnitTitle());
		NKCUtil.SetGameobjectActive(m_objNotHave, !NKCScenManager.CurrentUserData().m_ArmyData.HaveUnit(m_UnitTempletBase.m_UnitID, bIncludeRearm: true));
	}

	private Sprite GetSpriteUnitGrade(NKM_UNIT_GRADE grade)
	{
		string text = "";
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("BANNER_COMMON_PREFAB_Sprite", grade switch
		{
			NKM_UNIT_GRADE.NUG_R => "BANNER_COMMON_PREFAB_RANK_R", 
			NKM_UNIT_GRADE.NUG_SR => "BANNER_COMMON_PREFAB_RANK_SR", 
			NKM_UNIT_GRADE.NUG_SSR => "BANNER_COMMON_PREFAB_RANK_SSR", 
			_ => "BANNER_COMMON_PREFAB_RANK_N", 
		});
	}

	private Sprite GetUnitRoleIconAssetName(NKM_UNIT_ROLE_TYPE roleType)
	{
		string text = "";
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("BANNER_COMMON_PREFAB_Sprite", roleType switch
		{
			NKM_UNIT_ROLE_TYPE.NURT_STRIKER => "BANNER_COMMON_PREFAB_CLASS_STRIKER", 
			NKM_UNIT_ROLE_TYPE.NURT_RANGER => "BANNER_COMMON_PREFAB_CLASS_RANGER", 
			NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => "BANNER_COMMON_PREFAB_CLASS_DEFENCE", 
			NKM_UNIT_ROLE_TYPE.NURT_SNIPER => "BANNER_COMMON_PREFAB_CLASS_SNIPER", 
			NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => "BANNER_COMMON_PREFAB_CLASS_SUPPORTER", 
			NKM_UNIT_ROLE_TYPE.NURT_SIEGE => "BANNER_COMMON_PREFAB_CLASS_SIEGE", 
			NKM_UNIT_ROLE_TYPE.NURT_TOWER => "BANNER_COMMON_PREFAB_CLASS_TOWER", 
			_ => "", 
		});
	}

	private void OnClickDetail()
	{
		NKCUICollectionUnitInfo.CheckInstanceAndOpen(NKCUtil.MakeDummyUnit(m_UnitTempletBase.m_UnitID, 100, 3), null, null, NKCUICollectionUnitInfo.eCollectionState.CS_PROFILE, isGauntlet: false, NKCUIUpsideMenu.eMode.BackButtonOnly);
	}
}
