using System;
using System.Collections.Generic;
using ClientPacket.Mode;
using NKC.Templet;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperationSubSummarySlot : MonoBehaviour
{
	[Header("공용")]
	public NKCUIComStateButton m_btn;

	public GameObject m_objRemainTime;

	public Text m_lbRemainTime;

	public GameObject m_ObjEventDrop;

	[Header("에피소드 타이틀")]
	public Text m_lbTitle;

	[Header("Big 전용")]
	public Text m_lbCategory;

	[Header("Small 전용")]
	public Image m_imgSlotBG;

	public Image m_imgCategory;

	[Header("격전지원 결산중 표시")]
	public GameObject m_objFierceReward;

	[Header("스테이지 진행도")]
	public Text m_lbStageNum;

	private NKCEpisodeSummaryTemplet m_SummaryTemplet;

	private bool m_bUseRemainTime;

	private DateTime m_EndDateUTC = DateTime.MinValue;

	private NKM_SHORTCUT_TYPE m_ShortcutType;

	private string m_ShortcutParam;

	private float m_fDeltaTime;

	public bool SetData(NKCEpisodeSummaryTemplet summaryTemplet)
	{
		m_fDeltaTime = 0f;
		m_btn.PointerClick.RemoveAllListeners();
		m_btn.PointerClick.AddListener(OnClickBtn);
		if (summaryTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return false;
		}
		m_SummaryTemplet = summaryTemplet;
		m_ShortcutType = summaryTemplet.m_ShortcutType;
		m_ShortcutParam = summaryTemplet.m_ShortcutParam;
		if (m_imgSlotBG != null && !string.IsNullOrEmpty(summaryTemplet.m_SubResourceID))
		{
			NKCUtil.SetImageSprite(m_imgSlotBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_OPERATION_Thumbnail", summaryTemplet.m_SubResourceID));
		}
		if (m_imgCategory != null)
		{
			NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet = NKMEpisodeGroupTemplet.Find(m_SummaryTemplet.m_EPCategory);
			if (nKMEpisodeGroupTemplet != null)
			{
				NKCUtil.SetImageSprite(m_imgCategory, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_OPERATION_Thumbnail", nKMEpisodeGroupTemplet.m_EPGroupIcon));
			}
		}
		if (m_lbStageNum != null)
		{
			NKCUtil.SetLabelText(m_lbStageNum, "");
		}
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(summaryTemplet.m_EpisodeID, summaryTemplet.m_Difficulty);
		if (nKMEpisodeTempletV != null)
		{
			NKCUtil.SetLabelText(m_lbTitle, nKMEpisodeTempletV.GetEpisodeName());
			if (m_lbCategory != null)
			{
				NKCUtil.SetLabelText(m_lbCategory, string.Format(NKCUtilString.GET_STRING_EPISODE_PROGRESS, NKCUtilString.GetEpisodeCategory(nKMEpisodeTempletV.m_EPCategory)));
			}
			NKCUtil.SetGameobjectActive(m_ObjEventDrop, nKMEpisodeTempletV.HaveEventDrop || nKMEpisodeTempletV.HaveBuffDrop);
			if (m_SummaryTemplet.HasDateLimit())
			{
				m_EndDateUTC = m_SummaryTemplet.IntervalTemplet.GetEndDateUtc();
				if (m_EndDateUTC >= NKMConst.Post.UnlimitedExpirationUtcDate)
				{
					m_bUseRemainTime = false;
				}
				else
				{
					m_bUseRemainTime = true;
				}
				NKCUtil.SetGameobjectActive(m_objRemainTime, m_bUseRemainTime);
				if (m_bUseRemainTime)
				{
					SetRemainTime();
				}
			}
			else
			{
				m_bUseRemainTime = false;
				NKCUtil.SetGameobjectActive(m_objRemainTime, bValue: false);
			}
			NKCUtil.SetGameobjectActive(m_objFierceReward, bValue: false);
		}
		else
		{
			if (m_SummaryTemplet.m_EPCategory == EPISODE_CATEGORY.EC_FIERCE)
			{
				NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_FIERCE);
				NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
				NKCFierceBattleSupportDataMgr.FIERCE_STATUS status = nKCFierceBattleSupportDataMgr.GetStatus();
				if (nKCFierceBattleSupportDataMgr.FierceTemplet == null || !nKCFierceBattleSupportDataMgr.IsCanAccessFierce())
				{
					NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
					return false;
				}
				m_ShortcutType = NKM_SHORTCUT_TYPE.SHORTCUT_FIERCE;
				switch (status)
				{
				case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_WAIT:
					m_EndDateUTC = NKMTime.LocalToUTC(nKCFierceBattleSupportDataMgr.FierceTemplet.FierceGameStart);
					break;
				case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_ACTIVATE:
					m_EndDateUTC = NKMTime.LocalToUTC(nKCFierceBattleSupportDataMgr.FierceTemplet.FierceGameEnd);
					break;
				case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_REWARD:
				case NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_COMPLETE:
					m_EndDateUTC = NKMTime.LocalToUTC(nKCFierceBattleSupportDataMgr.FierceTemplet.FierceRewardPeriodEnd);
					break;
				}
				NKCUtil.SetGameobjectActive(m_objFierceReward, status == NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_REWARD || status == NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_COMPLETE);
				m_bUseRemainTime = m_EndDateUTC != DateTime.MinValue;
				NKCUtil.SetGameobjectActive(m_objRemainTime, m_bUseRemainTime);
				if (m_bUseRemainTime)
				{
					SetRemainTime();
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objFierceReward, bValue: false);
			}
			NKCUtil.SetGameobjectActive(m_ObjEventDrop, bValue: false);
		}
		return true;
	}

	public bool SetLastPlayInfo(NKMShortCutInfo lastPlayData)
	{
		m_btn.PointerClick.RemoveAllListeners();
		m_btn.PointerClick.AddListener(OnClickBtn);
		if (lastPlayData != null || lastPlayData.gameType == 0)
		{
			NKCEpisodeSummaryTemplet nKCEpisodeSummaryTemplet = null;
			switch ((NKM_GAME_TYPE)(byte)lastPlayData.gameType)
			{
			case NKM_GAME_TYPE.NGT_PRACTICE:
			case NKM_GAME_TYPE.NGT_DUNGEON:
			case NKM_GAME_TYPE.NGT_TUTORIAL:
			case NKM_GAME_TYPE.NGT_CUTSCENE:
			case NKM_GAME_TYPE.NGT_PHASE:
			{
				NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(lastPlayData.stageId);
				if (nKMStageTempletV != null)
				{
					NKCUtil.SetLabelText(m_lbTitle, nKMStageTempletV.EpisodeTemplet.GetEpisodeName());
					NKMDungeonTempletBase dungeonTempletBase = nKMStageTempletV.DungeonTempletBase;
					if (dungeonTempletBase != null && dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE)
					{
						NKCUtil.SetLabelText(m_lbStageNum, string.Format(NKCUtilString.GET_STRING_EP_CUTSCEN_NUMBER, nKMStageTempletV.m_StageUINum));
					}
					else
					{
						NKCUtil.SetLabelText(m_lbStageNum, $"{nKMStageTempletV.ActId}-{nKMStageTempletV.m_StageUINum}");
					}
					NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet2 = NKMEpisodeGroupTemplet.Find(nKMStageTempletV.EpisodeCategory);
					if (nKMEpisodeGroupTemplet2 != null)
					{
						NKCUtil.SetImageSprite(m_imgCategory, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_OPERATION_Thumbnail", nKMEpisodeGroupTemplet2.m_EPGroupIcon));
					}
					m_bUseRemainTime = nKMStageTempletV.EpisodeTemplet.HasEventTimeLimit;
					NKCUtil.SetGameobjectActive(m_objRemainTime, m_bUseRemainTime);
					if (m_bUseRemainTime)
					{
						m_EndDateUTC = nKMStageTempletV.EpisodeTemplet.EpisodeDateEndUtc;
					}
					SetRemainTime();
					m_ShortcutType = NKM_SHORTCUT_TYPE.SHORTCUT_DUNGEON;
					m_ShortcutParam = nKMStageTempletV.Key.ToString();
					NKCUtil.SetGameobjectActive(m_ObjEventDrop, NKMEpisodeMgr.CheckStageHasEventDrop(nKMStageTempletV) || NKMEpisodeMgr.CheckStageHasBuffDrop(nKMStageTempletV));
					NKCUtil.SetGameobjectActive(m_objFierceReward, bValue: false);
					return true;
				}
				break;
			}
			case NKM_GAME_TYPE.NGT_FIERCE:
				nKCEpisodeSummaryTemplet = NKCEpisodeSummaryTemplet.Find(EPISODE_CATEGORY.EC_FIERCE, 0);
				if (nKCEpisodeSummaryTemplet != null)
				{
					return SetData(nKCEpisodeSummaryTemplet);
				}
				break;
			case NKM_GAME_TYPE.NGT_SHADOW_PALACE:
				nKCEpisodeSummaryTemplet = NKCEpisodeSummaryTemplet.Find(EPISODE_CATEGORY.EC_SHADOW, 0);
				if (nKCEpisodeSummaryTemplet == null)
				{
					break;
				}
				m_SummaryTemplet = nKCEpisodeSummaryTemplet;
				m_ShortcutType = nKCEpisodeSummaryTemplet.m_ShortcutType;
				m_ShortcutParam = nKCEpisodeSummaryTemplet.m_ShortcutParam;
				if (m_imgCategory != null)
				{
					NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet3 = NKMEpisodeGroupTemplet.Find(m_SummaryTemplet.m_EPCategory);
					if (nKMEpisodeGroupTemplet3 != null)
					{
						NKCUtil.SetImageSprite(m_imgCategory, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_OPERATION_Thumbnail", nKMEpisodeGroupTemplet3.m_EPGroupIcon));
					}
				}
				if (m_lbStageNum != null)
				{
					NKCUtil.SetLabelText(m_lbStageNum, "");
				}
				NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GetEpisodeCategory(EPISODE_CATEGORY.EC_SHADOW));
				m_bUseRemainTime = false;
				NKCUtil.SetGameobjectActive(m_objRemainTime, bValue: false);
				NKCUtil.SetGameobjectActive(m_ObjEventDrop, bValue: false);
				NKCUtil.SetGameobjectActive(m_objFierceReward, bValue: false);
				return true;
			case NKM_GAME_TYPE.NGT_TRIM:
				nKCEpisodeSummaryTemplet = NKCEpisodeSummaryTemplet.Find(EPISODE_CATEGORY.EC_TRIM, 0);
				if (nKCEpisodeSummaryTemplet == null)
				{
					break;
				}
				m_SummaryTemplet = nKCEpisodeSummaryTemplet;
				m_ShortcutType = nKCEpisodeSummaryTemplet.m_ShortcutType;
				m_ShortcutParam = nKCEpisodeSummaryTemplet.m_ShortcutParam;
				if (m_imgCategory != null)
				{
					NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet = NKMEpisodeGroupTemplet.Find(m_SummaryTemplet.m_EPCategory);
					if (nKMEpisodeGroupTemplet != null)
					{
						NKCUtil.SetImageSprite(m_imgCategory, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_OPERATION_Thumbnail", nKMEpisodeGroupTemplet.m_EPGroupIcon));
					}
				}
				if (m_lbStageNum != null)
				{
					NKCUtil.SetLabelText(m_lbStageNum, "");
				}
				NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GetEpisodeCategory(EPISODE_CATEGORY.EC_TRIM));
				m_bUseRemainTime = false;
				NKCUtil.SetGameobjectActive(m_objRemainTime, bValue: false);
				NKCUtil.SetGameobjectActive(m_ObjEventDrop, bValue: false);
				NKCUtil.SetGameobjectActive(m_objFierceReward, bValue: false);
				return true;
			}
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		return false;
	}

	public bool SetMainStreamProgress()
	{
		m_btn.PointerClick.RemoveAllListeners();
		m_btn.PointerClick.AddListener(OnClickBtn);
		NKMStageTempletV2 nKMStageTempletV = FindPlaybleStageTemplet();
		if (nKMStageTempletV == null)
		{
			return false;
		}
		m_ShortcutType = NKM_SHORTCUT_TYPE.SHORTCUT_DUNGEON;
		m_ShortcutParam = nKMStageTempletV.Key.ToString();
		NKCUtil.SetLabelText(m_lbTitle, nKMStageTempletV.EpisodeTemplet.GetEpisodeName());
		NKMDungeonTempletBase dungeonTempletBase = nKMStageTempletV.DungeonTempletBase;
		if (dungeonTempletBase != null && dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE)
		{
			NKCUtil.SetLabelText(m_lbStageNum, string.Format(NKCUtilString.GET_STRING_EP_CUTSCEN_NUMBER, nKMStageTempletV.m_StageUINum));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbStageNum, $"{nKMStageTempletV.ActId}-{nKMStageTempletV.m_StageUINum}");
		}
		NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet = NKMEpisodeGroupTemplet.Find(nKMStageTempletV.EpisodeCategory);
		if (nKMEpisodeGroupTemplet != null)
		{
			NKCUtil.SetImageSprite(m_imgCategory, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_OPERATION_Thumbnail", nKMEpisodeGroupTemplet.m_EPGroupIcon));
		}
		NKCEpisodeSummaryTemplet nKCEpisodeSummaryTemplet = NKCEpisodeSummaryTemplet.Find(nKMStageTempletV.EpisodeCategory, nKMStageTempletV.EpisodeId);
		if (nKCEpisodeSummaryTemplet != null && m_imgSlotBG != null && !string.IsNullOrEmpty(nKCEpisodeSummaryTemplet.m_SubResourceID))
		{
			NKCUtil.SetImageSprite(m_imgSlotBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_OPERATION_Thumbnail", nKCEpisodeSummaryTemplet.m_SubResourceID));
		}
		m_bUseRemainTime = false;
		NKCUtil.SetGameobjectActive(m_objRemainTime, bValue: false);
		NKCUtil.SetGameobjectActive(m_ObjEventDrop, nKMStageTempletV.EpisodeTemplet.HaveEventDrop || nKMStageTempletV.EpisodeTemplet.HaveBuffDrop);
		NKCUtil.SetGameobjectActive(m_objFierceReward, bValue: false);
		return true;
	}

	private NKMStageTempletV2 FindPlaybleStageTemplet()
	{
		NKMUserData cNKMUserData = NKCScenManager.CurrentUserData();
		List<NKMEpisodeTempletV2> listNKMEpisodeTempletByCategory = NKMEpisodeMgr.GetListNKMEpisodeTempletByCategory(EPISODE_CATEGORY.EC_MAINSTREAM, bOnlyOpen: true, EPISODE_DIFFICULTY.HARD);
		NKMEpisodeTempletV2 nKMEpisodeTempletV = null;
		for (int num = listNKMEpisodeTempletByCategory.Count - 1; num >= 0; num--)
		{
			if (NKMEpisodeMgr.IsPossibleEpisode(cNKMUserData, listNKMEpisodeTempletByCategory[num].m_EpisodeID, listNKMEpisodeTempletByCategory[num].m_Difficulty))
			{
				nKMEpisodeTempletV = listNKMEpisodeTempletByCategory[num];
				break;
			}
		}
		NKMStageTempletV2 result = null;
		bool flag = false;
		foreach (KeyValuePair<int, List<NKMStageTempletV2>> item in nKMEpisodeTempletV.m_DicStage)
		{
			if (flag)
			{
				break;
			}
			for (int i = 0; i < item.Value.Count; i++)
			{
				if (flag)
				{
					break;
				}
				if (NKMContentUnlockManager.IsContentUnlocked(cNKMUserData, in item.Value[i].m_UnlockInfo))
				{
					result = item.Value[i];
				}
				else
				{
					flag = true;
				}
			}
		}
		return result;
	}

	private void SetRemainTime()
	{
		NKCUtil.SetLabelText(m_lbRemainTime, NKCUtilString.GetRemainTimeString(m_EndDateUTC, 1));
		if (NKCSynchronizedTime.IsFinished(m_EndDateUTC))
		{
			m_bUseRemainTime = false;
		}
	}

	private void Update()
	{
		if (m_bUseRemainTime && m_objRemainTime != null && m_objRemainTime.activeSelf)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > 1f)
			{
				m_fDeltaTime -= 1f;
				SetRemainTime();
			}
		}
	}

	private void OnClickBtn()
	{
		if (m_ShortcutType == NKM_SHORTCUT_TYPE.SHORTCUT_FIERCE)
		{
			NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
			nKCFierceBattleSupportDataMgr.GetStatus();
			if (nKCFierceBattleSupportDataMgr.FierceTemplet == null || !nKCFierceBattleSupportDataMgr.IsCanAccessFierce())
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
				return;
			}
		}
		NKCContentManager.MoveToShortCut(m_ShortcutType, m_ShortcutParam, bForce: true);
	}
}
