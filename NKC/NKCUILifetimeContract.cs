using System.Collections;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC;

public class NKCUILifetimeContract : MonoBehaviour
{
	public delegate void OnEndDrag();

	public delegate void OnEndAni();

	public Animator m_aniContract;

	public NKCUIComDrag m_comDrag;

	[Header("드래그 시작하면 끔")]
	public GameObject m_objHelp;

	public GameObject m_objArrow;

	[Header("도장 위치")]
	public GameObject m_objStamp;

	public float m_fStampMaxY;

	public float m_fStampMinY;

	public float m_fStampInitY;

	public float m_fSealY;

	[Header("도장 찍고나면 킴")]
	public GameObject m_objPlayerStamp;

	[Header("계약서")]
	public Text m_txtTitle;

	public Text m_txtSubTitle;

	public Image m_imgUnitFace;

	public Image m_imgUnitBG;

	public Text m_txtUnitName;

	public Text m_txtDesc;

	public Text m_txtUnitSign;

	public Text m_txtPlayerSign;

	public Text m_txtUnitStamp;

	private bool m_bDrag;

	private OnEndDrag dOnEndDrag;

	private OnEndAni dOnEndAni;

	public void Init(OnEndDrag onEndDrag, OnEndAni onEndAni)
	{
		m_comDrag.BeginDrag.RemoveAllListeners();
		m_comDrag.BeginDrag.AddListener(OnDragStart);
		m_comDrag.Drag.RemoveAllListeners();
		m_comDrag.Drag.AddListener(OnDraging);
		m_comDrag.EndDrag.RemoveAllListeners();
		m_comDrag.EndDrag.AddListener(OnDragEnd);
		dOnEndDrag = onEndDrag;
		dOnEndAni = onEndAni;
	}

	public void SetData(NKMUnitData unit)
	{
		m_bDrag = false;
		Vector3 localPosition = m_objStamp.transform.localPosition;
		localPosition.y = m_fStampInitY;
		m_objStamp.transform.localPosition = localPosition;
		NKCUtil.SetGameobjectActive(m_objHelp, bValue: true);
		NKCUtil.SetGameobjectActive(m_objArrow, bValue: true);
		NKCUtil.SetGameobjectActive(m_objPlayerStamp, bValue: false);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unit.m_UnitID);
		if (unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_MECHANIC)
		{
			m_txtTitle.text = NKCUtilString.GET_STRING_LIFETIME_CONTRACT_TITLE_MECHANIC;
		}
		else
		{
			m_txtTitle.text = NKCUtilString.GET_STRING_LIFETIME_CONTRACT_TITLE;
		}
		m_txtSubTitle.text = NKCUtilString.GetUnitStyleString(unitTempletBase);
		m_imgUnitBG.sprite = GetUnitGradeBG(unitTempletBase.m_NKM_UNIT_GRADE);
		m_imgUnitFace.sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unit);
		m_txtUnitName.text = unitTempletBase.GetUnitName();
		m_txtDesc.text = NKCUtilString.GetLifetimeContractDesc(unitTempletBase, NKCScenManager.CurrentUserData().m_UserNickName);
		NKCUtil.SetLabelText(m_txtUnitSign, NKCUtilString.GET_STRING_LIFETIME_CONTRACT_UNIT_SIGN_TWO_PARAM, unitTempletBase.GetUnitTitle(), unitTempletBase.GetUnitName());
		m_txtUnitStamp.text = unitTempletBase.GetUnitName();
		NKCUtil.SetLabelText(m_txtPlayerSign, NKCUtilString.GET_STRING_LIFETIME_CONTRACT_PLAYER_SIGN_ONE_PARAM, NKCScenManager.CurrentUserData().m_UserNickName);
		m_objStamp.GetComponent<Image>().color = Color.white;
	}

	public void PlayAni()
	{
		m_aniContract.Play("NKM_UI_UNIT_INFO_LIFETIME_OUTRO");
		StartCoroutine(EndAni());
	}

	private IEnumerator EndAni()
	{
		do
		{
			yield return null;
		}
		while (!(m_aniContract.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f));
		dOnEndAni?.Invoke();
	}

	private void OnDragStart(PointerEventData eventData)
	{
		NKCUtil.SetGameobjectActive(m_objHelp, bValue: false);
		NKCUtil.SetGameobjectActive(m_objArrow, bValue: false);
	}

	private void OnDraging(PointerEventData eventData)
	{
		if (!m_bDrag)
		{
			Vector3 localPosition = m_objStamp.transform.localPosition;
			localPosition.y += eventData.delta.y;
			if (localPosition.y < m_fStampMinY)
			{
				localPosition.y = m_fStampMinY;
			}
			else if (localPosition.y > m_fStampMaxY)
			{
				localPosition.y = m_fStampMaxY;
			}
			m_objStamp.transform.localPosition = localPosition;
			if (localPosition.y <= m_fSealY)
			{
				m_bDrag = true;
				NKCUtil.SetGameobjectActive(m_objPlayerStamp, bValue: true);
				dOnEndDrag?.Invoke();
			}
		}
	}

	private void OnDragEnd(PointerEventData eventData)
	{
		if (!m_bDrag)
		{
			Vector3 localPosition = m_objStamp.transform.localPosition;
			if (localPosition.y <= m_fSealY)
			{
				m_bDrag = true;
				NKCUtil.SetGameobjectActive(m_objPlayerStamp, bValue: true);
				dOnEndDrag?.Invoke();
			}
			else
			{
				localPosition.y = m_fStampInitY;
				m_objStamp.transform.localPosition = localPosition;
				NKCUtil.SetGameobjectActive(m_objHelp, bValue: true);
				NKCUtil.SetGameobjectActive(m_objArrow, bValue: true);
			}
		}
	}

	private Sprite GetUnitGradeBG(NKM_UNIT_GRADE grade)
	{
		string bundleName = "ab_ui_unit_slot_card_sprite";
		return NKCResourceUtility.GetAssetResource(bundleName, grade switch
		{
			NKM_UNIT_GRADE.NUG_R => "NKM_UI_UNIT_SELECT_LIST_GRADE_R", 
			NKM_UNIT_GRADE.NUG_SR => "NKM_UI_UNIT_SELECT_LIST_GRADE_SR", 
			NKM_UNIT_GRADE.NUG_SSR => "NKM_UI_UNIT_SELECT_LIST_GRADE_SSR", 
			_ => "NKM_UI_UNIT_SELECT_LIST_GRADE_N", 
		})?.GetAsset<Sprite>();
	}
}
