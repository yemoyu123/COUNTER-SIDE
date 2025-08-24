using System.Collections.Generic;
using NKC.Templet;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionUnitProfile : MonoBehaviour
{
	public ScrollRect m_ScrollRect;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd")]
	public NKCUICollectionUnitProfileSlot m_prfUnitProfileSlot;

	public NKCUICollectionUnitCRFSlot m_prfCRFSlot;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public TMP_Text m_CharaterType;

	public Transform m_ProfileSlotParent;

	public NKCUICollectionProfileToolTip m_CharacterToolTip;

	[Header("CRF")]
	public GameObject m_objCFRRoot;

	public TMP_Text m_CRFType;

	public Transform m_CRFSlotParent;

	public NKCUICollectionProfileToolTip m_CRFToolTip;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_TeamUpRoot;

	public Image m_TeamMarkImage;

	public TMP_Text m_TeamName;

	public TMP_Text m_TeamIntel;

	private Queue<NKCUICollectionUnitProfileSlot> m_profileSlotQue = new Queue<NKCUICollectionUnitProfileSlot>();

	private Queue<NKCUICollectionUnitCRFSlot> m_CRFSlotQue = new Queue<NKCUICollectionUnitCRFSlot>();

	public void Init()
	{
		NKCUICollectionUnitProfileSlot[] componentsInChildren = m_ProfileSlotParent.GetComponentsInChildren<NKCUICollectionUnitProfileSlot>();
		if (componentsInChildren != null)
		{
			NKCUICollectionUnitProfileSlot[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i]?.Init();
			}
		}
		m_CharacterToolTip?.Init();
		NKCUICollectionUnitCRFSlot[] componentsInChildren2 = m_CRFSlotParent.GetComponentsInChildren<NKCUICollectionUnitCRFSlot>();
		if (componentsInChildren2 != null)
		{
			NKCUICollectionUnitCRFSlot[] array2 = componentsInChildren2;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i]?.Init();
			}
		}
		m_CRFToolTip?.Init();
	}

	public void SetData(int unitId)
	{
		NKCCollectionEmployeeTemplet employeeTemplet = NKCCollectionManager.GetEmployeeTemplet(unitId);
		if (employeeTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_ProfileSlotParent, bValue: false);
			NKCUtil.SetGameobjectActive(m_CRFSlotParent, bValue: false);
			NKCUtil.SetLabelText(m_TeamName, "");
			NKCUtil.SetLabelText(m_TeamIntel, "");
			NKCUtil.SetImageSprite(m_TeamMarkImage, null);
			return;
		}
		NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(unitId);
		bool flag = unitTemplet != null && !unitTemplet.m_bExclude && unitTemplet.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL;
		NKCUtil.SetGameobjectActive(m_TeamUpRoot, flag);
		if (flag)
		{
			NKCUtil.SetLabelText(m_TeamName, employeeTemplet.GetTeamName());
			NKCUtil.SetLabelText(m_TeamIntel, employeeTemplet.GetTeamConcept());
			Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_COMMON_TEAMUP", employeeTemplet.TeamUpMarkStrID);
			NKCUtil.SetImageSprite(m_TeamMarkImage, orLoadAssetResource);
		}
		bool flag2 = unitTemplet.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR;
		NKCUtil.SetGameobjectActive(m_objCFRRoot, !flag2);
		NKCUtil.SetGameobjectActive(m_ProfileSlotParent, bValue: true);
		NKCUtil.SetGameobjectActive(m_CRFSlotParent, bValue: true);
		NKCUtil.SetLabelText(m_CharaterType, NKCStringTable.GetString(employeeTemplet.CharacterType));
		NKCUtil.SetLabelText(m_CRFType, NKCStringTable.GetString(employeeTemplet.CRFType));
		m_CharacterToolTip?.SetDescData(employeeTemplet.CharacterType);
		m_CRFToolTip?.SetDescData(employeeTemplet.CRFType);
		PrepareProfileSlotQue();
		if (!string.IsNullOrEmpty(employeeTemplet.NameType))
		{
			SetProfileSlotData(employeeTemplet.NameType, employeeTemplet.NameValue);
		}
		if (!string.IsNullOrEmpty(employeeTemplet.GenderType))
		{
			SetProfileSlotData(employeeTemplet.GenderType, employeeTemplet.GenderValueStrID);
		}
		if (!string.IsNullOrEmpty(employeeTemplet.BirthType))
		{
			SetProfileSlotData(employeeTemplet.BirthType, employeeTemplet.BirthValueStrID);
		}
		if (!string.IsNullOrEmpty(employeeTemplet.HeightType))
		{
			SetProfileSlotData(employeeTemplet.HeightType, employeeTemplet.HeightValueStrID);
		}
		if (!string.IsNullOrEmpty(employeeTemplet.SpecialityType))
		{
			SetProfileSlotData(employeeTemplet.SpecialityType, employeeTemplet.SpecialityValueStrID);
		}
		if (!string.IsNullOrEmpty(employeeTemplet.LikeType))
		{
			SetProfileSlotData(employeeTemplet.LikeType, employeeTemplet.LikeValueStrID);
		}
		if (!string.IsNullOrEmpty(employeeTemplet.DisLikeType))
		{
			SetProfileSlotData(employeeTemplet.DisLikeType, employeeTemplet.DisLikeValueStrID);
		}
		if (!string.IsNullOrEmpty(employeeTemplet.CombatLevelType) && !flag2)
		{
			SetProfileSlotData(employeeTemplet.CombatLevelType, employeeTemplet.CombatLevelValue);
		}
		if (!string.IsNullOrEmpty(employeeTemplet.CommandLevelType) && !flag2)
		{
			SetProfileSlotData(employeeTemplet.CommandLevelType, employeeTemplet.CommandLevelValue);
		}
		DeactivateUnusedProfileSlot();
		PrepareCRFSlotQue();
		if (!flag2)
		{
			int count = employeeTemplet.CRFSubType.Count;
			for (int i = 0; i < count; i++)
			{
				string valueStrKey = null;
				float ratio = 0f;
				if (i < employeeTemplet.CRFSubType.Count && i < employeeTemplet.CRFSubAmount.Count)
				{
					valueStrKey = NKCCollectionManager.GetCRFTemplet(employeeTemplet.CRFSubType[i], employeeTemplet.CRFSubAmount[i])?.ProfileAmountDescStrID;
					ratio = Mathf.Clamp((float)employeeTemplet.CRFSubAmount[i] / 100f, 0f, 1f);
				}
				SetCRFSlotData(employeeTemplet.CRFSubType[i], valueStrKey, ratio);
			}
		}
		if (flag2)
		{
			if (!string.IsNullOrEmpty(employeeTemplet.OprProfileType01))
			{
				SetProfileSlotData("SI_COLLECTION_OPR_PROFILE_TYPE_01", employeeTemplet.OprProfileType01);
			}
			if (!string.IsNullOrEmpty(employeeTemplet.OprProfileType02))
			{
				SetProfileSlotData("SI_COLLECTION_OPR_PROFILE_TYPE_02", employeeTemplet.OprProfileType02);
			}
			if (!string.IsNullOrEmpty(employeeTemplet.OprProfileType03))
			{
				SetProfileSlotData("SI_COLLECTION_OPR_PROFILE_TYPE_03", employeeTemplet.OprProfileType03);
			}
		}
		DeactivateUnusedCRFSlot();
		m_ScrollRect.verticalNormalizedPosition = 1f;
	}

	public void ResetCRFGauge()
	{
		NKCUICollectionUnitCRFSlot[] componentsInChildren = m_CRFSlotParent.GetComponentsInChildren<NKCUICollectionUnitCRFSlot>();
		if (componentsInChildren != null)
		{
			NKCUICollectionUnitCRFSlot[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ResetGauge();
			}
		}
	}

	private void PrepareProfileSlotQue()
	{
		m_profileSlotQue.Clear();
		NKCUICollectionUnitProfileSlot[] componentsInChildren = m_ProfileSlotParent.GetComponentsInChildren<NKCUICollectionUnitProfileSlot>();
		if (componentsInChildren != null)
		{
			NKCUICollectionUnitProfileSlot[] array = componentsInChildren;
			foreach (NKCUICollectionUnitProfileSlot item in array)
			{
				m_profileSlotQue.Enqueue(item);
			}
		}
	}

	private NKCUICollectionUnitProfileSlot GetProfileSlot()
	{
		if (m_profileSlotQue.Count > 0)
		{
			NKCUICollectionUnitProfileSlot nKCUICollectionUnitProfileSlot = m_profileSlotQue.Dequeue();
			if ((object)nKCUICollectionUnitProfileSlot != null)
			{
				nKCUICollectionUnitProfileSlot.SetActive(value: true);
				return nKCUICollectionUnitProfileSlot;
			}
			return nKCUICollectionUnitProfileSlot;
		}
		NKCUICollectionUnitProfileSlot nKCUICollectionUnitProfileSlot2 = Object.Instantiate(m_prfUnitProfileSlot);
		nKCUICollectionUnitProfileSlot2.Init();
		nKCUICollectionUnitProfileSlot2.transform.localPosition = Vector3.zero;
		nKCUICollectionUnitProfileSlot2.transform.localScale = Vector3.one;
		nKCUICollectionUnitProfileSlot2.GetComponent<RectTransform>().SetParent(m_ProfileSlotParent);
		return nKCUICollectionUnitProfileSlot2;
	}

	private void DeactivateUnusedProfileSlot()
	{
		while (m_profileSlotQue.Count > 0)
		{
			m_profileSlotQue.Dequeue()?.SetActive(value: false);
		}
	}

	private void PrepareCRFSlotQue()
	{
		m_CRFSlotQue.Clear();
		NKCUICollectionUnitCRFSlot[] componentsInChildren = m_CRFSlotParent.GetComponentsInChildren<NKCUICollectionUnitCRFSlot>();
		if (componentsInChildren != null)
		{
			NKCUICollectionUnitCRFSlot[] array = componentsInChildren;
			foreach (NKCUICollectionUnitCRFSlot item in array)
			{
				m_CRFSlotQue.Enqueue(item);
			}
		}
	}

	private NKCUICollectionUnitCRFSlot GetCRFSlot()
	{
		if (m_CRFSlotQue.Count > 0)
		{
			NKCUICollectionUnitCRFSlot nKCUICollectionUnitCRFSlot = m_CRFSlotQue.Dequeue();
			if ((object)nKCUICollectionUnitCRFSlot != null)
			{
				nKCUICollectionUnitCRFSlot.SetActive(value: true);
				return nKCUICollectionUnitCRFSlot;
			}
			return nKCUICollectionUnitCRFSlot;
		}
		NKCUICollectionUnitCRFSlot nKCUICollectionUnitCRFSlot2 = Object.Instantiate(m_prfCRFSlot);
		nKCUICollectionUnitCRFSlot2.Init();
		nKCUICollectionUnitCRFSlot2.transform.localPosition = Vector3.zero;
		nKCUICollectionUnitCRFSlot2.transform.localScale = Vector3.one;
		nKCUICollectionUnitCRFSlot2.GetComponent<RectTransform>().SetParent(m_CRFSlotParent);
		return nKCUICollectionUnitCRFSlot2;
	}

	private void DeactivateUnusedCRFSlot()
	{
		while (m_CRFSlotQue.Count > 0)
		{
			m_CRFSlotQue.Dequeue()?.SetActive(value: false);
		}
	}

	private void SetProfileSlotData(string typeStrKey, string valueStrKey)
	{
		GetProfileSlot()?.SetData(typeStrKey, valueStrKey);
	}

	private void SetCRFSlotData(string typeStrKey, string valueStrKey, float ratio)
	{
		GetCRFSlot()?.SetData(typeStrKey, valueStrKey, ratio);
	}
}
