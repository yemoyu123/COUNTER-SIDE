using System.Collections.Generic;
using Cs.Logging;

namespace NKC;

public class NKCMMPAirbridge : NKCMMPModule
{
	private const string logPrefix = "NKCMMPAirbridge";

	private HashSet<string> m_eventIdList = new HashSet<string>();

	private bool m_enableLogging;

	public override void Init(bool enableLogging)
	{
		RegisterEventCode();
		m_enableLogging = enableLogging;
	}

	public override void SendEvent(string eventId)
	{
		if (!m_eventIdList.Contains(eventId))
		{
			Log.Debug("[NKCMMPAirbridge] eventId not exist. [" + eventId + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPAirbridge.cs", 35);
		}
		else if (m_enableLogging)
		{
			Log.Debug("[NKCMMPAirbridge] SendMMPEvent eventId[" + eventId + "]]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPAirbridge.cs", 41);
		}
	}

	public override void SendRevenueEvent(string eventId, int productId, double localPrice, string priceCurrency)
	{
		if (m_enableLogging)
		{
			Log.Debug("[NKCMMPAirbridge] SendMMPRevenueEvent eventId[" + eventId + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPAirbridge.cs", 54);
		}
	}

	private void RegisterEventCode()
	{
		m_eventIdList.Clear();
		m_eventIdList.Add("00_first_purchase");
		m_eventIdList.Add("01_appLaunch");
		m_eventIdList.Add("02_downLoad_start");
		m_eventIdList.Add("03_downLoad_complete");
		m_eventIdList.Add("04_loading_start");
		m_eventIdList.Add("05_loading_complete");
		m_eventIdList.Add("06_login_complete");
		m_eventIdList.Add("07_prologue");
		m_eventIdList.Add("08_ep1_1_1_stage_end");
		m_eventIdList.Add("09_ep1_1_1_stage_start");
		m_eventIdList.Add("10_ep1_1_2_stage_start");
		m_eventIdList.Add("11_ep1_1_3_stage_start");
		m_eventIdList.Add("12_ep1_1_4_stage_start");
		m_eventIdList.Add("13_ep1_1_4_cutscene2_start");
		m_eventIdList.Add("14_tutorial_hq1_start");
		m_eventIdList.Add("15_username_creation");
		m_eventIdList.Add("16_tutorial_hq2_ceooffice");
		m_eventIdList.Add("17_tutorial_gacha_newbie");
		m_eventIdList.Add("18_tutorial_missions");
		m_eventIdList.Add("19_ep1_2_1_missionstart");
		m_eventIdList.Add("20_ep1_2_1_cutscene1_start");
		m_eventIdList.Add("21_ep1_2_1_stage_start");
		m_eventIdList.Add("22_ep1_2_1_stage_result");
		m_eventIdList.Add("23_ep1_2_2_stage_start");
		m_eventIdList.Add("24_ep1_2_3_stage_start");
		m_eventIdList.Add("25_ep1_2_4_stage_start");
		m_eventIdList.Add("26_ep1_2_4_cutscene2_start");
		m_eventIdList.Add("27_gacha_newbie_special");
		m_eventIdList.Add("28_level_02");
		m_eventIdList.Add("29_level_03");
		m_eventIdList.Add("30_level_04");
		m_eventIdList.Add("31_level_05");
		m_eventIdList.Add("32_level_06");
		m_eventIdList.Add("33_level_07");
		m_eventIdList.Add("34_level_08");
		m_eventIdList.Add("35_level_09");
		m_eventIdList.Add("36_level_10");
		m_eventIdList.Add("37_level_20");
		m_eventIdList.Add("38_level_30");
		m_eventIdList.Add("39_level_40");
		m_eventIdList.Add("40_level_50");
		m_eventIdList.Add("41_Beginner");
		m_eventIdList.Add("42_N_Recuitment");
		m_eventIdList.Add("43_C_Recuitment");
		m_eventIdList.Add("44_first_coin_use");
		Log.Debug(string.Format("[{0}] RegisterEventCode Count [{1}]", "NKCMMPAirbridge", m_eventIdList.Count), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPAirbridge.cs", 155);
	}
}
