using System;
using NKM.EventPass;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIEventPassRewardSlot : MonoBehaviour
{
	[Serializable]
	public struct EventPassRewardGroup
	{
		public NKCUISlot rewardIconSlot;

		public NKCUIComStateButton csbtnGetReward;

		public GameObject objRewardLocked;

		public void Release()
		{
			rewardIconSlot = null;
			csbtnGetReward = null;
			objRewardLocked = null;
		}
	}

	public delegate void dOnClickGetReward();

	public Text m_lbPassLevel;

	public Color m_colCompleteText;

	public Color m_colProceedText;

	public Color m_colLockText;

	public Image m_imgCenterGauge;

	public GameObject m_objComplete;

	public GameObject m_objProceeding;

	public GameObject m_objLock;

	public GameObject m_objProcedingLine;

	public GameObject m_objCompleteFull;

	public EventPassRewardGroup m_coreRewardGroup;

	public EventPassRewardGroup m_normalRewardGroup;

	private NKCAssetInstanceData m_InstanceData;

	private dOnClickGetReward m_dOnClickGetReward;

	private void OnDestroy()
	{
		m_lbPassLevel = null;
		m_objComplete = null;
		m_objProceeding = null;
		m_objLock = null;
		m_objProcedingLine = null;
		m_objCompleteFull = null;
		m_normalRewardGroup.Release();
		m_coreRewardGroup.Release();
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
	}

	public static NKCUIEventPassRewardSlot GetNewInstance(Transform parent, bool bMentoringSlot = false)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_EVENT_PASS", "NKM_UI_EVENT_PASS_SLOT");
		NKCUIEventPassRewardSlot nKCUIEventPassRewardSlot = nKCAssetInstanceData?.m_Instant.GetComponent<NKCUIEventPassRewardSlot>();
		if (nKCUIEventPassRewardSlot == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUIEventPassRewardSlot Prefab null!");
			return null;
		}
		nKCUIEventPassRewardSlot.m_InstanceData = nKCAssetInstanceData;
		nKCUIEventPassRewardSlot.Init();
		if (parent != null)
		{
			nKCUIEventPassRewardSlot.transform.SetParent(parent);
		}
		nKCUIEventPassRewardSlot.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		nKCUIEventPassRewardSlot.gameObject.SetActive(value: false);
		return nKCUIEventPassRewardSlot;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void Init()
	{
		m_normalRewardGroup.rewardIconSlot?.Init();
		m_coreRewardGroup.rewardIconSlot?.Init();
		NKCUtil.SetButtonClickDelegate(m_normalRewardGroup.csbtnGetReward, OnClickGetReward);
		NKCUtil.SetButtonClickDelegate(m_coreRewardGroup.csbtnGetReward, OnClickGetReward);
	}

	public void SetData(NKMEventPassRewardTemplet passRewardTemplet, int userPassLevel, int maxPassLevel, bool corePassPurchased, int normalRewardLevel, int coreRewardLevel, dOnClickGetReward onClickGetReward)
	{
		if (passRewardTemplet == null)
		{
			m_dOnClickGetReward = null;
			return;
		}
		NKCUtil.SetLabelText(m_lbPassLevel, $"{passRewardTemplet.PassLevel:00}");
		bool flag = passRewardTemplet.PassLevel <= userPassLevel;
		bool flag2 = passRewardTemplet.PassLevel <= normalRewardLevel;
		SetNormalRewardGroup(m_normalRewardGroup, passRewardTemplet, flag, flag2);
		bool flag3 = passRewardTemplet.PassLevel <= coreRewardLevel;
		SetCoreRewardGroup(m_coreRewardGroup, passRewardTemplet, flag, flag3, corePassPurchased);
		bool flag4 = (flag2 && !corePassPurchased) || (flag2 && corePassPurchased && flag3);
		NKCUtil.SetGameobjectActive(m_objCompleteFull, flag4);
		NKCUtil.SetGameobjectActive(m_objProcedingLine, flag && !flag4);
		if (flag)
		{
			NKCUtil.SetGameobjectActive(m_objProceeding, !flag4);
			NKCUtil.SetGameobjectActive(m_objComplete, flag4);
			NKCUtil.SetGameobjectActive(m_objLock, bValue: false);
			NKCUtil.SetLabelTextColor(m_lbPassLevel, flag4 ? m_colCompleteText : m_colProceedText);
			if (userPassLevel == passRewardTemplet.PassLevel || userPassLevel == maxPassLevel)
			{
				NKCUtil.SetImageFillAmount(m_imgCenterGauge, 0.5f);
			}
			else
			{
				NKCUtil.SetImageFillAmount(m_imgCenterGauge, 1f);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objLock, bValue: true);
			NKCUtil.SetImageFillAmount(m_imgCenterGauge, 0f);
			NKCUtil.SetLabelTextColor(m_lbPassLevel, m_colLockText);
		}
		m_dOnClickGetReward = onClickGetReward;
	}

	private void SetNormalRewardGroup(EventPassRewardGroup normalRewardGroup, NKMEventPassRewardTemplet passRewardTemplet, bool isUnderUserPassLevel, bool normalRewardReceiced)
	{
		NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(passRewardTemplet.NormalRewardItemType, passRewardTemplet.NormalRewardItemId, passRewardTemplet.NormalRewardItemCount);
		if (normalRewardGroup.rewardIconSlot != null)
		{
			if (slotData.eType == NKCUISlot.eSlotMode.ItemMisc)
			{
				normalRewardGroup.rewardIconSlot.SetData(slotData, bEnableLayoutElement: true, OnClickRewardIcon);
			}
			else
			{
				normalRewardGroup.rewardIconSlot.SetData(slotData);
			}
			normalRewardGroup.rewardIconSlot.SetCompleteMark(normalRewardReceiced);
		}
		NKCUtil.SetGameobjectActive(normalRewardGroup.csbtnGetReward?.gameObject, isUnderUserPassLevel);
		normalRewardGroup.csbtnGetReward?.SetLock(normalRewardReceiced);
	}

	private void SetCoreRewardGroup(EventPassRewardGroup coreRewardGroup, NKMEventPassRewardTemplet passRewardTemplet, bool isUnderUserPassLevel, bool coreRewardRecieved, bool corePassPurchased)
	{
		NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeRewardTypeData(passRewardTemplet.CoreRewardItemType, passRewardTemplet.CoreRewardItemId, passRewardTemplet.CoreRewardItemCount);
		if (coreRewardGroup.rewardIconSlot != null)
		{
			if (slotData.eType == NKCUISlot.eSlotMode.ItemMisc)
			{
				coreRewardGroup.rewardIconSlot.SetData(slotData, bEnableLayoutElement: true, OnClickRewardIcon);
			}
			else
			{
				coreRewardGroup.rewardIconSlot.SetData(slotData);
			}
			coreRewardGroup.rewardIconSlot.SetCompleteMark(coreRewardRecieved);
			coreRewardGroup.rewardIconSlot.SetDisable(!corePassPurchased);
		}
		if (corePassPurchased)
		{
			NKCUtil.SetGameobjectActive(coreRewardGroup.objRewardLocked, bValue: false);
			NKCUtil.SetGameobjectActive(coreRewardGroup.csbtnGetReward?.gameObject, isUnderUserPassLevel);
			coreRewardGroup.csbtnGetReward?.SetLock(coreRewardRecieved);
		}
		else
		{
			NKCUtil.SetGameobjectActive(coreRewardGroup.objRewardLocked, bValue: true);
			NKCUtil.SetGameobjectActive(coreRewardGroup.csbtnGetReward?.gameObject, bValue: false);
		}
	}

	private void OnClickRewardIcon(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKCPopupItemBox.Instance.Open(NKCPopupItemBox.eMode.Normal, slotData, null, singleOpenOnly: false, bShowCount: false, showDropInfo: false);
	}

	private void OnClickGetReward()
	{
		if (m_dOnClickGetReward != null)
		{
			m_dOnClickGetReward();
		}
	}
}
