using NKM.Unit;

namespace NKM;

public class NKMEventCameraMove : IEventConditionOwner, INKMUnitStateEvent
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public bool m_bAnimTime = true;

	public float m_fEventTimeMin;

	public float m_fEventTimeMax;

	public bool m_bForce;

	public float m_fPosXOffset;

	public float m_fPosYOffset;

	public float m_fZoom = -1f;

	public float m_fFocusBlur;

	public float m_fMoveTrackingTime;

	public float m_fZoomTrackingTime;

	public float m_fCameraRadius;

	public int m_Priority = -1;

	public NKMEventCondition Condition => m_Condition;

	public float EventStartTime => m_fEventTimeMin;

	public EventRollbackType RollbackType => EventRollbackType.Allowed;

	public bool bAnimTime => m_bAnimTime;

	public bool bStateEnd => false;

	public void DeepCopyFromSource(NKMEventCameraMove source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_bAnimTime = source.m_bAnimTime;
		m_fEventTimeMin = source.m_fEventTimeMin;
		m_fEventTimeMax = source.m_fEventTimeMax;
		m_bForce = source.m_bForce;
		m_fPosXOffset = source.m_fPosXOffset;
		m_fPosYOffset = source.m_fPosYOffset;
		m_fZoom = source.m_fZoom;
		m_fFocusBlur = source.m_fFocusBlur;
		m_fMoveTrackingTime = source.m_fMoveTrackingTime;
		m_fZoomTrackingTime = source.m_fZoomTrackingTime;
		m_fCameraRadius = source.m_fCameraRadius;
		m_Priority = source.m_Priority;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_bAnimTime", ref m_bAnimTime);
		cNKMLua.GetData("m_fEventTimeMin", ref m_fEventTimeMin);
		cNKMLua.GetData("m_fEventTimeMax", ref m_fEventTimeMax);
		cNKMLua.GetData("m_bForce", ref m_bForce);
		cNKMLua.GetData("m_fPosXOffset", ref m_fPosXOffset);
		cNKMLua.GetData("m_fPosYOffset", ref m_fPosYOffset);
		cNKMLua.GetData("m_fZoom", ref m_fZoom);
		cNKMLua.GetData("m_fFocusBlur", ref m_fFocusBlur);
		cNKMLua.GetData("m_fMoveTrackingTime", ref m_fMoveTrackingTime);
		cNKMLua.GetData("m_fZoomTrackingTime", ref m_fZoomTrackingTime);
		cNKMLua.GetData("m_fCameraRadius", ref m_fCameraRadius);
		cNKMLua.GetData("m_Priority", ref m_Priority);
		return true;
	}
}
