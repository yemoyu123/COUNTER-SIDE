using Cs.Logging;
using NKC.FX;
using NKC.UI;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCASEffect : NKMObjectPoolData
{
	public int m_EffectUID;

	public NKM_EFFECT_PARENT_TYPE m_NKM_EFFECT_PARENT_TYPE;

	public NKCAssetInstanceData m_EffectInstant;

	public RectTransform m_RectTransform;

	public Camera[] m_Cameras;

	public SpriteRenderer[] m_SpriteRenderer;

	public ParticleSystemRenderer[] m_ParticleSystemRenderer;

	public bool m_bPlayed;

	public Animator m_Animator;

	public ParticleSystem[] m_ParticleSystems;

	public bool m_bSpine;

	public bool m_bSpineUI;

	public string m_AnimName;

	public float m_fAnimSpeed;

	public NKCAnim2D m_Anim2D = new NKCAnim2D();

	public GameObject m_SPINE_SkeletonAnimationOrg;

	public MeshRenderer m_MeshRendererOrg;

	public GameObject m_SPINE_SkeletonAnimation;

	public GameObject m_SPINE_SkeletonGraphic;

	public MeshRenderer m_MeshRenderer;

	public NKCAnimSpine m_AnimSpine = new NKCAnimSpine();

	private Material m_Material;

	private Material m_DissolveMaterial;

	private Color m_DissolveColorOrg;

	private Vector3 m_RotateOrg;

	public bool m_bDEEffect;

	private bool m_bDie;

	public bool m_bEndAnim;

	public float m_fReserveDieTime;

	public bool m_bAutoDie;

	public bool m_bRight;

	public float m_OffsetX;

	public float m_OffsetY;

	public float m_OffsetZ;

	public float m_fAddRotate;

	public float m_fScaleX;

	public float m_fScaleY;

	public float m_fScaleZ;

	public float m_fScaleFactorX;

	public float m_fScaleFactorY;

	public float m_fScaleFactorZ;

	public bool m_bUseZScale;

	public bool m_bScaleChange;

	public string m_BoneName;

	public bool m_bUseBoneRotate;

	public bool m_bStateEndStop;

	public bool m_bStateEndStopForce;

	public bool m_bCutIn;

	public bool m_bUseGuageAsRoot;

	public bool m_UseMasterAnimSpeed;

	public short m_MasterUnitGameUID;

	private NKMUnit m_MasterUnit;

	private bool m_canIgnoreStopTime;

	public bool m_applyStopTime;

	public float m_fAnimSpeedFinal;

	public Vector3 m_TempVec3;

	private NKC_FXM_PLAYER[] m_NKC_FXM_PLAYERs;

	private NKC_FX_DELAY_EXECUTER[] m_NKC_FX_DELAY_EXECUTERs;

	private NKM_GAME_SPEED_TYPE m_NKM_GAME_SPEED_TYPE;

	private float[] m_ParticleSystem_SimulationSpeedOrg;

	public Text m_BuffText;

	public byte m_BuffDescTextPosYIndex;

	public Text m_DamageText;

	public Text m_DamageTextCritical;

	public Text m_NKM_UI_HUD_COOLTIME_COUNT_Text;

	public Text m_AB_FX_SKILL_CUTIN_COMMON_DESC_UNIT_NAME;

	public Text m_AB_FX_SKILL_CUTIN_COMMON_DESC_SKILL_NAME;

	public RectTransform m_AB_FX_SKILL_CUTIN_COMMON_DESC_UNIT_NAME_RectTransform;

	public RectTransform m_AB_FX_SKILL_CUTIN_COMMON_DESC_SKILL_NAME_RectTransform;

	public bool CanIgnoreStopTime => m_canIgnoreStopTime;

	public bool ApplyStopTime => m_applyStopTime;

	public NKMUnit GetMasterUnit()
	{
		if (m_MasterUnit != null)
		{
			return m_MasterUnit;
		}
		if (m_MasterUnitGameUID != 0 && NKCScenManager.GetScenManager().GetGameClient() != null)
		{
			m_MasterUnit = NKCScenManager.GetScenManager().GetGameClient().GetUnit(m_MasterUnitGameUID);
		}
		return m_MasterUnit;
	}

	public void SetCanIgnoreStopTime(bool bValue)
	{
		m_canIgnoreStopTime = bValue;
	}

	public void SetApplyStopTime(bool bValue)
	{
		m_applyStopTime = bValue;
	}

	public void SetUseMasterAnimSpeed(bool bValue)
	{
		m_UseMasterAnimSpeed = bValue;
	}

	public bool GetLoadFail()
	{
		if (m_EffectInstant != null)
		{
			return m_EffectInstant.GetLoadFail();
		}
		return true;
	}

	public NKCASEffect(string bundleName, string name, bool bAsync = false)
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKCASEffect;
		m_ObjectPoolBundleName = bundleName;
		m_ObjectPoolName = name;
		m_bUnloadable = true;
		Load(bAsync);
	}

	public override void Load(bool bAsync)
	{
		Init();
		m_EffectInstant = NKCAssetResourceManager.OpenInstance<GameObject>(m_ObjectPoolBundleName, m_ObjectPoolName, bAsync);
	}

	public override bool LoadComplete()
	{
		if (GetLoadFail())
		{
			return false;
		}
		m_RectTransform = m_EffectInstant.m_Instant.GetComponentInChildren<RectTransform>(includeInactive: true);
		m_SpriteRenderer = m_EffectInstant.m_Instant.GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		m_ParticleSystemRenderer = m_EffectInstant.m_Instant.GetComponentsInChildren<ParticleSystemRenderer>(includeInactive: true);
		m_Animator = m_EffectInstant.m_Instant.GetComponentInChildren<Animator>(includeInactive: true);
		m_ParticleSystems = m_EffectInstant.m_Instant.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
		m_ParticleSystem_SimulationSpeedOrg = new float[m_ParticleSystems.Length];
		m_Cameras = m_EffectInstant.m_Instant.GetComponentsInChildren<Camera>(includeInactive: true);
		LoadComplete_Anim();
		if (!LoadComplete_Spine())
		{
			LoadComplete_SpineUI();
		}
		if (!m_bSpine)
		{
			m_NKC_FXM_PLAYERs = m_EffectInstant.m_Instant.GetComponentsInChildren<NKC_FXM_PLAYER>(includeInactive: true);
			m_NKC_FX_DELAY_EXECUTERs = m_EffectInstant.m_Instant.GetComponentsInChildren<NKC_FX_DELAY_EXECUTER>(includeInactive: true);
			for (int i = 0; i < m_ParticleSystems.Length; i++)
			{
				ParticleSystem particleSystem = m_ParticleSystems[i];
				if (!(particleSystem == null))
				{
					ParticleSystem.MainModule main = particleSystem.main;
					m_ParticleSystem_SimulationSpeedOrg[i] = main.simulationSpeed;
				}
			}
		}
		Vector3 localScale = m_EffectInstant.m_InstantOrg.GetAsset<GameObject>().transform.localScale;
		m_fScaleX = localScale.x;
		m_fScaleY = localScale.y;
		m_fScaleZ = localScale.z;
		m_RotateOrg = m_EffectInstant.m_InstantOrg.GetAsset<GameObject>().transform.localEulerAngles;
		return true;
	}

	public bool LoadComplete_Anim()
	{
		if (m_Animator == null)
		{
			return false;
		}
		m_Anim2D.SetAnimObj(m_EffectInstant.m_Instant);
		if (m_Anim2D.GetAnimClipByName(m_ObjectPoolName + "_END") != null)
		{
			m_bEndAnim = true;
		}
		if (m_Anim2D.GetAnimClipByName(m_ObjectPoolName + "_BASE_LOOP") != null)
		{
			m_Anim2D.SetAnimAutoChange("BASE", "BASE_LOOP", bLoop: true, 1f);
		}
		return true;
	}

	public bool LoadComplete_Spine()
	{
		m_SPINE_SkeletonAnimation = null;
		Transform transform = m_EffectInstant.m_Instant.transform.Find("SPINE_SkeletonAnimation");
		if (transform == null)
		{
			return false;
		}
		m_SPINE_SkeletonAnimation = transform.gameObject;
		if (m_SPINE_SkeletonAnimation == null)
		{
			return false;
		}
		m_bSpine = true;
		m_MeshRenderer = m_SPINE_SkeletonAnimation.GetComponentInChildren<MeshRenderer>(includeInactive: true);
		m_Material = m_MeshRenderer.sharedMaterial;
		m_AnimSpine.SetAnimObj(m_EffectInstant.m_Instant, null, bPreload: true);
		if (m_AnimSpine.GetAnimByName("END") != null)
		{
			m_bEndAnim = true;
		}
		Transform transform2 = m_EffectInstant.m_InstantOrg.GetAsset<GameObject>().transform.Find("SPINE_SkeletonAnimation");
		if (transform2 == null)
		{
			Log.Error(m_ObjectPoolName + " has no sub prefab name SPINE_SkeletonAnimation", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCEffectManager.cs", 272);
			return false;
		}
		m_SPINE_SkeletonAnimationOrg = transform2.gameObject;
		m_MeshRendererOrg = m_SPINE_SkeletonAnimationOrg.GetComponentInChildren<MeshRenderer>(includeInactive: true);
		if (m_MeshRendererOrg.sharedMaterial != null)
		{
			if (m_Material == null)
			{
				m_Material = new Material(m_MeshRendererOrg.sharedMaterial);
				m_MeshRenderer.sharedMaterial = m_Material;
			}
			m_DissolveMaterial = new Material(m_MeshRendererOrg.sharedMaterial);
			m_DissolveMaterial.EnableKeyword("DISSOLVE_ON");
			if (!m_DissolveMaterial.HasProperty("_DissolveGlowColor"))
			{
				Log.Error("m_DissolveMaterial does not have _DissolveGlowColor prop, m_ObjectPoolName: " + m_ObjectPoolName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCEffectManager.cs", 291);
			}
			m_DissolveColorOrg = m_DissolveMaterial.GetColor("_DissolveGlowColor");
		}
		else
		{
			Log.Error("m_MeshRendererOrg.sharedMaterial is null, m_ObjectPoolName: " + m_ObjectPoolName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCEffectManager.cs", 297);
		}
		return true;
	}

	public bool LoadComplete_SpineUI()
	{
		m_SPINE_SkeletonGraphic = null;
		Transform transform = m_EffectInstant.m_Instant.transform.Find("SPINE_SkeletonGraphic");
		if (transform == null)
		{
			return false;
		}
		m_SPINE_SkeletonGraphic = transform.gameObject;
		if (m_SPINE_SkeletonGraphic == null)
		{
			return false;
		}
		m_bSpineUI = true;
		m_AnimSpine.SetAnimObj(m_EffectInstant.m_Instant, null, bPreload: true);
		return true;
	}

	public void Init()
	{
		m_EffectUID = 0;
		m_NKM_EFFECT_PARENT_TYPE = NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT;
		m_AnimName = "";
		m_fAnimSpeed = 1f;
		m_fReserveDieTime = -1f;
		m_bAutoDie = true;
		m_bRight = true;
		m_OffsetX = 0f;
		m_OffsetY = 0f;
		m_OffsetZ = 0f;
		m_fAddRotate = 0f;
		if (m_EffectInstant != null && m_EffectInstant.m_InstantOrg != null && !m_EffectInstant.GetLoadFail())
		{
			Vector3 localScale = m_EffectInstant.m_InstantOrg.GetAsset<GameObject>().transform.localScale;
			m_fScaleX = localScale.x;
			m_fScaleY = localScale.y;
			m_fScaleZ = localScale.z;
		}
		else
		{
			m_fScaleX = 1f;
			m_fScaleY = 1f;
			m_fScaleZ = 1f;
		}
		m_fScaleFactorX = 1f;
		m_fScaleFactorY = 1f;
		m_fScaleFactorZ = 1f;
		m_bUseZScale = false;
		m_bScaleChange = false;
		m_BoneName = "";
		m_bUseBoneRotate = false;
		m_bStateEndStop = false;
		m_bStateEndStopForce = false;
		m_bCutIn = false;
		m_UseMasterAnimSpeed = false;
		m_MasterUnitGameUID = 0;
		m_fAnimSpeedFinal = 1f;
		m_bDie = false;
		m_bDEEffect = false;
		m_NKM_GAME_SPEED_TYPE = NKM_GAME_SPEED_TYPE.NGST_1;
	}

	public override void Open()
	{
		m_bPlayed = false;
		if (m_bSpine || m_NKC_FXM_PLAYERs == null)
		{
			return;
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME && NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_STATE() == NKC_SCEN_STATE.NSS_START)
		{
			m_NKM_GAME_SPEED_TYPE = NKM_GAME_SPEED_TYPE.NGST_1;
			for (int i = 0; i < m_NKC_FXM_PLAYERs.Length; i++)
			{
				m_NKC_FXM_PLAYERs[i].SetExternalTimeScale(1.1f);
			}
			for (int j = 0; j < m_NKC_FX_DELAY_EXECUTERs.Length; j++)
			{
				m_NKC_FX_DELAY_EXECUTERs[j].GameTimeScale = 1.1f;
			}
		}
		else
		{
			m_NKM_GAME_SPEED_TYPE = NKM_GAME_SPEED_TYPE.NGST_1;
			for (int k = 0; k < m_NKC_FXM_PLAYERs.Length; k++)
			{
				m_NKC_FXM_PLAYERs[k].SetExternalTimeScale(1f);
			}
			for (int l = 0; l < m_NKC_FX_DELAY_EXECUTERs.Length; l++)
			{
				m_NKC_FX_DELAY_EXECUTERs[l].GameTimeScale = 1f;
			}
		}
		for (int m = 0; m < m_ParticleSystems.Length; m++)
		{
			ParticleSystem particleSystem = m_ParticleSystems[m];
			if (!(particleSystem == null))
			{
				ParticleSystem.MainModule main = particleSystem.main;
				if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME && NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_STATE() == NKC_SCEN_STATE.NSS_START)
				{
					main.simulationSpeed = m_ParticleSystem_SimulationSpeedOrg[m] * 1.1f;
				}
				else
				{
					main.simulationSpeed = m_ParticleSystem_SimulationSpeedOrg[m];
				}
			}
		}
	}

	public override void Close()
	{
		if (m_AnimSpine != null)
		{
			m_AnimSpine.ResetParticleSimulSpeedOrg();
		}
		if (m_EffectInstant != null && m_EffectInstant.m_Instant != null && !m_EffectInstant.GetLoadFail() && m_EffectInstant.m_Instant.activeSelf)
		{
			m_EffectInstant.m_Instant.SetActive(value: false);
		}
		if (m_MeshRenderer != null && m_Material != null)
		{
			m_MeshRenderer.sharedMaterial = m_Material;
		}
		Init();
	}

	public void ObjectParentWait()
	{
		if (m_EffectInstant != null && !(m_EffectInstant.m_Instant == null) && m_EffectInstant.m_Instant.transform.parent != NKCUIManager.m_TR_NKM_WAIT_INSTANT)
		{
			m_EffectInstant.m_Instant.transform.SetParent(NKCUIManager.m_TR_NKM_WAIT_INSTANT, worldPositionStays: false);
		}
	}

	public void ObjectParentRestore()
	{
		if (m_EffectInstant == null || m_EffectInstant.m_Instant == null)
		{
			return;
		}
		switch (m_NKM_EFFECT_PARENT_TYPE)
		{
		case NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT:
			if (NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
				.Get_NUM_GAME_BATTLE_EFFECT() != null && m_EffectInstant.m_Instant.transform.parent.gameObject != NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
				.Get_NUM_GAME_BATTLE_EFFECT())
			{
				m_EffectInstant.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_NUM_GAME_BATTLE_EFFECT()
					.transform, worldPositionStays: false);
				}
				break;
			case NKM_EFFECT_PARENT_TYPE.NEPT_NUF_BEFORE_HUD_EFFECT:
				if (NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_NUF_BEFORE_HUD_EFFECT() != null && m_EffectInstant.m_Instant.transform.parent.gameObject != NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_NUF_BEFORE_HUD_EFFECT())
				{
					m_EffectInstant.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
						.Get_NUF_BEFORE_HUD_EFFECT()
						.transform, worldPositionStays: false);
					}
					break;
				case NKM_EFFECT_PARENT_TYPE.NEPT_NUF_BEFORE_HUD_CONTROL_EFFECT:
					if (NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
						.Get_NUF_BEFORE_HUD_CONTROL_EFFECT() != null && m_EffectInstant.m_Instant.transform.parent.gameObject != NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
						.Get_NUF_BEFORE_HUD_CONTROL_EFFECT())
					{
						m_EffectInstant.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
							.Get_NUF_BEFORE_HUD_CONTROL_EFFECT()
							.transform, worldPositionStays: false);
						}
						break;
					case NKM_EFFECT_PARENT_TYPE.NEPT_NUF_AFTER_HUD_EFFECT:
						if (NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
							.Get_NUF_AFTER_HUD_EFFECT() != null && m_EffectInstant.m_Instant.transform.parent.gameObject != NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
							.Get_NUF_AFTER_HUD_EFFECT())
						{
							m_EffectInstant.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
								.Get_NUF_AFTER_HUD_EFFECT()
								.transform, worldPositionStays: false);
							}
							break;
						case NKM_EFFECT_PARENT_TYPE.NEPT_NUF_AFTER_UI_EFFECT:
							if (NKCScenManager.GetScenManager().Get_NUF_AFTER_UI_EFFECT() != null && m_EffectInstant.m_Instant.transform.parent != NKCScenManager.GetScenManager().Get_NUF_AFTER_UI_EFFECT())
							{
								m_EffectInstant.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_NUF_AFTER_UI_EFFECT(), worldPositionStays: false);
							}
							break;
						}
					}

					public override void Unload()
					{
						if (m_Cameras != null)
						{
							for (int i = 0; i < m_Cameras.Length; i++)
							{
								m_Cameras[i].targetTexture = null;
							}
						}
						m_Cameras = null;
						NKCAssetResourceManager.CloseInstance(m_EffectInstant);
						m_EffectInstant = null;
						m_RectTransform = null;
						m_SpriteRenderer = null;
						m_ParticleSystemRenderer = null;
						m_Animator = null;
						m_ParticleSystems = null;
						m_Anim2D.Init();
						m_Anim2D = null;
						m_SPINE_SkeletonAnimationOrg = null;
						m_MeshRendererOrg = null;
						m_SPINE_SkeletonAnimation = null;
						m_SPINE_SkeletonGraphic = null;
						m_MeshRenderer = null;
						m_AnimSpine.Init();
						m_AnimSpine = null;
						m_Material = null;
						m_DissolveMaterial = null;
						m_BuffText = null;
						m_DamageText = null;
						m_DamageTextCritical = null;
						m_NKM_UI_HUD_COOLTIME_COUNT_Text = null;
						m_AB_FX_SKILL_CUTIN_COMMON_DESC_UNIT_NAME = null;
						m_AB_FX_SKILL_CUTIN_COMMON_DESC_SKILL_NAME = null;
						m_AB_FX_SKILL_CUTIN_COMMON_DESC_UNIT_NAME_RectTransform = null;
						m_AB_FX_SKILL_CUTIN_COMMON_DESC_SKILL_NAME_RectTransform = null;
					}

					public void Update(float fDeltaTime)
					{
						if (m_fReserveDieTime != -1f && m_fReserveDieTime > 0f)
						{
							m_fReserveDieTime -= fDeltaTime;
							if (m_fReserveDieTime <= 0f)
							{
								m_fReserveDieTime = 0f;
								Stop();
							}
						}
						if (m_bScaleChange || m_bUseZScale)
						{
							if (m_EffectInstant != null && m_EffectInstant.m_Instant != null)
							{
								if (m_bUseZScale)
								{
									float zScaleFactor = NKCScenManager.GetScenManager().GetGameClient().GetZScaleFactor(m_EffectInstant.m_Instant.transform.position.z);
									NKCUtil.SetGameObjectLocalScale(m_EffectInstant.m_Instant, m_fScaleFactorX * m_fScaleX * zScaleFactor, m_fScaleFactorY * m_fScaleY * zScaleFactor);
								}
								else
								{
									NKCUtil.SetGameObjectLocalScale(m_EffectInstant.m_Instant, m_fScaleFactorX * m_fScaleX, m_fScaleFactorY * m_fScaleY);
								}
							}
							m_bScaleChange = false;
						}
						bool flag = false;
						float num = MakeAnimSpeed();
						if (Mathf.Abs(m_fAnimSpeedFinal - num) > 0.01f)
						{
							flag = true;
						}
						if (m_NKC_FXM_PLAYERs != null && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME && NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_STATE() == NKC_SCEN_STATE.NSS_START && m_NKM_GAME_SPEED_TYPE != NKCScenManager.GetScenManager().GetGameClient().GetGameRuntimeData()
							.m_NKM_GAME_SPEED_TYPE)
						{
							m_NKM_GAME_SPEED_TYPE = NKCScenManager.GetScenManager().GetGameClient().GetGameRuntimeData()
								.m_NKM_GAME_SPEED_TYPE;
							flag = true;
						}
						if (flag)
						{
							UpdateAnimSpeedInternal(num);
						}
						if (m_Animator != null)
						{
							m_Anim2D.Update(fDeltaTime);
						}
						if (m_SPINE_SkeletonAnimation != null)
						{
							m_AnimSpine.Update(fDeltaTime);
							if (m_bEndAnim && m_AnimName.Length > 1 && m_AnimSpine.IsAnimationEnd() && m_AnimSpine.GetAnimName().CompareTo("END") != 0)
							{
								m_AnimSpine.Play("END", bLoop: false);
							}
						}
						else if (m_SPINE_SkeletonGraphic != null)
						{
							m_AnimSpine.Update(fDeltaTime);
						}
						if (DieCheck())
						{
							SetDie();
						}
					}

					public void UpdateAnimSpeedInternal(float animSpeedFinal)
					{
						m_fAnimSpeedFinal = animSpeedFinal;
						if (m_Animator != null)
						{
							m_Anim2D.SetPlaySpeed(m_fAnimSpeedFinal);
						}
						if (m_SPINE_SkeletonAnimation != null)
						{
							m_AnimSpine.SetPlaySpeed(m_fAnimSpeedFinal);
						}
						else if (m_SPINE_SkeletonGraphic != null)
						{
							m_AnimSpine.SetPlaySpeed(m_fAnimSpeedFinal);
						}
						if (m_NKC_FXM_PLAYERs != null)
						{
							for (int i = 0; i < m_NKC_FXM_PLAYERs.Length; i++)
							{
								NKC_FXM_PLAYER nKC_FXM_PLAYER = m_NKC_FXM_PLAYERs[i];
								switch (m_NKM_GAME_SPEED_TYPE)
								{
								case NKM_GAME_SPEED_TYPE.NGST_1:
								case NKM_GAME_SPEED_TYPE.NGST_3:
								case NKM_GAME_SPEED_TYPE.NGST_10:
									nKC_FXM_PLAYER.SetExternalTimeScale(1.1f * m_fAnimSpeedFinal);
									break;
								case NKM_GAME_SPEED_TYPE.NGST_2:
									nKC_FXM_PLAYER.SetExternalTimeScale(1.5f * m_fAnimSpeedFinal);
									break;
								case NKM_GAME_SPEED_TYPE.NGST_05:
									nKC_FXM_PLAYER.SetExternalTimeScale(0.6f * m_fAnimSpeedFinal);
									break;
								}
							}
						}
						if (m_NKC_FX_DELAY_EXECUTERs != null)
						{
							for (int j = 0; j < m_NKC_FX_DELAY_EXECUTERs.Length; j++)
							{
								NKC_FX_DELAY_EXECUTER nKC_FX_DELAY_EXECUTER = m_NKC_FX_DELAY_EXECUTERs[j];
								switch (m_NKM_GAME_SPEED_TYPE)
								{
								case NKM_GAME_SPEED_TYPE.NGST_1:
								case NKM_GAME_SPEED_TYPE.NGST_3:
								case NKM_GAME_SPEED_TYPE.NGST_10:
									nKC_FX_DELAY_EXECUTER.GameTimeScale = 1.1f * m_fAnimSpeedFinal;
									break;
								case NKM_GAME_SPEED_TYPE.NGST_2:
									nKC_FX_DELAY_EXECUTER.GameTimeScale = 1.5f * m_fAnimSpeedFinal;
									break;
								case NKM_GAME_SPEED_TYPE.NGST_05:
									nKC_FX_DELAY_EXECUTER.GameTimeScale = 0.6f * m_fAnimSpeedFinal;
									break;
								}
							}
						}
						if (m_ParticleSystems == null)
						{
							return;
						}
						for (int k = 0; k < m_ParticleSystems.Length; k++)
						{
							ParticleSystem particleSystem = m_ParticleSystems[k];
							if (!(particleSystem == null))
							{
								ParticleSystem.MainModule main = particleSystem.main;
								switch (m_NKM_GAME_SPEED_TYPE)
								{
								case NKM_GAME_SPEED_TYPE.NGST_1:
								case NKM_GAME_SPEED_TYPE.NGST_3:
								case NKM_GAME_SPEED_TYPE.NGST_10:
									main.simulationSpeed = m_ParticleSystem_SimulationSpeedOrg[k] * 1.1f * m_fAnimSpeedFinal;
									break;
								case NKM_GAME_SPEED_TYPE.NGST_2:
									main.simulationSpeed = m_ParticleSystem_SimulationSpeedOrg[k] * 1.5f * m_fAnimSpeedFinal;
									break;
								case NKM_GAME_SPEED_TYPE.NGST_05:
									main.simulationSpeed = m_ParticleSystem_SimulationSpeedOrg[k] * 0.6f * m_fAnimSpeedFinal;
									break;
								}
							}
						}
					}

					public void StopEffectAnim()
					{
						UpdateAnimSpeedInternal(0f);
					}

					private bool DieCheck()
					{
						bool result = true;
						if (m_Animator != null && m_Anim2D != null && m_AnimName.Length > 1)
						{
							if (!m_bEndAnim)
							{
								if (!m_Anim2D.IsAnimationEnd())
								{
									result = false;
								}
							}
							else if (!m_Anim2D.IsAnimationEnd() || m_Anim2D.GetAnimName().CompareTo("END") != 0)
							{
								result = false;
							}
						}
						if (m_SPINE_SkeletonAnimation != null && m_AnimSpine != null && m_AnimName.Length > 1)
						{
							if (!m_bEndAnim)
							{
								if (!m_AnimSpine.IsAnimationEnd())
								{
									result = false;
								}
							}
							else if (!m_AnimSpine.IsAnimationEnd() || m_AnimSpine.GetAnimName().CompareTo("END") != 0)
							{
								result = false;
							}
						}
						else if (m_SPINE_SkeletonGraphic != null && m_AnimSpine != null && m_AnimName.Length > 1 && !m_AnimSpine.IsAnimationEnd())
						{
							result = false;
						}
						if (m_ParticleSystems != null)
						{
							for (int i = 0; i < m_ParticleSystems.Length; i++)
							{
								if (!(m_ParticleSystems[i] == null))
								{
									if (m_ParticleSystems[i].isEmitting && m_ParticleSystems[i].emission.enabled)
									{
										result = false;
									}
									if (m_ParticleSystems[i].particleCount > 0)
									{
										result = false;
									}
								}
							}
						}
						return result;
					}

					public void SetDie()
					{
						m_bDie = true;
					}

					public void SetReserveDieTime(float fReserveDieTime)
					{
						m_fReserveDieTime = fReserveDieTime;
					}

					public void ReStart()
					{
						m_bDie = false;
						if (m_EffectInstant != null && m_EffectInstant.m_Instant != null)
						{
							if (m_EffectInstant.m_Instant.activeSelf)
							{
								m_EffectInstant.m_Instant.SetActive(value: false);
							}
							m_EffectInstant.m_Instant.SetActive(value: true);
						}
						if (m_Animator != null && m_AnimName.Length > 1)
						{
							m_Anim2D.Play(m_AnimName, bLoop: false);
						}
						if (m_SPINE_SkeletonAnimation != null && m_AnimName.Length > 1)
						{
							m_AnimSpine.Play(m_AnimName, bLoop: false);
						}
						else if (m_SPINE_SkeletonGraphic != null && m_AnimName.Length > 1)
						{
							m_AnimSpine.Play(m_AnimName, bLoop: false);
						}
					}

					public void Stop(bool bForce = false)
					{
						if (m_bEndAnim && m_Animator != null && m_AnimName.Length > 1 && m_Anim2D.GetAnimName() != "END")
						{
							m_Anim2D.Play("END", bLoop: false);
						}
						if (m_bEndAnim && m_SPINE_SkeletonAnimation != null && m_AnimName.Length > 1 && m_AnimSpine.GetAnimName() != "END")
						{
							m_AnimSpine.Play("END", bLoop: false);
						}
						if (m_ParticleSystems != null)
						{
							for (int i = 0; i < m_ParticleSystems.Length; i++)
							{
								m_ParticleSystems[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
							}
						}
						if (bForce)
						{
							SetDie();
						}
					}

					private void StopParticleEmit()
					{
						for (int i = 0; i < m_ParticleSystems.Length; i++)
						{
							m_ParticleSystems[i].Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
						}
					}

					public bool IsEnd()
					{
						return m_bDie;
					}

					public void PlayAnim(string animName, bool bLoop = false, float fAnimSpeed = 1f)
					{
						m_fAnimSpeed = fAnimSpeed;
						float playSpeed = MakeAnimSpeed();
						m_bPlayed = true;
						if (m_Animator != null)
						{
							m_AnimName = animName;
							m_Anim2D.SetPlaySpeed(playSpeed);
							m_Anim2D.Play(m_AnimName, bLoop);
						}
						if (m_SPINE_SkeletonAnimation != null)
						{
							m_AnimName = animName;
							m_AnimSpine.SetPlaySpeed(playSpeed);
							m_AnimSpine.Play(m_AnimName, bLoop);
						}
						else if (m_SPINE_SkeletonGraphic != null)
						{
							m_AnimName = animName;
							m_AnimSpine.SetPlaySpeed(playSpeed);
							m_AnimSpine.Play(m_AnimName, bLoop);
						}
					}

					public float MakeAnimSpeed()
					{
						float num = m_fAnimSpeed;
						if (m_UseMasterAnimSpeed && GetMasterUnit() != null)
						{
							num *= GetMasterUnit().GetUnitFrameData().m_fAnimSpeed;
						}
						return num;
					}

					public void SetRight(bool bRight)
					{
						m_bRight = bRight;
						if (m_EffectInstant != null && !(m_EffectInstant.m_Instant == null) && !m_bUseBoneRotate)
						{
							if (m_bRight)
							{
								m_TempVec3 = m_EffectInstant.m_Instant.transform.localEulerAngles;
								m_TempVec3.y = m_RotateOrg.y;
								m_EffectInstant.m_Instant.transform.localEulerAngles = m_TempVec3;
							}
							else
							{
								m_TempVec3 = m_EffectInstant.m_Instant.transform.localEulerAngles;
								m_TempVec3.y = m_RotateOrg.y + 180f;
								m_EffectInstant.m_Instant.transform.localEulerAngles = m_TempVec3;
							}
						}
					}

					public void SetLookDir(float fAngle)
					{
						if (m_EffectInstant != null && !(m_EffectInstant.m_Instant == null))
						{
							m_TempVec3.x = 0f;
							m_TempVec3.y = 0f;
							m_TempVec3.z = fAngle;
							m_EffectInstant.m_Instant.transform.localEulerAngles = m_TempVec3;
						}
					}

					public void SetRotate(float fAngle)
					{
						if (m_EffectInstant != null && !(m_EffectInstant.m_Instant == null))
						{
							m_TempVec3 = m_EffectInstant.m_Instant.transform.localEulerAngles;
							m_TempVec3.x = 0f;
							m_TempVec3.z = fAngle;
							m_EffectInstant.m_Instant.transform.localEulerAngles = m_TempVec3;
						}
					}

					public void SetPos(float fX, float fY, float fZ)
					{
						if (m_EffectInstant == null || m_EffectInstant.m_Instant == null)
						{
							return;
						}
						if (m_bRight || m_bUseBoneRotate)
						{
							if (m_RectTransform == null)
							{
								NKCUtil.SetGameObjectLocalPos(m_EffectInstant.m_Instant, fX + m_OffsetX, fY + m_OffsetY, fZ + m_OffsetZ);
								return;
							}
							Vector2 anchoredPosition = m_RectTransform.anchoredPosition;
							anchoredPosition.Set(fX + m_OffsetX, fY + m_OffsetY);
							m_RectTransform.anchoredPosition = anchoredPosition;
						}
						else if (m_RectTransform == null)
						{
							NKCUtil.SetGameObjectLocalPos(m_EffectInstant.m_Instant, fX - m_OffsetX, fY + m_OffsetY, fZ + m_OffsetZ);
						}
						else
						{
							Vector2 anchoredPosition2 = m_RectTransform.anchoredPosition;
							anchoredPosition2.Set(fX - m_OffsetX, fY + m_OffsetY);
							m_RectTransform.anchoredPosition = anchoredPosition2;
						}
					}

					public void SetScale(float fX, float fY, float fZ)
					{
						m_fScaleX = fX;
						m_fScaleY = fY;
						m_fScaleZ = fZ;
						m_bScaleChange = true;
					}

					public void SetScaleFactor(float fX, float fY, float fZ)
					{
						m_fScaleFactorX = fX;
						m_fScaleFactorY = fY;
						m_fScaleFactorZ = fZ;
						m_bScaleChange = true;
					}

					public void SetGuageRoot(bool value)
					{
						m_bUseGuageAsRoot = value;
					}

					public void SetDissolveOn(bool bOn)
					{
						if (bOn && m_DissolveMaterial != null)
						{
							m_MeshRenderer.sharedMaterial = m_DissolveMaterial;
						}
						else if (m_Material != null)
						{
							m_MeshRenderer.sharedMaterial = m_Material;
						}
					}

					public void SetDissolveBlend(float fBlend)
					{
						if (!(m_DissolveMaterial == null))
						{
							m_DissolveMaterial.SetFloat("_DissolveBlend", fBlend);
						}
					}

					public void SetDissolveColor(Color color)
					{
						if (!(m_DissolveMaterial == null))
						{
							if (color.r == -1f)
							{
								color.r = m_DissolveColorOrg.r;
							}
							if (color.g == -1f)
							{
								color.g = m_DissolveColorOrg.g;
							}
							if (color.b == -1f)
							{
								color.b = m_DissolveColorOrg.b;
							}
							if (color.a == -1f)
							{
								color.a = m_DissolveColorOrg.a;
							}
							m_DissolveMaterial.SetColor("_DissolveGlowColor", color);
						}
					}

					public void DamageTextInit()
					{
						if (m_EffectInstant == null || m_EffectInstant.m_Instant == null)
						{
							return;
						}
						if (m_DamageText == null)
						{
							Transform transform = m_EffectInstant.m_Instant.transform.Find("AB_FX_DAMAGE_TEXT_Text");
							if (transform != null)
							{
								GameObject gameObject = transform.gameObject;
								if (gameObject != null)
								{
									m_DamageText = gameObject.GetComponent<Text>();
								}
							}
						}
						if (!(m_DamageTextCritical == null))
						{
							return;
						}
						Transform transform2 = m_EffectInstant.m_Instant.transform.Find("AB_FX_DAMAGE_TEXT_Text/AB_FX_DAMAGE_TEXT_CRITICAL");
						if (transform2 != null)
						{
							GameObject gameObject2 = transform2.gameObject;
							if (gameObject2 != null)
							{
								m_DamageTextCritical = gameObject2.GetComponent<Text>();
							}
						}
					}

					public void BuffTextInit(byte buffDescTextPosYIndex)
					{
						if (m_BuffText == null)
						{
							if (m_EffectInstant == null || m_EffectInstant.m_Instant == null)
							{
								return;
							}
							Transform transform = m_EffectInstant.m_Instant.transform.Find("AB_FX_BUFF_TEXT_Text");
							if (transform != null)
							{
								GameObject gameObject = transform.gameObject;
								if (gameObject != null)
								{
									m_BuffText = gameObject.GetComponent<Text>();
								}
							}
						}
						m_BuffDescTextPosYIndex = buffDescTextPosYIndex;
					}

					public void Init_AB_FX_COOLTIME()
					{
						if (!(m_NKM_UI_HUD_COOLTIME_COUNT_Text == null) || m_EffectInstant == null || m_EffectInstant.m_Instant == null)
						{
							return;
						}
						Transform transform = m_EffectInstant.m_Instant.transform.Find("NKM_UI_COOLTIME_CONTENT/NKM_UI_HUD_COOLTIME_COUNT");
						if (transform != null)
						{
							GameObject gameObject = transform.gameObject;
							if (gameObject != null)
							{
								m_NKM_UI_HUD_COOLTIME_COUNT_Text = gameObject.GetComponent<Text>();
							}
						}
					}

					public void Init_AB_FX_SKILL_CUTIN_COMMON_DESC()
					{
						if (!(m_AB_FX_SKILL_CUTIN_COMMON_DESC_UNIT_NAME == null) || m_EffectInstant == null || m_EffectInstant.m_Instant == null)
						{
							return;
						}
						Transform transform = m_EffectInstant.m_Instant.transform.Find("DESC/POS_TEXT_CHA_NAME/OFFSET_TEXT_CHA_NAME/TEXT_CHA_NAME");
						if (transform != null)
						{
							GameObject gameObject = transform.gameObject;
							if (gameObject != null)
							{
								m_AB_FX_SKILL_CUTIN_COMMON_DESC_UNIT_NAME = gameObject.GetComponent<Text>();
								m_AB_FX_SKILL_CUTIN_COMMON_DESC_UNIT_NAME_RectTransform = gameObject.GetComponent<RectTransform>();
							}
						}
						transform = m_EffectInstant.m_Instant.transform.Find("DESC/POS_TEXT_SPELL_NAME/OFFSET_TEXT_SPELL_NAME/TEXT_SPELL_NAME");
						if (transform != null)
						{
							GameObject gameObject2 = transform.gameObject;
							if (gameObject2 != null)
							{
								m_AB_FX_SKILL_CUTIN_COMMON_DESC_SKILL_NAME = gameObject2.GetComponent<Text>();
								m_AB_FX_SKILL_CUTIN_COMMON_DESC_SKILL_NAME_RectTransform = gameObject2.GetComponent<RectTransform>();
							}
						}
					}
				}
