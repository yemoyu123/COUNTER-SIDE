using System;
using Cs.Core.Util;
using NKC.UI.Shop;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildIntro : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM";

	private const string UI_ASSET_NAME = "NKM_UI_CONSORTIUM_INTRO";

	public NKCUIComStateButton m_btnCreate;

	public NKCUIComStateButton m_btnJoin;

	public GameObject m_objJoinLock;

	public Text m_lbJoinLockTime;

	public NKCUIComStateButton m_btnShop;

	public NKCUIComStateButton m_btnSeasonReward;

	public GameObject m_objSeasonRewardRedDot;

	private DateTime m_tNextJoinTime = DateTime.MinValue;

	private float m_fDeltaTime;

	private NKCUIShopSingle m_ConsortiumShop;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public override string MenuName => NKCUtilString.GET_STRING_CONSORTIUM_INTRO;

	private NKCUIShopSingle ConsortiumShop
	{
		get
		{
			if (m_ConsortiumShop == null)
			{
				m_ConsortiumShop = NKCUIShopSingle.GetInstance("ab_ui_nkm_ui_consortium_shop", "NKM_UI_CONSORTIUM_SHOP");
			}
			return m_ConsortiumShop;
		}
	}

	public static NKCAssetResourceData OpenInstanceAsync()
	{
		return NKCUIBase.OpenInstanceAsync<NKCUIGuildIntro>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_INTRO");
	}

	public static bool CheckInstanceLoaded(NKCAssetResourceData loadResourceData, out NKCUIGuildIntro retVal)
	{
		return NKCUIBase.CheckInstanceLoaded<NKCUIGuildIntro>(loadResourceData, NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), out retVal);
	}

	public void CloseInstance()
	{
		int num = NKCAssetResourceManager.CloseResource("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_INTRO");
		Debug.Log($"NKCUIConsortiumIntro close resource retval is {num}");
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnBackButton()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
	}

	public void InitUI()
	{
		m_btnCreate.PointerClick.RemoveAllListeners();
		m_btnCreate.PointerClick.AddListener(OnClickCreate);
		m_btnJoin.PointerClick.RemoveAllListeners();
		m_btnJoin.PointerClick.AddListener(OnClickJoin);
		m_btnShop.PointerClick.RemoveAllListeners();
		m_btnShop.PointerClick.AddListener(OnClickShop);
		m_btnSeasonReward.PointerClick.RemoveAllListeners();
		m_btnSeasonReward.PointerClick.AddListener(OnClickSeasonReward);
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(m_objJoinLock, ServiceTime.Recent < NKCGuildManager.MyData.guildJoinDisableTime);
		if (m_objJoinLock.activeSelf)
		{
			m_tNextJoinTime = NKCGuildManager.MyData.guildJoinDisableTime;
			SetRemainTime(m_tNextJoinTime);
			m_btnJoin.Lock();
		}
		else
		{
			m_tNextJoinTime = DateTime.MinValue;
			m_btnJoin.UnLock();
		}
		RefreshSeasonRewardRedDot();
		bool flag = NKCGuildCoopManager.CheckFirstSeasonStarted();
		NKCUtil.SetGameobjectActive(m_btnSeasonReward, NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_DUNGEON) && flag);
		NKCUtil.SetGameobjectActive(m_btnShop, NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_SHOP));
		UIOpened();
	}

	private void SetRemainTime(DateTime endTime)
	{
		if (ServiceTime.Recent < endTime)
		{
			NKCUtil.SetLabelText(m_lbJoinLockTime, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_JOIN_COOLTIME_ONE_PARAM, NKCUtilString.GetRemainTimeString(endTime - ServiceTime.Recent, 2)));
			return;
		}
		NKCUtil.SetGameobjectActive(m_objJoinLock, bValue: false);
		m_btnJoin.UnLock();
	}

	private void Update()
	{
		if (m_objJoinLock.activeSelf && m_tNextJoinTime > DateTime.MinValue)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > 1f)
			{
				m_fDeltaTime -= 1f;
				SetRemainTime(m_tNextJoinTime);
			}
		}
	}

	private void OnClickCreate()
	{
		NKCUIGuildCreate.Instance.Open();
	}

	private void OnClickJoin()
	{
		NKCUIGuildJoin.Instance.Open();
	}

	private void OnClickSeasonReward()
	{
		NKCPopupGuildCoopSeasonReward.Instance.Open(RefreshSeasonRewardRedDot);
	}

	private void RefreshSeasonRewardRedDot()
	{
		NKCUtil.SetGameobjectActive(m_objSeasonRewardRedDot, NKCGuildCoopManager.CheckSeasonRewardEnable());
	}

	private void OnClickShop()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.GUILD_SHOP))
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString("SI_PF_CONSORTIUM_LOBBY_NONE_SYSTEM"), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else
		{
			NKCUIShop.ShopShortcut("TAB_EXCHANGE_GUILD_COIN");
		}
	}
}
