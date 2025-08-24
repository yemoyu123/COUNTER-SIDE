namespace NKC.UI;

public interface IDungeonSlot
{
	public delegate void OnSelectedItemSlot(int dunIndex, string dunStrID, bool isPlaying);

	void SetSelectNode(bool bValue);
}
