using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionStorySlot : MonoBehaviour
{
	public Text m_NKM_UI_COLLECTION_STORY_SLOT_LIST_TEXT;

	public Text m_NKM_UI_COLLECTION_STORY_SLOT_LIST_TITLE_TEXT;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_STORY_SLOT_BUTTON_PLAY1;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_STORY_SLOT_BUTTON_PLAY2;

	public GameObject m_NKM_UI_COLLECTION_STORY_SLOT_TEXT_PART1;

	public GameObject m_NKM_UI_COLLECTION_STORY_SLOT_TEXT_PART2;

	public GameObject m_LOCK;

	private int m_StageID;

	private string m_beginStr;

	private string m_endStr;

	private bool m_bClear;

	private NKCUICollectionStory.OnPlayCutScene dOnPlayCutScene;

	public void Init(NKCUICollectionStory.OnPlayCutScene callback)
	{
		if (null != m_NKM_UI_COLLECTION_STORY_SLOT_BUTTON_PLAY1)
		{
			m_NKM_UI_COLLECTION_STORY_SLOT_BUTTON_PLAY1.PointerClick.RemoveAllListeners();
			m_NKM_UI_COLLECTION_STORY_SLOT_BUTTON_PLAY1.PointerClick.AddListener(delegate
			{
				CutScenePlay(1);
			});
		}
		if (null != m_NKM_UI_COLLECTION_STORY_SLOT_BUTTON_PLAY2)
		{
			m_NKM_UI_COLLECTION_STORY_SLOT_BUTTON_PLAY2.PointerClick.RemoveAllListeners();
			m_NKM_UI_COLLECTION_STORY_SLOT_BUTTON_PLAY2.PointerClick.AddListener(delegate
			{
				CutScenePlay(2);
			});
		}
		base.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
		if (callback != null)
		{
			dOnPlayCutScene = callback;
		}
	}

	public void SetData(string slotNumber, int stageID, string name, bool bClear, string startStr, string endStr, bool bIsDive = false)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_STORY_SLOT_LIST_TEXT, !bIsDive);
		NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_STORY_SLOT_LIST_TEXT, slotNumber);
		NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_STORY_SLOT_LIST_TITLE_TEXT, name);
		bool bValue = !string.IsNullOrEmpty(startStr);
		bool bValue2 = !string.IsNullOrEmpty(endStr);
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_STORY_SLOT_TEXT_PART1, bValue);
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_STORY_SLOT_TEXT_PART2, bValue2);
		m_NKM_UI_COLLECTION_STORY_SLOT_BUTTON_PLAY1?.SetLock(!bClear);
		m_NKM_UI_COLLECTION_STORY_SLOT_BUTTON_PLAY2?.SetLock(!bClear);
		NKCUtil.SetGameobjectActive(m_LOCK, !bClear);
		m_StageID = stageID;
		m_beginStr = startStr;
		m_endStr = endStr;
		m_bClear = bClear;
	}

	public void CutScenePlay(int part)
	{
		if (dOnPlayCutScene != null && m_bClear)
		{
			switch (part)
			{
			case 1:
				dOnPlayCutScene(m_beginStr, m_StageID);
				break;
			case 2:
				dOnPlayCutScene(m_endStr, m_StageID);
				break;
			}
		}
	}
}
