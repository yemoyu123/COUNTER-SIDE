using System.Collections.Generic;
using UnityEngine;

namespace NKC;

public class NKC2DMotionAfterImage
{
	private class MotionAfterImageSingleFrame
	{
		private MeshRenderer m_Renderer;

		private RenderTexture m_Texture;

		private GameObject m_GameObject;

		private float m_fRemainAlpha;

		private float m_fLifeTime;

		private Color m_BlurColor = new Color(1f, 1f, 1f, 1f);

		private const float zSpacing = 0.1f;

		public RenderTexture Texture => m_Texture;

		public bool IsAlive()
		{
			if (!(m_fRemainAlpha > 0f) || !(m_fLifeTime > 0f))
			{
				return m_GameObject.activeSelf;
			}
			return true;
		}

		public MotionAfterImageSingleFrame(Material mat)
		{
			m_GameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
			m_GameObject.name = "MotionAfterImage";
			m_GameObject.SetActive(value: false);
			if (NKCScenManager.GetScenManager() != null)
			{
				m_GameObject.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_GAME_BATTLE_UNIT_MOTION_BLUR()
					.transform, worldPositionStays: false);
				}
				m_Renderer = m_GameObject.GetComponent<MeshRenderer>();
				m_Renderer.material = mat;
				m_Texture = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
				m_Texture.wrapMode = TextureWrapMode.Clamp;
				m_Texture.antiAliasing = 1;
				m_Texture.filterMode = FilterMode.Bilinear;
				m_Texture.anisoLevel = 0;
				m_Texture.Create();
				m_Renderer.material.mainTexture = m_Texture;
			}

			public void Update(float fDeltaTime, float fAlphaSpeed)
			{
				if (m_Texture == null || m_GameObject == null || !m_GameObject.activeSelf)
				{
					return;
				}
				m_fLifeTime -= fDeltaTime;
				if (m_fLifeTime <= 0f)
				{
					m_fLifeTime = 0f;
					SetActive(value: false);
				}
				if (m_fRemainAlpha > 0f)
				{
					m_fRemainAlpha -= fAlphaSpeed * fDeltaTime;
					m_BlurColor.a = m_fRemainAlpha;
					SetColor(m_BlurColor);
					if (m_fRemainAlpha <= 0f)
					{
						m_fRemainAlpha = 0f;
						SetActive(value: false);
					}
				}
			}

			public void SetFrame(Renderer targetRenderer, Color color, float lifeTime)
			{
				Bounds bounds = targetRenderer.bounds;
				NKCScenManager.GetScenManager().TextureCapture(targetRenderer, bounds, ref m_Texture);
				m_fLifeTime = lifeTime;
				m_fRemainAlpha = color.a;
				m_BlurColor = color;
				SetColor(color);
				m_GameObject.transform.localScale = new Vector3(bounds.size.x, bounds.size.y);
				m_GameObject.transform.position = bounds.center;
				m_Renderer.sortingLayerID = targetRenderer.sortingLayerID;
				m_Renderer.sortingOrder = targetRenderer.sortingOrder - 1;
				SetActive(value: true);
			}

			public void SetActive(bool value)
			{
				if (m_GameObject != null && m_GameObject.activeSelf != value)
				{
					m_GameObject.SetActive(value);
				}
			}

			public void Delete()
			{
				Object.Destroy(m_GameObject);
				if (m_Texture != null)
				{
					m_Texture.Release();
					Object.DestroyImmediate(m_Texture);
				}
				m_Texture = null;
			}

			public void SetColor(Color color)
			{
				m_Renderer.material.color = color;
			}

