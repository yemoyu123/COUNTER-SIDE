using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupArtifactExchangeSlot : MonoBehaviour
{
	public NKCUISlot m_NKCUISlot;

	public Image m_imgGetMiscItemIcon;

	public Text m_lbGetMiscItemCount;

	private NKCAssetInstanceData m_NKCAssetInstanceData;

	public static NKCPopupArtifactExchangeSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WORLD_MAP_DIVE", "NKM_UI_DIVE_ARTIFACT_EXCHANGE_POPUP_SLOT");
		NKCPopupArtifactExchangeSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCPopupArtifactExchangeSlot>();
		if (component == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCPopupArtifactExchangeSlot Prefab null!");
			return null;
		}
		component.m_NKCAssetInstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		if (component.m_NKCUISlot != null)
		{
			component.m_NKCUISlot.Init();
		}
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void SetData(int artifactID)
	{
		m_NKCUISlot.SetDiveArtifactData(NKCUISlot.SlotData.MakeDiveArtifactData(artifactID), bShowName: false, bShowCount: false, bEnableLayoutElement: true, null);
		m_NKCUISlot.SetOnClickAction(default(NKCUISlot.SlotClickType));
		m_imgGetMiscItemIcon.sprite = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(NKMCommonConst.DiveArtifactReturnItemId);
		NKMDiveArtifactTemplet.Find(artifactID);
	}

	private void OnDestroy()
	{
		if (m_NKCAssetInstanceData != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceData);
		}
	}
}
