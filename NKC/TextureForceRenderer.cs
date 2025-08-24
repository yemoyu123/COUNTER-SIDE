using UnityEngine;

namespace NKC;

public class TextureForceRenderer : MonoBehaviour
{
	public Texture m_tex;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnPreRender()
	{
		if (m_tex != null)
		{
			Camera camera = NKCCamera.GetCamera();
			if (camera != null)
			{
				camera.clearFlags = CameraClearFlags.Nothing;
				Graphics.DrawTexture(new Rect
				{
					width = Screen.width,
					height = Screen.height,
					center = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f)
				}, m_tex);
			}
		}
	}
}
