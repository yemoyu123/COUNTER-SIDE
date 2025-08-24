using System;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUINewsSubUI : MonoBehaviour
{
	private const string BANNER_ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_news_thumbnail";

	private const float UPDATE_INTERVAL = 1f;

	public Image m_imgThumbnail;

	public GameObject m_objTimeCount;

	public Text m_lbTimeCount;

	public Text m_lbDesc;

	public GameObject m_objEmpty;

	public Text m_lbEmpty;

	private DateTime m_endDateTime;

	private bool m_bShowEndTime;

	private float m_time;

	private void InitUI()
	{
		m_imgThumbnail = base.transform.Find("AB_UI_NKM_UI_NEWS_CONTENT_SCROLL_VIEW_MAIN2/AB_UI_NKM_UI_NEWS_CONTENT_VIEWPORT_MAIN/NEWS_BANNER_THUMBNAIL").GetComponent<Image>();
		m_objTimeCount = m_imgThumbnail.transform.Find("AB_UI_NKM_UI_NEWS_CONTENT_TIME_COUNT").gameObject;
		m_lbTimeCount = m_objTimeCount.transform.Find("AB_UI_NKM_UI_NEWS_BUTTON_TEXT1").GetComponent<Text>();
		m_lbDesc = base.transform.Find("AB_UI_NKM_UI_NEWS_CONTENT_SCROLL_VIEW_MAIN2/AB_UI_NKM_UI_NEWS_CONTENT_VIEWPORT_MAIN/NEWS_MAIN_TEXT").GetComponent<Text>();
	}

	public void SetData(eNewsFilterType filterType, NKCNewsTemplet newsTemplet)
	{
		NKCUtil.SetGameobjectActive(m_objEmpty, newsTemplet == null);
		if (newsTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_imgThumbnail, bValue: true);
			NKCUtil.SetGameobjectActive(m_lbDesc, bValue: true);
			m_imgThumbnail.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_news_thumbnail", newsTemplet.m_BannerImage);
			m_lbDesc.text = newsTemplet.m_Contents;
			m_endDateTime = newsTemplet.m_DateEndUtc;
			m_bShowEndTime = newsTemplet.m_bDateAlert;
			if (m_bShowEndTime)
			{
				NKCUtil.SetGameobjectActive(m_objTimeCount, bValue: true);
				m_lbTimeCount.text = NKCUtilString.GetTimeString(newsTemplet.m_DateEndUtc);
				m_time = 0f;
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objTimeCount, bValue: false);
			}
		}
		else
		{
			m_bShowEndTime = false;
			NKCUtil.SetGameobjectActive(m_imgThumbnail, bValue: false);
			NKCUtil.SetGameobjectActive(m_lbDesc, bValue: false);
			if (filterType == eNewsFilterType.NEWS)
			{
				m_lbEmpty.text = NKCUtilString.GET_STRING_NEWS_DOES_NOT_HAVE_NEWS;
			}
			else
			{
				m_lbEmpty.text = NKCUtilString.GET_STRING_NEWS_DOES_NOT_HAVE_NOTICE;
			}
		}
	}

	private void Update()
	{
		if (m_bShowEndTime)
		{
			m_time += Time.deltaTime;
			if (1f < m_time)
			{
				m_time -= 1f;
				m_lbTimeCount.text = NKCUtilString.GetTimeString(m_endDateTime);
			}
		}
	}
}
