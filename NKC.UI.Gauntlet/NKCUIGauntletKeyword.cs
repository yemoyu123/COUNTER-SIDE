using Cs.Logging;
using NKC.Templet;
using NKC.UI.Trim;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletKeyword : MonoBehaviour
{
	public GameObject m_objOn;

	public GameObject m_objOff;

	public Text m_lbKeyword;

	public NKCUIComStateButton m_csbtnButton;

	private int m_keywordId;

	public void Init()
	{
		if (m_csbtnButton != null)
		{
			m_csbtnButton.PointerDown.RemoveAllListeners();
			m_csbtnButton.PointerDown.AddListener(OnButtonDown);
		}
	}

	public void SetData(int keywordId)
	{
		m_keywordId = keywordId;
		NKCUtil.SetGameobjectActive(m_objOn, keywordId > 0);
		NKCUtil.SetGameobjectActive(m_objOff, keywordId <= 0);
		if (keywordId > 0)
		{
			NKCGauntletKeywordTemplet nKCGauntletKeywordTemplet = NKCGauntletKeywordTemplet.Find(keywordId);
			if (nKCGauntletKeywordTemplet != null)
			{
				NKCUtil.SetLabelText(m_lbKeyword, NKCStringTable.GetString(nKCGauntletKeywordTemplet.KeywordNameStrId));
				return;
			}
			NKCUtil.SetLabelText(m_lbKeyword, "");
			Log.Error($"NKCGauntletKeywordTemplet with keywordId {keywordId} is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletKeyword.cs", 47);
		}
	}

	private void OnButtonDown(PointerEventData pointEventData)
	{
		NKCGauntletKeywordTemplet nKCGauntletKeywordTemplet = NKCGauntletKeywordTemplet.Find(m_keywordId);
		if (nKCGauntletKeywordTemplet != null)
		{
			string message = NKCStringTable.GetString(nKCGauntletKeywordTemplet.KeywordDescStrId);
			NKCUITrimToolTip.Instance.Open(message, pointEventData.position, 32);
		}
	}
}
