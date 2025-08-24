using System;
using UnityEngine;

namespace NKC.UI;

public class NKCJukeBoxSlot : MonoBehaviour
{
	public delegate void OnClick(int idx);

	private enum ButtonStat
	{
		NORMAL,
		LOCK,
		SELECTED
	}

	public NKCUIComStateButton m_csbtnClick;

	public NKCComText[] m_lbTitle;

	public NKCComText m_lbTime;

	public GameObject m_objFavorite;

	public GameObject m_objNew;

	[Header("버튼 상태")]
	public GameObject m_objBtnStatNormal;

	public GameObject m_objBtnStatSelected;

	public GameObject m_objBtnStatLock;

	private ButtonStat m_btnStat;

	private OnClick dClick;

	private int m_idx;

	private string m_strUnlockString;

	public int Index => m_idx;

	public void Init()
	{
		NKCUtil.SetBindFunction(m_csbtnClick, OnClickSlot);
		m_btnStat = ButtonStat.NORMAL;
	}

	public void SetData(NKCBGMInfoTemplet bgmTemplet, OnClick callback, bool bFavorite, bool bOpen, bool bNew)
	{
		if (bgmTemplet == null)
		{
			return;
		}
		NKCComText[] lbTitle = m_lbTitle;
		for (int i = 0; i < lbTitle.Length; i++)
		{
			NKCUtil.SetLabelText(lbTitle[i], NKCStringTable.GetString(bgmTemplet.m_BgmNameStringID));
		}
		if (!bOpen)
		{
			SetButtonStat(ButtonStat.LOCK);
			NKCUtil.SetGameobjectActive(m_objNew, bValue: false);
			SetFavorite(bSet: false);
			m_strUnlockString = bgmTemplet.m_BgmUnlcokStringID;
			return;
		}
		SetButtonStat(ButtonStat.NORMAL);
		SetFavorite(bFavorite);
		NKCUtil.SetGameobjectActive(m_objNew, bNew);
		AudioClip audioClip = NKCUIJukeBox.GetAudioClip(bgmTemplet.m_BgmAssetID);
		if (null != audioClip)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(audioClip.length);
			NKCUtil.SetLabelText(m_lbTime, $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
		}
		SetStat(bPlay: false);
		m_idx = bgmTemplet.Key;
		dClick = callback;
		m_strUnlockString = "";
	}

	public void SetStat(bool bPlay)
	{
		if (m_btnStat != ButtonStat.LOCK)
		{
			if (bPlay)
			{
				SetButtonStat(ButtonStat.SELECTED);
			}
			else
			{
				SetButtonStat(ButtonStat.NORMAL);
			}
		}
	}

	public void SetFavorite(bool bSet)
	{
		if (!bSet || m_btnStat != ButtonStat.LOCK)
		{
			NKCUtil.SetGameobjectActive(m_objFavorite, bSet);
		}
	}

	private void OnClickSlot()
	{
		if (m_btnStat == ButtonStat.LOCK)
		{
			if (!string.IsNullOrEmpty(m_strUnlockString))
			{
				NKCPopupMessageManager.AddPopupMessage(string.Format(NKCUtilString.GET_STRING_JUKEBOX_SLOT_UNLCOK_DESC_01, NKCStringTable.GetString(m_strUnlockString)));
			}
			else
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_JUKEBOX_BLOCK_SLOT_MSG);
			}
		}
		else
		{
			dClick?.Invoke(m_idx);
		}
	}

	private void SetButtonStat(ButtonStat stat)
	{
		NKCUtil.SetGameobjectActive(m_objBtnStatNormal, stat == ButtonStat.NORMAL);
		NKCUtil.SetGameobjectActive(m_objBtnStatSelected, stat == ButtonStat.SELECTED);
		NKCUtil.SetGameobjectActive(m_objBtnStatLock, stat == ButtonStat.LOCK);
		m_btnStat = stat;
	}
}
