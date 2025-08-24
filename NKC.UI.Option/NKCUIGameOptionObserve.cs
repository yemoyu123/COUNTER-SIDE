using UnityEngine.UI;

namespace NKC.UI.Option;

public class NKCUIGameOptionObserve : NKCUIGameOptionContentBase
{
	public Text m_NKM_UI_GAME_OPTION_OBSERVE_TEXT_TITLE;

	public Text m_NKM_UI_GAME_OPTION_OBSERVE_TEXT_SUB_TITLE;

	public Text m_NKM_UI_GAME_OPTION_OBSERVE_TEXT_DESC;

	public NKCUIComStateButton m_csbtnExit;

	private string OBSERVE_LEAVE_POPUP_TITLE => "관전 LEAVE TITLE";

	private string OBSERVE_LEAVE_POPUP_DESC => "관전 LEAVE DESC";

	public override void Init()
	{
		m_csbtnExit.PointerClick.RemoveAllListeners();
		m_csbtnExit.PointerClick.AddListener(LeaveObserve);
	}

	public override void SetContent()
	{
		NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_OBSERVE_TEXT_TITLE, NKCUtilString.GET_STRING_GAUNTLET.ToUpper());
		NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_OBSERVE_TEXT_SUB_TITLE, "");
		NKCUtil.SetLabelText(m_NKM_UI_GAME_OPTION_OBSERVE_TEXT_DESC, "");
	}

	public void LeaveObserve()
	{
		NKCPopupOKCancel.OpenOKCancelBox(OBSERVE_LEAVE_POPUP_TITLE, OBSERVE_LEAVE_POPUP_DESC, OnClickLeaveObserveOkButton);
	}

	private static void OnClickLeaveObserveOkButton()
	{
		NKCPrivatePVPRoomMgr.Send_NKMPacket_PRIVATE_PVP_EXIT_REQ();
	}
}
