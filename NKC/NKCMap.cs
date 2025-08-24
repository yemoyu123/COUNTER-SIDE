using System;
using System.Collections.Generic;
using NKM;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace NKC;

public class NKCMap
{
	private NKMMapTemplet m_NKMMapTemplet;

	private NKCAssetInstanceData m_MapObject;

	private SpriteRenderer[] m_MapObject_SpriteRenderer;

	private MeshRenderer[] m_MapObject_m_MeshRenderer;

	private MaterialPropertyBlock m_MPB = new MaterialPropertyBlock();

	private Color m_ColorMeshRenderer = new Color(1f, 1f, 1f, 1f);

	private float m_fMapColorROrg;

	private float m_fMapColorGOrg;

	private float m_fMapColorBOrg;

	private float m_fMapColorRNow;

	private float m_fMapColorGNow;

	private float m_fMapColorBNow;

	private float[] m_MapColorROrg;

	private float[] m_MapColorGOrg;

	private float[] m_MapColorBOrg;

	private float[] m_MapColorRNow;

	private float[] m_MapColorGNow;

	private float[] m_MapColorBNow;

	private NKCAssetInstanceData m_MapInvalidLand;

	private SpriteRenderer m_MapInvalidLand_SpriteRenderer;

	private NKMTrackingFloat m_MapInvalidLandWidth = new NKMTrackingFloat();

	private NKMTrackingFloat m_MapInvalidLandAlpha = new NKMTrackingFloat();

	private float m_fCustomWidth;

	private float m_fMapColorKeepTime;

	private float m_fMapColorReturnTime;

	private float m_fDeltaTime;

	private bool m_bObjectActive = true;

	private bool m_bHideObjectThisFrame;

	private Color m_ColorTemp;

	private Vector3 m_Vec3Temp;

	private Vector2 m_Vec2Temp;

	private Dictionary<int, NKCASLensFlare> m_dicLensFlare = new Dictionary<int, NKCASLensFlare>();

	private Dictionary<int, GameObject> m_dicMapLayer = new Dictionary<int, GameObject>();

	public SpriteRenderer GetInvalidLandRenderer()
	{
		return m_MapInvalidLand_SpriteRenderer;
	}

	public NKCMap()
	{
		Init();
	}

	public void Init()
	{
		m_NKMMapTemplet = null;
		if (m_MapObject_m_MeshRenderer != null)
		{
			for (int i = 0; i < m_MapObject_m_MeshRenderer.Length; i++)
			{
				if (m_MapObject_m_MeshRenderer[i] != null)
				{
					m_MapObject_m_MeshRenderer[i].SetPropertyBlock(null);
				}
			}
		}
		m_MapObject_m_MeshRenderer = null;
		NKCAssetResourceManager.CloseInstance(m_MapObject);
		m_MapObject = null;
		m_MapObject_SpriteRenderer = null;
		m_fMapColorROrg = 1f;
		m_fMapColorGOrg = 1f;
		m_fMapColorBOrg = 1f;
		m_fMapColorRNow = 1f;
		m_fMapColorGNow = 1f;
		m_fMapColorBNow = 1f;
		m_MapColorROrg = null;
		m_MapColorGOrg = null;
		m_MapColorBOrg = null;
		m_MapColorRNow = null;
		m_MapColorGNow = null;
		m_MapColorBNow = null;
		m_fMapColorKeepTime = 0f;
		m_fMapColorReturnTime = 0f;
		m_fDeltaTime = 0f;
		NKCAssetResourceManager.CloseInstance(m_MapInvalidLand);
		m_MapInvalidLand = null;
		m_MapInvalidLand_SpriteRenderer = null;
		m_MapInvalidLandWidth.SetNowValue(0f);
		m_MapInvalidLandAlpha.SetNowValue(0f);
		m_fCustomWidth = 0f;
		ClearLensFlares();
		m_dicMapLayer.Clear();
	}

