using NKC.Publisher;
using NKM;

namespace NKC.UI.Lobby;

public class NKCUISurvey : NKCUILobbyMenuButtonBase
{
	private NKCUIComStateButton m_csbtnSurvey;

	private void OnClickBtn()
	{
		long currSurveyID = NKCScenManager.GetScenManager().GetNKCSurveyMgr().GetCurrSurveyID();
		if (currSurveyID > 0)
		{
			NKCPublisherModule.Notice.OpenSurvey(currSurveyID, OnCompleteSurvey);
		}
	}

	private void OnCompleteSurvey(NKC_PUBLISHER_RESULT_CODE eNKC_PUBLISHER_RESULT_CODE, string additionalError)
	{
		NKCPublisherModule.CheckError(eNKC_PUBLISHER_RESULT_CODE, additionalError, bCloseWaitBox: false);
	}

	public void Init()
	{
		if (m_csbtnSurvey != null)
		{
			m_csbtnSurvey.PointerClick.RemoveAllListeners();
			m_csbtnSurvey.PointerClick.AddListener(OnClickBtn);
		}
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, NKCScenManager.GetScenManager().GetNKCSurveyMgr().CheckAvailableSurvey());
	}
}
