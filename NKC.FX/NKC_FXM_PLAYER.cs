using System;
using Beebyte.Obfuscator;
using UnityEngine;

namespace NKC.FX;

[Serializable]
[ExecuteAlways]
public class NKC_FXM_PLAYER : MonoBehaviour
{
	private enum EvaluaterState
	{
		Init,
		Restart,
		Loop,
		Reset
	}

	public float Duration = 1f;

	public float TimeScale = 1f;

	private float ExternalTimeScale = 1f;

	public bool AutoStart;

	public bool AutoDisable;

	public bool Loop;

	public bool SingleGO;

	public UpdateMode TimeMode;

	public RandomState RandomMode;

	public Space SimulationSpace = Space.Self;

	private float lastUpdateTime;

	private float waitTime;

	private float playbackTime;

	private bool onUpdate;

	private bool isPlaying;

	private bool isStopped;

	private bool isLocal;

	private bool m_bUseGameUpdate;

	private GameObject world;

	private Transform CaptureParent;

	private Vector3 CaptureLocalPos;

	private Vector3 CaptureLocalRot;

	private bool wait;

	private bool init;

	private NKC_FXM_EVALUATER[] evaluater;

	private readonly string methodRestart = "Restart";

	public float PlaybackTime
	{
		get
		{
			return playbackTime;
		}
		set
		{
			playbackTime = value;
		}
	}

	public bool OnUpdate
	{
		get
		{
			return onUpdate;
		}
		set
		{
			onUpdate = value;
		}
	}

	public bool IsPlaying => isPlaying;

	public bool IsStopped => isStopped;

	public bool IsLocal => isLocal;

	public void SetExternalTimeScale(float _TimeScale)
	{
		ExternalTimeScale = _TimeScale;
	}

	public void SetUseGameUpdate(bool bUseGameUpdate)
	{
		m_bUseGameUpdate = bUseGameUpdate;
	}

	private void OnDestroy()
	{
		if (CaptureParent != null)
		{
			CaptureParent = null;
		}
		if (evaluater != null)
		{
			evaluater = null;
		}
		if (world != null)
		{
			world = null;
		}
	}

	private void Awake()
	{
		Init();
	}

	private void OnEnable()
	{
		if (AutoStart && Application.isPlaying)
		{
			Invoke(methodRestart, 0f);
		}
	}

	private void OnDisable()
	{
		if (init)
		{
			Stop();
			CancelInvoke(methodRestart);
		}
	}

	public void Init()
	{
		isPlaying = false;
		isStopped = true;
		if (Application.isPlaying && SimulationSpace == Space.World)
		{
			CaptureTransform();
			world = GameObject.Find("NKM_GLOBAL_EFFECT");
		}
		SetEvaluater(SingleGO);
		SetEvaluaterState(EvaluaterState.Init);
		init = true;
	}

	public void SetEvaluater(bool _singleGO)
	{
		if (_singleGO)
		{
			evaluater = GetComponents<NKC_FXM_EVALUATER>();
		}
		else
		{
			evaluater = GetComponentsInChildren<NKC_FXM_EVALUATER>(includeInactive: true);
		}
	}

	[SkipRename]
	public void Restart()
	{
		SetEvaluaterState(EvaluaterState.Restart);
		playbackTime = 0f;
		lastUpdateTime = 0f;
		waitTime = 0f;
		Play();
	}

	[SkipRename]
	[Obsolete]
	public void Restart(bool _b)
	{
		if (_b && !onUpdate)
		{
			Restart();
		}
	}

	public void RestartOnNotPlayed()
	{
		if (!onUpdate)
		{
			Restart();
		}
	}

	public void RestartAndActive()
	{
		base.gameObject.SetActive(value: true);
		Restart();
	}

	[SkipRename]
	public void Play()
	{
		isPlaying = true;
		isStopped = false;
		onUpdate = true;
		wait = false;
	}

	[SkipRename]
	public void Pause()
	{
		onUpdate = false;
	}

