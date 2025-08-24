using System;
using System.Collections.Generic;
using Cs.Logging;
using NKM;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace NKC.UI;

public class NKCUIEventSequence : NKCUIBase
{
	[Serializable]
	public struct SequenceEvent
	{
		public float fTime;

		public GameObject objTarget;

		public EventType eEvent;

		public string strArgument;

		public float floatArgument;

		public bool boolArgument;
	}

	public enum EventType
	{
		ObjEnable = 0,
		PlaySpineAnim = 10,
		PlayAnimatorAnim = 11,
		PlaySound = 20,
		PlayMusic = 21,
		PlayVoice = 22,
		Close = 99999
	}

	public delegate void OnClose();

	[Header("메인 Rect와 스케일링 옵션")]
	public RectTransform m_rtMainRect;

	public NKCCamera.FitMode m_eFitMode = NKCCamera.FitMode.FitAuto;

	[Header("이벤트 시퀀스")]
	public List<SequenceEvent> m_lstEvent;

	private OnClose dOnClose;

	private float m_fTimer;

	private float m_fFinishTime;

	private int m_currentEventIndex;

	private bool m_bPlaying;

	private bool m_bSoloUI;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "AnimSequence";

	public bool SoloUI => m_bSoloUI;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public static NKCUIEventSequence OpenInstance(NKMAssetName assetName)
	{
		return NKCUIManager.OpenNewInstance<NKCUIEventSequence>(assetName, NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null).GetInstance<NKCUIEventSequence>();
	}

	public static NKCUIEventSequence OpenInstance(string bundleName, string assetName)
	{
		return NKCUIManager.OpenNewInstance<NKCUIEventSequence>(bundleName, assetName, NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null).GetInstance<NKCUIEventSequence>();
	}

