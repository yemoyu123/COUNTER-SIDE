using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Guild;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildDonation : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM";

	private const string UI_ASSET_NAME = "NKM_UI_CONSORTIUM_POPUP_DONATION";

	private static NKCPopupGuildDonation m_Instance;

	public NKCUIComStateButton m_btnBG;

	public NKCUIComStateButton m_btnClose;

	public Text m_lbRemainContibutionCount;

	public List<NKCPopupGuildDonationSlot> m_lstSlot = new List<NKCPopupGuildDonationSlot>();

	private List<GuildDonationTemplet> m_lstTemplet = new List<GuildDonationTemplet>();

	public static NKCPopupGuildDonation Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildDonation>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_POPUP_DONATION", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), CleanupInstance).GetInstance<NKCPopupGuildDonation>();
				if (m_Instance != null)
				{
					m_Instance.InitUI();
				}
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

	public override string MenuName => "";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		m_lstTemplet.Clear();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void UnHide()
	{
		base.UnHide();
		UpdateState();
	}

	private void InitUI()
	{
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_btnBG.PointerClick.RemoveAllListeners();
		m_btnBG.PointerClick.AddListener(base.Close);
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			m_lstSlot[i].InitUI(OnClickSlot);
		}
	}

	public void Open()
	{
		if (m_lstSlot.Count != NKMTempletContainer<GuildDonationTemplet>.Values.Count())
		{
			Log.Error("기부 종류가 UI와 템플릿이 다름", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/NKCPopupGuildDonation.cs", 95);
		}
		m_lstTemplet = NKMTempletContainer<GuildDonationTemplet>.Values.ToList();
		UpdateState();
		UIOpened();
	}

	private void UpdateState()
	{
		NKCUtil.SetLabelText(m_lbRemainContibutionCount, string.Format(NKCUtilString.GET_STRING_CONTRACT_COUNT_ONE_PARAM, NKCGuildManager.GetRemainDonationCount()));
		if (NKCGuildManager.GetRemainDonationCount() > 0)
		{
			NKCUtil.SetLabelTextColor(m_lbRemainContibutionCount, NKCUtil.GetColor("#C3C3C3"));
		}
		else
		{
			NKCUtil.SetLabelTextColor(m_lbRemainContibutionCount, Color.red);
		}
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			m_lstSlot[i].SetData(m_lstTemplet[i]);
		}
	}

	private void OnClickSlot(int donationID)
	{
		GuildDonationTemplet guildDonationTemplet = GuildDonationTemplet.Find(donationID);
		if (guildDonationTemplet != null)
		{
			NKCPopupGuildDonationMultiply.Instance.Open(guildDonationTemplet);
		}
		else
		{
			Log.Error($"templet is null - ID : {donationID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/NKCPopupGuildDonation.cs", 129);
		}
	}

	public override void OnGuildDataChanged()
	{
		NKCUtil.SetLabelText(m_lbRemainContibutionCount, string.Format(NKCUtilString.GET_STRING_CONTRACT_COUNT_ONE_PARAM, NKCGuildManager.GetRemainDonationCount()));
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			m_lstSlot[i].CheckState(GuildDonationTemplet.Find(i + 1));
		}
	}
}
