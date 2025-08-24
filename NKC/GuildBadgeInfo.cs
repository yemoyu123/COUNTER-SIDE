using System.Text;

namespace NKC;

public class GuildBadgeInfo
{
	public long BadgeId;

	public int FrameId = 1;

	public int FrameColorId = 1;

	public int MarkId = 1;

	public int MarkColorId = 1;

	private GuildBadgeInfo()
	{
	}

	public GuildBadgeInfo(long badgeId)
	{
		string text = badgeId.ToString();
		if (text.Length > 9 && text.Length <= 12)
		{
			BadgeId = badgeId;
			string s = text.Substring(0, text.Length - 9);
			FrameId = int.Parse(s);
			string s2 = text.Substring(text.Length - 9, 3);
			FrameColorId = int.Parse(s2);
			string s3 = text.Substring(text.Length - 6, 3);
			MarkId = int.Parse(s3);
			string s4 = text.Substring(text.Length - 3, 3);
			MarkColorId = int.Parse(s4);
		}
		else
		{
			FrameId = 1;
			FrameColorId = 1;
			MarkId = 1;
			MarkColorId = 1;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(FrameId.ToString("D3"));
			stringBuilder.Append(FrameColorId.ToString("D3"));
			stringBuilder.Append(MarkId.ToString("D3"));
			stringBuilder.Append(MarkColorId.ToString("D3"));
			BadgeId = long.Parse(stringBuilder.ToString());
			stringBuilder.Clear();
		}
	}

	public GuildBadgeInfo(int frameId, int frameColorId, int markId, int markColorId)
	{
		FrameId = frameId;
		FrameColorId = frameColorId;
		MarkId = markId;
		MarkColorId = markColorId;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(frameId.ToString("D3"));
		stringBuilder.Append(frameColorId.ToString("D3"));
		stringBuilder.Append(markId.ToString("D3"));
		stringBuilder.Append(markColorId.ToString("D3"));
		BadgeId = long.Parse(stringBuilder.ToString());
		stringBuilder.Clear();
	}
}
