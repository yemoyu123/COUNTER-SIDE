using System.Collections;
using System.Collections.Generic;
using ClientPacket.Common;
using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Result;

public class NKCUIResultSubUIReward : NKCUIResultSubUIBase
{
	public class Data
	{
		public NKMRewardData rewardData;

		public NKMArmyData armyData;

		public bool bAutoSkipUnitGain;

		public NKMRewardData firstRewardData;

		public bool bIgnoreAutoClose;

		public NKMRewardData selectRewardData;

		public string selectSlotText = "";

		public bool bAllowRewardDataNull;

		public NKMRewardData onetimeRewardData;

		public NKMRewardData firstAllClearRewardData;

		public NKMAdditionalReward additionalReward;

		public BATTLE_RESULT_TYPE battleResultType;
	}

	private const string REWARD_SOUND_BUNDLE_NAME = "ab_sound";

	private const string REWARD_SOUND_ASSET_NAME = "FX_UI_ITEM_RESULT_LISTUP";

	private const float SLOT_APPEAR_DELAY = 0.1f;

	public NKCUIResultRewardSlot m_pfbSlot;

	private Animator m_animator;

	public CanvasGroup m_canvasGroup;

	private LoopVerticalScrollRect m_loopRewardList;

	private Transform m_trSlotObjectRoot;

	private Transform m_trIdleObjectRoot;

	public GameObject m_objRefund;

	private List<NKCUIResultRewardSlot> m_lstSlot;

	private Stack<NKCUIResultRewardSlot> m_stkSlotIdle;

	private List<NKCUISlot.SlotData> m_lstRewardSlotData;

	private EventTrigger m_eventTrigger;

	private int m_iSlotCount;

	private int m_iFirstRewardCount;

	private int m_iFirstAllClearRewardCnt;

	private int m_iOnetimeRewardCount;

	private int m_iSelectRewardCount;

	private string m_strSelectRewardText;

	private bool m_bFinished;

	private bool m_bWaitForUnitGain;

	private Vector3 m_orgSlotRootPos;

	private string m_assetName = "";

	private bool m_bAutoSkip;

	private Queue<NKCUIResultRewardSlot> m_queueSlot = new Queue<NKCUIResultRewardSlot>();

	private bool m_bNeedEnqueue;

	private int m_iLastIndex = -1;

	private bool m_bReservedDoubleTokenAddCount;

	private long m_DoubleTokenAddCount;

	private Text m_lbDoubleTokenCount;

	private Animator m_amtorDoubleToken;

	private float m_fElapsedTime;

	private bool bWaiting;

	public void SetReservedDoubleTokenAddCount(long count)
	{
		m_bReservedDoubleTokenAddCount = true;
		m_DoubleTokenAddCount = count;
	}

	public void Init(Text lbDoubleTokenCount, Animator amtorDoubleToken)
	{
		m_amtorDoubleToken = amtorDoubleToken;
		m_animator = base.transform.GetComponent<Animator>();
		if (m_animator != null)
		{
			m_animator.speed = 1.5f;
		}
		m_loopRewardList = base.transform.Find("MASK/LoopScroll").GetComponent<LoopVerticalScrollRect>();
		m_trSlotObjectRoot = base.transform.Find("MASK/LoopScroll/item");
		m_trIdleObjectRoot = base.transform.Find("IdleObjectParent");
		m_lstSlot = new List<NKCUIResultRewardSlot>();
		m_stkSlotIdle = new Stack<NKCUIResultRewardSlot>();
		m_lstRewardSlotData = new List<NKCUISlot.SlotData>();
		m_iSlotCount = 0;
		m_loopRewardList.dOnGetObject += GetObject;
		m_loopRewardList.dOnReturnObject += ReturnObject;
		m_loopRewardList.dOnProvideData += ProvideData;
		m_loopRewardList.ContentConstraintCount = 5;
		m_loopRewardList.PrepareCells();
		NKCUtil.SetScrollHotKey(m_loopRewardList);
		m_orgSlotRootPos = m_trSlotObjectRoot.localPosition;
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback = new EventTrigger.TriggerEvent();
		entry.callback.AddListener(OnUserInputEvent);
		m_eventTrigger = base.gameObject.GetComponent<EventTrigger>();
		m_eventTrigger.triggers.Add(entry);
		m_assetName = "FX_UI_ITEM_RESULT_LISTUP";
		m_lbDoubleTokenCount = lbDoubleTokenCount;
	}

