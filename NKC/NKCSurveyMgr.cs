using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using NKM;

namespace NKC;

public class NKCSurveyMgr
{
	private List<SurveyInfo> m_lstSurveyInfo = new List<SurveyInfo>();

	public void Clear()
	{
		m_lstSurveyInfo.Clear();
	}

	public void UpdaterOrAdd(SurveyInfo si)
	{
		for (int i = 0; i < m_lstSurveyInfo.Count; i++)
		{
			SurveyInfo surveyInfo = m_lstSurveyInfo[i];
			if (surveyInfo.surveyId == si.surveyId)
			{
				surveyInfo.userLevel = si.userLevel;
				surveyInfo.startDate = si.startDate;
				surveyInfo.endDate = si.endDate;
				return;
			}
		}
		m_lstSurveyInfo.Add(si);
	}

	public void Delete(long id)
	{
		for (int i = 0; i < m_lstSurveyInfo.Count; i++)
		{
			if (m_lstSurveyInfo[i].surveyId == id)
			{
				m_lstSurveyInfo.RemoveAt(i);
				break;
			}
		}
	}

	public bool CheckAvailableSurvey()
	{
		for (int i = 0; i < m_lstSurveyInfo.Count; i++)
		{
			if (CheckAvailableSurvey(m_lstSurveyInfo[i]))
			{
				return true;
			}
		}
		return false;
	}

	public long GetCurrSurveyID()
	{
		if (m_lstSurveyInfo == null || m_lstSurveyInfo.Count <= 0)
		{
			return -1L;
		}
		m_lstSurveyInfo = m_lstSurveyInfo.OrderBy((SurveyInfo x) => x.surveyId).ToList();
		return m_lstSurveyInfo[0].surveyId;
	}

	private bool CheckAvailableSurvey(SurveyInfo si)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return false;
		}
		if (nKMUserData.UserLevel < si.userLevel)
		{
			return false;
		}
		if (!NKCSynchronizedTime.IsEventTime(si.startDate, si.endDate))
		{
			return false;
		}
		return true;
	}
}
