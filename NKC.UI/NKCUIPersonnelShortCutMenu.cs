using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCUIPersonnelShortCutMenu : MonoBehaviour
{
	public NKCUIComToggle m_tglNegotiate;

	public GameObject m_objNegotiateEvent;

	public NKCUIComToggle m_tglLifetime;

	public NKCUIComToggle m_tglScout;

	private bool bInitComplete;

	private void Init()
	{
		m_tglNegotiate.m_bGetCallbackWhileLocked = true;
		m_tglNegotiate.OnValueChanged.RemoveAllListeners();
		m_tglNegotiate.OnValueChanged.AddListener(OnNegotiate);
		m_tglLifetime.m_bGetCallbackWhileLocked = true;
		m_tglLifetime.OnValueChanged.RemoveAllListeners();
		m_tglLifetime.OnValueChanged.AddListener(OnLifetime);
		m_tglScout.m_bGetCallbackWhileLocked = true;
		m_tglScout.OnValueChanged.RemoveAllListeners();
		m_tglScout.OnValueChanged.AddListener(OnScout);
		bInitComplete = true;
	}

	public void SetData(NKC_SCEN_BASE.eUIOpenReserve selectedType)
	{
		if (!bInitComplete)
		{
			Init();
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_PERSONNAL))
		{
			m_tglNegotiate.Lock();
		}
		else
		{
			m_tglNegotiate.UnLock();
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_PERSONNAL))
		{
			m_tglLifetime.Lock();
		}
		else
		{
			m_tglLifetime.UnLock();
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_PERSONNAL))
		{
			m_tglScout.Lock();
		}
		else
		{
			m_tglScout.UnLock();
		}
		switch (selectedType)
		{
		case NKC_SCEN_BASE.eUIOpenReserve.Personnel_Negotiate:
			m_tglNegotiate.Select(bSelect: true, bForce: true);
			break;
		case NKC_SCEN_BASE.eUIOpenReserve.Personnel_Lifetime:
			m_tglLifetime.Select(bSelect: true, bForce: true);
			break;
		case NKC_SCEN_BASE.eUIOpenReserve.Personnel_Scout:
			m_tglScout.Select(bSelect: true, bForce: true);
			break;
		}
		NKCUtil.SetGameobjectActive(m_objNegotiateEvent, NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_PERSONNAL_NEGOTIATION_CREDIT_DISCOUNT));
	}

	private void OnNegotiate(bool bSet)
	{
		if (m_tglNegotiate.m_bLock)
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.BASE_PERSONNAL);
		}
		else if (bSet)
		{
			NKCUIManager.OnBackButton();
			NKCScenManager.GetScenManager().Get_SCEN_BASE().SetOpenReserve(NKC_SCEN_BASE.eUIOpenReserve.Personnel_Negotiate, 0L);
		}
	}

	private void OnLifetime(bool bSet)
	{
		if (m_tglLifetime.m_bLock)
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.BASE_PERSONNAL);
		}
		else if (bSet)
		{
			NKCUIManager.OnBackButton();
			NKCScenManager.GetScenManager().Get_SCEN_BASE().SetOpenReserve(NKC_SCEN_BASE.eUIOpenReserve.Personnel_Lifetime, 0L);
		}
	}

	private void OnScout(bool bSet)
	{
		if (m_tglScout.m_bLock)
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.BASE_PERSONNAL);
		}
		else if (bSet)
		{
			NKCUIManager.OnBackButton();
			NKCScenManager.GetScenManager().Get_SCEN_BASE().SetOpenReserve(NKC_SCEN_BASE.eUIOpenReserve.Personnel_Scout, 0L);
		}
	}
}
