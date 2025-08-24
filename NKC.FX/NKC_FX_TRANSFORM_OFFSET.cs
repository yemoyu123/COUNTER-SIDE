using System.Collections.Generic;
using UnityEngine;

namespace NKC.FX;

[ExecuteAlways]
public class NKC_FX_TRANSFORM_OFFSET : MonoBehaviour
{
	public Vector3 m_OffsetPosition = new Vector3(0f, 0f, 0f);

	public Vector3 m_OffsetRotation = new Vector3(0f, 0f, 0f);

	[Space]
	public bool SyncUpdate;

	[Space]
	public string BindingID = "NONAME";

	public List<Transform> m_Transform = new List<Transform>();

	private bool init;

	private void OnDestroy()
	{
		if (m_Transform != null)
		{
			m_Transform.Clear();
			m_Transform = null;
		}
	}

	private void OnEnable()
	{
		Init();
		GetTranform();
		SetOffset();
	}

	private void OnValidate()
	{
		SetOffset();
	}

	private void Update()
	{
		if (base.enabled && SyncUpdate)
		{
			SetOffset();
		}
	}

	private void Init()
	{
		if (m_Transform.Count > 0)
		{
			init = true;
		}
		else
		{
			init = false;
		}
	}

	private void GetTranform()
	{
		if (init)
		{
			return;
		}
		NKC_FX_BINDING[] componentsInChildren = base.transform.GetComponentsInChildren<NKC_FX_BINDING>();
		m_Transform.Clear();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].ID == BindingID)
			{
				m_Transform.Add(componentsInChildren[i].transform);
			}
		}
	}

	private void SetOffset()
	{
		if (init && m_Transform != null && m_Transform.Count > 0)
		{
			for (int i = 0; i < m_Transform.Count; i++)
			{
				m_Transform[i].localPosition = m_OffsetPosition * i;
				m_Transform[i].localEulerAngles = m_OffsetRotation * i;
			}
		}
	}
}
