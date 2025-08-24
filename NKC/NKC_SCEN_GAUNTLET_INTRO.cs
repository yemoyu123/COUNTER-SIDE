using NKC.UI.Gauntlet;
using NKM;
using UnityEngine.Video;

namespace NKC;

public class NKC_SCEN_GAUNTLET_INTRO : NKC_SCEN_BASIC
{
	private NKCAssetResourceData m_UILoadResourceData;

	private NKCUIGauntletIntro m_NKCUIGauntletIntro;

	public NKC_SCEN_GAUNTLET_INTRO()
	{
		m_NKM_SCEN_ID = NKM_SCEN_ID.NSI_GAUNTLET_INTRO;
	}

	public void ClearCacheData()
	{
		if (m_NKCUIGauntletIntro != null)
		{
			m_NKCUIGauntletIntro.CloseInstance();
			m_NKCUIGauntletIntro = null;
		}
	}

	public override void ScenLoadUIStart()
	{
		base.ScenLoadUIStart();
		if (m_NKCUIGauntletIntro == null)
		{
			m_UILoadResourceData = NKCUIGauntletIntro.OpenInstanceAsync();
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
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null && subUICameraVideoPlayer.IsPreparing())
		{
			return;
		}
		if (m_NKCUIGauntletIntro == null && m_UILoadResourceData != null)
		{
			if (!NKCUIGauntletIntro.CheckInstanceLoaded(m_UILoadResourceData, out m_NKCUIGauntletIntro))
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
		if (m_NKCUIGauntletIntro != null)
		{
			m_NKCUIGauntletIntro.InitUI();
		}
		SetBG();
	}

	private void SetBG()
	{
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
			subUICameraVideoPlayer.m_fMoviePlaySpeed = 1f;
			subUICameraVideoPlayer.Play("Gauntlet_BG.mp4", bLoop: true);
		}
	}

	public override void ScenStart()
	{
		base.ScenStart();
		NKCCamera.EnableBloom(bEnable: false);
		if (m_NKCUIGauntletIntro != null)
		{
			m_NKCUIGauntletIntro.Open();
			NKCContentManager.SetUnlockedContent();
			NKCContentManager.ShowContentUnlockPopup(null);
		}
	}

	public override void ScenEnd()
	{
		base.ScenEnd();
		if (m_NKCUIGauntletIntro != null)
		{
			m_NKCUIGauntletIntro.Close();
		}
	}

	public override void ScenUpdate()
	{
		base.ScenUpdate();
	}

	public override bool ScenMsgProc(NKCMessageData cNKCMessageData)
	{
		return false;
	}
}
