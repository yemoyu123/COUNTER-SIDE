using System.Collections;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCComSoundPlayer : MonoBehaviour
{
	public string AssetName;

	[Range(0f, 1f)]
	public float Volume = 1f;

	public bool PlayOnEnable = true;

	public float Delay;

	public float PlayStartPos;

	public bool Loop;

	public bool StopOnDisable;

	private int soundID;

	private void OnEnable()
	{
		if (PlayOnEnable)
		{
			if (Delay > 0f)
			{
				StartCoroutine(PlayCoroutine());
			}
			else
			{
				Play();
			}
		}
	}

	private void OnDisable()
	{
		if (Loop || StopOnDisable)
		{
			NKCSoundManager.StopSound(soundID);
		}
	}

	public void Preload(bool bAsync)
	{
		if (AssetName.Contains("@"))
		{
			NKCResourceUtility.LoadAssetResourceTemp<AudioClip>(NKMAssetName.ParseBundleName("AB_SOUND", AssetName), bAsync);
		}
		else
		{
			NKCResourceUtility.LoadAssetResourceTemp<AudioClip>(AssetName, bAsync);
		}
	}

	public void Play()
	{
		if (NKCSoundManager.IsInit() && !string.IsNullOrWhiteSpace(AssetName))
		{
			if (AssetName.Contains("@"))
			{
				NKMAssetName assetName = NKMAssetName.ParseBundleName("AB_SOUND", AssetName);
				soundID = NKCSoundManager.PlaySound(assetName, Volume, 0f, 0f, Loop, 0f, bShowCaption: false, PlayStartPos);
			}
			else
			{
				soundID = NKCSoundManager.PlaySound(AssetName, Volume, 0f, 0f, Loop, 0f, bShowCaption: false, PlayStartPos);
			}
		}
	}

	private IEnumerator PlayCoroutine()
	{
		yield return new WaitForSeconds(Delay);
		Play();
	}
}
