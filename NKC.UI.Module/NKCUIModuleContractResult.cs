using System;
using System.Collections.Generic;
using Cs.Math;
using DG.Tweening;
using NKM;
using NKM.Templet;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace NKC.UI.Module;

public class NKCUIModuleContractResult : NKCUIBase, IInitializePotentialDragHandler, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
	public enum GACHA_SEQUENCE
	{
		NONE,
		BASE,
		IDLE,
		BASE2,
		DRAG,
		AUTO_PLAY,
		AUTO_BACK,
		RESULT1,
		RESULT2,
		RESULT3,
		RESULT4,
		RESULT5
	}

	[Serializable]
	public struct ShakeOption
	{
		public Transform trTarget;

		public float startValue;

		public float endValue;

		public Vector3 startStrength;

		public Vector3 endStrength;

		public int startVibrato;

		public int EndVibrato;

		public float startRandomness;

		public float endRandomness;
	}

	[Serializable]
	public struct EventSoundData
	{
		public string eventKey;

		public string audioClipName;
	}

	public enum DRAG_TYPE
	{
		DRAG_DOWN,
		DRAG_CIRCLE,
		DRAG_RIGHT
	}

	public SkeletonGraphic m_SkeletonGraphicBackGround;

	public SkeletonGraphic m_SkeletonGraphicMain;

	private Vector3 m_ShakeObjectOriPos = Vector3.zero;

	public NKCUIComStateButton m_csbtnSkip;

	[Header("\ufffd\ufffd\ufffd\ufffd")]
	public float m_fAutoMoveSpeed = 3f;

	[Header("Shake Option")]
	public bool m_bShakeCharacter = true;

	public ShakeOption m_ObjectShake;

	[Header("\ufffd\ufffdġ \ufffd\u05b4ϸ\ufffd\ufffd\u033c\ufffd")]
	public bool m_bTouchAni;

	public List<string> m_lstTouchAni;

	public float m_fTouchCheckDistance = 0.1f;

	[Header("Idle \ufffd\u05b4ϸ\ufffd\ufffd\u033c\ufffd")]
	public bool m_bRandomIdleAni;

	public List<string> m_lstIdleAnimationName;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public string m_strBGMClipName;

	public List<EventSoundData> m_lstEventSound;

	private bool m_bInit;

	private UnityAction dClose;

	private NKM_UNIT_GRADE m_targetGrade;

	private bool m_bAwake;

	private GACHA_SEQUENCE m_curState;

	private TrackEntry TrackEntry1;

	private TrackEntry TrackEntry2;

	private bool m_bPlayResult;

	private float m_fResultAniEndTime;

	private float m_fAniEndTime;

	private float m_fAutoMoveOffSet = 0.03f;

	private Vector2 m_vecTouchStartPosition = Vector2.zero;

	private float m_fCurMoveValue;

	[Header("\ufffd巡\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public float m_targetMoveRate = 0.25f;

	public float m_fMoveRate = 1f;

	public float m_fOffset = 0.003f;

	private float m_fMoveValue;

	private bool m_bDragging;

	[Header("\ufffd\ufffdƮ\ufffd\ufffd")]
	public DRAG_TYPE m_ControlType = DRAG_TYPE.DRAG_CIRCLE;

	public RectTransform m_rtCenter;

	private float m_fAngleLastFrame;

	private float m_fCurrentRotation;

	public float m_fMaxAngle = 360f;

	private float m_fStartVal;

	private float m_fEndVal;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "NKCUIModuleResult \ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd";

	public static NKCUIModuleContractResult MakeInstance(string bundleName, string assetName)
	{
		return NKCUIManager.OpenNewInstance<NKCUIModuleContractResult>(bundleName, assetName, NKCUIManager.eUIBaseRect.UIOverlay, null).GetInstance<NKCUIModuleContractResult>();
	}

	public override void CloseInternal()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public override void OnBackButton()
	{
		base.OnBackButton();
		if (!string.IsNullOrEmpty(m_strBGMClipName))
		{
			NKCUIModuleHome.PlayBGMMusic();
		}
		dClose?.Invoke();
	}

	public void Open(List<NKMUnitData> lstUnits, UnityAction close = null)
	{
		if (lstUnits.Count == 0)
		{
			return;
		}
		if (m_ObjectShake.trTarget != null)
		{
			m_ShakeObjectOriPos = m_ObjectShake.trTarget.transform.position;
		}
		NKCUtil.SetBindFunction(m_csbtnSkip, OnSkip);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		dClose = close;
		m_targetGrade = NKM_UNIT_GRADE.NUG_N;
		m_bAwake = false;
		m_curState = GACHA_SEQUENCE.NONE;
		m_bPlayResult = false;
		foreach (NKMUnitData lstUnit in lstUnits)
		{
			if (m_targetGrade < lstUnit.GetUnitGrade())
			{
				m_targetGrade = lstUnit.GetUnitGrade();
			}
			if (lstUnit.GetUnitGrade() == NKM_UNIT_GRADE.NUG_SSR)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(lstUnit);
				if (unitTempletBase != null && !m_bAwake)
				{
					m_bAwake = unitTempletBase.m_bAwaken;
				}
			}
		}
		if (m_SkeletonGraphicMain == null)
		{
			OnBackButton();
		}
		else
		{
			UpdateAniState(GACHA_SEQUENCE.BASE);
		}
		if (!string.IsNullOrEmpty(m_strBGMClipName))
		{
			NKCSoundManager.PlayMusic(m_strBGMClipName, bLoop: true);
		}
		if (!m_bInit)
		{
			m_SkeletonGraphicMain.AnimationState.Complete += SetNextAni;
			m_SkeletonGraphicMain.AnimationState.Event += HandleEvent;
			m_bInit = true;
		}
		UIOpened();
	}

	private void UpdateAniState(GACHA_SEQUENCE newState)
	{
		if (m_curState != newState)
		{
			m_curState = newState;
			m_SkeletonGraphicBackGround?.AnimationState.ClearTrack(0);
			m_SkeletonGraphicMain.AnimationState.ClearTrack(0);
			switch (m_curState)
			{
			case GACHA_SEQUENCE.BASE:
				TrackEntry1 = m_SkeletonGraphicBackGround?.AnimationState.SetAnimation(0, m_curState.ToString(), loop: false);
				TrackEntry2 = m_SkeletonGraphicMain.AnimationState.SetAnimation(0, m_curState.ToString(), loop: false);
				break;
			case GACHA_SEQUENCE.IDLE:
			case GACHA_SEQUENCE.DRAG:
			{
				TrackEntry1 = m_SkeletonGraphicBackGround?.AnimationState.SetAnimation(0, GACHA_SEQUENCE.IDLE.ToString(), loop: true);
				string idleAnimationName = GetIdleAnimationName();
				TrackEntry2 = m_SkeletonGraphicMain.AnimationState.SetAnimation(0, idleAnimationName, loop: true);
				break;
			}
			case GACHA_SEQUENCE.RESULT1:
			case GACHA_SEQUENCE.RESULT2:
			case GACHA_SEQUENCE.RESULT3:
			case GACHA_SEQUENCE.RESULT4:
			case GACHA_SEQUENCE.RESULT5:
				TrackEntry1 = m_SkeletonGraphicBackGround?.AnimationState.SetAnimation(0, m_curState.ToString(), loop: false);
				TrackEntry2 = m_SkeletonGraphicMain.AnimationState.SetAnimation(0, m_curState.ToString(), loop: false);
				m_fResultAniEndTime = m_SkeletonGraphicMain.AnimationState.GetCurrent(0).AnimationEnd;
				m_bPlayResult = true;
				break;
			}
			NKCUtil.SetGameobjectActive(m_csbtnSkip, m_curState < GACHA_SEQUENCE.RESULT1);
		}
	}

	private void SetNextAni(TrackEntry trackEntry)
	{
		switch (m_curState)
		{
		case GACHA_SEQUENCE.BASE:
			UpdateAniState(GACHA_SEQUENCE.IDLE);
			break;
		case GACHA_SEQUENCE.IDLE:
			UpdateAniState(GACHA_SEQUENCE.DRAG);
			break;
		case GACHA_SEQUENCE.BASE2:
		case GACHA_SEQUENCE.DRAG:
		case GACHA_SEQUENCE.AUTO_PLAY:
		case GACHA_SEQUENCE.AUTO_BACK:
			if (m_lstTouchAni.Contains(trackEntry.Animation.Name))
			{
				UpdateAniState(GACHA_SEQUENCE.IDLE);
			}
			break;
		case GACHA_SEQUENCE.RESULT1:
		case GACHA_SEQUENCE.RESULT2:
		case GACHA_SEQUENCE.RESULT3:
		case GACHA_SEQUENCE.RESULT4:
		case GACHA_SEQUENCE.RESULT5:
			OnBackButton();
			break;
		}
	}

	private void HandleEvent(TrackEntry trackEntry, Spine.Event e)
	{
		foreach (EventSoundData item in m_lstEventSound)
		{
			if (string.Equals(item.eventKey, e.String))
			{
				NKCSoundManager.PlaySound(item.audioClipName, 1f, 0f, 0f);
			}
		}
	}

	private void Update()
	{
		if (m_curState == GACHA_SEQUENCE.AUTO_BACK)
		{
			m_fStartVal = Mathf.Lerp(m_fStartVal, m_fEndVal, Time.deltaTime * m_fAutoMoveSpeed);
			MoveTrack(TrackEntry2, m_fStartVal);
			if (m_fStartVal <= m_fAutoMoveOffSet)
			{
				UpdateAniState(GACHA_SEQUENCE.DRAG);
			}
		}
		else if (m_curState == GACHA_SEQUENCE.AUTO_PLAY)
		{
			m_fStartVal = Mathf.Lerp(m_fStartVal, m_fEndVal, Time.deltaTime * m_fAutoMoveSpeed);
			MoveTrack(TrackEntry2, m_fStartVal);
			if (m_fStartVal >= m_fAniEndTime - m_fAutoMoveOffSet)
			{
				SetCachaResult();
			}
		}
		else if (m_curState == GACHA_SEQUENCE.DRAG && m_bDragging && m_bShakeCharacter && m_ObjectShake.trTarget != null)
		{
			float num = Mathf.Min(1f, m_fMoveValue / m_fAniEndTime);
			if (m_ObjectShake.startValue < num && m_ObjectShake.endValue >= num)
			{
				float num2 = m_ObjectShake.endValue - m_ObjectShake.startValue;
				float num3 = (float)Math.Round((num - m_ObjectShake.startValue) * 100f / num2, 3) * 0.01f;
				int vibrato = (int)((float)m_ObjectShake.startVibrato + (float)(m_ObjectShake.EndVibrato - m_ObjectShake.startVibrato) * num3);
				float randomness = m_ObjectShake.startRandomness + (m_ObjectShake.endRandomness - m_ObjectShake.startRandomness) * num3;
				Vector3 strength = m_ObjectShake.startStrength + (m_ObjectShake.endStrength - m_ObjectShake.startStrength) * num3;
				m_ObjectShake.trTarget.DOShakePosition(1f, strength, vibrato, randomness);
			}
		}
		if (m_bPlayResult)
		{
			float trackTime = m_SkeletonGraphicMain.AnimationState.GetCurrent(0).TrackTime;
			if (m_fResultAniEndTime <= trackTime - 0.2f)
			{
				OnBackButton();
			}
		}
	}

	private void SetCachaResult()
	{
		switch (m_targetGrade)
		{
		default:
			UpdateAniState(GACHA_SEQUENCE.RESULT1);
			break;
		case NKM_UNIT_GRADE.NUG_SR:
			UpdateAniState(GACHA_SEQUENCE.RESULT2);
			break;
		case NKM_UNIT_GRADE.NUG_SSR:
			if (m_bAwake)
			{
				if (RandomGenerator.Range(1, 10) <= 5)
				{
					UpdateAniState(GACHA_SEQUENCE.RESULT4);
				}
				else
				{
					UpdateAniState(GACHA_SEQUENCE.RESULT5);
				}
			}
			else
			{
				UpdateAniState(GACHA_SEQUENCE.RESULT3);
			}
			break;
		}
	}

	public void OnInitializePotentialDrag(PointerEventData eventData)
	{
		eventData.useDragThreshold = false;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (m_curState == GACHA_SEQUENCE.BASE || m_curState == GACHA_SEQUENCE.IDLE)
		{
			UpdateAniState(GACHA_SEQUENCE.DRAG);
		}
		m_vecTouchStartPosition = eventData.position;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (m_curState <= GACHA_SEQUENCE.DRAG && !m_bPlayResult && m_bTouchAni && m_lstTouchAni.Count > 0 && Vector2.Distance(m_vecTouchStartPosition, eventData.position) <= m_fTouchCheckDistance)
		{
			m_SkeletonGraphicMain.AnimationState.ClearTrack(0);
			int index = UnityEngine.Random.Range(0, m_lstTouchAni.Count);
			m_SkeletonGraphicMain.AnimationState.SetAnimation(0, m_lstTouchAni[index].ToString(), loop: false);
		}
	}

	public void OnSkip()
	{
		if (m_curState != GACHA_SEQUENCE.BASE && m_curState != GACHA_SEQUENCE.IDLE)
		{
			OnBackButton();
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (m_curState == GACHA_SEQUENCE.DRAG)
		{
			TrackEntry2 = m_SkeletonGraphicMain.AnimationState.SetAnimation(0, GACHA_SEQUENCE.BASE2.ToString(), loop: false);
			m_fAniEndTime = TrackEntry2.AnimationEnd;
			TrackEntry2.TimeScale = 0f;
			MoveTrack(TrackEntry2, 0f);
			m_fCurMoveValue = 0f;
			m_fMoveValue = 0f;
			m_fCurrentRotation = 0f;
			m_fAngleLastFrame = GetCurrentAngle(eventData.position);
		}
	}

	private float GetCurrentAngle(Vector2 mousePos)
	{
		if (null == m_rtCenter)
		{
			return 0f;
		}
		Vector2 vector = NKCCamera.GetCamera().WorldToScreenPoint(m_rtCenter.position);
		Vector2 vector2 = mousePos - vector;
		Vector3 position = m_rtCenter.position;
		position.x += vector2.x;
		position.y += vector2.y;
		Debug.DrawLine(m_rtCenter.position, position);
		return 0f - Quaternion.FromToRotation(Vector3.up, vector2).eulerAngles.z;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (m_curState != GACHA_SEQUENCE.DRAG)
		{
			return;
		}
		if (m_ControlType == DRAG_TYPE.DRAG_DOWN)
		{
			Vector2 lhs = eventData.delta * m_fMoveRate;
			m_fCurMoveValue += Vector2.Dot(lhs, Vector2.down);
			m_fMoveValue = m_fCurMoveValue * m_fOffset;
		}
		else if (m_ControlType == DRAG_TYPE.DRAG_RIGHT)
		{
			Vector2 lhs2 = eventData.delta * m_fMoveRate;
			m_fCurMoveValue += Vector2.Dot(lhs2, Vector2.right);
			m_fMoveValue = m_fCurMoveValue * m_fOffset;
		}
		else if (m_ControlType == DRAG_TYPE.DRAG_CIRCLE)
		{
			float currentAngle = GetCurrentAngle(eventData.position);
			float num = currentAngle - m_fAngleLastFrame;
			if (num < -180f)
			{
				num += 360f;
			}
			if (num > 180f)
			{
				num -= 360f;
			}
			m_fCurrentRotation += num * m_fOffset;
			m_fCurrentRotation = Mathf.Clamp(m_fCurrentRotation, 0f, m_fMaxAngle);
			m_fMoveValue = m_fCurrentRotation / m_fMaxAngle;
			m_fAngleLastFrame = currentAngle;
			if (m_fMoveValue <= 0f && m_fMoveValue >= 1f)
			{
				return;
			}
		}
		m_bDragging = true;
		MoveTrack(TrackEntry2, m_fMoveValue);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (m_curState == GACHA_SEQUENCE.DRAG)
		{
			m_bDragging = false;
			DOTween.Kill(m_ObjectShake.trTarget);
			m_ObjectShake.trTarget.DOMove(m_ShakeObjectOriPos, 0.1f);
			TrackEntry2.TimeScale = 1f;
			m_fStartVal = m_fMoveValue;
			if (m_fMoveValue > m_fAniEndTime * m_targetMoveRate)
			{
				m_curState = GACHA_SEQUENCE.AUTO_PLAY;
				m_fEndVal = m_fAniEndTime;
			}
			else
			{
				m_curState = GACHA_SEQUENCE.AUTO_BACK;
				m_fEndVal = 0f;
			}
			m_fCurMoveValue = 0f;
			m_fMoveValue = 0f;
		}
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

	private string GetIdleAnimationName()
	{
		if (m_bRandomIdleAni && m_lstIdleAnimationName.Count > 0)
		{
			int index = UnityEngine.Random.Range(0, m_lstIdleAnimationName.Count);
			return m_lstIdleAnimationName[index].ToString();
		}
		return GACHA_SEQUENCE.IDLE.ToString();
	}
}
