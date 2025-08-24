using System.Collections.Generic;
using NKC.UI.Tooltip;
using NKM;
using NKM.Templet.Base;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC;

public class NKCGameHudCombo : MonoBehaviour
{
	public CanvasGroup m_cgComboSlots;

	public NKCGameHudComboSlot[] m_arNKCGameHudComboSlot;

	public Image m_imgSkill;

	public Image m_imgSkillGray;

	public NKCUIComStateButton m_cbtnSkill;

	public GameObject m_objSkillCoolTime;

	public Image m_imgSkillCoolTime;

	public Animator m_amtCoolTimeOn;

	public Text m_lbSkillLevel;

	public GameObject m_objActive;

	public TextMeshProUGUI m_lbActiveTime;

	public GameObject m_objCoolTime;

	public Slider m_sldCoolTime;

	public Image m_imgSldFill;

	private int m_SlotMaxCount;

	private NKMTacticalCommandTemplet m_NKMTacticalCommandTemplet;

	private int m_TCLevel = 1;

	private Color m_TimeWarningColor;

	private Color m_TimeNormalColor;

	private Color m_SkillLvOriginalColor;

	public void Awake()
	{
		if (m_arNKCGameHudComboSlot != null)
		{
			m_SlotMaxCount = m_arNKCGameHudComboSlot.Length;
		}
		if (m_cbtnSkill != null)
		{
			m_cbtnSkill.PointerDown.RemoveAllListeners();
			m_cbtnSkill.PointerDown.AddListener(OnPointerDownSkill);
		}
		m_TimeWarningColor = NKCUtil.GetColor("#c2190d");
		m_TimeNormalColor = NKCUtil.GetColor("#e8c043");
		m_SkillLvOriginalColor = m_lbSkillLevel.color;
	}

	public void OnPointerDownSkill(PointerEventData e)
	{
		if (m_NKMTacticalCommandTemplet != null)
		{
			NKCUITooltip.Instance.Open(m_NKMTacticalCommandTemplet, m_TCLevel, e.position);
		}
	}

	private void OnDestroy()
	{
		NKCUtil.SetImageSprite(m_imgSkill, null);
		NKCUtil.SetImageSprite(m_imgSkillGray, null);
	}

	public void UpdatePerFrame(NKMTacticalCommandData cNKMTacticalCommandData)
	{
		if (m_NKMTacticalCommandTemplet == null || cNKMTacticalCommandData == null || m_NKMTacticalCommandTemplet.m_TCID != cNKMTacticalCommandData.m_TCID || m_arNKCGameHudComboSlot == null)
		{
			return;
		}
		int count = m_NKMTacticalCommandTemplet.m_listComboType.Count;
		if (count <= 0)
		{
			return;
		}
		int num = m_SlotMaxCount - count;
		int num2 = 0;
		for (int i = num; i < m_arNKCGameHudComboSlot.Length; i++)
		{
			NKCGameHudComboSlot nKCGameHudComboSlot = m_arNKCGameHudComboSlot[i];
			if (!(nKCGameHudComboSlot == null))
			{
				_ = m_NKMTacticalCommandTemplet.m_listComboType[num2];
				nKCGameHudComboSlot.SetComboSucess(num2 < cNKMTacticalCommandData.m_ComboCount);
				num2++;
			}
		}
		m_sldCoolTime.value = cNKMTacticalCommandData.m_fComboResetCoolTimeNow / m_NKMTacticalCommandTemplet.m_fComboResetCoolTime;
		if (m_imgSldFill != null && m_sldCoolTime != null)
		{
			if ((double)m_sldCoolTime.value < 0.33)
			{
				m_imgSldFill.color = m_TimeWarningColor;
			}
			else
			{
				m_imgSldFill.color = m_TimeNormalColor;
			}
		}
		NKCUtil.SetGameobjectActive(m_objSkillCoolTime, !cNKMTacticalCommandData.m_bCoolTimeOn);
		if (m_cgComboSlots != null)
		{
			float alpha = m_cgComboSlots.alpha;
			if (cNKMTacticalCommandData.m_bCoolTimeOn)
			{
				m_cgComboSlots.alpha = 1f;
			}
			else
			{
				m_cgComboSlots.alpha = 0.4f;
			}
			if (alpha != 1f && alpha != m_cgComboSlots.alpha && m_amtCoolTimeOn != null)
			{
				m_amtCoolTimeOn.Play("NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_READY", -1, 0f);
			}
		}
		SetSkillImg(cNKMTacticalCommandData);
		if (m_imgSkillCoolTime != null && m_NKMTacticalCommandTemplet.m_fCoolTime > 0f)
		{
			m_imgSkillCoolTime.fillAmount = (m_NKMTacticalCommandTemplet.m_fCoolTime - cNKMTacticalCommandData.m_fCoolTimeNow) / m_NKMTacticalCommandTemplet.m_fCoolTime;
		}
		SetActiveTimeUI(cNKMTacticalCommandData);
		SetResetCoolTimeVisible(cNKMTacticalCommandData);
	}

	private void SetSkillImg(NKMTacticalCommandData cNKMTacticalCommandData)
	{
		if (cNKMTacticalCommandData != null)
		{
			bool flag = cNKMTacticalCommandData.m_bCoolTimeOn || cNKMTacticalCommandData.m_fActiveTime > 0f;
			NKCUtil.SetGameobjectActive(m_imgSkill, flag);
			NKCUtil.SetGameobjectActive(m_imgSkillGray, !flag);
		}
	}

