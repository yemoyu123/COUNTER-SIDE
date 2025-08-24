using System.Text;
using NKC.UI.Tooltip;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.HUD;

public class NKCUIHudTacticalCommandDeck
{
	public int m_Index;

	public NKCGameHud m_NKCGameHud;

	public GameObject m_NUF_GAME_HUD_TACTICAL_COMMAND_MAIN;

	public NKMTacticalCommandTemplet m_NKMTacticalCommandTemplet;

	public NKMTacticalCommandData m_NKMTacticalCommandData;

	public GameObject m_NKM_UI_TACTICAL_COMMAND_DECK;

	public NKCUIComStateButton m_NKM_UI_TACTICAL_COMMAND_DECK_NKCUIComStateButton;

	public GameObject m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD;

	public Animator m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_Animator;

	public GameObject m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_Panel;

	public Image m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_Panel_Image;

	public GameObject m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_GRAY_Panel;

	public Image m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_GRAY_Panel_Image;

	public GameObject m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_ADD_Panel;

	public Image m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_ADD_Panel_Image;

	public GameObject m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_BG_Panel;

	public GameObject m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_Panel;

	public Image m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_Panel_Image;

	public GameObject m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_UNIT_BORDER;

	public GameObject m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME;

	public RectTransform m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_RectTransform;

	public GameObject m_NKM_UI_GAME_TACTICAL_COMMAND_COST_Text;

	public Text m_NKM_UI_GAME_TACTICAL_COMMAND_COST_Text_Text;

	private bool m_bEventControl;

	private StringBuilder m_StringBuilder = new StringBuilder();

	private int m_NeedCostBefore;

	public void InitUI(NKCGameHud cNKCGameHud, GameObject deckMainObject, int index)
	{
		m_NUF_GAME_HUD_TACTICAL_COMMAND_MAIN = deckMainObject;
		m_NKCGameHud = cNKCGameHud;
		m_Index = index;
		m_StringBuilder.Remove(0, m_StringBuilder.Length);
		m_StringBuilder.AppendFormat("NKM_UI_TACTICAL_COMMAND_DECK{0}", index);
		m_NKM_UI_TACTICAL_COMMAND_DECK = m_NUF_GAME_HUD_TACTICAL_COMMAND_MAIN.transform.Find(m_StringBuilder.ToString()).gameObject;
		m_NKM_UI_TACTICAL_COMMAND_DECK_NKCUIComStateButton = m_NKM_UI_TACTICAL_COMMAND_DECK.GetComponentInChildren<NKCUIComStateButton>();
		m_NKM_UI_TACTICAL_COMMAND_DECK_NKCUIComStateButton.PointerClick.RemoveAllListeners();
		m_NKM_UI_TACTICAL_COMMAND_DECK_NKCUIComStateButton.PointerClick.AddListener(Click);
		m_NKM_UI_TACTICAL_COMMAND_DECK_NKCUIComStateButton.PointerDown.RemoveAllListeners();
		m_NKM_UI_TACTICAL_COMMAND_DECK_NKCUIComStateButton.PointerDown.AddListener(PointerDown);
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD = m_NKM_UI_TACTICAL_COMMAND_DECK.transform.Find("NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD").gameObject;
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_Animator = m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD.GetComponentInChildren<Animator>();
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_Panel = m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD.transform.Find("NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_Panel").gameObject;
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_Panel_Image = m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_Panel.GetComponentInChildren<Image>();
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_GRAY_Panel = m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD.transform.Find("NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_GRAY_Panel").gameObject;
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_GRAY_Panel_Image = m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_GRAY_Panel.GetComponentInChildren<Image>();
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_ADD_Panel = m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD.transform.Find("NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_ADD_Panel").gameObject;
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_ADD_Panel_Image = m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_ADD_Panel.GetComponentInChildren<Image>();
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_UNIT_BORDER = m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD.transform.Find("NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_BORDER").gameObject;
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME = m_NKM_UI_TACTICAL_COMMAND_DECK.transform.Find("NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME").gameObject;
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_RectTransform = m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME.GetComponentInChildren<RectTransform>();
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_BG_Panel = m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME.transform.Find("NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_BG_Panel").gameObject;
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_Panel = m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME.transform.Find("NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_Panel").gameObject;
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_Panel_Image = m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_Panel.GetComponentInChildren<Image>();
		m_NKM_UI_GAME_TACTICAL_COMMAND_COST_Text = m_NKM_UI_TACTICAL_COMMAND_DECK.transform.Find("NKM_UI_GAME_TACTICAL_COMMAND_COST_BG_Panel/NKM_UI_GAME_TACTICAL_COMMAND_COST_Text").gameObject;
		m_NKM_UI_GAME_TACTICAL_COMMAND_COST_Text_Text = m_NKM_UI_GAME_TACTICAL_COMMAND_COST_Text.GetComponentInChildren<Text>();
	}

