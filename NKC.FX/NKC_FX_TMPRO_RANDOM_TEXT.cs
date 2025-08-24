using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace NKC.FX;

[Serializable]
[ExecuteAlways]
public class NKC_FX_TMPRO_RANDOM_TEXT : MonoBehaviour
{
	public TextMeshPro Target;

	public bool UseExtensionA;

	public bool UseExtensionB;

	public NKC_FX_TMPRO_RANDOM_TEXT_LIST RTL;

	public NKC_FX_TMPRO_RANDOM_TEXT_LIST RTL_A;

	public NKC_FX_TMPRO_RANDOM_TEXT_LIST RTL_B;

	private StringBuilder sb = new StringBuilder();

	private bool init;

	private void OnValidate()
	{
		Init();
	}

	private void OnDestroy()
	{
		if (sb != null)
		{
			sb = null;
		}
		if (RTL != null)
		{
			RTL = null;
		}
		if (RTL_A != null)
		{
			RTL_A = null;
		}
		if (RTL_B != null)
		{
			RTL_B = null;
		}
		if (Target != null)
		{
			Target = null;
		}
	}

	private void OnEnable()
	{
		Init();
	}

	private void Init()
	{
		init = true;
		if (Target == null)
		{
			Target = base.transform.GetComponent<TextMeshPro>();
			if (Target == null)
			{
				init = false;
				Debug.LogWarning("Null Target", base.gameObject);
				return;
			}
		}
		if (RTL == null)
		{
			init = false;
			Debug.LogWarning("Null RTL", base.gameObject);
		}
		else if (UseExtensionA)
		{
			if (RTL_A == null)
			{
				init = false;
				Debug.LogWarning("Null RTL_A", base.gameObject);
			}
			else if (UseExtensionB && RTL_B == null)
			{
				init = false;
				Debug.LogWarning("Null RTL_B", base.gameObject);
			}
		}
	}

	public void SetRandomText()
	{
		if (!init)
		{
			return;
		}
		sb.Clear();
		if (Application.isPlaying)
		{
			sb.Append(NKCStringTable.GetString(GetRandomText(RTL.Infos)));
		}
		else
		{
			sb.Append(GetRandomText(RTL.Infos));
		}
		if (UseExtensionA)
		{
			if (Application.isPlaying)
			{
				sb.Append(NKCStringTable.GetString(GetRandomText(RTL_A.Infos)));
			}
			else
			{
				sb.Append(GetRandomText(RTL_A.Infos));
			}
			if (UseExtensionB)
			{
				if (Application.isPlaying)
				{
					sb.Append(NKCStringTable.GetString(GetRandomText(RTL_B.Infos)));
				}
				else
				{
					sb.Append(GetRandomText(RTL_B.Infos));
				}
			}
		}
		Target.text = sb.ToString();
	}

	private string GetRandomText(RandomTextInfo[] _info)
	{
		float[] array = new float[_info.Length];
		for (int i = 0; i < _info.Length; i++)
		{
			if (_info[i].Prob > 0f)
			{
				array[i] = _info[i].Prob;
				continue;
			}
			array[i] = 0f;
			Debug.LogWarning("Prob Zero.", base.gameObject);
		}
		return _info[Choose(array)].Text;
	}

	private int Choose(float[] probs)
	{
		float num = 0f;
		foreach (float num2 in probs)
		{
			num += num2;
		}
		float num3 = UnityEngine.Random.value * num;
		for (int j = 0; j < probs.Length; j++)
		{
			if (num3 < probs[j])
			{
				return j;
			}
			num3 -= probs[j];
		}
		return probs.Length - 1;
	}
}
