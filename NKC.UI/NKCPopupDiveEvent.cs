using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupDiveEvent : NKCUIBase
{
	public delegate void OnCloseCallBack();

	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_WORLD_MAP_DIVE";

	public const string UI_ASSET_NAME = "NKM_UI_DIVE_EVENT_POPUP";

	public Text m_NKM_UI_DIVE_EVENT_POPUP_TITLE_TEXT;

	public Text m_NKM_UI_DIVE_EVENT_POPUP_SUBTITLE_TEXT;

	public RawImage m_NKM_UI_DIVE_EVENT_POPUP_THUMBNAIL;

	public GameObject m_NKM_UI_DIVE_EVENT_POPUP_ICON_SLOT_AREA;

	public GameObject m_NKM_UI_DIVE_EVENT_POPUP_FX;

	public EventTrigger m_evtBG;

	private NKCUISlot m_NKCUISlot;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private OnCloseCallBack m_OnCloseCallBack;

	private bool m_bAutoClose;

	private float m_fElapsedTimeToAutoClose;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_DIVE_EVENT_POPUP;

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		base.gameObject.SetActive(value: false);
		m_evtBG.triggers.Clear();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_evtBG.triggers.Add(entry);
	}

	public void Update()
	{
		if (!base.IsOpen)
		{
			return;
		}
		m_NKCUIOpenAnimator.Update();
		if (m_bAutoClose)
		{
			float num = Time.deltaTime;
			if (num > NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f)
			{
				num = NKCScenManager.GetScenManager().GetFixedFrameTime() * 2f;
			}
			m_fElapsedTimeToAutoClose += num;
			if (m_fElapsedTimeToAutoClose >= 5f / NKCClientConst.DiveAutoSpeed)
			{
				m_bAutoClose = false;
				Close();
			}
		}
	}

	public void Open(bool bAutoClose, NKMDiveSlot cNKMDiveSlot, NKMRewardData cRewardData, OnCloseCallBack _OnCloseCallBack = null)
	{
		if (cNKMDiveSlot == null)
		{
			return;
		}
		m_bAutoClose = bAutoClose;
		m_fElapsedTimeToAutoClose = 0f;
		if (cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_ITEM)
		{
			m_NKM_UI_DIVE_EVENT_POPUP_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_EVENT_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_EVENT_ITEM");
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_FX, bValue: true);
		}
		else if (cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_UNIT)
		{
			m_NKM_UI_DIVE_EVENT_POPUP_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_EVENT_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_EVENT_RESCUE_SIGNAL");
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_FX, bValue: true);
		}
		else if (cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_REPAIR)
		{
			m_NKM_UI_DIVE_EVENT_POPUP_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_EVENT_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_EVENT_REPAIR_KIT");
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_FX, bValue: true);
		}
		else if (cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_SUPPLY)
		{
			m_NKM_UI_DIVE_EVENT_POPUP_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_EVENT_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_EVENT_LOST_CONTAINER");
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_FX, bValue: true);
		}
		else if (cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_LOSTSHIP_ITEM)
		{
			m_NKM_UI_DIVE_EVENT_POPUP_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_EVENT_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_EVENT_ITEM");
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_FX, bValue: true);
		}
		else if (cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_LOSTSHIP_UNIT)
		{
			m_NKM_UI_DIVE_EVENT_POPUP_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_EVENT_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_EVENT_RESCUE_SIGNAL");
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_FX, bValue: true);
		}
		else if (cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_LOSTSHIP_REPAIR)
		{
			m_NKM_UI_DIVE_EVENT_POPUP_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_EVENT_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_EVENT_REPAIR_KIT");
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_FX, bValue: true);
		}
		else if (cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_LOSTSHIP_SUPPLY)
		{
			m_NKM_UI_DIVE_EVENT_POPUP_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_EVENT_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_EVENT_LOST_CONTAINER");
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_FX, bValue: true);
		}
		else if (cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_BEACON_DUNGEON)
		{
			m_NKM_UI_DIVE_EVENT_POPUP_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_EVENT_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_EVENT_ENEMY_ATTACK");
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_FX, bValue: false);
		}
		else if (cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_BEACON_BLANK)
		{
			m_NKM_UI_DIVE_EVENT_POPUP_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_EVENT_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_EVENT_SAFETY");
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_FX, bValue: false);
		}
		else if (cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_BEACON_ITEM)
		{
			m_NKM_UI_DIVE_EVENT_POPUP_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_EVENT_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_EVENT_ITEM");
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_FX, bValue: true);
		}
		else if (cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_BEACON_UNIT)
		{
			m_NKM_UI_DIVE_EVENT_POPUP_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_EVENT_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_EVENT_RESCUE_SIGNAL");
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_FX, bValue: true);
		}
		else if (cNKMDiveSlot.EventType == NKM_DIVE_EVENT_TYPE.NDET_BEACON_STORM)
		{
			m_NKM_UI_DIVE_EVENT_POPUP_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_EVENT_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_EVENT_WEATHER");
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_FX, bValue: false);
		}
		else
		{
			if (cNKMDiveSlot.EventType != NKM_DIVE_EVENT_TYPE.NDET_ARTIFACT)
			{
				m_OnCloseCallBack = null;
				return;
			}
			m_NKM_UI_DIVE_EVENT_POPUP_THUMBNAIL.texture = NKCResourceUtility.GetOrLoadAssetResource<Texture>("AB_UI_NKM_UI_WORLD_MAP_DIVE_EVENT_THUMBNAIL", "NKM_UI_WORLD_MAP_DIVE_EVENT_ITEM");
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_FX, bValue: true);
		}
		NKCUtilString.GetDiveEventText(cNKMDiveSlot.EventType, out var title, out var subTitle);
		m_NKM_UI_DIVE_EVENT_POPUP_TITLE_TEXT.text = title;
		m_NKM_UI_DIVE_EVENT_POPUP_SUBTITLE_TEXT.text = subTitle;
		m_OnCloseCallBack = _OnCloseCallBack;
		if (cRewardData != null)
		{
			if (m_NKCUISlot == null)
			{
				m_NKCUISlot = NKCUISlot.GetNewInstance(m_NKM_UI_DIVE_EVENT_POPUP_ICON_SLOT_AREA.transform);
				m_NKCUISlot.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
				m_NKCUISlot.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
				m_NKCUISlot.transform.localPosition = new Vector3(0f, 0f, 0f);
				m_NKCUISlot.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			List<NKCUISlot.SlotData> list = NKCUISlot.MakeSlotDataListFromReward(cRewardData);
			if (list.Count > 0)
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_ICON_SLOT_AREA, bValue: true);
				m_NKCUISlot.SetData(list[0]);
				NKCUtil.SetGameobjectActive(m_NKCUISlot, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_ICON_SLOT_AREA, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_DIVE_EVENT_POPUP_ICON_SLOT_AREA, bValue: false);
		}
		UIOpened();
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
		if (m_OnCloseCallBack != null)
		{
			m_OnCloseCallBack();
		}
		m_OnCloseCallBack = null;
	}
}
