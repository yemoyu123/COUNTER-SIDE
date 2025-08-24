using System.Collections.Generic;
using UnityEngine;

namespace NKC;

public class NKCAssetInstanceData
{
	public string m_BundleName;

	public string m_AssetName;

	public NKCAssetResourceData m_InstantOrg;

	public GameObject m_Instant;

	public bool m_bLoad;

	public Transform[] m_Transforms;

	public Dictionary<string, Transform> m_dicTransform = new Dictionary<string, Transform>();

	public float m_fTime;

	public bool m_bLoadTypeAsync;

	public NKCAssetInstanceData()
	{
		Init();
	}

	public bool GetLoadFail()
	{
		if (m_InstantOrg != null)
		{
			if (m_InstantOrg.IsDone() && m_InstantOrg.GetAsset<Object>() == null)
			{
				return true;
			}
			return m_InstantOrg.GetLoadFail();
		}
		return false;
	}

	public void Init()
	{
		m_BundleName = "";
		m_AssetName = "";
		m_InstantOrg = null;
		m_Instant = null;
		m_bLoad = false;
		m_Transforms = null;
		m_dicTransform.Clear();
		m_fTime = 0f;
		m_bLoadTypeAsync = false;
	}

	public void Unload()
	{
		if (m_Instant != null)
		{
			Object.Destroy(m_Instant);
			NKCAssetResourceManager.CloseResource(m_InstantOrg);
		}
		Init();
	}

	public void BuildTranformDic()
	{
		m_Transforms = m_Instant.GetComponentsInChildren<Transform>();
		m_dicTransform.Clear();
		for (int i = 0; i < m_Transforms.Length; i++)
		{
			if (!m_dicTransform.ContainsKey(m_Transforms[i].name))
			{
				m_dicTransform.Add(m_Transforms[i].name, m_Transforms[i]);
			}
		}
	}

	public Transform GetTransform(string boneName)
	{
		if (m_dicTransform.Count == 0)
		{
			BuildTranformDic();
		}
		for (int i = 0; i < m_Transforms.Length; i++)
		{
			if (m_dicTransform.ContainsKey(boneName))
			{
				return m_dicTransform[boneName];
			}
		}
		return null;
	}

	public void Close()
	{
		if (!(m_Instant == null))
		{
			if (NKCScenManager.GetScenManager().Get_NKM_NEW_INSTANT() != null)
			{
				m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_NKM_NEW_INSTANT().transform);
			}
			ResetTransformToOrg();
			if (m_Instant.activeSelf)
			{
				m_Instant.SetActive(value: false);
			}
		}
	}

	public void ResetTransformToOrg()
	{
		if (m_InstantOrg != null && !(m_Instant == null))
		{
			GameObject asset = m_InstantOrg.GetAsset<GameObject>();
			if (!(asset == null))
			{
				m_Instant.transform.localPosition = asset.transform.localPosition;
				m_Instant.transform.localRotation = asset.transform.localRotation;
				m_Instant.transform.localScale = asset.transform.localScale;
			}
		}
	}
}
