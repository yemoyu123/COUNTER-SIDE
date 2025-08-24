using UnityEngine;

namespace NKC;

public class NKCComAudioPlayer : MonoBehaviour
{
	[Header("Only Play On Enable")]
	public AudioClip AudioClip;

	[Range(0f, 1f)]
	public float Volume = 1f;

	private void OnEnable()
	{
		AudioSource audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
		{
			audioSource = base.gameObject.AddComponent<AudioSource>();
			audioSource.clip = AudioClip;
		}
		audioSource.volume = NKCSoundManager.GetFinalVol(0f, 0f, Volume);
		audioSource.Play();
	}
}
