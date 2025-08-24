using ClientPacket.Common;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component.Office;

public class NKCUIComOfficeFriendInfo : MonoBehaviour
{
	public Text m_lbFriendInfo;

	public void SetData(NKMUserProfileData profileData)
	{
		if (profileData == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		NKCUtil.SetLabelText(m_lbFriendInfo, NKCUtilString.GET_STRING_INGAME_USER_A_NAME_TWO_PARAM, profileData.commonProfile.level, profileData.commonProfile.nickname);
	}

	private void Update()
	{
		if (base.transform.lossyScale.x < 0f)
		{
			base.transform.localScale = new Vector3(0f - base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
		}
	}
}
