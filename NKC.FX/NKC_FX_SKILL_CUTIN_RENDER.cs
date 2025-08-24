using System;
using UnityEngine;

namespace NKC.FX;

public class NKC_FX_SKILL_CUTIN_RENDER : MonoBehaviour
{
	[Serializable]
	public class SkillCutinRender
	{
		public Camera m_Camara;

		public RenderTexture m_RendTexture;
	}

	public SkillCutinRender[] RenderCam;

	private void OnDestroy()
	{
		if (RenderCam != null)
		{
			if (RenderCam.Length != 0)
			{
				ReleaseRender();
				ClearRender();
			}
			RenderCam = null;
		}
	}

	private void OnEnable()
	{
		if (RenderCam.Length != 0)
		{
			PrepareRender();
		}
	}

	private void OnDisable()
	{
		if (RenderCam.Length != 0)
		{
			ReleaseRender();
		}
	}

	public void PrepareRender()
	{
		for (int i = 0; i < RenderCam.Length; i++)
		{
			if (RenderCam[i].m_Camara != null && (bool)RenderCam[i].m_RendTexture)
			{
				RenderCam[i].m_Camara.enabled = true;
				if (RenderCam[i].m_Camara.targetTexture == null)
				{
					RenderCam[i].m_Camara.targetTexture = RenderCam[i].m_RendTexture;
				}
			}
		}
	}

	public void ReleaseRender()
	{
		for (int i = 0; i < RenderCam.Length; i++)
		{
			if (RenderCam[i].m_Camara != null && (bool)RenderCam[i].m_RendTexture)
			{
				RenderCam[i].m_Camara.enabled = false;
				if (RenderCam[i].m_Camara.targetTexture != null)
				{
					RenderCam[i].m_Camara.targetTexture = null;
				}
			}
		}
	}

	public void ClearRender()
	{
		for (int i = 0; i < RenderCam.Length; i++)
		{
			if (RenderCam[i].m_Camara != null && (bool)RenderCam[i].m_RendTexture)
			{
				RenderCam[i].m_Camara.enabled = false;
				if ((bool)RenderCam[i].m_RendTexture)
				{
					RenderCam[i].m_RendTexture.Release();
				}
			}
		}
	}
}
