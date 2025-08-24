using ClientPacket.Common;
using NKC.UI;
using NKC.UI.Component;
using NKC.UI.Guild;
using NKM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUILobbyUserInfo : MonoBehaviour
{
	public NKCUISlotProfile m_UserProfileSlot;

	public GameObject m_objProfileReddot;

	public TMP_Text m_lbLevel;

	public Image m_imgExp;

	public TMP_Text m_lbUserName;

	public NKCUIComStateButton m_btnGuild;

	public NKCUIGuildBadge m_BadgeUI;

	public TMP_Text m_lbGuildName;

	public GameObject m_objGuildNotify;

	public NKCUIComItemCount m_UICountCredit;

	public NKCUIComItemCount m_UICountEternium;

	public NKCUIComItemCount m_UICountCash;

	public NKCUIComStateButton m_csbtnUserBuff;

	public NKCUIComStateButton m_csbtnUserBuffNone;

	public Text m_lbUserBuffCount;

	public NKCUIRechargeEternium m_RechargeEternium;

	public NKCUIComTitlePanel m_TitlePanel;

	public float ExpBarValue
	{
		get
		{
			return m_imgExp.fillAmount;
		}
		set
		{
			m_imgExp.fillAmount = value;
		}
	}

	public void Init()
	{
		m_UserProfileSlot.Init();
		SetMoveToShop(m_UICountCredit);
		SetMoveToShop(m_UICountEternium);
		SetMoveToShop(m_UICountCash);
		if (m_btnGuild != null)
		{
			m_btnGuild.PointerClick.RemoveAllListeners();
			m_btnGuild.PointerClick.AddListener(OnClickGuild);
		}
		m_csbtnUserBuff.PointerClick.RemoveAllListeners();
		m_csbtnUserBuff.PointerClick.AddListener(OpenUserBuff);
		m_csbtnUserBuffNone.PointerClick.RemoveAllListeners();
		m_csbtnUserBuffNone.PointerClick.AddListener(OpenUserBuff);
	}

	private void SetMoveToShop(NKCUIComItemCount targetButton)
	{
		if (!(targetButton == null))
		{
			targetButton.SetOnClickPlusBtn(targetButton.OpenMoveToShopPopup);
		}
	}

	public void SetData(NKMUserData userData)
	{
		UpdateUserProfileIcon(userData);
		UpdateLevelAndExp(userData);
		UpdateUserBuffCount(userData);
		RefreshNickname();
		SetGuildData();
		NKCUtil.SetGameobjectActive(m_objGuildNotify, NKCAlarmManager.CheckGuildNotify(userData));
		m_UICountCredit.SetData(userData, 1);
		m_UICountEternium.SetData(userData, 2);
		m_UICountCash.SetData(userData, 101);
		m_RechargeEternium.UpdateData(userData);
		UpdateUserTitle(userData);
	}

	public void SetGuildData()
	{
		if (m_btnGuild != null)
		{
			NKCUtil.SetGameobjectActive(m_btnGuild, NKCGuildManager.HasGuild());
			if (m_btnGuild.gameObject.activeSelf)
			{
				m_BadgeUI.SetData(NKCGuildManager.MyGuildData.badgeId, bOpponent: false);
				NKCUtil.SetLabelText(m_lbGuildName, NKCUtilString.GetUserGuildName(NKCGuildManager.MyGuildData.name, bOpponent: false));
			}
		}
	}

	public void UpdateUserProfileIcon(NKMUserData userData)
	{
		NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
		if (userProfileData != null)
		{
			m_UserProfileSlot.SetProfiledata(userProfileData, OpenUserInfo, NKCTacticUpdateUtil.IsMaxTacticLevel(userProfileData.commonProfile.mainUnitTacticLevel));
		}
		else
		{
			m_UserProfileSlot.SetProfiledata(1001, 0, 0, OpenUserInfo);
		}
		NKCUtil.SetGameobjectActive(m_objProfileReddot, NeedSetupBirthday());
	}

	private bool NeedSetupBirthday()
	{
		if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.BIRTHDAY))
		{
			return false;
		}
		if (NKCScenManager.CurrentUserData().m_BirthDayData != null)
		{
			return false;
		}
		return true;
	}

	public void UpdateLevelAndExp(NKMUserData userData)
	{
		NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, userData.UserLevel));
		ExpBarValue = NKCExpManager.GetNextLevelExpProgress(userData);
		m_RechargeEternium.UpdateData(userData);
	}

	public void UpdateUserBuffCount(NKMUserData userData)
	{
		if (userData != null)
		{
			NKCCompanyBuff.RemoveExpiredBuffs(userData.m_companyBuffDataList);
			NKCUtil.SetGameobjectActive(m_csbtnUserBuff, userData.m_companyBuffDataList.Count > 0);
			NKCUtil.SetGameobjectActive(m_csbtnUserBuffNone, userData.m_companyBuffDataList.Count == 0);
			m_lbUserBuffCount.text = userData.m_companyBuffDataList.Count.ToString();
		}
	}

	public void OnResourceValueChange(NKMItemMiscData itemData)
	{
		if (itemData != null)
		{
			switch (itemData.ItemID)
			{
			case 1:
				m_UICountCredit.UpdateData(itemData);
				break;
			case 2:
				m_UICountEternium.UpdateData(itemData);
				m_RechargeEternium.UpdateData(NKCScenManager.CurrentUserData());
				break;
			case 101:
				m_UICountCash.UpdateData(itemData);
				break;
			}
		}
	}

	private void OnClickGuild()
	{
		if (NKCGuildManager.HasGuild())
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_LOBBY);
		}
	}

	private void OpenUserInfo(NKCUISlotProfile slot, int frameID)
	{
		if (NKCDefineManager.DEFINE_UNITY_EDITOR() && Input.GetKey(KeyCode.LeftControl))
		{
			NKCUIUserInfo.Instance.Open(NKCScenManager.GetScenManager().GetMyUserData());
		}
		else
		{
			NKCUIUserInfoV2.Instance.Open(NKCScenManager.GetScenManager().GetMyUserData());
		}
	}

	private void OpenUserBuff()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			NKCCompanyBuff.RemoveExpiredBuffs(nKMUserData.m_companyBuffDataList);
			if (nKMUserData.m_companyBuffDataList.Count > 0)
			{
				NKCPopupCompanyBuff.Instance.Open();
			}
			else
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_LOBBY_USER_BUFF_NONE);
			}
		}
	}

	public void RefreshNickname()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			NKCUtil.SetLabelText(m_lbUserName, NKCUtilString.GetUserNickname(myUserData.m_UserNickName, bOpponent: false));
		}
	}

	public void RefreshRechargeEternium()
	{
		m_RechargeEternium?.UpdateData(NKCScenManager.GetScenManager().GetMyUserData());
	}

	private void UpdateUserTitle(NKMUserData userData)
	{
		NKMCommonProfile data = userData?.UserProfileData?.commonProfile;
		m_TitlePanel?.SetData(data);
	}
}
