using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperationSubStorySlot : MonoBehaviour
{
	public delegate void OnSelectSlot(int episodeID);

	[Header("------ 해당 에피소드 아이디 직접 입력해줘야함 ------")]
	public int m_EpisodeID;

	[Header("아래는 전부 링크 필요")]
	public NKCUIComStateButton m_btn;

	public TMP_Text m_lbTitlie;

	public TMP_Text m_lbSubTitle;

	public GameObject m_objSupplement;

	public GameObject m_objSubstream;

	public GameObject m_objReddot;

	[Space]
	public Image m_imgIcon;

	[Space]
	public GameObject m_objEventDrop;

	[Header("레벨")]
	public TMP_Text m_lbLevel;

	[Header("조건 걸려서 잠겼을 때")]
	public GameObject m_objLock;

	[Header("태그로 막혔을 때")]
	public GameObject m_objBlind;

	[Header("오브젝트 강조 이펙트")]
	public GameObject m_objFocusFX;

	private OnSelectSlot m_dOnSelectSlot;

	public int GetEpisodeID()
	{
		return m_EpisodeID;
	}

	public void SetEpisodeID(int episodeID)
	{
		m_EpisodeID = episodeID;
	}

	public void InitUI(OnSelectSlot onSelectSlot = null)
	{
		m_btn.PointerClick.RemoveAllListeners();
		m_btn.PointerClick.AddListener(OnClickSlot);
		m_dOnSelectSlot = onSelectSlot;
	}

	public void SetData(bool bSetImage = false)
	{
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
		if (nKMEpisodeTempletV == null || !nKMEpisodeTempletV.EnableByTag)
		{
			NKCUtil.SetGameobjectActive(m_objSubstream, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSupplement, bValue: false);
			NKCUtil.SetGameobjectActive(m_objLock, bValue: false);
			NKCUtil.SetGameobjectActive(m_objBlind, bValue: true);
			NKCUtil.SetGameobjectActive(m_objReddot, bValue: false);
			NKCUtil.SetGameobjectActive(m_objEventDrop, bValue: false);
			NKCUtil.SetGameobjectActive(m_objFocusFX, bValue: false);
			NKCUtil.SetLabelText(m_lbLevel, "");
			return;
		}
		NKCUtil.SetLabelText(m_lbTitlie, nKMEpisodeTempletV.GetEpisodeTitle());
		NKCUtil.SetLabelText(m_lbSubTitle, nKMEpisodeTempletV.GetEpisodeName());
		NKCUtil.SetGameobjectActive(m_objSubstream, !nKMEpisodeTempletV.m_bIsSupplement);
		NKCUtil.SetGameobjectActive(m_objSupplement, nKMEpisodeTempletV.m_bIsSupplement);
		int firstBattleStageLevel = nKMEpisodeTempletV.GetFirstBattleStageLevel(1);
		if (firstBattleStageLevel > 0)
		{
			NKCUtil.SetLabelText(m_lbLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, firstBattleStageLevel));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbLevel, "");
		}
		if (bSetImage)
		{
			NKCUtil.SetGameobjectActive(m_imgIcon, bValue: true);
			NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_OPERATION_Texture_Res", nKMEpisodeTempletV.m_EPThumbnail_SUB_Node));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_imgIcon, bValue: false);
		}
		NKCUtil.SetGameobjectActive(m_objLock, !NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), nKMEpisodeTempletV.GetUnlockInfo()));
		NKCUtil.SetGameobjectActive(m_objBlind, !nKMEpisodeTempletV.IsOpen);
		NKCUtil.SetGameobjectActive(m_objReddot, NKMEpisodeMgr.HasReddot(nKMEpisodeTempletV.m_EpisodeID));
		NKCUtil.SetGameobjectActive(m_objEventDrop, NKMEpisodeMgr.CheckEpisodeHasEventDrop(nKMEpisodeTempletV) || NKMEpisodeMgr.CheckEpisodeHasBuffDrop(nKMEpisodeTempletV));
		NKCUtil.SetGameobjectActive(m_objFocusFX, bValue: false);
	}

	public void Refresh()
	{
		NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
		if (nKMEpisodeTempletV != null && nKMEpisodeTempletV.EnableByTag)
		{
			NKCUtil.SetGameobjectActive(m_objLock, !NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), nKMEpisodeTempletV.GetUnlockInfo()));
			NKCUtil.SetGameobjectActive(m_objBlind, !nKMEpisodeTempletV.IsOpen);
			NKCUtil.SetGameobjectActive(m_objReddot, NKMEpisodeMgr.HasReddot(nKMEpisodeTempletV.m_EpisodeID));
			NKCUtil.SetGameobjectActive(m_objEventDrop, NKMEpisodeMgr.CheckEpisodeHasEventDrop(nKMEpisodeTempletV) || NKMEpisodeMgr.CheckEpisodeHasBuffDrop(nKMEpisodeTempletV));
			NKCUtil.SetGameobjectActive(m_objFocusFX, bValue: false);
		}
	}

	public void ShowFocusFx()
	{
		NKCUtil.SetGameobjectActive(m_objFocusFX, bValue: false);
		NKCUtil.SetGameobjectActive(m_objFocusFX, bValue: true);
	}

	private void OnClickSlot()
	{
		if (m_objBlind.activeSelf)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_EPISODE_SUBSTREAM_DATA_EXPUNGED, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (m_objLock.activeSelf)
		{
			NKMEpisodeTempletV2 nKMEpisodeTempletV = NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL);
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCContentManager.MakeUnlockConditionString(nKMEpisodeTempletV.GetUnlockInfo(), bSimple: false), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (m_dOnSelectSlot != null)
		{
			m_dOnSelectSlot(m_EpisodeID);
		}
		else
		{
			NKCUIOperationSubStoryPopup.Instance.Open(NKMEpisodeTempletV2.Find(m_EpisodeID, EPISODE_DIFFICULTY.NORMAL));
		}
	}
}
