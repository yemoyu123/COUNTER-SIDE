using NKC.UI;
using NKM;
using UnityEngine;

namespace NKC.PacketHandler;

public static class NKCPacketHandlers
{
	public static bool Check_NKM_ERROR_CODE(NKM_ERROR_CODE eNKM_ERROR_CODE, bool bCloseWaitBox = true, NKCPopupOKCancel.OnButton onOK_Button = null, int addErrorCode = int.MinValue)
	{
		if (bCloseWaitBox)
		{
			NKMPopUpBox.CloseWaitBox();
		}
		if (eNKM_ERROR_CODE == NKM_ERROR_CODE.NEC_OK)
		{
			return true;
		}
		string text = GetErrorMessage(eNKM_ERROR_CODE);
		Debug.LogWarning("Server Error Code : " + eNKM_ERROR_CODE);
		if (int.MinValue != addErrorCode)
		{
			text = text + " (" + addErrorCode + ")";
		}
		NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_ERROR, text, onOK_Button);
		return false;
	}

	public static string GetErrorMessage(NKM_ERROR_CODE eNKM_ERROR_CODE)
	{
		string text = null;
		if (NKCStringTable.CheckExistString(eNKM_ERROR_CODE.ToString()))
		{
			string text2 = NKCStringTable.GetString(eNKM_ERROR_CODE.ToString());
			if (!string.IsNullOrEmpty(text2))
			{
				text = text2;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			text = NKCStringTable.GetString("SI_ERROR_DEFAULT_MESSAGE");
		}
		if (NKCScenManager.CurrentUserData() != null && (int)NKCScenManager.CurrentUserData().m_eAuthLevel > 1)
		{
			return $"{text}\n({(int)eNKM_ERROR_CODE} {eNKM_ERROR_CODE})";
		}
		return $"{text}\n({(int)eNKM_ERROR_CODE})";
	}
}
