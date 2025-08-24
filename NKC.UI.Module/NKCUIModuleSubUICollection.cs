using System.Collections.Generic;
using ClientPacket.Event;
using NKM;
using NKM.Event;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Module;

public class NKCUIModuleSubUICollection : NKCUIModuleSubUIBase
{
	public LoopScrollFlexibleRect m_loopScrollFlxRect;

	public Text m_lbAchieveRate;

	public Text m_lbAchieveCount;

	public Image m_imgAchieveGauge;

	private NKMEventCollectionTemplet m_eventCollectionTemplet;

	private List<(int, List<(int, bool)>)> m_unitList = new List<(int, List<(int, bool)>)>();

	private string m_slotBundleName;

	private string m_slotAssetName;

	private bool m_bScrollRectInit;

	public override void Init()
	{
	}

	public override void OnOpen(NKMEventCollectionIndexTemplet templet)
	{
		base.OnOpen(templet);
		if (templet != null)
		{
			m_eventCollectionTemplet = NKMEventCollectionTemplet.Find(templet.EventCollectionGroupId);
			m_slotBundleName = templet.EventCollectionSlotPrefabID_BundleName;
			m_slotAssetName = templet.EventCollectionSlotPrefabID_AssetName;
			if (!m_bScrollRectInit && m_loopScrollFlxRect != null)
			{
				m_loopScrollFlxRect.dOnGetObject += GetSlot;
				m_loopScrollFlxRect.dOnReturnObject += ReturnSlot;
				m_loopScrollFlxRect.dOnProvideData += ProvideData;
				m_loopScrollFlxRect.ContentConstraintCount = 1;
				m_loopScrollFlxRect.TotalCount = 0;
				m_loopScrollFlxRect.PrepareCells();
				m_bScrollRectInit = true;
			}
			Refresh();
		}
	}

	public override void Refresh()
	{
		base.Refresh();
		if (!base.gameObject.activeSelf || m_eventCollectionTemplet == null)
		{
			return;
		}
		NKMEventCollectionInfo nKMEventCollectionInfo = NKCScenManager.CurrentUserData()?.EventCollectionInfo;
		int num = 0;
		int num2 = 0;
		Dictionary<int, List<(int, bool)>> dictionary = new Dictionary<int, List<(int, bool)>>();
		foreach (NKMEventCollectionDetailTemplet detail in m_eventCollectionTemplet.Details)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(detail.Key);
			if (unitTempletBase != null)
			{
				if (!dictionary.ContainsKey(detail.CollectionGradeGroupId))
				{
					dictionary.Add(detail.CollectionGradeGroupId, new List<(int, bool)>());
				}
				bool flag = false;
				if (nKMEventCollectionInfo != null)
				{
					flag = nKMEventCollectionInfo.goodsCollection.Contains(unitTempletBase.m_UnitID);
				}
				dictionary[detail.CollectionGradeGroupId].Add((unitTempletBase.m_UnitID, flag));
				num++;
				if (flag)
				{
					num2++;
				}
			}
		}
		m_unitList.Clear();
		foreach (KeyValuePair<int, List<(int, bool)>> item in dictionary)
		{
			m_unitList.Add((item.Key, item.Value));
		}
		m_unitList.Sort(delegate((int, List<(int, bool)>) e1, (int, List<(int, bool)>) e2)
		{
			if (e1.Item1 > e2.Item1)
			{
				return -1;
			}
			return (e1.Item1 < e2.Item1) ? 1 : 0;
		});
		m_loopScrollFlxRect.TotalCount = m_unitList.Count;
		m_loopScrollFlxRect.SetIndexPosition(0);
		float num3 = (float)num2 / (float)num;
		NKCUtil.SetLabelText(m_lbAchieveRate, Mathf.FloorToInt(num3 * 100f).ToString());
		NKCUtil.SetLabelText(m_lbAchieveCount, $"{num2}/{num}");
		NKCUtil.SetImageFillAmount(m_imgAchieveGauge, num3);
	}

	public override void OnClose()
	{
		base.OnClose();
		m_slotBundleName = null;
		m_slotAssetName = null;
		m_unitList.Clear();
		m_eventCollectionTemplet = null;
	}

	private RectTransform GetSlot(int index)
	{
		NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(m_slotBundleName, m_slotAssetName);
		return NKCUIModuleCollectionGroup.GetNewInstance(null, nKMAssetName.m_BundleName.ToLower(), nKMAssetName.m_AssetName)?.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform tr)
	{
		NKCUIModuleCollectionGroup component = tr.GetComponent<NKCUIModuleCollectionGroup>();
		tr.SetParent(null);
		if (component != null)
		{
			component.DestoryInstance();
		}
		else
		{
			Object.Destroy(tr.gameObject);
		}
	}

	private void ProvideData(Transform tr, int index)
	{
		NKCUIModuleCollectionGroup component = tr.GetComponent<NKCUIModuleCollectionGroup>();
		if (!(component == null) && index < m_unitList.Count)
		{
			component.SetData(m_unitList[index].Item1, m_unitList[index].Item2);
		}
	}
}
