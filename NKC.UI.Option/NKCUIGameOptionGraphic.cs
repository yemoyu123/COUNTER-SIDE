using NKM;
using UnityEngine;

namespace NKC.UI.Option;

public class NKCUIGameOptionGraphic : NKCUIGameOptionContentBase
{
	private string[] GRAPHIC_QUALITY_STRINGS;

	private string[] GRAPHIC_ANIMATION_QUALITY_STRINGS;

	private string[] GRAPHIC_GAME_FRAME_LIMIT_STRINGS;

	private string[] GRAPHIC_QUALITY_LEVEL_STRINGS;

	private string[] GRAPHIC_LOGIN_CUTIN_STRINGS;

	public NKCUIGameOptionSliderWithButton m_GraphicSlot1SliderWithButton;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT2_TOGGLE;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT3_TOGGLE;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_GRAPHIC_HIT_EFFECT_TOGGLE;

	public NKCUIGameOptionSliderWithButton m_sldrEffectOpacity;

	public NKCUIGameOptionSliderWithButton m_sldrEffectEnemyOpacity;

	public NKCUIGameOptionMultiStateButton m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT4_BUTTON;

	public NKCUIGameOptionMultiStateButton m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT5_BUTTON;

	public NKCUIGameOptionMultiStateButton m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT6_BUTTON;

	public NKCUIComToggle m_tglUseBuffEffect;

	public NKCUIGameOptionMultiStateButton m_btnLoginCutin;

	public NKCUIComToggle m_NKM_UI_GAME_OPTION_GRAPHIC_VIDEO_TEXTURE;

	private string GRAPHIC_DISABLED_QUALITY_STRING => NKCUtilString.GET_STRING_CUSTOM;

	private string HIGH_GRAPHIC_OPTION_CHANGE_TITLE_STRING => NKCUtilString.GET_STRING_WARNING;

	private string HIGH_GRAPHIC_OPTION_CHANGE_CONTENT_STRING => NKCUtilString.GET_STRING_OPTION_CHANGE_WARNING;

