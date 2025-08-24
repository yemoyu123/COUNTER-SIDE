using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUILeaderBoardSubTab : MonoBehaviour
{
	public delegate void OnSelectSubTab(int tabID);

	public NKCUIComToggle m_tgl;

	public Text m_lbTitle;

	private OnSelectSubTab dOnSelectSubTab;

	public int m_tabID { get; private set; }

	public void SetData(NKCUIComToggleGroup tglGroup, string title, int tabID, OnSelectSubTab onSelectSubTab)
	{
		NKCUtil.SetLabelText(m_lbTitle, title);
		m_tgl.SetToggleGroup(tglGroup);
		m_tabID = tabID;
		dOnSelectSubTab = onSelectSubTab;
		m_tgl.OnValueChanged.RemoveAllListeners();
		m_tgl.OnValueChanged.AddListener(OnValueChanged);
	}

	public void OnValueChanged(bool bValue)
	{
		if (bValue)
		{
			dOnSelectSubTab?.Invoke(m_tabID);
		}
	}
}