	public void PointerDown(PointerEventData e)
	{
		if (m_NKMTacticalCommandTemplet != null && m_NKMTacticalCommandData != null)
		{
			NKCUITooltip.Instance.Open(m_NKMTacticalCommandTemplet, m_NKMTacticalCommandData.m_Level, e.position);
		}
	}

	public void Click()
	{
		if (m_NKMTacticalCommandTemplet != null && m_NKMTacticalCommandData != null && CanUse())
		{
			m_NKCGameHud.GetGameClient().Send_Packet_GAME_TACTICAL_COMMAND_REQ(m_NKMTacticalCommandTemplet.m_TCID);
			m_NKCGameHud.GetGameClient().AddCostHolderTC(m_NKMTacticalCommandTemplet.m_TCID, m_NKMTacticalCommandTemplet.GetNeedCost(m_NKMTacticalCommandData));
			m_NKCGameHud.SetRespawnCost();
			SetActive(bActive: false);
		}
	}

	public void Init()
	{
		m_NKMTacticalCommandTemplet = null;
		NKCAssetResourceData nKCAssetResourceData = null;
		nKCAssetResourceData = NKCResourceUtility.GetAssetResource("AB_UI_TACTICAL_COMMAND_ICON", "ICON_TC_NO_SKILL_ICON");
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_Panel.SetActive(value: true);
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_GRAY_Panel.SetActive(value: true);
		if (nKCAssetResourceData != null)
		{
			m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_Panel_Image.sprite = nKCAssetResourceData.GetAsset<Sprite>();
			m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_GRAY_Panel_Image.sprite = nKCAssetResourceData.GetAsset<Sprite>();
			m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_ADD_Panel_Image.sprite = nKCAssetResourceData.GetAsset<Sprite>();
		}
		else
		{
			m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_Panel_Image.sprite = null;
			m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_GRAY_Panel_Image.sprite = null;
			m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_ADD_Panel_Image.sprite = null;
		}
		if (m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_BG_Panel.activeSelf)
		{
			m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_BG_Panel.SetActive(value: false);
		}
		if (m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_Panel.activeSelf)
		{
			m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_Panel.SetActive(value: false);
		}
		m_bEventControl = false;
		m_NeedCostBefore = -1;
		SetActive(bActive: false);
	}

	public void SetDeckSprite(NKMTacticalCommandTemplet cNKMTacticalCommandTemplet)
	{
		if (cNKMTacticalCommandTemplet == null)
		{
			Init();
			return;
		}
		m_NKMTacticalCommandTemplet = cNKMTacticalCommandTemplet;
		SetActive(bActive: true);
		if (m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_ADD_Panel.activeSelf)
		{
			m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_ADD_Panel.SetActive(value: false);
		}
		NKCAssetResourceData nKCAssetResourceData = null;
		m_StringBuilder.Remove(0, m_StringBuilder.Length);
		m_StringBuilder.Append(m_NKMTacticalCommandTemplet.m_TCIconName);
		nKCAssetResourceData = NKCResourceUtility.GetAssetResource("AB_UI_TACTICAL_COMMAND_ICON", m_StringBuilder.ToString());
		if (nKCAssetResourceData != null)
		{
			m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_Panel_Image.sprite = nKCAssetResourceData.GetAsset<Sprite>();
			m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_ADD_Panel_Image.sprite = nKCAssetResourceData.GetAsset<Sprite>();
			m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_GRAY_Panel_Image.sprite = nKCAssetResourceData.GetAsset<Sprite>();
		}
		else
		{
			m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_Panel_Image.sprite = null;
			m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_ADD_Panel_Image.sprite = null;
			m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_GRAY_Panel_Image.sprite = null;
		}
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_Panel.SetActive(value: true);
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_GRAY_Panel.SetActive(value: true);
	}

