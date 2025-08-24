using System;
using ClientPacket.Guild;
using NKM;
using NKM.Guild;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildLobbyWelfareSlot : MonoBehaviour
{
	public Image m_imgIcon;

	public GameObject m_objLevel;

	public Text m_lbLevel;

	public Text m_lbName;

	public Text m_lbDesc;

	public NKCUIComStateButton m_btnBuy;

	public Image m_imgCostIcon;

	public Text m_lbCost;

	public GameObject m_objApply;

	public Text m_lbRemainTime;

	public GameObject m_objLock;

	public Text m_lbLock;

	private float m_fDeltaTime;

	private GuildWelfareTemplet m_GuildWelfareTemplet;

	public void InitUI()
	{
		m_btnBuy.PointerClick.RemoveAllListeners();
		m_btnBuy.PointerClick.AddListener(OnClickBuy);
	}

	public void SetData(GuildWelfareTemplet templet)
	{
		if (templet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_fDeltaTime = 0f;
		m_GuildWelfareTemplet = templet;
		NKMCompanyBuffTemplet nKMCompanyBuffTemplet = NKMCompanyBuffTemplet.Find(templet.CompanyBuffID);
		NKCUtil.SetImageSprite(m_imgIcon, NKCUtil.GetCompanyBuffIconSprite(nKMCompanyBuffTemplet.m_CompanyBuffIcon));
		if (templet.WelfareLvDisplay > 0)
		{
			NKCUtil.SetGameobjectActive(m_objLevel, bValue: true);
			NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, templet.WelfareLvDisplay));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objLevel, bValue: false);
		}
		NKCUtil.SetLabelText(m_lbName, NKCStringTable.GetString(templet.WelfareTextTitle));
		NKCUtil.SetLabelText(m_lbDesc, NKCStringTable.GetString(templet.WelfareTextDesc));
		SetBtnState();
		NKCUtil.SetGameobjectActive(m_objLock, !NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in templet.m_UnlockInfo));
		if (m_objLock.activeSelf)
		{
			NKCUtil.SetLabelText(m_lbLock, NKCContentManager.MakeUnlockConditionString(in templet.m_UnlockInfo, bSimple: true));
		}
	}

	private void SetBtnState()
	{
		if (NKCScenManager.CurrentUserData().HasBuffGroup(m_GuildWelfareTemplet.CompanyBuffGroupID))
		{
			if (NKCScenManager.CurrentUserData().HasBuff(m_GuildWelfareTemplet.CompanyBuffID))
			{
				NKCUtil.SetGameobjectActive(m_btnBuy, bValue: false);
				NKCUtil.SetGameobjectActive(m_objApply, bValue: true);
				SetRemainTime(NKCScenManager.CurrentUserData().GetBuffExpireTimeByGroupId(m_GuildWelfareTemplet.CompanyBuffGroupID));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_btnBuy, bValue: false);
				NKCUtil.SetGameobjectActive(m_objApply, bValue: false);
			}
			return;
		}
		NKCUtil.SetGameobjectActive(m_btnBuy, bValue: true);
		NKCUtil.SetGameobjectActive(m_objApply, bValue: false);
		if (m_GuildWelfareTemplet.WelfareCategory == WELFARE_BUFF_TYPE.GUILD)
		{
			if (NKCGuildManager.MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID).grade == GuildMemberGrade.Member)
			{
				m_btnBuy.Lock();
			}
			else
			{
				m_btnBuy.UnLock();
			}
		}
		else
		{
			m_btnBuy.UnLock();
		}
		NKCUtil.SetGameobjectActive(m_objApply, bValue: false);
		NKCUtil.SetImageSprite(m_imgCostIcon, NKCResourceUtility.GetOrLoadMiscItemSmallIcon(m_GuildWelfareTemplet.WelfareRequireItemID));
		NKCUtil.SetLabelText(m_lbCost, m_GuildWelfareTemplet.WelfareRequireItemValue.ToString("N0"));
		if (m_GuildWelfareTemplet.WelfareCategory == WELFARE_BUFF_TYPE.PERSONAL)
		{
			if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_GuildWelfareTemplet.WelfareRequireItemID) < m_GuildWelfareTemplet.WelfareRequireItemValue)
			{
				NKCUtil.SetLabelTextColor(m_lbCost, Color.red);
			}
			else
			{
				NKCUtil.SetLabelTextColor(m_lbCost, NKCUtil.GetColor("#582817"));
			}
		}
		else if (m_GuildWelfareTemplet.WelfareCategory == WELFARE_BUFF_TYPE.GUILD)
		{
			if (NKCGuildManager.MyGuildData.unionPoint < m_GuildWelfareTemplet.WelfareRequireItemValue)
			{
				NKCUtil.SetLabelTextColor(m_lbCost, Color.red);
			}
			else
			{
				NKCUtil.SetLabelTextColor(m_lbCost, NKCUtil.GetColor("#582817"));
			}
		}
	}

	private void SetRemainTime(DateTime expireTime)
	{
		NKCUtil.SetLabelText(m_lbRemainTime, NKCUtilString.GetTimeSpanString(expireTime - NKCSynchronizedTime.GetServerUTCTime()));
	}

	private void OnClickBuy()
	{
		if (m_GuildWelfareTemplet.WelfareCategory == WELFARE_BUFF_TYPE.PERSONAL)
		{
			if (NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_GuildWelfareTemplet.WelfareRequireItemID) >= m_GuildWelfareTemplet.WelfareRequireItemValue)
			{
				NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_CONSORTIUM_WELFARE_PERSONAL_CONFIRM_TITLE, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_WELFARE_SUBTAB_PERSONAL_CONFIRM_BODY, NKCStringTable.GetString(m_GuildWelfareTemplet.WelfareTextTitle)), m_GuildWelfareTemplet.WelfareRequireItemID, m_GuildWelfareTemplet.WelfareRequireItemValue, OnConfirm);
			}
			else
			{
				NKCPopupItemLack.Instance.OpenItemMiscLackPopup(m_GuildWelfareTemplet.WelfareRequireItemID, m_GuildWelfareTemplet.WelfareRequireItemValue);
			}
		}
		else if (m_GuildWelfareTemplet.WelfareCategory == WELFARE_BUFF_TYPE.GUILD)
		{
			if (NKCGuildManager.MyGuildData.unionPoint >= m_GuildWelfareTemplet.WelfareRequireItemValue)
			{
				NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_CONSORTIUM_WELFARE_GUILD_CONFIRM_TITLE, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_WELFARE_SUBTAB_GUILD_CONFIRM_BODY, NKCStringTable.GetString(m_GuildWelfareTemplet.WelfareTextTitle)), m_GuildWelfareTemplet.WelfareRequireItemID, m_GuildWelfareTemplet.WelfareRequireItemValue, NKCGuildManager.MyGuildData.unionPoint, OnConfirm);
			}
			else
			{
				NKCPopupItemLack.Instance.OpenItemMiscLackPopup(m_GuildWelfareTemplet.WelfareRequireItemID, m_GuildWelfareTemplet.WelfareRequireItemValue, NKCGuildManager.MyGuildData.unionPoint);
			}
		}
	}

	private void OnConfirm()
	{
		NKCPacketSender.Send_NKMPacket_GUILD_BUY_BUFF_REQ(m_GuildWelfareTemplet.ID);
	}

	private void Update()
	{
		if (!m_objApply.activeSelf)
		{
			return;
		}
		m_fDeltaTime += Time.deltaTime;
		if (m_fDeltaTime > 1f)
		{
			m_fDeltaTime -= 1f;
			if (NKCScenManager.CurrentUserData().HasBuffGroup(m_GuildWelfareTemplet.CompanyBuffGroupID))
			{
				SetRemainTime(NKCScenManager.CurrentUserData().GetBuffExpireTimeByGroupId(m_GuildWelfareTemplet.CompanyBuffGroupID));
			}
			else
			{
				SetBtnState();
			}
		}
	}
}
