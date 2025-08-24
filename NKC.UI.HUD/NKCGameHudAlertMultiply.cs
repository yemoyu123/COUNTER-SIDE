using System;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.HUD;

public class NKCGameHudAlertMultiply : MonoBehaviour, IGameHudAlert
{
	private NKCUIGameHUDMultiplyReward m_NKCUIGameHUDMultiplyReward;

	private NKCAssetInstanceData m_NKCAssetInstanceDataMultiplyReward;

	public bool IsFinished()
	{
		throw new NotImplementedException();
	}

	public void OnCleanup()
	{
		if (m_NKCAssetInstanceDataMultiplyReward != null)
		{
			NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataMultiplyReward);
		}
		m_NKCAssetInstanceDataMultiplyReward = null;
		m_NKCUIGameHUDMultiplyReward = null;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void OnStart()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKCUIGameHUDMultiplyReward, bValue: true);
	}

	public void OnUpdate()
	{
	}

	public void SetMultiply(int count)
	{
		if (m_NKCUIGameHUDMultiplyReward != null)
		{
			m_NKCUIGameHUDMultiplyReward.SetMultiply(count);
		}
	}

	public void LoadMultiplyReward(NKMGameData cNKMGameData)
	{
		if (cNKMGameData != null && NKCContentManager.IsContentsUnlocked(ContentsType.OPERATION_MULTIPLY) && cNKMGameData.GetGameType() != NKM_GAME_TYPE.NGT_PRACTICE && m_NKCUIGameHUDMultiplyReward == null)
		{
			if (m_NKCAssetInstanceDataMultiplyReward != null)
			{
				NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceDataMultiplyReward);
			}
			m_NKCAssetInstanceDataMultiplyReward = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_HUD_RENEWAL", "AB_UI_GAME_HUD_MULTIPLY_REWARD");
			m_NKCUIGameHUDMultiplyReward = m_NKCAssetInstanceDataMultiplyReward.m_Instant.GetComponent<NKCUIGameHUDMultiplyReward>();
			m_NKCAssetInstanceDataMultiplyReward.m_Instant.transform.SetParent(base.transform, worldPositionStays: false);
			m_NKCAssetInstanceDataMultiplyReward.m_Instant.transform.localPosition = new Vector3(m_NKCAssetInstanceDataMultiplyReward.m_Instant.transform.localPosition.x, m_NKCAssetInstanceDataMultiplyReward.m_Instant.transform.localPosition.y, 0f);
			NKCUtil.SetGameobjectActive(m_NKCUIGameHUDMultiplyReward, bValue: false);
		}
	}
}
