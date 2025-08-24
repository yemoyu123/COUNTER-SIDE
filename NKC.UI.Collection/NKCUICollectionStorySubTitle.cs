using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionStorySubTitle : MonoBehaviour
{
	public Text m_NKM_UI_COLLECTION_STORY_SLOT_ACT_TEXT;

	public void Init()
	{
	}

	public void SetTitle(string str)
	{
		NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_STORY_SLOT_ACT_TEXT, str);
	}
}
