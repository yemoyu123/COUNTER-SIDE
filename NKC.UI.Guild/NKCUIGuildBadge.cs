using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildBadge : MonoBehaviour
{
	public Image m_imgFrame;

	public Image m_imgMark;

	public void InitUI()
	{
		SetData(new GuildBadgeInfo(0L));
	}

	public void SetData(long badgeId)
	{
		SetData(new GuildBadgeInfo(badgeId));
	}

	public void SetData(long badgeId, bool bOpponent)
	{
		SetData(new GuildBadgeInfo(badgeId), bOpponent);
	}

	public void SetData(int frameId, int frameColorId, int markId, int markColorId)
	{
		SetData(new GuildBadgeInfo(frameId, frameColorId, markId, markColorId));
	}

	public void SetData(GuildBadgeInfo guildBadgeInfo)
	{
		NKMGuildBadgeFrameTemplet nKMGuildBadgeFrameTemplet = NKMGuildBadgeFrameTemplet.Find(guildBadgeInfo.FrameId);
		if (nKMGuildBadgeFrameTemplet != null)
		{
			NKCUtil.SetImageSprite(m_imgFrame, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_Mark", nKMGuildBadgeFrameTemplet.BadgeFrameImg));
		}
		NKMGuildBadgeColorTemplet nKMGuildBadgeColorTemplet = NKMGuildBadgeColorTemplet.Find(guildBadgeInfo.FrameColorId);
		if (nKMGuildBadgeColorTemplet != null)
		{
			NKCUtil.SetImageColor(m_imgFrame, NKCUtil.GetColor(nKMGuildBadgeColorTemplet.BadgeColorCode));
		}
		NKMGuildBadgeMarkTemplet nKMGuildBadgeMarkTemplet = NKMGuildBadgeMarkTemplet.Find(guildBadgeInfo.MarkId);
		if (nKMGuildBadgeMarkTemplet != null)
		{
			NKCUtil.SetImageSprite(m_imgMark, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_Mark", nKMGuildBadgeMarkTemplet.BadgeMarkImg));
		}
		nKMGuildBadgeColorTemplet = NKMGuildBadgeColorTemplet.Find(guildBadgeInfo.MarkColorId);
		if (nKMGuildBadgeColorTemplet != null)
		{
			NKCUtil.SetImageColor(m_imgMark, NKCUtil.GetColor(nKMGuildBadgeColorTemplet.BadgeColorCode));
		}
	}

	public void SetData(GuildBadgeInfo guildBadgeInfo, bool bOpponent)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			if (bOpponent)
			{
				if (gameOptionData.StreamingHideOpponentInfo)
				{
					SetDataHide();
					return;
				}
			}
			else if (gameOptionData.StreamingHideMyInfo)
			{
				SetDataHide();
				return;
			}
		}
		NKMGuildBadgeFrameTemplet nKMGuildBadgeFrameTemplet = NKMGuildBadgeFrameTemplet.Find(guildBadgeInfo.FrameId);
		if (nKMGuildBadgeFrameTemplet != null)
		{
			NKCUtil.SetImageSprite(m_imgFrame, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_Mark", nKMGuildBadgeFrameTemplet.BadgeFrameImg));
		}
		NKMGuildBadgeColorTemplet nKMGuildBadgeColorTemplet = NKMGuildBadgeColorTemplet.Find(guildBadgeInfo.FrameColorId);
		if (nKMGuildBadgeColorTemplet != null)
		{
			NKCUtil.SetImageColor(m_imgFrame, NKCUtil.GetColor(nKMGuildBadgeColorTemplet.BadgeColorCode));
		}
		NKMGuildBadgeMarkTemplet nKMGuildBadgeMarkTemplet = NKMGuildBadgeMarkTemplet.Find(guildBadgeInfo.MarkId);
		if (nKMGuildBadgeMarkTemplet != null)
		{
			NKCUtil.SetImageSprite(m_imgMark, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_Mark", nKMGuildBadgeMarkTemplet.BadgeMarkImg));
		}
		nKMGuildBadgeColorTemplet = NKMGuildBadgeColorTemplet.Find(guildBadgeInfo.MarkColorId);
		if (nKMGuildBadgeColorTemplet != null)
		{
			NKCUtil.SetImageColor(m_imgMark, NKCUtil.GetColor(nKMGuildBadgeColorTemplet.BadgeColorCode));
		}
	}

	public void SetDataHide()
	{
		NKMGuildBadgeFrameTemplet nKMGuildBadgeFrameTemplet = NKMGuildBadgeFrameTemplet.Find(0);
		if (nKMGuildBadgeFrameTemplet != null)
		{
			NKCUtil.SetImageSprite(m_imgFrame, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_Mark", nKMGuildBadgeFrameTemplet.BadgeFrameImg));
		}
		NKMGuildBadgeColorTemplet nKMGuildBadgeColorTemplet = NKMGuildBadgeColorTemplet.Find(0);
		if (nKMGuildBadgeColorTemplet != null)
		{
			NKCUtil.SetImageColor(m_imgFrame, NKCUtil.GetColor(nKMGuildBadgeColorTemplet.BadgeColorCode));
		}
		NKMGuildBadgeMarkTemplet nKMGuildBadgeMarkTemplet = NKMGuildBadgeMarkTemplet.Find(0);
		if (nKMGuildBadgeMarkTemplet != null)
		{
			NKCUtil.SetImageSprite(m_imgMark, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_Mark", nKMGuildBadgeMarkTemplet.BadgeMarkImg));
		}
		nKMGuildBadgeColorTemplet = NKMGuildBadgeColorTemplet.Find(0);
		if (nKMGuildBadgeColorTemplet != null)
		{
			NKCUtil.SetImageColor(m_imgMark, NKCUtil.GetColor(nKMGuildBadgeColorTemplet.BadgeColorCode));
		}
	}
}
