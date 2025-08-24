using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCDeckListButton : MonoBehaviour
{
	public delegate void dOnChangedSelected(int index, bool bSet);

	public NKMDeckIndex m_DeckIndex;

	public NKCUIComStateButton m_cbtnButton;

	public GameObject m_BG_On;

	public GameObject m_BG_Selected;

	public GameObject m_BG_Locked;

	public GameObject m_BG_On_Multi;

	public GameObject m_BG_Selected_Multi;

	public GameObject m_BG_Locked_Multi;

	public GameObject m_Trim_Normal;

	public GameObject m_Trim_Selected;

	public GameObject m_objSingle;

	public GameObject m_objMulti;

	public GameObject m_objTrim;

	[Header("싱글")]
	public GameObject m_objTextRoot;

	public Text m_lbIndex;

	public Text m_lbDeckState;

	public Text m_lbSQUAD;

	public Image m_imgSupportIcon;

	public Text m_lbSupportText;

	public Image m_imgBGSelected;

	public Image m_imgBGNormal;

	[Header("멀티")]
	public GameObject m_objTextRootForMulti;

	public Text m_lbIndexForMulti;

	public Text m_lbDeckStateForMulti;

	public Text m_lbSQUADForMulti;

	public GameObject m_objToggleForMulti;

	public NKCUIComToggle m_ctToggleForMulti;

	public Image m_imgBGSelectedForMulti;

	public Image m_imgBGNormalForMulti;

	public Text m_lbMultiSelectedSeq;

	[Header("트림 전용")]
	public Text m_lbTrimDeckIndexNormal;

	public Text m_lbTrimDeckIndexSelected;

	[Header("컬러")]
	public Color m_colTextSelected;

	public Color m_colTextNormal;

	public Color m_colTextSQUADNormal;

	public Color m_colImgBGNormal;

	public Color m_colImgBGUnusable;

	public Color m_colImgBGSelected;

	private NKMDeckData m_DeckData;

	private string m_stateText = string.Empty;

	private dOnChangedSelected m_dOnChangedSelected;

	public void Init(int index, dOnChangedSelected _dOnChangedSelected = null)
	{
		m_DeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, index);
		m_dOnChangedSelected = _dOnChangedSelected;
		m_lbIndex.text = GetIndexText(index);
		m_lbIndexForMulti.text = GetIndexText(index);
		m_ctToggleForMulti.OnValueChanged.RemoveAllListeners();
		m_ctToggleForMulti.OnValueChanged.AddListener(OnChangedMultiSelected);
	}

	private void OnChangedMultiSelected(bool bSet)
	{
		if (m_dOnChangedSelected != null)
		{
			m_dOnChangedSelected(m_DeckIndex.m_iIndex, bSet);
		}
	}

	public void SetMultiSelect(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objSingle, !bValue);
		NKCUtil.SetGameobjectActive(m_objMulti, bValue);
		if (bValue)
		{
			m_cbtnButton.m_ButtonBG_Selected = m_BG_Selected_Multi;
			m_cbtnButton.m_ButtonBG_Locked = m_BG_Locked_Multi;
			m_cbtnButton.m_ButtonBG_Normal = m_BG_On_Multi;
		}
		else
		{
			m_cbtnButton.m_ButtonBG_Selected = m_BG_Selected;
			m_cbtnButton.m_ButtonBG_Locked = m_BG_Locked;
			m_cbtnButton.m_ButtonBG_Normal = m_BG_On;
		}
	}

	public void SetTrimDeckSelect(bool isTrimSquad)
	{
		if (isTrimSquad)
		{
			NKCUtil.SetGameobjectActive(m_objSingle, bValue: false);
			NKCUtil.SetGameobjectActive(m_objMulti, bValue: false);
			NKCUtil.SetLabelText(m_lbTrimDeckIndexNormal, (m_DeckIndex.m_iIndex + 1).ToString());
			NKCUtil.SetLabelText(m_lbTrimDeckIndexSelected, (m_DeckIndex.m_iIndex + 1).ToString());
			m_cbtnButton.m_ButtonBG_Selected = m_Trim_Selected;
			m_cbtnButton.m_ButtonBG_Normal = m_Trim_Normal;
		}
		NKCUtil.SetGameobjectActive(m_objTrim, isTrimSquad);
	}

	private string GetIndexText(int index)
	{
		return (index + 1).ToString();
	}

	private string GetStateText(NKMDeckData deckData)
	{
		if (deckData == null)
		{
			return "";
		}
		if (!string.IsNullOrEmpty(m_stateText))
		{
			return m_stateText;
		}
		return deckData.IsValidState() switch
		{
			NKM_ERROR_CODE.NEC_FAIL_WORLDMAP_MISSION_DOING => NKCUtilString.GET_STRING_DECK_STATE_DOING_MISSION, 
			NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING => NKCUtilString.GET_STRING_DECK_STATE_DOING_WARFARE, 
			NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING => NKCUtilString.GET_STRING_DECK_STATE_DOING_DIVE, 
			_ => NKCUIDeckViewer.GetDeckName(m_DeckIndex), 
		};
	}

	public void SetData(NKMArmyData armyData, NKMDeckIndex deckIndex, string stateText = "")
	{
		m_DeckIndex = deckIndex;
		NKMDeckData deckData = armyData.GetDeckData(m_DeckIndex);
		m_DeckData = deckData;
		m_stateText = stateText;
		UpdateUI();
	}

	public void UpdateUI()
	{
		bool bSelect = m_cbtnButton.m_bSelect;
		bool flag = m_DeckData != null && m_DeckData.IsValidState() == NKM_ERROR_CODE.NEC_OK;
		if (!string.IsNullOrEmpty(m_stateText))
		{
			flag = true;
		}
		if (flag && bSelect)
		{
			m_lbIndex.color = m_colTextSelected;
			m_lbIndexForMulti.color = m_colTextSelected;
			m_lbDeckState.color = m_colTextSelected;
			m_lbDeckStateForMulti.color = m_colTextSelected;
			m_lbSQUAD.color = m_colTextSelected;
			m_lbSQUADForMulti.color = m_colTextSelected;
		}
		else
		{
			m_lbIndex.color = m_colTextNormal;
			m_lbIndexForMulti.color = m_colTextNormal;
			m_lbDeckState.color = m_colTextNormal;
			m_lbDeckStateForMulti.color = m_colTextNormal;
			m_lbSQUAD.color = m_colTextSQUADNormal;
			m_lbSQUADForMulti.color = m_colTextSQUADNormal;
		}
		if (!flag)
		{
			m_imgBGNormal.color = m_colImgBGUnusable;
			m_imgBGNormalForMulti.color = m_colImgBGUnusable;
			m_imgBGSelected.color = m_colImgBGUnusable;
			m_imgBGSelectedForMulti.color = m_colImgBGUnusable;
		}
		else
		{
			m_imgBGNormal.color = m_colImgBGNormal;
			m_imgBGNormalForMulti.color = m_colImgBGNormal;
			m_imgBGSelected.color = m_colImgBGSelected;
			m_imgBGSelectedForMulti.color = m_colImgBGSelected;
		}
		m_lbDeckState.text = GetStateText(m_DeckData);
		m_lbDeckStateForMulti.text = GetStateText(m_DeckData);
	}

	public void Lock()
	{
		m_cbtnButton.Lock(bForce: true);
		NKCUtil.SetGameobjectActive(m_objTextRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTextRootForMulti, bValue: false);
		NKCUtil.SetGameobjectActive(m_objToggleForMulti, bValue: false);
	}

	public void UnLock()
	{
		m_cbtnButton.UnLock(bForce: true);
		NKCUtil.SetGameobjectActive(m_objTextRoot, bValue: true);
		NKCUtil.SetGameobjectActive(m_objTextRootForMulti, bValue: true);
		NKCUtil.SetGameobjectActive(m_objToggleForMulti, bValue: true);
	}

	public void ButtonSelect()
	{
		m_cbtnButton.Select(bSelect: true);
		UpdateUI();
	}

	public void ButtonDeSelect()
	{
		m_cbtnButton.Select(bSelect: false);
		UpdateUI();
	}
}
