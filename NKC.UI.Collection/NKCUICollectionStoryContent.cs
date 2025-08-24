using System.Collections.Generic;
using NKC.Templet;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionStoryContent : MonoBehaviour
{
	[Header("설정")]
	public GameObject m_objMainStream;

	public GameObject m_objSideStory;

	public Text m_NKM_UI_COLLECTION_STORY_TITLE_TEXT_MAIN_01;

	public Text m_NKM_UI_COLLECTION_STORY_TITLE_TEXT_MAIN_02;

	public Text m_NKM_UI_COLLECTION_STORY_TITLE_TEXT_SIDE_01;

	public Text m_NKM_UI_COLLECTION_STORY_TITLE_TEXT_SIDE_02;

	public RectTransform m_rtParent;

	private const int ExtraActDivision = 100;

	private List<RectTransform> m_lstRentalSlot = new List<RectTransform>();

	private List<RectTransform> m_lstRentalSubTitle = new List<RectTransform>();

	private HashSet<int> m_ActTitle = new HashSet<int>();

	public List<RectTransform> GetRentalList()
	{
		return m_lstRentalSlot;
	}

	public List<RectTransform> GetSubRentalList()
	{
		return m_lstRentalSubTitle;
	}

	public void ClearRentalList()
	{
		m_lstRentalSlot.Clear();
		m_lstRentalSubTitle.Clear();
	}

	public void Init()
	{
	}

	public void SetData(NKCUICollectionStory.StorySlotData SlotData, List<RectTransform> lstSlot, List<RectTransform> lstSubTitle)
	{
		if (SlotData == null)
		{
			return;
		}
		m_ActTitle.Clear();
		NKCCollectionManager.COLLECTION_STORY_CATEGORY eCategory = SlotData.m_eCategory;
		bool flag = false;
		switch (eCategory)
		{
		case NKCCollectionManager.COLLECTION_STORY_CATEGORY.MAINSTREAM:
			NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_STORY_TITLE_TEXT_MAIN_01, SlotData.m_EpisodeTitle);
			NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_STORY_TITLE_TEXT_MAIN_02, SlotData.m_EpisodeName);
			break;
		case NKCCollectionManager.COLLECTION_STORY_CATEGORY.WORLDMAP:
			NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_STORY_TITLE_TEXT_SIDE_01, NKCUtilString.GET_STRING_DIVE);
			NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_STORY_TITLE_TEXT_SIDE_02, "");
			break;
		case NKCCollectionManager.COLLECTION_STORY_CATEGORY.ETC:
			NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_STORY_TITLE_TEXT_SIDE_01, "");
			NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_STORY_TITLE_TEXT_SIDE_02, SlotData.m_EpisodeName);
			flag = true;
			break;
		default:
			NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_STORY_TITLE_TEXT_SIDE_01, SlotData.m_EpisodeTitle);
			NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_STORY_TITLE_TEXT_SIDE_02, SlotData.m_EpisodeName);
			break;
		}
		NKCUtil.SetGameobjectActive(m_objMainStream, eCategory == NKCCollectionManager.COLLECTION_STORY_CATEGORY.MAINSTREAM);
		NKCUtil.SetGameobjectActive(m_objSideStory, eCategory != NKCCollectionManager.COLLECTION_STORY_CATEGORY.MAINSTREAM);
		List<NKCUICollectionStory.StoryData> lstStoryData = SlotData.m_lstStoryData;
		int num = 0;
		for (int i = 0; i < lstStoryData.Count; i++)
		{
			NKCUICollectionStorySlot component = lstSlot[i].GetComponent<NKCUICollectionStorySlot>();
			if (!(null != component))
			{
				continue;
			}
			if (lstStoryData[i].m_bShowUnlockedOnly && !NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in lstStoryData[i].m_UnlockInfo))
			{
				NKCUtil.SetGameobjectActive(component, bValue: false);
				continue;
			}
			if (CheckActTitle(lstStoryData[i].m_ActID))
			{
				NKCUICollectionStorySubTitle component2 = lstSubTitle[num].GetComponent<NKCUICollectionStorySubTitle>();
				string title = string.Empty;
				if (eCategory != NKCCollectionManager.COLLECTION_STORY_CATEGORY.WORLDMAP)
				{
					title = ((lstStoryData[i].m_ActID >= 100) ? string.Format(NKCUtilString.GET_STRING_COLLECTION_STORY_EXTRA_TITLE_ONE_PARAM, lstStoryData[i].m_ActID % 100) : string.Format(NKCUtilString.GET_STRING_COLLECTION_STORY_SUB_TITLE_ONE_PARAM, lstStoryData[i].m_ActID));
				}
				else
				{
					NKMDiveTemplet nKMDiveTemplet = NKMDiveTemplet.Find(lstStoryData[i].m_UnlockInfo.reqValue);
					if (nKMDiveTemplet != null)
					{
						title = nKMDiveTemplet.Get_STAGE_NAME();
					}
				}
				component2.SetTitle(title);
				lstSubTitle[num].SetParent(m_rtParent);
				lstSubTitle[num].GetComponent<RectTransform>().localScale = Vector3.one;
				NKCUtil.SetGameobjectActive(lstSubTitle[num].gameObject, !flag);
				m_lstRentalSubTitle.Add(lstSubTitle[num]);
				num++;
			}
			int actID = ((lstStoryData[i].m_ActID < 100) ? lstStoryData[i].m_ActID : (lstStoryData[i].m_ActID % 100));
			SetSlotData(actID, lstStoryData[i].m_MissionIdx, component, lstStoryData[i].m_UnlockInfo, lstStoryData[i].m_bClear, lstStoryData[i].m_strBeforeCutScene, lstStoryData[i].m_strAfterCutScene);
			m_lstRentalSlot.Add(lstSlot[i]);
		}
	}

	private void SetSlotData(int actID, int missionID, NKCUICollectionStorySlot slot, UnlockInfo unlockInfo, bool bClear, string ForceBeforeCutScene, string ForceAfterCutScene)
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME();
		int reqValue = unlockInfo.reqValue;
		STAGE_UNLOCK_REQ_TYPE eReqType = unlockInfo.eReqType;
		switch (eReqType)
		{
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE:
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(reqValue);
			if (nKMWarfareTemplet != null)
			{
				string slotNumber2 = ((nKMWarfareTemplet.StageTemplet.DungeonTempletBase != null && nKMWarfareTemplet.StageTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE) ? "" : NKCUtilString.GetEpisodeNumber(nKMWarfareTemplet.StageTemplet.EpisodeTemplet, nKMWarfareTemplet.StageTemplet));
				slot.SetData(slotNumber2, nKMWarfareTemplet.StageTemplet.Key, nKMWarfareTemplet.GetWarfareName(), bClear, nKMWarfareTemplet.m_CutScenStrIDBefore, nKMWarfareTemplet.m_CutScenStrIDAfter);
			}
			else
			{
				Debug.Log($"fail!!?!?!?{eReqType}");
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON:
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(reqValue);
			if (dungeonTempletBase != null)
			{
				string slotNumber5 = ((dungeonTempletBase.StageTemplet.DungeonTempletBase != null && dungeonTempletBase.StageTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE) ? "" : NKCUtilString.GetEpisodeNumber(dungeonTempletBase.StageTemplet.EpisodeTemplet, dungeonTempletBase.StageTemplet));
				slot.SetData(slotNumber5, dungeonTempletBase.StageTemplet.Key, dungeonTempletBase.GetDungeonName(), bClear, dungeonTempletBase.m_CutScenStrIDBefore, dungeonTempletBase.m_CutScenStrIDAfter);
			}
			else
			{
				Debug.Log($"fail!!?!?!?{eReqType}");
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PHASE:
		{
			NKMPhaseTemplet nKMPhaseTemplet = NKMPhaseTemplet.Find(reqValue);
			if (nKMPhaseTemplet != null)
			{
				string slotNumber = ((nKMPhaseTemplet.StageTemplet.DungeonTempletBase != null && nKMPhaseTemplet.StageTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE) ? "" : NKCUtilString.GetEpisodeNumber(nKMPhaseTemplet.StageTemplet.EpisodeTemplet, nKMPhaseTemplet.StageTemplet));
				slot.SetData(slotNumber, nKMPhaseTemplet.StageTemplet.Key, nKMPhaseTemplet.GetName(), bClear, nKMPhaseTemplet.m_CutScenStrIDBefore, nKMPhaseTemplet.m_CutScenStrIDAfter);
			}
			else
			{
				Debug.Log($"fail!!?!?!?{eReqType}");
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_DIVE_HISTORY_CLEARED:
		{
			NKMDiveTemplet nKMDiveTemplet = NKMDiveTemplet.Find(reqValue);
			if (nKMDiveTemplet != null)
			{
				string slotNumber3 = $"{actID}-{missionID}";
				slot.SetData(slotNumber3, 0, $"{nKMDiveTemplet.Get_STAGE_NAME()}-{missionID}", bClear, nKMDiveTemplet.GetCutsceneID(missionID), "", bIsDive: true);
			}
			else
			{
				Debug.Log($"fail!!?!?!?{eReqType}");
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_TRIM:
		{
			NKMTrimTemplet nKMTrimTemplet = NKMTrimTemplet.Find(reqValue);
			if (nKMTrimTemplet != null)
			{
				string slotNumber4 = $"{actID}-{missionID}";
				slot.SetData(slotNumber4, 0, $"{NKCStringTable.GetString(nKMTrimTemplet.TirmGroupName)}-{missionID}", bClear, ForceBeforeCutScene, ForceAfterCutScene, bIsDive: true);
			}
			else
			{
				Debug.Log($"fail!!?!?!?{eReqType}");
			}
			break;
		}
		case STAGE_UNLOCK_REQ_TYPE.SURT_HISTORY_BIRTHDAY:
		{
			if (NKCCollectionManager.GetCollectionCutsceneData().TryGetValue(-3, out var value))
			{
				NKCCollectionEtcCutsceneTemplet nKCCollectionEtcCutsceneTemplet = value.Find((NKCCollectionEtcCutsceneTemplet x) => x.UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_HISTORY_BIRTHDAY && x.UnlockInfo.reqValue == reqValue);
				if (nKCCollectionEtcCutsceneTemplet != null)
				{
					slot.SetData("", 0, NKCStringTable.GetString(nKCCollectionEtcCutsceneTemplet.m_CutSceneDesc), NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in nKCCollectionEtcCutsceneTemplet.UnlockInfo), ForceBeforeCutScene, ForceAfterCutScene);
				}
			}
			else
			{
				Debug.Log($"fail!!?!?!?{eReqType}");
			}
			break;
		}
		default:
			Debug.LogError($"NKCUICollectionStoryContent::SetSlotData - Can not define unlock-reqType : {eReqType}");
			break;
		}
		NKCUtil.SetGameobjectActive(slot.gameObject, bValue: true);
		slot.transform.SetParent(m_rtParent, worldPositionStays: false);
		slot.GetComponent<RectTransform>().localScale = Vector3.one;
	}

	private bool CheckActTitle(int ActID)
	{
		return m_ActTitle.Add(ActID);
	}
}
