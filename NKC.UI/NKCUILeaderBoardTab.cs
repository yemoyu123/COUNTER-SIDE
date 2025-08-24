using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUILeaderBoardTab : MonoBehaviour
{
	public delegate void OnValueChange(LeaderBoardType boardType);

	public NKCUIComToggle m_tgl;

	public Image m_imgIcon;

	public Image m_imgIconOff;

	public Text m_lbTitle;

	public GameObject m_objRedDot;

	private OnValueChange dOnValueChange;

	private Color TITLE_COLOR_TAB_ON = new Color(0.003921569f, 9f / 85f, 0.23137255f);

	private Color TITLE_COLOR_TAB_OFF = Color.white;

	private LeaderBoardType m_BoardType;

	public void SetData(LeaderBoardType boardType, NKCUIComToggleGroup tglGroup, OnValueChange onValueChange)
	{
		m_tgl.SetToggleGroup(tglGroup);
		m_tgl.OnValueChanged.RemoveAllListeners();
		m_tgl.OnValueChanged.AddListener(OnValueChanged);
		dOnValueChange = onValueChange;
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(boardType, 0);
		if (nKMLeaderBoardTemplet == null)
		{
			Debug.LogWarning($"NKMLeaderBoardTemplet is null - {boardType}");
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_BoardType = boardType;
		NKCUtil.SetLabelText(m_lbTitle, nKMLeaderBoardTemplet.GetTabName());
		SetTitleColor(m_tgl.m_bSelect);
		NKCUtil.SetImageSprite(m_imgIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_LEADER_BOARD_SPRITE", nKMLeaderBoardTemplet.m_BoardTabIcon));
		NKCUtil.SetImageSprite(m_imgIconOff, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_LEADER_BOARD_SPRITE", nKMLeaderBoardTemplet.m_BoardTabIcon));
		CheckRedDot();
	}

	private void OnValueChanged(bool bValue)
	{
		if (bValue)
		{
			dOnValueChange?.Invoke(m_BoardType);
		}
	}

	public void SetTitleColor(bool bValue)
	{
		if (m_tgl.m_bSelect)
		{
			NKCUtil.SetLabelTextColor(m_lbTitle, TITLE_COLOR_TAB_ON);
		}
		else
		{
			NKCUtil.SetLabelTextColor(m_lbTitle, TITLE_COLOR_TAB_OFF);
		}
	}

	public void CheckRedDot()
	{
		if (m_BoardType == LeaderBoardType.BT_FIERCE)
		{
			NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
		}
	}
}
