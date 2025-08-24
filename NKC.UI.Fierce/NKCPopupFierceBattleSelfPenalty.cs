using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI.Fierce;

public class NKCPopupFierceBattleSelfPenalty : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_fierce_battle";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_FIERCE_BATTLE_PEN";

	private static NKCPopupFierceBattleSelfPenalty m_Instance;

	[Header("Left Menu")]
	public Image m_imgBoss;

	public Text m_lbBossLv;

	public Text m_lbBossName;

	public Text m_lbSelectDesc;

	public NKCUIComStateButton m_csbtnReset;

	public NKCUIComStateButton m_csbtnConfirm;

	public NKCUIComStateButton m_csbtnCancel;

	[Header("옵션")]
	public NKCUIFierceBattleSelfPenaltyContent m_pfbSlot;

	public RectTransform m_rtDebuffSlotsParents;

	[Header("결과")]
	public RectTransform m_rtResultSlotParent;

	public GameObject m_objNoneResult;

	public Text m_lbTotalPercent;

	public Text m_lbTotalDesc;

	public NKCUIFierceBattleSelfPenaltySumSlot m_pfbResultSlot;

	private List<NKCUIFierceBattleSelfPenaltyContent> m_lstPenaltyContests = new List<NKCUIFierceBattleSelfPenaltyContent>();

	private List<NKCUIFierceBattleSelfPenaltySumSlot> m_lstPenaltySum = new List<NKCUIFierceBattleSelfPenaltySumSlot>();

	private UnityAction m_dCallBack;

	public static NKCPopupFierceBattleSelfPenalty Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupFierceBattleSelfPenalty>("ab_ui_nkm_ui_fierce_battle", "NKM_UI_POPUP_FIERCE_BATTLE_PEN", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupFierceBattleSelfPenalty>();
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

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "FIERCE_BATTLE_SELF_PENALTY_POPUP";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void OnBackButton()
	{
	}

	private void Init()
	{
		NKCUtil.SetBindFunction(m_csbtnConfirm, OnClickConfirm);
		NKCUtil.SetBindFunction(m_csbtnReset, OnClickReset);
		NKCUtil.SetBindFunction(m_csbtnCancel, base.Close);
		NKCUtil.SetHotkey(m_csbtnConfirm, HotkeyEventType.Confirm);
		InitFiercePenaltyData();
	}

	private void InitFiercePenaltyData()
	{
		NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
		if (nKCFierceBattleSupportDataMgr == null)
		{
			return;
		}
		NKMFierceBossGroupTemplet nKMFierceBossGroupTemplet = NKMFierceBossGroupTemplet.Find(nKCFierceBattleSupportDataMgr.CurBossID);
		if (nKMFierceBossGroupTemplet == null)
		{
			return;
		}
		Dictionary<int, Dictionary<int, List<NKMFiercePenaltyTemplet>>> dictionary = new Dictionary<int, Dictionary<int, List<NKMFiercePenaltyTemplet>>>();
		foreach (KeyValuePair<int, NKMBattleConditionTemplet> item in NKMBattleConditionManager.Dic)
		{
			if (item.Value.UseContentsType != NKMBattleConditionTemplet.USE_CONTENT_TYPE.UCT_FIERCE_PENALTY)
			{
				continue;
			}
			foreach (NKMFiercePenaltyTemplet value in NKMTempletContainer<NKMFiercePenaltyTemplet>.Values)
			{
				if (value != null && nKMFierceBossGroupTemplet.BossPenaltyGroupID.Contains(value.BossPenaltyGroupID) && item.Value == value.battleCondition)
				{
					if (dictionary.ContainsKey(value.BossPenaltyGroupID))
					{
						Dictionary<int, List<NKMFiercePenaltyTemplet>> _dicSubGroup = dictionary[value.BossPenaltyGroupID];
						UpdateFiercePenaltySubGroup(ref _dicSubGroup, value);
					}
					else
					{
						Dictionary<int, List<NKMFiercePenaltyTemplet>> _dicSubGroup2 = new Dictionary<int, List<NKMFiercePenaltyTemplet>>();
						UpdateFiercePenaltySubGroup(ref _dicSubGroup2, value);
						dictionary.Add(value.BossPenaltyGroupID, _dicSubGroup2);
					}
				}
			}
		}
		foreach (KeyValuePair<int, Dictionary<int, List<NKMFiercePenaltyTemplet>>> item2 in dictionary)
		{
			foreach (KeyValuePair<int, List<NKMFiercePenaltyTemplet>> item3 in item2.Value)
			{
				NKCUIFierceBattleSelfPenaltyContent penaltyContentSlot = GetPenaltyContentSlot();
				if (penaltyContentSlot != null)
				{
					penaltyContentSlot.SetData(item3.Value, OnClickPenaltySlot);
					m_lstPenaltyContests.Add(penaltyContentSlot);
				}
			}
		}
	}

	private void UpdateFiercePenaltySubGroup(ref Dictionary<int, List<NKMFiercePenaltyTemplet>> _dicSubGroup, NKMFiercePenaltyTemplet penaltyTemplet)
	{
		if (_dicSubGroup.ContainsKey(penaltyTemplet.PenaltyGroupID))
		{
			NKMFiercePenaltyTemplet nKMFiercePenaltyTemplet = _dicSubGroup[penaltyTemplet.PenaltyGroupID].Find((NKMFiercePenaltyTemplet e) => e.battleCondition == penaltyTemplet.battleCondition);
			if (nKMFiercePenaltyTemplet == null)
			{
				_dicSubGroup[penaltyTemplet.PenaltyGroupID].Add(penaltyTemplet);
			}
		}
		else
		{
			_dicSubGroup.Add(penaltyTemplet.PenaltyGroupID, new List<NKMFiercePenaltyTemplet> { penaltyTemplet });
		}
	}

	private NKCUIFierceBattleSelfPenaltyContent GetPenaltyContentSlot()
	{
		NKCUIFierceBattleSelfPenaltyContent nKCUIFierceBattleSelfPenaltyContent = Object.Instantiate(m_pfbSlot);
		if (null != nKCUIFierceBattleSelfPenaltyContent)
		{
			nKCUIFierceBattleSelfPenaltyContent.Init(m_rtDebuffSlotsParents);
		}
		return nKCUIFierceBattleSelfPenaltyContent;
	}

	public void OnClickPenaltySlot(NKMFiercePenaltyTemplet penaltyTemplet)
	{
		if (penaltyTemplet == null)
		{
			return;
		}
		bool flag = false;
		if (m_lstPenaltySum.Count > 0)
		{
			for (int i = 0; i < m_lstPenaltySum.Count; i++)
			{
				if (m_lstPenaltySum[i].PenaltyTemplet.Key == penaltyTemplet.Key)
				{
					Object.Destroy(m_lstPenaltySum[i].gameObject);
					m_lstPenaltySum[i] = null;
					m_lstPenaltySum.RemoveAt(i);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				for (int j = 0; j < m_lstPenaltySum.Count; j++)
				{
					if (m_lstPenaltySum[j].PenaltyTemplet.PenaltyGroupID == penaltyTemplet.PenaltyGroupID)
					{
						m_lstPenaltySum[j].SetData(penaltyTemplet);
						flag = true;
						break;
					}
				}
			}
		}
		if (!flag)
		{
			NKCUIFierceBattleSelfPenaltySumSlot nKCUIFierceBattleSelfPenaltySumSlot = AddPenaltySumSlot(penaltyTemplet);
			if (null != nKCUIFierceBattleSelfPenaltySumSlot)
			{
				m_lstPenaltySum.Add(nKCUIFierceBattleSelfPenaltySumSlot);
			}
		}
		UpdateRightUI();
	}

	private void ClearCurResultSlots()
	{
		for (int i = 0; i < m_lstPenaltySum.Count; i++)
		{
			if (null != m_lstPenaltySum[i])
			{
				Object.Destroy(m_lstPenaltySum[i].gameObject);
				m_lstPenaltySum[i] = null;
			}
		}
		m_lstPenaltySum.Clear();
	}

	private NKCUIFierceBattleSelfPenaltySumSlot AddPenaltySumSlot(NKMFiercePenaltyTemplet penaltyTemplet)
	{
		if (penaltyTemplet != null)
		{
			NKCUIFierceBattleSelfPenaltySumSlot nKCUIFierceBattleSelfPenaltySumSlot = Object.Instantiate(m_pfbResultSlot);
			if (null != nKCUIFierceBattleSelfPenaltySumSlot)
			{
				nKCUIFierceBattleSelfPenaltySumSlot.SetData(penaltyTemplet);
				nKCUIFierceBattleSelfPenaltySumSlot.transform.SetParent(m_rtResultSlotParent);
				return nKCUIFierceBattleSelfPenaltySumSlot;
			}
		}
		Debug.LogError("<color=red>Fail - NKCPopupFierceBattleSelfPenalty::AddPenaltySumSlot</color>");
		return null;
	}

	private void UpdateRightUI()
	{
		float num = 0f;
		foreach (NKCUIFierceBattleSelfPenaltySumSlot item in m_lstPenaltySum)
		{
			num += item.PenaltyTemplet.FierceScoreRate;
		}
		NKCUtil.SetGameobjectActive(m_lbTotalDesc.gameObject, num != 0f);
		num *= 0.01f;
		if (num >= 0f)
		{
			NKCUtil.SetLabelText(m_lbTotalDesc, NKCUtilString.GET_STRING_FIERCE_PENALTY_BUFF_POINT);
			NKCUtil.SetLabelText(m_lbTotalPercent, string.Format(NKCUtilString.GET_STRING_FIERCE_PENALTY_SCORE_PLUS, num));
		}
		else
		{
			num *= -1f;
			NKCUtil.SetLabelText(m_lbTotalPercent, string.Format(NKCUtilString.GET_STRING_FIERCE_PENALTY_SCORE_MINUS, num));
			NKCUtil.SetLabelText(m_lbTotalDesc, NKCUtilString.GET_STRING_FIERCE_PENALTY_DEBUFF_POINT);
		}
		NKCUtil.SetGameobjectActive(m_objNoneResult, m_lstPenaltySum.Count <= 0);
	}

	public void Open(int FierceID, UnityAction callback)
	{
		m_dCallBack = callback;
		UpdateUIWhenOpend();
		UpdateUI();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (!m_Instance.IsOpen)
		{
			UIOpened();
		}
	}

	private void UpdateUIWhenOpend()
	{
		List<NKMFiercePenaltyTemplet> list = LoadSelfPenaltyData();
		if (list != null)
		{
			foreach (NKCUIFierceBattleSelfPenaltyContent lstPenaltyContest in m_lstPenaltyContests)
			{
				lstPenaltyContest.UnCheckChildSlots();
				foreach (NKMFiercePenaltyTemplet item in list)
				{
					if (lstPenaltyContest.PenaltyGroupID == item.PenaltyGroupID)
					{
						lstPenaltyContest.SelectChildSlot(item.Key);
					}
				}
			}
			ClearCurResultSlots();
			foreach (NKMFiercePenaltyTemplet item2 in list)
			{
				NKCUIFierceBattleSelfPenaltySumSlot nKCUIFierceBattleSelfPenaltySumSlot = AddPenaltySumSlot(item2);
				if (null != nKCUIFierceBattleSelfPenaltySumSlot)
				{
					m_lstPenaltySum.Add(nKCUIFierceBattleSelfPenaltySumSlot);
				}
			}
		}
		UpdateBossData();
	}

	private void UpdateBossData()
	{
		NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
		if (nKCFierceBattleSupportDataMgr == null)
		{
			return;
		}
		int curBossGroupID = nKCFierceBattleSupportDataMgr.CurBossGroupID;
		int curSelectedBossLv = nKCFierceBattleSupportDataMgr.GetCurSelectedBossLv();
		if (NKMFierceBossGroupTemplet.Groups.ContainsKey(curBossGroupID))
		{
			foreach (NKMFierceBossGroupTemplet item in NKMFierceBossGroupTemplet.Groups[curBossGroupID])
			{
				if (item.Level == curSelectedBossLv)
				{
					Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_fierce_battle_boss_thumbnail", item.UI_BossFaceSlot);
					NKCUtil.SetImageSprite(m_imgBoss, orLoadAssetResource);
					break;
				}
			}
		}
		NKCUtil.SetLabelText(m_lbBossName, nKCFierceBattleSupportDataMgr.GetCurBossName());
		string curSelectedBossLvDesc = nKCFierceBattleSupportDataMgr.GetCurSelectedBossLvDesc(curBossGroupID);
		NKCUtil.SetLabelText(m_lbBossLv, curSelectedBossLvDesc);
	}

	private List<NKMFiercePenaltyTemplet> LoadSelfPenaltyData()
	{
		List<NKMFiercePenaltyTemplet> list = new List<NKMFiercePenaltyTemplet>();
		List<int> selfPenalty = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr().GetSelfPenalty();
		if (selfPenalty != null && selfPenalty.Count > 0)
		{
			foreach (int item in selfPenalty)
			{
				NKMFiercePenaltyTemplet nKMFiercePenaltyTemplet = NKMTempletContainer<NKMFiercePenaltyTemplet>.Find(item);
				if (nKMFiercePenaltyTemplet != null)
				{
					list.Add(nKMFiercePenaltyTemplet);
				}
			}
		}
		return list;
	}

	private void UnSelectAllSotUI()
	{
		foreach (NKCUIFierceBattleSelfPenaltyContent lstPenaltyContest in m_lstPenaltyContests)
		{
			lstPenaltyContest.UnCheckChildSlots();
		}
		ClearCurResultSlots();
	}

	private void UpdateUI()
	{
		NKCUtil.SetLabelText(m_lbSelectDesc, NKCUtilString.GET_STRING_FIERCE_POPUP_SELF_PENALTY_DEBUFF);
		UpdateSlotUI();
		UpdateRightUI();
	}

	private void UpdateSlotUI()
	{
		foreach (NKCUIFierceBattleSelfPenaltySumSlot item in m_lstPenaltySum)
		{
			NKCUtil.SetGameobjectActive(item.gameObject, bValue: true);
		}
	}

	private void OnClickConfirm()
	{
		SavePenaltyData();
		m_dCallBack?.Invoke();
		Close();
	}

	private void SavePenaltyData()
	{
		List<int> list = new List<int>();
		foreach (NKCUIFierceBattleSelfPenaltySumSlot item in m_lstPenaltySum)
		{
			list.Add(item.PenaltyTemplet.Key);
		}
		NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr()?.SetSelfPenalty(list);
	}

	private void OnClickReset()
	{
		if (m_lstPenaltyContests != null)
		{
			foreach (NKCUIFierceBattleSelfPenaltyContent lstPenaltyContest in m_lstPenaltyContests)
			{
				lstPenaltyContest.UnCheckChildSlots();
			}
		}
		ClearCurResultSlots();
		UpdateRightUI();
	}
}
