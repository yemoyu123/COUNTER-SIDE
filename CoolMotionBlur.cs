using UnityEngine;

[ExecuteInEditMode]
public class CoolMotionBlur : MonoBehaviour
{
	[SerializeField]
	private Material screenMat;

	[SerializeField]
	private Vector2 movingCenter = new Vector2(0.5f, 0.5f);

	public Material ScreenMat => screenMat;

	private void Start()
	{
		screenMat.SetVector("_Center", new Vector4(movingCenter.x, movingCenter.y, 0f, 0f));
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (screenMat != null)
		{
			Graphics.Blit(src, dst, screenMat);
		}
	}
}
