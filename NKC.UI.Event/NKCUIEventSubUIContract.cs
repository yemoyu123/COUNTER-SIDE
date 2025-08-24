using NKC.UI.Contract;
using NKM;
using NKM.Event;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI.Event;

public class NKCUIEventSubUIContract : NKCUIEventSubUIBase
{
	public NKM_SHORTCUT_TYPE m_ShortcutType;

	public string m_ShortcutParam;

	public Transform m_trBannerParent;

	private NKCAssetInstanceData m_AssetData;

	public override void Init()
	{
		base.Init();
		EventTrigger component = GetComponent<EventTrigger>();
		if (component != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(OnTouch);
			component.triggers.Clear();
			component.triggers.Add(entry);
		}
	}

	public override void Open(NKMEventTabTemplet tabTemplet)
	{
		m_tabTemplet = tabTemplet;
		if (m_tabTemplet != null)
		{
			m_ShortcutType = m_tabTemplet.m_ShortCutType;
			m_ShortcutParam = m_tabTemplet.m_ShortCut;
			SetDateLimit();
			if (m_trBannerParent.childCount == 0)
			{
				OpenInstanceByAssetName<NKCUIContractBanner>(m_tabTemplet.m_EventBannerPrefabName, m_tabTemplet.m_EventBannerPrefabName, m_trBannerParent).SetActiveEventTag(bValue: true);
			}
		}
	}

	private void OnDestroy()
	{
		NKCAssetResourceManager.CloseInstance(m_AssetData);
		m_AssetData = null;
	}

	public override void Refresh()
	{
	}

	private void OnTouch(BaseEventData baseEventData)
	{
		if (m_ShortcutType != NKM_SHORTCUT_TYPE.SHORTCUT_NONE && CheckEventTime())
		{
			NKCContentManager.MoveToShortCut(m_ShortcutType, m_ShortcutParam);
		}
	}

	public T OpenInstanceByAssetName<T>(string BundleName, string AssetName, Transform parent) where T : MonoBehaviour
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(BundleName, AssetName, bAsync: false, parent);
		if (nKCAssetInstanceData != null && nKCAssetInstanceData.m_Instant != null)
		{
			GameObject instant = nKCAssetInstanceData.m_Instant;
			T component = instant.GetComponent<T>();
			if (component == null)
			{
				Object.Destroy(instant);
				NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
				return null;
			}
			m_AssetData = nKCAssetInstanceData;
			return component;
		}
		Debug.LogWarning("prefab is null - " + BundleName + "/" + AssetName);
		return null;
	}
}
