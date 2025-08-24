using ClientPacket.Warfare;
using NKC.UI;
using NKM;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_SHADOW_BATTLE : NKC_SCEN_BASIC
{
	private NKCUIShadowBattle m_shadowBattle;

	private NKCUIManager.LoadedUIData m_loadUIData;

	private int m_palaceID;

	public NKC_SCEN_SHADOW_BATTLE()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_SHADOW_BATTLE;
	}

	public void SetShadowPalaceID(int palaceID)
	{
		m_palaceID = palaceID;
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (!NKCUIManager.IsValid(m_loadUIData))
		{
			m_loadUIData = NKCUIShadowBattle.OpenNewInstanceAsync();
		}
	}

	public override void ScenLoadUIComplete()
	{
		base.ScenLoadUIComplete();
		if (m_shadowBattle == null)
		{
			if (m_loadUIData != null && m_loadUIData.CheckLoadAndGetInstance<NKCUIShadowBattle>(out m_shadowBattle))
			{
				m_shadowBattle.Init();
			}
			else
			{
				Debug.LogError("NKC_SCEN_SHADOW_BATTLE.ScenLoadUIComplete - ui load fail");
			}
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		m_shadowBattle?.Open(m_palaceID);
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		m_shadowBattle?.Close();
		UnloadUI();
	}

	public override void UnloadUI()
	{
		base.UnloadUI();
		m_shadowBattle = null;
		m_loadUIData?.CloseInstance();
		m_loadUIData = null;
	}

	public void OnRecv(NKMPacket_WARFARE_GAME_GIVE_UP_ACK sPacket)
	{
		if (NKCUIShadowBattle.IsInstanceOpen)
		{
			m_shadowBattle.StartCurrentBattle();
		}
	}
}
