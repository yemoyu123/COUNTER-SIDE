using UnityEngine;

public class NKC_FX_UNIT_SCALE_CTRL : MonoBehaviour
{
	private Transform unitRoot;

	private Vector3 newScale;

	private void Start()
	{
	}

	public void SetScaleFactor(float _scale)
	{
		if (unitRoot != null)
		{
			newScale.Set(_scale, _scale, 1f);
			unitRoot.localScale = newScale;
		}
		else
		{
			Debug.LogWarning("Null Unit.");
		}
	}
}
