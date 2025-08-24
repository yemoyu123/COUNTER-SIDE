using System.Collections.Generic;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCPopupGauntletLeagueBanList : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_GAUNTLET";

	private const string UI_ASSET_NAME = "NKM_UI_GAUNTLET_LEAGUE_POPUP_BAN_LIST";

	private static NKCPopupGauntletLeagueBanList m_Instance;

	[Header("Ban Unit Slot")]
	public List<NKCUIUnitSelectListSlot> m_lstBanUnitSlots;

	public List<Image> m_lstBanShips;

	[Header("Animator")]
	public Animator m_AniGlobalBanUnitAnimator;

	private bool m_bCheckAniPlaying;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "PopupGauntletBanList";

	public static NKCPopupGauntletLeagueBanList Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGauntletLeagueBanList>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_LEAGUE_POPUP_BAN_LIST", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupGauntletLeagueBanList>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public void InitUI()
	{
		foreach (NKCUIUnitSelectListSlot lstBanUnitSlot in m_lstBanUnitSlots)
		{
			lstBanUnitSlot.Init();
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open()
	{
		List<int> globalBanUnitList = NKCBanManager.GetGlobalBanUnitList(NKM_UNIT_TYPE.NUT_NORMAL);
		List<int> globalBanUnitList2 = NKCBanManager.GetGlobalBanUnitList(NKM_UNIT_TYPE.NUT_SHIP);
		List<int> list = new List<int>();
		foreach (int item in globalBanUnitList2)
		{
			list.Add(NKCBanManager.ConvertShipGroupIdToShipId(item));
		}
		Open(globalBanUnitList, list);
	}

	public void Open(List<int> lstBanedUnitIDs, List<int> lstBanedShipIDs)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (null != m_AniGlobalBanUnitAnimator)
		{
			m_AniGlobalBanUnitAnimator.Play("BAN_LIST_INTRO");
		}
		for (int i = 0; i < m_lstBanUnitSlots.Count; i++)
		{
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(lstBanedUnitIDs[i]);
			m_lstBanUnitSlots[i].SetDataForBan(nKMUnitTempletBase, bEnableLayoutElement: true, null);
			NKCUtil.SetGameobjectActive(m_lstBanUnitSlots[i].gameObject, nKMUnitTempletBase != null);
		}
		for (int j = 0; j < m_lstBanShips.Count; j++)
		{
			NKMUnitTempletBase nKMUnitTempletBase2 = NKMUnitTempletBase.Find(lstBanedShipIDs[j]);
			NKCUtil.SetImageSprite(m_lstBanShips[j], NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, nKMUnitTempletBase2), bDisableIfSpriteNull: true);
			NKCUtil.SetGameobjectActive(m_lstBanShips[j].gameObject, nKMUnitTempletBase2 != null);
		}
		NKCSoundManager.PlaySound("FX_UI_TITLE_IN_TEST", 1f, 0f, 0f);
		UIOpened();
		m_bCheckAniPlaying = true;
	}

	private void Update()
	{
		if (m_bCheckAniPlaying && 1f <= m_AniGlobalBanUnitAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime)
		{
			m_bCheckAniPlaying = false;
			Close();
		}
	}
}
