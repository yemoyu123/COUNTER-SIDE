using NKM;
using UnityEngine;

namespace NKC;

public class NKCASUnitShadow : NKMObjectPoolData
{
	public NKCAssetInstanceData m_ShadowSpriteInstant;

	public GameObject m_TeamColor_Green;

	public GameObject m_TeamColor_Green_fx_mark_green_big;

	public GameObject m_TeamColor_Green_fx_mark_green_mini;

	public GameObject m_TeamColor_Red;

	public GameObject m_TeamColor_Red_fx_mark_red_big;

	public GameObject m_TeamColor_Red_fx_mark_red_mini;

	public GameObject m_Rearmament_Green;

	public GameObject m_Rearmament_Red;

	public GameObject m_TeamColor_common_shadow;

	public NKCComGroupColor m_NKCComGroupColor;

	public NKCASUnitShadow(bool bAsync = false)
	{
		m_NKM_OBJECT_POOL_TYPE = NKM_OBJECT_POOL_TYPE.NOPT_NKCASUnitShadow;
		Load(bAsync);
	}

	public override void Load(bool bAsync)
	{
		m_ShadowSpriteInstant = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UNIT_GAME_NKM_UNIT", "NKM_UNIT_SHADOW", bAsync);
	}

	public override bool LoadComplete()
	{
		if (m_ShadowSpriteInstant == null || m_ShadowSpriteInstant.m_Instant == null)
		{
			Debug.LogError("Shadow Sprite load failed!!");
			return false;
		}
		m_ShadowSpriteInstant.m_Instant.transform.SetParent(NKCScenManager.GetScenManager().Get_SCEN_GAME().Get_NKC_SCEN_GAME_UI_DATA()
			.Get_GAME_BATTLE_UNIT_SHADOW()
			.transform, worldPositionStays: false);
			m_TeamColor_Green = m_ShadowSpriteInstant.m_Instant.transform.Find("fx_mark_green").gameObject;
			m_TeamColor_Green_fx_mark_green_big = m_TeamColor_Green.transform.Find("fx_mark_green_big").gameObject;
			m_TeamColor_Green_fx_mark_green_mini = m_TeamColor_Green.transform.Find("fx_mark_green_mini").gameObject;
			m_TeamColor_Red = m_ShadowSpriteInstant.m_Instant.transform.Find("fx_mark_red").gameObject;
			m_TeamColor_Red_fx_mark_red_big = m_TeamColor_Red.transform.Find("fx_mark_red_big").gameObject;
			m_TeamColor_Red_fx_mark_red_mini = m_TeamColor_Red.transform.Find("fx_mark_red_mini").gameObject;
			m_Rearmament_Green = m_ShadowSpriteInstant.m_Instant.transform.Find("FX_REARMAMENT_GREEN").gameObject;
			m_Rearmament_Red = m_ShadowSpriteInstant.m_Instant.transform.Find("FX_REARMAMENT_RED").gameObject;
			m_TeamColor_common_shadow = m_ShadowSpriteInstant.m_Instant.transform.Find("common_shadow").gameObject;
			m_NKCComGroupColor = m_ShadowSpriteInstant.m_Instant.GetComponent<NKCComGroupColor>();
			return true;
		}

		public override void Open()
		{
			if (!m_ShadowSpriteInstant.m_Instant.activeSelf)
			{
				m_ShadowSpriteInstant.m_Instant.SetActive(value: true);
			}
		}

		public override void Close()
		{
			if (m_ShadowSpriteInstant.m_Instant.activeSelf)
			{
				m_ShadowSpriteInstant.m_Instant.SetActive(value: false);
			}
		}

		public override void Unload()
		{
			m_TeamColor_Green = null;
			m_TeamColor_Green_fx_mark_green_big = null;
			m_TeamColor_Green_fx_mark_green_mini = null;
			m_TeamColor_Red = null;
			m_TeamColor_Red_fx_mark_red_big = null;
			m_TeamColor_Red_fx_mark_red_mini = null;
			m_TeamColor_common_shadow = null;
			m_NKCComGroupColor = null;
			NKCAssetResourceManager.CloseInstance(m_ShadowSpriteInstant);
			m_ShadowSpriteInstant = null;
		}

