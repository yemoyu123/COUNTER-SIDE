using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using NKC.UI.NPC;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIBaseSceneMenu : NKCUIBase
{
	public enum BaseSceneMenuType
	{
		None = -1,
		Base,
		Lab,
		Factory,
		Hangar,
		Personnel
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct MenuAniSetting
	{
		public const float BASE_BTN_POS_X = 0f;

		public const float BASE_BTN_HIDE_POS_X = 320f;

		public const float ANIMATION_DELAY_TIME = 0.3f;

		public const float ANIMATION_DELAY_HALF_TIME = 0.15f;

		public const float BUTTON_DELAY_GAP = 0.05f;

		public const float SUB_BUTTON_HIDE_TIME = 0.075f;
	}

	private enum HeadquartersWorkState
	{
		Idle = -1,
		Working,
		Complete
	}

	public const string ASSET_BUNDLE_NAME = "ab_ui_nuf_base";

	public const string UI_ASSET_NAME = "NKM_UI_BASE";

	public RectTransform m_NUM_BASE_BG;

	[Header("본부 메뉴 토글 버튼 설정")]
	public NKCUIComToggle m_LabToggleBtn;

	public NKCUIComToggle m_FactoryToggleBtn;

	public NKCUIComToggle m_HangarToggleBtn;

	public NKCUIComToggle m_PersonnelToggleBtn;

	public NKCUIComToggleGroup m_MenuBtnToggleGroup;

	[Header("버튼 서브 메뉴 설정(주의Base는 0 인덱스에 설정)")]
	public BaseSceneMenuBtn[] m_BaseScenMenuBtn;

	public GameObject m_objFactoryEvent;

	public GameObject m_objPersonnalEvent;

	[Header("기본 메뉴(우측) 배경")]
	[Header("MidCanvas - SpriteRenderer")]
	public SpriteRenderer m_srScenBG;

	[Header("좌측 하단 오브젝트")]
	public GameObject m_NKM_UI_BASE_LEFT;

	public Animator m_Ani_LeftMenu;

	[Header("좌측 이미지")]
	public Image m_ImgMenuTitleDecoTop;

	public Image m_ImgMenuTitleDecoDown;

	[Header("좌측 하단 텍스트")]
	public Text BaseTitle;

	public Text BaseTitleEng;

	public Text SubTitle;

	private Text m_txtNKM_UI_BASE_LEFT_detail;

	private RectTransform m_rtLeftDownMenu;

	[Header("Spine NPC")]
	[Header("Base")]
	public GameObject m_objNPC_Base;

	public GameObject m_NPC_BASE_TouchArea;

	private NKCUINPCManagerKimHaNa m_UINPCBase_KimHaNa;

	[Header("LAB")]
	private NKCUINPCAssistantLeeYoonJung m_UINPC_Lab_Assistant;

	public GameObject m_objNPCLab_Assistant;

	public GameObject m_objNPCLab_Assistant_TouchArea;

	private NKCUINPCProfessorOlivia m_UINPC_Lab_Professor;

	public GameObject m_objNPCLab_Professor;

	public GameObject m_objNPCLab_Professor_TouchArea;

	[Header("Factory")]
	private NKCUINPCFactoryAnastasia m_UINPC_Factory;

	public GameObject m_objNPCFactory;

	public GameObject m_objNPCFactory_TouchArea;

	[Header("Hanger")]
	private NKCUINPCHangarNaHeeRin m_NPCHangar;

	public GameObject m_objNPCHangar;

	public GameObject m_NPCHanger_TouchArea;

	[Header("HR")]
	private NKCUINPCMachineGap m_npcPersonnel;

	public GameObject m_objNPCPersonnel;

	public GameObject m_NPCPersonnel_TouchArea;

	public RectTransform m_NKM_UI_BASE_NPC;

	private const int SUB_MENU_START = 1;

	private const int SUB_MENU_LAB = 1;

	private const int SUB_MENU_FACTORY = 2;

	private const int SUB_MENU_HANGAR = 3;

	private const int SUB_MENU_PERSONNEL = 4;

	private Image m_NKM_UI_BASE_NPC_BG_GLOW_Img;

	private Color m_NKM_UI_BASE_NPC_BG_GLOW_Img_OriginColor;

	private BaseSceneMenuType m_CurMenu;

	private BaseSceneMenuType m_OldMenu;

	private bool bTransition;

	private bool m_IsShortCutOpen;

	private UnityAction<NKC_SCEN_BASE.eUIOpenReserve> m_ReceiveUILoad;

	private UnityAction m_CallUI;

	private MenuAniSetting BaseMenuAnimSetting;

	private const string ANI_INTRO = "Intro";

	private const string ANI_OUTRO = "Outro";

	private const float MENU_CHANGE_ANIMATE_TIME = 0.3f;

	[Header("알람 표시")]
	public GameObject m_objFactoryRedDot;

	public GameObject m_objFactoryCraftRedDot;

	public GameObject m_objHangarRedDot;

	public GameObject m_objHangarBuildRedDot;

	public GameObject m_objPersonnelRedDot;

	public GameObject m_objScoutRedDot;

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.DEFAULT;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCUtilString.GetBaseMenuName(m_CurMenu);

	private string MenuNameEng => NKCUtilString.GetBaseMenuNameEng(m_CurMenu);

	private string SubMenuDetail => NKCUtilString.GetBaseSubMenuDetail(m_CurMenu);

	public override void OnCloseInstance()
	{
		Object.Destroy(m_NKM_UI_BASE_NPC.gameObject);
		Object.Destroy(m_NUM_BASE_BG.gameObject);
	}

	private void Awake()
	{
		InitEvent();
		GameObject gameObject = GameObject.Find("NKM_UI_BASE_LEFT_detail");
		if (gameObject == null)
		{
			Debug.LogError("Can not found GameObject - NKM_UI_BASE_LEFT_detail");
		}
		else
		{
			m_txtNKM_UI_BASE_LEFT_detail = gameObject.GetComponent<Text>();
		}
	}

	private void InitEvent()
	{
		EventTrigger component = GameObject.Find("NUM_BASE_BG").GetComponent<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.Drag;
		entry.callback.AddListener(NKCSystemEvent.UI_SCEN_BG_DRAG);
		component.triggers.Add(entry);
	}

	public void Open(bool bShortCutOpen = false)
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		m_IsShortCutOpen = bShortCutOpen;
		SwitchBackObject(bActive: true);
		SetUnlockContents();
		SetEventObject();
		OpenAnimation(m_CurMenu);
		CloseAllSubMenu();
		if (m_IsShortCutOpen)
		{
			ChangeMainMenu(m_CurMenu);
		}
		UIOpened();
		NotifyCheck();
		NKCUtil.SetGameobjectActive(m_NKM_UI_BASE_LEFT, bValue: false);
		TutorialCheck();
	}

	public override void CloseInternal()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		Debug.LogWarning("CloseInternal");
		SwitchBackObject(bActive: false);
		m_OldMenu = m_CurMenu;
		m_CurMenu = BaseSceneMenuType.Base;
		ChangeMenu(BaseSceneMenuType.Base);
		CloseAllNPC();
		CloseMenuDiscription(m_OldMenu, -1, returnBase: true);
		bTransition = false;
	}

	public override void Hide()
	{
		base.Hide();
		bTransition = false;
		SwitchBackObject(bActive: false, bBGActive: true);
		DisableMenuBtn(m_OldMenu);
	}

	public override void UnHide()
	{
		base.UnHide();
		SwitchBackObject(bActive: true);
		m_IsShortCutOpen = true;
		OpenAnimation(m_CurMenu);
		m_IsShortCutOpen = false;
		NotifyCheck();
		TutorialCheck();
	}

	public override void OnBackButton()
	{
		if (!bTransition)
		{
			if (m_CurMenu != BaseSceneMenuType.Base)
			{
				ChangeMenu(BaseSceneMenuType.Base);
				return;
			}
			bTransition = true;
			CallFuncAfterPlayCloseAnimation(m_CurMenu, ExitMenu, bCloseNPC: false);
		}
	}

	private void SwitchBackObject(bool bActive, bool bBGActive = false)
	{
		if (null != m_NKM_UI_BASE_NPC)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_BASE_NPC, bActive);
		}
		if (null != m_NUM_BASE_BG)
		{
			if (bBGActive)
			{
				NKCUtil.SetGameobjectActive(m_NUM_BASE_BG, bBGActive);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_NUM_BASE_BG, bActive);
			}
		}
	}

	private void ExitMenu()
	{
		if (m_CurMenu == BaseSceneMenuType.Base)
		{
			CloseNPC(m_CurMenu);
		}
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
	}

	public void Init(UnityAction<NKC_SCEN_BASE.eUIOpenReserve> BeginUILoading, UnityAction OpenSubUI)
	{
		m_ReceiveUILoad = BeginUILoading;
		m_CallUI = OpenSubUI;
		InitToggleBtn();
		InitBackground();
		InitNPC();
		if (m_BaseScenMenuBtn == null)
		{
			return;
		}
		for (int i = 0; i < m_BaseScenMenuBtn.Length; i++)
		{
			if (m_BaseScenMenuBtn[i].subBtn == null)
			{
				continue;
			}
			BaseSceneMenuBtn.BaseSceneMenuSubBtn[] subBtn = m_BaseScenMenuBtn[i].subBtn;
			foreach (BaseSceneMenuBtn.BaseSceneMenuSubBtn BaseScenMenuSubBtn in subBtn)
			{
				if (null != BaseScenMenuSubBtn.Btn)
				{
					BaseScenMenuSubBtn.Btn.PointerClick.AddListener(delegate
					{
						EnterSubUI(BaseScenMenuSubBtn.Type);
					});
				}
				if (null != BaseScenMenuSubBtn.LockedBtn)
				{
					BaseScenMenuSubBtn.LockedBtn.PointerClick.AddListener(delegate
					{
						EnterSubUI(BaseScenMenuSubBtn.Type);
					});
					BaseScenMenuSubBtn.LockedBtn.m_bGetCallbackWhileLocked = true;
				}
			}
		}
	}

	private void InitToggleBtn()
	{
		if (null != m_LabToggleBtn)
		{
			m_LabToggleBtn.m_bGetCallbackWhileLocked = true;
			m_LabToggleBtn.OnValueChanged.RemoveAllListeners();
			m_LabToggleBtn.OnValueChanged.AddListener(delegate
			{
				ChangeMenu(BaseSceneMenuType.Lab);
			});
		}
		else
		{
			Debug.LogError("NKCUIBaseSceneMenu - LabToggleBtn is null!");
		}
		if (null != m_FactoryToggleBtn)
		{
			m_FactoryToggleBtn.m_bGetCallbackWhileLocked = true;
			m_FactoryToggleBtn.OnValueChanged.RemoveAllListeners();
			m_FactoryToggleBtn.OnValueChanged.AddListener(delegate
			{
				ChangeMenu(BaseSceneMenuType.Factory);
			});
		}
		else
		{
			Debug.LogError("NKCUIBaseSceneMenu - FactoryToggleBtn is null!");
		}
		if (null != m_HangarToggleBtn)
		{
			m_HangarToggleBtn.m_bGetCallbackWhileLocked = true;
			m_HangarToggleBtn.OnValueChanged.RemoveAllListeners();
			m_HangarToggleBtn.OnValueChanged.AddListener(delegate
			{
				ChangeMenu(BaseSceneMenuType.Hangar);
			});
		}
		else
		{
			Debug.LogError("NKCUIBaseSceneMenu - HangarToggleBtn is null!");
		}
		if (null != m_PersonnelToggleBtn)
		{
			m_PersonnelToggleBtn.m_bGetCallbackWhileLocked = true;
			m_PersonnelToggleBtn.OnValueChanged.RemoveAllListeners();
			m_PersonnelToggleBtn.OnValueChanged.AddListener(delegate
			{
				ChangeMenu(BaseSceneMenuType.Personnel);
			});
		}
		else
		{
			Debug.LogError("NKCUIBaseSceneMenu - PersonnelToggleBtn is null!");
		}
		m_MenuBtnToggleGroup.SetAllToggleUnselected();
	}

	private void InitBackground()
	{
		if (null != m_NUM_BASE_BG)
		{
			m_NUM_BASE_BG.SetParent(NKCUIManager.rectMidCanvas);
			m_NUM_BASE_BG.anchoredPosition3D = new Vector3(0f, 0f, 150f);
			m_NUM_BASE_BG.anchoredPosition = Vector2.zero;
			NKCCamera.RescaleRectToCameraFrustrum(m_NUM_BASE_BG, NKCCamera.GetCamera(), new Vector2(200f, 200f), -1000f);
			m_srScenBG = m_NUM_BASE_BG.gameObject.GetComponentInChildren<SpriteRenderer>();
			NKCUtil.SetGameobjectActive(m_NUM_BASE_BG, bValue: false);
		}
	}

	private void InitNPC()
	{
		if (null != m_NKM_UI_BASE_NPC)
		{
			m_NKM_UI_BASE_NPC.SetParent(NKCUIManager.rectMidCanvas);
			m_NKM_UI_BASE_NPC.anchoredPosition3D = Vector3.zero;
			m_NKM_UI_BASE_NPC.anchoredPosition = Vector2.zero;
			m_NKM_UI_BASE_NPC_BG_GLOW_Img = GameObject.Find("NKM_UI_BASE_NPC_BG_GLOW").gameObject.GetComponent<Image>();
			m_NKM_UI_BASE_NPC_BG_GLOW_Img_OriginColor = m_NKM_UI_BASE_NPC_BG_GLOW_Img.color;
		}
	}

	public void ChangeMenu(BaseSceneMenuType newMenu, bool bReturnIfSameMenu = false)
	{
		if (bReturnIfSameMenu && m_CurMenu == newMenu)
		{
			return;
		}
		if (!IsContentsUnlocked(newMenu))
		{
			ShowLockedMessage(newMenu);
			return;
		}
		m_OldMenu = m_CurMenu;
		if (m_CurMenu == newMenu)
		{
			if (m_CurMenu != BaseSceneMenuType.Base)
			{
				m_CurMenu = BaseSceneMenuType.Base;
				OpenNPC(m_CurMenu);
				ExitSubUI();
				EnableBaseMenu();
				DisableMenuBtn(m_OldMenu);
			}
			return;
		}
		m_CurMenu = newMenu;
		if (m_OldMenu == BaseSceneMenuType.Base)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_BASE_LEFT, bValue: true);
			OpenSubMenuAnimate(m_CurMenu);
			OpenMenuDescription(m_CurMenu);
			CloseNPC(m_OldMenu);
			OpenNPC(m_CurMenu);
			NKCUIManager.UpdateUpsideMenu();
		}
		else if (m_CurMenu == BaseSceneMenuType.Base)
		{
			OpenNPC(m_CurMenu);
			ExitSubUI();
		}
		else
		{
			ChangeSubMenu(m_OldMenu, IsSubChange: true);
			OpenMenuBackGround(newMenu);
		}
	}

	private void EnableBaseMenu()
	{
		m_srScenBG.sprite = m_BaseScenMenuBtn[0].spBackground;
		m_srScenBG.DOColor(Color.white, 0.15f);
		NKCUIManager.UpdateUpsideMenu();
		m_MenuBtnToggleGroup.SetAllToggleUnselected();
	}

	private void EnterSubUI(NKC_SCEN_BASE.eUIOpenReserve Type)
	{
		switch (Type)
		{
		case NKC_SCEN_BASE.eUIOpenReserve.LAB_Train:
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.LAB_TRAINING))
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.LAB_TRAINING);
				return;
			}
			break;
		case NKC_SCEN_BASE.eUIOpenReserve.LAB_Transcendence:
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.LAB_LIMITBREAK))
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.LAB_LIMITBREAK);
				return;
			}
			break;
		case NKC_SCEN_BASE.eUIOpenReserve.Hangar_Build:
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.HANGER_SHIPBUILD))
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.HANGER_SHIPBUILD);
				return;
			}
			break;
		case NKC_SCEN_BASE.eUIOpenReserve.Hangar_Shipyard:
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.HANGER_SHIPYARD))
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.HANGER_SHIPYARD);
				return;
			}
			break;
		case NKC_SCEN_BASE.eUIOpenReserve.Factory_Craft:
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_CRAFT))
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.FACTORY_CRAFT);
				return;
			}
			break;
		case NKC_SCEN_BASE.eUIOpenReserve.Factory_Enchant:
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT))
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.FACTORY_ENCHANT);
				return;
			}
			break;
		case NKC_SCEN_BASE.eUIOpenReserve.Factory_Tunning:
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_TUNING))
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.FACTORY_TUNING);
				return;
			}
			break;
		case NKC_SCEN_BASE.eUIOpenReserve.Personnel_Lifetime:
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.PERSONNAL_LIFETIME))
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.PERSONNAL_LIFETIME);
				return;
			}
			break;
		case NKC_SCEN_BASE.eUIOpenReserve.Personnel_Scout:
			if (!NKCContentManager.IsContentsUnlocked(ContentsType.PERSONNAL_SCOUT))
			{
				NKCContentManager.ShowLockedMessagePopup(ContentsType.PERSONNAL_SCOUT);
				return;
			}
			break;
		}
		bTransition = true;
		m_ReceiveUILoad(Type);
		CallFuncAfterPlayCloseAnimation(m_CurMenu, m_CallUI);
	}

	private void ExitSubUI()
	{
		CloseSubMenuAnimate(m_OldMenu);
		CloseNPC(m_OldMenu);
		CloseMenuDiscription(m_OldMenu, -1, returnBase: true);
	}

	private void OpenAnimation(BaseSceneMenuType newMenu)
	{
		if (m_BaseScenMenuBtn != null)
		{
			if (newMenu == BaseSceneMenuType.Base)
			{
				EnableBaseMenu();
				OpenMenuDescription(newMenu);
				OpenNPC(newMenu);
			}
			else
			{
				OpenMenuBackGround(newMenu);
				OpenMenuDescription(newMenu);
				OpenNPC(newMenu);
				OpenSubMenuAnimate(newMenu);
			}
		}
	}

	private void CloseBaseMenu(bool IsHalfTime = false, bool needDelay = false, UnityAction callBackFunc = null)
	{
		m_BaseScenMenuBtn[0].animator.SetTrigger("Outro");
		if (callBackFunc != null)
		{
			StartCoroutine(waitCallBack(callBackFunc, 0.3f));
		}
	}

	private IEnumerator waitCallBack(UnityAction callBackFunc, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		callBackFunc();
	}

	private void OpenSubMenuAnimate(BaseSceneMenuType newMenu)
	{
		for (int i = 1; i < m_BaseScenMenuBtn.Length; i++)
		{
			BaseSceneMenuBtn baseSceneMenuBtn = m_BaseScenMenuBtn[i];
			if (baseSceneMenuBtn.Type == newMenu)
			{
				NKCUtil.SetGameobjectActive(baseSceneMenuBtn.obj, bValue: true);
				if (baseSceneMenuBtn.animator != null && base.gameObject.activeSelf)
				{
					baseSceneMenuBtn.animator.SetTrigger("Intro");
					Debug.Log("OpenSubMenuAnimate " + newMenu.ToString() + ", Play 'Intro'");
					StartCoroutine(PlayAniDelay(0.3f));
				}
				if (newMenu != BaseSceneMenuType.Base)
				{
					NKCUtil.SetGameobjectActive(baseSceneMenuBtn.obj, bValue: true);
				}
				m_srScenBG.sprite = baseSceneMenuBtn.spBackground;
				break;
			}
		}
	}

	private IEnumerator PlayAniDelay(float time)
	{
		bTransition = true;
		yield return new WaitForSeconds(time);
		bTransition = false;
	}

	private void OpenMenuBackGround(BaseSceneMenuType newMenu)
	{
		for (int i = 1; i < m_BaseScenMenuBtn.Length; i++)
		{
			BaseSceneMenuBtn baseSceneMenuBtn = m_BaseScenMenuBtn[i];
			for (int j = 0; j < baseSceneMenuBtn.subBtn.Length; j++)
			{
				NKCUtil.SetGameobjectActive(baseSceneMenuBtn.subBtn[j].m_objEvent, IsEventEnabled(baseSceneMenuBtn.subBtn[j].Type));
				if (newMenu == baseSceneMenuBtn.Type)
				{
					if (newMenu != BaseSceneMenuType.Base)
					{
						NKCUtil.SetGameobjectActive(baseSceneMenuBtn.obj, bValue: true);
					}
					m_srScenBG.sprite = baseSceneMenuBtn.spBackground;
					m_srScenBG.DOColor(Color.white, 0.15f);
				}
			}
		}
	}

	private bool IsEventEnabled(NKC_SCEN_BASE.eUIOpenReserve type)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			switch (type)
			{
			case NKC_SCEN_BASE.eUIOpenReserve.Factory_Craft:
				return NKCCompanyBuff.NeedShowEventMark(nKMUserData.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_CRAFT_CREDIT_DISCOUNT);
			case NKC_SCEN_BASE.eUIOpenReserve.Factory_Enchant:
			case NKC_SCEN_BASE.eUIOpenReserve.Factory_Tunning:
				return NKCCompanyBuff.NeedShowEventMark(nKMUserData.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT);
			case NKC_SCEN_BASE.eUIOpenReserve.Personnel_Negotiate:
				return NKCCompanyBuff.NeedShowEventMark(nKMUserData.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_PERSONNAL_NEGOTIATION_CREDIT_DISCOUNT);
			}
		}
		return false;
	}

	private void OpenMenuDescription(BaseSceneMenuType newMenu)
	{
		ChangeMenuDiscription(newMenu);
		OpenMenuDiscriptionAnimation(newMenu);
	}

	private void ChangeMenuDiscription(BaseSceneMenuType newMenu)
	{
		if (newMenu != BaseSceneMenuType.Base)
		{
			NKCUtil.SetLabelText(BaseTitle, MenuName);
			NKCUtil.SetLabelText(BaseTitleEng, MenuNameEng);
			NKCUtil.SetLabelText(m_txtNKM_UI_BASE_LEFT_detail, SubMenuDetail);
		}
	}

	private void OpenMenuDiscriptionAnimation(BaseSceneMenuType newMenu)
	{
		if (newMenu != BaseSceneMenuType.Base)
		{
			SwitchLeftMenuAnimation(bIntro: true);
		}
	}

	private void SwitchLeftMenuAnimation(bool bIntro)
	{
		if (bIntro)
		{
			m_Ani_LeftMenu.SetTrigger("Intro");
		}
		else
		{
			m_Ani_LeftMenu.SetTrigger("Outro");
		}
	}

	private void OpenNPC(BaseSceneMenuType newMenu)
	{
		switch (newMenu)
		{
		case BaseSceneMenuType.Base:
			NKCUtil.SetGameobjectActive(m_objNPC_Base, bValue: true);
			AddNPC(BaseSceneMenuType.Base);
			break;
		case BaseSceneMenuType.Lab:
			NKCUtil.SetGameobjectActive(m_objNPCLab_Assistant, bValue: true);
			NKCUtil.SetGameobjectActive(m_objNPCLab_Professor, bValue: true);
			AddNPC(BaseSceneMenuType.Lab);
			break;
		case BaseSceneMenuType.Hangar:
			NKCUtil.SetGameobjectActive(m_objNPCHangar, bValue: true);
			AddNPC(BaseSceneMenuType.Hangar);
			break;
		case BaseSceneMenuType.Personnel:
			NKCUtil.SetGameobjectActive(m_objNPCPersonnel, bValue: true);
			AddNPC(BaseSceneMenuType.Personnel);
			break;
		case BaseSceneMenuType.Factory:
			NKCUtil.SetGameobjectActive(m_objNPCFactory, bValue: true);
			AddNPC(BaseSceneMenuType.Factory);
			break;
		}
		m_NKM_UI_BASE_NPC_BG_GLOW_Img.DOColor(m_NKM_UI_BASE_NPC_BG_GLOW_Img_OriginColor, 0.15f);
	}

	private void CloseNPC(BaseSceneMenuType oldMenu)
	{
		if ((uint)oldMenu <= 4u)
		{
			ReleaseNPC(oldMenu);
		}
		Color nKM_UI_BASE_NPC_BG_GLOW_Img_OriginColor = m_NKM_UI_BASE_NPC_BG_GLOW_Img_OriginColor;
		nKM_UI_BASE_NPC_BG_GLOW_Img_OriginColor.a = 0f;
		m_NKM_UI_BASE_NPC_BG_GLOW_Img.DOColor(nKM_UI_BASE_NPC_BG_GLOW_Img_OriginColor, 0.15f);
	}

	public void ReleaseNPC(BaseSceneMenuType type)
	{
		switch (type)
		{
		case BaseSceneMenuType.Base:
			NKCUtil.SetGameobjectActive(m_objNPC_Base, bValue: false);
			break;
		case BaseSceneMenuType.Lab:
			NKCUtil.SetGameobjectActive(m_objNPCLab_Assistant, bValue: false);
			NKCUtil.SetGameobjectActive(m_objNPCLab_Professor, bValue: false);
			break;
		case BaseSceneMenuType.Factory:
			NKCUtil.SetGameobjectActive(m_objNPCFactory, bValue: false);
			break;
		case BaseSceneMenuType.Hangar:
			NKCUtil.SetGameobjectActive(m_objNPCHangar, bValue: false);
			break;
		case BaseSceneMenuType.Personnel:
			NKCUtil.SetGameobjectActive(m_objNPCPersonnel, bValue: false);
			break;
		}
		RemoveNPC(type);
	}

	private void CloseAllNPC()
	{
		NKCUtil.SetGameobjectActive(m_objNPC_Base, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNPCLab_Assistant, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNPCLab_Professor, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNPCHangar, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNPCPersonnel, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNPCFactory, bValue: false);
		RemoveNPC(BaseSceneMenuType.Base);
		RemoveNPC(BaseSceneMenuType.Lab);
		RemoveNPC(BaseSceneMenuType.Hangar);
		RemoveNPC(BaseSceneMenuType.Personnel);
		RemoveNPC(BaseSceneMenuType.Factory);
		NKCSoundManager.StopAllSound(SOUND_TRACK.VOICE);
	}

	private void CallFuncAfterPlayCloseAnimation(BaseSceneMenuType oldMenu, UnityAction callBackFunc = null, bool bCloseNPC = true)
	{
		if (m_BaseScenMenuBtn != null)
		{
			if (oldMenu == BaseSceneMenuType.Base)
			{
				CloseBaseMenu(IsHalfTime: false, needDelay: true, callBackFunc);
			}
			CloseMenuDiscription(oldMenu);
			if (bCloseNPC)
			{
				CloseNPC(oldMenu);
			}
		}
	}

	private void ChangeMainMenu(BaseSceneMenuType newMenu)
	{
		switch (newMenu)
		{
		case BaseSceneMenuType.Lab:
			m_LabToggleBtn.Select(bSelect: true, bForce: true, bImmediate: true);
			break;
		case BaseSceneMenuType.Factory:
			m_FactoryToggleBtn.Select(bSelect: true, bForce: true, bImmediate: true);
			break;
		case BaseSceneMenuType.Hangar:
			m_HangarToggleBtn.Select(bSelect: true, bForce: true, bImmediate: true);
			break;
		case BaseSceneMenuType.Personnel:
			m_PersonnelToggleBtn.Select(bSelect: true, bForce: true, bImmediate: true);
			break;
		default:
			Debug.LogWarning("ChangeMainMenu - Undefined BaseSceneMenu Type : " + newMenu);
			break;
		}
	}

	private void ChangeSubMenu(BaseSceneMenuType oldMenu, bool IsSubChange = false)
	{
		if (m_BaseScenMenuBtn != null)
		{
			int changedIndex = CloseSubMenuAnimate(oldMenu);
			CloseNPC(oldMenu);
			CloseMenuDiscription(oldMenu, changedIndex);
		}
	}

	private void OpenSubMenu(int targetNum)
	{
		BaseSceneMenuBtn baseSceneMenuBtn = m_BaseScenMenuBtn[targetNum];
		if (targetNum != -1 && baseSceneMenuBtn != null)
		{
			NKCUtil.SetGameobjectActive(baseSceneMenuBtn.obj, bValue: false);
			NKCUIManager.UpdateUpsideMenu();
			OpenSubMenuAnimate(m_CurMenu);
			OpenMenuDescription(m_CurMenu);
			OpenNPC(m_CurMenu);
		}
	}

	private int CloseSubMenuAnimate(BaseSceneMenuType oldMenu)
	{
		int result = -1;
		for (int i = 1; i < m_BaseScenMenuBtn.Length; i++)
		{
			BaseSceneMenuBtn baseSceneMenuBtn = m_BaseScenMenuBtn[i];
			if (oldMenu == baseSceneMenuBtn.Type)
			{
				result = i;
				baseSceneMenuBtn.animator.SetTrigger("Outro");
			}
		}
		return result;
	}

	private void CloseMenuDiscription(BaseSceneMenuType oldMenu, int ChangedIndex = -1, bool returnBase = false)
	{
		if (oldMenu != BaseSceneMenuType.Base)
		{
			if (ChangedIndex != -1)
			{
				PlaySubAni(oldMenu, ChangedIndex);
			}
			else if (returnBase)
			{
				SwitchLeftMenuAnimation(bIntro: false);
				EnableBaseMenu();
			}
			else
			{
				SwitchLeftMenuAnimation(bIntro: false);
			}
		}
	}

	private void PlaySubAni(BaseSceneMenuType oldMenu, int idx)
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(PlaySubAni(oldMenu, idx, 0.15f));
			return;
		}
		for (int i = 0; i < m_BaseScenMenuBtn.Length; i++)
		{
			if (m_BaseScenMenuBtn[i].Type == oldMenu)
			{
				NKCUtil.SetGameobjectActive(m_BaseScenMenuBtn[i].obj, bValue: false);
				break;
			}
		}
		OpenSubMenu(idx);
	}

	private IEnumerator PlaySubAni(BaseSceneMenuType oldMenu, int idx, float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		for (int i = 0; i < m_BaseScenMenuBtn.Length; i++)
		{
			if (m_BaseScenMenuBtn[i].Type == oldMenu)
			{
				NKCUtil.SetGameobjectActive(m_BaseScenMenuBtn[i].obj, bValue: false);
				break;
			}
		}
		OpenSubMenu(idx);
	}

	private void SwitchDecoLine(bool bIn)
	{
		int num;
		int num2;
		if (bIn)
		{
			num = 0;
			num2 = 1;
		}
		else
		{
			num = 1;
			num2 = 0;
		}
		if (null != m_ImgMenuTitleDecoTop)
		{
			m_ImgMenuTitleDecoTop.fillAmount = num;
			m_ImgMenuTitleDecoTop.DOKill();
			m_ImgMenuTitleDecoTop.DOFillAmount(num2, 0.3f).SetEase(Ease.OutCubic);
		}
		if (null != m_ImgMenuTitleDecoDown)
		{
			m_ImgMenuTitleDecoDown.fillAmount = num;
			m_ImgMenuTitleDecoDown.DOKill();
			m_ImgMenuTitleDecoDown.DOFillAmount(num2, 0.3f).SetEase(Ease.OutCubic);
		}
	}

	private void CloseAllSubMenu()
	{
		for (int i = 1; i < m_BaseScenMenuBtn.Length; i++)
		{
			NKCUtil.SetGameobjectActive(m_BaseScenMenuBtn[i].obj, bValue: false);
		}
	}

	private void DisableMenuBtn(BaseSceneMenuType closeType)
	{
		for (int i = 1; i < m_BaseScenMenuBtn.Length; i++)
		{
			BaseSceneMenuBtn baseSceneMenuBtn = m_BaseScenMenuBtn[i];
			if (baseSceneMenuBtn.Type == closeType)
			{
				NKCUtil.SetGameobjectActive(baseSceneMenuBtn.obj, bValue: false);
				break;
			}
		}
	}

	private void AddNPC(BaseSceneMenuType newType)
	{
		bool flag = NKCGameEventManager.IsEventPlaying() || TutorialCheck(play: false);
		switch (newType)
		{
		case BaseSceneMenuType.Base:
			if (m_UINPCBase_KimHaNa == null)
			{
				m_UINPCBase_KimHaNa = m_NPC_BASE_TouchArea.GetComponent<NKCUINPCManagerKimHaNa>();
				m_UINPCBase_KimHaNa.Init();
			}
			m_UINPCBase_KimHaNa.PlayAni(NPC_ACTION_TYPE.ENTER_BASE, m_IsShortCutOpen || flag);
			break;
		case BaseSceneMenuType.Lab:
			if (m_UINPC_Lab_Assistant == null)
			{
				m_UINPC_Lab_Assistant = m_objNPCLab_Assistant_TouchArea.GetComponent<NKCUINPCAssistantLeeYoonJung>();
				m_UINPC_Lab_Assistant.Init();
			}
			m_UINPC_Lab_Assistant.PlayAni(NPC_ACTION_TYPE.START, m_IsShortCutOpen || flag);
			if (m_UINPC_Lab_Professor == null)
			{
				m_UINPC_Lab_Professor = m_objNPCLab_Professor_TouchArea.GetComponent<NKCUINPCProfessorOlivia>();
				m_UINPC_Lab_Professor.Init();
			}
			m_UINPC_Lab_Professor.PlayAni(NPC_ACTION_TYPE.START, m_IsShortCutOpen || flag);
			break;
		case BaseSceneMenuType.Factory:
			if (m_UINPC_Factory == null)
			{
				m_UINPC_Factory = m_objNPCFactory_TouchArea.GetComponent<NKCUINPCFactoryAnastasia>();
				m_UINPC_Factory.Init();
			}
			m_UINPC_Factory.PlayAni(NPC_ACTION_TYPE.START, m_IsShortCutOpen || flag);
			break;
		case BaseSceneMenuType.Personnel:
			if (m_npcPersonnel == null)
			{
				m_npcPersonnel = m_NPCPersonnel_TouchArea.GetComponent<NKCUINPCMachineGap>();
				m_npcPersonnel.Init();
			}
			m_npcPersonnel.PlayAni(NPC_ACTION_TYPE.START, m_IsShortCutOpen || flag);
			break;
		case BaseSceneMenuType.Hangar:
			if (m_NPCHangar == null)
			{
				m_NPCHangar = m_NPCHanger_TouchArea.GetComponent<NKCUINPCHangarNaHeeRin>();
				m_NPCHangar.Init();
			}
			m_NPCHangar.PlayAni(NPC_ACTION_TYPE.START, m_IsShortCutOpen || flag);
			break;
		}
	}

	private void RemoveNPC(BaseSceneMenuType newType)
	{
		switch (newType)
		{
		case BaseSceneMenuType.Base:
			if (m_UINPCBase_KimHaNa != null)
			{
				m_UINPCBase_KimHaNa = null;
			}
			break;
		case BaseSceneMenuType.Lab:
			if (m_UINPC_Lab_Assistant != null)
			{
				m_UINPC_Lab_Assistant = null;
			}
			if (m_UINPC_Lab_Professor != null)
			{
				m_UINPC_Lab_Professor = null;
			}
			break;
		case BaseSceneMenuType.Factory:
			if (m_UINPC_Factory != null)
			{
				m_UINPC_Factory = null;
			}
			break;
		case BaseSceneMenuType.Personnel:
			if (m_npcPersonnel != null)
			{
				m_npcPersonnel = null;
			}
			break;
		case BaseSceneMenuType.Hangar:
			if (m_NPCHangar != null)
			{
				m_NPCHangar = null;
			}
			break;
		}
	}

	private void SetEventObject()
	{
		NKCUtil.SetGameobjectActive(m_objFactoryEvent, NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_CRAFT_CREDIT_DISCOUNT) || NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT) || NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_POTENTIAL_SOCKET_CREDIT_DISCOUNT));
		NKCUtil.SetGameobjectActive(m_objPersonnalEvent, NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_PERSONNAL_NEGOTIATION_CREDIT_DISCOUNT));
		for (int i = 1; i < m_BaseScenMenuBtn.Length; i++)
		{
			BaseSceneMenuBtn baseSceneMenuBtn = m_BaseScenMenuBtn[i];
			for (int j = 0; j < baseSceneMenuBtn.subBtn.Length; j++)
			{
				NKCUtil.SetGameobjectActive(baseSceneMenuBtn.subBtn[j].m_objEvent, IsEventEnabled(baseSceneMenuBtn.subBtn[j].Type));
			}
		}
	}

	private void SetUnlockContents()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_LAB))
		{
			m_LabToggleBtn.Lock();
		}
		else
		{
			m_LabToggleBtn.UnLock();
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_FACTORY))
		{
			m_FactoryToggleBtn.Lock();
		}
		else
		{
			m_FactoryToggleBtn.UnLock();
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_HANGAR))
		{
			m_HangarToggleBtn.Lock();
		}
		else
		{
			m_HangarToggleBtn.UnLock();
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_PERSONNAL))
		{
			m_PersonnelToggleBtn.Lock();
		}
		else
		{
			m_PersonnelToggleBtn.UnLock();
		}
		for (int i = 1; i < m_BaseScenMenuBtn.Length; i++)
		{
			if (m_BaseScenMenuBtn[i] == null)
			{
				continue;
			}
			for (int j = 0; j < m_BaseScenMenuBtn[i].subBtn.Length; j++)
			{
				if (m_BaseScenMenuBtn[i].subBtn[j] != null && !(m_BaseScenMenuBtn[i].subBtn[j].Btn == null))
				{
					ContentsType contentsType = ContentsType.None;
					switch (m_BaseScenMenuBtn[i].subBtn[j].Type)
					{
					case NKC_SCEN_BASE.eUIOpenReserve.LAB_Enchant:
						contentsType = ContentsType.BASE_LAB;
						break;
					case NKC_SCEN_BASE.eUIOpenReserve.LAB_Train:
						contentsType = ContentsType.LAB_TRAINING;
						break;
					case NKC_SCEN_BASE.eUIOpenReserve.LAB_Transcendence:
						contentsType = ContentsType.LAB_LIMITBREAK;
						break;
					case NKC_SCEN_BASE.eUIOpenReserve.Hangar_Build:
						contentsType = ContentsType.HANGER_SHIPBUILD;
						break;
					case NKC_SCEN_BASE.eUIOpenReserve.Hangar_Shipyard:
						contentsType = ContentsType.HANGER_SHIPYARD;
						break;
					case NKC_SCEN_BASE.eUIOpenReserve.Factory_Craft:
						contentsType = ContentsType.FACTORY_CRAFT;
						break;
					case NKC_SCEN_BASE.eUIOpenReserve.Factory_Enchant:
						contentsType = ContentsType.FACTORY_ENCHANT;
						break;
					case NKC_SCEN_BASE.eUIOpenReserve.Factory_Tunning:
						contentsType = ContentsType.FACTORY_TUNING;
						break;
					case NKC_SCEN_BASE.eUIOpenReserve.Personnel_Lifetime:
						contentsType = ContentsType.PERSONNAL_LIFETIME;
						break;
					case NKC_SCEN_BASE.eUIOpenReserve.Personnel_Scout:
						contentsType = ContentsType.PERSONNAL_SCOUT;
						break;
					}
					if (!NKCContentManager.IsContentsUnlocked(contentsType))
					{
						m_BaseScenMenuBtn[i].subBtn[j].Btn.Lock();
						NKCUtil.SetGameobjectActive(m_BaseScenMenuBtn[i].subBtn[j].Btn, bValue: false);
						NKCUtil.SetGameobjectActive(m_BaseScenMenuBtn[i].subBtn[j].LockedBtn, bValue: true);
					}
					else
					{
						m_BaseScenMenuBtn[i].subBtn[j].Btn.UnLock();
						NKCUtil.SetGameobjectActive(m_BaseScenMenuBtn[i].subBtn[j].Btn, bValue: true);
						NKCUtil.SetGameobjectActive(m_BaseScenMenuBtn[i].subBtn[j].LockedBtn, bValue: false);
					}
				}
			}
		}
	}

	private bool IsContentsUnlocked(BaseSceneMenuType menuType)
	{
		switch (menuType)
		{
		case BaseSceneMenuType.Lab:
			return NKCContentManager.IsContentsUnlocked(ContentsType.BASE_LAB);
		case BaseSceneMenuType.Factory:
			return NKCContentManager.IsContentsUnlocked(ContentsType.BASE_FACTORY);
		case BaseSceneMenuType.Hangar:
			return NKCContentManager.IsContentsUnlocked(ContentsType.BASE_HANGAR);
		case BaseSceneMenuType.Personnel:
			return NKCContentManager.IsContentsUnlocked(ContentsType.BASE_PERSONNAL);
		default:
			Debug.LogWarning("IsContentsUnlocked - Undefined BaseSceneMenu Type : " + menuType);
			return true;
		}
	}

	private void ShowLockedMessage(BaseSceneMenuType menuType)
	{
		switch (menuType)
		{
		case BaseSceneMenuType.Lab:
			NKCContentManager.ShowLockedMessagePopup(ContentsType.BASE_LAB);
			break;
		case BaseSceneMenuType.Factory:
			NKCContentManager.ShowLockedMessagePopup(ContentsType.BASE_FACTORY);
			break;
		case BaseSceneMenuType.Hangar:
			NKCContentManager.ShowLockedMessagePopup(ContentsType.BASE_HANGAR);
			break;
		case BaseSceneMenuType.Personnel:
			NKCContentManager.ShowLockedMessagePopup(ContentsType.BASE_PERSONNAL);
			break;
		default:
			Debug.LogWarning("ShowLockedMessage - Undefined BaseSceneMenu Type : " + menuType);
			break;
		}
	}

	private void NotifyCheck()
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			HeadquartersWorkState headquartersWorkState = CheckNewUnlockShipBuild();
			NKCUtil.SetGameobjectActive(m_objHangarRedDot, headquartersWorkState == HeadquartersWorkState.Complete);
			NKCUtil.SetGameobjectActive(m_objHangarBuildRedDot, headquartersWorkState == HeadquartersWorkState.Complete);
			HeadquartersWorkState headquartersWorkState2 = CheckEquipCreationState(nKMUserData.m_CraftData);
			NKCUtil.SetGameobjectActive(m_objFactoryRedDot, headquartersWorkState2 == HeadquartersWorkState.Complete);
			NKCUtil.SetGameobjectActive(m_objFactoryCraftRedDot, headquartersWorkState2 == HeadquartersWorkState.Complete);
			bool bValue = NKCAlarmManager.CheckScoutNotify(nKMUserData);
			NKCUtil.SetGameobjectActive(m_objPersonnelRedDot, bValue);
			NKCUtil.SetGameobjectActive(m_objScoutRedDot, bValue);
		}
	}

	private HeadquartersWorkState CheckNewUnlockShipBuild()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.HANGER_SHIPBUILD))
		{
			return HeadquartersWorkState.Idle;
		}
		HeadquartersWorkState result = HeadquartersWorkState.Idle;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			foreach (NKMShipBuildTemplet value in NKMTempletContainer<NKMShipBuildTemplet>.Values)
			{
				if (value.ShipBuildUnlockType != NKMShipBuildTemplet.BuildUnlockType.BUT_UNABLE && NKMShipManager.CanUnlockShip(nKMUserData, value) && !PlayerPrefs.HasKey(string.Format("{0}_{1}_{2}", "SHIP_BUILD_SLOT_CHECK", nKMUserData.m_UserUID, value.ShipID)))
				{
					result = HeadquartersWorkState.Complete;
					break;
				}
			}
		}
		return result;
	}

	private HeadquartersWorkState CheckEquipCreationState(NKMCraftData creationData)
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_FACTORY))
		{
			return HeadquartersWorkState.Idle;
		}
		HeadquartersWorkState result = HeadquartersWorkState.Idle;
		foreach (KeyValuePair<byte, NKMCraftSlotData> slot in creationData.SlotList)
		{
			if (slot.Value.GetState(NKCSynchronizedTime.GetServerUTCTime()) == NKM_CRAFT_SLOT_STATE.NECSS_COMPLETED)
			{
				return HeadquartersWorkState.Complete;
			}
			if (slot.Value.GetState(NKCSynchronizedTime.GetServerUTCTime()) == NKM_CRAFT_SLOT_STATE.NECSS_CREATING_NOW)
			{
				result = HeadquartersWorkState.Working;
			}
		}
		return result;
	}

	private bool TutorialCheck(bool play = true)
	{
		return NKCTutorialManager.TutorialRequired(TutorialPoint.Base, play) != TutorialStep.None;
	}
}
