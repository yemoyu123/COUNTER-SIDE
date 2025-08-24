using System.Collections.Generic;
using Cs.Logging;
using NKC.FX;
using NKM;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace NKC;

public class NKCASUnitSpineSprite : NKMObjectPoolData
{
	public NKCAssetInstanceData m_UnitSpineSpriteInstant;

	public GameObject m_SPINE_SkeletonAnimationOrg;

	public MeshRenderer m_MeshRendererOrg;

	public SkeletonAnimation m_cSkeletonAnimation;

	public string m_SortingLayerNameOrg;

	public GameObject m_SPINE_SkeletonAnimation;

	public MeshRenderer m_MeshRenderer;

	private NKCASMaterialPropertyBlockAdapter m_MPBAdapter;

	protected Material m_Material;

	protected Material m_DissolveMaterial;

	protected Color m_DissolveColorOrg;

	protected NKCAssetResourceData m_ReplaceMatResource;

	protected Material m_ReplaceMaterial;

	protected Material m_ReplaceDissolveMaterial;

	public ParticleSystemRenderer[] m_MainParticleSystemRenderer;

	protected Color m_Color = new Color(1f, 1f, 1f, 1f);

	public Bone m_Bone_Move;

	public Vector3 m_Bone_MovePos;

	public Bone m_Bone_Head;

	public Vector3 m_Bone_HeadPos;

	private Dictionary<string, Bone> m_dicBone = new Dictionary<string, Bone>();

