using UnityEngine;
using UnityEngine.Events;

namespace NKC.FX;

public class NKC_FX_EVENT : MonoBehaviour
{
	public bool UseOnEnable;

	public bool UseOnDisable;

	public bool AutoStart;

	public bool AutoDisable;

	public bool WaitForStart;

	public UnityEvent m_FX_EVENT;

	public UnityEvent m_OnEnable;

	public UnityEvent m_OnDisable;

	[Range(0f, 1f)]
	public float ProbFxEvent = 1f;

	[Range(0f, 1f)]
	public float ProbOnEnable = 1f;

	[Range(0f, 1f)]
	public float ProbOnDisable = 1f;

	private bool executed;

	private bool isStarted;

	private void OnDestroy()
	{
		if (m_FX_EVENT != null)
		{
			m_FX_EVENT = null;
		}
		if (m_OnEnable != null)
		{
			m_OnEnable = null;
		}
		if (m_OnDisable != null)
		{
			m_OnDisable = null;
		}
	}

	private void Awake()
	{
		executed = false;
		isStarted = false;
	}

	private void OnEnable()
	{
		if (!UseOnEnable)
		{
			return;
		}
		if (WaitForStart)
		{
			if (isStarted)
			{
				HandleEvent(m_OnEnable, ProbOnEnable);
			}
		}
		else
		{
			HandleEvent(m_OnEnable, ProbOnEnable);
		}
	}

	private void Start()
	{
		isStarted = true;
	}

	private void Update()
	{
		if (!executed && AutoStart)
		{
			Execute();
		}
	}

	private void LateUpdate()
	{
		if (executed && AutoDisable)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void OnDisable()
	{
		executed = false;
		if (!UseOnDisable)
		{
			return;
		}
		if (WaitForStart)
		{
			if (isStarted)
			{
				HandleEvent(m_OnDisable, ProbOnDisable);
			}
		}
		else
		{
			HandleEvent(m_OnDisable, ProbOnDisable);
		}
	}

	public void Execute()
	{
		executed = HandleEvent(m_FX_EVENT, ProbFxEvent);
	}

	private bool HandleEvent(UnityEvent _evt, float _prob)
	{
		bool result = false;
		float value = Random.value;
		if (_evt.GetPersistentEventCount() > 0)
		{
			if (_prob > value)
			{
				_evt.Invoke();
				result = true;
			}
			else
			{
				result = false;
			}
		}
		return result;
	}
}
