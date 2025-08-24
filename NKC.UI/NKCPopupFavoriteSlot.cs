using System;
using NKC.UI.Tooltip;
using NKM;
using NKM.Shop;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupFavoriteSlot : MonoBehaviour
{
	public NKCUIComStateButton m_btnSlot;

	public Image m_imgCategory;

	public Image m_imgNormalDot;

	public Image m_imgHardDot;

	public TMP_Text m_lbStageNum;

	public TMP_Text m_lbStageName;

	public GameObject m_objMainReward;

	public NKCUISlot m_slotMainReward;

	public NKCUIComStateButton m_btnDelete;

	public TMP_Text m_lbStageEnterLimit;

	public GameObject m_objEventDrop;

	private NKMStageTempletV2 m_StageTemplet;

	[SerializeField]
	private GameObject SelectedEditMode;

	private Action<NKCPopupFavoriteSlot> _onSelected;

	private Func<bool> _isEditMode;

	public int StageID => m_StageTemplet?.Key ?? int.MinValue;

	public int IDX { get; private set; } = int.MinValue;

	private void Awake()
	{
		NKCUtil.SetGameobjectActive(SelectedEditMode, bValue: false);
	}

	private void OnClickSlotForEdit()
	{
		_onSelected?.Invoke(this);
	}

	public void Select(bool select)
	{
		NKCUtil.SetGameobjectActive(SelectedEditMode, select);
	}

	public void InitUI(Action<NKCPopupFavoriteSlot> onSelect, Func<bool> isEditMode)
	{
		m_btnSlot.PointerClick.RemoveAllListeners();
		m_btnSlot.PointerClick.AddListener(OnClickSlot);
		m_btnSlot.PointerClick.AddListener(OnClickSlotForEdit);
		m_btnSlot.m_bGetCallbackWhileLocked = true;
		m_btnDelete.PointerClick.RemoveAllListeners();
		m_btnDelete.PointerClick.AddListener(OnClickDelete);
		_onSelected = onSelect;
		_isEditMode = isEditMode;
	}

	public void SetData(int idx, NKMStageTempletV2 stageTemplet)
	{
		m_StageTemplet = stageTemplet;
		IDX = idx;
		NKCUtil.SetLabelText(m_lbStageNum, NKCUtilString.GetEpisodeNumber(stageTemplet.EpisodeTemplet, stageTemplet));
		NKCUtil.SetLabelText(m_lbStageName, stageTemplet.GetDungeonName());
		if (stageTemplet.MainRewardData != null && m_slotMainReward != null)
		{
			NKCUtil.SetGameobjectActive(m_objMainReward, bValue: true);
			NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(stageTemplet.MainRewardData.rewardType, stageTemplet.MainRewardData.ID, stageTemplet.MainRewardData.MinValue);
			m_slotMainReward.SetData(slotData);
			m_slotMainReward.DisableItemCount();
			NKCUIComButton component = m_slotMainReward.gameObject.GetComponent<NKCUIComButton>();
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
			NKCUtil.SetGameobjectActive(m_objMainReward, bValue: false);
		}
		bool flag = false;
		if (stageTemplet.EnterLimit > 0)
		{
			NKCUtil.SetGameobjectActive(m_lbStageEnterLimit, bValue: true);
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			int num = 0;
			if (nKMUserData != null)
			{
				num = nKMUserData.GetStatePlayCnt(m_StageTemplet.Key);
			}
			NKCUtil.SetLabelText(m_lbStageEnterLimit, $"{stageTemplet.EnterLimit - num}/{stageTemplet.EnterLimit}");
			flag = !nKMUserData.IsHaveStatePlayData(m_StageTemplet.Key) || num < m_StageTemplet.EnterLimit || ((nKMUserData.GetStageRestoreCnt(m_StageTemplet.Key) < m_StageTemplet.RestoreLimit) ? true : false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbStageEnterLimit, bValue: false);
			flag = true;
		}
		if (m_StageTemplet.EpisodeTemplet.IsOpen && m_StageTemplet.IsOpenedDayOfWeek() && flag)
		{
			m_btnSlot.UnLock();
		}
		else
		{
			m_btnSlot.Lock();
		}
		NKMEpisodeGroupTemplet nKMEpisodeGroupTemplet = NKMEpisodeGroupTemplet.Find(m_StageTemplet.EpisodeTemplet.m_GroupID);
		NKCUtil.SetImageSprite(m_imgCategory, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_OPERATION_Thumbnail", nKMEpisodeGroupTemplet.m_EPGroupIcon));
		if (NKMEpisodeMgr.HasHardDifficulty(stageTemplet.EpisodeId))
		{
			NKCUtil.SetGameobjectActive(m_imgNormalDot, stageTemplet.m_Difficulty == EPISODE_DIFFICULTY.NORMAL);
			NKCUtil.SetGameobjectActive(m_imgHardDot, stageTemplet.m_Difficulty == EPISODE_DIFFICULTY.HARD);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_imgNormalDot, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgHardDot, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objEventDrop, NKMEpisodeMgr.CheckStageHasEventDrop(stageTemplet) || NKMEpisodeMgr.CheckStageHasBuffDrop(stageTemplet));
	}

	private void OnClickSlot()
	{
		if (_isEditMode())
		{
			return;
		}
		if (m_btnSlot.m_bLock)
		{
			if (!m_StageTemplet.EpisodeTemplet.IsOpen || !m_StageTemplet.IsOpenedDayOfWeek())
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_FAVORITES_NO_ENTRY, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			}
			else
			{
				NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_ENTER_LIMIT_OVER, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
			}
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (m_StageTemplet.EnterLimit > 0 && nKMUserData.IsHaveStatePlayData(m_StageTemplet.Key) && nKMUserData.GetStatePlayCnt(m_StageTemplet.Key) >= m_StageTemplet.EnterLimit)
		{
			if (nKMUserData.GetStageRestoreCnt(m_StageTemplet.Key) >= m_StageTemplet.RestoreLimit)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ENTER_LIMIT_OVER);
				return;
			}
			NKCPopupResourceWithdraw.Instance.OpenForRestoreEnterLimit(m_StageTemplet, delegate
			{
				NKCPacketSender.Send_NKMPacket_RESET_STAGE_PLAY_COUNT_REQ(m_StageTemplet.Key);
			}, nKMUserData.GetStageRestoreCnt(m_StageTemplet.Key));
		}
		else if (m_StageTemplet.EpisodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_DAILY && m_StageTemplet.m_StageReqItemCount - nKMUserData.m_InventoryData.GetCountMiscItem(m_StageTemplet.m_StageReqItemID) > 0)
		{
			int dailyMissionTicketShopID = NKCShopManager.GetDailyMissionTicketShopID(m_StageTemplet.EpisodeTemplet.m_EpisodeID);
			if (NKCShopManager.GetBuyCountLeft(dailyMissionTicketShopID) > 0)
			{
				NKCShopManager.OnBtnProductBuy(ShopItemTemplet.Find(dailyMissionTicketShopID).Key, bSupply: false);
			}
			else
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_ENTER_LIMIT_OVER);
			}
		}
		else if (NKMEpisodeMgr.HasEnoughResource(m_StageTemplet))
		{
			NKCScenManager.GetScenManager().Get_SCEN_OPERATION().PlayByFavorite = true;
			if (m_StageTemplet.m_STAGE_TYPE == STAGE_TYPE.ST_PHASE)
			{
				NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().SetDungeonInfo(m_StageTemplet, DeckContents.PHASE);
			}
			else
			{
				NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().SetDungeonInfo(m_StageTemplet);
			}
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DUNGEON_ATK_READY);
		}
	}

	private void OnClickDelete()
	{
		NKCPacketSender.Send_NKMPacket_FAVORITES_STAGE_DELETE_REQ(m_StageTemplet.Key);
	}
}
