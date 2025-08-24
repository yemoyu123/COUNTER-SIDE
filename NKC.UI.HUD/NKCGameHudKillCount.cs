using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.HUD;

public class NKCGameHudKillCount : MonoBehaviour
{
	public Text m_lbCount;

	public void SetKillCount(long count)
	{
		NKCUtil.SetLabelText(m_lbCount, NKCStringTable.GetString("SI_DP_GAME_KILLCOUNT", count));
	}

	public void SetKillCount(string count)
	{
		NKCUtil.SetLabelText(m_lbCount, NKCStringTable.GetString("SI_DP_GAME_KILLCOUNT", count));
	}

	public void SetKillCountForDefence(long count)
	{
		NKCUtil.SetLabelText(m_lbCount, NKCStringTable.GetString("SI_PF_GAME_HUD_DEFENCE_TITLE_TEXT", count));
	}

	public void SetKillCountForDefence(string count)
	{
		NKCUtil.SetLabelText(m_lbCount, NKCStringTable.GetString("SI_PF_GAME_HUD_DEFENCE_TITLE_TEXT", count));
	}
}
