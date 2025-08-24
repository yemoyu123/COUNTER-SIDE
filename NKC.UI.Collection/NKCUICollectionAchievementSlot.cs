using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionAchievementSlot : MonoBehaviour
{
	public delegate void OnComplete(int unitId, int missionId, int stepId);

	public Text m_lbMissionDesc;

	public Text m_lbMissionCount;

	public NKCUISlot m_RewardSlot;

	public GameObject m_objCount;

	public GameObject m_objClear;

	public GameObject m_objComplete;

	public GameObject m_objClearLine;

	private int m_UnitId;

	private NKCAssetInstanceData m_InstanceData;

	private NKMUnitMissionStepTemplet m_UnitMissionStepTemplet;

	private OnComplete m_OnComplete;

	public void Init()
	{
		m_RewardSlot.Init();
	}

	public static NKCUICollectionAchievementSlot GetNewInstance(Transform parent, bool bMentoringSlot = false)
	{
		NKCAssetInstanceData nKCAssetInstanceData = null;
		nKCAssetInstanceData = ((!NKCCollectionManager.IsCollectionV2Active) ? NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_nkm_ui_collection", "NKM_UI_POPUP_COLLECTION_ACHIEVEMENT_SLOT") : NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_collection", "AB_UI_COLLECTION_POPUP_ACHIEVEMENT_SLOT"));
		NKCUICollectionAchievementSlot nKCUICollectionAchievementSlot = nKCAssetInstanceData?.m_Instant.GetComponent<NKCUICollectionAchievementSlot>();
		if (nKCUICollectionAchievementSlot == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKM_UI_POPUP_COLLECTION_ACHIEVEMENT_SLOT Prefab null!");
			return null;
		}
		nKCUICollectionAchievementSlot.m_InstanceData = nKCAssetInstanceData;
		nKCUICollectionAchievementSlot.Init();
		if (parent != null)
		{
			nKCUICollectionAchievementSlot.transform.SetParent(parent);
		}
		nKCUICollectionAchievementSlot.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		nKCUICollectionAchievementSlot.gameObject.SetActive(value: false);
		return nKCUICollectionAchievementSlot;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		m_UnitMissionStepTemplet = null;
		Object.Destroy(base.gameObject);
	}

	public void SetData(int unitId, NKMUnitMissionStepTemplet unitMissionStepTemplet, NKMMissionManager.MissionStateData missionStateData, OnComplete onComplete = null)
	{
		m_UnitId = unitId;
		if (unitMissionStepTemplet != null)
		{
			m_UnitMissionStepTemplet = unitMissionStepTemplet;
			m_OnComplete = onComplete;
			NKCUtil.SetLabelText(m_lbMissionDesc, NKCStringTable.GetString(unitMissionStepTemplet.MissionDesc));
			NKCUtil.SetLabelText(m_lbMissionCount, $"{missionStateData.progressCount}/{unitMissionStepTemplet.MissionValue}");
			NKCUtil.SetGameobjectActive(m_objCount, missionStateData.state == NKMMissionManager.MissionState.ONGOING);
			NKCUtil.SetGameobjectActive(m_objClear, missionStateData.state != NKMMissionManager.MissionState.ONGOING);
			NKCUtil.SetGameobjectActive(m_objComplete, missionStateData.state == NKMMissionManager.MissionState.COMPLETED);
			NKCUtil.SetGameobjectActive(m_objClearLine, missionStateData.state == NKMMissionManager.MissionState.CAN_COMPLETE);
			NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(unitMissionStepTemplet.RewardInfo);
			if (missionStateData.state == NKMMissionManager.MissionState.CAN_COMPLETE)
			{
				m_RewardSlot.SetData(data, bEnableLayoutElement: true, OnClickSlot);
			}
			else
			{
				m_RewardSlot.SetData(data);
			}
			m_RewardSlot.SetRewardFx(missionStateData.state == NKMMissionManager.MissionState.CAN_COMPLETE);
			m_RewardSlot.SetCompleteMark(missionStateData.state == NKMMissionManager.MissionState.COMPLETED);
		}
	}

	private void OnClickSlot(NKCUISlot.SlotData slotData, bool bLocked)
	{
		if (m_OnComplete != null)
		{
			int rewardEnableStepId = NKCUnitMissionManager.GetRewardEnableStepId(m_UnitId, m_UnitMissionStepTemplet.Owner.MissionId);
			if (rewardEnableStepId > 0)
			{
				m_OnComplete(m_UnitId, m_UnitMissionStepTemplet.Owner.MissionId, rewardEnableStepId);
			}
		}
	}
}
