using NKM;
using UnityEngine;

namespace NKC;

public class NKCASLensFlare : NKMObjectPoolData
{
	public NKCAssetInstanceData m_LensFlareInstant;

	public LensFlare m_LensFlare;

	public float m_LensFlareBrightOrg;

	public float m_LensFlareBrightNow;

	public float GetLensFlareBrightOrg()
	{
		return m_LensFlareBrightOrg;
	}

	public NKCASLensFlare(string bundleName, string name, bool bAsync = false)
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKCASLensFlare;
		m_ObjectPoolBundleName = bundleName;
		m_ObjectPoolName = name;
		m_bUnloadable = true;
		Load(bAsync);
	}

	public override void Load(bool bAsync)
	{
		m_LensFlareInstant = NKCAssetResourceManager.OpenInstance<GameObject>(m_ObjectPoolBundleName, m_ObjectPoolName, bAsync);
	}

	public override bool LoadComplete()
	{
		if (m_LensFlareInstant == null || m_LensFlareInstant.m_Instant == null)
		{
			return false;
		}
		m_LensFlareInstant.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKM_LENS_FLARE_LIST()
			.transform, worldPositionStays: false);
			m_LensFlare = m_LensFlareInstant.m_Instant.GetComponent<LensFlare>();
			m_LensFlareBrightOrg = m_LensFlare.brightness;
			return true;
		}

		public override void Open()
		{
			if (!m_LensFlareInstant.m_Instant.activeSelf)
			{
				m_LensFlareInstant.m_Instant.SetActive(value: true);
			}
		}

		public override void Close()
		{
			if (m_LensFlareInstant.m_Instant.activeSelf)
			{
				m_LensFlareInstant.m_Instant.SetActive(value: false);
			}
		}

		public override void Unload()
		{
			NKCAssetResourceManager.CloseInstance(m_LensFlareInstant);
			m_LensFlareInstant = null;
			m_LensFlare = null;
		}

		public void SetPos(float fX = -1f, float fY = -1f, float fZ = -1f)
		{
			Vector3 position = m_LensFlareInstant.m_Instant.transform.position;
			if (fX != -1f)
			{
				position.x = fX;
			}
			if (fY != -1f)
			{
				position.y = fY;
			}
			if (fZ != -1f)
			{
				position.z = fZ;
			}
			m_LensFlareInstant.m_Instant.transform.position = position;
		}

		public void SetLensFlareBright(float fBright)
		{
			m_LensFlareBrightNow = fBright;
			m_LensFlare.brightness = m_LensFlareBrightNow;
		}
	}
