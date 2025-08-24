using System;
using System.Collections;
using ClientPacket.User;
using Cs.Math;
using DG.Tweening;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC;

public class NKCUICharacterView : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler, IScrollHandler
{
	public enum eMode
	{
		Normal,
		CharacterView
	}

	public delegate void OnDragEvent(PointerEventData evt);

	public delegate void OnTouchEvent(PointerEventData evt, NKCUICharacterView charView);

	public enum EffectType
	{
		None,
		Hologram,
		HologramClose,
		Gray,
		VersusMaskL,
		VersusMaskR,
		TwopassTransparency
	}

	[Header("필수")]
	public RectTransform m_rectIllustRoot;

	[Header("유닛 뷰/확대/축소 사용시에만 필요")]
	public ScrollRect m_srScrollRect;

	public RectTransform m_rectSpineIllustPanel;

	public NKCUIRectMove m_rmIllustViewPanel;

	private int m_CurrentIllustUnitID;

	private int m_CurrentIllustSkinID;

	private string m_CurrentIllustUnitStrID;

	private NKM_UNIT_TYPE m_eCurrentUnitType;

	private NKCASUIUnitIllust m_NKCASUIUnitIllust;

	private bool m_bSkillCutin;

	private NKCComUITalkBox m_talkBox;

	private NKCAssetInstanceData m_talkInstance;

	private NKMUnitData m_NKMUnitData;

	private const string BASE_MOVE_SET = "Base";

	private const string VIEWMODE_MOVE_SET = "ViewMode";

	private OnDragEvent dOnDragEvent;

	private OnTouchEvent dOnTouchEvent;

	private Vector2 m_vecDefaultPos = Vector2.zero;

	private float m_fAniTime;

	private bool bTouchEventPossible;

	private bool bMovePossible;

	public float scrollSensibility = 0.1f;

	public const float DEFAULT_MIN_ZOOM_SCALE = 0.1f;

	public const float DEFAULT_MAX_ZOOM_SCALE = 3f;

	private float MIN_ZOOM_SCALE = 0.1f;

	private float MAX_ZOOM_SCALE = 3f;

	private EffectType m_eCurrentEffect;

	private NKCUICharacterViewEffectBase m_mbCurrentEffect;

	private NKCUICharacterViewEffectPinup m_NKCUICharacterViewEffectPinup;

	private Vector2 m_vRootOrigPosition;

	private Vector3 m_vRootOrigScale;

	private Vector2 m_vCharOffset = Vector2.zero;

	private float m_fCharScale = 1f;

	private float m_fRotation;

	private bool m_bFlip;

	public eMode CurrentMode { get; private set; }

	private RectTransform m_rectRoot
	{
		get
		{
			if (m_NKCUICharacterViewEffectPinup == null)
			{
				m_NKCUICharacterViewEffectPinup = GetComponentInChildren<NKCUICharacterViewEffectPinup>();
			}
			if (!(m_NKCUICharacterViewEffectPinup == null))
			{
				return m_NKCUICharacterViewEffectPinup.GetRectTransform();
			}
			return m_rectIllustRoot;
		}
	}

	public NKMUnitData GetCurrentUnitData()
	{
		return m_NKMUnitData;
	}

	public void Init(OnDragEvent onDragEvent = null, OnTouchEvent onTouchEvent = null)
	{
		if (m_srScrollRect != null)
		{
			m_srScrollRect.enabled = false;
		}
		m_vRootOrigPosition = m_rectIllustRoot.anchoredPosition;
		m_vRootOrigScale = m_rectIllustRoot.localScale;
		dOnDragEvent = onDragEvent;
		dOnTouchEvent = onTouchEvent;
	}

	private void OnDestroy()
	{
		CleanUp();
	}

	public void CleanUp()
	{
		CleanupAllEffect();
		CloseCharacterIllust();
		CloseTalk();
	}

