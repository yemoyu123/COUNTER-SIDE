using ClientPacket.Common;
using ClientPacket.Raid;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Component;

public class NKCUIComTitlePanel : MonoBehaviour
{
	public NKCUISlotTitle slotTitle;

	public void SetData(int titleId)
	{
		if (!NKMTitleTemplet.TitleOpenTagEnabled || titleId <= 0)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		NKMTitleTemplet nKMTitleTemplet = NKMTitleTemplet.Find(titleId);
		if (nKMTitleTemplet == null || !nKMTitleTemplet.EnableByTag)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		slotTitle?.SetData(titleId, showEmpty: false, showLock: false);
	}

	public void SetData(NKMUserProfileData userProfileData)
	{
		if (userProfileData == null)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			SetData(userProfileData.commonProfile);
		}
	}

	public void SetData(NKMCommonProfile commonProfile)
	{
		if (commonProfile == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && commonProfile.userUid == nKMUserData.m_UserUID && nKMUserData.UserProfileData != null)
		{
			commonProfile.titleId = nKMUserData.UserProfileData.commonProfile.titleId;
		}
		SetData(commonProfile.titleId);
	}

	public void SetData(NKMUserSimpleProfileData profileData)
	{
		if (profileData == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && profileData.userUid == nKMUserData.m_UserUID && nKMUserData.UserProfileData != null)
		{
			profileData.titleId = nKMUserData.UserProfileData.commonProfile.titleId;
		}
		SetData(profileData.titleId);
	}

	public void SetData(NKMRaidJoinData raidJoinData)
	{
		if (raidJoinData == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null && raidJoinData.userUID == nKMUserData.m_UserUID && nKMUserData.UserProfileData != null)
		{
			raidJoinData.titleId = nKMUserData.UserProfileData.commonProfile.titleId;
		}
		SetData(raidJoinData.titleId);
	}
}
