using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;

namespace NKC.UI;

public class NKCUIFactoryShortCutMenu : MonoBehaviour
{
	public NKCUIComToggle m_tglCraft;

	public NKCUIComToggle m_tglEnchant;

	public NKCUIComToggle m_tglTuning;

	public NKCUIComToggle m_tglHiddenOption;

	public GameObject m_objCraftEvent;

	public GameObject m_objEnchantEvent;

	public GameObject m_objTuningEvent;

	public GameObject m_objHiddinOptionEvent;

	private bool bInitComplete;

	private UnityAction CraftCallBackFunc;

	private void Init()
	{
		if (m_tglCraft != null)
		{
			m_tglCraft.m_bGetCallbackWhileLocked = true;
			m_tglCraft.OnValueChanged.RemoveAllListeners();
			m_tglCraft.OnValueChanged.AddListener(OnCraft);
		}
		if (m_tglEnchant != null)
		{
			m_tglEnchant.m_bGetCallbackWhileLocked = true;
			m_tglEnchant.SetbReverseSeqCallbackCall(bSet: true);
			m_tglEnchant.OnValueChanged.RemoveAllListeners();
			m_tglEnchant.OnValueChanged.AddListener(OnEnchant);
		}
		else
		{
			Debug.LogError("m_tglEnchant Null!");
		}
		if (m_tglTuning != null)
		{
			m_tglTuning.m_bGetCallbackWhileLocked = true;
			m_tglTuning.SetbReverseSeqCallbackCall(bSet: true);
			m_tglTuning.OnValueChanged.RemoveAllListeners();
			m_tglTuning.OnValueChanged.AddListener(OnTuning);
		}
		else
		{
			Debug.LogError("m_tglTuning Null!");
		}
		if (m_tglHiddenOption != null)
		{
			bool flag = NKMOpenTagManager.IsOpened("EQUIP_POTENTIAL");
			NKCUtil.SetGameobjectActive(m_tglHiddenOption, flag);
			if (flag)
			{
				m_tglHiddenOption.m_bGetCallbackWhileLocked = true;
				m_tglHiddenOption.SetbReverseSeqCallbackCall(bSet: true);
				m_tglHiddenOption.OnValueChanged.RemoveAllListeners();
				m_tglHiddenOption.OnValueChanged.AddListener(OnHiddenOption);
			}
		}
		else
		{
			Debug.LogError("m_tglHiddenOption Null!");
		}
		bInitComplete = true;
	}

	public void SetData(NKCUIForge.NKC_FORGE_TAB tabType)
	{
		switch (tabType)
		{
		case NKCUIForge.NKC_FORGE_TAB.NFT_ENCHANT:
			SetData(ContentsType.FACTORY_ENCHANT);
			break;
		case NKCUIForge.NKC_FORGE_TAB.NFT_TUNING:
			SetData(ContentsType.FACTORY_TUNING);
			break;
		case NKCUIForge.NKC_FORGE_TAB.NFT_HIDDEN_OPTION:
			SetData(ContentsType.FACTORY_HIDDEN_OPTION);
			break;
		}
	}

