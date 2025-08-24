using ClientPacket.Account;
using NKM;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupAccountSelectSlot : MonoBehaviour
{
	public Text m_creditCount;

	public Text m_eterniumCount;

	public Text m_cashCount;

	public Text m_medalCount;

	public Text m_nickName;

	public Text m_level;

	public Text m_publisherCode;

	public GameObject m_highlight;

	public GameObject m_grey;

	public GameObject m_iconSteam;

	public GameObject m_iconMobile;

	public NKCUIComToggle m_button;

	public void InitData()
	{
		m_creditCount.text = "0";
		m_eterniumCount.text = "0";
		m_cashCount.text = "0";
		m_medalCount.text = "0";
		m_nickName.text = "";
		m_level.text = "";
		m_publisherCode.text = "";
		NKCUtil.SetGameobjectActive(m_iconSteam, bValue: false);
		NKCUtil.SetGameobjectActive(m_iconMobile, bValue: false);
	}

	public void SetData(NKMAccountLinkUserProfile userProfile, UnityAction<bool> onToggleSelected)
	{
		m_creditCount.text = userProfile.creditCount.ToString();
		m_eterniumCount.text = userProfile.eterniumCount.ToString();
		m_cashCount.text = userProfile.cashCount.ToString();
		m_medalCount.text = userProfile.medalCount.ToString();
		m_nickName.text = userProfile.commonProfile.nickname;
		m_level.text = userProfile.commonProfile.level.ToString();
		m_publisherCode.text = NKCUtilString.GetFriendCode(userProfile.commonProfile.friendCode);
		NKCUtil.SetGameobjectActive(m_iconSteam, userProfile.publisherType == NKM_PUBLISHER_TYPE.NPT_STEAM);
		NKCUtil.SetGameobjectActive(m_iconMobile, userProfile.publisherType != NKM_PUBLISHER_TYPE.NPT_STEAM);
		m_button.OnValueChanged.RemoveAllListeners();
		if (onToggleSelected == null)
		{
			m_button.Select(bSelect: true, bForce: true, bImmediate: true);
		}
		else
		{
			m_button.OnValueChanged.AddListener(onToggleSelected);
		}
	}
}
