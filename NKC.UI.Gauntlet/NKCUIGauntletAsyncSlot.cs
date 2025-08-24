using ClientPacket.Common;
using ClientPacket.Pvp;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletAsyncSlot : MonoBehaviour
{
	public delegate void OnTouchBattle(long friendCode);

	public delegate void OnTouchBattleAsync(long friendCode, NKM_GAME_TYPE gameType);

	public delegate void OnTouchProfile(long friendCode);

	public Text m_txtLevel;

	public Text m_txtName;

	public Text m_txtCode;

	public NKCUILeagueTier m_leagueTier;

	public Text m_txtRank;

	public Text m_txtScore;

	public Text m_txtAddScore;

	public GameObject m_objAddScore;

	public Image m_imgMainUnit;

	public Image m_imgShip;

	public GameObject m_objOperator;

	public Image m_imgOperator;

	public Image[] m_imgDeck = new Image[8];

	public Text m_txtDeckPower;

	public NKCUIComStateButton m_btnBattle;

	public NKCUIComStateButton m_btnSlot;

	public GameObject m_objGuild;

	public NKCUIGuildBadge m_GuildBadgeUI;

	public Text m_lbGuildName;

	[Header("Sprite")]
	public Sprite SpriteFaceCardPrivate;

	public Sprite SpriteMiniMapFacePrivate;

	[Header("NPC")]
	public GameObject m_objNPC;

	public GameObject m_objLock;

	private NKCAssetInstanceData m_InstanceData;

	private OnTouchBattle dOnTouchBattle;

	private long m_friendCode;

	private OnTouchBattleAsync dOnTouchBattleAsync;

	private OnTouchProfile dOnTouchProfile;

	private NKM_GAME_TYPE m_gameType;

	public NKCUISlotProfile m_SlotProfile;

	public static NKCUIGauntletAsyncSlot GetNewInstance(Transform parent, OnTouchBattle onTouchBattle)
	{
		NKCUIGauntletAsyncSlot loadAsset = GetLoadAsset(parent, "AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_LOBBY_ASYNC_SLOT");
		if (null == loadAsset)
		{
			return null;
		}
		loadAsset.dOnTouchBattle = onTouchBattle;
		NKCUtil.SetBindFunction(loadAsset.m_btnBattle, loadAsset.OnTouchBattleBtn);
		NKCUtil.SetBindFunction(loadAsset.m_btnSlot, loadAsset.OnTouchBattleBtn);
		loadAsset.gameObject.SetActive(value: false);
		return loadAsset;
	}

	public static NKCUIGauntletAsyncSlot GetNewInstance(Transform parent, OnTouchBattleAsync onTouchBattle, OnTouchProfile onTouchProfile)
	{
		NKCUIGauntletAsyncSlot loadAsset = GetLoadAsset(parent, "AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_LOBBY_ASYNC_SLOT_SMALL");
		if (null == loadAsset)
		{
			return null;
		}
		loadAsset.dOnTouchBattleAsync = onTouchBattle;
		loadAsset.dOnTouchProfile = onTouchProfile;
		NKCUtil.SetBindFunction(loadAsset.m_btnSlot, loadAsset.OnClickProfile);
		NKCUtil.SetBindFunction(loadAsset.m_btnBattle, loadAsset.OnClickBattleAsync);
		loadAsset.gameObject.SetActive(value: false);
		return loadAsset;
	}

	public static NKCUIGauntletAsyncSlot GetLoadAsset(Transform parent, string bundleName, string assetName)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(bundleName, assetName);
		NKCUIGauntletAsyncSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIGauntletAsyncSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIGauntletAsyncSlot Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localScale = Vector3.one;
		return component;
	}

	public void SetUI(AsyncPvpTarget data)
	{
		if (data == null || data.asyncDeck == null)
		{
			return;
		}
		m_friendCode = data.userFriendCode;
		NKCUtil.SetLabelText(m_txtLevel, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, data.userLevel);
		NKCUtil.SetLabelText(m_txtName, NKCUtilString.GetUserNickname(data.userNickName, bOpponent: true));
		NKCUtil.SetLabelText(m_txtCode, NKCUtilString.GetFriendCode(data.userFriendCode, bOpponent: true));
		int seasonID = NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime());
		NKMPvpRankTemplet asyncPvpRankTempletByTier = NKCPVPManager.GetAsyncPvpRankTempletByTier(seasonID, data.tier);
		if (asyncPvpRankTempletByTier != null)
		{
			m_leagueTier.SetUI(asyncPvpRankTempletByTier);
		}
		if (data.rank == 0)
		{
			NKCUtil.SetLabelText(m_txtRank, "");
		}
		else
		{
			NKCUtil.SetLabelText(m_txtRank, string.Format($"{data.rank}{NKCUtilString.GetRankNumber(data.rank, bUpper: true)}"));
		}
		NKCUtil.SetLabelText(m_txtScore, data.score.ToString());
		if (null != m_SlotProfile)
		{
			int num = data.mainUnitId;
			int skinID = data.mainUnitSkinId;
			int selfieFrameId = data.selfieFrameId;
			int mainUnitTacticLevel = data.mainUnitTacticLevel;
			if (num == 0)
			{
				NKMAsyncDeckData asyncDeck = data.asyncDeck;
				if (asyncDeck.units.Count == 0)
				{
					Debug.LogError("Gauntlet Async Slot - target deck cout 0");
					return;
				}
				foreach (NKMAsyncUnitData unit in asyncDeck.units)
				{
					if (unit.unitId > 0)
					{
						num = unit.unitId;
						skinID = unit.skinId;
						mainUnitTacticLevel = data.mainUnitTacticLevel;
						break;
					}
				}
			}
			m_SlotProfile.SetProfiledata(num, skinID, selfieFrameId, null, NKCTacticUpdateUtil.IsMaxTacticLevel(mainUnitTacticLevel));
		}
		PvpState asyncData = NKCScenManager.CurrentUserData().m_AsyncData;
		if (asyncData != null)
		{
			NKMPvpRankTemplet asyncPvpRankTempletByScore = NKCPVPManager.GetAsyncPvpRankTempletByScore(seasonID, asyncData.Score);
			if (asyncPvpRankTempletByScore != null)
			{
				NKCUtil.SetGameobjectActive(m_objAddScore, bValue: true);
				int num2 = NKCUtil.CalcAddScore(asyncPvpRankTempletByScore.LeagueType, asyncData.Score, data.score);
				NKCUtil.SetLabelText(m_txtAddScore, $"+{num2}");
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objAddScore, bValue: false);
			}
			NKCUtil.SetGameobjectActive(m_objGuild, bValue: true);
			SetAsyncData(data.asyncDeck);
			SetGuildData(data);
			NKCUtil.SetGameobjectActive(m_objNPC, bValue: false);
			NKCUtil.SetGameobjectActive(m_objLock, bValue: false);
		}
	}

	public void SetUI(AsyncPvpTarget data, NKM_GAME_TYPE gameType)
	{
		m_gameType = gameType;
		SetUI(data);
	}

	public void SetUI(NpcPvpTarget npcTarget)
	{
		if (npcTarget != null && npcTarget.asyncDeck != null)
		{
			m_friendCode = npcTarget.userFriendCode;
			NKCUtil.SetLabelText(m_txtLevel, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, npcTarget.userLevel);
			NKCUtil.SetLabelText(m_txtName, NKCUtilString.GetUserNickname(npcTarget.userNickName, bOpponent: true));
			NKCUtil.SetLabelText(m_txtCode, NKCUtilString.GetFriendCode(npcTarget.userFriendCode, bOpponent: true));
			NKMPvpRankTemplet asyncPvpRankTempletByTier = NKCPVPManager.GetAsyncPvpRankTempletByTier(NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime()), npcTarget.tier);
			if (asyncPvpRankTempletByTier != null)
			{
				m_leagueTier.SetUI(asyncPvpRankTempletByTier);
			}
			NKCUtil.SetLabelText(m_txtRank, "");
			NKCUtil.SetLabelText(m_txtAddScore, npcTarget.score.ToString());
			NKCUtil.SetGameobjectActive(m_objAddScore, bValue: false);
			NKCUtil.SetGameobjectActive(m_objNPC, bValue: true);
			NKCUtil.SetGameobjectActive(m_objLock, !npcTarget.isOpened);
			SetAsyncData(npcTarget.asyncDeck);
			NKCUtil.SetGameobjectActive(m_objGuild, bValue: false);
		}
	}

	private void SetAsyncData(NKMAsyncDeckData asyncDeck)
	{
		if (asyncDeck == null)
		{
			return;
		}
		if (asyncDeck.units.Count == 0)
		{
			Debug.LogError("Gauntlet Async Slot - target deck cout 0");
			return;
		}
		if (asyncDeck.ship.unitId > 0)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(asyncDeck.ship.unitId);
			NKCUtil.SetGameobjectActive(m_imgShip, bValue: true);
			m_imgShip.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitTempletBase);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_imgShip, bValue: false);
			m_imgShip.sprite = null;
		}
		NKCUtil.SetGameobjectActive(m_objOperator, bValue: false);
		if (!NKCOperatorUtil.IsHide())
		{
			NKCUtil.SetGameobjectActive(m_objOperator, bValue: true);
			if (asyncDeck.operatorUnit != null && asyncDeck.operatorUnit.id > 0)
			{
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(asyncDeck.operatorUnit.id);
				if (unitTempletBase2 != null)
				{
					NKCUtil.SetImageSprite(m_imgOperator, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase2));
				}
			}
			else
			{
				NKCUtil.SetImageSprite(m_imgOperator, NKCOperatorUtil.GetSpriteEmptySlot());
			}
		}
		int num = 0;
		foreach (NKMAsyncUnitData unit in asyncDeck.units)
		{
			if (unit.unitId > 0)
			{
				NKMAsyncUnitData nKMAsyncUnitData = asyncDeck.units[num];
				m_imgMainUnit.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, nKMAsyncUnitData.unitId, nKMAsyncUnitData.skinId);
				break;
			}
			num++;
		}
		for (int i = 0; i < m_imgDeck.Length; i++)
		{
			Image image = m_imgDeck[i];
			if (i >= asyncDeck.units.Count)
			{
				NKCUtil.SetGameobjectActive(image, bValue: false);
				return;
			}
			NKCUtil.SetGameobjectActive(image, bValue: true);
			int unitId = asyncDeck.units[i].unitId;
			if (unitId > 0)
			{
				NKMUnitTempletBase unitTempletBase3 = NKMUnitManager.GetUnitTempletBase(unitId);
				image.sprite = NKCResourceUtility.GetOrLoadMinimapFaceIcon(unitTempletBase3);
			}
			else
			{
				image.sprite = SpriteMiniMapFacePrivate;
			}
		}
		if (asyncDeck.operationPower >= 0)
		{
			NKCUtil.SetLabelText(m_txtDeckPower, asyncDeck.operationPower.ToString("N0"));
		}
		else
		{
			NKCUtil.SetLabelText(m_txtDeckPower, "???");
		}
	}

	private void SetGuildData(AsyncPvpTarget data)
	{
		if (m_objGuild != null)
		{
			GameObject objGuild = m_objGuild;
			NKMGuildSimpleData guildData = data.guildData;
			NKCUtil.SetGameobjectActive(objGuild, guildData != null && guildData.guildUid > 0);
			if (m_objGuild.activeSelf)
			{
				m_GuildBadgeUI.SetData(data.guildData.badgeId, bOpponent: true);
				NKCUtil.SetLabelText(m_lbGuildName, NKCUtilString.GetUserGuildName(data.guildData.guildName, bOpponent: true));
			}
		}
	}

	private void OnTouchBattleBtn()
	{
		if (m_objLock.activeSelf)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_ASYNC_NPC_BLOCK_DESC);
		}
		else
		{
			dOnTouchBattle?.Invoke(m_friendCode);
		}
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	private void OnClickBattleAsync()
	{
		dOnTouchBattleAsync?.Invoke(m_friendCode, m_gameType);
	}

	private void OnClickProfile()
	{
		dOnTouchProfile?.Invoke(m_friendCode);
	}
}
