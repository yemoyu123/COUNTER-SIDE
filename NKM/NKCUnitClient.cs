using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ClientPacket.Game;
using NKC;
using NKC.Game.Fx;
using NKC.Game.Unit;
using NKC.UI;
using NKC.UI.HUD;
using NKM.Templet;
using NKM.Unit;
using Spine;
using UnityEngine;
using UnityEngine.UI;

namespace NKM;

public class NKCUnitClient : NKMUnit
{
	public enum NKC_CAMERA_PRIORITY
	{
		NONE = 0,
		HYPER = 20,
		AWAKEN_SPAWN = 30,
		MAX = 1000
	}

	private const int CAMERA_PRIORITY_AWAKEN_SPAWN = 30;

	private const int CAMERA_PRIORITY_HYPER = 20;

	private const int CAMERA_PRIORITY_NONE = 0;

	private NKCGameClient m_NKCGameClient;

	protected bool m_bLoadComplete;

	private NKCGameUnitObject m_GameUnitObject;

	private NKCAssetInstanceData m_UnitObject;

	private NKCUnitTouchObject m_NKCUnitTouchObject;

	private GameObject m_SpriteObject;

	private GameObject m_MainSpriteObject;

	private GameObject m_UNIT_GAGE;

	private RectTransform m_UNIT_GAGE_RectTransform;

	private GameObject m_UNIT_SKILL;

	private GameObject m_SKILL_BUTTON;

	private NKCUIComButton m_SKILL_BUTTON_Btn;

	private GameObject m_UNIT_GAGE_PANEL;

	private NKCUIComHealthBar m_UnitHealthBar;

	private GameObject m_UNIT_HP_GAGE;

	private float m_fOrgGageSize = 150f;

	private NKCUIComSkillGauge m_SKILL_GAUGE;

	private GameObject m_UNIT_LEVEL_BG;

	private Image m_UNIT_LEVEL_BG_Image;

	private GameObject m_UNIT_LEVEL;

	private RectTransform m_UNIT_LEVEL_RectTransform;

	private GameObject m_UNIT_LEVEL_TEXT;

	private NKCUIComTextUnitLevel m_UNIT_LEVEL_TEXT_Text;

	private GameObject m_UNIT_ARMOR_TYPE;

	private Image m_UNIT_ARMOR_TYPE_Image;

	private GameObject m_UNIT_ASSIST;

	private NKMTrackingFloat m_GageWide = new NKMTrackingFloat();

	private NKMTrackingFloat m_GageOffsetPosX = new NKMTrackingFloat();

	private NKMTrackingFloat m_GageOffsetPosY = new NKMTrackingFloat();

	private NKMTrackingFloat m_ObjPosX = new NKMTrackingFloat();

	private NKMTrackingFloat m_ObjPosZ = new NKMTrackingFloat();

	private NKCAnimSpine m_NKCAnimSpine = new NKCAnimSpine();

	private bool m_bDissolveEnable;

	private NKMTrackingFloat m_DissolveFactor = new NKMTrackingFloat();

	private List<int> m_listSoundUID = new List<int>();

	private Color m_ColorTemp;

	private NKMMinMaxVec3 m_TempMinMaxVec3 = new NKMMinMaxVec3();

	private LinkedList<NKCASEffect> m_llEffect = new LinkedList<NKCASEffect>();

	private Dictionary<NKM_UNIT_STATUS_EFFECT, NKCASEffect> m_dicStatusEffect = new Dictionary<NKM_UNIT_STATUS_EFFECT, NKCASEffect>();

	private Dictionary<string, NKCASEffect> m_dicLoadEffectTemp = new Dictionary<string, NKCASEffect>();

	private Dictionary<string, NKCAssetResourceData> m_dicLoadSoundTemp = new Dictionary<string, NKCAssetResourceData>();

	private Dictionary<short, NKCASEffect> m_dicBuffEffectRange = new Dictionary<short, NKCASEffect>();

	private Dictionary<short, NKCASEffect> m_dicBuffEffect = new Dictionary<short, NKCASEffect>();

	private LinkedList<NKCASEffect> m_llBuffTextEffect = new LinkedList<NKCASEffect>();

	private NKCASEffect m_RespawnTempletUnitEffect;

	private Vector3 m_Vector3Temp = new Vector3(0f, 0f, 0f);

	private Vector3 m_Vector3Temp2 = new Vector3(0f, 0f, 0f);

	private float m_fGagePosX;

	private float m_fGagePosY;

	private byte m_BuffDescTextPosYIndex;

	private NKCASUnitSprite m_NKCASUnitSprite;

	private NKCASUnitSpineSprite m_NKCASUnitSpineSprite;

	private NKCASUnitShadow m_NKCASUnitShadow;

	private NKCASUnitMiniMapFace m_NKCASUnitMiniMapFace;

	private NKCUnitViewer m_NKCUnitViewer = new NKCUnitViewer();

	private NKCASDangerChargeUI m_NKCASDangerChargeUI;

	private NKC2DMotionAfterImage m_NKCMotionAfterImage = new NKC2DMotionAfterImage();

	private Half m_HalfTemp;

	protected StringBuilder m_StringBuilder = new StringBuilder();

	private float m_fManualSkillUseAck;

	private bool m_bManualSkillUseStart;

	private byte m_bManualSkillUseStateID;

	private int m_BuffUnitLevelLastUpdate;

	private bool m_bHyperCutinLoaded;

	private HashSet<NKM_UNIT_STATUS_EFFECT> m_hsEffectToRemove = new HashSet<NKM_UNIT_STATUS_EFFECT>();

	public Dictionary<short, NKCASEffect> GetBuffEffectDic()
	{
		return m_dicBuffEffect;
	}

	public NKCASUnitSpineSprite GetNKCASUnitSpineSprite()
	{
		return m_NKCASUnitSpineSprite;
	}

	public NKCUnitViewer GetUnitViewer()
	{
		return m_NKCUnitViewer;
	}

