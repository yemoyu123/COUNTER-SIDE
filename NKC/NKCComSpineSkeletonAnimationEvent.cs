using System.Collections.Generic;
using Cs.Logging;
using NKC.FX;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace NKC;

[DisallowMultipleComponent]
public class NKCComSpineSkeletonAnimationEvent : MonoBehaviour
{
	public GameObject m_EFFECT_ROOT;

	private NKC_FX_LIFETIME[] m_FX_Lifetime;

	private bool m_bAddEvent;

	private bool m_bActiveEvent = true;

	private SkeletonAnimation m_SkeletonAnimation;

	private SkeletonGraphic m_SkeletonGraphic;

	private string m_SkeletonDataAssetName = "";

	private char[] m_SplitToken = new char[1] { ':' };

	private Dictionary<string, Bone> m_dicBone = new Dictionary<string, Bone>();

	private HashSet<GameObject> m_setGameObject = new HashSet<GameObject>();

	private GameObject m_NKM_GLOBAL_EFFECT;

	private Dictionary<string, List<GameObject>> m_dicGlobalEffect = new Dictionary<string, List<GameObject>>();

	private float sinceTime;

	private float deltatime;

	private float current;

	private TrackEntry currentEntry;

	public void SetActiveEvent(bool bActiveEvent)
	{
		m_bActiveEvent = bActiveEvent;
	}

	private void Awake()
	{
		m_SkeletonAnimation = GetComponent<SkeletonAnimation>();
		if (m_SkeletonAnimation == null)
		{
			m_SkeletonGraphic = GetComponent<SkeletonGraphic>();
		}
		m_dicBone.Clear();
		m_setGameObject.Clear();
		m_NKM_GLOBAL_EFFECT = GameObject.Find("NKM_GLOBAL_EFFECT");
		if (m_EFFECT_ROOT == null)
		{
			if (m_EFFECT_ROOT == null)
			{
				Debug.LogWarning("VFX 오브젝트가 없습니다. " + base.transform.parent?.name);
			}
		}
		else if (m_EFFECT_ROOT.name != "VFX")
		{
			Debug.LogWarning("VFX Root 오브젝트가 아닐 수도 있습니다. 현재 : " + m_EFFECT_ROOT.name + " ", base.gameObject);
		}
		AddEvent();
	}

	private void Start()
	{
		AddEvent();
		InitFxLifetime();
	}

	private void InitFxLifetime()
	{
		if (!(m_EFFECT_ROOT != null))
		{
			return;
		}
		m_FX_Lifetime = m_EFFECT_ROOT.GetComponentsInChildren<NKC_FX_LIFETIME>(includeInactive: true);
		if (m_FX_Lifetime != null && m_FX_Lifetime.Length != 0)
		{
			for (int i = 0; i < m_FX_Lifetime.Length; i++)
			{
				m_FX_Lifetime[i].Init = true;
			}
		}
	}

	public void AddEvent(bool bForce = false)
	{
		if (!bForce && m_bAddEvent)
		{
			return;
		}
		if (m_SkeletonAnimation != null && m_SkeletonAnimation.SkeletonDataAsset != null)
		{
			m_SkeletonDataAssetName = m_SkeletonAnimation.SkeletonDataAsset.name;
			if (m_SkeletonAnimation.AnimationState != null)
			{
				m_SkeletonAnimation.AnimationState.Start -= OnStart;
				m_SkeletonAnimation.AnimationState.Start += OnStart;
				m_SkeletonAnimation.UpdateComplete -= OnUpdate;
				m_SkeletonAnimation.UpdateComplete += OnUpdate;
				m_SkeletonAnimation.AnimationState.Event -= OnSkeletonAnimationEvent;
				m_SkeletonAnimation.AnimationState.Event += OnSkeletonAnimationEvent;
				m_bAddEvent = true;
			}
		}
		else if (m_SkeletonGraphic != null && m_SkeletonGraphic.SkeletonDataAsset != null)
		{
			m_SkeletonDataAssetName = m_SkeletonGraphic.SkeletonDataAsset.name;
			if (m_SkeletonGraphic.AnimationState != null)
			{
				m_SkeletonGraphic.AnimationState.Event -= OnSkeletonAnimationEvent;
				m_SkeletonGraphic.AnimationState.Event += OnSkeletonAnimationEvent;
				m_bAddEvent = true;
			}
		}
	}

	private void OnStart(TrackEntry entry)
	{
		if (entry != null)
		{
			currentEntry = entry;
			current = currentEntry.TrackTime;
		}
	}

	private void OnUpdate(ISkeletonAnimation s)
	{
		if (currentEntry == null)
		{
			return;
		}
		sinceTime = currentEntry.TrackTime;
		deltatime = Mathf.Clamp(sinceTime - current, 0f, Time.maximumDeltaTime);
		current = sinceTime;
		if (m_FX_Lifetime == null || m_FX_Lifetime.Length == 0)
		{
			return;
		}
		for (int i = 0; i < m_FX_Lifetime.Length; i++)
		{
			if (m_FX_Lifetime[i].gameObject.activeInHierarchy)
			{
				m_FX_Lifetime[i].UpdateLifeTime(deltatime * currentEntry.TimeScale);
			}
		}
	}