		public void SetShadowType(NKC_TEAM_COLOR_TYPE eNKC_TEAM_COLOR_TYPE, bool bTeamA, bool bRearm)
		{
			SetShadowType(eNKC_TEAM_COLOR_TYPE);
			SetShadowTeam(bTeamA, bRearm);
		}

		public void SetShadowType(NKC_TEAM_COLOR_TYPE eNKC_TEAM_COLOR_TYPE)
		{
			switch (eNKC_TEAM_COLOR_TYPE)
			{
			case NKC_TEAM_COLOR_TYPE.NTCT_NO:
				NKCUtil.SetGameobjectActive(m_TeamColor_Green_fx_mark_green_big, bValue: false);
				NKCUtil.SetGameobjectActive(m_TeamColor_Green_fx_mark_green_mini, bValue: false);
				NKCUtil.SetGameobjectActive(m_TeamColor_Red_fx_mark_red_big, bValue: false);
				NKCUtil.SetGameobjectActive(m_TeamColor_Red_fx_mark_red_mini, bValue: false);
				NKCUtil.SetGameobjectActive(m_TeamColor_common_shadow, bValue: false);
				break;
			case NKC_TEAM_COLOR_TYPE.NTCT_ONLY_SHADOW:
				NKCUtil.SetGameobjectActive(m_TeamColor_Green_fx_mark_green_big, bValue: false);
				NKCUtil.SetGameobjectActive(m_TeamColor_Green_fx_mark_green_mini, bValue: false);
				NKCUtil.SetGameobjectActive(m_TeamColor_Red_fx_mark_red_big, bValue: false);
				NKCUtil.SetGameobjectActive(m_TeamColor_Red_fx_mark_red_mini, bValue: false);
				NKCUtil.SetGameobjectActive(m_TeamColor_common_shadow, bValue: true);
				break;
			case NKC_TEAM_COLOR_TYPE.NTCT_SIMPLE:
				NKCUtil.SetGameobjectActive(m_TeamColor_Green_fx_mark_green_big, bValue: true);
				NKCUtil.SetGameobjectActive(m_TeamColor_Green_fx_mark_green_mini, bValue: false);
				NKCUtil.SetGameobjectActive(m_TeamColor_Red_fx_mark_red_big, bValue: true);
				NKCUtil.SetGameobjectActive(m_TeamColor_Red_fx_mark_red_mini, bValue: false);
				NKCUtil.SetGameobjectActive(m_TeamColor_common_shadow, bValue: true);
				break;
			case NKC_TEAM_COLOR_TYPE.NTCT_FULL:
				NKCUtil.SetGameobjectActive(m_TeamColor_Green_fx_mark_green_big, bValue: true);
				NKCUtil.SetGameobjectActive(m_TeamColor_Green_fx_mark_green_mini, bValue: true);
				NKCUtil.SetGameobjectActive(m_TeamColor_Red_fx_mark_red_big, bValue: true);
				NKCUtil.SetGameobjectActive(m_TeamColor_Red_fx_mark_red_mini, bValue: true);
				NKCUtil.SetGameobjectActive(m_TeamColor_common_shadow, bValue: true);
				break;
			case NKC_TEAM_COLOR_TYPE.NTCT_SIMPLE_NO_SHADOW:
				NKCUtil.SetGameobjectActive(m_TeamColor_Green_fx_mark_green_big, bValue: true);
				NKCUtil.SetGameobjectActive(m_TeamColor_Green_fx_mark_green_mini, bValue: false);
				NKCUtil.SetGameobjectActive(m_TeamColor_Red_fx_mark_red_big, bValue: true);
				NKCUtil.SetGameobjectActive(m_TeamColor_Red_fx_mark_red_mini, bValue: false);
				NKCUtil.SetGameobjectActive(m_TeamColor_common_shadow, bValue: false);
				break;
			}
		}

		public void SetShadowTeam(bool bTeamA, bool bRearm)
		{
			NKCUtil.SetGameobjectActive(m_Rearmament_Green, bTeamA && bRearm);
			NKCUtil.SetGameobjectActive(m_Rearmament_Red, !bTeamA && bRearm);
			NKCUtil.SetGameobjectActive(m_TeamColor_Green, bTeamA && !bRearm);
			NKCUtil.SetGameobjectActive(m_TeamColor_Red, !bTeamA && !bRearm);
		}
	}
