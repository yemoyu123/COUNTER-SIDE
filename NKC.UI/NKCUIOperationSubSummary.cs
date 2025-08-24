using System.Collections.Generic;
using Cs.Logging;
using NKC.Templet;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperationSubSummary : MonoBehaviour
{
	[Header("좌측 큰 이벤트배너 부모오브젝트")]
	public Transform m_trBigRoot;

	public GameObject m_objBigNone;

	public GameObject m_objPVE_01_None;

	public NKCUIOperationSubSummarySlot m_pfbPVE_01;

	public LoopScrollRect m_LoopPVE_01;

	public GameObject m_objPVE_02_None;

	public NKCUIOperationSubSummarySlot m_pfbPVE_02;

	public LoopScrollRect m_LoopPVE_02;

	public GameObject m_objProgressNone;

	public NKCUIOperationSubSummarySlot m_slotProgress;

	public GameObject m_objLastPlayNone;

	public NKCUIOperationSubSummarySlot m_slotLastPlay;

	private Stack<NKCUIOperationSubSummarySlot> m_stkPVE_01 = new Stack<NKCUIOperationSubSummarySlot>();

	private Stack<NKCUIOperationSubSummarySlot> m_stkPVE_02 = new Stack<NKCUIOperationSubSummarySlot>();

	private NKCEpisodeSummaryTemplet m_BigBannerTemplet;

	private List<NKCEpisodeSummaryTemplet> m_lstEpTempletForPVE_01 = new List<NKCEpisodeSummaryTemplet>();

	private List<NKCEpisodeSummaryTemplet> m_lstEpTempletForPVE_02 = new List<NKCEpisodeSummaryTemplet>();

	private static List<NKCAssetInstanceData> m_listNKCAssetResourceData = new List<NKCAssetInstanceData>();

	public void InitUI()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_LoopPVE_01.dOnGetObject += GetObjectPVE_01;
		m_LoopPVE_01.dOnReturnObject += ReturnObjectPVE_01;
		m_LoopPVE_01.dOnProvideData += ProvideDataPVE_01;
		m_LoopPVE_01.PrepareCells();
		m_LoopPVE_02.dOnGetObject += GetObjectPVE_02;
		m_LoopPVE_02.dOnReturnObject += ReturnObjectPVE_02;
		m_LoopPVE_02.dOnProvideData += ProvideDataPVE_02;
		m_LoopPVE_02.PrepareCells();
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void CloseBigSlot()
	{
		for (int i = 0; i < m_listNKCAssetResourceData.Count; i++)
		{
			NKCAssetResourceManager.CloseInstance(m_listNKCAssetResourceData[i]);
		}
		m_listNKCAssetResourceData.Clear();
	}

	private RectTransform GetObjectPVE_01(int index)
	{
		NKCUIOperationSubSummarySlot nKCUIOperationSubSummarySlot = null;
		nKCUIOperationSubSummarySlot = ((m_stkPVE_01.Count <= 0) ? Object.Instantiate(m_pfbPVE_01, m_LoopPVE_01.content) : m_stkPVE_01.Pop());
		return nKCUIOperationSubSummarySlot.GetComponent<RectTransform>();
	}

	private void ReturnObjectPVE_01(Transform tr)
	{
		NKCUIOperationSubSummarySlot component = tr.GetComponent<NKCUIOperationSubSummarySlot>();
		if (!(component == null))
		{
			m_stkPVE_01.Push(component);
			NKCUtil.SetGameobjectActive(component, bValue: false);
		}
	}

	private void ProvideDataPVE_01(Transform tr, int idx)
	{
		NKCUIOperationSubSummarySlot component = tr.GetComponent<NKCUIOperationSubSummarySlot>();
		if (!(component == null))
		{
			NKCUtil.SetGameobjectActive(component, bValue: true);
			component.SetData(m_lstEpTempletForPVE_01[idx]);
		}
	}

	private RectTransform GetObjectPVE_02(int index)
	{
		NKCUIOperationSubSummarySlot nKCUIOperationSubSummarySlot = null;
		nKCUIOperationSubSummarySlot = ((m_stkPVE_02.Count <= 0) ? Object.Instantiate(m_pfbPVE_02, m_LoopPVE_02.content) : m_stkPVE_02.Pop());
		return nKCUIOperationSubSummarySlot.GetComponent<RectTransform>();
	}

	private void ReturnObjectPVE_02(Transform tr)
	{
		NKCUIOperationSubSummarySlot component = tr.GetComponent<NKCUIOperationSubSummarySlot>();
		if (!(component == null))
		{
			m_stkPVE_02.Push(component);
			NKCUtil.SetGameobjectActive(component, bValue: false);
		}
	}

	private void ProvideDataPVE_02(Transform tr, int idx)
	{
		NKCUIOperationSubSummarySlot component = tr.GetComponent<NKCUIOperationSubSummarySlot>();
		if (!(component == null))
		{
			NKCUtil.SetGameobjectActive(component, bValue: true);
			component.SetData(m_lstEpTempletForPVE_02[idx]);
		}
	}

	public void Open()
	{
		BuildTempletList();
		if (m_listNKCAssetResourceData.Count > 0)
		{
			CloseBigSlot();
		}
		if (m_BigBannerTemplet != null)
		{
			NKCUIOperationSubSummarySlot nKCUIOperationSubSummarySlot = OpenInstanceByAssetName<NKCUIOperationSubSummarySlot>(m_BigBannerTemplet.m_BigResourceID, m_BigBannerTemplet.m_BigResourceID, m_trBigRoot);
			if (nKCUIOperationSubSummarySlot != null)
			{
				nKCUIOperationSubSummarySlot.transform.SetParent(m_trBigRoot);
				nKCUIOperationSubSummarySlot.SetData(m_BigBannerTemplet);
				NKCUtil.SetGameobjectActive(m_objBigNone, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objBigNone, bValue: true);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objBigNone, bValue: true);
		}
		m_LoopPVE_01.TotalCount = m_lstEpTempletForPVE_01.Count;
		NKCUtil.SetGameobjectActive(m_objPVE_01_None, m_LoopPVE_01.TotalCount == 0);
		if (m_LoopPVE_01.TotalCount > 0)
		{
			NKCUtil.SetGameobjectActive(m_LoopPVE_01, bValue: true);
			m_LoopPVE_01.SetIndexPosition(0);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_LoopPVE_01, bValue: false);
		}
		m_LoopPVE_02.TotalCount = m_lstEpTempletForPVE_02.Count;
		NKCUtil.SetGameobjectActive(m_objPVE_02_None, m_LoopPVE_02.TotalCount == 0);
		if (m_LoopPVE_02.TotalCount > 0)
		{
			NKCUtil.SetGameobjectActive(m_LoopPVE_02, bValue: true);
			m_LoopPVE_02.SetIndexPosition(0);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_LoopPVE_02, bValue: false);
		}
		if (m_slotProgress != null)
		{
			bool flag = m_slotProgress.SetMainStreamProgress();
			NKCUtil.SetGameobjectActive(m_slotProgress, flag);
			NKCUtil.SetGameobjectActive(m_objProgressNone, !flag);
		}
		if (m_slotLastPlay != null)
		{
			Log.Debug("[LastPlayInfo][NKCUIOperationSubSummary]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Operation/NKCUIOperationSubSummary.cs", 195);
			if (NKCScenManager.CurrentUserData().m_LastPlayInfo != null)
			{
				Log.Debug($"[LastPlayInfo][NKCUIOperationSubSummary] GameType[{NKCScenManager.CurrentUserData().m_LastPlayInfo.gameType}] StageId[{NKCScenManager.CurrentUserData().m_LastPlayInfo.stageId}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Operation/NKCUIOperationSubSummary.cs", 198);
			}
			bool flag2 = m_slotLastPlay.SetLastPlayInfo(NKCScenManager.CurrentUserData().m_LastPlayInfo);
			NKCUtil.SetGameobjectActive(m_slotLastPlay, flag2);
			NKCUtil.SetGameobjectActive(m_objLastPlayNone, !flag2);
		}
		if (NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedEpisodeTemplet() != null)
		{
			NKMEpisodeTempletV2 reservedEpisodeTemplet = NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedEpisodeTemplet();
			if (NKMEpisodeMgr.CheckPossibleAct(NKCScenManager.CurrentUserData(), reservedEpisodeTemplet.m_EpisodeID, 1))
			{
				NKCUIOperationNodeViewer.Instance.Open(NKCScenManager.GetScenManager().Get_SCEN_OPERATION().GetReservedEpisodeTemplet());
				return;
			}
		}
		TutorialCheck();
	}

	private void BuildTempletList()
	{
		NKMEpisodeMgr.BuildSummaryTemplet(out m_BigBannerTemplet, out m_lstEpTempletForPVE_01, out m_lstEpTempletForPVE_02);
	}

	public static T OpenInstanceByAssetName<T>(string BundleName, string AssetName, Transform parent) where T : MonoBehaviour
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(BundleName, AssetName, bAsync: false, parent);
		if (nKCAssetInstanceData != null && nKCAssetInstanceData.m_Instant != null)
		{
			GameObject instant = nKCAssetInstanceData.m_Instant;
			T component = instant.GetComponent<T>();
			if (component == null)
			{
				Object.Destroy(instant);
				NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
				return null;
			}
			m_listNKCAssetResourceData.Add(nKCAssetInstanceData);
			return component;
		}
		Debug.LogWarning("prefab is null - " + BundleName + "/" + AssetName);
		return null;
	}

	private void TutorialCheck()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.Operation_Summary);
	}
}
