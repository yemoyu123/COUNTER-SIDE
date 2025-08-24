using System.Collections.Generic;
using NKC.UI.Collection;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComContractInfoOpr : MonoBehaviour
{
	private const string SpriteBundleName = "BANNER_COMMON_PREFAB_Sprite";

	public string m_OperatorStrID;

	public bool m_bSetDataOnStart;

	[Header("등급")]
	public Image m_imgGrade;

	[Header("오퍼레이터 이름")]
	public Text m_lbName;

	[Header("상세보기 버튼")]
	public NKCUIComStateButton m_btnDetail;

	[Header("스킬")]
	public NKCUIOperatorSkill m_Skill;

	[Header("스킬 콤보")]
	public List<NKCGameHudComboSlot> m_lstComboSlot;

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
		if (!m_bSetDataOnStart)
		{
			SetData();
		}
	}

	private void Start()
	{
		if (m_bSetDataOnStart)
		{
			SetData();
		}
	}

	public void SetData()
	{
		m_UnitTempletBase = NKMUnitTempletBase.Find(m_OperatorStrID);
		if (m_UnitTempletBase == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		if (m_UnitTempletBase.m_NKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetImageSprite(m_imgGrade, GetSpriteUnitGrade(m_UnitTempletBase.m_NKM_UNIT_GRADE));
		NKCUtil.SetLabelText(m_lbName, m_UnitTempletBase.GetUnitName());
		NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(m_UnitTempletBase.m_lstSkillStrID[0]);
		if (skillTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		if (skillTemplet.m_OperSkillType == OperatorSkillType.m_Tactical)
		{
			NKMTacticalCommandTemplet tacticalCommandTempletByStrID = NKMTacticalCommandManager.GetTacticalCommandTempletByStrID(skillTemplet.m_OperSkillTarget);
			if (tacticalCommandTempletByStrID.m_listComboType != null && tacticalCommandTempletByStrID.m_listComboType.Count > 0)
			{
				List<NKMTacticalCombo> listComboType = tacticalCommandTempletByStrID.m_listComboType;
				for (int i = 0; i < m_lstComboSlot.Count; i++)
				{
					if (listComboType.Count <= i)
					{
						NKCUtil.SetGameobjectActive(m_lstComboSlot[i].gameObject, bValue: false);
						continue;
					}
					NKCUtil.SetGameobjectActive(m_lstComboSlot[i].gameObject, bValue: true);
					NKCUtil.SetImageSprite(m_lstComboSlot[i].m_img, GetUnitRoleIconAssetName(listComboType[i].m_NKM_UNIT_ROLE_TYPE));
				}
			}
		}
		if (m_Skill != null)
		{
			m_Skill.SetDataForCollection(m_UnitTempletBase.m_UnitID);
		}
		NKCUtil.SetGameobjectActive(m_objNotHave, NKCScenManager.CurrentUserData().m_ArmyData.GetUnitCountByID(m_UnitTempletBase.m_UnitID) == 0);
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
		NKCUICollectionOperatorInfoV2.CheckInstanceAndOpen(NKCOperatorUtil.GetDummyOperator(m_UnitTempletBase.m_UnitID, bSetMaximum: true), null, NKCUICollectionOperatorInfoV2.eCollectionState.CS_PROFILE, NKCUIUpsideMenu.eMode.BackButtonOnly);
	}
}
