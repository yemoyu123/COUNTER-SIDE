using ClientPacket.Pvp;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletReplaySlot : MonoBehaviour
{
	public delegate void OnSelectReplayData(int replayIndex);

	public delegate void OnPlayReplay(int replayIndex);

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

	private NKCAssetInstanceData m_InstanceData;

	private OnSelectReplayData dOnSelectReplayData;

	private OnPlayReplay dOnPlayReplay;

	private int m_replayDataIndex;

	public static NKCUIGauntletReplaySlot GetNewInstance(Transform parent, OnSelectReplayData onSelectReplayData, OnPlayReplay onPlayReplay)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_LOBBY_REPLAY_SLOT");
		NKCUIGauntletReplaySlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIGauntletReplaySlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIGauntletReplaySlot Prefab null!");
			return null;
		}
		component.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localScale = Vector3.one;
		component.dOnSelectReplayData = onSelectReplayData;
		component.dOnPlayReplay = onPlayReplay;
		component.m_btnBattle.PointerClick.RemoveAllListeners();
		component.m_btnBattle.PointerClick.AddListener(component.OnPlayReplayBtn);
		component.m_btnSlot.PointerClick.RemoveAllListeners();
		component.m_btnSlot.PointerClick.AddListener(component.OnSelectReplayDataBtn);
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void SetUI(int index, ReplayData cReplayData)
	{
		if (cReplayData == null)
		{
			return;
		}
		m_replayDataIndex = index;
		int userLevel = cReplayData.gameData.m_NKMGameTeamDataB.m_UserLevel;
		string userNickname = cReplayData.gameData.m_NKMGameTeamDataB.m_UserNickname;
		long friendCode = cReplayData.gameData.m_NKMGameTeamDataB.m_FriendCode;
		int num = 0;
		int score = cReplayData.gameData.m_NKMGameTeamDataB.m_Score;
		int tier = cReplayData.gameData.m_NKMGameTeamDataB.m_Tier;
		string msg = "";
		long guildUid = cReplayData.gameData.m_NKMGameTeamDataB.guildSimpleData.guildUid;
		long data = 0L;
		int num2 = 0;
		int unitID = cReplayData.gameData.m_NKMGameTeamDataB.m_MainShip.m_UnitID;
		NKMOperator nKMOperator = cReplayData.gameData.m_NKMGameTeamDataB.m_Operator;
		NKCUtil.SetLabelText(m_txtLevel, NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, userLevel);
		NKCUtil.SetLabelText(m_txtName, NKCUtilString.GetUserNickname(userNickname, bOpponent: true));
		NKCUtil.SetLabelText(m_txtCode, NKCUtilString.GetFriendCode(friendCode, bOpponent: true));
		int seasonID = NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime());
		m_leagueTier.SetUI(NKCPVPManager.GetTierIconByTier(cReplayData.gameData.m_NKM_GAME_TYPE, seasonID, tier), NKCPVPManager.GetTierNumberByTier(cReplayData.gameData.m_NKM_GAME_TYPE, seasonID, tier));
		if (num == 0)
		{
			NKCUtil.SetLabelText(m_txtRank, "");
		}
		else
		{
			NKCUtil.SetLabelText(m_txtRank, string.Format($"{num}{NKCUtilString.GetRankNumber(num, bUpper: true)}"));
		}
		NKCUtil.SetLabelText(m_txtScore, score.ToString());
		NKCUtil.SetGameobjectActive(m_objAddScore, bValue: false);
		if (unitID > 0)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitID);
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
			if (nKMOperator != null && nKMOperator.id > 0)
			{
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(nKMOperator.id);
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
		NKMUnitData leaderUnitData = cReplayData.gameData.m_NKMGameTeamDataB.GetLeaderUnitData();
		if (leaderUnitData != null && leaderUnitData.m_UnitID > 0)
		{
			m_imgMainUnit.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, leaderUnitData.m_UnitID, leaderUnitData.m_SkinID);
		}
		else
		{
			m_imgMainUnit.sprite = SpriteFaceCardPrivate;
		}
		for (int i = 0; i < m_imgDeck.Length; i++)
		{
			Image image = m_imgDeck[i];
			if (i >= cReplayData.gameData.m_NKMGameTeamDataB.m_listUnitData.Count)
			{
				NKCUtil.SetGameobjectActive(image, bValue: false);
				return;
			}
			NKCUtil.SetGameobjectActive(image, bValue: true);
			NKMUnitData nKMUnitData = cReplayData.gameData.m_NKMGameTeamDataB.m_listUnitData[i];
			if (nKMUnitData.m_UnitID > 0)
			{
				NKMUnitTempletBase unitTempletBase3 = NKMUnitManager.GetUnitTempletBase(nKMUnitData.m_UnitID);
				image.sprite = NKCResourceUtility.GetOrLoadMinimapFaceIcon(unitTempletBase3);
			}
			else
			{
				image.sprite = SpriteMiniMapFacePrivate;
			}
		}
		if (num2 >= 0)
		{
			NKCUtil.SetLabelText(m_txtDeckPower, num2.ToString());
		}
		else
		{
			NKCUtil.SetLabelText(m_txtDeckPower, "???");
		}
		if (m_objGuild != null)
		{
			NKCUtil.SetGameobjectActive(m_objGuild, guildUid > 0);
			if (m_objGuild.activeSelf)
			{
				m_GuildBadgeUI.SetData(data);
				NKCUtil.SetLabelText(m_lbGuildName, msg);
			}
		}
	}

	private void OnPlayReplayBtn()
	{
		dOnPlayReplay?.Invoke(m_replayDataIndex);
	}

	private void OnSelectReplayDataBtn()
	{
		dOnSelectReplayData?.Invoke(m_replayDataIndex);
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}
}
