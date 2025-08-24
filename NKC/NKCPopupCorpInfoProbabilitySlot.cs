using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCPopupCorpInfoProbabilitySlot : MonoBehaviour
{
	public Text m_lbName;

	public Text m_lbProbability;

	public void SetData(string name, float probability)
	{
		NKCUtil.SetLabelText(m_lbName, name);
		NKCUtil.SetLabelText(m_lbProbability, $"{probability:0.0000}%");
	}
}