	public NKCASUnitSpineSprite(string bundleName, string name, bool bAsync = false)
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitSpineSprite;
		m_ObjectPoolBundleName = bundleName;
		m_ObjectPoolName = name;
		m_bUnloadable = true;
		Load(bAsync);
	}

	public override void Load(bool bAsync)
	{
		m_UnitSpineSpriteInstant = NKCAssetResourceManager.OpenInstance<GameObject>(m_ObjectPoolBundleName, m_ObjectPoolName, bAsync);
	}

	public void SetReplaceMatResource(string bundleName, string assetName, bool bAsync = false)
	{
		if (m_ReplaceMatResource != null)
		{
			NKCAssetResourceManager.CloseResource(m_ReplaceMatResource);
			m_ReplaceMatResource = null;
		}
		m_ReplaceMaterial = null;
		if (assetName.Length > 1)
		{
			m_ReplaceMatResource = NKCAssetResourceManager.OpenResource<Material>(bundleName, assetName, bAsync);
		}
	}

	public void SetOverrideMaterial()
	{
		if (!m_bIsLoaded)
		{
			return;
		}
		if (m_cSkeletonAnimation == null)
		{
			ClearReplaceMat();
			return;
		}
		if (m_ReplaceMatResource == null)
		{
			ClearReplaceMat();
			return;
		}
		if (!m_ReplaceMatResource.IsDone())
		{
			ClearReplaceMat();
			return;
		}
		Material asset = m_ReplaceMatResource.GetAsset<Material>();
		if (asset == null)
		{
			ClearReplaceMat();
			return;
		}
		m_ReplaceMaterial = new Material(asset);
		m_ReplaceDissolveMaterial = new Material(asset);
		m_ReplaceDissolveMaterial.EnableKeyword("DISSOLVE_ON");
		m_cSkeletonAnimation.CustomMaterialOverride.Clear();
		if (!m_cSkeletonAnimation.CustomMaterialOverride.ContainsKey(m_Material))
		{
			m_cSkeletonAnimation.CustomMaterialOverride.Add(m_Material, m_ReplaceMaterial);
		}
	}

	protected void ClearReplaceMat()
	{
		m_cSkeletonAnimation?.CustomMaterialOverride.Clear();
		m_ReplaceMaterial = null;
		m_ReplaceDissolveMaterial = null;
	}

	public override bool LoadComplete()
	{
		if (m_UnitSpineSpriteInstant == null || m_UnitSpineSpriteInstant.m_Instant == null)
		{
			return false;
		}
		NKCComSpineAnimControl component = m_UnitSpineSpriteInstant.m_Instant.GetComponent<NKCComSpineAnimControl>();
		if (component != null)
		{
			component.enabled = false;
		}
		Transform transform = m_UnitSpineSpriteInstant.m_InstantOrg.GetAsset<GameObject>().transform.Find("SPINE_SkeletonAnimation");
		if (transform == null)
		{
			Log.Error(m_ObjectPoolName + " has no sub prefab name SPINE_SkeletonAnimation", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCASUnitSpineSprite.cs", 138);
		}
		m_SPINE_SkeletonAnimationOrg = transform.gameObject;
		m_MeshRendererOrg = m_SPINE_SkeletonAnimationOrg.GetComponentInChildren<MeshRenderer>(includeInactive: true);
		m_SortingLayerNameOrg = m_MeshRendererOrg.sortingLayerName;
		if (m_MeshRendererOrg.sharedMaterial == null)
		{
			Debug.LogErrorFormat("m_MeshRendererOrg.sharedMaterial is null : {0} / {1}", m_ObjectPoolBundleName, m_ObjectPoolName);
			m_DissolveMaterial = null;
		}
		else
		{
			m_DissolveMaterial = new Material(m_MeshRendererOrg.sharedMaterial);
		}
		if (m_DissolveMaterial != null)
		{
			m_DissolveMaterial.EnableKeyword("DISSOLVE_ON");
		}
		m_SPINE_SkeletonAnimation = m_UnitSpineSpriteInstant.m_Instant.transform.Find("SPINE_SkeletonAnimation").gameObject;
		m_MeshRenderer = m_SPINE_SkeletonAnimation.GetComponentInChildren<MeshRenderer>(includeInactive: true);
		if (m_MeshRenderer == null)
		{
			Debug.LogErrorFormat("m_MeshRenderer is null : {0} / {1}", m_ObjectPoolBundleName, m_ObjectPoolName);
		}
		else
		{
			m_Material = m_MeshRenderer.sharedMaterial;
			if (!m_MeshRenderer.gameObject.TryGetComponent<NKCASMaterialPropertyBlockAdapter>(out m_MPBAdapter))
			{
				m_MPBAdapter = m_MeshRenderer.gameObject.AddComponent<NKCASMaterialPropertyBlockAdapter>();
			}
			m_MPBAdapter.Init(bApplyCleanMPB: false);
		}
		m_MainParticleSystemRenderer = m_UnitSpineSpriteInstant.m_Instant.GetComponentsInChildren<ParticleSystemRenderer>(includeInactive: true);
		NKCUtil.SetParticleSystemRendererSortOrder(m_MainParticleSystemRenderer);
		m_Color.r = 1f;
		m_Color.g = 1f;
		m_Color.b = 1f;
		m_Color.a = 1f;
		if (m_DissolveMaterial != null)
		{
			m_DissolveColorOrg = m_DissolveMaterial.GetColor("_DissolveGlowColor");
		}
		m_cSkeletonAnimation = m_SPINE_SkeletonAnimation.GetComponent<SkeletonAnimation>();
		if (m_cSkeletonAnimation != null)
		{
			m_Bone_Move = m_cSkeletonAnimation.skeleton.FindBone("MOVE");
			m_Bone_Head = m_cSkeletonAnimation.skeleton.FindBone("BIP01_HEAD");
		}
		SetOverrideMaterial();
		if (m_cSkeletonAnimation.maskMaterials != null)
		{
			for (int i = 0; i < m_cSkeletonAnimation.maskMaterials.materialsMaskDisabled.Length; i++)
			{
				m_cSkeletonAnimation.maskMaterials.materialsMaskDisabled[i] = new Material(m_cSkeletonAnimation.maskMaterials.materialsMaskDisabled[i]);
			}
		}
		return true;
	}

	public override void Open()
	{
		if (m_UnitSpineSpriteInstant != null && m_UnitSpineSpriteInstant.m_Instant != null && !m_UnitSpineSpriteInstant.m_Instant.activeSelf)
		{
			m_UnitSpineSpriteInstant.m_Instant.SetActive(value: true);
		}
		SetColor(1f, 1f, 1f, 1f, bForce: true);
		SetOverrideMaterial();
	}

	public override void Close()
	{
		SetSortingLayerName(m_SortingLayerNameOrg);
		m_UnitSpineSpriteInstant?.Close();
		if (m_MeshRenderer != null)
		{
			m_MeshRenderer.sharedMaterial = m_Material;
			m_MeshRenderer.SetPropertyBlock(null);
		}
		if (m_MPBAdapter != null)
		{
			m_MPBAdapter.Init(bApplyCleanMPB: false);
		}
		ClearReplaceMat();
	}

	public override void Unload()
	{
		ClearReplaceMat();
		m_SPINE_SkeletonAnimationOrg = null;
		m_MeshRendererOrg = null;
		m_cSkeletonAnimation = null;
		m_SPINE_SkeletonAnimation = null;
		m_MeshRenderer = null;
		m_MPBAdapter = null;
		m_Material = null;
		m_DissolveMaterial = null;
		m_MainParticleSystemRenderer = null;
		m_Bone_Move = null;
		m_Bone_Head = null;
		m_ReplaceMaterial = null;
		m_ReplaceDissolveMaterial = null;
		m_MainParticleSystemRenderer = null;
		m_Bone_Move = null;
		m_Bone_Head = null;
		m_dicBone.Clear();
		if (m_ReplaceMatResource != null)
		{
			NKCAssetResourceManager.CloseResource(m_ReplaceMatResource);
		}
		m_ReplaceMatResource = null;
		NKCAssetResourceManager.CloseInstance(m_UnitSpineSpriteInstant);
		m_UnitSpineSpriteInstant = null;
	}

	public void SetColor(float fR, float fG, float fB, float fA = -1f, bool bForce = false)
	{
		if (!bForce)
		{
			bool flag = false;
			if (m_Color.r != fR)
			{
				m_Color.r = fR;
				flag = true;
			}
			if (m_Color.g != fG)
			{
				m_Color.g = fG;
				flag = true;
			}
			if (m_Color.b != fB)
			{
				m_Color.b = fB;
				flag = true;
			}
			if (fA != -1f && m_Color.a != fA)
			{
				m_Color.a = fA;
				flag = true;
			}
			if (!flag)
			{
				return;
			}
		}
		else
		{
			m_Color.r = fR;
			m_Color.g = fG;
			m_Color.b = fB;
			if (fA != -1f)
			{
				m_Color.a = fA;
			}
		}
		if (m_MPBAdapter != null)
		{
			m_MPBAdapter.SetColorNKC(m_Color);
		}
	}

	public void SetSortingLayerName(string sortingLayerName)
	{
		if (m_MeshRenderer != null)
		{
			m_MeshRenderer.sortingLayerName = sortingLayerName;
		}
	}

	public void SetDissolveOn(bool bOn)
	{
		if (m_DissolveMaterial == null || m_Material == null || m_cSkeletonAnimation == null)
		{
			return;
		}
		if (bOn)
		{
			if (m_cSkeletonAnimation.CustomMaterialOverride.ContainsKey(m_Material))
			{
				m_cSkeletonAnimation.CustomMaterialOverride.Remove(m_Material);
			}
			if (m_ReplaceDissolveMaterial == null)
			{
				m_cSkeletonAnimation.CustomMaterialOverride.Add(m_Material, m_DissolveMaterial);
			}
			else
			{
				m_cSkeletonAnimation.CustomMaterialOverride.Add(m_Material, m_ReplaceDissolveMaterial);
			}
			if (m_cSkeletonAnimation.maskMaterials != null)
			{
				for (int i = 0; i < m_cSkeletonAnimation.maskMaterials.materialsMaskDisabled.Length; i++)
				{
					m_cSkeletonAnimation.maskMaterials.materialsMaskDisabled[i].EnableKeyword("DISSOLVE_ON");
				}
			}
			return;
		}
		if (m_cSkeletonAnimation.CustomMaterialOverride.ContainsKey(m_Material))
		{
			m_cSkeletonAnimation.CustomMaterialOverride.Remove(m_Material);
		}
		if (m_ReplaceMaterial != null)
		{
			m_cSkeletonAnimation.CustomMaterialOverride.Add(m_Material, m_ReplaceMaterial);
		}
		if (m_cSkeletonAnimation.maskMaterials != null)
		{
			for (int j = 0; j < m_cSkeletonAnimation.maskMaterials.materialsMaskDisabled.Length; j++)
			{
				m_cSkeletonAnimation.maskMaterials.materialsMaskDisabled[j].DisableKeyword("DISSOLVE_ON");
			}
		}
	}

	public void SetDissolveBlend(float fBlend)
	{
		if (m_ReplaceDissolveMaterial != null)
		{
			m_ReplaceDissolveMaterial.SetFloat("_DissolveBlend", fBlend);
		}
		else if (m_DissolveMaterial != null)
		{
			m_DissolveMaterial.SetFloat("_DissolveBlend", fBlend);
		}
		if (m_cSkeletonAnimation.maskMaterials != null)
		{
			for (int i = 0; i < m_cSkeletonAnimation.maskMaterials.materialsMaskDisabled.Length; i++)
			{
				m_cSkeletonAnimation.maskMaterials.materialsMaskDisabled[i].SetFloat("_DissolveBlend", fBlend);
			}
		}
	}

	public void SetDissolveColor(Color color)
	{
		if (color.r == -1f)
		{
			color.r = m_DissolveColorOrg.r;
		}
		if (color.g == -1f)
		{
			color.g = m_DissolveColorOrg.g;
		}
		if (color.b == -1f)
		{
			color.b = m_DissolveColorOrg.b;
		}
		if (color.a == -1f)
		{
			color.a = m_DissolveColorOrg.a;
		}
		if (m_ReplaceDissolveMaterial != null)
		{
			m_ReplaceDissolveMaterial.SetColor("_DissolveGlowColor", color);
		}
		else if (m_DissolveMaterial != null)
		{
			m_DissolveMaterial.SetColor("_DissolveGlowColor", color);
		}
	}

	public void CalcBoneMoveWorldPos()
	{
		if (m_Bone_Move != null)
		{
			m_Bone_MovePos = m_SPINE_SkeletonAnimation.transform.TransformPoint(m_Bone_Move.WorldX, m_Bone_Move.WorldY, 0f);
		}
	}

	public void CalcBoneHeadWorldPos()
	{
		if (m_Bone_Head != null)
		{
			m_Bone_HeadPos = m_SPINE_SkeletonAnimation.transform.TransformPoint(m_Bone_Head.WorldX, m_Bone_Head.WorldY, 0f);
		}
	}

	public Bone GetBone(string boneName)
	{
		if (m_cSkeletonAnimation == null)
		{
			return null;
		}
		Bone bone = null;
		if (!m_dicBone.ContainsKey(boneName))
		{
			bone = m_cSkeletonAnimation.skeleton.FindBone(boneName);
			if (bone == null)
			{
				return null;
			}
			m_dicBone.Add(boneName, bone);
		}
		return m_dicBone[boneName];
	}

	public bool GetBoneWorldPos(string boneName, ref Vector3 worldPos)
	{
		Bone bone = GetBone(boneName);
		if (bone == null)
		{
			return false;
		}
		worldPos = m_SPINE_SkeletonAnimation.transform.TransformPoint(bone.WorldX, bone.WorldY, 0f);
		return true;
	}
}