	public NKCUnitClient()
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKCUnitClient;
		m_NKM_UNIT_CLASS_TYPE = NKM_UNIT_CLASS_TYPE.NCT_UNIT_CLIENT;
		InitUnitObject();
		InitSpriteObject();
		InitGage();
		InitShadow();
	}

	private void InitUnitObject()
	{
		m_UnitObject = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UNIT_GAME_NKM_UNIT", "NKM_GAME_UNIT");
		if (m_UnitObject != null)
		{
			m_UnitObject.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
				.Get_GAME_BATTLE_UNIT()
				.transform, worldPositionStays: false);
				NKCUtil.SetGameObjectLocalPos(m_UnitObject.m_Instant, 0f, 0f, 0f);
				m_NKCUnitTouchObject = new NKCUnitTouchObject();
				m_NKCUnitTouchObject.Init();
				m_GameUnitObject = m_UnitObject.m_Instant.GetComponent<NKCGameUnitObject>();
			}
		}

		private void InitSpriteObject()
		{
			Transform transform = m_UnitObject.m_Instant.transform.Find("SPRITE");
			m_SpriteObject = transform.gameObject;
			transform = m_SpriteObject.transform.Find("MAIN_SPRITE");
			m_MainSpriteObject = transform.gameObject;
		}

		private void InitGage()
		{
			Transform transform = m_UnitObject.m_Instant.transform.Find("UNIT_GAGE");
			m_UNIT_GAGE = transform.gameObject;
			m_UNIT_GAGE_RectTransform = m_UNIT_GAGE.GetComponent<RectTransform>();
			m_UNIT_SKILL = m_UNIT_GAGE.transform.Find("UNIT_SKILL").gameObject;
			m_SKILL_BUTTON = m_UNIT_GAGE.transform.Find("UNIT_SKILL/SKILL_BUTTON").gameObject;
			m_SKILL_BUTTON_Btn = m_SKILL_BUTTON.GetComponent<NKCUIComButton>();
			m_NKCUnitTouchObject.GetButton().PointerClick.RemoveAllListeners();
			m_NKCUnitTouchObject.GetButton().PointerClick.AddListener(UseManualSkill);
			m_UNIT_GAGE_PANEL = m_UNIT_GAGE.transform.Find("UNIT_GAGE_PANEL").gameObject;
			m_UNIT_HP_GAGE = m_UNIT_GAGE.transform.Find("UNIT_GAGE_PANEL/UNIT_HP_GAGE").gameObject;
			Transform transform2 = m_UNIT_HP_GAGE.transform.Find("GAGE_BAR_SHADED");
			if (transform2 != null)
			{
				m_UnitHealthBar = transform2.gameObject.GetComponent<NKCUIComHealthBar>();
				if (m_UnitHealthBar == null)
				{
					m_UnitHealthBar = transform2.gameObject.AddComponent<NKCUIComHealthBar>();
				}
				NKCUtil.SetGameobjectActive(m_UnitHealthBar, bValue: true);
				m_UnitHealthBar.Init();
			}
			m_SKILL_GAUGE = m_UNIT_GAGE.transform.Find("UNIT_GAGE_PANEL").GetComponent<NKCUIComSkillGauge>();
			m_UNIT_LEVEL_BG = m_UNIT_GAGE.transform.Find("UNIT_LEVEL/UNIT_LEVEL_BG").gameObject;
			m_UNIT_LEVEL_BG_Image = m_UNIT_LEVEL_BG.GetComponent<Image>();
			m_UNIT_LEVEL = m_UNIT_GAGE.transform.Find("UNIT_LEVEL").gameObject;
			m_UNIT_LEVEL_RectTransform = m_UNIT_LEVEL.GetComponent<RectTransform>();
			m_UNIT_LEVEL_TEXT = m_UNIT_GAGE.transform.Find("UNIT_LEVEL/UNIT_LEVEL_TEXT").gameObject;
			m_UNIT_LEVEL_TEXT_Text = m_UNIT_LEVEL_TEXT.GetComponent<NKCUIComTextUnitLevel>();
			m_UNIT_ARMOR_TYPE = m_UNIT_GAGE.transform.Find("UNIT_LEVEL/UNIT_ARMOR_TYPE").gameObject;
			m_UNIT_ARMOR_TYPE_Image = m_UNIT_ARMOR_TYPE.GetComponent<Image>();
			m_UNIT_ASSIST = m_UNIT_GAGE.transform.Find("UNIT_LEVEL/UNIT_ASSIST").gameObject;
			m_fOrgGageSize = m_UNIT_GAGE_RectTransform.sizeDelta.x;
		}

		private void InitShadow()
		{
			m_NKCASUnitShadow = (NKCASUnitShadow)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitShadow);
			if (m_NKCASUnitShadow != null && m_NKCASUnitShadow.m_ShadowSpriteInstant != null && !(m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant == null))
			{
				m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
					.Get_GAME_BATTLE_UNIT_SHADOW()
					.transform, worldPositionStays: false);
					NKCUtil.SetGameObjectLocalPos(m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant, 0f, 0f, 0f);
					if (m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.activeSelf)
					{
						m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.SetActive(value: false);
					}
				}
			}

			public override void Open()
			{
				base.Open();
			}

			public override void Close()
			{
				if (m_SpriteObject != null)
				{
					Vector3 localScale = m_SpriteObject.transform.localScale;
					localScale.x = 1f;
					localScale.y = 1f;
					m_SpriteObject.transform.localScale = localScale;
				}
				if (m_UnitObject.m_Instant.activeSelf)
				{
					m_UnitObject.m_Instant.SetActive(value: false);
				}
				m_NKCUnitTouchObject.Close();
				if (m_NKCAnimSpine != null)
				{
					m_NKCAnimSpine.ResetParticleSimulSpeedOrg();
				}
				m_NKCAnimSpine.Init();
				m_NKCMotionAfterImage.Clear();
				ActiveObject(bActive: false);
				NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUnitSprite);
				m_NKCASUnitSprite = null;
				NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUnitSpineSprite);
				m_NKCASUnitSpineSprite = null;
				NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUnitMiniMapFace);
				m_NKCASUnitMiniMapFace = null;
				if (m_NKCUnitViewer == null)
				{
					m_NKCUnitViewer = new NKCUnitViewer();
				}
				m_NKCUnitViewer.Init();
				m_UNIT_ARMOR_TYPE_Image.sprite = null;
				foreach (NKCASEffect item in m_llEffect)
				{
					item.Stop();
					item.m_bAutoDie = true;
				}
				m_llEffect.Clear();
				foreach (KeyValuePair<short, NKCASEffect> item2 in m_dicBuffEffect)
				{
					NKCASEffect value = item2.Value;
					value.Stop();
					value.m_bAutoDie = true;
				}
				m_dicBuffEffect.Clear();
				foreach (NKCASEffect item3 in m_llBuffTextEffect)
				{
					item3.Stop();
					item3.m_bAutoDie = true;
				}
				m_llBuffTextEffect.Clear();
				foreach (KeyValuePair<short, NKCASEffect> item4 in m_dicBuffEffectRange)
				{
					NKCASEffect value2 = item4.Value;
					value2.Stop();
					value2.m_bAutoDie = true;
				}
				m_dicBuffEffectRange.Clear();
				foreach (KeyValuePair<NKM_UNIT_STATUS_EFFECT, NKCASEffect> item5 in m_dicStatusEffect)
				{
					NKCASEffect value3 = item5.Value;
					value3.Stop();
					value3.m_bAutoDie = true;
				}
				m_dicStatusEffect.Clear();
				RemoveUnitRespawnTempletEffect(bImmediate: true);
				m_listBuffDelete.Clear();
				m_listBuffDieEvent.Clear();
				m_listNKMStaticBuffDataRuntime.Clear();
				m_listDamageResistUnit.Clear();
				m_lstTriggerRepeatRuntime = null;
				if (m_NKCASDangerChargeUI != null)
				{
					NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASDangerChargeUI);
					m_NKCASDangerChargeUI = null;
				}
				base.Close();
				m_bLoadComplete = false;
			}

			public override void Unload()
			{
				m_NKCASUnitShadow.Unload();
				m_NKCASUnitShadow = null;
				if (m_NKCUnitViewer != null)
				{
					m_NKCUnitViewer.Unload();
					m_NKCUnitViewer = null;
				}
				if (m_NKCASDangerChargeUI != null)
				{
					m_NKCASDangerChargeUI.Unload();
					m_NKCASDangerChargeUI = null;
				}
				m_NKCUnitTouchObject.Unload();
				m_NKCUnitTouchObject = null;
				NKCAssetResourceManager.CloseInstance(m_UnitObject);
				m_UnitObject = null;
				m_NKCGameClient = null;
				m_SpriteObject = null;
				m_MainSpriteObject = null;
				m_UNIT_GAGE = null;
				m_UNIT_GAGE_RectTransform = null;
				m_UNIT_GAGE_PANEL = null;
				m_UNIT_HP_GAGE = null;
				m_UnitHealthBar = null;
				m_SKILL_GAUGE = null;
				m_UNIT_LEVEL_BG = null;
				m_UNIT_LEVEL_BG_Image = null;
				m_UNIT_LEVEL = null;
				m_UNIT_LEVEL_RectTransform = null;
				m_UNIT_LEVEL_TEXT = null;
				m_UNIT_LEVEL_TEXT_Text = null;
				m_UNIT_ARMOR_TYPE = null;
				m_UNIT_ARMOR_TYPE_Image = null;
				m_UNIT_ASSIST = null;
				m_GameUnitObject.Unload();
				m_NKCAnimSpine.Init();
				m_NKCAnimSpine = null;
				m_llEffect.Clear();
				m_dicLoadEffectTemp.Clear();
				m_dicLoadSoundTemp.Clear();
				m_dicBuffEffectRange.Clear();
				m_dicBuffEffect.Clear();
				m_llBuffTextEffect.Clear();
				m_dicStatusEffect.Clear();
				m_RespawnTempletUnitEffect = null;
				m_NKCASUnitSprite = null;
				m_NKCASUnitSpineSprite = null;
				m_NKCASUnitShadow = null;
				m_NKCASUnitMiniMapFace = null;
				m_NKCASDangerChargeUI = null;
				m_NKCMotionAfterImage.Clear();
				m_NKCMotionAfterImage = null;
				base.Unload();
			}

			public void ObjectParentWait()
			{
				if (m_NKCASUnitMiniMapFace != null && m_NKCASUnitMiniMapFace.m_bIsLoaded)
				{
					m_NKCASUnitMiniMapFace.ObjectParentWait();
				}
			}

			public void ObjectParentRestore()
			{
				if (m_NKCASUnitMiniMapFace != null && m_NKCASUnitMiniMapFace.m_bIsLoaded)
				{
					m_NKCASUnitMiniMapFace.ObjectParentRestore();
				}
			}

			public override bool LoadUnit(NKMGame cNKMGame, NKMUnitData cNKMUnitData, short masterGameUnitUID, short gameUnitUID, float fNearTargetRange, NKM_TEAM_TYPE eNKM_TEAM_TYPE, bool bSub, bool bAsync)
			{
				if (!base.LoadUnit(cNKMGame, cNKMUnitData, masterGameUnitUID, gameUnitUID, fNearTargetRange, eNKM_TEAM_TYPE, bSub, bAsync))
				{
					return false;
				}
				m_NKCGameClient = (NKCGameClient)cNKMGame;
				LoadResInst(bAsync);
				m_NKCASUnitSpineSprite = OpenUnitSpineSprite(cNKMUnitData, bSub, bAsync);
				m_NKCASUnitMiniMapFace = LoadUnitMiniMapFace(m_UnitTemplet.m_UnitTempletBase, bSub, bAsync);
				m_SKILL_GAUGE.SetSkillType(m_UnitTemplet.m_UnitTempletBase.StopDefaultCoolTime);
				if (m_NKCUnitViewer == null)
				{
					m_NKCUnitViewer = new NKCUnitViewer();
				}
				if (!cNKMGame.IsEnemy(m_NKCGameClient.m_MyTeam, eNKM_TEAM_TYPE) && !cNKMGame.IsBoss(GetUnitDataGame().m_GameUnitUID))
				{
					m_NKCUnitViewer.LoadUnit(cNKMUnitData, bSub, NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
						.Get_GAME_BATTLE_UNIT_VIEWER(), bAsync);
				}
				else
				{
					m_NKCUnitViewer.Unload();
					m_NKCUnitViewer = null;
				}
				foreach (KeyValuePair<string, NKMUnitState> item in m_UnitTemplet.m_dicNKMUnitState)
				{
					if (item.Value.m_DangerCharge.m_fChargeTime > 0f)
					{
						m_NKCASDangerChargeUI = (NKCASDangerChargeUI)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASDangerChargeUI, "", "", bAsync);
						break;
					}
				}
				for (int i = 0; i < GetUnitTemplet().m_UnitTempletBase.GetSkillCount(); i++)
				{
					NKMUnitSkillTemplet unitSkillTemplet = NKMUnitSkillManager.GetUnitSkillTemplet(GetUnitTemplet().m_UnitTempletBase.GetSkillStrID(i), GetUnitData());
					if (unitSkillTemplet != null && unitSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER)
					{
						m_NKCUnitTouchObject.SetLinkButton(m_SKILL_BUTTON_Btn);
						break;
					}
				}
				if (cNKMUnitData.m_DungeonRespawnUnitTemplet != null)
				{
					LoadEffectInst(cNKMUnitData.m_DungeonRespawnUnitTemplet.GetUnitEffectName(), bAsync);
				}
				return true;
			}

			private NKCASUnitMiniMapFace LoadUnitMiniMapFace(NKMUnitTempletBase cTempletBase, bool bSub, bool bAsync)
			{
				if (bSub && cTempletBase.m_MiniMapFaceNameSub.Length > 1)
				{
					return (NKCASUnitMiniMapFace)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitMiniMapFace, "AB_UNIT_MINI_MAP_FACE", cTempletBase.m_MiniMapFaceNameSub, bAsync);
				}
				return (NKCASUnitMiniMapFace)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitMiniMapFace, "AB_UNIT_MINI_MAP_FACE", cTempletBase.m_MiniMapFaceName, bAsync);
			}

			public override void LoadUnitComplete()
			{
				if (m_bLoadComplete)
				{
					return;
				}
				LoadUnitCompleteRes();
				LoadUnitCompleteUnitSprite();
				LoadUnitCompleteUnitMiniMapFace();
				LoadUnitCompleteGage();
				NKMDungeonRespawnUnitTemplet nKMDungeonRespawnUnitTemplet = ((!HasMasterUnit()) ? m_UnitData.m_DungeonRespawnUnitTemplet : GetMasterUnit()?.GetUnitData().m_DungeonRespawnUnitTemplet);
				float fR;
				float fG;
				float fB;
				if (nKMDungeonRespawnUnitTemplet != null && nKMDungeonRespawnUnitTemplet.HasColorChange())
				{
					NKMColor colorChange = nKMDungeonRespawnUnitTemplet.GetColorChange();
					fR = colorChange.r;
					fG = colorChange.g;
					fB = colorChange.b;
				}
				else
				{
					fR = m_UnitTemplet.m_ColorR;
					fG = m_UnitTemplet.m_ColorG;
					fB = m_UnitTemplet.m_ColorB;
				}
				LoadUnitCompleteColor(fR, fG, fB);
				LoadUnitCompleteShadow();
				LoadUnitCompleteUnitViewer();
				NKCUtil.SetGameObjectLocalPos(m_SpriteObject, m_UnitTemplet.m_SpriteOffsetX, m_UnitTemplet.m_SpriteOffsetY, 0f);
				m_Vector3Temp.Set(m_UnitTemplet.m_SpriteScale, m_UnitTemplet.m_SpriteScale, m_SpriteObject.transform.localScale.z);
				m_SpriteObject.transform.localScale = m_Vector3Temp;
				m_Vector3Temp.Set(m_UnitSyncData.m_PosX, m_UnitSyncData.m_PosZ + m_UnitDataGame.m_RespawnJumpYPos, m_UnitSyncData.m_PosZ);
				m_UnitObject.m_Instant.transform.localPosition = m_Vector3Temp;
				m_NKCAnimSpine.SetAnimObj(m_MainSpriteObject, null, bPreload: true);
				if (m_UnitTemplet.m_bUseMotionBlur)
				{
					m_NKCMotionAfterImage.Init(10, m_NKCASUnitSpineSprite.m_MeshRenderer);
				}
				m_ObjPosX.SetNowValue(m_UnitSyncData.m_PosX);
				m_ObjPosZ.SetNowValue(m_UnitSyncData.m_PosZ + m_UnitDataGame.m_RespawnJumpYPos);
				m_Vector3Temp = m_SpriteObject.transform.localEulerAngles;
				m_Vector3Temp.y = 0f;
				m_SpriteObject.transform.localEulerAngles = m_Vector3Temp;
				if (m_NKCUnitTouchObject != null)
				{
					m_NKCUnitTouchObject.SetSize(GetUnitTemplet());
				}
				ActiveObject(bActive: false);
				if (m_UnitHealthBar != null)
				{
					float num = ((m_NKCGameClient?.GetGameData()?.GameStatRate != null) ? m_NKCGameClient.GetGameData().GameStatRate.GetStatValueRate(NKM_STAT_TYPE.NST_HP) : 1f);
					m_UnitHealthBar.SetStepRatio((num > 0.25f) ? 1f : 0.1f);
				}
				ValidateSound();
				if (m_UnitObject.m_Instant != null)
				{
					bool flag = false;
					if (m_NKCGameClient != null)
					{
						flag = m_NKCGameClient.m_MyTeam == GetTeam();
					}
					Renderer[] componentsInChildren = m_UnitObject.m_Instant.GetComponentsInChildren<Renderer>(includeInactive: true);
					foreach (Renderer renderer in componentsInChildren)
					{
						if (renderer.material != null)
						{
							renderer.material.SetFloat("_IsMine", flag ? 1 : 0);
						}
					}
				}
				m_bLoadComplete = true;
			}

			public override void RespawnUnit(float fPosX, float fPosZ, float fJumpYPos, bool bUseRight = false, bool bRight = true, float fInitHP = 0f, bool bInitHPRate = false, float rollbackTime = 0f)
			{
				base.RespawnUnit(fPosX, fPosZ, fJumpYPos, bUseRight, bRight, fInitHP, bInitHPRate);
				m_ObjPosX.SetNowValue(m_UnitSyncData.m_PosX);
				m_ObjPosZ.SetNowValue(m_UnitSyncData.m_PosZ + m_UnitDataGame.m_RespawnJumpYPos);
				m_DissolveFactor.StopTracking();
				m_DissolveFactor.SetNowValue(0f);
				m_bDissolveEnable = false;
				m_NKCASUnitSpineSprite.SetDissolveBlend(0f);
				m_NKCASUnitSpineSprite.SetDissolveOn(bOn: false);
				m_NKCASUnitSpineSprite.SetColor(1f, 1f, 1f, 1f, bForce: true);
				ActiveObject(bActive: false);
				if (m_NKCUnitViewer != null)
				{
					m_NKCUnitViewer.SetActiveSprite(bActive: false);
					m_NKCUnitViewer.SetActiveShadow(bActive: false);
					m_NKCUnitViewer.StopTimer();
				}
				float zScaleFactor = m_NKCGameClient.GetZScaleFactor(m_ObjPosZ.GetNowValue());
				InitGagePos(zScaleFactor);
				GageInit();
				m_GameUnitObject.SetData(m_NKCGameClient, this);
				m_UNIT_LEVEL_TEXT_Text.SetLevel(m_UnitData, GetUnitFrameData().m_BuffUnitLevel);
				if (m_NKCASUnitShadow != null && m_NKCASUnitShadow.m_NKCComGroupColor != null)
				{
					m_NKCASUnitShadow.m_NKCComGroupColor.SetColor(-1f, -1f, -1f, 0f);
					m_NKCASUnitShadow.m_NKCComGroupColor.SetColor(-1f, -1f, -1f, 0.9f, 2f);
				}
				m_NKCASUnitSpineSprite.SetOverrideMaterial();
				m_fManualSkillUseAck = 0f;
				m_bManualSkillUseStart = false;
				m_bManualSkillUseStateID = 0;
				m_BuffUnitLevelLastUpdate = 0;
				m_BuffDescTextPosYIndex = 0;
				SetShowUI();
				foreach (KeyValuePair<short, NKMBuffSyncData> dicBuffDatum in GetUnitSyncData().m_dicBuffData)
				{
					NKMBuffTemplet buffTempletByID = NKMBuffManager.GetBuffTempletByID(dicBuffDatum.Value.m_BuffID);
					if (buffTempletByID != null && buffTempletByID.m_UnitLevel != 0)
					{
						m_bBuffChangedThisFrame = true;
						m_bBuffUnitLevelChangedThisFrame = true;
						break;
					}
				}
				AddUnitRespawnTempletEffect(m_UnitData.m_DungeonRespawnUnitTemplet);
			}

			private void LoadUnitCompleteUnitSprite()
			{
				m_NKCASUnitSpineSprite.m_UnitSpineSpriteInstant.m_Instant.transform.SetParent(m_MainSpriteObject.transform, worldPositionStays: false);
				NKCUtil.SetGameObjectLocalPos(m_NKCASUnitSpineSprite.m_UnitSpineSpriteInstant.m_Instant, 0f, 0f, 0f);
				NKCScenManager.GetScenManager().ForceRender(m_NKCASUnitSpineSprite.m_MeshRenderer);
			}

			private void LoadUnitCompleteUnitMiniMapFace()
			{
				if (m_NKCASUnitMiniMapFace == null || m_NKCASUnitMiniMapFace.m_MarkerGreen == null || m_NKCASUnitMiniMapFace.m_MarkerRed == null)
				{
					Debug.LogError("LoadUnitCompleteUnitMiniMapFace null" + GetUnitTemplet().m_UnitTempletBase.m_UnitStrID);
				}
				else if (!m_NKMGame.IsEnemy(m_NKCGameClient.m_MyTeam, m_UnitDataGame.m_NKM_TEAM_TYPE_ORG))
				{
					m_NKCASUnitMiniMapFace.m_MarkerGreen.SetActive(value: true);
					m_NKCASUnitMiniMapFace.m_MarkerRed.SetActive(value: false);
				}
				else
				{
					m_NKCASUnitMiniMapFace.m_MarkerGreen.SetActive(value: false);
					m_NKCASUnitMiniMapFace.m_MarkerRed.SetActive(value: true);
				}
			}

			private void LoadUnitCompleteGage()
			{
				SetShowUI();
				if (m_UnitTemplet.m_listSkillStateData.Count > 0 && m_UnitTemplet.m_listSkillStateData[0] != null && IsStateUnlocked(m_UnitTemplet.m_listSkillStateData[0]))
				{
					m_SKILL_GAUGE.SetActiveSkillGauge(bActive: true);
				}
				else
				{
					m_SKILL_GAUGE.SetActiveSkillGauge(bActive: false);
				}
				if (m_UnitTemplet.m_listHyperSkillStateData.Count > 0 && m_UnitTemplet.m_listHyperSkillStateData[0] != null && IsStateUnlocked(m_UnitTemplet.m_listHyperSkillStateData[0]))
				{
					m_SKILL_GAUGE.SetActiveHyperGauge(bActive: true);
				}
				else
				{
					m_SKILL_GAUGE.SetActiveHyperGauge(bActive: false);
				}
				ChangeLevelGageBG();
				if (m_UnitHealthBar != null)
				{
					m_UnitHealthBar.SetColor(m_NKMGame.IsEnemy(m_NKCGameClient.m_MyTeam, m_UnitDataGame.m_NKM_TEAM_TYPE_ORG));
				}
				m_UNIT_LEVEL_TEXT_Text.SetLevel(m_UnitData, GetUnitFrameData().m_BuffUnitLevel);
				float zScaleFactor = m_NKCGameClient.GetZScaleFactor(m_ObjPosZ.GetNowValue());
				InitGagePos(zScaleFactor);
				GageInit();
				m_GameUnitObject.SetData(m_NKCGameClient, this);
			}

			private void GageInit()
			{
				m_GageWide.SetNowValue(0f);
				Vector2 sizeDelta = m_UNIT_GAGE_RectTransform.sizeDelta;
				sizeDelta.x = 0f;
				m_UNIT_GAGE_RectTransform.sizeDelta = sizeDelta;
				sizeDelta = m_UNIT_LEVEL_RectTransform.anchoredPosition;
				sizeDelta.x = m_UNIT_LEVEL_RectTransform.sizeDelta.x * 0.5f;
				m_UNIT_LEVEL_RectTransform.anchoredPosition = sizeDelta;
				m_UNIT_GAGE_PANEL.SetActive(value: false);
				m_SKILL_GAUGE.SetSkillCoolTime(0f);
				m_SKILL_GAUGE.SetHyperCoolTime(0f);
				m_UNIT_ARMOR_TYPE_Image.sprite = NKCResourceUtility.GetOrLoadUnitRoleIconInGame(GetUnitTemplet().m_UnitTempletBase);
				if (!m_NKCGameClient.GetGameData().GetTeamData(GetUnitDataGame().m_NKM_TEAM_TYPE_ORG).IsAssistUnit(GetUnitDataGame().m_UnitUID))
				{
					if (m_UNIT_ASSIST.activeSelf)
					{
						m_UNIT_ASSIST.SetActive(value: false);
					}
				}
				else if (!m_UNIT_ASSIST.activeSelf)
				{
					m_UNIT_ASSIST.SetActive(value: true);
				}
			}

			private void InitGagePos(float fZScaleFactor)
			{
				if (m_NKCASUnitSpineSprite.m_Bone_Move != null)
				{
					InitGagePosSpine(fZScaleFactor);
				}
			}

			private void InitGagePosSpine(float fZScaleFactor)
			{
				m_GageOffsetPosX.Init();
				m_GageOffsetPosY.Init();
				if (m_UnitSyncData.m_bRight)
				{
					m_fGagePosX = m_UnitTemplet.m_fGageOffsetX + m_NKCASUnitSpineSprite.m_Bone_Move.WorldX * m_NKCASUnitSpineSprite.m_SPINE_SkeletonAnimation.transform.localScale.x;
					m_fGagePosY = m_UnitTemplet.m_fGageOffsetY + m_NKCASUnitSpineSprite.m_Bone_Move.WorldY * m_NKCASUnitSpineSprite.m_SPINE_SkeletonAnimation.transform.localScale.y;
					m_fGagePosY *= fZScaleFactor;
				}
				else
				{
					m_fGagePosX = 0f - m_UnitTemplet.m_fGageOffsetX - m_NKCASUnitSpineSprite.m_Bone_Move.WorldX * m_NKCASUnitSpineSprite.m_SPINE_SkeletonAnimation.transform.localScale.x;
					m_fGagePosY = m_UnitTemplet.m_fGageOffsetY - m_NKCASUnitSpineSprite.m_Bone_Move.WorldY * m_NKCASUnitSpineSprite.m_SPINE_SkeletonAnimation.transform.localScale.y;
					m_fGagePosY *= fZScaleFactor;
				}
				Vector3 anchoredPosition3D = m_UNIT_GAGE_RectTransform.anchoredPosition3D;
				anchoredPosition3D.Set(m_fGagePosX, m_fGagePosY, 0f);
				m_UNIT_GAGE_RectTransform.anchoredPosition3D = anchoredPosition3D;
			}

			private void UpdateGagePos(float fZScaleFactor)
			{
				if (m_NKCASUnitSpineSprite.m_Bone_Move != null)
				{
					m_GageOffsetPosX.Update(m_DeltaTime);
					m_GageOffsetPosY.Update(m_DeltaTime);
					if (m_UnitSyncData.m_bRight)
					{
						m_fGagePosX = m_UnitTemplet.m_fGageOffsetX + m_GageOffsetPosX.GetNowValue() + m_NKCASUnitSpineSprite.m_Bone_Move.WorldX * m_NKCASUnitSpineSprite.m_SPINE_SkeletonAnimation.transform.localScale.x;
						m_fGagePosY = m_UnitTemplet.m_fGageOffsetY + m_GageOffsetPosY.GetNowValue() + m_NKCASUnitSpineSprite.m_Bone_Move.WorldY * m_NKCASUnitSpineSprite.m_SPINE_SkeletonAnimation.transform.localScale.y;
						m_fGagePosY *= fZScaleFactor;
					}
					else
					{
						m_fGagePosX = 0f - m_UnitTemplet.m_fGageOffsetX - m_GageOffsetPosX.GetNowValue() - m_NKCASUnitSpineSprite.m_Bone_Move.WorldX * m_NKCASUnitSpineSprite.m_SPINE_SkeletonAnimation.transform.localScale.x;
						m_fGagePosY = m_UnitTemplet.m_fGageOffsetY - m_GageOffsetPosY.GetNowValue() - m_NKCASUnitSpineSprite.m_Bone_Move.WorldY * m_NKCASUnitSpineSprite.m_SPINE_SkeletonAnimation.transform.localScale.y;
						m_fGagePosY *= fZScaleFactor;
					}
					Vector3 anchoredPosition3D = m_UNIT_GAGE_RectTransform.anchoredPosition3D;
					anchoredPosition3D.Set(m_fGagePosX, m_fGagePosY, 0f);
					m_UNIT_GAGE_RectTransform.anchoredPosition3D = anchoredPosition3D;
				}
			}

			private void ChangeLevelGageBG()
			{
				Sprite sprite = null;
				if (!m_NKMGame.IsEnemy(m_NKCGameClient.m_MyTeam, m_UnitDataGame.m_NKM_TEAM_TYPE_ORG))
				{
					sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UNIT_GAME_NKM_UNIT_SPRITE", "AB_UNIT_GAME_NKM_UNIT_LEVEL_BG");
					if (sprite != null)
					{
						m_UNIT_LEVEL_BG_Image.sprite = sprite;
					}
				}
				else
				{
					sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UNIT_GAME_NKM_UNIT_SPRITE", "AB_UNIT_GAME_NKM_UNIT_LEVEL_BG_ENEMY");
					if (sprite != null)
					{
						m_UNIT_LEVEL_BG_Image.sprite = sprite;
					}
				}
			}

			private void LoadUnitCompleteColor(float fR, float fG, float fB)
			{
				m_UnitFrameData.m_ColorR.SetNowValue(fR);
				m_UnitFrameData.m_ColorG.SetNowValue(fG);
				m_UnitFrameData.m_ColorB.SetNowValue(fB);
				m_NKCASUnitSpineSprite.SetColor(m_UnitFrameData.m_ColorR.GetNowValue(), m_UnitFrameData.m_ColorG.GetNowValue(), m_UnitFrameData.m_ColorB.GetNowValue());
			}

			private void LoadUnitCompleteShadow()
			{
				if (m_NKCASUnitShadow != null && m_NKCASUnitShadow.m_ShadowSpriteInstant != null && !(m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant == null))
				{
					if (!m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.activeSelf)
					{
						m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.SetActive(value: true);
					}
					m_Vector3Temp.Set(m_UnitTemplet.m_fShadowScaleX, m_UnitTemplet.m_fShadowScaleY, 1f);
					m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.transform.localScale = m_Vector3Temp;
					m_Vector3Temp.Set(m_UnitSyncData.m_PosX, m_UnitSyncData.m_PosZ, m_UnitSyncData.m_PosZ);
					m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.transform.localPosition = m_Vector3Temp;
					bool flag = true;
					flag = !m_NKMGame.IsEnemy(m_NKCGameClient.m_MyTeam, GetUnitDataGame().m_NKM_TEAM_TYPE_ORG);
					m_NKCASUnitShadow.SetShadowType(m_UnitTemplet.m_NKC_TEAM_COLOR_TYPE, flag, m_UnitTemplet.m_UnitTempletBase.IsRearmUnit);
					m_NKCASUnitShadow.m_NKCComGroupColor.SetColor(-1f, -1f, -1f, 0f);
					m_NKCASUnitShadow.m_NKCComGroupColor.SetColor(-1f, -1f, -1f, 0.9f, 2f);
				}
			}

			private void LoadUnitCompleteUnitViewer()
			{
				bool flag = true;
				flag = !m_NKMGame.IsEnemy(m_NKCGameClient.m_MyTeam, GetUnitDataGame().m_NKM_TEAM_TYPE_ORG);
				if (m_NKCUnitViewer == null)
				{
					return;
				}
				try
				{
					m_NKCUnitViewer.LoadUnitComplete();
					m_NKCUnitViewer.SetShadowType(flag);
					m_NKCUnitViewer.SetActiveSprite(bActive: false);
					m_NKCUnitViewer.SetActiveShadow(bActive: false);
					m_NKCUnitViewer.StopTimer();
					m_NKCUnitViewer.SetUnitUID(GetUnitData().m_UnitUID);
				}
				catch (Exception ex)
				{
					if (GetUnitTemplet() != null && GetUnitTemplet().m_UnitTempletBase != null)
					{
						Debug.LogError(GetUnitTemplet().m_UnitTempletBase.m_UnitStrID + ": LoadUnitCompleteUnitViewer Failed with exception : " + ex.Message);
					}
					else
					{
						Debug.LogError("Unknown unit : LoadUnitCompleteUnitViewer Failed with exception : " + ex.Message);
					}
				}
			}

			private void LoadResInst(bool bAsync)
			{
				Dictionary<string, NKMUnitState>.Enumerator enumerator = m_UnitTemplet.m_dicNKMUnitState.GetEnumerator();
				while (enumerator.MoveNext())
				{
					NKMUnitState value = enumerator.Current.Value;
					if (value == null)
					{
						continue;
					}
					for (int i = 0; i < value.m_listNKMEventSound.Count; i++)
					{
						NKMEventSound nKMEventSound = value.m_listNKMEventSound[i];
						if (nKMEventSound != null)
						{
							LoadEventSound(nKMEventSound, bAsync);
						}
					}
					for (int j = 0; j < value.m_listNKMEventAttack.Count; j++)
					{
						NKMEventAttack nKMEventAttack = value.m_listNKMEventAttack[j];
						if (nKMEventAttack != null)
						{
							LoadAttackResInst(nKMEventAttack, bAsync);
						}
					}
					for (int k = 0; k < value.m_listNKMEventEffect.Count; k++)
					{
						NKMEventEffect nKMEventEffect = value.m_listNKMEventEffect[k];
						if (nKMEventEffect != null)
						{
							LoadEffectInst(nKMEventEffect, bAsync);
						}
					}
					NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
					if (gameOptionData == null || gameOptionData.ViewSkillCutIn)
					{
						m_bHyperCutinLoaded = true;
						for (int l = 0; l < value.m_listNKMEventHyperSkillCutIn.Count; l++)
						{
							NKMEventHyperSkillCutIn nKMEventHyperSkillCutIn = value.m_listNKMEventHyperSkillCutIn[l];
							if (nKMEventHyperSkillCutIn != null)
							{
								string skillCutin = NKMSkinManager.GetSkillCutin(GetUnitData(), nKMEventHyperSkillCutIn.m_CutInEffectName);
								LoadEffectInst(skillCutin, bAsync);
							}
						}
					}
					else
					{
						m_bHyperCutinLoaded = false;
					}
					for (int m = 0; m < value.m_listNKMEventDamageEffect.Count; m++)
					{
						NKMEventDamageEffect nKMEventDamageEffect = value.m_listNKMEventDamageEffect[m];
						if (nKMEventDamageEffect != null)
						{
							LoadDEEffectInst(nKMEventDamageEffect, bAsync);
						}
					}
					for (int n = 0; n < value.m_listNKMEventBuff.Count; n++)
					{
						NKMEventBuff nKMEventBuff = value.m_listNKMEventBuff[n];
						if (nKMEventBuff != null)
						{
							NKMBuffTemplet buffTempletByStrID = NKMBuffManager.GetBuffTempletByStrID(nKMEventBuff.m_BuffStrID);
							if (buffTempletByStrID == null)
							{
								Debug.LogError("Bufftemplet " + nKMEventBuff.m_BuffStrID + " does not exist!!");
							}
							LoadBuff(buffTempletByStrID, bAsync);
						}
					}
				}
				if (m_UnitTemplet.m_dicTriggerSet != null)
				{
					foreach (KeyValuePair<int, NKMUnitTriggerSet> item in m_UnitTemplet.m_dicTriggerSet)
					{
						foreach (NKMUnitStateEventOneTime @event in item.Value.Events)
						{
							if (@event is NKMEventSound)
							{
								if (@event is NKMEventSound cNKMEventSound)
								{
									LoadEventSound(cNKMEventSound, bAsync);
								}
							}
							else if (@event is NKMEventEffect)
							{
								if (@event is NKMEventEffect cNKMEventEffect)
								{
									LoadEffectInst(cNKMEventEffect, bAsync);
								}
							}
							else if (@event is NKMEventHyperSkillCutIn)
							{
								NKCGameOptionData gameOptionData2 = NKCScenManager.GetScenManager().GetGameOptionData();
								NKMEventHyperSkillCutIn nKMEventHyperSkillCutIn2 = @event as NKMEventHyperSkillCutIn;
								if (gameOptionData2 == null || gameOptionData2.ViewSkillCutIn)
								{
									m_bHyperCutinLoaded = true;
									if (nKMEventHyperSkillCutIn2 != null)
									{
										string skillCutin2 = NKMSkinManager.GetSkillCutin(GetUnitData(), nKMEventHyperSkillCutIn2.m_CutInEffectName);
										LoadEffectInst(skillCutin2, bAsync);
									}
								}
							}
							else if (@event is NKMEventDamageEffect)
							{
								if (@event is NKMEventDamageEffect cNKMEventDamageEffect)
								{
									LoadDEEffectInst(cNKMEventDamageEffect, bAsync);
								}
							}
							else if (@event is NKMEventBuff && @event is NKMEventBuff nKMEventBuff2)
							{
								NKMBuffTemplet buffTempletByStrID2 = NKMBuffManager.GetBuffTempletByStrID(nKMEventBuff2.m_BuffStrID);
								if (buffTempletByStrID2 == null)
								{
									Debug.LogError("Bufftemplet " + nKMEventBuff2.m_BuffStrID + " does not exist!!");
								}
								LoadBuff(buffTempletByStrID2, bAsync);
							}
						}
					}
				}
				LoadSound("FX_UI_DUNGEON_RESPONE", bAsync);
			}

			private void LoadEventSound(NKMEventSound cNKMEventSound, bool bAsync)
			{
				if (m_UnitData == null || cNKMEventSound.IsRightSkin(m_UnitData.m_SkinID))
				{
					List<string> targetSoundList = cNKMEventSound.GetTargetSoundList(m_UnitData);
					for (int i = 0; i < targetSoundList.Count; i++)
					{
						LoadSound(targetSoundList[i], bAsync);
					}
				}
			}

			private void ValidateSound()
			{
				Dictionary<string, NKMUnitState>.Enumerator enumerator = m_UnitTemplet.m_dicNKMUnitState.GetEnumerator();
				while (enumerator.MoveNext())
				{
					NKMUnitState value = enumerator.Current.Value;
					if (value == null)
					{
						continue;
					}
					for (int i = 0; i < value.m_listNKMEventSound.Count; i++)
					{
						NKMEventSound nKMEventSound = value.m_listNKMEventSound[i];
						if (nKMEventSound != null)
						{
							ValidateSoundEvent(nKMEventSound);
						}
					}
				}
				if (m_UnitTemplet.m_dicTriggerSet == null)
				{
					return;
				}
				foreach (KeyValuePair<int, NKMUnitTriggerSet> item in m_UnitTemplet.m_dicTriggerSet)
				{
					foreach (NKMUnitStateEventOneTime @event in item.Value.Events)
					{
						if (@event is NKMEventSound && @event is NKMEventSound cNKMEventSound)
						{
							ValidateSoundEvent(cNKMEventSound);
						}
					}
				}
			}

			private void ValidateSoundEvent(NKMEventSound cNKMEventSound)
			{
				if (m_UnitData == null || cNKMEventSound.IsRightSkin(m_UnitData.m_SkinID))
				{
					cNKMEventSound.GetTargetSoundList(m_UnitData).RemoveAll(CheckSoundNonExist);
				}
			}

			private bool CheckSoundNonExist(string soundAssetName)
			{
				string bundleName = NKCAssetResourceManager.GetBundleName(soundAssetName, bIgnoreNotFoundError: true);
				if (string.IsNullOrEmpty(bundleName))
				{
					return true;
				}
				if (!NKCAssetResourceManager.IsAssetExists(bundleName, soundAssetName, loadUnloadedBundle: true))
				{
					Debug.LogError("soundAsset " + soundAssetName + " does not exist!");
					return true;
				}
				return false;
			}

			private void LoadBuff(NKMBuffTemplet cNKMBuffTemplet, bool bAsync)
			{
				if (cNKMBuffTemplet != null)
				{
					LoadEffectInst(cNKMBuffTemplet.m_RangeEffectName, bAsync);
					LoadEffectInst(cNKMBuffTemplet.GetMasterEffectName(m_UnitData.m_SkinID), bAsync);
					LoadEffectInst(cNKMBuffTemplet.GetSlaveEffectName(m_UnitData.m_SkinID), bAsync);
					LoadEffectInst(cNKMBuffTemplet.m_BarrierDamageEffectName, bAsync);
					LoadDamageTemplet(cNKMBuffTemplet.m_DTStart, bAsync);
					LoadDamageTemplet(cNKMBuffTemplet.m_DTEnd, bAsync);
					LoadDamageTemplet(cNKMBuffTemplet.m_DTDispel, bAsync);
				}
			}

			private void LoadDamageTemplet(NKMDamageTemplet cNKMDamageTemplet, bool bAsync)
			{
				if (cNKMDamageTemplet != null)
				{
					if (cNKMDamageTemplet.m_HitSoundName != null)
					{
						LoadSound(cNKMDamageTemplet.m_HitSoundName, bAsync);
					}
					LoadEffectInst(cNKMDamageTemplet.m_HitEffect, bAsync);
				}
			}

			private void LoadSound(string assetName, bool bAsync)
			{
				if (assetName.Length > 1 && !m_dicLoadSoundTemp.ContainsKey(assetName))
				{
					m_dicLoadSoundTemp.Add(assetName, NKCAssetResourceManager.OpenResource<AudioClip>(assetName, bAsync));
				}
			}

			private void LoadAttackResInst(NKMEventAttack cNKMEventAttack, bool bAsync)
			{
				if (cNKMEventAttack.m_SoundName != null)
				{
					LoadSound(cNKMEventAttack.m_SoundName, bAsync);
				}
				LoadEffectInst(cNKMEventAttack.m_EffectName, bAsync);
				NKMDamageTemplet templetByStrID = NKMDamageManager.GetTempletByStrID(cNKMEventAttack.m_DamageTempletName);
				if (templetByStrID != null)
				{
					if (templetByStrID.m_HitSoundName != null)
					{
						LoadSound(templetByStrID.m_HitSoundName, bAsync);
					}
					LoadEffectInst(templetByStrID.m_HitEffect, bAsync);
				}
			}

			private void LoadEffectInst(string effectName, bool bAsync)
			{
				if (effectName.Length > 1 && !m_dicLoadEffectTemp.ContainsKey(effectName))
				{
					NKCASEffect value = (NKCASEffect)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASEffect, effectName, effectName, bAsync);
					m_dicLoadEffectTemp.Add(effectName, value);
				}
			}

			private void LoadEffectInst(NKMEventEffect cNKMEventEffect, bool bAsync)
			{
				if (cNKMEventEffect != null && cNKMEventEffect.IsRightSkin(m_UnitData.m_SkinID))
				{
					LoadEffectInst(cNKMEventEffect.GetEffectName(m_UnitData), bAsync);
				}
			}

			private void LoadDEEffectInst(NKMEventDamageEffect cNKMEventDamageEffect, bool bAsync)
			{
				LoadDEEffectInst(cNKMEventDamageEffect.m_DEName, bAsync);
				if (cNKMEventDamageEffect.m_DENamePVP.Length > 1)
				{
					LoadDEEffectInst(cNKMEventDamageEffect.m_DENamePVP, bAsync);
				}
			}

			private void LoadDEEffectInst(string DEName, bool bAsync)
			{
				NKMDamageEffectTemplet dETemplet = NKMDETempletManager.GetDETemplet(DEName);
				if (dETemplet != null)
				{
					LoadEffectInst(dETemplet.GetMainEffectName(m_UnitData.m_SkinID), bAsync);
					Dictionary<string, NKMDamageEffectState>.Enumerator enumerator = dETemplet.m_dicNKMState.GetEnumerator();
					while (enumerator.MoveNext())
					{
						NKMDamageEffectState value = enumerator.Current.Value;
						LoadDEStateEffectInst(value, bAsync);
					}
				}
			}

			private bool LoadDEStateEffectInst(NKMDamageEffectState cNKMDamageEffectState, bool bAsync)
			{
				for (int i = 0; i < cNKMDamageEffectState.m_listNKMEventSound.Count; i++)
				{
					NKMEventSound nKMEventSound = cNKMDamageEffectState.m_listNKMEventSound[i];
					if (nKMEventSound != null && nKMEventSound.IsRightSkin(m_UnitData.m_SkinID))
					{
						List<string> targetSoundList = nKMEventSound.GetTargetSoundList(m_UnitData);
						for (int j = 0; j < targetSoundList.Count; j++)
						{
							LoadSound(targetSoundList[j], bAsync);
						}
					}
				}
				for (int k = 0; k < cNKMDamageEffectState.m_listNKMEventAttack.Count; k++)
				{
					NKMEventAttack cNKMEventAttack = cNKMDamageEffectState.m_listNKMEventAttack[k];
					LoadAttackResInst(cNKMEventAttack, bAsync);
				}
				for (int l = 0; l < cNKMDamageEffectState.m_listNKMEventEffect.Count; l++)
				{
					NKMEventEffect cNKMEventEffect = cNKMDamageEffectState.m_listNKMEventEffect[l];
					LoadEffectInst(cNKMEventEffect, bAsync);
				}
				for (int m = 0; m < cNKMDamageEffectState.m_listNKMEventBuff.Count; m++)
				{
					NKMEventBuff nKMEventBuff = cNKMDamageEffectState.m_listNKMEventBuff[m];
					if (nKMEventBuff != null)
					{
						NKMBuffTemplet buffTempletByStrID = NKMBuffManager.GetBuffTempletByStrID(nKMEventBuff.m_BuffStrID);
						if (buffTempletByStrID == null)
						{
							Debug.LogError("Bufftemplet " + nKMEventBuff.m_BuffStrID + " does not exist!!");
						}
						LoadBuff(buffTempletByStrID, bAsync);
					}
				}
				return true;
			}

			public void LoadUnitCompleteRes()
			{
				foreach (KeyValuePair<string, NKCASEffect> item in m_dicLoadEffectTemp)
				{
					NKCASEffect value = item.Value;
					if (value != null)
					{
						NKCScenManager.GetScenManager().GetObjectPool().CloseObj(value);
					}
				}
				m_dicLoadEffectTemp.Clear();
				foreach (KeyValuePair<string, NKCAssetResourceData> item2 in m_dicLoadSoundTemp)
				{
					NKCAssetResourceData value2 = item2.Value;
					if (value2 != null)
					{
						NKCAssetResourceManager.CloseResource(value2);
					}
				}
				m_dicLoadSoundTemp.Clear();
			}

			private void AddUnitRespawnTempletEffect(NKMDungeonRespawnUnitTemplet cNKMDungeonRespawnUnitTemplet)
			{
				if (cNKMDungeonRespawnUnitTemplet == null)
				{
					return;
				}
				NKMDungeonRespawnUnitTemplet.UnitEffect unitEffect = cNKMDungeonRespawnUnitTemplet.m_UnitEffect;
				if (unitEffect != NKMDungeonRespawnUnitTemplet.UnitEffect.NONE && unitEffect == NKMDungeonRespawnUnitTemplet.UnitEffect.SHADOW)
				{
					float offsetY = 0f;
					string boneName = "";
					float fBuffEffectScaleFactor = GetUnitTemplet().m_fBuffEffectScaleFactor;
					string unitEffectName = cNKMDungeonRespawnUnitTemplet.GetUnitEffectName();
					NKCASEffect nKCASEffect = m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, unitEffectName, unitEffectName, NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, m_UnitSyncData.m_PosX, m_UnitSyncData.m_PosZ + m_UnitSyncData.m_JumpYPos, m_UnitSyncData.m_PosZ, bRight: true, fBuffEffectScaleFactor, 0f, offsetY, -1f, m_bUseZtoY: false, 0f, bUseZScale: true, boneName, bUseBoneRotate: false, bAutoDie: false, "BASE");
					if (nKCASEffect != null)
					{
						m_llEffect.AddLast(nKCASEffect);
						m_RespawnTempletUnitEffect = nKCASEffect;
					}
				}
			}

			private void RemoveUnitRespawnTempletEffect(bool bImmediate = false)
			{
				if (m_RespawnTempletUnitEffect != null)
				{
					if (bImmediate)
					{
						m_RespawnTempletUnitEffect.Stop();
						m_RespawnTempletUnitEffect.m_bAutoDie = true;
					}
					else if (m_RespawnTempletUnitEffect.m_bEndAnim)
					{
						m_RespawnTempletUnitEffect.m_bAutoDie = true;
						m_RespawnTempletUnitEffect.PlayAnim("END");
					}
					else
					{
						m_NKCGameClient.GetNKCEffectManager().DeleteEffect(m_RespawnTempletUnitEffect.m_EffectUID);
					}
					m_RespawnTempletUnitEffect = null;
				}
			}

			public override void SetDying(bool bForce, bool bUnitChange = false)
			{
				base.SetDying(bForce, bUnitChange);
				if (m_UNIT_GAGE.activeSelf)
				{
					m_UNIT_GAGE.SetActive(value: false);
				}
			}

			public override bool SetDie(bool bCheckAllDie = true)
			{
				bool result = base.SetDie(bCheckAllDie);
				if (m_NKMGame.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_DEV || m_UnitTemplet.m_bDieDeActive)
				{
					ActiveObject(bActive: false);
				}
				return result;
			}

			public void SetShowUI()
			{
				if (!m_NKCGameClient.IsShowUI())
				{
					NKCUtil.SetGameobjectActive(m_UNIT_GAGE, bValue: false);
				}
				else if (m_UnitStateNow != null && !m_UnitStateNow.m_bShowGage)
				{
					NKCUtil.SetGameobjectActive(m_UNIT_GAGE, bValue: false);
				}
				else if (m_UnitData.m_DungeonRespawnUnitTemplet != null && m_UnitData.m_DungeonRespawnUnitTemplet.m_eShowGage != NKMDungeonRespawnUnitTemplet.ShowGageOverride.Default)
				{
					NKCUtil.SetGameobjectActive(m_UNIT_GAGE, m_UnitData.m_DungeonRespawnUnitTemplet.m_eShowGage == NKMDungeonRespawnUnitTemplet.ShowGageOverride.Show);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_UNIT_GAGE, m_UnitTemplet.m_bShowGage);
				}
			}

			public void ActiveObject(bool bActive)
			{
				if (m_UnitObject.m_Instant.activeSelf == !bActive)
				{
					m_UnitObject.m_Instant.SetActive(bActive);
				}
				if (bActive)
				{
					if (m_NKCASUnitSpineSprite != null && !m_NKCASUnitSpineSprite.m_SPINE_SkeletonAnimation.activeSelf)
					{
						m_NKCASUnitSpineSprite.m_SPINE_SkeletonAnimation.SetActive(value: true);
					}
				}
				else
				{
					m_NKCMotionAfterImage.StopMotionImage();
					m_NKCUnitTouchObject.ActiveObject(bActive: false);
				}
				if (m_NKCASUnitShadow != null && m_NKCASUnitShadow.m_ShadowSpriteInstant != null && m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant != null && m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.activeSelf == !bActive)
				{
					m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.SetActive(bActive);
				}
				if (m_NKCASUnitMiniMapFace != null && m_NKCASUnitMiniMapFace.m_MiniMapFaceInstant != null && m_NKCASUnitMiniMapFace.m_MiniMapFaceInstant.m_Instant != null && m_NKCASUnitMiniMapFace.m_MiniMapFaceInstant.m_Instant.activeSelf == !bActive)
				{
					m_NKCASUnitMiniMapFace.m_MiniMapFaceInstant.m_Instant.SetActive(bActive);
				}
				foreach (KeyValuePair<short, NKCASEffect> item in m_dicBuffEffect)
				{
					NKCASEffect value = item.Value;
					if (value == null)
					{
						continue;
					}
					if (!bActive)
					{
						if (value.m_bEndAnim)
						{
							value.PlayAnim("END");
						}
						else if (value.m_EffectInstant.m_Instant.activeSelf == !bActive)
						{
							value.m_EffectInstant.m_Instant.SetActive(bActive);
						}
					}
					else if (value.m_EffectInstant.m_Instant.activeSelf == !bActive)
					{
						value.m_EffectInstant.m_Instant.SetActive(bActive);
						value.PlayAnim("BASE");
					}
				}
				foreach (NKCASEffect item2 in m_llBuffTextEffect)
				{
					if (item2 != null && !bActive)
					{
						item2.SetDie();
					}
				}
				foreach (KeyValuePair<short, NKCASEffect> item3 in m_dicBuffEffectRange)
				{
					NKCASEffect value2 = item3.Value;
					if (value2 != null && value2.m_EffectInstant.m_Instant.activeSelf == !bActive)
					{
						value2.m_EffectInstant.m_Instant.SetActive(bActive);
					}
				}
				if (bActive)
				{
					return;
				}
				foreach (KeyValuePair<NKM_UNIT_STATUS_EFFECT, NKCASEffect> item4 in m_dicStatusEffect)
				{
					NKCASEffect value3 = item4.Value;
					if (value3 != null)
					{
						if (value3.m_bEndAnim)
						{
							value3.m_bAutoDie = true;
							value3.PlayAnim("END");
						}
						else
						{
							m_NKCGameClient.GetNKCEffectManager().DeleteEffect(value3.m_EffectUID);
						}
					}
				}
				m_dicStatusEffect.Clear();
				RemoveUnitRespawnTempletEffect();
			}

			public override void Update(float deltaTime)
			{
				base.Update(deltaTime);
			}

			protected override void StateEnd()
			{
				base.StateEnd();
				m_NKCGameClient.SetCameraFocusUnitOut(0);
			}

			protected override void StateStart()
			{
				for (int i = 0; i < m_listSoundUID.Count; i++)
				{
					NKCSoundManager.StopSound(m_listSoundUID[i]);
				}
				m_listSoundUID.Clear();
				base.StateStart();
				m_GageOffsetPosX.SetTracking(m_UnitStateNow.m_fGageOffsetX, 0.4f, TRACKING_DATA_TYPE.TDT_SLOWER);
				m_GageOffsetPosY.SetTracking(m_UnitStateNow.m_fGageOffsetY, 0.4f, TRACKING_DATA_TYPE.TDT_SLOWER);
				if (m_UnitStateNow == null)
				{
					return;
				}
				if (m_UnitObject.m_Instant.activeSelf && m_NKCAnimSpine != null)
				{
					m_NKCAnimSpine.SetPlaySpeed(m_UnitFrameData.m_fAnimSpeed);
					if (m_UnitStateNow.m_AnimName.Length > 1)
					{
						m_NKCAnimSpine.Play(m_UnitStateNow.m_AnimName, bLoop: false, m_UnitStateNow.m_fAnimStartTime);
					}
				}
				SetShowUI();
				if (m_UnitStateNow.m_NKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_DIE && (m_NKMGame.GetGameData().m_NKMGameTeamDataA.m_MainShip == null || GetUnitData().m_UnitUID != m_NKMGame.GetGameData().m_NKMGameTeamDataA.m_MainShip.m_UnitUID) && (m_NKMGame.GetGameData().m_NKMGameTeamDataB.m_MainShip == null || GetUnitData().m_UnitUID != m_NKMGame.GetGameData().m_NKMGameTeamDataB.m_MainShip.m_UnitUID) && m_NKCASUnitShadow != null && m_NKCASUnitShadow.m_NKCComGroupColor != null)
				{
					m_NKCASUnitShadow.m_NKCComGroupColor.SetColor(-1f, -1f, -1f, 0f, 3f);
				}
				if (m_UnitStateNow.m_bSkillCutIn || m_UnitStateNow.m_bHyperSkillCutIn)
				{
					NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
					if (gameOptionData == null || gameOptionData.ViewSkillCutIn)
					{
						Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, m_UnitData);
						if (sprite != null)
						{
							string skillName = "";
							NKMUnitSkillTemplet skillTempletNowState = GetSkillTempletNowState();
							if (skillTempletNowState != null)
							{
								skillName = skillTempletNowState.GetSkillName();
							}
							else if (m_UnitStateNow.m_SkillCutInName.Length > 1)
							{
								skillName = NKCStringTable.GetString(m_UnitStateNow.m_SkillCutInName);
							}
							if (m_UnitStateNow.m_bSkillCutIn)
							{
								if (!m_NKMGame.IsEnemy(m_NKCGameClient.m_MyTeam, m_UnitDataGame.m_NKM_TEAM_TYPE_ORG))
								{
									m_NKCGameClient.PlaySkillCutIn(this, bHyper: false, bRight: true, sprite, GetUnitTemplet().m_UnitTempletBase.GetUnitName(), skillName);
								}
								else
								{
									m_NKCGameClient.PlaySkillCutIn(this, bHyper: false, bRight: false, sprite, GetUnitTemplet().m_UnitTempletBase.GetUnitName(), skillName);
								}
							}
							if (m_UnitStateNow.m_bHyperSkillCutIn)
							{
								if (!m_NKMGame.IsEnemy(m_NKCGameClient.m_MyTeam, m_UnitDataGame.m_NKM_TEAM_TYPE_ORG))
								{
									m_NKCGameClient.PlaySkillCutIn(this, bHyper: true, bRight: true, sprite, GetUnitTemplet().m_UnitTempletBase.GetUnitName(), skillName);
								}
								else
								{
									m_NKCGameClient.PlaySkillCutIn(this, bHyper: true, bRight: false, sprite, GetUnitTemplet().m_UnitTempletBase.GetUnitName(), skillName);
								}
							}
						}
					}
				}
				if (m_NKCASDangerChargeUI != null)
				{
					if (GetUnitFrameData().m_fDangerChargeTime > 0f)
					{
						m_NKCASDangerChargeUI.OpenDangerCharge(GetUnitFrameData().m_fDangerChargeTime, GetMaxHP(m_UnitStateNow.m_DangerCharge.m_fCancelDamageRate), m_UnitStateNow.m_DangerCharge.m_CancelHitCount);
					}
					else
					{
						m_NKCASDangerChargeUI.CloseDangerCharge();
					}
				}
				if (m_UnitStateNow.m_StateID == m_bManualSkillUseStateID)
				{
					m_bManualSkillUseStart = true;
				}
			}

			public override void ChangeAnimSpeed(float fAnimSpeed)
			{
				base.ChangeAnimSpeed(fAnimSpeed);
				if (m_UnitObject.m_Instant.activeSelf && m_NKCAnimSpine != null)
				{
					m_NKCAnimSpine.SetPlaySpeed(m_UnitFrameData.m_fAnimSpeed);
				}
			}

			protected override void StateUpdate()
			{
				if (m_UnitStateNow == null)
				{
					return;
				}
				base.StateUpdate();
				if (m_UnitObject.m_Instant.activeSelf && !NKCScenManager.GetScenManager().GetNKCPowerSaveMode().GetEnable())
				{
					if (m_UnitFrameData.m_bAnimPlayCountAddThisFrame)
					{
						m_NKCAnimSpine.Play(m_NKCAnimSpine.GetAnimName(), bLoop: false);
					}
					m_NKCAnimSpine.Update(m_DeltaTime);
					if (m_UnitTemplet.m_bUseMotionBlur)
					{
						m_NKCMotionAfterImage.Update(m_DeltaTime);
					}
					SetDataObject();
				}
				float num = 0f;
				bool flag = false;
				float fR = 1f;
				float fG = 1f;
				float fB = 1f;
				if (m_NKCGameClient.GetShipSkillDrag() && m_NKCGameClient.GetSelectShipSkillID() != 0)
				{
					flag = m_NKCGameClient.GetShipSkillDrag();
					if (flag)
					{
						fR = 1f;
						fG = 0.2f;
						fB = 0.2f;
						NKMShipSkillTemplet shipSkillTempletByID = NKMShipSkillManager.GetShipSkillTempletByID(m_NKCGameClient.GetSelectShipSkillID());
						if (shipSkillTempletByID != null)
						{
							if (shipSkillTempletByID.m_bFullMap)
							{
								num = m_NKCGameClient.GetMapTemplet().m_fMaxX - m_NKCGameClient.GetMapTemplet().m_fMinX;
							}
							else
							{
								num = shipSkillTempletByID.m_fRange;
								if (Math.Abs(m_UnitSyncData.m_PosX - m_NKCGameClient.GetShipSkillDragPosX()) > num * 0.5f)
								{
									flag = false;
								}
							}
							if (shipSkillTempletByID.m_bEnemy && !m_NKMGame.IsEnemy(m_NKCGameClient.m_MyTeam, m_UnitDataGame.m_NKM_TEAM_TYPE_ORG))
							{
								flag = false;
							}
							else if (!shipSkillTempletByID.m_bAir && IsAirUnit())
							{
								flag = false;
							}
						}
					}
				}
				if (m_NKCGameClient.GetDeckDrag() && m_NKCGameClient.DeckSelectUnitUID > 0)
				{
					NKCUnitClient dragUnit = m_NKCGameClient.GetDragUnit();
					NKMUnitTemplet.RespawnEffectData respawnEffectData = dragUnit?.GetRespawnEffectData();
					if (respawnEffectData != null && respawnEffectData.HasRespawnEffectRange)
					{
						bool flag2;
						if (respawnEffectData.m_RespawnEffectFullMap)
						{
							flag2 = true;
						}
						else
						{
							float deckSelectPosX = m_NKCGameClient.GetDeckSelectPosX();
							float num2 = m_UnitSyncData.m_PosX - deckSelectPosX;
							if (respawnEffectData.m_RespawnEffectUseUnitSize)
							{
								float num3 = respawnEffectData.m_RespawnEffectRange.m_Min;
								float num4 = respawnEffectData.m_RespawnEffectRange.m_Max;
								float num5 = m_NKCGameClient.DeckSelectUnitTemplet.m_UnitSizeX * 0.5f;
								if (num3 < 0f)
								{
									num3 -= num5;
								}
								if (num4 > 0f)
								{
									num4 += num5;
								}
								float num6 = GetUnitTemplet().m_UnitSizeX * 0.5f;
								flag2 = NKMMinMaxFloat.IsOverlaps(num3, num4, num2 - num6, num2 + num6);
							}
							else
							{
								flag2 = respawnEffectData.m_RespawnEffectRange.IsBetween(num2, NegativeIsOpen: false);
							}
						}
						if (flag2)
						{
							flag = CheckEventCondition(respawnEffectData.m_RespawnEffectCondition, dragUnit);
							if (respawnEffectData.m_RespawnAffectColorR >= 0f)
							{
								fR = respawnEffectData.m_RespawnAffectColorR;
								fG = respawnEffectData.m_RespawnAffectColorG;
								fB = respawnEffectData.m_RespawnAffectColorB;
							}
							else
							{
								fR = 1f;
								fG = 0.2f;
								fB = 0.2f;
							}
						}
					}
				}
				if (m_UnitFrameData.m_fHitLightTime > 0f)
				{
					m_NKCASUnitSpineSprite.SetColor(1f, 0.5f, 0.5f);
					if (m_NKCASUnitMiniMapFace != null)
					{
						m_NKCASUnitMiniMapFace.SetColor(1f, 0.5f, 0.5f);
					}
				}
				else if (flag)
				{
					m_NKCASUnitSpineSprite.SetColor(fR, fG, fB);
					if (m_NKCASUnitMiniMapFace != null)
					{
						m_NKCASUnitMiniMapFace.SetColor(fR, fG, fB);
					}
				}
				else
				{
					float fR2 = m_UnitFrameData.m_ColorR.GetNowValue();
					float fG2 = m_UnitFrameData.m_ColorG.GetNowValue();
					float fB2 = m_UnitFrameData.m_ColorB.GetNowValue();
					foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum in m_UnitFrameData.m_dicBuffData)
					{
						NKMBuffData value = dicBuffDatum.Value;
						if (value == null || value.m_NKMBuffTemplet == null)
						{
							continue;
						}
						if (value.m_BuffSyncData.m_MasterGameUnitUID == GetUnitSyncData().m_GameUnitUID)
						{
							if (value.m_NKMBuffTemplet.m_MasterColorR != -1f)
							{
								fR2 = value.m_NKMBuffTemplet.m_MasterColorR;
							}
							if (value.m_NKMBuffTemplet.m_MasterColorG != -1f)
							{
								fG2 = value.m_NKMBuffTemplet.m_MasterColorG;
							}
							if (value.m_NKMBuffTemplet.m_MasterColorB != -1f)
							{
								fB2 = value.m_NKMBuffTemplet.m_MasterColorB;
							}
						}
						else
						{
							if (value.m_NKMBuffTemplet.m_ColorR != -1f)
							{
								fR2 = value.m_NKMBuffTemplet.m_ColorR;
							}
							if (value.m_NKMBuffTemplet.m_ColorG != -1f)
							{
								fG2 = value.m_NKMBuffTemplet.m_ColorG;
							}
							if (value.m_NKMBuffTemplet.m_ColorB != -1f)
							{
								fB2 = value.m_NKMBuffTemplet.m_ColorB;
							}
						}
					}
					m_NKCASUnitSpineSprite.SetColor(fR2, fG2, fB2);
					if (m_NKCASUnitMiniMapFace != null)
					{
						m_NKCASUnitMiniMapFace.SetColor(fR2, fG2, fB2);
					}
				}
				if (m_fManualSkillUseAck > 0f)
				{
					m_fManualSkillUseAck -= m_DeltaTime;
					if (m_fManualSkillUseAck < 0f)
					{
						m_fManualSkillUseAck = 0f;
					}
				}
				if (m_bManualSkillUseStart && m_UnitStateNow.m_NKM_SKILL_TYPE < NKM_SKILL_TYPE.NST_HYPER)
				{
					m_fManualSkillUseAck = 0f;
				}
				if (!GetUnitTemplet().m_UnitTempletBase.IsEnv)
				{
					ProcessStatusEffect();
				}
			}

			private void SetDataObject()
			{
				if (Mathf.Abs(m_ObjPosX.GetNowValue() - m_UnitSyncData.m_PosX) > 300f)
				{
					m_ObjPosX.SetNowValue(m_UnitSyncData.m_PosX);
					m_ObjPosZ.SetNowValue(m_UnitSyncData.m_PosZ);
				}
				m_ObjPosX.SetTracking(m_UnitSyncData.m_PosX, 0.1f, TRACKING_DATA_TYPE.TDT_SLOWER);
				m_ObjPosZ.SetTracking(m_UnitSyncData.m_PosZ, 0.1f, TRACKING_DATA_TYPE.TDT_SLOWER);
				m_ObjPosX.Update(m_DeltaTime);
				m_ObjPosZ.Update(m_DeltaTime);
				m_Vector3Temp.Set(m_ObjPosX.GetNowValue(), m_ObjPosZ.GetNowValue() + m_UnitSyncData.m_JumpYPos, m_ObjPosZ.GetNowValue());
				m_UnitObject.m_Instant.transform.localPosition = m_Vector3Temp;
				float zScaleFactor = m_NKCGameClient.GetZScaleFactor(m_ObjPosZ.GetNowValue());
				m_Vector3Temp = m_SpriteObject.transform.localScale;
				m_Vector3Temp.x = m_UnitTemplet.m_SpriteScale * zScaleFactor;
				m_Vector3Temp.y = m_UnitTemplet.m_SpriteScale * zScaleFactor;
				m_SpriteObject.transform.localScale = m_Vector3Temp;
				if (m_UnitSyncData.m_bRight)
				{
					m_Vector3Temp = m_SpriteObject.transform.localEulerAngles;
					m_Vector3Temp.y = 0f;
					m_SpriteObject.transform.localEulerAngles = m_Vector3Temp;
					NKCUtil.SetGameObjectLocalPos(m_SpriteObject, m_UnitTemplet.m_SpriteOffsetX, m_UnitTemplet.m_SpriteOffsetY, 0f);
				}
				else
				{
					m_Vector3Temp = m_SpriteObject.transform.localEulerAngles;
					m_Vector3Temp.y = 180f;
					m_SpriteObject.transform.localEulerAngles = m_Vector3Temp;
					NKCUtil.SetGameObjectLocalPos(m_SpriteObject, 0f - m_UnitTemplet.m_SpriteOffsetX, m_UnitTemplet.m_SpriteOffsetY, 0f);
				}
				SetDataObject_DangerChargeUI();
				SetDataObject_Shadow(zScaleFactor);
				SetDataObject_HPGage(zScaleFactor);
				SetDataObject_SkillGage();
				SetDataObject_MiniMapFace();
			}

			private void SetDataObject_DangerChargeUI()
			{
				if (m_NKCASDangerChargeUI != null)
				{
					m_Vector3Temp.Set(m_ObjPosX.GetNowValue(), m_ObjPosZ.GetNowValue() + m_UnitSyncData.m_JumpYPos + GetUnitTemplet().m_UnitSizeY * 0.5f, m_ObjPosZ.GetNowValue());
					m_NKCASDangerChargeUI.SetPos(ref m_Vector3Temp);
					m_NKCASDangerChargeUI.Update(GetUnitFrameData().m_fDangerChargeTime, GetUnitFrameData().m_fDangerChargeDamage, GetUnitFrameData().m_DangerChargeHitCount);
				}
			}

			private void SetDataObject_Shadow(float fZScaleFactor)
			{
				if (m_NKCASUnitShadow != null && m_NKCASUnitShadow.m_ShadowSpriteInstant != null && !(m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant == null))
				{
					if (m_UnitSyncData.m_bRight)
					{
						m_Vector3Temp.Set(m_UnitTemplet.m_fShadowRotateX, 180f, m_UnitTemplet.m_fShadowRotateZ);
						m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.transform.localEulerAngles = m_Vector3Temp;
					}
					else
					{
						m_Vector3Temp.Set(m_UnitTemplet.m_fShadowRotateX, 0f, m_UnitTemplet.m_fShadowRotateZ);
						m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.transform.localEulerAngles = m_Vector3Temp;
					}
					if (m_NKCASUnitSpineSprite.m_Bone_Move != null)
					{
						m_NKCASUnitSpineSprite.CalcBoneMoveWorldPos();
						float x = m_NKCASUnitSpineSprite.m_Bone_MovePos.x;
						x = ((!m_UnitSyncData.m_bRight) ? (x + m_UnitTemplet.m_SpriteOffsetX) : (x - m_UnitTemplet.m_SpriteOffsetX));
						m_Vector3Temp.Set(x, m_ObjPosZ.GetNowValue(), m_ObjPosZ.GetNowValue());
					}
					else
					{
						m_Vector3Temp.Set(m_ObjPosX.GetNowValue(), m_ObjPosZ.GetNowValue(), m_ObjPosZ.GetNowValue());
					}
					if (m_UnitSyncData.m_bRight)
					{
						m_Vector3Temp.x += m_UnitTemplet.m_fShadowOffsetX;
					}
					else
					{
						m_Vector3Temp.x -= m_UnitTemplet.m_fShadowOffsetX;
					}
					m_Vector3Temp.y += m_UnitTemplet.m_fShadowOffsetY;
					m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant.transform.localPosition = m_Vector3Temp;
					float num = 1f - 0.2f * m_UnitSyncData.m_JumpYPos * 0.01f;
					if (num < 0.3f)
					{
						num = 0.3f;
					}
					NKCUtil.SetGameObjectLocalScale(m_NKCASUnitShadow.m_ShadowSpriteInstant.m_Instant, m_UnitTemplet.m_fShadowScaleX * num * fZScaleFactor, m_UnitTemplet.m_fShadowScaleY * num * fZScaleFactor, 1f);
				}
			}

			private void SetDataObject_HPGage(float fZScaleFactor)
			{
				float totalBarrierHP = GetTotalBarrierHP();
				if (m_UnitHealthBar != null)
				{
					m_UnitHealthBar.SetData(m_UnitSyncData.GetHP(), totalBarrierHP, m_UnitFrameData.m_StatData.GetStatFinal(NKM_STAT_TYPE.NST_HP));
				}
				UpdateGagePos(fZScaleFactor);
			}

			private float GetTotalBarrierHP()
			{
				float num = 0f;
				foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum in m_UnitFrameData.m_dicBuffData)
				{
					NKMBuffData value = dicBuffDatum.Value;
					if (value != null && value.m_NKMBuffTemplet.m_fBarrierHP != -1f && value.m_BuffSyncData.m_bAffect)
					{
						num += value.m_fBarrierHP;
					}
				}
				return num;
			}

			private void SetDataObject_SkillGage()
			{
				if (m_UNIT_GAGE_PANEL.activeSelf)
				{
					NKMAttackStateData fastestCoolTimeSkillData = GetFastestCoolTimeSkillData();
					if (fastestCoolTimeSkillData != null && m_SKILL_GAUGE.GetSkillGauge().activeSelf)
					{
						float skillCoolTime = 1f - GetStateCoolTime(fastestCoolTimeSkillData.m_StateName) / GetStateMaxCoolTime(fastestCoolTimeSkillData.m_StateName);
						m_SKILL_GAUGE?.SetSkillCoolTime(skillCoolTime);
					}
					fastestCoolTimeSkillData = GetFastestCoolTimeHyperSkillData();
					if (fastestCoolTimeSkillData != null && m_SKILL_GAUGE.GetHyperGauge().activeSelf)
					{
						float hyperCoolTime = 1f - GetStateCoolTime(fastestCoolTimeSkillData.m_StateName) / GetStateMaxCoolTime(fastestCoolTimeSkillData.m_StateName);
						m_SKILL_GAUGE?.SetHyperCoolTime(hyperCoolTime);
					}
				}
				if (!m_UNIT_HP_GAGE.activeSelf && !m_SKILL_GAUGE.GetSkillGauge().activeSelf && !m_SKILL_GAUGE.GetHyperGauge().activeSelf)
				{
					if (m_UNIT_GAGE_PANEL.activeSelf)
					{
						m_UNIT_GAGE_PANEL.SetActive(value: false);
						m_GageWide.SetNowValue(0f);
						Vector2 sizeDelta = m_UNIT_GAGE_RectTransform.sizeDelta;
						sizeDelta.x = m_GageWide.GetNowValue();
						m_UNIT_GAGE_RectTransform.sizeDelta = sizeDelta;
						sizeDelta = m_UNIT_LEVEL_RectTransform.anchoredPosition;
						sizeDelta.x = m_UNIT_LEVEL_RectTransform.sizeDelta.x * 0.5f;
						m_UNIT_LEVEL_RectTransform.anchoredPosition = sizeDelta;
					}
				}
				else if (!m_UNIT_GAGE_PANEL.activeSelf)
				{
					m_UNIT_GAGE_PANEL.SetActive(value: true);
					m_GageWide.SetTracking(m_fOrgGageSize, 1f, TRACKING_DATA_TYPE.TDT_SLOWER);
					Vector2 anchoredPosition = m_UNIT_LEVEL_RectTransform.anchoredPosition;
					anchoredPosition.x = 0f;
					m_UNIT_LEVEL_RectTransform.anchoredPosition = anchoredPosition;
				}
				m_GageWide.Update(m_DeltaTime);
				if (m_GageWide.IsTracking())
				{
					Vector2 sizeDelta2 = m_UNIT_GAGE_RectTransform.sizeDelta;
					sizeDelta2.x = m_GageWide.GetNowValue();
					m_UNIT_GAGE_RectTransform.sizeDelta = sizeDelta2;
				}
				bool bHyper = false;
				byte skillStateID = 0;
				if (m_NKCGameClient.GetMyRunTimeTeamData().m_NKM_GAME_AUTO_SKILL_TYPE != NKM_GAME_AUTO_SKILL_TYPE.NGST_AUTO && CanUseManualSkill(bUse: false, out bHyper, out skillStateID) && m_fManualSkillUseAck <= 0f && !IsDyingOrDie() && IsMyTeam() && !IsBoss())
				{
					m_NKCUnitTouchObject.ActiveObject(bActive: true);
					if (!m_UNIT_SKILL.activeSelf)
					{
						m_UNIT_SKILL.SetActive(value: true);
					}
					Vector3 worldPos = default(Vector3);
					if (!m_NKCASUnitSpineSprite.GetBoneWorldPos("MOVE", ref worldPos))
					{
						worldPos = m_UnitObject.m_Instant.transform.position;
					}
					NKCCamera.GetWorldPosToScreenPos(out m_Vector3Temp, worldPos.x, worldPos.y, worldPos.z);
					m_NKCUnitTouchObject.GetRectTransform().position = m_Vector3Temp;
				}
				else
				{
					m_NKCUnitTouchObject.ActiveObject(bActive: false);
					if (m_UNIT_SKILL.activeSelf)
					{
						m_UNIT_SKILL.SetActive(value: false);
					}
				}
			}

			public void MoveToLastTouchObject()
			{
				m_NKCUnitTouchObject.MoveToLastTouchObject();
			}

			private void SetDataObject_MiniMapFace()
			{
				Vector2 vector = new Vector2(0f, 0f);
				if (m_NKCASUnitMiniMapFace != null && m_NKCASUnitMiniMapFace.m_RectTransform != null)
				{
					vector = m_NKCASUnitMiniMapFace.m_RectTransform.anchoredPosition;
				}
				NKMMapTemplet mapTemplet = m_NKMGame.GetMapTemplet();
				if (mapTemplet != null)
				{
					float newX = (m_ObjPosX.GetNowValue() - mapTemplet.m_fMinX) / (mapTemplet.m_fMaxX - mapTemplet.m_fMinX) * m_NKCGameClient.GetGameHud().GetMiniMapRectWidth();
					vector.Set(newX, 0f);
				}
				if (m_NKCASUnitMiniMapFace != null && m_NKCASUnitMiniMapFace.m_RectTransform != null)
				{
					m_NKCASUnitMiniMapFace.m_RectTransform.anchoredPosition = vector;
				}
				if (m_UnitSyncData.m_bRight)
				{
					if (m_NKCASUnitMiniMapFace != null && m_NKCASUnitMiniMapFace.m_RectTransform != null)
					{
						vector = m_NKCASUnitMiniMapFace.m_RectTransform.localScale;
						vector.Set(1f, 1f);
					}
				}
				else if (m_NKCASUnitMiniMapFace != null && m_NKCASUnitMiniMapFace.m_RectTransform != null)
				{
					vector = m_NKCASUnitMiniMapFace.m_RectTransform.localScale;
					vector.Set(-1f, 1f);
				}
				if (m_NKCASUnitMiniMapFace != null && m_NKCASUnitMiniMapFace.m_RectTransform != null)
				{
					m_NKCASUnitMiniMapFace.m_RectTransform.localScale = vector;
				}
				if (m_UnitSyncData.m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_PLAY && m_NKCASUnitMiniMapFace != null && m_NKCASUnitMiniMapFace.m_MiniMapFaceInstant != null && m_NKCASUnitMiniMapFace.m_MiniMapFaceInstant.m_Instant != null && m_NKCASUnitMiniMapFace.m_MiniMapFaceInstant.m_Instant.activeSelf)
				{
					m_NKCASUnitMiniMapFace.m_MiniMapFaceInstant.m_Instant.SetActive(value: false);
				}
			}

			private void StateCamera()
			{
			}

			public float GetSkillCoolRate()
			{
				NKMAttackStateData fastestCoolTimeSkillData = GetFastestCoolTimeSkillData();
				if (fastestCoolTimeSkillData == null)
				{
					return 0f;
				}
				float stateCoolTime = GetStateCoolTime(fastestCoolTimeSkillData.m_StateName);
				float stateMaxCoolTime = GetStateMaxCoolTime(fastestCoolTimeSkillData.m_StateName);
				return stateCoolTime / stateMaxCoolTime;
			}

			public float GetHyperSkillCoolRate()
			{
				NKMAttackStateData fastestCoolTimeHyperSkillData = GetFastestCoolTimeHyperSkillData();
				if (fastestCoolTimeHyperSkillData == null)
				{
					return 0f;
				}
				float stateCoolTime = GetStateCoolTime(fastestCoolTimeHyperSkillData.m_StateName);
				float stateMaxCoolTime = GetStateMaxCoolTime(fastestCoolTimeHyperSkillData.m_StateName);
				return stateCoolTime / stateMaxCoolTime;
			}

			protected override void PhysicProcess()
			{
				if (m_UnitStateNow != null)
				{
					base.PhysicProcess();
				}
			}

			public override bool ProcessCamera()
			{
				if (m_UnitStateNow == null)
				{
					return false;
				}
				if (m_NKCGameClient.GetCameraMode() == NKM_GAME_CAMERA_MODE.NGCM_DRAG)
				{
					return false;
				}
				NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
				if (gameOptionData != null && gameOptionData.ActionCamera == ActionCameraType.None)
				{
					return false;
				}
				if (!IsMyTeam() && gameOptionData != null && gameOptionData.ActionCamera != ActionCameraType.All)
				{
					return false;
				}
				if (m_NKCGameClient.GetDeckDrag() || m_NKCGameClient.GetShipSkillDrag() || m_NKCGameClient.GetDeckTouchDown() || m_NKCGameClient.GetShipSkillDeckTouchDown() || m_NKCGameClient.GetCameraTouchDown())
				{
					return false;
				}
				bool result = false;
				for (int i = 0; i < m_UnitStateNow.m_listNKMEventCameraMove.Count; i++)
				{
					NKMEventCameraMove nKMEventCameraMove = m_UnitStateNow.m_listNKMEventCameraMove[i];
					if (nKMEventCameraMove == null || !CheckEventCondition(nKMEventCameraMove.m_Condition))
					{
						continue;
					}
					bool flag = false;
					if (EventTimer(nKMEventCameraMove.m_bAnimTime, nKMEventCameraMove.m_fEventTimeMin, nKMEventCameraMove.m_fEventTimeMax))
					{
						flag = true;
					}
					if (!flag)
					{
						continue;
					}
					result = true;
					if (!(nKMEventCameraMove.m_fCameraRadius <= 0f) && !(NKCCamera.GetDist(this) <= nKMEventCameraMove.m_fCameraRadius))
					{
						continue;
					}
					float num = -1f;
					if (nKMEventCameraMove.m_fPosXOffset != -1f)
					{
						num = m_UnitSyncData.m_PosX;
						if (m_UnitSyncData.m_bRight)
						{
							num += nKMEventCameraMove.m_fPosXOffset;
							num += m_UnitTemplet.m_SpriteOffsetX;
						}
						else
						{
							num -= nKMEventCameraMove.m_fPosXOffset;
							num -= m_UnitTemplet.m_SpriteOffsetX;
						}
					}
					float fY = -1f;
					if (nKMEventCameraMove.m_fPosYOffset != -1f)
					{
						fY = m_UnitSyncData.m_PosZ + m_UnitSyncData.m_JumpYPos;
						fY += nKMEventCameraMove.m_fPosYOffset;
						fY += m_UnitTemplet.m_SpriteOffsetY;
					}
					NKCCamera.TrackingPos(nKMEventCameraMove.m_fMoveTrackingTime, num, fY, -1f, GetBattleCameraPriority(nKMEventCameraMove.m_Priority));
					if (nKMEventCameraMove.m_fZoom != -1f)
					{
						NKCCamera.TrackingZoom(nKMEventCameraMove.m_fZoomTrackingTime, NKCCamera.GetCameraSizeOrg() + nKMEventCameraMove.m_fZoom);
					}
					if (nKMEventCameraMove.m_fFocusBlur > 0f)
					{
						float num2 = m_UnitSyncData.m_PosZ + m_UnitSyncData.m_JumpYPos;
						num2 += nKMEventCameraMove.m_fPosYOffset;
						num2 += m_UnitTemplet.m_SpriteOffsetY;
						num2 += m_UnitTemplet.m_UnitSizeY * 0.7f;
						NKCCamera.SetFocusBlur(nKMEventCameraMove.m_fFocusBlur, num, num2, m_UnitSyncData.m_PosZ);
					}
				}
				return result;
			}

			protected int GetBattleCameraPriority(int priority = -1)
			{
				if (priority >= 0)
				{
					return priority;
				}
				if (m_UnitStateNow.m_NKM_UNIT_STATE_TYPE == NKM_UNIT_STATE_TYPE.NUST_START && m_UnitTemplet.m_UnitTempletBase != null && m_UnitTemplet.m_UnitTempletBase.m_bAwaken)
				{
					return 30;
				}
				if (m_UnitStateNow.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER)
				{
					return 20;
				}
				return 0;
			}

			protected void ProcessStateEventClient<T>(List<T> lstEvent, bool bStateEnd = false) where T : INKCUnitstateEventOneTime
			{
				if (m_UnitStateNow == null)
				{
					return;
				}
				for (int i = 0; i < lstEvent.Count; i++)
				{
					INKCUnitstateEventOneTime iNKCUnitstateEventOneTime = lstEvent[i];
					if (iNKCUnitstateEventOneTime != null && iNKCUnitstateEventOneTime.bStateEnd == bStateEnd && (bStateEnd || EventTimer(iNKCUnitstateEventOneTime.bAnimTime, iNKCUnitstateEventOneTime.EventStartTime, bOneTime: true)) && CheckEventCondition(iNKCUnitstateEventOneTime.Condition))
					{
						iNKCUnitstateEventOneTime.ApplyEventClient(m_NKCGameClient, this);
					}
				}
			}

			protected override void ProcessStateEvents(bool bStateEnd)
			{
				base.ProcessStateEvents(bStateEnd);
				ProcessStateEventClient(m_UnitStateNow.m_listNKMEventSkillCutIn, bStateEnd);
			}

			protected override void ProcessEventText(bool bStateEnd = false)
			{
				if (m_UnitStateNow == null)
				{
					return;
				}
				for (int i = 0; i < m_UnitStateNow.m_listNKMEventText.Count; i++)
				{
					NKMEventText nKMEventText = m_UnitStateNow.m_listNKMEventText[i];
					if (nKMEventText != null && CheckEventCondition(nKMEventText.m_Condition))
					{
						bool flag = false;
						if (nKMEventText.m_bStateEndTime && bStateEnd)
						{
							flag = true;
						}
						else if (EventTimer(nKMEventText.m_bAnimTime, nKMEventText.m_fEventTime, bOneTime: true) && !nKMEventText.m_bStateEndTime)
						{
							flag = true;
						}
						if (flag)
						{
							ApplyEventText(nKMEventText);
						}
					}
				}
			}

			public override void ApplyEventText(NKMEventText cNKMEventText)
			{
				if (NKCScenManager.GetScenManager().GetGameClient().IsShowUI())
				{
					NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString(cNKMEventText.m_Text), NKCPopupMessage.eMessagePosition.TopIngame);
				}
			}

			protected override void ProcessEventAttack(bool bStateEnd)
			{
				if (bStateEnd || m_UnitStateNow == null)
				{
					return;
				}
				base.ProcessEventAttack(bStateEnd);
				for (int i = 0; i < m_UnitStateNow.m_listNKMEventAttack.Count; i++)
				{
					NKMEventAttack nKMEventAttack = m_UnitStateNow.m_listNKMEventAttack[i];
					if (nKMEventAttack != null && nKMEventAttack.m_SoundName.Length > 1 && EventTimer(nKMEventAttack.m_bAnimTime, nKMEventAttack.m_fEventTimeMin, bOneTime: true))
					{
						NKCSoundManager.PlaySound(nKMEventAttack.m_SoundName, nKMEventAttack.m_fLocalVol, m_SpriteObject.transform.position.x, 1200f);
					}
				}
			}

			protected override void ProcessAttackHitEffect(NKMEventAttack cNKMEventAttack)
			{
				if (cNKMEventAttack != null)
				{
					m_Vector3Temp.Set(m_ObjPosX.GetNowValue(), m_ObjPosZ.GetNowValue() + m_UnitSyncData.m_JumpYPos, m_ObjPosZ.GetNowValue());
					m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, cNKMEventAttack.m_EffectName, cNKMEventAttack.m_EffectName, NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, m_Vector3Temp.x, m_Vector3Temp.y, m_Vector3Temp.z, m_UnitSyncData.m_bRight, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: true);
				}
			}

			protected override void ProcessEventSound(bool bStateEnd = false)
			{
				if (m_UnitStateNow == null)
				{
					return;
				}
				for (int i = 0; i < m_UnitStateNow.m_listNKMEventSound.Count; i++)
				{
					NKMEventSound nKMEventSound = m_UnitStateNow.m_listNKMEventSound[i];
					if (nKMEventSound != null && CheckEventCondition(nKMEventSound.m_Condition) && nKMEventSound.IsRightSkin(m_UnitData.m_SkinID))
					{
						bool bOneTime = true;
						if (m_UnitStateNow.m_bAnimLoop)
						{
							bOneTime = false;
						}
						bool flag = false;
						if (nKMEventSound.m_bStateEndTime && bStateEnd)
						{
							flag = true;
						}
						else if (EventTimer(nKMEventSound.m_bAnimTime, nKMEventSound.m_fEventTime, bOneTime) && !nKMEventSound.m_bStateEndTime)
						{
							flag = true;
						}
						if (flag)
						{
							ApplyEventSound(nKMEventSound);
						}
					}
				}
			}

			public override void ApplyEventSound(NKMEventSound cNKMEventSound)
			{
				if (NKMRandom.Range(0f, 1f) <= cNKMEventSound.m_PlayRate && cNKMEventSound.GetRandomSound(m_UnitData, out var soundName))
				{
					int num = 0;
					num = (cNKMEventSound.m_bVoice ? NKCSoundManager.PlayVoice(soundName, GetUnitDataGame().m_GameUnitUID, bClearVoice: true, bIgnoreSameVoice: true, cNKMEventSound.m_fLocalVol, m_SpriteObject.transform.position.x, cNKMEventSound.m_fFocusRange, cNKMEventSound.m_bLoop) : NKCSoundManager.PlaySound(soundName, cNKMEventSound.m_fLocalVol, m_SpriteObject.transform.position.x, cNKMEventSound.m_fFocusRange, cNKMEventSound.m_bLoop));
					if (cNKMEventSound.m_bStopSound)
					{
						m_listSoundUID.Add(num);
					}
				}
			}

			protected override void ProcessEventColor(bool bStateEnd = false)
			{
				if (m_UnitStateNow == null)
				{
					return;
				}
				if (m_UnitFrameData.m_fColorEventTime > 0f)
				{
					m_UnitFrameData.m_fColorEventTime -= m_DeltaTime;
					m_UnitFrameData.m_ColorR.Update(m_DeltaTime);
					m_UnitFrameData.m_ColorG.Update(m_DeltaTime);
					m_UnitFrameData.m_ColorB.Update(m_DeltaTime);
					if (m_UnitFrameData.m_fColorEventTime <= 0f)
					{
						m_UnitFrameData.m_fColorEventTime = m_UnitFrameData.m_ColorR.GetTime() + 0.1f;
						m_UnitFrameData.m_ColorR.SetTracking(m_UnitTemplet.m_ColorR, m_UnitFrameData.m_ColorR.GetTime(), TRACKING_DATA_TYPE.TDT_SLOWER);
						m_UnitFrameData.m_ColorG.SetTracking(m_UnitTemplet.m_ColorG, m_UnitFrameData.m_ColorG.GetTime(), TRACKING_DATA_TYPE.TDT_SLOWER);
						m_UnitFrameData.m_ColorB.SetTracking(m_UnitTemplet.m_ColorB, m_UnitFrameData.m_ColorB.GetTime(), TRACKING_DATA_TYPE.TDT_SLOWER);
					}
				}
				for (int i = 0; i < m_UnitStateNow.m_listNKMEventColor.Count; i++)
				{
					NKMEventColor nKMEventColor = m_UnitStateNow.m_listNKMEventColor[i];
					if (nKMEventColor != null && CheckEventCondition(nKMEventColor.m_Condition))
					{
						bool flag = false;
						if (nKMEventColor.m_bStateEndTime && bStateEnd)
						{
							flag = true;
						}
						else if (EventTimer(nKMEventColor.m_bAnimTime, nKMEventColor.m_fEventTime, bOneTime: true) && !nKMEventColor.m_bStateEndTime)
						{
							flag = true;
						}
						if (flag)
						{
							ApplyEventColor(nKMEventColor);
						}
					}
				}
			}

			public override void ApplyEventColor(NKMEventColor cNKMEventColor)
			{
				m_UnitFrameData.m_fColorEventTime = cNKMEventColor.m_fColorTime;
				m_UnitFrameData.m_ColorR.SetTracking(cNKMEventColor.m_fColorR, cNKMEventColor.m_fTrackTime, TRACKING_DATA_TYPE.TDT_SLOWER);
				m_UnitFrameData.m_ColorG.SetTracking(cNKMEventColor.m_fColorG, cNKMEventColor.m_fTrackTime, TRACKING_DATA_TYPE.TDT_SLOWER);
				m_UnitFrameData.m_ColorB.SetTracking(cNKMEventColor.m_fColorB, cNKMEventColor.m_fTrackTime, TRACKING_DATA_TYPE.TDT_SLOWER);
			}

			protected override void ProcessEventCameraCrash(bool bStateEnd = false)
			{
				if (m_UnitStateNow == null)
				{
					return;
				}
				for (int i = 0; i < m_UnitStateNow.m_listNKMEventCameraCrash.Count; i++)
				{
					NKMEventCameraCrash nKMEventCameraCrash = m_UnitStateNow.m_listNKMEventCameraCrash[i];
					if (nKMEventCameraCrash != null && CheckEventCondition(nKMEventCameraCrash.m_Condition))
					{
						bool flag = false;
						if (nKMEventCameraCrash.m_bStateEndTime && bStateEnd)
						{
							flag = true;
						}
						else if (EventTimer(nKMEventCameraCrash.m_bAnimTime, nKMEventCameraCrash.m_fEventTime, bOneTime: true) && !nKMEventCameraCrash.m_bStateEndTime)
						{
							flag = true;
						}
						if (flag)
						{
							ApplyEventCameraCrash(nKMEventCameraCrash);
						}
					}
				}
			}

			public override void ApplyEventCameraCrash(NKMEventCameraCrash cNKMEventCameraCrash)
			{
				if (cNKMEventCameraCrash.m_fCrashRadius <= 0f || NKCCamera.GetDist(this) <= cNKMEventCameraCrash.m_fCrashRadius)
				{
					switch (cNKMEventCameraCrash.m_CameraCrashType)
					{
					case NKM_CAMERA_CRASH_TYPE.NCCT_UP:
						NKCCamera.UpCrashCamera(cNKMEventCameraCrash.m_fCameraCrashSpeed, cNKMEventCameraCrash.m_fCameraCrashAccel);
						break;
					case NKM_CAMERA_CRASH_TYPE.NCCT_DOWN:
						NKCCamera.DownCrashCamera(cNKMEventCameraCrash.m_fCameraCrashSpeed, cNKMEventCameraCrash.m_fCameraCrashAccel);
						break;
					case NKM_CAMERA_CRASH_TYPE.NCCT_UP_DOWN:
						NKCCamera.UpDownCrashCamera(cNKMEventCameraCrash.m_fCameraCrashGap, cNKMEventCameraCrash.m_fCameraCrashTime);
						break;
					case NKM_CAMERA_CRASH_TYPE.NCCT_UP_DOWN_NO_RESET:
						NKCCamera.UpDownCrashCameraNoReset(cNKMEventCameraCrash.m_fCameraCrashGap, cNKMEventCameraCrash.m_fCameraCrashTime);
						break;
					}
				}
			}

			protected override void ProcessEventCameraMove(bool bStateEnd = false)
			{
				if (m_UnitStateNow == null || m_NKCGameClient.GetCameraMode() == NKM_GAME_CAMERA_MODE.NGCM_DRAG)
				{
					return;
				}
				for (int i = 0; i < m_UnitStateNow.m_listNKMEventCameraMove.Count; i++)
				{
					NKMEventCameraMove nKMEventCameraMove = m_UnitStateNow.m_listNKMEventCameraMove[i];
					if (nKMEventCameraMove != null && CheckEventCondition(nKMEventCameraMove.m_Condition))
					{
						bool flag = false;
						if (EventTimer(nKMEventCameraMove.m_bAnimTime, nKMEventCameraMove.m_fEventTimeMin, bOneTime: true))
						{
							flag = true;
						}
						if (flag)
						{
							ApplyEventCameraMove(nKMEventCameraMove);
						}
					}
				}
			}

			public override void ApplyEventCameraMove(NKMEventCameraMove cNKMEventCameraMove)
			{
				m_NKCGameClient.SetCameraMode(NKM_GAME_CAMERA_MODE.NGCM_FOCUS_UNIT);
				m_NKCGameClient.SetCameraFocusUnit(m_UnitDataGame.m_GameUnitUID);
				m_NKCGameClient.SetCameraNormalTackingWaitTime(0.1f);
			}

			protected override void ProcessEventFadeWorld(bool bStateEnd)
			{
				if (bStateEnd || m_UnitStateNow == null)
				{
					return;
				}
				for (int i = 0; i < m_UnitStateNow.m_listNKMEventFadeWorld.Count; i++)
				{
					NKMEventFadeWorld nKMEventFadeWorld = m_UnitStateNow.m_listNKMEventFadeWorld[i];
					if (nKMEventFadeWorld != null && EventTimer(nKMEventFadeWorld.m_bAnimTime, nKMEventFadeWorld.m_fEventTimeMin, nKMEventFadeWorld.m_fEventTimeMax))
					{
						ApplyEventFadeWorld(nKMEventFadeWorld);
					}
				}
			}

			public override void ApplyEventFadeWorld(NKMEventFadeWorld cNKMEventFadeWorld)
			{
				m_NKCGameClient.FadeColor(this, cNKMEventFadeWorld.m_fColorR, cNKMEventFadeWorld.m_fColorG, cNKMEventFadeWorld.m_fColorB, cNKMEventFadeWorld.m_fMapColorKeepTime, cNKMEventFadeWorld.m_fMapColorReturnTime, cNKMEventFadeWorld.m_bHideMapObject);
			}

			protected override void ProcessEventDissolve(bool bStateEnd = false)
			{
				if (m_UnitStateNow == null)
				{
					return;
				}
				m_DissolveFactor.Update(m_DeltaTime);
				if (m_DissolveFactor.IsTracking())
				{
					m_NKCASUnitSpineSprite.SetDissolveBlend(m_DissolveFactor.GetNowValue());
				}
				else if (m_DissolveFactor.GetNowValue() <= 0f && m_bDissolveEnable)
				{
					m_bDissolveEnable = false;
					m_NKCASUnitSpineSprite.SetDissolveBlend(0f);
					m_NKCASUnitSpineSprite.SetDissolveOn(m_bDissolveEnable);
				}
				for (int i = 0; i < m_UnitStateNow.m_listNKMEventDissolve.Count; i++)
				{
					NKMEventDissolve nKMEventDissolve = m_UnitStateNow.m_listNKMEventDissolve[i];
					if (nKMEventDissolve != null && CheckEventCondition(nKMEventDissolve.m_Condition))
					{
						bool flag = false;
						if (nKMEventDissolve.m_bStateEndTime && bStateEnd)
						{
							flag = true;
						}
						else if (EventTimer(nKMEventDissolve.m_bAnimTime, nKMEventDissolve.m_fEventTime, bOneTime: true) && !nKMEventDissolve.m_bStateEndTime)
						{
							flag = true;
						}
						if (flag)
						{
							ApplyEventDissolve(nKMEventDissolve);
						}
					}
				}
			}

			public override void ApplyEventDissolve(NKMEventDissolve cNKMEventDissolve)
			{
				if (!m_bDissolveEnable)
				{
					m_bDissolveEnable = true;
					m_NKCASUnitSpineSprite.SetDissolveOn(m_bDissolveEnable);
					m_ColorTemp.r = cNKMEventDissolve.m_fColorR;
					m_ColorTemp.g = cNKMEventDissolve.m_fColorG;
					m_ColorTemp.b = cNKMEventDissolve.m_fColorB;
					m_ColorTemp.a = 1f;
					m_NKCASUnitSpineSprite.SetDissolveColor(m_ColorTemp);
				}
				m_DissolveFactor.SetTracking(cNKMEventDissolve.m_fDissolve, cNKMEventDissolve.m_fTrackTime, TRACKING_DATA_TYPE.TDT_NORMAL);
			}

			protected override void ProcessEventMotionBlur(bool bStateEnd)
			{
				if (bStateEnd || m_UnitStateNow == null)
				{
					return;
				}
				m_NKCMotionAfterImage.SetEnable(bEnable: false);
				for (int i = 0; i < m_UnitStateNow.m_listNKMEventMotionBlur.Count; i++)
				{
					NKMEventMotionBlur nKMEventMotionBlur = m_UnitStateNow.m_listNKMEventMotionBlur[i];
					if (nKMEventMotionBlur != null && CheckEventCondition(nKMEventMotionBlur.m_Condition) && EventTimer(nKMEventMotionBlur.m_bAnimTime, nKMEventMotionBlur.m_fEventTimeMin, nKMEventMotionBlur.m_fEventTimeMax))
					{
						ApplyEventMotionBlur(nKMEventMotionBlur);
					}
				}
			}

			public override void ApplyEventMotionBlur(NKMEventMotionBlur cNKMEventMotionBlur)
			{
				if (m_NKCMotionAfterImage != null)
				{
					m_NKCMotionAfterImage.SetLifeTime(cNKMEventMotionBlur.m_fLifeTime);
					m_NKCMotionAfterImage.SetGapTime(cNKMEventMotionBlur.m_fGapTime);
					m_NKCMotionAfterImage.SetMaxImageCount(cNKMEventMotionBlur.m_maxImageCount);
					m_NKCMotionAfterImage.SetFadeSpeed(cNKMEventMotionBlur.m_fFadeSpeed);
					m_NKCMotionAfterImage.SetColor(new Color(cNKMEventMotionBlur.m_fColorR, cNKMEventMotionBlur.m_fColorG, cNKMEventMotionBlur.m_fColorB));
					m_NKCMotionAfterImage.SetEnable(bEnable: true);
				}
			}

			private void ProcessStatusEffect()
			{
				if (IsDyingOrDie() || !m_NKCGameClient.IsShowUI())
				{
					foreach (NKCASEffect value2 in m_dicStatusEffect.Values)
					{
						if (value2 != null)
						{
							value2.Stop(bForce: true);
							value2.m_bAutoDie = true;
						}
					}
					m_dicStatusEffect.Clear();
					return;
				}
				if (m_UnitStateNow != null)
				{
					foreach (NKM_UNIT_STATUS_EFFECT item in m_UnitStateNow.m_listFixedStatusEffect)
					{
						AddStatusEffect(item, bFixedStatus: false);
					}
				}
				foreach (NKM_UNIT_STATUS_EFFECT item2 in m_UnitTemplet.m_listFixedStatusEffect)
				{
					if ((m_UnitStateNow == null || !m_UnitStateNow.m_listFixedStatusImmune.Contains(item2)) && !m_UnitTemplet.m_listFixedStatusImmune.Contains(item2))
					{
						AddStatusEffect(item2, bFixedStatus: true);
					}
				}
				foreach (NKM_UNIT_STATUS_EFFECT item3 in GetUnitFrameData().m_hsStatus)
				{
					if (!m_UnitTemplet.m_listFixedStatusEffect.Contains(item3) && !m_UnitTemplet.m_listFixedStatusImmune.Contains(item3) && (m_UnitStateNow == null || !m_UnitStateNow.m_listFixedStatusImmune.Contains(item3)) && !GetUnitFrameData().m_hsImmuneStatus.Contains(item3))
					{
						AddStatusEffect(item3, bFixedStatus: false);
					}
				}
				m_hsEffectToRemove.Clear();
				foreach (KeyValuePair<NKM_UNIT_STATUS_EFFECT, NKCASEffect> item4 in m_dicStatusEffect)
				{
					if (!HasStatus(item4.Key))
					{
						NKCASEffect value = item4.Value;
						if (value != null)
						{
							value.Stop(bForce: true);
							value.m_bAutoDie = true;
						}
						m_hsEffectToRemove.Add(item4.Key);
					}
				}
				foreach (NKM_UNIT_STATUS_EFFECT item5 in m_hsEffectToRemove)
				{
					m_dicStatusEffect.Remove(item5);
				}
			}

			private void AddStatusEffect(NKM_UNIT_STATUS_EFFECT status, bool bFixedStatus)
			{
				if (!m_dicStatusEffect.TryGetValue(status, out var value))
				{
					value = null;
				}
				if (value != null && m_NKCGameClient.GetNKCEffectManager().IsLiveEffect(value.m_EffectUID))
				{
					return;
				}
				NKMUnitStatusTemplet nKMUnitStatusTemplet = NKMUnitStatusTemplet.Find(status);
				if (nKMUnitStatusTemplet == null || (bFixedStatus && !nKMUnitStatusTemplet.m_bShowFixedStatus) || string.IsNullOrEmpty(nKMUnitStatusTemplet.m_StatusEffectName))
				{
					return;
				}
				float fScaleFactor = 1f;
				float offsetY;
				string boneName;
				switch (nKMUnitStatusTemplet.m_StatusEffectPosition)
				{
				case NKMUnitStatusTemplet.EffectPosition.OverGuageMark:
					offsetY = 80f;
					boneName = "";
					break;
				case NKMUnitStatusTemplet.EffectPosition.Head:
					offsetY = 0f;
					boneName = "BIP01_HEAD";
					fScaleFactor = GetUnitTemplet().m_fBuffEffectScaleFactor;
					break;
				default:
					boneName = "BIP01_PELVIS";
					if (m_NKCASUnitSpineSprite.GetBone(boneName) == null)
					{
						boneName = "BIP01_SPINE1";
					}
					fScaleFactor = GetUnitTemplet().m_fBuffEffectScaleFactor;
					offsetY = 0f;
					break;
				case NKMUnitStatusTemplet.EffectPosition.Ground:
					offsetY = 0f;
					boneName = "";
					fScaleFactor = GetUnitTemplet().m_fBuffEffectScaleFactor;
					break;
				}
				NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(nKMUnitStatusTemplet.m_StatusEffectName, nKMUnitStatusTemplet.m_StatusEffectName);
				value = m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, nKMAssetName.m_BundleName, nKMAssetName.m_AssetName, NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, m_UnitSyncData.m_PosX, m_UnitSyncData.m_PosZ + m_UnitSyncData.m_JumpYPos, m_UnitSyncData.m_PosZ, bRight: true, fScaleFactor, 0f, offsetY, -1f, m_bUseZtoY: false, 0f, bUseZScale: true, boneName, bUseBoneRotate: false, bAutoDie: false, "BASE");
				m_dicStatusEffect[status] = value;
				if (value != null)
				{
					m_llEffect.AddLast(value);
					if (nKMUnitStatusTemplet.m_StatusEffectPosition == NKMUnitStatusTemplet.EffectPosition.OverGuageMark)
					{
						value.SetGuageRoot(value: true);
					}
				}
			}

			protected override void ProcessEventEffect(bool bStateEnd = false)
			{
				if (m_UnitStateNow == null)
				{
					return;
				}
				if (bStateEnd)
				{
					foreach (NKCASEffect item in m_llEffect)
					{
						if (item != null && item.m_bStateEndStop && m_NKCGameClient.GetNKCEffectManager().IsLiveEffect(item.m_EffectUID))
						{
							item.Stop(item.m_bStateEndStopForce);
						}
					}
				}
				else
				{
					LinkedListNode<NKCASEffect> linkedListNode = m_llEffect.First;
					while (linkedListNode != null)
					{
						LinkedListNode<NKCASEffect> next = linkedListNode.Next;
						NKCASEffect value = linkedListNode.Value;
						if (value == null || !m_NKCGameClient.GetNKCEffectManager().IsLiveEffect(value.m_EffectUID))
						{
							m_llEffect.Remove(linkedListNode);
						}
						else if (value.m_bUseGuageAsRoot)
						{
							value.SetPos(m_ObjPosX.GetNowValue() + m_fGagePosX, m_ObjPosZ.GetNowValue() + m_UnitSyncData.m_JumpYPos + m_fGagePosY, m_ObjPosZ.GetNowValue());
						}
						else if (value.m_BoneName.Length > 1)
						{
							Bone bone = m_NKCASUnitSpineSprite.GetBone(value.m_BoneName);
							if (bone != null)
							{
								m_Vector3Temp = m_NKCASUnitSpineSprite.m_SPINE_SkeletonAnimation.transform.TransformPoint(bone.WorldX, bone.WorldY, 0f);
							}
							value.SetPos(m_Vector3Temp.x, m_Vector3Temp.y, m_Vector3Temp.z);
							if (value.m_bUseBoneRotate && bone != null)
							{
								Vector3 eulerAngles = m_NKCASUnitSpineSprite.m_SPINE_SkeletonAnimation.transform.rotation.eulerAngles;
								value.m_EffectInstant.m_Instant.transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z + bone.WorldRotationX);
							}
						}
						else
						{
							value.SetRight(m_UnitSyncData.m_bRight);
							value.SetPos(m_ObjPosX.GetNowValue(), m_ObjPosZ.GetNowValue() + m_UnitSyncData.m_JumpYPos, m_ObjPosZ.GetNowValue());
						}
						linkedListNode = next;
					}
				}
				for (int i = 0; i < m_UnitStateNow.m_listNKMEventEffect.Count; i++)
				{
					NKMEventEffect nKMEventEffect = m_UnitStateNow.m_listNKMEventEffect[i];
					if (nKMEventEffect == null)
					{
						continue;
					}
					if (nKMEventEffect.m_bCutIn)
					{
						NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
						if (gameOptionData != null && !gameOptionData.ViewSkillCutIn)
						{
							continue;
						}
					}
					if (CheckEventCondition(nKMEventEffect.m_Condition) && nKMEventEffect.IsRightSkin(m_UnitData.m_SkinID))
					{
						bool flag = false;
						if (nKMEventEffect.m_bStateEndTime && bStateEnd)
						{
							flag = true;
						}
						else if (EventTimer(nKMEventEffect.m_bAnimTime, nKMEventEffect.m_fEventTime, bOneTime: true) && !nKMEventEffect.m_bStateEndTime)
						{
							flag = true;
						}
						if (flag)
						{
							ApplyEventEffect(nKMEventEffect);
						}
					}
				}
			}

			public override void ApplyEventEffect(NKMEventEffect cNKMEventEffect)
			{
				m_Vector3Temp2.Set(cNKMEventEffect.m_OffsetX, cNKMEventEffect.m_OffsetY, cNKMEventEffect.m_OffsetZ);
				if (cNKMEventEffect.m_bFixedPos)
				{
					m_Vector3Temp.Set(cNKMEventEffect.m_OffsetX, cNKMEventEffect.m_OffsetY, cNKMEventEffect.m_OffsetZ);
					m_Vector3Temp2.Set(0f, 0f, 0f);
				}
				else if (cNKMEventEffect.m_BoneName.Length > 1)
				{
					Bone bone = m_NKCASUnitSpineSprite.GetBone(cNKMEventEffect.m_BoneName);
					if (bone != null)
					{
						m_Vector3Temp = m_NKCASUnitSpineSprite.m_SPINE_SkeletonAnimation.transform.TransformPoint(bone.WorldX, bone.WorldY, 0f);
					}
				}
				else
				{
					m_Vector3Temp.Set(m_ObjPosX.GetNowValue(), m_ObjPosZ.GetNowValue() + m_UnitSyncData.m_JumpYPos, m_ObjPosZ.GetNowValue());
					if (cNKMEventEffect.m_bLandConnect)
					{
						m_Vector3Temp.y = m_ObjPosZ.GetNowValue();
					}
				}
				bool bRight = m_UnitSyncData.m_bRight;
				if (cNKMEventEffect.m_bForceRight)
				{
					bRight = true;
				}
				if (cNKMEventEffect.m_bCutIn)
				{
					m_NKCGameClient.GetNKCEffectManager().StopCutInEffect();
				}
				string effectName = cNKMEventEffect.GetEffectName(m_UnitData);
				NKCASEffect nKCASEffect = m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, effectName, effectName, cNKMEventEffect.m_ParentType, m_Vector3Temp.x, m_Vector3Temp.y, m_Vector3Temp.z, bRight, cNKMEventEffect.m_fScaleFactor, m_Vector3Temp2.x, m_Vector3Temp2.y, m_Vector3Temp2.z, cNKMEventEffect.m_bUseOffsetZtoY, cNKMEventEffect.m_fAddRotate, cNKMEventEffect.m_bUseZScale, cNKMEventEffect.m_BoneName, cNKMEventEffect.m_bUseBoneRotate, bAutoDie: true, cNKMEventEffect.m_AnimName, cNKMEventEffect.m_fAnimSpeed, bNotStart: false, cNKMEventEffect.m_bCutIn, cNKMEventEffect.m_fReserveTime);
				if (nKCASEffect != null)
				{
					if (cNKMEventEffect.m_bHold || cNKMEventEffect.m_bStateEndStop)
					{
						nKCASEffect.m_bStateEndStop = cNKMEventEffect.m_bStateEndStop;
						nKCASEffect.m_bStateEndStopForce = cNKMEventEffect.m_bStateEndStopForce;
						m_llEffect.AddLast(nKCASEffect);
					}
					nKCASEffect.SetUseMasterAnimSpeed(cNKMEventEffect.m_UseMasterAnimSpeed);
					nKCASEffect.SetApplyStopTime(cNKMEventEffect.m_ApplyStopTime);
				}
			}

			protected override void ProcessEventHyperSkillCutIn(bool bStateEnd)
			{
				if (bStateEnd || m_UnitStateNow == null)
				{
					return;
				}
				NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
				if ((gameOptionData != null && !gameOptionData.ViewSkillCutIn) || !m_bHyperCutinLoaded)
				{
					return;
				}
				for (int i = 0; i < m_UnitStateNow.m_listNKMEventHyperSkillCutIn.Count; i++)
				{
					NKMEventHyperSkillCutIn nKMEventHyperSkillCutIn = m_UnitStateNow.m_listNKMEventHyperSkillCutIn[i];
					if (nKMEventHyperSkillCutIn != null && CheckEventCondition(nKMEventHyperSkillCutIn.m_Condition))
					{
						bool flag = false;
						if (EventTimer(nKMEventHyperSkillCutIn.m_bAnimTime, nKMEventHyperSkillCutIn.m_fEventTime, bOneTime: true))
						{
							flag = true;
						}
						if (flag)
						{
							ApplyEventHyperSkillCutIn(nKMEventHyperSkillCutIn);
						}
					}
				}
			}

			public override void ApplyEventHyperSkillCutIn(NKMEventHyperSkillCutIn cNKMEventHyperSkillCutIn)
			{
				m_NKCGameClient.GetNKCEffectManager().StopCutInEffect();
				float num = 1.1f;
				num = m_NKCGameClient.GetGameRuntimeData().m_NKM_GAME_SPEED_TYPE switch
				{
					NKM_GAME_SPEED_TYPE.NGST_2 => 1.5f, 
					NKM_GAME_SPEED_TYPE.NGST_05 => 0.6f, 
					_ => 1.1f, 
				};
				m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, cNKMEventHyperSkillCutIn.m_BGEffectName, cNKMEventHyperSkillCutIn.m_BGEffectName, NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, 0f, 0f, 0f, GetUnitSyncData().m_bRight, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: true, "BASE", num, bNotStart: false, bCutIn: true, 0f, cNKMEventHyperSkillCutIn.m_fDurationTime);
				string skillCutin = NKMSkinManager.GetSkillCutin(GetUnitData(), cNKMEventHyperSkillCutIn.m_CutInEffectName);
				m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, skillCutin, skillCutin, NKM_EFFECT_PARENT_TYPE.NEPT_NUF_BEFORE_HUD_CONTROL_EFFECT, 0f, 0f, 0f, GetUnitSyncData().m_bRight, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: true, cNKMEventHyperSkillCutIn.m_CutInEffectAnimName, num, bNotStart: false, bCutIn: true);
				NKCASEffect nKCASEffect = m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, "AB_FX_SKILL_CUTIN_COMMON_DESC", "AB_FX_SKILL_CUTIN_COMMON_DESC", NKM_EFFECT_PARENT_TYPE.NEPT_NUF_BEFORE_HUD_CONTROL_EFFECT, 0f, 0f, 0f, GetUnitSyncData().m_bRight, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: true, "BASE", num, bNotStart: false, bCutIn: true, 0f, cNKMEventHyperSkillCutIn.m_fDurationTime);
				if (nKCASEffect != null)
				{
					nKCASEffect.Init_AB_FX_SKILL_CUTIN_COMMON_DESC();
					NKCUtil.SetLabelText(nKCASEffect.m_AB_FX_SKILL_CUTIN_COMMON_DESC_UNIT_NAME, GetUnitTemplet().m_UnitTempletBase.GetUnitName());
					if (GetUnitSyncData().m_bRight)
					{
						NKCUtil.SetRectTransformLocalRotate(nKCASEffect.m_AB_FX_SKILL_CUTIN_COMMON_DESC_UNIT_NAME_RectTransform, 0f, 0f, 0f);
					}
					else
					{
						NKCUtil.SetRectTransformLocalRotate(nKCASEffect.m_AB_FX_SKILL_CUTIN_COMMON_DESC_UNIT_NAME_RectTransform, 0f, 180f, 0f);
					}
					NKMUnitSkillTemplet skillTempletNowState = GetSkillTempletNowState();
					if (skillTempletNowState != null)
					{
						NKCUtil.SetLabelText(nKCASEffect.m_AB_FX_SKILL_CUTIN_COMMON_DESC_SKILL_NAME, skillTempletNowState.GetSkillName());
					}
					else
					{
						NKCUtil.SetLabelText(nKCASEffect.m_AB_FX_SKILL_CUTIN_COMMON_DESC_SKILL_NAME, "");
					}
					if (GetUnitSyncData().m_bRight)
					{
						NKCUtil.SetRectTransformLocalRotate(nKCASEffect.m_AB_FX_SKILL_CUTIN_COMMON_DESC_SKILL_NAME_RectTransform, 0f, 0f, 0f);
					}
					else
					{
						NKCUtil.SetRectTransformLocalRotate(nKCASEffect.m_AB_FX_SKILL_CUTIN_COMMON_DESC_SKILL_NAME_RectTransform, 0f, 180f, 0f);
					}
				}
			}

			protected override void ProcessBuff()
			{
				base.ProcessBuff();
				if (m_BuffUnitLevelLastUpdate != GetUnitFrameData().m_BuffUnitLevel)
				{
					m_BuffUnitLevelLastUpdate = GetUnitFrameData().m_BuffUnitLevel;
					m_UNIT_LEVEL_TEXT_Text.SetLevel(m_UnitData, GetUnitFrameData().m_BuffUnitLevel);
					NKCGameHudDeckSlot hudDeckByUnitUID = m_NKCGameClient.GetGameHud().GetHudDeckByUnitUID(m_UnitData.m_UnitUID);
					if (hudDeckByUnitUID != null)
					{
						hudDeckByUnitUID.SetDeckUnitLevel(m_UnitData, GetUnitFrameData().m_BuffUnitLevel);
					}
				}
				m_GameUnitObject.ProcessBuffIcon(this);
				foreach (KeyValuePair<short, NKCASEffect> item in m_dicBuffEffect)
				{
					NKCASEffect value = item.Value;
					if (value == null)
					{
						continue;
					}
					float fX = 0f;
					float fY = GetUnitSyncData().m_PosZ + GetUnitSyncData().m_JumpYPos;
					float fZ = GetUnitSyncData().m_PosZ - 0.01f;
					NKMBuffData buff = GetBuff(item.Key);
					if (buff != null)
					{
						string text = "";
						text = ((buff.m_BuffSyncData.m_MasterGameUnitUID != GetUnitSyncData().m_GameUnitUID) ? buff.m_NKMBuffTemplet.m_SlaveEffectBoneName : buff.m_NKMBuffTemplet.m_MasterEffectBoneName);
						if (!buff.m_NKMBuffTemplet.IsFixedPosBuff() || buff.m_BuffSyncData.m_bRangeSon)
						{
							if (text.Length > 1)
							{
								if (m_NKCASUnitSpineSprite.GetBoneWorldPos(text, ref m_Vector3Temp))
								{
									fX = m_Vector3Temp.x;
									fY = m_Vector3Temp.y;
									fZ = m_Vector3Temp.z - 1f;
								}
								else
								{
									fX = GetUnitSyncData().m_PosX + m_fGagePosX;
								}
							}
							else
							{
								fX = GetUnitSyncData().m_PosX + m_fGagePosX;
							}
						}
						else
						{
							fX = buff.m_fBuffPosX;
						}
					}
					value.SetPos(fX, fY, fZ);
					if (!buff.m_NKMBuffTemplet.m_bIgnoreUnitScaleFactor)
					{
						value.SetScaleFactor(GetUnitTemplet().m_fBuffEffectScaleFactor, GetUnitTemplet().m_fBuffEffectScaleFactor, GetUnitTemplet().m_fBuffEffectScaleFactor);
					}
				}
				LinkedListNode<NKCASEffect> linkedListNode = m_llBuffTextEffect.First;
				while (linkedListNode != null)
				{
					LinkedListNode<NKCASEffect> next = linkedListNode.Next;
					NKCASEffect value2 = linkedListNode.Value;
					if (value2 == null)
					{
						continue;
					}
					value2.SetPos(m_UnitSyncData.m_PosX + m_fGagePosX, m_UnitSyncData.m_PosZ + m_UnitSyncData.m_JumpYPos + m_fGagePosY + 30f + 35f + 35f * (float)(int)value2.m_BuffDescTextPosYIndex, m_UnitSyncData.m_PosZ);
					if ((value2.m_bPlayed && !m_NKCGameClient.GetNKCEffectManager().IsLiveEffect(value2.m_EffectUID)) || value2.m_MasterUnitGameUID != GetUnitDataGame().m_GameUnitUID)
					{
						m_llBuffTextEffect.Remove(linkedListNode);
						if (m_BuffDescTextPosYIndex > 1)
						{
							m_BuffDescTextPosYIndex = 0;
						}
					}
					linkedListNode = next;
				}
				if (m_llBuffTextEffect.Count == 0)
				{
					m_BuffDescTextPosYIndex = 0;
				}
				foreach (KeyValuePair<short, NKCASEffect> item2 in m_dicBuffEffectRange)
				{
					NKCASEffect value3 = item2.Value;
					if (value3 != null)
					{
						float fX2 = 0f;
						NKMBuffData buff2 = GetBuff(item2.Key);
						if (buff2 != null)
						{
							fX2 = ((buff2.m_NKMBuffTemplet.IsFixedPosBuff() && !buff2.m_BuffSyncData.m_bRangeSon) ? buff2.m_fBuffPosX : (GetUnitSyncData().m_PosX + m_fGagePosX));
						}
						value3.SetPos(fX2, GetUnitSyncData().m_PosZ, GetUnitSyncData().m_PosZ);
					}
				}
			}

			protected override bool ProcessDangerCharge()
			{
				return base.ProcessDangerCharge();
			}

			public override float GetBasePosX(NKMEventPosData.MoveBase moveBase, NKMEventPosData.MoveBaseType posType, bool isATeam, float mapPosFactor, params (NKMEventPosData.EventPosExtraUnitType, NKMUnit)[] extraParams)
			{
				if (m_NKCGameClient.IsReversePosTeam(m_NKCGameClient.m_MyTeam))
				{
					isATeam = !isATeam;
				}
				return base.GetBasePosX(moveBase, posType, isATeam, mapPosFactor, extraParams);
			}

			public override bool GetOffsetDirRight(NKMEventPosData.MoveOffset offsetType, float basePos, bool isATeam, float mapPosFactor, params (NKMEventPosData.EventPosExtraUnitType, NKMUnit)[] extraParams)
			{
				if (m_NKCGameClient.IsReversePosTeam(m_NKCGameClient.m_MyTeam))
				{
					isATeam = !isATeam;
				}
				return base.GetOffsetDirRight(offsetType, basePos, isATeam, mapPosFactor, extraParams);
			}

			public override void DamageReact(NKMDamageInst cNKMDamageInst, bool bBuffDamage)
			{
				base.DamageReact(cNKMDamageInst, bBuffDamage);
				if (cNKMDamageInst == null || cNKMDamageInst.m_Templet == null || NKCScenManager.GetScenManager().GetNKCPowerSaveMode().GetEnable() || cNKMDamageInst.m_ReActResult == NKM_REACT_TYPE.NRT_NO)
				{
					return;
				}
				bool flag = false;
				if (cNKMDamageInst.m_ReAttackCount == 0)
				{
					if (cNKMDamageInst.m_EventAttack != null && cNKMDamageInst.m_EventAttack.m_AttackUnitCount + cNKMDamageInst.m_AttackerAddAttackUnitCount <= cNKMDamageInst.m_AttackCount)
					{
						flag = true;
					}
				}
				else if (cNKMDamageInst.m_bReAttackCountOver)
				{
					flag = true;
				}
				if (!flag && cNKMDamageInst.m_Templet.m_HitSoundName.Length > 1)
				{
					NKCSoundManager.PlaySound(cNKMDamageInst.m_Templet.m_HitSoundName, cNKMDamageInst.m_Templet.m_fLocalVol, m_SpriteObject.transform.position.x, 800f, bLoop: false, 0.1f * (float)cNKMDamageInst.m_listHitUnit.Count);
				}
				if (!flag && NKCCamera.GetDist(this) <= 800f)
				{
					NKCCamera.UpDownCrashCamera(cNKMDamageInst.m_Templet.m_fCameraCrashGap, cNKMDamageInst.m_Templet.m_fCameraCrashTime);
				}
				if (IsAirUnit() && cNKMDamageInst.m_Templet.m_HitEffectAir.Length > 1)
				{
					if (!flag)
					{
						m_NKCGameClient.HitEffect(this, cNKMDamageInst, cNKMDamageInst.m_Templet.m_HitEffectAir, cNKMDamageInst.m_Templet.m_HitEffectAir, cNKMDamageInst.m_Templet.m_HitEffectAirAnimName, cNKMDamageInst.m_Templet.m_fHitEffectAirRange, cNKMDamageInst.m_Templet.m_fHitEffectAirOffsetZ, bHitEffectLand: false);
					}
					else
					{
						m_NKCGameClient.HitEffect(this, cNKMDamageInst, "AB_fx_hit_b_blue_small", "AB_fx_hit_b_blue_small", "BASE", 50f, 0f, bHitEffectLand: false);
					}
				}
				else if (!flag)
				{
					m_NKCGameClient.HitEffect(this, cNKMDamageInst, cNKMDamageInst.m_Templet.m_HitEffect, cNKMDamageInst.m_Templet.m_HitEffect, cNKMDamageInst.m_Templet.m_HitEffectAnimName, cNKMDamageInst.m_Templet.m_fHitEffectRange, cNKMDamageInst.m_Templet.m_fHitEffectOffsetZ, cNKMDamageInst.m_Templet.m_bHitEffectLand);
				}
				else
				{
					m_NKCGameClient.HitEffect(this, cNKMDamageInst, "AB_fx_hit_b_blue_small", "AB_fx_hit_b_blue_small", "BASE", 50f, 0f, bHitEffectLand: false);
				}
			}

			public void OnRecv(NKMUnitSyncData cNKMUnitSyncData)
			{
				if (cNKMUnitSyncData.GetHP() > 0f)
				{
					ActiveObject(bActive: true);
				}
				SyncDamageData(cNKMUnitSyncData.m_listDamageData);
				SyncBuffData(cNKMUnitSyncData.m_dicBuffData);
				SyncStatusTimeData(cNKMUnitSyncData.m_listStatusTimeData);
				if (m_UnitSyncData.m_StateID != cNKMUnitSyncData.m_StateID || m_UnitSyncData.m_StateChangeCount != cNKMUnitSyncData.m_StateChangeCount)
				{
					StateChange(cNKMUnitSyncData.m_StateID, bForceChange: true, bImmediate: true);
				}
				byte stateID = m_UnitSyncData.m_StateID;
				NKM_UNIT_PLAY_STATE nKM_UNIT_PLAY_STATE = m_UnitSyncData.m_NKM_UNIT_PLAY_STATE;
				m_UnitSyncData.DeepCopyWithoutDamageAndMarkFrom(cNKMUnitSyncData);
				if (nKM_UNIT_PLAY_STATE != m_UnitSyncData.m_NKM_UNIT_PLAY_STATE && m_UnitSyncData.m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING)
				{
					m_EventMovePosX.StopTracking();
					m_EventMovePosZ.StopTracking();
					m_EventMovePosJumpY.StopTracking();
				}
				SyncUnitSyncHalfData(m_UnitSyncData);
				m_TargetUnit = GetTargetUnit();
				m_SubTargetUnit = GetTargetUnit(m_UnitSyncData.m_SubTargetUID, m_UnitTemplet.m_SubTargetFindData);
				m_UnitSyncData.m_StateID = stateID;
				SyncEventMark(cNKMUnitSyncData.m_listNKM_UNIT_EVENT_MARK);
				SyncEventTrigger(cNKMUnitSyncData.m_listInvokedTrigger);
				SyncEventVariables(cNKMUnitSyncData.m_dicEventVariables);
				SyncReaction(cNKMUnitSyncData.m_listUpdatedReaction);
			}

			private void SyncEventMark(List<NKMUnitEventSyncData> lstEventMark)
			{
				if (!m_NKCGameClient.IsShowUI())
				{
					return;
				}
				for (int i = 0; i < lstEventMark.Count; i++)
				{
					NKM_UNIT_EVENT_MARK eventType = lstEventMark[i].eventType;
					float value = lstEventMark[i].value;
					switch (eventType)
					{
					case NKM_UNIT_EVENT_MARK.NUEM_GET_AGRO:
						m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, "AB_FX_EXCLAMATION_MARK", "AB_FX_EXCLAMATION_MARK", NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, m_UnitSyncData.m_PosX + m_fGagePosX, m_UnitSyncData.m_PosZ + m_UnitSyncData.m_JumpYPos + m_fGagePosY + 30f, m_UnitSyncData.m_PosZ, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: true, "", bUseBoneRotate: false, bAutoDie: true, "BASE", 1f, bNotStart: false, bCutIn: false, 0.2f * (float)i);
						break;
					case NKM_UNIT_EVENT_MARK.NUEM_INSTA_KILL:
					{
						Bone bone = m_NKCASUnitSpineSprite.GetBone("BIP01_SPINE1");
						if (bone != null)
						{
							m_Vector3Temp = m_NKCASUnitSpineSprite.m_SPINE_SkeletonAnimation.transform.TransformPoint(bone.WorldX, bone.WorldY, 0f);
						}
						m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, "AB_FX_CMN_INSTANTKILL", "AB_FX_CMN_INSTANTKILL", NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, m_Vector3Temp.x, m_Vector3Temp.y, m_Vector3Temp.z, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: true, "BIP01_SPINE1", bUseBoneRotate: false, bAutoDie: true, "BASE");
						break;
					}
					case NKM_UNIT_EVENT_MARK.NUEM_DISPEL:
					{
						NKCASEffect value2 = m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, "AB_FX_CMN_DISPEL", "AB_FX_CMN_DISPEL", NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, m_Vector3Temp.x, m_Vector3Temp.y, m_Vector3Temp.z, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: true, "BIP01_SPINE1", bUseBoneRotate: false, bAutoDie: true, "BASE");
						m_llEffect.AddLast(value2);
						break;
					}
					case NKM_UNIT_EVENT_MARK.NUEM_RE_RESPAWN_EFFECT:
					case NKM_UNIT_EVENT_MARK.NUEM_TACTICAL_COMMAND_MYTEAM_EFFECT:
					case NKM_UNIT_EVENT_MARK.NUEM_TACTICAL_COMMAND_ENEMY_EFFECT:
						m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, "AB_FX_CMN_RECALL", "AB_FX_CMN_RECALL", NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, m_UnitSyncData.m_PosX + m_fGagePosX, m_UnitSyncData.m_PosZ + m_UnitSyncData.m_JumpYPos, m_UnitSyncData.m_PosZ, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: true, "", bUseBoneRotate: false, bAutoDie: true, "BASE", 1f, bNotStart: false, bCutIn: false, 0.2f * (float)i);
						NKCSoundManager.PlaySound("FX_UI_DUNGEON_RESPONE", 1f, m_SpriteObject.transform.position.x, 1200f);
						break;
					case NKM_UNIT_EVENT_MARK.NUEM_ALLY_RESPAWN_COST:
					case NKM_UNIT_EVENT_MARK.NUEM_ENEMY_RESPAWN_COST:
					{
						bool flag = IsMyTeam();
						if ((eventType == NKM_UNIT_EVENT_MARK.NUEM_ALLY_RESPAWN_COST && !flag) || (eventType == NKM_UNIT_EVENT_MARK.NUEM_ENEMY_RESPAWN_COST && flag))
						{
							return;
						}
						NKCASEffect nKCASEffect = m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, "AB_FX_COST", "AB_FX_COST_NEW", NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, m_UnitSyncData.m_PosX + m_fGagePosX, m_UnitSyncData.m_PosZ + m_UnitSyncData.m_JumpYPos + m_fGagePosY + 30f, m_UnitSyncData.m_PosZ, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: true, "", bUseBoneRotate: false, bAutoDie: true, "BASE", 1f, bNotStart: false, bCutIn: false, 0.2f * (float)i);
						if (nKCASEffect != null && nKCASEffect.m_EffectInstant != null && nKCASEffect.m_EffectInstant.m_Instant != null && nKCASEffect.m_EffectInstant.m_Instant.TryGetComponent<NKCGameFxCost>(out var component))
						{
							component.SetData(value);
						}
						break;
					}
					}
				}
			}

			private void SyncDamageData(List<NKMDamageData> listDamageData)
			{
				for (int i = 0; i < listDamageData.Count; i++)
				{
					NKMDamageData nKMDamageData = listDamageData[i];
					if (nKMDamageData != null)
					{
						switch (nKMDamageData.m_NKM_DAMAGE_RESULT_TYPE)
						{
						case NKM_DAMAGE_RESULT_TYPE.NDRT_NORMAL:
						case NKM_DAMAGE_RESULT_TYPE.NDRT_NO_MARK:
						case NKM_DAMAGE_RESULT_TYPE.NDRT_PROTECT:
						case NKM_DAMAGE_RESULT_TYPE.NDRT_CRITICAL:
						case NKM_DAMAGE_RESULT_TYPE.NDRT_MISS:
						case NKM_DAMAGE_RESULT_TYPE.NDRT_WEAK:
						case NKM_DAMAGE_RESULT_TYPE.NDRT_HEAL:
							SyncDamageData_Damage(nKMDamageData, i);
							break;
						case NKM_DAMAGE_RESULT_TYPE.NDRT_COOL_TIME:
							SyncDamageData_CoolTime(nKMDamageData, i);
							break;
						}
					}
				}
			}

			private void SyncStatusTimeData(List<NKMUnitStatusTimeSyncData> listStatusTimeData)
			{
				foreach (NKMUnitStatusTimeSyncData listStatusTimeDatum in listStatusTimeData)
				{
					if (listStatusTimeDatum != null)
					{
						m_UnitFrameData.m_dicStatusTime[listStatusTimeDatum.m_eStatusType] = listStatusTimeDatum.m_fTime;
						StatusTimeAffectEffect(listStatusTimeDatum.m_eStatusType);
					}
				}
			}

			private void SyncDamageData_Damage(NKMDamageData cNKMDamageData, int i)
			{
				float num = cNKMDamageData.m_FinalDamage;
				if (num > 0f && num < 1f)
				{
					num = 1f;
				}
				if (num == 0f)
				{
					return;
				}
				NKM_DAMAGE_RESULT_TYPE nKM_DAMAGE_RESULT_TYPE = cNKMDamageData.m_NKM_DAMAGE_RESULT_TYPE;
				if (nKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_NORMAL || nKM_DAMAGE_RESULT_TYPE - 2 <= NKM_DAMAGE_RESULT_TYPE.NDRT_CRITICAL)
				{
					if (GetUnitFrameData().m_BarrierBuffData != null)
					{
						GetUnitFrameData().m_BarrierBuffData.m_fBarrierHP -= num;
						if (GetUnitFrameData().m_BarrierBuffData.m_fBarrierHP < 0f)
						{
							GetUnitFrameData().m_BarrierBuffData.m_fBarrierHP = 0f;
						}
					}
					else
					{
						GetUnitFrameData().m_fDangerChargeDamage += num;
						GetUnitFrameData().m_DangerChargeHitCount++;
					}
				}
				if (!m_NKCGameClient.IsShowUI() || NKCScenManager.GetScenManager().GetNKCPowerSaveMode().GetEnable())
				{
					return;
				}
				if (cNKMDamageData.m_NKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_HEAL)
				{
					NKCASEffect nKCASEffect = m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, "AB_FX_BUFF_IN_HEAL", "AB_FX_BUFF_IN_HEAL", NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, m_UnitSyncData.m_PosX, m_UnitSyncData.m_PosZ + m_UnitSyncData.m_JumpYPos, m_UnitSyncData.m_PosZ, bRight: true, GetUnitTemplet().m_fBuffEffectScaleFactor, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: true, "", bUseBoneRotate: false, bAutoDie: true, "BASE", 1f, bNotStart: false, bCutIn: false, 0.2f * (float)i);
					if (nKCASEffect != null)
					{
						m_llEffect.AddLast(nKCASEffect);
					}
				}
				ShowDamageNumber(cNKMDamageData, num, i);
			}

			private void SyncEventTrigger(List<NKMUnitSyncData.InvokedTriggerInfo> lstInvokedTrigger)
			{
				if (lstInvokedTrigger == null)
				{
					return;
				}
				foreach (NKMUnitSyncData.InvokedTriggerInfo item in lstInvokedTrigger)
				{
					NKMUnit unit = m_NKCGameClient.GetUnit(item.masterUnit, bChain: true, bPool: true);
					if (unit != null)
					{
						unit.GetUnitTemplet().GetTriggerSet(item.triggerId);
						RegisterInvokedTrigger(item.triggerId, unit);
					}
				}
			}

			protected override bool InvokeTimedTrigger(NKMUnitTriggerSet triggerSet, float timeBefore, float timeNow)
			{
				return triggerSet.InvokeTimedTriggerClient(timeBefore, timeNow, m_NKCGameClient, this);
			}

			protected override void InvokeEndTriggerEvents(NKMUnitTriggerSet triggerSet)
			{
				triggerSet.InvokeEndTriggerEventClient(m_NKCGameClient, this);
			}

			private void SyncEventVariables(Dictionary<string, int> dicEventVariables)
			{
				foreach (KeyValuePair<string, int> dicEventVariable in dicEventVariables)
				{
					m_UnitSyncData.m_dicEventVariables[dicEventVariable.Key] = dicEventVariable.Value;
				}
			}

			public void SyncReaction(List<NKMUnitSyncData.ReactionSync> lstReaction)
			{
				if (lstReaction == null)
				{
					return;
				}
				foreach (NKMUnitSyncData.ReactionSync item in lstReaction)
				{
					ReactionInstance reactionInstance = GetReactionInstance(item.masterUnitID, item.ID);
					if (reactionInstance != null)
					{
						if (item.Remove)
						{
							reactionInstance.m_bFinished = true;
							continue;
						}
						reactionInstance.m_currentCount = item.m_currentCount;
						reactionInstance.m_fTimeLeft = item.m_fTimeLeft;
					}
					else
					{
						NKMUnit unit = m_NKMGame.GetUnit(item.masterUnitID, bChain: true, bPool: true);
						m_lstReactionEventInstance.Add(new ReactionInstance(unit, item.ID, item.m_fTimeLeft));
					}
				}
			}

			public NKMUnitTemplet.RespawnEffectData GetRespawnEffectData()
			{
				if (m_UnitTemplet.m_lstRespawnEffectData == null)
				{
					return null;
				}
				foreach (NKMUnitTemplet.RespawnEffectData lstRespawnEffectDatum in m_UnitTemplet.m_lstRespawnEffectData)
				{
					if (CheckEventCondition(lstRespawnEffectDatum.Condition))
					{
						return lstRespawnEffectDatum;
					}
				}
				return null;
			}

			public bool HasRespawnOffset()
			{
				if (m_UnitTemplet.m_fForceRespawnXpos >= 0f)
				{
					return true;
				}
				return GetRespawnEffectData()?.HasRespawnOffset ?? false;
			}

			private void ShowDamageNumber(NKMDamageData cNKMDamageData, float fDamage, int i)
			{
				NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
				NKCGameOptionDataSt.GameOptionDamageNumber gameOptionDamageNumber = NKCGameOptionDataSt.GameOptionDamageNumber.Off;
				if (gameOptionData != null)
				{
					gameOptionDamageNumber = gameOptionData.UseDamageAndBuffNumberFx;
				}
				switch (cNKMDamageData.m_NKM_DAMAGE_RESULT_TYPE)
				{
				case NKM_DAMAGE_RESULT_TYPE.NDRT_NO_MARK:
					return;
				case NKM_DAMAGE_RESULT_TYPE.NDRT_PROTECT:
				case NKM_DAMAGE_RESULT_TYPE.NDRT_CRITICAL:
				case NKM_DAMAGE_RESULT_TYPE.NDRT_MISS:
				case NKM_DAMAGE_RESULT_TYPE.NDRT_WEAK:
				case NKM_DAMAGE_RESULT_TYPE.NDRT_HEAL:
					if (gameOptionDamageNumber == NKCGameOptionDataSt.GameOptionDamageNumber.Off)
					{
						return;
					}
					break;
				case NKM_DAMAGE_RESULT_TYPE.NDRT_NORMAL:
				case NKM_DAMAGE_RESULT_TYPE.NDRT_COOL_TIME:
					if (gameOptionDamageNumber != NKCGameOptionDataSt.GameOptionDamageNumber.On)
					{
						return;
					}
					break;
				}
				if (gameOptionDamageNumber == NKCGameOptionDataSt.GameOptionDamageNumber.Off || cNKMDamageData.m_NKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_NO_MARK)
				{
					return;
				}
				string animName = "BASE";
				switch (cNKMDamageData.m_NKM_DAMAGE_RESULT_TYPE)
				{
				case NKM_DAMAGE_RESULT_TYPE.NDRT_CRITICAL:
					animName = "CRITICAL";
					break;
				case NKM_DAMAGE_RESULT_TYPE.NDRT_WEAK:
					animName = "WEAK";
					break;
				case NKM_DAMAGE_RESULT_TYPE.NDRT_PROTECT:
					animName = "PROTECT";
					break;
				}
				if (cNKMDamageData.m_bAttackCountOver || cNKMDamageData.m_NKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_MISS)
				{
					animName = ((cNKMDamageData.m_NKM_DAMAGE_RESULT_TYPE != NKM_DAMAGE_RESULT_TYPE.NDRT_PROTECT) ? "SMALL" : "SMALL_PROTECT");
				}
				NKCASEffect nKCASEffect = m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, "AB_FX_DAMAGE_TEXT", "AB_FX_DAMAGE_TEXT", NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, m_UnitSyncData.m_PosX + m_fGagePosX, m_UnitSyncData.m_PosZ + m_UnitSyncData.m_JumpYPos + m_fGagePosY + 30f, m_UnitSyncData.m_PosZ, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: true, "", bUseBoneRotate: false, bAutoDie: true, animName, 1f, bNotStart: false, bCutIn: false, 0.2f * (float)i);
				if (nKCASEffect == null || nKCASEffect.m_EffectInstant == null || !(nKCASEffect.m_EffectInstant.m_Instant != null))
				{
					return;
				}
				nKCASEffect.DamageTextInit();
				if (!(nKCASEffect.m_DamageText != null) || !(nKCASEffect.m_DamageTextCritical != null))
				{
					return;
				}
				switch (cNKMDamageData.m_NKM_DAMAGE_RESULT_TYPE)
				{
				case NKM_DAMAGE_RESULT_TYPE.NDRT_NORMAL:
				case NKM_DAMAGE_RESULT_TYPE.NDRT_PROTECT:
				case NKM_DAMAGE_RESULT_TYPE.NDRT_CRITICAL:
				case NKM_DAMAGE_RESULT_TYPE.NDRT_WEAK:
					if (gameOptionDamageNumber == NKCGameOptionDataSt.GameOptionDamageNumber.On)
					{
						nKCASEffect.m_DamageText.text = $"{(int)fDamage}";
					}
					else
					{
						nKCASEffect.m_DamageText.text = "";
					}
					break;
				case NKM_DAMAGE_RESULT_TYPE.NDRT_MISS:
					if (gameOptionDamageNumber == NKCGameOptionDataSt.GameOptionDamageNumber.On)
					{
						nKCASEffect.m_DamageText.text = $"<color=FFFFFF>m</color>{(int)fDamage}";
					}
					else
					{
						nKCASEffect.m_DamageText.text = $"<color=FFFFFF>m</color>";
					}
					break;
				case NKM_DAMAGE_RESULT_TYPE.NDRT_HEAL:
					nKCASEffect.m_DamageText.text = $"+{(int)fDamage}";
					break;
				}
				Color color = nKCASEffect.m_DamageText.color;
				if (cNKMDamageData.m_NKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_HEAL)
				{
					color.r = 0f;
					color.g = 1f;
					color.b = 0f;
				}
				else if (!m_NKMGame.IsEnemy(m_NKCGameClient.m_MyTeam, GetUnitDataGame().m_NKM_TEAM_TYPE_ORG))
				{
					color.r = 1f;
					color.g = 0.5f;
					color.b = 0f;
				}
				else
				{
					color.r = 1f;
					color.g = 0f;
					color.b = 0f;
				}
				nKCASEffect.m_DamageText.color = color;
				nKCASEffect.m_DamageTextCritical.color = color;
			}

			private void SyncDamageData_CoolTime(NKMDamageData cNKMDamageData, int i)
			{
				NKCASEffect nKCASEffect = m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, "AB_FX_COOLTIME", "AB_FX_COOLTIME", NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, m_UnitSyncData.m_PosX + m_fGagePosX, m_UnitSyncData.m_PosZ + m_UnitSyncData.m_JumpYPos + m_fGagePosY + 30f, m_UnitSyncData.m_PosZ, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: true, "", bUseBoneRotate: false, bAutoDie: true, "BASE", 1f, bNotStart: false, bCutIn: false, 0.2f * (float)i);
				if (nKCASEffect == null || nKCASEffect.m_EffectInstant == null || !(nKCASEffect.m_EffectInstant.m_Instant != null))
				{
					return;
				}
				nKCASEffect.Init_AB_FX_COOLTIME();
				if (nKCASEffect.m_NKM_UI_HUD_COOLTIME_COUNT_Text != null)
				{
					float num = cNKMDamageData.m_FinalDamage;
					if (num > 0f)
					{
						nKCASEffect.m_NKM_UI_HUD_COOLTIME_COUNT_Text.text = string.Format(NKCUtilString.GET_STRING_SKILL_COOLTIME_INC, (int)num);
						return;
					}
					nKCASEffect.m_NKM_UI_HUD_COOLTIME_COUNT_Text.text = string.Format(NKCUtilString.GET_STRING_SKILL_COOLTIME_DEC, (int)num);
					nKCASEffect.PlayAnim("MINUS");
				}
			}

			private void SetBuffOverlap(short buffID, byte count)
			{
				NKMBuffData buff = GetBuff(buffID);
				if (buff != null && buff.m_BuffSyncData.m_OverlapCount != count)
				{
					buff.m_BuffSyncData.m_OverlapCount = count;
					m_bBuffChangedThisFrame = true;
					m_bPushSimpleSyncData = true;
				}
			}

			private void SyncBuffData(Dictionary<short, NKMBuffSyncData> dicBuffData)
			{
				foreach (KeyValuePair<short, NKMBuffSyncData> dicBuffDatum in dicBuffData)
				{
					NKMBuffSyncData value = dicBuffDatum.Value;
					if (value == null)
					{
						continue;
					}
					if (!IsBuffLive(value.m_BuffID) || value.m_bNew)
					{
						if (AddBuffByID(dicBuffDatum.Key, value.m_BuffStatLevel, value.m_BuffTimeLevel, value.m_MasterGameUnitUID, value.m_bUseMasterStat, value.m_bRangeSon, bStateEndRemove: false, 1) != 0)
						{
							SetBuffOverlap(dicBuffDatum.Key, value.m_OverlapCount);
							m_bBuffChangedThisFrame = true;
						}
						NKMBuffData buff = GetBuff(dicBuffDatum.Key);
						if (buff != null && buff.m_NKMBuffTemplet != null && buff.m_NKMBuffTemplet.m_UnitLevel != 0)
						{
							m_bBuffUnitLevelChangedThisFrame = true;
						}
					}
					else
					{
						NKMBuffData buff2 = GetBuff(value.m_BuffID);
						if (buff2 != null && buff2.m_BuffSyncData.m_MasterGameUnitUID == value.m_MasterGameUnitUID && (buff2.m_BuffSyncData.m_OverlapCount != value.m_OverlapCount || buff2.m_BuffSyncData.m_BuffStatLevel != value.m_BuffStatLevel || buff2.m_BuffSyncData.m_BuffTimeLevel != value.m_BuffTimeLevel))
						{
							buff2.m_BuffSyncData.m_OverlapCount = value.m_OverlapCount;
							buff2.m_BuffSyncData.m_BuffStatLevel = value.m_BuffStatLevel;
							buff2.m_BuffSyncData.m_BuffTimeLevel = value.m_BuffTimeLevel;
							m_bBuffChangedThisFrame = true;
							BuffAffectEffect(buff2);
						}
					}
				}
				m_listBuffDelete.Clear();
				foreach (KeyValuePair<short, NKMBuffData> dicBuffDatum2 in m_UnitFrameData.m_dicBuffData)
				{
					NKMBuffData value2 = dicBuffDatum2.Value;
					if (value2 != null && !dicBuffData.ContainsKey(value2.m_BuffSyncData.m_BuffID))
					{
						m_listBuffDelete.Add(value2.m_BuffSyncData.m_BuffID);
					}
				}
				foreach (short item in m_listBuffDelete)
				{
					DeleteBuff(item, NKMBuffTemplet.BuffEndDTType.NoUse);
				}
				if (m_listBuffDelete.Count > 0)
				{
					m_bBuffChangedThisFrame = true;
				}
				m_listBuffDelete.Clear();
			}

			public override short AddBuffByID(short buffID, byte buffLevel, byte buffTimeLevel, short masterGameUnitUID, bool bUseMasterStat, bool bRangeSon, bool bStateEndRemove, int overlapCount)
			{
				NKCASEffect nKCASEffect = null;
				short num = base.AddBuffByID(buffID, buffLevel, buffTimeLevel, masterGameUnitUID, bUseMasterStat, bRangeSon, bStateEndRemove, overlapCount);
				if (num != 0)
				{
					NKMBuffData buff = GetBuff(buffID);
					if (buff != null && buff.m_NKMBuffTemplet != null)
					{
						string text = "";
						string text2 = "";
						if (buff.m_BuffSyncData.m_MasterGameUnitUID == GetUnitSyncData().m_GameUnitUID)
						{
							text = buff.m_NKMBuffTemplet.m_MasterEffectBoneName;
							text2 = buff.m_NKMBuffTemplet.GetMasterEffectName(m_UnitData.m_SkinID);
						}
						else
						{
							int skinID = m_NKCGameClient.GetUnit(masterGameUnitUID, bChain: true, bPool: true)?.GetUnitData().m_SkinID ?? 0;
							text = buff.m_NKMBuffTemplet.m_SlaveEffectBoneName;
							text2 = buff.m_NKMBuffTemplet.GetSlaveEffectName(skinID);
						}
						float num2 = 0f;
						float posY = GetUnitSyncData().m_PosZ + GetUnitSyncData().m_JumpYPos;
						float posZ = GetUnitSyncData().m_PosZ - 0.01f;
						if (!buff.m_NKMBuffTemplet.IsFixedPosBuff() || bRangeSon)
						{
							if (text.Length > 1)
							{
								if (m_NKCASUnitSpineSprite.GetBoneWorldPos(text, ref m_Vector3Temp))
								{
									num2 = m_Vector3Temp.x;
									posY = m_Vector3Temp.y;
									posZ = m_Vector3Temp.z - 1f;
								}
								else
								{
									num2 = GetUnitSyncData().m_PosX + m_fGagePosX;
								}
							}
							else
							{
								num2 = GetUnitSyncData().m_PosX + m_fGagePosX;
							}
						}
						else
						{
							num2 = buff.m_fBuffPosX;
						}
						if (text2.Length > 1)
						{
							NKCGameOptionData nKCGameOptionData = NKCScenManager.GetScenManager()?.GetGameOptionData();
							if (nKCGameOptionData == null || nKCGameOptionData.UseBuffEffect)
							{
								nKCASEffect = m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, text2, text2, NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, num2, posY, posZ, m_UnitSyncData.m_bRight, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: true, "", bUseBoneRotate: false, bAutoDie: false, "BASE");
								if (nKCASEffect != null)
								{
									if (!m_dicBuffEffect.ContainsKey(buffID))
									{
										m_dicBuffEffect.Add(buffID, nKCASEffect);
									}
									if (!buff.m_NKMBuffTemplet.m_bIgnoreUnitScaleFactor)
									{
										nKCASEffect.SetScaleFactor(GetUnitTemplet().m_fBuffEffectScaleFactor, GetUnitTemplet().m_fBuffEffectScaleFactor, GetUnitTemplet().m_fBuffEffectScaleFactor);
									}
								}
							}
						}
						if (buff.m_BuffSyncData.m_MasterGameUnitUID == GetUnitSyncData().m_GameUnitUID && buff.m_NKMBuffTemplet.m_Range > 0f && buff.m_NKMBuffTemplet.m_RangeEffectName.Length > 1)
						{
							nKCASEffect = m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, buff.m_NKMBuffTemplet.m_RangeEffectName, buff.m_NKMBuffTemplet.m_RangeEffectName, NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, num2, GetUnitSyncData().m_PosZ, GetUnitSyncData().m_PosZ, m_UnitSyncData.m_bRight, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: false, "BASE");
							if (nKCASEffect != null)
							{
								if (!m_dicBuffEffectRange.ContainsKey(buffID))
								{
									m_dicBuffEffectRange.Add(buffID, nKCASEffect);
								}
								float num3 = buff.m_NKMBuffTemplet.m_Range;
								if (buff.m_NKMBuffTemplet.m_bUseUnitSize)
								{
									num3 += m_UnitTemplet.m_UnitSizeX;
								}
								nKCASEffect.SetScaleFactor(num3, num3, num3);
							}
						}
						if (m_NKMGame.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PVP_LEAGUE)
						{
							NKCLeaguePVPMgr.CheckLeagueModeBuff(buff, this);
						}
					}
				}
				return num;
			}

			protected override void BuffAffectEffect(NKMBuffData cNKMBuffData)
			{
				if (cNKMBuffData != null && cNKMBuffData.m_NKMBuffTemplet != null && !cNKMBuffData.m_NKMBuffTemplet.m_bShowBuffText)
				{
					return;
				}
				NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
				if (gameOptionData == null || gameOptionData.UseDamageAndBuffNumberFx == NKCGameOptionDataSt.GameOptionDamageNumber.On)
				{
					bool bDebuff = cNKMBuffData.m_NKMBuffTemplet.m_bDebuff;
					if (cNKMBuffData.m_BuffSyncData.m_bRangeSon)
					{
						bDebuff = cNKMBuffData.m_NKMBuffTemplet.m_bDebuffSon;
					}
					AddBuffDescString(GetBuffDescText(cNKMBuffData.m_NKMBuffTemplet, cNKMBuffData.m_BuffSyncData), bDebuff);
				}
			}

			private void StatusTimeAffectEffect(NKM_UNIT_STATUS_EFFECT status)
			{
				NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
				if (gameOptionData == null || gameOptionData.UseDamageAndBuffNumberFx == NKCGameOptionDataSt.GameOptionDamageNumber.On)
				{
					AddBuffDescString(NKCUtilString.GetStatusName(status, addMark: true), NKMUnitStatusTemplet.IsDebuff(status));
				}
			}

			private void AddBuffDescString(string text, bool bDebuff)
			{
				NKCASEffect nKCASEffect = m_NKCGameClient.GetNKCEffectManager().UseEffect(GetUnitSyncData().m_GameUnitUID, "AB_FX_BUFF_TEXT", "AB_FX_BUFF_TEXT", NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, m_UnitSyncData.m_PosX + m_fGagePosX, m_UnitSyncData.m_PosZ + m_UnitSyncData.m_JumpYPos + m_fGagePosY + 30f + 35f + 35f * (float)(int)m_BuffDescTextPosYIndex, m_UnitSyncData.m_PosZ, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: true, "", bUseBoneRotate: false, bAutoDie: true, "BASE", 1f, bNotStart: false, bCutIn: false, 0.2f * (float)(int)m_BuffDescTextPosYIndex);
				if (nKCASEffect != null && nKCASEffect.m_EffectInstant != null && nKCASEffect.m_EffectInstant.m_Instant != null)
				{
					nKCASEffect.BuffTextInit(m_BuffDescTextPosYIndex);
					if (nKCASEffect.m_BuffText != null)
					{
						nKCASEffect.m_BuffText.text = text;
						if (!bDebuff)
						{
							Color color = nKCASEffect.m_BuffText.color;
							color.r = 1f;
							color.g = 1f;
							color.b = 1f;
							nKCASEffect.m_BuffText.color = color;
						}
						else
						{
							Color color2 = nKCASEffect.m_BuffText.color;
							color2.r = 1f;
							color2.g = 0f;
							color2.b = 0f;
							nKCASEffect.m_BuffText.color = color2;
						}
					}
					m_llBuffTextEffect.AddFirst(nKCASEffect);
				}
				m_BuffDescTextPosYIndex++;
			}

			public string GetBuffDescText(NKMBuffTemplet buffTemplet, NKMBuffSyncData buffSyncData)
			{
				if (!m_NKCGameClient.IsShowUI())
				{
					return "";
				}
				StringBuilder builder = NKMString.GetBuilder();
				bool bAppend = false;
				foreach (NKM_UNIT_STATUS_EFFECT item in buffTemplet.m_ApplyStatus)
				{
					if (bAppend)
					{
						builder.Append(", ");
					}
					builder.Append(NKCUtilString.GetStatusName(item));
					if (buffTemplet.m_bInfinity || buffTemplet.m_bNotDispel)
					{
						builder.Append(NKCUtilString.GET_STRING_INGAME_NUSE_ONLY_NO_DISPEL_CODE);
					}
					bAppend = true;
				}
				foreach (NKM_UNIT_STATUS_EFFECT item2 in buffTemplet.m_ImmuneStatus)
				{
					if (bAppend)
					{
						builder.Append(", ");
					}
					builder.Append(NKCUtilString.GetStatusImmuneName(item2));
					bAppend = true;
				}
				if (buffTemplet.m_AddAttackUnitCount > 0)
				{
					if (bAppend)
					{
						builder.Append(", ");
					}
					builder.Append(NKCStringTable.GetString("SI_BATTLE_BUFF_ADD_ATTACK_UNIT_COUNT"));
					bAppend = true;
				}
				if (buffTemplet.m_fAddAttackRange > 0f)
				{
					if (bAppend)
					{
						builder.Append(", ");
					}
					builder.Append(NKCStringTable.GetString("SI_BATTLE_BUFF_ADD_ATTACK_RANGE"));
					bAppend = true;
				}
				if (buffTemplet.m_bDispelBuff)
				{
					if (bAppend)
					{
						builder.Append(", ");
					}
					builder.Append(NKCStringTable.GetString("SI_BATTLE_BUFF_DISPEL_BUFF"));
					bAppend = true;
				}
				if (buffTemplet.m_bRangeSonDispelBuff && buffSyncData.m_bRangeSon)
				{
					if (bAppend)
					{
						builder.Append(", ");
					}
					builder.Append(NKCStringTable.GetString("SI_BATTLE_BUFF_DISPEL_BUFF"));
					bAppend = true;
				}
				if (buffTemplet.m_bNotCastSummon)
				{
					if (bAppend)
					{
						builder.Append(", ");
					}
					builder.Append(NKCStringTable.GetString("SI_BATTLE_BUFF_NOT_CAST_SUMMON"));
					bAppend = true;
				}
				if (buffTemplet.m_bRangeSonDispelDebuff && buffSyncData.m_bRangeSon)
				{
					if (bAppend)
					{
						builder.Append(", ");
					}
					builder.Append(NKCStringTable.GetString("SI_BATTLE_BUFF_DISPEL_DEBUFF"));
					bAppend = true;
				}
				if (buffTemplet.m_bDispelDebuff)
				{
					if (bAppend)
					{
						builder.Append(", ");
					}
					builder.Append(NKCStringTable.GetString("SI_BATTLE_BUFF_DISPEL_DEBUFF"));
					bAppend = true;
				}
				switch (buffTemplet.m_SuperArmorLevel)
				{
				case NKM_SUPER_ARMOR_LEVEL.NSAL_SKILL:
					if (bAppend)
					{
						builder.Append(", ");
					}
					builder.Append(NKCStringTable.GetString("SI_BATTLE_BUFF_SUPERARMOR_SKILL"));
					bAppend = true;
					break;
				case NKM_SUPER_ARMOR_LEVEL.NSAL_HYPER:
					if (bAppend)
					{
						builder.Append(", ");
					}
					builder.Append(NKCStringTable.GetString("SI_BATTLE_BUFF_SUPERARMOR_HYPER"));
					bAppend = true;
					break;
				case NKM_SUPER_ARMOR_LEVEL.NSAL_SUPER:
					if (bAppend)
					{
						builder.Append(", ");
					}
					builder.Append(NKCStringTable.GetString("SI_BATTLE_BUFF_SUPERARMOR_SUPER"));
					bAppend = true;
					break;
				}
				if (buffTemplet.m_fDamageTransfer > 0f)
				{
					if (bAppend)
					{
						builder.Append(", ");
					}
					builder.Append(NKCStringTable.GetString("SI_BATTLE_DAMAGE_TRANSFER", Mathf.FloorToInt(buffTemplet.m_fDamageTransfer * 100f)));
					bAppend = true;
				}
				if (buffTemplet.m_bGuard)
				{
					if (bAppend)
					{
						builder.Append(", ");
					}
					builder.Append(NKCStringTable.GetString("SI_BATTLE_BUFF_GUARD"));
					bAppend = true;
				}
				if (buffTemplet.m_fDamageReflection > 0f)
				{
					if (bAppend)
					{
						builder.Append(", ");
					}
					builder.Append(NKCStringTable.GetString("SI_BATTLE_DAMAGE_REFLECTION", Mathf.FloorToInt(buffTemplet.m_fDamageReflection * 100f)));
					bAppend = true;
				}
				if (buffTemplet.m_fHealFeedback > 0f)
				{
					if (bAppend)
					{
						builder.Append(", ");
					}
					float num = buffTemplet.m_fHealFeedback;
					if (buffSyncData.m_BuffStatLevel > 0)
					{
						num += buffTemplet.m_fHealFeedbackPerLevel * (float)(buffSyncData.m_BuffStatLevel - 1);
					}
					builder.Append(NKCStringTable.GetString("SI_BATTLE_HEAL_FEEDBACK", Mathf.FloorToInt(num * 100f)));
					bAppend = true;
				}
				if (buffTemplet.m_fHealTransfer > 0f)
				{
					if (bAppend)
					{
						builder.Append(", ");
					}
					builder.Append(NKCStringTable.GetString("SI_BATTLE_HEAL_TRANSFER", Mathf.FloorToInt(buffTemplet.m_fHealTransfer * 100f)));
					bAppend = true;
				}
				if (buffTemplet.m_UnitLevel != 0)
				{
					if (bAppend)
					{
						builder.Append(", ");
					}
					if (buffTemplet.m_UnitLevel > 0)
					{
						builder.AppendFormat(NKCStringTable.GetString("SI_BATTLE_BUFF_UNIT_LEVEL_UP"), buffTemplet.m_UnitLevel * buffSyncData.m_OverlapCount);
					}
					else
					{
						builder.AppendFormat(NKCStringTable.GetString("SI_BATTLE_BUFF_UNIT_LEVEL_DOWN"), buffTemplet.m_UnitLevel * buffSyncData.m_OverlapCount);
					}
					bAppend = true;
				}
				GetBuffDescText(ref bAppend, builder, buffTemplet.m_StatType1, buffTemplet.m_StatValue1, buffTemplet.m_StatAddPerLevel1, buffSyncData.m_BuffStatLevel, buffSyncData.m_OverlapCount);
				GetBuffDescText(ref bAppend, builder, buffTemplet.m_StatType2, buffTemplet.m_StatValue2, buffTemplet.m_StatAddPerLevel2, buffSyncData.m_BuffStatLevel, buffSyncData.m_OverlapCount);
				GetBuffDescText(ref bAppend, builder, buffTemplet.m_StatType3, buffTemplet.m_StatValue3, buffTemplet.m_StatAddPerLevel3, buffSyncData.m_BuffStatLevel, buffSyncData.m_OverlapCount);
				return builder.ToString();
			}

			public void GetBuffDescText(ref bool bAppend, StringBuilder cStringBuilder, NKM_STAT_TYPE statType, int statValue, int statAddPerLevel, byte buffLevel, byte overlapCount)
			{
				if (statType != NKM_STAT_TYPE.NST_END)
				{
					if (bAppend)
					{
						cStringBuilder.Append(", ");
					}
					int num = statValue + statAddPerLevel * (buffLevel - 1);
					num *= overlapCount;
					if (NKMUnitStatManager.IsPercentStat(statType))
					{
						cStringBuilder.Append(NKCUtilString.GetBuffStatFactorShortString(statType, num, TTRate: true));
					}
					else
					{
						cStringBuilder.Append(NKCUtilString.GetBuffStatValueShortString(statType, num, TTRate: true));
					}
					bAppend = true;
				}
			}

			public override bool DeleteBuff(short buffID, NKMBuffTemplet.BuffEndDTType eEndDTType)
			{
				if (m_dicBuffEffect.ContainsKey(buffID))
				{
					NKCASEffect nKCASEffect = m_dicBuffEffect[buffID];
					if (nKCASEffect.m_bEndAnim)
					{
						nKCASEffect.m_bAutoDie = true;
						nKCASEffect.PlayAnim("END");
					}
					else
					{
						m_NKCGameClient.GetNKCEffectManager().DeleteEffect(nKCASEffect.m_EffectUID);
					}
					m_dicBuffEffect.Remove(buffID);
				}
				if (m_dicBuffEffectRange.ContainsKey(buffID))
				{
					NKCASEffect nKCASEffect2 = m_dicBuffEffectRange[buffID];
					m_NKCGameClient.GetNKCEffectManager().DeleteEffect(nKCASEffect2.m_EffectUID);
					m_dicBuffEffectRange.Remove(buffID);
				}
				return base.DeleteBuff(buffID, eEndDTType);
			}

			private float HalfToFloat(ushort usHalf)
			{
				if (usHalf == 0)
				{
					return 0f;
				}
				m_HalfTemp.value = usHalf;
				object obj = m_HalfTemp;
				IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
				return ((IConvertible)obj).ToSingle(invariantCulture);
			}

			private void SyncUnitSyncHalfData(NKMUnitSyncData cNKMUnitSyncData)
			{
				m_UnitFrameData.m_fSpeedX = HalfToFloat(cNKMUnitSyncData.m_usSpeedX);
				m_UnitFrameData.m_fSpeedY = HalfToFloat(cNKMUnitSyncData.m_usSpeedY);
				m_UnitFrameData.m_fSpeedZ = HalfToFloat(cNKMUnitSyncData.m_usSpeedZ);
				m_UnitFrameData.m_fDamageSpeedX = HalfToFloat(cNKMUnitSyncData.m_usDamageSpeedX);
				if (cNKMUnitSyncData.m_bDamageSpeedXNegative)
				{
					m_UnitFrameData.m_fDamageSpeedX *= -1f;
				}
				m_UnitFrameData.m_fDamageSpeedZ = HalfToFloat(cNKMUnitSyncData.m_usDamageSpeedZ);
				m_UnitFrameData.m_fDamageSpeedJumpY = HalfToFloat(cNKMUnitSyncData.m_usDamageSpeedJumpY);
				m_UnitFrameData.m_fDamageSpeedKeepTimeX = HalfToFloat(cNKMUnitSyncData.m_usDamageSpeedKeepTimeX);
				m_UnitFrameData.m_fDamageSpeedKeepTimeZ = HalfToFloat(cNKMUnitSyncData.m_usDamageSpeedKeepTimeZ);
				m_UnitFrameData.m_fDamageSpeedKeepTimeJumpY = HalfToFloat(cNKMUnitSyncData.m_usDamageSpeedKeepTimeJumpY);
				for (int i = 0; i < m_UnitTemplet.m_listSkillStateData.Count; i++)
				{
					if (m_UnitTemplet.m_listSkillStateData[i] != null)
					{
						SetStateCoolTimeClient(m_UnitTemplet.m_listSkillStateData[i].m_StateName, HalfToFloat(cNKMUnitSyncData.m_usSkillCoolTime));
					}
				}
				for (int j = 0; j < m_UnitTemplet.m_listHyperSkillStateData.Count; j++)
				{
					if (m_UnitTemplet.m_listHyperSkillStateData[j] != null)
					{
						SetStateCoolTimeClient(m_UnitTemplet.m_listHyperSkillStateData[j].m_StateName, HalfToFloat(cNKMUnitSyncData.m_usHyperSkillCoolTime));
					}
				}
			}

			protected void SetStateCoolTimeClient(string stateName, float fCoolTime)
			{
				if (stateName.Length <= 1)
				{
					return;
				}
				NKMUnitState unitState = GetUnitState(stateName);
				if (unitState != null)
				{
					if (!m_dicStateCoolTime.ContainsKey(unitState.m_StateID))
					{
						NKMStateCoolTime nKMStateCoolTime = (NKMStateCoolTime)m_NKMGame.GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKMStateCoolTime);
						nKMStateCoolTime.m_CoolTime = fCoolTime;
						m_dicStateCoolTime.Add(unitState.m_StateID, nKMStateCoolTime);
					}
					else
					{
						m_dicStateCoolTime[unitState.m_StateID].m_CoolTime = fCoolTime;
					}
				}
			}

			public void MiniMapFaceWarrning()
			{
				if (m_NKCASUnitMiniMapFace != null)
				{
					m_NKCASUnitMiniMapFace.Warnning();
				}
			}

			public GameObject GetObjectUnitSkillGuage()
			{
				return m_SKILL_GAUGE?.GetSkillGauge();
			}

			public GameObject GetObjectUnitHyperGuage()
			{
				return m_SKILL_GAUGE?.GetHyperGauge();
			}

			public GameObject GetObjectUnitHyper()
			{
				return m_UNIT_SKILL;
			}

			public bool IsMyTeam()
			{
				if (m_NKCGameClient.GetMyTeamData().m_eNKM_TEAM_TYPE == GetUnitDataGame().m_NKM_TEAM_TYPE)
				{
					return true;
				}
				return false;
			}

			public void UseManualSkill()
			{
				if (!NKCReplayMgr.IsPlayingReplay())
				{
					m_NKCGameClient.Send_Packet_GAME_USE_UNIT_SKILL_REQ(GetUnitDataGame().m_GameUnitUID);
				}
			}

			public void OnRecv(NKMGameSyncDataSimple_Unit cNKMGameSyncDataSimple_Unit)
			{
				m_UnitSyncData.m_TargetUID = cNKMGameSyncDataSimple_Unit.m_TargetUID;
				m_UnitSyncData.m_SubTargetUID = cNKMGameSyncDataSimple_Unit.m_SubTargetUID;
				m_UnitSyncData.m_bRight = cNKMGameSyncDataSimple_Unit.m_bRight;
				SyncBuffData(cNKMGameSyncDataSimple_Unit.m_dicBuffData);
				SyncEventMark(cNKMGameSyncDataSimple_Unit.m_listNKM_UNIT_EVENT_MARK);
				SyncStatusTimeData(cNKMGameSyncDataSimple_Unit.m_listStatusTimeData);
				SyncEventTrigger(cNKMGameSyncDataSimple_Unit.m_listInvokedTrigger);
				SyncEventVariables(cNKMGameSyncDataSimple_Unit.m_dicEventVariables);
				SyncReaction(cNKMGameSyncDataSimple_Unit.m_listUpdatedReaction);
			}

			public void OnRecv(NKMPacket_GAME_USE_UNIT_SKILL_ACK cNKMPacket_GAME_USE_UNIT_SKILL_ACK)
			{
				if (cNKMPacket_GAME_USE_UNIT_SKILL_ACK.errorCode == NKM_ERROR_CODE.NEC_OK)
				{
					m_fManualSkillUseAck = 3f;
					m_bManualSkillUseStart = false;
					m_bManualSkillUseStateID = (byte)cNKMPacket_GAME_USE_UNIT_SKILL_ACK.skillStateID;
				}
			}

			public static NKCASUnitSpineSprite OpenUnitSpineSprite(NKMUnitData unitData, bool bSub, bool bAsync)
			{
				NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(unitData);
				if (skinTemplet != null)
				{
					return OpenUnitSpineSprite(skinTemplet, bSub, bAsync);
				}
				return OpenUnitSpineSprite(NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID), bSub, bAsync);
			}

			public static NKCASUnitSpineSprite OpenUnitSpineSprite(NKMUnitTempletBase unitTempletBase, bool bSub, bool bAsync)
			{
				if (unitTempletBase == null)
				{
					return null;
				}
				NKCASUnitSpineSprite nKCASUnitSpineSprite;
				if (bSub && !string.IsNullOrEmpty(unitTempletBase.m_SpriteNameSub))
				{
					nKCASUnitSpineSprite = (NKCASUnitSpineSprite)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitSpineSprite, unitTempletBase.m_SpriteBundleNameSub, unitTempletBase.m_SpriteNameSub, bAsync);
					if (!string.IsNullOrEmpty(unitTempletBase.m_SpriteMaterialNameSub))
					{
						nKCASUnitSpineSprite.SetReplaceMatResource(unitTempletBase.m_SpriteBundleNameSub, unitTempletBase.m_SpriteMaterialNameSub, bAsync);
					}
					else
					{
						nKCASUnitSpineSprite.SetReplaceMatResource("", "", bAsync);
					}
				}
				else
				{
					nKCASUnitSpineSprite = (NKCASUnitSpineSprite)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitSpineSprite, unitTempletBase.m_SpriteBundleName, unitTempletBase.m_SpriteName, bAsync);
					if (!string.IsNullOrEmpty(unitTempletBase.m_SpriteMaterialName))
					{
						nKCASUnitSpineSprite.SetReplaceMatResource(unitTempletBase.m_SpriteBundleName, unitTempletBase.m_SpriteMaterialName, bAsync);
					}
					else
					{
						nKCASUnitSpineSprite.SetReplaceMatResource("", "", bAsync);
					}
				}
				return nKCASUnitSpineSprite;
			}

			public static NKCASUnitSpineSprite OpenUnitSpineSprite(NKMSkinTemplet skinTemplet, bool bSub, bool bAsync)
			{
				if (skinTemplet == null)
				{
					return null;
				}
				NKCASUnitSpineSprite nKCASUnitSpineSprite;
				if (bSub && !string.IsNullOrEmpty(skinTemplet.m_SpriteNameSub))
				{
					nKCASUnitSpineSprite = (NKCASUnitSpineSprite)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitSpineSprite, skinTemplet.m_SpriteBundleNameSub, skinTemplet.m_SpriteNameSub, bAsync);
					if (!string.IsNullOrEmpty(skinTemplet.m_SpriteMaterialNameSub))
					{
						nKCASUnitSpineSprite.SetReplaceMatResource(skinTemplet.m_SpriteBundleNameSub, skinTemplet.m_SpriteMaterialNameSub, bAsync);
					}
					else
					{
						nKCASUnitSpineSprite.SetReplaceMatResource("", "", bAsync);
					}
				}
				else
				{
					nKCASUnitSpineSprite = (NKCASUnitSpineSprite)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitSpineSprite, skinTemplet.m_SpriteBundleName, skinTemplet.m_SpriteName, bAsync);
					if (!string.IsNullOrEmpty(skinTemplet.m_SpriteMaterialName))
					{
						nKCASUnitSpineSprite.SetReplaceMatResource(skinTemplet.m_SpriteBundleName, skinTemplet.m_SpriteMaterialName, bAsync);
					}
					else
					{
						nKCASUnitSpineSprite.SetReplaceMatResource("", "", bAsync);
					}
				}
				return nKCASUnitSpineSprite;
			}
		}
