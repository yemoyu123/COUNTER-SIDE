using System.Collections.Generic;
using NKM;
using UnityEngine;

namespace NKC;

public class NKCWarfareGameBattleCondition
{
	public class WFBattleCondition
	{
		public int BCID;

		public NKCAssetInstanceData BCInstance;

		public WFBattleCondition(int id, NKCAssetInstanceData instance)
		{
			BCID = id;
			BCInstance = instance;
		}
	}

	private Transform _trBattleConditionParent;

	private Dictionary<int, WFBattleCondition> _dicWFBattleConditionByTileIndex = new Dictionary<int, WFBattleCondition>();

	private readonly Vector3 BC_SCALE = new Vector3(2.2f, 2.2f, 2.2f);

	private readonly Vector3 BC_POS_ADD = Vector3.up * 18f;

	private const string BUNDLE_NAME = "AB_UI_NKM_UI_WARFARE";

	public NKCWarfareGameBattleCondition(Transform trBattleConditionParent)
	{
		_trBattleConditionParent = trBattleConditionParent;
	}

	public void Init()
	{
		_dicWFBattleConditionByTileIndex.Clear();
	}

	public void Close()
	{
		Dictionary<int, WFBattleCondition>.Enumerator enumerator = _dicWFBattleConditionByTileIndex.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NKCAssetResourceManager.CloseInstance(enumerator.Current.Value.BCInstance);
		}
		_dicWFBattleConditionByTileIndex.Clear();
	}

	public void RemoveBattleCondition(int tileIndex)
	{
		if (_dicWFBattleConditionByTileIndex.ContainsKey(tileIndex))
		{
			NKCAssetResourceManager.CloseInstance(_dicWFBattleConditionByTileIndex[tileIndex].BCInstance);
			_dicWFBattleConditionByTileIndex.Remove(tileIndex);
		}
	}

	public void SetBattleCondition(int tileIndex, int battleConditionID, Vector3 tilePos)
	{
		if (_dicWFBattleConditionByTileIndex.ContainsKey(tileIndex))
		{
			if (_dicWFBattleConditionByTileIndex[tileIndex].BCID == battleConditionID)
			{
				return;
			}
			RemoveBattleCondition(tileIndex);
		}
		NKMBattleConditionTemplet templetByID = NKMBattleConditionManager.GetTempletByID(battleConditionID);
		if (templetByID != null)
		{
			string battleCondWFIcon = templetByID.BattleCondWFIcon;
			NKCAssetInstanceData nKCAssetInstanceData = CreateBattleCondition(battleCondWFIcon, tilePos);
			if (nKCAssetInstanceData != null)
			{
				WFBattleCondition value = new WFBattleCondition(battleConditionID, nKCAssetInstanceData);
				_dicWFBattleConditionByTileIndex.Add(tileIndex, value);
			}
		}
	}

	private NKCAssetInstanceData CreateBattleCondition(string fileName, Vector3 tilePos)
	{
		if (string.IsNullOrEmpty(fileName))
		{
			return null;
		}
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WARFARE", fileName);
		GameObject instant = nKCAssetInstanceData.m_Instant;
		if (instant == null)
		{
			Debug.LogError($"전역 전투 환경 오브젝트를 찾을 수 없음 - {fileName}");
			return null;
		}
		if (_trBattleConditionParent != null)
		{
			instant.transform.SetParent(_trBattleConditionParent);
			instant.transform.localScale = BC_SCALE;
			instant.transform.localPosition = tilePos + BC_POS_ADD;
		}
		if (instant.GetComponent<NKCUICamFaceBillboard>() == null)
		{
			instant.AddComponent<NKCUICamFaceBillboard>();
		}
		return nKCAssetInstanceData;
	}
}
