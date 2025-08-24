using NKM.Guild;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUISeasonPointSlot : MonoBehaviour
{
	public class SeasonPointSlotData
	{
		public int ID;

		public int SlotPoint;

		public NKM_REWARD_TYPE RewardType;

		public int RewardID;

		public int RewardCount;

		public static SeasonPointSlotData MakeEmptyData()
		{
			return new SeasonPointSlotData
			{
				ID = 0,
				SlotPoint = 0,
				RewardType = NKM_REWARD_TYPE.RT_NONE,
				RewardID = 0,
				RewardCount = 0
			};
		}

		public static SeasonPointSlotData MakeSeasonPointSlotData(NKMRaidSeasonRewardTemplet cNKMRaidSeasonRewardTemplet)
		{
			return new SeasonPointSlotData
			{
				ID = cNKMRaidSeasonRewardTemplet.RewardBoardId,
				SlotPoint = cNKMRaidSeasonRewardTemplet.RaidPoint,
				RewardType = cNKMRaidSeasonRewardTemplet.RewardType,
				RewardID = cNKMRaidSeasonRewardTemplet.RewardId,
				RewardCount = cNKMRaidSeasonRewardTemplet.RewardValue
			};
		}

		public static SeasonPointSlotData MakeSeasonPointSlotData(GuildSeasonRewardTemplet cGuildSeasonRewardTemplet)
		{
			return new SeasonPointSlotData
			{
				ID = cGuildSeasonRewardTemplet.Key,
				SlotPoint = cGuildSeasonRewardTemplet.GetRewardCountValue(),
				RewardType = cGuildSeasonRewardTemplet.GetRewardItemType(),
				RewardID = cGuildSeasonRewardTemplet.GetRewardItemId(),
				RewardCount = cGuildSeasonRewardTemplet.GetRewardItemValue()
			};
		}
	}

	public delegate void OnClickSlot(SeasonPointSlotData slotData);

	public NKCUISlot m_slot;

	public GameObject m_objNormal;

	public GameObject m_objClear;

	public GameObject m_objComplete;

	public Text m_lbScore;

	public GameObject m_objGauge;

	public Image m_imgGauge;

	private OnClickSlot m_dOnClickSlot;

	private int m_slotScore;

	private bool m_bBtnLocked;

	private SeasonPointSlotData m_SeasonSlotData;

	private bool m_bInitComplete;

	public int GetSlotScore()
	{
		return m_slotScore;
	}

	public void Init()
	{
		m_slot.Init();
		m_bInitComplete = true;
	}

	public void SetData(SeasonPointSlotData seasonPointSlotData, int myPoint, int receivedPoint, bool bIsFinalSlot, OnClickSlot dOnClickSlot)
	{
		m_SeasonSlotData = seasonPointSlotData;
		NKCUISlot.SlotData slotData = null;
		if (seasonPointSlotData.ID != 0)
		{
			slotData = NKCUISlot.SlotData.MakeRewardTypeData(seasonPointSlotData.RewardType, seasonPointSlotData.RewardID, seasonPointSlotData.RewardCount);
		}
		SetData(slotData, seasonPointSlotData.SlotPoint, myPoint, receivedPoint, bIsFinalSlot, dOnClickSlot);
	}

	private void SetData(NKCUISlot.SlotData slotData, int slotScore, int myScore, int receivedScore, bool bIsFinalSlot, OnClickSlot dOnClickSlot)
	{
		if (!m_bInitComplete)
		{
			Init();
		}
		m_dOnClickSlot = dOnClickSlot;
		m_slotScore = slotScore;
		if (slotData == null)
		{
			NKCUtil.SetGameobjectActive(m_slot, bValue: false);
			NKCUtil.SetGameobjectActive(m_lbScore, bValue: false);
			NKCUtil.SetGameobjectActive(m_objNormal, bValue: false);
			NKCUtil.SetGameobjectActive(m_objClear, bValue: false);
			NKCUtil.SetGameobjectActive(m_objComplete, bValue: false);
			NKCUtil.SetGameobjectActive(m_objGauge, bValue: true);
			m_bBtnLocked = true;
		}
		else
		{
			m_bBtnLocked = false;
			NKCUtil.SetGameobjectActive(m_slot, bValue: true);
			m_slot.SetData(slotData, bEnableLayoutElement: true, OnClickBtn);
			m_slot.SetEventGet(slotScore <= receivedScore);
			m_slot.SetDisable(slotScore <= receivedScore);
			m_slot.SetRewardFx(slotScore <= myScore && slotScore > receivedScore);
			NKCUtil.SetGameobjectActive(m_lbScore, bValue: true);
			NKCUtil.SetLabelText(m_lbScore, slotScore.ToString("N0"));
			NKCUtil.SetGameobjectActive(m_objGauge, !bIsFinalSlot);
			NKCUtil.SetGameobjectActive(m_objNormal, slotScore > myScore);
			NKCUtil.SetGameobjectActive(m_objClear, myScore >= slotScore && slotScore > receivedScore);
			NKCUtil.SetGameobjectActive(m_objComplete, slotScore <= receivedScore);
		}
	}

	public void SetGaugeProgress(float progressPercent)
	{
		if (m_imgGauge != null)
		{
			m_imgGauge.fillAmount = progressPercent;
		}
	}

	public void OnClickBtn(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (m_objClear.activeSelf && !m_bBtnLocked)
		{
			m_bBtnLocked = true;
			m_dOnClickSlot?.Invoke(m_SeasonSlotData);
		}
		else
		{
			NKCPopupItemBox.Instance.Open(NKCPopupItemBox.eMode.Normal, slotData);
		}
	}
}