	private void SetResetCoolTimeVisible(NKMTacticalCommandData cNKMTacticalCommandData)
	{
		if (cNKMTacticalCommandData != null)
		{
			NKCUtil.SetGameobjectActive(m_objCoolTime, cNKMTacticalCommandData.m_fCoolTimeNow <= 0f && cNKMTacticalCommandData.m_ComboCount > 0);
		}
	}

	private void SetActiveTimeUI(NKMTacticalCommandData cNKMTacticalCommandData)
	{
		if (cNKMTacticalCommandData != null)
		{
			float fActiveTime = cNKMTacticalCommandData.m_fActiveTime;
			if (fActiveTime > 0f)
			{
				NKCUtil.SetGameobjectActive(m_objActive, bValue: true);
				NKCUtil.SetLabelText(m_lbActiveTime, fActiveTime.ToString("0.##"));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objActive, bValue: false);
			}
		}
	}

	public void SetUI(NKMGameData cNKMGameData, NKM_TEAM_TYPE myTeamType)
	{
		if (cNKMGameData == null)
		{
			return;
		}
		int num = 0;
		List<NKMTacticalCombo> list = null;
		string text = "";
		NKMTacticalCommandData cNKMTacticalCommandData = null;
		NKMGameTeamData teamData = cNKMGameData.GetTeamData(myTeamType);
		if (teamData != null)
		{
			int num2 = 0;
			Color col = m_SkillLvOriginalColor;
			if (teamData.m_Operator != null && cNKMGameData.m_dicNKMBanOperatorData != null && cNKMGameData.m_dicNKMBanOperatorData.Count > 0)
			{
				foreach (KeyValuePair<int, NKMBanOperatorData> dicNKMBanOperatorDatum in cNKMGameData.m_dicNKMBanOperatorData)
				{
					if (dicNKMBanOperatorDatum.Value.m_OperatorID == teamData.m_Operator.id && dicNKMBanOperatorDatum.Value.m_BanLevel > 0)
					{
						col = NKCOperatorUtil.BAN_COLOR_RED;
						num2 = dicNKMBanOperatorDatum.Value.m_BanLevel;
					}
				}
			}
			for (int i = 0; i < teamData.m_listTacticalCommandData.Count; i++)
			{
				NKMTacticalCommandData nKMTacticalCommandData = teamData.m_listTacticalCommandData[i];
				if (nKMTacticalCommandData == null)
				{
					continue;
				}
				NKMTacticalCommandTemplet tacticalCommandTempletByID = NKMTacticalCommandManager.GetTacticalCommandTempletByID(nKMTacticalCommandData.m_TCID);
				if (tacticalCommandTempletByID != null && tacticalCommandTempletByID.m_NKM_TACTICAL_COMMAND_TYPE == NKM_TACTICAL_COMMAND_TYPE.NTCT_COMBO)
				{
					text = tacticalCommandTempletByID.m_TCIconName;
					num = tacticalCommandTempletByID.m_listComboType.Count;
					list = tacticalCommandTempletByID.m_listComboType;
					m_NKMTacticalCommandTemplet = tacticalCommandTempletByID;
					if (num2 > 0)
					{
						m_NKMTacticalCommandTemplet.m_fCoolTime *= 1f + 0.2f * (float)num2;
						Debug.Log($"<color=red>SetUI.Operator Skill Cool Time - {m_NKMTacticalCommandTemplet.m_TCName} modified -> {m_NKMTacticalCommandTemplet.m_fCoolTime}</color>");
					}
					cNKMTacticalCommandData = nKMTacticalCommandData;
					string arg = "???";
					m_TCLevel = 1;
					if (teamData.m_Operator != null && teamData.m_Operator.mainSkill != null && NKMTempletContainer<NKMOperatorSkillTemplet>.Find(teamData.m_Operator.mainSkill.id).m_OperSkillTarget == tacticalCommandTempletByID.m_TCStrID)
					{
						arg = teamData.m_Operator.mainSkill.level.ToString();
						m_TCLevel = teamData.m_Operator.mainSkill.level;
					}
					NKCUtil.SetLabelText(m_lbSkillLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, arg));
					NKCUtil.SetLabelTextColor(m_lbSkillLevel, col);
					break;
				}
			}
		}
		if (num <= 0 || num > m_SlotMaxCount)
		{
			return;
		}
		if (m_arNKCGameHudComboSlot != null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			int num3 = m_SlotMaxCount - num;
			int num4 = 0;
			for (int j = 0; j < m_arNKCGameHudComboSlot.Length; j++)
			{
				NKCGameHudComboSlot nKCGameHudComboSlot = m_arNKCGameHudComboSlot[j];
				if (!(nKCGameHudComboSlot == null))
				{
					NKCUtil.SetGameobjectActive(nKCGameHudComboSlot, j >= num3);
					if (j >= num3)
					{
						NKMTacticalCombo cNKMTacticalCombo = list[num4];
						nKCGameHudComboSlot.SetUI(cNKMTacticalCombo);
						num4++;
					}
				}
			}
		}
		if (!string.IsNullOrWhiteSpace(text))
		{
			Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_TACTICAL_COMMAND_ICON", text);
			NKCUtil.SetImageSprite(m_imgSkill, orLoadAssetResource);
			NKCUtil.SetImageSprite(m_imgSkillGray, orLoadAssetResource);
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgSkill, null);
			NKCUtil.SetImageSprite(m_imgSkillGray, null);
		}
		UpdatePerFrame(cNKMTacticalCommandData);
	}
}
