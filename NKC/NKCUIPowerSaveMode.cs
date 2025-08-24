using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC;

public class NKCUIPowerSaveMode : MonoBehaviour
{
	private const float DRAG_SENSITIVITY = 0.002f;

	public Text m_lbTime;

	public GameObject m_objBlackScreen;

	public Animator m_amtorEffect;

	public EventTrigger m_etBG;

	public Animator m_amtorBG;

	public Image m_imgFadeIn;

	public GameObject m_objBattleProgress;

	public Text m_lbBattleProgressCount;

	public GameObject m_objBattery;

	public Image m_ImgBattery;

	public Text m_lbBatteryPercent;

	public GameObject m_objInternet;

	private float m_fLastTimeUpdate;

	private float m_fDragOffset;

	private float m_fDragMoveX;

	private bool m_bComplete;

	[Header("주크박스")]
	public GameObject m_objJukeBoxBG;

	public Text m_lbJukeBoxPlayTitle;

	public GameObject m_objBattleText;

	public GameObject m_objJukeBoxText;

	private bool m_bJukeBoxMode;

	private bool bFinishJukeBox;

	private void OnDestroy()
	{
		m_imgFadeIn.DOKill();
	}

	private void Start()
	{
		SetBlackScreen(bSet: false);
		NKCUtil.SetEventTriggerDelegate(m_etBG, delegate(BaseEventData data)
		{
			OnBeginDrag(data);
		}, EventTriggerType.BeginDrag);
		NKCUtil.SetEventTriggerDelegate(m_etBG, delegate(BaseEventData data)
		{
			OnDrag(data);
		}, EventTriggerType.Drag, bInit: false);
		NKCUtil.SetEventTriggerDelegate(m_etBG, delegate(BaseEventData data)
		{
			OnEndDrag(data);
		}, EventTriggerType.EndDrag, bInit: false);
	}

	private void ResetDragState()
	{
		m_fDragOffset = 0f;
		m_fDragMoveX = 0f;
	}

	public void Open()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		ResetDragState();
		m_bJukeBoxMode = NKCScenManager.GetScenManager().GetNKCPowerSaveMode().IsJukeBoxMode;
		NKCUtil.SetGameobjectActive(m_objJukeBoxBG, m_bJukeBoxMode);
		NKCUtil.SetGameobjectActive(m_amtorBG.gameObject, !m_bJukeBoxMode);
		NKCUtil.SetGameobjectActive(m_lbJukeBoxPlayTitle.gameObject, m_bJukeBoxMode);
		NKCUtil.SetGameobjectActive(m_objBattleText, !m_bJukeBoxMode);
		NKCUtil.SetGameobjectActive(m_objJukeBoxText, m_bJukeBoxMode);
		NKCUtil.SetGameobjectActive(m_lbBattleProgressCount, !m_bJukeBoxMode);
		m_amtorEffect.SetFloat("Length", 0f);
		m_bComplete = false;
		UpdateUI();
		NKCUtil.SetGameobjectActive(m_imgFadeIn, bValue: true);
		m_imgFadeIn.color = new Color(m_imgFadeIn.color.r, m_imgFadeIn.color.g, m_imgFadeIn.color.b, 1f);
		m_imgFadeIn.DOFade(0f, 1.3f);
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_bComplete = false;
	}

	public void SetBlackScreen(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_objBlackScreen, bSet);
		ResetDragState();
		if (!bSet)
		{
			m_amtorEffect.SetFloat("Length", 0f);
			UpdateUI();
		}
	}

	private void UpdateUI()
	{
		NKCUtil.SetLabelText(m_lbTime, DateTime.Now.Hour.ToString("D2") + ":" + DateTime.Now.Minute.ToString("D2"));
		bool isOnGoing = NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing();
		if (!m_bComplete && !isOnGoing)
		{
			m_bComplete = true;
			NKCScenManager.GetScenManager().GetNKCPowerSaveMode().SetLastKeyInputTime(Time.time);
			m_amtorBG.Play("NKM_UI_SLEEP_MODE_EFFECT_COMPLETE", -1, 0f);
		}
		NKCUtil.SetGameobjectActive(m_objBattleProgress, !bFinishJukeBox && (isOnGoing || m_bJukeBoxMode));
		if (isOnGoing)
		{
			long currRepeatCount = NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetCurrRepeatCount();
			long maxRepeatCount = NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetMaxRepeatCount();
			NKCUtil.SetLabelText(m_lbBattleProgressCount, $"({currRepeatCount}/{maxRepeatCount})");
		}
		float batteryLevel = SystemInfo.batteryLevel;
		if (batteryLevel == -1f)
		{
			NKCUtil.SetGameobjectActive(m_objBattery, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objBattery, bValue: true);
			m_ImgBattery.fillAmount = batteryLevel;
			NKCUtil.SetLabelText(m_lbBatteryPercent, (int)(batteryLevel * 100f) + "%");
			if (batteryLevel <= 0.2f)
			{
				m_ImgBattery.color = new Color(1f, 0f, 0f, 1f);
			}
			else
			{
				m_ImgBattery.color = new Color(1f, 1f, 1f, 1f);
			}
		}
		NKCUtil.SetGameobjectActive(m_objInternet, bValue: false);
	}

	private void Update()
	{
		if (m_fLastTimeUpdate + 1f < Time.time)
		{
			m_fLastTimeUpdate = Time.time;
			UpdateUI();
		}
	}

	public void OnBeginDrag(BaseEventData eventData)
	{
		ResetDragState();
		m_amtorEffect.SetFloat("Length", 0f);
	}

	public void OnDrag(BaseEventData eventData)
	{
		Vector2 delta = (eventData as PointerEventData).delta;
		m_fDragOffset += Vector2.Dot(delta, Vector2.right);
		m_fDragMoveX = m_fDragOffset * 0.002f;
		m_amtorEffect.SetFloat("Length", Mathf.Clamp(m_fDragMoveX, 0f, 1f));
	}

	public void OnEndDrag(BaseEventData eventData)
	{
		float num = Mathf.Clamp(m_fDragMoveX, 0f, 1f);
		if (num >= 0.85f)
		{
			m_amtorEffect.SetFloat("Length", num);
			NKCScenManager.GetScenManager().GetNKCPowerSaveMode().SetEnable(bEnable: false);
		}
		else
		{
			m_amtorEffect.SetFloat("Length", 0f);
		}
		ResetDragState();
	}

	public void SetJukeBoxTitle(string lbTitle)
	{
		NKCUtil.SetLabelText(m_lbJukeBoxPlayTitle, lbTitle);
	}

	public void SetFinishJukeBox(bool bFinish)
	{
		bFinishJukeBox = bFinish;
		SetJukeBoxTitle(NKCUtilString.GET_STRING_JUKEBOX_FINISH_SLEEP_MODE);
	}
}
