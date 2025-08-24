using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Office;

public class NKCUIOfficeMemberEditSlot : MonoBehaviour
{
	public delegate void SelectSlot(long unitUId);

	public Text m_lbUnitId;

	public Text m_lbUnitName;

	public Image m_imgUnitGradeBg;

	public Image m_imgUnitFace;

	public Image m_imgRoyaltyGauge;

	public GameObject m_objRoyalty;

	public GameObject m_objRoyaltyMax;

	public GameObject m_objLifeTimeContract;

	public GameObject m_objMovedIn;

	[Header("스킨 아이콘")]
	public GameObject m_objSkin;

	public Image m_imgSkin;

	[Space]
	public GameObject m_objSelected;

	public Text m_lbSelectedNumber;

	public NKCUIComStateButton m_csbtnSelectSlot;

	private NKCAssetInstanceData m_InstanceData;

	private int m_iRoomId;

	private int m_iUnitId;

	private long m_lUnitUId;

	private SelectSlot m_dOnSelectSlot;

	public int UnitId => m_iUnitId;

	public void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnSelectSlot, OnBtnSelectSlot);
	}

	public static NKCUIOfficeMemberEditSlot GetNewInstance(Transform parent, bool bMentoringSlot = false)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_office", "AB_UI_POPUP_OFFICE_MEMBER_EDIT_SLOT");
		NKCUIOfficeMemberEditSlot nKCUIOfficeMemberEditSlot = nKCAssetInstanceData?.m_Instant.GetComponent<NKCUIOfficeMemberEditSlot>();
		if (nKCUIOfficeMemberEditSlot == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUIOfficeMemberEditSlot Prefab null!");
			return null;
		}
		nKCUIOfficeMemberEditSlot.m_InstanceData = nKCAssetInstanceData;
		nKCUIOfficeMemberEditSlot.Init();
		if (parent != null)
		{
			nKCUIOfficeMemberEditSlot.transform.SetParent(parent);
		}
		nKCUIOfficeMemberEditSlot.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		nKCUIOfficeMemberEditSlot.gameObject.SetActive(value: false);
		return nKCUIOfficeMemberEditSlot;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	public void SetData(List<long> unitAssignedList, NKMUnitData unitData, int roomId, SelectSlot onSelectSlot)
	{
		m_iRoomId = roomId;
		if (unitData == null)
		{
			return;
		}
		m_iUnitId = unitData.m_UnitID;
		m_lUnitUId = unitData.m_UnitUID;
		NKCUtil.SetLabelText(m_lbUnitId, NKCCollectionManager.GetEmployeeNumber(unitData.m_UnitID));
		NKCUtil.SetImageFillAmount(m_imgRoyaltyGauge, (float)unitData.loyalty / 10000f);
		NKCUtil.SetGameobjectActive(m_objRoyaltyMax, unitData.loyalty >= 10000);
		NKCUtil.SetGameobjectActive(m_objLifeTimeContract, unitData.IsPermanentContract);
		int num = -1;
		if (unitAssignedList != null)
		{
			num = unitAssignedList.FindIndex((long e) => e == unitData.m_UnitUID);
		}
		NKCUtil.SetGameobjectActive(m_objSelected, num >= 0);
		if (num >= 0)
		{
			NKCUtil.SetLabelText(m_lbSelectedNumber, (num + 1).ToString("D2"));
		}
		NKCUtil.SetGameobjectActive(m_objMovedIn, unitData.OfficeRoomId > 0 && unitData.OfficeRoomId != roomId);
		int unitID = unitData.m_UnitID;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		if (unitTempletBase != null)
		{
			unitID = unitTempletBase.m_BaseUnitID;
			NKCUtil.SetLabelText(m_lbUnitName, NKCStringTable.GetString(unitTempletBase.Name));
			switch (unitTempletBase.m_NKM_UNIT_GRADE)
			{
			case NKM_UNIT_GRADE.NUG_SSR:
				NKCUtil.SetImageSprite(m_imgUnitGradeBg, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_office_sprite", "AB_UI_OFFICE_UNIT_CARD_BG_SSR"));
				break;
			case NKM_UNIT_GRADE.NUG_SR:
				NKCUtil.SetImageSprite(m_imgUnitGradeBg, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_office_sprite", "AB_UI_OFFICE_UNIT_CARD_BG_SR"));
				break;
			case NKM_UNIT_GRADE.NUG_R:
				NKCUtil.SetImageSprite(m_imgUnitGradeBg, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_office_sprite", "AB_UI_OFFICE_UNIT_CARD_BG_R"));
				break;
			case NKM_UNIT_GRADE.NUG_N:
				NKCUtil.SetImageSprite(m_imgUnitGradeBg, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_office_sprite", "AB_UI_OFFICE_UNIT_CARD_BG_N"));
				break;
			}
			NKCUtil.SetImageSprite(m_imgUnitFace, NKCResourceUtility.GetOrLoadMinimapFaceIcon(unitTempletBase));
			NKCUtil.SetGameobjectActive(m_objRoyalty, unitTempletBase.IsUnitStyleType());
		}
		if (unitData.m_SkinID > 0)
		{
			Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, unitID, unitData.m_SkinID);
			NKCUtil.SetImageSprite(m_imgSkin, sprite);
			NKCUtil.SetGameobjectActive(m_objSkin, sprite != null);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objSkin, bValue: false);
		}
		m_dOnSelectSlot = onSelectSlot;
	}

	private void OnBtnSelectSlot()
	{
		if (m_dOnSelectSlot != null)
		{
			m_dOnSelectSlot(m_lUnitUId);
		}
	}
}
