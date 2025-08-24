using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIEpisodeActSlotCC : MonoBehaviour
{
	public delegate void OnSelectedItemSlot(int actID);

	public NKCUIComButton m_comBtn;

	public Image m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_BG;

	public Image m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_UNIT;

	public GameObject m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_NODATA;

	public GameObject m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_COMPLETE;

	public GameObject m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_PROGRESS;

	public Text m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_NICKNAME;

	public Text m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_NAME;

	public GameObject m_objRedDot;

	public List<GameObject> m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_PROGRESS_BAR;

	private OnSelectedItemSlot m_OnSelectedSlot;

	private int m_ActID;

	private int m_UnitID;

	private NKCAssetInstanceData m_NKCAssetInstanceData;

	public int UnitID => m_UnitID;

	private void OnDestroy()
	{
		NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceData);
	}

	public static NKCUIEpisodeActSlotCC GetNewInstance(Transform parent, OnSelectedItemSlot selectedSlot = null)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_COUNTER_CASE", "NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT");
		NKCUIEpisodeActSlotCC component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIEpisodeActSlotCC>();
		if (component == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUIEpisodeActSlotCC Prefab null!");
			return null;
		}
		component.m_NKCAssetInstanceData = nKCAssetInstanceData;
		component.SetOnSelectedItemSlot(selectedSlot);
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.m_comBtn.PointerClick.RemoveAllListeners();
		component.m_comBtn.PointerClick.AddListener(component.OnSelectedItemSlotImpl);
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void SetOnSelectedItemSlot(OnSelectedItemSlot selectedSlot)
	{
		if (selectedSlot != null)
		{
			m_OnSelectedSlot = selectedSlot;
		}
	}

	private void OnSelectedItemSlotImpl()
	{
		if (m_OnSelectedSlot != null)
		{
			m_OnSelectedSlot(m_ActID);
			NKCContentManager.RemoveUnlockedCounterCaseKey(m_ActID);
		}
	}

	private bool IsComplete(NKMEpisodeTempletV2 cNKMEpisodeTemplet, int actID)
	{
		if (cNKMEpisodeTemplet == null)
		{
			return false;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return false;
		}
		if (cNKMEpisodeTemplet.m_DicStage.ContainsKey(actID))
		{
			for (int i = 0; i < cNKMEpisodeTemplet.m_DicStage[actID].Count; i++)
			{
				if (!NKMEpisodeMgr.CheckClear(myUserData, cNKMEpisodeTemplet.m_DicStage[actID][i]))
				{
					return false;
				}
			}
		}
		return true;
	}

	private void UpdateProgressBarUI(NKMEpisodeTempletV2 cNKMEpisodeTemplet, int actID)
	{
		if (cNKMEpisodeTemplet == null)
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null || !cNKMEpisodeTemplet.m_DicStage.ContainsKey(actID))
		{
			return;
		}
		for (int i = 0; i < m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_PROGRESS_BAR.Count; i++)
		{
			if (i < cNKMEpisodeTemplet.m_DicStage[actID].Count)
			{
				NKCUtil.SetImageColor(m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_PROGRESS_BAR[i].GetComponent<Image>(), Color.white);
				if (NKMEpisodeMgr.CheckClear(myUserData, cNKMEpisodeTemplet.m_DicStage[actID][i]))
				{
					NKCUtil.SetGameobjectActive(m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_PROGRESS_BAR[i], bValue: true);
					continue;
				}
				if (PlayerPrefs.HasKey(NKCContentManager.GetCounterCaseNormalKey(actID)))
				{
					NKCUtil.SetGameobjectActive(m_objRedDot, bValue: true);
				}
				NKCUtil.SetGameobjectActive(m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_PROGRESS_BAR[i], bValue: false);
			}
			else
			{
				NKCUtil.SetImageColor(m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_PROGRESS_BAR[i].GetComponent<Image>(), Color.black);
				NKCUtil.SetGameobjectActive(m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_PROGRESS_BAR[i], bValue: true);
			}
		}
	}

	public void SetData(NKMEpisodeTempletV2 cNKMEpisodeTemplet, int actID)
	{
		if (cNKMEpisodeTemplet == null)
		{
			return;
		}
		m_ActID = actID;
		m_UnitID = NKMEpisodeMgr.GetUnitID(cNKMEpisodeTemplet, actID);
		bool flag = NKMEpisodeMgr.CheckLockCounterCase(NKCScenManager.GetScenManager().GetMyUserData(), cNKMEpisodeTemplet, actID);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitID);
		if (unitTempletBase == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_PROGRESS, !flag);
		NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
		m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_UNIT.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UNIT_FACE_CARD", unitTempletBase.m_FaceCardName);
		if (!flag)
		{
			m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_NICKNAME.text = unitTempletBase.GetUnitTitle();
			m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_NAME.text = unitTempletBase.GetUnitName();
			m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_UNIT.color = new Color(1f, 1f, 1f, 1f);
			bool flag2 = IsComplete(cNKMEpisodeTemplet, actID);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_COMPLETE, flag2);
			if (flag2)
			{
				m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_BG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COUNTER_CASE_SPRITE", "AB_UI_NKM_UI_COUNTER_CASE_UNIT_SLOT_BG_COMPLETE");
			}
			else
			{
				m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_BG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COUNTER_CASE_SPRITE", "AB_UI_NKM_UI_COUNTER_CASE_UNIT_SLOT_BG");
			}
			UpdateProgressBarUI(cNKMEpisodeTemplet, actID);
		}
		else
		{
			m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_BG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_COUNTER_CASE_SPRITE", "AB_UI_NKM_UI_COUNTER_CASE_UNIT_SLOT_BG");
			m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_NICKNAME.text = "";
			m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_NAME.text = "";
			m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_UNIT.color = new Color(0f, 0f, 0f, 1f);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_COMPLETE, bValue: false);
		}
		m_comBtn.enabled = !flag;
		if (m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_NODATA.activeSelf == !flag)
		{
			m_NKM_UI_COUNTER_CASE_UNIT_LIST_SLOT_NODATA.SetActive(flag);
		}
		base.gameObject.SetActive(value: true);
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
}
