using System.Collections;
using NKM;
using NKM.Templet;
using NKM.Unit;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Result;

public class NKCUIResultUnitSlot : MonoBehaviour
{
	[Header("Common")]
	public Image m_imgBG;

	public Image m_imgUnitFace;

	public NKCUIComTextUnitLevel m_lbLevel;

	public Image m_imgBattleType;

	public GameObject m_objLeader;

	public GameObject m_objPermanantContract;

	[Header("Exp")]
	public Slider m_sliderExpOld;

	public Slider m_sliderExpNew;

	public Text m_lbExpGain;

	public GameObject m_objLevelUp;

	public GameObject m_objExpFX;

	public GameObject m_objExpMax;

	[Header("애사심 증감")]
	public GameObject m_objLoyalty;

	public Animator m_aniLoyalty;

	[Header("각성 Fx")]
	public Animator m_animAwakenFX;

	private int m_iLastLevel;

	private int m_iLastExp;

	private bool m_bLevelUp;

	private Vector3 m_ExpFxOrgPosition;

	private NKCUIResultSubUIUnitExp.UNIT_LOYALTY m_eLoyalty;

	private const string LOYALTY_UP_ANI_NAME = "AB_UI_NKM_UI_RESULT_LOYALTY_UP_BASE";

	private const string LOYALTY_DOWN_ANI_NAME = "AB_UI_NKM_UI_RESULT_LOYALTY_DOWN_BASE";

	private int m_UnitID;

	public bool ProgressFinished { get; private set; }

