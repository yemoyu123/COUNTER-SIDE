using System;
using ClientPacket.Contract;
using NKC.Templet;
using NKC.UI.Contract;
using NKM;
using NKM.Contract2;
using NKM.Event;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Module;

public class NKCUIModuleSubUIContract : NKCUIModuleSubUIBase
{
	[Header("UI")]
	public NKCUIComResourceButton m_btnContractLeft;

	public NKCUIComResourceButton m_btnContractRight;

	public Text m_lbContractRightTryCnt;

	public Text m_lbContractRightTryCntOff;

	public Text m_lbContractLeftTryCnt;

	public Image m_imgContractRightIcon;

	public GameObject m_objContractRightOn;

	public GameObject m_objContractRightOff;

	public NKCUIComStateButton m_csbtnShowList;

	public Image m_imgResourceIcon;

	public Text m_lbMiscCnt;

	private ContractTempletBase m_TargetContract;

	private int m_iCurMultiContractTryCnt;

	public override void Init()
	{
		NKCUtil.SetBindFunction(m_csbtnShowList, OnClickShowList);
		NKCUtil.SetLabelText(m_lbContractLeftTryCnt, string.Format(NKCUtilString.GET_STRING_CONTRACT_COUNT_ONE_PARAM, 1));
		NKCUtil.SetLabelText(m_lbContractRightTryCntOff, string.Format(NKCUtilString.GET_STRING_CONTRACT_COUNT_ONE_PARAM, 1));
	}

	public override void OnOpen(NKMEventCollectionIndexTemplet templet)
	{
		m_TargetContract = null;
		if (templet != null)
		{
			ContractTempletBase targetTemplet = GetTargetTemplet(templet.EventContractId);
			if (targetTemplet == null)
			{
				Debug.Log("<color=red>\ufffdش\ufffd\ufffdϴ\ufffd ä\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\u0378\ufffd ã\ufffd\ufffd \ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffdϴ\ufffd.</color>");
				return;
			}
			m_TargetContract = targetTemplet;
		}
		UpdateUI();
	}

	public override void Refresh()
	{
		UpdateUI();
	}

	private void UpdateUI()
	{
		if (m_TargetContract == null || !(m_TargetContract is ContractTempletV2) || !(m_TargetContract is ContractTempletV2 contractTempletV))
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < contractTempletV.m_SingleTryRequireItems.Length; i++)
		{
			MiscItemUnit reqItem = contractTempletV.m_SingleTryRequireItems[i];
			if (reqItem == null)
			{
				continue;
			}
			if (flag)
			{
				break;
			}
			NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
			ContractCostType _costType = ((i == 0) ? ContractCostType.Ticket : ContractCostType.Money);
			int num = (int)inventoryData.GetCountMiscItem(reqItem.ItemId);
			NKCUtil.SetLabelText(m_lbMiscCnt, num.ToString());
			m_iCurMultiContractTryCnt = Math.Min(num / reqItem.Count32, 10);
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(reqItem.ItemId);
			if (itemMiscTempletByID != null)
			{
				Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(itemMiscTempletByID);
				NKCUtil.SetImageSprite(m_imgResourceIcon, orLoadMiscItemSmallIcon);
				NKCUtil.SetImageSprite(m_imgContractRightIcon, orLoadMiscItemSmallIcon);
			}
			m_btnContractLeft.PointerClick.RemoveAllListeners();
			m_btnContractLeft.SetData(reqItem.ItemId, reqItem.Count32);
			if (num >= reqItem.Count)
			{
				m_btnContractLeft.PointerClick.AddListener(delegate
				{
					NKCPacketSender.Send_NKMPacket_CONTRACT_REQ(m_TargetContract.Key, _costType, 1);
				});
				flag = true;
			}
			else
			{
				m_btnContractLeft.PointerClick.AddListener(delegate
				{
					NKCShopManager.OpenItemLackPopup(reqItem.ItemId, reqItem.Count32);
				});
			}
			int num2 = Math.Max(1, m_iCurMultiContractTryCnt);
			m_btnContractRight.PointerClick.RemoveAllListeners();
			m_btnContractRight.SetData(reqItem.ItemId, reqItem.Count32 * num2);
			NKCUtil.SetLabelText(m_lbContractRightTryCnt, string.Format(NKCUtilString.GET_STRING_CONTRACT_COUNT_ONE_PARAM, num2.ToString()));
			if (m_iCurMultiContractTryCnt > 0 && num >= reqItem.Count32 * m_iCurMultiContractTryCnt)
			{
				m_btnContractRight.PointerClick.AddListener(delegate
				{
					NKCPacketSender.Send_NKMPacket_CONTRACT_REQ(m_TargetContract.Key, _costType, m_iCurMultiContractTryCnt);
				});
			}
			NKCUtil.SetGameobjectActive(m_objContractRightOn, m_iCurMultiContractTryCnt > 0);
			NKCUtil.SetGameobjectActive(m_objContractRightOff, m_iCurMultiContractTryCnt <= 0);
		}
	}

	public static ContractTempletBase GetTargetTemplet(int _contractID)
	{
		ContractTempletBase contractTempletBase = ContractTempletBase.FindBase(_contractID);
		if (contractTempletBase == null)
		{
			return null;
		}
		if (!contractTempletBase.EnableByTag)
		{
			return null;
		}
		NKCContractDataMgr nKCContractDataMgr = NKCScenManager.GetScenManager().GetNKCContractDataMgr();
		if (nKCContractDataMgr == null)
		{
			return null;
		}
		if (!NKCSynchronizedTime.IsEventTime(contractTempletBase.EventIntervalTemplet))
		{
			return null;
		}
		if (!nKCContractDataMgr.CheckOpenCond(contractTempletBase))
		{
			return null;
		}
		NKCContractCategoryTemplet nKCContractCategoryTemplet = NKCContractCategoryTemplet.Find(contractTempletBase.Category);
		if (nKCContractCategoryTemplet == null)
		{
			return null;
		}
		if (nKCContractCategoryTemplet.m_Type != NKCContractCategoryTemplet.TabType.Hidden)
		{
			return null;
		}
		return contractTempletBase;
	}

	private void OnClickShowList()
	{
		ContractTempletV2 contractTempletV = ContractTempletV2.Find(m_TargetContract.Key);
		if (contractTempletV != null)
		{
			NKCUIContractPopupRateV2.Instance.Open(contractTempletV);
		}
	}
}
