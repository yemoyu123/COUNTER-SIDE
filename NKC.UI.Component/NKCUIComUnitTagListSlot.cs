using NKC.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI.Component;

public class NKCUIComUnitTagListSlot : MonoBehaviour
{
	public TextMeshProUGUI m_TMPText;

	public Text m_lbText;

	public NKCUIComStateButton m_csbtnButton;

	public bool SetData(string unitTag, UnityAction onClick)
	{
		NKCUnitTagInfoTemplet nKCUnitTagInfoTemplet = NKCUnitTagInfoTemplet.Find(unitTag);
		if (nKCUnitTagInfoTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return false;
		}
		string tagString = nKCUnitTagInfoTemplet.GetTagString();
		NKCUtil.SetLabelText(m_TMPText, tagString);
		NKCUtil.SetLabelText(m_lbText, tagString);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetButtonClickDelegate(m_csbtnButton, onClick);
		return true;
	}
}
