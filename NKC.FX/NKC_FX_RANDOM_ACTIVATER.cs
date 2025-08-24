using UnityEngine;

namespace NKC.FX;

public class NKC_FX_RANDOM_ACTIVATER : MonoBehaviour
{
	public GameObject Obj;

	[Range(0f, 1f)]
	public float rate = 0.5f;

	private bool init;

	private void OnDestroy()
	{
		if (Obj != null)
		{
			Obj = null;
		}
	}

	private void Awake()
	{
		if (Obj != null && Obj.activeInHierarchy)
		{
			Obj.SetActive(value: false);
			init = true;
		}
	}

	private void OnEnable()
	{
		if (init && Random.Range(0f, 1f) > rate && !Obj.activeInHierarchy)
		{
			Obj.SetActive(value: true);
		}
	}
}
