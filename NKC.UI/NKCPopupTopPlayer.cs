using System.Collections.Generic;
using ClientPacket.Common;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupTopPlayer : NKCUIBase
{
	public delegate void OnReward(long uid);

	[Header("팝업 상단")]
	public Text m_lbTopTitleLeft;

	public Text m_lbTopTitleRight;

	public Text m_lbSubTitle;

	public Image m_imgTitle;

	public NKCUIComStateButton m_btnClose;

	[Header(" 1등유저 전용 정보")]
	public Text m_lbUserLevel;

	public Text m_lbUserName;

	public List<NKCUISlot> m_lstEmblem = new List<NKCUISlot>();

	public NKCUICharacterView m_CharacterView;

	[Header(" 탑3 유저 정보")]
	public List<NKCPopupTopPlayerSlot> m_lstTopPlayerSlot = new List<NKCPopupTopPlayerSlot>();

	[Header("4등 이상 있을 경우 우측에 나오는 유저 정보")]
	public GameObject m_objNone;

	public LoopScrollRect m_loop;

	public Transform m_trContent;

	public Transform m_trPoolParent;

	public NKCPopupTopPlayerSlot m_pfbSlot;

	[Header("보상버튼")]
	public NKCUIComStateButton m_btnReward;

	private OnReward m_dOnReward;

	private long m_popupUID;

	private bool m_bWaitingReward;

	private List<LeaderBoardSlotData> m_lstSlotData = new List<LeaderBoardSlotData>();

	private Stack<NKCPopupTopPlayerSlot> m_stktRankListPool = new Stack<NKCPopupTopPlayerSlot>();

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "";

	public static NKCPopupTopPlayer OpenInstance(string bundleName, string assetName)
	{
		NKCPopupTopPlayer instance = NKCUIManager.OpenNewInstance<NKCPopupTopPlayer>(bundleName, assetName, NKCUIManager.eUIBaseRect.UIFrontPopup, null).GetInstance<NKCPopupTopPlayer>();
		if ((object)instance != null)
		{
			instance.Init();
			return instance;
		}
		return instance;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Init()
	{
		if (m_btnClose != null)
		{
			m_btnClose.PointerClick.RemoveAllListeners();
			m_btnClose.PointerClick.AddListener(base.Close);
		}
		if (m_loop != null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
			m_loop.dOnGetObject += GetObject;
			m_loop.dOnReturnObject += ReturnObject;
			m_loop.dOnProvideData += ProvideData;
			m_loop.PrepareCells();
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		}
		if (m_btnReward != null)
		{
			m_btnReward.PointerClick.RemoveAllListeners();
			m_btnReward.PointerClick.AddListener(OnClickReward);
		}
	}

	private RectTransform GetObject(int idx)
	{
		NKCPopupTopPlayerSlot nKCPopupTopPlayerSlot = null;
		nKCPopupTopPlayerSlot = ((m_stktRankListPool.Count <= 0) ? Object.Instantiate(m_pfbSlot) : m_stktRankListPool.Pop());
		if (nKCPopupTopPlayerSlot == null)
		{
			return null;
		}
		NKCUtil.SetGameobjectActive(nKCPopupTopPlayerSlot, bValue: true);
		nKCPopupTopPlayerSlot.transform.SetParent(m_trContent);
		return nKCPopupTopPlayerSlot.GetComponent<RectTransform>();
	}

	private void ReturnObject(Transform tr)
	{
		tr.SetParent(m_trPoolParent);
		NKCPopupTopPlayerSlot component = tr.GetComponent<NKCPopupTopPlayerSlot>();
		NKCUtil.SetGameobjectActive(component, bValue: false);
		m_stktRankListPool.Push(component);
	}

	private void ProvideData(Transform tr, int idx)
	{
		NKCPopupTopPlayerSlot component = tr.GetComponent<NKCPopupTopPlayerSlot>();
		if (!(component == null))
		{
			NKCUtil.SetGameobjectActive(component, bValue: true);
			idx += m_lstTopPlayerSlot.Count;
			if (idx < m_lstSlotData.Count)
			{
				component.SetData(m_lstSlotData[idx].Profile, m_lstSlotData[idx].GuildData, m_lstSlotData[idx].score, m_lstSlotData[idx].raidTryCount, m_lstSlotData[idx].raidTryMaxCount, idx + 1);
			}
		}
	}

	public void Open(string title_1, string title_2, string subTitle, Sprite sprTitle, List<LeaderBoardSlotData> lstSlotData, List<NKMEmblemData> emblems, long uid = 0L, OnReward dOnReward = null)
	{
		if (lstSlotData == null || lstSlotData.Count == 0)
		{
			CloseInternal();
			return;
		}
		m_bWaitingReward = false;
		m_lstSlotData = lstSlotData;
		m_popupUID = uid;
		m_dOnReward = dOnReward;
		NKCUtil.SetLabelText(m_lbTopTitleLeft, title_1);
		NKCUtil.SetLabelText(m_lbTopTitleRight, title_2);
		NKCUtil.SetLabelText(m_lbSubTitle, subTitle);
		if (sprTitle != null)
		{
			NKCUtil.SetImageSprite(m_imgTitle, sprTitle);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetLabelText(m_lbUserLevel, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, m_lstSlotData[0].Profile.level));
		NKCUtil.SetLabelText(m_lbUserName, m_lstSlotData[0].Profile.nickname);
		for (int i = 0; i < m_lstEmblem.Count; i++)
		{
			if (i < emblems.Count)
			{
				NKCUtil.SetGameobjectActive(m_lstEmblem[i], bValue: true);
				NKMEmblemData nKMEmblemData = emblems[i];
				m_lstEmblem[i].SetData(NKCUISlot.SlotData.MakeMiscItemData(nKMEmblemData.id, nKMEmblemData.count));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstEmblem[i], bValue: false);
			}
		}
		if (m_CharacterView != null)
		{
			if (m_lstSlotData[0].Profile.mainUnitSkinId == 0)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_lstSlotData[0].Profile.mainUnitId);
				m_CharacterView.SetCharacterIllust(unitTempletBase);
			}
			else
			{
				NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(m_lstSlotData[0].Profile.mainUnitSkinId);
				m_CharacterView.SetCharacterIllust(skinTemplet);
			}
		}
		for (int j = 0; j < m_lstTopPlayerSlot.Count; j++)
		{
			if (j < m_lstSlotData.Count)
			{
				m_lstTopPlayerSlot[j].SetData(m_lstSlotData[j].Profile, m_lstSlotData[j].GuildData, m_lstSlotData[j].score, m_lstSlotData[j].raidTryCount, m_lstSlotData[j].raidTryMaxCount, j + 1);
			}
			else
			{
				m_lstTopPlayerSlot[j].SetEmpty();
			}
		}
		NKCUtil.SetGameobjectActive(m_objNone, m_lstSlotData.Count <= m_lstTopPlayerSlot.Count);
		if (m_loop != null)
		{
			if (m_lstSlotData.Count > m_lstTopPlayerSlot.Count)
			{
				m_loop.TotalCount = m_lstSlotData.Count - m_lstTopPlayerSlot.Count;
				m_loop.SetIndexPosition(0);
			}
			else
			{
				m_loop.TotalCount = 0;
				m_loop.RefreshCells();
			}
		}
		UIOpened();
	}

	private void OnClickReward()
	{
		if (!m_bWaitingReward)
		{
			Close();
			m_bWaitingReward = true;
			m_dOnReward?.Invoke(m_popupUID);
		}
	}
}
