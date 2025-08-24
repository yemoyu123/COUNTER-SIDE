using NKC.UI;

namespace NKC.Publisher.CallBack;

public static class GameBaseCallBack
{
	public static void LoginResponseCallBack(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalInfo)
	{
		switch (resultCode)
		{
		case NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_LOGIN_QUIT_USER:
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, "ID withdrawal in progress. \n immediate withdrawal : Ok , withdrawal cancel : Cancel", delegate
			{
				NKCPublisherModule.Auth.Withdraw(null);
			}, delegate
			{
				NKCPublisherModule.Auth.TryResolveUser(null);
			});
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_LOGIN_FAIL_ALREADY_LOGIN:
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, "Already logged in. \n Do you want to log out of the logged in ID?", delegate
			{
				NKCPublisherModule.Auth.Logout(null);
			});
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_LOGIN_FAIL:
			NKCPopupOKCancel.OpenOKBox("GameBase", "Invalid idp string \n " + additionalInfo);
			break;
		default:
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, "Login fail", null);
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_OK:
			break;
		}
	}

	public static void AddMappingResponseCallBack(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalInfo)
	{
		switch (resultCode)
		{
		case NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_CHANGEACCOUNT_SUCCESS_GUEST_SYNC:
			NKCPopupOKCancel.OpenOKBox("GameBase", "Mapping success \n Guest -> [" + additionalInfo + "]!");
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_CHANGEACCOUNT_FAIL_GUEST_ALREADY_MAPPED:
			NKCPopupOKCancel.OpenOKBox("GameBase", "Mapping fail \n Already mapped [" + additionalInfo + "]");
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_CHANGEACCOUNT_FAIL_NO_CHANGEABLE:
			NKCPopupOKCancel.OpenOKBox("GameBase", "Mapping fail \n [" + additionalInfo + "]");
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_AUTH_CHANGEACCOUNT_SUCCESS_QUIT:
			break;
		}
	}

	public static void BillingProductListResponseCallBack(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalInfo)
	{
	}

	public static void TermResponseCallBack(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalInfo)
	{
		if (resultCode != NKC_PUBLISHER_RESULT_CODE.NPRC_OK && resultCode == NKC_PUBLISHER_RESULT_CODE.NKRC_TERM_FAIL)
		{
			NKCPopupOKCancel.OpenOKBox("GameBase", "Term fail \n [" + additionalInfo + "]");
		}
	}
}
