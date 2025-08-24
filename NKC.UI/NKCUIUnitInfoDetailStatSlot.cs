using NKM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitInfoDetailStatSlot : MonoBehaviour
{
	public Text m_STAT_NAME_01;

	public Text m_STAT_TEXT;

	public NKCComStatInfoToolTip m_InfoToolTip;

	[Header("TextMeshPro 대응")]
	public TMP_Text m_TMP_STAT_NAME;

	public TMP_Text m_TMP_STAT_TEXT;

	private NKCAssetInstanceData m_Instance;

	public static NKCUIUnitInfoDetailStatSlot GetNewInstance(string bundleName, string assetName, Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>(bundleName, assetName);
		if (nKCAssetInstanceData == null || nKCAssetInstanceData.m_Instant == null)
		{
			Debug.LogError(assetName + " Prefab null!");
			return null;
		}
		NKCUIUnitInfoDetailStatSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUIUnitInfoDetailStatSlot>();
		if (component == null)
		{
			Debug.LogError("NKCUIUnitInfoDetailStatSlot null!");
			return null;
		}
		component.Init();
		component.m_Instance = nKCAssetInstanceData;
		component.transform.SetParent(parent, worldPositionStays: false);
		component.gameObject.SetActive(value: false);
		return component;
	}

	private void Init()
	{
		if (m_InfoToolTip == null && m_STAT_NAME_01 != null)
		{
			m_STAT_NAME_01.raycastTarget = true;
			m_InfoToolTip = m_STAT_NAME_01.gameObject.GetComponent<NKCComStatInfoToolTip>();
			if (m_InfoToolTip == null)
			{
				m_InfoToolTip = m_STAT_NAME_01.gameObject.AddComponent<NKCComStatInfoToolTip>();
			}
		}
	}

	public void SetData(NKM_STAT_TYPE eType, NKMStatData statData)
	{
		decimal num = NKMUnitStatManager.GetFinalStatForUIOutput(eType, statData);
		string statShortName = NKCUtilString.GetStatShortName(eType, num);
		NKCUtil.SetLabelText(m_STAT_NAME_01, statShortName);
		NKCUtil.SetLabelText(m_TMP_STAT_NAME, statShortName);
		bool num2 = NKMUnitStatManager.IsPercentStat(eType);
		bool bNegative = false;
		if (NKCUtilString.IsNameReversedIfNegative(eType) && num < 0m)
		{
			bNegative = true;
			num = -num;
		}
		float statPercentage = NKCUtil.GetStatPercentage(eType, statData.GetStatBase(eType) + statData.GetBaseBonusStat(eType));
		string text = null;
		text = (num2 ? ((statPercentage == 0f) ? $"{num:P1}" : string.Format("{0:P1}({1}%)", num, statPercentage.ToString("N2"))) : ((statPercentage == 0f) ? $"{num:#;-#;0}" : string.Format("{0:#;-#;0}({1}%)", num, statPercentage.ToString("N2"))));
		NKCUtil.SetLabelText(m_STAT_TEXT, text);
		NKCUtil.SetLabelText(m_TMP_STAT_TEXT, text);
		if (m_InfoToolTip != null)
		{
			m_InfoToolTip.SetType(eType, bNegative);
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
	}

	public void Clear()
	{
		if (m_Instance != null)
		{
			NKCAssetResourceManager.CloseInstance(m_Instance);
		}
	}
}
