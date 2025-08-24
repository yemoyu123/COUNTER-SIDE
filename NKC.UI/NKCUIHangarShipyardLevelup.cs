using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIHangarShipyardLevelup : MonoBehaviour
{
	[Header("레벨 표시")]
	public GameObject m_NKM_UI_SHIPYARD_LevelUp_buttons;

	public GameObject m_NKM_UI_SHIPYARD_LevelUp_MAXLEVEL_ROOT;

	public NKCUIComStarRank m_StarRank;

	[Header("레벨업 정보")]
	public NKCUIComStateButton m_NKM_UI_SHIPYARD_LV_MIN;

	public Image m_img_NKM_UI_SHIPYARD_LV_MIN;

	public Image m_img_NKM_UI_SHIPYARD_LV_MIN_ICON;

	public NKCUIComStateButton m_NKM_UI_SHIPYARD_LV_PREV;

	public Image m_img_NKM_UI_SHIPYARD_LV_PREV;

	public Image m_img_NKM_UI_SHIPYARD_LV_PREV_ICON;

	public Text m_txt_NKM_UI_SHIPYARD_LV_TEXT;

	private RectTransform m_rt_NKM_UI_SHIPYARD_LV_TEXT;

	public Text m_txt_NKM_UI_SHIPYARD_LV_TEXT_DUMMY;

	private RectTransform m_rt_NKM_UI_SHIPYARD_LV_TEXT_DUMMY;

	public NKCUIComStateButton m_NKM_UI_SHIPYARD_LV_NEXT;

	public Image m_img_NKM_UI_SHIPYARD_LV_NEXT;

	public Image m_img_NKM_UI_SHIPYARD_LV_NEXT_ICON;

	public NKCUIComStateButton m_NKM_UI_SHIPYARD_LV_MAX;

	public Image m_img_NKM_UI_SHIPYARD_LV_MAX;

	public Image m_img_NKM_UI_SHIPYARD_LV_MAX_ICON;

	private const int LevelTextMovePositionX = 300;

	private int m_iCurShipId;

	private int m_iCurShipStarGrade;

	private int m_iCurShipLevel;

	private int m_iStartShipLevel;

	private int m_iCurrentShipMaxLevel;

	private int m_iSelectLevel;

	private int m_iLimitBreakLevel;

	private bool m_bCanTryLevelUp;

	public const float BUTTON_DELAY_GAP = 0.15f;

	public void Init(UnityAction MinBtn, UnityAction PreBtn, UnityAction NextBtn, UnityAction MaxBtn)
	{
		if (MinBtn != null)
		{
			m_NKM_UI_SHIPYARD_LV_MIN.PointerClick.RemoveAllListeners();
			m_NKM_UI_SHIPYARD_LV_MIN.PointerClick.AddListener(MinBtn);
		}
		if (PreBtn != null)
		{
			m_NKM_UI_SHIPYARD_LV_PREV.PointerClick.RemoveAllListeners();
			m_NKM_UI_SHIPYARD_LV_PREV.PointerClick.AddListener(PreBtn);
		}
		if (NextBtn != null)
		{
			m_NKM_UI_SHIPYARD_LV_NEXT.PointerClick.RemoveAllListeners();
			m_NKM_UI_SHIPYARD_LV_NEXT.PointerClick.AddListener(NextBtn);
		}
		if (MaxBtn != null)
		{
			m_NKM_UI_SHIPYARD_LV_MAX.PointerClick.RemoveAllListeners();
			m_NKM_UI_SHIPYARD_LV_MAX.PointerClick.AddListener(MaxBtn);
		}
		if (m_txt_NKM_UI_SHIPYARD_LV_TEXT_DUMMY != null)
		{
			m_rt_NKM_UI_SHIPYARD_LV_TEXT_DUMMY = m_txt_NKM_UI_SHIPYARD_LV_TEXT_DUMMY.gameObject.GetComponent<RectTransform>();
			NKCUtil.SetGameobjectActive(m_txt_NKM_UI_SHIPYARD_LV_TEXT_DUMMY.gameObject, bValue: false);
		}
		if (m_txt_NKM_UI_SHIPYARD_LV_TEXT != null)
		{
			m_rt_NKM_UI_SHIPYARD_LV_TEXT = m_txt_NKM_UI_SHIPYARD_LV_TEXT.gameObject.GetComponent<RectTransform>();
		}
		InitUI();
	}

	private void InitUI()
	{
		m_img_NKM_UI_SHIPYARD_LV_NEXT_ICON.color = NKCUtil.GetColor("#222222");
		m_img_NKM_UI_SHIPYARD_LV_MAX_ICON.color = NKCUtil.GetColor("#222222");
		m_img_NKM_UI_SHIPYARD_LV_NEXT.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY);
		m_img_NKM_UI_SHIPYARD_LV_MAX.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY);
		m_img_NKM_UI_SHIPYARD_LV_NEXT.color = NKCUtil.GetColor("#4E4E4E");
		m_img_NKM_UI_SHIPYARD_LV_MAX.color = NKCUtil.GetColor("#4E4E4E");
		m_img_NKM_UI_SHIPYARD_LV_MIN_ICON.color = NKCUtil.GetColor("#222222");
		m_img_NKM_UI_SHIPYARD_LV_PREV_ICON.color = NKCUtil.GetColor("#222222");
		m_img_NKM_UI_SHIPYARD_LV_MIN.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY);
		m_img_NKM_UI_SHIPYARD_LV_PREV.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY);
		m_img_NKM_UI_SHIPYARD_LV_MIN.color = NKCUtil.GetColor("#4E4E4E");
		m_img_NKM_UI_SHIPYARD_LV_PREV.color = NKCUtil.GetColor("#4E4E4E");
		NKCUtil.SetGameobjectActive(m_NKM_UI_SHIPYARD_LevelUp_buttons, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_SHIPYARD_LevelUp_MAXLEVEL_ROOT, bValue: false);
	}

	public void OnClickMaximumButton()
	{
		m_img_NKM_UI_SHIPYARD_LV_NEXT_ICON.color = NKCUtil.GetColor("#222222");
		m_img_NKM_UI_SHIPYARD_LV_MAX_ICON.color = NKCUtil.GetColor("#222222");
		m_img_NKM_UI_SHIPYARD_LV_NEXT.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY);
		m_img_NKM_UI_SHIPYARD_LV_MAX.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY);
		m_img_NKM_UI_SHIPYARD_LV_NEXT.color = NKCUtil.GetColor("#4E4E4E");
		m_img_NKM_UI_SHIPYARD_LV_MAX.color = NKCUtil.GetColor("#4E4E4E");
	}

	public void UpdateShipData(int unitId, int starGrade, int curLv, int startLv, int curMaxLv, int selectLv = 0, bool canTryLvUp = false, int limitBreakLevel = 0)
	{
		m_iCurShipId = unitId;
		m_iCurShipStarGrade = starGrade;
		m_iCurShipLevel = curLv;
		m_iStartShipLevel = startLv;
		m_iCurrentShipMaxLevel = curMaxLv;
		m_iLimitBreakLevel = limitBreakLevel;
		UpdateTextUI(selectLv);
		m_iSelectLevel = selectLv;
		m_bCanTryLevelUp = canTryLvUp;
		UpdateButtonUI();
	}

	private void UpdateTextUI(int newLevel, bool bAnimation = true)
	{
		if (!bAnimation)
		{
			m_txt_NKM_UI_SHIPYARD_LV_TEXT.text = string.Format(NKCUtilString.GET_STRING_HANGAR_SHIP_LEVEL_TWO_PARAM, newLevel, m_iCurrentShipMaxLevel);
			m_rt_NKM_UI_SHIPYARD_LV_TEXT.position = Vector3.zero;
			NKCUtil.SetGameobjectActive(m_txt_NKM_UI_SHIPYARD_LV_TEXT.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_txt_NKM_UI_SHIPYARD_LV_TEXT_DUMMY.gameObject, bValue: false);
			return;
		}
		if (m_iSelectLevel == 0)
		{
			m_txt_NKM_UI_SHIPYARD_LV_TEXT.text = string.Format(NKCUtilString.GET_STRING_HANGAR_SHIP_LEVEL_TWO_PARAM, newLevel, m_iCurrentShipMaxLevel);
			NKCUtil.SetGameobjectActive(m_txt_NKM_UI_SHIPYARD_LV_TEXT_DUMMY.gameObject, bValue: false);
			return;
		}
		m_txt_NKM_UI_SHIPYARD_LV_TEXT.text = string.Format(NKCUtilString.GET_STRING_HANGAR_SHIP_LEVEL_TWO_PARAM, m_iSelectLevel, m_iCurrentShipMaxLevel);
		m_txt_NKM_UI_SHIPYARD_LV_TEXT_DUMMY.text = string.Format(NKCUtilString.GET_STRING_HANGAR_SHIP_LEVEL_TWO_PARAM, newLevel, m_iCurrentShipMaxLevel);
		if (newLevel > m_iSelectLevel)
		{
			m_rt_NKM_UI_SHIPYARD_LV_TEXT_DUMMY.anchoredPosition = new Vector2(300f, m_rt_NKM_UI_SHIPYARD_LV_TEXT.anchoredPosition.y);
			m_rt_NKM_UI_SHIPYARD_LV_TEXT_DUMMY.DOAnchorPosX(0f, 0.15f);
			m_rt_NKM_UI_SHIPYARD_LV_TEXT.anchoredPosition = new Vector2(0f, m_rt_NKM_UI_SHIPYARD_LV_TEXT.anchoredPosition.y);
			m_rt_NKM_UI_SHIPYARD_LV_TEXT.DOAnchorPosX(-300f, 0.15f);
		}
		else
		{
			m_rt_NKM_UI_SHIPYARD_LV_TEXT_DUMMY.anchoredPosition = new Vector2(-300f, m_rt_NKM_UI_SHIPYARD_LV_TEXT.anchoredPosition.y);
			m_rt_NKM_UI_SHIPYARD_LV_TEXT_DUMMY.DOAnchorPosX(0f, 0.15f);
			m_rt_NKM_UI_SHIPYARD_LV_TEXT.anchoredPosition = new Vector2(0f, m_rt_NKM_UI_SHIPYARD_LV_TEXT.anchoredPosition.y);
			m_rt_NKM_UI_SHIPYARD_LV_TEXT.DOAnchorPosX(300f, 0.15f);
		}
		NKCUtil.SetGameobjectActive(m_txt_NKM_UI_SHIPYARD_LV_TEXT_DUMMY.gameObject, bValue: true);
	}

	private void UpdateButtonUI()
	{
		if (m_iCurrentShipMaxLevel == m_iCurShipLevel)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_SHIPYARD_LevelUp_buttons, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_SHIPYARD_LevelUp_MAXLEVEL_ROOT, bValue: true);
			m_StarRank?.SetStarRankShip(m_iCurShipId, m_iLimitBreakLevel);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_SHIPYARD_LevelUp_buttons, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_SHIPYARD_LevelUp_MAXLEVEL_ROOT, bValue: false);
		}
		if (m_bCanTryLevelUp && m_iSelectLevel < m_iCurrentShipMaxLevel)
		{
			m_img_NKM_UI_SHIPYARD_LV_NEXT_ICON.color = NKCUtil.GetColor("#582817");
			m_img_NKM_UI_SHIPYARD_LV_MAX_ICON.color = NKCUtil.GetColor("#582817");
			m_img_NKM_UI_SHIPYARD_LV_NEXT.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW);
			m_img_NKM_UI_SHIPYARD_LV_MAX.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW);
			m_img_NKM_UI_SHIPYARD_LV_NEXT.color = Color.white;
			m_img_NKM_UI_SHIPYARD_LV_MAX.color = Color.white;
		}
		if (!m_bCanTryLevelUp || m_iCurrentShipMaxLevel == m_iSelectLevel)
		{
			m_img_NKM_UI_SHIPYARD_LV_NEXT_ICON.color = NKCUtil.GetColor("#222222");
			m_img_NKM_UI_SHIPYARD_LV_MAX_ICON.color = NKCUtil.GetColor("#222222");
			m_img_NKM_UI_SHIPYARD_LV_NEXT.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY);
			m_img_NKM_UI_SHIPYARD_LV_MAX.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY);
			m_img_NKM_UI_SHIPYARD_LV_NEXT.color = NKCUtil.GetColor("#4E4E4E");
			m_img_NKM_UI_SHIPYARD_LV_MAX.color = NKCUtil.GetColor("#4E4E4E");
		}
		if (m_iSelectLevel > m_iStartShipLevel)
		{
			m_img_NKM_UI_SHIPYARD_LV_MIN_ICON.color = NKCUtil.GetColor("#582817");
			m_img_NKM_UI_SHIPYARD_LV_PREV_ICON.color = NKCUtil.GetColor("#582817");
			m_img_NKM_UI_SHIPYARD_LV_MIN.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW);
			m_img_NKM_UI_SHIPYARD_LV_PREV.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW);
			m_img_NKM_UI_SHIPYARD_LV_MIN.color = Color.white;
			m_img_NKM_UI_SHIPYARD_LV_PREV.color = Color.white;
		}
		else
		{
			m_img_NKM_UI_SHIPYARD_LV_MIN_ICON.color = NKCUtil.GetColor("#222222");
			m_img_NKM_UI_SHIPYARD_LV_PREV_ICON.color = NKCUtil.GetColor("#222222");
			m_img_NKM_UI_SHIPYARD_LV_MIN.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY);
			m_img_NKM_UI_SHIPYARD_LV_PREV.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY);
			m_img_NKM_UI_SHIPYARD_LV_MIN.color = NKCUtil.GetColor("#4E4E4E");
			m_img_NKM_UI_SHIPYARD_LV_PREV.color = NKCUtil.GetColor("#4E4E4E");
		}
	}
}
