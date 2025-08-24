using System.Collections.Generic;
using ClientPacket.Guild;
using Cs.Logging;
using NKC.UI.Result;
using NKM;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildCoopSessionResult : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM_COOP";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_CONSORTIUM_COOP_RESULT";

	private static NKCPopupGuildCoopSessionResult m_Instance;

	[Header("좌측")]
	public Text m_lbTitle;

	public NKCUIComStateButton m_btnStatus;

	[Header("보스")]
	public Image m_imgBossFaceCard;

	public Text m_lbStep;

	public Text m_lbRemainHP;

	public Text m_lbDamagePoint;

	public Image m_imgBossHp;

	public GameObject m_objClear;

	[Header("전리품")]
	public LoopScrollRect m_loopReward;

	public Transform m_trRewardParent;

	public GameObject m_objNone;

	[Header("")]
	public NKCUIComStateButton m_btnOK;

	public Image m_imgBgBlur;

	private Stack<NKCUIWRRewardSlot> m_stkRewardSlot = new Stack<NKCUIWRRewardSlot>();

	private List<NKCUISlot.SlotData> m_lstClearRewardData = new List<NKCUISlot.SlotData>();

	private List<NKCUISlot.SlotData> m_lstArtifactRewardData = new List<NKCUISlot.SlotData>();

	public static NKCPopupGuildCoopSessionResult Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildCoopSessionResult>("AB_UI_NKM_UI_CONSORTIUM_COOP", "NKM_UI_POPUP_CONSORTIUM_COOP_RESULT", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), CloseInstance).GetInstance<NKCPopupGuildCoopSessionResult>();
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

	private static void CloseInstance()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		if (m_btnStatus != null)
		{
			m_btnStatus.PointerClick.RemoveAllListeners();
			m_btnStatus.PointerClick.AddListener(OnClickStatus);
		}
		NKCUtil.SetButtonClickDelegate(m_btnOK, base.Close);
		NKCUtil.SetHotkey(m_btnOK, HotkeyEventType.Confirm);
		if (m_loopReward != null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			m_loopReward.dOnGetObject += GetObjectReward;
			m_loopReward.dOnReturnObject += ReturnObjectReward;
			m_loopReward.dOnProvideData += ProvideDataReward;
			m_loopReward.PrepareCells();
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private RectTransform GetObjectReward(int idx)
	{
		NKCUIWRRewardSlot nKCUIWRRewardSlot = null;
		nKCUIWRRewardSlot = ((m_stkRewardSlot.Count <= 0) ? NKCUIWRRewardSlot.GetNewInstance(m_trRewardParent) : m_stkRewardSlot.Pop());
		nKCUIWRRewardSlot.transform.SetParent(base.transform);
		return nKCUIWRRewardSlot.GetComponent<RectTransform>();
	}

	private void ReturnObjectReward(Transform tr)
	{
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.transform);
		NKCUIWRRewardSlot component = tr.GetComponent<NKCUIWRRewardSlot>();
		if (component != null)
		{
			m_stkRewardSlot.Push(component);
		}
	}

	private void ProvideDataReward(Transform tr, int idx)
	{
		NKCUIWRRewardSlot component = tr.GetComponent<NKCUIWRRewardSlot>();
		if (component == null || idx > m_lstClearRewardData.Count + m_lstArtifactRewardData.Count)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
			return;
		}
		if (idx < m_lstClearRewardData.Count)
		{
			component.SetUI(m_lstClearRewardData[idx], idx);
			component.SetKillRewardMark(bSet: true);
			component.SetArtifactMark(bSet: false);
		}
		else
		{
			component.SetUI(m_lstArtifactRewardData[idx - m_lstClearRewardData.Count], idx);
			component.SetKillRewardMark(bSet: false);
			component.SetArtifactMark(bSet: true);
		}
		component.InvalidAni();
	}

	public void Open(NKMPacket_GUILD_DUNGEON_SESSION_REWARD_ACK sPacket)
	{
		if (sPacket == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			Log.Error("NKMPacket_GUILD_DUNGEON_SESSION_REWARD_ACK is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/GuildCoop/NKCPopupGuildCoopSessionResult.cs", 174);
			return;
		}
		GuildSeasonTemplet guildSeasonTemplet = GuildSeasonTemplet.Find(NKCGuildCoopManager.m_SeasonId);
		NKCUtil.SetImageSprite(m_imgBgBlur, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_COOP_Texture", guildSeasonTemplet.GetSeasonBgBlurName()));
		GuildRaidTemplet guildRaidTemplet = null;
		guildRaidTemplet = ((GuildDungeonTempletManager.GetRaidTempletList(guildSeasonTemplet.GetSeasonRaidGroup()).Count <= sPacket.stageIndex) ? GuildDungeonTempletManager.GetRaidTempletList(guildSeasonTemplet.GetSeasonRaidGroup())[sPacket.stageIndex - 1] : GuildDungeonTempletManager.GetRaidTempletList(guildSeasonTemplet.GetSeasonRaidGroup())[sPacket.stageIndex]);
		if (guildRaidTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			Log.Error($"dungeonTempletBase is null - stageIndex : {sPacket.stageIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/GuildCoop/NKCPopupGuildCoopSessionResult.cs", 195);
			return;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(guildRaidTemplet.GetStageId());
		if (dungeonTempletBase == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			Log.Error($"dungeonTempletBase is null - stageID : {guildRaidTemplet.GetStageId()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/GuildCoop/NKCPopupGuildCoopSessionResult.cs", 203);
			return;
		}
		if (sPacket.stageIndex == 0)
		{
			sPacket.remainHp = (long)NKMDungeonManager.GetBossHp(guildRaidTemplet.GetStageId(), dungeonTempletBase.m_DungeonLevel);
		}
		NKCUtil.SetGameobjectActive(m_imgBossFaceCard, bValue: true);
		NKCUtil.SetImageSprite(m_imgBossFaceCard, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_unit_face_card", guildRaidTemplet.GetRaidBossFaceCardName()));
		if (sPacket.stageIndex == 0)
		{
			NKCUtil.SetGameobjectActive(m_objClear, bValue: false);
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_POPUP_CONSORTIUM_COOP_RESULT_TITLE02);
			NKCUtil.SetLabelText(m_lbStep, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_RESULT_BOSS_SUMMARY_INFO_FAIL_2, dungeonTempletBase.GetDungeonName()));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objClear, bValue: true);
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_POPUP_CONSORTIUM_COOP_RESULT_TITLE01);
			if (sPacket.stageIndex > 15)
			{
				NKCUtil.SetLabelText(m_lbStep, NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_RESULT_BOSS_SUMMARY_INFO_2);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbStep, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_RESULT_BOSS_SUMMARY_INFO, sPacket.stageIndex));
			}
		}
		float bossHp = NKMDungeonManager.GetBossHp(guildRaidTemplet.GetStageId(), dungeonTempletBase.m_DungeonLevel);
		NKCUtil.SetLabelText(m_lbRemainHP, string.Format("{0} ({1:0.##}%)", sPacket.remainHp.ToString("N0"), (float)sPacket.remainHp / bossHp * 100f));
		m_imgBossHp.fillAmount = (float)sPacket.remainHp / bossHp;
		NKCUtil.SetLabelText(m_lbDamagePoint, sPacket.clearPoint.ToString("N0"));
		m_lstClearRewardData.Clear();
		for (int i = 0; i < sPacket.rewardList.Count; i++)
		{
			m_lstClearRewardData.Add(NKCUISlot.SlotData.MakeMiscItemData(sPacket.rewardList[i]));
		}
		m_lstArtifactRewardData.Clear();
		for (int j = 0; j < sPacket.artifactReward.Count; j++)
		{
			m_lstArtifactRewardData.Add(NKCUISlot.SlotData.MakeMiscItemData(sPacket.artifactReward[j]));
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loopReward.TotalCount = m_lstClearRewardData.Count + m_lstArtifactRewardData.Count;
		m_loopReward.SetIndexPosition(0);
		NKCUtil.SetGameobjectActive(m_objNone, m_loopReward.TotalCount == 0);
		UIOpened();
	}

	private void OnClickStatus()
	{
		NKCPopupGuildCoopStatus.Instance.Open();
	}
}
