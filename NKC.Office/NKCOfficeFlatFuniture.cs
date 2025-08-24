using NKM;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.Office;

public class NKCOfficeFlatFuniture : NKCOfficeFuniture
{
	public override void SetData(NKMOfficeInteriorTemplet templet, float tileSize)
	{
		base.SetData(templet, tileSize);
		m_eFunitureType = templet.Target;
		if (m_imgFuniture != null)
		{
			NKMAssetName cNKMAssetName = NKMAssetName.ParseBundleName(templet.PrefabName, templet.PrefabName);
			m_imgFuniture.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(cNKMAssetName);
			m_imgFuniture.type = Image.Type.Tiled;
		}
	}

	public override void SetInvert(bool bInvert, bool bEditMode = false)
	{
		switch (m_eFunitureType)
		{
		case InteriorTarget.Floor:
		case InteriorTarget.Tile:
			if (bInvert)
			{
				if (bEditMode)
				{
					m_rtFloor.rotation = Quaternion.Euler(66.5f, 0f, -45f);
				}
				else
				{
					m_rtFloor.localRotation = Quaternion.Euler(0f, 0f, -90f);
				}
			}
			else if (bEditMode)
			{
				m_rtFloor.rotation = Quaternion.Euler(66.5f, 0f, 45f);
			}
			else
			{
				m_rtFloor.localRotation = Quaternion.identity;
			}
			break;
		case InteriorTarget.Wall:
			if (bInvert)
			{
				if (bEditMode)
				{
					m_rtFloor.rotation = Quaternion.Euler(-16.377f, 47.477f, -17.091f);
				}
				else
				{
					m_rtFloor.localRotation = Quaternion.identity;
				}
			}
			else if (bEditMode)
			{
				m_rtFloor.rotation = Quaternion.Euler(-16.377f, -47.477f, 17.091f);
			}
			else
			{
				m_rtFloor.localRotation = Quaternion.identity;
			}
			break;
		}
	}
}
