using Cs.Logging;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildNameChange : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_consortium";

	private const string UI_ASSET_NAME = "NKM_UI_CONSORTIUM_POPUP_NAME_CHANGE";

	private static NKCPopupGuildNameChange m_Instance;

	public NKCUIComStateButton m_btnBackground;

	public NKCUIComStateButton m_btnCancel;

	public NKCUIComStateButton m_btnOK;

	public InputField m_inputName;

	public Image m_imgNameValid;

	public Text m_lbNameValid;

	private string m_GuildName = string.Empty;

	private bool m_bValidName;

	public static NKCPopupGuildNameChange Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupGuildNameChange>("ab_ui_nkm_ui_consortium", "NKM_UI_CONSORTIUM_POPUP_NAME_CHANGE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupGuildNameChange>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "Guild Name Change";

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_btnBackground, base.Close);
		NKCUtil.SetButtonClickDelegate(m_btnCancel, base.Close);
		NKCUtil.SetButtonClickDelegate(m_btnOK, OnClickOK);
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.NICKNAME_LIMIT_ENG))
		{
			m_inputName.contentType = InputField.ContentType.Alphanumeric;
		}
		else
		{
			m_inputName.onValidateInput = NKCFilterManager.FilterEmojiInput;
		}
		m_inputName.onValueChanged.RemoveAllListeners();
		m_inputName.onValueChanged.AddListener(OnNameChanged);
		m_inputName.onEndEdit.RemoveAllListeners();
		m_inputName.onEndEdit.AddListener(OnNameChangeEnd);
		m_inputName.characterLimit = 16;
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Open()
	{
		base.gameObject.SetActive(value: true);
		m_inputName.text = "";
		CheckGuildName(m_inputName.text, bChangeBadchat: false);
		UIOpened();
	}

	private void OnNameChanged(string str)
	{
		Log.Debug("GuildName : " + str, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/NKCPopupGuildNameChange.cs", 94);
		CheckGuildName(str, bChangeBadchat: false);
	}

	private void CheckGuildName(string str, bool bChangeBadchat)
	{
		m_bValidName = true;
		m_GuildName = str;
		if (!NKCGuildManager.CheckNameLength(m_GuildName, 2, 16))
		{
			m_bValidName = false;
		}
		if (!NKCFilterManager.CheckNickNameFilter(m_GuildName))
		{
			m_bValidName = false;
		}
		if (bChangeBadchat)
		{
			m_inputName.text = NKCFilterManager.CheckBadChat(m_GuildName);
			if (!string.Equals(m_inputName.text, m_GuildName))
			{
				m_bValidName = false;
			}
		}
		else if (!string.Equals(NKCFilterManager.CheckBadChat(m_GuildName), m_GuildName))
		{
			m_bValidName = false;
		}
		if (!NKCFilterManager.CheckBadGuildname(m_GuildName))
		{
			m_bValidName = false;
		}
		if (m_GuildName == NKCGuildManager.MyGuildData.name)
		{
			m_bValidName = false;
		}
		if (m_bValidName)
		{
			NKCUtil.SetLabelText(m_lbNameValid, NKCUtilString.GET_STRING_CONSORTIUM_CREATE_NAME_SUB_GUIDE_USEFUL);
			NKCUtil.SetLabelTextColor(m_lbNameValid, NKCUtil.GetColor("#12a9ff"));
			NKCUtil.SetGameobjectActive(m_imgNameValid, bValue: true);
			NKCUtil.SetImageSprite(m_imgNameValid, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_Sprite", "AB_UI_NKM_UI_CONSORTIUM_ICON_CHECK"));
		}
		else if (string.IsNullOrWhiteSpace(m_GuildName))
		{
			if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.NICKNAME_LIMIT_ENG))
			{
				NKCUtil.SetLabelText(m_lbNameValid, NKCUtilString.GET_STRING_CONSORTIUM_CREATE_NAME_SUB_GUIDE_BASIC_DESC_GLOBAL);
			}
			else
			{
				NKCUtil.SetLabelText(m_lbNameValid, NKCUtilString.GET_STRING_CONSORTIUM_CREATE_NAME_SUB_GUIDE_BASIC_DESC);
			}
			NKCUtil.SetLabelTextColor(m_lbNameValid, NKCUtil.GetColor("#656565"));
			NKCUtil.SetGameobjectActive(m_imgNameValid, bValue: false);
		}
		else
		{
			NKCUtil.SetLabelText(m_lbNameValid, NKCUtilString.GET_STRING_CONSORTIUM_CREATE_NAME_SUB_GUIDE_BADWORD);
			NKCUtil.SetLabelTextColor(m_lbNameValid, NKCUtil.GetColor("#ff2626"));
			NKCUtil.SetGameobjectActive(m_imgNameValid, bValue: true);
			NKCUtil.SetImageSprite(m_imgNameValid, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONSORTIUM_Sprite", "AB_UI_NKM_UI_CONSORTIUM_ICON_DENIED"));
		}
		SetButtonState();
	}

	private void OnNameChangeEnd(string str)
	{
		CheckGuildName(str, bChangeBadchat: true);
	}

	private void SetButtonState()
	{
		if (!m_bValidName)
		{
			m_btnOK.Lock();
		}
		else
		{
			m_btnOK.UnLock();
		}
	}

	private void ReqRename()
	{
		NKCPacketSender.Send_NKMPacket_GUILD_RENAME_REQ(m_inputName.text);
	}

	public void OnClickOK()
	{
		int num = NKMCommonConst.Guild.ConsortiumNameChangeFree - NKCGuildManager.MyGuildData.renameCount;
		string content = string.Format(NKCUtilString.GET_STRING_CONSORTIUM_NAME_CHANGE_CONFIRM, m_inputName.text);
		if (num > 0)
		{
			string text = string.Format(NKCUtilString.GET_STRING_CONSORTIUM_NAME_CHANGE_FREE_COUNT, num);
			NKCPopupResourceConfirmBox.Instance.OpenWithText(NKCUtilString.GET_STRING_CONSORTIUM_NAME_CHANGE, content, text, ReqRename);
		}
		else
		{
			NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_CONSORTIUM_NAME_CHANGE, content, NKMCommonConst.Guild.ConsortiumNameChangeResourceItemId, (int)NKMCommonConst.Guild.ConsortiumNameChangeResourceValue, NKCGuildManager.MyGuildData.unionPoint, ReqRename);
		}
	}
}
