using NKC.UI;
using NKM;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_SHADOW_PALACE : NKC_SCEN_BASIC
{
	private NKCUIShadowPalace m_shadowPalace;

	private NKCUIManager.LoadedUIData m_loadUIData;

	public NKC_SCEN_SHADOW_PALACE()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_SHADOW_PALACE;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (!NKCUIManager.IsValid(m_loadUIData))
		{
			m_loadUIData = NKCUIShadowPalace.OpenNewInstanceAsync();
		}
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (m_shadowPalace == null)
		{
			if (m_loadUIData != null && m_loadUIData.CheckLoadAndGetInstance<NKCUIShadowPalace>(out m_shadowPalace))
			{
				m_shadowPalace.Init();
			}
			else
			{
				Debug.LogError("NKC_SCEN_SHADOW_PALACE.ScenLoadUIComplete - ui load fail");
			}
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		m_shadowPalace?.Open(NKCScenManager.CurrentUserData().m_ShadowPalace.currentPalaceId);
		if (NKCScenManager.CurrentUserData().UserProfileData == null)
		{
			NKCPacketSender.Send_NKMPacket_MY_USER_PROFILE_INFO_REQ();
		}
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		m_shadowPalace?.Close();
		UnloadUI();
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_shadowPalace = null;
		m_loadUIData?.CloseInstance();
		m_loadUIData = null;
	}
}
