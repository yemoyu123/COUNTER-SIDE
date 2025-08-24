using ClientPacket.Common;
using ClientPacket.Pvp;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletAsyncSlotNew : MonoBehaviour
{
	public delegate void OnTouchBattle(long friendCode, NKM_GAME_TYPE gameType);

	public delegate void OnTouchProfile(long friendCode);

	public Text m_txtLevel;

	public Text m_txtName;

	public Text m_txtCode;

	public NKCUILeagueTier m_leagueTier;

	public Text m_txtScore;

	public NKCUISlotProfile m_SlotProfile;

	public GameObject m_objSelected;

	public NKCUIComStateButton m_btnProfle;

	public NKCUIComStateButton m_btnBattle;

	public GameObject m_objGuild;

	public NKCUIGuildBadge m_GuildBadgeUI;

	public Text m_lbGuildName;

	private NKCAssetInstanceData m_InstanceData;

	private OnTouchBattle dOnTouchBattle;

	private OnTouchProfile dOnTouchProfile;

	private long m_friendCode;

	private NKM_GAME_TYPE m_gameType;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objRevengeResult;

	public GameObject m_objRevengeResultWin;

	public GameObject m_objRevengeResultLose;

	public static NKCUIGauntletAsyncSlotNew GetNewInstance(Transform parent, OnTouchBattle onTouchBattle, OnTouchProfile onTouchProfile)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_ASYNC_SLOT_NEW");
		NKCUIGauntletAsyncSlotNew component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIGauntletAsyncSlotNew>();
		if (component == null)
		{
			Debug.LogError("NKCUIGauntletAsyncSlotNew Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localScale = Vector3.one;
		component.dOnTouchBattle = onTouchBattle;
		component.dOnTouchProfile = onTouchProfile;
		NKCUtil.SetBindFunction(component.m_btnProfle, component.OnClickProfile);
		NKCUtil.SetBindFunction(component.m_btnBattle, component.OnClickBattle);
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void SetUI(AsyncPvpTarget data, NKM_GAME_TYPE gameType)
	{
		if (data == null || data.asyncDeck == null)
		{
			return;
		}
		m_gameType = gameType;
		m_friendCode = data.userFriendCode;
		NKCUtil.SetLabelText(m_txtLevel, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, data.userLevel);
		NKCUtil.SetLabelText(m_txtName, NKCUtilString.GetUserNickname(data.userNickName, bOpponent: true));
		NKCUtil.SetLabelText(m_txtCode, NKCUtilString.GetFriendCode(data.userFriendCode, bOpponent: true));
		NKMPvpRankTemplet asyncPvpRankTempletByTier = NKCPVPManager.GetAsyncPvpRankTempletByTier(NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime()), data.tier);
		if (asyncPvpRankTempletByTier != null)
		{
			m_leagueTier.SetUI(asyncPvpRankTempletByTier);
		}
		NKCUtil.SetLabelText(m_txtScore, data.score.ToString());
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
		SetGuildData(data);
		NKCUtil.SetGameobjectActive(m_btnBattle.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_objRevengeResult, bValue: false);
	}

	public void SetUI(RevengePvpTarget data, NKM_GAME_TYPE gameType)
	{
		if (data != null && data.asyncDeck != null)
		{
			SetUI(NKCUIGauntletLobbyAsyncV2.ConventToAsyncPvpTarget(data), gameType);
			m_btnBattle.SetLock(!data.revengeAble);
			NKCUtil.SetGameobjectActive(m_btnBattle.gameObject, data.revengeAble);
			NKCUtil.SetGameobjectActive(m_objRevengeResult, !data.revengeAble);
			NKCUtil.SetGameobjectActive(m_objRevengeResultWin, data.result == PVP_RESULT.WIN);
			NKCUtil.SetGameobjectActive(m_objRevengeResultLose, data.result == PVP_RESULT.LOSE || data.result == PVP_RESULT.DRAW);
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

	private void OnClickBattle()
	{
		dOnTouchBattle?.Invoke(m_friendCode, m_gameType);
	}

	private void OnClickProfile()
	{
		dOnTouchProfile?.Invoke(m_friendCode);
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}
}
