using ClientPacket.Community;
using ClientPacket.User;
using NKM;

namespace NKC.UI.Collection;

public abstract class NKCUICollectionGeneral : NKCUIBase
{
	public enum CollectionType
	{
		CT_NONE = -1,
		CT_TEAM_UP,
		CT_UNIT,
		CT_SHIP,
		CT_OPERATOR,
		CT_ILLUST,
		CT_STORY,
		CT_EMBLEM,
		CT_FRAME,
		CT_BACKGROUND
	}

	protected CollectionType GetCollectionTypeFromMiscItem(NKM_ITEM_MISC_TYPE miscType)
	{
		return miscType switch
		{
			NKM_ITEM_MISC_TYPE.IMT_SELFIE_FRAME => CollectionType.CT_FRAME, 
			NKM_ITEM_MISC_TYPE.IMT_BACKGROUND => CollectionType.CT_BACKGROUND, 
			NKM_ITEM_MISC_TYPE.IMT_EMBLEM => CollectionType.CT_EMBLEM, 
			_ => CollectionType.CT_NONE, 
		};
	}

	public abstract void Init();

	public abstract NKM_SHORTCUT_TYPE GetShortcutType();

	public abstract void Open(CollectionType reserveType = CollectionType.CT_NONE, string reserveUnitStrID = "");

	public abstract void OnRecvReviewTagVoteCancelAck(NKMPacket_UNIT_REVIEW_TAG_VOTE_CANCEL_ACK sPacket);

	public abstract void OnRecvReviewTagVoteAck(NKMPacket_UNIT_REVIEW_TAG_VOTE_ACK sPacket);

	public abstract void OnRecvReviewTagListAck(NKMPacket_UNIT_REVIEW_TAG_LIST_ACK sPacket);

	public abstract void OnRecvTeamCollectionRewardAck(NKMPacket_TEAM_COLLECTION_REWARD_ACK sPacket);

	public abstract void OnRecvUnitMissionReward(int unitId);
}
