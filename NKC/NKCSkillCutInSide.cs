using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCSkillCutInSide
{
	public string m_MidEffectName;

	public string m_FrontEffectName;

	public NKCASEffect m_SkillCutInMID;

	public NKCASEffect m_SkillCutInFRONT;

	public SpriteRenderer m_SkillCutInSpriteRenderer;

	public Text m_SkillCutInTextUnitName;

	public Text m_SkillCutInTextSkillName;

	public void Load(List<NKCASEffect> listEffectLoadTemp, string midEffectName, string frontEffectName)
	{
		m_MidEffectName = midEffectName;
		m_FrontEffectName = frontEffectName;
		NKCASEffect item = (NKCASEffect)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASEffect, m_MidEffectName, m_MidEffectName, bAsync: true);
		listEffectLoadTemp.Add(item);
		item = (NKCASEffect)NKCScenManager.GetScenManager().GetObjectPool().OpenObj(NKM_OBJECT_POOL_TYPE.NOPT_NKCASEffect, m_FrontEffectName, m_FrontEffectName, bAsync: true);
		listEffectLoadTemp.Add(item);
	}

	public void Play(NKCEffectManager cNKCEffectManager, Sprite faceSprite, string unitName, string skillName)
	{
		if (m_SkillCutInMID == null)
		{
			m_SkillCutInMID = cNKCEffectManager.UseEffect(0, m_MidEffectName, m_MidEffectName, NKM_EFFECT_PARENT_TYPE.NEPT_NUM_GAME_BATTLE_EFFECT, -1f, -1f, -1f, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, -1f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: false, "BASE", 1f, bNotStart: true);
			m_SkillCutInFRONT = cNKCEffectManager.UseEffect(0, m_FrontEffectName, m_FrontEffectName, NKM_EFFECT_PARENT_TYPE.NEPT_NUF_AFTER_HUD_EFFECT, -1f, -1f, -1f, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, -1f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: false, "BASE", 1f, bNotStart: true);
			if (m_SkillCutInMID != null && m_SkillCutInMID.m_EffectInstant != null && m_SkillCutInMID.m_EffectInstant.m_Instant != null)
			{
				Transform transform = m_SkillCutInMID.m_EffectInstant.m_Instant.transform.Find("RENDER_GROUP/RENDER_CAM_CUTIN_PORTRAIT/SPRITE_PORTRAIT");
				if (transform != null)
				{
					m_SkillCutInSpriteRenderer = transform.GetComponent<SpriteRenderer>();
				}
			}
			if (m_SkillCutInFRONT != null && m_SkillCutInFRONT.m_EffectInstant != null && m_SkillCutInFRONT.m_EffectInstant.m_Instant != null)
			{
				Transform transform2 = m_SkillCutInFRONT.m_EffectInstant.m_Instant.transform.Find("DESC/POS_TEXT_CHA_NAME/OFFSET_TEXT_CHA_NAME/TEXT_CHA_NAME");
				if (transform2 != null)
				{
					m_SkillCutInTextUnitName = transform2.GetComponent<Text>();
				}
				transform2 = m_SkillCutInFRONT.m_EffectInstant.m_Instant.transform.Find("DESC/POS_TEXT_SPELL_NAME/OFFSET_TEXT_SPELL_NAME/TEXT_SPELL_NAME");
				if (transform2 != null)
				{
					m_SkillCutInTextSkillName = transform2.GetComponent<Text>();
				}
			}
		}
		cNKCEffectManager.StopCutInEffect();
		if (m_SkillCutInMID != null)
		{
			if (m_SkillCutInSpriteRenderer != null)
			{
				m_SkillCutInSpriteRenderer.sprite = faceSprite;
			}
			m_SkillCutInMID.ReStart();
		}
		if (m_SkillCutInFRONT != null)
		{
			NKCUtil.SetLabelText(m_SkillCutInTextUnitName, unitName);
			NKCUtil.SetLabelText(m_SkillCutInTextSkillName, skillName);
			m_SkillCutInFRONT.ReStart();
		}
	}

	public void Stop()
	{
		if (m_SkillCutInMID != null)
		{
			m_SkillCutInMID.Stop();
			m_SkillCutInFRONT?.Stop();
		}
	}
}
