using NKM;
using UnityEngine;

namespace NKC.FX;

public class NKC_FX_UNIT_FOLLOWER : MonoBehaviour, IFxProperty
{
	public enum TargetUnitType
	{
		MasterUnit,
		TargetUnit
	}

	public TargetUnitType m_eTarget;

	public bool m_bFirstFrameOnly;

	public string BoneName;

	public bool m_bFollowScriptOffset;

	public Vector3 m_vLocalOffset = Vector3.zero;

	private NKCUnitClient m_targetUnit;

	public void SetFxProperty(NKCUnitClient masterUnit, NKCUnitClient targetUnit, NKMDamageEffectData DEData)
	{
		switch (m_eTarget)
		{
		case TargetUnitType.MasterUnit:
			m_targetUnit = masterUnit;
			break;
		case TargetUnitType.TargetUnit:
			m_targetUnit = targetUnit;
			break;
		}
		if (m_bFollowScriptOffset && DEData != null)
		{
			m_vLocalOffset = new Vector3(DEData.m_fOffsetX, DEData.m_fOffsetY, DEData.m_fOffsetZ);
		}
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		if (m_targetUnit != null && m_targetUnit.IsDyingOrDie())
		{
			m_targetUnit = null;
		}
		if (m_targetUnit == null)
		{
			return;
		}
		Vector3 worldPos = Vector3.zero;
		if (string.IsNullOrEmpty(BoneName))
		{
			if (m_targetUnit.GetNKCASUnitSpineSprite().m_SPINE_SkeletonAnimation != null)
			{
				worldPos = m_targetUnit.GetNKCASUnitSpineSprite().m_SPINE_SkeletonAnimation.transform.position;
			}
		}
		else if (!m_targetUnit.GetNKCASUnitSpineSprite().GetBoneWorldPos(BoneName, ref worldPos))
		{
			worldPos = m_targetUnit.GetNKCASUnitSpineSprite().m_SPINE_SkeletonAnimation.transform.position;
		}
		base.transform.position = worldPos;
		base.transform.localPosition = base.transform.localPosition + m_vLocalOffset;
	}

	private void Update()
	{
		if (!m_bFirstFrameOnly)
		{
			UpdatePosition();
		}
	}
}
