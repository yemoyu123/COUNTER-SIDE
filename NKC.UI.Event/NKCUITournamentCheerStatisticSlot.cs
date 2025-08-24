using ClientPacket.Common;
using NKC.UI.Component;
using NKC.UI.Guild;
using TMPro;
using UnityEngine;

namespace NKC.UI.Event;

public class NKCUITournamentCheerStatisticSlot : MonoBehaviour
{
	public NKCUISlotProfile m_slotProfile;

	public NKCUIComTitlePanel m_titlePanel;

	public TMP_Text m_level;

	public TMP_Text m_userName;

	public TMP_Text m_cheerRank;

	public TMP_Text m_cheerPercent;

	public GameObject m_objGuildRoot;

	public NKCUIGuildBadge m_guildBadge;

	public TMP_Text m_guildName;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd±\ufffd")]
	public GameObject m_objCountryTag;

	public GameObject m_objKorea;

	public GameObject m_objGlobal;

	private NKCAssetInstanceData m_InstanceData;

	public static NKCUITournamentCheerStatisticSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ui_single_tournament", "UI_SINGLE_POPUP_TOURNAMENT_CHEERING_SLOT");
		NKCUITournamentCheerStatisticSlot nKCUITournamentCheerStatisticSlot = nKCAssetInstanceData?.m_Instant.GetComponent<NKCUITournamentCheerStatisticSlot>();
		if (nKCUITournamentCheerStatisticSlot == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUITournamentCheerStatisticSlot Prefab null!");
			return null;
		}
		nKCUITournamentCheerStatisticSlot.m_InstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			nKCUITournamentCheerStatisticSlot.transform.SetParent(parent);
		}
		nKCUITournamentCheerStatisticSlot.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		nKCUITournamentCheerStatisticSlot.gameObject.SetActive(value: false);
		return nKCUITournamentCheerStatisticSlot;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	public void SetData(NKMTournamentProfileData profileData, float percent, int cheerRank)
	{
		string msg = "";
		string msg2 = "";
		if (profileData != null)
		{
			m_slotProfile.SetProfiledata(profileData.commonProfile, null);
			m_titlePanel.SetData(profileData.commonProfile);
			msg = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, profileData.commonProfile.level);
			msg2 = profileData.commonProfile.nickname;
		}
		NKCUtil.SetLabelText(m_level, msg);
		NKCUtil.SetLabelText(m_userName, msg2);
		NKCUtil.SetGameobjectActive(m_objGuildRoot, profileData != null && profileData.guildData != null && profileData.guildData.guildUid > 0);
		if (profileData != null && profileData.guildData != null)
		{
			NKCUtil.SetGameobjectActive(m_objGuildRoot, profileData.guildData.guildUid > 0);
			m_guildBadge.SetData(profileData.guildData.badgeId);
			NKCUtil.SetLabelText(m_guildName, profileData.guildData.guildName);
		}
		NKCUtil.SetLabelText(m_cheerRank, $"{cheerRank}{NKCUtilString.GetRankNumber(cheerRank)}");
		NKCUtil.SetLabelText(m_cheerPercent, $"{percent:F1}%");
		NKCUtil.SetGameobjectActive(m_objCountryTag, profileData.countryCode != NKMTournamentCountryCode.None);
		NKCUtil.SetGameobjectActive(m_objKorea, profileData.countryCode == NKMTournamentCountryCode.KR);
		NKCUtil.SetGameobjectActive(m_objGlobal, profileData.countryCode == NKMTournamentCountryCode.GL);
	}
}