	private void OnDisable()
	{
		HashSet<GameObject>.Enumerator enumerator = m_setGameObject.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current.activeSelf)
			{
				enumerator.Current.SetActive(value: false);
			}
		}
		m_bAddEvent = false;
	}

	private void OnDestroy()
	{
		if (m_SkeletonAnimation != null)
		{
			m_SkeletonAnimation.UpdateComplete -= OnUpdate;
			if (m_SkeletonAnimation.state != null)
			{
				m_SkeletonAnimation.state.Start -= OnStart;
				m_SkeletonAnimation.state.Event -= OnSkeletonAnimationEvent;
			}
			m_SkeletonAnimation = null;
		}
		if (m_SkeletonGraphic != null)
		{
			if (m_SkeletonGraphic.AnimationState != null)
			{
				m_SkeletonGraphic.AnimationState.Event -= OnSkeletonAnimationEvent;
			}
			m_SkeletonGraphic = null;
		}
		Dictionary<string, List<GameObject>>.Enumerator enumerator = m_dicGlobalEffect.GetEnumerator();
		while (enumerator.MoveNext())
		{
			List<GameObject> value = enumerator.Current.Value;
			for (int i = 0; i < value.Count; i++)
			{
				Object.Destroy(value[i]);
			}
			value.Clear();
		}
		m_dicGlobalEffect.Clear();
		m_bAddEvent = false;
	}

	private void OnSkeletonAnimationEvent(TrackEntry trackEntry, Spine.Event e)
	{
		if (!base.enabled || !m_bActiveEvent)
		{
			return;
		}
		if (e.Data.Name == "SE_ACTIVE")
		{
			if (e.String.Length >= 2)
			{
				SPINE_EVENT_OPTION eSPINE_EVENT_OPTION = (SPINE_EVENT_OPTION)e.Int;
				string[] array = e.String.Split(m_SplitToken);
				if (array.Length > 1)
				{
					Event_ACTIVE(eSPINE_EVENT_OPTION, e.Data.Name, array[0], array[1]);
				}
				else
				{
					Event_ACTIVE(eSPINE_EVENT_OPTION, e.Data.Name, array[0], null);
				}
			}
		}
		else if (e.Data.Name == "SE_ACTIVE_GLOBAL")
		{
			if (e.String.Length >= 2)
			{
				string[] array2 = e.String.Split(m_SplitToken);
				if (array2.Length > 1)
				{
					Event_ACTIVE_GLOBAL(e.Data.Name, array2[0], array2[1]);
				}
				else
				{
					Event_ACTIVE_GLOBAL(e.Data.Name, array2[0], null);
				}
			}
		}
		else if (e.Data.Name == "SE_SOUND")
		{
			if (e.String.Length >= 2)
			{
				Event_SOUND(e.Data.Name, e.String);
			}
		}
		else
		{
			Log.Warn("Invalid Event Name : [" + e.Data.Name + "]", this, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Components/NKCComSpineSkeletonAnimationEvent.cs", 251);
		}
	}

	public void ValidateSkeletonAnimationEvent(Spine.Event e)
	{
		Awake();
		Start();
		Log.Info("Valid Check: " + m_SkeletonDataAssetName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Components/NKCComSpineSkeletonAnimationEvent.cs", 260);
		OnSkeletonAnimationEvent(null, e);
	}

	private void Event_ACTIVE(SPINE_EVENT_OPTION eSPINE_EVENT_OPTION, string eventName, string objectName, string boneName)
	{
		if (eventName == null)
		{
			Log.Error("Event_ACTIVE eventName null: " + m_SkeletonDataAssetName + ", " + eventName + ", " + objectName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Components/NKCComSpineSkeletonAnimationEvent.cs", 268);
			return;
		}
		if (objectName == null)
		{
			Log.Error("Event_ACTIVE objectName null: " + m_SkeletonDataAssetName + ", " + eventName + ", " + objectName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Components/NKCComSpineSkeletonAnimationEvent.cs", 274);
			return;
		}
		if (m_EFFECT_ROOT == null)
		{
			Log.Error("Event_ACTIVE m_EFFECT_ROOT null: " + m_SkeletonDataAssetName + ", " + eventName + ", " + objectName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Components/NKCComSpineSkeletonAnimationEvent.cs", 280);
			return;
		}
		if (m_EFFECT_ROOT.transform == null)
		{
			Log.Error("Event_ACTIVE m_EFFECT_ROOT.transform null: " + m_SkeletonDataAssetName + ", " + eventName + ", " + objectName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Components/NKCComSpineSkeletonAnimationEvent.cs", 286);
			return;
		}
		Transform transform = m_EFFECT_ROOT.transform.Find(objectName);
		if (transform == null)
		{
			Log.Error("Event_ACTIVE invalid objectName: " + m_SkeletonDataAssetName + ", " + eventName + ", " + objectName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Components/NKCComSpineSkeletonAnimationEvent.cs", 294);
			return;
		}
		GameObject gameObject = transform.gameObject;
		if (gameObject == null)
		{
			Log.Error("Event_ACTIVE cTargetObject null: " + m_SkeletonDataAssetName + ", " + eventName + ", " + objectName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Components/NKCComSpineSkeletonAnimationEvent.cs", 301);
			return;
		}
		if (eSPINE_EVENT_OPTION == SPINE_EVENT_OPTION.SEO_NORMAL && gameObject.activeSelf)
		{
			gameObject.SetActive(value: false);
		}
		if (boneName != null && boneName.Length > 1)
		{
			Bone bone = null;
			if (!m_dicBone.ContainsKey(boneName))
			{
				if (m_SkeletonAnimation != null && m_SkeletonAnimation.skeleton != null)
				{
					bone = m_SkeletonAnimation.skeleton.FindBone(boneName);
				}
				else
				{
					if (!(m_SkeletonGraphic != null) || m_SkeletonGraphic.Skeleton == null)
					{
						Log.Error("Event_ACTIVE skeleton null: " + m_SkeletonDataAssetName + ", " + eventName + ", " + objectName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Components/NKCComSpineSkeletonAnimationEvent.cs", 322);
						return;
					}
					bone = m_SkeletonGraphic.Skeleton.FindBone(boneName);
				}
				if (bone != null)
				{
					m_dicBone.Add(boneName, bone);
				}
			}
			else
			{
				bone = m_dicBone[boneName];
			}
			if (bone != null)
			{
				Vector3 localPosition = gameObject.transform.localPosition;
				localPosition.x = bone.WorldX;
				localPosition.y = bone.WorldY;
				gameObject.transform.localPosition = localPosition;
			}
			else
			{
				Log.Error($"Event_ACTIVE cBone null: {m_SkeletonDataAssetName}, {eventName}, {objectName}, {bone}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Components/NKCComSpineSkeletonAnimationEvent.cs", 345);
			}
		}
		if (!gameObject.activeSelf)
		{
			gameObject.SetActive(value: true);
		}
		if (!m_setGameObject.Contains(gameObject))
		{
			m_setGameObject.Add(gameObject);
		}
	}

	private void Event_ACTIVE_GLOBAL(string eventName, string objectName, string boneName)
	{
		Transform transform = m_EFFECT_ROOT.transform.Find(objectName);
		if (transform == null)
		{
			Log.Error("Event_ACTIVE invalid objectName: " + m_EFFECT_ROOT.name + ", " + eventName + ", " + objectName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Components/NKCComSpineSkeletonAnimationEvent.cs", 359);
			return;
		}
		List<GameObject> list = null;
		if (!m_dicGlobalEffect.ContainsKey(transform.gameObject.name))
		{
			list = new List<GameObject>();
			m_dicGlobalEffect.Add(transform.gameObject.name, list);
		}
		list = m_dicGlobalEffect[transform.gameObject.name];
		GameObject gameObject = null;
		for (int i = 0; i < list.Count; i++)
		{
			if (!list[i].activeSelf)
			{
				gameObject = list[i];
				break;
			}
		}
		if (gameObject == null)
		{
			gameObject = Object.Instantiate(transform.gameObject, m_EFFECT_ROOT.transform);
			list.Add(gameObject);
		}
		if (boneName != null && boneName.Length > 1)
		{
			Bone bone = null;
			if (!m_dicBone.ContainsKey(boneName))
			{
				if (m_SkeletonAnimation != null)
				{
					bone = m_SkeletonAnimation.skeleton.FindBone(boneName);
				}
				else if (m_SkeletonGraphic != null)
				{
					bone = m_SkeletonGraphic.Skeleton.FindBone(boneName);
				}
				if (bone != null)
				{
					m_dicBone.Add(boneName, bone);
				}
			}
			else
			{
				bone = m_dicBone[boneName];
			}
			if (bone != null)
			{
				Vector3 localPosition = gameObject.transform.localPosition;
				localPosition.x = bone.WorldX;
				localPosition.y = bone.WorldY;
				gameObject.transform.localPosition = localPosition;
			}
			else
			{
				Log.Error($"Event_ACTIVE cBone null: {m_EFFECT_ROOT.name}, {eventName}, {objectName}, {bone}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Components/NKCComSpineSkeletonAnimationEvent.cs", 415);
			}
		}
		gameObject.transform.SetParent(m_NKM_GLOBAL_EFFECT.transform);
		if (!gameObject.activeSelf)
		{
			gameObject.SetActive(value: true);
		}
	}

	private void Event_SOUND(string eventName, string objectName)
	{
		NKCSoundManager.PlaySound(objectName, 1f, m_EFFECT_ROOT.transform.position.x, 800f);
	}
}
