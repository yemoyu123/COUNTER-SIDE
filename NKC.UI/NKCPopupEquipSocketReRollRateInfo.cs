using System.Collections.Generic;
using System.Linq;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupEquipSocketReRollRateInfo : NKCUIBase
{
	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_factory";

	public const string UI_ASSET_NAME = "NKM_UI_FACTORY_POPUP_SOCKET_RATE";

	private static NKCPopupEquipSocketReRollRateInfo m_Instance;

	[Header("Ÿ\ufffd\ufffdƲ")]
	public GameObject m_objOptionNum;

	public Text m_lbOptionName;

	public GameObject m_objOptionName_2;

	public Text m_lbOptionName_2;

	[Header("\ufffd\ufffdư\ufffd\ufffd")]
	public NKCUIComStateButton m_csbtnOK;

	[Header("etc")]
	public EventTrigger m_evtTrigger;

	[Header("Slot Recttransform")]
	public RectTransform m_rtSocketParent;

	public RectTransform m_rtRateParent;

	[Header("Prefab")]
	public NKCPopupEquipSocketReRollRateInfoSlot m_objPrefab;

	private List<NKCPopupEquipSocketReRollRateInfoSlot> m_lstReatSlot = new List<NKCPopupEquipSocketReRollRateInfoSlot>();

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "\ufffd\ufffd\ufffd\ufffd \ufffdɷ\ufffdġ \ufffd缳\ufffd\ufffd Ȯ\ufffd\ufffd";

	public static NKCPopupEquipSocketReRollRateInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupEquipSocketReRollRateInfo>("ab_ui_nkm_ui_factory", "NKM_UI_FACTORY_POPUP_SOCKET_RATE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupEquipSocketReRollRateInfo>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public void InitUI()
	{
		NKCUtil.SetBindFunction(m_csbtnOK, base.Close);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		if (null == m_rtSocketParent || null == m_rtRateParent)
		{
			Debug.LogError("NKCPopupEquipSocketReRollRateInfo::InitUI - can not found m_rtSocketParent / m_rtRateParent");
		}
		else if (m_evtTrigger != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				Close();
			});
			m_evtTrigger.triggers.Add(entry);
		}
	}

	public void Open(NKM_STAT_TYPE statType, int potentialOpKey, NKM_STAT_TYPE statType_2 = NKM_STAT_TYPE.NST_RANDOM, int potentialOpKey_2 = 0)
	{
		NKMPotentialOptionTemplet nKMPotentialOptionTemplet = NKMPotentialOptionTemplet.Find(potentialOpKey);
		if (nKMPotentialOptionTemplet == null || nKMPotentialOptionTemplet.sockets.Length == 0)
		{
			return;
		}
		List<NKMPotentialSocketTemplet> list = new List<NKMPotentialSocketTemplet>();
		List<NKMPotentialSocketTemplet> list2 = new List<NKMPotentialSocketTemplet>();
		if (nKMPotentialOptionTemplet.StatType == statType)
		{
			list = nKMPotentialOptionTemplet.sockets.ToList();
			bool flag = false;
			bool bNegative = false;
			if (NKCUtilString.IsNameReversedIfNegative(statType))
			{
				foreach (NKMPotentialSocketTemplet item in list)
				{
					if (item.IsPrecentStat && item.MinStat < 0f && item.MaxStat < 0f)
					{
						flag = true;
					}
				}
			}
			NKCUtil.SetLabelText(m_lbOptionName, NKCUtilString.GetStatShortName(statType, flag));
			NKCUtil.SetGameobjectActive(m_objOptionNum, statType_2 != NKM_STAT_TYPE.NST_RANDOM);
			NKCUtil.SetGameobjectActive(m_objOptionName_2, statType_2 != NKM_STAT_TYPE.NST_RANDOM);
			if (statType_2 != NKM_STAT_TYPE.NST_RANDOM)
			{
				nKMPotentialOptionTemplet = NKMPotentialOptionTemplet.Find(potentialOpKey_2);
				if (nKMPotentialOptionTemplet == null || nKMPotentialOptionTemplet.sockets.Length == 0)
				{
					return;
				}
				list2 = nKMPotentialOptionTemplet.sockets.ToList();
				if (NKCUtilString.IsNameReversedIfNegative(statType))
				{
					foreach (NKMPotentialSocketTemplet item2 in list2)
					{
						if (item2.IsPrecentStat && item2.MinStat < 0f && item2.MaxStat < 0f)
						{
							bNegative = true;
						}
					}
				}
				NKCUtil.SetLabelText(m_lbOptionName_2, NKCUtilString.GetStatShortName(statType_2, bNegative));
			}
			Dictionary<int, List<NKMPrecisionWeightTemplet.PrecisionWeightEntity>> dicPrecisionWeight = NKMPrecisionWeightTemplet.m_dicPrecisionWeight;
			NKMUnitStatManager.IsPercentStat(statType);
			NKMUnitStatManager.IsPercentStat(statType_2);
			foreach (KeyValuePair<int, List<NKMPrecisionWeightTemplet.PrecisionWeightEntity>> item3 in dicPrecisionWeight)
			{
				if (item3.Key != nKMPotentialOptionTemplet.rerollPrecisionWeightId)
				{
					continue;
				}
				for (int i = 0; i < item3.Value.Count(); i++)
				{
					string strDesc = string.Format(NKCUtilString.GET_STRING_EQUIP_REROLL_RATE_TEXT, (float)item3.Value[i].Weight * 0.01f);
					AddSlot(strDesc, string.Empty, m_rtRateParent);
					int precision = item3.Value[i].Precision;
					for (int j = 0; j < list.Count; j++)
					{
						float num = list[j].CalcStat(precision);
						string strDesc_ = string.Empty;
						if (flag)
						{
							num = Mathf.Abs(num);
						}
						if (list2.Count > 0)
						{
							float num2 = list2[j].CalcStat(precision);
							if (flag)
							{
								num2 = Mathf.Abs(num2);
							}
							strDesc_ = NKCUtil.GetPotentialSocketStatText(NKMUnitStatManager.IsPercentStat(statType_2), num2, bShowDetail: true);
						}
						AddSlot(NKCUtil.GetPotentialSocketStatText(NKMUnitStatManager.IsPercentStat(statType), num, bShowDetail: true), strDesc_, m_rtSocketParent);
					}
				}
			}
		}
		UIOpened();
	}

	private void AddSlot(string strDesc, string strDesc_2, RectTransform m_Parent)
	{
		if (!(null == m_objPrefab))
		{
			NKCPopupEquipSocketReRollRateInfoSlot nKCPopupEquipSocketReRollRateInfoSlot = Object.Instantiate(m_objPrefab, m_Parent);
			if (null != nKCPopupEquipSocketReRollRateInfoSlot)
			{
				NKCUtil.SetGameobjectActive(nKCPopupEquipSocketReRollRateInfoSlot, bValue: true);
				nKCPopupEquipSocketReRollRateInfoSlot.transform.localScale = Vector3.one;
				nKCPopupEquipSocketReRollRateInfoSlot.SetData(strDesc, strDesc_2);
				m_lstReatSlot.Add(nKCPopupEquipSocketReRollRateInfoSlot);
			}
			else
			{
				Debug.Log("<color=red>NKCPopupEquipSocketReRollRateInfo::AddSlot() - Fail</color>");
			}
		}
	}

	public override void CloseInternal()
	{
		foreach (NKCPopupEquipSocketReRollRateInfoSlot item in m_lstReatSlot)
		{
			Object.Destroy(item.gameObject);
		}
		m_lstReatSlot.Clear();
		base.gameObject.SetActive(value: false);
	}
}
