using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Worldmap;

public class NKCPopupWorldmapEventOKCancel : NKCUIBase
{
	public delegate void OnClickOKOrCancel();

	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_WORLD_MAP_RENEWAL";

	public const string UI_ASSET_NAME = "NKM_UI_WORLD_MAP_POPUP_EventOKCancel";

	public EventTrigger m_evtBG;

	public NKCUIComStateButton m_csbtnOK;

	public NKCUIComStateButton m_csbtnCancel;

	public Text m_lbDesc;

	public Text m_lbCityLevel;

	public Image m_imgCityExp;

	public Text m_lbCityName;

	public Image m_imgEventType;

	public Text m_lbEventLevel;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private OnClickOKOrCancel m_dOnClickOK;

	private OnClickOKOrCancel m_dOnClickCancel;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "PopupWorldmapEventOKCancel";

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		m_csbtnOK.PointerClick.RemoveAllListeners();
		m_csbtnOK.PointerClick.AddListener(OnClickOK);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		m_csbtnCancel.PointerClick.RemoveAllListeners();
		m_csbtnCancel.PointerClick.AddListener(OnClickCancel);
		if (m_evtBG != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(OnClickBG);
			m_evtBG.triggers.Add(entry);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnClickBG(BaseEventData cBaseEventData)
	{
		Close();
	}

	private void OnClickOK()
	{
		if (m_dOnClickOK != null)
		{
			m_dOnClickOK();
		}
	}

	private void OnClickCancel()
	{
		if (m_dOnClickCancel != null)
		{
			m_dOnClickCancel();
		}
	}

	private void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public void Open(string desc, int cityLevel, float cityExpPercent, string cityName, NKM_WORLDMAP_EVENT_TYPE eventType, int eventLevel, OnClickOKOrCancel dOnClickOK, OnClickOKOrCancel dOnClickCancel = null)
	{
		m_dOnClickOK = dOnClickOK;
		m_dOnClickCancel = dOnClickCancel;
		NKCUtil.SetLabelText(m_lbDesc, desc);
		NKCUtil.SetLabelText(m_lbCityLevel, cityLevel.ToString());
		NKCUtil.SetLabelText(m_lbCityName, cityName);
		m_imgCityExp.fillAmount = cityExpPercent;
		switch (eventType)
		{
		case NKM_WORLDMAP_EVENT_TYPE.WET_RAID:
			m_imgEventType.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_RENEWAL_SPRITE", "NKM_UI_WORLD_MAP_RENEWAL_EVENT_TYPE_ICON_RAID");
			NKCUtil.SetLabelText(m_lbEventLevel, string.Format(NKCUtilString.GET_STRING_WORLDMAP_EVENT_POPUP_OK_CANCEL_RAID_LEVEL, eventLevel));
			break;
		case NKM_WORLDMAP_EVENT_TYPE.WET_DIVE:
			m_imgEventType.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_RENEWAL_SPRITE", "NKM_UI_WORLD_MAP_RENEWAL_EVENT_TYPE_ICON_DIVE");
			NKCUtil.SetLabelText(m_lbEventLevel, string.Format(NKCUtilString.GET_STRING_WORLDMAP_EVENT_POPUP_OK_CANCEL_DIVE_LEVEL, eventLevel));
			break;
		}
		m_NKCUIOpenAnimator.PlayOpenAni();
		UIOpened();
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}
}
