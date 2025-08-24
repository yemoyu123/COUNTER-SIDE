using System;
using Cs.Logging;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIEpisodeActSlot : MonoBehaviour
{
	public delegate void OnSelectedItemSlot(bool bSet, int actID, NKMEpisodeTempletV2 cNKMEpisodeTemplet, EPISODE_DIFFICULTY difficulty);

	[Header("Act")]
	public Text m_lbActNameOn;

	public Text m_lbActNameOff;

	public GameObject m_NKM_UI_LOPERATION_ACT_NUMBER_ACT_ON;

	public GameObject m_NKM_UI_LOPERATION_ACT_NUMBER_ACT_OFF;

	public Text m_NKM_UI_LOPERATION_ACT_NUMBER_SUB_TEXT_ON;

	public Text m_NKM_UI_LOPERATION_ACT_NUMBER_SUB_TEXT_OFF;

	public Text m_lbTitleOn;

	public Text m_lbTitleOff;

	public Text m_lbTitleLock;

	public Text m_lbSubTitleOn;

	public Text m_lbSubTitleOff;

	public Text m_lbSubTitleLock;

	public GameObject m_goEmblemOn;

	public GameObject m_goEmblemOff;

	public Image m_imgEmblemOn;

	public Image m_imgEmblemOff;

	public GameObject m_objLock;

	public Text m_lbLock;

	public Text m_lbLockWithTime;

	public NKCUIComToggle m_ToggleButton;

	public GameObject m_objNew;

	public GameObject m_objEventBadge;

	private NKCUIComToggleGroup m_ToggleGroup;

	private OnSelectedItemSlot m_OnSelectedSlot;

	private int m_ActID;

	private NKMEpisodeTempletV2 m_cNKMEpisodeTemplet;

	private EPISODE_DIFFICULTY m_diffculty;

	private bool m_bUseLockEndTime;

	private DateTime m_lockEndTimeUtc = DateTime.MinValue;

	private NKCAssetInstanceData m_instance;

	private float m_deltaTime;

	private void OnDestroy()
	{
		NKCAssetResourceManager.CloseInstance(m_instance);
	}

	public static NKCUIEpisodeActSlot GetNewInstance(Transform parent, OnSelectedItemSlot selectedSlot = null)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_OPERATION", "NKM_UI_OPERATION_EPISODE_MENU_BUTTON");
		NKCUIEpisodeActSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIEpisodeActSlot>();
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
		component.gameObject.SetActive(value: false);
		return component;
	}

	public NKMEpisodeTempletV2 GetNKMEpisodeTemplet()
	{
		return m_cNKMEpisodeTemplet;
	}

	public int GetActIndex()
	{
		return m_ActID;
	}

	public void SetOnSelectedItemSlot(OnSelectedItemSlot selectedSlot)
	{
		if (selectedSlot != null)
		{
			m_ToggleButton.m_bGetCallbackWhileLocked = true;
			m_ToggleButton.OnValueChanged.RemoveAllListeners();
			m_OnSelectedSlot = selectedSlot;
			m_ToggleButton.OnValueChanged.AddListener(OnSelectedItemSlotImpl);
		}
	}

	private void OnSelectedItemSlotImpl(bool bSet)
	{
		if (!CheckContentsUnlocked())
		{
			ShowLockedMessage();
			m_ToggleButton.Select(bSelect: false, bForce: true, bImmediate: true);
		}
		else if (m_OnSelectedSlot != null && m_ToggleButton.m_ToggleGroup != null)
		{
			m_OnSelectedSlot(bSet, m_ActID, m_cNKMEpisodeTemplet, m_diffculty);
		}
	}

	public void SetData(NKMEpisodeTempletV2 cNKMEpisodeTemplet, int actID, EPISODE_DIFFICULTY difficulty, NKCUIComToggleGroup toggleGroup, bool bLock = false, bool bEPSlot = false)
	{
		m_ActID = actID;
		m_cNKMEpisodeTemplet = cNKMEpisodeTemplet;
		m_diffculty = difficulty;
		m_ToggleGroup = toggleGroup;
		NKMStageTempletV2 firstStageTemplet = NKCContentManager.GetFirstStageTemplet(m_cNKMEpisodeTemplet, m_ActID, m_diffculty);
		if (firstStageTemplet != null)
		{
			bLock = bLock || !NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in firstStageTemplet.m_UnlockInfo) || !firstStageTemplet.EnableByTag;
		}
		if (!bLock)
		{
			m_ToggleButton?.SetToggleGroup(toggleGroup);
			NKCUtil.SetGameobjectActive(m_goEmblemOff, bEPSlot);
			NKCUtil.SetGameobjectActive(m_goEmblemOn, bEPSlot);
			NKCUtil.SetGameobjectActive(m_NKM_UI_LOPERATION_ACT_NUMBER_ACT_ON, !bEPSlot);
			NKCUtil.SetGameobjectActive(m_NKM_UI_LOPERATION_ACT_NUMBER_ACT_OFF, !bEPSlot);
			if (!bEPSlot)
			{
				m_lbActNameOn.text = actID.ToString();
				m_lbActNameOff.text = m_lbActNameOn.text;
				m_lbTitleOn.text = "";
				m_lbTitleOff.text = "";
				m_lbTitleLock.text = "";
				m_lbSubTitleOn.text = "";
				m_lbSubTitleOff.text = "";
				m_lbSubTitleLock.text = "";
				switch (m_cNKMEpisodeTemplet.m_EPCategory)
				{
				case EPISODE_CATEGORY.EC_SIDESTORY:
					m_NKM_UI_LOPERATION_ACT_NUMBER_SUB_TEXT_ON.text = NKCUtilString.GET_STRING_SIDE_STORY;
					m_NKM_UI_LOPERATION_ACT_NUMBER_SUB_TEXT_OFF.text = NKCUtilString.GET_STRING_SIDE_STORY;
					break;
				case EPISODE_CATEGORY.EC_FIELD:
					m_NKM_UI_LOPERATION_ACT_NUMBER_SUB_TEXT_ON.text = NKCUtilString.GET_STRING_FREE_ORDER;
					m_NKM_UI_LOPERATION_ACT_NUMBER_SUB_TEXT_OFF.text = NKCUtilString.GET_STRING_FREE_ORDER;
					break;
				case EPISODE_CATEGORY.EC_EVENT:
					m_NKM_UI_LOPERATION_ACT_NUMBER_SUB_TEXT_ON.text = NKCUtilString.GET_STRING_EVENT;
					m_NKM_UI_LOPERATION_ACT_NUMBER_SUB_TEXT_OFF.text = NKCUtilString.GET_STRING_EVENT;
					break;
				case EPISODE_CATEGORY.EC_CHALLENGE:
					m_NKM_UI_LOPERATION_ACT_NUMBER_SUB_TEXT_ON.text = NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_CHALLENGE");
					m_NKM_UI_LOPERATION_ACT_NUMBER_SUB_TEXT_OFF.text = NKCStringTable.GetString("SI_DP_EPISODE_CATEGORY_EC_CHALLENGE");
					break;
				default:
					m_NKM_UI_LOPERATION_ACT_NUMBER_SUB_TEXT_ON.text = NKCUtilString.GET_STRING_MAIN_STREAM;
					m_NKM_UI_LOPERATION_ACT_NUMBER_SUB_TEXT_OFF.text = NKCUtilString.GET_STRING_MAIN_STREAM;
					break;
				}
			}
			else
			{
				m_lbActNameOn.text = "";
				m_lbActNameOff.text = m_lbActNameOn.text;
				m_lbTitleOn.text = cNKMEpisodeTemplet.GetEpisodeName();
				m_lbTitleOff.text = cNKMEpisodeTemplet.GetEpisodeName();
				m_lbTitleLock.text = cNKMEpisodeTemplet.GetEpisodeName();
				m_lbSubTitleOn.text = cNKMEpisodeTemplet.GetEpisodeTitle();
				m_lbSubTitleOff.text = cNKMEpisodeTemplet.GetEpisodeTitle();
				m_lbSubTitleLock.text = cNKMEpisodeTemplet.GetEpisodeTitle();
				m_NKM_UI_LOPERATION_ACT_NUMBER_SUB_TEXT_ON.text = "";
				m_NKM_UI_LOPERATION_ACT_NUMBER_SUB_TEXT_OFF.text = "";
				switch (cNKMEpisodeTemplet.m_EpisodeID)
				{
				case 101:
					if (!NKCContentManager.IsContentsUnlocked(ContentsType.DAILY_ATTACK))
					{
						m_ToggleButton.Lock();
					}
					else
					{
						m_ToggleButton.UnLock();
					}
					m_imgEmblemOff.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", "NKM_UI_OPERATION_DAILYMISSION_EMBLEM_01");
					m_imgEmblemOn.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", "NKM_UI_OPERATION_DAILYMISSION_EMBLEM_01_SELECT");
					break;
				case 103:
					if (!NKCContentManager.IsContentsUnlocked(ContentsType.DAILY_DEFENCE))
					{
						m_ToggleButton.Lock();
					}
					else
					{
						m_ToggleButton.UnLock();
					}
					m_imgEmblemOff.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", "NKM_UI_OPERATION_DAILYMISSION_EMBLEM_03");
					m_imgEmblemOn.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", "NKM_UI_OPERATION_DAILYMISSION_EMBLEM_03_SELECT");
					break;
				case 102:
					if (!NKCContentManager.IsContentsUnlocked(ContentsType.DAILY_SEARCH))
					{
						m_ToggleButton.Lock();
					}
					else
					{
						m_ToggleButton.UnLock();
					}
					m_imgEmblemOff.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", "NKM_UI_OPERATION_DAILYMISSION_EMBLEM_04");
					m_imgEmblemOn.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", "NKM_UI_OPERATION_DAILYMISSION_EMBLEM_04_SELECT");
					break;
				default:
					if (cNKMEpisodeTemplet.m_EPCategory != EPISODE_CATEGORY.EC_SUPPLY)
					{
						Log.Error("에피소드슬롯 쓰는게 맞는지 카테고리/아이디 확인 필요함", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIEpisodeActSlot.cs", 246);
						break;
					}
					NKCUtil.SetImageSprite(m_imgEmblemOff, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", cNKMEpisodeTemplet.m_DefaultSubTabIcon));
					NKCUtil.SetImageSprite(m_imgEmblemOn, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", cNKMEpisodeTemplet.m_SelectSubTabIcon));
					break;
				}
			}
			if (!bEPSlot)
			{
				bool flag = false;
				flag = CheckExistClearMission(cNKMEpisodeTemplet, actID);
				if (m_objNew.activeSelf == flag)
				{
					m_objNew.SetActive(!flag);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objNew, bValue: false);
			}
		}
		else
		{
			if (m_ToggleButton != null)
			{
				m_ToggleButton.SetToggleGroup(null);
				m_ToggleButton.Select(bSelect: false, bForce: true);
			}
			if (m_objNew.activeSelf)
			{
				m_objNew.SetActive(value: false);
			}
		}
		if (m_objLock.activeSelf == !bLock)
		{
			m_objLock.SetActive(bLock);
		}
		m_bUseLockEndTime = false;
		NKCUtil.SetGameobjectActive(m_lbLock, bValue: true);
		NKCUtil.SetGameobjectActive(m_lbLockWithTime, bValue: false);
		if (firstStageTemplet != null && !NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in firstStageTemplet.m_UnlockInfo))
		{
			if (!NKMContentUnlockManager.IsStarted(firstStageTemplet.m_UnlockInfo))
			{
				m_lockEndTimeUtc = NKMContentUnlockManager.GetConditionStartTime(firstStageTemplet.m_UnlockInfo);
				NKCUtil.SetGameobjectActive(m_lbLock, bValue: false);
				NKCUtil.SetGameobjectActive(m_lbLockWithTime, bValue: true);
				m_bUseLockEndTime = true;
				SetLockText(m_lockEndTimeUtc);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lbLock, bValue: true);
				NKCUtil.SetGameobjectActive(m_lbLockWithTime, bValue: false);
				m_bUseLockEndTime = false;
			}
		}
		base.gameObject.SetActive(value: true);
	}

	private void Update()
	{
		if (m_bUseLockEndTime)
		{
			m_deltaTime += Time.deltaTime;
			if (m_deltaTime > 1f)
			{
				m_deltaTime = 0f;
				SetLockText(m_lockEndTimeUtc);
			}
		}
	}

	public void SetLockText(DateTime lockEndTimeUtc)
	{
		if (lockEndTimeUtc < NKCSynchronizedTime.GetServerUTCTime())
		{
			SetData(m_cNKMEpisodeTemplet, m_ActID, m_diffculty, m_ToggleGroup);
			return;
		}
		if (NKCSynchronizedTime.GetTimeLeft(lockEndTimeUtc).TotalSeconds < 1.0)
		{
			NKCUtil.SetLabelText(m_lbLockWithTime, NKCUtilString.GET_STRING_QUIT);
			return;
		}
		string remainTimeString = NKCUtilString.GetRemainTimeString(lockEndTimeUtc, 2);
		NKCUtil.SetLabelText(m_lbLockWithTime, string.Format(NKCUtilString.GET_STRING_SHOP_CHAIN_NEXT_RESET_ONE_PARAM_CLOSE, remainTimeString));
	}

	private bool CheckExistClearMission(NKMEpisodeTempletV2 cNKMEpisodeTemplet, int actID)
	{
		if (!cNKMEpisodeTemplet.m_DicStage.ContainsKey(actID))
		{
			return false;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		for (int i = 0; i < cNKMEpisodeTemplet.m_DicStage[actID].Count; i++)
		{
			NKMStageTempletV2 nKMStageTempletV = cNKMEpisodeTemplet.m_DicStage[actID][i];
			if (nKMStageTempletV == null)
			{
				continue;
			}
			switch (nKMStageTempletV.m_STAGE_TYPE)
			{
			case STAGE_TYPE.ST_WARFARE:
				if (myUserData.CheckWarfareClear(nKMStageTempletV.m_StageBattleStrID))
				{
					return true;
				}
				break;
			case STAGE_TYPE.ST_DUNGEON:
				if (myUserData.CheckDungeonClear(nKMStageTempletV.m_StageBattleStrID))
				{
					return true;
				}
				break;
			case STAGE_TYPE.ST_PHASE:
				if (NKCPhaseManager.CheckPhaseStageClear(nKMStageTempletV))
				{
					return true;
				}
				break;
			}
		}
		return false;
	}

	private bool CheckContentsUnlocked()
	{
		if (m_cNKMEpisodeTemplet.m_EpisodeID == 101)
		{
			return NKCContentManager.IsContentsUnlocked(ContentsType.DAILY_ATTACK);
		}
		if (m_cNKMEpisodeTemplet.m_EpisodeID == 103)
		{
			return NKCContentManager.IsContentsUnlocked(ContentsType.DAILY_DEFENCE);
		}
		if (m_cNKMEpisodeTemplet.m_EpisodeID == 102)
		{
			return NKCContentManager.IsContentsUnlocked(ContentsType.DAILY_SEARCH);
		}
		NKMStageTempletV2 firstStageTemplet = NKCContentManager.GetFirstStageTemplet(m_cNKMEpisodeTemplet, m_ActID, m_diffculty);
		if (firstStageTemplet != null)
		{
			if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in firstStageTemplet.m_UnlockInfo))
			{
				return false;
			}
			if (!firstStageTemplet.EnableByTag)
			{
				return false;
			}
		}
		return true;
	}

	private void ShowLockedMessage()
	{
		if (m_cNKMEpisodeTemplet.m_EpisodeID == 101)
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.DAILY_ATTACK);
		}
		if (m_cNKMEpisodeTemplet.m_EpisodeID == 103)
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.DAILY_DEFENCE);
		}
		if (m_cNKMEpisodeTemplet.m_EpisodeID == 102)
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.DAILY_SEARCH);
		}
		NKMStageTempletV2 firstStageTemplet = NKCContentManager.GetFirstStageTemplet(m_cNKMEpisodeTemplet, m_ActID, m_diffculty);
		if (firstStageTemplet != null)
		{
			if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in firstStageTemplet.m_UnlockInfo))
			{
				NKCPopupMessageManager.AddPopupMessage(NKCContentManager.MakeUnlockConditionString(in firstStageTemplet.m_UnlockInfo, bSimple: true));
			}
			else if (!firstStageTemplet.EnableByTag)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_DP_UNLOCK_CONDITION_REQUIRE_DESC_SURT_ALWAYS_LOCKED"));
			}
		}
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

	public void Close()
	{
		NKCAssetResourceManager.CloseInstance(m_instance);
		m_instance = null;
	}
}
