using Cs.Logging;
using NKC.Templet;
using NKM;
using UnityEngine;

namespace NKC.UI.Gauntlet;

public class NKCUIGauntletKeywordGroup : MonoBehaviour
{
	public NKCUIGauntletKeyword[] m_keywordSlot;

	public void Init()
	{
		if (m_keywordSlot != null)
		{
			for (int i = 0; i < m_keywordSlot.Length; i++)
			{
				m_keywordSlot[i]?.Init();
			}
		}
	}

	public void SetData(NKM_GAME_TYPE gameType)
	{
		if (m_keywordSlot == null)
		{
			return;
		}
		NKCGauntletKeywordSlotTemplet nKCGauntletKeywordSlotTemplet = NKCGauntletKeywordSlotTemplet.Find(gameType);
		if (nKCGauntletKeywordSlotTemplet == null)
		{
			for (int i = 0; i < m_keywordSlot.Length; i++)
			{
				m_keywordSlot[i]?.SetData(0);
			}
			Log.Error($"NKCGauntletKeywordSlotTemplet of NKM_GAME_TYPE {gameType} is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Gauntlet/NKCUIGauntletKeywordGroup.cs", 34);
			return;
		}
		for (int j = 0; j < m_keywordSlot.Length; j++)
		{
			if (j >= nKCGauntletKeywordSlotTemplet.SlotKeywordId.Length)
			{
				m_keywordSlot[j]?.SetData(0);
				continue;
			}
			int data = nKCGauntletKeywordSlotTemplet.SlotKeywordId[j];
			m_keywordSlot[j].SetData(data);
		}
	}
}
