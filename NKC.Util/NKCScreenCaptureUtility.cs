using System;
using System.IO;
using UnityEngine;

namespace NKC.Util;

public static class NKCScreenCaptureUtility
{
	private const string SCREENSHOT_DIR = "ScreenShot/";

	private const string FILEPATH_PREFIX = "CounterSide-";

	private const string FILENAME_DATE_FORMAT = "yyyy-MM-dd HH-mm-ss";

	private const string FILE_EXTENSION = ".png";

	public static bool CaptureScreen()
	{
		if (!Directory.Exists("ScreenShot/"))
		{
			Directory.CreateDirectory("ScreenShot/");
		}
		string text = MakeCaptureFileName();
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		ScreenCapture.CaptureScreenshot(text);
		Debug.Log("Screencapture : " + text);
		return true;
	}

	public static bool CaptureCamera(Camera camera, string path, int width, int height)
	{
		try
		{
			RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
			RenderTexture targetTexture = camera.targetTexture;
			camera.targetTexture = renderTexture;
			camera.Render();
			camera.targetTexture = targetTexture;
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = renderTexture;
			Texture2D texture2D = new Texture2D(width, height);
			texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
			texture2D.Apply();
			RenderTexture.active = active;
			byte[] bytes = texture2D.EncodeToPNG();
			File.WriteAllBytes(path, bytes);
		}
		catch (Exception ex)
		{
			Debug.LogError("CaptureCamera Failed : Exception " + ex.Message + "\n" + ex.StackTrace);
			return false;
		}
		return true;
	}

	private static string MakeCaptureFileName()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo("ScreenShot/");
		if (directoryInfo == null || !directoryInfo.Exists)
		{
			return null;
		}
		string text = directoryInfo.FullName + "CounterSide-" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
		string text2 = text + ".png";
		if (!File.Exists(text2))
		{
			return text2;
		}
		int num = 60;
		for (int i = 1; i < num; i++)
		{
			text2 = text + $" ({i})" + ".png";
			if (!File.Exists(text2))
			{
				return text2;
			}
		}
		return null;
	}

	public static bool CaptureScreenWithThumbnail(string capturePath, string thumbnailPath)
	{
		Texture2D texture2D = CaptureScreenTexture();
		if (texture2D == null)
		{
			return false;
		}
		byte[] bytes = texture2D.EncodeToPNG();
		File.WriteAllBytes(capturePath, bytes);
		Debug.Log("Screenshot Saved in : " + capturePath);
		if (!string.IsNullOrEmpty(thumbnailPath))
		{
			byte[] bytes2 = MakeThumbnailTexture(texture2D, 120).EncodeToPNG();
			File.WriteAllBytes(thumbnailPath, bytes2);
			Debug.Log("Thumbnail Saved in : " + thumbnailPath);
		}
		return true;
	}

	private static Texture2D CaptureScreenTexture()
	{
		return ScreenCapture.CaptureScreenshotAsTexture();
	}

	private static Texture2D MakeThumbnailTexture(Texture2D source, int thumbnailSize)
	{
		int num;
		int num2;
		int num3;
		int num4;
		if (source.width < source.height)
		{
			num = 0;
			num2 = source.width;
			num3 = (source.height - source.width) / 2;
			num4 = (source.height + source.width) / 2;
		}
		else if (source.width > source.height)
		{
			num = (source.width - source.height) / 2;
			num2 = (source.width + source.height) / 2;
			num3 = 0;
			num4 = source.height;
		}
		else
		{
			num = 0;
			num2 = source.width;
			num3 = 0;
			num4 = source.height;
		}
		TextureFormat textureFormat = TextureFormat.RGB24;
		Texture2D texture2D = new Texture2D(thumbnailSize, thumbnailSize, textureFormat, mipChain: false);
		int num5 = num2 - num;
		float num6 = (float)num / (float)source.width;
		float num7 = (float)num5 / (float)(thumbnailSize * source.width);
		int num8 = num4 - num3;
		float num9 = (float)num3 / (float)source.height;
		float num10 = (float)num8 / (float)(thumbnailSize * source.height);
		Color[] pixels = texture2D.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			float u = num6 + num7 * (float)(i % thumbnailSize);
			float v = num9 + num10 * (float)(i / thumbnailSize);
			pixels[i] = source.GetPixelBilinear(u, v);
		}
		texture2D.SetPixels(pixels, 0);
		texture2D.Apply();
		return texture2D;
	}
}