	[SkipRename]
	public void Stop()
	{
		if (!isStopped)
		{
			isStopped = true;
			isPlaying = false;
			onUpdate = false;
			playbackTime = 0f;
			lastUpdateTime = 0f;
			waitTime = 0f;
			SetEvaluaterState(EvaluaterState.Reset);
			if (Application.isPlaying && AutoDisable)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}

	[SkipRename]
	[Obsolete]
	public void Stop(bool _b)
	{
		if (_b && onUpdate)
		{
			Stop();
		}
	}

	private void Update()
	{
		if (TimeMode == UpdateMode.Normal)
		{
			if (Time.deltaTime > Time.maximumParticleDeltaTime)
			{
				Update(Time.maximumParticleDeltaTime);
			}
			else
			{
				Update(Time.deltaTime);
			}
		}
		else
		{
			Update(Time.unscaledDeltaTime);
		}
	}

	public void Update(float _dt)
	{
		if (!m_bUseGameUpdate)
		{
			UpdateInternal(_dt);
		}
	}

	public void UpdateInternal(float _deltaTime)
	{
		float num = (m_bUseGameUpdate ? (_deltaTime * TimeScale) : (_deltaTime * TimeScale * ExternalTimeScale));
		if (!onUpdate || !init)
		{
			return;
		}
		if (playbackTime == 0f && SimulationSpace == Space.World && Application.isPlaying)
		{
			if (!isLocal)
			{
				ResetTransform();
			}
			if (isLocal)
			{
				UpdateTransform();
			}
		}
		if (!Loop)
		{
			playbackTime = Mathf.Clamp(playbackTime + num, 0f, Duration);
			if (0f < playbackTime && playbackTime < Duration)
			{
				UpdateEvaluater(num);
			}
			else if (Duration <= playbackTime)
			{
				UpdateEvaluater(num);
				OnComplete();
			}
		}
		else
		{
			if (Duration < playbackTime + num)
			{
				SetEvaluaterState(EvaluaterState.Loop);
			}
			playbackTime = Mathf.Repeat(playbackTime + num, Duration);
			if (0f < playbackTime && playbackTime < Duration)
			{
				UpdateEvaluater(num);
			}
		}
	}

	private void LateUpdate()
	{
		if (!isLocal && SimulationSpace == Space.World && Application.isPlaying && isStopped && !onUpdate)
		{
			ResetTransform();
		}
		if (isPlaying)
		{
			CalculateWaitState();
		}
	}

	private void CalculateWaitState()
	{
		if (lastUpdateTime < playbackTime)
		{
			wait = false;
			waitTime = 0f;
			lastUpdateTime = playbackTime;
		}
		else if (!wait && lastUpdateTime == playbackTime)
		{
			waitTime += Time.deltaTime;
		}
		if (!wait && Time.deltaTime < waitTime)
		{
			UpdateEvaluater(0f);
			wait = true;
		}
	}

	private void OnComplete()
	{
		Stop();
	}

	private void UpdateEvaluater(float _deltaTime)
	{
		if (evaluater.Length == 0)
		{
			return;
		}
		for (int i = 0; i < evaluater.Length; i++)
		{
			if (evaluater[i] != null && !evaluater[i].IsCompleted)
			{
				if (evaluater[i].StartTime <= playbackTime)
				{
					evaluater[i].UpdateInternal(playbackTime, _deltaTime);
				}
				else if (0f < evaluater[i].GetPlaybackTime())
				{
					evaluater[i].UpdateInternal(0f, _deltaTime);
				}
			}
		}
	}

	private void UpdateTransform()
	{
		if (base.gameObject.activeInHierarchy)
		{
			if (world != null)
			{
				base.transform.parent = world.transform;
				isLocal = false;
			}
			else
			{
				base.transform.parent = null;
				isLocal = false;
			}
		}
	}

	private void CaptureTransform()
	{
		if (base.transform.parent != null)
		{
			CaptureParent = base.transform.parent;
			CaptureLocalPos = base.transform.localPosition;
			CaptureLocalRot = base.transform.localEulerAngles;
		}
	}

	private void ResetTransform()
	{
		if (CaptureParent != null)
		{
			base.transform.parent = CaptureParent;
			base.transform.localPosition = CaptureLocalPos;
			base.transform.localEulerAngles = CaptureLocalRot;
			isLocal = true;
		}
		else
		{
			Debug.LogWarning("VFX Player Destroyed with self on World Space :: " + base.gameObject.name);
			UnityEngine.Object.Destroy(base.gameObject, Time.maximumDeltaTime * 2f);
		}
	}

	private void SetEvaluaterState(EvaluaterState _es)
	{
		if (evaluater == null || evaluater.Length == 0)
		{
			return;
		}
		for (int i = 0; i < evaluater.Length; i++)
		{
			if (evaluater[i] != null)
			{
				ExecuteEvaluaterState(evaluater[i], _es);
			}
		}
	}

	private void ExecuteEvaluaterState(NKC_FXM_EVALUATER _evaluater, EvaluaterState _evaluaterState)
	{
		switch (_evaluaterState)
		{
		case EvaluaterState.Init:
			_evaluater.Init();
			break;
		case EvaluaterState.Restart:
			if (RandomMode == RandomState.EditorAndRuntime)
			{
				_evaluater.SetRandomValue(_resimulate: true);
			}
			else if (Application.isPlaying)
			{
				_evaluater.SetRandomValue(_resimulate: true);
			}
			else
			{
				_evaluater.SetRandomValue(_resimulate: false);
			}
			_evaluater.ResetEvaluater();
			break;
		case EvaluaterState.Loop:
			_evaluater.IsStarted = false;
			_evaluater.IsCompleted = false;
			if (RandomMode == RandomState.EditorAndRuntime)
			{
				if (_evaluater.Resimulate)
				{
					_evaluater.SetRandomValue(_resimulate: true);
				}
			}
			else if (Application.isPlaying)
			{
				if (_evaluater.Resimulate)
				{
					_evaluater.SetRandomValue(_resimulate: true);
				}
			}
			else
			{
				_evaluater.SetRandomValue(_resimulate: false);
			}
			break;
		case EvaluaterState.Reset:
			_evaluater.ResetEvaluater();
			break;
		}
	}
}
