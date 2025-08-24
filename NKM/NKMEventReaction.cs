using NKM.Templet.Base;

namespace NKM;

public class NKMEventReaction : NKMUnitStateAreaEventOneTime
{
	public string m_Reaction;

	public float m_fTime = -1f;

	public bool m_bRemove;

	public override EventHostType HostType => EventHostType.Server;

	public override EventRollbackType RollbackType => EventRollbackType.Prohibited;

	public void DeepCopyFromSource(NKMEventReaction source)
	{
		DeepCopy(source);
		m_Reaction = source.m_Reaction;
		m_fTime = source.m_fTime;
		m_bRemove = source.m_bRemove;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		bool result = base.LoadFromLUA(cNKMLua) & cNKMLua.GetData("m_Reaction", ref m_Reaction);
		cNKMLua.GetData("m_fTime", ref m_fTime);
		cNKMLua.GetData("m_bRemove", ref m_bRemove);
		return result;
	}

	public override void OnAreaEventToTarget(NKMGame cNKMGame, NKMUnit eventOwner, NKMUnit target)
	{
		int reactionID = eventOwner.GetUnitTemplet().GetReactionID(m_Reaction);
		if (m_bRemove)
		{
			target.RemoveReaction(eventOwner, reactionID);
		}
		else
		{
			target.RegisterReaction(eventOwner, reactionID, m_fTime);
		}
	}

	public bool Validate(NKMUnitTemplet templet)
	{
		if (templet.GetReaction(m_Reaction) == null || templet.GetReactionID(m_Reaction) < 0)
		{
			NKMTempletError.Add("[NKMEventReaction] Reaction " + m_Reaction + " not found from unit " + templet.m_UnitTempletBase.DebugName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStateEvent.cs", 4989);
			return false;
		}
		return true;
	}
}
