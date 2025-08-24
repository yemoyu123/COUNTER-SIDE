using ClientPacket.Common;
using NKC.Trim;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Result;

public class NKCUIResultSubUITrimSlot : MonoBehaviour
{
	public enum State
	{
		Clear,
		Fail,
		Undecided
	}

	public GameObject m_goCurrent;

	public GameObject m_goNormal;

	public Text m_lbStageNumber;

	public Text m_lbStageNumber_Current;

	public Text m_lbScore;

	public GameObject m_goClear;

	public GameObject m_goFail;

	public GameObject m_goNone;

	public void SetData(int index)
	{
		if (NKCTrimManager.TrimModeState == null)
		{
			SetData(index, 0, State.Undecided, bCurrent: false);
			return;
		}
		NKMTrimStageData finishedTrimStageData = NKCTrimManager.GetFinishedTrimStageData(index);
		if (finishedTrimStageData == null)
		{
			SetData(index, 0, State.Undecided, bCurrent: false);
			return;
		}
		bool bCurrent = NKCTrimManager.TrimModeState.lastClearStage != null && NKCTrimManager.TrimModeState.lastClearStage.index == index;
		SetData(index, finishedTrimStageData.score, (!finishedTrimStageData.isWin) ? State.Fail : State.Clear, bCurrent);
	}

	private void SetData(int index, int score, State state, bool bCurrent)
	{
		int num = index + 1;
		NKCUtil.SetLabelText(m_lbStageNumber, num.ToString());
		NKCUtil.SetLabelText(m_lbStageNumber_Current, num.ToString());
		NKCUtil.SetLabelText(m_lbScore, score.ToString());
		NKCUtil.SetGameobjectActive(m_goCurrent, bCurrent);
		NKCUtil.SetGameobjectActive(m_goNormal, !bCurrent);
		NKCUtil.SetGameobjectActive(m_goClear, state == State.Clear);
		NKCUtil.SetGameobjectActive(m_goNone, state == State.Undecided || (m_goFail == null && state != State.Clear));
		NKCUtil.SetGameobjectActive(m_goFail, state == State.Fail);
	}
}
