using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Cs.Logging;
using Cs.Math;
using NKC.UI.Component;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupMatchTen : MonoBehaviour
{
	public delegate void OnGameEnd(int score, int remainTime);

	public delegate void OnClose();

	public Animator m_Ani;

	public List<Sprite> m_lstSlotIcon = new List<Sprite>();

	public NKCUIComStateButton m_btnBack;

	public NKCComTMPUIText m_lbScore;

	public NKCComTMPUIText m_lbRemainTime;

	public Transform m_trSlotParent;

	public NKCPopupMatchTenSlot m_pfbSlot;

	public NKCUIComScreenDragSelection m_DragSelection;

	public GameObject m_objCountdown;

	public Image m_imgCountdownNum;

	public GameObject m_objReroll;

	public NKCUIComStateButton m_btnReroll;

	private OnGameEnd m_dOnGameEnd;

	private OnClose m_dOnClose;

	private NKCPopupMatchTenSlot[,] m_BoardSlots = new NKCPopupMatchTenSlot[NKCMatchTenManager.TOTAL_COL_COUNT, NKCMatchTenManager.TOTAL_ROW_COUNT];

	private bool m_bPause;

	private const int SCORE_THRESHOLD = 1;

	private const int TIME_THRESHOLD = 1000;

	private int m_iconIdx;

	private int m_Score_1;

	private List<NKCPopupMatchTenSlot> m_lstSelectedSlot = new List<NKCPopupMatchTenSlot>();

	public int m_TotalSecond = NKCMatchTenManager.GetTotalTimeSec();

	private Stopwatch m_Stopwatch = new Stopwatch();

	private int m_Score_2;

	private Stopwatch m_StopwatchTotal = new Stopwatch();

	private byte m_DataEncryptSeed;

	private Stopwatch m_StopwatchPause = new Stopwatch();

	private int m_Score_3;

	private float m_deltaTime;

	public void Encrypt()
	{
		int score = GetScore();
		m_DataEncryptSeed = (byte)RandomGenerator.Range(10, 100);
		SetScore(score);
	}

	public int GetScore()
	{
		return m_Score_2 - m_DataEncryptSeed;
	}

	public void SetScore(int score)
	{
		m_Score_2 = score + m_DataEncryptSeed;
		m_Score_3 = score;
		if (m_DataEncryptSeed > 50)
		{
			m_Score_1 = score + RandomGenerator.Range(10, 100);
		}
		NKCUtil.SetLabelText(m_lbScore, GetScore().ToString());
	}

	public void InitUI(OnGameEnd onGameEnd, OnClose onClose)
	{
		NKCUtil.SetButtonClickDelegate(m_btnBack, OnClickBackButton);
		NKCUtil.SetButtonClickDelegate(m_btnReroll, OnConfirmReroll);
		m_DragSelection.Init(OnDrag);
		m_dOnGameEnd = onGameEnd;
		m_dOnClose = onClose;
	}

	public void Close()
	{
		StopAllCoroutines();
		m_dOnClose?.Invoke();
	}

	public void PresetData()
	{
		if (m_trSlotParent.childCount > 0)
		{
			for (int num = m_trSlotParent.childCount - 1; num >= 0; num--)
			{
				UnityEngine.Object.Destroy(m_trSlotParent.GetChild(num).gameObject);
			}
		}
		m_BoardSlots = new NKCPopupMatchTenSlot[NKCMatchTenManager.TOTAL_COL_COUNT, NKCMatchTenManager.TOTAL_ROW_COUNT];
		m_trSlotParent.GetComponent<GridLayoutGroup>().constraintCount = NKCMatchTenManager.TOTAL_COL_COUNT;
		for (int i = 0; i < NKCMatchTenManager.TOTAL_ROW_COUNT; i++)
		{
			for (int j = 0; j < NKCMatchTenManager.TOTAL_COL_COUNT; j++)
			{
				NKCPopupMatchTenSlot nKCPopupMatchTenSlot = UnityEngine.Object.Instantiate(m_pfbSlot, m_trSlotParent);
				m_BoardSlots[j, i] = nKCPopupMatchTenSlot;
				nKCPopupMatchTenSlot.gameObject.name = $"{j}_{i}";
				nKCPopupMatchTenSlot.transform.SetAsLastSibling();
			}
		}
		m_Stopwatch.Reset();
		m_StopwatchPause.Reset();
		m_StopwatchTotal.Reset();
		Encrypt();
		SetScore(0);
		SetTimeLabel();
	}

	public void Open(bool bResetIconIdx = false)
	{
		SetScore(0);
		m_Stopwatch.Reset();
		m_StopwatchPause.Reset();
		m_StopwatchTotal.Reset();
		SetTimeLabel();
		m_bPause = true;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_trSlotParent, bValue: true);
		SetBoardData(bResetIconIdx);
		StartGame();
	}

	public void SetBoardData(bool bResetIconIdx = false)
	{
		if (bResetIconIdx)
		{
			m_iconIdx = RandomGenerator.Range(0, m_lstSlotIcon.Count);
		}
		if (m_iconIdx >= m_lstSlotIcon.Count)
		{
			return;
		}
		Sprite sprIcon = m_lstSlotIcon[m_iconIdx];
		for (int i = 0; i < NKCMatchTenManager.TOTAL_COL_COUNT; i++)
		{
			for (int j = 0; j < NKCMatchTenManager.TOTAL_ROW_COUNT; j++)
			{
				m_BoardSlots[i, j].SetData(i, j, NKCMatchTenManager.GetBoardData()[i, j], sprIcon);
			}
		}
	}

	private void StartGame(bool bIsReroll = false)
	{
		NKCUtil.SetGameobjectActive(m_objReroll, bValue: false);
		if (!bIsReroll)
		{
			StartCoroutine(WaitForCountdown());
			return;
		}
		m_Stopwatch.Start();
		m_StopwatchPause.Stop();
		m_bPause = false;
	}

	private void OnDrag(Vector2 minPos, Vector2 maxPos, bool bDragEnd)
	{
		if (m_bPause)
		{
			return;
		}
		m_lstSelectedSlot.Clear();
		int num = NKCMatchTenManager.TOTAL_COL_COUNT;
		int num2 = 0;
		int num3 = NKCMatchTenManager.TOTAL_ROW_COUNT;
		int num4 = 0;
		for (int i = 0; i < NKCMatchTenManager.TOTAL_COL_COUNT; i++)
		{
			for (int j = 0; j < NKCMatchTenManager.TOTAL_ROW_COUNT; j++)
			{
				NKCPopupMatchTenSlot component = m_BoardSlots[i, j].GetComponent<NKCPopupMatchTenSlot>();
				if (component == null || !component.m_objNum.activeInHierarchy)
				{
					continue;
				}
				NKCDebugUtil.DebugDrawCircle(m_DragSelection.targetRect, component.transform.localPosition, 10f, Color.red);
				if (IsVectorContainsSlot(minPos, maxPos, component.transform))
				{
					component.SetSelected(bValue: true);
					int col = component.GetCol();
					if (num > col)
					{
						num = col;
					}
					if (num2 < col)
					{
						num2 = col;
					}
					int row = component.GetRow();
					if (num3 > row)
					{
						num3 = row;
					}
					if (num4 < row)
					{
						num4 = row;
					}
					if (component.GetNumber() > 0)
					{
						m_lstSelectedSlot.Add(component);
					}
				}
				else
				{
					component.SetSelected(bValue: false);
				}
			}
		}
		if (!bDragEnd || m_bPause)
		{
			return;
		}
		for (int k = 0; k < m_lstSelectedSlot.Count; k++)
		{
			m_lstSelectedSlot[k].SetSelected(bValue: false);
		}
		if (NKCMatchTenManager.GetSum(num, num3, num2, num4, out var _) == NKCMatchTenManager.GetTargetNum())
		{
			NKCMatchTenManager.RemoveMatched(num, num3, num2, num4);
			AddScore(m_lstSelectedSlot.Count);
			for (int l = 0; l < m_lstSelectedSlot.Count; l++)
			{
				NKCPopupMatchTenSlot nKCPopupMatchTenSlot = m_lstSelectedSlot[l];
				if (nKCPopupMatchTenSlot != null)
				{
					nKCPopupMatchTenSlot.SetNumberOff();
				}
			}
		}
		if (GetScore() >= NKCMatchTenManager.GetPerfectScoreValue() - 1)
		{
			Log.Info($"\ufffd\u05b0\ufffd\ufffd\ufffd \ufffd\u07bc\ufffd - {GetScore()} / {NKCMatchTenManager.GetPerfectScoreValue()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Module/NKCPopupMatchTen.cs", 272);
			SetScore(NKCMatchTenManager.GetPerfectScoreValue());
			m_Stopwatch.Stop();
			m_StopwatchPause.Start();
			m_StopwatchTotal.Stop();
			m_bPause = true;
			CheckDataCorrect();
			m_dOnGameEnd?.Invoke(GetScore(), GetRemainTimeInt());
		}
		else if (NKCMatchTenManager.IsNeedReroll())
		{
			m_Stopwatch.Stop();
			m_StopwatchPause.Start();
			m_bPause = true;
			NKCUtil.SetGameobjectActive(m_objReroll, bValue: true);
		}
	}

	private bool IsVectorContainsSlot(Vector2 minPos, Vector2 maxPos, Transform trSlot)
	{
		if (minPos.x > trSlot.transform.localPosition.x)
		{
			return false;
		}
		if (minPos.y > trSlot.transform.localPosition.y)
		{
			return false;
		}
		if (maxPos.x < trSlot.transform.localPosition.x)
		{
			return false;
		}
		if (maxPos.y < trSlot.transform.localPosition.y)
		{
			return false;
		}
		return true;
	}

	private void AddScore(int addScore)
	{
		SetScore(GetScore() + addScore);
		CheckDataCorrect();
	}

	private void CheckDataCorrect()
	{
		if (Math.Abs(NKCMatchTenManager.GetZeroCount() - GetScore()) > 1)
		{
			m_bPause = true;
			Log.Error($"Score changed? m_Score : {GetScore()}, ZeroCount = {NKCMatchTenManager.GetZeroCount()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Module/NKCPopupMatchTen.cs", 326);
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_NEC_FAIL_MATCHTEN_INVALID_DATA, delegate
			{
				Application.Quit();
			});
		}
		if (m_StopwatchTotal.ElapsedMilliseconds - m_Stopwatch.ElapsedMilliseconds - m_StopwatchPause.ElapsedMilliseconds > 1000)
		{
			m_bPause = true;
			Log.Error($"Time Changed ? {m_Stopwatch.ElapsedMilliseconds} + {m_StopwatchPause.ElapsedMilliseconds} != {m_StopwatchTotal.ElapsedMilliseconds}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Module/NKCPopupMatchTen.cs", 333);
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_NEC_FAIL_MATCHTEN_INVALID_DATA, delegate
			{
				Close();
			});
		}
	}

	public void OnClickBackButton()
	{
		if (!m_objReroll.activeInHierarchy)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_POPUP_MATCHTEN_HOME_DESC, Close);
		}
	}

	private void OnConfirmReroll()
	{
		NKCMatchTenManager.SetBoard(bIsReroll: true);
		SetBoardData();
		NKCUtil.SetGameobjectActive(m_objReroll, bValue: false);
		StartGame(bIsReroll: true);
	}

	private void Update()
	{
		if (m_bPause)
		{
			return;
		}
		if (m_Stopwatch.ElapsedMilliseconds >= m_TotalSecond * 1000)
		{
			m_Stopwatch.Stop();
			m_StopwatchTotal.Stop();
			m_bPause = true;
			CheckDataCorrect();
			m_dOnGameEnd?.Invoke(GetScore(), GetRemainTimeInt());
		}
		else if (GetScore() >= NKCMatchTenManager.GetPerfectScoreValue() - 1)
		{
			m_Stopwatch.Stop();
			m_StopwatchTotal.Stop();
			m_bPause = true;
			CheckDataCorrect();
			m_dOnGameEnd?.Invoke(GetScore(), GetRemainTimeInt());
		}
		else
		{
			m_deltaTime += Time.deltaTime;
			if (m_deltaTime > 0.01f)
			{
				m_deltaTime -= 0.01f;
				SetTimeLabel();
			}
		}
	}

	private void SetTimeLabel()
	{
		if ((float)GetRemainTimeInt() < 1000f)
		{
			NKCUtil.SetLabelTextColor(m_lbRemainTime, Color.red);
		}
		else
		{
			NKCUtil.SetLabelTextColor(m_lbRemainTime, NKCUtil.GetColor("#FFB92D"));
		}
		NKCUtil.SetLabelText(m_lbRemainTime, ((float)GetRemainTimeInt() / 100f).ToString("F2"));
	}

	private IEnumerator WaitForCountdown()
	{
		m_bPause = true;
		m_Ani.SetTrigger("Countdown");
		yield return null;
		while (m_Ani.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			yield return null;
		}
		m_bPause = false;
		SetStartTime();
	}

	private void SetStartTime()
	{
		m_Stopwatch.Start();
		m_StopwatchTotal.Start();
		m_StopwatchPause.Stop();
	}

	public void SetBlind()
	{
		m_bPause = true;
		NKCUtil.SetGameobjectActive(m_objCountdown, bValue: true);
		m_imgCountdownNum.color = new Color(m_imgCountdownNum.color.r, m_imgCountdownNum.color.g, m_imgCountdownNum.color.b, 0f);
	}

	private int GetRemainTimeInt()
	{
		int num = (int)(m_TotalSecond * 1000 - m_Stopwatch.ElapsedMilliseconds) / 10;
		if (num < 0)
		{
			num = 0;
		}
		return num;
	}
}