	public void Load(int mapID, bool bAsync = true)
	{
		Init();
		m_NKMMapTemplet = NKMMapManager.GetMapTempletByID(mapID);
		m_MapObject = NKCAssetResourceManager.OpenInstance<GameObject>(m_NKMMapTemplet.m_MapAssetName, bAsync);
		for (int i = 0; i < m_NKMMapTemplet.m_listBloomPoint.Count; i++)
		{
			NKMBloomPoint nKMBloomPoint = m_NKMMapTemplet.m_listBloomPoint[i];
			if (nKMBloomPoint.m_LensFlareName.m_AssetName.Length > 1)
			{
				NKCASLensFlare value = (NKCASLensFlare)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASLensFlare, nKMBloomPoint.m_LensFlareName.m_BundleName, nKMBloomPoint.m_LensFlareName.m_AssetName, bAsync);
				m_dicLensFlare.Add(i, value);
			}
		}
		m_MapInvalidLand = NKCAssetResourceManager.OpenInstance<GameObject>("AB_MAP_RESPAWN_INVALID_LAND", "AB_MAP_RESPAWN_INVALID_LAND", bAsync);
	}

	public bool LoadComplete()
	{
		if (!m_MapObject.m_Instant.activeSelf)
		{
			m_MapObject.m_Instant.SetActive(value: true);
		}
		m_MapObject.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
			.Get_GAME_BATTLE_MAP()
			.transform, worldPositionStays: false);
			NKCUtil.SetGameObjectLocalPos(m_MapObject.m_Instant, m_NKMMapTemplet.m_fInitPosX, 0f, 0f);
			m_MapObject_SpriteRenderer = m_MapObject.m_Instant.GetComponentsInChildren<SpriteRenderer>();
			m_MapObject_m_MeshRenderer = m_MapObject.m_Instant.GetComponentsInChildren<MeshRenderer>(includeInactive: true);
			m_MapColorROrg = new float[m_MapObject_SpriteRenderer.Length];
			m_MapColorGOrg = new float[m_MapObject_SpriteRenderer.Length];
			m_MapColorBOrg = new float[m_MapObject_SpriteRenderer.Length];
			m_MapColorRNow = new float[m_MapObject_SpriteRenderer.Length];
			m_MapColorGNow = new float[m_MapObject_SpriteRenderer.Length];
			m_MapColorBNow = new float[m_MapObject_SpriteRenderer.Length];
			for (int i = 0; i < m_MapObject_SpriteRenderer.Length; i++)
			{
				m_MapColorROrg[i] = m_MapObject_SpriteRenderer[i].color.r;
				m_MapColorGOrg[i] = m_MapObject_SpriteRenderer[i].color.g;
				m_MapColorBOrg[i] = m_MapObject_SpriteRenderer[i].color.b;
				m_MapColorRNow[i] = m_MapObject_SpriteRenderer[i].color.r;
				m_MapColorGNow[i] = m_MapObject_SpriteRenderer[i].color.g;
				m_MapColorBNow[i] = m_MapObject_SpriteRenderer[i].color.b;
			}
			for (int j = 0; j < m_NKMMapTemplet.m_listBloomPoint.Count; j++)
			{
				NKMBloomPoint nKMBloomPoint = m_NKMMapTemplet.m_listBloomPoint[j];
				if (nKMBloomPoint.m_LensFlareName.m_AssetName.Length > 1)
				{
					m_dicLensFlare[j].SetPos(nKMBloomPoint.m_fBloomPointX, nKMBloomPoint.m_fBloomPointY);
				}
			}
			for (int k = 0; k < m_NKMMapTemplet.m_listMapLayer.Count; k++)
			{
				NKMMapLayer nKMMapLayer = m_NKMMapTemplet.m_listMapLayer[k];
				Transform transform = m_MapObject.m_Instant.transform.Find(nKMMapLayer.m_LayerName);
				if (!(transform == null))
				{
					m_Vec3Temp = transform.localPosition;
					m_Vec3Temp.Set(0f, 0f, 0f);
					transform.localPosition = m_Vec3Temp;
					m_dicMapLayer.Add(k, transform.gameObject);
				}
			}
			m_MapInvalidLandWidth.SetNowValue(0.6f);
			m_MapInvalidLand_SpriteRenderer = m_MapInvalidLand.m_Instant.GetComponentInChildren<SpriteRenderer>();
			m_MapInvalidLand.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
				.Get_GAME_BATTLE_MAP()
				.transform, worldPositionStays: false);
				NKCUtil.SetGameObjectLocalPos(m_MapInvalidLand.m_Instant, 0f, 0f, 0f);
				if (m_MapInvalidLand.m_Instant.activeSelf)
				{
					m_MapInvalidLand.m_Instant.SetActive(value: false);
				}
				return true;
			}

			public void ClearLensFlares()
			{
				Dictionary<int, NKCASLensFlare>.Enumerator enumerator = m_dicLensFlare.GetEnumerator();
				while (enumerator.MoveNext())
				{
					NKCASLensFlare value = enumerator.Current.Value;
					NKCScenManager.GetScenManager().GetObjectPool().CloseObj(value);
				}
				m_dicLensFlare.Clear();
			}

			public void Update(float fDeltaTime)
			{
				if (m_MapObject == null || m_MapObject_SpriteRenderer == null)
				{
					return;
				}
				m_fDeltaTime = fDeltaTime;
				if (m_fMapColorKeepTime > 0f)
				{
					m_fMapColorKeepTime -= m_fDeltaTime;
					if (m_fMapColorKeepTime < 0f)
					{
						m_fMapColorKeepTime = 0f;
					}
				}
				else
				{
					if (!m_bObjectActive && !m_bHideObjectThisFrame)
					{
						SetActiveMap(bActive: true);
					}
					if (m_fMapColorReturnTime > 0f)
					{
						m_fMapColorReturnTime -= m_fDeltaTime;
						if (m_fMapColorReturnTime < 0f)
						{
							m_fMapColorReturnTime = 0f;
						}
						ColorProcess();
					}
				}
				for (int i = 0; i < m_MapObject_SpriteRenderer.Length; i++)
				{
					Color color = m_MapObject_SpriteRenderer[i].color;
					color.r = m_MapColorRNow[i];
					color.g = m_MapColorGNow[i];
					color.b = m_MapColorBNow[i];
					m_MapObject_SpriteRenderer[i].color = color;
				}
				SetColorMeshRenderer(m_fMapColorRNow, m_fMapColorGNow, m_fMapColorBNow);
				UpdateLayer();
				UpdateInvalidLand();
				m_bHideObjectThisFrame = false;
			}

			public void UpdateLayer()
			{
				float posNowX = NKCCamera.GetPosNowX();
				foreach (KeyValuePair<int, GameObject> item in m_dicMapLayer)
				{
					NKMMapLayer nKMMapLayer = m_NKMMapTemplet.m_listMapLayer[item.Key];
					GameObject value = item.Value;
					m_Vec3Temp = value.transform.localPosition;
					m_Vec3Temp.x = posNowX * nKMMapLayer.m_fMoveFactor;
					value.transform.localPosition = m_Vec3Temp;
				}
			}

			public void UpdateInvalidLand()
			{
				m_MapInvalidLandWidth.Update(m_fDeltaTime);
				if (m_MapInvalidLandWidth.IsTracking())
				{
					SetRespawnValidLandAlpha(2f);
				}
				m_MapInvalidLandAlpha.Update(m_fDeltaTime);
				if (m_MapInvalidLandAlpha.GetNowValue() > 0f)
				{
					float num = m_MapInvalidLandWidth.GetNowValue();
					if (m_fCustomWidth > 0f)
					{
						num = m_fCustomWidth;
					}
					if (!m_MapInvalidLand.m_Instant.activeSelf)
					{
						m_MapInvalidLand.m_Instant.SetActive(value: true);
					}
					m_Vec2Temp.x = (m_NKMMapTemplet.m_fMaxX - m_NKMMapTemplet.m_fMinX) * num;
					m_Vec2Temp.y = m_NKMMapTemplet.m_fMaxZ - m_NKMMapTemplet.m_fMinZ;
					m_Vec2Temp.x *= 0.01f;
					m_Vec2Temp.y *= 0.01f;
					m_MapInvalidLand_SpriteRenderer.size = m_Vec2Temp;
					m_Vec3Temp = m_MapInvalidLand.m_Instant.transform.localPosition;
					m_Vec3Temp.y = m_NKMMapTemplet.m_fMinZ + (m_NKMMapTemplet.m_fMaxZ - m_NKMMapTemplet.m_fMinZ) * 0.5f;
					m_Vec3Temp.x = m_NKMMapTemplet.m_fMaxX - (m_NKMMapTemplet.m_fMaxX - m_NKMMapTemplet.m_fMinX) * num * 0.5f;
					m_MapInvalidLand.m_Instant.transform.localPosition = m_Vec3Temp;
					m_ColorTemp = m_MapInvalidLand_SpriteRenderer.color;
					m_ColorTemp.a = m_MapInvalidLandAlpha.GetNowValue();
					m_MapInvalidLand_SpriteRenderer.color = m_ColorTemp;
				}
				else
				{
					if (m_MapInvalidLand.m_Instant.activeSelf)
					{
						m_MapInvalidLand.m_Instant.SetActive(value: false);
					}
					m_fCustomWidth = 0f;
				}
			}

			public void SetRespawnValidLandFactor(float fFactor, bool bTracking)
			{
				if (bTracking)
				{
					m_MapInvalidLandWidth.SetTracking(1f - fFactor, 2f, TRACKING_DATA_TYPE.TDT_SLOWER);
				}
				else
				{
					m_MapInvalidLandWidth.SetNowValue(1f - fFactor);
				}
			}

			public void SetRespawnValidLandAlpha(float fTrackingTime, float fCustomWidth = 0f)
			{
				m_MapInvalidLandAlpha.SetNowValue(1f);
				m_MapInvalidLandAlpha.SetTracking(0f, fTrackingTime, TRACKING_DATA_TYPE.TDT_SLOWER);
				if (!m_MapInvalidLand.m_Instant.activeSelf)
				{
					m_MapInvalidLand.m_Instant.SetActive(value: true);
				}
				m_fCustomWidth = fCustomWidth;
			}

			private void ColorProcess()
			{
				for (int i = 0; i < m_MapObject_SpriteRenderer.Length; i++)
				{
					ColorProcess(i);
				}
				ColorProcess(ref m_fMapColorROrg, ref m_fMapColorRNow);
				ColorProcess(ref m_fMapColorGOrg, ref m_fMapColorGNow);
				ColorProcess(ref m_fMapColorBOrg, ref m_fMapColorBNow);
			}

			private void ColorProcess(int index)
			{
				ColorProcess(ref m_MapColorROrg[index], ref m_MapColorRNow[index]);
				ColorProcess(ref m_MapColorGOrg[index], ref m_MapColorGNow[index]);
				ColorProcess(ref m_MapColorBOrg[index], ref m_MapColorBNow[index]);
				ColorProcess(ref m_fMapColorROrg, ref m_fMapColorRNow);
				ColorProcess(ref m_fMapColorGOrg, ref m_fMapColorGNow);
				ColorProcess(ref m_fMapColorBOrg, ref m_fMapColorBNow);
			}

			private void ColorProcess(ref float colorOrg, ref float colorNow)
			{
				if (colorOrg != colorNow)
				{
					if (m_fMapColorReturnTime <= 0f)
					{
						colorNow = colorOrg;
						return;
					}
					float num = colorOrg - colorNow;
					num /= m_fMapColorReturnTime;
					colorNow += num * m_fDeltaTime;
				}
			}

			public void FadeColor(float fR, float fG, float fB, float fMapColorKeepTime, float fMapColorReturnTime, bool bHideObject)
			{
				for (int i = 0; i < m_MapObject_SpriteRenderer.Length; i++)
				{
					m_MapColorRNow[i] = fR;
					m_MapColorGNow[i] = fG;
					m_MapColorBNow[i] = fB;
				}
				m_fMapColorRNow = fR;
				m_fMapColorGNow = fG;
				m_fMapColorBNow = fB;
				m_fMapColorKeepTime = fMapColorKeepTime;
				m_fMapColorReturnTime = fMapColorReturnTime;
				SetActiveMap(!bHideObject);
			}

			public void SetColorMeshRenderer(float fR, float fG, float fB, float fA = -1f)
			{
				bool flag = false;
				if (m_ColorMeshRenderer.r != fR)
				{
					m_ColorMeshRenderer.r = fR;
					flag = true;
				}
				if (m_ColorMeshRenderer.g != fG)
				{
					m_ColorMeshRenderer.g = fG;
					flag = true;
				}
				if (m_ColorMeshRenderer.b != fB)
				{
					m_ColorMeshRenderer.b = fB;
					flag = true;
				}
				if (fA != -1f && m_ColorMeshRenderer.a != fA)
				{
					m_ColorMeshRenderer.a = fA;
					flag = true;
				}
				if (!flag)
				{
					return;
				}
				if (m_ColorMeshRenderer.r == 1f && m_ColorMeshRenderer.g == 1f && m_ColorMeshRenderer.b == 1f && m_ColorMeshRenderer.a == 1f)
				{
					for (int i = 0; i < m_MapObject_m_MeshRenderer.Length; i++)
					{
						m_MapObject_m_MeshRenderer[i].SetPropertyBlock(null);
					}
					return;
				}
				m_MPB.SetColor(Shader.PropertyToID("_Color"), m_ColorMeshRenderer);
				for (int j = 0; j < m_MapObject_m_MeshRenderer.Length; j++)
				{
					m_MapObject_m_MeshRenderer[j].SetPropertyBlock(m_MPB);
				}
			}

			public float GetMapBright()
			{
				float num = (m_fMapColorRNow + m_fMapColorGNow + m_fMapColorBNow) / 3f;
				float num2 = (m_fMapColorROrg + m_fMapColorGOrg + m_fMapColorBOrg) / 3f;
				return num / num2;
			}

			public NKCASLensFlare GetLensFlare(int index)
			{
				if (m_dicLensFlare.ContainsKey(index))
				{
					return m_dicLensFlare[index];
				}
				return null;
			}

			public void SetActiveMap(bool bActive)
			{
				if (m_MapObject != null)
				{
					NKCUtil.SetGameobjectActive(m_MapObject.m_Instant, bActive);
					m_bObjectActive = bActive;
					if (!bActive)
					{
						m_bHideObjectThisFrame = true;
					}
				}
			}

			public void PlayMapSpineAnimation(string animNames)
			{
				if (m_MapObject == null || m_MapObject.m_Instant == null)
				{
					return;
				}
				SkeletonAnimation[] componentsInChildren = m_MapObject.m_Instant.GetComponentsInChildren<SkeletonAnimation>();
				if (componentsInChildren == null)
				{
					return;
				}
				string[] array = animNames.Split(new char[2] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
				SkeletonAnimation[] array2 = componentsInChildren;
				foreach (SkeletonAnimation skeletonAnimation in array2)
				{
					if (skeletonAnimation.SkeletonDataAsset == null)
					{
						continue;
					}
					for (int j = 0; j < array.Length; j++)
					{
						bool loop = j == array.Length - 1;
						string text = array[j];
						Spine.Animation animation = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(quiet: false).FindAnimation(text);
						if (animation != null)
						{
							if (j == 0)
							{
								if (animation != null)
								{
									skeletonAnimation.state.SetAnimation(0, animation, loop);
								}
							}
							else if (animation != null)
							{
								skeletonAnimation.state.AddAnimation(0, animation, loop, 0f);
							}
						}
						else
						{
							Debug.LogWarning($"animName {text} not found in SkeletionAnimation {skeletonAnimation.gameObject} ");
						}
					}
				}
			}

			public void PlayMapSpineAnimationLastOnly(string animNames)
			{
				if (m_MapObject == null || m_MapObject.m_Instant == null)
				{
					return;
				}
				SkeletonAnimation[] componentsInChildren = m_MapObject.m_Instant.GetComponentsInChildren<SkeletonAnimation>();
				if (componentsInChildren == null)
				{
					return;
				}
				string[] array = animNames.Split(new char[2] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length == 0)
				{
					return;
				}
				SkeletonAnimation[] array2 = componentsInChildren;
				foreach (SkeletonAnimation skeletonAnimation in array2)
				{
					if (!(skeletonAnimation.SkeletonDataAsset == null))
					{
						string animationName = array[array.Length - 1];
						Spine.Animation animation = skeletonAnimation.SkeletonDataAsset.GetSkeletonData(quiet: false).FindAnimation(animationName);
						if (animation != null)
						{
							skeletonAnimation.state.SetAnimation(0, animation, loop: true);
						}
					}
				}
			}
		}
