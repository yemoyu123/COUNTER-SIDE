using System.Collections;
using System.Collections.Generic;
using ClientPacket.Contract;
using NKC.UI.Contract;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Result;

public class NKCUIResultMiscContract : NKCUIResultSubUIBase
{
	public LoopScrollRect m_loopRewardList;

	public NKCUIResultRewardSlot m_pfbSlot;

	public Transform m_trIdleObjectRoot;

	private List<NKCUIResultRewardSlot> m_lstSlot;

	private Stack<NKCUIResultRewardSlot> m_stkSlotIdle;

	private List<NKCUISlot.SlotData> m_lstRewardSlotData;

	private Queue<NKCUIResultRewardSlot> m_queueSlot = new Queue<NKCUIResultRewardSlot>();

	private bool m_bNeedEnqueue;

	private int m_iLastIndex = -1;

	private List<MiscContractResult> m_lstContractResult;

	private MiscContractResult m_contractResult;

	private bool m_bWait;

	private int contractIndex;

	private Animator m_animator;

	public CanvasGroup m_canvasGroup;

	private float m_fWaitTimeForCloseAnimation = 1f;

	private bool m_bAutoSkip;

	private bool m_bFinished;

	private const string REWARD_SOUND_BUNDLE_NAME = "ab_sound";

	private const string REWARD_SOUND_ASSET_NAME = "FX_UI_ITEM_RESULT_LISTUP";

	private const float SLOT_APPEAR_DELAY = 0.1f;

	private float m_fElapsedTime;

	private bool bWaiting;

	public void Init()
	{
		m_lstSlot = new List<NKCUIResultRewardSlot>();
		m_stkSlotIdle = new Stack<NKCUIResultRewardSlot>();
		m_lstRewardSlotData = new List<NKCUISlot.SlotData>();
		m_loopRewardList.dOnGetObject += GetObject;
		m_loopRewardList.dOnReturnObject += ReturnObject;
		m_loopRewardList.dOnProvideData += ProvideData;
		m_loopRewardList.ContentConstraintCount = 5;
		m_loopRewardList.PrepareCells();
		EventTrigger component = base.gameObject.GetComponent<EventTrigger>();
		if (component != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback = new EventTrigger.TriggerEvent();
			entry.callback.AddListener(OnUserInputEvent);
			component.triggers.Add(entry);
		}
		m_animator = base.transform.GetComponent<Animator>();
		if (m_animator != null)
		{
			m_animator.speed = 1.5f;
		}
	}

	private void OnUserInputEvent(BaseEventData eventData)
	{
		if (!m_bWait)
		{
			m_bHadUserInput = true;
		}
	}

