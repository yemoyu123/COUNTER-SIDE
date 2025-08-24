using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace NKC;

[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(RawImage))]
public class NKCUIComVideoTexture : NKCUIComVideoPlayer
{
	private RenderTexture m_RenderTexture;

	private RawImage m_rimgTarget;

	[Header("영상 로딩 중일때 보여줄 오브젝트")]
	public GameObject m_objLoading;

	[Header("영상이 재생되지 않는 동안(로딩중/재생실패) 보여줄 오브젝트")]
	public GameObject m_objFallback;

	private RawImage ImgTarget
	{
		get
		{
			if (m_rimgTarget == null)
			{
				m_rimgTarget = GetComponent<RawImage>();
			}
			return m_rimgTarget;
		}
	}

	private void Awake()
	{
		base.VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
	}

	private void OnApplicationQuit()
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		CleanUp();
	}

	public override void Prepare()
	{
		base.Prepare();
		PrepareTexture();
	}

	protected override void OnStateChange(VideoState state)
	{
		base.OnStateChange(state);
		switch (state)
		{
		case VideoState.Stop:
			ImgTarget.enabled = false;
			NKCUtil.SetGameobjectActive(m_objFallback, bValue: true);
			NKCUtil.SetGameobjectActive(m_objLoading, bValue: false);
			break;
		case VideoState.PreparingPlay:
			ImgTarget.enabled = false;
			NKCUtil.SetGameobjectActive(m_objFallback, bValue: true);
			NKCUtil.SetGameobjectActive(m_objLoading, bValue: true);
			break;
		case VideoState.Playing:
			ImgTarget.enabled = true;
			NKCUtil.SetGameobjectActive(m_objFallback, bValue: false);
			NKCUtil.SetGameobjectActive(m_objLoading, bValue: false);
			break;
		}
	}

	public void PrepareTexture()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null && !gameOptionData.UseVideoTexture)
		{
			ImgTarget.enabled = false;
			NKCUtil.SetGameobjectActive(m_objFallback, bValue: true);
			NKCUtil.SetGameobjectActive(m_objLoading, bValue: false);
			return;
		}
		if (m_RenderTexture == null)
		{
			RectTransform component = ImgTarget.GetComponent<RectTransform>();
			m_RenderTexture = new RenderTexture((int)component.GetWidth(), (int)component.GetHeight(), 0);
			m_RenderTexture.hideFlags = HideFlags.HideAndDontSave;
		}
		base.VideoPlayer.targetTexture = m_RenderTexture;
		ImgTarget.texture = m_RenderTexture;
	}

	public override void CleanUp()
	{
		base.CleanUp();
		if (m_rimgTarget != null)
		{
			m_rimgTarget.texture = null;
		}
		if (m_RenderTexture != null)
		{
			m_RenderTexture.Release();
			Object.DestroyImmediate(m_RenderTexture);
			m_RenderTexture = null;
		}
	}
}
