using System;
using ClientPacket.Common;
using ClientPacket.Office;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component.Office;

public class NKCUIComOfficeBizCard : MonoBehaviour
{
	public delegate void OnClick(long uid);

	public const string DEFAULT_BUNDLE_NAME = "AB_INVEN_ICON_BIZ_CARD";

	public const string DEFAULT_ASSET_NAME = "BIZ_CARD_000";

	public NKCUIComUserSimpleInfo m_userInfo;

	public Text m_lbTimeleft;

	public NKCUIComStateButton m_csbtnCard;

	private DateTime m_dtEndTime;

	private OnClick dOnClick;

	private long m_userUID;

	public static NKCUIComOfficeBizCard GetInstance(int ID, Transform parent)
	{
		NKMAssetName nKMAssetName = new NKMAssetName("AB_INVEN_ICON_BIZ_CARD", "BIZ_CARD_000");
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(nKMAssetName, bAsync: false, parent);
		if (nKCAssetInstanceData?.m_Instant == null)
		{
			Debug.LogError($"NKCUIComOfficeBizCard : {nKMAssetName} not found!");
			return null;
		}
		NKCUIComOfficeBizCard component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIComOfficeBizCard>();
		if (component != null)
		{
			component.Init();
		}
		return component;
	}

	private void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnCard, OnTouch);
	}

	public void SetData(NKMOfficePost post, OnClick onClick)
	{
		if (post != null)
		{
			SetData(post.senderProfile, post.senderGuildData, post.expirationDate, onClick);
		}
	}

	public void SetData(NKMCommonProfile profile, NKMGuildSimpleData guildData, OnClick onClick)
	{
		if (m_userInfo != null)
		{
			m_userInfo.SetData(profile, guildData);
		}
		m_dtEndTime = DateTime.MinValue;
		NKCUtil.SetGameobjectActive(m_lbTimeleft, bValue: false);
		m_userUID = profile?.userUid ?? 0;
		dOnClick = onClick;
	}

	public void SetData(NKMUserProfileData userProfileData, OnClick onClick)
	{
		if (m_userInfo != null)
		{
			m_userInfo.SetData(userProfileData);
		}
		m_dtEndTime = DateTime.MinValue;
		NKCUtil.SetGameobjectActive(m_lbTimeleft, bValue: false);
		m_userUID = userProfileData?.commonProfile.userUid ?? 0;
		dOnClick = onClick;
	}

	public void SetData(NKMCommonProfile profile, NKMGuildSimpleData guildData, DateTime endTime, OnClick onClick)
	{
		if (m_userInfo != null)
		{
			m_userInfo.SetData(profile, guildData);
		}
		NKCUtil.SetGameobjectActive(m_lbTimeleft, bValue: true);
		m_dtEndTime = endTime;
		SetTime();
		m_userUID = profile?.userUid ?? 0;
		dOnClick = onClick;
	}

	public void SetData(NKMUserProfileData userProfileData, DateTime endTime, OnClick onClick)
	{
		if (m_userInfo != null)
		{
			m_userInfo.SetData(userProfileData);
		}
		NKCUtil.SetGameobjectActive(m_lbTimeleft, bValue: true);
		m_dtEndTime = endTime;
		SetTime();
		m_userUID = userProfileData?.commonProfile.userUid ?? 0;
		dOnClick = onClick;
	}

	private void SetTime()
	{
		if (m_dtEndTime > DateTime.MinValue)
		{
			NKCUtil.SetLabelText(m_lbTimeleft, NKCSynchronizedTime.GetTimeLeftString(m_dtEndTime));
		}
	}

	private void Update()
	{
		SetTime();
	}

	private void OnTouch()
	{
		dOnClick?.Invoke(m_userUID);
	}
}
