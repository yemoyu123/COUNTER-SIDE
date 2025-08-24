using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionTagSlot : MonoBehaviour
{
	public delegate void OnSelected(short tagType, int Idx);

	public GameObject m_OFF;

	public GameObject m_ON;

	public Text m_OFF_COUNT_COUNT_TEXT;

	public Text m_ON_COUNT_COUNT_TEXT;

	public Text m_OFF_TEXT;

	public Text m_ON_TEXT;

	public GameObject m_offCrownIcon;

	public GameObject m_onCrownIcon;

	private NKCUIComStateButton m_btn_Toggle;

	private OnSelected m_Callback;

	private int m_idx;

	private short m_tagType;

	public void TagClicked()
	{
		if (m_Callback != null)
		{
			m_Callback(m_tagType, m_idx);
		}
	}

	private void Awake()
	{
		m_btn_Toggle = GetComponent<NKCUIComStateButton>();
		if (null != m_btn_Toggle)
		{
			m_btn_Toggle.PointerClick.AddListener(delegate
			{
				TagClicked();
			});
		}
	}

	public int GetSlotIndex()
	{
		return m_idx;
	}

	public short GetTagType()
	{
		return m_tagType;
	}

	public void SetData(OnSelected callback, short type, int slotIdx, bool bActive, string title, int count, bool bTop)
	{
		m_Callback = callback;
		m_tagType = type;
		m_idx = slotIdx;
		NKCUtil.SetGameobjectActive(m_ON, bActive);
		NKCUtil.SetGameobjectActive(m_OFF, !bActive);
		NKCUtil.SetLabelText(m_ON_TEXT, title);
		NKCUtil.SetLabelText(m_OFF_TEXT, title);
		NKCUtil.SetLabelText(m_ON_COUNT_COUNT_TEXT, count.ToString());
		NKCUtil.SetLabelText(m_OFF_COUNT_COUNT_TEXT, count.ToString());
		NKCUtil.SetGameobjectActive(m_offCrownIcon, bTop);
		NKCUtil.SetGameobjectActive(m_onCrownIcon, bTop);
	}

	public void UpdateVoteData(int count, bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_ON, bActive);
		NKCUtil.SetGameobjectActive(m_OFF, !bActive);
		NKCUtil.SetLabelText(m_ON_COUNT_COUNT_TEXT, count.ToString());
		NKCUtil.SetLabelText(m_OFF_COUNT_COUNT_TEXT, count.ToString());
	}
}
