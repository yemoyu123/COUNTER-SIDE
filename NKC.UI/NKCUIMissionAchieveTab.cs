using System;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIMissionAchieveTab : MonoBehaviour
{
	public delegate void OnClickTab(int tabID, bool bSet);

	private const string SPRITE_ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_mission_sprite";

	private const string LOCK_ICON_IMAGE_NAME = "AB_UI_NKM_UI_MISSION_ICON_LOCK";

	private const string SPRITE_ASSET_BUNDLE_NAME_GUIDE = "ui_mission_guide_sprite";

	public NKCUIComToggle m_Tgl;

	public Text m_lbTitle;

	public Image m_imgOnIcon;

	public Image m_imgOffIcon;

	public Image m_imgLockIcon;

	public GameObject m_objLock;

	public GameObject m_objOffComplete;

	public GameObject m_objOnComplete;

	public GameObject m_objNew;

	public GameObject m_objFesta;

	public Text m_lbFestaTime;

	private OnClickTab dOnClickTab;

	private NKCAssetInstanceData m_InstanceData;

	private int m_MissionTabID;

	private bool m_bLimited;

	private bool m_bLocked;

	private bool m_bCompleted;

	private DateTime m_tEndTime;

	private NKMMissionTabTemplet m_NKMMissionTabTemplet;

	private float m_tDeltaTime;

	private Color TITLE_COLOR_TAB_ON
	{
		get
		{
			if (m_NKMMissionTabTemplet != null && m_NKMMissionTabTemplet.m_MissionType == NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION)
			{
				return NKCUtil.GetColor("#222222");
			}
			return new Color(0.003921569f, 9f / 85f, 0.23137255f);
		}
	}

	private Color TITLE_COLOR_TAB_OFF
	{
		get
		{
			if (m_NKMMissionTabTemplet != null && m_NKMMissionTabTemplet.m_MissionType == NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION)
			{
				return NKCUtil.GetColor("#E5E5E5");
			}
			return Color.white;
		}
	}

	private Color TITLE_COLOR_TAB_LOCK
	{
		get
		{
			if (m_NKMMissionTabTemplet != null && m_NKMMissionTabTemplet.m_MissionType == NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION)
			{
				return NKCUtil.GetColor("#747474");
			}
			return Color.white;
		}
	}

	public static NKCUIMissionAchieveTab GetNewInstance(Transform parent, string bundleName, string assetName)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(bundleName, assetName);
		NKCUIMissionAchieveTab component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIMissionAchieveTab>();
		if (component == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUIMissionAchieveTab Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		component.m_Tgl.m_bGetCallbackWhileLocked = true;
		component.m_Tgl.OnValueChanged.RemoveAllListeners();
		component.m_Tgl.OnValueChanged.AddListener(component.OnValueChanged);
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		component.gameObject.SetActive(value: false);
		return component;
	}

	private void OnDestroy()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void SetData(NKMMissionTabTemplet tabTemplet, NKCUIComToggleGroup toggleGroup, OnClickTab onClickTab)
	{
		dOnClickTab = onClickTab;
		NKCUtil.SetGameobjectActive(m_objOffComplete, bValue: false);
		NKCUtil.SetGameobjectActive(m_objOnComplete, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNew, bValue: false);
		m_NKMMissionTabTemplet = tabTemplet;
		m_MissionTabID = tabTemplet.m_tabID;
		m_Tgl.SetToggleGroup(toggleGroup);
		NKCUtil.SetLabelText(m_lbTitle, tabTemplet.GetDesc());
		NKCUtil.SetImageSprite(m_imgOnIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>(GetSpriteBundleName(), tabTemplet.m_MissionTabIconName));
		NKCUtil.SetImageSprite(m_imgOffIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>(GetSpriteBundleName(), tabTemplet.m_MissionTabIconName));
		NKCUtil.SetImageSprite(m_imgLockIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_mission_sprite", "AB_UI_NKM_UI_MISSION_ICON_LOCK"));
		CheckTabState(tabTemplet, out m_bLocked, out m_bCompleted);
		SetLabelColor(m_Tgl.m_bSelect);
		m_bLimited = NKMMissionManager.TryGetMissionTabExpireUtcTime(tabTemplet, NKCScenManager.CurrentUserData(), out m_tEndTime);
		if (m_bLimited)
		{
			NKCUtil.SetLabelText(m_lbFestaTime, string.Format(NKCUtilString.GET_STRING_REMAIN_TIME_LEFT_ONE_PARAM, NKCUtilString.GetRemainTimeString(m_tEndTime, 1)));
		}
		NKCUtil.SetGameobjectActive(m_objFesta, m_bLimited);
	}

	private string GetSpriteBundleName()
	{
		if (m_NKMMissionTabTemplet != null && m_NKMMissionTabTemplet.m_MissionType == NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION)
		{
			return "ui_mission_guide_sprite";
		}
		return "ab_ui_nkm_ui_mission_sprite";
	}

	private void SetLabelColor(bool bSelected)
	{
		if (m_bLocked)
		{
			NKCUtil.SetLabelTextColor(m_lbTitle, TITLE_COLOR_TAB_LOCK);
		}
		else if (bSelected)
		{
			NKCUtil.SetLabelTextColor(m_lbTitle, TITLE_COLOR_TAB_ON);
		}
		else
		{
			NKCUtil.SetLabelTextColor(m_lbTitle, TITLE_COLOR_TAB_OFF);
		}
	}

	public void SetSelected(bool bSelected)
	{
		m_Tgl.Select(bSelected, bForce: true, bImmediate: true);
		SetLabelColor(bSelected);
	}

	public void SetCompleteObject(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objOffComplete, bSet);
		NKCUtil.SetGameobjectActive(m_objOnComplete, bSet);
	}

	public void SetNewObject(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objNew, bSet);
	}

	public void SetLockObject(bool bSkipCompletLock = false)
	{
		NKCUtil.SetGameobjectActive(m_objLock, m_bLocked);
		if ((m_bLocked && !m_NKMMissionTabTemplet.m_VisibleWhenLocked) || (m_bCompleted && !bSkipCompletLock))
		{
			m_Tgl.Lock();
		}
		else
		{
			m_Tgl.UnLock();
		}
	}

	public NKCUIComToggle GetToggle()
	{
		return m_Tgl;
	}

	public bool GetLocked()
	{
		return m_bLocked;
	}

	public bool GetCompleted()
	{
		return m_bCompleted;
	}

	public void RefreshTab()
	{
		CheckTabState(m_NKMMissionTabTemplet, out m_bLocked, out m_bCompleted);
	}

	public void OnValueChanged(bool bSet)
	{
		if (m_Tgl.m_bLock)
		{
			if (m_NKMMissionTabTemplet != null && m_NKMMissionTabTemplet.m_MissionType == NKM_MISSION_TYPE.COMBINE_GUIDE_MISSION)
			{
				string missionTabUnlockCondition = NKMMissionManager.GetMissionTabUnlockCondition(m_MissionTabID, NKCScenManager.CurrentUserData());
				if (!string.IsNullOrEmpty(missionTabUnlockCondition))
				{
					NKCPopupMessageManager.AddPopupMessage(missionTabUnlockCondition);
				}
			}
			else if (m_objOffComplete.activeInHierarchy)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_MISSION_COMPLETE_GROWTH_TAB);
			}
			else
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_MISSION_LOCK_GROWTH_TAB);
			}
			return;
		}
		SetLabelColor(bSet);
		if (!bSet)
		{
			return;
		}
		if (m_bLocked)
		{
			string missionTabUnlockCondition2 = NKMMissionManager.GetMissionTabUnlockCondition(m_MissionTabID, NKCScenManager.CurrentUserData());
			if (!string.IsNullOrEmpty(missionTabUnlockCondition2))
			{
				NKCPopupMessageManager.AddPopupMessage(missionTabUnlockCondition2);
			}
		}
		if (!m_bLocked || m_NKMMissionTabTemplet.m_VisibleWhenLocked)
		{
			dOnClickTab?.Invoke(m_MissionTabID, m_Tgl.m_bChecked);
		}
	}

	private void CheckTabState(NKMMissionTabTemplet tabTemplet, out bool bLocked, out bool bCompleted)
	{
		bCompleted = false;
		bLocked = false;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(tabTemplet.m_completeMissionID);
		if (missionTemplet != null)
		{
			NKMMissionData missionData = nKMUserData.m_MissionData.GetMissionData(missionTemplet);
			if (missionData != null && missionData.IsComplete)
			{
				bCompleted = true;
			}
		}
		NKMMissionTemplet missionTemplet2 = NKMMissionManager.GetMissionTemplet(tabTemplet.m_firstMissionID);
		if (missionTemplet2 != null && missionTemplet2.m_MissionRequire > 0 && (nKMUserData.m_MissionData.GetMissionDataByMissionId(missionTemplet2.m_MissionRequire) == null || !nKMUserData.m_MissionData.GetMissionDataByMissionId(missionTemplet2.m_MissionRequire).IsComplete))
		{
			bLocked = true;
		}
		if (!NKMContentUnlockManager.IsContentUnlocked(nKMUserData, in tabTemplet.m_UnlockInfo))
		{
			bLocked = true;
		}
		NKCUtil.SetGameobjectActive(m_objLock, bLocked);
		if (bLocked | bCompleted)
		{
			m_Tgl.Lock();
			SetLabelColor(bSelected: false);
		}
		else
		{
			m_Tgl.UnLock();
		}
		NKCUtil.SetGameobjectActive(m_objOffComplete, bCompleted);
		NKCUtil.SetGameobjectActive(m_objOnComplete, bCompleted);
	}

	private void Update()
	{
		if (m_bLimited && !m_bLocked && !m_bCompleted)
		{
			m_tDeltaTime += Time.deltaTime;
			if (m_tDeltaTime > 1f)
			{
				m_tDeltaTime -= 1f;
				NKCUtil.SetLabelText(m_lbFestaTime, string.Format(NKCUtilString.GET_STRING_REMAIN_TIME_LEFT_ONE_PARAM, NKCUtilString.GetRemainTimeString(m_tEndTime, 1)));
			}
		}
	}
}
