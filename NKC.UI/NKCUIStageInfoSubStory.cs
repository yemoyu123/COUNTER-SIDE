using System;
using NKC.UI.NPC;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCUIStageInfoSubStory : NKCUIStageInfoSubBase
{
	[Header("스토리")]
	[Header("상단")]
	public NKCUINPCSpineIllust m_NKCUINPCSpineIllust;

	public Vector2 DEFAULT_CHAR_POS = new Vector2(-9.97f, -190f);

	[Space]
	[Header("하단")]
	public NKCUIComStateButton m_btnReady;

	private const string DEFAULT_CHAR_STR = "NKM_NPC_OPERATOR_LENA";

	private NKCASUISpineIllust m_unitIllust;

	private float m_UpdateTimer;

	public override void InitUI(OnButton onButton)
	{
		base.InitUI(onButton);
		m_btnReady.PointerClick.RemoveAllListeners();
		m_btnReady.PointerClick.AddListener(base.OnOK);
	}

	public void Close()
	{
		if (m_unitIllust != null)
		{
			m_unitIllust.Unload();
			m_unitIllust = null;
		}
	}

	public override void SetData(NKMStageTempletV2 stageTemplet, bool bFirstOpen = true)
	{
		base.SetData(stageTemplet, bFirstOpen);
		if (m_unitIllust != null)
		{
			m_unitIllust.Unload();
			m_unitIllust = null;
		}
		if (string.IsNullOrEmpty(stageTemplet.m_StageCharStr))
		{
			m_unitIllust = AddSpineIllustration("NKM_NPC_OPERATOR_LENA");
		}
		else
		{
			m_unitIllust = AddSpineIllustration(stageTemplet.m_StageCharStr);
		}
		if (m_unitIllust == null || !(m_NKCUINPCSpineIllust != null))
		{
			return;
		}
		m_NKCUINPCSpineIllust.m_spUnitIllust = m_unitIllust.m_SpineIllustInstant_SkeletonGraphic;
		m_unitIllust.SetParent(m_NKCUINPCSpineIllust.transform, worldPositionStays: false);
		m_unitIllust.SetAnchoredPosition(DEFAULT_CHAR_POS);
		NKCASUIUnitIllust.eAnimation result;
		if (string.IsNullOrEmpty(stageTemplet.m_StageCharStrFace))
		{
			if (m_unitIllust.HasAnimation(NKCASUIUnitIllust.eAnimation.UNIT_IDLE))
			{
				m_unitIllust.SetAnimation(NKCASUIUnitIllust.eAnimation.UNIT_IDLE, loop: true);
			}
		}
		else if (Enum.TryParse<NKCASUIUnitIllust.eAnimation>(stageTemplet.m_StageCharStrFace, out result) && m_unitIllust.HasAnimation(result))
		{
			m_unitIllust.SetAnimation(result, loop: true);
		}
		m_UpdateTimer = 0.1f;
	}

	public override void Update()
	{
		base.Update();
		m_UpdateTimer -= Time.deltaTime;
		if (m_UpdateTimer < 0f)
		{
			m_UpdateTimer = 5f;
			m_unitIllust.InvalidateWorldRect();
			m_unitIllust.GetWorldRect();
		}
	}

	private NKCASUISpineIllust AddSpineIllustration(string prefabStrID)
	{
		return (NKCASUISpineIllust)NKCResourceUtility.OpenSpineIllustWithManualNaming(prefabStrID);
	}
}
