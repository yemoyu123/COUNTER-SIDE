using System.Collections;
using System.Collections.Generic;
using Cs.Math;
using NKM.Templet;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NKC.UI.Contract;

public class NKCUIContractSequence : NKCUIBase, IInitializePotentialDragHandler, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
	private enum eSequence
	{
		NONE = -1,
		START,
		WAIT,
		SCENE_1,
		SCENE_2,
		ENABLE,
		DRAG,
		AUTO_CLOSE,
		AUTO_OPEN
	}

	private enum RECEPTION_MEMBER
	{
		HANA,
		HANA_SUYEON,
		GAB_HANA_SUYEON
	}

	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONTRACT_V2";

	private const string UI_ASSET_NAME = "NKM_UI_CONTRACT_V2_SEQUENCE";

	private static NKCUIContractSequence m_Instance;

	[Header("Background")]
	public RectTransform m_rtBackground;

	[Header("ARRIVAL")]
	public GameObject m_NKM_UI_CONTRACT_ARRIVAL;

	public Animator m_NUM_CONTRACT_NEWMEMBER_ARRIVAL;

	[Header("START")]
	public GameObject m_NKM_UI_CONTRACT_START;

	public SkeletonAnimation m_NKM_CONTRACT_START;

	[Header("HELI")]
	public GameObject m_NKM_UI_CONTRACT_HELI;

	public SkeletonAnimation m_NKM_CONTRACT_HELI_FX1;

	public SkeletonAnimation m_NKM_CONTRACT_HELI_FX2;

	public SkeletonAnimation m_NKM_CONTRACT_HELI_FX3;

	public SkeletonAnimation m_NKM_CONTRACT_HELI_FX4;

	public SkeletonAnimation m_NKM_CONTRACT_HELI_FX5;

	public SkeletonAnimation m_NKM_CONTRACT_HELI_FX6;

	public SkeletonAnimation m_NKM_CONTRACT_HELI_FX7;

	[Header("VFX")]
	public Animator m_NKM_CONTRACT_HELI_Animator;

	public GameObject m_VFX;

	public GameObject EnhanceSR;

	public GameObject EnhanceSSR;

	public GameObject EnhanceAWAKEN;

	public MeshRenderer m_mrNKM_CONTRACT_START;

	private float m_fAniEndTime = 2f;

	[Header("Sound-FX")]
	public GameObject m_FX_SOUND_FX_UI_RECRUITMNET_INTRO_01;

	public GameObject m_FX_SOUND_FX_UI_RECRUITMNET_INTRO_02;

	private eSequence m_CurStatus = eSequence.NONE;

	private int m_iTargetGrade;

	private int m_iVfxGrade;

	private TrackEntry TrackEntry1;

	private TrackEntry TrackEntry2;

	private TrackEntry TrackEntry3;

	private TrackEntry TrackEntry5;

	private TrackEntry TrackEntry7;

	private float m_fStartVal;

	private float m_fEndVal;

	public float m_fMoveSpeed = 5f;

	public float m_fCloseTime = 2f;

	private const int SR_TRANSITION_PHASE = 3;

	private const int SSR_TRANSITION_PHASE = 5;

	private const int HALF_TRANSITION_PHASE = 5;

	private const int FORCE_TRANSITION_PHASE = 6;

	[Header("화살표")]
	public GameObject m_objDrag_Induce;

	private UnityAction dClose;

	private List<int> m_lstCurPlaySoundID = new List<int>();

	private NKM_UNIT_GRADE m_eSelectGrade;

	private bool m_bAwaken;

	private string m_strFirstHeliAniName = "";

	private string m_strSecondHeliAniName = "";

	private const float closeOffset = 0.033f;

	private const float openOffset = 0.026f;

	private bool m_bPlaySoundLand2;

	private bool m_bPlaySoundLoop2;

	private bool m_bSkip;

	private float m_fOffset;

	public float m_fMoveRate = 1f;

	private float m_fMoveY;

	private bool bOpenGate;

	private bool bNormalSoundPlay;

	private bool bSRSoundPlay;

	private bool bSSRSoundPlay;

	private bool bGradeRePlay;

	[Header("채용 연출 설정")]
	[Space]
	[Header("헬기유도 fade out 시작 지점(%)")]
	[Range(0f, 1f)]
	public float m_fFadeOutPercent = 0.8f;

	[Header("헬기도착씬 등장 타이밍(alpha기준)")]
	[Range(0f, 1f)]
	public float m_fActiveValue = 0.5f;

	[Header("fade out 종료 값(100~x)")]
	[Range(0f, 1f)]
	public float m_fFadeOutStartValue = 1f;

	[Range(0f, 1f)]
	public float m_fFadeOutEndValue;

	[Range(0f, 1f)]
	public float m_fFadeOutTime = 0.2f;

	public float m_fUpdateTime = 0.02f;

	private int m_iVibrateCnt;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "채용 연출";

	public static NKCUIContractSequence Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIContractSequence>("AB_UI_NKM_UI_CONTRACT_V2", "NKM_UI_CONTRACT_V2_SEQUENCE", NKCUIManager.eUIBaseRect.UIOverlay, CleanupInstance).GetInstance<NKCUIContractSequence>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		StopCurPlaySound();
		Object.Destroy(m_Instance.gameObject);
		m_Instance = null;
	}

	public override void OnBackButton()
	{
		if (!m_bSkip)
		{
			OnSkip();
			return;
		}
		StopCurPlaySound();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_NKM_CONTRACT_HELI_FX1.ClearState();
		m_NKM_CONTRACT_HELI_FX2.ClearState();
		m_NKM_CONTRACT_HELI_FX3.ClearState();
		m_NKM_CONTRACT_HELI_FX4.ClearState();
		m_NKM_CONTRACT_HELI_FX5.ClearState();
		m_NKM_CONTRACT_HELI_FX6.ClearState();
		m_NKM_CONTRACT_HELI_FX7.ClearState();
		TrackEntry1 = null;
		TrackEntry2 = null;
		TrackEntry3 = null;
		TrackEntry5 = null;
		TrackEntry7 = null;
		m_iTargetGrade = 0;
		base.OnBackButton();
		if (dClose != null)
		{
			dClose();
		}
	}

	private void Init()
	{
		if (NKCCamera.GetCamera().aspect < 1.777f)
		{
			NKCCamera.RescaleRectToCameraFrustrum(m_rtBackground, NKCCamera.GetCamera(), Vector2.zero, 1200f, NKCCamera.FitMode.FitToScreen);
		}
	}

	public void Open(NKM_UNIT_GRADE targetGrade, bool bAwaken, UnityAction close = null)
	{
		Clear();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		dClose = close;
		m_eSelectGrade = targetGrade;
		m_bAwaken = bAwaken;
		NKCUtil.SetGameobjectActive(m_FX_SOUND_FX_UI_RECRUITMNET_INTRO_01, bValue: false);
		NKCUtil.SetGameobjectActive(m_FX_SOUND_FX_UI_RECRUITMNET_INTRO_02, bValue: false);
		UpdateState(eSequence.START);
		UIOpened();
		m_iVibrateCnt = 0;
	}

	private void SetSSRHeliAni()
	{
		int num = RandomGenerator.Range(1, 100);
		if (1 == num)
		{
			SetHeliAni(RECEPTION_MEMBER.HANA);
		}
		else if (5 >= num)
		{
			SetHeliAni(RECEPTION_MEMBER.HANA_SUYEON);
		}
		else
		{
			SetHeliAni(RECEPTION_MEMBER.GAB_HANA_SUYEON);
		}
	}

	private void SetHeliAni(RECEPTION_MEMBER _type)
	{
		switch (_type)
		{
		case RECEPTION_MEMBER.GAB_HANA_SUYEON:
			m_strFirstHeliAniName = "BASE";
			m_strSecondHeliAniName = "BASE2";
			break;
		case RECEPTION_MEMBER.HANA_SUYEON:
			m_strFirstHeliAniName = "BASE3";
			m_strSecondHeliAniName = "BASE4";
			break;
		default:
			m_strFirstHeliAniName = "BASE5";
			m_strSecondHeliAniName = "BASE6";
			break;
		}
	}

	private void UpdateState(eSequence type)
	{
		if (m_CurStatus == type)
		{
			return;
		}
		m_CurStatus = type;
		switch (m_CurStatus)
		{
		case eSequence.START:
			m_lstCurPlaySoundID.Clear();
			NKCUtil.SetGameobjectActive(m_VFX, bValue: false);
			NKCUtil.SetGameobjectActive(EnhanceSR, bValue: false);
			NKCUtil.SetGameobjectActive(EnhanceSSR, bValue: false);
			NKCUtil.SetGameobjectActive(EnhanceAWAKEN, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_CONTRACT_ARRIVAL, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_CONTRACT_START, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_CONTRACT_HELI, bValue: false);
			if (m_bAwaken)
			{
				SetSSRHeliAni();
				m_iTargetGrade = 3;
				m_NKM_CONTRACT_HELI_Animator.SetInteger("TargetGrade", 3);
				m_NKM_CONTRACT_HELI_Animator.SetBool("DecideAwaken", value: true);
			}
			else
			{
				switch (m_eSelectGrade)
				{
				case NKM_UNIT_GRADE.NUG_N:
					m_iTargetGrade = 0;
					SetHeliAni(RECEPTION_MEMBER.HANA);
					break;
				case NKM_UNIT_GRADE.NUG_R:
					m_iTargetGrade = 0;
					SetHeliAni(RECEPTION_MEMBER.HANA_SUYEON);
					break;
				case NKM_UNIT_GRADE.NUG_SR:
					m_iTargetGrade = 1;
					if (5 >= RandomGenerator.Range(1, 100))
					{
						SetHeliAni(RECEPTION_MEMBER.HANA);
					}
					else
					{
						SetHeliAni(RECEPTION_MEMBER.HANA_SUYEON);
					}
					break;
				case NKM_UNIT_GRADE.NUG_SSR:
					m_iTargetGrade = 2;
					SetSSRHeliAni();
					break;
				}
				m_NKM_CONTRACT_HELI_Animator.SetInteger("TargetGrade", (int)m_eSelectGrade);
			}
			InitializeContractRandomPhase(m_iTargetGrade);
			m_NKM_CONTRACT_HELI_Animator.enabled = false;
			break;
		case eSequence.WAIT:
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_CONTRACT_ARRIVAL, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_CONTRACT_START, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_CONTRACT_HELI, bValue: false);
			m_lstCurPlaySoundID.Add(NKCSoundManager.PlaySound("FX_UI_RECRUITMNET_HELICOPTER_START", 1f, 0f, 0f));
			m_NKM_CONTRACT_START.ClearState();
			float fDelay = m_NKM_CONTRACT_START.AnimationState.SetAnimation(0, "BASE", loop: false).AnimationEnd * m_fFadeOutPercent;
			StartCoroutine(OnFadeOut(fDelay));
			m_CurStatus = eSequence.SCENE_1;
			break;
		}
		case eSequence.SCENE_2:
			NKCUtil.SetGameobjectActive(m_NKM_UI_CONTRACT_HELI, bValue: true);
			m_bPlaySoundLand2 = false;
			m_bPlaySoundLoop2 = false;
			m_NKM_CONTRACT_HELI_FX1.ClearState();
			TrackEntry1 = m_NKM_CONTRACT_HELI_FX1.AnimationState.SetAnimation(0, "BASE", loop: false);
			m_NKM_CONTRACT_HELI_FX1.AnimationState.Complete += SetNextAni;
			NKCUtil.SetGameobjectActive(m_NKM_CONTRACT_HELI_FX2.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_CONTRACT_HELI_FX3.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_CONTRACT_HELI_FX7.gameObject, bValue: false);
			m_NKM_CONTRACT_HELI_FX4.ClearState();
			m_NKM_CONTRACT_HELI_FX4.AnimationState.SetAnimation(0, m_strFirstHeliAniName, loop: false);
			m_NKM_CONTRACT_HELI_FX4.AnimationState.AddAnimation(0, m_strSecondHeliAniName, loop: true, 0f);
			m_NKM_CONTRACT_HELI_FX5.ClearState();
			m_NKM_CONTRACT_HELI_FX5.AnimationState.SetAnimation(0, "BASE", loop: false);
			m_NKM_CONTRACT_HELI_FX6.ClearState();
			m_NKM_CONTRACT_HELI_FX6.AnimationState.SetAnimation(0, "BASE", loop: false);
			m_NKM_CONTRACT_HELI_FX6.AnimationState.AddAnimation(0, "BASE2", loop: true, 0f);
			m_lstCurPlaySoundID.Add(NKCSoundManager.PlaySound("FX_UI_RECRUITMNET_HELICOPTER_LAND", 1f, 0f, 0f));
			m_lstCurPlaySoundID.Add(NKCSoundManager.PlaySound("FX_UI_RECRUITMNET_HELICOPTER_LOOP", 1f, 0f, 0f));
			NKCUtil.SetGameobjectActive(m_VFX, bValue: false);
			break;
		case eSequence.ENABLE:
			m_bSkip = true;
			m_NKM_CONTRACT_HELI_FX1.AnimationState.Complete -= SetNextAni;
			m_NKM_CONTRACT_HELI_FX1.AnimationState.ClearTracks();
			m_NKM_CONTRACT_HELI_FX5.AnimationState.ClearTracks();
			NKCUtil.SetGameobjectActive(m_NKM_CONTRACT_HELI_FX2.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_CONTRACT_HELI_FX3.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_CONTRACT_HELI_FX7.gameObject, bValue: true);
			m_NKM_CONTRACT_HELI_Animator.enabled = true;
			m_NKM_CONTRACT_HELI_FX1.AnimationState.SetAnimation(0, "BASE2", loop: true);
			TrackEntry2 = m_NKM_CONTRACT_HELI_FX2.AnimationState.SetAnimation(0, "BASE2", loop: false);
			m_NKM_CONTRACT_HELI_FX2.Update(0.001f);
			TrackEntry3 = m_NKM_CONTRACT_HELI_FX3.AnimationState.SetAnimation(0, "BASE2", loop: false);
			m_NKM_CONTRACT_HELI_FX3.Update(0.001f);
			TrackEntry5 = m_NKM_CONTRACT_HELI_FX5.AnimationState.SetAnimation(0, "BASE2", loop: false);
			TrackEntry7 = m_NKM_CONTRACT_HELI_FX7.AnimationState.SetAnimation(0, "BASE2", loop: false);
			m_NKM_CONTRACT_HELI_FX7.Update(0.001f);
			m_NKM_CONTRACT_HELI_FX4.AnimationState.SetAnimation(0, m_strSecondHeliAniName, loop: true);
			m_NKM_CONTRACT_HELI_FX6.AnimationState.SetAnimation(0, "BASE2", loop: true);
			m_fAniEndTime = TrackEntry2.AnimationEnd;
			m_NKM_CONTRACT_HELI_FX2.AnimationState.TimeScale = 0f;
			m_NKM_CONTRACT_HELI_FX3.AnimationState.TimeScale = 0f;
			m_NKM_CONTRACT_HELI_FX5.AnimationState.TimeScale = 0f;
			m_NKM_CONTRACT_HELI_FX7.AnimationState.TimeScale = 0f;
			NKCUtil.SetGameobjectActive(m_VFX, bValue: true);
			NKCUtil.SetGameobjectActive(m_objDrag_Induce, bValue: true);
			break;
		case eSequence.SCENE_1:
		case eSequence.DRAG:
		case eSequence.AUTO_CLOSE:
		case eSequence.AUTO_OPEN:
			break;
		}
	}

	private void Clear()
	{
		for (int i = 0; i < m_mrNKM_CONTRACT_START.materials.Length; i++)
		{
			Color color = m_mrNKM_CONTRACT_START.materials[i].color;
			color.a = 1f;
			m_mrNKM_CONTRACT_START.materials[i].SetColor("_Color", color);
		}
		m_CurStatus = eSequence.NONE;
		m_fStartVal = 0f;
		m_fEndVal = 0f;
		m_NKM_CONTRACT_HELI_Animator.enabled = true;
		m_NKM_CONTRACT_HELI_Animator.Update(0.001f);
		m_NKM_CONTRACT_HELI_Animator.enabled = false;
		m_iTargetGrade = 0;
		m_iVfxGrade = 0;
		bNormalSoundPlay = false;
		bSRSoundPlay = false;
		bSSRSoundPlay = false;
		bGradeRePlay = false;
	}

	public void SetNextAni(TrackEntry trackEntry)
	{
		UpdateState(eSequence.ENABLE);
	}

	private void Update()
	{
		if (m_CurStatus == eSequence.START && !AnimatorIsPlaying(m_NUM_CONTRACT_NEWMEMBER_ARRIVAL))
		{
			UpdateState(eSequence.WAIT);
		}
		PlayHeliLandingSound();
		if (m_CurStatus == eSequence.AUTO_CLOSE)
		{
			if (m_fStartVal <= 0.033f)
			{
				NKCSoundManager.PlaySound("FX_UI_RECRUITMNET_HELICOPTER_CLOSE", 1f, 0f, 0f);
				m_CurStatus = eSequence.ENABLE;
			}
			EnableAutomaticDoor();
		}
		else if (m_CurStatus == eSequence.AUTO_OPEN)
		{
			NKCUtil.SetGameobjectActive(m_objDrag_Induce, bValue: false);
			if (m_fStartVal >= m_fAniEndTime - 0.026f)
			{
				StartCoroutine(CallOnBackButton(m_fCloseTime));
				m_CurStatus = eSequence.NONE;
			}
			EnableAutomaticDoor();
		}
	}

	private IEnumerator CallOnBackButton(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		OnBackButton();
	}

	private bool AnimatorIsPlaying(Animator ani)
	{
		return 1f > ani.GetCurrentAnimatorStateInfo(0).normalizedTime;
	}

	private void PlayHeliLandingSound()
	{
		if (m_CurStatus == eSequence.SCENE_2 && TrackEntry1 != null)
		{
			if (!m_bPlaySoundLand2 && TrackEntry1.AnimationTime * 30f >= 105f)
			{
				m_lstCurPlaySoundID.Add(NKCSoundManager.PlaySound("FX_UI_RECRUITMNET_HELICOPTER_LAND_02", 1f, 0f, 0f));
				m_bPlaySoundLand2 = true;
			}
			else if (!m_bPlaySoundLoop2 && TrackEntry1.AnimationTime * 30f >= 110f)
			{
				m_lstCurPlaySoundID.Add(NKCSoundManager.PlaySound("FX_UI_RECRUITMNET_HELICOPTER_LOOP_02", 1f, 0f, 0f, bLoop: true));
				m_bPlaySoundLoop2 = true;
			}
		}
	}

	public void OnInitializePotentialDrag(PointerEventData eventData)
	{
		eventData.useDragThreshold = false;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!m_bSkip && m_CurStatus >= eSequence.WAIT)
		{
			OnSkip();
		}
		m_fOffset = 0f;
		m_fMoveY = 0f;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!m_bSkip && m_CurStatus >= eSequence.WAIT)
		{
			OnSkip();
		}
		m_fOffset = 0f;
		m_fMoveY = 0f;
	}

	private void OnSkip()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_CONTRACT_ARRIVAL, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_CONTRACT_START, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_CONTRACT_HELI, bValue: true);
		StopAllCoroutines();
		StopCurPlaySound();
		m_lstCurPlaySoundID.Add(NKCSoundManager.PlaySound("FX_UI_RECRUITMNET_HELICOPTER_LOOP_02", 1f, 0f, 0f, bLoop: true));
		SetNextAni(null);
		m_bSkip = true;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (m_CurStatus != eSequence.ENABLE && m_CurStatus != eSequence.DRAG)
		{
			return;
		}
		Vector2 lhs = eventData.delta * m_fMoveRate;
		m_fOffset += Vector2.Dot(lhs, Vector2.up);
		m_fMoveY = m_fOffset * -0.003f;
		MoveTrack(TrackEntry2, m_fMoveY);
		MoveTrack(TrackEntry3, m_fMoveY);
		MoveTrack(TrackEntry5, m_fMoveY);
		MoveTrack(TrackEntry7, m_fMoveY);
		m_NKM_CONTRACT_HELI_Animator.SetFloat("Length", Mathf.Clamp(m_fMoveY, 0f, m_fAniEndTime));
		int integer = m_NKM_CONTRACT_HELI_Animator.GetInteger("Phase");
		if (!bOpenGate && integer >= 1)
		{
			NKCSoundManager.PlaySound("FX_UI_RECRUITMNET_HELICOPTER_OPEN", 1f, 0f, 0f);
			bOpenGate = true;
		}
		else if (bOpenGate && integer < 1)
		{
			NKCSoundManager.PlaySound("FX_UI_RECRUITMNET_HELICOPTER_CLOSE", 1f, 0f, 0f);
			bOpenGate = false;
		}
		if (m_iTargetGrade > 0 && m_iVfxGrade < 2)
		{
			if (integer >= 3 && !m_NKM_CONTRACT_HELI_Animator.GetBool("DecideSR"))
			{
				m_iVfxGrade = Mathf.Clamp(1, 0, m_iTargetGrade);
				m_NKM_CONTRACT_HELI_Animator.SetInteger("Grade", m_iVfxGrade);
			}
			else if (integer >= 5 && !m_NKM_CONTRACT_HELI_Animator.GetBool("DecideSSR"))
			{
				m_iVfxGrade = Mathf.Clamp(2, 0, m_iTargetGrade);
				m_NKM_CONTRACT_HELI_Animator.SetInteger("Grade", m_iVfxGrade);
			}
			if (!bSRSoundPlay && integer < 3 && m_NKM_CONTRACT_HELI_Animator.GetBool("DecideSR"))
			{
				bSRSoundPlay = true;
			}
			if (!bSSRSoundPlay && integer < 3 && m_NKM_CONTRACT_HELI_Animator.GetBool("DecideSR"))
			{
				bSSRSoundPlay = true;
			}
			if (integer >= 5 && (bSRSoundPlay || bSSRSoundPlay) && !bGradeRePlay)
			{
				if (bSRSoundPlay)
				{
					NKCSoundManager.PlaySound("FX_UI_RECRUITMNET_GET_02", 1f, 0f, 0f);
				}
				else
				{
					NKCSoundManager.PlaySound("FX_UI_RECRUITMNET_GET_03", 1f, 0f, 0f);
				}
				bGradeRePlay = true;
			}
		}
		else if (integer >= 5 && !bNormalSoundPlay)
		{
			bNormalSoundPlay = true;
			NKCSoundManager.PlaySound("FX_UI_RECRUITMNET_GET", 1f, 0f, 0f);
		}
		if (integer >= 6 || m_fMoveY >= 0.8f)
		{
			m_CurStatus = eSequence.AUTO_OPEN;
			m_fStartVal = m_fMoveY;
			m_fEndVal = m_fAniEndTime;
		}
		else
		{
			m_CurStatus = eSequence.DRAG;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (m_CurStatus == eSequence.DRAG)
		{
			if (m_NKM_CONTRACT_HELI_Animator.GetInteger("Phase") < 5)
			{
				m_CurStatus = eSequence.AUTO_CLOSE;
				m_fStartVal = m_fMoveY;
				m_fEndVal = 0f;
			}
			else
			{
				m_CurStatus = eSequence.AUTO_OPEN;
				m_fStartVal = m_fMoveY;
				m_fEndVal = m_fAniEndTime;
			}
			m_fOffset = 0f;
			m_fMoveY = 0f;
		}
	}

	private void EnableAutomaticDoor()
	{
		m_fStartVal = Mathf.Lerp(m_fStartVal, m_fEndVal, Time.deltaTime * m_fMoveSpeed);
		MoveTrack(TrackEntry2, m_fStartVal);
		MoveTrack(TrackEntry3, m_fStartVal);
		MoveTrack(TrackEntry5, m_fStartVal);
		MoveTrack(TrackEntry7, m_fStartVal);
		m_NKM_CONTRACT_HELI_Animator.SetFloat("Length", Mathf.Clamp(m_fStartVal, 0f, m_fAniEndTime));
	}

	private void MoveTrack(TrackEntry target, float time)
	{
		if (target != null)
		{
			if (time <= 0f)
			{
				time = 0f;
			}
			if (time >= target.AnimationEnd)
			{
				time = target.AnimationEnd;
			}
			target.TrackTime = time;
		}
	}

	private void InitializeContractRandomPhase(int grade)
	{
		float value = Random.value;
		if (value < 0.333f)
		{
			m_iVfxGrade = Mathf.Clamp(2, 0, grade);
		}
		else if (value < 0.666f)
		{
			m_iVfxGrade = Mathf.Clamp(1, 0, grade);
		}
		else
		{
			m_iVfxGrade = 0;
		}
		m_NKM_CONTRACT_HELI_Animator.SetInteger("Grade", m_iVfxGrade);
		switch (m_iVfxGrade)
		{
		case 0:
			m_NKM_CONTRACT_HELI_Animator.SetBool("DecideSR", value: false);
			m_NKM_CONTRACT_HELI_Animator.SetBool("DecideSSR", value: false);
			break;
		case 1:
			m_NKM_CONTRACT_HELI_Animator.SetBool("DecideSR", value: true);
			m_NKM_CONTRACT_HELI_Animator.SetBool("DecideSSR", value: false);
			break;
		case 2:
			m_NKM_CONTRACT_HELI_Animator.SetBool("DecideSR", value: true);
			m_NKM_CONTRACT_HELI_Animator.SetBool("DecideSSR", value: true);
			break;
		}
	}

	public void EventEnhanceGrade(int grade)
	{
		if (grade > 2)
		{
			NKCUtil.SetGameobjectActive(EnhanceAWAKEN, bValue: true);
		}
		else if (grade > 1)
		{
			NKCUtil.SetGameobjectActive(EnhanceSSR, bValue: true);
		}
		else if (grade > 0)
		{
			NKCUtil.SetGameobjectActive(EnhanceSR, bValue: true);
		}
	}

	private void StopCurPlaySound()
	{
		for (int i = 0; i < m_lstCurPlaySoundID.Count; i++)
		{
			NKCSoundManager.StopSound(m_lstCurPlaySoundID[i]);
		}
		m_lstCurPlaySoundID.Clear();
	}

	public void EndAnimation()
	{
		m_NKM_CONTRACT_HELI_Animator.SetBool("End", value: true);
	}

	private IEnumerator OnFadeOut(float fDelay)
	{
		yield return new WaitForSeconds(fDelay);
		float num = m_fFadeOutTime / m_fUpdateTime;
		float fAlphaVal = m_fFadeOutStartValue;
		float fAddVal = (m_fFadeOutStartValue - m_fFadeOutEndValue) / num;
		bool bFadeIn = false;
		while (fAlphaVal > m_fFadeOutEndValue)
		{
			fAlphaVal -= fAddVal;
			if (!bFadeIn && fAlphaVal < m_fActiveValue)
			{
				UpdateState(eSequence.SCENE_2);
				bFadeIn = true;
			}
			for (int i = 0; i < m_mrNKM_CONTRACT_START.materials.Length; i++)
			{
				Color color = m_mrNKM_CONTRACT_START.materials[i].color;
				color.a = Mathf.Clamp(fAlphaVal, 0f, 1f);
				m_mrNKM_CONTRACT_START.materials[i].SetColor("_Color", color);
			}
			yield return new WaitForSeconds(m_fUpdateTime);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_CONTRACT_START, bValue: false);
		yield return null;
	}

	public void OnContractVibrate()
	{
		if (m_iVibrateCnt <= 0)
		{
			m_iVibrateCnt++;
		}
	}
}
