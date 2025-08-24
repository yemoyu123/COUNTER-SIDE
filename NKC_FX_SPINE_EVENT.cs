using System;
using NKC;
using Spine;
using Spine.Unity;
using UnityEngine;

[ExecuteAlways]
public class NKC_FX_SPINE_EVENT : MonoBehaviour
{
	[Serializable]
	public class AnimationData
	{
		public string m_AnimationName;

		public bool m_Loop;

		public Color m_KeyColor;

		public float m_TimeScale;

		public float m_Duration;
	}

	[Serializable]
	public class AnimationEvent
	{
		public bool m_Enable;

		public bool m_External;

		[SpineAnimation("", "skeletonDataAsset", true, false)]
		public string m_AnimationName;

		public float m_Frame;

		public string m_EventName;

		public int m_EventObjInstanceID;

		[HideInInspector]
		public bool activated;
	}

	public bool DebugMode;

	public GameObject m_SpinePrefab;

	public GameObject m_ExternalPrefab;

	public string PathExport = "D:";

	public bool AutoKeyName = true;

	public GameObject m_BaseBtnPrefab;

	public SkeletonDataAsset skeletonDataAsset;

	private SkeletonAnimation skeletonAnimation;

	private SkeletonGraphic skeletonGraphic;

	private NKCComSpineSkeletonAnimationEvent SkeletonEvent;

	public AnimationData[] AnimationDatas;

	public AnimationEvent[] EventList;

	private ExposedList<Spine.Animation> animations;

	private TrackEntry currentTrackEntry;

	private Transform VFX;

	private float currentTime;

	private int index;

	private bool init;

	private const int baseFrame = 30;

	private int childCount;

	private int newIndex;

	private int animationIndex;

	private int eventIndex;

	private string[] sortedEventName;

	private Transform[] tempTrs;

	private void OnEnable()
	{
		Initialize();
	}

	private void Start()
	{
		Shader.WarmupAllShaders();
		if (Application.isPlaying)
		{
			CheckEventMode();
			InitExternal(_active: false);
		}
	}

	public void Initialize()
	{
		if (!(m_SpinePrefab != null))
		{
			return;
		}
		Transform transform = m_SpinePrefab.transform.Find("SPINE_SkeletonAnimation");
		if (transform != null)
		{
			skeletonAnimation = transform.GetComponentInChildren<SkeletonAnimation>(includeInactive: true);
			if (skeletonAnimation != null)
			{
				skeletonDataAsset = skeletonAnimation.SkeletonDataAsset;
				InitSkeletonAnimation();
			}
			return;
		}
		transform = m_SpinePrefab.transform.Find("SPINE_SkeletonGraphic");
		if (transform != null)
		{
			skeletonGraphic = transform.GetComponentInChildren<SkeletonGraphic>(includeInactive: true);
			if (skeletonGraphic != null)
			{
				skeletonDataAsset = skeletonGraphic.SkeletonDataAsset;
				InitSkeletonGraphic();
			}
		}
		else
		{
			Debug.LogWarning("Can not found SPINE Object.");
		}
	}

	public void InitializeAnimationData()
	{
		if (skeletonDataAsset == null)
		{
			return;
		}
		animations = skeletonDataAsset.GetAnimationStateData().SkeletonData.Animations;
		int count = animations.Count;
		AnimationDatas = new AnimationData[count];
		for (int i = 0; i < count; i++)
		{
			Spine.Animation animation = animations.Items[i];
			AnimationDatas[i] = new AnimationData();
			AnimationDatas[i].m_AnimationName = animation.Name;
			AnimationDatas[i].m_Duration = animation.Duration;
			AnimationDatas[i].m_Loop = false;
			if (animation.Name.Contains("ASTAND") || animation.Name.Contains("RUN") || animation.Name.Contains("IDLE"))
			{
				AnimationDatas[i].m_Loop = true;
			}
			AnimationDatas[i].m_KeyColor = Color.white;
			AnimationDatas[i].m_TimeScale = 1f;
		}
	}

	private void InitExternal(bool _active)
	{
		if (!m_ExternalPrefab)
		{
			return;
		}
		for (int i = 0; i < m_ExternalPrefab.transform.childCount; i++)
		{
			GameObject gameObject = m_ExternalPrefab.transform.GetChild(i).gameObject;
			if (gameObject.name != "SPINE_SkeletonGraphic")
			{
				gameObject.SetActive(_active);
			}
		}
	}

