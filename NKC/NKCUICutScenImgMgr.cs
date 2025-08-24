using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUICutScenImgMgr : MonoBehaviour
{
	private static NKCUICutScenImgMgr m_scNKCUICutScenImgMgr;

	private Vector2 m_OrgPos = new Vector2(0f, 0f);

	private RectTransform m_goRectTransform;

	public RectTransform m_rtImg;

	public Image m_imgImg;

	public CanvasGroup m_CanvasGroup;

	private bool m_bFinished = true;

	private string m_imgFileName = "";

	private bool m_bPause;

	public static NKCUICutScenImgMgr GetCutScenImgMgr()
	{
		return m_scNKCUICutScenImgMgr;
	}

	public void SetPause(bool bPause)
	{
		m_bPause = bPause;
	}

	public static void InitUI(GameObject goNKM_UI_CUTSCEN_PLAYER)
	{
		if (!(m_scNKCUICutScenImgMgr != null))
		{
			m_scNKCUICutScenImgMgr = goNKM_UI_CUTSCEN_PLAYER.transform.Find("NKM_UI_CUTSCEN_IMG_MGR").gameObject.GetComponent<NKCUICutScenImgMgr>();
			m_scNKCUICutScenImgMgr.m_goRectTransform = m_scNKCUICutScenImgMgr.GetComponent<RectTransform>();
			m_scNKCUICutScenImgMgr.m_OrgPos = m_scNKCUICutScenImgMgr.m_goRectTransform.anchoredPosition;
			m_scNKCUICutScenImgMgr.Close();
		}
	}

	public void Reset()
	{
		SetPause(bPause: false);
	}

	private void Update()
	{
		if (!m_bPause && !m_bFinished && m_CanvasGroup.alpha < 1f)
		{
			m_CanvasGroup.alpha += Time.deltaTime * 3f;
			if (m_CanvasGroup.alpha >= 1f)
			{
				m_CanvasGroup.alpha = 1f;
				m_bFinished = true;
			}
		}
	}

	public void Open(string imgFileName, Vector2 offsetPos, float scale)
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		if (m_imgFileName != imgFileName)
		{
			m_CanvasGroup.alpha = 0f;
			m_bFinished = false;
		}
		else
		{
			m_CanvasGroup.alpha = 1f;
			m_bFinished = true;
		}
		m_imgFileName = imgFileName;
		Vector2 anchoredPosition = new Vector2(m_OrgPos.x + offsetPos.x, m_OrgPos.y + offsetPos.y);
		m_goRectTransform.anchoredPosition = anchoredPosition;
		m_goRectTransform.localScale = new Vector2(scale, scale);
		NKCAssetResourceData nKCAssetResourceData = null;
		nKCAssetResourceData = NKCResourceUtility.GetAssetResource("AB_UI_NKM_UI_CUTSCEN_IMG", "AB_UI_NKM_UI_CUTSCEN_IMG_" + imgFileName);
		if (nKCAssetResourceData != null)
		{
			m_imgImg.sprite = nKCAssetResourceData.GetAsset<Sprite>();
			if (m_imgImg.sprite != null)
			{
				m_rtImg.sizeDelta = m_imgImg.sprite.rect.size;
			}
			return;
		}
		nKCAssetResourceData = NKCAssetResourceManager.OpenResource<Sprite>("AB_UI_NKM_UI_CUTSCEN_IMG", "AB_UI_NKM_UI_CUTSCEN_IMG_" + imgFileName);
		if (nKCAssetResourceData != null)
		{
			m_imgImg.sprite = nKCAssetResourceData.GetAsset<Sprite>();
			m_rtImg.sizeDelta = m_imgImg.sprite.rect.size;
			NKCAssetResourceManager.CloseResource(nKCAssetResourceData);
		}
	}

	public bool IsFinished()
	{
		return m_bFinished;
	}

	public void Finish()
	{
		if (!m_bFinished)
		{
			m_bFinished = true;
			m_CanvasGroup.alpha = 1f;
		}
	}

	public void Close()
	{
		Finish();
		m_imgFileName = "";
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
