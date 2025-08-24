using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guide;

public class NKCUIStatInfoSlot : MonoBehaviour
{
	public Text m_title;

	public Text m_Desc;

	private NKCAssetInstanceData m_Instance;

	public static NKCUIStatInfoSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("nkm_ui_tutorial_pf_unit", "TUTORIAL_PF_UNIT_STAT_SLOT");
		if (nKCAssetInstanceData == null || nKCAssetInstanceData.m_Instant == null)
		{
			Debug.LogError("TUTORIAL_PF_UNIT_STAT_SLOT Prefab null!");
			return null;
		}
		NKCUIStatInfoSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIStatInfoSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIUnitInfoDetailStatSlot null!");
			return null;
		}
		component.m_Instance = nKCAssetInstanceData;
		component.transform.SetParent(parent, worldPositionStays: false);
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_Instance);
		m_Instance = null;
		Object.Destroy(base.gameObject);
	}

	public void SetData(string title, string desc)
	{
		NKCUtil.SetLabelText(m_title, title);
		NKCUtil.SetLabelText(m_Desc, desc);
	}
}
