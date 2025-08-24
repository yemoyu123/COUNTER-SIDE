using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCDeckViewList : MonoBehaviour
{
	public delegate void OnSelectDeck(NKMDeckIndex index);

	public delegate void OnChangedMuiltiSelectedDeckCount(int selectedCount);

	public delegate void OnSupportList();

	private bool m_bOpen;

	public RectTransform m_rtDeckViewList;

	public ScrollRect m_scrollRect;

	public NKCUIComToggleGroup m_ToggleGroup;

	public List<NKCDeckListButton> m_listDeckListButton;

	public NKCSimpleButton m_supportListButton;

	private int m_iUnlockedDeckCount;

	private OnSelectDeck dOnSelectDeck;

	private OnSelectDeck dOnDeckUnloakRequest;

	private OnChangedMuiltiSelectedDeckCount dOnChangedMuiltiSelectedDeckCount;

	private OnSupportList dOnSupportList;

	private NKM_DECK_TYPE m_eCurrentDeckType;

	private List<int> m_lstMultiSelectedIndex = new List<int>();

	public void Init(OnSelectDeck onSelectDeck, OnSelectDeck onDeckUnlockRequest, OnChangedMuiltiSelectedDeckCount onChangedMuiltiSelectedDeckCount, OnSupportList onSupportList)
	{
		dOnSelectDeck = onSelectDeck;
		dOnDeckUnloakRequest = onDeckUnlockRequest;
		dOnChangedMuiltiSelectedDeckCount = onChangedMuiltiSelectedDeckCount;
		dOnSupportList = onSupportList;
		for (byte b = 0; b < m_listDeckListButton.Count; b++)
		{
			NKCDeckListButton cNKCDeckListButton = m_listDeckListButton[b];
			cNKCDeckListButton.Init(b, OnChangedMultiSelectedCount);
			cNKCDeckListButton.m_cbtnButton.PointerClick.RemoveAllListeners();
			cNKCDeckListButton.m_cbtnButton.PointerClick.AddListener(delegate
			{
				DeckViewListClick(new NKMDeckIndex(m_eCurrentDeckType, cNKCDeckListButton.m_DeckIndex.m_iIndex));
			});
			cNKCDeckListButton.m_ctToggleForMulti.SetbReverseSeqCallbackCall(bSet: true);
			cNKCDeckListButton.m_ctToggleForMulti.SetToggleGroup(m_ToggleGroup);
		}
		m_supportListButton.Init(OnClickSupportButton);
	}

	private void OnChangedMultiSelectedCount(int index, bool bSet)
	{
		if (bSet)
		{
			m_lstMultiSelectedIndex.Add(index);
		}
		else
		{
			for (int i = 0; i < m_lstMultiSelectedIndex.Count; i++)
			{
				if (m_lstMultiSelectedIndex[i] == index)
				{
					m_lstMultiSelectedIndex.RemoveAt(i);
				}
			}
		}
		UpdateMultiSelectedSeqUI();
		if (dOnChangedMuiltiSelectedDeckCount != null)
		{
			dOnChangedMuiltiSelectedDeckCount(GetMultiSelectedCount());
		}
	}

	public List<NKMDeckIndex> GetMultiSelectedDeckIndexList()
	{
		List<NKMDeckIndex> list = new List<NKMDeckIndex>();
		for (int i = 0; i < m_lstMultiSelectedIndex.Count; i++)
		{
			list.Add(new NKMDeckIndex(m_eCurrentDeckType, m_lstMultiSelectedIndex[i]));
		}
		return list;
	}

	public int GetMultiSelectedCount()
	{
		int num = 0;
		for (byte b = 0; b < m_listDeckListButton.Count; b++)
		{
			if (m_listDeckListButton[b].m_ctToggleForMulti.m_bChecked)
			{
				num++;
			}
		}
		return num;
	}

	public void Open(bool bMultiSelect, NKMArmyData armyData, NKM_DECK_TYPE eDeckType, int selectedIndex, NKCUIDeckViewer.DeckViewerOption deckViewOption)
	{
		if (m_bOpen)
		{
			return;
		}
		m_bOpen = true;
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		m_rtDeckViewList.SetWidth(310f);
		m_eCurrentDeckType = eDeckType;
		SetDeckListButton(bMultiSelect, armyData, deckViewOption, selectedIndex);
		m_ToggleGroup.transform.localPosition = Vector3.zero;
		bool flag = true;
		RectTransform content = m_scrollRect.content;
		LayoutRebuilder.ForceRebuildLayoutImmediate(content);
		if (m_listDeckListButton.Count > 0)
		{
			flag = content.GetHeight() > m_scrollRect.GetComponent<RectTransform>().GetHeight();
		}
		m_scrollRect.enabled = flag;
		m_lstMultiSelectedIndex.Clear();
		if (!bMultiSelect)
		{
			return;
		}
		if (deckViewOption.lstMultiSelectedDeckIndex != null)
		{
			List<NKMDeckIndex> lstMultiSelectedDeckIndex = deckViewOption.lstMultiSelectedDeckIndex;
			for (int i = 0; i < lstMultiSelectedDeckIndex.Count; i++)
			{
				m_lstMultiSelectedIndex.Add(lstMultiSelectedDeckIndex[i].m_iIndex);
			}
		}
		ProcessMultiSelectUI_WhenOpen();
	}

	private void ProcessMultiSelectUI_WhenOpen()
	{
		for (int i = 0; i < m_listDeckListButton.Count; i++)
		{
			NKCDeckListButton nKCDeckListButton = m_listDeckListButton[i];
			if (!(nKCDeckListButton != null))
			{
				continue;
			}
			bool bSelect = false;
			int num = 0;
			if (m_lstMultiSelectedIndex != null)
			{
				for (num = 0; num < m_lstMultiSelectedIndex.Count; num++)
				{
					if (nKCDeckListButton.m_DeckIndex.m_iIndex == m_lstMultiSelectedIndex[num])
					{
						bSelect = true;
						break;
					}
				}
			}
			nKCDeckListButton.m_ctToggleForMulti.Select(bSelect, bForce: true);
		}
		UpdateMultiSelectedSeqUI();
	}

	private void UpdateMultiSelectedSeqUI()
	{
		for (int i = 0; i < m_listDeckListButton.Count; i++)
		{
			NKCDeckListButton nKCDeckListButton = m_listDeckListButton[i];
			if (!(nKCDeckListButton != null))
			{
				continue;
			}
			int num = 0;
			if (m_lstMultiSelectedIndex == null)
			{
				continue;
			}
			for (num = 0; num < m_lstMultiSelectedIndex.Count; num++)
			{
				if (nKCDeckListButton.m_DeckIndex.m_iIndex == m_lstMultiSelectedIndex[num])
				{
					NKCUtil.SetLabelText(nKCDeckListButton.m_lbMultiSelectedSeq, (num + 1).ToString());
					break;
				}
			}
		}
	}

	public void Close()
	{
		if (m_bOpen)
		{
			m_bOpen = false;
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}

	public void SetDeckListButton(bool bMultiSelect, NKMArmyData armyData, NKCUIDeckViewer.DeckViewerOption deckViewOption, int selectedIndex, bool bSelectDeck = true)
	{
		m_ToggleGroup.m_MaxMultiCount = deckViewOption.maxMultiSelectCount;
		NKCUtil.SetGameobjectActive(m_supportListButton, deckViewOption.bUsableSupporter);
		m_eCurrentDeckType = deckViewOption.DeckIndex.m_eDeckType;
		m_iUnlockedDeckCount = armyData.GetUnlockedDeckCount(m_eCurrentDeckType);
		bool flag = NKCContentManager.IsContentsUnlocked(ContentsType.DECKVIEW_LIST);
		bool flag2 = NKCTutorialManager.TutorialCompleted(TutorialStep.SecondDeckSetup);
		for (int i = 0; i < m_listDeckListButton.Count; i++)
		{
			NKCDeckListButton nKCDeckListButton = m_listDeckListButton[i];
			if (nKCDeckListButton != null)
			{
				nKCDeckListButton.SetMultiSelect(bMultiSelect);
				nKCDeckListButton.SetTrimDeckSelect(deckViewOption.eDeckviewerMode == NKCUIDeckViewer.DeckViewerMode.PrepareLocalDeck);
			}
			bool flag3 = true;
			if (deckViewOption.ShowDeckIndexList != null && deckViewOption.ShowDeckIndexList.Count > 0)
			{
				flag3 = deckViewOption.ShowDeckIndexList.Contains(i);
			}
			if (deckViewOption.eDeckviewerMode == NKCUIDeckViewer.DeckViewerMode.PrepareLocalDeck)
			{
				NKCUtil.SetGameobjectActive(nKCDeckListButton, flag3);
				if (flag3)
				{
					NKCUtil.SetGameobjectActive(nKCDeckListButton, flag3);
					nKCDeckListButton.UnLock();
				}
			}
			else if (i < armyData.GetUnlockedDeckCount(m_eCurrentDeckType))
			{
				if (i == 0 || flag || flag2)
				{
					NKCUtil.SetGameobjectActive(nKCDeckListButton, flag3);
					if (nKCDeckListButton != null && flag3)
					{
						nKCDeckListButton.SetData(armyData, new NKMDeckIndex(m_eCurrentDeckType, i), deckViewOption.DeckListButtonStateText);
						nKCDeckListButton.UnLock();
					}
				}
				else
				{
					NKCUtil.SetGameobjectActive(nKCDeckListButton, bValue: false);
				}
			}
			else if (i < armyData.GetMaxDeckCount(m_eCurrentDeckType))
			{
				if (flag || flag2)
				{
					NKCUtil.SetGameobjectActive(nKCDeckListButton, flag3);
					nKCDeckListButton?.Lock();
				}
				else
				{
					NKCUtil.SetGameobjectActive(nKCDeckListButton, bValue: false);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCDeckListButton, bValue: false);
			}
		}
		if (bSelectDeck)
		{
			SelectDeckList(armyData, selectedIndex);
		}
	}

	public void UpdateDeckListButton(NKMArmyData armyData, NKMDeckIndex targetindex)
	{
		if (!(m_listDeckListButton[targetindex.m_iIndex] == null))
		{
			if (targetindex.m_iIndex < armyData.GetUnlockedDeckCount(targetindex.m_eDeckType))
			{
				m_listDeckListButton[targetindex.m_iIndex].SetData(armyData, targetindex);
				m_listDeckListButton[targetindex.m_iIndex].UnLock();
			}
			else
			{
				m_listDeckListButton[targetindex.m_iIndex].Lock();
			}
		}
	}

	public void UpdateDeckState()
	{
		for (int i = 0; i < m_listDeckListButton.Count; i++)
		{
			m_listDeckListButton[i].UpdateUI();
		}
	}

	public void SelectDeckList(NKMArmyData armyData, int selectedIndex)
	{
		bool flag = false;
		for (int i = 0; i < m_listDeckListButton.Count; i++)
		{
			NKCDeckListButton nKCDeckListButton = m_listDeckListButton[i];
			if (i < armyData.GetDeckCount(m_eCurrentDeckType))
			{
				if (i == selectedIndex)
				{
					nKCDeckListButton.ButtonSelect();
					flag = true;
				}
				else
				{
					nKCDeckListButton.ButtonDeSelect();
				}
			}
		}
		SelectSupButton(!flag);
	}

	public void DeckViewListClick(NKMDeckIndex index)
	{
		if (index.m_iIndex < m_iUnlockedDeckCount)
		{
			if (dOnSelectDeck != null)
			{
				dOnSelectDeck(index);
			}
		}
		else if (dOnDeckUnloakRequest != null)
		{
			dOnDeckUnloakRequest(index);
		}
	}

	public NKCDeckListButton GetDeckListButton(int index)
	{
		if (index < m_listDeckListButton.Count)
		{
			return m_listDeckListButton[index];
		}
		return null;
	}

	public void SetScrollPosition(int index)
	{
		int childCount = m_scrollRect.content.transform.childCount;
		float num = 0f;
		if (childCount > 0)
		{
			num = (float)index / (float)childCount;
		}
		m_scrollRect.verticalNormalizedPosition = 1f - num;
	}

	private void OnClickSupportButton()
	{
		dOnSupportList?.Invoke();
	}

	public void SelectSupButton(bool bSet)
	{
		if (bSet)
		{
			m_supportListButton.On();
		}
		else
		{
			m_supportListButton.Off();
		}
	}
}
