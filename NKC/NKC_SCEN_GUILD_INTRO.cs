using NKC.UI.Guild;
using NKM;

namespace NKC;

public class NKC_SCEN_GUILD_INTRO : NKC_SCEN_BASIC
{
	private NKCAssetResourceData m_UILoadResourceData;

	private NKCUIGuildIntro m_NKCUIGuildIntro;

	public NKC_SCEN_GUILD_INTRO()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_GUILD_INTRO;
	}

	public void ClearCacheData()
	{
		if (m_NKCUIGuildIntro != null)
		{
			m_NKCUIGuildIntro.CloseInstance();
			m_NKCUIGuildIntro = null;
		}
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (m_NKCUIGuildIntro == null)
		{
			m_UILoadResourceData = NKCUIGuildIntro.OpenInstanceAsync();
		}
		else
		{
			m_UILoadResourceData = null;
		}
	}

	public override void ScenLoadUpdate()
	{
		if (!NKCAssetResourceManager.IsLoadEnd())
		{
			return;
		}
		if (m_NKCUIGuildIntro == null && m_UILoadResourceData != null)
		{
			if (!NKCUIGuildIntro.CheckInstanceLoaded(m_UILoadResourceData, out m_NKCUIGuildIntro))
			{
				return;
			}
			m_UILoadResourceData = null;
		}
		ScenLoadLastStart();
	}

	public override void ScenLoadComplete()
	{
		base.ScenLoadComplete();
		if (m_NKCUIGuildIntro != null)
		{
			m_NKCUIGuildIntro.InitUI();
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		NKCCamera.EnableBloom(bEnable: false);
		if (m_NKCUIGuildIntro != null)
		{
			if (NKCScenManager.CurrentUserData().UserProfileData == null)
			{
				NKCPacketSender.Send_NKMPacket_MY_USER_PROFILE_INFO_REQ();
			}
			m_NKCUIGuildIntro.Open();
			TutorialCheck();
		}
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		if (m_NKCUIGuildIntro != null)
		{
			m_NKCUIGuildIntro.Close();
		}
		ClearCacheData();
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}

	public void TutorialCheck()
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_INTRO)
		{
			NKCTutorialManager.TutorialRequired(TutorialPoint.ConsortiumMain);
		}
	}
}
