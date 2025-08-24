using NKM;
using Spine.Unity;
using UnityEngine;

namespace NKC.UI;

public class NKCUIComPrefabLoader : MonoBehaviour, INKCUIValidator
{
	public string m_BundleName;

	public string m_AssetName;

	public Vector3 Position = Vector3.zero;

	public Vector3 Scale = Vector3.one;

	public GameObject m_targetObj;

	public bool m_bUseTwoPassTransparency;

	private NKCAssetInstanceData m_InstanceData;

	private bool m_bValidated;

	private void Awake()
	{
		Validate();
	}

	public void Validate()
	{
		if (m_bValidated)
		{
			return;
		}
		m_bValidated = true;
		if (m_targetObj == null || m_targetObj.gameObject.name.Contains("Missing Prefab"))
		{
			ReplaceObject();
		}
		else
		{
			Transform transform = m_targetObj.transform.Find("SPINE_SkeletonGraphic");
			if (transform != null)
			{
				SkeletonGraphic component = transform.GetComponent<SkeletonGraphic>();
				if (component == null || component.SkeletonDataAsset == null)
				{
					RebuildSpine(component);
				}
			}
		}
		if (m_bUseTwoPassTransparency && m_targetObj != null)
		{
			SkeletonGraphic[] componentsInChildren = m_targetObj.GetComponentsInChildren<SkeletonGraphic>(includeInactive: true);
			NKCASMaterial nKCASMaterial = (NKCASMaterial)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASMaterial, "shaders", "SkeletonGraphic2Pass");
			SkeletonGraphic[] array = componentsInChildren;
			foreach (SkeletonGraphic obj in array)
			{
				obj.material = nKCASMaterial.m_Material;
				obj.MeshGenerator.settings.zSpacing = -0.0001f;
			}
			nKCASMaterial.Close();
			nKCASMaterial.Unload();
		}
	}

	private void RebuildSpine(SkeletonGraphic graphic)
	{
		NKCAssetResourceData nKCAssetResourceData = NKCAssetResourceManager.OpenResource<GameObject>(m_BundleName, m_AssetName);
		if (nKCAssetResourceData == null)
		{
			return;
		}
		GameObject asset = nKCAssetResourceData.GetAsset<GameObject>();
		if (asset == null)
		{
			return;
		}
		Transform transform = asset.transform.Find("SPINE_SkeletonGraphic");
		if (!(transform == null))
		{
			graphic.transform.localPosition = transform.localPosition;
			graphic.transform.localRotation = transform.localRotation;
			graphic.transform.localScale = transform.localScale;
			SkeletonGraphic component = transform.GetComponent<SkeletonGraphic>();
			if (!(component == null))
			{
				graphic.skeletonDataAsset = component.skeletonDataAsset;
				graphic.Initialize(overwrite: true);
				NKCAssetResourceManager.CloseResource(nKCAssetResourceData);
			}
		}
	}

	private void ReplaceObject()
	{
		if (m_targetObj != null)
		{
			Object.Destroy(m_targetObj);
		}
		if (m_InstanceData == null)
		{
			m_InstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(m_BundleName, m_AssetName);
		}
		m_targetObj = m_InstanceData.m_Instant;
		if (m_targetObj != null)
		{
			m_targetObj.transform.parent = base.transform;
			m_targetObj.transform.localPosition = Position;
			m_targetObj.transform.localScale = Scale;
		}
	}

	private void OnDestroy()
	{
		if (m_InstanceData != null)
		{
			NKCAssetResourceManager.CloseInstance(m_InstanceData);
		}
	}
}
