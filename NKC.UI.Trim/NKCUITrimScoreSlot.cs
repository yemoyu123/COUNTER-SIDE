using ClientPacket.Common;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Trim;

public class NKCUITrimScoreSlot : MonoBehaviour
{
	public Text m_lbStageIndex;

	public Text m_lbScore;

	public GameObject m_objNone;

	public GameObject m_objClear;

	public void SetData(NKMTrimStageData trimStageData)
	{
		NKCUtil.SetLabelText(m_lbStageIndex, string.Format(NKCUtilString.GET_STRING_TRIM_STAGE_INDEX, (trimStageData.index + 1).ToString("D2")));
		NKCUtil.SetLabelText(m_lbScore, $"{trimStageData.score.ToString():#,0}");
		NKCUtil.SetGameobjectActive(m_objNone, !trimStageData.isWin);
		NKCUtil.SetGameobjectActive(m_objClear, trimStageData.isWin);
	}

	public void SetActive(bool value)
	{
		base.gameObject.SetActive(value);
	}
}