	public void SetData(ContentsType selectedType)
	{
		if (!bInitComplete)
		{
			Init();
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_CRAFT))
		{
			m_tglCraft?.Lock();
		}
		else
		{
			m_tglCraft?.UnLock();
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT))
		{
			m_tglEnchant.Lock();
		}
		else
		{
			m_tglEnchant.UnLock();
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_TUNING))
		{
			m_tglTuning.Lock();
		}
		else
		{
			m_tglTuning.UnLock();
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_HIDDEN_OPTION))
		{
			m_tglHiddenOption.Lock();
		}
		else
		{
			m_tglHiddenOption.UnLock();
		}
		switch (selectedType)
		{
		case ContentsType.FACTORY_CRAFT:
			m_tglCraft?.Select(bSelect: true, bForce: true);
			break;
		case ContentsType.FACTORY_ENCHANT:
			m_tglEnchant.Select(bSelect: true, bForce: true);
			break;
		case ContentsType.FACTORY_TUNING:
			m_tglTuning.Select(bSelect: true, bForce: true);
			break;
		case ContentsType.FACTORY_HIDDEN_OPTION:
			m_tglHiddenOption.Select(bSelect: true, bForce: true);
			break;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			NKCUtil.SetGameobjectActive(m_objCraftEvent, NKCCompanyBuff.NeedShowEventMark(nKMUserData.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_CRAFT_CREDIT_DISCOUNT));
			NKCUtil.SetGameobjectActive(m_objEnchantEvent, NKCCompanyBuff.NeedShowEventMark(nKMUserData.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT));
			NKCUtil.SetGameobjectActive(m_objTuningEvent, NKCCompanyBuff.NeedShowEventMark(nKMUserData.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT));
			NKCUtil.SetGameobjectActive(m_objHiddinOptionEvent, NKCCompanyBuff.NeedShowEventMark(nKMUserData.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_POTENTIAL_SOCKET_CREDIT_DISCOUNT));
		}
	}

	private void OnCraft(bool bSet)
	{
		if (null != m_tglCraft && m_tglCraft.m_bLock)
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.FACTORY_CRAFT);
		}
		else if (bSet && !NKCUIForgeCraft.IsInstanceOpen)
		{
			if (NKCUIForge.IsInstanceOpen)
			{
				CraftCallBackFunc?.Invoke();
			}
			else
			{
				MoveToCraft();
			}
		}
	}

	private void OnEnchant(bool bSet)
	{
		if (m_tglEnchant.m_bLock)
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.FACTORY_ENCHANT);
		}
		else if (bSet)
		{
			if (!NKCUIForge.Instance.IsHiddenOptionEffectStopped())
			{
				m_tglHiddenOption.Select(bSelect: true, bForce: true);
			}
			else if (!NKCUIForge.IsInstanceOpen)
			{
				NKCUIManager.OnBackButton();
				NKCUIForge.Instance.Open(NKCUIForge.NKC_FORGE_TAB.NFT_ENCHANT, 0L);
			}
			else
			{
				NKCUIForge.Instance.SetTab(NKCUIForge.NKC_FORGE_TAB.NFT_ENCHANT);
			}
		}
	}

	private void OnTuning(bool bSet)
	{
		if (m_tglTuning.m_bLock)
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.FACTORY_TUNING);
		}
		else if (bSet)
		{
			if (!NKCUIForge.Instance.IsHiddenOptionEffectStopped())
			{
				m_tglHiddenOption.Select(bSelect: true, bForce: true);
			}
			else if (!NKCUIForge.IsInstanceOpen)
			{
				NKCUIManager.OnBackButton();
				NKCUIForge.Instance.Open(NKCUIForge.NKC_FORGE_TAB.NFT_TUNING, 0L);
			}
			else
			{
				NKCUIForge.Instance.SetTab(NKCUIForge.NKC_FORGE_TAB.NFT_TUNING);
			}
		}
	}

	private void OnHiddenOption(bool bSet)
	{
		if (m_tglHiddenOption.m_bLock)
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.FACTORY_HIDDEN_OPTION);
		}
		else if (bSet && NKCUIForge.Instance.IsHiddenOptionEffectStopped())
		{
			if (!NKCUIForge.IsInstanceOpen)
			{
				NKCUIManager.OnBackButton();
				NKCUIForge.Instance.Open(NKCUIForge.NKC_FORGE_TAB.NFT_HIDDEN_OPTION, 0L);
			}
			else
			{
				NKCUIForge.Instance.SetTab(NKCUIForge.NKC_FORGE_TAB.NFT_HIDDEN_OPTION);
			}
		}
	}

	public void OnConfirmBeforeChangeToCraft(UnityAction action)
	{
		CraftCallBackFunc = action;
	}

	public void MoveToCraft()
	{
		NKCUIManager.OnBackButton();
		NKCUIForgeCraft.Instance.Open();
	}
}
