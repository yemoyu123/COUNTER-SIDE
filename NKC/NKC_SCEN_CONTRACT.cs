using System.Collections.Generic;
using ClientPacket.Contract;
using NKC.UI;
using NKC.UI.Collection;
using NKC.UI.Contract;
using NKC.UI.Module;
using NKC.UI.Result;
using NKM;
using NKM.Contract2;
using NKM.Event;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_CONTRACT : NKC_SCEN_BASIC
{
	public const double CONTRACT_SLOT_CLIENT_TIME_DELAY_SECONDS = -1.0;

	private NKCUIContractV3 m_NKCUIContract;

	private NKCUIManager.LoadedUIData m_UIContractData;

	private string m_sReserveContractStrID = "";

	private NKMRewardData m_RewardUnit;

	private bool bWaitEvent;

	public NKC_SCEN_CONTRACT()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_CONTRACT;
	}

	public override void ScenLoadUIStart()
	{
		if (!NKCUIManager.IsValid(m_UIContractData))
		{
			m_UIContractData = NKCUIManager.OpenNewInstanceAsync<NKCUIContractV3>("AB_UI_NKM_UI_CONTRACT_V2", "NKM_UI_CONTRACT_V2", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), null);
		}
		base.ScenLoadUIStart();
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (m_NKCUIContract == null)
		{
			if (m_UIContractData != null && m_UIContractData.CheckLoadAndGetInstance<NKCUIContractV3>(out m_NKCUIContract))
			{
				m_NKCUIContract.Init();
			}
			else
			{
				Debug.LogError("Error - NKC_SCEN_CONTRACT.ScenLoadComplete() : UI Load Failed!");
			}
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		Open();
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		Close();
		m_NKCUIContract = null;
		m_UIContractData?.CloseInstance();
		m_UIContractData = null;
		NKCUICollectionUnitInfo.CheckInstanceAndClose();
		NKCUICollectionOperatorInfo.CheckInstanceAndClose();
		NKCCamera.GetTrackingPos().SetPause(bSet: false);
	}

	public void Open()
	{
		if (m_NKCUIContract != null)
		{
			m_NKCUIContract.Open(m_sReserveContractStrID);
		}
		m_sReserveContractStrID = "";
	}

	public void Close()
	{
		if (m_NKCUIContract != null)
		{
			m_NKCUIContract.Close();
		}
	}

	public void SetReserveContractID(string reserveStrID)
	{
		m_sReserveContractStrID = reserveStrID;
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	public void DoAfterLogout()
	{
		NKCScenManager.GetScenManager().GetNKCContractDataMgr().ResetContractState();
		NKCScenManager.GetScenManager().GetNKCContractDataMgr().ResetInstantContract();
	}

	public void OnUIForceRefresh(bool bForce = false)
	{
		if (m_NKCUIContract != null)
		{
			m_NKCUIContract.ResetContractUI(bForce);
		}
	}

	public void OnRecv(NKMPacket_CONTRACT_ACK sPacket)
	{
		ContractTempletV2 contractTempletV = ContractTempletV2.Find(sPacket.contractId);
		if (contractTempletV != null)
		{
			if (sPacket.rewardData == null)
			{
				sPacket.rewardData = new NKMRewardData();
			}
			ContractComplet(sPacket.units, sPacket.operators, sPacket.rewardData.MiscItemDataList, bSelectableContract: false, contractTempletV.MissionCountIgnore, sPacket.requestCount, contractTempletV.EventCollectionMergeID);
		}
	}

	public void OnRecv(NKMPacket_SELECTABLE_CONTRACT_CONFIRM_ACK sPacket)
	{
		if (sPacket.units != null && sPacket.units.Count > 0)
		{
			ContractComplet(sPacket.units, null, null, bSelectableContract: true);
		}
	}

	public void OnRecv(NKMPacket_CUSTOM_PICUP_SELECT_TARGET_ACK sPacket)
	{
		m_NKCUIContract.UpdateChildUI();
	}

	public void OnRecv(NKMPacket_CUSTOM_PICKUP_ACK sPacket)
	{
		CustomPickupContractTemplet customPickupContractTemplet = CustomPickupContractTemplet.Find(sPacket.customPickupId);
		if (customPickupContractTemplet != null)
		{
			if (sPacket.rewardData == null)
			{
				sPacket.rewardData = new NKMRewardData();
			}
			ContractComplet(sPacket.units, sPacket.operators, sPacket.rewardData.MiscItemDataList, bSelectableContract: false, customPickupContractTemplet.MissionCountIgnore, sPacket.requestCount);
		}
	}

	private void ContractComplet(List<NKMUnitData> lstUnit, List<NKMOperator> lstOper, List<NKMItemMiscData> lstMisc = null, bool bSelectableContract = false, bool bMissionCountIgnore = false, int requestCount = 0, int eventMergeID = 0)
	{
		if (m_NKCUIContract != null)
		{
			NKMRewardData nKMRewardData = new NKMRewardData();
			nKMRewardData.SetUnitData(lstUnit);
			if (lstOper != null)
			{
				nKMRewardData.SetOperatorList(lstOper);
			}
			if (lstMisc != null)
			{
				nKMRewardData.SetMiscItemData(lstMisc);
			}
			m_RewardUnit = nKMRewardData;
			if (bSelectableContract)
			{
				OpenReward(requestCount, bDisplayGetUnit: false);
				return;
			}
			NKM_UNIT_GRADE nKM_UNIT_GRADE = NKM_UNIT_GRADE.NUG_N;
			bool bAwaken = false;
			if (m_RewardUnit.UnitDataList != null)
			{
				foreach (NKMUnitData unitData in m_RewardUnit.UnitDataList)
				{
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
					if (unitTempletBase.m_NKM_UNIT_GRADE > nKM_UNIT_GRADE)
					{
						nKM_UNIT_GRADE = unitTempletBase.m_NKM_UNIT_GRADE;
					}
					if (unitTempletBase.m_bAwaken)
					{
						bAwaken = true;
					}
				}
			}
			if (m_RewardUnit.OperatorList != null)
			{
				foreach (NKMOperator @operator in m_RewardUnit.OperatorList)
				{
					NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(@operator.id);
					if (unitTempletBase2.m_NKM_UNIT_GRADE > nKM_UNIT_GRADE)
					{
						nKM_UNIT_GRADE = unitTempletBase2.m_NKM_UNIT_GRADE;
					}
					if (unitTempletBase2.m_bAwaken)
					{
						bAwaken = true;
					}
				}
			}
			if (!bMissionCountIgnore)
			{
				NKCUIContractSequence.Instance.Open(nKM_UNIT_GRADE, bAwaken, delegate
				{
					OpenReward(requestCount);
				});
			}
			else
			{
				OpenReward(requestCount);
			}
			return;
		}
		foreach (NKMEventCollectionIndexTemplet collectionIdxTemplet in NKMTempletContainer<NKMEventCollectionIndexTemplet>.Values)
		{
			if (!collectionIdxTemplet.IsOpen || !collectionIdxTemplet.IsOpen || collectionIdxTemplet.CollectionMergeId != eventMergeID)
			{
				continue;
			}
			if (string.IsNullOrEmpty(collectionIdxTemplet.EventContractAnimationPrefabID))
			{
				break;
			}
			NKCUIModuleContractResult moduleContractResult = NKCUIModuleContractResult.MakeInstance(collectionIdxTemplet.EventContractAniPrefabID_AssetName, collectionIdxTemplet.EventContractAniPrefabID_BundleName);
			if (!(moduleContractResult != null))
			{
				break;
			}
			moduleContractResult.Open(lstUnit, delegate
			{
				NKMRewardData nKMRewardData2 = new NKMRewardData();
				nKMRewardData2.SetUnitData(lstUnit);
				if (lstMisc != null)
				{
					nKMRewardData2.SetMiscItemData(lstMisc);
				}
				moduleContractResult?.Close();
				moduleContractResult = null;
				if (!string.IsNullOrEmpty(collectionIdxTemplet.EventResultPrefabID))
				{
					NKCUIPopupModuleResult moduleResultPopup = NKCUIPopupModuleResult.MakeInstance(collectionIdxTemplet.EventResultPrefabID, collectionIdxTemplet.EventResultPrefabID);
					if (null != moduleResultPopup)
					{
						moduleResultPopup.Init();
						moduleResultPopup.Open(nKMRewardData2, delegate
						{
							moduleResultPopup.Close();
							moduleResultPopup = null;
							ClosedReward();
						});
					}
				}
				else
				{
					NKCUIResult.Instance.OpenBoxGain(NKCScenManager.CurrentUserData().m_ArmyData, nKMRewardData2, NKCUtilString.GET_STRING_CONTRACT_SLOT_UNIT, delegate
					{
						ClosedReward();
					}, bDisplayUnitGet: false, requestCount, bDefaultSort: false);
				}
			});
			break;
		}
	}

	public void OpenReward(int requestCount, bool bDisplayGetUnit = true)
	{
		bWaitEvent = false;
		if (NKCGameEventManager.IsWaiting())
		{
			bWaitEvent = true;
			NKCUIResult.Instance.OpenBoxGain(NKCScenManager.CurrentUserData().m_ArmyData, m_RewardUnit, NKCUtilString.GET_STRING_CONTRACT_SLOT_UNIT, delegate
			{
				ClosedReward(bDisplayGetUnit);
			}, bDisplayGetUnit, requestCount, bDefaultSort: false);
		}
		else
		{
			NKCUIResult.Instance.OpenBoxGain(NKCScenManager.CurrentUserData().m_ArmyData, m_RewardUnit, NKCUtilString.GET_STRING_CONTRACT_SLOT_UNIT, delegate
			{
				ClosedReward(bDisplayGetUnit);
			}, bDisplayGetUnit, requestCount, bDefaultSort: false);
		}
	}

	private void ClosedReward(bool bDisplayGetUnit = true)
	{
		if (bWaitEvent)
		{
			NKCGameEventManager.WaitFinished();
		}
		if (!bDisplayGetUnit)
		{
			NKCUIContractSelection.Instance.Close();
		}
		m_NKCUIContract?.OnContractCompleteAck();
	}

	public void OnRecv(NKMPacket_SELECTABLE_CONTRACT_CHANGE_POOL_ACK sPacket)
	{
		if (m_NKCUIContract != null)
		{
			m_NKCUIContract.UpdateChildUI();
		}
		List<NKMUnitData> lstDummyData = new List<NKMUnitData>();
		foreach (int unitId in sPacket.selectableContractState.unitIdList)
		{
			NKMUnitData nKMUnitData = NKCUtil.MakeDummyUnit(unitId, 1, 0);
			if (nKMUnitData != null)
			{
				lstDummyData.Add(nKMUnitData);
			}
		}
		NKM_UNIT_GRADE nKM_UNIT_GRADE = NKM_UNIT_GRADE.NUG_N;
		bool bAwaken = false;
		foreach (NKMUnitData item in lstDummyData)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(item.m_UnitID);
			if (unitTempletBase.m_NKM_UNIT_GRADE > nKM_UNIT_GRADE)
			{
				nKM_UNIT_GRADE = unitTempletBase.m_NKM_UNIT_GRADE;
			}
			if (unitTempletBase.m_bAwaken)
			{
				bAwaken = true;
			}
		}
		if (sPacket.selectableContractState.unitPoolChangeCount <= 1)
		{
			NKCUIContractSequence.Instance.Open(nKM_UNIT_GRADE, bAwaken, delegate
			{
				NKCUIGameResultGetUnit.ShowNewUnitGetUIForSelectableContract(lstDummyData, delegate
				{
					NKCUIContractSelection.Instance.Open(sPacket.selectableContractState);
				});
			});
		}
		else
		{
			NKCUIGameResultGetUnit.ShowNewUnitGetUIForSelectableContract(lstDummyData, delegate
			{
				NKCUIContractSelection.Instance.Open(sPacket.selectableContractState);
			});
		}
	}

	public void SelectRecruitBanner(string contractStrID)
	{
		if (!(m_NKCUIContract == null) && m_NKCUIContract.IsOpen && !m_NKCUIContract.SelectRecruitBanner(contractStrID))
		{
			m_NKCUIContract.SelectFirstBanner();
		}
	}
}
