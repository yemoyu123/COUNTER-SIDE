using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NKC.FX;

[Serializable]
[ExecuteAlways]
public class NKC_FX_DELAY_EXECUTER : MonoBehaviour
{
	[Serializable]
	public class SequenceInfo
	{
		public bool enabled;

		public ExecuteType m_Type;

		public float m_StartTime;

		public GameObject m_Obj;
	}

	[FormerlySerializedAs("PlayTime")]
	public float Duration = 1f;

	public float TimeScale = 1f;

	public float GameTimeScale = 1.1f;

	public UpdateMode TimeMode;

	public bool AutoStart;

	public bool AutoDisable;

	public bool Loop;

	public SequenceInfo[] Sequence;

	private bool onUpdate;

	private bool isPlaying;

	private bool isStopped;

	private bool m_bUseGameUpdate;

	private bool init;

	private bool appState;

	private List<SequenceInfo> aSeq = new List<SequenceInfo>();

	private List<SequenceInfo> bSeq = new List<SequenceInfo>();

	private float playbackTime;

	private float deltaTime;

	private float maximumParticleDeltaTime;

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

	public float PlaybackTime => playbackTime;

	public void SetUseGameUpdate(bool bUseGameUpdate)
	{
		m_bUseGameUpdate = bUseGameUpdate;
	}

	private void OnDestroy()
	{
		if (Sequence != null)
		{
			Sequence = null;
		}
		if (aSeq != null)
		{
			aSeq.Clear();
			aSeq = null;
		}
		if (bSeq != null)
		{
			bSeq.Clear();
			bSeq = null;
		}
	}

	private void OnValidate()
	{
		init = false;
		Init();
	}

	private void Awake()
	{
		Init();
	}

	private void OnEnable()
	{
		if (init && Application.isPlaying && AutoStart)
		{
			ResetSequence();
			Restart();
		}
	}

	private void OnDisable()
	{
		Stop();
	}

	public void Init()
	{
		isPlaying = false;
		isStopped = true;
		appState = Application.isPlaying;
		if (init || Sequence == null || Sequence.Length == 0)
		{
			return;
		}
		aSeq.Clear();
		bSeq.Clear();
		aSeq.Capacity = Sequence.Length;
		bSeq.Capacity = Sequence.Length;
		for (int i = 0; i < Sequence.Length; i++)
		{
			if (Sequence[i].enabled)
			{
				aSeq.Add(Sequence[i]);
				if (Sequence[i].m_Obj != null)
				{
					init = true;
					continue;
				}
				init = false;
				Sequence[i].enabled = false;
				Sequence[i].m_Obj = null;
				Debug.LogError("Null Object Sequence (" + i + ") -> " + base.transform.root.name + " -> " + base.gameObject.name, this);
			}
		}
		SortSequence(_t: false);
		for (int j = 0; j < aSeq.Count; j++)
		{
			bSeq.Add(aSeq[j]);
		}
		aSeq.TrimExcess();
		bSeq.TrimExcess();
	}

	public void SortSequence(bool _t)
	{
		aSeq.Sort(Comparer);
		if (_t)
		{
			Sequence = aSeq.ToArray();
		}
	}

	private int Comparer(SequenceInfo A, SequenceInfo B)
	{
		if (A.m_StartTime > B.m_StartTime)
		{
			return 1;
		}
		if (A.m_StartTime < B.m_StartTime)
		{
			return -1;
		}
		return 0;
	}

	public void Restart()
	{
		CopyToList(bSeq, aSeq);
		playbackTime = 0f;
		Play();
	}

	[Obsolete]
	public void Restart(bool _b)
	{
		if (_b && !onUpdate)
		{
			Restart();
		}
	}

	public void Play()
	{
		isPlaying = true;
		isStopped = false;
		onUpdate = true;
	}

	public void Pause()
	{
		onUpdate = false;
	}

