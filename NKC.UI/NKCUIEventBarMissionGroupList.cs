using System.Collections;
using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIEventBarMissionGroupList : MonoBehaviour
{
	public enum MissionType
	{
		MissionGroupId,
		MissionTabId
	}

	public CanvasGroup m_canvasGroup;

	public LoopScrollRect m_LoopScrollRect;

	public NKCUIComStateButton m_csbtnClose;

	[Header("Outro Animation")]
	public Animator m_animator;

	public string m_outroAnimation;

	private List<NKMMissionTemplet> m_missionTempletList;

	private Coroutine m_coroutine;

	private string m_slotBundleName;

	private string m_slotAssetName;

	public void Init(string slotBundleName, string slotAssetName)
	{
		m_slotBundleName = slotBundleName;
		m_slotAssetName = slotAssetName;
		if (m_LoopScrollRect != null)
		{
			m_LoopScrollRect.dOnGetObject += GetPresetSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnPresetSlot;
			m_LoopScrollRect.dOnProvideData += ProvidePresetData;
			m_LoopScrollRect.ContentConstraintCount = 1;
			m_LoopScrollRect.PrepareCells();
			m_LoopScrollRect.TotalCount = 0;
			m_LoopScrollRect.RefreshCells();
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, Close);
	}

	public void Open(MissionType missionType, int missionId)
	{
		if (m_canvasGroup != null)
		{
			m_canvasGroup.blocksRaycasts = true;
		}
		if (m_coroutine != null)
		{
			StopCoroutine(m_coroutine);
			m_coroutine = null;
		}
		if (m_missionTempletList == null)
		{
			List<NKMMissionTemplet> list = null;
			switch (missionType)
			{
			case MissionType.MissionGroupId:
				list = NKMMissionManager.GetMissionTempletListByGroupID(missionId);
				break;
			case MissionType.MissionTabId:
				list = NKMMissionManager.GetMissionTempletListByTabID(missionId);
				break;
			}
			if (list == null)
			{
				return;
			}
			m_missionTempletList = list;
		}
		base.gameObject.SetActive(value: false);
		base.gameObject.SetActive(value: true);
		m_LoopScrollRect.TotalCount = m_missionTempletList.Count;
		m_LoopScrollRect.SetIndexPosition(0);
	}

	public bool IsOpened()
	{
		if (base.gameObject.activeSelf)
		{
			return m_coroutine == null;
		}
		return false;
	}

	public void Refresh()
	{
		m_LoopScrollRect.RefreshCells();
	}

	public void CloseImmediately()
	{
		if (m_coroutine != null)
		{
			StopCoroutine(m_coroutine);
			m_coroutine = null;
		}
		base.gameObject.SetActive(value: false);
	}

	public bool IsClosed()
	{
		if (m_coroutine == null)
		{
			return !base.gameObject.activeSelf;
		}
		return true;
	}

	public void Close()
	{
		if (base.gameObject.activeSelf && m_coroutine == null)
		{
			m_coroutine = StartCoroutine(Outro());
		}
	}

	private IEnumerator Outro()
	{
		if (m_canvasGroup != null)
		{
			m_canvasGroup.blocksRaycasts = false;
		}
		if (m_animator != null)
		{
			m_animator.Play(m_outroAnimation, -1, 0f);
			float startTime = m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			while (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime - startTime < 1f)
			{
				yield return null;
			}
		}
		base.gameObject.SetActive(value: false);
		m_coroutine = null;
	}

	private RectTransform GetPresetSlot(int index)
	{
		return NKCUIEventBarMissionGroupListSlot.GetNewInstance(null, m_slotBundleName, m_slotAssetName)?.GetComponent<RectTransform>();
	}

	private void ReturnPresetSlot(Transform tr)
	{
		NKCUIEventBarMissionGroupListSlot component = tr.GetComponent<NKCUIEventBarMissionGroupListSlot>();
		tr.SetParent(null);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			Object.Destroy(tr.gameObject);
		}
	}

	private void ProvidePresetData(Transform tr, int index)
	{
		NKCUIEventBarMissionGroupListSlot component = tr.GetComponent<NKCUIEventBarMissionGroupListSlot>();
		if (!(component == null))
		{
			component.SetData(m_missionTempletList[index]);
		}
	}

	private void OnDestroy()
	{
		m_slotBundleName = null;
		m_slotAssetName = null;
		if (m_missionTempletList != null)
		{
			m_missionTempletList.Clear();
			m_missionTempletList = null;
		}
		m_coroutine = null;
	}
}
