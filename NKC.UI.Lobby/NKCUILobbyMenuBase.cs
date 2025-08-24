using System.Collections;
using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC.UI.Lobby;

public class NKCUILobbyMenuBase : NKCUILobbyMenuButtonBase
{
	public delegate bool DotEnableConditionFunction(NKMUserData userData);

	public delegate void OnButton();

	private enum HeadquartersWorkState
	{
		Idle = -1,
		Working,
		Complete
	}

	public GameObject m_objEvent;

	public GameObject m_objReddot;

	public NKCUIComStateButton m_csbtnButton;

	private DotEnableConditionFunction dDotEnableConditionFunction;

	private OnButton dOnButton;

	public void Init(DotEnableConditionFunction conditionFunc, OnButton onButton, ContentsType contentsType)
	{
		dDotEnableConditionFunction = conditionFunc;
		dOnButton = onButton;
		m_ContentsType = contentsType;
		m_csbtnButton.PointerClick.RemoveAllListeners();
		m_csbtnButton.PointerClick.AddListener(OnBtn);
	}

	protected override void ContentsUpdate(NKMUserData userData)
	{
		if (userData != null)
		{
			HeadquartersWorkState num = CheckNewUnlockShipBuild();
			HeadquartersWorkState headquartersWorkState = HeadquartersWorkState.Idle;
			if (num != HeadquartersWorkState.Complete)
			{
				headquartersWorkState = CheckEquipCreationState(userData.m_CraftData);
			}
			HeadquartersWorkState headquartersWorkState2 = HeadquartersWorkState.Idle;
			if (num == HeadquartersWorkState.Complete || headquartersWorkState == HeadquartersWorkState.Complete)
			{
				headquartersWorkState2 = HeadquartersWorkState.Complete;
			}
			else if (headquartersWorkState == HeadquartersWorkState.Working)
			{
				headquartersWorkState2 = HeadquartersWorkState.Working;
			}
			if (headquartersWorkState2 != HeadquartersWorkState.Complete && NKCAlarmManager.CheckScoutNotify(userData))
			{
				headquartersWorkState2 = HeadquartersWorkState.Complete;
			}
			if (headquartersWorkState2 != HeadquartersWorkState.Complete && NKCAlarmManager.CheckOfficeDormNotify(userData))
			{
				headquartersWorkState2 = HeadquartersWorkState.Complete;
			}
			UpdateState(headquartersWorkState2);
			CheckEvent();
		}
	}

	public override void CleanUp()
	{
		StopCoroutine(CheckState());
	}

	private void CheckEvent()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			NKCUtil.SetGameobjectActive(m_objEvent, bValue: false);
			return;
		}
		bool flag = false;
		flag |= NKCCompanyBuff.NeedShowEventMark(nKMUserData.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_PERSONNAL_NEGOTIATION_CREDIT_DISCOUNT);
		flag |= NKCCompanyBuff.NeedShowEventMark(nKMUserData.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_CRAFT_CREDIT_DISCOUNT);
		flag |= NKCCompanyBuff.NeedShowEventMark(nKMUserData.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT);
		flag |= NKCCompanyBuff.NeedShowEventMark(nKMUserData.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_POTENTIAL_SOCKET_CREDIT_DISCOUNT);
		NKCUtil.SetGameobjectActive(m_objEvent, flag);
	}

	private HeadquartersWorkState CheckNewUnlockShipBuild()
	{
		HeadquartersWorkState result = HeadquartersWorkState.Idle;
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.HANGER_SHIPBUILD))
		{
			return HeadquartersWorkState.Idle;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			foreach (NKMShipBuildTemplet value in NKMTempletContainer<NKMShipBuildTemplet>.Values)
			{
				if (value.ShipBuildUnlockType == NKMShipBuildTemplet.BuildUnlockType.BUT_UNABLE)
				{
					continue;
				}
				bool flag = false;
				foreach (KeyValuePair<long, NKMUnitData> item in nKMUserData.m_ArmyData.m_dicMyShip)
				{
					if (NKMShipManager.IsSameKindShip(item.Value.m_UnitID, value.Key))
					{
						flag = true;
						break;
					}
				}
				if (NKMShipManager.CanUnlockShip(nKMUserData, value))
				{
					string key = string.Format("{0}_{1}_{2}", "SHIP_BUILD_SLOT_CHECK", nKMUserData.m_UserUID, value.ShipID);
					if (!flag && !PlayerPrefs.HasKey(key))
					{
						result = HeadquartersWorkState.Complete;
						break;
					}
				}
			}
		}
		return result;
	}

	private HeadquartersWorkState CheckEquipCreationState(NKMCraftData creationData)
	{
		HeadquartersWorkState result = HeadquartersWorkState.Idle;
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_CRAFT))
		{
			return HeadquartersWorkState.Idle;
		}
		foreach (KeyValuePair<byte, NKMCraftSlotData> slot in creationData.SlotList)
		{
			if (slot.Value.GetState(NKCSynchronizedTime.GetServerUTCTime()) == NKM_CRAFT_SLOT_STATE.NECSS_COMPLETED)
			{
				return HeadquartersWorkState.Complete;
			}
			if (slot.Value.GetState(NKCSynchronizedTime.GetServerUTCTime()) == NKM_CRAFT_SLOT_STATE.NECSS_CREATING_NOW)
			{
				result = HeadquartersWorkState.Working;
			}
		}
		return result;
	}

	private void UpdateState(HeadquartersWorkState state)
	{
		switch (state)
		{
		case HeadquartersWorkState.Complete:
			NKCUtil.SetGameobjectActive(m_objReddot, bValue: true);
			break;
		case HeadquartersWorkState.Working:
			NKCUtil.SetGameobjectActive(m_objReddot, bValue: false);
			StartCoroutine(CheckState());
			break;
		case HeadquartersWorkState.Idle:
			NKCUtil.SetGameobjectActive(m_objReddot, bValue: false);
			break;
		}
	}

	private IEnumerator CheckState()
	{
		NKMUserData userData = NKCScenManager.CurrentUserData();
		if (userData != null)
		{
			while (IsWork(userData))
			{
				yield return new WaitForSeconds(1f);
			}
		}
		yield return null;
	}

	private bool IsWork(NKMUserData userData)
	{
		HeadquartersWorkState headquartersWorkState = HeadquartersWorkState.Idle;
		if (-1 != 1)
		{
			headquartersWorkState = CheckEquipCreationState(userData.m_CraftData);
		}
		bool flag = -1 == 1 || headquartersWorkState == HeadquartersWorkState.Complete;
		NKCUtil.SetGameobjectActive(m_objReddot, flag);
		return !flag;
	}

	private void OnBtn()
	{
		if (m_bLocked)
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.BASE);
		}
		else if (dOnButton != null)
		{
			dOnButton();
		}
	}
}
