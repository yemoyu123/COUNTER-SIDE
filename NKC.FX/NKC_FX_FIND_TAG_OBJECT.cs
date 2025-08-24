using UnityEngine;

namespace NKC.FX;

public class NKC_FX_FIND_TAG_OBJECT : MonoBehaviour
{
	public string Tag = string.Empty;

	public GameObject Target;

	public bool SyncTargetPosition;

	public NKC_FX_EVENT ConnectionEvent;

	public NKC_FX_EVENT DisconnectionEvent;

	public NKC_FX_EVENT MoveCompleteEvent;

	private GameObject[] taggedObjs;

	private NKC_FX_TAG_OBJECT obj;

	private int smallestIndex;

	private int targetIndex;

	private bool isFound;

	private bool executed;

	private void OnDestroy()
	{
		if (Target != null)
		{
			Target = null;
		}
		if (ConnectionEvent != null)
		{
			ConnectionEvent = null;
		}
		if (DisconnectionEvent != null)
		{
			DisconnectionEvent = null;
		}
		if (MoveCompleteEvent != null)
		{
			MoveCompleteEvent = null;
		}
		if (taggedObjs != null)
		{
			taggedObjs = null;
		}
		if (obj != null)
		{
			obj = null;
		}
	}

	private void Reset()
	{
		smallestIndex = int.MaxValue;
		isFound = false;
		executed = false;
	}

	private void OnEnable()
	{
		Reset();
		taggedObjs = GameObject.FindGameObjectsWithTag(Tag);
		FindTarget();
	}

	private void OnDisable()
	{
		if (obj != null)
		{
			obj.IsConnetect = false;
		}
	}

	private void Update()
	{
		if (!isFound || !(Target != null) || !SyncTargetPosition)
		{
			return;
		}
		base.transform.position = Target.transform.position;
		if (!executed)
		{
			executed = true;
			if (MoveCompleteEvent != null)
			{
				MoveCompleteEvent.Execute();
			}
		}
	}

	private void FindTarget()
	{
		for (int i = 0; i < taggedObjs.Length; i++)
		{
			obj = taggedObjs[i].GetComponent<NKC_FX_TAG_OBJECT>();
			if (obj.isActiveAndEnabled && !obj.IsConnetect && smallestIndex >= obj.Index)
			{
				smallestIndex = obj.Index;
				Target = taggedObjs[i];
				targetIndex = i;
				if (Target != null)
				{
					isFound = true;
				}
				else
				{
					isFound = false;
				}
			}
		}
		if (!(Target != null))
		{
			return;
		}
		for (int j = 0; j < taggedObjs.Length; j++)
		{
			obj = taggedObjs[j].GetComponent<NKC_FX_TAG_OBJECT>();
			if (j == targetIndex)
			{
				obj.LinkedTagObj = this;
				obj.IsConnetect = true;
				break;
			}
		}
	}
}
