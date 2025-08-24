using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.UI;

public class NKCPopupInGameEmoticon : NKCUIBase
{
	public delegate void dOnClose();

	public EventTrigger m_etBG;

	public NKCUIComToggle m_ctglBlock;

	public GameObject m_objBlocking;

	public List<NKCPopupEmoticonSlotComment> m_lstNKCPopupEmoticonSlotComment = new List<NKCPopupEmoticonSlotComment>();

	public List<NKCPopupEmoticonSlotSD> m_lstNKCPopupEmoticonSlotSD = new List<NKCPopupEmoticonSlotSD>();

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "NKCPopupInGameEmoticon";

	public void Init()
	{
		m_etBG.triggers.Clear();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_etBG.triggers.Add(entry);
		m_ctglBlock.OnValueChanged.RemoveAllListeners();
		m_ctglBlock.OnValueChanged.AddListener(OnChangedValueBlockState);
		for (int num = 0; num < m_lstNKCPopupEmoticonSlotComment.Count; num++)
		{
			m_lstNKCPopupEmoticonSlotComment[num].SetClickEvent(OnClickEmoticon);
		}
		for (int num2 = 0; num2 < m_lstNKCPopupEmoticonSlotSD.Count; num2++)
		{
			m_lstNKCPopupEmoticonSlotSD[num2].SetClickEvent(OnClickEmoticon);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnClickEmoticon(NKCUISlot.SlotData slotData, bool bLocked)
	{
		OnClickEmoticon(slotData.ID);
	}

	private void OnClickEmoticon(int emoticonID)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null && !gameOptionData.UseEmoticonBlock)
		{
			NKCPacketSender.Send_NKMPacket_GAME_EMOTICON_REQ(emoticonID);
		}
	}

	private void OnChangedValueBlockState(bool bSet)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData == null)
		{
			return;
		}
		gameOptionData.UseEmoticonBlock = bSet;
		gameOptionData.Save();
		NKCUtil.SetGameobjectActive(m_objBlocking, gameOptionData.UseEmoticonBlock);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
		{
			NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
			if (gameClient != null && gameClient.GetGameHud() != null && gameClient.GetGameHud().GetNKCGameHudEmoticon() != null)
			{
				gameClient.GetGameHud().GetNKCGameHudEmoticon().SetBlockUI();
			}
		}
		if (bSet)
		{
			Close();
		}
	}

	public void Open(List<int> lstEmoticonID_SD, List<int> lstEmoticonIDComment)
	{
		bool flag = false;
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			flag = gameOptionData.UseEmoticonBlock;
		}
		m_ctglBlock.Select(flag, bForce: true);
		NKCUtil.SetGameobjectActive(m_objBlocking, flag);
		UIOpened();
		if (lstEmoticonID_SD != null)
		{
			for (int i = 0; i < m_lstNKCPopupEmoticonSlotSD.Count; i++)
			{
				if (lstEmoticonID_SD.Count > i)
				{
					m_lstNKCPopupEmoticonSlotSD[i].SetUI(lstEmoticonID_SD[i]);
				}
			}
		}
		if (lstEmoticonIDComment == null)
		{
			return;
		}
		for (int j = 0; j < m_lstNKCPopupEmoticonSlotComment.Count; j++)
		{
			if (lstEmoticonIDComment.Count > j)
			{
				m_lstNKCPopupEmoticonSlotComment[j].SetUI(lstEmoticonIDComment[j]);
			}
		}
	}
}
