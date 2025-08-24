using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class NKC_FX_TIMESCALE : MonoBehaviour
{
	[Range(0f, 10f)]
	public float TimeScale = 1f;

	public Text TextBoxTimeScale;

	public Text TextBoxPaused;

	private bool init;

	private bool IsOn;

	private bool IsStepping;

	private void Start()
	{
		if (TextBoxTimeScale != null && TextBoxPaused != null)
		{
			TextBoxPaused.enabled = false;
			init = true;
		}
	}

	public void SetTimeScale(float timeScale)
	{
		if (init)
		{
			TimeScale = timeScale;
			TimeScale = Mathf.Clamp(TimeScale, 0f, 3f);
			Time.timeScale = TimeScale;
			if (TextBoxTimeScale != null)
			{
				TextBoxTimeScale.text = Time.timeScale.ToString("N2");
			}
		}
	}

	public void IncDecTimeScale(float timeScale)
	{
		if (init)
		{
			TimeScale += timeScale;
			TimeScale = Mathf.Clamp(TimeScale, 0f, 3f);
			Time.timeScale = TimeScale;
			if (TextBoxTimeScale != null)
			{
				TextBoxTimeScale.text = Time.timeScale.ToString("N2");
			}
		}
	}

	private void Update()
	{
		if (init)
		{
			if (Input.GetKeyDown(KeyCode.Z))
			{
				Toggle();
			}
			else if (Input.GetKeyDown(KeyCode.X))
			{
				Step();
			}
			else if (Input.GetKey(KeyCode.C))
			{
				Step();
			}
			if (IsStepping)
			{
				Time.timeScale = Mathf.Clamp(TimeScale, 0f, 3f);
				IsStepping = false;
			}
			else
			{
				Time.timeScale = (IsOn ? 0f : Mathf.Clamp(TimeScale, 0f, 3f));
			}
		}
	}

	private void Toggle()
	{
		IsOn = !IsOn;
		TextBoxPaused.enabled = !TextBoxPaused.enabled;
	}

	private void Step()
	{
		IsStepping = true;
	}
}
