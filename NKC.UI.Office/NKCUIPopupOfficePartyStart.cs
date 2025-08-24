using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NKC.Templet;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Office;

public class NKCUIPopupOfficePartyStart : NKCUIBase
{
	public delegate void OnClose(NKCOfficePartyTemplet partyTemplet);

	private const string ASSET_BUNDLE_NAME = "ab_ui_office";

	private const string UI_ASSET_NAME = "AB_UI_POPUP_OFFICE_PARTY_START";

	private static NKCUIPopupOfficePartyStart m_Instance;

	public Transform m_trIllustRoot;

	public Text m_lbProgress;

	public Image m_imgProgress;

	public List<NKCUISlot> m_lstSlotReward;

	public NKCUIComStateButton m_csbtnClose;

	public Animator m_Animator;

	public float m_fTimeBeforeFirstReward = 1f;

	public float m_fTimePerReward = 0.25f;

	public float m_fTimeAfterLastReward = 1f;

	public float m_fTimeAfterFinishAnimation = 1f;

	public float m_fSlotAnimTime = 0.5f;

	public Ease m_fSlotAnimEase = Ease.OutBack;

	private GameObject m_objIllust;

	private OnClose dOnClose;

	private NKCOfficePartyTemplet m_partyTemplet;

	private int m_activeRewardCount;

	private bool m_bProgress;

	private float m_fTotalTime;

	private float m_fCurrentTime;

	public float m_fSlotScale = 0.75f;

	private const string ANIMATOR_TRIGGER = "PARTY_OVER";

	public static NKCUIPopupOfficePartyStart Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPopupOfficePartyStart>("ab_ui_office", "AB_UI_POPUP_OFFICE_PARTY_START", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUIPopupOfficePartyStart>();
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

	public override string MenuName => string.Empty;

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void OnBackButton()
	{
		if (m_bProgress)
		{
			Finish();
		}
		else
		{
			Close();
		}
	}

	public override void CloseInternal()
	{
		m_Animator.ResetTrigger("PARTY_OVER");
		base.gameObject.SetActive(value: false);
		dOnClose?.Invoke(m_partyTemplet);
	}

	private void InitUI()
	{
		foreach (NKCUISlot item in m_lstSlotReward)
		{
			item.Init();
			NKCUtil.SetGameobjectActive(item, bValue: false);
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnClose, OnBackButton);
	}

	public void Open(NKMRewardData rewardData, OnClose onClose)
	{
		m_partyTemplet = NKCOfficePartyTemplet.GetRandomTemplet();
		if (m_Animator != null)
		{
			m_Animator.ResetTrigger("PARTY_OVER");
		}
		if (m_partyTemplet == null)
		{
			Debug.LogError("party templet not loaded!");
			base.gameObject.SetActive(value: false);
			onClose?.Invoke(null);
		}
		else
		{
			dOnClose = onClose;
			SetIllust(m_partyTemplet);
			SetReward(rewardData);
			UIOpened();
			StartCoroutine(Process());
		}
	}

	private IEnumerator Process()
	{
		m_fTotalTime = m_fTimeBeforeFirstReward + m_fTimePerReward * (float)m_activeRewardCount + m_fTimeAfterLastReward + m_fTimeAfterFinishAnimation;
		m_fCurrentTime = 0f;
		m_bProgress = true;
		foreach (NKCUISlot item in m_lstSlotReward)
		{
			NKCUtil.SetGameobjectActive(item, bValue: false);
		}
		yield return WaitTime(m_fTimeBeforeFirstReward);
		for (int i = 0; i < m_activeRewardCount; i++)
		{
			NKCUISlot nKCUISlot = m_lstSlotReward[i];
			NKCUtil.SetGameobjectActive(nKCUISlot, bValue: true);
			if (nKCUISlot != null)
			{
				nKCUISlot.transform.localScale = Vector3.zero;
				nKCUISlot.transform.DOScale(m_fSlotScale, m_fSlotAnimTime).SetEase(m_fSlotAnimEase);
			}
			yield return WaitTime(m_fTimePerReward);
		}
		yield return WaitTime(m_fTimeAfterLastReward);
		m_Animator.SetTrigger("PARTY_OVER");
		yield return WaitTime(m_fTimeAfterFinishAnimation);
		Finish();
	}

	private void SetProgress(float progress)
	{
		NKCUtil.SetImageFillAmount(m_imgProgress, progress);
		NKCUtil.SetLabelText(m_lbProgress, $"{progress:0.00%}");
	}

	private IEnumerator WaitTime(float waitTime)
	{
		float currentTime = 0f;
		if (waitTime <= 0f)
		{
			yield break;
		}
		while (currentTime < waitTime)
		{
			if (Input.anyKey)
			{
				currentTime += Time.deltaTime * 2f;
				m_fCurrentTime += Time.deltaTime * 2f;
			}
			else
			{
				currentTime += Time.deltaTime;
				m_fCurrentTime += Time.deltaTime;
			}
			SetProgress(m_fCurrentTime / m_fTotalTime);
			yield return null;
		}
	}

	private void Finish()
	{
		StopAllCoroutines();
		SetProgress(1f);
		for (int i = 0; i < m_lstSlotReward.Count; i++)
		{
			NKCUISlot nKCUISlot = m_lstSlotReward[i];
			if (!(nKCUISlot == null))
			{
				NKCUtil.SetGameobjectActive(nKCUISlot, i < m_activeRewardCount);
				nKCUISlot.transform.DOKill();
				nKCUISlot.transform.localScale = Vector3.one * m_fSlotScale;
			}
		}
		m_Animator.SetTrigger("PARTY_OVER");
		m_bProgress = false;
	}

	private void SetReward(NKMRewardData rewardData)
	{
		List<NKCUISlot.SlotData> list = NKCUISlot.MakeSlotDataListFromReward(rewardData);
		NKCUISlot.SetSlotListData(m_lstSlotReward, list, false, true, true, null, default(NKCUISlot.SlotClickType));
		m_activeRewardCount = Mathf.Min(list.Count, m_lstSlotReward.Count);
	}

	private void SetIllust(NKCOfficePartyTemplet partyTemplet)
	{
		Object.Destroy(m_objIllust);
		NKCAssetResourceData nKCAssetResourceData = NKCAssetResourceManager.OpenResource<GameObject>(partyTemplet.IllustName);
		if (nKCAssetResourceData != null && nKCAssetResourceData.GetAsset<GameObject>() != null)
		{
			m_objIllust = Object.Instantiate(nKCAssetResourceData.GetAsset<GameObject>(), m_trIllustRoot);
		}
	}
}
