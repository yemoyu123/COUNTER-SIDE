using Cs.Logging;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperationSubGrowthEPSlot : MonoBehaviour
{
	public delegate void OnClickEpSlot(int episodeID);

	public GameObject m_objLock;

	public Image m_img;

	public Text m_lbTitle;

	public Text m_lbSubTitle;

	public Text m_lbRewardDesc;

	public Text m_lbEpSubDesc;

	public Text m_lbRemainTime;

	public GameObject m_objEventBadge;

	public GameObject m_objReddot;

	public NKCUIComStateButton m_btn;

	private OnClickEpSlot m_dOnClickEpSlot;

	private NKMEpisodeTempletV2 m_EpisodeTemplet;

	private int m_EpisodeID;

	private float m_deltaTime;

	public void InitUI(OnClickEpSlot onClickEpSlot)
	{
		m_btn.PointerClick.RemoveAllListeners();
		m_btn.PointerClick.AddListener(OnBtn);
		m_dOnClickEpSlot = onClickEpSlot;
	}

	public bool SetData(NKMEpisodeTempletV2 epTemplet)
	{
		if (epTemplet == null)
		{
			return false;
		}
		m_EpisodeTemplet = epTemplet;
		m_EpisodeID = epTemplet.m_EpisodeID;
		NKMUserData cNKMUserData = NKCScenManager.CurrentUserData();
		if (!epTemplet.IsOpen)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return false;
		}
		NKCUtil.SetImageSprite(m_img, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_OPERATION_Thumbnail", epTemplet.m_EPThumbnail));
		NKCUtil.SetLabelText(m_lbTitle, epTemplet.GetEpisodeName());
		NKCUtil.SetLabelText(m_lbRewardDesc, epTemplet.GetEpisodeDesc());
		NKCUtil.SetLabelText(m_lbEpSubDesc, epTemplet.GetEpisodeDescSub());
		NKCUtil.SetGameobjectActive(m_objReddot, NKMEpisodeMgr.HasReddot(epTemplet.m_EpisodeID));
		NKMStageTempletV2 firstStage = m_EpisodeTemplet.GetFirstStage(1);
		if (firstStage != null)
		{
			bool flag = NKMEpisodeMgr.IsPossibleEpisode(cNKMUserData, m_EpisodeTemplet);
			bool flag2 = !NKMContentUnlockManager.IsContentUnlocked(cNKMUserData, in firstStage.m_UnlockInfo);
			bool flag3 = m_EpisodeTemplet.IsOpenedDayOfWeek();
			NKCUtil.SetGameobjectActive(m_objLock, !flag || flag2 || !flag3);
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objEventBadge, m_EpisodeTemplet.HaveEventDrop || m_EpisodeTemplet.HaveBuffDrop);
		if (m_objLock.activeSelf)
		{
			UnlockInfo unlockInfo = m_EpisodeTemplet.GetUnlockInfo();
			if (!NKMContentUnlockManager.IsStarted(unlockInfo))
			{
				NKCUtil.SetGameobjectActive(m_lbRemainTime, bValue: true);
				string msg = NKCStringTable.GetString("SI_DP_TIME_TO_OPEN", NKCUtilString.GetRemainTimeString(NKMContentUnlockManager.GetConditionStartTime(unlockInfo), 2));
				NKCUtil.SetLabelText(m_lbRemainTime, msg);
			}
			else if (m_EpisodeTemplet.HasEventTimeLimit)
			{
				NKCUtil.SetGameobjectActive(m_lbRemainTime, bValue: true);
				NKCUtil.SetLabelText(m_lbRemainTime, NKCUtilString.GetRemainTimeStringEx(m_EpisodeTemplet.EpisodeDateEndUtc));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lbRemainTime, bValue: false);
			}
		}
		else if (m_EpisodeTemplet.HasEventTimeLimit)
		{
			NKCUtil.SetGameobjectActive(m_lbRemainTime, bValue: true);
			NKCUtil.SetLabelText(m_lbRemainTime, NKCUtilString.GetRemainTimeStringEx(m_EpisodeTemplet.EpisodeDateEndUtc));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbRemainTime, bValue: false);
		}
		m_deltaTime = 0f;
		return true;
	}

	private void Update()
	{
		m_deltaTime += Time.deltaTime;
		if (!(m_deltaTime > 1f))
		{
			return;
		}
		m_deltaTime -= 1f;
		if (m_EpisodeTemplet == null)
		{
			return;
		}
		if (m_objLock.activeSelf)
		{
			if (NKMEpisodeMgr.IsPossibleEpisode(NKCScenManager.CurrentUserData(), m_EpisodeTemplet) && m_EpisodeTemplet.IsOpenedDayOfWeek())
			{
				NKCUtil.SetGameobjectActive(m_objLock, bValue: false);
				return;
			}
			UnlockInfo unlockInfo = m_EpisodeTemplet.GetUnlockInfo();
			if (!NKMContentUnlockManager.IsStarted(unlockInfo))
			{
				string msg = NKCStringTable.GetString("SI_DP_TIME_TO_OPEN", NKCUtilString.GetRemainTimeString(NKMContentUnlockManager.GetConditionStartTime(unlockInfo), 2));
				NKCUtil.SetLabelText(m_lbRemainTime, msg);
			}
		}
		else if (!m_EpisodeTemplet.IsOpenedDayOfWeek())
		{
			NKCUtil.SetGameobjectActive(m_objLock, bValue: true);
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EXCEPTION_EVENT_EXPIRED_POPUP, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
		}
		else
		{
			if (!m_EpisodeTemplet.HasEventTimeLimit)
			{
				return;
			}
			if (!m_EpisodeTemplet.IsOpen)
			{
				Log.Warn($"{m_EpisodeTemplet.m_EpisodeID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Operation/NKCUIOperationSubGrowthEPSlot.cs", 173);
				NKCUtil.SetGameobjectActive(m_objLock, bValue: true);
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EXCEPTION_EVENT_EXPIRED_POPUP, delegate
				{
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
				});
			}
			else
			{
				NKCUtil.SetLabelText(m_lbRemainTime, NKCUtilString.GetRemainTimeStringEx(m_EpisodeTemplet.EpisodeDateEndUtc));
				if (!m_EpisodeTemplet.IsOpen)
				{
					NKCUtil.SetGameobjectActive(m_objLock, bValue: true);
				}
			}
		}
	}

	private void OnBtn()
	{
		if (m_objLock.gameObject.activeSelf)
		{
			NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GetUnlockConditionRequireDesc(nKMEpisodeTempletV.GetFirstStage(1).m_UnlockInfo), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else
		{
			m_dOnClickEpSlot?.Invoke(m_EpisodeID);
		}
	}
}
