using UnityEngine;
using UnityEngine.UI;

public class NKC_FX_GLOBAL_OPACITY : MonoBehaviour
{
	private void Start()
	{
		float globalFloat = Shader.GetGlobalFloat("_FxGlobalTransparency");
		Slider component = GetComponent<Slider>();
		if (component != null)
		{
			component.value = globalFloat;
		}
	}

	private void OnDestroy()
	{
	}

	public static void Execute(float _factor)
	{
		Shader.SetGlobalFloat("_FxGlobalTransparency", _factor);
		Shader.SetGlobalFloat("_FxGlobalTransparencyEnemy", _factor);
	}
}
