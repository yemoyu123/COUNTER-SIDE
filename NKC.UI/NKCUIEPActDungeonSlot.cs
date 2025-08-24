using ClientPacket.Common;
using ClientPacket.Warfare;
using NKC.UI.Tooltip;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIEPActDungeonSlot : MonoBehaviour, IDungeonSlot
{
	[Header("Dungeon")]
	public Image m_imgBoss;

	public GameObject m_objTraining;

	public GameObject m_objDungeonEPNumber;

	public Text m_lbDungeonEPNumber;

	public Text m_lbDungeonNumber;

	public Text m_lbDungeonName;

	public Text m_lbRecommendFightPower;

	public NKCUIComButton m_Button;

	public GameObject m_NKM_UI_OPERATION_EPISODE_lIST_SLOT_SHADOW_COVER_TITLE;

	public GameObject m_objWarfareBG;

	public Image m_NKM_UI_OPERATION_EPISODE_lIST_SLOT_BG_SHADOW;

	public Image m_NKM_UI_OPERATION_EPISODE_lIST_SLOT_BG_SHADOW_CLEAR;

	public GameObject m_objTrainingBG;

	public GameObject m_objTrainingClearBG;

	public GameObject m_objCutscenBG;

	public GameObject m_objCutscenDungeonBG;

	public GameObject m_objCutscenDungeonBGClear;

	public GameObject m_NKM_UI_OPERATION_EPISODE_lIST_SLOT_LEVEL;

	public GameObject m_objPlaying;

	public GameObject m_goMissionList;

	public GameObject m_goMission1_BG;

	public GameObject m_goMission2_BG;

	public GameObject m_goMission3_BG;

	public GameObject m_goMission1;

	public GameObject m_goMission2;

	public GameObject m_goMission3;

	public GameObject m_goClearOn;

	public GameObject m_goClearOff;

	public GameObject m_goLock;

	public Text m_lbLockDesc;

	public GameObject m_objNew;

	public GameObject m_objEventBadge;

	public CanvasGroup m_CGDGSlot;

	public Color m_colNormal;

	public Color m_colHard;

	[Header("보너스")]
	public GameObject m_NKM_UI_OPERATION_EPISODE_BONUS_REWARD_BONUS_TYPE;

	public Image m_Img_NKM_UI_OPERATION_EPISODE_BONUS_REWARD_BONUS_TYPE;

	public GameObject m_NKM_UI_OPERATION_EPISODE_BONUS_REWARD_MAIN_REWARD;

	public NKCUISlot m_NKM_UI_OPERATION_EPISODE_BONUS_REWARD_MAIN_REWARD_SLOT;

	[Header("최초 획득")]
	public GameObject m_objFirstReward;

	public NKCUISlot m_slotFirstReward;

	[Header("메달 보상")]
	public GameObject m_objMedalClear;

	public NKCUISlot m_slotMedalClear;

	[Header("일일 입장 제한")]
	public GameObject m_NKM_UI_OPERATION_EPISODE_ENTER_LIMIT;

	public Text EnterLimit_COUNT_TEXT;

	public GameObject m_NKM_UI_OPERATION_EPISODE_LIST_MISSION_BLACK;

	[Header("언락 재화")]
	public GameObject m_objUnlockItem;

	public NKCUISlot m_slotUnlockItem;

	[Header("클리어 시간")]
	public GameObject m_objClearTime;

	public Text m_lbClearTime;

	private float m_fElapsedTime;

	private IDungeonSlot.OnSelectedItemSlot m_OnSelectedSlot;

	private int m_ActID;

	private int m_StageIndex;

	private string m_StageBattleStrID = "";

	private int m_IndexToAnimateAlpha;

	private NKCAssetInstanceData m_instance;

	public int GetStageIndex()
	{
		return m_StageIndex;
	}

	public bool CheckLock()
	{
		return m_goLock.activeSelf;
	}

	private void OnDestroy()
	{
		NKCAssetResourceManager.CloseInstance(m_instance);
	}

	public static NKCUIEPActDungeonSlot GetNewInstance(Transform parent, IDungeonSlot.OnSelectedItemSlot selectedSlot = null)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_OPERATION", "NKM_UI_OPERATION_EPISODE_DUNGEON_SLOT");
		NKCUIEPActDungeonSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIEPActDungeonSlot>();
		if (component == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUIEpisodeActSlot Prefab null!");
			return null;
		}
		component.m_instance = nKCAssetInstanceData;
		component.SetOnSelectedItemSlot(selectedSlot);
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void SetOnSelectedItemSlot(IDungeonSlot.OnSelectedItemSlot selectedSlot)
	{
		if (selectedSlot != null)
		{
			m_Button.PointerClick.RemoveAllListeners();
			m_OnSelectedSlot = selectedSlot;
			m_Button.PointerClick.AddListener(OnSelectedItemSlotImpl);
		}
	}

	private void OnSelectedItemSlotImpl()
	{
		if (m_NKM_UI_OPERATION_EPISODE_LIST_MISSION_BLACK != null && m_NKM_UI_OPERATION_EPISODE_LIST_MISSION_BLACK.activeSelf)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ENTER_LIMIT_OVER);
		}
		else if (m_OnSelectedSlot != null)
		{
			bool isPlaying = false;
			if (m_objPlaying != null)
			{
				isPlaying = m_objPlaying.activeSelf;
			}
			m_OnSelectedSlot(m_StageIndex, m_StageBattleStrID, isPlaying);
		}
	}

	private void Update()
	{
		if (base.gameObject.activeSelf && m_CGDGSlot != null && m_CGDGSlot.alpha < 1f)
		{
			m_fElapsedTime += Time.deltaTime;
			if ((float)m_IndexToAnimateAlpha * 0.08f < m_fElapsedTime)
			{
				m_CGDGSlot.alpha = Mathf.Min(1f, m_fElapsedTime * 2f - (float)m_IndexToAnimateAlpha * 0.08f);
			}
		}
	}

	public void SetIndexToAnimateAlpha(int index)
	{
		m_IndexToAnimateAlpha = index;
	}

	public void SetData(NKMStageTempletV2 stageTemplet)
	{
		SetData(stageTemplet.ActId, stageTemplet.m_StageIndex, stageTemplet.m_StageBattleStrID, NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in stageTemplet.m_UnlockInfo), stageTemplet.m_Difficulty);
	}

	public void SetData(int actID, int stageIndex, string stageBattleStrID, bool bLock = false, EPISODE_DIFFICULTY difficulty = EPISODE_DIFFICULTY.NORMAL)
	{
		m_fElapsedTime = 0f;
		if (m_CGDGSlot != null)
		{
			m_CGDGSlot.alpha = 0f;
		}
		m_ActID = actID;
		m_StageIndex = stageIndex;
		m_IndexToAnimateAlpha = stageIndex;
		m_StageBattleStrID = stageBattleStrID;
		NKCUtil.SetGameobjectActive(m_goLock, bLock);
		m_Button.enabled = !bLock;
		NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(stageBattleStrID);
		if (bLock)
		{
			NKCUtil.SetLabelText(m_lbLockDesc, NKCUtilString.GetUnlockConditionRequireDesc(nKMStageTempletV));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbLockDesc, "");
		}
		NKCUtil.SetGameobjectActive(m_objDungeonEPNumber, bValue: false);
		NKCUtil.SetGameobjectActive(m_objUnlockItem, bValue: false);
		NKCUtil.SetGameobjectActive(m_objClearTime, bValue: false);
		if (nKMStageTempletV != null)
		{
			bool bValue = true;
			bool flag = false;
			NKMEpisodeTempletV2 episodeTemplet = nKMStageTempletV.EpisodeTemplet;
			if (episodeTemplet != null)
			{
				int count = episodeTemplet.m_DicStage[m_ActID].Count;
				bool flag2 = false;
				if (nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE)
				{
					bValue = false;
				}
				NKCUtil.SetGameobjectActive(m_lbRecommendFightPower, nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_NORMAL || nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_TUTORIAL || nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_KILLCOUNT);
				NKCUtil.SetGameobjectActive(m_objTraining, nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE);
				NKCUtil.SetGameobjectActive(m_imgBoss, nKMStageTempletV.m_STAGE_SUB_TYPE != STAGE_SUB_TYPE.SST_PRACTICE);
				NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_EPISODE_lIST_SLOT_SHADOW_COVER_TITLE, nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_NORMAL || nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_TUTORIAL);
				if (nKMStageTempletV.m_STAGE_TYPE == STAGE_TYPE.ST_WARFARE)
				{
					NKCUtil.SetGameobjectActive(m_goMissionList, bValue: true);
					NKCUtil.SetGameobjectActive(m_goClearOff, bValue: false);
					NKCUtil.SetGameobjectActive(m_goClearOn, bValue: false);
					NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_StageBattleStrID);
					if (nKMWarfareTemplet != null)
					{
						if (episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_MAINSTREAM)
						{
							NKCUtil.SetGameobjectActive(m_objDungeonEPNumber, bValue: true);
							NKCUtil.SetLabelText(m_lbDungeonEPNumber, episodeTemplet.GetEpisodeTitle());
							NKCUtil.SetLabelText(m_lbDungeonNumber, m_ActID + "-" + nKMStageTempletV.m_StageUINum);
						}
						else if (episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_DAILY)
						{
							NKCUtil.SetLabelText(m_lbDungeonNumber, "<size=40>" + NKCUtilString.GetDailyDungeonLVDesc(nKMStageTempletV.m_StageUINum) + "</size>");
						}
						else
						{
							NKCUtil.SetLabelText(m_lbDungeonNumber, m_ActID + "-" + nKMStageTempletV.m_StageUINum);
						}
						NKCUtil.SetLabelText(m_lbDungeonName, nKMWarfareTemplet.GetWarfareName());
						NKCUtil.SetLabelText(m_lbRecommendFightPower, string.Format(NKCUtilString.GET_STRING_ACT_DUNGEON_SLOT_FIGHT_POWER_DESC, nKMWarfareTemplet.m_WarfareLevel.ToString()));
						if (m_imgBoss != null)
						{
							Sprite sprite = null;
							sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_UNIT", "AB_INVEN_ICON_" + nKMWarfareTemplet.m_WarfareIcon);
							if (sprite != null)
							{
								NKCUtil.SetImageSprite(m_imgBoss, sprite);
							}
							else
							{
								NKCAssetResourceData nKCAssetResourceData = null;
								nKCAssetResourceData = NKCResourceUtility.GetAssetResourceUnitInvenIconEmpty();
								if (nKCAssetResourceData != null)
								{
									NKCUtil.SetImageSprite(m_imgBoss, nKCAssetResourceData.GetAsset<Sprite>());
								}
							}
						}
						NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
						if (myUserData != null)
						{
							NKMWarfareClearData warfareClearData = myUserData.GetWarfareClearData(nKMWarfareTemplet.m_WarfareID);
							bool bValue2 = true;
							bool flag3 = nKMWarfareTemplet.m_WFMissionType_1 != WARFARE_GAME_MISSION_TYPE.WFMT_NONE;
							bool flag4 = nKMWarfareTemplet.m_WFMissionType_2 != WARFARE_GAME_MISSION_TYPE.WFMT_NONE;
							if (warfareClearData != null)
							{
								if (!m_goMission1.activeSelf)
								{
									m_goMission1.SetActive(value: true);
								}
								if (m_goMission2.activeSelf == !warfareClearData.m_mission_result_1)
								{
									m_goMission2.SetActive(warfareClearData.m_mission_result_1);
								}
								if (m_goMission3.activeSelf == !warfareClearData.m_mission_result_2)
								{
									m_goMission3.SetActive(warfareClearData.m_mission_result_2);
								}
								if (count == m_StageIndex)
								{
									flag2 = true;
								}
								NKCUtil.SetGameobjectActive(m_goMission1_BG, bValue: false);
								NKCUtil.SetGameobjectActive(m_goMission2_BG, flag3 && !warfareClearData.m_mission_result_1);
								NKCUtil.SetGameobjectActive(m_goMission3_BG, flag4 && !warfareClearData.m_mission_result_2);
							}
							else
							{
								NKCUtil.SetGameobjectActive(m_goMission1_BG, bValue2);
								NKCUtil.SetGameobjectActive(m_goMission2_BG, flag3);
								NKCUtil.SetGameobjectActive(m_goMission3_BG, flag4);
								if (m_goMission1.activeSelf)
								{
									m_goMission1.SetActive(value: false);
								}
								if (m_goMission2.activeSelf)
								{
									m_goMission2.SetActive(value: false);
								}
								if (m_goMission3.activeSelf)
								{
									m_goMission3.SetActive(value: false);
								}
							}
						}
					}
				}
				else if (nKMStageTempletV.m_STAGE_TYPE == STAGE_TYPE.ST_DUNGEON)
				{
					NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_StageBattleStrID);
					if (dungeonTempletBase != null)
					{
						if (episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_MAINSTREAM)
						{
							NKCUtil.SetGameobjectActive(m_objDungeonEPNumber, bValue: true);
							NKCUtil.SetLabelText(m_lbDungeonEPNumber, episodeTemplet.GetEpisodeTitle());
						}
						if (nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE)
						{
							NKCUtil.SetLabelText(m_lbDungeonNumber, string.Format(NKCUtilString.GET_STRING_EP_TRAINING_NUMBER, nKMStageTempletV.m_StageUINum));
						}
						else if (dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE)
						{
							NKCUtil.SetLabelText(m_lbDungeonNumber, string.Format(NKCUtilString.GET_STRING_EP_CUTSCEN_NUMBER, nKMStageTempletV.m_StageUINum));
						}
						else if (episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_MAINSTREAM)
						{
							NKCUtil.SetLabelText(m_lbDungeonNumber, m_ActID + "-" + nKMStageTempletV.m_StageUINum);
						}
						else if (episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_DAILY)
						{
							NKCUtil.SetLabelText(m_lbDungeonNumber, "<size=40>" + NKCUtilString.GetDailyDungeonLVDesc(nKMStageTempletV.m_StageUINum) + "</size>");
						}
						else
						{
							NKCUtil.SetLabelText(m_lbDungeonNumber, m_ActID + "-" + nKMStageTempletV.m_StageUINum);
						}
						NKCUtil.SetLabelText(m_lbDungeonName, dungeonTempletBase.GetDungeonName());
						if (dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE)
						{
							NKCUtil.SetLabelText(m_lbRecommendFightPower, "");
							bValue = false;
							flag = true;
						}
						else
						{
							NKCUtil.SetLabelText(m_lbRecommendFightPower, string.Format(NKCUtilString.GET_STRING_ACT_DUNGEON_SLOT_FIGHT_POWER_DESC, dungeonTempletBase.m_DungeonLevel.ToString()));
						}
						if (m_imgBoss != null)
						{
							Sprite sprite2 = null;
							sprite2 = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_UNIT", "AB_INVEN_ICON_" + dungeonTempletBase.m_DungeonIcon);
							if (sprite2 != null)
							{
								NKCUtil.SetImageSprite(m_imgBoss, sprite2);
							}
							else
							{
								NKCAssetResourceData nKCAssetResourceData2 = null;
								nKCAssetResourceData2 = NKCResourceUtility.GetAssetResourceUnitInvenIconEmpty();
								if (nKCAssetResourceData2 != null)
								{
									NKCUtil.SetImageSprite(m_imgBoss, nKCAssetResourceData2.GetAsset<Sprite>());
								}
							}
						}
						NKMUserData myUserData2 = NKCScenManager.GetScenManager().GetMyUserData();
						if (myUserData2 != null)
						{
							NKMDungeonClearData dungeonClearData = myUserData2.GetDungeonClearData(dungeonTempletBase.m_DungeonID);
							if (episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_DAILY || dungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE || nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE || nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_TUTORIAL)
							{
								NKCUtil.SetGameobjectActive(m_goMissionList, bValue: false);
								if (dungeonClearData != null)
								{
									if (count == m_StageIndex)
									{
										flag2 = true;
									}
									NKCUtil.SetGameobjectActive(m_goClearOff, bValue: false);
									NKCUtil.SetGameobjectActive(m_goClearOn, bValue: true);
								}
								else
								{
									NKCUtil.SetGameobjectActive(m_goClearOff, bValue: true);
									NKCUtil.SetGameobjectActive(m_goClearOn, bValue: false);
								}
							}
							else
							{
								NKCUtil.SetGameobjectActive(m_goMissionList, bValue: true);
								NKCUtil.SetGameobjectActive(m_goClearOff, bValue: false);
								NKCUtil.SetGameobjectActive(m_goClearOn, dungeonClearData != null && episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_CHALLENGE);
								bool bValue3 = true;
								bool flag5 = dungeonTempletBase.m_DGMissionType_1 != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE;
								bool flag6 = dungeonTempletBase.m_DGMissionType_2 != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE;
								if (dungeonClearData != null)
								{
									if (!m_goMission1.activeSelf)
									{
										m_goMission1.SetActive(value: true);
									}
									if (m_goMission2.activeSelf == !dungeonClearData.missionResult1)
									{
										m_goMission2.SetActive(dungeonClearData.missionResult1);
									}
									if (m_goMission3.activeSelf == !dungeonClearData.missionResult2)
									{
										m_goMission3.SetActive(dungeonClearData.missionResult2);
									}
									if (count == m_StageIndex)
									{
										flag2 = true;
									}
									NKCUtil.SetGameobjectActive(m_goMission1_BG, bValue: false);
									NKCUtil.SetGameobjectActive(m_goMission2_BG, flag5 && !dungeonClearData.missionResult1);
									NKCUtil.SetGameobjectActive(m_goMission3_BG, flag6 && !dungeonClearData.missionResult2);
								}
								else
								{
									if (m_goMission1.activeSelf)
									{
										m_goMission1.SetActive(value: false);
									}
									if (m_goMission2.activeSelf)
									{
										m_goMission2.SetActive(value: false);
									}
									if (m_goMission3.activeSelf)
									{
										m_goMission3.SetActive(value: false);
									}
									NKCUtil.SetGameobjectActive(m_goMission1_BG, bValue3);
									NKCUtil.SetGameobjectActive(m_goMission2_BG, flag5);
									NKCUtil.SetGameobjectActive(m_goMission3_BG, flag6);
								}
							}
						}
					}
				}
				else if (nKMStageTempletV.m_STAGE_TYPE == STAGE_TYPE.ST_PHASE)
				{
					NKMPhaseTemplet nKMPhaseTemplet = NKMPhaseTemplet.Find(m_StageBattleStrID);
					if (nKMPhaseTemplet != null)
					{
						if (episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_MAINSTREAM)
						{
							NKCUtil.SetGameobjectActive(m_objDungeonEPNumber, bValue: true);
							NKCUtil.SetLabelText(m_lbDungeonEPNumber, episodeTemplet.GetEpisodeTitle());
						}
						if (nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE)
						{
							NKCUtil.SetLabelText(m_lbDungeonNumber, string.Format(NKCUtilString.GET_STRING_EP_TRAINING_NUMBER, nKMStageTempletV.m_StageUINum));
						}
						else if (episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_MAINSTREAM)
						{
							NKCUtil.SetLabelText(m_lbDungeonNumber, m_ActID + "-" + nKMStageTempletV.m_StageUINum);
						}
						else if (episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_DAILY)
						{
							NKCUtil.SetLabelText(m_lbDungeonNumber, "<size=40>" + NKCUtilString.GetDailyDungeonLVDesc(nKMStageTempletV.m_StageUINum) + "</size>");
						}
						else
						{
							NKCUtil.SetLabelText(m_lbDungeonNumber, m_ActID + "-" + nKMStageTempletV.m_StageUINum);
						}
						NKCUtil.SetLabelText(m_lbDungeonName, nKMPhaseTemplet.GetName());
						NKCUtil.SetLabelText(m_lbRecommendFightPower, string.Format(NKCUtilString.GET_STRING_ACT_DUNGEON_SLOT_FIGHT_POWER_DESC, nKMPhaseTemplet.PhaseLevel.ToString()));
						if (m_imgBoss != null)
						{
							Sprite sprite3 = null;
							sprite3 = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_UNIT", "AB_INVEN_ICON_" + nKMPhaseTemplet.Icon);
							if (sprite3 != null)
							{
								NKCUtil.SetImageSprite(m_imgBoss, sprite3);
							}
							else
							{
								NKCAssetResourceData nKCAssetResourceData3 = null;
								nKCAssetResourceData3 = NKCResourceUtility.GetAssetResourceUnitInvenIconEmpty();
								if (nKCAssetResourceData3 != null)
								{
									NKCUtil.SetImageSprite(m_imgBoss, nKCAssetResourceData3.GetAsset<Sprite>());
								}
							}
						}
						if (NKCScenManager.GetScenManager().GetMyUserData() != null)
						{
							NKMPhaseClearData phaseClearData = NKCPhaseManager.GetPhaseClearData(nKMPhaseTemplet);
							if (episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_DAILY || nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE || nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_TUTORIAL)
							{
								NKCUtil.SetGameobjectActive(m_goMissionList, bValue: false);
								if (phaseClearData != null)
								{
									if (count == m_StageIndex)
									{
										flag2 = true;
									}
									NKCUtil.SetGameobjectActive(m_goClearOff, bValue: false);
									NKCUtil.SetGameobjectActive(m_goClearOn, bValue: true);
								}
								else
								{
									NKCUtil.SetGameobjectActive(m_goClearOff, bValue: true);
									NKCUtil.SetGameobjectActive(m_goClearOn, bValue: false);
								}
							}
							else
							{
								NKCUtil.SetGameobjectActive(m_goMissionList, bValue: true);
								NKCUtil.SetGameobjectActive(m_goClearOff, bValue: false);
								NKCUtil.SetGameobjectActive(m_goClearOn, phaseClearData != null && episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_CHALLENGE);
								bool bValue4 = true;
								bool flag7 = nKMPhaseTemplet.m_DGMissionType_1 != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE;
								bool flag8 = nKMPhaseTemplet.m_DGMissionType_2 != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE;
								if (phaseClearData != null)
								{
									if (!m_goMission1.activeSelf)
									{
										m_goMission1.SetActive(value: true);
									}
									if (m_goMission2.activeSelf == !phaseClearData.missionResult1)
									{
										m_goMission2.SetActive(phaseClearData.missionResult1);
									}
									if (m_goMission3.activeSelf == !phaseClearData.missionResult2)
									{
										m_goMission3.SetActive(phaseClearData.missionResult2);
									}
									if (count == m_StageIndex)
									{
										flag2 = true;
									}
									NKCUtil.SetGameobjectActive(m_goMission1_BG, bValue: false);
									NKCUtil.SetGameobjectActive(m_goMission2_BG, flag7 && !phaseClearData.missionResult1);
									NKCUtil.SetGameobjectActive(m_goMission3_BG, flag8 && !phaseClearData.missionResult2);
								}
								else
								{
									if (m_goMission1.activeSelf)
									{
										m_goMission1.SetActive(value: false);
									}
									if (m_goMission2.activeSelf)
									{
										m_goMission2.SetActive(value: false);
									}
									if (m_goMission3.activeSelf)
									{
										m_goMission3.SetActive(value: false);
									}
									NKCUtil.SetGameobjectActive(m_goMission1_BG, bValue4);
									NKCUtil.SetGameobjectActive(m_goMission2_BG, flag7);
									NKCUtil.SetGameobjectActive(m_goMission3_BG, flag8);
								}
							}
						}
					}
				}
				UpdateSubUI(nKMStageTempletV);
				NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_EPISODE_BONUS_REWARD_MAIN_REWARD, nKMStageTempletV.MainRewardData != null && nKMStageTempletV.MainRewardData.rewardType != NKM_REWARD_TYPE.RT_NONE);
				NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_EPISODE_lIST_SLOT_LEVEL, bValue);
				NKCUtil.SetGameobjectActive(m_objCutscenBG, flag);
				NKCUtil.SetGameobjectActive(m_objCutscenDungeonBG, !flag2 && (flag || nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_TUTORIAL));
				NKCUtil.SetGameobjectActive(m_objCutscenDungeonBGClear, flag2 && (flag || nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_TUTORIAL));
				NKCUtil.SetGameobjectActive(m_objWarfareBG, !flag);
				NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_EPISODE_lIST_SLOT_BG_SHADOW, !flag2 && nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_NORMAL && !flag);
				NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_EPISODE_lIST_SLOT_BG_SHADOW_CLEAR, flag2 && nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_NORMAL && !flag);
				NKCUtil.SetGameobjectActive(m_objTrainingBG, !flag2 && nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE);
				NKCUtil.SetGameobjectActive(m_objTrainingClearBG, flag2 && nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_PRACTICE);
				Color color = m_colNormal;
				if (difficulty == EPISODE_DIFFICULTY.HARD)
				{
					color = m_colHard;
				}
				NKCUtil.SetImageColor(m_NKM_UI_OPERATION_EPISODE_lIST_SLOT_BG_SHADOW, color);
				if (nKMStageTempletV.NeedToUnlock && !NKMEpisodeMgr.IsUnlockedStage(nKMStageTempletV))
				{
					NKCUtil.SetGameobjectActive(m_goLock, bValue: true);
					if (nKMStageTempletV.EpisodeCategory == EPISODE_CATEGORY.EC_SIDESTORY)
					{
						NKCUtil.SetLabelText(m_lbLockDesc, NKCUtilString.GetSidestoryUnlockRequireDesc(nKMStageTempletV));
					}
					else
					{
						NKCUtil.SetLabelText(m_lbLockDesc, NKCUtilString.GetUnlockConditionRequireDesc(nKMStageTempletV));
					}
					NKCUtil.SetGameobjectActive(m_objUnlockItem, bValue: true);
					m_slotUnlockItem.SetData(NKCUISlot.SlotData.MakeMiscItemData(nKMStageTempletV.UnlockReqItem.ItemId, nKMStageTempletV.UnlockReqItem.Count32));
				}
				NKCUtil.SetGameobjectActive(m_objClearTime, nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_TIMEATTACK);
				if (nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_TIMEATTACK)
				{
					string msg = "-:--:--";
					NKCUtil.SetGameobjectActive(m_objClearTime, bValue: true);
					NKCUtil.SetLabelText(m_lbClearTime, msg);
					if (NKCScenManager.CurrentUserData().IsHaveStatePlayData(nKMStageTempletV.Key) && NKCScenManager.CurrentUserData().GetStageBestClearSec(nKMStageTempletV.Key) > 0)
					{
						msg = NKCUtilString.GetTimeStringFromSeconds(NKCScenManager.CurrentUserData().GetStageBestClearSec(nKMStageTempletV.Key));
						NKCUtil.SetLabelTextColor(m_lbClearTime, NKCUtil.GetColor("#ED173A"));
					}
					else
					{
						NKCUtil.SetLabelTextColor(m_lbClearTime, NKCUtil.GetColor("#FFFFFF"));
					}
					NKCUtil.SetLabelText(m_lbClearTime, msg);
				}
			}
			UpdateINGWarfareDirectGoUI(nKMStageTempletV);
			if (m_goLock != null && !m_goLock.activeSelf)
			{
				NKCUtil.SetGameobjectActive(m_goLock, !nKMStageTempletV.IsOpenedDayOfWeek());
				if (m_goLock.activeSelf)
				{
					NKCUtil.SetLabelText(m_lbLockDesc, NKCUtilString.GET_STRING_DAILY_CHECK_DAY);
					m_Button.enabled = false;
				}
			}
		}
		base.gameObject.SetActive(value: true);
	}

	private void UpdateSubUI(NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet == null)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		if (stageTemplet.EnterLimit <= 0)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_EPISODE_ENTER_LIMIT, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_EPISODE_LIST_MISSION_BLACK, bValue: false);
			NKCUtil.SetLabelTextColor(EnterLimit_COUNT_TEXT, Color.white);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_EPISODE_ENTER_LIMIT, bValue: true);
			if (stageTemplet.EnterLimit - nKMUserData.GetStatePlayCnt(stageTemplet.Key) <= 0)
			{
				NKCUtil.SetLabelTextColor(EnterLimit_COUNT_TEXT, Color.red);
			}
			else
			{
				NKCUtil.SetLabelTextColor(EnterLimit_COUNT_TEXT, Color.white);
			}
			NKCUtil.SetLabelText(EnterLimit_COUNT_TEXT, $"({stageTemplet.EnterLimit - nKMUserData.GetStatePlayCnt(stageTemplet.Key)}/{stageTemplet.EnterLimit})");
			if (nKMUserData.IsHaveStatePlayData(stageTemplet.Key) && nKMUserData.GetStatePlayCnt(stageTemplet.Key) >= stageTemplet.EnterLimit && nKMUserData.GetStageRestoreCnt(stageTemplet.Key) >= stageTemplet.RestoreLimit)
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_EPISODE_LIST_MISSION_BLACK, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_EPISODE_LIST_MISSION_BLACK, bValue: false);
			}
		}
		if (stageTemplet.m_BuffType.Equals(RewardTuningType.None))
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_EPISODE_BONUS_REWARD_BONUS_TYPE, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_EPISODE_BONUS_REWARD_BONUS_TYPE, bValue: true);
			NKCUtil.SetImageSprite(m_Img_NKM_UI_OPERATION_EPISODE_BONUS_REWARD_BONUS_TYPE, NKCUtil.GetBounsTypeIcon(stageTemplet.m_BuffType));
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_EPISODE_BONUS_REWARD_MAIN_REWARD_SLOT, bValue: false);
		bool flag = true;
		if (stageTemplet.MainRewardData != null)
		{
			flag = NKMRewardTemplet.IsOpenedReward(stageTemplet.MainRewardData.rewardType, stageTemplet.MainRewardData.ID, useRandomContract: false);
		}
		if (m_NKM_UI_OPERATION_EPISODE_BONUS_REWARD_MAIN_REWARD_SLOT != null && stageTemplet.MainRewardData != null && stageTemplet.MainRewardData.rewardType != NKM_REWARD_TYPE.RT_NONE && NKCUtil.IsValidReward(stageTemplet.MainRewardData.rewardType, stageTemplet.MainRewardData.ID) && flag)
		{
			NKCUISlot.SlotData slotData = null;
			switch (stageTemplet.MainRewardData.rewardType)
			{
			case NKM_REWARD_TYPE.RT_UNIT:
			case NKM_REWARD_TYPE.RT_SHIP:
			case NKM_REWARD_TYPE.RT_OPERATOR:
				slotData = NKCUISlot.SlotData.MakeUnitData(stageTemplet.MainRewardData.ID, 1);
				break;
			case NKM_REWARD_TYPE.RT_MISC:
				slotData = NKCUISlot.SlotData.MakeMiscItemData(stageTemplet.MainRewardData.ID, stageTemplet.MainRewardData.MaxValue);
				break;
			case NKM_REWARD_TYPE.RT_MOLD:
				slotData = NKCUISlot.SlotData.MakeMoldItemData(stageTemplet.MainRewardData.ID, stageTemplet.MainRewardData.MaxValue);
				break;
			case NKM_REWARD_TYPE.RT_EQUIP:
				slotData = NKCUISlot.SlotData.MakeEquipData(stageTemplet.MainRewardData.ID, stageTemplet.MainRewardData.MinValue);
				break;
			}
			m_NKM_UI_OPERATION_EPISODE_BONUS_REWARD_MAIN_REWARD_SLOT.SetData(slotData);
			NKCUIComButton component = m_NKM_UI_OPERATION_EPISODE_BONUS_REWARD_MAIN_REWARD_SLOT.gameObject.GetComponent<NKCUIComButton>();
			if (component != null)
			{
				component.PointerDown.RemoveAllListeners();
				component.PointerDown.AddListener(delegate(PointerEventData x)
				{
					NKCUITooltip.Instance.Open(slotData, x.position);
				});
			}
			m_NKM_UI_OPERATION_EPISODE_BONUS_REWARD_MAIN_REWARD_SLOT.DisableItemCount();
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_EPISODE_BONUS_REWARD_MAIN_REWARD_SLOT, bValue: true);
		}
		SetFirstReward(nKMUserData, stageTemplet);
		SetMedalClear(nKMUserData, stageTemplet);
	}

	private void SetFirstReward(NKMUserData userData, NKMStageTempletV2 stageTemplet)
	{
		if (m_objFirstReward != null && stageTemplet.GetFirstRewardData() != FirstRewardData.Empty)
		{
			NKCUtil.SetGameobjectActive(m_objFirstReward, bValue: true);
			FirstRewardData firstRewardData = stageTemplet.GetFirstRewardData();
			bool completeMark = NKMEpisodeMgr.CheckClear(userData, stageTemplet);
			if (!(m_slotFirstReward != null) || firstRewardData == null || firstRewardData.Type == NKM_REWARD_TYPE.RT_NONE || firstRewardData.RewardId == 0)
			{
				return;
			}
			NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(firstRewardData.Type, firstRewardData.RewardId, firstRewardData.RewardQuantity);
			m_slotFirstReward.SetData(slotData);
			m_slotFirstReward.SetCompleteMark(completeMark);
			m_slotFirstReward.SetFirstGetMark(bValue: true);
			NKCUIComButton component = m_slotFirstReward.gameObject.GetComponent<NKCUIComButton>();
			if (component != null)
			{
				component.PointerDown.RemoveAllListeners();
				component.PointerDown.AddListener(delegate(PointerEventData x)
				{
					NKCUITooltip.Instance.Open(slotData, x.position);
				});
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objFirstReward, bValue: false);
		}
	}

	private void SetMedalClear(NKMUserData userData, NKMStageTempletV2 stageTemplet)
	{
		if (m_objMedalClear != null && m_slotMedalClear != null && stageTemplet.MissionReward != null && stageTemplet.MissionReward.rewardType != NKM_REWARD_TYPE.RT_NONE && stageTemplet.MissionReward.ID != 0)
		{
			NKCUtil.SetGameobjectActive(m_objMedalClear, bValue: true);
			NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(stageTemplet.MissionReward.rewardType, stageTemplet.MissionReward.ID, stageTemplet.MissionReward.Count);
			m_slotMedalClear.SetData(slotData);
			m_slotMedalClear.SetFirstAllClearMark(bValue: true);
			bool medalAllClear = NKMEpisodeMgr.GetMedalAllClear(userData, stageTemplet);
			m_slotMedalClear.SetCompleteMark(medalAllClear);
			NKCUIComButton component = m_slotMedalClear.gameObject.GetComponent<NKCUIComButton>();
			if (component != null)
			{
				component.PointerDown.RemoveAllListeners();
				component.PointerDown.AddListener(delegate(PointerEventData x)
				{
					NKCUITooltip.Instance.Open(slotData, x.position);
				});
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objMedalClear, bValue: false);
		}
	}

	public void UpdateINGWarfareDirectGoUI(NKMStageTempletV2 _stageTemplet)
	{
		WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
		if (warfareGameData != null && warfareGameData.warfareGameState != NKM_WARFARE_GAME_STATE.NWGS_STOP)
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareGameData.warfareTempletID);
			if (nKMWarfareTemplet != null && NKMEpisodeMgr.FindStageTempletByBattleStrID(nKMWarfareTemplet.m_WarfareStrID).Key == _stageTemplet.Key)
			{
				NKCUtil.SetGameobjectActive(m_objPlaying, bValue: true);
				return;
			}
		}
		NKCUtil.SetGameobjectActive(m_objPlaying, bValue: false);
	}

	public void SetActive(bool bSet)
	{
		if (base.gameObject.activeSelf == !bSet)
		{
			base.gameObject.SetActive(bSet);
		}
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	public void InvokeSelectSlot()
	{
		OnSelectedItemSlotImpl();
	}

	public void SetEnableNewMark(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objNew, bValue);
	}

	public void SetSelectNode(bool bValue)
	{
	}

	public void Close()
	{
		NKCAssetResourceManager.CloseInstance(m_instance);
		m_instance = null;
	}
}
