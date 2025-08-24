using System.Collections.Generic;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperationSubSeasonal : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_OPERATION";

	private const string UI_ASSET_NAME = "AB_UI_OPERATION_UI_SUB_03";

	private static NKCUIOperationSubSeasonal m_Instance;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffdũ\ufffd\ufffd")]
	public NKCUIOperationSubMainStreamEPSlot m_pfbSlot;

	public LoopScrollRect m_loop;

	[Header("\ufffd߾\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public Image m_imgBG;

	public NKCComTMPUIText m_lbTitle;

	public NKCComTMPUIText m_lbSubTitle;

	public NKCComTMPUIText m_lbDesc;

	public GameObject m_objBGM;

	public NKCComTMPUIText m_lbBGM;

	public NKCUIComStateButton m_btnStart;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffdӴٿ\ufffd")]
	public NKCUIOperationRelateEpList m_DropDown;

	[Space]
	public float m_FadeTime = 0.3f;

	private Stack<NKCUIOperationSubMainStreamEPSlot> m_stkSlot = new Stack<NKCUIOperationSubMainStreamEPSlot>();

	private Dictionary<int, NKCUIOperationSubMainStreamEPSlot> m_dicSlot = new Dictionary<int, NKCUIOperationSubMainStreamEPSlot>();

	private List<NKMEpisodeTempletV2> m_lstEpTemplet = new List<NKMEpisodeTempletV2>();

	private NKMEpisodeTempletV2 m_curEpisodeTemplet;

	public static NKCUIOperationSubSeasonal Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIOperationSubSeasonal>("AB_UI_OPERATION", "AB_UI_OPERATION_UI_SUB_03", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIOperationSubSeasonal>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => "";

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.LeftsideWithHamburger;

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static bool isOpen()
	{
		if (m_Instance != null)
		{
			return m_Instance.IsOpen;
		}
		return false;
	}

	public void InitUI()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loop.dOnGetObject += GetObject;
		m_loop.dOnReturnObject += ReturnObject;
		m_loop.dOnProvideData += ProvideData;
		m_loop.PrepareCells();
		m_btnStart.PointerClick.RemoveAllListeners();
		m_btnStart.PointerClick.AddListener(OnClickStart);
		m_DropDown.InitUI();
		m_dicSlot.Clear();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public override void UnHide()
	{
		base.UnHide();
		if (m_curEpisodeTemplet != null)
		{
			foreach (KeyValuePair<int, NKCUIOperationSubMainStreamEPSlot> item in m_dicSlot)
			{
				item.Value.SetSelected(item.Key == m_curEpisodeTemplet.m_EpisodeID);
			}
			SetData(m_curEpisodeTemplet);
		}
		else
		{
			NKCUIFadeInOut.FadeIn(m_FadeTime);
		}
	}

	public override void OnBackButton()
	{
		base.OnBackButton();
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public RectTransform GetObject(int idx)
	{
		NKCUIOperationSubMainStreamEPSlot nKCUIOperationSubMainStreamEPSlot = null;
		if (m_stkSlot.Count > 0)
		{
			nKCUIOperationSubMainStreamEPSlot = m_stkSlot.Pop();
		}
		else
		{
			nKCUIOperationSubMainStreamEPSlot = Object.Instantiate(m_pfbSlot, m_loop.content);
			nKCUIOperationSubMainStreamEPSlot.InitUI(OnEpSlotSelect, m_loop.content.GetComponent<NKCUIComToggleGroup>());
		}
		return nKCUIOperationSubMainStreamEPSlot.GetComponent<RectTransform>();
	}

	public void ReturnObject(Transform tr)
	{
		NKCUIOperationSubMainStreamEPSlot component = tr.GetComponent<NKCUIOperationSubMainStreamEPSlot>();
		if (!(component == null))
		{
			if (component.GetEpisodeID() > 0 && m_dicSlot.ContainsKey(component.GetEpisodeID()))
			{
				m_dicSlot.Remove(component.GetEpisodeID());
			}
			m_stkSlot.Push(component);
			NKCUtil.SetGameobjectActive(component.gameObject, bValue: false);
		}
	}

	public void ProvideData(Transform tr, int idx)
	{
		NKCUIOperationSubMainStreamEPSlot component = tr.GetComponent<NKCUIOperationSubMainStreamEPSlot>();
		if (!(component == null) && idx < m_lstEpTemplet.Count)
		{
			m_dicSlot.Remove(component.GetEpisodeID());
			NKCUtil.SetGameobjectActive(component, bValue: true);
			component.SetData(m_lstEpTemplet[idx].m_EpisodeID, m_lstEpTemplet[idx].GetEpisodeName(), 0);
			component.SetSelected(m_lstEpTemplet[idx].m_EpisodeID == m_curEpisodeTemplet.m_EpisodeID);
			m_dicSlot.Add(m_lstEpTemplet[idx].m_EpisodeID, component);
		}
	}

	public void Open()
	{
		m_dicSlot.Clear();
		m_lstEpTemplet.Clear();
		m_lstEpTemplet = NKMEpisodeMgr.GetListNKMEpisodeTempletByCategory(EPISODE_CATEGORY.EC_SEASONAL);
		if (m_lstEpTemplet.Count == 0)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		if (NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetLastPlayedSeasonal() > 0)
		{
			m_curEpisodeTemplet = m_lstEpTemplet.Find((NKMEpisodeTempletV2 x) => x.m_EpisodeID == NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetLastPlayedSeasonal());
		}
		if (m_curEpisodeTemplet == null)
		{
			m_curEpisodeTemplet = m_lstEpTemplet[m_lstEpTemplet.Count - 1];
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_loop.TotalCount = m_lstEpTemplet.Count;
		m_loop.SetIndexPosition(m_lstEpTemplet.Count - 1);
		SetData(m_curEpisodeTemplet);
		NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetLastPlayedSeasonal(0);
		UIOpened();
	}

	private void SetData(NKMEpisodeTempletV2 epTemplet)
	{
		NKCUtil.SetLabelText(m_lbTitle, epTemplet.GetEpisodeTitle());
		NKCUtil.SetLabelText(m_lbSubTitle, epTemplet.GetEpisodeName());
		NKCUtil.SetLabelText(m_lbDesc, epTemplet.GetEpisodeDesc());
		NKCUtil.SetImageSprite(m_imgBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_OPERATION_Bg", epTemplet.m_EPThumbnail));
		NKCBGMInfoTemplet nKCBGMInfoTemplet = NKCBGMInfoTemplet.Find(epTemplet.m_BG_Music);
		if (nKCBGMInfoTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_objBGM, bValue: true);
			NKCUtil.SetLabelText(m_lbBGM, NKCStringTable.GetString(nKCBGMInfoTemplet.m_BgmNameStringID));
			NKCSoundManager.PlayMusic(nKCBGMInfoTemplet.m_BgmAssetID, bLoop: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objBGM, bValue: false);
		}
		m_DropDown.SetData(epTemplet);
		NKCUIFadeInOut.FadeIn(m_FadeTime);
	}

	private void OnEpSlotSelect(int episodeID)
	{
		m_curEpisodeTemplet = NKMEpisodeTempletV2.Find(episodeID, EPISODE_DIFFICULTY.NORMAL);
		foreach (KeyValuePair<int, NKCUIOperationSubMainStreamEPSlot> item in m_dicSlot)
		{
			item.Value.ChangeSelected(item.Key == episodeID);
		}
		NKCUIFadeInOut.FadeOut(m_FadeTime, delegate
		{
			SetData(m_curEpisodeTemplet);
		});
	}

	private void OnClickStart()
	{
		NKCScenManager.GetScenManager().Get_SCEN_OPERATION().SetLastPlayedSeasonal(m_curEpisodeTemplet.m_EpisodeID);
		NKCUIOperationNodeViewer.Instance.Open(m_curEpisodeTemplet);
	}

	private void OnClickDropDown(bool bValue)
	{
	}
}
