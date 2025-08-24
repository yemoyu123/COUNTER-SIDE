using System;
using NKC.FX;
using NKC.UI;
using NKM;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace NKC;

public class NKCASUISpineIllust : NKCASUIUnitIllust
{
	public NKCAssetInstanceData m_SpineIllustInstant;

	public RectTransform m_SpineIllustInstant_RectTransform;

	public SkeletonGraphic m_SpineIllustInstant_SkeletonGraphic;

	public SkeletonGraphic m_SpineIllustBG;

	public NKCComSpineSkeletonAnimationEvent m_NKCComSpineSkeletonAnimationEvent;

	public NKC_FX_SPINE_ILLUST m_NKCFxSpineIllust;

	public NKC_FX_SPINE_EVENT_STATE m_NKCFxSpineEventState;

	private Material m_matDefault;

	private Material m_matDefaultBG;

	private Transform m_vfx_back;

	private Transform m_vfx_front;

	private Transform m_trTalk_L;

	private Transform m_trTalk_R;

	private Transform m_trResultTalk;

	private float m_fR = 1f;

	private float m_fG = 1f;

	private float m_fB = 1f;

	private float m_fA = 1f;

	private Color m_ColorOrg;

	private Color m_ColorOrgBG;

	private Color m_ColorTemp;

	public const string DEFAULT_SKIN_NAME = "default";

	public const string UNITONLY_SKIN_NAME = "ONLY_UNIT";

	public const string SKIN_OPTION_NAME = "SKIN_{0}";

