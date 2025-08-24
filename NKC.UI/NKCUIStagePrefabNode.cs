using NKC.UI.Tooltip;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIStagePrefabNode : MonoBehaviour
{
	[Header("공용")]
	public Text m_lbStageNum;

	public NKCUIComStateButton m_btn;

	public GameObject m_objLock;

	public GameObject m_objNew;

	public GameObject m_objSelected;

	[Header("주요 보상")]
	[Header("사용할 오브젝트만 링크")]
	public GameObject m_objMainReward;

	public NKCUISlot m_RewardSlot;

	public Image m_imgRewardIcon;

	[Header("최초 클리어 보상")]
	public GameObject m_objFirstReward;

	public NKCUISlot m_FirstRewardSlot;

	[Header("3메달 보상")]
	public GameObject m_objMedalReward;

	public NKCUISlot m_MedalRewardSlot;

	[Header("메달")]
	public NKCUIComMedal m_Medal;

	[Header("보스초상화")]
	public Image m_imgBoss;

	[Header("레벨")]
	public Text m_lbLevel;

	[Header("클리어 체크표시")]
	public GameObject m_objClear;

	[Header("입장 횟수 제한")]
	public GameObject m_objPlayLimit;

	public Text m_lbPlayLimit;

	[Header("이벤트 드롭")]
	public GameObject m_objEventDrop;

	private IDungeonSlot.OnSelectedItemSlot m_OnSelectedSlot;

	private int m_StageIndex;

	private string m_StageBattleStrID;

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	public bool CheckLock()
	{
		return m_objLock.activeSelf;
	}

	public int GetStageIndex()
	{
		return m_StageIndex;
	}

	public void SetData(NKMStageTempletV2 stageTemplet, IDungeonSlot.OnSelectedItemSlot selectedSlot = null)
	{
		if (stageTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_StageIndex = stageTemplet.m_StageIndex;
		m_StageBattleStrID = stageTemplet.m_StageBattleStrID;
		SetOnSelectedItemSlot(selectedSlot);
		NKCUtil.SetGameobjectActive(m_objSelected, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNew, bValue: false);
		NKMUserData cNKMUserData = NKCScenManager.CurrentUserData();
		NKCUtil.SetLabelText(m_lbStageNum, NKCUtilString.GetEpisodeNumber(stageTemplet.EpisodeTemplet, stageTemplet));
		NKCUtil.SetGameobjectActive(m_objLock, !NKMContentUnlockManager.IsContentUnlocked(cNKMUserData, in stageTemplet.m_UnlockInfo));
		SetMainReward(cNKMUserData, stageTemplet);
		SetFirstReward(cNKMUserData, stageTemplet);
		SetMedalReward(cNKMUserData, stageTemplet);
		SetMedalInfo(stageTemplet);
		SetPlayLimit(cNKMUserData, stageTemplet);
		if (m_imgBoss != null)
		{
			NKMDungeonTempletBase dungeonTempletBase = stageTemplet.DungeonTempletBase;
			if (dungeonTempletBase != null)
			{
				Sprite sprite = null;
				sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_UNIT", "AB_INVEN_ICON_" + dungeonTempletBase.m_DungeonIcon);
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
		}
		if (m_lbLevel != null)
		{
			if (stageTemplet.DungeonTempletBase != null)
			{
				NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, stageTemplet.DungeonTempletBase.m_DungeonLevel));
			}
			else if (stageTemplet.PhaseTemplet != null)
			{
				NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, stageTemplet.PhaseTemplet.PhaseLevel));
			}
		}
		SetDungeonClear(cNKMUserData, stageTemplet);
		NKCUtil.SetGameobjectActive(m_objEventDrop, NKMEpisodeMgr.CheckStageHasEventDrop(stageTemplet) || NKMEpisodeMgr.CheckStageHasBuffDrop(stageTemplet));
	}

	private void SetMainReward(NKMUserData cNKMUserData, NKMStageTempletV2 stageTemplet)
	{
		if (!(m_objMainReward != null))
		{
			return;
		}
		if (stageTemplet.MainRewardData != null && stageTemplet.MainRewardData.rewardType != NKM_REWARD_TYPE.RT_NONE)
		{
			NKCUtil.SetGameobjectActive(m_objMainReward, bValue: true);
			if (m_RewardSlot != null)
			{
				m_RewardSlot.Init();
				NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(stageTemplet.MainRewardData.rewardType, stageTemplet.MainRewardData.ID, stageTemplet.MainRewardData.MinValue);
				m_RewardSlot.SetData(slotData, bEnableLayoutElement: true, OnClickMainReward);
				m_RewardSlot.DisableItemCount();
				NKCUIComButton component = m_RewardSlot.gameObject.GetComponent<NKCUIComButton>();
				if (component != null)
				{
					component.PointerDown.RemoveAllListeners();
					component.PointerDown.AddListener(delegate(PointerEventData x)
					{
						NKCUITooltip.Instance.Open(slotData, x.position);
					});
				}
			}
			else if (m_imgRewardIcon != null && NKMItemMiscTemplet.Find(stageTemplet.MainRewardData.ID) != null)
			{
				NKCUtil.SetImageSprite(m_imgRewardIcon, NKCResourceUtility.GetRewardInvenIcon(stageTemplet.MainRewardData.rewardType, stageTemplet.MainRewardData.ID));
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objMainReward, bValue: false);
		}
	}

	private void SetFirstReward(NKMUserData cNKMUserData, NKMStageTempletV2 stageTemplet)
	{
		if (!(m_objFirstReward != null))
		{
			return;
		}
		if (stageTemplet.GetFirstRewardData() != FirstRewardData.Empty)
		{
			NKCUtil.SetGameobjectActive(m_objFirstReward, bValue: true);
			FirstRewardData firstRewardData = stageTemplet.GetFirstRewardData();
			bool completeMark = NKMEpisodeMgr.CheckClear(cNKMUserData, stageTemplet);
			if (m_FirstRewardSlot != null && firstRewardData != null && firstRewardData.Type != NKM_REWARD_TYPE.RT_NONE && firstRewardData.RewardId != 0)
			{
				NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(firstRewardData.Type, firstRewardData.RewardId, firstRewardData.RewardQuantity);
				m_FirstRewardSlot.SetData(slotData);
				m_FirstRewardSlot.SetCompleteMark(completeMark);
				m_FirstRewardSlot.SetFirstGetMark(bValue: true);
				NKCUIComButton component = m_FirstRewardSlot.gameObject.GetComponent<NKCUIComButton>();
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
		else
		{
			NKCUtil.SetGameobjectActive(m_objFirstReward, bValue: false);
		}
	}

	private void SetMedalInfo(NKMStageTempletV2 stageTemplet)
	{
		if (m_Medal != null)
		{
			if (stageTemplet.m_STAGE_TYPE == STAGE_TYPE.ST_PHASE)
			{
				m_Medal.SetData(stageTemplet.PhaseTemplet);
			}
			else if (stageTemplet.DungeonTempletBase != null)
			{
				m_Medal.SetData(stageTemplet.DungeonTempletBase);
			}
		}
	}

	private void SetPlayLimit(NKMUserData cNKMUserData, NKMStageTempletV2 stageTemplet)
	{
		if (stageTemplet.EnterLimit > 0)
		{
			NKCUtil.SetGameobjectActive(m_objPlayLimit, bValue: true);
			int statePlayCnt = cNKMUserData.GetStatePlayCnt(stageTemplet.Key);
			NKCUtil.SetLabelText(m_lbPlayLimit, $"{stageTemplet.EnterLimit - statePlayCnt}/{stageTemplet.EnterLimit}");
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objPlayLimit, bValue: false);
		}
	}

	private void SetMedalReward(NKMUserData cNKMUserData, NKMStageTempletV2 stageTemplet)
	{
		if (!(m_objMedalReward != null))
		{
			return;
		}
		if (stageTemplet.MissionReward != null && stageTemplet.MissionReward.rewardType != NKM_REWARD_TYPE.RT_NONE && stageTemplet.MissionReward.ID != 0)
		{
			NKCUtil.SetGameobjectActive(m_objMedalReward, bValue: true);
			NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(stageTemplet.MissionReward.rewardType, stageTemplet.MissionReward.ID, stageTemplet.MissionReward.Count);
			m_MedalRewardSlot.SetData(slotData);
			m_MedalRewardSlot.SetFirstAllClearMark(bValue: true);
			bool medalAllClear = NKMEpisodeMgr.GetMedalAllClear(cNKMUserData, stageTemplet);
			m_MedalRewardSlot.SetCompleteMark(medalAllClear);
			NKCUIComButton component = m_MedalRewardSlot.gameObject.GetComponent<NKCUIComButton>();
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
			NKCUtil.SetGameobjectActive(m_objMedalReward, bValue: false);
		}
	}

	private void SetDungeonClear(NKMUserData cNKMUserData, NKMStageTempletV2 stageTemplet)
	{
		if (!(m_objClear != null))
		{
			return;
		}
		bool bValue = false;
		switch (stageTemplet.m_STAGE_TYPE)
		{
		case STAGE_TYPE.ST_DUNGEON:
		{
			NKMDungeonTempletBase dungeonTempletBase = stageTemplet.DungeonTempletBase;
			if (dungeonTempletBase != null && cNKMUserData.GetDungeonClearData(dungeonTempletBase.m_DungeonID) != null)
			{
				bValue = true;
			}
			break;
		}
		case STAGE_TYPE.ST_PHASE:
		{
			NKMPhaseTemplet nKMPhaseTemplet = NKMPhaseTemplet.Find(m_StageBattleStrID);
			if (nKMPhaseTemplet != null && NKCPhaseManager.GetPhaseClearData(nKMPhaseTemplet) != null)
			{
				bValue = true;
			}
			break;
		}
		}
		NKCUtil.SetGameobjectActive(m_objClear, bValue);
	}

	public void SetOnSelectedItemSlot(IDungeonSlot.OnSelectedItemSlot selectedSlot)
	{
		if (selectedSlot != null)
		{
			m_btn.PointerClick.RemoveAllListeners();
			m_OnSelectedSlot = selectedSlot;
			m_btn.PointerClick.AddListener(OnSelectedItemSlotImpl);
		}
	}

	private void OnSelectedItemSlotImpl()
	{
		if (m_OnSelectedSlot != null)
		{
			m_OnSelectedSlot(m_StageIndex, m_StageBattleStrID, isPlaying: false);
		}
	}

	public void SetSelectNode(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objSelected, bValue);
	}

	public void RefreshSlot(NKMUserData cNKMUserData, NKMStageTempletV2 stageTemplet)
	{
		SetDungeonClear(cNKMUserData, stageTemplet);
		SetFirstReward(cNKMUserData, stageTemplet);
		SetMainReward(cNKMUserData, stageTemplet);
		SetMedalInfo(stageTemplet);
		SetMedalReward(cNKMUserData, stageTemplet);
	}

	public void SetEnableNewMark(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objNew, bValue);
	}

	private void OnClickMainReward(NKCUISlot.SlotData slotData, bool bLocked)
	{
	}
}
