using System;
using System.Collections.Generic;
using ClientPacket.Raid;
using ClientPacket.WorldMap;
using Cs.Logging;
using Cs.Math;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIRaid : NKCUIBase
{
	private List<int> m_lstUpsideMenuResource;

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_WORLD_MAP_RAID";

	private const string UI_ASSET_NAME = "NKM_UI_WORLD_MAP_RAID";

	[Header("왼쪽 UI")]
	public Image m_imgCityExp;

	public Text m_lbCityLevel;

	public Text m_lbCityName;

	public GameObject m_objSupportParent;

	public GameObject m_objSupportREQNotYet;

	public NKCUIComStateButton m_csbtnSupportREQ;

	public GameObject m_objSupportREQOnGoing;

	public LoopScrollRect m_lvsrSupportUser;

	public Transform m_trSupportUserListRoot;

	public GameObject m_objSupport;

	public Text m_lbSupport;

	public GameObject m_objNone;

	[Header("가운데 UI")]
	public NKCUICharacterView m_NKCUICharacterViewBoss;

	public GameObject m_objRaidFail;

	[Header("남은 시간")]
	public Text m_lbRemainTime;

	public GameObject m_objRemainTime_Play;

	public GameObject m_objRemainTime_Clear;

	[Header("오른쪽 UI")]
	public NKCUIRaidRightSide m_NKCUIRaidRightSide;

	private bool m_bInit;

	private bool m_bFirstOpenLoopScroll = true;

	private long m_RaidUID;

	private float m_fNextUpdateTime;

	private bool m_fromEventListPopup;

	public override string MenuName => NKCUtilString.GET_STRING_RAID;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	public override List<int> UpsideMenuShowResourceList
	{
		get
		{
			if (m_lstUpsideMenuResource == null)
			{
				return base.UpsideMenuShowResourceList;
			}
			return m_lstUpsideMenuResource;
		}
	}

	public override string GuideTempletID => "ARTICLE_RAID_INFO";

	public static NKCAssetResourceData OpenInstanceAsync()
	{
		return NKCUIBase.OpenInstanceAsync<NKCUIBaseSceneMenu>("AB_UI_NKM_UI_WORLD_MAP_RAID", "NKM_UI_WORLD_MAP_RAID");
	}

	public static bool CheckInstanceLoaded(NKCAssetResourceData loadResourceData, out NKCUIRaid retVal)
	{
		return NKCUIBase.CheckInstanceLoaded<NKCUIRaid>(loadResourceData, NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), out retVal);
	}

	public void CloseInstance()
	{
		NKCAssetResourceManager.CloseResource("AB_UI_NKM_UI_WORLD_MAP_RAID", "NKM_UI_WORLD_MAP_RAID");
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void InitUI()
	{
		if (!m_bInit)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			m_csbtnSupportREQ.PointerClick.RemoveAllListeners();
			m_csbtnSupportREQ.PointerClick.AddListener(OnClickSupportREQ);
			m_lvsrSupportUser.dOnGetObject += GetSlot;
			m_lvsrSupportUser.dOnReturnObject += ReturnSlot;
			m_lvsrSupportUser.dOnProvideData += ProvideData;
			m_NKCUICharacterViewBoss.Init();
			m_NKCUIRaidRightSide.Init();
			m_bInit = true;
		}
	}

	public RectTransform GetSlot(int index)
	{
		NKCUIRaidSupportUserSlot newInstance = NKCUIRaidSupportUserSlot.GetNewInstance(m_trSupportUserListRoot);
		if (newInstance == null)
		{
			return null;
		}
		return newInstance.GetComponent<RectTransform>();
	}

	public void ReturnSlot(Transform tr)
	{
		NKCUIRaidSupportUserSlot component = tr.GetComponent<NKCUIRaidSupportUserSlot>();
		tr.SetParent(base.transform);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			UnityEngine.Object.Destroy(tr.gameObject);
		}
	}

	public void ProvideData(Transform tr, int index)
	{
		NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_RaidUID);
		if (nKMRaidDetailData != null)
		{
			NKCUIRaidSupportUserSlot component = tr.GetComponent<NKCUIRaidSupportUserSlot>();
			if (component != null && nKMRaidDetailData.raidJoinDataList.Count > index && index >= 0)
			{
				component.SetUI(nKMRaidDetailData, nKMRaidDetailData.raidJoinDataList[index], index + 1);
			}
		}
	}

	public bool SetUI()
	{
		NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_RaidUID);
		if (nKMRaidDetailData == null)
		{
			return false;
		}
		NKMWorldMapData worldmapData = NKCScenManager.CurrentUserData().m_WorldmapData;
		if (worldmapData == null)
		{
			return false;
		}
		NKMWorldMapCityData cityData = worldmapData.GetCityData(nKMRaidDetailData.cityID);
		NKMWorldMapCityTemplet cityTemplet = NKMWorldMapManager.GetCityTemplet(nKMRaidDetailData.cityID);
		if (cityTemplet == null)
		{
			return false;
		}
		NKMRaidTemplet nKMRaidTemplet = NKMRaidTemplet.Find(nKMRaidDetailData.stageID);
		if (nKMRaidTemplet == null)
		{
			return false;
		}
		NKCUtil.SetGameobjectActive(m_objSupportParent, nKMRaidTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_RAID);
		NKCUtil.SetGameobjectActive(m_objRaidFail, NKCSynchronizedTime.IsFinished(nKMRaidDetailData.expireDate) && nKMRaidDetailData.curHP > 0f);
		if (cityData == null)
		{
			m_imgCityExp.fillAmount = 0f;
			m_lbCityLevel.text = "0";
		}
		else
		{
			NKMWorldMapCityExpTemplet cityExpTable = NKMWorldMapManager.GetCityExpTable(cityData.level);
			if (cityExpTable != null && cityExpTable.m_ExpRequired != 0)
			{
				m_imgCityExp.fillAmount = (float)cityData.exp / (float)cityExpTable.m_ExpRequired;
			}
			else
			{
				m_imgCityExp.fillAmount = 1f;
			}
			m_lbCityLevel.text = cityData.level.ToString();
		}
		m_lbCityName.text = cityTemplet.GetName();
		NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(nKMRaidTemplet.DungeonTempletBase.m_DungeonID);
		if (dungeonTemplet != null)
		{
			m_NKCUICharacterViewBoss.SetCharacterIllust(dungeonTemplet.m_BossUnitStrID, 0, bEnableBackground: true, bVFX: true, bAsync: false);
		}
		UpdateUI();
		NKCUIRaidRightSide.NKC_RAID_SUB_BUTTON_TYPE eNKC_RAID_SUB_BUTTON_TYPE = NKCUIRaidRightSide.NKC_RAID_SUB_BUTTON_TYPE.NRSBT_READY;
		if (nKMRaidDetailData.curHP.IsNearlyZero() || NKCSynchronizedTime.IsFinished(nKMRaidDetailData.expireDate))
		{
			eNKC_RAID_SUB_BUTTON_TYPE = NKCUIRaidRightSide.NKC_RAID_SUB_BUTTON_TYPE.NRSBT_EXIT;
		}
		_ = nKMRaidDetailData.FindJoinData(NKCScenManager.CurrentUserData().m_UserUID)?.tryAssist;
		m_NKCUIRaidRightSide.SetUI(m_RaidUID, NKCUIRaidRightSide.NKC_RAID_SUB_MENU_TYPE.NRSMT_REMAIN_TIME, eNKC_RAID_SUB_BUTTON_TYPE);
		if (nKMRaidTemplet.StageReqItemID == 1)
		{
			m_lstUpsideMenuResource = new List<int> { 1, 101 };
		}
		else
		{
			m_lstUpsideMenuResource = new List<int> { nKMRaidTemplet.StageReqItemID, 101 };
		}
		bool flag = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().CheckCompletableRaid(m_RaidUID);
		NKCUtil.SetGameobjectActive(m_objRemainTime_Play, !flag);
		NKCUtil.SetGameobjectActive(m_objRemainTime_Clear, flag);
		if (!flag)
		{
			UpdateRemainTime();
		}
		return true;
	}

	private void OnClickSupportREQ()
	{
		string gET_STRING_WARNING = NKCUtilString.GET_STRING_WARNING;
		string gET_STRING_RAID_COOP_REQ_WARNING = NKCUtilString.GET_STRING_RAID_COOP_REQ_WARNING;
		NKCPopupOKCancel.OpenOKCancelBox(gET_STRING_WARNING, gET_STRING_RAID_COOP_REQ_WARNING, delegate
		{
			if (!NKCScenManager.GetScenManager().GetNKCRaidDataMgr().CheckRaidCoopOn(m_RaidUID))
			{
				NKCPacketSender.Send_NKMPacket_RAID_SET_COOP_REQ(m_RaidUID);
			}
		});
	}

	public void Open(long raidUID, bool fromEventListPopup)
	{
		m_RaidUID = raidUID;
		m_fromEventListPopup = fromEventListPopup;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (SetUI())
		{
			UIOpened();
			CheckTutorial();
		}
		else
		{
			Log.Error("SetUI Failed!!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIRaid.cs", 280);
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnBackButton()
	{
		if (m_fromEventListPopup)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_WORLDMAP().SetReserveOpenEventList(selectLastEventListTab: true);
		}
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WORLDMAP);
	}

	private void Update()
	{
		if (!(m_fNextUpdateTime + 1f > Time.time))
		{
			UpdateUI(bFrameMoveUpdate: true);
			UpdateRemainTime();
			m_fNextUpdateTime = Time.time;
		}
	}

	private void UpdateUI(bool bFrameMoveUpdate = false)
	{
		NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_RaidUID);
		if (nKMRaidDetailData == null || NKMRaidTemplet.Find(nKMRaidDetailData.stageID) == null)
		{
			return;
		}
		bool isCoop = nKMRaidDetailData.isCoop;
		bool flag = nKMRaidDetailData.curHP <= 0f || NKCSynchronizedTime.IsFinished(nKMRaidDetailData.expireDate);
		NKCUtil.SetGameobjectActive(m_objSupportREQNotYet, !isCoop && !flag);
		NKCUtil.SetGameobjectActive(m_objSupportREQOnGoing, isCoop);
		if (!bFrameMoveUpdate && isCoop)
		{
			nKMRaidDetailData.SortJoinDataByDamage();
			if (m_bFirstOpenLoopScroll)
			{
				m_lvsrSupportUser.PrepareCells();
				m_bFirstOpenLoopScroll = false;
			}
			m_lvsrSupportUser.TotalCount = nKMRaidDetailData.raidJoinDataList.Count;
			m_lvsrSupportUser.velocity = new Vector2(0f, 0f);
			m_lvsrSupportUser.SetIndexPosition(0);
			NKCUtil.SetGameobjectActive(m_objNone, m_lvsrSupportUser.TotalCount == 0);
			if (!flag)
			{
				NKCUtil.SetGameobjectActive(m_objSupport, bValue: true);
				NKCUtil.SetLabelText(m_lbSupport, NKCUtilString.GET_STRING_RAID_REQ_SUPPORT);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objSupport, bValue: false);
			}
		}
		if (flag)
		{
			m_NKCUICharacterViewBoss.PlayEffect(NKCUICharacterView.EffectType.Gray);
			m_NKCUICharacterViewBoss.SetVFX(bSet: false);
		}
	}

	private void UpdateRemainTime()
	{
		if (!m_objRemainTime_Play.activeSelf)
		{
			return;
		}
		NKMRaidDetailData nKMRaidDetailData = NKCScenManager.GetScenManager().GetNKCRaidDataMgr().Find(m_RaidUID);
		if (nKMRaidDetailData != null)
		{
			if (NKCSynchronizedTime.IsFinished(nKMRaidDetailData.expireDate))
			{
				NKCUtil.SetLabelText(m_lbRemainTime, string.Format(NKCUtilString.GET_STRING_WORLD_MAP_RAID_REMAIN_TIME, NKCUtilString.GetTimeSpanString(TimeSpan.Zero)));
				return;
			}
			DateTime dateTime = new DateTime(nKMRaidDetailData.expireDate);
			DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime();
			TimeSpan timeSpan = dateTime - serverUTCTime;
			NKCUtil.SetLabelText(m_lbRemainTime, string.Format(NKCUtilString.GET_STRING_WORLD_MAP_RAID_REMAIN_TIME, NKCUtilString.GetTimeSpanString(timeSpan)));
		}
	}

	private void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.RaidReady);
	}
}
