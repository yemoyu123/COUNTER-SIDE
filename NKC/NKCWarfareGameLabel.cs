using System;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCWarfareGameLabel : MonoBehaviour
{
	[Serializable]
	public class WFLabel
	{
		public GameObject _obj;

		public Text _text;

		public GameObject _fx;
	}

	public WFLabel m_NUM_WARFARE_ENTER;

	public WFLabel m_NUM_WARFARE_HOLD;

	public WFLabel m_NUM_WARFARE_SUMMON;

	public int Index { get; private set; }

	public WARFARE_LABEL_TYPE LabelType { get; private set; }

	public static NKCWarfareGameLabel GetNewInstance(Transform parent)
	{
		NKCWarfareGameLabel component = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WARFARE", "NUM_WARFARE_LABEL").m_Instant.GetComponent<NKCWarfareGameLabel>();
		if (component == null)
		{
			Debug.LogError("NKCWarfareGameLabel Prefab null!");
			return null;
		}
		if (parent != null)
		{
			component.transform.SetParent(parent);
			component.transform.localScale = Vector3.one;
		}
		component.Hide();
		return component;
	}

	public void Hide()
	{
		SetWFLabelType(-1, WARFARE_LABEL_TYPE.NONE);
	}

	public void SetWFLabelType(int index, WARFARE_LABEL_TYPE labelType)
	{
		Index = index;
		LabelType = labelType;
		base.gameObject.SetActive(labelType != WARFARE_LABEL_TYPE.NONE);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_ENTER._obj, bValue: false);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_HOLD._obj, labelType == WARFARE_LABEL_TYPE.HOLD);
		NKCUtil.SetGameobjectActive(m_NUM_WARFARE_SUMMON._obj, labelType == WARFARE_LABEL_TYPE.SUMMON);
	}

	public void SetWFLabelCount(int count)
	{
		WFLabel currentWFLabel = GetCurrentWFLabel();
		if (currentWFLabel != null)
		{
			NKCUtil.SetLabelText(currentWFLabel._text, count.ToString());
			NKCUtil.SetGameobjectActive(currentWFLabel._fx, count == 0);
		}
	}

	private WFLabel GetCurrentWFLabel()
	{
		return LabelType switch
		{
			WARFARE_LABEL_TYPE.ENTER => m_NUM_WARFARE_ENTER, 
			WARFARE_LABEL_TYPE.HOLD => m_NUM_WARFARE_HOLD, 
			WARFARE_LABEL_TYPE.SUMMON => m_NUM_WARFARE_SUMMON, 
			_ => null, 
		};
	}
}
