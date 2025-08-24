using System.Collections.Generic;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIComTextUnitLevel : Text
{
	[Header("연계된 다른 Text들. 색이 같이 바뀐다")]
	public List<Text> m_lstLinkedText;

	private Color originalColor;

	private static readonly Color tier1Color = new Color(0.772549f, 0.4823529f, 0.9568627f, 1f);

	private static readonly Color Tier2Color = new Color(0.22f, 0.7725f, 0.9843f, 1f);

	private bool m_bInit;

	public static string GetColorTag(int tier)
	{
		return tier switch
		{
			1 => "<color=#C57BF4>", 
			2 => "<color=#33C5FB>", 
			_ => "<color=#FFFFFF>", 
		};
	}

	protected override void Awake()
	{
		Init();
	}

	private void Init()
	{
		if (!m_bInit)
		{
			originalColor = base.color;
			m_bInit = true;
		}
	}

	public void SetLevel(NKMUnitData unitData, int buffUnitLevel, params Text[] linkedLabels)
	{
		if (unitData == null)
		{
			SetText("0", 0, linkedLabels);
		}
		else
		{
			SetText((unitData.m_UnitLevel + buffUnitLevel).ToString(), NKMUnitLimitBreakManager.GetLBTier(unitData), linkedLabels);
		}
	}

	public void SetLevel(NKMOperator operatorData, params Text[] linkedLabels)
	{
		if (operatorData == null)
		{
			SetText("0", 0, linkedLabels);
		}
		else
		{
			SetText(operatorData.level.ToString(), 0, linkedLabels);
		}
	}

	public void SetLevel(NKMUnitData unitData, int buffUnitLevel, string format, params Text[] linkedLabels)
	{
		if (unitData == null)
		{
			SetText(string.Format(format, 0), 0, linkedLabels);
		}
		else
		{
			SetText(string.Format(format, unitData.m_UnitLevel + buffUnitLevel), NKMUnitLimitBreakManager.GetLBTier(unitData), linkedLabels);
		}
	}

	public void SetText(string _text, NKMUnitData unitData, params Text[] linkedLabels)
	{
		SetText(_text, NKMUnitLimitBreakManager.GetLBTier(unitData), linkedLabels);
	}

	public void SetText(string _text, int lbTier, params Text[] linkedLabels)
	{
		if (!m_bInit)
		{
			Init();
		}
		this.text = _text;
		Color color = (this.color = lbTier switch
		{
			1 => tier1Color, 
			2 => Tier2Color, 
			_ => originalColor, 
		});
		foreach (Text text in linkedLabels)
		{
			if (text != null)
			{
				text.color = color;
			}
		}
		foreach (Text item in m_lstLinkedText)
		{
			if (item != null)
			{
				item.color = color;
			}
		}
	}
}