			public void SetColor(float fR, float fG, float fB, float fA)
			{
				Color color = new Color(fR, fG, fB, fA);
				m_Renderer.material.color = color;
			}
		}

		private static Material DrawMaterial;

		private static Stack<MotionAfterImageSingleFrame> s_stkFrame = new Stack<MotionAfterImageSingleFrame>();

		private const int AfterImageLayer = 31;

		private Queue<MotionAfterImageSingleFrame> m_qFrames = new Queue<MotionAfterImageSingleFrame>();

		private bool m_bEnable;

		private bool m_bOnProcess;

		private float m_fGapTime = 0.1f;

		private float m_fFadeSpeed = 1.5f;

		private float m_fLifeTime = 1f;

		private Color m_fColor;

		private Renderer m_rendererTarget;

		private float m_fGapTimeNow = 0.1f;

		private int maxImageCount;

		public static void Init()
		{
			if (DrawMaterial == null)
			{
				DrawMaterial = NKCAssetResourceManager.OpenResource<Material>("AB_MATERIAL", "MAT_NKC_AFTERIMAGE").GetAsset<Material>();
			}
			if (s_stkFrame.Count == 0)
			{
				for (int i = 0; i < 25; i++)
				{
					s_stkFrame.Push(MakeFrame());
				}
			}
		}

		private static MotionAfterImageSingleFrame MakeFrame()
		{
			return new MotionAfterImageSingleFrame(DrawMaterial);
		}

		private static MotionAfterImageSingleFrame GetFrame()
		{
			if (s_stkFrame.Count > 0)
			{
				return s_stkFrame.Pop();
			}
			return null;
		}

		private static void ReturnFrame(MotionAfterImageSingleFrame frame)
		{
			frame.SetActive(value: false);
			s_stkFrame.Push(frame);
		}

		public static void CleanUp()
		{
			foreach (MotionAfterImageSingleFrame item in s_stkFrame)
			{
				item.Delete();
			}
			s_stkFrame.Clear();
		}

		public void ResetGapTime()
		{
			m_fGapTimeNow = 0f;
		}

		private MotionAfterImageSingleFrame GetOldestFrame()
		{
			if (m_qFrames.Count == 0)
			{
				return null;
			}
			return m_qFrames.Dequeue();
		}

		private void AddSingleFrame(Renderer targetRenderer, Color color)
		{
			if (!m_bEnable)
			{
				return;
			}
			MotionAfterImageSingleFrame motionAfterImageSingleFrame;
			if (m_qFrames.Count < maxImageCount)
			{
				motionAfterImageSingleFrame = GetFrame();
				if (motionAfterImageSingleFrame == null)
				{
					motionAfterImageSingleFrame = GetOldestFrame();
					if (motionAfterImageSingleFrame == null)
					{
						return;
					}
				}
				m_qFrames.Enqueue(motionAfterImageSingleFrame);
			}
			else
			{
				motionAfterImageSingleFrame = GetOldestFrame();
				if (motionAfterImageSingleFrame == null)
				{
					return;
				}
				m_qFrames.Enqueue(motionAfterImageSingleFrame);
			}
			motionAfterImageSingleFrame.SetFrame(targetRenderer, color, m_fLifeTime);
		}

		public void Init(int MaxImageCount, Renderer renderTarget)
		{
			Clear();
			maxImageCount = MaxImageCount;
			m_rendererTarget = renderTarget;
		}

		public void SetColor(Color color)
		{
			m_fColor = color;
		}

		public void SetGapTime(float time)
		{
			m_fGapTime = time;
		}

		public void SetFadeSpeed(float value)
		{
			m_fFadeSpeed = value;
		}

		public void SetMaxImageCount(int value)
		{
			maxImageCount = value;
		}

		public void SetLifeTime(float value)
		{
			m_fLifeTime = value;
		}

		public void SetColor(float fR, float fG, float fB, float fA)
		{
			m_fColor.r = fR;
			m_fColor.g = fG;
			m_fColor.b = fB;
			m_fColor.a = fA;
		}

		public void Clear()
		{
			m_bEnable = false;
			m_bOnProcess = false;
			foreach (MotionAfterImageSingleFrame qFrame in m_qFrames)
			{
				ReturnFrame(qFrame);
			}
			m_qFrames.Clear();
		}

		public void SetEnable(bool bEnable)
		{
			if (m_rendererTarget == null)
			{
				bEnable = false;
			}
			if (bEnable)
			{
				m_bOnProcess = true;
			}
			m_bEnable = bEnable;
		}

		public void StopMotionImage()
		{
			SetEnable(bEnable: false);
			Clear();
		}

		public void Update(float fDeltaTime)
		{
			if (!m_bOnProcess)
			{
				return;
			}
			if (!m_bEnable && m_qFrames.Count == 0)
			{
				Clear();
				return;
			}
			m_fGapTimeNow -= fDeltaTime;
			if (m_fGapTimeNow < 0f)
			{
				m_fGapTimeNow = m_fGapTime;
				AddSingleFrame(m_rendererTarget, m_fColor);
			}
			bool flag = false;
			foreach (MotionAfterImageSingleFrame qFrame in m_qFrames)
			{
				if (qFrame != null)
				{
					qFrame.Update(fDeltaTime, m_fFadeSpeed);
					if (!qFrame.IsAlive())
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				ReturnFrame(m_qFrames.Dequeue());
			}
		}
	}
