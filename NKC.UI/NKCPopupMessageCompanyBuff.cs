using System.Collections;
using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupMessageCompanyBuff : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_MESSAGE_EVENTBUFF";

	private static NKCPopupMessageCompanyBuff m_Instance;

	private const float MESSAGE_STAY_TIME = 3f;

	public Animator m_Ani;

	public RectTransform m_rtMessageRoot;

	public Image m_imgIcon;

	public Text m_lbTitle;

	public Text m_lbMessage;

	private Queue<NKMCompanyBuffData> m_lstBuffData = new Queue<NKMCompanyBuffData>();

	private Queue<bool> m_lstAddBuff = new Queue<bool>();

	private bool m_bPlaying;

	public static NKCPopupMessageCompanyBuff Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupMessageCompanyBuff>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_MESSAGE_EVENTBUFF", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCPopupMessageCompanyBuff>();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Overlay;

	public override string MenuName => "Message";

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public void Open(NKMCompanyBuffData buffData, bool bAddBuff)
	{
		m_lstBuffData.Enqueue(buffData);
		m_lstAddBuff.Enqueue(bAddBuff);
		if (!m_bPlaying)
		{
			base.gameObject.SetActive(value: true);
			UIOpened();
			StartCoroutine(Process());
			m_bPlaying = true;
		}
	}

	private IEnumerator Process()
	{
		while (m_lstBuffData.Count > 0 && m_lstAddBuff.Count > 0)
		{
			NKMCompanyBuffData nKMCompanyBuffData = m_lstBuffData.Dequeue();
			bool bAddBuff = m_lstAddBuff.Dequeue();
			if (nKMCompanyBuffData != null)
			{
				NKMCompanyBuffTemplet nKMCompanyBuffTemplet = NKMTempletContainer<NKMCompanyBuffTemplet>.Find(nKMCompanyBuffData.Id);
				NKCUtil.SetGameobjectActive(m_rtMessageRoot, bValue: false);
				m_imgIcon.sprite = NKCUtil.GetCompanyBuffIconSprite(nKMCompanyBuffTemplet.m_CompanyBuffIcon);
				m_lbTitle.text = NKCUtilString.GetCompanyBuffTitle(nKMCompanyBuffTemplet.GetBuffName(), bAddBuff);
				m_lbMessage.text = NKCUtil.GetCompanyBuffDesc(nKMCompanyBuffTemplet.m_CompanyBuffID);
				yield return StartCoroutine(ProcessShowBuff(bAddBuff));
			}
		}
		Close();
	}

	private IEnumerator ProcessShowBuff(bool bAddBuff)
	{
		NKCUtil.SetGameobjectActive(m_rtMessageRoot, bValue: true);
		if (bAddBuff)
		{
			m_Ani.Play("NKM_UI_POPUP_MESSAGE_EVENTBUFF_INTRO");
		}
		else
		{
			m_Ani.Play("NKM_UI_POPUP_MESSAGE_EVENTBUFF_OFF");
		}
		yield return new WaitForSeconds(3f);
		NKCUtil.SetGameobjectActive(m_rtMessageRoot, bValue: false);
	}

	public override void CloseInternal()
	{
		m_bPlaying = false;
		m_lstBuffData.Clear();
		m_lstAddBuff.Clear();
		NKCUtil.SetGameobjectActive(m_rtMessageRoot, bValue: false);
		base.gameObject.SetActive(value: false);
	}
}
