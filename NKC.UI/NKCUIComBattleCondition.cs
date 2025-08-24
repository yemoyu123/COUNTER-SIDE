using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComBattleCondition : MonoBehaviour
{
	public delegate void OnDownButton(string battleCondId, Vector3 position);

	public Image m_imgIcon;

	public NKCUIComStateButton m_csbtnButton;

	private string m_battleConditionStrId;

	private NKCAssetInstanceData m_InstanceData;

	private OnDownButton m_dOnDownButton;

	public static NKCUIComBattleCondition GetNewInstance(string bundleName, string assetName, Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(bundleName, assetName);
		NKCUIComBattleCondition nKCUIComBattleCondition = nKCAssetInstanceData?.m_Instant.GetComponent<NKCUIComBattleCondition>();
		if (nKCUIComBattleCondition == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUIComBattleCondition Prefab null!");
			return null;
		}
		nKCUIComBattleCondition.m_InstanceData = nKCAssetInstanceData;
		nKCUIComBattleCondition.Init();
		if (parent != null)
		{
			nKCUIComBattleCondition.transform.SetParent(parent);
		}
		nKCUIComBattleCondition.GetComponent<RectTransform>().localScale = Vector3.one;
		nKCUIComBattleCondition.gameObject.SetActive(value: false);
		return nKCUIComBattleCondition;
	}

	public void Init(OnDownButton onDownButton = null)
	{
		m_battleConditionStrId = null;
		m_dOnDownButton = onDownButton;
		if (m_csbtnButton != null)
		{
			m_csbtnButton.PointerDown.RemoveAllListeners();
			m_csbtnButton.PointerDown.AddListener(OnButtonDown);
		}
	}

	public void SetButtonDownFunc(OnDownButton onDownButton)
	{
		m_dOnDownButton = onDownButton;
	}

	public void SetData(string battleConditionId)
	{
		m_battleConditionStrId = battleConditionId;
		NKMBattleConditionTemplet templetByStrID = NKMBattleConditionManager.GetTempletByStrID(battleConditionId);
		NKCUtil.SetImageSprite(m_imgIcon, NKCUtil.GetSpriteBattleConditionICon(templetByStrID), bDisableIfSpriteNull: true);
	}

	public void SetData(int battleConditionId)
	{
		NKMBattleConditionTemplet templetByID = NKMBattleConditionManager.GetTempletByID(battleConditionId);
		m_battleConditionStrId = templetByID.BattleCondStrID;
		NKCUtil.SetImageSprite(m_imgIcon, NKCUtil.GetSpriteBattleConditionICon(templetByID), bDisableIfSpriteNull: true);
	}

	private void OnButtonDown(PointerEventData pointEventData)
	{
		if (m_dOnDownButton != null)
		{
			m_dOnDownButton(m_battleConditionStrId, pointEventData.position);
		}
	}

	private void OnDestroy()
	{
		m_battleConditionStrId = null;
		m_dOnDownButton = null;
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
	}
}