	public static void PlaySkinCutin(NKMSkinTemplet skinTemplet, OnClose dOnClose)
	{
		if (skinTemplet == null || !skinTemplet.HasLoginCutin)
		{
			dOnClose?.Invoke();
			return;
		}
		NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(skinTemplet.m_LoginCutin, skinTemplet.m_LoginCutin);
		NKCUIEventSequence nKCUIEventSequence = OpenInstance(nKMAssetName);
		if (nKCUIEventSequence == null)
		{
			Log.Error($"Skin cutin asset {nKMAssetName} not found!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIEventSequence.cs", 48);
		}
		nKCUIEventSequence.Open(dOnClose);
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public override void Initialize()
	{
	}

	public override void OpenByShortcut(Dictionary<string, string> dicParam)
	{
		Open(null);
	}

	public void Open(OnClose onClose)
	{
		dOnClose = onClose;
		base.gameObject.SetActive(value: true);
		if (m_rtMainRect != null)
		{
			NKCCamera.RescaleRectToCameraFrustrum(m_rtMainRect, NKCCamera.GetSubUICamera(), Vector2.zero, NKCCamera.GetSubUICamera().transform.position.z, m_eFitMode);
		}
		UIOpened();
		StartEvent(bSoloUI: true);
	}

	public void StartEvent(bool bSoloUI = false)
	{
		m_fTimer = 0f;
		m_fFinishTime = 0f;
		m_currentEventIndex = 0;
		m_bPlaying = true;
		m_bSoloUI = bSoloUI;
		m_lstEvent.Sort((SequenceEvent a, SequenceEvent b) => a.fTime.CompareTo(b.fTime));
		m_fFinishTime = m_lstEvent[m_lstEvent.Count - 1].fTime;
		NKCUIVoiceManager.StopVoice();
		ProcessFrame();
	}

	private void ProcessFrame()
	{
		while (m_currentEventIndex < m_lstEvent.Count)
		{
			SequenceEvent sequenceEvent = m_lstEvent[m_currentEventIndex];
			if (!(sequenceEvent.fTime > m_fTimer))
			{
				ProcessEvent(sequenceEvent);
				m_currentEventIndex++;
				continue;
			}
			break;
		}
	}

	private void ProcessEvent(SequenceEvent sequenceEvent)
	{
		switch (sequenceEvent.eEvent)
		{
		case EventType.Close:
			Finish();
			break;
		case EventType.ObjEnable:
			NKCUtil.SetGameobjectActive(sequenceEvent.objTarget, sequenceEvent.boolArgument);
			break;
		case EventType.PlaySpineAnim:
			if (!(sequenceEvent.objTarget == null))
			{
				SkeletonGraphic component = sequenceEvent.objTarget.GetComponent<SkeletonGraphic>();
				if (!(component == null))
				{
					SetAnimation(sequenceEvent.fTime, component, sequenceEvent.strArgument, sequenceEvent.boolArgument);
				}
			}
			break;
		case EventType.PlayAnimatorAnim:
			if (!(sequenceEvent.objTarget == null))
			{
				Animator component2 = sequenceEvent.objTarget.GetComponent<Animator>();
				if (!(component2 == null))
				{
					component2.Play(sequenceEvent.strArgument);
				}
			}
			break;
		case EventType.PlayMusic:
			if (string.IsNullOrEmpty(sequenceEvent.strArgument))
			{
				NKCSoundManager.StopMusic();
			}
			else
			{
				NKCSoundManager.PlayMusic(sequenceEvent.strArgument, bLoop: false, 1f, bForce: false, sequenceEvent.floatArgument);
			}
			break;
		case EventType.PlaySound:
		{
			NKMAssetName nKMAssetName2 = NKMAssetName.ParseBundleName("", sequenceEvent.strArgument);
			if (string.IsNullOrEmpty(nKMAssetName2.m_BundleName))
			{
				NKCSoundManager.PlaySound(sequenceEvent.strArgument, 1f, 0f, 0f);
			}
			else
			{
				NKCSoundManager.PlaySound(nKMAssetName2, 1f, 0f, 0f);
			}
			break;
		}
		case EventType.PlayVoice:
		{
			NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName("", sequenceEvent.strArgument);
			if (string.IsNullOrEmpty(nKMAssetName.m_BundleName))
			{
				NKCSoundManager.PlayVoice(nKMAssetName.m_AssetName, 0, bClearVoice: false, bIgnoreSameVoice: false, 1f, 0f, 0f, bLoop: false, 0f, sequenceEvent.boolArgument);
			}
			else
			{
				NKCUIVoiceManager.ForcePlayVoice(nKMAssetName, 0f, 1f, sequenceEvent.boolArgument);
			}
			break;
		}
		}
	}

	public void SetAnimation(float startTime, SkeletonGraphic skeletonGraphic, string animationName, bool loop, int trackIndex = 0, bool bForceRestart = true, float fStartTime = 0f)
	{
		if (skeletonGraphic == null || skeletonGraphic.AnimationState == null)
		{
			Debug.LogError("AnimSequence : SkeletonGraphic not found!");
		}
		else
		{
			if (skeletonGraphic.SkeletonData == null)
			{
				return;
			}
			Spine.Animation animation = skeletonGraphic.SkeletonData.FindAnimation(animationName);
			if (animation == null)
			{
				Debug.LogError("AnimSequence : Animation " + animationName + " not found!");
				return;
			}
			m_fFinishTime = Mathf.Max(m_fFinishTime, startTime + animation.Duration - fStartTime);
			skeletonGraphic.SetUseHalfUpdate(value: false);
			if (bForceRestart)
			{
				skeletonGraphic.Skeleton?.SetToSetupPose();
				TrackEntry trackEntry = skeletonGraphic.AnimationState.SetAnimation(trackIndex, animationName, loop);
				if (fStartTime > 0f)
				{
					trackEntry.TrackTime = fStartTime;
				}
			}
			skeletonGraphic.Update();
		}
	}

	private void Update()
	{
		if (m_bPlaying)
		{
			m_fTimer += Time.deltaTime;
			ProcessFrame();
			if (m_bPlaying && m_fTimer > m_fFinishTime)
			{
				Finish();
			}
			if (Input.anyKeyDown)
			{
				OnTryCancel();
			}
		}
	}

	private void OnTryCancel()
	{
		if (m_bSoloUI && NKCUIManager.IsTopmostUI(this))
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_CUTSCENE_MOVIE_SKIP_TITLE, NKCUtilString.GET_STRING_CUTSCENE_MOVIE_SKIP_DESC, Finish);
		}
	}

	private void Finish()
	{
		m_currentEventIndex = m_lstEvent.Count;
		m_fFinishTime = 0f;
		m_bPlaying = false;
		NKCUIVoiceManager.StopVoice();
		NKCSoundManager.StopAllSound();
		if (m_bSoloUI)
		{
			Close();
		}
		NKCSoundManager.PlayScenMusic();
		dOnClose?.Invoke();
		dOnClose = null;
	}
}
