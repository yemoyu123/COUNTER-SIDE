using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupArtifactExchange : NKCUIBase
{
	public delegate void dOnClose();

	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_WORLD_MAP_DIVE";

	public const string UI_ASSET_NAME = "NKM_UI_DIVE_ARTIFACT_EXCHANGE_POPUP";

	public EventTrigger m_etBG;

	public Text m_lbArtifactCount;

	public Text m_lbTotalGetItem;

	public Image m_imgTotalGetItemIcon;

	public Text m_lbTotalGetItemCount;

	public LoopScrollRect m_lsrArtifact;

	public GridLayoutGroup m_GridLayoutGroup;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private List<int> m_lstArtifact = new List<int>();

	private bool m_bFirstOpen = true;

	private dOnClose m_dOnClose;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "PopupArtifactExchange";

	public void InitUI()
	{
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_etBG.triggers.Add(entry);
		m_lsrArtifact.dOnGetObject += GetArtifactSlot;
		m_lsrArtifact.dOnReturnObject += ReturnArtifactSlot;
		m_lsrArtifact.dOnProvideData += ProvideArtifactSlot;
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public RectTransform GetArtifactSlot(int index)
	{
		return NKCPopupArtifactExchangeSlot.GetNewInstance(null).GetComponent<RectTransform>();
	}

	public void ReturnArtifactSlot(Transform tr)
	{
		tr.SetParent(base.transform);
		Object.Destroy(tr.gameObject);
	}

	public void ProvideArtifactSlot(Transform tr, int index)
	{
		NKCPopupArtifactExchangeSlot component = tr.GetComponent<NKCPopupArtifactExchangeSlot>();
		if (component != null && m_lstArtifact.Count > index)
		{
			component.SetData(m_lstArtifact[index]);
		}
	}

	public void Open(List<int> _lstArtifact, int getMiscItemID, dOnClose _dOnClose = null)
	{
		m_dOnClose = _dOnClose;
		m_lstArtifact.Clear();
		if (_lstArtifact != null)
		{
			m_lstArtifact.AddRange(_lstArtifact);
		}
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
		SetUI(getMiscItemID);
	}

	private void SetUI(int getMiscItemID)
	{
		if (m_bFirstOpen)
		{
			m_lsrArtifact.PrepareCells();
			m_bFirstOpen = false;
		}
		m_lsrArtifact.TotalCount = m_lstArtifact.Count;
		m_lsrArtifact.SetIndexPosition(0);
		NKCUtil.SetLabelText(m_lbArtifactCount, string.Format(NKCStringTable.GetString("SI_DP_DIVE_ARTIFACT_COUNT_DESC"), m_lstArtifact.Count));
		int num = 0;
		for (int i = 0; i < m_lstArtifact.Count; i++)
		{
			NKMDiveArtifactTemplet.Find(m_lstArtifact[i]);
		}
		m_imgTotalGetItemIcon.sprite = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(getMiscItemID);
		NKCUtil.SetLabelText(m_lbTotalGetItemCount, num.ToString());
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(getMiscItemID);
		if (itemMiscTempletByID != null)
		{
			NKCUtil.SetLabelText(m_lbTotalGetItem, string.Format(NKCUtilString.GET_STRING_DIVE_ARTIFACT_EXCHANGE_TOTAL_GET_ITEM, itemMiscTempletByID.GetItemName()));
		}
	}

	private void Update()
	{
		m_NKCUIOpenAnimator.Update();
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (m_dOnClose != null)
		{
			m_dOnClose();
			m_dOnClose = null;
		}
	}
}