	public void SetData(NKCUIResultSubUIUnitExp.UnitLevelupUIData unitLevelupData, Sprite spSlotBGN, Sprite spSlotBGR, Sprite spSlotBGSR, Sprite spSlotBGSSR)
	{
		StopAllCoroutines();
		NKCUtil.SetGameobjectActive(m_objExpMax, bValue: false);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitLevelupData.m_UnitData.m_UnitID);
		m_UnitID = unitLevelupData.m_UnitData.m_UnitID;
		Sprite sp = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitLevelupData.m_UnitData);
		NKCUtil.SetImageSprite(m_imgUnitFace, sp, bDisableIfSpriteNull: true);
		NKCUtil.SetImageSprite(m_imgBattleType, NKCResourceUtility.GetOrLoadUnitRoleAttackTypeIcon(unitTempletBase, bSmall: true), bDisableIfSpriteNull: true);
		switch (unitTempletBase.m_NKM_UNIT_GRADE)
		{
		case NKM_UNIT_GRADE.NUG_N:
			m_imgBG.sprite = spSlotBGN;
			break;
		case NKM_UNIT_GRADE.NUG_R:
			m_imgBG.sprite = spSlotBGR;
			break;
		case NKM_UNIT_GRADE.NUG_SR:
			m_imgBG.sprite = spSlotBGSR;
			break;
		case NKM_UNIT_GRADE.NUG_SSR:
			m_imgBG.sprite = spSlotBGSSR;
			break;
		default:
			Debug.LogError("Unexpected unit Grade Type");
			m_imgBG.sprite = spSlotBGN;
			break;
		}
		m_lbLevel?.SetText(unitLevelupData.m_iLevelOld.ToString(), unitLevelupData.m_UnitData);
		NKCUtil.SetGameobjectActive(m_objLeader, bValue: false);
		NKCUtil.SetGameobjectActive(m_objPermanantContract, unitLevelupData.m_UnitData.IsPermanentContract);
		NKCUtil.SetGameobjectActive(m_objLevelUp, bValue: false);
		NKMUnitExpTemplet nKMUnitExpTemplet = NKMUnitExpTemplet.FindByUnitId(unitLevelupData.m_UnitData.m_UnitID, unitLevelupData.m_iLevelOld);
		if (nKMUnitExpTemplet == null)
		{
			m_sliderExpOld.value = 0f;
			m_sliderExpNew.value = 0f;
		}
		else if (nKMUnitExpTemplet.m_iExpRequired == 0)
		{
			m_sliderExpOld.value = 1f;
			m_sliderExpNew.value = 1f;
		}
		else
		{
			float value = (float)unitLevelupData.m_iExpOld / (float)nKMUnitExpTemplet.m_iExpRequired;
			m_sliderExpOld.value = value;
			m_sliderExpNew.value = value;
		}
		m_iLastLevel = unitLevelupData.m_iLevelNew;
		m_iLastExp = unitLevelupData.m_iExpNew;
		m_bLevelUp = unitLevelupData.m_iLevelOld != unitLevelupData.m_iLevelNew;
		m_lbExpGain.text = string.Format(NKCUtilString.GET_STRING_PLUS_EXP_ONE_PARAM, unitLevelupData.m_iTotalExpGain);
		NKCUtil.SetGameobjectActive(m_objLoyalty, bValue: false);
		m_eLoyalty = unitLevelupData.m_loyalty;
	}

	public void SetLeader(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objLeader, bValue);
	}

	public void StartExpProcess(int unitID, int oldLevel, int oldExp, int newLevel, int newExp, int expGain)
	{
		m_UnitID = unitID;
		ProgressFinished = false;
		m_ExpFxOrgPosition = m_objExpFX.transform.localPosition;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		NKCUtil.SetAwakenFX(m_animAwakenFX, unitTempletBase);
		StartCoroutine(ExpProcess(unitID, oldLevel, oldExp, newLevel, newExp, expGain));
	}

	private IEnumerator ExpProcess(int unitID, int oldLevel, int oldExp, int newLevel, int newExp, int expGain)
	{
		m_iLastLevel = newLevel;
		m_iLastExp = newExp;
		m_bLevelUp = oldLevel != newLevel;
		ProgressFinished = false;
		NKMUnitExpTemplet nKMUnitExpTemplet = NKMUnitExpTemplet.FindByUnitId(unitID, oldLevel);
		if (nKMUnitExpTemplet.m_iExpRequired == 0)
		{
			Finish();
			yield break;
		}
		float beginValue = (float)oldExp / (float)nKMUnitExpTemplet.m_iExpRequired;
		m_sliderExpOld.value = beginValue;
		m_sliderExpNew.value = beginValue;
		m_objExpFX.transform.localPosition = new Vector3(m_sliderExpNew.transform.localPosition.x + m_sliderExpNew.fillRect.GetWidth() * m_sliderExpNew.value, m_ExpFxOrgPosition.y, m_ExpFxOrgPosition.z);
		NKCUtil.SetGameobjectActive(m_objLevelUp, bValue: false);
		NKMUnitExpTemplet nKMUnitExpTemplet2 = NKMUnitExpTemplet.FindByUnitId(unitID, newLevel);
		bool num = nKMUnitExpTemplet2.m_iExpRequired == 0;
		float endValue = newLevel - oldLevel;
		if (!num)
		{
			endValue += (float)newExp / (float)nKMUnitExpTemplet2.m_iExpRequired;
		}
		float totalTime = (endValue - beginValue) * 0.6f;
		float deltaTime = 0f;
		int currentLevelUpCount = 0;
		while (deltaTime < totalTime)
		{
			float num2 = NKCUtil.TrackValue(TRACKING_DATA_TYPE.TDT_SLOWER, beginValue, endValue, deltaTime, totalTime);
			int num3 = (int)num2;
			if (currentLevelUpCount != num3)
			{
				currentLevelUpCount = num3;
				m_sliderExpOld.value = 0f;
				m_objLevelUp.SetActive(value: false);
				m_objLevelUp.SetActive(value: true);
				m_lbLevel.text = (oldLevel + currentLevelUpCount).ToString();
			}
			m_sliderExpNew.value = num2 % 1f;
			m_objExpFX.transform.localPosition = new Vector3(m_sliderExpNew.transform.localPosition.x + m_sliderExpNew.fillRect.GetWidth() * m_sliderExpNew.value, m_ExpFxOrgPosition.y, m_ExpFxOrgPosition.z);
			deltaTime += Time.deltaTime;
			yield return null;
		}
		NKCUtil.SetGameobjectActive(m_objLoyalty, m_eLoyalty != NKCUIResultSubUIUnitExp.UNIT_LOYALTY.None);
		if (m_eLoyalty == NKCUIResultSubUIUnitExp.UNIT_LOYALTY.Up)
		{
			m_aniLoyalty.Play("AB_UI_NKM_UI_RESULT_LOYALTY_UP_BASE");
		}
		else if (m_eLoyalty == NKCUIResultSubUIUnitExp.UNIT_LOYALTY.Down)
		{
			m_aniLoyalty.Play("AB_UI_NKM_UI_RESULT_LOYALTY_DOWN_BASE");
		}
		yield return null;
		Finish();
	}

	public void Finish()
	{
		ProgressFinished = true;
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		StopAllCoroutines();
		m_lbLevel.text = m_iLastLevel.ToString();
		if (m_bLevelUp)
		{
			m_sliderExpOld.value = 0f;
			NKCUtil.SetGameobjectActive(m_objLevelUp, bValue: true);
		}
		NKMUnitExpTemplet nKMUnitExpTemplet = NKMUnitExpTemplet.FindByUnitId(m_UnitID, m_iLastLevel);
		if (nKMUnitExpTemplet == null || nKMUnitExpTemplet.m_iExpRequired == 0)
		{
			m_sliderExpNew.value = 1f;
		}
		else
		{
			float value = (float)m_iLastExp / (float)nKMUnitExpTemplet.m_iExpRequired;
			m_sliderExpNew.value = value;
		}
		m_objExpFX.transform.localPosition = new Vector3(m_sliderExpNew.transform.localPosition.x + m_sliderExpNew.fillRect.GetWidth() * m_sliderExpNew.value, m_ExpFxOrgPosition.y, m_ExpFxOrgPosition.z);
		NKCUtil.SetGameobjectActive(m_objLoyalty, m_eLoyalty != NKCUIResultSubUIUnitExp.UNIT_LOYALTY.None);
		if (m_eLoyalty == NKCUIResultSubUIUnitExp.UNIT_LOYALTY.Up)
		{
			if (!m_aniLoyalty.GetCurrentAnimatorStateInfo(0).IsName("AB_UI_NKM_UI_RESULT_LOYALTY_UP_BASE"))
			{
				m_aniLoyalty.Play("AB_UI_NKM_UI_RESULT_LOYALTY_UP_BASE");
			}
		}
		else if (m_eLoyalty == NKCUIResultSubUIUnitExp.UNIT_LOYALTY.Down && !m_aniLoyalty.GetCurrentAnimatorStateInfo(0).IsName("AB_UI_NKM_UI_RESULT_LOYALTY_DOWN_BASE"))
		{
			m_aniLoyalty.Play("AB_UI_NKM_UI_RESULT_LOYALTY_DOWN_BASE");
		}
	}
}
