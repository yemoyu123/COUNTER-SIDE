using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class UITextSync : MonoBehaviour
{
	public Text Target;

	private string oldName = string.Empty;

	private void Awake()
	{
		if (Target == null)
		{
			Target = base.gameObject.GetComponentInChildren<Text>();
		}
	}

	private void Start()
	{
		if (Application.isPlaying)
		{
			Object.DestroyImmediate(this);
		}
	}

	private void Update()
	{
		if (base.gameObject.name != oldName)
		{
			oldName = base.gameObject.name;
			Target.text = oldName;
		}
	}
}
