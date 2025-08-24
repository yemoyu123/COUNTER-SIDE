using System;
using System.Collections.Generic;
using ClientPacket.User;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupAchieveRateReward : NKCUIBase
{
	[Serializable]
	public struct EP_POPUP_PROGRESS
	{
		public GameObject ObjProgress;

		public Text NKM_UI_OPERATION_POPUP_MEDAL_TEXT;

		public Slider NKM_UI_OPERATION_EPISODE_PROGRESSBAR;

		public NKCUISlot[] AB_ICON_SLOT;

		public GameObject[] AB_ICON_SLOT_REWARD_FX;

		public GameObject[] CHECK;

		public RectTransform[] rtMETAL;

		public Image[] METAL;

		public GameObject[] FX;

		public Text[] TEXT;
	}

	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_OPERATION";

	public const string UI_ASSET_NAME = "NKM_UI_OPERATION_POPUP_MEDAL";

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	public Text m_NKM_UI_OPERATION_POPUP_EP;

	public Text m_NKM_UI_OPERATION_POPUP_TITLE;

	public Text m_NKM_UI_OPERATION_POPUP_MEDAL_TEXT;

	public List<GameObject> m_NKM_UI_OPERATION_EPISODE_REWARD;

	public List<NKCUISlot> m_lstRewardSlot;

	public List<GameObject> m_lstRewardFX;

	public List<GameObject> m_lstRewardCheck;

	public List<Text> m_NKM_UI_OPERATION_EPISODE_REWARD_Goal_Count;

	public List<Image> m_lstSmallMedal;

	public List<GameObject> m_lstSmallMedalFX;

	public Slider m_NKM_UI_OPERATION_EPISODE_PROGRESSBAR;

	public NKCUIComButton m_btnOK;

	public NKCUIComButton m_btnCancel;

	public EventTrigger m_eventTriggerBG;

	public GameObject m_NKM_UI_POPUP_OK_CANCEL_BOX_OK_DISABLE;

	private NKMEpisodeTempletV2 m_NKMEpisodeTemplet;

	[Header("클리어 정보")]
	public EP_POPUP_PROGRESS[] m_EpisodePopupProgress;

	public RectTransform m_rtREWARD;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "달성도 보상 팝업";

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		for (int i = 0; i < m_EpisodePopupProgress.Length; i++)
		{
			for (int j = 0; j < m_EpisodePopupProgress[i].AB_ICON_SLOT.Length; j++)
			{
				if (m_EpisodePopupProgress[i].AB_ICON_SLOT[j] != null)
				{
					m_EpisodePopupProgress[i].AB_ICON_SLOT[j].Init();
				}
			}
		}
		m_btnOK.PointerClick.RemoveAllListeners();
		m_btnOK.PointerClick.AddListener(OnClickGetAllReward);
		NKCUtil.SetHotkey(m_btnOK, HotkeyEventType.Confirm);
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(base.Close);
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_eventTriggerBG.triggers.Add(entry);
		base.gameObject.SetActive(value: false);
	}

	public void Open(NKMEpisodeTempletV2 cNKMEpisodeTemplet)
	{
		if (cNKMEpisodeTemplet != null)
		{
			base.gameObject.SetActive(value: true);
			m_NKCUIOpenAnimator.PlayOpenAni();
			SetData(cNKMEpisodeTemplet);
			UIOpened();
		}
	}

	public void OnClickGetAllReward()
	{
		if (m_NKMEpisodeTemplet == null)
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < 3; i++)
		{
			foreach (EPISODE_DIFFICULTY value in Enum.GetValues(typeof(EPISODE_DIFFICULTY)))
			{
				if (NKMEpisodeMgr.CanGetEpisodeCompleteReward(myUserData, m_NKMEpisodeTemplet.m_EpisodeID, value, i) == NKM_ERROR_CODE.NEC_OK)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			NKMPopUpBox.OpenWaitBox();
			NKMPacket_EPISODE_COMPLETE_REWARD_ALL_REQ nKMPacket_EPISODE_COMPLETE_REWARD_ALL_REQ = new NKMPacket_EPISODE_COMPLETE_REWARD_ALL_REQ();
			nKMPacket_EPISODE_COMPLETE_REWARD_ALL_REQ.episodeID = m_NKMEpisodeTemplet.m_EpisodeID;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EPISODE_COMPLETE_REWARD_ALL_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}
	}

	public void ResetUI()
	{
		if (m_NKMEpisodeTemplet != null)
		{
			SetData(m_NKMEpisodeTemplet);
		}
	}

	private void SetData(NKMEpisodeTempletV2 cNKMEpisodeTemplet)
	{
		if (cNKMEpisodeTemplet == null)
		{
			return;
		}
		m_NKMEpisodeTemplet = cNKMEpisodeTemplet;
		m_NKM_UI_OPERATION_POPUP_EP.text = m_NKMEpisodeTemplet.GetEpisodeTitle();
		m_NKM_UI_OPERATION_POPUP_TITLE.text = m_NKMEpisodeTemplet.GetEpisodeName();
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return;
		}
		float width = m_rtREWARD.GetWidth();
		bool flag = false;
		for (int i = 0; i < m_EpisodePopupProgress.Length; i++)
		{
			NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(cNKMEpisodeTemplet.m_EpisodeID, (EPISODE_DIFFICULTY)i);
			if (i > 1)
			{
				Debug.LogError("$추가 난이도가 있는 경우, 추가 작업 해주세요.");
				break;
			}
			if (i == 1)
			{
				NKCUtil.SetGameobjectActive(m_EpisodePopupProgress[i].ObjProgress, nKMEpisodeTempletV != null);
				if (nKMEpisodeTempletV == null)
				{
					continue;
				}
			}
			EPISODE_DIFFICULTY ePISODE_DIFFICULTY = (EPISODE_DIFFICULTY)i;
			NKCUtil.SetLabelText(m_EpisodePopupProgress[i].NKM_UI_OPERATION_POPUP_MEDAL_TEXT, NKMEpisodeMgr.GetEPProgressClearCount(myUserData, nKMEpisodeTempletV).ToString());
			List<int> list = new List<int>();
			for (int j = 0; j < nKMEpisodeTempletV.m_CompletionReward.Length; j++)
			{
				if (nKMEpisodeTempletV.m_CompletionReward[j] != null)
				{
					list.Add(nKMEpisodeTempletV.m_CompletionReward[j].m_CompleteRate);
				}
			}
			if (list.Count == 0)
			{
				NKCUtil.SetGameobjectActive(m_EpisodePopupProgress[i].ObjProgress, bValue: false);
				continue;
			}
			int ePProgressTotalCount = NKMEpisodeMgr.GetEPProgressTotalCount(nKMEpisodeTempletV);
			for (int k = 0; k < m_EpisodePopupProgress[i].TEXT.Length && list.Count > k; k++)
			{
				float num = (float)(ePProgressTotalCount * list[k]) * 0.01f;
				NKCUtil.SetLabelText(m_EpisodePopupProgress[i].TEXT[k], ((int)num).ToString());
			}
			float ePProgressPercent = NKMEpisodeMgr.GetEPProgressPercent(myUserData, nKMEpisodeTempletV);
			m_EpisodePopupProgress[i].NKM_UI_OPERATION_EPISODE_PROGRESSBAR.value = ePProgressPercent;
			NKC_EP_ACHIEVE_RATE nKC_EP_ACHIEVE_RATE = ((list.Count > 2 && ePProgressPercent >= (float)list[2] * 0.01f) ? NKC_EP_ACHIEVE_RATE.AR_CLEAR_STEP_3 : ((list.Count > 1 && ePProgressPercent >= (float)list[1] * 0.01f) ? NKC_EP_ACHIEVE_RATE.AR_CLEAR_STEP_2 : ((list.Count <= 0 || !(ePProgressPercent >= (float)list[0] * 0.01f)) ? NKC_EP_ACHIEVE_RATE.AR_NONE : NKC_EP_ACHIEVE_RATE.AR_CLEAR_STEP_1)));
			for (int l = 0; l < 3 && l < list.Count; l++)
			{
				if (l <= (int)nKC_EP_ACHIEVE_RATE)
				{
					switch (ePISODE_DIFFICULTY)
					{
					case EPISODE_DIFFICULTY.NORMAL:
						NKCUtil.SetImageColor(m_EpisodePopupProgress[i].METAL[l], NKCUtil.GetColor("#00D8FF"));
						break;
					case EPISODE_DIFFICULTY.HARD:
						NKCUtil.SetImageColor(m_EpisodePopupProgress[i].METAL[l], NKCUtil.GetColor("#FFDE00"));
						break;
					}
					NKCUtil.SetGameobjectActive(m_EpisodePopupProgress[i].FX[l], bValue: true);
				}
				else
				{
					NKCUtil.SetImageColor(m_EpisodePopupProgress[i].METAL[l], NKCUtil.GetColor("#FFFFFF"));
					NKCUtil.SetGameobjectActive(m_EpisodePopupProgress[i].FX[l], bValue: false);
				}
			}
			NKMEpisodeCompleteData episodeCompleteData = myUserData.GetEpisodeCompleteData(nKMEpisodeTempletV.m_EpisodeID, ePISODE_DIFFICULTY);
			for (int m = 0; m < 3; m++)
			{
				if (m >= list.Count)
				{
					NKCUtil.SetGameobjectActive(m_EpisodePopupProgress[i].rtMETAL[m].gameObject, bValue: false);
					continue;
				}
				NKCUtil.SetGameobjectActive(m_EpisodePopupProgress[i].rtMETAL[m].gameObject, bValue: true);
				float x = width * (float)list[m] * 0.01f;
				m_EpisodePopupProgress[i].rtMETAL[m].anchoredPosition = new Vector2(x, m_EpisodePopupProgress[i].rtMETAL[m].anchoredPosition.y);
				NKCUISlot.OnClick onClick = null;
				NKMRewardInfo rewardInfo = nKMEpisodeTempletV.m_CompletionReward[m].m_RewardInfo;
				if (NKMEpisodeMgr.CanGetEpisodeCompleteReward(myUserData, nKMEpisodeTempletV.m_EpisodeID, ePISODE_DIFFICULTY, m) == NKM_ERROR_CODE.NEC_OK)
				{
					switch (m)
					{
					case 0:
						switch (i)
						{
						case 0:
							onClick = OnClickIndex0;
							break;
						case 1:
							onClick = OnClickIndex3;
							break;
						}
						break;
					case 1:
						switch (i)
						{
						case 0:
							onClick = OnClickIndex1;
							break;
						case 1:
							onClick = OnClickIndex4;
							break;
						}
						break;
					case 2:
						switch (i)
						{
						case 0:
							onClick = OnClickIndex2;
							break;
						case 1:
							onClick = OnClickIndex5;
							break;
						}
						break;
					}
				}
				if (episodeCompleteData != null)
				{
					NKCUtil.SetGameobjectActive(m_EpisodePopupProgress[i].CHECK[m], episodeCompleteData.m_bRewards[m]);
					if (NKMEpisodeMgr.CanGetEpisodeCompleteReward(myUserData, nKMEpisodeTempletV.m_EpisodeID, ePISODE_DIFFICULTY, m) == NKM_ERROR_CODE.NEC_OK)
					{
						NKCUtil.SetGameobjectActive(m_EpisodePopupProgress[i].AB_ICON_SLOT_REWARD_FX[m], !episodeCompleteData.m_bRewards[m]);
						m_EpisodePopupProgress[i].AB_ICON_SLOT[m].SetDisable(disable: false);
						if (!flag)
						{
							flag = true;
						}
					}
					else
					{
						m_EpisodePopupProgress[i].AB_ICON_SLOT[m].SetDisable(disable: true);
						NKCUtil.SetGameobjectActive(m_EpisodePopupProgress[i].AB_ICON_SLOT_REWARD_FX[m], bValue: false);
					}
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_EpisodePopupProgress[i].CHECK[m], bValue: false);
					NKCUtil.SetGameobjectActive(m_EpisodePopupProgress[i].AB_ICON_SLOT_REWARD_FX[m], bValue: false);
					m_EpisodePopupProgress[i].AB_ICON_SLOT[m].SetDisable(disable: true);
				}
				m_EpisodePopupProgress[i].AB_ICON_SLOT[m].SetData(NKCUISlot.SlotData.MakeRewardTypeData(rewardInfo), bEnableLayoutElement: true, onClick);
				NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATION_EPISODE_REWARD[m], bValue: true);
			}
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_OK_CANCEL_BOX_OK_DISABLE, !flag);
	}

	private void Send_NKMPacket_EPISODE_COMPLETE_REWARD_REQ(int episodeDifficulty, int index)
	{
		if (m_NKMEpisodeTemplet != null)
		{
			NKMPacket_EPISODE_COMPLETE_REWARD_REQ nKMPacket_EPISODE_COMPLETE_REWARD_REQ = new NKMPacket_EPISODE_COMPLETE_REWARD_REQ();
			nKMPacket_EPISODE_COMPLETE_REWARD_REQ.episodeID = m_NKMEpisodeTemplet.m_EpisodeID;
			nKMPacket_EPISODE_COMPLETE_REWARD_REQ.episodeDifficulty = episodeDifficulty;
			nKMPacket_EPISODE_COMPLETE_REWARD_REQ.rewardIndex = (sbyte)index;
			NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EPISODE_COMPLETE_REWARD_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
		}
	}

	public void OnClickIndex0(NKCUISlot.SlotData slotData, bool bLocked)
	{
		Send_NKMPacket_EPISODE_COMPLETE_REWARD_REQ(0, 0);
	}

	public void OnClickIndex1(NKCUISlot.SlotData slotData, bool bLocked)
	{
		Send_NKMPacket_EPISODE_COMPLETE_REWARD_REQ(0, 1);
	}

	public void OnClickIndex2(NKCUISlot.SlotData slotData, bool bLocked)
	{
		Send_NKMPacket_EPISODE_COMPLETE_REWARD_REQ(0, 2);
	}

	public void OnClickIndex3(NKCUISlot.SlotData slotData, bool bLocked)
	{
		Send_NKMPacket_EPISODE_COMPLETE_REWARD_REQ(1, 0);
	}

	public void OnClickIndex4(NKCUISlot.SlotData slotData, bool bLocked)
	{
		Send_NKMPacket_EPISODE_COMPLETE_REWARD_REQ(1, 1);
	}

	public void OnClickIndex5(NKCUISlot.SlotData slotData, bool bLocked)
	{
		Send_NKMPacket_EPISODE_COMPLETE_REWARD_REQ(1, 2);
	}

	public void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public void CloseAchieveRateRewardPopup()
	{
		Close();
	}

	public void OnCloseBtn()
	{
		Close();
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}
}
