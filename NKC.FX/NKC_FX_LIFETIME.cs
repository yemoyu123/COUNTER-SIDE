using UnityEngine;

namespace NKC.FX;

public class NKC_FX_LIFETIME : MonoBehaviour
{
	[Range(0.1f, 10f)]
	public float Lifetime = 8f;

	[HideInInspector]
	public bool Init;

	private float elapsedTime;

	private bool isPassed;

	private void OnEnable()
	{
		if (Init)
		{
			elapsedTime = 0f;
			isPassed = false;
		}
	}

	public void UpdateLifeTime(float _dt)
	{
		elapsedTime += _dt;
		if (elapsedTime >= Lifetime && !isPassed)
		{
			isPassed = true;
			if (base.transform.gameObject.activeSelf)
			{
				base.transform.gameObject.SetActive(value: false);
			}
		}
	}
}