	public NKCASUISpineIllust(string bundleName, string name, bool bAsync = false)
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKCASUISpineIllust;
		m_ObjectPoolBundleName = bundleName;
		m_ObjectPoolName = name;
		m_bUnloadable = true;
		Load(bAsync);
	}

	public override void Load(bool bAsync)
	{
		m_SpineIllustInstant = NKCAssetResourceManager.OpenInstance<GameObject>(m_ObjectPoolBundleName, m_ObjectPoolName, bAsync);
	}

	public override bool LoadComplete()
	{
		if (m_SpineIllustInstant == null || m_SpineIllustInstant.m_Instant == null)
		{
			return false;
		}
		m_SpineIllustInstant_RectTransform = m_SpineIllustInstant.m_Instant.GetComponentInChildren<RectTransform>();
		SkeletonGraphic[] componentsInChildren = m_SpineIllustInstant.m_Instant.GetComponentsInChildren<SkeletonGraphic>(includeInactive: true);
		if (componentsInChildren.Length == 1)
		{
			m_SpineIllustInstant_SkeletonGraphic = componentsInChildren[0];
		}
		else if (componentsInChildren.Length > 1)
		{
			SkeletonGraphic[] array = componentsInChildren;
			foreach (SkeletonGraphic skeletonGraphic in array)
			{
				if (string.Compare(skeletonGraphic.gameObject.name, "SPINE_SkeletonGraphic", ignoreCase: true) == 0)
				{
					m_SpineIllustInstant_SkeletonGraphic = skeletonGraphic;
				}
				if (string.Compare(skeletonGraphic.gameObject.name, "SPINE_BG", ignoreCase: true) == 0)
				{
					m_SpineIllustBG = skeletonGraphic;
				}
			}
			if (m_SpineIllustInstant_SkeletonGraphic == null)
			{
				Debug.LogError("복수개의 Spine 오브젝트가 존재하나 SPINE_SkeletonGraphic가 존재하지 않음. 가장 앞의 오브젝트를 메인 오브젝트로 사용함");
				m_SpineIllustInstant_SkeletonGraphic = componentsInChildren[componentsInChildren.Length - 1];
			}
		}
		else
		{
			m_SpineIllustInstant_SkeletonGraphic = null;
		}
		if (m_SpineIllustInstant_SkeletonGraphic != null)
		{
			m_ColorOrg = m_SpineIllustInstant_SkeletonGraphic.color;
			m_matDefault = m_SpineIllustInstant_SkeletonGraphic.material;
		}
		if (m_SpineIllustBG != null)
		{
			m_ColorOrgBG = m_SpineIllustBG.color;
			m_matDefaultBG = m_SpineIllustBG.material;
		}
		m_NKCComSpineSkeletonAnimationEvent = m_SpineIllustInstant.m_Instant.GetComponentInChildren<NKCComSpineSkeletonAnimationEvent>();
		m_NKCFxSpineIllust = m_SpineIllustInstant.m_Instant.GetComponentInChildren<NKC_FX_SPINE_ILLUST>();
		if (m_NKCComSpineSkeletonAnimationEvent != null && m_NKCComSpineSkeletonAnimationEvent.m_EFFECT_ROOT != null)
		{
			m_NKCFxSpineEventState = m_NKCComSpineSkeletonAnimationEvent.m_EFFECT_ROOT.GetComponent<NKC_FX_SPINE_EVENT_STATE>();
		}
		m_vfx_back = m_SpineIllustInstant.m_Instant.transform.Find("VFX_BACK");
		m_vfx_front = m_SpineIllustInstant.m_Instant.transform.Find("VFX_FRONT");
		m_trTalk_L = m_SpineIllustInstant.m_Instant.transform.Find("Root_Speach_Bubble_L");
		m_trTalk_R = m_SpineIllustInstant.m_Instant.transform.Find("Root_Speach_Bubble_R");
		m_trResultTalk = m_SpineIllustInstant.m_Instant.transform.Find("Pos_Result_Speach");
		return true;
	}

	public override void Open()
	{
		if (m_SpineIllustInstant != null)
		{
			SetDefaultMaterial();
			if (m_vfx_back != null)
			{
				m_vfx_back.gameObject.SetActive(value: true);
			}
			if (m_vfx_front != null)
			{
				m_vfx_front.gameObject.SetActive(value: true);
			}
			if (!m_SpineIllustInstant.m_Instant.activeSelf)
			{
				m_SpineIllustInstant.m_Instant.SetActive(value: true);
			}
			SetColor(1f, 1f, 1f, 1f);
		}
	}

	public override void Close()
	{
		if (m_SpineIllustInstant != null)
		{
			m_SpineIllustInstant.Close();
			base.Close();
		}
	}

	public override void SetMaterial(Material mat)
	{
		if (m_SpineIllustInstant_SkeletonGraphic != null)
		{
			m_SpineIllustInstant_SkeletonGraphic.material = mat;
		}
		if (m_SpineIllustBG != null)
		{
			m_SpineIllustBG.material = mat;
		}
	}

	public override void SetDefaultMaterial()
	{
		if (m_SpineIllustInstant_SkeletonGraphic != null)
		{
			m_SpineIllustInstant_SkeletonGraphic.material = m_matDefault;
		}
		if (m_SpineIllustBG != null)
		{
			m_SpineIllustBG.material = m_matDefaultBG;
		}
	}

	protected override NKCASMaterial MakeEffectMaterial(NKCUICharacterView.EffectType effect)
	{
		UnloadEffectMaterial();
		switch (effect)
		{
		case NKCUICharacterView.EffectType.Hologram:
		case NKCUICharacterView.EffectType.HologramClose:
		case NKCUICharacterView.EffectType.Gray:
			return (NKCASMaterial)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASMaterial, "ab_material", "MAT_NKC_SPINE_GRAYSCALE");
		case NKCUICharacterView.EffectType.VersusMaskL:
			return (NKCASMaterial)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASMaterial, "ab_material", "MAT_NKC_SPINE_VERSUS_MASK_L");
		case NKCUICharacterView.EffectType.VersusMaskR:
			return (NKCASMaterial)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASMaterial, "ab_material", "MAT_NKC_SPINE_VERSUS_MASK_R");
		case NKCUICharacterView.EffectType.TwopassTransparency:
			return (NKCASMaterial)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASMaterial, "shaders", "SkeletonGraphic2Pass");
		default:
			return null;
		}
	}

	protected override void ProcessEffect(NKCUICharacterView.EffectType effect)
	{
		if (effect != NKCUICharacterView.EffectType.TwopassTransparency)
		{
			SetZSpacing(0f);
		}
		else
		{
			SetZSpacing(-0.0001f);
		}
	}

	private void SetZSpacing(float value)
	{
		if (m_SpineIllustInstant_SkeletonGraphic != null)
		{
			m_SpineIllustInstant_SkeletonGraphic.MeshGenerator.settings.zSpacing = value;
		}
		if (m_SpineIllustBG != null)
		{
			m_SpineIllustBG.MeshGenerator.settings.zSpacing = value;
		}
	}

	public override void Unload()
	{
		if (m_SpineIllustInstant != null)
		{
			NKCAssetResourceManager.CloseInstance(m_SpineIllustInstant);
			m_SpineIllustInstant = null;
			m_SpineIllustInstant_RectTransform = null;
			m_SpineIllustInstant_SkeletonGraphic = null;
			m_SpineIllustBG = null;
			m_NKCComSpineSkeletonAnimationEvent = null;
			m_NKCFxSpineIllust = null;
			m_NKCFxSpineEventState = null;
			m_matDefault = null;
			m_matDefaultBG = null;
			m_vfx_back = null;
			m_vfx_front = null;
			base.Unload();
		}
	}

	private GameObject GetRootObject()
	{
		if (m_SpineIllustInstant == null)
		{
			return null;
		}
		return m_SpineIllustInstant.m_Instant;
	}

	public override bool PurgeHyperCutsceneIllust()
	{
		if (m_SpineIllustInstant_SkeletonGraphic == null)
		{
			return false;
		}
		GameObject rootObject = GetRootObject();
		if (rootObject == null)
		{
			return false;
		}
		NKCComSpineActiveReset componentInChildren = m_SpineIllustInstant_SkeletonGraphic.gameObject.GetComponentInChildren<NKCComSpineActiveReset>();
		if (componentInChildren == null)
		{
			return false;
		}
		componentInChildren.enabled = false;
		if (m_SpineIllustInstant_SkeletonGraphic != null)
		{
			m_SpineIllustInstant_SkeletonGraphic.enabled = true;
		}
		foreach (Transform item in rootObject.transform)
		{
			if (!item.TryGetComponent<SkeletonGraphic>(out var _))
			{
				NKCUtil.SetGameobjectActive(item, bValue: false);
			}
		}
		if (rootObject.TryGetComponent<Canvas>(out var component2))
		{
			UnityEngine.Object.Destroy(component2);
		}
		return true;
	}

	public override Color GetColor()
	{
		if (m_SpineIllustInstant_SkeletonGraphic == null)
		{
			return Color.white;
		}
		return m_SpineIllustInstant_SkeletonGraphic.color;
	}

	public override void SetColor(Color color)
	{
		SetColor(color.r, color.g, color.b, color.a);
	}

	public override void SetColor(float fR = -1f, float fG = -1f, float fB = -1f, float fA = -1f)
	{
		bool flag = false;
		if (fR != -1f && m_fR != fR)
		{
			m_fR = fR;
			flag = true;
		}
		if (fG != -1f && m_fG != fG)
		{
			m_fG = fG;
			flag = true;
		}
		if (fB != -1f && m_fB != fB)
		{
			m_fB = fB;
			flag = true;
		}
		if (fA != -1f && m_fA != fA)
		{
			m_fA = fA;
			flag = true;
		}
		if (flag)
		{
			if (m_SpineIllustInstant_SkeletonGraphic != null)
			{
				m_ColorTemp.r = m_ColorOrg.r * m_fR;
				m_ColorTemp.g = m_ColorOrg.g * m_fG;
				m_ColorTemp.b = m_ColorOrg.b * m_fB;
				m_ColorTemp.a = m_ColorOrg.a * m_fA;
				m_SpineIllustInstant_SkeletonGraphic.color = m_ColorTemp;
			}
			if (m_SpineIllustBG != null)
			{
				m_ColorTemp.r = m_ColorOrgBG.r * m_fR;
				m_ColorTemp.g = m_ColorOrgBG.g * m_fG;
				m_ColorTemp.b = m_ColorOrgBG.b * m_fB;
				m_ColorTemp.a = m_ColorOrgBG.a * m_fA;
				m_SpineIllustBG.color = m_ColorTemp;
			}
		}
	}

	public override void SetParent(Transform parent, bool worldPositionStays)
	{
		if (m_SpineIllustInstant != null)
		{
			m_SpineIllustInstant.m_Instant.transform.SetParent(parent, worldPositionStays);
		}
	}

	public override void SetAnimation(string AnimationName, bool loop, int trackIndex = 0, bool bForceRestart = true, float fStartTime = 0f, bool bReturnDefault = true)
	{
		if (!(m_SpineIllustInstant_SkeletonGraphic != null) || m_SpineIllustInstant_SkeletonGraphic.AnimationState == null)
		{
			return;
		}
		if (m_SpineIllustInstant_SkeletonGraphic.SkeletonData == null || m_SpineIllustInstant_SkeletonGraphic.SkeletonData.FindAnimation(AnimationName) == null)
		{
			Debug.LogError("Animation name " + AnimationName + " does not exist!");
			return;
		}
		m_SpineIllustInstant_SkeletonGraphic.SetUseHalfUpdate(value: false);
		if (bForceRestart)
		{
			if (m_NKCComSpineSkeletonAnimationEvent != null)
			{
				m_NKCComSpineSkeletonAnimationEvent.AddEvent(bForce: true);
			}
			m_SpineIllustInstant_SkeletonGraphic.Skeleton?.SetToSetupPose();
			TrackEntry trackEntry = m_SpineIllustInstant_SkeletonGraphic.AnimationState.SetAnimation(trackIndex, AnimationName, loop);
			if (fStartTime > 0f)
			{
				trackEntry.TrackTime = fStartTime;
			}
		}
		else
		{
			TrackEntry current = m_SpineIllustInstant_SkeletonGraphic.AnimationState.GetCurrent(trackIndex);
			if (current == null || (current != null && current.Animation.Name != AnimationName))
			{
				m_SpineIllustInstant_SkeletonGraphic.Skeleton?.SetToSetupPose();
				TrackEntry trackEntry2 = m_SpineIllustInstant_SkeletonGraphic.AnimationState.SetAnimation(trackIndex, AnimationName, loop);
				if (fStartTime > 0f)
				{
					trackEntry2.TrackTime = fStartTime;
				}
			}
		}
		ForceUpdateAnimation();
		if (!loop && bReturnDefault)
		{
			AddAnimation(m_eDefaultAnimation, loop: true);
		}
	}

	public override void SetAnimation(eAnimation eAnim, bool loop, int trackIndex = 0, bool bForceRestart = true, float fStartTime = 0f, bool bReturnDefault = true)
	{
		string animationName = NKCASUIUnitIllust.GetAnimationName(eAnim);
		SetAnimation(animationName, loop, trackIndex, bForceRestart, fStartTime, bReturnDefault);
		if (m_SpineIllustInstant_SkeletonGraphic != null)
		{
			m_SpineIllustInstant_SkeletonGraphic.SetUseHalfUpdate(IsHalfFrameAnim(eAnim));
		}
		if (m_SpineIllustBG != null)
		{
			m_SpineIllustBG.SetUseHalfUpdate(IsHalfFrameAnim(eAnim));
		}
	}

	public override bool HasAnimation(eAnimation eAnim)
	{
		string animationName = NKCASUIUnitIllust.GetAnimationName(eAnim);
		return HasAnimation(animationName);
	}

	public override bool HasAnimation(string name)
	{
		if (m_SpineIllustInstant_SkeletonGraphic == null)
		{
			return false;
		}
		if (m_SpineIllustInstant_SkeletonGraphic.SkeletonData == null || m_SpineIllustInstant_SkeletonGraphic.SkeletonData.FindAnimation(name) == null)
		{
			return false;
		}
		return true;
	}

	public override void SetIllustBackgroundEnable(bool bValue)
	{
		SetSkin(bValue ? "default" : "ONLY_UNIT");
		if (m_NKCFxSpineIllust != null)
		{
			m_NKCFxSpineIllust.EnableExceptionalVfx(bValue);
		}
	}

	public override void SetSkin(string skinName)
	{
		SetSkin(m_SpineIllustInstant_SkeletonGraphic, skinName);
		SetSkin(m_SpineIllustBG, skinName);
	}

	public override bool HasSkin(string skinName)
	{
		if (HasSkin(m_SpineIllustInstant_SkeletonGraphic, skinName))
		{
			return true;
		}
		return HasSkin(m_SpineIllustBG, skinName);
	}

	private bool HasSkin(SkeletonGraphic targetSkeleton, string skinName)
	{
		if (targetSkeleton == null || targetSkeleton.SkeletonData == null)
		{
			return false;
		}
		if (targetSkeleton.SkeletonData.FindSkin(skinName) == null)
		{
			return false;
		}
		return true;
	}

	public override int GetSkinOptionCount()
	{
		int num = 0;
		while (true)
		{
			string skinName = $"SKIN_{num + 1}";
			if (!HasSkin(skinName))
			{
				break;
			}
			num++;
		}
		return num;
	}

	public override void SetSkinOption(int index)
	{
		string skin = $"SKIN_{index + 1}";
		SetSkin(skin);
	}

	private void SetSkin(SkeletonGraphic targetSkeleton, string skinName)
	{
		if (targetSkeleton == null || targetSkeleton.SkeletonData == null)
		{
			return;
		}
		Skin skin = targetSkeleton.SkeletonData.FindSkin(skinName);
		if (skin != null)
		{
			targetSkeleton.Skeleton.SetSkin(skin);
			targetSkeleton.Skeleton.SetSlotsToSetupPose();
			if (targetSkeleton.AnimationState != null)
			{
				targetSkeleton.AnimationState.Apply(targetSkeleton.Skeleton);
			}
		}
	}

	public override void ForceUpdateAnimation()
	{
		if (m_SpineIllustInstant_SkeletonGraphic != null)
		{
			m_SpineIllustInstant_SkeletonGraphic.UpdateMesh();
		}
		if (m_SpineIllustBG != null)
		{
			m_SpineIllustBG.UpdateMesh();
		}
	}

	public override void InitializeAnimation()
	{
		if (!(m_SpineIllustInstant_SkeletonGraphic != null))
		{
			return;
		}
		m_SpineIllustInstant_SkeletonGraphic.Initialize(overwrite: true);
		if (!(m_SpineIllustInstant_RectTransform != null))
		{
			return;
		}
		BoneFollowerGraphic[] componentsInChildren = m_SpineIllustInstant_RectTransform.GetComponentsInChildren<BoneFollowerGraphic>(includeInactive: true);
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Initialize();
			}
		}
	}

	private void AddAnimation(string AnimationName, bool loop, float delay, int trackIndex = 0)
	{
		if (m_SpineIllustInstant_SkeletonGraphic != null && m_SpineIllustInstant_SkeletonGraphic.AnimationState != null)
		{
			m_SpineIllustInstant_SkeletonGraphic.AnimationState.AddAnimation(trackIndex, AnimationName, loop, delay);
		}
	}

	private void AddAnimation(eAnimation eAnim, bool loop, float delay = 0f, int trackIndex = 0)
	{
		string animationName = NKCASUIUnitIllust.GetAnimationName(eAnim);
		AddAnimation(animationName, loop, delay, trackIndex);
	}

	public override float GetAnimationTime(eAnimation eAnim)
	{
		return GetAnimationTime(NKCASUIUnitIllust.GetAnimationName(eAnim));
	}

	public override float GetAnimationTime(string animName)
	{
		if (m_SpineIllustInstant_SkeletonGraphic != null && m_SpineIllustInstant_SkeletonGraphic.SkeletonData != null)
		{
			Spine.Animation animation = m_SpineIllustInstant_SkeletonGraphic.SkeletonData.FindAnimation(animName);
			if (animation != null)
			{
				return animation.Duration;
			}
		}
		return 0f;
	}

	public override eAnimation GetCurrentAnimation(int trackIndex = 0)
	{
		if (m_SpineIllustInstant_SkeletonGraphic != null && m_SpineIllustInstant_SkeletonGraphic.AnimationState != null)
		{
			TrackEntry current = m_SpineIllustInstant_SkeletonGraphic.AnimationState.GetCurrent(trackIndex);
			if (current != null && current.Animation != null)
			{
				string name = current.Animation.Name;
				foreach (eAnimation value in Enum.GetValues(typeof(eAnimation)))
				{
					if (name == NKCASUIUnitIllust.GetAnimationName(value))
					{
						return value;
					}
				}
			}
		}
		return eAnimation.NONE;
	}

	public override string GetCurrentAnimationName(int trackIndex = 0)
	{
		if (m_SpineIllustInstant_SkeletonGraphic != null && m_SpineIllustInstant_SkeletonGraphic.AnimationState != null)
		{
			TrackEntry current = m_SpineIllustInstant_SkeletonGraphic.AnimationState.GetCurrent(trackIndex);
			if (current != null && current.Animation != null)
			{
				return current.Animation.Name;
			}
		}
		return "";
	}

	public override float GetCurrentAnimationTime(int trackIndex = 0)
	{
		if (m_SpineIllustInstant_SkeletonGraphic != null && m_SpineIllustInstant_SkeletonGraphic.AnimationState != null)
		{
			TrackEntry current = m_SpineIllustInstant_SkeletonGraphic.AnimationState.GetCurrent(trackIndex);
			if (current != null)
			{
				return current.AnimationTime;
			}
		}
		return 0f;
	}

	public override void SetCurrentAnimationTime(float time, int trackIndex = 0, bool immediate = false)
	{
		if (m_SpineIllustInstant_SkeletonGraphic != null && m_SpineIllustInstant_SkeletonGraphic.AnimationState != null)
		{
			TrackEntry current = m_SpineIllustInstant_SkeletonGraphic.AnimationState.GetCurrent(trackIndex);
			if (current != null)
			{
				if (immediate)
				{
					current.HoldPrevious = false;
					current.MixDuration = 0f;
				}
				current.TrackTime = time;
			}
		}
		if (m_SpineIllustBG != null && m_SpineIllustBG.AnimationState != null)
		{
			TrackEntry current2 = m_SpineIllustBG.AnimationState.GetCurrent(trackIndex);
			if (current2 != null)
			{
				if (immediate)
				{
					current2.HoldPrevious = false;
					current2.MixDuration = 0f;
				}
				current2.TrackTime = time;
			}
		}
		ForceUpdateAnimation();
	}

	public override void SetAnimSpeed(float value)
	{
		if (m_SpineIllustInstant_SkeletonGraphic != null)
		{
			m_SpineIllustInstant_SkeletonGraphic.timeScale = value;
		}
		if (m_SpineIllustBG != null)
		{
			m_SpineIllustBG.timeScale = value;
		}
	}

	public override float GetAnimSpeed()
	{
		if (m_SpineIllustInstant_SkeletonGraphic != null)
		{
			return m_SpineIllustInstant_SkeletonGraphic.timeScale;
		}
		return 1f;
	}

	public override RectTransform GetRectTransform()
	{
		if (m_SpineIllustInstant != null && m_SpineIllustInstant.m_Instant != null)
		{
			return m_SpineIllustInstant.m_Instant.GetComponent<RectTransform>();
		}
		return null;
	}

	private bool IsHalfFrameAnim(eAnimation eAnim)
	{
		if (eAnim == eAnimation.UNIT_TOUCH)
		{
			return false;
		}
		return true;
	}

	public override void SetVFX(bool bSet)
	{
		if (m_NKCFxSpineIllust != null)
		{
			m_NKCFxSpineIllust.InitEventListener();
		}
		if (m_NKCFxSpineEventState != null)
		{
			m_NKCFxSpineEventState.InitEventListener();
		}
		if (m_vfx_back != null)
		{
			NKCUtil.SetGameobjectActive(m_vfx_back.gameObject, bSet);
		}
		if (m_vfx_front != null)
		{
			NKCUtil.SetGameobjectActive(m_vfx_front.gameObject, bSet);
		}
	}

	public override Transform GetTalkTransform(bool bLeft)
	{
		if (!bLeft)
		{
			return m_trTalk_R;
		}
		return m_trTalk_L;
	}

	public override Transform GetResultTalkTransform()
	{
		return m_trResultTalk;
	}

	public override Vector3 GetBoneWorldPosition(string boneName)
	{
		Bone bone = m_SpineIllustInstant_SkeletonGraphic?.Skeleton?.FindBone(boneName);
		if (bone != null)
		{
			float referencePixelsPerUnit = NKCUIManager.FrontCanvas.referencePixelsPerUnit;
			return m_SpineIllustInstant_SkeletonGraphic.transform.TransformPoint(bone.WorldX * referencePixelsPerUnit, bone.WorldY * referencePixelsPerUnit, 0f);
		}
		Debug.LogError("Bone " + boneName + " not found!");
		return Vector3.zero;
	}

	public override void SetTimeScale(float value)
	{
		if (m_SpineIllustInstant_SkeletonGraphic != null)
		{
			m_SpineIllustInstant_SkeletonGraphic.timeScale = value;
		}
		if (m_SpineIllustBG != null)
		{
			m_SpineIllustBG.timeScale = value;
		}
	}

	public override Rect GetWorldRect(bool bRecalculateBound = false)
	{
		if (m_SpineIllustInstant_SkeletonGraphic != null)
		{
			if (!m_bRectCalculated)
			{
				Vector2 vector = m_SpineIllustInstant_SkeletonGraphic.rectTransform.localPosition;
				if (bRecalculateBound)
				{
					m_SpineIllustInstant_SkeletonGraphic.GetLastMesh().RecalculateBounds();
				}
				Bounds bounds = m_SpineIllustInstant_SkeletonGraphic.GetLastMesh().bounds;
				Vector3 size = bounds.size;
				Vector3 center = bounds.center;
				Vector2 pivot = new Vector2(0.5f - center.x / size.x, 0.5f - center.y / size.y);
				m_SpineIllustInstant_SkeletonGraphic.rectTransform.sizeDelta = size;
				m_SpineIllustInstant_SkeletonGraphic.rectTransform.pivot = pivot;
				m_SpineIllustInstant_SkeletonGraphic.rectTransform.localPosition = vector;
				m_bRectCalculated = true;
			}
			return m_SpineIllustInstant_SkeletonGraphic.rectTransform.GetWorldRect();
		}
		if (m_SpineIllustInstant_RectTransform != null)
		{
			return m_SpineIllustInstant_RectTransform.GetWorldRect();
		}
		return default(Rect);
	}
}
