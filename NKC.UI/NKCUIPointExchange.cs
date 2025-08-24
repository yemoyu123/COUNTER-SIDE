using System;
using System.Linq;
using Cs.Logging;
using NKC.UI.Tooltip;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPointExchange : MonoBehaviour
{
	[Serializable]
	public struct PointCoinInfo
	{
		public GameObject objRoot;

		public Image pointCoinImage;

		public Text pointCountText;

		public NKCUIComStateButton imageButton;

		private int itemId;

		private NKCUISlot.SlotData slotData;

		public void Init(int pointCoinId)
		{
			itemId = pointCoinId;
			NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(pointCoinId);
			if (nKMItemMiscTemplet == null)
			{
				Log.Debug($"PointExchange PointItem (Id: {pointCoinId}) is not exist", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIPointExchange.cs", 32);
				NKCUtil.SetGameobjectActive(objRoot, bValue: false);
				return;
			}
			NKCUtil.SetGameobjectActive(objRoot, bValue: true);
			NKCUtil.SetImageSprite(pointCoinImage, NKCResourceUtility.GetOrLoadMiscItemSmallIcon(nKMItemMiscTemplet));
			if (imageButton != null)
			{
				imageButton.PointerDown.RemoveAllListeners();
				imageButton.PointerDown.AddListener(OnImageDown);
			}
		}

		public void SetCount(NKMUserData userData)
		{
			if (objRoot.activeSelf && userData != null)
			{
				NKCUtil.SetLabelText(pointCountText, userData.m_InventoryData.GetCountMiscItem(itemId).ToString());
			}
		}

		public void Release()
		{
			slotData = null;
		}

		private void OnImageDown(PointerEventData eventData)
		{
			if (slotData == null)
			{
				NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
				if (nKMUserData == null)
				{
					return;
				}
				long countMiscItem = nKMUserData.m_InventoryData.GetCountMiscItem(itemId);
				slotData = NKCUISlot.SlotData.MakeMiscItemData(itemId, countMiscItem);
			}
			NKCUITooltip.Instance.Open(slotData, eventData.position);
		}
	}

	public delegate void OnClose();

	public delegate void OnInformation();

	public PointCoinInfo[] m_pointCoinInfoArray;

	public Text m_lbEventTime;

	public LoopScrollFlexibleRect m_loopScrollRect;

	public NKCUIComStateButton m_csbtnClose;

	public NKCUIComStateButton m_csbtnMissionInfo;

	public string m_bgmName;

	private NKMPointExchangeTemplet m_pointExchangeTemplet;

	private OnClose m_dOnClose;

	private OnInformation m_dOnInformation;

	public bool PlayingBGM { get; private set; }

	public void Init(OnClose onClose, OnInformation onInformation)
	{
		m_pointExchangeTemplet = NKMPointExchangeTemplet.GetByTime(NKCSynchronizedTime.ServiceTime);
		if (m_loopScrollRect != null)
		{
			m_loopScrollRect.dOnGetObject += GetPresetSlot;
			m_loopScrollRect.dOnReturnObject += ReturnPresetSlot;
			m_loopScrollRect.dOnProvideData += ProvidePresetData;
			m_loopScrollRect.ContentConstraintCount = 1;
			m_loopScrollRect.TotalCount = 1;
			m_loopScrollRect.PrepareCells();
		}
		if (m_pointExchangeTemplet != null)
		{
			if (NKCSynchronizedTime.GetTimeLeft(NKMTime.LocalToUTC(m_pointExchangeTemplet.EndDate)).TotalDays > (double)NKCSynchronizedTime.UNLIMITD_REMAIN_DAYS)
			{
				NKCUtil.SetLabelText(m_lbEventTime, NKCUtilString.GET_STRING_EVENT_DATE_UNLIMITED_TEXT);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbEventTime, NKCUtilString.GetTimeIntervalString(m_pointExchangeTemplet.StartDate, m_pointExchangeTemplet.EndDate, NKMTime.INTERVAL_FROM_UTC));
			}
		}
		int num = ((m_pointExchangeTemplet != null) ? m_pointExchangeTemplet.UsePointId.Count : 0);
		int num2 = ((m_pointCoinInfoArray != null) ? m_pointCoinInfoArray.Length : 0);
		NKMUserData count = NKCScenManager.CurrentUserData();
		for (int i = 0; i < num2; i++)
		{
			if (i >= num)
			{
				NKCUtil.SetGameobjectActive(m_pointCoinInfoArray[i].objRoot, bValue: false);
				continue;
			}
			m_pointCoinInfoArray[i].Init(m_pointExchangeTemplet.UsePointId[i]);
			m_pointCoinInfoArray[i].SetCount(count);
		}
		m_dOnClose = onClose;
		m_dOnInformation = onInformation;
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, OnClickClose);
		NKCUtil.SetButtonClickDelegate(m_csbtnMissionInfo, OnClickMissionInformation);
	}

	public void ResetUI()
	{
		RefreshPoint();
		m_loopScrollRect?.SetIndexPosition(0);
	}

	public void RefreshPoint()
	{
		int num = ((m_pointCoinInfoArray != null) ? m_pointCoinInfoArray.Length : 0);
		NKMUserData count = NKCScenManager.CurrentUserData();
		for (int i = 0; i < num; i++)
		{
			m_pointCoinInfoArray[i].SetCount(count);
		}
	}

	public void RefreshScrollRect()
	{
		m_loopScrollRect?.RefreshCells();
	}

	public void PlayMusic()
	{
		if (!string.IsNullOrEmpty(m_bgmName) && !PlayingBGM)
		{
			NKCSoundManager.PlayMusic(m_bgmName, bLoop: true);
			PlayingBGM = true;
		}
	}

	public void RevertMusic()
	{
		if (PlayingBGM)
		{
			NKCSoundManager.PlayScenMusic();
			PlayingBGM = false;
		}
	}

	private RectTransform GetPresetSlot(int index)
	{
		if (m_pointExchangeTemplet == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(m_pointExchangeTemplet.PrefabId))
		{
			return null;
		}
		string text = null;
		string text2 = null;
		if (m_pointExchangeTemplet.BannerId.Contains('@'))
		{
			NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(m_pointExchangeTemplet.PrefabId, m_pointExchangeTemplet.PrefabId);
			text = nKMAssetName.m_BundleName;
			text2 = nKMAssetName.m_AssetName;
		}
		else
		{
			text = m_pointExchangeTemplet.PrefabId;
			text2 = m_pointExchangeTemplet.PrefabId;
		}
		if (text == null || text2 == null)
		{
			return null;
		}
		return NKCUIPointExchangeSlot.GetNewInstance(null, text, text2 + "_SLOT_ALL")?.GetComponent<RectTransform>();
	}

	private void ReturnPresetSlot(Transform tr)
	{
		NKCUIPointExchangeSlot component = tr.GetComponent<NKCUIPointExchangeSlot>();
		tr.SetParent(null);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			UnityEngine.Object.Destroy(tr.gameObject);
		}
	}

	private void ProvidePresetData(Transform tr, int index)
	{
		NKCUIPointExchangeSlot component = tr.GetComponent<NKCUIPointExchangeSlot>();
		if (!(component == null) && m_pointExchangeTemplet != null)
		{
			component.SetData(m_pointExchangeTemplet);
		}
	}

	private void OnClickClose()
	{
		if (m_dOnClose != null)
		{
			m_dOnClose();
		}
	}

	private void OnClickMissionInformation()
	{
		if (m_dOnInformation != null)
		{
			m_dOnInformation();
		}
	}

	private void OnDestroy()
	{
		m_pointExchangeTemplet = null;
		if (m_pointCoinInfoArray != null)
		{
			int num = m_pointCoinInfoArray.Length;
			for (int i = 0; i < num; i++)
			{
				m_pointCoinInfoArray[i].Release();
			}
		}
		m_dOnClose = null;
		m_dOnInformation = null;
	}
}
