using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.HUD;

public class NKCGameHudPractice : MonoBehaviour
{
	private NKCGameHud m_GameHud;

	public NKCUIComButton m_btnUpsideBack;

	public NKCUIComToggle m_tglAI;

	public NKCUIComStateButton m_csbtnResetPlayer;

	public NKCUIComStateButton m_csbtnResetEnemy;

	public NKCUIComStateButton m_csbtnSummonAir;

	public NKCUIComStateButton m_csbtnSummonGround;

	public NKCUIComStateButton m_csbtnSkill01;

	public NKCUIComStateButton m_csbtnSkill02;

	public NKCUIComToggle m_tglHeal;

	public NKCUIComToggle m_tglFixedDamage;

	public NKCUIComToggle m_tglGameSpeed;

	public NKCUISkillSlot m_NKCUISkillSlot_Practice_1;

	public NKCUISkillSlot m_NKCUISkillSlot_Practice_2;

	private NKCGameClient GameClient => m_GameHud.GetGameClient();

	public void Init(NKCGameHud gameHud)
	{
		m_GameHud = gameHud;
		NKCUtil.SetButtonClickDelegate(m_btnUpsideBack, PracticeGoBack);
		NKCUtil.SetButtonClickDelegate(m_tglAI, PracticeAIEnable);
		NKCUtil.SetButtonClickDelegate(m_csbtnResetPlayer, PracticeResetMy);
		NKCUtil.SetButtonClickDelegate(m_csbtnResetEnemy, PracticeResetEnemy);
		NKCUtil.SetButtonClickDelegate(m_csbtnSummonAir, PracticeRespawnAir);
		NKCUtil.SetButtonClickDelegate(m_csbtnSummonGround, PracticeRespawnLand);
		m_NKCUISkillSlot_Practice_1.Init(null);
		m_NKCUISkillSlot_Practice_2.Init(null);
		NKCUtil.SetButtonClickDelegate(m_csbtnSkill01, PracticeSkillReset);
		NKCUtil.SetButtonClickDelegate(m_csbtnSkill02, PracticeHyperSkillReset);
		NKCUtil.SetButtonClickDelegate(m_tglHeal, PracticeHealEnable);
		NKCUtil.SetButtonClickDelegate(m_tglFixedDamage, PracticeFixedDamageEnable);
		NKCUtil.SetButtonClickDelegate(m_tglGameSpeed, ToggleSlowMode);
	}

