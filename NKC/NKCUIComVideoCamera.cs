using UnityEngine;
using UnityEngine.Video;

namespace NKC;

[RequireComponent(typeof(Camera))]
public class NKCUIComVideoCamera : NKCUIComVideoPlayer
{
	public VideoRenderMode renderMode
	{
		get
		{
			return base.VideoPlayer.renderMode;
		}
		set
		{
			if ((uint)value <= 1u)
			{
				base.VideoPlayer.renderMode = value;
			}
			else
			{
				Debug.Log("not allowed VideoRendertype");
			}
		}
	}

	private void Awake()
	{
		base.VideoPlayer.targetCamera = GetComponent<Camera>();
		base.VideoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
	}

	protected override bool CanPlayVideo()
	{
		if (base.VideoPlayer.targetCamera != null && !base.VideoPlayer.targetCamera.isActiveAndEnabled)
		{
			return false;
		}
		return base.CanPlayVideo();
	}

	private void OnDestroy()
	{
		CleanUp();
	}
}
