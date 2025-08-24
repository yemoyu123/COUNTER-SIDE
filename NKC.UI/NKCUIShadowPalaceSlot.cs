using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Mode;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIShadowPalaceSlot : MonoBehaviour
{
	public delegate void OnTouchSlot(int palaceID);

	[Header("Info")]
	public Text m_txtNumber;

	public Text m_txtTime;

	public Image m_imgBoss;

	[Header("State")]
	public GameObject m_objLock;

	public GameObject m_objCurrent;

	[Header("Animation")]
	public Animator m_aniSelect;

	[Header("deco")]
	public GameObject m_objLeftLine;

	public GameObject m_objRightLine;

	[Header("button")]
	public NKCUIComStateButton m_btn;

	private OnTouchSlot dOnTouchSlot;

	private NKMShadowPalaceTemplet m_templet;

	private NKMPalaceData m_data;

	public int PalaceID
	{
		get
		{
			if (m_templet == null)
			{
				return 0;
			}
			return m_templet.PALACE_ID;
		}
	}

	public void Init()
	{
		m_btn?.PointerClick.RemoveAllListeners();
		m_btn?.PointerClick.AddListener(OnTouch);
	}

	public void SetData(NKMShadowPalaceTemplet palaceTemplet, NKMPalaceData palaceData, OnTouchSlot onTouchSlot)
	{
		m_templet = palaceTemplet;
		m_data = palaceData;
		dOnTouchSlot = onTouchSlot;
		NKCUtil.SetLabelText(m_txtNumber, string.Format("#" + palaceTemplet.PALACE_NUM_UI));
		List<NKMShadowBattleTemplet> battleTemplets = NKMShadowPalaceManager.GetBattleTemplets(palaceTemplet.PALACE_ID);
		if (battleTemplets == null)
		{
			return;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(battleTemplets.Last().DUNGEON_ID);
		Sprite sp = null;
		if (dungeonTempletBase != null)
		{
			sp = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_UNIT", palaceTemplet.PALACE_IMG);
		}
		NKCUtil.SetImageSprite(m_imgBoss, sp, bDisableIfSpriteNull: true);
		string msg = "-:--:--";
		if (palaceData != null && palaceData.dungeonDataList.Count == battleTemplets.Count)
		{
			int num = 0;
			for (int i = 0; i < palaceData.dungeonDataList.Count; i++)
			{
				NKMPalaceDungeonData nKMPalaceDungeonData = palaceData.dungeonDataList[i];
				num += nKMPalaceDungeonData.bestTime;
			}
			if (num > 0)
			{
				msg = NKCUtilString.GetTimeSpanString(TimeSpan.FromSeconds(num));
			}
		}
		NKCUtil.SetLabelText(m_txtTime, msg);
	}

	public void SetLock(bool bLock)
	{
		NKCUtil.SetGameobjectActive(m_objLock, bLock);
	}

	public void SetProgress(bool bCurrent)
	{
		NKCUtil.SetGameobjectActive(m_objCurrent, bCurrent);
	}

	public void SetLine(bool bFirst, bool bLast)
	{
		NKCUtil.SetGameobjectActive(m_objLeftLine, !bFirst);
		NKCUtil.SetGameobjectActive(m_objRightLine, !bLast);
	}

	public void PlaySelect(bool bSelect, bool bEffect)
	{
		if (bSelect)
		{
			if (bEffect)
			{
				m_aniSelect.Play("NKM_UI_SHADOW_PALACE_SLOT_SELECT_INTRO");
			}
			else
			{
				m_aniSelect.Play("NKM_UI_SHADOW_PALACE_SLOT_SELECT_IDLE");
			}
		}
		else if (bEffect)
		{
			m_aniSelect.Play("NKM_UI_SHADOW_PALACE_SLOT_SELECT_OUTRO");
		}
		else
		{
			m_aniSelect.Play("NKM_UI_SHADOW_PALACE_SLOT_BASE");
		}
	}

	public void OnTouch()
	{
		dOnTouchSlot?.Invoke(m_templet.PALACE_ID);
	}
}
