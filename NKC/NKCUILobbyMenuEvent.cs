using System;
using System.Collections.Generic;
using NKM;
using NKM.Event;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUILobbyMenuEvent : MonoBehaviour
{
	[Serializable]
	public struct EventTypeObject
	{
		public string type;

		public GameObject obj;

		public Text text;
	}

	public delegate void OnButton(NKMEventTabTemplet tabTemplet);

	public NKCUIComStateButton m_btnEvent;

	public Image m_imgBG;

	public Text m_lbEventTitle;

	public List<EventTypeObject> m_lstTypeObject;

	private OnButton m_dOnButton;

	private NKMEventTabTemplet m_NKMEventTabTemplet;

	public void SetData(NKMEventTabTemplet eventTabTemplet, OnButton onButton)
	{
		if (eventTabTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_btnEvent.PointerClick.RemoveAllListeners();
		m_btnEvent.PointerClick.AddListener(OnClick);
		m_dOnButton = onButton;
		m_NKMEventTabTemplet = eventTabTemplet;
		SetTypeObject(eventTabTemplet);
		NKCUtil.SetLabelText(m_lbEventTitle, eventTabTemplet.GetTitle());
		NKCUtil.SetImageSprite(m_imgBG, NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("ab_ui_nkm_ui_event_texture", eventTabTemplet.m_EventTabImage)));
	}

	private void SetTypeObject(NKMEventTabTemplet tabTemplet)
	{
		foreach (EventTypeObject item in m_lstTypeObject)
		{
			if (string.Equals(item.type, tabTemplet.m_LobbyBannerType, StringComparison.InvariantCultureIgnoreCase))
			{
				string text = NKCStringTable.GetString(tabTemplet.m_LobbyBannerText);
				if (string.IsNullOrEmpty(text))
				{
					NKCUtil.SetGameobjectActive(item.obj, bValue: false);
					continue;
				}
				NKCUtil.SetGameobjectActive(item.obj, bValue: true);
				NKCUtil.SetLabelText(item.text, text);
			}
			else
			{
				NKCUtil.SetGameobjectActive(item.obj, bValue: false);
			}
		}
	}

	private void OnClick()
	{
		m_dOnButton?.Invoke(m_NKMEventTabTemplet);
	}
}
