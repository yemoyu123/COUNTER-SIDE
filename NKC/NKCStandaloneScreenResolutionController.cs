using UnityEngine;

namespace NKC;

public class NKCStandaloneScreenResolutionController : MonoBehaviour
{
	private int fullScreenWidth;

	private int fullScreenHeight;

	private bool isFullScreen;

	private void Awake()
	{
		isFullScreen = Screen.fullScreen;
		fullScreenWidth = Screen.currentResolution.width;
		fullScreenHeight = Screen.currentResolution.height;
	}

	private void Update()
	{
		if (isFullScreen != Screen.fullScreen)
		{
			isFullScreen = Screen.fullScreen;
			if (isFullScreen)
			{
				Screen.SetResolution(fullScreenWidth, fullScreenHeight, isFullScreen);
				return;
			}
			fullScreenWidth = Screen.currentResolution.width;
			fullScreenHeight = Screen.currentResolution.height;
		}
	}
}
