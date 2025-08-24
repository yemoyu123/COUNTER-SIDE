using BehaviorDesigner.Runtime;
using NKC.UI.NPC;
using NKM;
using UnityEngine;

namespace NKC.Office;

public class NKCOfficeCharacterNPC : NKCOfficeCharacter
{
	public string SpineAssetName;

	public string BTAssetName;

	public NPC_TYPE m_eNPCType;

	private const string NPC_BUNDLE_NAME = "ab_unit_office_sd";

	private const string NPC_ASSET_NAME = "UNIT_OFFICE_SD_NPC";

	public static NKCOfficeCharacterNPC GetNPCInstance()
	{
		return GetNPCInstance(new NKMAssetName("ab_unit_office_sd", "UNIT_OFFICE_SD_NPC"));
	}

	public static NKCOfficeCharacterNPC GetNPCInstance(NKMAssetName assetName)
	{
		GameObject gameObject = Object.Instantiate(NKCResourceUtility.GetOrLoadAssetResource<GameObject>(assetName));
		if (gameObject == null)
		{
			return null;
		}
		NKCOfficeCharacterNPC component = gameObject.GetComponent<NKCOfficeCharacterNPC>();
		if (component == null)
		{
			Debug.LogError("NKCUIOfficeCharacter loadprefab failed!");
			Object.DestroyImmediate(gameObject);
			return null;
		}
		return component;
	}

	public virtual void Init(NKCOfficeBuildingBase officeBuilding)
	{
		NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(SpineAssetName, SpineAssetName);
		SetSpineIllust(NKCResourceUtility.OpenSpineSD(nKMAssetName.m_BundleName, nKMAssetName.m_BundleName), bSetParent: true);
		if (m_SDIllust != null)
		{
			m_SDIllust.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE);
			m_SDIllust.GetRectTransform().localPosition = Vector3.zero;
			m_SDIllust.GetRectTransform().pivot = new Vector2(0.5f, 0.5f);
			m_SDIllust.GetRectTransform().anchorMin = new Vector2(0.5f, 0.5f);
			m_SDIllust.GetRectTransform().anchorMax = new Vector2(0.5f, 0.5f);
		}
		base.transform.rotation = Quaternion.identity;
		m_OfficeBuilding = officeBuilding;
		BT = GetComponent<BehaviorTree>();
		if (BT == null)
		{
			Debug.LogWarning("Office SD : BT Not found. Using Default BT");
			BT = base.gameObject.AddComponent<BehaviorTree>();
			BT.StartWhenEnabled = false;
			if (string.IsNullOrEmpty(BTAssetName))
			{
				BT.ExternalBehavior = NKCResourceUtility.GetOrLoadAssetResource<ExternalBehavior>("ab_ui_office_bt", "OFFICE_BT_DEFAULT");
			}
			else
			{
				BT.ExternalBehavior = NKCResourceUtility.GetOrLoadAssetResource<ExternalBehavior>("ab_ui_office_bt", BTAssetName);
			}
		}
		else
		{
			BT.StartWhenEnabled = false;
			if (!string.IsNullOrEmpty(BTAssetName))
			{
				BT.ExternalBehavior = NKCResourceUtility.GetOrLoadAssetResource<ExternalBehavior>("ab_ui_office_bt", BTAssetName);
			}
		}
		BT.DisableBehavior();
		m_UnitData = null;
		NKCUtil.SetGameobjectActive(m_comLoyalty, bValue: false);
		BT.RestartWhenComplete = true;
		BT.OnBehaviorRestart += base.OnBTRestart;
		base.transform.SetParent(officeBuilding.trActorRoot);
	}

	protected override void PlayTouchVoice()
	{
		NKCNPCTemplet nPCTemplet = NKCUINPCBase.GetNPCTemplet(m_eNPCType, NPC_ACTION_TYPE.TOUCH);
		if (nPCTemplet != null)
		{
			NKCUINPCBase.PlayVoice(m_eNPCType, nPCTemplet, bStopCurrentSound: true, bIgnoreCoolTime: false, bShowCaption: true);
		}
	}
}
