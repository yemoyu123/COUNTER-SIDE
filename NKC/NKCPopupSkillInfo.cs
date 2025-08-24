using System;
using System.Collections.Generic;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCPopupSkillInfo : MonoBehaviour
{
	[Serializable]
	public struct ShipSkillSlot
	{
		public GameObject RANK_SLOT;

		public CanvasGroup RANK_SLOT_CANVAS;

		public Image RANK_ICON;

		public Text RANK_COUNT;

		public Text SKILL_TEXT;
	}

	public NKCUISkillSlot m_NKCUISkillSlot;

	public GameObject m_objSkillLvMax;

	[Header("상세정보")]
	public CanvasGroup m_Canvas;

	public Text m_lbSkillName;

	public Text m_lbSkillType;

	public GameObject m_objSkillLockRoot;

	public List<GameObject> m_lstObjSkillLockStar;

	public GameObject m_objSkillCooldown;

	public Image m_imgSkillCooldown;

	public Text m_lbSkillCooldown;

	public GameObject m_objSkillAttackCount;

	public Text m_lbSkillAttackCount;

	public NKCComTMPUIText m_lbSkillDescription;

	public RectTransform m_rtSkillDescription;

	[Header("스킬 레벨 정보")]
	public RectTransform m_rtSkillInfoPanel;

	public NKCUIComSkillLevelDetail m_pfbSkillLevelDetail;

	private Stack<NKCUIComSkillLevelDetail> m_stkSkillLevelDetail = new Stack<NKCUIComSkillLevelDetail>();

	private List<NKCUIComSkillLevelDetail> m_lstSkillLevelDetail = new List<NKCUIComSkillLevelDetail>();

	public List<ShipSkillSlot> m_lstShipSkillDetail;

	[Header("스킬 표시")]
	public GameObject m_obj_LAYOUT;

	public GameObject m_obj_NKM_UI_POPUP_SKILL_LV;

	public GameObject m_obj_NKM_UI_POPUP_SHIP_SKILL_LV;

	[Header("리더 스킬")]
	public GameObject m_objLeaderSkillBG;

	public GameObject m_objDot;

	public GameObject m_objLeaderSkillIcon;

	public void InitUI()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_NKCUISkillSlot.Init(null);
		m_Canvas.alpha = 1f;
	}

	private void SetSkillType(NKM_SKILL_TYPE type)
	{
		m_lbSkillType.color = NKCUtil.GetSkillTypeColor(type);
		m_lbSkillType.text = NKCUtilString.GetSkillTypeName(type);
	}

	private int GetSkillAttackCount(NKMUnitSkillTemplet unitTemplet)
	{
		return unitTemplet?.m_AttackCount ?? 0;
	}

	public void OpenForUnit(NKMUnitSkillTemplet skillTemplet, int unitStarGradeMax, int unitLimitBreakLevel, int rearmGrade, bool bIsFuryType)
	{
		if (skillTemplet == null)
		{
			Debug.LogError("Skill Templet Null!!");
			return;
		}
		m_NKCUISkillSlot.SetData(skillTemplet, skillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER);
		m_NKCUISkillSlot.LockSkill(NKMUnitSkillManager.IsLockedSkill(skillTemplet.m_ID, unitLimitBreakLevel));
		NKCUtil.SetGameobjectActive(m_objSkillLvMax, NKMUnitSkillManager.GetMaxSkillLevelFromLimitBreakLevel(skillTemplet.m_ID, unitLimitBreakLevel) == skillTemplet.m_Level);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_obj_LAYOUT, bValue: true);
		NKCUtil.SetGameobjectActive(m_obj_NKM_UI_POPUP_SKILL_LV, bValue: true);
		NKCUtil.SetGameobjectActive(m_obj_NKM_UI_POPUP_SHIP_SKILL_LV, bValue: false);
		if (m_lbSkillName != null)
		{
			m_lbSkillName.text = skillTemplet.GetSkillName();
		}
		SetSkillType(skillTemplet.m_NKM_SKILL_TYPE);
		if (!NKMUnitSkillManager.IsLockedSkill(skillTemplet.m_ID, unitLimitBreakLevel))
		{
			NKCUtil.SetGameobjectActive(m_objSkillLockRoot, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objSkillLockRoot, bValue: true);
			NKCUtil.SetSkillUnlockStarRank(m_lstObjSkillLockStar, skillTemplet, unitStarGradeMax);
		}
		if (skillTemplet.m_fCooltimeSecond > 0f)
		{
			NKCUtil.SetGameobjectActive(m_objSkillCooldown, bValue: true);
			if (bIsFuryType)
			{
				NKCUtil.SetImageSprite(m_imgSkillCooldown, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_ICON_GAUNTLET_SMALL"));
				NKCUtil.SetLabelText(m_lbSkillCooldown, string.Format(NKCUtilString.GET_STRING_COUNT_ONE_PARAM, skillTemplet.m_fCooltimeSecond));
			}
			else
			{
				NKCUtil.SetImageSprite(m_imgSkillCooldown, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_ICON_TIME"));
				NKCUtil.SetLabelText(m_lbSkillCooldown, string.Format(NKCUtilString.GET_STRING_TIME_SECOND_ONE_PARAM, skillTemplet.m_fCooltimeSecond));
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objSkillCooldown, bValue: false);
		}
		int skillAttackCount = GetSkillAttackCount(skillTemplet);
		if (skillAttackCount > 0)
		{
			NKCUtil.SetGameobjectActive(m_objSkillAttackCount, bValue: true);
			NKCUtil.SetLabelText(m_lbSkillAttackCount, string.Format(NKCUtilString.GET_STRING_SKILL_ATTACK_COUNT_ONE_PARAM, skillAttackCount));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objSkillAttackCount, bValue: false);
		}
		if (skillTemplet.m_Level == 1)
		{
			NKCUtil.SetLabelText(m_lbSkillDescription, skillTemplet.GetSkillDesc());
		}
		else
		{
			NKMUnitSkillTemplet skillTemplet2 = NKMUnitSkillManager.GetSkillTemplet(skillTemplet.m_ID, 1);
			if (skillTemplet2 != null)
			{
				NKCUtil.SetLabelText(m_lbSkillDescription, skillTemplet2.GetSkillDesc());
			}
		}
		UpdateUnitSkillDetail(skillTemplet, rearmGrade);
		if (m_rtSkillDescription != null)
		{
			m_rtSkillDescription.anchoredPosition = new Vector2(m_rtSkillDescription.anchoredPosition.x, 0f);
		}
		UpdateLeaderSkillUI(skillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_LEADER);
	}

	private void UpdateUnitSkillDetail(NKMUnitSkillTemplet skillTemplet, int rearmGradeLv)
	{
		foreach (KeyValuePair<int, NKMUnitSkillTemplet> dicTemplet in NKMUnitSkillManager.GetSkillTempletContainer(skillTemplet.m_ID).dicTemplets)
		{
			if (dicTemplet.Value.m_Level != 1)
			{
				NKCUIComSkillLevelDetail nKCUIComSkillLevelDetail = null;
				nKCUIComSkillLevelDetail = ((m_stkSkillLevelDetail.Count > 0) ? m_stkSkillLevelDetail.Pop() : UnityEngine.Object.Instantiate(m_pfbSkillLevelDetail));
				if (null == nKCUIComSkillLevelDetail)
				{
					break;
				}
				nKCUIComSkillLevelDetail.gameObject.transform.SetParent(m_rtSkillInfoPanel);
				nKCUIComSkillLevelDetail.transform.localScale = Vector3.one;
				NKCUtil.SetGameobjectActive(nKCUIComSkillLevelDetail.gameObject, bValue: true);
				nKCUIComSkillLevelDetail.SetData(dicTemplet.Value.m_ID, dicTemplet.Value.m_Level <= skillTemplet.m_Level, dicTemplet.Value.m_Level);
				m_lstSkillLevelDetail.Add(nKCUIComSkillLevelDetail);
			}
		}
	}

	public void Clear()
	{
		for (int i = 0; i < m_lstSkillLevelDetail.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstSkillLevelDetail[i], bValue: false);
			m_stkSkillLevelDetail.Push(m_lstSkillLevelDetail[i]);
		}
		m_lstSkillLevelDetail.Clear();
		while (m_stkSkillLevelDetail.Count > 0)
		{
			NKCUIComSkillLevelDetail nKCUIComSkillLevelDetail = m_stkSkillLevelDetail.Pop();
			if (nKCUIComSkillLevelDetail != null)
			{
				UnityEngine.Object.Destroy(nKCUIComSkillLevelDetail.gameObject);
			}
		}
	}

	public void OpenForShip(int slotIdx, int UnitID, int hasShipLv = 0)
	{
		int maxLevelShipID = NKMShipManager.GetMaxLevelShipID(UnitID);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(maxLevelShipID);
		if (unitTempletBase == null)
		{
			Debug.LogError($"함선 정보를 획득 할 수 없습니다. id({maxLevelShipID}) 의 함선정보를 확인해주세요. ");
			return;
		}
		NKMShipSkillTemplet shipSkillTempletByIndex = NKMShipSkillManager.GetShipSkillTempletByIndex(unitTempletBase, slotIdx);
		if (shipSkillTempletByIndex == null)
		{
			Debug.Log($"함선 스킬정보를 확인 할 수 없습니다. shipID : {maxLevelShipID} slot idx : {slotIdx}");
			return;
		}
		NKCUtil.SetLabelText(m_lbSkillName, shipSkillTempletByIndex.GetName());
		NKCUtil.SetLabelText(m_lbSkillType, NKCUtilString.GetSkillTypeName(shipSkillTempletByIndex.m_NKM_SKILL_TYPE));
		NKCUtil.SetLabelText(m_lbSkillDescription, shipSkillTempletByIndex.GetDesc());
		bool flag = false;
		if (m_NKCUISkillSlot != null)
		{
			m_NKCUISkillSlot.Cleanup();
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(UnitID);
			if (unitTempletBase2 != null)
			{
				NKMShipSkillTemplet shipSkillTempletByIndex2 = NKMShipSkillManager.GetShipSkillTempletByIndex(unitTempletBase2, slotIdx);
				flag = hasShipLv == 0 || shipSkillTempletByIndex2 == null;
				m_NKCUISkillSlot.SetShipData(shipSkillTempletByIndex, flag);
			}
		}
		NKCUtil.SetGameobjectActive(m_objSkillLvMax, bValue: false);
		int num = UnitID % 1000;
		int num2 = 20;
		int num3 = 0;
		int num4 = 0;
		HashSet<string> hashSet = new HashSet<string>();
		for (int i = 0; i < 6; i++)
		{
			NKMUnitTempletBase unitTempletBase3 = NKMUnitManager.GetUnitTempletBase((num2 + i + 1) * 1000 + num);
			if (unitTempletBase3 == null)
			{
				continue;
			}
			NKMShipSkillTemplet shipSkillTempletByIndex3 = NKMShipSkillManager.GetShipSkillTempletByIndex(unitTempletBase3, slotIdx);
			if (shipSkillTempletByIndex3 != null && m_lstShipSkillDetail.Count > hashSet.Count && !hashSet.Contains(shipSkillTempletByIndex3.m_ShipSkillStrID))
			{
				NKCUtil.SetLabelText(m_lstShipSkillDetail[hashSet.Count].RANK_COUNT, (i + 1).ToString());
				NKCUtil.SetLabelText(m_lstShipSkillDetail[hashSet.Count].SKILL_TEXT, shipSkillTempletByIndex3.GetBuildDesc());
				NKCUtil.SetImageSprite(m_lstShipSkillDetail[hashSet.Count].RANK_ICON, NKCUtil.GetSpriteCommonIConStar(i + 1));
				hashSet.Add(shipSkillTempletByIndex3.m_ShipSkillStrID);
				if (hasShipLv > 0 && i < hasShipLv)
				{
					num3 = hashSet.Count;
					NKCUtil.SetLabelText(m_lbSkillName, shipSkillTempletByIndex3.GetName());
					NKCUtil.SetLabelText(m_lbSkillDescription, shipSkillTempletByIndex3.GetDesc());
				}
				if (flag && num4 == 0)
				{
					num4 = i + 1;
				}
			}
		}
		if (flag)
		{
			for (int j = 0; j < m_lstShipSkillDetail.Count; j++)
			{
				if (j < hashSet.Count)
				{
					NKCUtil.SetLabelTextColor(m_lstShipSkillDetail[j].RANK_COUNT, NKCUtil.GetColor("#FFFFFF"));
					CanvasGroup rANK_SLOT_CANVAS = m_lstShipSkillDetail[j].RANK_SLOT_CANVAS;
					if (rANK_SLOT_CANVAS != null)
					{
						rANK_SLOT_CANVAS.alpha = 0.4f;
					}
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstShipSkillDetail[j].RANK_SLOT, bValue: false);
				}
			}
			NKCUtil.SetStarRank(m_lstObjSkillLockStar, num4, 6);
			NKCUtil.SetGameobjectActive(m_obj_LAYOUT, bValue: false);
		}
		else
		{
			for (int k = 0; k < m_lstShipSkillDetail.Count; k++)
			{
				if (hashSet.Count > k)
				{
					CanvasGroup rANK_SLOT_CANVAS2 = m_lstShipSkillDetail[k].RANK_SLOT_CANVAS;
					if (rANK_SLOT_CANVAS2 != null)
					{
						if (k + 1 == num3)
						{
							rANK_SLOT_CANVAS2.alpha = 1f;
						}
						else
						{
							rANK_SLOT_CANVAS2.alpha = 0.4f;
						}
					}
					if (k >= num3)
					{
						NKCUtil.SetLabelTextColor(m_lstShipSkillDetail[k].RANK_COUNT, NKCUtil.GetColor("#FFFFFF"));
					}
					else
					{
						NKCUtil.SetLabelTextColor(m_lstShipSkillDetail[k].RANK_COUNT, NKCUtil.GetColor("#FFCF3B"));
					}
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_lstShipSkillDetail[k].RANK_SLOT, bValue: false);
				}
			}
			NKCUtil.SetGameobjectActive(m_obj_LAYOUT, bValue: true);
			NKCUtil.SetGameobjectActive(m_objSkillAttackCount, bValue: false);
			if (shipSkillTempletByIndex.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_SHIP_ACTIVE && shipSkillTempletByIndex.m_fCooltimeSecond > 0f)
			{
				NKCUtil.SetGameobjectActive(m_objSkillCooldown, bValue: true);
				NKCUtil.SetImageSprite(m_imgSkillCooldown, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_ICON_TIME"));
				NKCUtil.SetLabelText(m_lbSkillCooldown, string.Format(NKCUtilString.GET_STRING_TIME_SECOND_ONE_PARAM, shipSkillTempletByIndex.m_fCooltimeSecond));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objSkillCooldown, bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_objSkillLockRoot, flag);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_obj_NKM_UI_POPUP_SKILL_LV, bValue: false);
		NKCUtil.SetGameobjectActive(m_obj_NKM_UI_POPUP_SHIP_SKILL_LV, bValue: true);
		if (m_rtSkillDescription != null)
		{
			m_rtSkillDescription.anchoredPosition = new Vector2(m_rtSkillDescription.anchoredPosition.x, 0f);
		}
		UpdateLeaderSkillUI();
	}

	private void UpdateLeaderSkillUI(bool bLeader = false)
	{
		NKCUtil.SetGameobjectActive(m_objLeaderSkillBG, bLeader);
		NKCUtil.SetGameobjectActive(m_objLeaderSkillIcon, bLeader);
		NKCUtil.SetGameobjectActive(m_objDot, !bLeader);
	}
}
