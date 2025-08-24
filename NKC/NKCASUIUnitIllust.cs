using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public abstract class NKCASUIUnitIllust : NKMObjectPoolData
{
	public enum eAnimation
	{
		NONE = -1,
		UNIT_ENUM_START = 0,
		UNIT_TOUCH = 1,
		UNIT_IDLE = 2,
		UNIT_LAUGH = 3,
		UNIT_HATE = 4,
		UNIT_SERIOUS = 5,
		UNIT_SERIOUS2 = 6,
		UNIT_SURPRISE = 7,
		UNIT_PRIDE = 8,
		UNIT_DESPAIR = 9,
		UNIT_CONFUSION = 10,
		UNIT_CONFUSION2 = 11,
		UNIT_CONFUSION3 = 12,
		UNIT_HURT = 13,
		UNIT_TIRED = 14,
		UNIT_SKILL = 15,
		UNIT_HYPER_CUTIN = 16,
		UNIT_ENUM_END = 17,
		SD_ENUM_START = 1000,
		SD_IDLE = 1001,
		SD_ATTACK = 1002,
		SD_WORKING = 1003,
		SD_MINING = 1004,
		SD_WALK = 1005,
		SD_RUN = 1006,
		SD_TOUCH = 1007,
		SD_DRAG = 1008,
		SD_WIN = 1009,
		SD_START = 1010,
		SD_DOWN = 1011,
		SD_ENUM_END = 1012,
		SHIP_ENUM_START = 2000,
		SHIP_IDLE = 2001,
		SHIP_ENUM_END = 2002,
		BASE_STONE = 2003,
		BASE_STONE_END = 2004,
		BASE_STONE_FAIL = 2005,
		BASE_HURDLE = 2006,
		BASE_HURDLE_END = 2007,
		BASE_HURDLE_FAIL = 2008,
		BASE_VH = 2009,
		BASE_VH_END = 2010,
		BASE_VH_FAIL = 2011,
		BASE_TROPHY = 2012,
		BASE_TROPHY_END = 2013,
		BASE_GOAL = 2014,
		SKILL1 = 2015,
		DAMAGE_DOWN = 2016,
		INVALID = 2017
	}

	public enum UnitIllustType
	{
		Spine,
		Cubism
	}

	public const string TOUCH_ANIM_NAME = "TOUCH";

	public const string HYPER_CUTIN_ANIMNAME = "BASE";

	protected eAnimation m_eDefaultAnimation = eAnimation.UNIT_IDLE;

	protected eAnimation m_eCurrentAnimation = eAnimation.NONE;

	protected NKCASMaterial m_matTemp;

	protected bool m_bRectCalculated;

	public override void Close()
	{
		if (m_matTemp != null)
		{
			m_matTemp.Close();
		}
		base.Close();
	}

	public override void Unload()
	{
		UnloadEffectMaterial();
		base.Unload();
	}

	public abstract bool PurgeHyperCutsceneIllust();

	public abstract Color GetColor();

	public abstract void SetColor(Color color);

	public abstract void SetColor(float fR = -1f, float fG = -1f, float fB = -1f, float fA = -1f);

	public abstract void SetParent(Transform parent, bool worldPositionStays);

	public abstract float GetAnimationTime(eAnimation eAnim);

	public abstract float GetAnimationTime(string animName);

	public abstract eAnimation GetCurrentAnimation(int trackIndex = 0);

	public abstract string GetCurrentAnimationName(int trackIndex = 0);

	public abstract float GetCurrentAnimationTime(int trackIndex = 0);

	public abstract void SetCurrentAnimationTime(float time, int trackIndex = 0, bool immediate = false);

	public abstract void SetAnimSpeed(float value);

	public abstract float GetAnimSpeed();

	public abstract void SetAnimation(eAnimation eAnim, bool loop, int trackIndex = 0, bool bForceRestart = true, float fStartTime = 0f, bool bReturnDefault = true);

	public abstract void SetAnimation(string AnimationName, bool loop, int trackIndex = 0, bool bForceRestart = true, float fStartTime = 0f, bool bReturnDefault = true);

	public abstract void ForceUpdateAnimation();

	public abstract void InitializeAnimation();

	public abstract RectTransform GetRectTransform();

	public abstract void SetMaterial(Material mat);

	public abstract void SetDefaultMaterial();

	public abstract void SetVFX(bool bSet);

	public abstract Transform GetTalkTransform(bool bLeft);

	public abstract Transform GetResultTalkTransform();

	public abstract Vector3 GetBoneWorldPosition(string name);

	public abstract void SetTimeScale(float value);

	protected void UnloadEffectMaterial()
	{
		if (m_matTemp != null)
		{
			m_matTemp.Close();
			m_matTemp.Unload();
			m_matTemp = null;
		}
	}

	public void SetEffectMaterial(NKCUICharacterView.EffectType effect)
	{
		UnloadEffectMaterial();
		m_matTemp = MakeEffectMaterial(effect);
		if (m_matTemp != null)
		{
			SetMaterial(m_matTemp.m_Material);
			ProcessEffect(effect);
		}
		else
		{
			Debug.LogWarning("EffectMaterial Load Failed!");
			SetDefaultMaterial();
			ProcessEffect(NKCUICharacterView.EffectType.None);
		}
	}

	protected abstract NKCASMaterial MakeEffectMaterial(NKCUICharacterView.EffectType effect);

	protected abstract void ProcessEffect(NKCUICharacterView.EffectType effect);

	public void SetAnchoredPosition(Vector2 pos)
	{
		RectTransform rectTransform = GetRectTransform();
		if (rectTransform != null)
		{
			rectTransform.anchoredPosition = pos;
		}
	}

	public void SetDefaultAnimation(eAnimation value, bool bPlay = true, bool bInitialize = false, float fStartTime = 0f)
	{
		m_eDefaultAnimation = value;
		if (bPlay)
		{
			if (bInitialize)
			{
				InitializeAnimation();
			}
			SetAnimation(value, loop: true, 0, bForceRestart: false, fStartTime);
		}
	}

	public void SetDefaultAnimation(NKMUnitTempletBase unitTempletBase, bool bPlay = true, bool bInitialize = false)
	{
		if (unitTempletBase != null)
		{
			if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP && !unitTempletBase.m_bMonster)
			{
				SetDefaultAnimation(eAnimation.SHIP_IDLE, bPlay, bInitialize);
			}
			else
			{
				SetDefaultAnimation(eAnimation.UNIT_IDLE, bPlay, bInitialize);
			}
		}
	}

	public abstract bool HasAnimation(eAnimation value);

	public abstract bool HasAnimation(string name);

	public abstract void SetIllustBackgroundEnable(bool bValue);

	public abstract void SetSkin(string skinName);

	public abstract bool HasSkin(string skinName);

	public abstract int GetSkinOptionCount();

	public abstract void SetSkinOption(int index);

	public static string GetAnimationName(eAnimation eAnim)
	{
		return eAnim switch
		{
			eAnimation.UNIT_TOUCH => "TOUCH", 
			eAnimation.UNIT_IDLE => "IDLE", 
			eAnimation.UNIT_LAUGH => "LAUGH", 
			eAnimation.UNIT_HATE => "HATE", 
			eAnimation.UNIT_SERIOUS => "SERIOUS", 
			eAnimation.UNIT_SERIOUS2 => "SERIOUS2", 
			eAnimation.UNIT_SURPRISE => "SURPRISE", 
			eAnimation.UNIT_PRIDE => "PRIDE", 
			eAnimation.UNIT_DESPAIR => "DESPAIR", 
			eAnimation.UNIT_CONFUSION => "CONFUSION", 
			eAnimation.UNIT_CONFUSION2 => "CONFUSION2", 
			eAnimation.UNIT_CONFUSION3 => "CONFUSION3", 
			eAnimation.UNIT_HURT => "HURT", 
			eAnimation.UNIT_TIRED => "TIRED", 
			eAnimation.UNIT_SKILL => "SKILL", 
			eAnimation.SD_IDLE => "ASTAND", 
			eAnimation.SD_ATTACK => "ATTACK", 
			eAnimation.SD_DRAG => "DRAG", 
			eAnimation.SD_MINING => "MINING", 
			eAnimation.SD_RUN => "RUN", 
			eAnimation.SD_TOUCH => "TOUCH", 
			eAnimation.SD_WALK => "WALK", 
			eAnimation.SD_WIN => "WIN", 
			eAnimation.SD_WORKING => "WORKING", 
			eAnimation.SD_START => "START", 
			eAnimation.SD_DOWN => "DOWN", 
			eAnimation.SHIP_IDLE => "ASTAND", 
			eAnimation.BASE_STONE => "BASE_STONE", 
			eAnimation.BASE_STONE_END => "BASE_STONE_END", 
			eAnimation.BASE_STONE_FAIL => "BASE_STONE_FAIL", 
			eAnimation.BASE_HURDLE => "BASE_HURDLE", 
			eAnimation.BASE_HURDLE_END => "BASE_HURDLE_END", 
			eAnimation.BASE_HURDLE_FAIL => "BASE_HURDLE_FAIL", 
			eAnimation.BASE_VH => "BASE_VH", 
			eAnimation.BASE_VH_END => "BASE_VH_END", 
			eAnimation.BASE_VH_FAIL => "BASE_VH_FAIL", 
			eAnimation.BASE_TROPHY => "BASE_TROPHY", 
			eAnimation.BASE_TROPHY_END => "BASE_TROPHY_END", 
			eAnimation.BASE_GOAL => "BASE_GOAL", 
			eAnimation.SKILL1 => "SKILL1", 
			eAnimation.DAMAGE_DOWN => "DAMAGE_DOWN", 
			eAnimation.UNIT_HYPER_CUTIN => "BASE", 
			_ => "", 
		};
	}

	public static bool IsEmotionAnimation(eAnimation eAnim)
	{
		if ((uint)(eAnim - 2) <= 12u)
		{
			return true;
		}
		return false;
	}

	public static bool IsEmotionAnimation(string strAnim)
	{
		if (GetAnimationName(eAnimation.UNIT_IDLE).StartsWith(strAnim))
		{
			return true;
		}
		if (GetAnimationName(eAnimation.UNIT_LAUGH).StartsWith(strAnim))
		{
			return true;
		}
		if (GetAnimationName(eAnimation.UNIT_HATE).StartsWith(strAnim))
		{
			return true;
		}
		if (GetAnimationName(eAnimation.UNIT_SERIOUS).StartsWith(strAnim))
		{
			return true;
		}
		if (GetAnimationName(eAnimation.UNIT_SURPRISE).StartsWith(strAnim))
		{
			return true;
		}
		if (GetAnimationName(eAnimation.UNIT_PRIDE).StartsWith(strAnim))
		{
			return true;
		}
		if (GetAnimationName(eAnimation.UNIT_DESPAIR).StartsWith(strAnim))
		{
			return true;
		}
		if (GetAnimationName(eAnimation.UNIT_CONFUSION).StartsWith(strAnim))
		{
			return true;
		}
		if (GetAnimationName(eAnimation.UNIT_HURT).StartsWith(strAnim))
		{
			return true;
		}
		if (GetAnimationName(eAnimation.UNIT_TIRED).StartsWith(strAnim))
		{
			return true;
		}
		return false;
	}

	public void InvalidateWorldRect()
	{
		m_bRectCalculated = false;
	}

	public abstract Rect GetWorldRect(bool bRecalculateBound = false);
}
