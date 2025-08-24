using System.Collections;
using System.Collections.Generic;
using NKC.Templet;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPopupVoice : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_VOICE";

	private static NKCUIPopupVoice m_Instance;

	public LoopVerticalScrollRect m_loopScrollRect;

	public Transform m_contents;

	public NKCUIComStateButton m_btnClose;

	public NKCUIComStateButton m_btnBG;

	public Text m_lbVoiceActorName;

	private List<NKCUIPopupVoiceSlot> m_slotList = new List<NKCUIPopupVoiceSlot>();

	private Stack<RectTransform> m_slotPool = new Stack<RectTransform>();

	private List<NKCUIVoiceManager.VoiceData> m_listData = new List<NKCUIVoiceManager.VoiceData>();

	private string m_unitStrID;

	private int m_skinID;

	private bool m_bLifetime;

	private int m_currentIndex;

	private Coroutine m_toggleCoroutine;

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.ON_PLAY_GAME;

	public static NKCUIPopupVoice Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupVoice>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_VOICE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupVoice>();
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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "VOICE";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void InitUI()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_btnBG.PointerClick.RemoveAllListeners();
		m_btnBG.PointerClick.AddListener(base.Close);
		m_loopScrollRect.dOnGetObject += OnGetObject;
		m_loopScrollRect.dOnProvideData += OnProvideData;
		m_loopScrollRect.dOnReturnObject += OnReturnObject;
		m_loopScrollRect.PrepareCells();
		NKCUtil.SetScrollHotKey(m_loopScrollRect);
	}

	private RectTransform OnGetObject(int index)
	{
		if (m_slotPool.Count > 0)
		{
			RectTransform rectTransform = m_slotPool.Pop();
			NKCUtil.SetGameobjectActive(rectTransform, bValue: true);
			return rectTransform;
		}
		NKCUIPopupVoiceSlot nKCUIPopupVoiceSlot = NKCUIPopupVoiceSlot.newInstance(m_contents);
		if (nKCUIPopupVoiceSlot == null)
		{
			return null;
		}
		m_slotList.Add(nKCUIPopupVoiceSlot);
		return nKCUIPopupVoiceSlot.GetComponent<RectTransform>();
	}

	private void OnProvideData(Transform tr, int index)
	{
		NKCUIPopupVoiceSlot component = tr.GetComponent<NKCUIPopupVoiceSlot>();
		if (component == null)
		{
			return;
		}
		if (m_listData.Count <= index)
		{
			Debug.LogError($"popupvoice - index {index}, data Count {m_listData.Count}");
			return;
		}
		NKCUIVoiceManager.VoiceData voiceData = m_listData[index];
		if (voiceData != null)
		{
			NKCCollectionVoiceTemplet templet = NKMTempletContainer<NKCCollectionVoiceTemplet>.Find(voiceData.idx);
			component.SetUI(index, templet, voiceData.voiceBundle == VOICE_BUNDLE.SKIN, OnTouhchSlot);
			component.SetToggle(m_currentIndex);
		}
	}

	private void OnReturnObject(Transform tr)
	{
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.transform);
		m_slotPool.Push(tr.GetComponent<RectTransform>());
	}

	public void Open(NKMUnitData unit)
	{
		if (unit != null)
		{
			Open(unit.m_UnitID, unit.m_SkinID, unit.IsPermanentContract);
		}
	}

	public void Open(NKMUnitTempletBase unitTempletBase, bool bLifetime = false)
	{
		if (unitTempletBase != null)
		{
			Open(unitTempletBase.m_UnitID, 0, bLifetime);
		}
	}

	public void Open(NKMSkinTemplet skinTemplet, bool bLifetime = false)
	{
		if (skinTemplet != null)
		{
			Open(skinTemplet.m_SkinEquipUnitID, skinTemplet.m_SkinID, bLifetime);
		}
	}

	public void Open(int unitID, int skinID, bool bLifetime)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		m_unitStrID = unitTempletBase.m_UnitStrID;
		m_skinID = skinID;
		m_bLifetime = bLifetime;
		SetUI();
		UIOpened();
	}

	public override void CloseInternal()
	{
		EndCoroutine();
		NKCUIVoiceManager.StopVoice();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnDestroy()
	{
		Clear();
		m_Instance = null;
	}

	private void Clear()
	{
		for (int i = 0; i < m_slotList.Count; i++)
		{
			m_slotList[i].Clear();
		}
		m_slotList.Clear();
		m_slotPool.Clear();
		m_listData.Clear();
	}

	private void SetUI()
	{
		m_currentIndex = -1;
		RefreshSlotToggle(m_currentIndex);
		m_listData.Clear();
		m_listData = NKCUIVoiceManager.GetListVoice(m_unitStrID, m_skinID, m_bLifetime);
		m_listData.Sort(CompareVoiceData);
		m_loopScrollRect.TotalCount = m_listData.Count;
		m_loopScrollRect.SetIndexPosition(0);
		m_loopScrollRect.RefreshCells(bForce: true);
		NKCUtil.SetLabelText(m_lbVoiceActorName, NKCVoiceActorNameTemplet.FindActorName(m_unitStrID, m_skinID));
	}

	private void OnTouhchSlot(bool bPlay, int index)
	{
		if (index >= 0 && m_listData.Count > index)
		{
			EndCoroutine();
			m_currentIndex = index;
			PlayVoice(bPlay, index);
			if (bPlay)
			{
				RefreshSlotToggle(index);
				m_toggleCoroutine = StartCoroutine(UpdateSlotToggle());
			}
		}
	}

	private void PlayVoice(bool bPlay, int index)
	{
		if (bPlay)
		{
			if (index < 0 || m_listData.Count <= index)
			{
				return;
			}
			NKCUIVoiceManager.VoiceData voiceData = m_listData[index];
			if (voiceData == null)
			{
				return;
			}
			NKCCollectionVoiceTemplet nKCCollectionVoiceTemplet = NKMTempletContainer<NKCCollectionVoiceTemplet>.Find(voiceData.idx);
			if (nKCCollectionVoiceTemplet != null)
			{
				VOICE_BUNDLE voiceBundle = voiceData.voiceBundle;
				NKMAssetName nKMAssetName = NKCUIVoiceManager.PlayOnUI(m_unitStrID, m_skinID, nKCCollectionVoiceTemplet.m_VoicePostID, 100f, voiceBundle);
				if (nKMAssetName != null && nKCCollectionVoiceTemplet.m_VoiceCategory != NKC_VOICE_TYPE.ETC)
				{
					NKCUIManager.NKCUIOverlayCaption.OpenCaption(NKCUtilString.GetVoiceCaption(nKMAssetName), NKCUIVoiceManager.GetCurrentSoundUID());
				}
			}
		}
		else
		{
			NKCUIVoiceManager.StopVoice();
		}
	}

	private void RefreshSlotToggle(int seletedIndex)
	{
		for (int i = 0; i < m_slotList.Count; i++)
		{
			m_slotList[i].SetToggle(seletedIndex);
		}
	}

	private IEnumerator UpdateSlotToggle()
	{
		while (NKCSoundManager.IsPlayingVoice())
		{
			yield return null;
		}
		m_toggleCoroutine = null;
		m_currentIndex = -1;
		RefreshSlotToggle(m_currentIndex);
	}

	private void EndCoroutine()
	{
		if (m_toggleCoroutine != null)
		{
			StopCoroutine(m_toggleCoroutine);
			m_toggleCoroutine = null;
		}
	}

	private int CompareVoiceData(NKCUIVoiceManager.VoiceData a, NKCUIVoiceManager.VoiceData b)
	{
		if (a.idx == b.idx)
		{
			return a.voiceBundle.CompareTo(b.voiceBundle);
		}
		return a.idx.CompareTo(b.idx);
	}
}
