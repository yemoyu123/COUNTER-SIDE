using System.Collections.Generic;
using ClientPacket.Common;
using NKC.UI.Collection;
using NKC.UI.Component;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletBattleRecordInfo : MonoBehaviour
{
	public NKCUILeagueTier m_NKCUILeagueTier;

	public Text m_lbLevel;

	public Text m_lbUserNickName;

	public Text m_lbFriendCode;

	public Text m_lbGameResult;

	public Text m_lbScore;

	public Text m_lbAddScore;

	public Image m_imgShip;

	public NKCUIComButton m_shipButton;

	public Image m_imgShipFace;

	public NKCUIComButton m_shipFaceButton;

	public NKCUIOperatorDeckSlot m_operatorSlot;

	public NKCUIShipInfoSummary m_NKCUIShipInfoSummary;

	public GameObject m_objUnitRoot;

	public List<NKCDeckViewUnitSlot> m_lstNKCDeckViewUnitSlot;

	public GameObject m_objGuild;

	public NKCUIGuildBadge m_badgeUI;

	public Text m_lbGuildName;

	public GameObject m_objLocalBan;

	public NKCDeckViewUnitSlot m_slotLocalBan;

	public GameObject m_objGlobalBan;

	public List<NKCDeckViewUnitSlot> m_lstGlobalBan;

	public NKCUIComTitlePanel m_title;

	public Image m_imgGlobalBanShip;

	private List<NKMUnitData> m_lstNKMUnitData;

	private NKMUnitData m_shipUnitData;

	private NKMOperator m_operatorData;

	private List<NKMEquipItemData> m_lstEquip;

	public void Init()
	{
		m_lstNKMUnitData = new List<NKMUnitData>();
		for (int i = 0; i < m_lstNKCDeckViewUnitSlot.Count; i++)
		{
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_lstNKCDeckViewUnitSlot[i];
			if (nKCDeckViewUnitSlot != null)
			{
				nKCDeckViewUnitSlot.Init(i);
				nKCDeckViewUnitSlot.m_NKCUIComButton.PointerClick.RemoveAllListeners();
				int index = i;
				nKCDeckViewUnitSlot.m_NKCUIComButton.PointerClick.AddListener(delegate
				{
					OnClickViewUnitSlot(index);
				});
			}
		}
		if (m_slotLocalBan != null)
		{
			m_slotLocalBan.Init(0);
		}
		for (int num = 0; num < m_lstGlobalBan.Count; num++)
		{
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot2 = m_lstGlobalBan[num];
			if (nKCDeckViewUnitSlot2 != null)
			{
				nKCDeckViewUnitSlot2.Init(num);
			}
		}
		if (m_shipButton != null)
		{
			m_shipButton.enabled = true;
			m_shipButton.PointerClick.RemoveAllListeners();
			m_shipButton.PointerClick.AddListener(OnClickShipSlot);
		}
		if (m_operatorSlot != null)
		{
			m_operatorSlot.Init(OnClickOperatorSlot);
		}
		NKCUtil.SetButtonClickDelegate(m_shipFaceButton, OnClickShipSlot);
	}

	public void SetLeagueInfo(NKM_GAME_TYPE gameType, int seasonId, int tier)
	{
		m_NKCUILeagueTier.SetUI(NKCPVPManager.GetTierIconByTier(gameType, seasonId, tier), NKCPVPManager.GetTierNumberByTier(gameType, seasonId, tier));
	}

	public void SetUserInfo(int level, string nickName, long friendCode, int score, int titleId, long guildUid, long guildBadgeId, string guildName, bool bOpponent, bool activeScore)
	{
		m_lbLevel.text = NKCStringTable.GetString("SI_DP_LEVEL_ONE_PARAM", level);
		m_lbUserNickName.text = NKCUtilString.GetUserNickname(nickName, bOpponent);
		m_lbFriendCode.text = NKCUtilString.GetFriendCode(friendCode, bOpponent);
		m_lbScore.text = score.ToString();
		m_lbAddScore.text = "";
		m_title?.SetData(titleId);
		if (m_objGuild != null)
		{
			NKCUtil.SetGameobjectActive(m_objGuild, guildUid > 0);
			if (m_objGuild.activeSelf)
			{
				m_badgeUI.SetData(guildBadgeId, bOpponent);
				NKCUtil.SetLabelText(m_lbGuildName, NKCUtilString.GetUserGuildName(guildName, bOpponent));
			}
		}
		NKCUtil.SetGameobjectActive(m_lbScore, activeScore);
	}

	public void SetPvpResult(PVP_RESULT pvpResult)
	{
		switch (pvpResult)
		{
		case PVP_RESULT.WIN:
			m_lbGameResult.text = NKCUtilString.GET_STRING_WIN;
			m_lbGameResult.color = NKCUtil.GetColor("#FFDF5D");
			break;
		case PVP_RESULT.LOSE:
			m_lbGameResult.text = NKCUtilString.GET_STRING_LOSE;
			m_lbGameResult.color = NKCUtil.GetColor("#FF4747");
			break;
		case PVP_RESULT.DRAW:
			m_lbGameResult.text = NKCUtilString.GET_STRING_DRAW;
			m_lbGameResult.color = NKCUtil.GetColor("#D4D4D4");
			break;
		}
	}

	private bool IsDetailHistoryOpened()
	{
		return NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_DETAIL_HISTORY);
	}

	public void SetDeckInfo(NKMAsyncDeckData deckData, List<int> banUnitIdList, List<int> banShipIdList, bool bDraftBanMode)
	{
		m_lstNKMUnitData.Clear();
		m_shipUnitData = null;
		m_operatorData = null;
		m_lstEquip = null;
		if (deckData != null)
		{
			for (int i = 0; i < m_lstNKCDeckViewUnitSlot.Count && i < 8; i++)
			{
				NKMUnitData nKMUnitData = null;
				if (deckData.units.Count > i && deckData.units[i] != null)
				{
					nKMUnitData = new NKMUnitData();
					nKMUnitData.FillDataFromAsyncUnitData(deckData.units[i]);
					nKMUnitData.m_UnitUID = i;
				}
				NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_lstNKCDeckViewUnitSlot[i];
				nKCDeckViewUnitSlot.SetData(nKMUnitData);
				nKCDeckViewUnitSlot.SetUpBanData(nKMUnitData, deckData.unitBanData, deckData.unitUpData, i == deckData.leaderIndex);
				m_lstNKMUnitData.Add(nKMUnitData);
			}
			NKCUtil.SetGameobjectActive(m_imgShip, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKCUIShipInfoSummary, bValue: true);
			if (deckData.ship != null)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(deckData.ship.unitId);
				if (unitTempletBase != null)
				{
					Sprite sp = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase);
					if (unitTempletBase.IsShip())
					{
						NKCUtil.SetImageSprite(m_imgShip, sp);
						NKCUtil.SetGameobjectActive(m_imgShip, bValue: true);
						NKCUtil.SetGameobjectActive(m_shipFaceButton, bValue: false);
					}
					else
					{
						NKCUtil.SetImageSprite(m_imgShipFace, sp);
						NKCUtil.SetGameobjectActive(m_imgShip, bValue: false);
						NKCUtil.SetGameobjectActive(m_shipFaceButton, bValue: true);
					}
					m_shipUnitData = new NKMUnitData();
					m_shipUnitData.FillDataFromAsyncUnitData(deckData.ship);
					m_NKCUIShipInfoSummary.SetShipData(m_shipUnitData, unitTempletBase);
				}
			}
			NKCUtil.SetGameobjectActive(m_operatorSlot, !NKCOperatorUtil.IsHide());
			if (!NKCOperatorUtil.IsActive())
			{
				m_operatorSlot.SetLock();
			}
			else if (deckData.operatorUnit != null)
			{
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(deckData.operatorUnit.id);
				if (unitTempletBase2 != null)
				{
					m_operatorData = new NKMOperator();
					m_operatorData = deckData.operatorUnit;
					m_operatorSlot.SetData(unitTempletBase2, deckData.operatorUnit.level);
				}
				else
				{
					m_operatorSlot.SetEmpty();
				}
			}
			else
			{
				m_operatorSlot.SetEmpty();
			}
			m_lstEquip = deckData.equips;
		}
		else
		{
			foreach (NKCDeckViewUnitSlot item in m_lstNKCDeckViewUnitSlot)
			{
				item.SetData(null);
				item.m_NKCUIComButton.PointerClick.RemoveAllListeners();
			}
			NKCUtil.SetGameobjectActive(m_imgShip, bValue: false);
			NKCUtil.SetGameobjectActive(m_shipFaceButton, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKCUIShipInfoSummary, bValue: false);
			m_operatorSlot.SetEmpty();
		}
		NKCUtil.SetGameobjectActive(m_objLocalBan, bDraftBanMode);
		NKCUtil.SetGameobjectActive(m_objGlobalBan, bDraftBanMode);
		if (!bDraftBanMode)
		{
			return;
		}
		if (m_slotLocalBan != null)
		{
			NKMUnitData nKMUnitData2 = null;
			if (deckData.banishedUnit != null)
			{
				nKMUnitData2 = new NKMUnitData();
				nKMUnitData2.FillDataFromAsyncUnitData(deckData.banishedUnit);
				nKMUnitData2.m_UnitUID = deckData.units.Count;
			}
			m_slotLocalBan.SetData(nKMUnitData2, bEnableButton: false);
			m_slotLocalBan.m_NKCUIComButton.PointerClick.RemoveAllListeners();
			m_slotLocalBan.SetLeagueBan(bBanUnit: true);
			m_lstNKMUnitData.Add(nKMUnitData2);
		}
		for (int j = 0; j < m_lstGlobalBan.Count; j++)
		{
			if (j < banUnitIdList.Count)
			{
				NKMUnitData cNKMUnitData = null;
				if (banUnitIdList[j] != 0)
				{
					cNKMUnitData = new NKMUnitData(banUnitIdList[j], j, islock: false, isPermanentContract: false, isSeized: false, fromContract: false);
				}
				NKCDeckViewUnitSlot nKCDeckViewUnitSlot2 = m_lstGlobalBan[j];
				nKCDeckViewUnitSlot2.SetData(cNKMUnitData, bEnableButton: false);
				nKCDeckViewUnitSlot2.SetLeagueBan(bBanUnit: true);
				nKCDeckViewUnitSlot2.SetEnableShowLevelText(bShow: false);
			}
		}
		using List<int>.Enumerator enumerator2 = banShipIdList.GetEnumerator();
		if (enumerator2.MoveNext())
		{
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(enumerator2.Current);
			NKCUtil.SetImageSprite(m_imgGlobalBanShip, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, nKMUnitTempletBase), bDisableIfSpriteNull: true);
			NKCUtil.SetGameobjectActive(m_imgGlobalBanShip, nKMUnitTempletBase != null);
		}
	}

	public void OnClickViewUnitSlot(int index)
	{
		for (int i = 0; i < m_lstNKCDeckViewUnitSlot.Count; i++)
		{
			NKCDeckViewUnitSlot nKCDeckViewUnitSlot = m_lstNKCDeckViewUnitSlot[i];
			if (i != index)
			{
				nKCDeckViewUnitSlot.ButtonDeSelect();
			}
			else if (!IsDetailHistoryOpened())
			{
				nKCDeckViewUnitSlot.ButtonDeSelect();
			}
			else
			{
				if (nKCDeckViewUnitSlot.m_NKMUnitData == null)
				{
					continue;
				}
				nKCDeckViewUnitSlot.ButtonSelect();
				List<NKMUnitData> unitDataList = new List<NKMUnitData>();
				m_lstNKMUnitData.ForEach(delegate(NKMUnitData e)
				{
					if (e != null)
					{
						unitDataList.Add(e);
					}
				});
				NKCUIUnitInfo.OpenOption openOption = new NKCUIUnitInfo.OpenOption(unitDataList, i);
				NKCUICollectionUnitInfo.CheckInstanceAndOpen(nKCDeckViewUnitSlot.m_NKMUnitData, openOption, m_lstEquip, NKCUICollectionUnitInfo.eCollectionState.CS_STATUS, isGauntlet: true);
			}
		}
	}

	public void OnClickOperatorSlot(long operatorUID)
	{
		if (m_operatorData != null && IsDetailHistoryOpened())
		{
			NKCUICollectionOperatorInfo.Instance.Open(m_operatorData, null, NKCUICollectionOperatorInfo.eCollectionState.CS_STATUS, NKCUIUpsideMenu.eMode.Normal, isGauntlet: true);
		}
	}

	public void OnClickShipSlot()
	{
		if (m_shipUnitData != null && IsDetailHistoryOpened())
		{
			NKCUICollectionShipInfo.CheckInstanceAndOpen(m_shipUnitData, NKMDeckIndex.None, null, null, isGauntlet: true);
		}
	}
}
