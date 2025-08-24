using System;
using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Core.Util;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLeagueMatch : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_LEAGUE_MATCH";

	[SerializeField]
	[Header("유저 정보")]
	public NKCUIGauntletLeagueMain.LeagueUserInfoUI m_UserLeft;

	public NKCUIGauntletLeagueMain.LeagueUserInfoUI m_UserRight;

	[Header("애니메이터")]
	public Animator m_animatorStart;

	[Header("유닛 일러스트")]
	public NKCUICharacterView m_CharacterViewLeft;

	public NKCUICharacterView m_CharacterViewRight;

	private DateTime m_endTime;

	private int m_prevRemainingSeconds;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override bool IgnoreBackButtonWhenOpen => true;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override string MenuName => NKCUtilString.GET_STRING_GAUNTLET;

	public override List<int> UpsideMenuShowResourceList => new List<int> { 101 };

	public static NKCUIManager.LoadedUIData OpenNewInstanceAsync()
	{
		return NKCUIManager.OpenNewInstanceAsync<NKCUIGauntletLeagueMatch>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_LEAGUE_MATCH", NKCUIManager.eUIBaseRect.UIFrontCommon, null);
	}

	public override void CloseInternal()
	{
		if (m_CharacterViewLeft != null)
		{
			m_CharacterViewLeft.CleanUp();
			m_CharacterViewLeft = null;
		}
		if (m_CharacterViewRight != null)
		{
			m_CharacterViewRight.CleanUp();
			m_CharacterViewRight = null;
		}
	}

	public void Init()
	{
		m_prevRemainingSeconds = 0;
		NKCUtil.SetGameobjectActive(this, bValue: false);
		if (m_CharacterViewLeft != null)
		{
			m_CharacterViewLeft.Init(null, delegate
			{
			});
		}
		if (m_CharacterViewRight != null)
		{
			m_CharacterViewRight.Init(null, delegate
			{
			});
		}
		m_UserLeft.Init(null, null);
		m_UserRight.Init(null, null);
	}

	public void Open(DateTime endTime)
	{
		NKCUtil.SetGameobjectActive(this, bValue: true);
		RefreshDraftData(endTime);
		UIOpened();
		if (m_animatorStart != null)
		{
			m_animatorStart.Play("GAUNTLET_LEAGUE_MATCH");
		}
		NKCSoundManager.PlaySound("FX_UI_PVP_MATCH_OK", 1f, 0f, 0f);
	}

	private void Update()
	{
		if (m_prevRemainingSeconds > 0)
		{
			int num = Math.Max(0, Convert.ToInt32((m_endTime - ServiceTime.Now).TotalSeconds));
			if (num != m_prevRemainingSeconds)
			{
				m_prevRemainingSeconds = num;
			}
		}
	}

	public void RefreshDraftData(DateTime endTime)
	{
		SetEndTime(endTime);
		UpdateUserInfo();
	}

	public void SetEndTime(DateTime endTime)
	{
		m_endTime = endTime;
		m_prevRemainingSeconds = Math.Max(0, Convert.ToInt32((m_endTime - ServiceTime.Now).TotalSeconds));
	}

	public void UpdateUserInfo()
	{
		m_UserLeft.SetData(NKCLeaguePVPMgr.GetLeftDraftTeamData(), isLeftTeam: true, includePickBan: false, forceEnableButton: false, -1, -1, NKCLeaguePVPMgr.IsPrivate());
		m_UserRight.SetData(NKCLeaguePVPMgr.GetRightDraftTeamData(), isLeftTeam: false, includePickBan: false, forceEnableButton: false, -1, -1, NKCLeaguePVPMgr.IsPrivate());
		UpdateCharacterView(m_CharacterViewLeft, NKCLeaguePVPMgr.GetTeamLeaderUnit(isLeft: true));
		UpdateCharacterView(m_CharacterViewRight, NKCLeaguePVPMgr.GetTeamLeaderUnit(isLeft: false));
	}

	private void UpdateCharacterView(NKCUICharacterView characterView, NKMAsyncUnitData leaderUnitData)
	{
		if (leaderUnitData != null && !(characterView == null))
		{
			if (leaderUnitData.skinId != 0)
			{
				NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(leaderUnitData.skinId);
				characterView.SetCharacterIllust(skinTemplet, bAsync: false, bEnableBackground: false);
			}
			else if (leaderUnitData.unitId != 0)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(leaderUnitData.unitId);
				characterView.SetCharacterIllust(unitTempletBase, 0, bAsync: false, bEnableBackground: false);
			}
		}
	}
}
