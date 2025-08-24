using UnityEngine;

namespace NKC.FX;

public class NKC_FX_TAG_OBJECT : MonoBehaviour
{
	private int index;

	private bool isConnetect;

	private NKC_FX_FIND_TAG_OBJECT linkedTagObj;

	private GameObject[] taggedObjs;

	private NKC_FX_TAG_OBJECT obj;

	private int biggestIndex;

	private NKC_FX_EVENT fxEvent;

	public int Index
	{
		get
		{
			return index;
		}
		set
		{
			index = value;
		}
	}

	public bool IsConnetect
	{
		get
		{
			return isConnetect;
		}
		set
		{
			isConnetect = value;
			ConnectionEvent(isConnetect);
		}
	}

	public NKC_FX_FIND_TAG_OBJECT LinkedTagObj
	{
		get
		{
			return linkedTagObj;
		}
		set
		{
			linkedTagObj = value;
		}
	}

	private void OnDestroy()
	{
		if (linkedTagObj != null)
		{
			linkedTagObj = null;
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

	private void OnEnable()
	{
		isConnetect = false;
		index = 0;
		biggestIndex = 0;
		SetIndex();
	}

	private void OnDisable()
	{
		isConnetect = false;
		index = 0;
		biggestIndex = 0;
	}

	private void ConnectionEvent(bool isConnected)
	{
		if (!(linkedTagObj != null))
		{
			return;
		}
		if (isConnected)
		{
			fxEvent = linkedTagObj.ConnectionEvent;
			if (fxEvent != null)
			{
				fxEvent.Execute();
			}
		}
		else
		{
			fxEvent = linkedTagObj.DisconnectionEvent;
			if (fxEvent != null)
			{
				fxEvent.Execute();
			}
		}
	}

	private void SetIndex()
	{
		taggedObjs = GameObject.FindGameObjectsWithTag(base.gameObject.tag);
		if (taggedObjs.Length.Equals(1))
		{
			index = 0;
			return;
		}
		for (int i = 0; i < taggedObjs.Length; i++)
		{
			obj = taggedObjs[i].GetComponent<NKC_FX_TAG_OBJECT>();
			if (obj != null)
			{
				if (biggestIndex <= obj.Index)
				{
					biggestIndex = obj.Index;
				}
			}
			else
			{
				Debug.LogWarning("Null NKC_FX_TAG_OBJECT", base.gameObject);
			}
		}
		if (biggestIndex == 0)
		{
			index = 1;
		}
		else
		{
			index = biggestIndex + 1;
		}
	}
}
