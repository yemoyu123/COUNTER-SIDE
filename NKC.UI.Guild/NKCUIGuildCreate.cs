using System.Linq;
using System.Text;
using ClientPacket.Guild;
using Cs.Logging;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCUIGuildCreate : NKCUIBase
{
	private const string BUNDLE_NAME = "AB_UI_NKM_UI_CONSORTIUM";

	private const string ASSET_NAME = "NKM_UI_CONSORTIUM_FOUNDATION";

	private static NKCUIGuildCreate m_Instance;

	public InputField m_inputName;

	public Image m_imgNameValid;

	public Text m_lbNameValid;

	public InputField m_inputDesc;

	public NKCUIComToggle m_tglJoinTypeDirect;

	public NKCUIComToggle m_tglJoinTypeApproval;

	public NKCUIComToggle m_tglJoinTypeClosed;

	public Text m_lbJoinTypeDesc;

	public NKCUIGuildBadge m_badgeUI;

	public NKCUIComStateButton m_btnBadgeSetting;

	public NKCUIComStateButton m_btnBadgeRandom;

	public Text m_lbGuildName;

	public Text m_lbDesc;

	public NKCUIComStateButton m_btnCreate;

	public Text m_lbBtn;

	public Text m_lbCost;

	public Image m_imgCostItem;

	private string m_GuildName = string.Empty;

	private GuildJoinType m_JoinType;

	private long m_BadgeId;

	private bool m_bValidName;

	private string NORMAL_TEXT_COLOR_TEXT = "#582817";

	private string DISABLE_TEXT_COLOR_TEXT = "#212122";

	public static NKCUIGuildCreate Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIGuildCreate>("AB_UI_NKM_UI_CONSORTIUM", "NKM_UI_CONSORTIUM_FOUNDATION", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontCommon), CleanupInstance).GetInstance<NKCUIGuildCreate>();
				if (m_Instance != null)
				{
					m_Instance.InitUI();
				}
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

	public override string MenuName => NKCUtilString.GET_STRING_CONSORTIUM_INTRO_FOUNDATION;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Normal;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void InitUI()
	{
		m_tglJoinTypeDirect.OnValueChanged.RemoveAllListeners();
		m_tglJoinTypeDirect.OnValueChanged.AddListener(OnSelectDirect);
		m_tglJoinTypeApproval.OnValueChanged.RemoveAllListeners();
		m_tglJoinTypeApproval.OnValueChanged.AddListener(OnSelectApproval);
		m_tglJoinTypeClosed.OnValueChanged.RemoveAllListeners();
		m_tglJoinTypeClosed.OnValueChanged.AddListener(OnSelectClosed);
		m_badgeUI?.InitUI();
		m_btnBadgeSetting.PointerClick.RemoveAllListeners();
		m_btnBadgeSetting.PointerClick.AddListener(OnClickBadgeSetting);
		m_btnBadgeRandom.PointerClick.RemoveAllListeners();
		m_btnBadgeRandom.PointerClick.AddListener(OnClickBadgeRandom);
		m_btnCreate.PointerClick.RemoveAllListeners();
		m_btnCreate.PointerClick.AddListener(OnClickCreate);
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
		m_inputDesc.onValidateInput = NKCFilterManager.FilterEmojiInput;
		m_inputDesc.onEndEdit.RemoveAllListeners();
		m_inputDesc.onEndEdit.AddListener(OnDescChanged);
		m_inputDesc.characterLimit = 40;
	}

	public void Open()
	{
		ResetUI();
		NKCUtil.SetImageSprite(m_imgCostItem, NKCResourceUtility.GetOrLoadMiscItemSmallIcon(NKMCommonConst.Guild.Creation.ReqMiscItems[0].ItemId));
		NKCUtil.SetLabelText(m_lbCost, NKMCommonConst.Guild.Creation.ReqMiscItems[0].Count.ToString("#,##0"));
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		SetButtonState();
		UIOpened();
	}

	private void ResetUI()
	{
		m_inputName.text = string.Empty;
		m_inputDesc.text = string.Empty;
		NKCUtil.SetLabelText(m_lbGuildName, $"[{m_GuildName}]");
		m_BadgeId = 0L;
		OnNameChanged(m_inputName.text);
		SetGuildBadge(m_BadgeId);
		switch (m_JoinType)
		{
		case GuildJoinType.DirectJoin:
			NKCUtil.SetLabelText(m_lbJoinTypeDesc, NKCUtilString.GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_GUIDE_RIGHTOFF_DESC);
			break;
		case GuildJoinType.NeedApproval:
			NKCUtil.SetLabelText(m_lbJoinTypeDesc, NKCUtilString.GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_GUIDE_CONFIRM_DESC);
			break;
		case GuildJoinType.Closed:
			NKCUtil.SetLabelText(m_lbJoinTypeDesc, NKCUtilString.GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_GUIDE_BLIND_DESC);
			break;
		}
		NKCUtil.SetLabelText(m_lbDesc, string.Format(NKCStringTable.GetString("SI_DP_CONSORTIUM_CREATE_STORY_BODY_DESC"), NKCScenManager.CurrentUserData().m_UserNickName));
	}

	private void SetButtonState()
	{
		if (m_BadgeId == 0L || !m_bValidName)
		{
			m_btnCreate.Lock();
			NKCUtil.SetLabelTextColor(m_lbBtn, NKCUtil.GetColor(DISABLE_TEXT_COLOR_TEXT));
			NKCUtil.SetLabelTextColor(m_lbCost, NKCUtil.GetColor(DISABLE_TEXT_COLOR_TEXT));
		}
		else
		{
			m_btnCreate.UnLock();
			NKCUtil.SetLabelTextColor(m_lbBtn, NKCUtil.GetColor(NORMAL_TEXT_COLOR_TEXT));
			NKCUtil.SetLabelTextColor(m_lbCost, NKCUtil.GetColor(NORMAL_TEXT_COLOR_TEXT));
		}
	}

	public void OnSelectDirect(bool bValue)
	{
		if (bValue)
		{
			m_JoinType = GuildJoinType.DirectJoin;
			NKCUtil.SetLabelText(m_lbJoinTypeDesc, NKCUtilString.GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_GUIDE_RIGHTOFF_DESC);
		}
	}

	public void OnSelectApproval(bool bValue)
	{
		if (bValue)
		{
			m_JoinType = GuildJoinType.NeedApproval;
			NKCUtil.SetLabelText(m_lbJoinTypeDesc, NKCUtilString.GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_GUIDE_CONFIRM_DESC);
		}
	}

	public void OnSelectClosed(bool bValue)
	{
		if (bValue)
		{
			m_JoinType = GuildJoinType.Closed;
			NKCUtil.SetLabelText(m_lbJoinTypeDesc, NKCUtilString.GET_STRING_CONSORTIUM_CREATE_JOIN_METHOD_GUIDE_BLIND_DESC);
		}
	}

	public void SetGuildBadge(long badgeId)
	{
		if (badgeId == 0L)
		{
			NKCUtil.SetGameobjectActive(m_badgeUI, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_badgeUI, bValue: true);
			m_badgeUI.SetData(badgeId);
			m_BadgeId = badgeId;
		}
		SetButtonState();
	}

	private void OnNameChanged(string str)
	{
		Log.Debug("GuildName : " + str, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Guild/NKCUIGuildCreate.cs", 250);
		CheckGuildName(str, bChangeBadchat: false);
	}

	private void OnNameChangeEnd(string str)
	{
		CheckGuildName(str, bChangeBadchat: true);
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
		NKCUtil.SetLabelText(m_lbGuildName, $"[{m_GuildName}]");
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

	private void OnDescChanged(string str)
	{
		m_inputDesc.text = NKCFilterManager.CheckBadChat(str);
	}

	private void OnClickBadgeRandom()
	{
		int num = Random.Range(1, NKMTempletContainer<NKMGuildBadgeFrameTemplet>.Values.Count());
		int num2 = Random.Range(1, NKMTempletContainer<NKMGuildBadgeColorTemplet>.Values.Count());
		int num3 = Random.Range(1, NKMTempletContainer<NKMGuildBadgeMarkTemplet>.Values.Count());
		int num4 = Random.Range(1, NKMTempletContainer<NKMGuildBadgeColorTemplet>.Values.Count());
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(num.ToString("D3"));
		stringBuilder.Append(num2.ToString("D3"));
		stringBuilder.Append(num3.ToString("D3"));
		stringBuilder.Append(num4.ToString("D3"));
		m_BadgeId = long.Parse(stringBuilder.ToString());
		SetGuildBadge(m_BadgeId);
	}

	private void OnClickBadgeSetting()
	{
		if (!NKCPopupGuildBadgeSetting.IsInstanceOpen)
		{
			NKCPopupGuildBadgeSetting.Instance.Open(SetGuildBadge, m_BadgeId);
		}
	}

	private void OnClickCreate()
	{
		if (m_bValidName)
		{
			if (NKCScenManager.CurrentUserData().UserLevel < NKMCommonConst.Guild.Creation.UserMinLevel)
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_REQUIRE_MORE_USER_LEVEL);
			}
			else
			{
				NKCPopupResourceConfirmBox.Instance.Open(NKCUtilString.GET_STRING_CONSORTIUM_CREATE_CONFIRM_POPUP_TITLE, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_CREATE_CONFIRM_POPUP_BODY, m_GuildName), NKMCommonConst.Guild.Creation.ReqMiscItems[0].ItemId, (int)NKMCommonConst.Guild.Creation.ReqMiscItems[0].Count, OnCreateConfirm);
			}
		}
	}

	private void OnCreateConfirm()
	{
		NKCPacketSender.Send_NKMPacket_GUILD_CREATE_REQ(m_inputName.text, m_JoinType, m_BadgeId, m_inputDesc.text);
	}

	public override void OnGuildDataChanged()
	{
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GUILD_LOBBY);
	}
}
