using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Guild;
using Cs.Logging;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildCoopSeasonReward : NKCUIBase
{
	public delegate void OnClose();

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM_COOP";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_CONSORTIUM_COOP_REWARD";

	private static NKCPopupGuildCoopSeasonReward m_Instance;

	public NKCPopupGuildCoopSeasonRewardSlot m_pfbSlot;

	public Text m_lbTitle;

	public Text m_lbRemainTime;

	public NKCUIComStateButton m_btnClose;

	public Image m_imgBgBlur;

	[Header("처치점수 / 전투참여 토글버튼")]
	public NKCUIComToggle m_tglDamagePoint;

	public NKCUIComToggle m_tglBattleCount;

	public GameObject m_objKillPointRedDot;

	public GameObject m_objDungeonTryRedDot;

	[Header("좌우 보상있을경우 이동버튼")]
	public NKCUIComStateButton m_btnMoveToLeft;

	public NKCUIComStateButton m_btnMoveToRight;

	[Header("내 점수")]
	public NKCUISlotProfile m_slotProfile;

	public Image m_imgPoint;

	public Text m_lbMyPointTitle;

	public Text m_lbMyPoint;

	[Header("포인트 보상")]
	public LoopScrollRect m_loop;

	public Transform m_trSlotParent;

	public NKCUIComStateButton m_btnReceiveAll;

	private OnClose m_dOnClose;

	private Stack<NKCPopupGuildCoopSeasonRewardSlot> m_stkSlot = new Stack<NKCPopupGuildCoopSeasonRewardSlot>();

	private List<GuildSeasonRewardTemplet> m_lstRewardByRank = new List<GuildSeasonRewardTemplet>();

	private List<GuildSeasonRewardTemplet> m_lstRewardByTry = new List<GuildSeasonRewardTemplet>();

	private GuildSeasonTemplet m_GuildSeasonTemplet;

	private GuildDungeonRewardCategory m_CurCategory;

	private float m_fDeltaTime;

	public static NKCPopupGuildCoopSeasonReward Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildCoopSeasonReward>("AB_UI_NKM_UI_CONSORTIUM_COOP", "NKM_UI_POPUP_CONSORTIUM_COOP_REWARD", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null).GetInstance<NKCPopupGuildCoopSeasonReward>();
				if (m_Instance != null)
				{
					m_Instance.InitUI();
				}
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	public void InitUI()
	{
		if (m_btnClose != null)
		{
			m_btnClose.PointerClick.RemoveAllListeners();
			m_btnClose.PointerClick.AddListener(base.Close);
		}
		if (m_tglDamagePoint != null)
		{
			m_tglDamagePoint.OnValueChanged.RemoveAllListeners();
			m_tglDamagePoint.OnValueChanged.AddListener(OnChangedKillPoint);
		}
		if (m_tglBattleCount != null)
		{
			m_tglBattleCount.OnValueChanged.RemoveAllListeners();
			m_tglBattleCount.OnValueChanged.AddListener(OnChangedDungeonTry);
		}
		if (m_btnMoveToLeft != null)
		{
			m_btnMoveToLeft.PointerClick.RemoveAllListeners();
			m_btnMoveToLeft.PointerClick.AddListener(MoveToProgress);
		}
		if (m_btnMoveToRight != null)
		{
			m_btnMoveToRight.PointerClick.RemoveAllListeners();
			m_btnMoveToRight.PointerClick.AddListener(MoveToProgress);
		}
		if (m_loop != null)
		{
			m_loop.dOnGetObject += GetObject;
			m_loop.dOnReturnObject += ReturnObject;
			m_loop.dOnProvideData += ProvideData;
			m_loop.PrepareCells();
			NKCUtil.SetScrollHotKey(m_loop);
		}
		if (m_slotProfile != null)
		{
			m_slotProfile.Init();
		}
		if (m_btnReceiveAll != null)
		{
			m_btnReceiveAll.PointerClick.RemoveAllListeners();
			m_btnReceiveAll.PointerClick.AddListener(OnClickReceiveAll);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_dOnClose?.Invoke();
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void UnHide()
	{
		base.UnHide();
		RefreshUI();
	}

	private RectTransform GetObject(int idx)
	{
		NKCPopupGuildCoopSeasonRewardSlot nKCPopupGuildCoopSeasonRewardSlot = null;
		if (m_stkSlot.Count > 0)
		{
			nKCPopupGuildCoopSeasonRewardSlot = m_stkSlot.Pop();
		}
		else
		{
			nKCPopupGuildCoopSeasonRewardSlot = Object.Instantiate(m_pfbSlot);
			nKCPopupGuildCoopSeasonRewardSlot.InitUI();
		}
		nKCPopupGuildCoopSeasonRewardSlot.transform.SetParent(m_trSlotParent);
		return nKCPopupGuildCoopSeasonRewardSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.transform);
		m_stkSlot.Push(tr.GetComponent<NKCPopupGuildCoopSeasonRewardSlot>());
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCPopupGuildCoopSeasonRewardSlot component = tr.GetComponent<NKCPopupGuildCoopSeasonRewardSlot>();
		if (component == null)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		tr.SetParent(m_trSlotParent);
		NKCUtil.SetGameobjectActive(tr, bValue: true);
		GuildDungeonSeasonRewardData guildDungeonSeasonRewardData = NKCGuildCoopManager.m_LastReceivedSeasonRewardData.Find((GuildDungeonSeasonRewardData x) => x.category == m_CurCategory);
		if (m_CurCategory == GuildDungeonRewardCategory.RANK)
		{
			float gaugeProgress = 0f;
			if (idx == 0)
			{
				gaugeProgress = (float)NKCGuildCoopManager.m_KillPoint / (float)m_lstRewardByRank[0].GetRewardCountValue();
				component.SetData(null, bCanReward: false, bIsRewarded: false, bIsFinalSlot: false, null);
				component.SetGaugeProgress(gaugeProgress);
				return;
			}
			GuildSeasonRewardTemplet guildSeasonRewardTemplet = m_lstRewardByRank[idx - 1];
			GuildSeasonRewardTemplet guildSeasonRewardTemplet2 = ((idx < m_lstRewardByRank.Count) ? m_lstRewardByRank[idx] : null);
			if (guildSeasonRewardTemplet2 != null && NKCGuildCoopManager.m_KillPoint > guildSeasonRewardTemplet.GetRewardCountValue())
			{
				long num = guildSeasonRewardTemplet2.GetRewardCountValue() - guildSeasonRewardTemplet.GetRewardCountValue();
				gaugeProgress = (float)(NKCGuildCoopManager.m_KillPoint - guildSeasonRewardTemplet.GetRewardCountValue()) / (float)num;
			}
			int rewardCountValue = guildSeasonRewardTemplet.GetRewardCountValue();
			component.SetData(m_lstRewardByRank[idx - 1], NKCGuildCoopManager.m_KillPoint >= rewardCountValue && guildDungeonSeasonRewardData.receivedValue < rewardCountValue, guildDungeonSeasonRewardData.receivedValue >= rewardCountValue, idx == m_lstRewardByRank.Count, OnClickSlot);
			component.SetGaugeProgress(gaugeProgress);
			return;
		}
		float num2 = 0f;
		if (idx == 0)
		{
			num2 = (float)NKCGuildCoopManager.m_TryCount / (float)m_lstRewardByTry[0].GetRewardCountValue();
			component.SetData(null, bCanReward: false, bIsRewarded: false, bIsFinalSlot: false, null);
			component.SetGaugeProgress((float)NKCGuildCoopManager.m_TryCount / (float)m_lstRewardByTry[0].GetRewardCountValue());
			if (num2 >= 1f && guildDungeonSeasonRewardData.receivedValue < m_lstRewardByTry[0].GetRewardCountValue())
			{
				NKCUtil.SetGameobjectActive(m_objDungeonTryRedDot, bValue: true);
			}
			return;
		}
		GuildSeasonRewardTemplet guildSeasonRewardTemplet3 = m_lstRewardByTry[idx - 1];
		GuildSeasonRewardTemplet guildSeasonRewardTemplet4 = ((idx < m_lstRewardByTry.Count) ? m_lstRewardByTry[idx] : null);
		if (guildSeasonRewardTemplet4 != null && NKCGuildCoopManager.m_TryCount > guildSeasonRewardTemplet3.GetRewardCountValue())
		{
			long num3 = guildSeasonRewardTemplet4.GetRewardCountValue() - guildSeasonRewardTemplet3.GetRewardCountValue();
			num2 = (float)(NKCGuildCoopManager.m_TryCount - guildSeasonRewardTemplet3.GetRewardCountValue()) / (float)num3;
		}
		int rewardCountValue2 = guildSeasonRewardTemplet3.GetRewardCountValue();
		component.SetData(m_lstRewardByTry[idx - 1], NKCGuildCoopManager.m_TryCount >= rewardCountValue2 && guildDungeonSeasonRewardData.receivedValue < rewardCountValue2, guildDungeonSeasonRewardData != null && guildDungeonSeasonRewardData.receivedValue >= rewardCountValue2, idx == m_lstRewardByTry.Count, OnClickSlot);
		component.SetGaugeProgress(num2);
		if (num2 >= 1f && guildDungeonSeasonRewardData.receivedValue < m_lstRewardByTry[idx - 1].GetRewardCountValue())
		{
			NKCUtil.SetGameobjectActive(m_objDungeonTryRedDot, bValue: true);
		}
	}

	public void Open(OnClose onClose)
	{
		m_dOnClose = onClose;
		m_CurCategory = GuildDungeonRewardCategory.RANK;
		List<GuildSeasonRewardTemplet> seasonRewardList = GuildDungeonTempletManager.GetSeasonRewardList(NKCGuildCoopManager.m_SeasonId);
		if (seasonRewardList == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_GuildSeasonTemplet = GuildDungeonTempletManager.GetGuildSeasonTemplet(NKCGuildCoopManager.m_SeasonId);
		m_lstRewardByRank = seasonRewardList.FindAll((GuildSeasonRewardTemplet x) => x.GetRewardCategory() == GuildDungeonRewardCategory.RANK);
		m_lstRewardByTry = seasonRewardList.FindAll((GuildSeasonRewardTemplet x) => x.GetRewardCategory() == GuildDungeonRewardCategory.DUNGEON_TRY);
		if (m_CurCategory == GuildDungeonRewardCategory.DUNGEON_TRY)
		{
			m_tglBattleCount.Select(bSelect: true, bForce: true, bImmediate: true);
		}
		else
		{
			m_tglDamagePoint.Select(bSelect: true, bForce: true, bImmediate: true);
		}
		if (NKCGuildManager.HasGuild())
		{
			NKMGuildMemberData nKMGuildMemberData = NKCGuildManager.MyGuildData.members.Find((NKMGuildMemberData x) => x.commonProfile.userUid == NKCScenManager.CurrentUserData().m_UserUID);
			if (nKMGuildMemberData != null)
			{
				m_slotProfile.SetProfiledata(nKMGuildMemberData.commonProfile, null);
			}
		}
		else
		{
			NKMUserProfileData userProfileData = NKCScenManager.CurrentUserData().UserProfileData;
			m_slotProfile.SetProfiledata(userProfileData, null);
		}
		NKCUtil.SetLabelText(m_lbTitle, "[" + NKCStringTable.GetString(m_GuildSeasonTemplet.GetSeasonNameID()) + "]");
		NKCUtil.SetImageSprite(m_imgBgBlur, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_COOP_Texture", m_GuildSeasonTemplet.GetSeasonBgBlurName()));
		SetRemainTime();
		RefreshUI();
		UIOpened();
	}

	private void SetRemainTime()
	{
		NKCUtil.SetLabelText(m_lbRemainTime, NKCUtilString.GetRemainTimeStringEx(NKMTime.LocalToUTC(m_GuildSeasonTemplet.GetSeasonEndDate())));
	}

	public void RefreshUI()
	{
		NKCUtil.SetGameobjectActive(m_objDungeonTryRedDot, NKCGuildCoopManager.CheckSeasonRewardEnable(GuildDungeonRewardCategory.DUNGEON_TRY));
		NKCUtil.SetGameobjectActive(m_objKillPointRedDot, NKCGuildCoopManager.CheckSeasonRewardEnable(GuildDungeonRewardCategory.RANK));
		if (m_CurCategory == GuildDungeonRewardCategory.RANK)
		{
			NKCUtil.SetLabelText(m_lbMyPointTitle, NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_MENU_SEASON_REWARD_KILL_SCORE_STATUS);
			NKCUtil.SetLabelText(m_lbMyPoint, NKCGuildCoopManager.m_KillPoint.ToString("N0"));
			m_loop.TotalCount = m_lstRewardByRank.Count + 1;
		}
		else if (m_CurCategory == GuildDungeonRewardCategory.DUNGEON_TRY)
		{
			NKCUtil.SetLabelText(m_lbMyPointTitle, NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_MENU_SEASON_REWARD_PARTICIPATION_SCORE_STATUS);
			NKCUtil.SetLabelText(m_lbMyPoint, NKCGuildCoopManager.m_TryCount.ToString("N0"));
			m_loop.TotalCount = m_lstRewardByTry.Count + 1;
		}
		if (!base.gameObject.activeSelf)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		}
		m_loop.SetIndexPosition(GetCurrentIndex());
		if (NKCGuildCoopManager.CheckSeasonRewardEnable(m_CurCategory))
		{
			m_btnReceiveAll.UnLock();
		}
		else
		{
			m_btnReceiveAll.Lock();
		}
	}

	private int GetCurrentIndex()
	{
		List<GuildSeasonRewardTemplet> list = new List<GuildSeasonRewardTemplet>();
		switch (m_CurCategory)
		{
		case GuildDungeonRewardCategory.RANK:
			list = m_lstRewardByRank.FindAll((GuildSeasonRewardTemplet x) => x.GetRewardCountValue() <= NKCGuildCoopManager.GetLastReceivedPoint(GuildDungeonRewardCategory.RANK));
			break;
		case GuildDungeonRewardCategory.DUNGEON_TRY:
			list = m_lstRewardByTry.FindAll((GuildSeasonRewardTemplet x) => x.GetRewardCountValue() <= NKCGuildCoopManager.GetLastReceivedPoint(GuildDungeonRewardCategory.DUNGEON_TRY));
			break;
		}
		return list.Count;
	}

	private void OnChangedKillPoint(bool bValue)
	{
		if (bValue)
		{
			m_tglDamagePoint.Select(bSelect: true);
			m_CurCategory = GuildDungeonRewardCategory.RANK;
			RefreshUI();
		}
	}

	private void OnChangedDungeonTry(bool bValue)
	{
		if (bValue)
		{
			m_tglBattleCount.Select(bSelect: true);
			m_CurCategory = GuildDungeonRewardCategory.DUNGEON_TRY;
			RefreshUI();
		}
	}

	private void OnClickReceiveAll()
	{
		if (m_btnReceiveAll.m_bLock)
		{
			return;
		}
		GuildSeasonRewardTemplet guildSeasonRewardTemplet = null;
		int lastIndexRewardEnabled = NKCGuildCoopManager.GetLastIndexRewardEnabled(m_CurCategory);
		if (lastIndexRewardEnabled >= 0)
		{
			switch (m_CurCategory)
			{
			case GuildDungeonRewardCategory.DUNGEON_TRY:
				guildSeasonRewardTemplet = m_lstRewardByTry[lastIndexRewardEnabled];
				break;
			case GuildDungeonRewardCategory.RANK:
				guildSeasonRewardTemplet = m_lstRewardByRank[lastIndexRewardEnabled];
				break;
			}
			if (guildSeasonRewardTemplet != null)
			{
				NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_SEASON_REWARD_REQ(guildSeasonRewardTemplet.GetRewardCategory(), guildSeasonRewardTemplet.GetRewardCountValue());
			}
		}
	}

	private void MoveToProgress()
	{
	}

	private void Update()
	{
		m_fDeltaTime += Time.deltaTime;
		if (m_fDeltaTime > 1f)
		{
			m_fDeltaTime -= 1f;
			SetRemainTime();
		}
	}

	private void OnClickSlot()
	{
		GuildSeasonRewardTemplet guildSeasonRewardTemplet = null;
		int num = 0;
		switch (m_CurCategory)
		{
		case GuildDungeonRewardCategory.DUNGEON_TRY:
			num = m_lstRewardByTry.FindIndex((GuildSeasonRewardTemplet x) => x.GetRewardCountValue() == NKCGuildCoopManager.GetLastReceivedPoint(GuildDungeonRewardCategory.DUNGEON_TRY));
			if (m_lstRewardByTry.Count > num + 1)
			{
				guildSeasonRewardTemplet = m_lstRewardByTry[num + 1];
			}
			break;
		case GuildDungeonRewardCategory.RANK:
			num = m_lstRewardByRank.FindIndex((GuildSeasonRewardTemplet x) => x.GetRewardCountValue() == NKCGuildCoopManager.GetLastReceivedPoint(GuildDungeonRewardCategory.RANK));
			if (m_lstRewardByRank.Count > num + 1)
			{
				guildSeasonRewardTemplet = m_lstRewardByRank[num + 1];
			}
			break;
		}
		if (guildSeasonRewardTemplet != null)
		{
			NKCPacketSender.Send_NKMPacket_GUILD_DUNGEON_SEASON_REWARD_REQ(guildSeasonRewardTemplet.GetRewardCategory(), guildSeasonRewardTemplet.GetRewardCountValue());
		}
		else
		{
			Log.Error("GuildSeasonRewardTemplet is null - ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/GuildCoop/NKCPopupGuildCoopSeasonReward.cs", 465);
		}
	}
}
