using NKM;
using UnityEngine;
using UnityEngine.Rendering;

namespace NKC;

public class NKCASUnitSprite : NKMObjectPoolData
{
	public NKCAssetInstanceData m_UnitSpriteInstant;

	public NKCASMaterial m_MainMaterial;

	public SortingGroup m_SortingGroup;

	public SpriteRenderer[] m_MainSpriteRenderer;

	public ParticleSystemRenderer[] m_MainParticleSystemRenderer;

	public SpriteRenderer[] m_OrgSpriteRenderer;

	public Color[] m_OrgColor;

	public float m_fColorR;

	public float m_fColorG;

	public float m_fColorB;

	public float m_fColorA;

	private string m_OrgSortingLayerName;

	public NKCASUnitSprite(string bundleName, string name, bool bAsync = false)
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitSprite;
		m_ObjectPoolBundleName = bundleName;
		m_ObjectPoolName = name;
		m_bUnloadable = true;
		Load(bAsync);
	}

	public override void Load(bool bAsync)
	{
		m_UnitSpriteInstant = NKCAssetResourceManager.OpenInstance<GameObject>(m_ObjectPoolBundleName, m_ObjectPoolName, bAsync);
		m_MainMaterial = (NKCASMaterial)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASMaterial, "AB_MATERIAL", "MAT_NKC_MAIN", bAsync);
	}

	public override bool LoadComplete()
	{
		if (m_UnitSpriteInstant == null || m_UnitSpriteInstant.m_Instant == null || m_MainMaterial == null || m_MainMaterial.m_Material == null)
		{
			return false;
		}
		GameObject asset = m_UnitSpriteInstant.m_InstantOrg.GetAsset<GameObject>();
		m_OrgSpriteRenderer = asset.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		m_MainSpriteRenderer = m_UnitSpriteInstant.m_Instant.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		m_OrgColor = new Color[m_OrgSpriteRenderer.Length];
		for (int i = 0; i < m_OrgSpriteRenderer.Length; i++)
		{
			m_OrgColor[i] = m_OrgSpriteRenderer[i].color;
		}
		m_MainParticleSystemRenderer = m_UnitSpriteInstant.m_Instant.GetComponentsInChildren<ParticleSystemRenderer>(includeInactive: true);
		NKCUtil.SetParticleSystemRendererSortOrder(m_MainParticleSystemRenderer);
		m_SortingGroup = m_UnitSpriteInstant.m_Instant.GetComponentInChildren<SortingGroup>();
		if (m_SortingGroup != null)
		{
			m_OrgSortingLayerName = m_SortingGroup.sortingLayerName;
		}
		return true;
	}

	public override void Open()
	{
		if (!m_UnitSpriteInstant.m_Instant.activeSelf)
		{
			m_UnitSpriteInstant.m_Instant.SetActive(value: true);
		}
	}

	public override void Close()
	{
		if (m_SortingGroup != null)
		{
			m_SortingGroup.sortingLayerName = m_OrgSortingLayerName;
		}
		m_UnitSpriteInstant.Close();
	}

	public void ChangeSortingLayerName(string layerName)
	{
		if (m_SortingGroup != null)
		{
			m_SortingGroup.sortingLayerName = layerName;
		}
	}

	public override void Unload()
	{
		m_MainParticleSystemRenderer = null;
		m_MainSpriteRenderer = null;
		m_MainMaterial.Unload();
		m_MainMaterial = null;
		NKCAssetResourceManager.CloseInstance(m_UnitSpriteInstant);
		m_UnitSpriteInstant = null;
		m_SortingGroup = null;
		m_MainSpriteRenderer = null;
		m_MainParticleSystemRenderer = null;
		m_OrgSpriteRenderer = null;
	}

	public void SetColor(float fR, float fG, float fB, float fA = -1f)
	{
		bool flag = false;
		if (m_fColorR != fR)
		{
			m_fColorR = fR;
			flag = true;
		}
		if (m_fColorG != fG)
		{
			m_fColorG = fG;
			flag = true;
		}
		if (m_fColorB != fB)
		{
			m_fColorB = fB;
			flag = true;
		}
		if (m_fColorA != fA)
		{
			m_fColorA = fA;
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		for (int i = 0; i < m_MainSpriteRenderer.Length; i++)
		{
			Color color = m_OrgColor[i];
			color.r *= fR;
			color.g *= fG;
			color.b *= fB;
			if (fA != -1f)
			{
				color.a *= fA;
			}
			m_MainSpriteRenderer[i].color = color;
		}
	}

	public void SetMaterial(bool bOrg = false)
	{
		for (int i = 0; i < m_MainSpriteRenderer.Length; i++)
		{
			if (bOrg)
			{
				m_MainSpriteRenderer[i].sharedMaterial = m_OrgSpriteRenderer[i].sharedMaterial;
			}
			else
			{
				m_MainSpriteRenderer[i].sharedMaterial = m_MainMaterial.m_Material;
			}
		}
	}
}
