using NKC.UI.Lobby;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCPopupHamburgerMenuSimpleButton : NKCUILobbyMenuButtonBase
{
	public delegate bool DotEnableConditionFunction(NKMUserData userData);

	public delegate void OnButton();

	public delegate void OnLocked();

	public GameObject m_objReddot;

	public NKCUIComStateButton m_csbtnButton;

	public GameObject m_objEvent;

	private DotEnableConditionFunction dDotEnableConditionFunction;

	private OnButton dOnButton;

	private OnLocked dOnLocked;

	public void Init(DotEnableConditionFunction dotConditionFunc, OnButton onButton, ContentsType contentsType = ContentsType.None)
	{
		dDotEnableConditionFunction = dotConditionFunc;
		dOnButton = onButton;
		m_ContentsType = contentsType;
		m_csbtnButton.PointerClick.RemoveAllListeners();
		m_csbtnButton.PointerClick.AddListener(OnBtn);
		NKCUtil.SetGameobjectActive(m_objEvent, bValue: false);
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		bool flag = dDotEnableConditionFunction != null && dDotEnableConditionFunction(userData);
		SetNotify(flag);
		NKCUtil.SetGameobjectActive(m_objReddot, flag);
	}

	protected override void SetNotify(bool value)
	{
		base.SetNotify(value);
		NKCUtil.SetGameobjectActive(m_objReddot, value);
	}

	private void OnBtn()
	{
		if (m_bLocked)
		{
			NKCContentManager.ShowLockedMessagePopup(m_ContentsType);
		}
		else if (m_ContentsType != ContentsType.BASE_FACTORY && NKCUIForge.IsInstanceOpen && NKCScenManager.GetScenManager().GetMyUserData().hasReservedEquipTuningData())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_TUNING_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_Equip_Tuning_Cancel_REQ();
				dOnButton?.Invoke();
			});
		}
		else if (m_ContentsType != ContentsType.BASE_FACTORY && NKCUIForge.IsInstanceOpen && NKCScenManager.GetScenManager().GetMyUserData().hasReservedSetOptionData())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_SET_OPTION_TUNING_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_Equip_Tuning_Cancel_REQ();
				dOnButton?.Invoke();
			});
		}
		else if (m_ContentsType != ContentsType.BASE_FACTORY && NKCUIForge.IsInstanceOpen && NKCScenManager.GetScenManager().GetMyUserData().hasReservedHiddenOptionRerollData())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_RELIC_REROLL_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CANCLE_REQ();
				dOnButton?.Invoke();
			});
		}
		else if (NKCPopupShipCommandModule.IsInstanceOpen && NKCScenManager.GetScenManager().GetMyUserData().GetShipCandidateData()
			.shipUid > 0)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_SHIP_COMMANDMODULE_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_SHIP_SLOT_OPTION_CANCEL_REQ();
				dOnButton?.Invoke();
			});
		}
		else
		{
			dOnButton?.Invoke();
		}
	}

	public void SetEnableEvent(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objEvent, bValue);
	}
}
