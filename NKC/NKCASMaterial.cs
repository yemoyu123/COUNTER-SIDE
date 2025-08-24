using NKM;
using UnityEngine;

namespace NKC;

public class NKCASMaterial : NKMObjectPoolData
{
	public NKCAssetResourceData m_MaterialOrg;

	public Material m_Material;

	public NKCASMaterial(string bundleName, string name, bool bAsync = false)
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKCASMaterial;
		m_ObjectPoolBundleName = bundleName;
		m_ObjectPoolName = name;
		Load(bAsync);
	}

	public override void Load(bool bAsync)
	{
		m_MaterialOrg = NKCAssetResourceManager.OpenResource<Material>(m_ObjectPoolBundleName, m_ObjectPoolName, bAsync);
	}

	public override bool LoadComplete()
	{
		if (m_MaterialOrg == null || m_MaterialOrg.GetAsset<Material>() == null)
		{
			return false;
		}
		m_Material = new Material(m_MaterialOrg.GetAsset<Material>());
		return true;
	}

	public override void Unload()
	{
		m_Material = null;
		NKCAssetResourceManager.CloseResource(m_MaterialOrg);
		m_MaterialOrg = null;
	}
}
