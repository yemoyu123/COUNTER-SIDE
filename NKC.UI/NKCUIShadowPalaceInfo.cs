using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIShadowPalaceInfo : MonoBehaviour
{
	public delegate void OnTouchStart();

	public delegate void OnTouchProgress();

	public delegate void OnTouchRank();

	[Header("INFO")]
	public Text m_txtNum;

	public Text m_txtName;

	public Text m_txtDesc;

	public Text m_txtLv;

	public Text m_txtBattleCnt;

	[Header("REWARD")]
	public Transform m_trReward;

	[Header("LOCK")]
	public GameObject m_objDenied;

	public GameObject m_objLockBtn;

	[Header("ANIMATION")]
	public Animator m_ani;

	[Header("BUTTON")]
	public NKCUIComStateButton m_btnRank;

	public NKCUIComStateButton m_btnProgress;

	public NKCUIComResourceButton m_btnStart;

	[Header("COST")]
	public Image m_imgCost;

	public Text m_txtCost;

	private OnTouchStart dOnTouchStart;

	private OnTouchProgress dOnTouchProgress;

	private OnTouchRank dOnTouchRank;

	private List<NKCUISlot> m_lstRewardSlot = new List<NKCUISlot>();

	public void Init(OnTouchStart onTouchStart, OnTouchProgress onTouchProgress, OnTouchRank onTouchRank)
	{
		m_btnRank?.PointerClick.RemoveAllListeners();
		m_btnRank?.PointerClick.AddListener(TouchRank);
		m_btnStart?.PointerClick.RemoveAllListeners();
		m_btnStart?.PointerClick.AddListener(TouchStart);
		m_btnStart?.SetHotkey(HotkeyEventType.Confirm);
		m_btnProgress?.PointerClick.RemoveAllListeners();
		m_btnProgress?.PointerClick.AddListener(TouchProgress);
		dOnTouchStart = onTouchStart;
		dOnTouchProgress = onTouchProgress;
		dOnTouchRank = onTouchRank;
		m_ani.Play("NKM_UI_SHADOW_INFO_INTRO_IDLE");
	}

	public void SetData(NKMShadowPalaceTemplet palaceTemplet, int currSkipCount, bool bCurrentPalace, bool bUnlock)
	{
		NKCUtil.SetLabelText(m_txtNum, string.Format(NKCUtilString.GET_SHADOW_PALACE_NUMBER, palaceTemplet.PALACE_NUM_UI));
		NKCUtil.SetLabelText(m_txtName, palaceTemplet.PalaceName);
		List<NKMShadowBattleTemplet> battleTemplets = NKMShadowPalaceManager.GetBattleTemplets(palaceTemplet.PALACE_ID);
		if (battleTemplets == null)
		{
			Debug.LogError($"ShadowBattleTemplet 찾을 수 없음 - palace#{palaceTemplet.PALACE_NUM_UI}");
			return;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(battleTemplets[battleTemplets.Count - 1].DUNGEON_ID);
		string msg = string.Empty;
		if (dungeonTempletBase != null)
		{
			msg = dungeonTempletBase.m_DungeonLevel.ToString();
		}
		NKCUtil.SetLabelText(m_txtLv, msg);
		NKCUtil.SetLabelText(m_txtBattleCnt, battleTemplets.Count.ToString());
		int count = m_lstRewardSlot.Count;
		int count2 = palaceTemplet.COMPLETE_REWARDS.Count;
		for (int i = count; i < count2; i++)
		{
			NKCUISlot newInstance = NKCUISlot.GetNewInstance(m_trReward);
			m_lstRewardSlot.Add(newInstance);
		}
		for (int j = 0; j < m_lstRewardSlot.Count; j++)
		{
			NKCUISlot nKCUISlot = m_lstRewardSlot[j];
			if (j < count2)
			{
				NKCUtil.SetGameobjectActive(nKCUISlot, bValue: true);
				NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(palaceTemplet.COMPLETE_REWARDS[j]);
				nKCUISlot.SetData(data);
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCUISlot, bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_btnStart, bUnlock && !bCurrentPalace);
		NKCUtil.SetGameobjectActive(m_btnProgress, bUnlock && bCurrentPalace);
		NKCUtil.SetGameobjectActive(m_objDenied, !bUnlock);
		NKCUtil.SetGameobjectActive(m_objLockBtn, !bUnlock);
		if (bUnlock)
		{
			NKCUtil.SetLabelText(m_txtDesc, palaceTemplet.PalaceDesc);
		}
		else
		{
			NKCUtil.SetLabelText(m_txtDesc, string.Empty);
		}
		int itemCount = palaceTemplet.STAGE_REQ_ITEM_COUNT * currSkipCount;
		m_btnStart.SetData(palaceTemplet.STAGE_REQ_ITEM_ID, itemCount);
	}

	private void TouchRank()
	{
		dOnTouchRank?.Invoke();
	}

	private void TouchStart()
	{
		dOnTouchStart?.Invoke();
	}

	private void TouchProgress()
	{
		dOnTouchProgress?.Invoke();
	}

	public void PlayIntroAni()
	{
		m_ani.Play("NKM_UI_SHADOW_INFO_INTRO");
	}
}
