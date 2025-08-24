using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUICCNormalSlot : MonoBehaviour
{
	public delegate void OnSelectedCCSlot(int stageIndex, string stageBattleStrID);

	public Text m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_TITLE;

	public Text m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_TEXT;

	public Text m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_NUMBER;

	public GameObject m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_NEW;

	public GameObject m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_RESOURCE;

	public Text m_RESOURCE_TEXT;

	public GameObject m_AB_ICON_SLOT;

	public NKCUISlot m_NKCUISlot;

	public GameObject m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_BUTTON_01;

	public GameObject m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_BUTTON_02;

	public NKCUIComButton m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_BUTTON_02_Btn;

	public Text m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_BUTTON_TEXT;

	public GameObject m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_LOCK;

	public Text m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_LOCK_TEXT;

	public CanvasGroup m_CanvasGroup;

	public DOTweenAnimation m_DOTweenAnimation;

	private float m_fAlphaAniStartTime = float.MaxValue;

	private OnSelectedCCSlot m_OnSelectedSlot;

	private int m_StageIndex;

	private string m_StageBattleStrID = "";

	private bool m_bLock;

	private NKCAssetInstanceData m_NKCAssetInstanceData;

	public void SetOnSelectedItemSlot(OnSelectedCCSlot _OnSelectedSlot)
	{
		m_OnSelectedSlot = _OnSelectedSlot;
	}

	public int GetStageIndex()
	{
		return m_StageIndex;
	}

	public bool GetLock()
	{
		return m_bLock;
	}

	private void OnDestroy()
	{
		NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceData);
	}

	public static NKCUICCNormalSlot GetNewInstance(Transform parent, OnSelectedCCSlot dOnSelectedItemSlot)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_COUNTER_CASE", "NKM_UI_COUNTER_CASE_NORMAL_SLOT");
		NKCUICCNormalSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUICCNormalSlot>();
		if (component == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUICCNormalSlot Prefab null!");
			return null;
		}
		component.m_NKCAssetInstanceData = nKCAssetInstanceData;
		component.SetOnSelectedItemSlot(dOnSelectedItemSlot);
		component.m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_BUTTON_02_Btn.PointerClick.RemoveAllListeners();
		component.m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_BUTTON_02_Btn.PointerClick.AddListener(component.OnSelectedSlotImpl);
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.m_NKCUISlot.Init();
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void OnSelectedSlotImpl()
	{
		if (m_OnSelectedSlot != null)
		{
			m_OnSelectedSlot(m_StageIndex, m_StageBattleStrID);
		}
	}

	public void SetData(NKMStageTempletV2 stageTemplet, int dungeonIDForBtnAni = -1)
	{
		if (stageTemplet == null)
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		m_bLock = !NKMEpisodeMgr.CheckEpisodeMission(myUserData, stageTemplet) && !NKMEpisodeMgr.CheckClear(myUserData, stageTemplet);
		m_StageIndex = stageTemplet.m_StageIndex;
		m_StageBattleStrID = stageTemplet.m_StageBattleStrID;
		base.gameObject.SetActive(value: true);
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(m_StageBattleStrID);
		if (dungeonTempletBase == null)
		{
			return;
		}
		m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_TITLE.text = dungeonTempletBase.GetDungeonName();
		m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_NUMBER.text = stageTemplet.m_StageUINum.ToString();
		NKCUtil.SetGameobjectActive(m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_LOCK, m_bLock);
		NKCUtil.SetGameobjectActive(m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_BUTTON_01, m_bLock);
		NKCUtil.SetGameobjectActive(m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_BUTTON_02, !m_bLock);
		m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_BUTTON_TEXT.text = NKCUtilString.GET_STRING_COUNTER_CASE_SLOT_BUTTON_LOCK;
		if (m_bLock)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_RESOURCE, bValue: false);
			NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT, bValue: false);
			m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_TEXT.text = "";
			m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_LOCK_TEXT.text = NKCUtilString.GetUnlockConditionRequireDesc(stageTemplet);
			return;
		}
		bool completeMark = NKMEpisodeMgr.CheckClear(myUserData, stageTemplet);
		bool flag = myUserData.CheckUnlockedCounterCase(dungeonTempletBase.m_DungeonID);
		NKCUtil.SetGameobjectActive(m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_RESOURCE, !flag);
		m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_TEXT.text = dungeonTempletBase.GetDungeonDesc();
		if (!flag)
		{
			m_RESOURCE_TEXT.text = ((stageTemplet.UnlockReqItem != null) ? stageTemplet.UnlockReqItem.Count32.ToString() : stageTemplet.m_StageReqItemCount.ToString());
		}
		else
		{
			m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_BUTTON_TEXT.text = NKCUtilString.GET_STRING_COUNTER_CASE_SLOT_BUTTON_UNLOCK;
			if (dungeonIDForBtnAni == dungeonTempletBase.m_DungeonID)
			{
				m_DOTweenAnimation.DORestart();
				NKCSoundManager.PlaySound("FX_UI_COUNTERCASE_DUNGEON_OPEN", 1f, 0f, 0f);
			}
		}
		FirstRewardData firstRewardData = dungeonTempletBase.GetFirstRewardData();
		if (firstRewardData.Type == NKM_REWARD_TYPE.RT_NONE)
		{
			NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_AB_ICON_SLOT, bValue: true);
		NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeRewardTypeData(firstRewardData.Type, firstRewardData.RewardId, firstRewardData.RewardQuantity);
		m_NKCUISlot.SetData(data);
		m_NKCUISlot.SetCompleteMark(completeMark);
	}

	public void SetAlphaAni(int index)
	{
		m_CanvasGroup.alpha = 0f;
		m_fAlphaAniStartTime = Time.time + (float)index * 0.125f;
	}

	private void Update()
	{
		if (m_fAlphaAniStartTime < Time.time && m_CanvasGroup.alpha < 1f)
		{
			m_CanvasGroup.alpha += Time.deltaTime * 2.5f;
			if (m_CanvasGroup.alpha >= 1f)
			{
				m_CanvasGroup.alpha = 1f;
				m_fAlphaAniStartTime = float.MaxValue;
			}
		}
	}

	public void SetActive(bool bSet)
	{
		if (base.gameObject.activeSelf == !bSet)
		{
			base.gameObject.SetActive(bSet);
		}
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}

	public RectTransform GetBtnRect()
	{
		return m_NKM_UI_COUNTER_CASE_NORMAL_SLOT_BUTTON_02_Btn.GetComponent<RectTransform>();
	}
}
