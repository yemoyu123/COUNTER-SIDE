namespace NKC;

public class NKCUnitTagData
{
	public readonly short TagType;

	public bool Voted;

	public bool IsTop;

	public int VoteCount;

	public NKCUnitTagData(short type, bool vote, int count, bool top)
	{
		TagType = type;
		Voted = vote;
		VoteCount = count;
		IsTop = top;
	}
}
