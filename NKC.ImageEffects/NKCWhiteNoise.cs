using UnityEngine;

namespace NKC.ImageEffects;

public class NKCWhiteNoise : MonoBehaviour
{
	[SerializeField]
	[Range(0f, 1f)]
	public float Intensity;

	[SerializeField]
	[Range(0f, 1f)]
	public float ScanLineJitter;

	[SerializeField]
	[Range(0f, 1f)]
	public float VerticalJump;

	[SerializeField]
	[Range(0f, 1f)]
	public float HorizontalShake;

	private float VerticalJumpTime;

	public Shader _shader;

	private Material _material;

	private Texture2D _noiseTexture;

	public void Init()
	{
		SetUpResources();
	}

	private void SetUpResources()
	{
		if (!(_material != null))
		{
			_material = GetComponent<Renderer>().material;
			_material.hideFlags = HideFlags.DontSave;
			_noiseTexture = new Texture2D(128, 128, TextureFormat.ARGB32, mipChain: false);
			_noiseTexture.hideFlags = HideFlags.DontSave;
			_noiseTexture.wrapMode = TextureWrapMode.Clamp;
			_noiseTexture.filterMode = FilterMode.Point;
			_material.SetTexture("_MainTex", _noiseTexture);
			_material.SetFloat("_Intensity", Intensity);
			UpdateNoiseTexture();
		}
	}

	private void UpdateNoiseTexture()
	{
		for (int i = 0; i < _noiseTexture.height; i++)
		{
			for (int j = 0; j < _noiseTexture.width; j++)
			{
				if (Random.value > 0.5f)
				{
					_noiseTexture.SetPixel(j, i, Color.white);
				}
				else
				{
					_noiseTexture.SetPixel(j, i, Color.black);
				}
			}
		}
		_noiseTexture.Apply();
	}

	private void Update()
	{
		SetUpResources();
		if (Random.value > Mathf.Lerp(0.9f, 0.5f, Intensity))
		{
			UpdateNoiseTexture();
		}
		VerticalJumpTime += Time.deltaTime * VerticalJump * 11.3f;
		float y = Mathf.Clamp01(1f - ScanLineJitter * 1.2f);
		float x = 0.002f + Mathf.Pow(ScanLineJitter, 3f) * 0.05f;
		_material.SetVector("_ScanLineJitter", new Vector2(x, y));
		Vector2 vector = new Vector2(VerticalJump, VerticalJumpTime);
		_material.SetVector("_VerticalJump", vector);
		_material.SetFloat("_HorizontalShake", HorizontalShake * 0.2f);
	}
}
