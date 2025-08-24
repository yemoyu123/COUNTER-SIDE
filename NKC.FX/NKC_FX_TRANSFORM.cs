using System.Collections.Generic;
using UnityEngine;

namespace NKC.FX;

[ExecuteAlways]
public class NKC_FX_TRANSFORM : MonoBehaviour
{
	public Vector3 m_Position = new Vector3(0f, 0f, 0f);

	public Vector3 m_Rotation = new Vector3(0f, 0f, 0f);

	public Vector3 m_Scale = new Vector3(1f, 1f, 1f);

	[Space]
	public float m_PositionMultiplier = 1f;

	public float m_RotationMultiplier = 1f;

	public float m_ScaleMultiplier = 1f;

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
		SetTransform();
	}

	private void OnValidate()
	{
		SetTransform();
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

	private void Update()
	{
		if (base.enabled && SyncUpdate)
		{
			SetTransform();
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
		if (m_Transform.Count > 0)
		{
			init = true;
		}
	}

	private void SetTransform()
	{
		if (init && m_Transform != null && m_Transform.Count > 0)
		{
			for (int i = 0; i < m_Transform.Count; i++)
			{
				m_Transform[i].localPosition = m_Position * m_PositionMultiplier;
				m_Transform[i].localEulerAngles = m_Rotation * m_RotationMultiplier;
				m_Transform[i].localScale = m_Scale * m_ScaleMultiplier;
			}
		}
	}
}
