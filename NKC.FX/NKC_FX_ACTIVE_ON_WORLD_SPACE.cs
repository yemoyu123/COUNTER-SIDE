using Beebyte.Obfuscator;
using Cs.Logging;
using UnityEngine;

namespace NKC.FX;

public class NKC_FX_ACTIVE_ON_WORLD_SPACE : MonoBehaviour
{
	public Transform World;

	private Transform origin;

	private Vector3 tPosition;

	private Vector3 tRotation;

	private Vector3 tScale;

	private bool isLocal;

	private bool init;

	private void OnDestroy()
	{
		if (origin != null)
		{
			origin = null;
		}
		if (World != null)
		{
			World = null;
		}
	}

	private void Awake()
	{
		Init();
	}

	private void OnDisable()
	{
		if (init && !isLocal)
		{
			Log.Warn("Exeception located space.", base.gameObject, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/FX/NKC_FX_ACTIVE_ON_WORLD_SPACE.cs", 37);
		}
	}

	public void Init()
	{
		if (init)
		{
			return;
		}
		tPosition = base.transform.localPosition;
		tRotation = base.transform.localEulerAngles;
		tScale = base.transform.localScale;
		GameObject gameObject = GameObject.Find("NKM_GLOBAL_EFFECT");
		if (World == null && gameObject != null)
		{
			World = gameObject.transform;
		}
		if (base.transform.parent != null)
		{
			origin = base.transform.parent;
			if (origin != null)
			{
				init = true;
			}
			else
			{
				init = false;
			}
			isLocal = true;
		}
		else
		{
			Log.Warn("Already located world space.", base.gameObject, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/FX/NKC_FX_ACTIVE_ON_WORLD_SPACE.cs", 74);
			init = false;
		}
	}

	public void SetParent()
	{
		if (init && base.enabled && isLocal)
		{
			isLocal = false;
			if (IsInvoking())
			{
				CancelInvoke();
			}
			Invoke("HandleSetParent", Time.deltaTime);
			base.transform.parent = World;
		}
	}

	[SkipRename]
	private void HandleSetParent()
	{
		base.transform.parent = World;
	}

	public void ReParent()
	{
		if (!init || !base.enabled || isLocal)
		{
			return;
		}
		isLocal = true;
		if (origin != null)
		{
			if (IsInvoking())
			{
				CancelInvoke();
			}
			Invoke("HandleReParent", Time.deltaTime);
		}
		else
		{
			Log.Warn("Can't found Origin GameObject. This GameObject will self destroy.", base.gameObject, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/FX/NKC_FX_ACTIVE_ON_WORLD_SPACE.cs", 111);
			base.transform.parent = null;
			Object.Destroy(base.transform.root.gameObject, Time.maximumDeltaTime * 2f);
		}
	}

	[SkipRename]
	private void HandleReParent()
	{
		base.transform.parent = origin;
		base.transform.localScale = tScale;
		base.transform.localPosition = tPosition;
		base.transform.localEulerAngles = tRotation;
	}
}