	public void SetData(List<MiscContractResult> lstContractResult)
	{
		m_lstContractResult = lstContractResult;
		contractIndex = 0;
		m_iLastIndex = -1;
		m_canvasGroup.alpha = 1f;
		m_lstRewardSlotData.Clear();
		base.ProcessRequired = false;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (m_lstContractResult != null)
		{
			foreach (MiscContractResult item in m_lstContractResult)
			{
				if (item != null && item.units.Count > 0)
				{
					base.ProcessRequired = true;
					break;
				}
			}
		}
		m_bNeedEnqueue = base.ProcessRequired;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void SetData(List<NKMRewardData> lstRewardData)
	{
		List<MiscContractResult> list = new List<MiscContractResult>();
		foreach (NKMRewardData lstRewardDatum in lstRewardData)
		{
			if (lstRewardDatum?.ContractList != null)
			{
				list.AddRange(lstRewardDatum.ContractList);
			}
		}
		if (list.Count > 0)
		{
			SetData(list);
		}
		else
		{
			base.ProcessRequired = false;
		}
	}

	public bool WillProcess()
	{
		if (m_lstContractResult == null)
		{
			return false;
		}
		m_iLastIndex = -1;
		while (contractIndex < m_lstContractResult.Count)
		{
			m_contractResult = m_lstContractResult[contractIndex];
			contractIndex++;
			if (m_contractResult != null && m_contractResult.units != null && m_contractResult.units.Count > 0)
			{
				m_lstRewardSlotData = new List<NKCUISlot.SlotData>(NKCUISlot.SlotData.MakeUnitData(m_contractResult.units));
				m_loopRewardList.TotalCount = 0;
				m_loopRewardList.RefreshCells();
				return true;
			}
		}
		m_contractResult = null;
		return false;
	}

	protected override IEnumerator InnerProcess(bool bAutoSkip)
	{
		m_bFinished = false;
		m_bAutoSkip = bAutoSkip;
		m_animator.enabled = false;
		m_bHadUserInput = false;
		m_canvasGroup.blocksRaycasts = false;
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstSlot[i], bValue: false);
			m_lstSlot[i].SetEffectEnable(bEnable: false);
		}
		NKM_UNIT_GRADE nKM_UNIT_GRADE = NKM_UNIT_GRADE.NUG_N;
		bool bAwaken = false;
		foreach (NKMUnitData unit in m_contractResult.units)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unit);
			if (unitTempletBase.m_NKM_UNIT_GRADE > nKM_UNIT_GRADE)
			{
				nKM_UNIT_GRADE = unitTempletBase.m_NKM_UNIT_GRADE;
			}
			if (unitTempletBase.m_bAwaken)
			{
				bAwaken = true;
			}
		}
		m_bWait = true;
		NKMRewardData dummyReward = new NKMRewardData();
		dummyReward.SetUnitData(m_contractResult.units);
		NKCUIContractSequence.Instance.Open(nKM_UNIT_GRADE, bAwaken, delegate
		{
			NKCUIGameResultGetUnit.ShowNewUnitGetUI(dummyReward, delegate
			{
				m_bWait = false;
			}, bAutoSkip);
		});
		while (m_bWait)
		{
			yield return null;
		}
		m_bNeedEnqueue = true;
		m_loopRewardList.TotalCount = m_lstRewardSlotData.Count;
		m_loopRewardList.RefreshCells();
		yield return new WaitForSeconds(0.2f);
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
					yield return ShowAllSlot();
					break;
				}
				m_fElapsedTime = 0f;
				NKCUtil.SetGameobjectActive(slot, bValue: true);
				slot.SetEffectEnable(bEnable: true);
				NKCSoundManager.PlaySound("FX_UI_ITEM_RESULT_LISTUP", 1f, base.transform.position.x, 0f);
				slot.GetComponent<CanvasGroup>().alpha = 0.1f;
				if (index % m_loopRewardList.ContentConstraintCount == 0 && index > m_loopRewardList.ContentConstraintCount)
				{
					yield return null;
					m_loopRewardList.ScrollToCell(index, 0.4f, LoopScrollRect.ScrollTarget.Center);
					yield return new WaitForSeconds(0.1f);
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
		FinishProcess();
	}

	public override void FinishProcess()
	{
		if (base.gameObject.activeInHierarchy && !bWaiting)
		{
			m_bNeedEnqueue = false;
			bWaiting = true;
			StopAllCoroutines();
			StartCoroutine(ShowAllSlot());
			m_canvasGroup.blocksRaycasts = true;
			m_bHadUserInput = false;
			StartCoroutine(WaitForCloseAnimation());
		}
	}

	public IEnumerator WaitForCloseAnimation()
	{
		m_bHadUserInput = false;
		m_fWaitTimeForCloseAnimation = 1f;
		if (m_bAutoSkip)
		{
			float currentTime = 0f;
			while (m_fWaitTimeForCloseAnimation > currentTime)
			{
				if (m_bHadUserInput)
				{
					currentTime += 1f;
					m_bHadUserInput = false;
				}
				if (!m_bPause)
				{
					currentTime += Time.deltaTime;
				}
				_ = m_fWaitTimeForCloseAnimation;
				_ = currentTime;
				yield return null;
			}
		}
		else
		{
			yield return new WaitForSeconds(0.5f);
			yield return WaitAniOrInput(null);
			for (int i = 0; i < m_lstSlot.Count; i++)
			{
				m_lstSlot[i].SetEffectEnable(bEnable: false);
			}
		}
		m_animator.enabled = true;
		while (m_bPause)
		{
			yield return null;
		}
		yield return PlayCloseAnimation(m_animator);
		m_bFinished = true;
		bWaiting = false;
	}

	private IEnumerator ShowAllSlot()
	{
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			if (i < m_lstRewardSlotData.Count)
			{
				NKCUtil.SetGameobjectActive(m_lstSlot[i], bValue: true);
				yield return null;
				m_lstSlot[i].GetComponent<CanvasGroup>().alpha = 1f;
				m_lstSlot[i].SetEffectEnable(bEnable: true);
			}
		}
		yield return null;
		m_loopRewardList.SetIndexPosition(m_loopRewardList.TotalCount - 1);
	}

	public override bool IsProcessFinished()
	{
		return m_bFinished;
	}

	public void CleanUpData()
	{
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			m_lstSlot[i].SetEffectEnable(bEnable: false);
		}
		m_queueSlot.Clear();
		m_loopRewardList.PrepareCells();
		m_canvasGroup.alpha = 1f;
		m_contractResult = null;
		m_lstRewardSlotData.Clear();
	}

	public override void OnUserInput()
	{
		if (!m_bWait)
		{
			base.OnUserInput();
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
			NKCUISlot.SlotData data = m_lstRewardSlotData[idx];
			component.SetData(data, idx);
			NKCUtil.SetGameobjectActive(component, bValue: true);
		}
		else
		{
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

	public void SetActiveLoopScrollList(bool bActivate)
	{
		m_loopRewardList.enabled = bActivate;
		if (bActivate)
		{
			m_loopRewardList.RefreshCells();
		}
	}
}
