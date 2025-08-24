using ClientPacket.Common;
using NKC.UI.Component;
using NKC.UI.Guild;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletLRSlot : MonoBehaviour
{
	public delegate void OnDragBegin();

	public NKCUILeagueTier m_NKCUILeagueTier;

	public Text m_lbRank;

	public GameObject m_obj1STCrown;

	public GameObject m_obj1ST_BG;

	public GameObject m_obj2NDCrown;

	public GameObject m_obj2ND_BG;

	public GameObject m_obj3RDCrown;

	public GameObject m_obj3RD_BG;

	public Text m_lbLevel;

	public Text m_lbName;

	public Text m_lbUID;

	public Text m_lbScore;

	public NKCUIComStateButton m_csbtnSimpleUserInfoSlot;

	public GameObject m_objMySlot;

	public GameObject m_objGuild;

	public NKCUIGuildBadge m_GuildBadgeUI;

	public Text m_lbGuildName;

	public NKCUISlotProfile m_SlotProfile;

	public NKCUIComTitlePanel m_TitlePanel;

	private long m_UserUID;

	private OnDragBegin m_dOnDragBegin;

	private NKCAssetInstanceData m_InstanceData;

	public static NKCUIGauntletLRSlot GetNewInstance(Transform parent, OnDragBegin onDragBegin)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_GAUNTLET", "NKM_UI_GAUNTLET_RANK_SLOT");
		NKCUIGauntletLRSlot retVal = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIGauntletLRSlot>();
		if (retVal == null)
		{
			Debug.LogError("NKCUIGauntletLRSlot Prefab null!");
			return null;
		}
		retVal.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			retVal.transform.SetParent(parent);
		}
		retVal.transform.localScale = new Vector3(1f, 1f, 1f);
		retVal.m_dOnDragBegin = onDragBegin;
		retVal.transform.localPosition = new Vector3(retVal.transform.localPosition.x, retVal.transform.localPosition.y, 0f);
		retVal.m_csbtnSimpleUserInfoSlot.PointerClick.RemoveAllListeners();
		retVal.m_csbtnSimpleUserInfoSlot.PointerClick.AddListener(retVal.OnClick);
		retVal.m_csbtnSimpleUserInfoSlot.PointerDown.RemoveAllListeners();
		retVal.m_csbtnSimpleUserInfoSlot.PointerDown.AddListener(delegate
		{
			retVal.OnDragBeginImpl();
		});
		retVal.gameObject.SetActive(value: false);
		return retVal;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	private void OnDragBeginImpl()
	{
		if (m_dOnDragBegin != null)
		{
			m_dOnDragBegin();
		}
	}

	private void OnClick()
	{
		if (m_UserUID > 0)
		{
			NKCPacketSender.Send_NKMPacket_USER_PROFILE_INFO_REQ(m_UserUID, NKM_DECK_TYPE.NDT_PVP);
		}
	}

	public void SetUI(NKMUserSimpleProfileData cNKMUserSimpleProfileData, int rank, NKM_GAME_TYPE game_type)
	{
		if (cNKMUserSimpleProfileData != null)
		{
			m_UserUID = cNKMUserSimpleProfileData.userUid;
			int seasonID = NKCPVPManager.FindPvPSeasonID(game_type, NKCSynchronizedTime.GetServerUTCTime());
			m_NKCUILeagueTier.SetUI(NKCPVPManager.GetTierIconByTier(game_type, seasonID, cNKMUserSimpleProfileData.pvpTier), NKCPVPManager.GetTierNumberByTier(game_type, seasonID, cNKMUserSimpleProfileData.pvpTier));
			NKCUtil.SetGameobjectActive(m_obj1STCrown, rank == 1);
			NKCUtil.SetGameobjectActive(m_obj1ST_BG, rank == 1);
			NKCUtil.SetGameobjectActive(m_obj2NDCrown, rank == 2);
			NKCUtil.SetGameobjectActive(m_obj2ND_BG, rank == 2);
			NKCUtil.SetGameobjectActive(m_obj3RDCrown, rank == 3);
			NKCUtil.SetGameobjectActive(m_obj3RD_BG, rank == 3);
			bool bOpponent = cNKMUserSimpleProfileData.userUid != NKCScenManager.CurrentUserData().m_UserUID;
			m_lbName.text = NKCUtilString.GetUserNickname(cNKMUserSimpleProfileData.nickname, bOpponent);
			m_lbUID.text = NKCUtilString.GetFriendCode(cNKMUserSimpleProfileData.friendCode, bOpponent);
			m_lbScore.text = cNKMUserSimpleProfileData.pvpScore.ToString();
			m_lbLevel.text = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, cNKMUserSimpleProfileData.level);
			m_lbRank.text = rank + NKCUtilString.GetRankNumber(rank, bUpper: true);
			bool flag = false;
			if (m_UserUID == NKCScenManager.CurrentUserData().m_UserUID)
			{
				flag = true;
			}
			NKCUtil.SetGameobjectActive(m_objMySlot, flag);
			if (flag)
			{
				m_lbName.color = NKCUtil.GetColor("#FFDF5D");
			}
			else
			{
				m_lbName.color = Color.white;
			}
			if (cNKMUserSimpleProfileData.mainUnitId != 0)
			{
				m_SlotProfile.SetProfiledata(cNKMUserSimpleProfileData.mainUnitId, cNKMUserSimpleProfileData.mainUnitSkinId, cNKMUserSimpleProfileData.frameId, null, NKCTacticUpdateUtil.IsMaxTacticLevel(cNKMUserSimpleProfileData.mainUnitTacticLevel));
			}
			NKCUtil.SetGameobjectActive(m_SlotProfile, cNKMUserSimpleProfileData.mainUnitId != 0);
			SetGuildData(cNKMUserSimpleProfileData, bOpponent);
			m_TitlePanel?.SetData(cNKMUserSimpleProfileData);
		}
	}

	private void SetGuildData(NKMUserSimpleProfileData data, bool bOpponent)
	{
		if (m_objGuild != null)
		{
			GameObject objGuild = m_objGuild;
			NKMGuildSimpleData guildData = data.guildData;
			NKCUtil.SetGameobjectActive(objGuild, guildData != null && guildData.guildUid > 0);
			if (m_objGuild.activeSelf)
			{
				m_GuildBadgeUI.SetData(data.guildData.badgeId, bOpponent);
				NKCUtil.SetLabelText(m_lbGuildName, NKCUtilString.GetUserGuildName(data.guildData.guildName, bOpponent));
			}
		}
	}
}
