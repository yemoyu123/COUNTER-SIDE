using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component;

public class NKCUIComDragSelectablePanelSlot : MonoBehaviour
{
	public delegate void OnClick(int data);

	public NKCUIComStateButton m_Button;

	public Image m_Image;

	public RectTransform m_rtPrefabRoot;

	private NKCAssetInstanceData m_prefabInstance;

	private int m_Data;

	private OnClick dOnClick;

	public void SetPrefabData(NKMAssetName assetName, int data, OnClick onClick)
	{
		CleanUp();
		NKCUtil.SetButtonClickDelegate(m_Button, OnButton);
		m_Data = data;
		dOnClick = onClick;
		m_prefabInstance = NKCAssetResourceManager.OpenInstance<GameObject>(assetName);
		if (m_prefabInstance != null && m_prefabInstance.m_Instant != null)
		{
			m_prefabInstance.m_Instant.transform.SetParent(m_rtPrefabRoot, worldPositionStays: false);
		}
		else
		{
			Debug.Log($"SetPrefabData Fail, file : {assetName}");
		}
		NKCUtil.SetGameobjectActive(m_Image, bValue: false);
		NKCUtil.SetGameobjectActive(m_rtPrefabRoot, bValue: true);
	}

	public void SetImageData(NKMAssetName assetName, int data, OnClick onClick)
	{
		CleanUp();
		NKCUtil.SetButtonClickDelegate(m_Button, OnButton);
		m_Data = data;
		dOnClick = onClick;
		NKCUtil.SetImageSprite(m_Image, NKCResourceUtility.GetOrLoadAssetResource<Sprite>(assetName));
		NKCUtil.SetGameobjectActive(m_Image, bValue: true);
		NKCUtil.SetGameobjectActive(m_rtPrefabRoot, bValue: false);
	}

	private void CleanUp()
	{
		if (m_prefabInstance != null)
		{
			NKCAssetResourceManager.CloseInstance(m_prefabInstance);
			m_prefabInstance = null;
		}
		dOnClick = null;
	}

	private void OnButton()
	{
		dOnClick?.Invoke(m_Data);
	}

	private void OnDestroy()
	{
		CleanUp();
	}
}
