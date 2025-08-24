using System.Collections.Generic;
using Cs.Logging;

namespace NKC;

public class NKCMMPAdjust : NKCMMPModule
{
	private const string logPrefix = "NKCMMPAdjust";

	private Dictionary<string, string> m_eventIdToEventCodeList = new Dictionary<string, string>();

	private bool m_enableLogging;

	public override void Init(bool enableLogging)
	{
		RegisterEventCode();
		m_enableLogging = enableLogging;
	}

	public override void SendEvent(string eventId)
	{
		if (!m_eventIdToEventCodeList.TryGetValue(eventId, out var value))
		{
			Log.Debug("[NKCMMPAdjust] eventCode not exist. [" + eventId + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPAdjust.cs", 58);
		}
		else if (string.IsNullOrEmpty(value))
		{
			Log.Debug("[NKCMMPAdjust] eventCode is null or empty. eventId[" + eventId + "] eventCode[" + value + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPAdjust.cs", 64);
		}
		else if (m_enableLogging)
		{
			Log.Debug("[NKCMMPAdjust] SendMMPeEvent eventId[" + eventId + "] eventCode[" + value + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPAdjust.cs", 70);
		}
	}

	public override void SendRevenueEvent(string eventId, int productId, double localPrice, string priceCurrency)
	{
		if (!m_eventIdToEventCodeList.TryGetValue(eventId, out var value))
		{
			Log.Debug("[NKCMMPAdjust] eventCode not exist. [" + eventId + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPAdjust.cs", 83);
		}
		else if (string.IsNullOrEmpty(value))
		{
			Log.Debug("[NKCMMPAdjust] eventCode is null or empty. eventId[" + eventId + "] eventCode[" + value + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPAdjust.cs", 89);
		}
		else if (m_enableLogging)
		{
			Log.Debug("[NKCMMPAdjust] SendMMPRevenueEvent eventId[" + eventId + "] eventCode[" + value + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPAdjust.cs", 95);
		}
	}

	private void RegisterEventCode()
	{
		m_eventIdToEventCodeList.Clear();
		m_eventIdToEventCodeList.Add("00_first_purchase", "dxdwev");
		m_eventIdToEventCodeList.Add("01_appLaunch", "75i8a5");
		m_eventIdToEventCodeList.Add("02_downLoad_start", "pr6ym3");
		m_eventIdToEventCodeList.Add("03_downLoad_complete", "5e9eo5");
		m_eventIdToEventCodeList.Add("04_loading_start", "6tglx6");
		m_eventIdToEventCodeList.Add("05_loading_complete", "xzedrv");
		m_eventIdToEventCodeList.Add("06_login_complete", "9x016h");
		m_eventIdToEventCodeList.Add("07_prologue", "5pdniy");
		m_eventIdToEventCodeList.Add("08_ep1_1_1_stage_start", "stk6ev");
		m_eventIdToEventCodeList.Add("09_ep1_1_2_stage_start", "160za9");
		m_eventIdToEventCodeList.Add("10_ep1_1_3_stage_start", "1ugryd");
		m_eventIdToEventCodeList.Add("11_ep1_1_4_stage_start", "xtp8jt");
		m_eventIdToEventCodeList.Add("12_ep1_1_4_cutscene2_start", "qwqpog");
		m_eventIdToEventCodeList.Add("12_tutorial_hq1_start", "c8hcek");
		m_eventIdToEventCodeList.Add("13_username_creation", "inbde0");
		m_eventIdToEventCodeList.Add("14_tutorial_hq2_ceooffice", "1g5joo");
		m_eventIdToEventCodeList.Add("15_tutorial_gacha_newbie", "2pl5bl");
		m_eventIdToEventCodeList.Add("16_tutorial_missions", "6j4mk3");
		m_eventIdToEventCodeList.Add("17_ep1_2_1_missionstart", "sztouq");
		m_eventIdToEventCodeList.Add("18_ep1_2_1_cutscene1_start", "74wipr");
		m_eventIdToEventCodeList.Add("19_ep1_2_1_stage_start", "8sslcw");
		m_eventIdToEventCodeList.Add("20_ep1_2_1_stage_result", "odtmx4");
		m_eventIdToEventCodeList.Add("21_ep1_2_2_stage_start", "81dn1z");
		m_eventIdToEventCodeList.Add("23_ep1_2_3_stage_start", "z8itse");
		m_eventIdToEventCodeList.Add("24_ep1_2_4_stage_start", "3a8kwj");
		m_eventIdToEventCodeList.Add("25_ep1_2_4_cutscene2_start", "76h47v");
		m_eventIdToEventCodeList.Add("26_gacha_newbie_special", "dqpo2x");
		m_eventIdToEventCodeList.Add("27_level_05", "t1o2ko");
		m_eventIdToEventCodeList.Add("28_level_10", "np8e88");
		m_eventIdToEventCodeList.Add("29_level_20", "7fcj81");
		m_eventIdToEventCodeList.Add("30_level_30", "2d0kfd");
		m_eventIdToEventCodeList.Add("31_level_40", "i4pybd");
		m_eventIdToEventCodeList.Add("32_level_50", "9jg2wb");
		m_eventIdToEventCodeList.Add("33_ApprenticeAdmin", "twirdf");
		m_eventIdToEventCodeList.Add("34_AD_eternium", "r26dsm");
		m_eventIdToEventCodeList.Add("35_AD_ch_inventory", "96vzgd");
		m_eventIdToEventCodeList.Add("36_shop_coin_3401", "2v0qpb");
		m_eventIdToEventCodeList.Add("36_shop_coin_3402", "7g9lhn");
		m_eventIdToEventCodeList.Add("36_shop_coin_3403", "wdeok5");
		m_eventIdToEventCodeList.Add("36_shop_coin_3404", "vwsx8g");
		m_eventIdToEventCodeList.Add("36_shop_coin_3405", "jbsngh");
		m_eventIdToEventCodeList.Add("36_shop_coin_3406", "hqxphd");
		m_eventIdToEventCodeList.Add("36_shop_coin_160384", "5s4eu7");
		m_eventIdToEventCodeList.Add("36_shop_coin_160263", "pyagtv");
		m_eventIdToEventCodeList.Add("36_shop_coin_160517", "4gfgfj");
		m_eventIdToEventCodeList.Add("36_shop_coin_160492", "oqn1wo");
		m_eventIdToEventCodeList.Add("36_shop_coin_160466", "1xr0cx");
		m_eventIdToEventCodeList.Add("36_shop_coin_160541", "xf9gt1");
		m_eventIdToEventCodeList.Add("36_shop_coin_160542", "s0g5e1");
		m_eventIdToEventCodeList.Add("36_shop_coin_160561", "z86u46");
		m_eventIdToEventCodeList.Add("36_shop_coin_160478", "hg9mur");
		m_eventIdToEventCodeList.Add("36_shop_coin_160479", "r4tqpv");
		m_eventIdToEventCodeList.Add("36_shop_coin_160480", "dong3w");
		m_eventIdToEventCodeList.Add("36_shop_coin_160481", "lrfq3x");
		m_eventIdToEventCodeList.Add("36_shop_coin_160452", "i84qh4");
		m_eventIdToEventCodeList.Add("36_shop_coin_160453", "ex0cqd");
		m_eventIdToEventCodeList.Add("36_shop_coin_160454", "kn9dqw");
		m_eventIdToEventCodeList.Add("36_shop_coin_160455", "i6hhyp");
		m_eventIdToEventCodeList.Add("36_shop_coin_160500", "togmnl");
		m_eventIdToEventCodeList.Add("36_shop_coin_160491", "6ds7q0");
		m_eventIdToEventCodeList.Add("36_shop_coin_160270", "ncpwpr");
		m_eventIdToEventCodeList.Add("36_shop_coin_160520", "2qzu6t");
		m_eventIdToEventCodeList.Add("36_shop_coin_160076", "iuwbh3");
		m_eventIdToEventCodeList.Add("36_shop_coin_160398", "lp5pke");
		m_eventIdToEventCodeList.Add("36_shop_coin_160399", "n38rv5");
		m_eventIdToEventCodeList.Add("36_shop_coin_160400", "rvzu2h");
		m_eventIdToEventCodeList.Add("36_shop_coin_160519", "x42les");
		m_eventIdToEventCodeList.Add("36_shop_coin_160518", "760g31");
		m_eventIdToEventCodeList.Add("36_shop_coin_160271", "l8etw6");
		m_eventIdToEventCodeList.Add("36_shop_coin_160272", "j8q8mq");
		m_eventIdToEventCodeList.Add("36_shop_coin_160209", "d9swng");
		m_eventIdToEventCodeList.Add("36_shop_coin_160210", "qvg55d");
		m_eventIdToEventCodeList.Add("36_shop_coin_160211", "kd8nbw");
		m_eventIdToEventCodeList.Add("36_shop_coin_160212", "7jfxtl");
		m_eventIdToEventCodeList.Add("36_shop_coin_160222", "pofy68");
		m_eventIdToEventCodeList.Add("36_shop_coin_160449", "5kuzlb");
		m_eventIdToEventCodeList.Add("36_shop_coin_160407", "1b1w3k");
		m_eventIdToEventCodeList.Add("36_shop_coin_160408", "w6qyeq");
		m_eventIdToEventCodeList.Add("36_shop_coin_160409", "fz8vx7");
		m_eventIdToEventCodeList.Add("36_shop_coin_160364", "mstgj6");
		m_eventIdToEventCodeList.Add("36_shop_coin_160363", "cu8yzb");
		m_eventIdToEventCodeList.Add("36_shop_coin_160504", "4su9jf");
		m_eventIdToEventCodeList.Add("36_shop_coin_160505", "l1t9xt");
		m_eventIdToEventCodeList.Add("36_shop_coin_160506", "283v0m");
		m_eventIdToEventCodeList.Add("36_shop_coin_160279", "cn9n97");
		m_eventIdToEventCodeList.Add("36_shop_coin_160459", "rehrwn");
		m_eventIdToEventCodeList.Add("36_shop_coin_160460", "pdlado");
		m_eventIdToEventCodeList.Add("36_shop_coin_160461", "gifppd");
		m_eventIdToEventCodeList.Add("36_shop_coin_160524", "743msu");
		m_eventIdToEventCodeList.Add("36_shop_coin_160525", "iv4e29");
		m_eventIdToEventCodeList.Add("36_shop_coin_160417", "qzp7tu");
		m_eventIdToEventCodeList.Add("36_shop_coin_160418", "31v747");
		m_eventIdToEventCodeList.Add("36_shop_coin_160419", "ndzuky");
		m_eventIdToEventCodeList.Add("36_shop_coin_160442", "hrqreu");
		m_eventIdToEventCodeList.Add("36_shop_coin_160443", "lnflyo");
		m_eventIdToEventCodeList.Add("36_shop_coin_160388", "uzs1w7");
		m_eventIdToEventCodeList.Add("36_shop_coin_5014", "10k48u");
		m_eventIdToEventCodeList.Add("36_shop_coin_5076", "5438pl");
		m_eventIdToEventCodeList.Add("36_shop_coin_5081", "njyw2c");
		m_eventIdToEventCodeList.Add("36_shop_coin_5004", "nkty2g");
		m_eventIdToEventCodeList.Add("36_shop_coin_5015", "jmu89z");
		m_eventIdToEventCodeList.Add("36_shop_coin_5011", "cx4w9o");
		m_eventIdToEventCodeList.Add("36_shop_coin_5005", "zaflnt");
		m_eventIdToEventCodeList.Add("36_shop_coin_5010", "vkeecl");
		m_eventIdToEventCodeList.Add("36_shop_coin_5024", "sye9bi");
		m_eventIdToEventCodeList.Add("36_shop_coin_5042", "u8l0ix");
		m_eventIdToEventCodeList.Add("36_shop_coin_5043", "q138r8");
		m_eventIdToEventCodeList.Add("36_shop_coin_5044", "p2wkj7");
		m_eventIdToEventCodeList.Add("36_shop_coin_5045", "ddlqa7");
		m_eventIdToEventCodeList.Add("36_shop_coin_5070", "k45bp1");
		m_eventIdToEventCodeList.Add("36_shop_coin_5051", "qi6iu7");
		m_eventIdToEventCodeList.Add("36_shop_coin_5053", "b7f2ai");
		m_eventIdToEventCodeList.Add("36_shop_coin_5012", "laljj7");
		m_eventIdToEventCodeList.Add("36_shop_coin_5050", "bftqfn");
		m_eventIdToEventCodeList.Add("36_shop_coin_5063", "mhaf24");
		m_eventIdToEventCodeList.Add("36_shop_coin_5062", "x5sahy");
		m_eventIdToEventCodeList.Add("36_shop_coin_160462", "53hmu3");
		m_eventIdToEventCodeList.Add("36_shop_coin_160463", "mgk3n5");
		m_eventIdToEventCodeList.Add("36_shop_coin_160387", "ncfskk");
		m_eventIdToEventCodeList.Add("36_shop_coin_160324", "7sza4i");
		m_eventIdToEventCodeList.Add("36_shop_coin_160325", "u3xsbn");
		m_eventIdToEventCodeList.Add("36_shop_coin_160434", "x5l3ug");
		m_eventIdToEventCodeList.Add("36_shop_coin_160521", "uxk8hc");
		m_eventIdToEventCodeList.Add("36_shop_coin_160394", "u4onu8");
		m_eventIdToEventCodeList.Add("36_shop_coin_160395", "vx888e");
		m_eventIdToEventCodeList.Add("36_shop_coin_160396", "t49sp4");
		m_eventIdToEventCodeList.Add("36_shop_coin_5047", "j0sfnz");
		m_eventIdToEventCodeList.Add("36_shop_coin_5079", "tjpmfu");
		m_eventIdToEventCodeList.Add("36_shop_coin_5046", "tajok0");
		m_eventIdToEventCodeList.Add("36_shop_coin_5078", "kcgwbq");
		m_eventIdToEventCodeList.Add("36_shop_coin_5019", "1r8xc7");
		m_eventIdToEventCodeList.Add("36_shop_coin_5020", "qnc6g5");
		m_eventIdToEventCodeList.Add("36_shop_coin_5021", "jijl62");
		m_eventIdToEventCodeList.Add("36_shop_coin_5022", "j2jkjh");
		m_eventIdToEventCodeList.Add("36_shop_coin_5049", "ktvo10");
		m_eventIdToEventCodeList.Add("36_shop_coin_160523", "8hmieb");
		m_eventIdToEventCodeList.Add("36_shop_coin_160522", "v9y7bw");
		m_eventIdToEventCodeList.Add("36_shop_coin_160482", "7s2gvt");
		m_eventIdToEventCodeList.Add("36_shop_coin_160531", "v7m65k");
		m_eventIdToEventCodeList.Add("36_shop_coin_160532", "8ndwvi");
		m_eventIdToEventCodeList.Add("36_shop_coin_160386", "8xaelf");
		m_eventIdToEventCodeList.Add("36_shop_coin_160483", "g9qfhb");
		m_eventIdToEventCodeList.Add("36_shop_coin_160511", "i6wcxi");
		m_eventIdToEventCodeList.Add("36_shop_coin_160512", "z7zy6z");
		m_eventIdToEventCodeList.Add("36_shop_coin_160390", "hwfr99");
		m_eventIdToEventCodeList.Add("36_shop_coin_160526", "uqgavu");
		m_eventIdToEventCodeList.Add("36_shop_coin_160527", "9b0oic");
		m_eventIdToEventCodeList.Add("36_shop_coin_160457", "mb3skf");
		m_eventIdToEventCodeList.Add("36_shop_coin_160566", "d53xek");
		m_eventIdToEventCodeList.Add("36_shop_coin_160464", "x7r7pb");
		m_eventIdToEventCodeList.Add("36_shop_coin_160465", "qfqj2x");
		m_eventIdToEventCodeList.Add("36_shop_coin_160392", "e5bbt9");
		m_eventIdToEventCodeList.Add("36_shop_coin_5069", "9ysst3");
		m_eventIdToEventCodeList.Add("36_shop_coin_160582", "29ri6c");
		m_eventIdToEventCodeList.Add("36_shop_coin_5041", "t9o14d");
		m_eventIdToEventCodeList.Add("36_shop_coin_5038", "j5fz5j");
		m_eventIdToEventCodeList.Add("36_shop_coin_5033", "8dks6m");
		m_eventIdToEventCodeList.Add("36_shop_coin_5034", "5fz06w");
		m_eventIdToEventCodeList.Add("36_shop_coin_5036", "uige7q");
		m_eventIdToEventCodeList.Add("36_shop_coin_5039", "v3ze2w");
		m_eventIdToEventCodeList.Add("36_shop_coin_5040", "5hwk3s");
		m_eventIdToEventCodeList.Add("36_shop_coin_5037", "hbbasd");
		m_eventIdToEventCodeList.Add("36_shop_coin_5029", "c4jiu6");
		m_eventIdToEventCodeList.Add("36_shop_coin_5030", "poow0z");
		m_eventIdToEventCodeList.Add("36_shop_coin_5031", "8k4j55");
		m_eventIdToEventCodeList.Add("36_shop_coin_5032", "x0j1vk");
		m_eventIdToEventCodeList.Add("36_shop_coin_160507", "bacizu");
		m_eventIdToEventCodeList.Add("36_shop_coin_160508", "1u33z6");
		m_eventIdToEventCodeList.Add("36_shop_coin_160385", "ub7i3i");
		m_eventIdToEventCodeList.Add("36_shop_coin_5064", "g9iyop");
		m_eventIdToEventCodeList.Add("36_shop_coin_5068", "6odw6y");
		m_eventIdToEventCodeList.Add("36_shop_coin_5065", "dxltti");
		m_eventIdToEventCodeList.Add("36_shop_coin_5066", "tmt6nr");
		m_eventIdToEventCodeList.Add("36_shop_coin_5067", "695fgl");
		m_eventIdToEventCodeList.Add("36_shop_coin_160432", "kseu41");
		m_eventIdToEventCodeList.Add("36_shop_coin_160433", "npe8m5");
		m_eventIdToEventCodeList.Add("36_shop_coin_160391", "du8lpt");
		m_eventIdToEventCodeList.Add("36_shop_coin_160456", "wxl2gd");
		m_eventIdToEventCodeList.Add("36_shop_coin_5101", "fubpfm");
		m_eventIdToEventCodeList.Add("36_shop_coin_5099", "2da8tm");
		m_eventIdToEventCodeList.Add("36_shop_coin_5100", "xhi9lg");
		m_eventIdToEventCodeList.Add("36_shop_coin_5061", "iyyqth");
		m_eventIdToEventCodeList.Add("36_shop_coin_5060", "ot8961");
		m_eventIdToEventCodeList.Add("36_shop_coin_5056", "5f32po");
		m_eventIdToEventCodeList.Add("36_shop_coin_5057", "256e2q");
		m_eventIdToEventCodeList.Add("36_shop_coin_5058", "zed1e4");
		m_eventIdToEventCodeList.Add("36_shop_coin_5059", "3z9qbi");
		m_eventIdToEventCodeList.Add("36_shop_coin_5109", "u2hr4g");
		m_eventIdToEventCodeList.Add("36_shop_coin_5108", "lwgnne");
		m_eventIdToEventCodeList.Add("36_shop_coin_5107", "jl1mj7");
		m_eventIdToEventCodeList.Add("36_shop_coin_5106", "18k0mr");
		m_eventIdToEventCodeList.Add("36_shop_coin_160509", "rny9wd");
		m_eventIdToEventCodeList.Add("36_shop_coin_160510", "3sfx8j");
		m_eventIdToEventCodeList.Add("36_shop_coin_160393", "l6akke");
		m_eventIdToEventCodeList.Add("36_shop_coin_160612", "14prph");
		m_eventIdToEventCodeList.Add("36_shop_coin_160614", "3cupm8");
		m_eventIdToEventCodeList.Add("36_shop_coin_160610", "hyzq9h");
		m_eventIdToEventCodeList.Add("36_shop_coin_160613", "ih2zsi");
		m_eventIdToEventCodeList.Add("36_shop_coin_160611", "mo1esf");
		m_eventIdToEventCodeList.Add("36_shop_coin_190912", "ysaerw");
		m_eventIdToEventCodeList.Add("36_shop_coin_160615", "aq17ki");
		m_eventIdToEventCodeList.Add("36_shop_coin_160609", "ho0vw1");
		m_eventIdToEventCodeList.Add("36_shop_coin_160599", "pyxb4p");
		m_eventIdToEventCodeList.Add("36_shop_coin_160405", "bw0463");
		m_eventIdToEventCodeList.Add("36_shop_coin_160406", "pwz613");
		m_eventIdToEventCodeList.Add("36_shop_coin_160389", "c2hz1w");
		m_eventIdToEventCodeList.Add("36_shop_coin_160606", "fvmw98");
		m_eventIdToEventCodeList.Add("36_shop_coin_5087", "fchxzu");
		m_eventIdToEventCodeList.Add("36_shop_coin_5088", "xr892y");
		m_eventIdToEventCodeList.Add("36_shop_coin_5085", "ipib46");
		m_eventIdToEventCodeList.Add("36_shop_coin_5086", "eqssnl");
		m_eventIdToEventCodeList.Add("36_shop_coin_5072", "uzpoa1");
		m_eventIdToEventCodeList.Add("36_shop_coin_5074", "zgcixw");
		m_eventIdToEventCodeList.Add("36_shop_coin_5073", "52w1vk");
		m_eventIdToEventCodeList.Add("38_counterpass_sp", "2cphzq");
		m_eventIdToEventCodeList.Add("38_counterpass_spplus", "io8k80");
		m_eventIdToEventCodeList.Add("38_counterpass_lvup", "ydkno0");
		m_eventIdToEventCodeList.Add("37_inapp_purchase", "qwrar4");
		Log.Debug(string.Format("[{0}] RegisterEventCode Count [{1}]", "NKCMMPAdjust", m_eventIdToEventCodeList.Count), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/MMP/NKCMMPAdjust.cs", 347);
	}
}
