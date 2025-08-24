using NKC.UI.HUD;
using UnityEngine;

namespace NKC;

public class NKC_SCEN_GAME_UI_DATA
{
	public NKCAssetInstanceData m_NUF_GAME_PREFAB;

	public GameObject m_GAME_BATTLE_MAP;

	public GameObject m_GAME_BATTLE_UNIT;

	public GameObject m_GAME_BATTLE_UNIT_SHADOW;

	public GameObject m_GAME_BATTLE_UNIT_MOTION_BLUR;

	public GameObject m_GAME_BATTLE_UNIT_VIEWER;

	public GameObject m_NUF_GAME_HUD_MINI_MAP;

	public GameObject m_NUM_GAME_BATTLE_EFFECT;

	public NKCGameHudObjects m_NUFGameObjects;

	public NKC_SCEN_GAME_UI_DATA()
	{
		Init();
	}

	public void Init()
	{
		m_NUF_GAME_PREFAB = null;
		m_GAME_BATTLE_MAP = null;
		m_GAME_BATTLE_UNIT = null;
		m_GAME_BATTLE_UNIT_SHADOW = null;
		m_GAME_BATTLE_UNIT_MOTION_BLUR = null;
		m_GAME_BATTLE_UNIT_VIEWER = null;
		m_NUF_GAME_HUD_MINI_MAP = null;
		m_NUM_GAME_BATTLE_EFFECT = null;
		m_NUFGameObjects = null;
	}

	public GameObject Get_GAME_BATTLE_MAP()
	{
		return m_GAME_BATTLE_MAP;
	}

	public GameObject Get_GAME_BATTLE_UNIT()
	{
		return m_GAME_BATTLE_UNIT;
	}

	public GameObject Get_GAME_BATTLE_UNIT_SHADOW()
	{
		return m_GAME_BATTLE_UNIT_SHADOW;
	}

	public GameObject Get_GAME_BATTLE_UNIT_MOTION_BLUR()
	{
		return m_GAME_BATTLE_UNIT_MOTION_BLUR;
	}

	public GameObject Get_GAME_BATTLE_UNIT_VIEWER()
	{
		return m_GAME_BATTLE_UNIT_VIEWER;
	}

	public GameObject Get_NUF_GAME_HUD_MINI_MAP()
	{
		return m_NUF_GAME_HUD_MINI_MAP;
	}

	public GameObject Get_NUM_GAME_BATTLE_EFFECT()
	{
		return m_NUM_GAME_BATTLE_EFFECT;
	}

	public NKCGameHudObjects GetHudObjects()
	{
		return m_NUFGameObjects;
	}

	public GameObject Get_NUF_BEFORE_HUD_EFFECT()
	{
		if (!(m_NUFGameObjects != null))
		{
			return null;
		}
		return m_NUFGameObjects.m_NUF_BEFORE_HUD_EFFECT;
	}

	public GameObject Get_NUF_BEFORE_HUD_CONTROL_EFFECT_ANCHOR()
	{
		if (!(m_NUFGameObjects != null))
		{
			return null;
		}
		return m_NUFGameObjects.m_NUF_BEFORE_HUD_CONTROL_EFFECT_ANCHOR;
	}

	public GameObject Get_NUF_BEFORE_HUD_CONTROL_EFFECT()
	{
		if (!(m_NUFGameObjects != null))
		{
			return null;
		}
		return m_NUFGameObjects.m_NUF_BEFORE_HUD_CONTROL_EFFECT;
	}

	public GameObject Get_NUF_AFTER_HUD_EFFECT()
	{
		if (!(m_NUFGameObjects != null))
		{
			return null;
		}
		return m_NUFGameObjects.m_NUF_AFTER_HUD_EFFECT;
	}
}
