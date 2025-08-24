using System;
using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupEquipChange : NKCUIBase
{
	public delegate void OnEquipChangePopupOK();

	public class ItemStatCompare
	{
		public float statValue;

		public float changedStatValue;

		public float statFactor;

		public float changedStatFactor;
	}

	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_UNIT_CHANGE_POPUP";

	public const string UI_ASSET_NAME = "NKM_UI_EQUIP_CHANGE_POPUP";

	public NKCUIComButton m_cbtnOK;

	public NKCUIComButton m_cbtnCancel;

	public NKCUIInvenEquipSlot m_slotBefore;

	public NKCUIInvenEquipSlot m_slotAfter;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private OnEquipChangePopupOK dOnOK;

	public ScrollRect m_ScrollRect;

	public NKCPopupEquipChangeStatSlot m_pfbStatSlot;

	private List<NKCPopupEquipChangeStatSlot> m_lstStatSlots;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "장비 변경 확인";

	public void InitUI()
	{
		m_cbtnOK.PointerClick.AddListener(OnOkButton);
		NKCUtil.SetHotkey(m_cbtnOK, HotkeyEventType.Confirm);
		m_cbtnCancel.PointerClick.AddListener(base.Close);
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		m_lstStatSlots = new List<NKCPopupEquipChangeStatSlot>();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(NKMEquipItemData beforeNKMEquipItemData, NKMEquipItemData afterNKMEquipItemData, OnEquipChangePopupOK onOK, bool bShowFierceInfo = false)
	{
		if (beforeNKMEquipItemData != null && afterNKMEquipItemData != null)
		{
			m_slotBefore.SetData(beforeNKMEquipItemData, bShowFierceInfo);
			m_slotAfter.SetData(afterNKMEquipItemData, bShowFierceInfo);
			SetChangedStat(beforeNKMEquipItemData, afterNKMEquipItemData);
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			dOnOK = onOK;
			m_NKCUIOpenAnimator.PlayOpenAni();
			UIOpened();
		}
	}

	private void SetChangedStat(NKMEquipItemData beforeNKMEquipItemData, NKMEquipItemData afterNKMEquipItemData)
	{
		Dictionary<NKM_STAT_TYPE, ItemStatCompare> dicItemStatCompare = new Dictionary<NKM_STAT_TYPE, ItemStatCompare>();
		if (afterNKMEquipItemData.potentialOptions.Count > 0)
		{
			for (int i = 0; i < afterNKMEquipItemData.potentialOptions.Count; i++)
			{
				if (afterNKMEquipItemData.potentialOptions[i] == null || afterNKMEquipItemData.potentialOptions[i].sockets == null || afterNKMEquipItemData.potentialOptions[i].sockets.Length == 0)
				{
					continue;
				}
				NKMPotentialOption.SocketData[] sockets = afterNKMEquipItemData.potentialOptions[i].sockets;
				for (int j = 0; j < sockets.Length; j++)
				{
					if (sockets.Length > j && sockets[j] != null)
					{
						SetStatCompare(ref dicItemStatCompare, afterNKMEquipItemData.potentialOptions[i].statType, sockets[j].statValue, bBeforeItem: true);
					}
				}
			}
		}
		foreach (EQUIP_ITEM_STAT item in afterNKMEquipItemData.m_Stat)
		{
			float fValue = item.stat_value + (float)afterNKMEquipItemData.m_EnchantLevel * item.stat_level_value;
			SetStatCompare(ref dicItemStatCompare, item.type, fValue, bBeforeItem: true);
		}
		if (beforeNKMEquipItemData.potentialOptions.Count > 0)
		{
			for (int k = 0; k < beforeNKMEquipItemData.potentialOptions.Count; k++)
			{
				if (beforeNKMEquipItemData.potentialOptions[k] == null || beforeNKMEquipItemData.potentialOptions[k].sockets == null || beforeNKMEquipItemData.potentialOptions[k].sockets.Length == 0)
				{
					continue;
				}
				NKMPotentialOption.SocketData[] sockets2 = beforeNKMEquipItemData.potentialOptions[k].sockets;
				for (int l = 0; l < sockets2.Length; l++)
				{
					if (sockets2.Length > l && sockets2[l] != null)
					{
						SetStatCompare(ref dicItemStatCompare, beforeNKMEquipItemData.potentialOptions[k].statType, sockets2[l].statValue * -1f);
					}
				}
			}
		}
		foreach (EQUIP_ITEM_STAT item2 in beforeNKMEquipItemData.m_Stat)
		{
			float num = item2.stat_value + (float)beforeNKMEquipItemData.m_EnchantLevel * item2.stat_level_value;
			SetStatCompare(ref dicItemStatCompare, item2.type, num * -1f);
		}
		foreach (KeyValuePair<NKM_STAT_TYPE, ItemStatCompare> item3 in dicItemStatCompare)
		{
			string statShortName = NKCUtilString.GetStatShortName(item3.Key, item3.Value.statValue);
			string statValue = ((!NKMUnitStatManager.IsPercentStat(item3.Key)) ? NKCUtilString.GetStatShortString("{1:0}", item3.Key, item3.Value.statValue) : NKCUtilString.GetStatShortString("{1:P1}", item3.Key, item3.Value.statValue));
			float num2 = ((NKCUtilString.IsNameReversedIfNegative(item3.Key) && item3.Value.statValue < 0f) ? (0f - item3.Value.changedStatValue) : item3.Value.changedStatValue);
			string text = "";
			string changedValueColor = "";
			text = ((!NKMUnitStatManager.IsPercentStat(item3.Key)) ? $"{num2:+#;-#;''}" : $"{Math.Round(new decimal(num2 * 1000f)) / 1000m:+0.#%;-0.#%;''}");
			if (num2 > 0f)
			{
				text = string.Format("<size=20>{0}</size>{1}", "▲", text);
				changedValueColor = "#A3FF66";
			}
			else if (num2 < 0f)
			{
				text = string.Format("<size=20>{0}</size>{1}", "▼", text);
				changedValueColor = "#FF3D40";
			}
			if (!string.IsNullOrEmpty(text))
			{
				AddChangedStatSlot(statShortName, statValue, text, changedValueColor);
			}
			if (item3.Value.statFactor == 0f && item3.Value.changedStatFactor == 0f)
			{
				continue;
			}
			int num3;
			float num4;
			if (NKCUtilString.IsNameReversedIfNegative(item3.Key))
			{
				num3 = ((item3.Value.statFactor < 0f) ? 1 : 0);
				if (num3 != 0)
				{
					num4 = 0f - item3.Value.statFactor;
					goto IL_042e;
				}
			}
			else
			{
				num3 = 0;
			}
			num4 = item3.Value.statFactor;
			goto IL_042e;
			IL_042e:
			float num5 = num4;
			float num6 = ((num3 != 0) ? (0f - item3.Value.changedStatFactor) : item3.Value.changedStatFactor);
			decimal num7 = new decimal((num5 != 0f) ? num5 : num6);
			statValue = $"{Math.Round(num7 * 1000m) / 1000m:P1}";
			text = $"{statValue:+0.#%;-0.#%;''}";
			if (num7 < 0m)
			{
				statValue = $"{Math.Round(0.0) / 1000.0:P1}";
			}
			if (num6 > 0f)
			{
				text = string.Format("<size=20>{0}</size>{1}", "▲", text);
				changedValueColor = "#A3FF66";
			}
			else if (num6 < 0f)
			{
				text = string.Format("<size=20>{0}</size>{1}", "▼", text);
				changedValueColor = "#FF3D40";
			}
			if (!string.IsNullOrEmpty(text))
			{
				AddChangedStatSlot(statShortName, statValue, text, changedValueColor);
			}
		}
	}

	private void SetStatCompare(ref Dictionary<NKM_STAT_TYPE, ItemStatCompare> dicItemStatCompare, NKM_STAT_TYPE type, float fValue, bool bBeforeItem = false)
	{
		if (dicItemStatCompare.ContainsKey(type))
		{
			if (bBeforeItem)
			{
				dicItemStatCompare[type].statValue += fValue;
			}
			dicItemStatCompare[type].changedStatValue += fValue;
		}
		else
		{
			ItemStatCompare itemStatCompare = new ItemStatCompare();
			itemStatCompare.statValue = (bBeforeItem ? fValue : 0f);
			itemStatCompare.changedStatValue = fValue;
			dicItemStatCompare.Add(type, itemStatCompare);
		}
	}

	private void AddChangedStatSlot(string statShortName, string statValue, string changedValueStr, string changedValueColor)
	{
		NKCPopupEquipChangeStatSlot nKCPopupEquipChangeStatSlot = UnityEngine.Object.Instantiate(m_pfbStatSlot, m_ScrollRect.content);
		if (null != nKCPopupEquipChangeStatSlot)
		{
			nKCPopupEquipChangeStatSlot.SetData(statShortName, statValue, changedValueStr, changedValueColor);
			NKCUtil.SetGameobjectActive(nKCPopupEquipChangeStatSlot, bValue: true);
			m_lstStatSlots.Add(nKCPopupEquipChangeStatSlot);
		}
	}

	public void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
		foreach (NKCPopupEquipChangeStatSlot lstStatSlot in m_lstStatSlots)
		{
			UnityEngine.Object.Destroy(lstStatSlot.gameObject);
		}
		m_lstStatSlots.Clear();
	}

	private void OnOkButton()
	{
		if (dOnOK != null)
		{
			dOnOK();
		}
	}
}