	public void CloseCharacterIllust()
	{
		SetAnimationSpeed(1f);
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUIUnitIllust);
		m_NKMUnitData = null;
		m_NKCASUIUnitIllust = null;
		m_CurrentIllustUnitID = 0;
		m_CurrentIllustSkinID = 0;
		m_CurrentIllustUnitStrID = "";
		m_eCurrentUnitType = NKM_UNIT_TYPE.NUT_INVALID;
	}

	public NKCASUIUnitIllust GetUnitIllust()
	{
		return m_NKCASUIUnitIllust;
	}

	public bool HasCharacterIllust()
	{
		return m_NKCASUIUnitIllust != null;
	}

	public bool IsDiffrentCharacter(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return IsDiffrentCharacter(0, 0);
		}
		return IsDiffrentCharacter(unitData.m_UnitID, unitData.m_SkinID);
	}

	public bool IsDiffrentCharacter(int characterID, int skinID)
	{
		if (m_CurrentIllustUnitID == characterID)
		{
			return m_CurrentIllustSkinID != skinID;
		}
		return true;
	}

	public bool IsDiffrentCharacter(string characterStrID, int skinID)
	{
		if (m_CurrentIllustUnitStrID != characterStrID)
		{
			return true;
		}
		return m_CurrentIllustSkinID != skinID;
	}

	public Color GetColor()
	{
		if (m_NKCASUIUnitIllust != null)
		{
			return m_NKCASUIUnitIllust.GetColor();
		}
		return Color.black;
	}

	public void SetColor(float fR = -1f, float fG = -1f, float fB = -1f, float fA = -1f, bool bIncludeEffect = false)
	{
		if (m_NKCASUIUnitIllust == null)
		{
			return;
		}
		if (m_mbCurrentEffect != null)
		{
			Color effectColor = m_mbCurrentEffect.EffectColor;
			if (fR >= 0f)
			{
				fR *= effectColor.r;
			}
			if (fG >= 0f)
			{
				fG *= effectColor.g;
			}
			if (fB >= 0f)
			{
				fB *= effectColor.b;
			}
			if (fA >= 0f)
			{
				fA *= effectColor.a;
			}
			if (bIncludeEffect)
			{
				m_mbCurrentEffect.SetColor(fR, fG, fB, fA);
			}
		}
		m_NKCASUIUnitIllust.SetColor(fR, fG, fB, fA);
	}

	public void SetColor(Color col, bool bIncludeEffect = false)
	{
		if (m_NKCASUIUnitIllust == null)
		{
			return;
		}
		if (m_mbCurrentEffect != null)
		{
			m_NKCASUIUnitIllust.SetColor(m_mbCurrentEffect.EffectColor * col);
			if (bIncludeEffect)
			{
				m_mbCurrentEffect.SetColor(col);
			}
		}
		else
		{
			m_NKCASUIUnitIllust.SetColor(col);
		}
	}

	public void SetCharacterIllust(NKMBackgroundUnitInfo bgUnitInfo, bool bAsync = false, bool bVfX = true)
	{
		if (bgUnitInfo == null)
		{
			CloseCharacterIllust();
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		switch (bgUnitInfo.unitType)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
		case NKM_UNIT_TYPE.NUT_SHIP:
		{
			NKMUnitData unitOrShipFromUID = nKMUserData.m_ArmyData.GetUnitOrShipFromUID(bgUnitInfo.unitUid);
			SetCharacterIllust(unitOrShipFromUID, bAsync, bgUnitInfo.backImage, bVfX, bgUnitInfo.skinOption);
			break;
		}
		case NKM_UNIT_TYPE.NUT_OPERATOR:
		{
			NKMOperator operatorFromUId = nKMUserData.m_ArmyData.GetOperatorFromUId(bgUnitInfo.unitUid);
			SetCharacterIllust(operatorFromUId, bAsync, bgUnitInfo.backImage, bVfX, bgUnitInfo.skinOption);
			break;
		}
		default:
			CloseCharacterIllust();
			return;
		}
		SetOffset(new Vector2(bgUnitInfo.unitPosX, bgUnitInfo.unitPosY));
		if (bgUnitInfo.unitSize == 0f)
		{
			bgUnitInfo.unitSize = 1f;
		}
		SetScale(bgUnitInfo.unitSize);
		SetRotation(bgUnitInfo.rotation);
		SetFlip(bgUnitInfo.flip);
		bool flag = bgUnitInfo.animTime > 0f;
		if (bgUnitInfo.unitFace == 0)
		{
			SetDefaultAnimation(bgUnitInfo.unitType);
		}
		else if (bgUnitInfo.unitFace == -10 && flag)
		{
			SetDefaultAnimation(bgUnitInfo.unitType);
			SetAnimation(NKCASUIUnitIllust.eAnimation.UNIT_TOUCH, loop: false, bgUnitInfo.animTime);
		}
		else
		{
			NKMLobbyFaceTemplet nKMLobbyFaceTemplet = NKMTempletContainer<NKMLobbyFaceTemplet>.Find(bgUnitInfo.unitFace);
			if (nKMLobbyFaceTemplet != null)
			{
				SetDefaultAnimation(nKMLobbyFaceTemplet);
			}
			else
			{
				SetDefaultAnimation(bgUnitInfo.unitType);
			}
		}
		if (flag)
		{
			SetAnimationTrackTime(bgUnitInfo.animTime, 0, ignoreTransition: true);
			SetAnimationSpeed(0f);
		}
		else
		{
			SetAnimationSpeed(1f);
		}
	}

	public void SetCharacterIllust(NKMUnitData unitData, bool bAsync = false, bool bEnableBackground = true, bool bVFX = true, int skinOption = 0)
	{
		if (unitData == null)
		{
			CloseCharacterIllust();
			return;
		}
		m_NKMUnitData = unitData;
		_SetIllust(unitData.m_UnitID, unitData.m_SkinID, bEnableBackground, bVFX, bAsync, skinOption);
	}

	public void SetCharacterIllust(NKMOperator operatorData, bool bAsync = false, bool bEnableBackground = true, bool bVFX = true, int skinOption = 0)
	{
		if (operatorData == null)
		{
			CloseCharacterIllust();
			return;
		}
		m_NKMUnitData = null;
		_SetIllust(operatorData.id, 0, bEnableBackground, bVFX, bAsync, skinOption);
	}

	public void SetCharacterIllust(NKMSkinTemplet skinTemplet, bool bAsync = false, bool bEnableBackground = true, int skinOption = 0)
	{
		if (skinTemplet == null)
		{
			CloseCharacterIllust();
		}
		else
		{
			_SetIllust(skinTemplet.m_SkinEquipUnitID, skinTemplet.m_SkinID, bEnableBackground, bVFX: true, bAsync, skinOption);
		}
	}

	public void SetCharacterIllust(NKMUnitData unitData, int skinID, bool bAsync = false, int skinOption = 0)
	{
		if (unitData == null)
		{
			CloseCharacterIllust();
			return;
		}
		m_NKMUnitData = unitData;
		_SetIllust(unitData.m_UnitID, skinID, bEnableBackground: true, bVFX: true, bAsync, skinOption);
	}

	public void SetCharacterIllust(NKMUnitTempletBase unitTempletBase, int skinID = 0, bool bAsync = false, bool bEnableBackground = true, int skinOption = 0)
	{
		if (unitTempletBase == null)
		{
			CloseCharacterIllust();
		}
		else
		{
			_SetIllust(unitTempletBase.m_UnitID, skinID, bEnableBackground, bVFX: true, bAsync, skinOption);
		}
	}

	public void SetCharacterIllust(int unitID, int skinID = 0, bool bAsync = false, bool bEnableBackground = true, int skinOption = 0)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
		SetCharacterIllust(unitTempletBase, skinID, bAsync, bEnableBackground, skinOption);
	}

	private void _SetIllust(int characterID, int skinID, bool bEnableBackground, bool bVFX, bool bAsync, int skinOption)
	{
		if (m_CurrentIllustUnitID == characterID && m_CurrentIllustSkinID == skinID)
		{
			SetCharacterIllustBackgroundEnable(bEnableBackground);
			SetSkinOption(skinOption);
			return;
		}
		if (m_eCurrentEffect != EffectType.None)
		{
			CleanupAllEffect();
		}
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUIUnitIllust);
		m_CurrentIllustUnitID = characterID;
		m_CurrentIllustSkinID = skinID;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(characterID);
		m_eCurrentUnitType = unitTempletBase.m_NKM_UNIT_TYPE;
		m_CurrentIllustUnitStrID = unitTempletBase.m_UnitStrID;
		m_NKCASUIUnitIllust = NKCResourceUtility.OpenSpineIllust(unitTempletBase, skinID, bAsync);
		if (!bAsync)
		{
			if (m_NKCASUIUnitIllust != null)
			{
				m_NKCASUIUnitIllust.SetParent(m_rectRoot, worldPositionStays: false);
				m_bSkillCutin = m_NKCASUIUnitIllust.PurgeHyperCutsceneIllust();
				SetDefaultAnimation(unitTempletBase);
				m_NKCASUIUnitIllust.SetAnchoredPosition(Vector2.zero);
				SetVFX(bVFX);
				SetCharacterIllustBackgroundEnable(bEnableBackground);
				SetSkinOption(skinOption);
				SetAnimationSpeed(1f);
			}
		}
		else
		{
			StartCoroutine(ProcessAsyncLoad(bEnableBackground, bVFX, skinOption));
		}
	}

	public void SetCharacterIllust(string unitStrID, int skinID, bool bEnableBackground, bool bVFX, bool bAsync, int skinOption = 0)
	{
		if (unitStrID == m_CurrentIllustUnitStrID)
		{
			return;
		}
		if (m_eCurrentEffect != EffectType.None)
		{
			CleanupAllEffect();
		}
		NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUIUnitIllust);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitStrID);
		if (unitTempletBase != null)
		{
			m_eCurrentUnitType = unitTempletBase.m_NKM_UNIT_TYPE;
			m_CurrentIllustUnitID = unitTempletBase.m_UnitID;
			m_CurrentIllustSkinID = skinID;
			m_CurrentIllustUnitStrID = unitStrID;
			m_NKCASUIUnitIllust = NKCResourceUtility.OpenSpineIllust(unitTempletBase, skinID);
		}
		else
		{
			m_eCurrentUnitType = NKM_UNIT_TYPE.NUT_NORMAL;
			m_CurrentIllustUnitID = 0;
			m_CurrentIllustSkinID = 0;
			m_CurrentIllustUnitStrID = unitStrID;
			m_NKCASUIUnitIllust = NKCResourceUtility.OpenSpineIllustWithManualNaming(unitStrID, bAsync);
		}
		if (!bAsync)
		{
			if (m_NKCASUIUnitIllust != null)
			{
				m_NKCASUIUnitIllust.SetParent(m_rectRoot, worldPositionStays: false);
				m_NKCASUIUnitIllust.SetAnchoredPosition(Vector2.zero);
				m_bSkillCutin = m_NKCASUIUnitIllust.PurgeHyperCutsceneIllust();
				SetDefaultAnimation(unitTempletBase);
				SetVFX(bVFX);
				SetCharacterIllustBackgroundEnable(bEnableBackground);
				SetSkinOption(skinOption);
			}
		}
		else
		{
			StartCoroutine(ProcessAsyncLoad(bEnableBackground, bVFX, skinOption));
		}
	}

	private void SetDefaultAnimation(NKMUnitTempletBase templetBase)
	{
		if (m_NKCASUIUnitIllust != null)
		{
			if (templetBase != null)
			{
				m_NKCASUIUnitIllust.SetDefaultAnimation(templetBase, bPlay: true, bInitialize: true);
			}
			else
			{
				SetDefaultAnimation(m_eCurrentUnitType);
			}
		}
	}

	public void SetDefaultAnimation(NKM_UNIT_TYPE unitType, bool bPlay = true)
	{
		if (m_NKCASUIUnitIllust != null)
		{
			switch (unitType)
			{
			case NKM_UNIT_TYPE.NUT_NORMAL:
			case NKM_UNIT_TYPE.NUT_OPERATOR:
				m_NKCASUIUnitIllust.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.UNIT_IDLE, bPlay);
				break;
			case NKM_UNIT_TYPE.NUT_SHIP:
				m_NKCASUIUnitIllust.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SHIP_IDLE, bPlay);
				break;
			}
		}
	}

	public void SetDefaultAnimation(NKCASUIUnitIllust.eAnimation eAnimation, bool bPlay = true)
	{
		if (m_NKCASUIUnitIllust != null)
		{
			if (bPlay && CheckEmotionToEmotion(eAnimation))
			{
				m_NKCASUIUnitIllust.SetDefaultAnimation(eAnimation, bPlay: true, bInitialize: false, m_NKCASUIUnitIllust.GetCurrentAnimationTime());
			}
			else
			{
				m_NKCASUIUnitIllust.SetDefaultAnimation(eAnimation, bPlay);
			}
		}
	}

	public void SetDefaultAnimation(NKMLobbyFaceTemplet faceTemplet)
	{
		if (faceTemplet == null || m_NKCASUIUnitIllust == null)
		{
			return;
		}
		if (Enum.TryParse<NKCASUIUnitIllust.eAnimation>(faceTemplet.AnimationName, out var result))
		{
			if (CheckEmotionToEmotion(result))
			{
				m_NKCASUIUnitIllust.SetDefaultAnimation(result, bPlay: true, bInitialize: false, m_NKCASUIUnitIllust.GetCurrentAnimationTime());
			}
			else
			{
				m_NKCASUIUnitIllust.SetDefaultAnimation(result);
			}
		}
		else
		{
			Debug.LogError("Animation " + faceTemplet.AnimationName + " not exist!");
		}
	}

	private IEnumerator ProcessAsyncLoad(bool bEnableBackground, bool bVFX, int skinOption)
	{
		if (m_NKCASUIUnitIllust != null)
		{
			while (!m_NKCASUIUnitIllust.m_bIsLoaded)
			{
				yield return null;
			}
			m_NKCASUIUnitIllust.SetParent(m_rectRoot, worldPositionStays: false);
			m_bSkillCutin = m_NKCASUIUnitIllust.PurgeHyperCutsceneIllust();
			SetDefaultAnimation(m_eCurrentUnitType);
			m_NKCASUIUnitIllust.SetAnchoredPosition(Vector2.zero);
			SetVFX(bVFX);
			SetCharacterIllustBackgroundEnable(bEnableBackground);
			SetSkinOption(skinOption);
			SetAnimationSpeed(1f);
		}
	}

	public string GetAnimationName(int trackIndex = 0)
	{
		if (m_NKCASUIUnitIllust == null)
		{
			return "";
		}
		return m_NKCASUIUnitIllust.GetCurrentAnimationName(trackIndex);
	}

	public void SetAnimation(NKCASUIUnitIllust.eAnimation Animation, bool loop, float startTime = 0f)
	{
		if (m_NKCASUIUnitIllust != null)
		{
			if (m_bSkillCutin && Animation == NKCASUIUnitIllust.eAnimation.UNIT_TOUCH && !m_NKCASUIUnitIllust.HasAnimation(NKCASUIUnitIllust.eAnimation.UNIT_TOUCH))
			{
				Animation = NKCASUIUnitIllust.eAnimation.UNIT_HYPER_CUTIN;
			}
			if (loop && startTime == 0f && CheckEmotionToEmotion(Animation))
			{
				m_NKCASUIUnitIllust.SetAnimation(Animation, loop, 0, bForceRestart: true, m_NKCASUIUnitIllust.GetCurrentAnimationTime());
			}
			else
			{
				m_NKCASUIUnitIllust.SetAnimation(Animation, loop, 0, bForceRestart: true, startTime);
			}
		}
	}

	public void SetAnimation(string Animation, bool loop, float startTime = 0f)
	{
		if (m_NKCASUIUnitIllust != null)
		{
			string animationName = NKCASUIUnitIllust.GetAnimationName(NKCASUIUnitIllust.eAnimation.UNIT_TOUCH);
			if (m_bSkillCutin && string.Equals(Animation, animationName) && !m_NKCASUIUnitIllust.HasAnimation(animationName))
			{
				Animation = NKCASUIUnitIllust.GetAnimationName(NKCASUIUnitIllust.eAnimation.UNIT_HYPER_CUTIN);
			}
			if (loop && startTime == 0f && CheckEmotionToEmotion(Animation))
			{
				m_NKCASUIUnitIllust.SetAnimation(Animation, loop, 0, bForceRestart: true, m_NKCASUIUnitIllust.GetCurrentAnimationTime());
			}
			else
			{
				m_NKCASUIUnitIllust.SetAnimation(Animation, loop, 0, bForceRestart: true, startTime);
			}
		}
	}

	public float GetCurrentAnimationTime(int trackIndex = 0)
	{
		if (m_NKCASUIUnitIllust == null)
		{
			return 0f;
		}
		return m_NKCASUIUnitIllust.GetCurrentAnimationTime(trackIndex);
	}

	public void SetAnimationTrackTime(float fTrackTime, int trackIndex = 0, bool ignoreTransition = false)
	{
		if (m_NKCASUIUnitIllust != null)
		{
			m_NKCASUIUnitIllust.SetCurrentAnimationTime(fTrackTime, trackIndex, ignoreTransition);
		}
	}

	public void SetAnimationSpeed(float value)
	{
		if (m_NKCASUIUnitIllust != null)
		{
			m_NKCASUIUnitIllust.SetAnimSpeed(value);
		}
	}

	public float GetAnimationSpeed()
	{
		if (m_NKCASUIUnitIllust != null)
		{
			return m_NKCASUIUnitIllust.GetAnimSpeed();
		}
		return 1f;
	}

	private bool CheckEmotionToEmotion(NKCASUIUnitIllust.eAnimation nextAnimation)
	{
		if (NKCASUIUnitIllust.IsEmotionAnimation(nextAnimation))
		{
			NKCASUIUnitIllust.eAnimation currentAnimation = m_NKCASUIUnitIllust.GetCurrentAnimation();
			if (NKCASUIUnitIllust.IsEmotionAnimation(currentAnimation) && m_NKCASUIUnitIllust.GetAnimationTime(nextAnimation).IsNearlyEqual(m_NKCASUIUnitIllust.GetAnimationTime(currentAnimation)))
			{
				return true;
			}
		}
		return false;
	}

	private bool CheckEmotionToEmotion(string nextAnimation)
	{
		if (NKCASUIUnitIllust.IsEmotionAnimation(nextAnimation))
		{
			string currentAnimationName = m_NKCASUIUnitIllust.GetCurrentAnimationName();
			if (NKCASUIUnitIllust.IsEmotionAnimation(currentAnimationName) && m_NKCASUIUnitIllust.GetAnimationTime(nextAnimation).IsNearlyEqual(m_NKCASUIUnitIllust.GetAnimationTime(currentAnimationName)))
			{
				return true;
			}
		}
		return false;
	}

	public void SetDefaultTransform(float fDurationTime = 0.25f)
	{
		m_fAniTime = fDurationTime;
		m_vecDefaultPos = m_rectSpineIllustPanel.anchoredPosition;
	}

	public void SetMode(eMode mode, bool bAnimate = true)
	{
		CurrentMode = mode;
		if (m_rectSpineIllustPanel != null)
		{
			m_rectSpineIllustPanel.DOComplete();
		}
		switch (mode)
		{
		case eMode.Normal:
			if (m_rmIllustViewPanel != null)
			{
				m_rmIllustViewPanel.Move("Base", bAnimate);
			}
			if (m_srScrollRect != null)
			{
				m_srScrollRect.enabled = false;
			}
			if (bAnimate)
			{
				if (m_rectSpineIllustPanel != null)
				{
					m_rectSpineIllustPanel.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.OutCubic);
				}
			}
			else if (m_rectSpineIllustPanel != null)
			{
				m_rectSpineIllustPanel.localRotation = Quaternion.identity;
			}
			if (m_fAniTime > 0f && m_rectSpineIllustPanel != null)
			{
				m_rectSpineIllustPanel.DOAnchorPos3D(m_vecDefaultPos, m_fAniTime);
				m_rectSpineIllustPanel.DOScale(1f, m_fAniTime);
			}
			break;
		case eMode.CharacterView:
			if (m_rmIllustViewPanel != null)
			{
				m_rmIllustViewPanel.Move("ViewMode", bAnimate);
			}
			if (NKCDefineManager.DEFINE_UNITY_STANDALONE())
			{
				break;
			}
			if (bAnimate)
			{
				if (m_rectSpineIllustPanel != null)
				{
					m_rectSpineIllustPanel.DOLocalRotate(new Vector3(0f, 0f, 90f), 0.4f).SetEase(Ease.OutCubic);
				}
			}
			else if (m_rectSpineIllustPanel != null)
			{
				m_rectSpineIllustPanel.localRotation = Quaternion.Euler(0f, 0f, 90f);
			}
			break;
		}
	}

	public void SetVFX(bool bSet)
	{
		if (m_NKCASUIUnitIllust != null)
		{
			m_NKCASUIUnitIllust.SetVFX(bSet);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		switch (CurrentMode)
		{
		case eMode.CharacterView:
			if (Input.touchCount == 1)
			{
				bTouchEventPossible = true;
				bMovePossible = true;
				if (m_srScrollRect != null)
				{
					m_srScrollRect.enabled = CurrentMode == eMode.CharacterView;
				}
			}
			else
			{
				bTouchEventPossible = false;
				bMovePossible = false;
				if (m_srScrollRect != null)
				{
					m_srScrollRect.enabled = false;
				}
			}
			if ((NKCDefineManager.DEFINE_UNITY_EDITOR() || NKCDefineManager.DEFINE_UNITY_STANDALONE()) && Input.touchCount == 0)
			{
				bTouchEventPossible = true;
				bMovePossible = true;
				if (m_srScrollRect != null)
				{
					m_srScrollRect.enabled = CurrentMode == eMode.CharacterView;
				}
			}
			break;
		case eMode.Normal:
			bTouchEventPossible = true;
			bMovePossible = false;
			if (m_srScrollRect != null)
			{
				m_srScrollRect.enabled = false;
			}
			break;
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (!bTouchEventPossible)
		{
			return;
		}
		if (dOnTouchEvent == null)
		{
			if (GetAnimationSpeed() > 0f)
			{
				TouchIllust();
			}
		}
		else
		{
			dOnTouchEvent?.Invoke(eventData, this);
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		bTouchEventPossible = false;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (CurrentMode == eMode.CharacterView)
		{
			if (NKCScenManager.GetScenManager().GetHasPinch())
			{
				OnPinchZoom(NKCScenManager.GetScenManager().GetPinchCenter(), NKCScenManager.GetScenManager().GetPinchDeltaMagnitude());
			}
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				m_srScrollRect.enabled = false;
				OnPinchZoom(eventData.position, (eventData.delta.x + eventData.delta.y) / (float)Screen.width);
			}
			else if (bMovePossible)
			{
				m_srScrollRect.enabled = true;
				if (m_srScrollRect != null)
				{
					m_srScrollRect.OnDrag(eventData);
				}
			}
		}
		dOnDragEvent?.Invoke(eventData);
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (CurrentMode == eMode.CharacterView)
		{
			OnPinchZoom(eventData.position, eventData.scrollDelta.y * scrollSensibility);
		}
	}

	public void TouchIllust()
	{
		NKM_UNIT_TYPE eCurrentUnitType = m_eCurrentUnitType;
		if (eCurrentUnitType == NKM_UNIT_TYPE.NUT_NORMAL || eCurrentUnitType == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			SetAnimation(NKCASUIUnitIllust.eAnimation.UNIT_TOUCH, loop: false);
			PlayTouchVoice();
		}
	}

	public void SetScaleLimit(float min, float max)
	{
		MIN_ZOOM_SCALE = min;
		MAX_ZOOM_SCALE = max;
	}

	public void OnPinchZoom(Vector2 PinchCenter, float pinchMagnitude)
	{
		if (!(m_rectSpineIllustPanel == null))
		{
			float value = GetScale() * Mathf.Pow(4f, pinchMagnitude);
			value = Mathf.Clamp(value, MIN_ZOOM_SCALE, MAX_ZOOM_SCALE);
			SetScale(value);
		}
	}

	private void PlayTouchVoice()
	{
		if (m_NKMUnitData != null)
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_TOUCH, m_NKMUnitData, bIgnoreShowNormalAfterLifeTimeOption: false, bShowCaption: true);
		}
		else
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_TOUCH, m_CurrentIllustUnitID, m_CurrentIllustSkinID, bIgnoreShowNormalAfterLifeTimeOption: false, bShowCaption: true);
		}
	}

	private void OpenTalk(NKMAssetName voiceAssetName, bool bLeft, NKC_UI_TALK_BOX_DIR dir)
	{
		if (voiceAssetName != null && NKCStringTable.GetNationalCode() != NKM_NATIONAL_CODE.NNC_KOREA)
		{
			OpenTalk(bLeft, dir, voiceAssetName.m_BundleName + "@" + voiceAssetName.m_AssetName, 1f);
		}
	}

	public void OpenTalk(bool bLeft, NKC_UI_TALK_BOX_DIR dir, string talk, float fadeTime = 0f)
	{
		if (m_talkBox == null)
		{
			NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_talk_box", "AB_UI_TALK_BOX");
			m_talkBox = nKCAssetInstanceData.m_Instant.GetComponent<NKCComUITalkBox>();
			if (m_talkBox == null)
			{
				NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
				return;
			}
			m_talkInstance = nKCAssetInstanceData;
		}
		Transform talkTransform = m_NKCASUIUnitIllust.GetTalkTransform(bLeft);
		m_talkBox.transform.SetParent(talkTransform);
		m_talkBox.transform.localPosition = Vector3.zero;
		NKCUtil.SetGameobjectActive(m_talkBox, bValue: true);
		m_talkBox.SetDir(dir);
		m_talkBox.SetText(talk, fadeTime);
	}

	private void CloseTalk()
	{
		if (m_talkInstance != null)
		{
			NKCAssetResourceManager.CloseInstance(m_talkInstance);
			m_talkInstance = null;
			m_talkBox = null;
		}
	}

	public void SetCharacterIllustBackgroundEnable(bool bValue)
	{
		if (m_NKCASUIUnitIllust != null)
		{
			m_NKCASUIUnitIllust.SetIllustBackgroundEnable(bValue);
		}
	}

	public void SetSkinOption(int index)
	{
		if (m_NKCASUIUnitIllust != null)
		{
			m_NKCASUIUnitIllust.SetSkinOption(index);
		}
	}

	public void SetSkinOption(string name)
	{
		if (m_NKCASUIUnitIllust != null)
		{
			m_NKCASUIUnitIllust.SetSkin(name);
		}
	}

	public void CloseImmediatelyIllust()
	{
		m_NKCASUIUnitIllust?.Unload();
		m_NKMUnitData = null;
		m_NKCASUIUnitIllust = null;
		m_CurrentIllustUnitID = 0;
		m_CurrentIllustSkinID = 0;
	}

	public EffectType GetCurrEffectType()
	{
		return m_eCurrentEffect;
	}

	public void CleanupAllEffect()
	{
		if (m_mbCurrentEffect != null)
		{
			m_mbCurrentEffect.CleanUp(m_NKCASUIUnitIllust, m_rectRoot);
			UnityEngine.Object.Destroy(m_mbCurrentEffect);
		}
		m_eCurrentEffect = EffectType.None;
		m_mbCurrentEffect = null;
		CleanUpPinupEffect();
	}

	public void SetPinup(bool bPinup, float bEasingTime)
	{
		if (m_NKCUICharacterViewEffectPinup != null)
		{
			if (!bPinup && bEasingTime == 0f)
			{
				m_NKCUICharacterViewEffectPinup.SetDeActive();
			}
			else if (bPinup)
			{
				m_NKCUICharacterViewEffectPinup.StartPinupEffect(bEasingTime);
			}
			else
			{
				m_NKCUICharacterViewEffectPinup.ClosePinupEffect(bEasingTime);
			}
		}
	}

	public void CleanUpPinupEffect()
	{
		if (m_NKCUICharacterViewEffectPinup != null)
		{
			m_NKCUICharacterViewEffectPinup.CleanUp(m_NKCASUIUnitIllust, m_rectRoot);
		}
	}

	public void PlayEffect(EffectType type)
	{
		if (m_eCurrentEffect == type)
		{
			return;
		}
		bool flag = MakeEffectInstance(type);
		switch (type)
		{
		case EffectType.Hologram:
			if (!flag)
			{
				NKCUICharacterViewEffectHologram nKCUICharacterViewEffectHologram = m_mbCurrentEffect as NKCUICharacterViewEffectHologram;
				if (nKCUICharacterViewEffectHologram != null)
				{
					nKCUICharacterViewEffectHologram.HologramIn();
				}
			}
			break;
		case EffectType.HologramClose:
		{
			NKCUICharacterViewEffectHologram nKCUICharacterViewEffectHologram2 = m_mbCurrentEffect as NKCUICharacterViewEffectHologram;
			if (nKCUICharacterViewEffectHologram2 != null)
			{
				nKCUICharacterViewEffectHologram2.HologramOut();
			}
			break;
		}
		default:
			if (m_NKCASUIUnitIllust != null)
			{
				m_NKCASUIUnitIllust.SetEffectMaterial(type);
			}
			break;
		case EffectType.None:
			break;
		}
	}

	private bool MakeEffectInstance(EffectType type)
	{
		bool flag;
		if (type == EffectType.None || (uint)(type - 1) > 1u)
		{
			CleanupAllEffect();
			flag = false;
		}
		else if (m_mbCurrentEffect is NKCUICharacterViewEffectHologram)
		{
			flag = false;
		}
		else
		{
			CleanupAllEffect();
			m_mbCurrentEffect = base.gameObject.AddComponent<NKCUICharacterViewEffectHologram>();
			flag = true;
		}
		m_eCurrentEffect = type;
		if (flag)
		{
			m_mbCurrentEffect.SetEffect(m_NKCASUIUnitIllust, m_rectRoot);
		}
		return flag;
	}

	public void SetOffset(Vector2 anchoredPos)
	{
		m_vCharOffset = anchoredPos;
		m_rectIllustRoot.anchoredPosition = m_vRootOrigPosition + m_vCharOffset;
	}

	public Vector3 OffsetToWorldPos(Vector2 offset)
	{
		Vector2 vector = m_vRootOrigPosition + offset;
		return m_rectIllustRoot.parent.TransformPoint(vector.x, vector.y, m_rectIllustRoot.transform.localPosition.z);
	}

	public Vector2 WorldPosToOffset(Vector2 worldPos)
	{
		Vector3 vector = m_rectIllustRoot.parent.InverseTransformPoint(worldPos);
		return new Vector2(vector.x, vector.y) - m_vRootOrigPosition;
	}

	public void SetScale(float scale)
	{
		m_fCharScale = scale;
		Vector3 localScale = m_vRootOrigScale * m_fCharScale;
		localScale.x *= ((!m_bFlip) ? 1 : (-1));
		m_rectIllustRoot.localScale = localScale;
	}

	public void SetRotation(float rotation)
	{
		m_fRotation = rotation;
		m_rectIllustRoot.rotation = Quaternion.Euler(0f, 0f, rotation);
	}

	public void SetFlip(bool value)
	{
		m_bFlip = value;
		Vector3 localScale = m_vRootOrigScale * m_fCharScale;
		localScale.x *= ((!m_bFlip) ? 1 : (-1));
		m_rectIllustRoot.localScale = localScale;
	}

	public Vector2 GetOffset()
	{
		return m_vCharOffset;
	}

	public float GetScale()
	{
		return m_fCharScale;
	}

	public float GetRotation()
	{
		return m_fRotation;
	}

	public bool GetFlip()
	{
		return m_bFlip;
	}

	public Vector2 GetScaleVector()
	{
		Vector3 vector = m_vRootOrigScale * m_fCharScale;
		vector.x *= ((!m_bFlip) ? 1 : (-1));
		return vector;
	}
}
