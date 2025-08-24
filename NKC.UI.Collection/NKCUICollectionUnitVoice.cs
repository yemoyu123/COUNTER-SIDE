using System.Collections;
using System.Collections.Generic;
using NKC.Templet;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionUnitVoice : MonoBehaviour
{
	public LoopVerticalScrollRect m_loopScrollRect;

	public Transform m_contents;

	public Text m_lbVoiceActorName;

	public GameObject m_objNoData;

	private List<NKCUICollectionUnitVoiceSlot> m_slotList = new List<NKCUICollectionUnitVoiceSlot>();

	private Stack<RectTransform> m_slotPool = new Stack<RectTransform>();

	private List<NKCUIVoiceManager.VoiceData> m_listData = new List<NKCUIVoiceManager.VoiceData>();

	private string m_unitStrID;

	private int m_skinID;

	private bool m_bLifetime;

	private int m_currentIndex;

	private Coroutine m_toggleCoroutine;

	public void InitUI()
	{
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
		NKCUICollectionUnitVoiceSlot nKCUICollectionUnitVoiceSlot = NKCUICollectionUnitVoiceSlot.newInstance(m_contents);
		if (nKCUICollectionUnitVoiceSlot == null)
		{
			return null;
		}
		m_slotList.Add(nKCUICollectionUnitVoiceSlot);
		return nKCUICollectionUnitVoiceSlot.GetComponent<RectTransform>();
	}

	private void OnProvideData(Transform tr, int index)
	{
		NKCUICollectionUnitVoiceSlot component = tr.GetComponent<NKCUICollectionUnitVoiceSlot>();
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

	public void SetData(NKMUnitData unit)
	{
		if (unit != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unit.m_UnitID);
			m_unitStrID = unitTempletBase.m_UnitStrID;
			m_skinID = unit.m_SkinID;
			m_bLifetime = false;
			NKMArmyData nKMArmyData = NKCScenManager.CurrentUserData()?.m_ArmyData;
			if (nKMArmyData != null && nKMArmyData.IsCollectedUnit(unit.m_UnitID))
			{
				m_bLifetime = nKMArmyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_NORMAL, unit.m_UnitID, NKMArmyData.UNIT_SEARCH_OPTION.Devotion, 0);
			}
			SetUI();
		}
	}

	public void SetData(NKMOperator oper)
	{
		if (oper != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(oper.id);
			m_unitStrID = unitTempletBase.m_UnitStrID;
			m_skinID = 0;
			m_bLifetime = false;
			NKMArmyData nKMArmyData = NKCScenManager.CurrentUserData()?.m_ArmyData;
			if (nKMArmyData != null && nKMArmyData.IsCollectedUnit(oper.id))
			{
				m_bLifetime = nKMArmyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_OPERATOR, oper.id, NKMArmyData.UNIT_SEARCH_OPTION.Devotion, 0);
			}
			SetUI();
		}
	}

	private void OnDisable()
	{
		StopVoice();
	}

	private void OnDestroy()
	{
		Clear();
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
		NKMUnitManager.GetUnitTempletBase(m_unitStrID);
		m_currentIndex = -1;
		RefreshSlotToggle(m_currentIndex);
		m_listData.Clear();
		m_listData = NKCUIVoiceManager.GetListVoice(m_unitStrID, m_skinID, m_bLifetime);
		m_listData.Sort(CompareVoiceData);
		bool flag = m_listData.Count > 0;
		NKCUtil.SetGameobjectActive(m_objNoData, !flag);
		NKCUtil.SetGameobjectActive(m_loopScrollRect, flag);
		if (flag)
		{
			m_loopScrollRect.TotalCount = m_listData.Count;
			m_loopScrollRect.SetIndexPosition(0);
			m_loopScrollRect.RefreshCells(bForce: true);
			NKCUtil.SetLabelText(m_lbVoiceActorName, NKCVoiceActorNameTemplet.FindActorName(m_unitStrID, m_skinID));
		}
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

	public void StopVoice()
	{
		EndCoroutine();
		NKCUIVoiceManager.StopVoice();
		m_currentIndex = -1;
		RefreshSlotToggle(m_currentIndex);
	}

	public bool PlayingVoice()
	{
		return m_toggleCoroutine != null;
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
