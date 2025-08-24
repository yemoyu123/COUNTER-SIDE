using System.Collections;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Result;

public class NKCUIResultSubUIWorldmapBranchExp : NKCUIResultSubUIBase
{
	public CanvasGroup m_cgLayout;

	public Text m_lbLevel;

	public Text m_lbCityName;

	public Text m_lbExp;

	public Image m_imgExpGauge;

	public GameObject m_objGreatSuccess;

	private bool m_bInit;

	private bool m_bFinished;

	private int m_oldLevel;

	private int m_newLevel;

	private int m_oldExp;

	private int m_newExp;

	private float m_fExpBeginRatio;

	private float m_fExpTargetRatio;

	private void Init()
	{
		if (!m_bInit)
		{
			_ = base.transform;
			m_bInit = true;
		}
	}

	public void SetData(int cityID, int cityOldLevel, int cityNewLevel, int cityOldExp, int cityNewExp, bool bGreatSuccess = false, bool bIgnoreAutoClose = false)
	{
		Init();
		NKMWorldMapCityTemplet cityTemplet = NKMWorldMapManager.GetCityTemplet(cityID);
		if (cityTemplet == null)
		{
			base.ProcessRequired = false;
			return;
		}
		base.ProcessRequired = true;
		m_bIgnoreAutoClose = bIgnoreAutoClose;
		NKMWorldMapCityExpTemplet cityExpTable = NKMWorldMapManager.GetCityExpTable(cityOldLevel);
		if (cityExpTable == null)
		{
			base.ProcessRequired = false;
			return;
		}
		m_lbCityName.text = cityTemplet.GetName();
		m_lbLevel.text = cityOldLevel.ToString();
		m_oldLevel = cityOldLevel;
		m_newLevel = cityNewLevel;
		m_oldExp = cityOldExp;
		m_newExp = cityNewExp;
		if (cityOldLevel == cityNewLevel)
		{
			m_lbExp.text = $"+{(cityNewExp - cityOldExp).ToString()}";
		}
		else
		{
			m_lbExp.text = $"+{(cityExpTable.m_ExpRequired - cityOldExp + cityNewExp).ToString()}";
		}
		if (cityExpTable.m_ExpRequired == 0)
		{
			m_fExpBeginRatio = 1f;
			m_fExpTargetRatio = 1f;
		}
		else
		{
			NKMWorldMapCityExpTemplet cityExpTable2 = NKMWorldMapManager.GetCityExpTable(cityNewLevel);
			if (cityExpTable2 != null)
			{
				m_fExpBeginRatio = (float)m_oldExp / (float)cityExpTable.m_ExpRequired;
				m_fExpTargetRatio = (float)m_newExp / (float)cityExpTable2.m_ExpRequired + (float)(cityNewLevel - cityOldLevel);
			}
		}
		NKCUtil.SetGameobjectActive(m_objGreatSuccess, bGreatSuccess);
		m_imgExpGauge.fillAmount = m_fExpBeginRatio;
	}

	public override bool IsProcessFinished()
	{
		return m_bFinished;
	}

	protected override IEnumerator InnerProcess(bool bAutoSkip)
	{
		yield return null;
		while (m_cgLayout.alpha < 1f)
		{
			yield return null;
		}
		m_bFinished = false;
		float totalTime = (m_fExpTargetRatio - m_fExpBeginRatio) * 0.6f;
		float deltaTime = 0f;
		int levelDelta = 0;
		while (deltaTime < totalTime)
		{
			float num = NKCUtil.TrackValue(TRACKING_DATA_TYPE.TDT_SLOWER, m_fExpBeginRatio, m_fExpTargetRatio, deltaTime, totalTime);
			if (num - (float)levelDelta >= 1f)
			{
				levelDelta++;
				m_lbLevel.text = (m_oldLevel + levelDelta).ToString();
			}
			m_imgExpGauge.fillAmount = num - (float)levelDelta;
			deltaTime += Time.deltaTime;
			yield return null;
		}
		yield return null;
		FinishProcess();
	}

	public override void FinishProcess()
	{
		if (base.gameObject.activeInHierarchy)
		{
			m_imgExpGauge.fillAmount = m_fExpTargetRatio - (float)(m_newLevel - m_oldLevel);
			m_lbLevel.text = m_newLevel.ToString();
			m_bFinished = true;
		}
	}
}
