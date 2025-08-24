using UnityEngine;

public class NKC_FX_ANIMATOR_SET_FLOAT : MonoBehaviour
{
	public Animator Target;

	public string Name = string.Empty;

	public float Value;

	private void Start()
	{
		if (Target == null)
		{
			Target = GetComponent<Animator>();
		}
	}

	private void Update()
	{
		if (Target != null)
		{
			Target.SetFloat(Name, Value);
		}
	}
}