	public void Stop()
	{
		if (!isStopped)
		{
			isStopped = true;
			isPlaying = false;
			onUpdate = false;
			playbackTime = 0f;
			CopyToList(bSeq, aSeq);
			ResetSequence();
		}
	}

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
			deltaTime = Time.deltaTime;
			maximumParticleDeltaTime = Time.maximumParticleDeltaTime;
			if (deltaTime > maximumParticleDeltaTime)
			{
				Update(maximumParticleDeltaTime);
			}
			else
			{
				Update(deltaTime);
			}
		}
		else
		{
			deltaTime = Time.unscaledDeltaTime;
			Update(deltaTime);
		}
	}

	public void Update(float delta)
	{
		if (!m_bUseGameUpdate)
		{
			UpdateInternal(delta);
		}
	}

	public void UpdateInternal(float _deltatime)
	{
		float num = (m_bUseGameUpdate ? (_deltatime * TimeScale) : (_deltatime * TimeScale * GameTimeScale));
		if (onUpdate && init)
		{
			if (Duration < playbackTime)
			{
				Complete();
				return;
			}
			playbackTime += num;
			Execute();
		}
	}

	private void Complete()
	{
		if (Loop)
		{
			CopyToList(bSeq, aSeq);
			Mathf.Clamp(playbackTime -= Duration, 0f, Duration);
			Execute();
			return;
		}
		playbackTime = Duration;
		Execute();
		Stop();
		if (Application.isPlaying && AutoDisable)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Execute()
	{
		if (!base.enabled)
		{
			return;
		}
		appState = Application.isPlaying;
		if (aSeq.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < aSeq.Count && aSeq[i].enabled && aSeq[i].m_StartTime <= playbackTime; i++)
		{
			switch (aSeq[i].m_Type)
			{
			case ExecuteType.Enable:
				if (appState)
				{
					aSeq[i].m_Obj.SetActive(value: true);
				}
				break;
			case ExecuteType.Disable:
				if (appState)
				{
					aSeq[i].m_Obj.SetActive(value: false);
				}
				break;
			case ExecuteType.Restart:
				if ((bool)aSeq[i].m_Obj.GetComponent<NKC_FXM_PLAYER>())
				{
					aSeq[i].m_Obj.GetComponent<NKC_FXM_PLAYER>().Restart();
				}
				else if ((bool)aSeq[i].m_Obj.GetComponent<NKC_FX_DELAY_EXECUTER>())
				{
					aSeq[i].m_Obj.GetComponent<NKC_FX_DELAY_EXECUTER>().Restart();
				}
				else
				{
					Debug.LogWarning("Can not found Receiver Object -> " + aSeq[i].m_Obj.name, base.gameObject);
				}
				break;
			case ExecuteType.Play:
				if ((bool)aSeq[i].m_Obj.GetComponent<NKC_FXM_PLAYER>())
				{
					aSeq[i].m_Obj.GetComponent<NKC_FXM_PLAYER>().Play();
				}
				else if ((bool)aSeq[i].m_Obj.GetComponent<NKC_FX_DELAY_EXECUTER>())
				{
					aSeq[i].m_Obj.GetComponent<NKC_FX_DELAY_EXECUTER>().Play();
				}
				else
				{
					Debug.LogWarning("Can not found Receiver Object -> " + aSeq[i].m_Obj.name, base.gameObject);
				}
				break;
			case ExecuteType.Stop:
				if ((bool)aSeq[i].m_Obj.GetComponent<NKC_FXM_PLAYER>())
				{
					aSeq[i].m_Obj.GetComponent<NKC_FXM_PLAYER>().Stop();
				}
				else if ((bool)aSeq[i].m_Obj.GetComponent<NKC_FX_DELAY_EXECUTER>())
				{
					aSeq[i].m_Obj.GetComponent<NKC_FX_DELAY_EXECUTER>().Stop();
				}
				else
				{
					Debug.LogWarning("Can not found Receiver Object -> " + aSeq[i].m_Obj.name, base.gameObject);
				}
				break;
			case ExecuteType.Setparent:
				if (appState)
				{
					if ((bool)aSeq[i].m_Obj.GetComponent<NKC_FX_ACTIVE_ON_WORLD_SPACE>())
					{
						aSeq[i].m_Obj.GetComponent<NKC_FX_ACTIVE_ON_WORLD_SPACE>().SetParent();
					}
					else
					{
						Debug.LogWarning("Can not found Receiver Object -> " + aSeq[i].m_Obj.name, base.gameObject);
					}
				}
				break;
			case ExecuteType.Reparent:
				if (appState)
				{
					if ((bool)aSeq[i].m_Obj.GetComponent<NKC_FX_ACTIVE_ON_WORLD_SPACE>())
					{
						aSeq[i].m_Obj.GetComponent<NKC_FX_ACTIVE_ON_WORLD_SPACE>().ReParent();
					}
					else
					{
						Debug.LogWarning("Can not found Receiver Object -> " + aSeq[i].m_Obj.name, base.gameObject);
					}
				}
				break;
			case ExecuteType.Execute:
				if ((bool)aSeq[i].m_Obj.GetComponent<NKC_FX_EVENT>())
				{
					aSeq[i].m_Obj.GetComponent<NKC_FX_EVENT>().Execute();
				}
				else
				{
					Debug.LogWarning("Can not found Receiver Object -> " + aSeq[i].m_Obj.name, base.gameObject);
				}
				break;
			}
			aSeq.RemoveAt(i);
		}
	}

	private void ResetSequence()
	{
		if (!init || !Application.isPlaying)
		{
			return;
		}
		for (int i = 0; i < aSeq.Count; i++)
		{
			SequenceInfo sequenceInfo = aSeq[i];
			if (sequenceInfo.enabled && sequenceInfo.m_Obj != null && sequenceInfo.m_Type == ExecuteType.Enable && sequenceInfo.m_Obj.activeSelf)
			{
				sequenceInfo.m_Obj.SetActive(value: false);
			}
		}
	}

	private void CopyToList(List<SequenceInfo> _source, List<SequenceInfo> _destination)
	{
		_destination.Clear();
		for (int i = 0; i < _source.Count; i++)
		{
			_destination.Add(_source[i]);
		}
	}
}