	public void OnUserInputByResult()
	{
		if (!m_bWaitForUnitGain)
		{
			m_bHadUserInput = true;
		}
	}

	private void OnUserInputEvent(BaseEventData eventData)
	{
		if (!m_bWaitForUnitGain)
		{
			m_bHadUserInput = true;
		}
	}

	public override bool IsProcessFinished()
	{
		return m_bFinished;
	}

	protected override IEnumerator InnerProcess(bool bAutoSkip)
	{
		m_bFinished = false;
		m_bAutoSkip = bAutoSkip;
		m_animator.enabled = false;
		m_bHadUserInput = false;
		m_canvasGroup.blocksRaycasts = false;
		NKCUtil.SetGameobjectActive(m_amtorDoubleToken.gameObject, bValue: false);
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstSlot[i], bValue: false);
			m_lstSlot[i].SetEffectEnable(bEnable: false);
			m_lstSlot[i].m_NKCUISlot.m_lbItemCount.DOKill();
			if (m_bReservedDoubleTokenAddCount && m_lstSlot[i].m_NKCUISlot.GetSlotData() != null && m_lstSlot[i].m_NKCUISlot.GetSlotData().ID == 5)
			{
				m_lstSlot[i].m_NKCUISlot.m_lbItemCount.text = (m_lstSlot[i].m_NKCUISlot.GetCount() - m_DoubleTokenAddCount).ToString();
				break;
			}
		}
		yield return WaitTimeOrUserInput(0.2f);
		while (m_queueSlot.Count > 0)
		{
			NKCUIResultRewardSlot slot = m_queueSlot.Dequeue();
			int index = slot.GetIdx();
			if (index <= m_iLastIndex)
			{
				continue;
			}
			if (index < m_lstRewardSlotData.Count)
			{
				if (m_bHadUserInput)
				{
					ShowAllSlot();
					break;
				}
				m_fElapsedTime = 0f;
				NKCUtil.SetGameobjectActive(slot, bValue: true);
				slot.SetEffectEnable(bEnable: true);
				NKCSoundManager.PlaySound(m_assetName, 1f, base.transform.position.x, 0f);
				slot.GetComponent<CanvasGroup>().alpha = 0.1f;
				if (index % m_loopRewardList.ContentConstraintCount == 0 && index > m_loopRewardList.ContentConstraintCount)
				{
					yield return null;
					m_loopRewardList.ScrollToCell(index, 0.4f, LoopScrollRect.ScrollTarget.Center);
					yield return WaitTimeOrUserInput(0.1f);
				}
				while (m_fElapsedTime < 0.1f)
				{
					m_fElapsedTime += Time.deltaTime;
					float num = m_fElapsedTime / 0.1f;
					if (num < 0f)
					{
						num = 0f;
					}
					if (num >= 1f)
					{
						num = 1f;
					}
					slot.GetComponent<CanvasGroup>().alpha = num;
					yield return null;
				}
			}
			m_iLastIndex = index;
			if (index == m_loopRewardList.TotalCount - 1)
			{
				break;
			}
		}
		if (m_bReservedDoubleTokenAddCount)
		{
			yield return new WaitForSeconds(0.8f);
			NKCUtil.SetGameobjectActive(m_amtorDoubleToken.gameObject, bValue: true);
			m_amtorDoubleToken.Play("NKM_UI_RESULT_PVPPOINTX2_INTRO");
			yield return new WaitForSeconds(0.8f);
			long countMiscItem = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetCountMiscItem(301);
			if (m_lbDoubleTokenCount.text.CompareTo(countMiscItem.ToString()) != 0)
			{
				m_lbDoubleTokenCount.DOText(countMiscItem.ToString(), 0.4f, richTextEnabled: true, ScrambleMode.Numerals);
			}
			for (int j = 0; j < m_lstSlot.Count; j++)
			{
				NKCUIResultRewardSlot nKCUIResultRewardSlot = m_lstSlot[j];
				if (!(nKCUIResultRewardSlot == null) && nKCUIResultRewardSlot.m_NKCUISlot.GetSlotData() != null && nKCUIResultRewardSlot.m_NKCUISlot.GetSlotData().ID == 5)
				{
					nKCUIResultRewardSlot.m_NKCUISlot.SetNewCountAni(m_DoubleTokenAddCount + nKCUIResultRewardSlot.m_NKCUISlot.GetCount());
					break;
				}
			}
			yield return new WaitForSeconds(1.4f);
		}
		FinishProcess();
	}

	public override void FinishProcess()
	{
		if (!base.gameObject.activeInHierarchy || bWaiting)
		{
			return;
		}
		m_bNeedEnqueue = false;
		bWaiting = true;
		StopAllCoroutines();
		ShowAllSlot();
		if (m_bReservedDoubleTokenAddCount)
		{
			NKCUtil.SetGameobjectActive(m_amtorDoubleToken.gameObject, bValue: true);
			m_amtorDoubleToken.Play("NKM_UI_RESULT_PVPPOINTX2_IDLE");
			long num = 0L;
			num = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetCountMiscItem(301);
			if (DOTween.IsTweening(m_lbDoubleTokenCount))
			{
				m_lbDoubleTokenCount.DOKill();
			}
			if (m_lbDoubleTokenCount.text.CompareTo(num.ToString()) != 0)
			{
				m_lbDoubleTokenCount.text = num.ToString();
			}
		}
		m_canvasGroup.blocksRaycasts = true;
		m_bHadUserInput = false;
		StartCoroutine(WaitForCloseAnimation());
	}

	public IEnumerator WaitForCloseAnimation()
	{
		m_bHadUserInput = false;
		if (m_bWillPlayCountdown)
		{
			yield return WaitTimeOrUserInput(1f);
		}
		else
		{
			yield return WaitAniOrInput(null);
		}
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			m_lstSlot[i].SetEffectEnable(bEnable: false);
		}
		m_animator.enabled = true;
		if (NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetAlarmRepeatOperationSuccess())
		{
			m_bPause = true;
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetAlarmRepeatOperationSuccess(bSet: false);
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetStopReason(NKCUtilString.GET_STRING_REPEAT_OPERATION_IS_TERMINATED);
			NKCPopupRepeatOperation.Instance.OpenForResult(delegate
			{
				m_bPause = false;
			});
		}
		while (m_bPause)
		{
			yield return null;
		}
		if (m_bReservedDoubleTokenAddCount)
		{
			NKCUtil.SetGameobjectActive(m_amtorDoubleToken.gameObject, bValue: true);
			m_amtorDoubleToken.Play("NKM_UI_RESULT_PVPPOINTX2_OUTRO");
		}
		if (!m_bWillPlayCountdown)
		{
			yield return PlayCloseAnimation(m_animator);
		}
		m_bFinished = true;
		bWaiting = false;
	}

	private void ShowAllSlot()
	{
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			if (i < m_iSlotCount)
			{
				NKCUtil.SetGameobjectActive(m_lstSlot[i], bValue: true);
				m_lstSlot[i].GetComponent<CanvasGroup>().alpha = 1f;
				m_lstSlot[i].SetEffectEnable(bEnable: true);
			}
		}
		m_loopRewardList.SetIndexPosition(m_loopRewardList.TotalCount - 1);
	}

	public override void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		CleanUpData();
	}

	private void CleanUpData()
	{
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			m_lstSlot[i].SetEffectEnable(bEnable: false);
		}
		m_queueSlot.Clear();
		m_loopRewardList.PrepareCells();
		m_canvasGroup.alpha = 1f;
		m_lstRewardSlotData.Clear();
		m_bReservedDoubleTokenAddCount = false;
		m_bFinished = true;
		bWaiting = false;
	}

	public void SetData(Data data)
	{
		m_bReservedDoubleTokenAddCount = false;
		if (data == null)
		{
			base.ProcessRequired = false;
			return;
		}
		if (!data.bAllowRewardDataNull && data.rewardData == null)
		{
			base.ProcessRequired = false;
			return;
		}
		m_bIgnoreAutoClose = data.bIgnoreAutoClose;
		m_canvasGroup.alpha = 1f;
		m_trSlotObjectRoot.localPosition = m_orgSlotRootPos;
		m_lstRewardSlotData.Clear();
		NKCUtil.SetGameobjectActive(m_objRefund, bValue: true);
		if (data.battleResultType == BATTLE_RESULT_TYPE.BRT_LOSE)
		{
			NKCUtil.SetGameobjectActive(m_objRefund, bValue: true);
			m_loopRewardList.scrollSensitivity = 0f;
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objRefund, bValue: false);
			m_loopRewardList.scrollSensitivity = 1f;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_iFirstRewardCount = 0;
		if (data.firstRewardData != null)
		{
			m_lstRewardSlotData.AddRange(NKCUISlot.MakeSlotDataListFromReward(data.firstRewardData));
			m_iFirstRewardCount = m_lstRewardSlotData.Count;
		}
		m_iFirstAllClearRewardCnt = 0;
		if (data.firstAllClearRewardData != null)
		{
			List<NKCUISlot.SlotData> list = NKCUISlot.MakeSlotDataListFromReward(data.firstAllClearRewardData);
			m_iFirstAllClearRewardCnt = list.Count;
			m_lstRewardSlotData.AddRange(list);
		}
		m_iOnetimeRewardCount = 0;
		if (data.onetimeRewardData != null)
		{
			List<NKCUISlot.SlotData> list2 = NKCUISlot.MakeSlotDataListFromReward(data.onetimeRewardData);
			m_iOnetimeRewardCount = list2.Count;
			m_lstRewardSlotData.AddRange(list2);
		}
		if (data.rewardData != null)
		{
			m_lstRewardSlotData.AddRange(NKCUISlot.MakeSlotDataListFromReward(data.rewardData));
		}
		if (data.additionalReward != null)
		{
			m_lstRewardSlotData.AddRange(NKCUISlot.MakeSlotDataListFromReward(data.additionalReward));
		}
		m_iSelectRewardCount = 0;
		m_strSelectRewardText = data.selectSlotText;
		if (data.selectRewardData != null)
		{
			List<NKCUISlot.SlotData> list3 = NKCUISlot.MakeSlotDataListFromReward(data.selectRewardData);
			m_lstRewardSlotData.AddRange(list3);
			m_iSelectRewardCount = list3.Count;
		}
		m_iSlotCount = m_lstRewardSlotData.Count;
		if (!data.bAllowRewardDataNull)
		{
			base.ProcessRequired = m_iSlotCount != 0;
		}
		else
		{
			base.ProcessRequired = true;
		}
		if (m_iSlotCount > 0)
		{
			m_bNeedEnqueue = base.ProcessRequired;
		}
		else
		{
			m_bNeedEnqueue = false;
		}
		m_iLastIndex = -1;
		m_loopRewardList.TotalCount = m_iSlotCount;
		m_loopRewardList.RefreshCells();
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			m_lstSlot[i].GetComponent<CanvasGroup>().alpha = 0f;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void SetBoxGainData(List<NKCUISlot.SlotData> lstReward)
	{
		if (lstReward == null || lstReward.Count == 0)
		{
			base.ProcessRequired = false;
			return;
		}
		m_bReservedDoubleTokenAddCount = false;
		m_canvasGroup.alpha = 1f;
		m_trSlotObjectRoot.localPosition = m_orgSlotRootPos;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_objRefund, bValue: false);
		m_iFirstRewardCount = 0;
		m_iFirstAllClearRewardCnt = 0;
		m_iOnetimeRewardCount = 0;
		m_iSelectRewardCount = 0;
		m_strSelectRewardText = "";
		m_lstRewardSlotData = lstReward;
		m_iSlotCount = ((m_lstRewardSlotData != null) ? m_lstRewardSlotData.Count : 0);
		base.ProcessRequired = m_iSlotCount != 0;
		m_bNeedEnqueue = base.ProcessRequired;
		m_iLastIndex = -1;
		m_loopRewardList.TotalCount = m_iSlotCount;
		m_loopRewardList.RefreshCells();
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			m_lstSlot[i].GetComponent<CanvasGroup>().alpha = 0f;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void SetActiveLoopScrollList(bool bActivate)
	{
		m_loopRewardList.enabled = bActivate;
		if (bActivate)
		{
			m_loopRewardList.RefreshCells();
		}
	}

	private RectTransform GetObject(int index)
	{
		if (m_stkSlotIdle.Count > 0)
		{
			NKCUIResultRewardSlot nKCUIResultRewardSlot = m_stkSlotIdle.Pop();
			NKCUtil.SetGameobjectActive(nKCUIResultRewardSlot, bValue: true);
			m_lstSlot.Add(nKCUIResultRewardSlot);
			nKCUIResultRewardSlot.SetEffectEnable(bEnable: false);
			return nKCUIResultRewardSlot.GetComponent<RectTransform>();
		}
		NKCUIResultRewardSlot nKCUIResultRewardSlot2 = Object.Instantiate(m_pfbSlot);
		nKCUIResultRewardSlot2.Init();
		nKCUIResultRewardSlot2.transform.localScale = Vector3.one;
		nKCUIResultRewardSlot2.transform.localPosition = Vector3.zero;
		nKCUIResultRewardSlot2.gameObject.AddComponent<CanvasGroup>();
		NKCUtil.SetGameobjectActive(nKCUIResultRewardSlot2, bValue: true);
		m_lstSlot.Add(nKCUIResultRewardSlot2);
		return nKCUIResultRewardSlot2.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform go)
	{
		NKCUtil.SetGameobjectActive(go, bValue: false);
		go.SetParent(m_trIdleObjectRoot);
		NKCUIResultRewardSlot component = go.GetComponent<NKCUIResultRewardSlot>();
		component.SetEffectEnable(bEnable: false);
		if (m_lstSlot.Contains(component))
		{
			m_lstSlot.Remove(component);
		}
		if (!m_stkSlotIdle.Contains(component))
		{
			m_stkSlotIdle.Push(component);
		}
	}

	private void ProvideData(Transform transform, int idx)
	{
		NKCUIResultRewardSlot component = transform.GetComponent<NKCUIResultRewardSlot>();
		if (m_lstRewardSlotData.Count > idx)
		{
			component.SetData(m_lstRewardSlotData[idx], idx);
			component.SetFirstRewardMark(idx < m_iFirstRewardCount);
			component.SetFirstAllClearRewardMark(idx >= m_iFirstRewardCount && idx < m_iFirstAllClearRewardCnt + m_iFirstRewardCount);
			component.SetOnetimeRewardMark(idx >= m_iFirstRewardCount + m_iFirstAllClearRewardCnt && idx < m_iOnetimeRewardCount + m_iFirstRewardCount + m_iFirstAllClearRewardCnt);
			bool flag = idx >= m_lstRewardSlotData.Count - m_iSelectRewardCount;
			component.SetSelect(flag);
			component.SetText(flag ? m_strSelectRewardText : "");
			NKCUtil.SetGameobjectActive(component, bValue: true);
		}
		else
		{
			Debug.Log($"[NKCUIResultSubUIReward] Slot list count is lower than idx. count : {m_lstRewardSlotData.Count}, idx : {idx}, NeedEnqueue : {m_bNeedEnqueue}, LastIndex : {m_iLastIndex}");
			NKCUtil.SetGameobjectActive(component, bValue: false);
		}
		if (m_bNeedEnqueue && idx > m_iLastIndex)
		{
			component.SetEffectEnable(bEnable: false);
			component.GetComponent<CanvasGroup>().alpha = 0f;
			m_queueSlot.Enqueue(component);
		}
		else
		{
			component.GetComponent<CanvasGroup>().alpha = 1f;
		}
		if (idx == m_loopRewardList.TotalCount)
		{
			m_bNeedEnqueue = false;
		}
	}

	public override void OnUserInput()
	{
		if (!m_bWaitForUnitGain)
		{
			m_bHadUserInput = true;
		}
	}
}
