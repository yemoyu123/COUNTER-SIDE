using Cs.Logging;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLobbyNormal : MonoBehaviour
{
	public NKCUIComStateButton m_csbtnBattleRecord;

	public NKCUIComStateButton m_csbtnBattleReady;

	public Text m_lbAccumWin;

	public Text m_lbMaxStreakWin;

	public void Init()
	{
		m_csbtnBattleRecord.PointerClick.RemoveAllListeners();
		m_csbtnBattleRecord.PointerClick.AddListener(OnClickBattleHistory);
		m_csbtnBattleReady.PointerClick.RemoveAllListeners();
		m_csbtnBattleReady.PointerClick.AddListener(OnClickBattleReady);
	}

	public void SetUI()
	{
		_ = NKCScenManager.CurrentUserData()?.m_PvpData;
	}

	private void OnClickBattleHistory()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY()?.OpenBattleRecord(NKM_GAME_TYPE.NGT_INVALID);
	}

	private void OnClickBattleReady()
	{
		Log.Error("일반전 삭제됨", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletLobbyNormal.cs", 53);
	}
}