	public void SetEnable(bool value)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, value);
		if (value)
		{
			if (m_tglAI != null)
			{
				m_tglAI.Select(bSelect: false);
			}
			if (m_tglHeal != null)
			{
				m_tglHeal.Select(bSelect: true);
			}
			if (m_tglFixedDamage != null)
			{
				m_tglFixedDamage.Select(bSelect: false);
			}
			if (m_tglGameSpeed != null)
			{
				m_tglGameSpeed.Select(bSelect: false);
			}
		}
	}

	public void LoadComplete(NKMGameData cNKMGameData)
	{
		if (cNKMGameData == null || cNKMGameData.GetGameType() != NKM_GAME_TYPE.NGT_PRACTICE || cNKMGameData.m_NKMGameTeamDataA.m_listUnitData.Count <= 0)
		{
			return;
		}
		NKMUnitData nKMUnitData = cNKMGameData.m_NKMGameTeamDataA.m_listUnitData[0];
		if (nKMUnitData == null)
		{
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(nKMUnitData.m_UnitID);
		if (unitTempletBase == null)
		{
			return;
		}
		NKMUnitSkillTemplet nKMUnitSkillTemplet = null;
		NKMUnitSkillTemplet nKMUnitSkillTemplet2 = null;
		for (int i = 0; i < unitTempletBase.GetSkillCount(); i++)
		{
			NKMUnitSkillTemplet unitSkillTemplet = NKMUnitSkillManager.GetUnitSkillTemplet(unitTempletBase.GetSkillStrID(i), nKMUnitData);
			if (unitSkillTemplet != null)
			{
				if (unitSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_SKILL)
				{
					nKMUnitSkillTemplet = unitSkillTemplet;
				}
				else if (unitSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER)
				{
					nKMUnitSkillTemplet2 = unitSkillTemplet;
				}
			}
		}
		if (nKMUnitSkillTemplet != null && !NKMUnitSkillManager.IsLockedSkill(nKMUnitSkillTemplet.m_ID, nKMUnitData.m_LimitBreakLevel))
		{
			m_NKCUISkillSlot_Practice_1.SetData(nKMUnitSkillTemplet, bIsHyper: false);
		}
		else
		{
			m_NKCUISkillSlot_Practice_1.LockSkill(value: true);
		}
		if (nKMUnitSkillTemplet2 != null && !NKMUnitSkillManager.IsLockedSkill(nKMUnitSkillTemplet2.m_ID, nKMUnitData.m_LimitBreakLevel))
		{
			m_NKCUISkillSlot_Practice_2.SetData(nKMUnitSkillTemplet2, bIsHyper: true);
		}
		else
		{
			m_NKCUISkillSlot_Practice_2.LockSkill(value: true);
		}
	}

	public void PracticeGoBack()
	{
		if (GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().EndPracticeGame();
		}
	}

	private void PracticeAIEnable(bool bOn)
	{
		if (GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
		{
			if (bOn)
			{
				NKCLocalServerManager.GetGameServerLocal().GetGameRuntimeData().m_NKMGameRuntimeTeamDataB.m_bAIDisable = false;
			}
			else
			{
				NKCLocalServerManager.GetGameServerLocal().GetGameRuntimeData().m_NKMGameRuntimeTeamDataB.m_bAIDisable = true;
			}
		}
	}

	public void PracticeResetMy()
	{
		if (GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
		{
			NKCLocalServerManager.LocalGameUnitAllKill();
		}
	}

	public void PracticeResetEnemy()
	{
		if (GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
		{
			NKCLocalServerManager.LocalGameUnitAllKill(bEnemy: true);
		}
	}

	public void PracticeRespawnAir()
	{
		if (GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
		{
			NKCLocalServerManager.GetGameServerLocal().PracticeBossStateChange("USN_RESPAWN_AIR");
		}
	}

	public void PracticeRespawnLand()
	{
		if (GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
		{
			NKCLocalServerManager.GetGameServerLocal().PracticeBossStateChange("USN_RESPAWN_LAND");
		}
	}

	public void PracticeSkillReset()
	{
		if (GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE && NKCScenManager.GetScenManager().GetGameClient() != null)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_GAME_SKILL_NORMAL_COOL_RESET();
		}
	}

	public void PracticeHyperSkillReset()
	{
		if (GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE && NKCScenManager.GetScenManager().GetGameClient() != null)
		{
			NKCScenManager.GetScenManager().GetGameClient().UI_GAME_SKILL_HYPER_COOL_RESET();
		}
	}

	public void PracticeHealEnable(bool value)
	{
		if (GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
		{
			NKCLocalServerManager.GetGameServerLocal().PracticeHealEnable(value);
		}
	}

	public void PracticeFixedDamageEnable(bool value)
	{
		if (GameClient.GetGameData().GetGameType() == NKM_GAME_TYPE.NGT_PRACTICE)
		{
			NKCLocalServerManager.GetGameServerLocal().PracticeFixedDamageEnable(value);
		}
	}

	public void ToggleSlowMode(bool value)
	{
		NKM_GAME_SPEED_TYPE eNKM_GAME_SPEED_TYPE = (value ? NKM_GAME_SPEED_TYPE.NGST_05 : NKM_GAME_SPEED_TYPE.NGST_1);
		GameClient.Send_Packet_GAME_SPEED_2X_REQ(eNKM_GAME_SPEED_TYPE);
	}
}
