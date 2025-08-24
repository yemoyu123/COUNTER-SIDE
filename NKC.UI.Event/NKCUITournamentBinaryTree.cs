using System.Collections.Generic;

namespace NKC.UI.Event;

public class NKCUITournamentBinaryTree
{
	public const int SlotIndexOffset = 0;

	private List<NKCUITournamentPlayerSlotTree> tournamentTree = new List<NKCUITournamentPlayerSlotTree>();

	private List<NKCUITournamentPlayerSlot> slotTree = new List<NKCUITournamentPlayerSlot>();

	public List<NKCUITournamentPlayerSlot> PlayerSlotTree => slotTree;

	public List<NKCUITournamentPlayerSlotTree> TournamentTree => tournamentTree;

	public void AddNode(NKCUITournamentPlayerSlotTree node)
	{
		if (node == null)
		{
			return;
		}
		for (int i = 0; i < tournamentTree.Count; i++)
		{
			if (tournamentTree[i].TreeUnderA == null)
			{
				tournamentTree[i].TreeUnderA = node;
				node.TreeUpper = tournamentTree[i];
				tournamentTree.Add(node);
				return;
			}
			if (tournamentTree[i].TreeUnderB == null)
			{
				tournamentTree[i].TreeUnderB = node;
				node.TreeUpper = tournamentTree[i];
				tournamentTree.Add(node);
				return;
			}
		}
		tournamentTree.Add(node);
	}

	public void InsertToTournamentTree(int index, NKCUITournamentPlayerSlotTree slotTree)
	{
		if (index < 0)
		{
			tournamentTree.Add(slotTree);
		}
		else
		{
			tournamentTree.Insert(index, slotTree);
		}
	}

	public void CreateSlotTree(NKCUITournamentPlayerSlot rootSlot)
	{
		slotTree.Clear();
		slotTree.Add(rootSlot);
		tournamentTree.ForEach(delegate(NKCUITournamentPlayerSlotTree e)
		{
			slotTree.Add(e.m_playerA);
			slotTree.Add(e.m_playerB);
		});
		if (rootSlot != null && slotTree.Count >= 3)
		{
			slotTree[0].SetPlayerSlotUnder(slotTree[1], slotTree[2]);
		}
	}

	public NKCUITournamentPlayerSlotTree GetSlotTree(int index)
	{
		int num = (index - 1) / 2;
		if (num < 0 || num >= tournamentTree.Count)
		{
			return null;
		}
		return tournamentTree[num];
	}

	public NKCUITournamentPlayerSlot GetPlayerSlot(int index)
	{
		index = index;
		if (index < 0 || index >= slotTree.Count)
		{
			return null;
		}
		return slotTree[index];
	}

	public void SetCheerEnable(bool value)
	{
		tournamentTree.ForEach(delegate(NKCUITournamentPlayerSlotTree e)
		{
			e.SetCheerEnable(value);
		});
	}

	public void SetSlotLink()
	{
		tournamentTree.ForEach(delegate(NKCUITournamentPlayerSlotTree e)
		{
			e.SetSlotLink();
		});
	}

	public void SetDetailButtonActive()
	{
		tournamentTree.ForEach(delegate(NKCUITournamentPlayerSlotTree e)
		{
			e.SetDetailButtonActive();
		});
	}

	public bool IsCheeringCompleted(out List<int> notCheeredSlotIndexList, out int _cheerSlotCount, out int _notCheeringCount)
	{
		notCheeredSlotIndexList = new List<int>();
		int num = 0;
		int num2 = 0;
		int num3 = 1;
		bool result = true;
		foreach (NKCUITournamentPlayerSlotTree item in tournamentTree)
		{
			if (!item.IsBlankTree())
			{
				num++;
				if (!item.IsCheering())
				{
					result = false;
					notCheeredSlotIndexList.Add(num3);
					notCheeredSlotIndexList.Add(num3 + 1);
					num2++;
				}
			}
			num3 += 2;
		}
		_cheerSlotCount = num;
		_notCheeringCount = num2;
		return result;
	}

	public List<long> GetCheeringUIdList()
	{
		List<long> list = new List<long>();
		int count = slotTree.Count;
		for (int i = 0; i < count; i++)
		{
			if (slotTree[i] == null)
			{
				list.Add(0L);
				continue;
			}
			long num = 0L;
			if (slotTree[i].ProfileData != null)
			{
				num = slotTree[i].ProfileData.commonProfile.userUid;
			}
			else if (slotTree[i].ProfileDataPredict != null)
			{
				num = slotTree[i].ProfileDataPredict.commonProfile.userUid;
			}
			if (num == 0L && slotTree[i].IsBlankSlot())
			{
				num = -1L;
			}
			list.Add(num);
		}
		return list;
	}

	public void ReleaseData()
	{
		tournamentTree.ForEach(delegate(NKCUITournamentPlayerSlotTree e)
		{
			e.ReleaseData();
		});
	}

	public void ClearTree()
	{
		tournamentTree.Clear();
		slotTree.Clear();
	}
}
