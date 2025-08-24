using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCUIStageInfo : MonoBehaviour
{
	public delegate void OnButton(NKMStageTempletV2 stageTemplet, bool bSkip, int skipCount);

	[Header("스테이지")]
	public NKCUIStageInfoSubStage m_StageUI;

	[Header("스토리")]
	public NKCUIStageInfoSubStory m_StoryUI;

	private OnButton dOnOKButton;

	private NKMStageTempletV2 m_StageTemplet;

	public bool IsOpened()
	{
		return base.gameObject.activeSelf;
	}

	public void InitUI(OnButton onButton)
	{
		m_StageUI.InitUI(OnOK);
		m_StoryUI.InitUI(OnOK);
		dOnOKButton = onButton;
	}

	public void Open(NKMStageTempletV2 stageTemplet)
	{
		if (m_StageTemplet != stageTemplet)
		{
			m_StageTemplet = stageTemplet;
			if (m_StageTemplet.DungeonTempletBase != null && m_StageTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE)
			{
				SetStoryInfo();
			}
			else
			{
				SetStageInfo();
			}
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		}
	}

	public void Close()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_StageTemplet = null;
		m_StageUI.Close();
		m_StoryUI.Close();
	}

	public void RefreshUI()
	{
		if (m_StageTemplet.DungeonTempletBase != null && m_StageTemplet.DungeonTempletBase.m_DungeonType == NKM_DUNGEON_TYPE.NDT_CUTSCENE)
		{
			SetStoryInfo();
		}
		else
		{
			SetStageInfo(bFirstOpen: false);
		}
	}

	private void SetStageInfo(bool bFirstOpen = true)
	{
		NKCUtil.SetGameobjectActive(m_StageUI, bValue: true);
		NKCUtil.SetGameobjectActive(m_StoryUI, bValue: false);
		m_StageUI.SetData(m_StageTemplet, bFirstOpen);
	}

	private void SetStoryInfo()
	{
		NKCUtil.SetGameobjectActive(m_StageUI, bValue: false);
		NKCUtil.SetGameobjectActive(m_StoryUI, bValue: true);
		m_StoryUI.SetData(m_StageTemplet);
	}

	public void RefreshFavoriteState()
	{
		m_StageUI.RefreshFavoriteState();
	}

	public void OnOK(bool bSkip, int skipCount)
	{
		if (dOnOKButton != null)
		{
			dOnOKButton(m_StageTemplet, bSkip, skipCount);
		}
	}
}
