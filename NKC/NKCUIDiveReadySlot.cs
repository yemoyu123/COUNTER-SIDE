using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIDiveReadySlot : MonoBehaviour
{
	public delegate void OnSelectedDiveReadySlot(NKCUIDiveReadySlot cNKCUIDiveReadySlot);

	public NKCUIComButton m_NKM_UI_DIVE_SLOT;

	public Text m_NKM_UI_DIVE_SLOT_TITLE_TEXT;

	public Text m_NKM_UI_DIVE_SLOT_SUBTITLE_TEXT;

	public GameObject m_NKM_UI_DIVE_SLOT_NEW_TEXT;

	public Image m_NKM_UI_DIVE_SLOT_LINE_PREV;

	public Image m_NKM_UI_DIVE_SLOT_LINE_NEXT;

	[Header("일반")]
	public GameObject m_NKM_UI_DIVE_SLOT_LOCKED;

	public GameObject m_NKM_UI_DIVE_SLOT_SELECTED;

	public GameObject m_NKM_UI_DIVE_SLOT_SELECTED_BUT_LOCKED;

	[Header("하드")]
	public GameObject m_NKM_UI_DIVE_SLOT_SELECTED_HURDLE;

	public GameObject m_NKM_UI_DIVE_SLOT_SELECTED_HURDLE_BUT_LOCKED;

	[Header("스케빈저")]
	public GameObject m_NKM_UI_DIVE_SLOT_SELECTED_SCAVENGER;

	public GameObject m_NKM_UI_DIVE_SLOT_SELECTED_SCAVENGER_BUT_LOCKED;

	[Header("")]
	public GameObject m_NKM_UI_DIVE_SLOT_CLEARED;

	public GameObject m_NKM_UI_DIVE_SLOT_HURDLE;

	public GameObject m_NKM_UI_DIVE_SLOT_SCAVENGER;

	public GameObject m_NKM_UI_DIVE_SLOT_DIVE_ING;

	public Animator m_NKM_UI_DIVE_SLOT_SHOW_FX;

	private OnSelectedDiveReadySlot m_OnSelectedDiveReadySlot;

	private NKCAssetInstanceData m_InstanceData;

	private int m_Index = -1;

	private NKMDiveTemplet m_NKMDiveTemplet;

	private int m_cityID = -1;

	public int GetIndex()
	{
		return m_Index;
	}

	public NKMDiveTemplet GetNKMDiveTemplet()
	{
		return m_NKMDiveTemplet;
	}

	public static NKCUIDiveReadySlot GetNewInstance(Transform parent, OnSelectedDiveReadySlot _OnSelectedDiveReadySlot = null)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WORLD_MAP_DIVE", "NKM_UI_DIVE_SLOT");
		NKCUIDiveReadySlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIDiveReadySlot>();
		if (component == null)
		{
			Debug.LogError("NKM_UI_DIVE_SLOT Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localScale = new Vector3(1f, 1f, 1f);
		component.m_OnSelectedDiveReadySlot = _OnSelectedDiveReadySlot;
		component.m_NKM_UI_DIVE_SLOT.PointerClick.RemoveAllListeners();
		component.m_NKM_UI_DIVE_SLOT.PointerClick.AddListener(component.OnClicked);
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void PlayScrollArriveEffect()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_SHOW_FX.gameObject, bValue: true);
		m_NKM_UI_DIVE_SLOT_SHOW_FX.Play("NKM_UI_DIVE_SLOT_SHOW_FX");
	}

	private void OnDestroy()
	{
		DestoryInstance(bPureDestory: true);
	}

	public void DestoryInstance(bool bPureDestory = false)
	{
		if (m_InstanceData != null)
		{
			NKCAssetResourceManager.CloseInstance(m_InstanceData);
		}
		m_InstanceData = null;
		if (!bPureDestory)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnClicked()
	{
		if (m_OnSelectedDiveReadySlot != null)
		{
			m_OnSelectedDiveReadySlot(this);
		}
	}

	public void SetUI(int index, NKMDiveTemplet cNKMDiveTemplet, int cityID = -1)
	{
		if (cNKMDiveTemplet == null)
		{
			return;
		}
		m_cityID = cityID;
		m_NKMDiveTemplet = cNKMDiveTemplet;
		m_Index = index;
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_HURDLE, cNKMDiveTemplet.StageType == NKM_DIVE_STAGE_TYPE.NDST_HARD);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_SCAVENGER, cNKMDiveTemplet.StageType == NKM_DIVE_STAGE_TYPE.NDST_SCAVENGER);
		SetSelected(bSet: false);
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		bool flag = NKMContentUnlockManager.IsContentUnlocked(myUserData, new UnlockInfo(cNKMDiveTemplet.StageUnlockReqType, cNKMDiveTemplet.StageUnlockReqValue));
		SetLocked(!flag);
		if (myUserData.m_DiveClearData != null)
		{
			SetCleared(myUserData.m_DiveClearData.Contains(cNKMDiveTemplet.StageID));
		}
		else
		{
			SetCleared(bSet: false);
		}
		if (cNKMDiveTemplet.IsEventDive)
		{
			m_NKM_UI_DIVE_SLOT_LINE_PREV.enabled = false;
			m_NKM_UI_DIVE_SLOT_LINE_NEXT.enabled = false;
		}
		else
		{
			if (cNKMDiveTemplet.IndexID == NKCDiveManager.BeginIndex)
			{
				m_NKM_UI_DIVE_SLOT_LINE_PREV.enabled = false;
			}
			else
			{
				m_NKM_UI_DIVE_SLOT_LINE_PREV.enabled = true;
				if (flag)
				{
					m_NKM_UI_DIVE_SLOT_LINE_PREV.color = new Color(m_NKM_UI_DIVE_SLOT_LINE_PREV.color.r, m_NKM_UI_DIVE_SLOT_LINE_PREV.color.g, m_NKM_UI_DIVE_SLOT_LINE_PREV.color.b, 1f);
				}
				else
				{
					m_NKM_UI_DIVE_SLOT_LINE_PREV.color = new Color(m_NKM_UI_DIVE_SLOT_LINE_PREV.color.r, m_NKM_UI_DIVE_SLOT_LINE_PREV.color.g, m_NKM_UI_DIVE_SLOT_LINE_PREV.color.b, 0.3f);
				}
			}
			if (cNKMDiveTemplet.IndexID == NKCDiveManager.EndIndex)
			{
				m_NKM_UI_DIVE_SLOT_LINE_NEXT.enabled = false;
			}
			else
			{
				m_NKM_UI_DIVE_SLOT_LINE_NEXT.enabled = true;
				NKMDiveTemplet templetByUnlockData = NKCDiveManager.GetTempletByUnlockData(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DIVE, cNKMDiveTemplet.StageID);
				if (templetByUnlockData != null)
				{
					if (NKMContentUnlockManager.IsContentUnlocked(myUserData, new UnlockInfo(templetByUnlockData.StageUnlockReqType, templetByUnlockData.StageUnlockReqValue)))
					{
						m_NKM_UI_DIVE_SLOT_LINE_NEXT.color = new Color(m_NKM_UI_DIVE_SLOT_LINE_NEXT.color.r, m_NKM_UI_DIVE_SLOT_LINE_NEXT.color.g, m_NKM_UI_DIVE_SLOT_LINE_NEXT.color.b, 1f);
					}
					else
					{
						m_NKM_UI_DIVE_SLOT_LINE_NEXT.color = new Color(m_NKM_UI_DIVE_SLOT_LINE_NEXT.color.r, m_NKM_UI_DIVE_SLOT_LINE_NEXT.color.g, m_NKM_UI_DIVE_SLOT_LINE_NEXT.color.b, 0.3f);
					}
				}
			}
		}
		m_NKM_UI_DIVE_SLOT_TITLE_TEXT.text = cNKMDiveTemplet.Get_STAGE_NAME();
		m_NKM_UI_DIVE_SLOT_SUBTITLE_TEXT.text = cNKMDiveTemplet.Get_STAGE_NAME_SUB();
		bool bValue = false;
		if (myUserData.m_DiveGameData != null && myUserData.m_DiveGameData.Floor.Templet.StageID == m_NKMDiveTemplet.StageID)
		{
			if (cNKMDiveTemplet.IsEventDive)
			{
				if (NKCScenManager.CurrentUserData().m_WorldmapData.GetCityIDByEventData(NKM_WORLDMAP_EVENT_TYPE.WET_DIVE, myUserData.m_DiveGameData.DiveUid) == m_cityID)
				{
					bValue = true;
				}
			}
			else
			{
				bValue = true;
			}
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_DIVE_ING, bValue);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_SHOW_FX.gameObject, bValue: false);
	}

	public bool IsSelected()
	{
		if (m_NKM_UI_DIVE_SLOT_SELECTED.activeSelf || m_NKM_UI_DIVE_SLOT_SELECTED_HURDLE.activeSelf || m_NKM_UI_DIVE_SLOT_SELECTED_SCAVENGER.activeSelf)
		{
			return true;
		}
		return false;
	}

	public void SetSelected(bool bSet)
	{
		if (m_NKMDiveTemplet == null)
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData != null)
		{
			bool flag = NKMContentUnlockManager.IsContentUnlocked(myUserData, new UnlockInfo(m_NKMDiveTemplet.StageUnlockReqType, m_NKMDiveTemplet.StageUnlockReqValue));
			switch (m_NKMDiveTemplet.StageType)
			{
			default:
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_SELECTED, bSet);
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_SELECTED_HURDLE, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_SELECTED_SCAVENGER, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_SELECTED_BUT_LOCKED, !flag);
				break;
			case NKM_DIVE_STAGE_TYPE.NDST_HARD:
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_SELECTED, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_SELECTED_HURDLE, bSet);
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_SELECTED_SCAVENGER, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_SELECTED_HURDLE_BUT_LOCKED, !flag);
				break;
			case NKM_DIVE_STAGE_TYPE.NDST_SCAVENGER:
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_SELECTED, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_SELECTED_HURDLE, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_SELECTED_SCAVENGER, bSet);
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_SELECTED_SCAVENGER_BUT_LOCKED, !flag);
				break;
			}
		}
	}

	public void SetLocked(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_LOCKED, bSet);
	}

	public void SetCleared(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_CLEARED, bSet);
		NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_SLOT_NEW_TEXT, !bSet);
	}
}
