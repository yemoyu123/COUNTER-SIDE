using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class DrawHitBox : MonoBehaviour
{
	public Color BoxColor;

	public Vector2 BoxSize;

	public bool Centered;

	public bool AutoDisable;

	[Range(0f, 5f)]
	public float Lifetime = 1f;

	private bool show = true;

	private LineRenderer rend;

	private Vector3 pos = new Vector3(0f, 0f, 0f);

	public void ShowHitBox(bool _toggle)
	{
		show = _toggle;
		if (!show)
		{
			rend = GetComponent<LineRenderer>();
			rend.enabled = false;
		}
	}

	private void OnEnable()
	{
		rend = GetComponent<LineRenderer>();
		if (show)
		{
			rend.enabled = true;
		}
		else
		{
			rend.enabled = false;
		}
		if (AutoDisable)
		{
			HandleDisappear();
		}
	}

	private void HandleDisappear()
	{
		if (Application.isPlaying)
		{
			CancelInvoke();
			Invoke("Disappear", Lifetime);
		}
	}

	private void Disappear()
	{
		rend.enabled = false;
	}

	private void OnValidate()
	{
		if (!(rend == null))
		{
			rend.useWorldSpace = false;
			rend.widthMultiplier = BoxSize.y;
			rend.sortingLayerName = "GAME_UI_FRONT";
			rend.sortingOrder = 0;
			rend.startColor = BoxColor;
			rend.endColor = BoxColor;
			if (Centered)
			{
				pos.Set(0f - BoxSize.x * 0.5f, 0f, 0f);
				rend.SetPosition(0, pos);
				pos.Set(BoxSize.x * 0.5f, 0f, 0f);
				rend.SetPosition(1, pos);
			}
			else
			{
				pos.Set(0f, 0f, 0f);
				rend.SetPosition(0, pos);
				pos.Set(BoxSize.x, 0f, 0f);
				rend.SetPosition(1, pos);
			}
		}
	}
}
