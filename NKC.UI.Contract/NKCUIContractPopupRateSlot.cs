using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Contract;

public class NKCUIContractPopupRateSlot : MonoBehaviour
{
	public Text m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_RANK;

	public GameObject m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_UP;

	public Text m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_NAME;

	public Text m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_TYPE;

	public Text m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_CLASS;

	public Text m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_PERCENT;

	public GameObject m_objNoGet;

	public GameObject m_objAwaken;

	public GameObject m_objRearmment;

	[Header("픽업")]
	public GameObject m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_FEATURED;

	[Header("최대 2개")]
	public GameObject m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_PERCENTUP1;

	public GameObject m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_PERCENTUP2;

	[Header("RATE IMG")]
	public GameObject m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_RANK_IMG_SSR;

	public GameObject m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_RANK_IMG_SR;

	public GameObject m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_RANK_IMG_R;

	public GameObject m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_RANK_IMG_N;

	public void SetData(ContractUnitSlotData unitData, bool bPickup = false, bool bHideRatePercent = false)
	{
		if (unitData == null)
		{
			return;
		}
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_RANK, GetGradleText(unitData.grade));
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_RANK_IMG_SSR, unitData.grade == NKM_UNIT_GRADE.NUG_SSR);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_RANK_IMG_SR, unitData.grade == NKM_UNIT_GRADE.NUG_SR);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_RANK_IMG_R, unitData.grade == NKM_UNIT_GRADE.NUG_R);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_RANK_IMG_N, unitData.grade == NKM_UNIT_GRADE.NUG_N);
		string text = unitData.Name;
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitData.UnitID);
		if (unitData.type == NKM_UNIT_STYLE_TYPE.NUST_OPERATOR)
		{
			if (nKMUnitTempletBase != null)
			{
				NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_NAME, $"[{nKMUnitTempletBase.GetUnitTitle()}] {text}");
			}
			else
			{
				NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_NAME, text);
			}
			NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_CLASS, NKCUtilString.GET_STRING_OPERATOR_CONTRACT_STYLE);
			NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_TYPE, "");
		}
		else
		{
			if (nKMUnitTempletBase != null && (nKMUnitTempletBase.m_bAwaken || nKMUnitTempletBase.IsRearmUnit))
			{
				NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_NAME, $"[{nKMUnitTempletBase.GetUnitTitle()}] {text}");
			}
			else
			{
				NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_NAME, text);
			}
			NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_TYPE, NKCUtilString.GetUnitStyleType(unitData.type));
			NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_CLASS, NKCUtilString.GetRoleText(unitData.role, bAwaken: false));
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_PERCENT, !bHideRatePercent);
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_PERCENT, unitData.Percent.ToString("N3") + "%");
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_FEATURED, bPickup);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_PERCENTUP2, !bPickup && unitData.RatioUp);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_CONTRACT_RATE_SLOT_UP, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNoGet, !NKCScenManager.CurrentUserData().m_ArmyData.IsCollectedUnit(unitData.UnitID));
		NKCUtil.SetGameobjectActive(m_objAwaken, unitData.Awaken);
		NKCUtil.SetGameobjectActive(m_objRearmment, unitData.Rearm);
	}

	private string GetGradleText(NKM_UNIT_GRADE grade)
	{
		return grade switch
		{
			NKM_UNIT_GRADE.NUG_SSR => "SSR", 
			NKM_UNIT_GRADE.NUG_SR => "SR", 
			NKM_UNIT_GRADE.NUG_R => "R", 
			NKM_UNIT_GRADE.NUG_N => "N", 
			_ => "", 
		};
	}
}
