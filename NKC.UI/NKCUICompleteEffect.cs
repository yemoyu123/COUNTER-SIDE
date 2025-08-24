using DG.Tweening;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUICompleteEffect : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_COMPLETE";

	private static NKCUICompleteEffect m_Instance;

	public GameObject m_objMarkSkillUpgrade;

	public GameObject m_objMarkCityOpen;

	public Text m_lbText;

	public GameObject m_objSkillSlotRoot;

	public NKCUISkillSlot m_slotSkillBefore;

	public NKCUISkillSlot m_slotSkillAfter;

	private float m_fOpenTime = 2f;

	private float m_fcurrentTime;

	public static NKCUICompleteEffect Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUICompleteEffect>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_COMPLETE", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCUICompleteEffect>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public override string MenuName => "완료";

	public override eMenutype eUIType => eMenutype.Popup;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
		if (NKCGameEventManager.IsWaiting())
		{
			NKCGameEventManager.WaitFinished();
		}
	}

	public override void OnBackButton()
	{
	}

	public void Init()
	{
		base.transform.position = Vector3.zero;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void OpenCityOpened(float time = 4f)
	{
		DefaultSetup(time);
		NKCUtil.SetGameobjectActive(m_objMarkCityOpen, bValue: true);
		m_lbText.text = NKCUtilString.GET_STRING_WORLDMAP_CITY_MAKE_COMPLETE;
		base.gameObject.SetActive(value: true);
		UIOpened();
		PlayOpenSound();
		PlayOpenAni();
	}

	public void OpenSkillUpgrade(NKMUnitSkillTemplet oldSkillTemplet, NKMUnitSkillTemplet newSkillTemplet, float time = 3f)
	{
		DefaultSetup(time);
		NKCUtil.SetGameobjectActive(m_objMarkSkillUpgrade, bValue: true);
		m_lbText.text = NKCUtilString.GET_STRING_SKILL_TRAINING_COMPLETE;
		bool bIsHyper = false;
		if (oldSkillTemplet != null)
		{
			bIsHyper = oldSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER;
		}
		m_slotSkillBefore.SetData(oldSkillTemplet, bIsHyper);
		bIsHyper = false;
		if (newSkillTemplet != null)
		{
			bIsHyper = newSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_HYPER;
		}
		m_slotSkillAfter.SetData(newSkillTemplet, bIsHyper);
		base.gameObject.SetActive(value: true);
		UIOpened();
		PlayOpenSound();
		PlayOpenAni();
	}

	private void DefaultSetup(float openTime)
	{
		m_fOpenTime = openTime;
		m_fcurrentTime = 0f;
		NKCUtil.SetGameobjectActive(m_objMarkSkillUpgrade, bValue: false);
		NKCUtil.SetGameobjectActive(m_objMarkCityOpen, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSkillSlotRoot, bValue: false);
	}

	private void Update()
	{
		m_fcurrentTime += Time.deltaTime;
		if (m_fcurrentTime > m_fOpenTime)
		{
			m_fcurrentTime = 0f;
			Close();
		}
	}

	private void PlayOpenSound()
	{
		NKCSoundManager.PlaySound("FX_UI_TITLE_START", 1f, 0f, 0f);
	}

	private void PlayOpenAni()
	{
		DOTweenAnimation[] componentsInChildren = GetComponentsInChildren<DOTweenAnimation>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DORestart();
		}
	}

	private void DoKill()
	{
		DOTweenAnimation[] componentsInChildren = GetComponentsInChildren<DOTweenAnimation>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DOKill();
		}
	}

	private void OnDestroy()
	{
		DoKill();
		m_Instance = null;
	}
}
