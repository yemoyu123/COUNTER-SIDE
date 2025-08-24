using ClientPacket.Common;
using ClientPacket.Guild;
using NKC.UI.Guild;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Friend;

public class NKCUIFriendMentoringSlot : MonoBehaviour
{
	public GameObject m_Deco_Right;

	public GameObject m_Deco_Left;

	public GameObject m_MENTORING_TAG;

	public GameObject m_NKM_UI_FRIEND_MENTORING_SLOT_INFO;

	public NKCUIComStateButton m_NKM_UI_FRIEND_MENTORING_SLOT_ADD_BUTTON;

	[Header("유닛 정보")]
	public Image m_INVEN_ICON_Root;

	public Text m_LV;

	public Text m_NICKNAME;

	public Text m_UID;

	public GameObject m_CONSORTIUM;

	public Text m_CONSORTIUM_NAME;

	public NKCUIGuildBadge m_NKM_UI_CONSORTIUM_MARK;

	public void SetData(MentoringIdentity myMentoringType, NKMCommonProfile userProfile, NKMGuildData guildData)
	{
		NKMGuildSimpleData nKMGuildSimpleData = new NKMGuildSimpleData();
		if (guildData != null)
		{
			nKMGuildSimpleData.guildUid = guildData.guildUid;
			nKMGuildSimpleData.badgeId = guildData.badgeId;
			nKMGuildSimpleData.guildName = guildData.name;
		}
		SetData(myMentoringType, userProfile, nKMGuildSimpleData);
	}

	public void SetData(MentoringIdentity myMentoringType, NKMCommonProfile userProfile, NKMGuildSimpleData guildData)
	{
		NKCUtil.SetGameobjectActive(m_Deco_Right, myMentoringType == MentoringIdentity.Mentee);
		NKCUtil.SetGameobjectActive(m_Deco_Left, myMentoringType == MentoringIdentity.Mentor);
		NKCUtil.SetGameobjectActive(m_MENTORING_TAG, userProfile != null && myMentoringType == MentoringIdentity.Mentor);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_SLOT_INFO, userProfile != null);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FRIEND_MENTORING_SLOT_ADD_BUTTON, myMentoringType != MentoringIdentity.Mentee && userProfile == null);
		NKCUtil.SetGameobjectActive(m_CONSORTIUM, guildData != null && guildData.guildUid != 0);
		if (guildData != null && guildData.guildUid != 0L)
		{
			m_NKM_UI_CONSORTIUM_MARK.SetData(guildData.badgeId);
			NKCUtil.SetLabelText(m_CONSORTIUM_NAME, guildData.guildName);
		}
		if (userProfile != null)
		{
			NKCUtil.SetLabelText(m_LV, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, userProfile.level));
			NKCUtil.SetLabelText(m_NICKNAME, userProfile.nickname);
			NKCUtil.SetLabelText(m_UID, $"#{userProfile.friendCode}");
			NKCUtil.SetGameobjectActive(m_INVEN_ICON_Root, bValue: true);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(userProfile.mainUnitId);
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(userProfile.mainUnitSkinId);
			Sprite sprite = null;
			sprite = ((skinTemplet == null || skinTemplet.m_SkinEquipUnitID != userProfile.mainUnitId) ? NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitTempletBase) : NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, skinTemplet));
			if (sprite != null)
			{
				NKCUtil.SetImageSprite(m_INVEN_ICON_Root, sprite);
			}
		}
	}
}
