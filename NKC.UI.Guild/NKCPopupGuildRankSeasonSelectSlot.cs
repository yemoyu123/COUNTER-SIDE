using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildRankSeasonSelectSlot : MonoBehaviour
{
	public delegate void OnClick(int seasonId);

	public NKCUIComStateButton m_btn;

	public Text m_lbSeasonNameOn;

	public Text m_lbSeasonNameOff;

	private OnClick m_dOnClick;

	private int m_seasonId;

	public void InitUI()
	{
		m_btn.PointerClick.RemoveAllListeners();
		m_btn.PointerClick.AddListener(OnClickSlot);
	}

	public void SetData(int seasonId, string seasonName, bool bSelected, OnClick dOnClick)
	{
		m_seasonId = seasonId;
		m_dOnClick = dOnClick;
		m_btn.Select(bSelected, bForce: true, bImmediate: true);
		NKCUtil.SetLabelText(m_lbSeasonNameOn, seasonName);
		NKCUtil.SetLabelText(m_lbSeasonNameOff, seasonName);
	}

	public void OnClickSlot()
	{
		m_dOnClick?.Invoke(m_seasonId);
	}
}