	public override void Init()
	{
		GRAPHIC_QUALITY_STRINGS = new string[5]
		{
			NKCUtilString.GET_STRING_WORST,
			NKCUtilString.GET_STRING_LOW,
			NKCUtilString.GET_STRING_NORMAL,
			NKCUtilString.GET_STRING_GOOD,
			NKCUtilString.GET_STRING_BEST
		};
		GRAPHIC_ANIMATION_QUALITY_STRINGS = new string[2]
		{
			NKCUtilString.GET_STRING_NORMAL,
			NKCUtilString.GET_STRING_OPTION_HIGH_QUALITY
		};
		GRAPHIC_GAME_FRAME_LIMIT_STRINGS = new string[2]
		{
			NKCUtilString.GET_STRING_OPTION_30_FPS,
			NKCUtilString.GET_STRING_OPTION_60_FPS
		};
		GRAPHIC_QUALITY_LEVEL_STRINGS = new string[2]
		{
			NKCUtilString.GET_STRING_LOW2,
			NKCUtilString.GET_STRING_HIGH
		};
		GRAPHIC_LOGIN_CUTIN_STRINGS = new string[4]
		{
			NKCStringTable.GetString("SI_DP_OPTION_GRAPHIC_LOGIN_ANIM_ALWAYS"),
			NKCStringTable.GetString("SI_DP_OPTION_GRAPHIC_LOGIN_ANIM_RANDOM"),
			NKCStringTable.GetString("SI_DP_OPTION_GRAPHIC_LOGIN_ANIM_DAYONE"),
			NKCStringTable.GetString("SI_DP_OPTION_GRAPHIC_LOGIN_ANIM_OFF")
		};
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			if (gameOptionData.DisabledGraphicQuality)
			{
				m_GraphicSlot1SliderWithButton.SetDisabled(disabled: true, GRAPHIC_DISABLED_QUALITY_STRING);
			}
			m_GraphicSlot1SliderWithButton.Init(0, 4, (int)gameOptionData.GraphicQuality, GRAPHIC_QUALITY_STRINGS, ChangeGraphicQuality);
			m_GraphicSlot1SliderWithButton.SetWarningPopup(4, HIGH_GRAPHIC_OPTION_CHANGE_TITLE_STRING, HIGH_GRAPHIC_OPTION_CHANGE_CONTENT_STRING);
			m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT4_BUTTON.Init(0, 1, (int)gameOptionData.AnimationQuality, GRAPHIC_ANIMATION_QUALITY_STRINGS, OnClickAnimationQualityButton);
			m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT5_BUTTON.Init(0, 1, (int)gameOptionData.GameFrameLimit, GRAPHIC_GAME_FRAME_LIMIT_STRINGS, OnClickGameFrameLimitButton);
			m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT6_BUTTON.Init(0, 1, (int)gameOptionData.QualityLevel, GRAPHIC_QUALITY_LEVEL_STRINGS, OnClickQualityLevelButton);
			if (m_btnLoginCutin != null)
			{
				m_btnLoginCutin.Init(0, 3, (int)gameOptionData.LoginCutin, GRAPHIC_LOGIN_CUTIN_STRINGS, OnClickLoginCutinButton);
			}
			m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT2_TOGGLE.OnValueChanged.AddListener(OnClickUseGameEffectButton);
			m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT3_TOGGLE.OnValueChanged.AddListener(OnClickUseCommonEffectButton);
			m_NKM_UI_GAME_OPTION_GRAPHIC_HIT_EFFECT_TOGGLE.OnValueChanged.AddListener(OnClickUseHitEffectButton);
			m_NKM_UI_GAME_OPTION_GRAPHIC_VIDEO_TEXTURE.OnValueChanged.AddListener(OnClickUseVideoTexture);
			NKCUtil.SetToggleValueChangedDelegate(m_tglUseBuffEffect, OnClickUseBuffEffect);
			if (m_sldrEffectOpacity != null)
			{
				NKCUtil.SetGameobjectActive(m_sldrEffectOpacity.transform.parent, bValue: true);
				m_sldrEffectOpacity.Init(35, 100, gameOptionData.EffectOpacity, null, OnChangeGameOpacity);
			}
			if (m_sldrEffectEnemyOpacity != null)
			{
				NKCUtil.SetGameobjectActive(m_sldrEffectEnemyOpacity.transform.parent, bValue: true);
				m_sldrEffectEnemyOpacity.Init(35, 100, gameOptionData.EffectEnemyOpacity, null, OnChangeGameEnemyOpacity);
			}
		}
	}

	public override void SetContent()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			if (gameOptionData.DisabledGraphicQuality)
			{
				m_GraphicSlot1SliderWithButton.SetDisabled(disabled: true, GRAPHIC_DISABLED_QUALITY_STRING);
				ApplyGameOptionData(gameOptionData);
				return;
			}
			m_GraphicSlot1SliderWithButton.SetDisabled(disabled: false);
			m_GraphicSlot1SliderWithButton.m_Slider.value = (float)gameOptionData.GraphicQuality;
			m_GraphicSlot1SliderWithButton.UpdateButtonText();
			ApplyGraphicQualityDetail(gameOptionData.GraphicQuality);
		}
	}

	private void ChangeGraphicQuality()
	{
		OnClickChangeGraphicQualityConfirmButton();
	}

	private void OnClickChangeGraphicQualityConfirmButton()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			NKC_GAME_OPTION_GRAPHIC_QUALITY value = (NKC_GAME_OPTION_GRAPHIC_QUALITY)m_GraphicSlot1SliderWithButton.GetValue();
			gameOptionData.DisabledGraphicQuality = m_GraphicSlot1SliderWithButton.isDisabled();
			gameOptionData.GraphicQuality = value;
			ApplyGraphicQualityDetail(value);
		}
	}

	private void ApplyGraphicQualityDetail(NKC_GAME_OPTION_GRAPHIC_QUALITY graphicQuality)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null && !gameOptionData.DisabledGraphicQuality)
		{
			gameOptionData.SetGameOptionDataByGrahpicQuality(graphicQuality);
			ApplyGameOptionData(gameOptionData);
		}
	}

	private void ApplyGameOptionData(NKCGameOptionData gameOptionData)
	{
		m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT2_TOGGLE.Select(gameOptionData.UseGameEffect, bForce: true);
		m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT3_TOGGLE.Select(gameOptionData.UseCommonEffect, bForce: true);
		m_NKM_UI_GAME_OPTION_GRAPHIC_HIT_EFFECT_TOGGLE.Select(gameOptionData.UseHitEffect, bForce: true);
		m_NKM_UI_GAME_OPTION_GRAPHIC_VIDEO_TEXTURE.Select(gameOptionData.UseVideoTexture, bForce: true);
		m_tglUseBuffEffect?.Select(gameOptionData.UseBuffEffect, bForce: true);
		m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT4_BUTTON.ChangeValue((int)gameOptionData.AnimationQuality);
		m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT5_BUTTON.ChangeValue((int)gameOptionData.GameFrameLimit);
		m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT6_BUTTON.ChangeValue((int)gameOptionData.QualityLevel);
		m_btnLoginCutin.ChangeValue((int)gameOptionData.LoginCutin);
		m_sldrEffectOpacity.ChangeValue(gameOptionData.EffectOpacity);
		m_sldrEffectEnemyOpacity.ChangeValue(gameOptionData.EffectEnemyOpacity);
		ChangeUseGameEffect(gameOptionData.UseGameEffect);
		ChangeUseCommonEffect(gameOptionData.UseCommonEffect);
		ChangeUseHitEffect(gameOptionData.UseHitEffect);
		ChangeAnimationQuality();
		ChangeGameFrameLimit();
		ChangeQualityLevel();
		ChangeUseVideoTexture(gameOptionData.UseVideoTexture);
		OnClickLoginCutinButton();
	}

	private void ChangeUseGameEffect(bool use)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.UseGameEffect = m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT2_TOGGLE.m_bChecked;
		}
	}

	private void ChangeUseVideoTexture(bool use)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.UseVideoTexture = m_NKM_UI_GAME_OPTION_GRAPHIC_VIDEO_TEXTURE.m_bChecked;
		}
	}

	private void ChangeUseCommonEffect(bool use)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.UseCommonEffect = m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT3_TOGGLE.m_bChecked;
			NKCCamera.SetBloomEnableUI(gameOptionData.UseCommonEffect);
		}
	}

	private void OnClickUseBuffEffect(bool value)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.UseBuffEffect = value;
			ChangeCustomGraphicQuality();
		}
	}

	private void OnChangeGameOpacity()
	{
		int value = m_sldrEffectOpacity.GetValue();
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.EffectOpacity = value;
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
			{
				float num = (float)value / 100f;
				Shader.SetGlobalFloat("_FxGlobalTransparency", 1f - num * num);
			}
		}
	}

	private void OnChangeGameEnemyOpacity()
	{
		int value = m_sldrEffectEnemyOpacity.GetValue();
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.EffectEnemyOpacity = value;
			if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME)
			{
				float num = (float)value / 100f;
				Shader.SetGlobalFloat("_FxGlobalTransparencyEnemy", 1f - num * num);
			}
		}
	}

	private void ChangeAnimationQuality()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			NKCGameOptionDataSt.GraphicOptionAnimationQuality value = (NKCGameOptionDataSt.GraphicOptionAnimationQuality)m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT4_BUTTON.GetValue();
			gameOptionData.AnimationQuality = value;
		}
	}

	private void ChangeGameFrameLimit()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			NKCGameOptionDataSt.GraphicOptionGameFrameLimit value = (NKCGameOptionDataSt.GraphicOptionGameFrameLimit)m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT5_BUTTON.GetValue();
			gameOptionData.GameFrameLimit = value;
			Application.targetFrameRate = gameOptionData.GetFrameLimit();
		}
	}

	private void ChangeQualityLevel()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			int num = (int)(gameOptionData.QualityLevel = (NKCGameOptionDataSt.GraphicOptionQualityLevel)m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT6_BUTTON.GetValue());
			if (QualitySettings.GetQualityLevel() != num)
			{
				QualitySettings.SetQualityLevel(num, applyExpensiveChanges: true);
			}
		}
	}

	private void OnClickLoginCutinButton()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.LoginCutin = (NKCGameOptionDataSt.GraphicOptionLoginCutin)m_btnLoginCutin.GetValue();
		}
	}

	private void ChangeUseHitEffect(bool use)
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData != null)
		{
			gameOptionData.UseHitEffect = m_NKM_UI_GAME_OPTION_GRAPHIC_HIT_EFFECT_TOGGLE.m_bChecked;
		}
	}

	private void OnClickUseGameEffectButton(bool use)
	{
		if (use)
		{
			NKCPopupOKCancel.OpenOKCancelBox(HIGH_GRAPHIC_OPTION_CHANGE_TITLE_STRING, HIGH_GRAPHIC_OPTION_CHANGE_CONTENT_STRING, delegate
			{
				OnClickUseGameEffectConfirmButton(use);
			}, delegate
			{
				SetContent();
			});
		}
		else
		{
			OnClickUseGameEffectConfirmButton(use);
		}
	}

	private void OnClickUseVideoTexture(bool use)
	{
		ChangeUseVideoTexture(use);
		ChangeCustomGraphicQuality();
	}

	private void OnClickUseGameEffectConfirmButton(bool use)
	{
		ChangeUseGameEffect(use);
		ChangeCustomGraphicQuality();
	}

	private void OnClickUseCommonEffectButton(bool use)
	{
		ChangeUseCommonEffect(use);
		ChangeCustomGraphicQuality();
	}

	private void OnClickUseHitEffectButton(bool use)
	{
		ChangeUseHitEffect(use);
		ChangeCustomGraphicQuality();
	}

	private void OnClickAnimationQualityButton()
	{
		if (m_NKM_UI_GAME_OPTION_GRAPHIC_SLOT4_BUTTON.GetValue() == 1)
		{
			NKCPopupOKCancel.OpenOKCancelBox(HIGH_GRAPHIC_OPTION_CHANGE_TITLE_STRING, HIGH_GRAPHIC_OPTION_CHANGE_CONTENT_STRING, delegate
			{
				OnClickAnimationQualityConfirmButton();
			}, delegate
			{
				SetContent();
			});
		}
		else
		{
			OnClickAnimationQualityConfirmButton();
		}
	}

	private void OnClickAnimationQualityConfirmButton()
	{
		ChangeAnimationQuality();
		ChangeCustomGraphicQuality();
	}

	private void OnClickGameFrameLimitButton()
	{
		ChangeGameFrameLimit();
		ChangeCustomGraphicQuality();
	}

	private void OnClickQualityLevelButton()
	{
		ChangeQualityLevel();
		ChangeCustomGraphicQuality();
	}

	private void ChangeCustomGraphicQuality()
	{
		NKCGameOptionData gameOptionData = NKCScenManager.GetScenManager().GetGameOptionData();
		if (gameOptionData == null)
		{
			return;
		}
		NKCGameOptionData nKCGameOptionData = new NKCGameOptionData();
		int num = -1;
		for (int i = 0; i < 5; i++)
		{
			nKCGameOptionData.SetGameOptionDataByGrahpicQuality((NKC_GAME_OPTION_GRAPHIC_QUALITY)i);
			if (CheckGameOptionData(gameOptionData, nKCGameOptionData))
			{
				num = i;
				break;
			}
		}
		if (num == -1)
		{
			gameOptionData.DisabledGraphicQuality = true;
			m_GraphicSlot1SliderWithButton.SetDisabled(disabled: true, GRAPHIC_DISABLED_QUALITY_STRING);
			return;
		}
		gameOptionData.DisabledGraphicQuality = false;
		gameOptionData.GraphicQuality = (NKC_GAME_OPTION_GRAPHIC_QUALITY)num;
		m_GraphicSlot1SliderWithButton.SetDisabled(disabled: false);
		m_GraphicSlot1SliderWithButton.ChangeValue((int)gameOptionData.GraphicQuality);
	}

	private bool CheckGameOptionData(NKCGameOptionData currentData, NKCGameOptionData compareData)
	{
		if (currentData == null || compareData == null)
		{
			return false;
		}
		if (currentData.UseGameEffect != compareData.UseGameEffect)
		{
			return false;
		}
		if (currentData.UseCommonEffect != compareData.UseCommonEffect)
		{
			return false;
		}
		if (currentData.UseHitEffect != compareData.UseHitEffect)
		{
			return false;
		}
		if (currentData.AnimationQuality != compareData.AnimationQuality)
		{
			return false;
		}
		if (currentData.GameFrameLimit != compareData.GameFrameLimit)
		{
			return false;
		}
		if (currentData.QualityLevel != compareData.QualityLevel)
		{
			return false;
		}
		if (currentData.UseVideoTexture != compareData.UseVideoTexture)
		{
			return false;
		}
		if (currentData.UseBuffEffect != compareData.UseBuffEffect)
		{
			return false;
		}
		return true;
	}
}