	private void LoadSpineAnimationEvent()
	{
		if (!(m_SpinePrefab != null))
		{
			return;
		}
		SkeletonEvent = m_SpinePrefab.GetComponentInChildren<NKCComSpineSkeletonAnimationEvent>();
		if (SkeletonEvent != null)
		{
			if (SkeletonEvent.m_EFFECT_ROOT != null)
			{
				VFX = SkeletonEvent.m_EFFECT_ROOT.transform;
			}
			else
			{
				Debug.LogWarning(m_SpinePrefab.name + " -> <color=red>EFFECT_ROOT is NULL</color>", SkeletonEvent);
			}
		}
		else
		{
			Debug.LogWarning(m_SpinePrefab.name + " -> <color=red>Event Component can not found.</color>", SkeletonEvent);
		}
	}

	private void CheckEventMode()
	{
		LoadSpineAnimationEvent();
		if (DebugMode)
		{
			Debug.Log(m_SpinePrefab.name + " -> <color=red>DEBUG MODE</color>");
			SkeletonEvent.enabled = false;
		}
		else
		{
			Debug.Log(m_SpinePrefab.name + " -> <color=green>GAME MODE</color>");
			SkeletonEvent.enabled = true;
		}
	}

	private void ResetEventsActivation()
	{
		for (int i = 0; i < EventList.Length; i++)
		{
			EventList[i].activated = false;
		}
	}

	private void InitSkeletonAnimation()
	{
		skeletonAnimation.Awake();
		if (EventList != null)
		{
			skeletonAnimation.UpdateComplete += delegate
			{
				if (currentTrackEntry != null)
				{
					UpdateInternal(currentTrackEntry);
				}
			};
			skeletonAnimation.AnimationState.Start += delegate(TrackEntry entry)
			{
				if (currentTrackEntry != null && currentTrackEntry.Animation != null && currentTrackEntry.Animation.Name != entry.Animation.Name)
				{
					ResetEventsActivation();
				}
				currentTrackEntry = entry;
			};
			skeletonAnimation.AnimationState.Complete += delegate
			{
				ResetEventsActivation();
			};
		}
		init = true;
	}

	private void InitSkeletonGraphic()
	{
		skeletonGraphic.Initialize(overwrite: false);
		if (EventList != null)
		{
			skeletonGraphic.UpdateComplete += delegate
			{
				if (currentTrackEntry != null)
				{
					UpdateInternal(currentTrackEntry);
				}
			};
			skeletonGraphic.AnimationState.Start += delegate(TrackEntry entry)
			{
				if (currentTrackEntry != null && currentTrackEntry.Animation != null && currentTrackEntry.Animation.Name != entry.Animation.Name)
				{
					ResetEventsActivation();
				}
				currentTrackEntry = entry;
			};
			skeletonGraphic.AnimationState.Complete += delegate
			{
				ResetEventsActivation();
			};
		}
		init = true;
	}

