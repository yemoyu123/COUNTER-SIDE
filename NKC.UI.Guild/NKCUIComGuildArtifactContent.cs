using System.Collections.Generic;
using System.Linq;
using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIComGuildArtifactContent : MonoBehaviour
{
	public enum GUILD_ARTIFACT_STORAGE_VIEW_TYPE
	{
		NONE,
		ALL,
		EACH,
		ARENA
	}

	public class ArtifactSlotData
	{
		public bool bIsArenaNum;

		public int id;

		public string iconName;

		public string name;

		public string desc;

		public static ArtifactSlotData MakeSlotData(int arenaNum)
		{
			return new ArtifactSlotData
			{
				bIsArenaNum = true,
				id = arenaNum
			};
		}

		public static ArtifactSlotData MakeSlotData(GuildDungeonArtifactTemplet templet)
		{
			return new ArtifactSlotData
			{
				bIsArenaNum = false,
				id = templet.GetArtifactId(),
				iconName = templet.GetIconName(),
				name = templet.GetName(),
				desc = templet.GetDescFull()
			};
		}
	}

	public NKCPopupGuildCoopArtifactStorageSlot m_pfbNormalSlot;

	public Animator m_Animator;

	public NKCUIComToggle m_tglEach;

	public NKCUIComToggle m_tglAll;

	public NKCUIComToggle m_tglArena;

	public LoopScrollRect m_loop;

	public Transform m_trParent;

	public Text m_lbAll;

	public GameObject m_objNone;

	private Stack<NKCPopupGuildCoopArtifactStorageSlot> m_stkSlot = new Stack<NKCPopupGuildCoopArtifactStorageSlot>();

	private List<ArtifactSlotData> m_lstSlotDataByArena = new List<ArtifactSlotData>();

	private List<ArtifactSlotData> m_lstSlotDataByID = new List<ArtifactSlotData>();

	private GUILD_ARTIFACT_STORAGE_VIEW_TYPE m_CurViewType;

	private GUILD_ARTIFACT_STORAGE_VIEW_TYPE m_LastViewType;

	public void Init()
	{
		if (m_loop != null)
		{
			m_loop.dOnGetObject += GetObject;
			m_loop.dOnReturnObject += ReturnObject;
			m_loop.dOnProvideData += ProvideData;
			m_loop.PrepareCells();
		}
		if (m_tglEach != null)
		{
			m_tglEach.OnValueChanged.RemoveAllListeners();
			m_tglEach.OnValueChanged.AddListener(OnChangedViewEach);
		}
		if (m_tglAll != null)
		{
			m_tglAll.OnValueChanged.RemoveAllListeners();
			m_tglAll.OnValueChanged.AddListener(OnChangedViewAll);
		}
		if (m_tglArena != null)
		{
			m_tglArena.OnValueChanged.RemoveAllListeners();
			m_tglArena.OnValueChanged.AddListener(OnChangedViewArena);
		}
	}

	public void Close()
	{
		m_LastViewType = m_CurViewType;
		m_CurViewType = GUILD_ARTIFACT_STORAGE_VIEW_TYPE.NONE;
	}

	private RectTransform GetObject(int idx)
	{
		NKCPopupGuildCoopArtifactStorageSlot nKCPopupGuildCoopArtifactStorageSlot = null;
		nKCPopupGuildCoopArtifactStorageSlot = ((m_stkSlot.Count <= 0) ? Object.Instantiate(m_pfbNormalSlot, m_trParent) : m_stkSlot.Pop());
		return nKCPopupGuildCoopArtifactStorageSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		NKCUtil.SetGameobjectActive(tr, bValue: false);
		tr.SetParent(base.transform);
		NKCPopupGuildCoopArtifactStorageSlot component = tr.GetComponent<NKCPopupGuildCoopArtifactStorageSlot>();
		if (!(component == null))
		{
			m_stkSlot.Push(component);
		}
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCPopupGuildCoopArtifactStorageSlot component = tr.GetComponent<NKCPopupGuildCoopArtifactStorageSlot>();
		if (component == null)
		{
			NKCUtil.SetGameobjectActive(tr, bValue: false);
		}
		else if (m_CurViewType == GUILD_ARTIFACT_STORAGE_VIEW_TYPE.ARENA)
		{
			component.SetData(m_lstSlotDataByArena[idx]);
		}
		else
		{
			component.SetData(m_lstSlotDataByID[idx]);
		}
	}

	public void SetData(Dictionary<int, List<GuildDungeonArtifactTemplet>> dicArtifactTemplet)
	{
		m_lstSlotDataByID.Clear();
		m_lstSlotDataByArena.Clear();
		if (m_CurViewType == GUILD_ARTIFACT_STORAGE_VIEW_TYPE.NONE)
		{
			m_LastViewType = GUILD_ARTIFACT_STORAGE_VIEW_TYPE.NONE;
			m_CurViewType = GUILD_ARTIFACT_STORAGE_VIEW_TYPE.ARENA;
			m_tglArena.Select(bSelect: true, bForce: true, bImmediate: true);
		}
		List<int> list = new List<int>();
		List<int> list2 = dicArtifactTemplet.Keys.ToList();
		list2.Sort();
		for (int i = 0; i < list2.Count; i++)
		{
			m_lstSlotDataByArena.Add(ArtifactSlotData.MakeSlotData(list2[i]));
			List<GuildDungeonArtifactTemplet> list3 = dicArtifactTemplet[list2[i]];
			for (int j = 0; j < list3.Count; j++)
			{
				list.Add(list3[j].GetArtifactId());
				m_lstSlotDataByArena.Add(ArtifactSlotData.MakeSlotData(list3[j]));
				m_lstSlotDataByID.Add(ArtifactSlotData.MakeSlotData(list3[j]));
			}
		}
		m_lstSlotDataByID.Sort(CompByID);
		if (m_CurViewType == GUILD_ARTIFACT_STORAGE_VIEW_TYPE.ARENA)
		{
			m_loop.TotalCount = m_lstSlotDataByArena.Count;
		}
		else
		{
			m_loop.TotalCount = m_lstSlotDataByID.Count;
		}
		m_loop.SetIndexPosition(0);
		NKCUtil.SetGameobjectActive(m_objNone, m_loop.TotalCount == 0);
		NKCUtil.SetGameobjectActive(m_lbAll, m_CurViewType == GUILD_ARTIFACT_STORAGE_VIEW_TYPE.ALL);
		NKCUtil.SetLabelText(m_lbAll, NKCUtilString.GetGuildArtifactTotalViewDesc(list));
		PlayAnimation();
	}

	private int CompByID(ArtifactSlotData left, ArtifactSlotData right)
	{
		return left.id.CompareTo(right.id);
	}

	private void OnChangedViewEach(bool bValue)
	{
		if (bValue && m_LastViewType != m_CurViewType)
		{
			m_LastViewType = m_CurViewType;
			m_CurViewType = GUILD_ARTIFACT_STORAGE_VIEW_TYPE.EACH;
			RefreshUI(bResetScroll: true);
		}
	}

	private void OnChangedViewAll(bool bValue)
	{
		if (bValue && m_LastViewType != m_CurViewType)
		{
			m_LastViewType = m_CurViewType;
			m_CurViewType = GUILD_ARTIFACT_STORAGE_VIEW_TYPE.ALL;
			RefreshUI(bResetScroll: true);
		}
	}

	private void OnChangedViewArena(bool bValue)
	{
		if (bValue && m_LastViewType != m_CurViewType)
		{
			m_LastViewType = m_CurViewType;
			m_CurViewType = GUILD_ARTIFACT_STORAGE_VIEW_TYPE.ARENA;
			RefreshUI(bResetScroll: true);
		}
	}

	private void PlayAnimation()
	{
		switch (m_LastViewType)
		{
		case GUILD_ARTIFACT_STORAGE_VIEW_TYPE.NONE:
			if (m_CurViewType == GUILD_ARTIFACT_STORAGE_VIEW_TYPE.ARENA || m_CurViewType == GUILD_ARTIFACT_STORAGE_VIEW_TYPE.EACH)
			{
				m_Animator.Play("NKM_UI_POPUP_CONSORTIUM_COOP_ARTIFACT_STORAGE_CONTENT_OPEN_EACH");
			}
			else if (m_CurViewType == GUILD_ARTIFACT_STORAGE_VIEW_TYPE.ALL)
			{
				m_Animator.Play("NKM_UI_POPUP_CONSORTIUM_COOP_ARTIFACT_STORAGE_CONTENT_OPEN");
			}
			break;
		case GUILD_ARTIFACT_STORAGE_VIEW_TYPE.ALL:
			if (m_CurViewType == GUILD_ARTIFACT_STORAGE_VIEW_TYPE.ARENA || m_CurViewType == GUILD_ARTIFACT_STORAGE_VIEW_TYPE.EACH)
			{
				m_Animator.Play("NKM_UI_POPUP_CONSORTIUM_COOP_ARTIFACT_STORAGE_CONTENT_OPEN_TO_EACH");
			}
			break;
		case GUILD_ARTIFACT_STORAGE_VIEW_TYPE.EACH:
		case GUILD_ARTIFACT_STORAGE_VIEW_TYPE.ARENA:
			if (m_CurViewType == GUILD_ARTIFACT_STORAGE_VIEW_TYPE.ALL)
			{
				m_Animator.Play("NKM_UI_POPUP_CONSORTIUM_COOP_ARTIFACT_STORAGE_CONTENT_EACH_TO_OPEN");
			}
			break;
		}
	}

	public void RefreshUI(bool bResetScroll = false)
	{
		NKCUtil.SetGameobjectActive(m_lbAll, m_CurViewType == GUILD_ARTIFACT_STORAGE_VIEW_TYPE.ALL);
		if (m_CurViewType == GUILD_ARTIFACT_STORAGE_VIEW_TYPE.ARENA)
		{
			m_loop.TotalCount = m_lstSlotDataByArena.Count;
		}
		else
		{
			m_loop.TotalCount = m_lstSlotDataByID.Count;
		}
		m_loop.SetIndexPosition(0);
		NKCUtil.SetGameobjectActive(m_objNone, m_loop.TotalCount == 0);
		PlayAnimation();
	}
}