	public void SetDeckData(float fRespawnCostNow, NKMTacticalCommandData cNKMTacticalCommandData)
	{
		float num = 1f;
		if (m_NKMTacticalCommandTemplet != null)
		{
			m_NKMTacticalCommandData = cNKMTacticalCommandData;
			float needCost = m_NKMTacticalCommandTemplet.GetNeedCost(m_NKMTacticalCommandData);
			float num2 = fRespawnCostNow / needCost;
			float num3 = 1f - cNKMTacticalCommandData.m_fCoolTimeNow / m_NKMTacticalCommandTemplet.m_fCoolTime;
			num = ((!(num2 < num3)) ? num3 : num2);
			if (m_NeedCostBefore != (int)needCost)
			{
				m_NeedCostBefore = (int)needCost;
				m_StringBuilder.Remove(0, m_StringBuilder.Length);
				m_StringBuilder.Append(m_NeedCostBefore);
				m_NKM_UI_GAME_TACTICAL_COMMAND_COST_Text_Text.text = m_StringBuilder.ToString();
			}
		}
		m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_Panel_Image.fillAmount = num;
		if (num < 1f)
		{
			if (m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_Panel.activeSelf)
			{
				m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_Panel.SetActive(value: false);
			}
			if (!m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_GRAY_Panel.activeSelf)
			{
				m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_GRAY_Panel.SetActive(value: true);
			}
		}
		else
		{
			if (!m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_Panel.activeSelf)
			{
				m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_Panel.SetActive(value: true);
				if (m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_Animator.gameObject.activeInHierarchy)
				{
					m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_Animator.Play("NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_READY", -1, 0f);
				}
			}
			if (m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_GRAY_Panel.activeSelf)
			{
				m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_ICON_GRAY_Panel.SetActive(value: false);
			}
		}
		if (num >= 1f)
		{
			if (m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_BG_Panel.activeSelf)
			{
				m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_BG_Panel.SetActive(value: false);
			}
			if (m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_Panel.activeSelf)
			{
				m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_Panel.SetActive(value: false);
			}
			if (!m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_UNIT_BORDER.activeSelf)
			{
				m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_UNIT_BORDER.SetActive(value: true);
			}
		}
		else
		{
			if (!m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_BG_Panel.activeSelf)
			{
				m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_BG_Panel.SetActive(value: true);
			}
			if (!m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_Panel.activeSelf)
			{
				m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_Panel.SetActive(value: true);
			}
			if (m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_UNIT_BORDER.activeSelf)
			{
				m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_UNIT_BORDER.SetActive(value: false);
			}
		}
	}

	public bool CanUse()
	{
		if (m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME_Panel_Image.fillAmount >= 1f)
		{
			return true;
		}
		return false;
	}

	public void ReturnTacticalCommandDeck()
	{
		SetActive(bActive: true);
	}

	public void SetActive(bool bActive, bool bEventControl = false)
	{
		if (!bActive || !m_bEventControl || bEventControl)
		{
			m_bEventControl = bEventControl;
			if (m_NKM_UI_TACTICAL_COMMAND_DECK.activeSelf != bActive)
			{
				m_NKM_UI_TACTICAL_COMMAND_DECK.SetActive(bActive);
			}
			if (m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME.activeSelf != bActive)
			{
				m_NKM_UI_GAME_TACTICAL_COMMAND_DECK_CARD_COOL_TIME.SetActive(bActive);
			}
		}
	}
}
