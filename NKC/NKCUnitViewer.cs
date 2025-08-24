using NKC.Game;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public class NKCUnitViewer : NKMObjectPoolData
{
	private GameObject m_Parent;

	private NKCASUnitViewerSpineSprite m_NKCASUnitViewerSpineSprite;

	private NKCASUnitRespawnTimer m_RespawnTimer;

	private GameObject m_UnitSpriteOrg;

	private NKCASUnitShadow m_NKCASUnitShadow;

	private NKMUnitTemplet m_UnitTemplet;

	private NKCAnimSpine m_AnimSpine = new NKCAnimSpine();

	private bool m_bRight;

	private float m_fPosX;

	private float m_fPosY;

	private float m_fPosZ;

	private float m_fScaleX;

	private float m_fScaleY;

	private bool m_bUseAirHigh;

	private bool m_bRespawnReady;

	private long m_UnitUID;

	private Vector3 m_Vec3Temp;

	public NKMUnitTemplet GetUnitTemplet()
	{
		return m_UnitTemplet;
	}

	public void SetRespawnReady(bool bRespawnReady)
	{
		m_bRespawnReady = bRespawnReady;
	}

	public bool GetRespawnReady()
	{
		return m_bRespawnReady;
	}

	public void SetUnitUID(long unitUID)
	{
		m_UnitUID = unitUID;
	}

	public long GetUnitUID()
	{
		return m_UnitUID;
	}

	public NKCUnitViewer()
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKCUnitViewer;
	}

	public override bool LoadComplete()
	{
		if (m_NKCASUnitViewerSpineSprite == null || m_NKCASUnitViewerSpineSprite.m_UnitSpineSpriteInstant == null || m_NKCASUnitShadow == null || m_NKCASUnitShadow.m_ShadowSpriteInstant == null || m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant == null)
		{
			return false;
		}
		m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
			.Get_GAME_BATTLE_UNIT_SHADOW()
			.transform, worldPositionStays: false);
			NKCUtil.SetGameObjectLocalPos(m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant, 0f, 0f, 0f);
			if (m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.activeSelf)
			{
				m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.SetActive(value: false);
			}
			return true;
		}

		public override void Open()
		{
		}

		public override void Close()
		{
			if (m_AnimSpine != null)
			{
				m_AnimSpine.ResetParticleSimulSpeedOrg();
			}
			Init();
		}

		public override void Unload()
		{
			if (m_NKCASUnitShadow != null)
			{
				m_NKCASUnitShadow.Unload();
				m_NKCASUnitShadow = null;
			}
			m_Parent = null;
			m_NKCASUnitViewerSpineSprite = null;
			m_UnitSpriteOrg = null;
			m_UnitTemplet = null;
			m_AnimSpine.Init();
			m_AnimSpine = null;
			Object.Destroy(m_RespawnTimer);
			m_RespawnTimer = null;
		}

		public void Init()
		{
			m_Parent = null;
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUnitViewerSpineSprite);
			m_NKCASUnitViewerSpineSprite = null;
			m_RespawnTimer = null;
			m_UnitSpriteOrg = null;
			m_UnitTemplet = null;
			m_AnimSpine.Init();
			m_bRight = true;
			m_fPosX = 0f;
			m_fPosY = 0f;
			m_fPosZ = 0f;
			m_fScaleX = 1f;
			m_fScaleY = 1f;
			m_bUseAirHigh = true;
			m_bRespawnReady = false;
			m_UnitUID = 0L;
		}

		public void LoadUnit(NKMUnitData unitData, bool bSub, GameObject parent = null, bool bAsync = false)
		{
			if (unitData == null)
			{
				return;
			}
			m_UnitTemplet = NKMUnitManager.GetUnitTemplet(unitData.m_UnitID);
			if (m_UnitTemplet != null)
			{
				m_Parent = parent;
				m_NKCASUnitViewerSpineSprite = OpenUnitViewerSpineSprite(unitData, bSub, bAsync);
				if (m_NKCASUnitViewerSpineSprite == null)
				{
					Debug.LogError("Unit Spine sprite null!!");
				}
				if (m_NKCASUnitShadow == null)
				{
					m_NKCASUnitShadow = (NKCASUnitShadow)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitShadow, "", "", bAsync);
				}
				_ = m_RespawnTimer == null;
				if (!bAsync)
				{
					LoadUnitComplete();
				}
			}
		}

		public static NKCASUnitViewerSpineSprite OpenUnitViewerSpineSprite(NKMUnitData unitData, bool bSub, bool bAsync)
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(unitData);
			if (skinTemplet != null)
			{
				return OpenUnitViewerSpineSprite(skinTemplet, bSub, bAsync);
			}
			return OpenUnitViewerSpineSprite(NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID), bSub, bAsync);
		}

		public static NKCASUnitViewerSpineSprite OpenUnitViewerSpineSprite(NKMUnitTempletBase unitTempletBase, bool bSub, bool bAsync)
		{
			if (unitTempletBase == null)
			{
				return null;
			}
			NKCASUnitViewerSpineSprite nKCASUnitViewerSpineSprite;
			if (bSub && !string.IsNullOrEmpty(unitTempletBase.m_SpriteNameSub))
			{
				nKCASUnitViewerSpineSprite = (NKCASUnitViewerSpineSprite)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitViewerSpineSprite, unitTempletBase.m_SpriteBundleNameSub, unitTempletBase.m_SpriteNameSub, bAsync);
				if (!string.IsNullOrEmpty(unitTempletBase.m_SpriteMaterialNameSub))
				{
					nKCASUnitViewerSpineSprite.SetReplaceMatResource(unitTempletBase.m_SpriteBundleNameSub, unitTempletBase.m_SpriteMaterialNameSub, bAsync);
				}
				else
				{
					nKCASUnitViewerSpineSprite.SetReplaceMatResource("", "", bAsync);
				}
			}
			else
			{
				nKCASUnitViewerSpineSprite = (NKCASUnitViewerSpineSprite)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitViewerSpineSprite, unitTempletBase.m_SpriteBundleName, unitTempletBase.m_SpriteName, bAsync);
				if (!string.IsNullOrEmpty(unitTempletBase.m_SpriteMaterialName))
				{
					nKCASUnitViewerSpineSprite.SetReplaceMatResource(unitTempletBase.m_SpriteBundleName, unitTempletBase.m_SpriteMaterialName, bAsync);
				}
				else
				{
					nKCASUnitViewerSpineSprite.SetReplaceMatResource("", "", bAsync);
				}
			}
			return nKCASUnitViewerSpineSprite;
		}

		public static NKCASUnitViewerSpineSprite OpenUnitViewerSpineSprite(NKMSkinTemplet skinTemplet, bool bSub, bool bAsync)
		{
			if (skinTemplet == null)
			{
				return null;
			}
			NKCASUnitViewerSpineSprite nKCASUnitViewerSpineSprite;
			if (bSub && !string.IsNullOrEmpty(skinTemplet.m_SpriteNameSub))
			{
				nKCASUnitViewerSpineSprite = (NKCASUnitViewerSpineSprite)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitViewerSpineSprite, skinTemplet.m_SpriteBundleNameSub, skinTemplet.m_SpriteNameSub, bAsync);
				if (!string.IsNullOrEmpty(skinTemplet.m_SpriteMaterialNameSub))
				{
					nKCASUnitViewerSpineSprite.SetReplaceMatResource(skinTemplet.m_SpriteBundleNameSub, skinTemplet.m_SpriteMaterialNameSub, bAsync);
				}
				else
				{
					nKCASUnitViewerSpineSprite.SetReplaceMatResource("", "", bAsync);
				}
			}
			else
			{
				nKCASUnitViewerSpineSprite = (NKCASUnitViewerSpineSprite)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitViewerSpineSprite, skinTemplet.m_SpriteBundleName, skinTemplet.m_SpriteName, bAsync);
				if (!string.IsNullOrEmpty(skinTemplet.m_SpriteMaterialName))
				{
					nKCASUnitViewerSpineSprite.SetReplaceMatResource(skinTemplet.m_SpriteBundleName, skinTemplet.m_SpriteMaterialName, bAsync);
				}
				else
				{
					nKCASUnitViewerSpineSprite.SetReplaceMatResource("", "", bAsync);
				}
			}
			return nKCASUnitViewerSpineSprite;
		}

		public void LoadUnitComplete()
		{
			if (m_Parent != null)
			{
				m_NKCASUnitViewerSpineSprite.m_UnitSpineSpriteInstant.m_Instant.transform.SetParent(m_Parent.transform, worldPositionStays: false);
				NKCUtil.SetGameObjectLocalPos(m_NKCASUnitViewerSpineSprite.m_UnitSpineSpriteInstant.m_Instant, 0f, 0f, 0f);
			}
			m_UnitSpriteOrg = m_NKCASUnitViewerSpineSprite.m_UnitSpineSpriteInstant.m_InstantOrg.GetAsset<GameObject>();
			m_AnimSpine.SetAnimObj(m_NKCASUnitViewerSpineSprite.m_UnitSpineSpriteInstant.m_Instant, null, bPreload: true);
			if (m_RespawnTimer != null && m_UnitSpriteOrg != null)
			{
				m_RespawnTimer.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_GAME_BATTLE_UNIT()
					.transform, worldPositionStays: false);
					m_RespawnTimer.transform.localPosition = new Vector3(0f, -75f, 0f);
				}
				NKCUtil.SetGameobjectActive(m_RespawnTimer, bValue: false);
				SetRight(bRight: true);
				SetPos(0f, 0f, 0f);
				SetScale(m_UnitTemplet.m_SpriteScale, m_UnitTemplet.m_SpriteScale);
			}

			public void Update(float fDeltaTime)
			{
				m_AnimSpine.Update(fDeltaTime);
				if (m_NKCASUnitViewerSpineSprite.m_UnitSpineSpriteInstant.m_Instant.activeSelf)
				{
					UpdateObject();
				}
			}

			private void UpdateObject()
			{
				float fPosX = m_fPosX;
				float num = m_fPosY + m_UnitTemplet.m_SpriteOffsetY + m_UnitSpriteOrg.transform.localPosition.y;
				if (m_bUseAirHigh)
				{
					num += m_UnitTemplet.m_fAirHigh;
				}
				NKCUtil.SetGameObjectLocalPos(x: (!m_bRight) ? (fPosX - (m_UnitTemplet.m_SpriteOffsetX - m_UnitSpriteOrg.transform.localPosition.x)) : (fPosX + (m_UnitTemplet.m_SpriteOffsetX + m_UnitSpriteOrg.transform.localPosition.x)), go: m_NKCASUnitViewerSpineSprite.m_UnitSpineSpriteInstant.m_Instant, y: m_fPosZ + num, z: m_fPosZ);
				fPosX = m_fScaleX * m_UnitSpriteOrg.transform.localScale.x;
				num = m_fScaleY * m_UnitSpriteOrg.transform.localScale.y;
				if (!m_bRight)
				{
					fPosX = 0f - fPosX;
				}
				NKCUtil.SetGameObjectLocalScale(m_NKCASUnitViewerSpineSprite.m_UnitSpineSpriteInstant.m_Instant, fPosX, num, 1f);
				SetDataObject_Shadow();
			}

			private void SetDataObject_Shadow()
			{
				if (m_NKCASUnitShadow != null && m_NKCASUnitShadow.m_ShadowSpriteInstant != null && !(m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant == null))
				{
					if (m_bRight)
					{
						m_Vec3Temp.Set(m_UnitTemplet.m_fShadowRotateX, 180f, m_UnitTemplet.m_fShadowRotateZ);
						m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.transform.localEulerAngles = m_Vec3Temp;
					}
					else
					{
						m_Vec3Temp.Set(m_UnitTemplet.m_fShadowRotateX, 0f, m_UnitTemplet.m_fShadowRotateZ);
						m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.transform.localEulerAngles = m_Vec3Temp;
					}
					m_Vec3Temp.Set(m_fPosX, m_fPosZ, m_fPosZ);
					if (m_bRight)
					{
						m_Vec3Temp.x += m_UnitTemplet.m_fShadowOffsetX;
					}
					else
					{
						m_Vec3Temp.x -= m_UnitTemplet.m_fShadowOffsetX;
					}
					m_Vec3Temp.y += m_UnitTemplet.m_fShadowOffsetY;
					m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.transform.localPosition = m_Vec3Temp;
					float num = 1f - 0.2f * m_fPosY * 0.01f;
					if (num < 0.3f)
					{
						num = 0.3f;
					}
					NKCUtil.SetGameObjectLocalScale(m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant, m_UnitTemplet.m_fShadowScaleX * num, m_UnitTemplet.m_fShadowScaleY * num, 1f);
					if (m_RespawnTimer != null)
					{
						m_RespawnTimer.SetPosition(m_Vec3Temp);
					}
				}
			}

			public void SetRight(bool bRight)
			{
				m_bRight = bRight;
			}

			public void SetPos(float fPosX, float fPosY, float fPosZ, bool bUseAirHigh = true)
			{
				m_fPosX = fPosX;
				m_fPosY = fPosY;
				m_fPosZ = fPosZ;
				m_bUseAirHigh = bUseAirHigh;
			}

			public void SetScale(float fScaleX, float fScaleY)
			{
				m_fScaleX = fScaleX;
				m_fScaleY = fScaleY;
			}

			public void SetActiveSprite(bool bActive)
			{
				if (bActive != m_NKCASUnitViewerSpineSprite.m_UnitSpineSpriteInstant.m_Instant.activeSelf)
				{
					m_NKCASUnitViewerSpineSprite.m_UnitSpineSpriteInstant.m_Instant.SetActive(bActive);
				}
			}

			public void SetActiveShadow(bool bActive)
			{
				if (m_NKCASUnitShadow != null && m_NKCASUnitShadow.m_ShadowSpriteInstant != null && !(m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant == null) && bActive != m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.activeSelf)
				{
					m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.SetActive(bActive);
				}
			}

			public void SetShadowType(bool bTeamA)
			{
				if (m_NKCASUnitShadow != null && m_NKCASUnitShadow.m_ShadowSpriteInstant != null && !(m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant == null))
				{
					m_NKCASUnitShadow.SetShadowType(m_UnitTemplet.m_NKC_TEAM_COLOR_TYPE, bTeamA, m_UnitTemplet.m_UnitTempletBase.IsRearmUnit);
				}
			}

			public void SetShadowType(NKC_TEAM_COLOR_TYPE eNKC_TEAM_COLOR_TYPE, bool bTeamA, bool bRearm)
			{
				if (m_NKCASUnitShadow != null && m_NKCASUnitShadow.m_ShadowSpriteInstant != null && !(m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant == null))
				{
					m_NKCASUnitShadow.SetShadowType(eNKC_TEAM_COLOR_TYPE, bTeamA, bRearm);
				}
			}

			public void Play(string animName, bool bLoop, float fNormalTime = 0f)
			{
				m_AnimSpine.Play(animName, bLoop, fNormalTime);
			}

			public void SetColor(float fR, float fG, float fB, float fA)
			{
				m_NKCASUnitViewerSpineSprite.SetColor(fR, fG, fB, fA);
			}

			public void SetLayer(string layerName)
			{
				m_NKCASUnitViewerSpineSprite.SetSortingLayerName(layerName);
			}

			public NKCASUnitShadow GetShadow()
			{
				return m_NKCASUnitShadow;
			}

			public void PlayTimer(float time = 1f)
			{
				if (!(m_RespawnTimer == null))
				{
					m_RespawnTimer.Play(time);
				}
			}

			public void StopTimer()
			{
				if (!(m_RespawnTimer == null))
				{
					m_RespawnTimer.Stop();
				}
			}
		}
