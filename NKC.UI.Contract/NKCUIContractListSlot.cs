using NKC.Templet;
using NKC.UI.Component;
using NKM.Contract2;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Contract;

public class NKCUIContractListSlot : NKCUIComFoldableListSlot
{
	private const string TAB_IMAGE_ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_CONTRACT_V2_Tab_Bg";

	public Image m_imgContract;

	public Color m_colBasic;

	public Color m_colClassified;

	public Color m_colConfirm;

	public GameObject m_objRedDot;

	protected override void SetData(NKCUIComFoldableList.Element element)
	{
		NKCContractCategoryTemplet nKCContractCategoryTemplet = NKCContractCategoryTemplet.Find(element.MajorKey);
		if (element.isMajor)
		{
			NKCUtil.SetImageColor(m_imgContract, m_colBasic);
			if (m_Toggle != null)
			{
				m_Toggle.SetTitleText(nKCContractCategoryTemplet.GetName());
			}
			SetImage("");
		}
		else if (NKCScenManager.GetScenManager().GetNKCContractDataMgr() != null)
		{
			switch (nKCContractCategoryTemplet.m_Type)
			{
			case NKCContractCategoryTemplet.TabType.Awaken:
				NKCUtil.SetImageColor(m_imgContract, m_colClassified);
				break;
			case NKCContractCategoryTemplet.TabType.Basic:
				NKCUtil.SetImageColor(m_imgContract, m_colBasic);
				break;
			case NKCContractCategoryTemplet.TabType.FollowTarget:
			{
				NKCUtil.SetImageColor(m_imgContract, m_colBasic);
				ContractTempletV2 contractTempletV = ContractTempletV2.Find(element.MinorKey);
				if (contractTempletV == null)
				{
					break;
				}
				foreach (RandomUnitTempletV2 unitTemplet in contractTempletV.UnitPoolTemplet.UnitTemplets)
				{
					if (unitTemplet.PickUpTarget && unitTemplet.UnitTemplet.m_bAwaken)
					{
						NKCUtil.SetImageColor(m_imgContract, m_colClassified);
						break;
					}
				}
				break;
			}
			case NKCContractCategoryTemplet.TabType.Confirm:
				NKCUtil.SetImageColor(m_imgContract, m_colConfirm);
				break;
			}
			if (!(m_Toggle != null))
			{
				return;
			}
			string titleText = "";
			ContractTempletBase contractTempletBase = ContractTempletBase.FindBase(element.MinorKey);
			if (contractTempletBase != null)
			{
				titleText = contractTempletBase.GetContractName();
			}
			else
			{
				CustomPickupContractTemplet customPickupContractTemplet = CustomPickupContractTemplet.Find(element.MinorKey);
				if (customPickupContractTemplet != null)
				{
					titleText = customPickupContractTemplet.GetContractName();
				}
			}
			m_Toggle.SetTitleText(titleText);
		}
		else
		{
			NKCUtil.SetImageColor(m_imgContract, m_colBasic);
			if (m_Toggle != null)
			{
				m_Toggle.SetTitleText("");
			}
		}
	}

	public void SetActiveRedDot(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objRedDot, bValue);
	}

	public void SetImage(string imageName)
	{
		if (!string.IsNullOrEmpty(imageName))
		{
			NKCUtil.SetImageSprite(m_imgContract, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_CONTRACT_V2_Tab_Bg", imageName));
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgContract, null);
		}
	}
}