	private void UpdateInternal(TrackEntry entry)
	{
		if (!init || !base.enabled)
		{
			return;
		}
		currentTime = entry.AnimationTime;
		if (!(0f <= currentTime) || !(currentTime < entry.Animation.Duration))
		{
			return;
		}
		for (int i = 0; i < EventList.Length; i++)
		{
			string text = "<color=black><b>";
			string text2 = "<color=yellow><b>";
			if (!EventList[i].m_Enable || EventList[i].activated || !(EventList[i].m_AnimationName == entry.Animation.Name) || !(EventList[i].m_Frame < currentTime * 30f))
			{
				continue;
			}
			EventList[i].activated = true;
			if (!EventList[i].m_External)
			{
				if (!DebugMode)
				{
					break;
				}
				if (VFX.Find(EventList[i].m_EventName) != null && EventList[i].m_EventName != string.Empty)
				{
					GameObject gameObject = VFX.Find(EventList[i].m_EventName).gameObject;
					if (gameObject.activeInHierarchy)
					{
						gameObject.SetActive(value: false);
					}
					if (!gameObject.activeInHierarchy)
					{
						gameObject.SetActive(value: true);
					}
					if (DebugMode)
					{
						Debug.Log("<color=#84FF37><b>" + entry.Animation.Name + "</b></color> : " + currentTime.ToString("N3") + " / " + entry.Animation.Duration.ToString("N3") + $", [{EventList[i].m_Frame} / {Mathf.Round(entry.Animation.Duration * 30f)}], {entry.TimeScale}x -> " + text + EventList[i].m_EventName + "</b></color>");
					}
				}
				else if (DebugMode)
				{
					Debug.Log("<color=#84FF37><b>" + entry.Animation.Name + "</b></color> : " + currentTime.ToString("N3") + " / " + entry.Animation.Duration.ToString("N3") + $", [{EventList[i].m_Frame} / {Mathf.Round(entry.Animation.Duration * 30f)}], {entry.TimeScale}x -> " + text + EventList[i].m_EventName + "</b></color><color=red> (Not Found)</color>");
				}
			}
			else if (m_ExternalPrefab.transform.Find(EventList[i].m_EventName) != null && EventList[i].m_EventName != string.Empty)
			{
				GameObject gameObject2 = m_ExternalPrefab.transform.Find(EventList[i].m_EventName).gameObject;
				if (gameObject2.activeInHierarchy)
				{
					gameObject2.SetActive(value: false);
				}
				if (!gameObject2.activeInHierarchy)
				{
					gameObject2.SetActive(value: true);
				}
				if (DebugMode)
				{
					Debug.Log("<color=#84FF37><b>" + entry.Animation.Name + "</b></color> : " + currentTime.ToString("N3") + " / " + entry.Animation.Duration.ToString("N3") + $", [{EventList[i].m_Frame} / {Mathf.Round(entry.Animation.Duration * 30f)}], {entry.TimeScale}x -> " + text2 + EventList[i].m_EventName + "</b></color>");
				}
			}
			else if (DebugMode)
			{
				Debug.Log("<color=#84FF37><b>" + entry.Animation.Name + "</b></color> : " + currentTime.ToString("N3") + " / " + entry.Animation.Duration.ToString("N3") + $", [{EventList[i].m_Frame} / {Mathf.Round(entry.Animation.Duration * 30f)}], {entry.TimeScale}x -> " + text2 + EventList[i].m_EventName + "</b></color><color=red> (Not Found)</color>");
			}
		}
	}

	public void SetAnimationName(string _animationName)
	{
		if (!init || AnimationDatas.Length == 0)
		{
			return;
		}
		if (ContainAnimation(_animationName))
		{
			if (currentTrackEntry != null)
			{
				ResetEventsActivation();
			}
			index = GetAnimationIndex(_animationName);
			bool loop = AnimationDatas[index].m_Loop;
			float timeScale = AnimationDatas[index].m_TimeScale;
			if (skeletonAnimation != null)
			{
				skeletonAnimation.AnimationState.SetAnimation(0, _animationName, loop).TimeScale = timeScale;
			}
			else if (skeletonGraphic != null)
			{
				skeletonGraphic.AnimationState.SetAnimation(0, _animationName, loop).TimeScale = timeScale;
			}
		}
		else
		{
			Debug.LogWarning("Not found animation name. : " + _animationName);
		}
	}

	public void AddAnimationName(string _animationName)
	{
		if (!init || AnimationDatas.Length == 0)
		{
			return;
		}
		if (ContainAnimation(_animationName))
		{
			index = GetAnimationIndex(_animationName);
			bool loop = AnimationDatas[index].m_Loop;
			float timeScale = AnimationDatas[index].m_TimeScale;
			if (skeletonAnimation != null)
			{
				skeletonAnimation.AnimationState.AddAnimation(0, _animationName, loop, 0f).TimeScale = timeScale;
			}
			else if (skeletonGraphic != null)
			{
				skeletonGraphic.AnimationState.AddAnimation(0, _animationName, loop, 0f).TimeScale = timeScale;
			}
		}
		else
		{
			Debug.LogWarning("Not found animation name. : " + _animationName);
		}
	}

	private bool ContainAnimation(string _animationName)
	{
		bool result = false;
		for (int i = 0; i < AnimationDatas.Length; i++)
		{
			if (AnimationDatas[i].m_AnimationName.Equals(_animationName))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private int GetAnimationIndex(string _animationName)
	{
		int result = 0;
		for (int i = 0; i < AnimationDatas.Length; i++)
		{
			if (AnimationDatas[i].m_AnimationName.Equals(_animationName))
			{
				result = i;
				break;
			}
		}
		return result;
	}
}
