using NKC.UI.Trim;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperationSubChallengeSlot : MonoBehaviour
{
	public delegate void OnClickSlot(int key);

	public NKCUIComStateButton m_btn;

	[Space]
	public Image m_imgThumbnail;

	public Text m_lbTitle;

	public Text m_lbDesc;

	public Text m_lbRewardDesc;

	[Space]
	public GameObject m_objLock;

	[Space]
	public GameObject m_objEventDrop;

	[Header("입장재화")]
	public GameObject m_objResource;

	public Image m_imgResourceIcon;

	public Text m_lbResourceCount;

	private OnClickSlot m_dOnClickSlot;

	private int m_Key;

	private string m_strLockedMessage = "";

	public void SetData(NKMEpisodeTempletV2 epTemplet, OnClickSlot onClickSlot)
	{
		m_dOnClickSlot = onClickSlot;
		m_btn.PointerClick.RemoveAllListeners();
		m_btn.PointerClick.AddListener(OnClickBtn);
		m_Key = epTemplet.m_EpisodeID;
		NKCUtil.SetImageSprite(m_imgThumbnail, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_OPERATION_Thumbnail", epTemplet.m_EPThumbnail));
		if (m_lbTitle != null && !string.IsNullOrEmpty(epTemplet.m_EpisodeName))
		{
			NKCUtil.SetLabelText(m_lbTitle, epTemplet.GetEpisodeName());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbTitle, "");
		}
		if (m_lbDesc != null && !string.IsNullOrEmpty(epTemplet.m_EpisodeDesc))
		{
			NKCUtil.SetLabelText(m_lbDesc, epTemplet.GetEpisodeDesc());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbDesc, "");
		}
		if (m_lbRewardDesc != null && !string.IsNullOrEmpty(epTemplet.m_EpisodeDescSub))
		{
			NKCUtil.SetLabelText(m_lbRewardDesc, epTemplet.GetEpisodeDescSub());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbRewardDesc, "");
		}
		NKCUtil.SetGameobjectActive(m_objLock, !NKMEpisodeMgr.IsPossibleEpisode(NKCScenManager.CurrentUserData(), epTemplet));
		m_strLockedMessage = NKCUtilString.GetUnlockConditionRequireDesc(epTemplet.GetUnlockInfo());
		NKMStageTempletV2 firstStage = epTemplet.GetFirstStage(1);
		if (firstStage != null)
		{
			if (firstStage.m_StageReqItemID > 0)
			{
				NKCUtil.SetGameobjectActive(m_objResource, bValue: true);
				NKCUtil.SetImageSprite(m_imgResourceIcon, NKCResourceUtility.GetOrLoadMiscItemIcon(firstStage.m_StageReqItemID));
				NKCUtil.SetLabelText(m_lbResourceCount, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(firstStage.m_StageReqItemID).ToString("#,##0"));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objResource, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objResource, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objEventDrop, NKMEpisodeMgr.CheckEpisodeHasEventDrop(epTemplet) || NKMEpisodeMgr.CheckEpisodeHasBuffDrop(epTemplet));
	}

	public void SetData(NKMEpisodeGroupTemplet groupTemplet, EPISODE_CATEGORY category, OnClickSlot onClickSlot)
	{
		m_dOnClickSlot = onClickSlot;
		m_btn.PointerClick.RemoveAllListeners();
		m_btn.PointerClick.AddListener(OnClickBtn);
		if (groupTemplet == null)
		{
			return;
		}
		m_Key = groupTemplet.EpisodeGroupID;
		if (groupTemplet.lstEpisodeTemplet.Count > 0)
		{
			NKMEpisodeTempletV2 nKMEpisodeTempletV = groupTemplet.lstEpisodeTemplet[0];
			if (nKMEpisodeTempletV != null)
			{
				NKCUtil.SetImageSprite(m_imgThumbnail, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_OPERATION_Thumbnail", nKMEpisodeTempletV.m_EPThumbnail));
				NKCUtil.SetLabelText(m_lbTitle, nKMEpisodeTempletV.GetEpisodeTitle());
				switch (category)
				{
				case EPISODE_CATEGORY.EC_TRIM:
					NKCUtil.SetGameobjectActive(m_objLock, !NKCContentManager.IsContentsUnlocked(ContentsType.DIMENSION_TRIM));
					m_strLockedMessage = NKCContentManager.GetLockedMessage(ContentsType.DIMENSION_TRIM);
					NKCUtil.SetGameobjectActive(m_objEventDrop, NKCUITrimUtility.HaveEventDrop());
					NKCUtil.SetGameobjectActive(m_objResource, bValue: false);
					break;
				case EPISODE_CATEGORY.EC_SHADOW:
					NKCUtil.SetGameobjectActive(m_objLock, !NKCContentManager.IsContentsUnlocked(ContentsType.SHADOW_PALACE));
					m_strLockedMessage = NKCContentManager.GetLockedMessage(ContentsType.SHADOW_PALACE);
					NKCUtil.SetGameobjectActive(m_objEventDrop, NKMEpisodeMgr.CheckEpisodeHasEventDrop(nKMEpisodeTempletV) || NKMEpisodeMgr.CheckEpisodeHasBuffDrop(nKMEpisodeTempletV));
					NKCUtil.SetGameobjectActive(m_objResource, bValue: true);
					NKCUtil.SetImageSprite(m_imgResourceIcon, NKCResourceUtility.GetOrLoadMiscItemIcon(19));
					NKCUtil.SetLabelText(m_lbResourceCount, NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(19).ToString("#,##0"));
					break;
				default:
					NKCUtil.SetGameobjectActive(m_objLock, !NKMEpisodeMgr.IsPossibleEpisode(NKCScenManager.CurrentUserData(), nKMEpisodeTempletV));
					m_strLockedMessage = NKCUtilString.GetUnlockConditionRequireDesc(nKMEpisodeTempletV.GetUnlockInfo());
					NKCUtil.SetGameobjectActive(m_objEventDrop, NKMEpisodeMgr.CheckEpisodeHasEventDrop(nKMEpisodeTempletV) || NKMEpisodeMgr.CheckEpisodeHasBuffDrop(nKMEpisodeTempletV));
					NKCUtil.SetGameobjectActive(m_objResource, bValue: false);
					break;
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objEventDrop, NKMEpisodeMgr.CheckEpisodeHasEventDrop(nKMEpisodeTempletV) || NKMEpisodeMgr.CheckEpisodeHasBuffDrop(nKMEpisodeTempletV));
				NKCUtil.SetGameobjectActive(m_objResource, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objEventDrop, bValue: false);
			NKCUtil.SetGameobjectActive(m_objResource, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_lbDesc, bValue: false);
		NKCUtil.SetGameobjectActive(m_lbRewardDesc, bValue: false);
	}

	private void OnClickBtn()
	{
		if (m_objLock.activeSelf)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(m_strLockedMessage, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else
		{
			m_dOnClickSlot?.Invoke(m_Key);
		}
	}
}
