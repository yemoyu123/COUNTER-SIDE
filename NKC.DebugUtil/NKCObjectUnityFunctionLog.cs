using UnityEngine;

namespace NKC.DebugUtil;

public class NKCObjectUnityFunctionLog : MonoBehaviour
{
	public bool m_bAwake;

	public bool m_bStart;

	public bool m_bUpdate;

	public bool m_bOnEnable;

	public bool m_bOnDisable;

	private void Awake()
	{
		if (m_bAwake)
		{
			Debug.Log(base.gameObject.name + " Awake");
		}
	}

	private void Start()
	{
		if (m_bStart)
		{
			Debug.Log(base.gameObject.name + " Start");
		}
	}

	private void Update()
	{
		if (m_bUpdate)
		{
			Debug.Log(base.gameObject.name + " Update");
		}
	}

	private void OnEnable()
	{
		if (m_bOnEnable)
		{
			Debug.Log(base.gameObject.name + " OnEnable");
		}
	}

	private void OnDisable()
	{
		if (m_bOnDisable)
		{
			Debug.Log(base.gameObject.name + " OnDisable");
		}
	}
}
