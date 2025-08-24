using System;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUINewsSlot : MonoBehaviour
{
	public delegate void OnSlot(int index);

	public delegate void OnTimeOut(bool bTimeOut, eNewsFilterType filterType = eNewsFilterType.NEWS, int slotKey = 0);

	private const string BG_ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_news_thumbnail";

	private const float UPDATE_INTERVAL = 1f;

	[Header("News")]
	public NKCUIComStateButton m_btnNews;

	public Image m_imgNewsBG;

	public GameObject m_objNewsDisable;

	public GameObject m_objNewsTimeCount;

	public Text m_lbNewsTimeCount;

	public GameObject m_objNewsSelect;

	[Header("Notice")]
	public NKCUIComStateButton m_btnNotice;

	public Image m_imgNoticeBG;

	public Text m_lbNoticeTitle;

	public GameObject m_objNoticeDisable;

	public GameObject m_objNoticeTimeCount;

	public Text m_lbNoticeTimeCount;

	public GameObject m_objNoticeSelect;

	private OnSlot dOnClickSlot;

	private OnTimeOut dOnTimeOut;

	private Text m_lbTargetText;

	private eNewsFilterType m_eSlotType;

	private int m_slotIdx;

	private DateTime m_endDateTime;

	private DateTime m_startDateTime;

	private bool m_bShowEndTime;

	private float m_time;

	public void InitUI()
	{
		m_btnNews = base.transform.Find("AB_UI_NKM_UI_NEWS").GetComponent<NKCUIComStateButton>();
		m_imgNewsBG = base.transform.Find("AB_UI_NKM_UI_NEWS/NEWS_IMAGE").GetComponent<Image>();
		m_objNewsDisable = base.transform.Find("AB_UI_NKM_UI_NEWS/NEWS_DISABLE").gameObject;
		m_objNewsTimeCount = base.transform.Find("AB_UI_NKM_UI_NEWS/AB_UI_NKM_UI_NEWS_TIME_COUNT").gameObject;
		m_lbNewsTimeCount = base.transform.Find("AB_UI_NKM_UI_NEWS/AB_UI_NKM_UI_NEWS_TIME_COUNT/NEWS_TIME_COUNT_TEXT1").GetComponent<Text>();
		m_objNewsSelect = base.transform.Find("AB_UI_NKM_UI_NEWS/AB_UI_NKM_UI_NEWS_SELECT").gameObject;
		m_btnNews.PointerClick.RemoveAllListeners();
		m_btnNews.PointerClick.AddListener(OnClickSlot);
		m_btnNotice = base.transform.Find("AB_UI_NKM_UI_NOTICE").GetComponent<NKCUIComStateButton>();
		m_imgNoticeBG = base.transform.Find("AB_UI_NKM_UI_NOTICE/NEWS_IMAGE").GetComponent<Image>();
		m_lbNoticeTitle = base.transform.Find("AB_UI_NKM_UI_NOTICE/NEWS_NOTICE_TEXT").GetComponent<Text>();
		m_objNoticeDisable = base.transform.Find("AB_UI_NKM_UI_NOTICE/NEWS_DISABLE").gameObject;
		m_objNoticeTimeCount = base.transform.Find("AB_UI_NKM_UI_NOTICE/AB_UI_NKM_UI_NEWS_TIME_COUNT_NOTICE").gameObject;
		m_lbNoticeTimeCount = base.transform.Find("AB_UI_NKM_UI_NOTICE/AB_UI_NKM_UI_NEWS_TIME_COUNT_NOTICE/NEWS_TIME_COUNT_TEXT1").GetComponent<Text>();
		m_objNoticeSelect = base.transform.Find("AB_UI_NKM_UI_NOTICE/AB_UI_NKM_UI_NEWS_SELECT").gameObject;
		m_btnNotice.PointerClick.RemoveAllListeners();
		m_btnNotice.PointerClick.AddListener(OnClickSlot);
	}

	public void SetData(NKCNewsTemplet newsTemplet, OnSlot onClickSlot, OnTimeOut onTimeOut)
	{
		m_slotIdx = newsTemplet.Idx;
		m_eSlotType = newsTemplet.m_FilterType;
		m_startDateTime = newsTemplet.m_DateStartUtc;
		m_endDateTime = newsTemplet.m_DateEndUtc;
		NKCUtil.SetGameobjectActive(m_btnNews.gameObject, newsTemplet.m_FilterType == eNewsFilterType.NEWS);
		NKCUtil.SetGameobjectActive(m_btnNotice.gameObject, newsTemplet.m_FilterType == eNewsFilterType.NOTICE);
		dOnClickSlot = onClickSlot;
		dOnTimeOut = onTimeOut;
		if (newsTemplet.m_FilterType == eNewsFilterType.NEWS)
		{
			m_imgNewsBG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_news_thumbnail", newsTemplet.m_TabImage);
			NKCUtil.SetGameobjectActive(m_objNewsDisable, bValue: true);
			NKCUtil.SetGameobjectActive(m_objNewsTimeCount, newsTemplet.m_bDateAlert);
			if (newsTemplet.m_bDateAlert)
			{
				m_lbTargetText = m_lbNewsTimeCount;
				m_time = 0f;
			}
			NKCUtil.SetGameobjectActive(m_objNewsSelect, bValue: false);
		}
		else if (newsTemplet.m_FilterType == eNewsFilterType.NOTICE)
		{
			m_imgNoticeBG.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_news_thumbnail", newsTemplet.m_TabImage);
			m_lbNoticeTitle.text = newsTemplet.m_Title;
			NKCUtil.SetGameobjectActive(m_objNoticeDisable, bValue: true);
			NKCUtil.SetGameobjectActive(m_objNoticeTimeCount, newsTemplet.m_bDateAlert);
			if (newsTemplet.m_bDateAlert)
			{
				m_lbTargetText = m_lbNoticeTimeCount;
				m_time = 0f;
			}
			NKCUtil.SetGameobjectActive(m_objNoticeSelect, bValue: false);
		}
		m_bShowEndTime = newsTemplet.m_bDateAlert;
		if (m_bShowEndTime)
		{
			m_lbTargetText.text = NKCUtilString.GetTimeString(m_endDateTime);
		}
	}

	public void Select(bool bSelect)
	{
		if (m_eSlotType == eNewsFilterType.NEWS)
		{
			NKCUtil.SetGameobjectActive(m_objNewsDisable, !bSelect);
			NKCUtil.SetGameobjectActive(m_objNewsSelect, bSelect);
		}
		else if (m_eSlotType == eNewsFilterType.NOTICE)
		{
			NKCUtil.SetGameobjectActive(m_objNoticeDisable, !bSelect);
			NKCUtil.SetGameobjectActive(m_objNoticeSelect, bSelect);
		}
	}

	public int GetSlotKey()
	{
		return m_slotIdx;
	}

	public void OnClickSlot()
	{
		if (dOnClickSlot != null)
		{
			dOnClickSlot(m_slotIdx);
		}
	}

	private void Update()
	{
		if (!m_bShowEndTime)
		{
			return;
		}
		m_time += Time.deltaTime;
		if (1f < m_time)
		{
			m_time -= 1f;
			if (!NKCSynchronizedTime.IsFinished(m_endDateTime))
			{
				m_lbTargetText.text = NKCUtilString.GetTimeString(m_endDateTime);
				return;
			}
			m_bShowEndTime = false;
			dOnTimeOut?.Invoke(bTimeOut: true, m_eSlotType, m_slotIdx);
		}
	}
}
