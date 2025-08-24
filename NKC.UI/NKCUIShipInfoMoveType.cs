using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIShipInfoMoveType : MonoBehaviour
{
	public Image m_imgRightShipStyleIcon;

	public Image m_imgMoveType;

	public Text m_lbRightShipStyleName;

	public Text m_lbRightShipStyleDesc;

	public void SetData(NKM_UNIT_STYLE_TYPE unitStyleType)
	{
		NKCUtil.SetImageSprite(m_imgRightShipStyleIcon, NKCResourceUtility.GetOrLoadUnitStyleIcon(unitStyleType, bSmall: true));
		NKCUtil.SetImageSprite(m_imgMoveType, GetSpriteMoveType(unitStyleType));
		NKCUtil.SetLabelText(m_lbRightShipStyleName, NKCUtilString.GetUnitStyleName(unitStyleType));
		NKCUtil.SetLabelText(m_lbRightShipStyleDesc, NKCUtilString.GetUnitStyleDesc(unitStyleType));
	}

	private Sprite GetSpriteMoveType(NKM_UNIT_STYLE_TYPE type)
	{
		string stringMoveType = GetStringMoveType(type);
		if (string.IsNullOrEmpty(stringMoveType))
		{
			return null;
		}
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_SHIP_INFO_TEXTURE", stringMoveType);
	}

	private string GetStringMoveType(NKM_UNIT_STYLE_TYPE type)
	{
		string result = string.Empty;
		switch (type)
		{
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_ASSAULT:
			result = "NKM_UI_SHIP_INFO_TEXTURE_MOVETYPE_1";
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY:
			result = "NKM_UI_SHIP_INFO_TEXTURE_MOVETYPE_4";
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER:
			result = "NKM_UI_SHIP_INFO_TEXTURE_MOVETYPE_2";
			break;
		case NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL:
			result = "NKM_UI_SHIP_INFO_TEXTURE_MOVETYPE_3";
			break;
		}
		return result;
	}
}
