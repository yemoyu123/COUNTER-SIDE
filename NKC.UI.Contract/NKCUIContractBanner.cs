using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Contract;

public class NKCUIContractBanner : MonoBehaviour
{
	[Header("n회 Text")]
	public Text m_txtRemainCount;

	[Header("기밀채용 전용 애니메이터")]
	public Animator m_Ani;

	[Header("이벤트용 태그")]
	public GameObject m_objEventTag;

	[Header("Character View")]
	public NKCUICharacterView m_charView;

	[Header("Face Icon")]
	public GameObject m_objFaceCard;

	public Image m_imgFaceCard;

	[Header("ContractInfo")]
	public GameObject m_objContractInfo;

	public NKCUIComContractInfo m_ContractInfo;

	public NKCUIComContractInfoOpr m_ContractInfoOpr;

	[Header("Empty Unit")]
	public GameObject m_objEmpty;

	public int ContractID { get; set; }

	public void SetRemainCount(int count)
	{
		NKCUtil.SetLabelText(m_txtRemainCount, string.Format(NKCUtilString.GET_STRING_CONTRACT_COUNT_ONE_PARAM, count.ToString()));
	}

	public void SetActiveEventTag(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objEventTag, bValue);
	}

	public void SetEnableAnimator(bool bValue)
	{
		if (m_Ani != null)
		{
			m_Ani.enabled = bValue;
		}
	}

	public void SetActiveEmptyBanner(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objEmpty, bValue);
		if (bValue)
		{
			SetFaceCard(0);
			SetContractInfo(0);
			SetCharacterView(0);
		}
	}

	public void SetUnitData(int iUnitID)
	{
		if (iUnitID != 0)
		{
			SetFaceCard(iUnitID);
			SetContractInfo(iUnitID);
			SetCharacterView(iUnitID);
		}
		SetActiveEmptyBanner(iUnitID == 0);
	}

	public void SetFaceCard(int iUnitID)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(iUnitID);
		if (unitTempletBase == null)
		{
			NKCUtil.SetGameobjectActive(m_objFaceCard, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objFaceCard, bValue: true);
		NKCUtil.SetImageSprite(m_imgFaceCard, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UNIT_FACE_CARD", unitTempletBase.m_FaceCardName));
	}

	public void SetContractInfo(int iUnitID)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(iUnitID);
		if (unitTempletBase == null)
		{
			NKCUtil.SetGameobjectActive(m_objContractInfo, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objContractInfo, bValue: true);
		if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			m_ContractInfoOpr.m_OperatorStrID = unitTempletBase.m_UnitStrID;
			m_ContractInfoOpr.SetData();
		}
		else
		{
			m_ContractInfo.m_UnitStrID = unitTempletBase.m_UnitStrID;
			m_ContractInfo.SetData();
		}
	}

	public void SetCharacterView(int iiUnitID)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(iiUnitID);
		if (unitTempletBase == null)
		{
			NKCUtil.SetGameobjectActive(m_charView, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_charView, bValue: true);
		m_charView.SetCharacterIllust(unitTempletBase);
	}
}
