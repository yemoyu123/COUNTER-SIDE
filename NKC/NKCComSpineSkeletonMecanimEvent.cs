using System.Collections.Generic;
using Cs.Logging;
using UnityEngine;

namespace NKC;

public class NKCComSpineSkeletonMecanimEvent : MonoBehaviour
{
	public GameObject m_EFFECT_ROOT;

	private Dictionary<string, GameObject> m_dicEffect = new Dictionary<string, GameObject>();

	private void Awake()
	{
		m_dicEffect.Clear();
		ChildAllDisable();
	}

	private void OnDisable()
	{
		ChildAllDisable();
	}

	private void OnDestroy()
	{
		m_dicEffect.Clear();
	}

	private void ChildAllDisable()
	{
		if (!(m_EFFECT_ROOT != null))
		{
			return;
		}
		for (int i = 0; i < m_EFFECT_ROOT.transform.childCount; i++)
		{
			GameObject gameObject = m_EFFECT_ROOT.transform.GetChild(i).gameObject;
			if (gameObject.activeSelf)
			{
				gameObject.SetActive(value: false);
			}
		}
	}

	private void Event_ACTIVE(string objectName)
	{
		if (m_EFFECT_ROOT != null)
		{
			if (!string.IsNullOrEmpty(objectName))
			{
				Log.Info("<color=cyan><b>Event_ACTIVE</b></color> : " + base.transform.parent.name + " : " + objectName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/Components/NKCComSpineSkeletonMecanimEvent.cs", 48);
				GameObject gameObject;
				if (!m_dicEffect.ContainsKey(objectName))
				{
					Transform transform = m_EFFECT_ROOT.transform.Find(objectName);
					if (transform != null)
					{
						gameObject = transform.gameObject;
						m_dicEffect.Add(objectName, gameObject);
						if (gameObject.activeSelf)
						{
							gameObject.SetActive(value: false);
						}
						if (!gameObject.activeSelf)
						{
							gameObject.SetActive(value: true);
						}
					}
					else
					{
						Debug.LogWarning("Null cTransform");
					}
				}
				else if (m_dicEffect.TryGetValue(objectName, out gameObject))
				{
					if (gameObject.activeSelf)
					{
						gameObject.SetActive(value: false);
					}
					if (!gameObject.activeSelf)
					{
						gameObject.SetActive(value: true);
					}
				}
				else
				{
					Debug.LogWarning("Null cTargetObject");
				}
			}
			else
			{
				Debug.LogWarning("Null objectName");
			}
		}
		else
		{
			Debug.LogWarning("Null m_EFFECT_ROOT");
		}
	}
}
