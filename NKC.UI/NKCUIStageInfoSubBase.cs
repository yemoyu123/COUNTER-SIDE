using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIStageInfoSubBase : MonoBehaviour
{
	public delegate void OnButton(bool bSkip, int skipCount);

	[Header("공용")]
	[Header("상단")]
	public TMP_Text m_lbStageNum;

	public TMP_Text m_lbStageName;

	[Header("중단")]
	public ScrollRect m_srDesc;

	public TMP_Text m_lbDesc;

	[Header("던전 보상 리스트 컴포넌트")]
	public GameObject m_objDropItem;

	public NKCUIComDungeonRewardList m_NKCUIComDungeonRewardList;

	[Header("치트")]
	public EventTrigger m_ETDungeonClearReward;

	private OnButton dOnOKButton;

	protected NKMStageTempletV2 m_StageTemplet;

	protected int m_SkipCount = 1;

	protected bool m_bOperationSkip;

	public virtual void InitUI(OnButton onButton)
	{
		m_NKCUIComDungeonRewardList.InitUI();
		dOnOKButton = onButton;
	}

	public virtual void SetData(NKMStageTempletV2 stageTemplet, bool bFirstOpen = true)
	{
		m_StageTemplet = stageTemplet;
		NKCUtil.SetLabelText(m_lbStageNum, NKCUtilString.GetEpisodeNumber(stageTemplet.EpisodeTemplet, stageTemplet));
		NKCUtil.SetLabelText(m_lbStageName, stageTemplet.GetDungeonName());
		NKCUtil.SetLabelText(m_lbDesc, stageTemplet.GetStageDesc());
		if (m_srDesc != null)
		{
			m_srDesc.normalizedPosition = new Vector2(0f, 1f);
		}
		if (m_NKCUIComDungeonRewardList.CreateRewardSlotDataList(NKCScenManager.CurrentUserData(), stageTemplet, stageTemplet.m_StageBattleStrID))
		{
			NKCUtil.SetGameobjectActive(m_objDropItem, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objDropItem, bValue: false);
		}
	}

	public virtual void Update()
	{
		if (base.gameObject.activeInHierarchy)
		{
			m_NKCUIComDungeonRewardList.ShowRewardListUpdate();
		}
	}

	public void OnOK()
	{
		if (dOnOKButton != null)
		{
			dOnOKButton(m_bOperationSkip, m_SkipCount);
		}
	}
}
