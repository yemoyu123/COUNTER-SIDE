using NKM;

namespace NKC.UI.HUD;

public class NKCGameHudAlertEffect : IGameHudAlert
{
	private NKCASEffect m_targetEffect;

	private NKM_EFFECT_PARENT_TYPE m_effectParentType;

	private string m_bundleName;

	private string m_assetName;

	private string m_animName;

	public NKCGameHudAlertEffect(string bundleName, string assetName, NKM_EFFECT_PARENT_TYPE effectParent = NKM_EFFECT_PARENT_TYPE.NEPT_NUF_BEFORE_HUD_EFFECT, string animName = "")
	{
		m_effectParentType = effectParent;
		m_bundleName = bundleName;
		m_assetName = assetName;
		m_animName = animName;
	}

	public void OnStart()
	{
		NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
		if (gameClient != null)
		{
			m_targetEffect = gameClient.GetNKCEffectManager().UseEffect(0, m_bundleName, m_assetName, m_effectParentType, 0f, 0f, 0f, bRight: true, 1f, 0f, 0f, 0f, m_bUseZtoY: false, 0f, bUseZScale: false, "", bUseBoneRotate: false, bAutoDie: true, m_animName);
		}
	}

	public bool IsFinished()
	{
		if (m_targetEffect == null)
		{
			return true;
		}
		if (!NKCScenManager.GetScenManager().GetGameClient().GetNKCEffectManager()
			.IsLiveEffect(m_targetEffect.m_EffectUID))
		{
			return true;
		}
		if (m_targetEffect.m_EffectInstant == null || m_targetEffect.m_EffectInstant.m_Instant == null)
		{
			return true;
		}
		if (!m_targetEffect.m_EffectInstant.m_Instant.activeSelf)
		{
			return true;
		}
		if (m_targetEffect.IsEnd())
		{
			return true;
		}
		return false;
	}

	public void OnCleanup()
	{
		NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
		if (gameClient != null)
		{
			gameClient.GetNKCEffectManager().DeleteEffect(m_targetEffect);
			m_targetEffect = null;
		}
	}

	public void OnUpdate()
	{
	}
}
