using System;
using System.Linq;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUISeasonExtraPoint : MonoBehaviour
{
	public NKCUIComStateButton m_csbtnReward;

	public NKCUIComStateButton m_csbtnRewardInfo;

	public NKCComText m_lbRewardPoint;

	public NKCComText m_lbCurPoint;

	public NKCComText m_lbExtraRewardExchangeValue;

	public Image m_imgExtraRewardIcon;

	public Text m_lbSeasonDate;

	private int m_ExtraRewardItemId;

	private bool m_bUseFixedDuration;

	private DateTime m_endDateUTC;

	private float m_fDeltaTime;

	public void Init(bool bUseFixedDuration)
	{
		NKCUtil.SetBindFunction(m_csbtnReward, OnClickReward);
		NKCUtil.SetBindFunction(m_csbtnRewardInfo, OnClickRewardInfo);
		m_bUseFixedDuration = bUseFixedDuration;
	}

	public void Open()
	{
		NKMRaidSeasonTemplet seasonTemplet = NKCRaidSeasonManager.GetNowSeasonTemplet();
		if (seasonTemplet == null)
		{
			return;
		}
		NKMRaidSeasonRewardTemplet nKMRaidSeasonRewardTemplet = (from e in NKMRaidSeasonRewardTemplet.Values
			where e.RewardBoardId == seasonTemplet.RaidBoardId
			where e.ExtraRewardId > 0
			select e).FirstOrDefault();
		if (nKMRaidSeasonRewardTemplet != null)
		{
			bool flag = nKMRaidSeasonRewardTemplet.ExtraRewardPoint <= NKCRaidSeasonManager.RaidSeason.monthlyPoint - NKCRaidSeasonManager.RaidSeason.recvRewardRaidPoint;
			m_csbtnReward.SetLock(!flag);
			Sprite orLoadMiscItemIcon = NKCResourceUtility.GetOrLoadMiscItemIcon(nKMRaidSeasonRewardTemplet.ExtraRewardId);
			if (null != orLoadMiscItemIcon)
			{
				NKCUtil.SetImageSprite(m_imgExtraRewardIcon, orLoadMiscItemIcon);
			}
			string.Format(NKCStringTable.GetString("SI_PF_WORLD_MAP_RENEWAL_EVENT_POPUP_EXTRA_REWARD_DESC"), nKMRaidSeasonRewardTemplet.ExtraRewardPoint);
			NKCUtil.SetLabelText(m_lbExtraRewardExchangeValue, string.Format(NKCStringTable.GetString("SI_PF_WORLD_MAP_RENEWAL_EVENT_POPUP_EXTRA_REWARD_DESC"), nKMRaidSeasonRewardTemplet.ExtraRewardPoint));
			NKCUtil.SetLabelText(m_lbCurPoint, NKCRaidSeasonManager.RaidSeason.monthlyPoint.ToString("N0"));
			NKCUtil.SetLabelText(m_lbRewardPoint, (NKCRaidSeasonManager.RaidSeason.monthlyPoint - NKCRaidSeasonManager.RaidSeason.recvRewardRaidPoint).ToString());
			m_ExtraRewardItemId = nKMRaidSeasonRewardTemplet.ExtraRewardId;
			m_endDateUTC = seasonTemplet.IntervalTemplet.GetEndDateUtc();
			if (!m_bUseFixedDuration)
			{
				SetRemainTime();
			}
			else
			{
				NKCUtil.SetLabelText(m_lbSeasonDate, NKCUtilString.GetTimeIntervalString(seasonTemplet.IntervalTemplet.StartDate, seasonTemplet.IntervalTemplet.EndDate, NKMTime.INTERVAL_FROM_UTC, bDateOnly: true));
			}
		}
	}

	private void OnClickReward()
	{
		NKCPacketSender.Send_NKMPacket_RAID_POINT_EXTRA_REWARD_REQ();
	}

	private void OnClickRewardInfo()
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(m_ExtraRewardItemId);
		if (itemMiscTempletByID != null && itemMiscTempletByID.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_RANDOMBOX)
		{
			NKCUISlotListViewer newInstance = NKCUISlotListViewer.GetNewInstance();
			if (newInstance != null)
			{
				newInstance.OpenPackageInfo(m_ExtraRewardItemId, NKCStringTable.GetString("SI_DP_SLOT_VIEWR"), NKCStringTable.GetString("SI_DP_SLOT_RATIO_VIEWER_DESC"));
			}
		}
	}

	private void Update()
	{
		if (!m_bUseFixedDuration)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > 1f)
			{
				m_fDeltaTime -= 1f;
				SetRemainTime();
			}
		}
	}

	private void SetRemainTime()
	{
		NKCUtil.SetLabelText(m_lbSeasonDate, NKCUtilString.GetRemainTimeStringEx(m_endDateUTC));
	}
}
