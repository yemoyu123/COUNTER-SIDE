using System.Collections.Generic;
using System.Text;
using NKC.Publisher;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace NKC.UI;

public class NKCUIGameResultGetUnit : NKCUIBase
{
	public enum Type
	{
		Get,
		LimitBreak,
		Ship,
		Skin
	}

	public delegate void NKCUIGRGetUnitCallBack();

	public enum eUnitTagType
	{
		None,
		GetUnit,
		NewUnit,
		LimitBreak,
		Transcendence,
		GetSkin,
		ShipUpgrade
	}

	public struct GetUnitResultData
	{
		public enum eMode
		{
			GetUnit,
			GetSkin
		}

		public int m_UnitID;

		public int m_SkinID;

		public long m_UnitUID;

		public int m_LimitBreakLevel;

		public int limitBreakStarCount;

		public int StarMaxCount;

		public bool m_bSkin;

		public GetUnitResultData(NKMUnitData unitData)
		{
			m_UnitID = unitData.m_UnitID;
			m_SkinID = unitData.m_SkinID;
			m_LimitBreakLevel = unitData.m_LimitBreakLevel;
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
			NKM_UNIT_TYPE nKM_UNIT_TYPE = unitTempletBase.m_NKM_UNIT_TYPE;
			if (nKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_NORMAL && nKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
			{
				limitBreakStarCount = unitData.GetStarGrade();
				StarMaxCount = 6;
			}
			else
			{
				limitBreakStarCount = unitData.GetStarGrade(unitTempletBase);
				StarMaxCount = unitTempletBase.m_StarGradeMax;
			}
			m_bSkin = false;
			m_UnitUID = unitData.m_UnitUID;
		}

		public GetUnitResultData(NKMOperator operatorData)
		{
			m_UnitID = operatorData.id;
			m_SkinID = 0;
			m_LimitBreakLevel = 3;
			limitBreakStarCount = -1;
			StarMaxCount = -1;
			m_bSkin = false;
			m_UnitUID = operatorData.uid;
		}

		public GetUnitResultData(NKMSkinTemplet skinTemplet)
		{
			m_UnitID = skinTemplet.m_SkinEquipUnitID;
			m_SkinID = skinTemplet.m_SkinID;
			m_LimitBreakLevel = 3;
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(skinTemplet.m_SkinEquipUnitID);
			StarMaxCount = unitTempletBase.m_StarGradeMax;
			limitBreakStarCount = unitTempletBase.m_StarGradeMax;
			m_bSkin = true;
			m_UnitUID = 0L;
		}

		public static List<GetUnitResultData> ConvertList(NKMRewardData rewardData)
		{
			if (rewardData == null)
			{
				return new List<GetUnitResultData>();
			}
			return ConvertList(rewardData.UnitDataList, rewardData.OperatorList, rewardData.SkinIdList);
		}

		public static List<GetUnitResultData> ConvertList(List<NKMRewardData> lstRewardData)
		{
			if (lstRewardData == null)
			{
				return new List<GetUnitResultData>();
			}
			List<GetUnitResultData> list = new List<GetUnitResultData>();
			foreach (NKMRewardData lstRewardDatum in lstRewardData)
			{
				list.AddRange(ConvertList(lstRewardDatum));
			}
			return list;
		}

		public static List<GetUnitResultData> ConvertList(IEnumerable<NKMUnitData> lstUnitData, IEnumerable<NKMOperator> lstOperator, IEnumerable<int> lstSkinID)
		{
			List<GetUnitResultData> list = new List<GetUnitResultData>();
			if (lstUnitData != null)
			{
				foreach (NKMUnitData lstUnitDatum in lstUnitData)
				{
					list.Add(new GetUnitResultData(lstUnitDatum));
				}
			}
			if (lstOperator != null)
			{
				foreach (NKMOperator item in lstOperator)
				{
					list.Add(new GetUnitResultData(item));
				}
			}
			if (lstSkinID != null)
			{
				foreach (int item2 in lstSkinID)
				{
					NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(item2);
					if (skinTemplet != null)
					{
						list.Add(new GetUnitResultData(skinTemplet));
					}
				}
			}
			return list;
		}
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_result";

	private const string UI_ASSET_NAME = "NKM_UI_RESULT_GET_UNIT";

	private static NKCUIGameResultGetUnit m_Instance;

	[Header("Sound")]
	public GameObject m_SFX_STAR;

	public GameObject m_SFX_STAR_SSR;

	public GameObject m_SFX_STAR_OTHER;

	public Animator m_animGetUnit;

	public EventTrigger m_evtScreen;

	[Header("유닛 기본정보")]
	public List<GameObject> m_lstUnitGradeStarOFF;

	public List<GameObject> m_lstUnitGradeStar;

	public List<NKCUIComResultStar> m_lstResultStar;

	public Text m_lbTitle;

	public Text m_lbName;

	public Text m_lbNameShadow;

	public List<GameObject> m_lstStar;

	public GameObject m_objSilhouetCounter;

	public GameObject m_objSilhouetMarkSoldier;

	public GameObject m_objSilhouetMarkMech;

	public GameObject m_objSilhouetMarkShip;

	public List<GameObject> m_lstUnitRank;

	public List<GameObject> m_lstUnitGradeFX;

	public List<GameObject> m_lstUnitLiiustFX;

	public GameObject m_objRankAwakenSR;

	public GameObject m_objRankAwakenSSR;

	public GameObject m_objBGAwakenSR;

	public GameObject m_objBGAwakenSSR;

	public GameObject m_NKM_UI_RESULT_GET_UNIT_GET;

	public GameObject m_NKM_UI_RESULT_GET_UNIT_NEW;

	public GameObject m_objUnitLimitBreak;

	public GameObject m_objUnitTranscendence;

	public GameObject m_objGetSkin;

	public GameObject m_NKM_UI_RESULT_GET_SHIP_UPGRADE;

	[Header("초월 관련")]
	public GameObject m_NKM_UI_RESULT_GET_UNIT_TRANSCENDENCE_INFO;

	public GameObject m_LimitBreakTextRoot;

	public Text m_lbMAXLEVEL_COUNT;

	public Text m_lbSHIP_MAXLEVEL_COUNT;

	public Text m_lbTRANSCENDENCE_TEXT1;

	public GameObject m_TRANSCENDENCE_TEXT2;

	[Header("초월각성 관련")]
	public GameObject m_objTranscendenceTextRoot;

	public Text m_lbTCCount;

	public Text m_lbTCMaxLevel;

	public Text m_lbTCGrowRate;

	[Header("그외")]
	public Image m_NKM_UI_RESULT_GET_UNIT_MARK;

	public Text m_lbNKM_UI_RESULT_GET_UNIT_SUMMARY_STYLE_NAME;

	public GameObject m_NKM_UI_RESULT_GET_UNIT_SUMMARY_CLASS;

	public Image m_NKM_UI_RESULT_GET_UNIT_SUMMARY_CLASS_ICON;

	public Text m_NKM_UI_RESULT_GET_UNIT_SUMMARY_CLASS_TEXT;

	public GameObject m_NKM_UI_RESULT_GET_SHIP_UPGRADE_SKILL;

	public List<Image> m_lstSkill;

	public List<Image> m_lstNewSkill;

	public Text m_lbDailoogue;

	public NKCUIComStateButton m_btnSkip;

	public GameObject m_objGameOpenAwakenFX;

	[Header("캐릭터 스파인 일러스트")]
	public Transform m_trRootSpineIllust;

	public Transform m_trRootSpineShip;

	public RectTransform m_rtSPINEAREA;

	public RectTransform m_rtSPINEAREA_SHIP;

	[Header("비디오 플레이 안 될때 대비용 배경")]
	public GameObject m_objFallbackBG;

	[Header("공유 버튼")]
	public NKCUIComStateButton m_csbtnShare;

	public GameObject m_objFacebookMark;

	private NKCUIGRGetUnitCallBack m_NKCUIGRGetUnitCallBack;

	private List<GetUnitResultData> m_listUnitData;

	private int m_CurrIdx;

	private bool m_bEnableNextChar = true;

	private float m_fTime;

	private float m_fTimeAutoSkip;

	private bool m_bCheckTimeAutoSkip;

	private bool m_bEnableTimeAutoSkip;

	private const float SKIP_INTERVAL_TIME = 4f;

	private bool m_bPlayVoice;

	private Type m_Type;

	private static HashSet<int> m_setFirstGetUnit = new HashSet<int>();

	private GetUnitResultData m_CurrentUnitResultData;

	private NKMUnitTempletBase m_NKMUnitTempletBase;

	private int m_skinID;

	private List<GameObject> m_lstStarOFF = new List<GameObject>();

	private List<GameObject> m_lstStarONParent = new List<GameObject>();

	private bool m_bForceHideGetUnitMark;

	private NKCASUIUnitIllust m_NKCASUISpineIllust;

	private NKCASUIUnitIllust m_NKCASUIBlackSpineIllust;

	private int m_iCurSkillCnt;

	private int m_iCurMaxShipLv;

	private int m_iNextMaxShipLv;

	public static NKCUIGameResultGetUnit Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIGameResultGetUnit>("ab_ui_nkm_ui_result", "NKM_UI_RESULT_GET_UNIT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIGameResultGetUnit>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override string MenuName => NKCUtilString.GET_STRING_GET_UNIT;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void OnDestroy()
	{
		ClearIllustMem();
		m_Instance = null;
	}

	public static void ShowNewSkinGetUI(HashSet<int> hsSkinID, NKCUIGRGetUnitCallBack callBack, bool bEnableAutoSkip = false, bool bSkipDuplicateNormalUnit = false)
	{
		List<GetUnitResultData> list = new List<GetUnitResultData>();
		foreach (int item in hsSkinID)
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(item);
			if (skinTemplet == null)
			{
				return;
			}
			list.Add(new GetUnitResultData(skinTemplet));
		}
		Instance.Open(list, callBack, bEnableAutoSkip, bUseDefaultSort: true, bSkipDuplicateNormalUnit, Type.Skin);
	}

	public static void ShowNewUnitGetUI(NKMRewardData rewardData, NKCUIGRGetUnitCallBack callBack, bool bEnableAutoSkip = false, bool bUseDefaultSort = true, bool bSkipDuplicateNormalUnit = false)
	{
		Instance.Open(GetUnitResultData.ConvertList(rewardData), callBack, bEnableAutoSkip, bUseDefaultSort, bSkipDuplicateNormalUnit);
	}

	public static void ShowNewUnitGetUI(List<NKMRewardData> lstRewardData, NKCUIGRGetUnitCallBack callBack, bool bEnableAutoSkip = false, bool bUseDefaultSort = true, bool bSkipDuplicateNormalUnit = false)
	{
		Instance.Open(GetUnitResultData.ConvertList(lstRewardData), callBack, bEnableAutoSkip, bUseDefaultSort, bSkipDuplicateNormalUnit);
	}

	public static void ShowNewUnitGetUIForSelectableContract(List<NKMUnitData> lstUnitData, NKCUIGRGetUnitCallBack callBack)
	{
		Instance.m_bForceHideGetUnitMark = true;
		Instance.Open(GetUnitResultData.ConvertList(lstUnitData, null, null), callBack);
	}

	public static void ShowUnitTranscendence(NKMUnitData unitData, NKCUIGRGetUnitCallBack callBack = null)
	{
		Instance.Open(new GetUnitResultData(unitData), callBack, bEnableAutoSkip: false, bUseDefaultSort: false, bSkipDuplicateNormalUnit: false, Type.LimitBreak);
	}

	public static void ShowShipTranscendence(NKMUnitData shipData, int curSkillCnt, int curMaxLv, int nextMaxLv)
	{
		Instance.SetShipData(curSkillCnt, curMaxLv, nextMaxLv);
		Instance.Open(new GetUnitResultData(shipData), null, bEnableAutoSkip: false, bUseDefaultSort: false, bSkipDuplicateNormalUnit: false, Type.Ship);
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
			m_Instance.InvokeCallback();
		}
	}

	public void InitUI()
	{
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback.AddListener(delegate
		{
			OnTouchAnywhere();
		});
		m_evtScreen.triggers.Clear();
		m_evtScreen.triggers.Add(entry);
		base.gameObject.SetActive(value: false);
		m_bEnableNextChar = true;
		m_lstStarOFF.Clear();
		m_lstStarOFF.Add(base.gameObject.transform.Find("Canvas_INFO/NKM_UI_RESULT_GET_UNIT_SUMMARY_Panel/NKM_UI_RESULT_GET_UNIT_NAME/NKM_UI_RESULT_GET_UNIT_STAR/NKM_UI_RESULT_GET_UNIT_INFO_STAR_OFF/NKM_STAR0").gameObject);
		m_lstStarOFF.Add(base.gameObject.transform.Find("Canvas_INFO/NKM_UI_RESULT_GET_UNIT_SUMMARY_Panel/NKM_UI_RESULT_GET_UNIT_NAME/NKM_UI_RESULT_GET_UNIT_STAR/NKM_UI_RESULT_GET_UNIT_INFO_STAR_OFF/NKM_STAR1").gameObject);
		m_lstStarOFF.Add(base.gameObject.transform.Find("Canvas_INFO/NKM_UI_RESULT_GET_UNIT_SUMMARY_Panel/NKM_UI_RESULT_GET_UNIT_NAME/NKM_UI_RESULT_GET_UNIT_STAR/NKM_UI_RESULT_GET_UNIT_INFO_STAR_OFF/NKM_STAR2").gameObject);
		m_lstStarOFF.Add(base.gameObject.transform.Find("Canvas_INFO/NKM_UI_RESULT_GET_UNIT_SUMMARY_Panel/NKM_UI_RESULT_GET_UNIT_NAME/NKM_UI_RESULT_GET_UNIT_STAR/NKM_UI_RESULT_GET_UNIT_INFO_STAR_OFF/NKM_STAR3").gameObject);
		m_lstStarOFF.Add(base.gameObject.transform.Find("Canvas_INFO/NKM_UI_RESULT_GET_UNIT_SUMMARY_Panel/NKM_UI_RESULT_GET_UNIT_NAME/NKM_UI_RESULT_GET_UNIT_STAR/NKM_UI_RESULT_GET_UNIT_INFO_STAR_OFF/NKM_STAR4").gameObject);
		m_lstStarOFF.Add(base.gameObject.transform.Find("Canvas_INFO/NKM_UI_RESULT_GET_UNIT_SUMMARY_Panel/NKM_UI_RESULT_GET_UNIT_NAME/NKM_UI_RESULT_GET_UNIT_STAR/NKM_UI_RESULT_GET_UNIT_INFO_STAR_OFF/NKM_STAR5").gameObject);
		m_lstStarONParent.Clear();
		m_lstStarONParent.Add(base.gameObject.transform.Find("Canvas_INFO/NKM_UI_RESULT_GET_UNIT_SUMMARY_Panel/NKM_UI_RESULT_GET_UNIT_NAME/NKM_UI_RESULT_GET_UNIT_STAR/NKM_UI_RESULT_GET_UNIT_INFO_STAR_AFTER/NKM_STAR0").gameObject);
		m_lstStarONParent.Add(base.gameObject.transform.Find("Canvas_INFO/NKM_UI_RESULT_GET_UNIT_SUMMARY_Panel/NKM_UI_RESULT_GET_UNIT_NAME/NKM_UI_RESULT_GET_UNIT_STAR/NKM_UI_RESULT_GET_UNIT_INFO_STAR_AFTER/NKM_STAR1").gameObject);
		m_lstStarONParent.Add(base.gameObject.transform.Find("Canvas_INFO/NKM_UI_RESULT_GET_UNIT_SUMMARY_Panel/NKM_UI_RESULT_GET_UNIT_NAME/NKM_UI_RESULT_GET_UNIT_STAR/NKM_UI_RESULT_GET_UNIT_INFO_STAR_AFTER/NKM_STAR2").gameObject);
		m_lstStarONParent.Add(base.gameObject.transform.Find("Canvas_INFO/NKM_UI_RESULT_GET_UNIT_SUMMARY_Panel/NKM_UI_RESULT_GET_UNIT_NAME/NKM_UI_RESULT_GET_UNIT_STAR/NKM_UI_RESULT_GET_UNIT_INFO_STAR_AFTER/NKM_STAR3").gameObject);
		m_lstStarONParent.Add(base.gameObject.transform.Find("Canvas_INFO/NKM_UI_RESULT_GET_UNIT_SUMMARY_Panel/NKM_UI_RESULT_GET_UNIT_NAME/NKM_UI_RESULT_GET_UNIT_STAR/NKM_UI_RESULT_GET_UNIT_INFO_STAR_AFTER/NKM_STAR4").gameObject);
		m_lstStarONParent.Add(base.gameObject.transform.Find("Canvas_INFO/NKM_UI_RESULT_GET_UNIT_SUMMARY_Panel/NKM_UI_RESULT_GET_UNIT_NAME/NKM_UI_RESULT_GET_UNIT_STAR/NKM_UI_RESULT_GET_UNIT_INFO_STAR_AFTER/NKM_STAR5").gameObject);
		m_btnSkip.PointerClick.RemoveAllListeners();
		m_btnSkip.PointerClick.AddListener(SkipAll);
		AnimationClip[] animationClips = m_animGetUnit.runtimeAnimatorController.animationClips;
		if (animationClips != null)
		{
			foreach (AnimationClip animationClip in animationClips)
			{
				if (string.Equals(animationClip.name, "NKM_UI_RESULT_GET_UNIT_TOP_BG_ON"))
				{
					AnimationEvent animationEvent = new AnimationEvent();
					animationEvent.functionName = "ActiveSFXStar";
					animationEvent.time = 0f;
					animationClip.AddEvent(animationEvent);
					break;
				}
			}
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnShare, OnShare);
	}

	public void ActiveSFXStar()
	{
		NKCUtil.SetGameobjectActive(m_SFX_STAR, bValue: true);
		NKCSoundManager.PlaySound("FX_UI_UNIT_GET_OPEN", 1f, 0f, 0f);
	}

	private void DeactiveSFXStar()
	{
		NKCUtil.SetGameobjectActive(m_SFX_STAR, bValue: false);
	}

	public void Open(GetUnitResultData getUnitData, NKCUIGRGetUnitCallBack _NKCUIGRGetUnitCallBack = null, bool bEnableAutoSkip = false, bool bUseDefaultSort = true, bool bSkipDuplicateNormalUnit = false, Type type = Type.Get)
	{
		List<GetUnitResultData> list = new List<GetUnitResultData>();
		list.Add(getUnitData);
		Open(list, _NKCUIGRGetUnitCallBack, bEnableAutoSkip, bUseDefaultSort, bSkipDuplicateNormalUnit, type);
	}

	public void Open(List<GetUnitResultData> listUnitData, NKCUIGRGetUnitCallBack _NKCUIGRGetUnitCallBack = null, bool bEnableAutoSkip = false, bool bUseDefaultSort = true, bool bSkipDuplicateNormalUnit = false, Type type = Type.Get)
	{
		if (listUnitData == null || listUnitData.Count <= 0)
		{
			_NKCUIGRGetUnitCallBack?.Invoke();
			return;
		}
		if (bSkipDuplicateNormalUnit)
		{
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			List<GetUnitResultData> list = new List<GetUnitResultData>();
			foreach (GetUnitResultData listUnitDatum in listUnitData)
			{
				if (HaveFirstGetUnit(listUnitDatum.m_UnitID) || false != nKMUserData?.m_ArmyData.IsFirstGetUnit(listUnitDatum.m_UnitID) || NKMUnitManager.GetUnitTempletBase(listUnitDatum.m_UnitID).m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SSR)
				{
					list.Add(listUnitDatum);
				}
			}
			if (list.Count == 0)
			{
				_NKCUIGRGetUnitCallBack?.Invoke();
				return;
			}
			m_listUnitData = list;
		}
		else
		{
			m_listUnitData = listUnitData;
		}
		m_bEnableTimeAutoSkip = bEnableAutoSkip;
		m_bCheckTimeAutoSkip = false;
		m_Type = type;
		if (bUseDefaultSort)
		{
			m_listUnitData.Sort(DefaultUnitSort);
		}
		m_CurrIdx = 0;
		m_NKCUIGRGetUnitCallBack = _NKCUIGRGetUnitCallBack;
		base.gameObject.SetActive(value: true);
		UIOpened();
		SetBG();
		m_bEnableNextChar = true;
		ShowNext();
	}

	private int DefaultUnitSort(GetUnitResultData A, GetUnitResultData B)
	{
		if (A.m_bSkin && B.m_bSkin)
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(A.m_SkinID);
			return NKMSkinManager.GetSkinTemplet(B.m_SkinID).m_SkinGrade.CompareTo(skinTemplet.m_SkinGrade);
		}
		if (A.m_bSkin != B.m_bSkin)
		{
			return B.m_bSkin.CompareTo(A.m_bSkin);
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(A.m_UnitID);
		NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(B.m_UnitID);
		if (unitTempletBase.m_bAwaken != unitTempletBase2.m_bAwaken)
		{
			return unitTempletBase2.m_bAwaken.CompareTo(unitTempletBase.m_bAwaken);
		}
		if (unitTempletBase.IsRearmUnit != unitTempletBase2.IsRearmUnit)
		{
			return unitTempletBase2.IsRearmUnit.CompareTo(unitTempletBase.IsRearmUnit);
		}
		if (unitTempletBase.m_NKM_UNIT_GRADE != unitTempletBase2.m_NKM_UNIT_GRADE)
		{
			return unitTempletBase2.m_NKM_UNIT_GRADE.CompareTo(unitTempletBase.m_NKM_UNIT_GRADE);
		}
		return A.m_UnitID.CompareTo(B.m_UnitID);
	}

	private void SetAutoSkipTimeStamp()
	{
		if (m_bEnableTimeAutoSkip)
		{
			m_fTimeAutoSkip = Time.time;
			m_bCheckTimeAutoSkip = true;
		}
	}

	private void OnLoginCutinFinished()
	{
		UnHide();
		ShowNext(bWillPlayCutin: false);
	}

	public void ShowNext(bool bWillPlayCutin = true)
	{
		DeactiveSFXStar();
		if (m_CurrIdx < m_listUnitData.Count && m_CurrIdx >= 0)
		{
			GetUnitResultData data = m_listUnitData[m_CurrIdx];
			m_animGetUnit.Rebind();
			bool flag = false;
			if (m_animGetUnit != null)
			{
				if (data.m_bSkin)
				{
					NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(data.m_SkinID);
					if (skinTemplet != null && skinTemplet.HasLoginCutin)
					{
						if (bWillPlayCutin)
						{
							Hide();
							NKCUIEventSequence.PlaySkinCutin(skinTemplet, OnLoginCutinFinished);
							return;
						}
						m_animGetUnit.SetTrigger("NOINTRO");
						flag = true;
					}
				}
				if (!flag)
				{
					m_animGetUnit.SetTrigger("RESTART");
				}
			}
			NKCUIVoiceManager.StopVoice();
			NKCUtil.SetGameobjectActive(m_objGameOpenAwakenFX, bValue: false);
			m_CurrIdx++;
			SetData(data);
			if (flag)
			{
				NKCUtil.SetGameobjectActive(m_objGameOpenAwakenFX, bValue: false);
			}
			m_bPlayVoice = false;
			SetAutoSkipTimeStamp();
		}
		else
		{
			NKCUIVoiceManager.StopVoice();
			m_bForceHideGetUnitMark = false;
			Close();
			InvokeCallback();
		}
	}

	public override void OnBackButton()
	{
		OnTouchAnywhere();
	}

	private void SetData(GetUnitResultData resultUnitData)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(resultUnitData.m_UnitID);
		if (unitTempletBase == null)
		{
			return;
		}
		m_CurrentUnitResultData = resultUnitData;
		m_NKMUnitTempletBase = unitTempletBase;
		m_skinID = resultUnitData.m_SkinID;
		NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(resultUnitData.m_UnitUID);
		int layerIndex = m_animGetUnit.GetLayerIndex("STAR");
		int layerIndex2 = m_animGetUnit.GetLayerIndex("STAR_SKIN");
		if (resultUnitData.m_bSkin)
		{
			m_animGetUnit.SetLayerWeight(layerIndex, 0f);
			m_animGetUnit.SetLayerWeight(layerIndex2, 1f);
		}
		else
		{
			m_animGetUnit.SetLayerWeight(layerIndex, 1f);
			m_animGetUnit.SetLayerWeight(layerIndex2, 0f);
		}
		m_lbTitle.text = unitTempletBase.GetUnitTitle();
		m_lbName.text = unitTempletBase.GetUnitName();
		m_lbNameShadow.text = unitTempletBase.GetUnitName();
		int num = 0;
		int num2 = NKCUtil.GetStarCntByUnitGrade(unitTempletBase) + 3;
		int num3 = unitTempletBase.m_StarGradeMax;
		if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
		{
			num2 = 6;
			num3 = 6;
		}
		else if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			num2 = 0;
			num3 = 0;
		}
		for (num = 0; num < m_lstUnitGradeStarOFF.Count; num++)
		{
			if (num < num2)
			{
				m_lstUnitGradeStarOFF[num].SetActive(value: true);
			}
			else
			{
				m_lstUnitGradeStarOFF[num].SetActive(value: false);
			}
		}
		NKCUtil.SetStarRank(m_lstUnitGradeStar, resultUnitData.limitBreakStarCount, resultUnitData.StarMaxCount);
		for (num = 0; num < m_lstResultStar.Count; num++)
		{
			if (m_lstResultStar[num] != null)
			{
				m_lstResultStar[num].SetTranscendence(resultUnitData.m_UnitID, resultUnitData.m_LimitBreakLevel);
			}
		}
		for (num = 0; num < m_lstStarOFF.Count; num++)
		{
			if (num < num3)
			{
				m_lstStarOFF[num].SetActive(value: true);
			}
			else
			{
				m_lstStarOFF[num].SetActive(value: false);
			}
		}
		for (num = 0; num < m_lstStarONParent.Count; num++)
		{
			if (num < num3)
			{
				m_lstStarONParent[num].SetActive(value: true);
			}
			else
			{
				m_lstStarONParent[num].SetActive(value: false);
			}
		}
		NKCUtil.SetStarRank(m_lstStar, resultUnitData.limitBreakStarCount, resultUnitData.StarMaxCount);
		for (num = 0; num < 4; num++)
		{
			if ((int)m_NKMUnitTempletBase.m_NKM_UNIT_GRADE == num)
			{
				NKCUtil.SetGameobjectActive(m_lstUnitRank[num], bValue: true);
				NKCUtil.SetGameobjectActive(m_lstUnitLiiustFX[num], bValue: true);
				NKCUtil.SetGameobjectActive(m_lstUnitGradeFX[num], bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lstUnitRank[num], bValue: false);
				NKCUtil.SetGameobjectActive(m_lstUnitLiiustFX[num], bValue: false);
				NKCUtil.SetGameobjectActive(m_lstUnitGradeFX[num], bValue: false);
			}
		}
		NKCUtil.SetGameobjectActive(m_SFX_STAR_SSR, m_NKMUnitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SSR);
		NKCUtil.SetGameobjectActive(m_SFX_STAR_OTHER, m_NKMUnitTempletBase.m_NKM_UNIT_GRADE != NKM_UNIT_GRADE.NUG_SSR);
		NKCUtil.SetGameobjectActive(m_objRankAwakenSR, m_NKMUnitTempletBase.m_bAwaken && m_NKMUnitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SR);
		NKCUtil.SetGameobjectActive(m_objRankAwakenSSR, m_NKMUnitTempletBase.m_bAwaken && m_NKMUnitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SSR);
		NKCUtil.SetGameobjectActive(m_objBGAwakenSR, m_NKMUnitTempletBase.m_bAwaken && m_NKMUnitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SR);
		NKCUtil.SetGameobjectActive(m_objBGAwakenSSR, m_NKMUnitTempletBase.m_bAwaken && m_NKMUnitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SSR);
		NKCUtil.SetImageSprite(m_NKM_UI_RESULT_GET_UNIT_MARK, NKCResourceUtility.GetOrLoadUnitStyleIcon(unitTempletBase.m_NKM_UNIT_STYLE_TYPE));
		NKCUtil.SetGameobjectActive(m_NKM_UI_RESULT_GET_UNIT_SUMMARY_CLASS, unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL && !unitTempletBase.IsTrophy);
		if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL && !unitTempletBase.IsTrophy)
		{
			NKCUtil.SetImageSprite(m_NKM_UI_RESULT_GET_UNIT_SUMMARY_CLASS_ICON, NKCResourceUtility.GetOrLoadUnitRoleIcon(unitTempletBase, bSmall: true));
			NKCUtil.SetLabelText(m_NKM_UI_RESULT_GET_UNIT_SUMMARY_CLASS_TEXT, NKCUtilString.GetRoleText(unitTempletBase));
		}
		else if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR)
		{
			NKCUtil.SetImageSprite(m_NKM_UI_RESULT_GET_UNIT_SUMMARY_CLASS_ICON, NKCResourceUtility.GetOrLoadUnitStyleIcon(unitTempletBase.m_NKM_UNIT_STYLE_TYPE, bSmall: true));
			NKCUtil.SetLabelText(m_NKM_UI_RESULT_GET_UNIT_SUMMARY_CLASS_TEXT, "");
		}
		NKCDescTemplet descTemplet = NKCDescMgr.GetDescTemplet(resultUnitData.m_UnitID, resultUnitData.m_SkinID);
		if (descTemplet != null)
		{
			string desc = descTemplet.m_arrDescData[TypeToDescType(m_Type)].GetDesc();
			desc = NKCUtil.TextSplitLine(desc, m_lbDailoogue);
			m_lbDailoogue.text = desc;
		}
		else
		{
			m_lbDailoogue.text = "";
		}
		string unitStyleMarkString = NKCUtilString.GetUnitStyleMarkString(unitTempletBase);
		NKCUtil.SetLabelText(m_lbNKM_UI_RESULT_GET_UNIT_SUMMARY_STYLE_NAME, unitStyleMarkString);
		bool flag = true;
		if (NKCScenManager.CurrentUserData().m_ArmyData != null)
		{
			flag = HaveFirstGetUnit(resultUnitData.m_UnitID);
		}
		if (resultUnitData.m_bSkin)
		{
			SetTag(eUnitTagType.GetSkin);
		}
		else
		{
			switch (m_Type)
			{
			case Type.Get:
				if (flag)
				{
					SetTag(eUnitTagType.NewUnit);
				}
				else if (m_bForceHideGetUnitMark)
				{
					SetTag(eUnitTagType.None);
				}
				else
				{
					SetTag(eUnitTagType.GetUnit);
				}
				break;
			case Type.LimitBreak:
				if (resultUnitData.m_LimitBreakLevel > 3)
				{
					SetTag(eUnitTagType.Transcendence);
				}
				else
				{
					SetTag(eUnitTagType.LimitBreak);
				}
				break;
			case Type.Ship:
				SetTag(eUnitTagType.ShipUpgrade);
				break;
			}
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_RESULT_GET_SHIP_UPGRADE_SKILL, bValue: false);
		if (m_Type == Type.LimitBreak)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_RESULT_GET_UNIT_TRANSCENDENCE_INFO, bValue: true);
			if (resultUnitData.m_LimitBreakLevel > 3)
			{
				NKCUtil.SetGameobjectActive(m_objTranscendenceTextRoot, bValue: true);
				NKCUtil.SetGameobjectActive(m_LimitBreakTextRoot, bValue: false);
				NKCUtil.SetGameobjectActive(m_TRANSCENDENCE_TEXT2, bValue: false);
				NKCUtil.SetLabelText(m_lbTCCount, NKCUtilString.GET_STRING_LIMITBREAK_TRANSCENDENCE_LEVEL_ONE_PARAM, NKMUnitLimitBreakManager.GetTranscendenceCount(unitFromUID));
				StringBuilder stringBuilder = new StringBuilder();
				NKMLimitBreakTemplet lBInfo = NKMUnitLimitBreakManager.GetLBInfo(resultUnitData.m_LimitBreakLevel - 1);
				NKMLimitBreakTemplet lBInfo2 = NKMUnitLimitBreakManager.GetLBInfo(resultUnitData.m_LimitBreakLevel);
				if (lBInfo != null && lBInfo2 != null)
				{
					stringBuilder.Append(NKCUIComTextUnitLevel.GetColorTag(lBInfo.m_Tier));
					stringBuilder.AppendFormat(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, lBInfo.m_iMaxLevel);
					stringBuilder.Append("</color>");
					stringBuilder.Append(" > ");
					stringBuilder.Append(NKCUIComTextUnitLevel.GetColorTag(lBInfo2.m_Tier));
					stringBuilder.AppendFormat(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, lBInfo2.m_iMaxLevel);
					stringBuilder.Append("</color>");
					NKCUtil.SetLabelText(m_lbTCMaxLevel, stringBuilder.ToString());
					float num4 = NKMUnitLimitBreakManager.GetLimitBreakStatMultiplier(resultUnitData.m_LimitBreakLevel) - NKMUnitLimitBreakManager.GetLimitBreakStatMultiplier(resultUnitData.m_LimitBreakLevel - 1);
					NKCUtil.SetLabelText(m_lbTCGrowRate, NKCStringTable.GetString("SI_DP_RESULT_TRANSCENDENCE_UNIT_GROWTH_DIFFERENCE_ONE_PARAM"), num4 * 100f);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objTranscendenceTextRoot, bValue: false);
				NKCUtil.SetGameobjectActive(m_LimitBreakTextRoot, bValue: true);
				NKCUtil.SetGameobjectActive(m_TRANSCENDENCE_TEXT2, unitFromUID?.IsUnlockAccessory2() ?? false);
				bool flag2 = false;
				string skillStrID = unitTempletBase.GetSkillStrID(3);
				if (!string.Equals("", skillStrID))
				{
					NKMUnitSkillTemplet unitSkillTemplet = NKMUnitSkillManager.GetUnitSkillTemplet(skillStrID, unitFromUID);
					if (unitSkillTemplet != null)
					{
						int unlockReqUpgradeFromSkillId = NKMUnitSkillManager.GetUnlockReqUpgradeFromSkillId(unitSkillTemplet.m_ID);
						if (resultUnitData.m_LimitBreakLevel == unlockReqUpgradeFromSkillId)
						{
							flag2 = true;
						}
					}
				}
				NKCUtil.SetGameobjectActive(m_lbTRANSCENDENCE_TEXT1.gameObject, bValue: true);
				string text = "";
				text = (flag2 ? NKCUtilString.GET_STRING_RESULT_LIMIT_BREAK_UNIT_ONE_PARAM_UNLOCK_HYPER_SKILL : NKCUtilString.GET_STRING_RESULT_LIMIT_BREAK_UNIT_ONE_PARAM);
				float num5 = NKMUnitLimitBreakManager.GetLimitBreakStatMultiplier(resultUnitData.m_LimitBreakLevel) - NKMUnitLimitBreakManager.GetLimitBreakStatMultiplier(resultUnitData.m_LimitBreakLevel - 1);
				NKCUtil.SetLabelText(m_lbTRANSCENDENCE_TEXT1, string.Format(text, num5 * 100f));
				NKMLimitBreakTemplet lBInfo3 = NKMUnitLimitBreakManager.GetLBInfo(resultUnitData.m_LimitBreakLevel - 1);
				NKMLimitBreakTemplet lBInfo4 = NKMUnitLimitBreakManager.GetLBInfo(resultUnitData.m_LimitBreakLevel);
				if (lBInfo3 != null && lBInfo4 != null)
				{
					NKCUtil.SetLabelText(m_lbMAXLEVEL_COUNT, string.Format(NKCUtilString.GET_STRING_RESULT_LIMIT_BREAK_UNIT_MAX_LEVEL_TWO_PARAM, lBInfo3.m_iMaxLevel, lBInfo4.m_iMaxLevel));
				}
			}
		}
		else if (m_Type == Type.Ship)
		{
			if (m_CurrentUnitResultData.m_LimitBreakLevel > 0)
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_RESULT_GET_UNIT_TRANSCENDENCE_INFO, bValue: true);
				NKCUtil.SetGameobjectActive(m_objTranscendenceTextRoot, bValue: true);
				NKCUtil.SetGameobjectActive(m_LimitBreakTextRoot, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_RESULT_GET_SHIP_UPGRADE_SKILL, bValue: false);
				StringBuilder stringBuilder2 = new StringBuilder();
				NKMShipLevelUpTemplet shipLevelupTemplet = NKMShipManager.GetShipLevelupTemplet(m_CurrentUnitResultData.limitBreakStarCount, m_CurrentUnitResultData.m_LimitBreakLevel - 1);
				NKMShipLevelUpTemplet shipLevelupTemplet2 = NKMShipManager.GetShipLevelupTemplet(m_CurrentUnitResultData.limitBreakStarCount, m_CurrentUnitResultData.m_LimitBreakLevel);
				if (shipLevelupTemplet2 != null && shipLevelupTemplet != null)
				{
					stringBuilder2.Clear();
					if (shipLevelupTemplet.ShipMaxLevel == 100)
					{
						stringBuilder2.AppendFormat(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, shipLevelupTemplet.ShipMaxLevel);
					}
					else
					{
						stringBuilder2.Append("<color=#C57BF4>");
						stringBuilder2.AppendFormat(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, shipLevelupTemplet.ShipMaxLevel);
						stringBuilder2.Append("</color>");
					}
					stringBuilder2.Append(" > ");
					stringBuilder2.Append("<color=#C57BF4>");
					stringBuilder2.AppendFormat(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, shipLevelupTemplet2.ShipMaxLevel);
					stringBuilder2.Append("</color>");
					NKCUtil.SetLabelText(m_lbTCMaxLevel, stringBuilder2.ToString());
				}
				if (NKMShipManager.GetShipLimitBreakTemplet(m_CurrentUnitResultData.m_UnitID, m_CurrentUnitResultData.m_LimitBreakLevel) != null)
				{
					stringBuilder2.Clear();
					stringBuilder2.Append(string.Format(NKCUtilString.GET_STRING_SHIP_COMMANDMODULE_OPEN, m_CurrentUnitResultData.m_LimitBreakLevel));
					NKCUtil.SetLabelText(m_lbTCCount, stringBuilder2.ToString());
				}
				float num6 = NKMUnitLimitBreakManager.GetLimitBreakStatMultiplierForShip(resultUnitData.m_LimitBreakLevel) - NKMUnitLimitBreakManager.GetLimitBreakStatMultiplierForShip(resultUnitData.m_LimitBreakLevel - 1);
				NKCUtil.SetLabelText(m_lbTCGrowRate, NKCStringTable.GetString("SI_DP_RESULT_TRANSCENDENCE_UNIT_GROWTH_DIFFERENCE_ONE_PARAM"), Mathf.RoundToInt(num6 * 100f));
			}
			else
			{
				for (num = 0; num < m_lstNewSkill.Count; num++)
				{
					NKCUtil.SetGameobjectActive(m_lstNewSkill[num], bValue: false);
				}
				NKCUtil.SetGameobjectActive(m_NKM_UI_RESULT_GET_UNIT_TRANSCENDENCE_INFO, bValue: false);
				NKCUtil.SetGameobjectActive(m_lbTRANSCENDENCE_TEXT1.gameObject, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_RESULT_GET_SHIP_UPGRADE_SKILL, bValue: true);
				NKCUtil.SetLabelText(m_lbSHIP_MAXLEVEL_COUNT, string.Format(NKCUtilString.GET_STRING_RESULT_LIMIT_BREAK_UNIT_MAX_LEVEL_TWO_PARAM, m_iCurMaxShipLv, m_iNextMaxShipLv));
				NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(resultUnitData.m_UnitID);
				if (unitTempletBase2 != null)
				{
					for (num = 0; num < m_lstSkill.Count; num++)
					{
						NKMShipSkillTemplet shipSkillTempletByIndex = NKMShipSkillManager.GetShipSkillTempletByIndex(unitTempletBase2, num);
						if (shipSkillTempletByIndex != null)
						{
							NKCUtil.SetImageSprite(m_lstSkill[num], NKCUtil.GetSkillIconSprite(shipSkillTempletByIndex));
						}
						else
						{
							NKCUtil.SetGameobjectActive(m_lstSkill[num].gameObject, bValue: false);
						}
						if (num > 1 && m_iCurSkillCnt - 1 < num)
						{
							NKCUtil.SetGameobjectActive(m_lstNewSkill[num - 1], bValue: true);
						}
					}
				}
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_RESULT_GET_UNIT_TRANSCENDENCE_INFO, bValue: false);
		}
		SetBlackIllust(unitTempletBase, resultUnitData);
		SetIllust(unitTempletBase, resultUnitData);
		NKCUtil.SetGameobjectActive(m_objGameOpenAwakenFX, unitTempletBase.m_bAwaken);
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.SNS_SHARE_BUTTON) && NKCPublisherModule.Marketing.SnsShareEnabled(unitFromUID))
		{
			if (NKCPublisherModule.Marketing.IsOnlyUnitShare())
			{
				NKCUtil.SetGameobjectActive(m_csbtnShare, unitFromUID != null && m_NKMUnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL);
				NKCUtil.SetGameobjectActive(m_objFacebookMark, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_csbtnShare, bValue: true);
				NKCUtil.SetGameobjectActive(m_objFacebookMark, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_csbtnShare, bValue: false);
		}
	}

	public void SetShipData(int curSkillCnt, int curMaxLv, int nextMaxLv)
	{
		m_iCurSkillCnt = curSkillCnt;
		m_iCurMaxShipLv = curMaxLv;
		m_iNextMaxShipLv = nextMaxLv;
	}

	public void OnTouchAnywhere()
	{
		if (NKCUIManager.IsTopmostUI(this))
		{
			if (!m_bEnableNextChar && m_fTime + 1.1f < Time.time)
			{
				m_bEnableNextChar = true;
				ShowNext();
			}
			else if (m_animGetUnit.GetCurrentAnimatorStateInfo(0).IsName("START") && m_NKMUnitTempletBase != null && !m_setFirstGetUnit.Contains(m_NKMUnitTempletBase.m_UnitID))
			{
				m_bEnableNextChar = false;
				m_fTime = Time.time;
				m_animGetUnit.SetTrigger("SKIP");
			}
		}
	}

	public void SkipAll()
	{
		Close();
		InvokeCallback();
	}

	public void OnShare()
	{
		NKMUnitData cNKMUnitData = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(m_CurrentUnitResultData.m_UnitUID);
		if (NKCPublisherModule.Marketing.IsUseSnsSharePopup())
		{
			NKCPopupSnsShareMenu.Instance.Open(delegate(NKCPublisherModule.SNS_SHARE_TYPE e)
			{
				NKCPopupSnsShare.Instance.Open(NKCScenManager.CurrentUserData(), cNKMUnitData, e);
			});
		}
		else
		{
			NKCPopupSnsShare.Instance.Open(NKCScenManager.CurrentUserData(), cNKMUnitData, NKCPublisherModule.SNS_SHARE_TYPE.SST_FACEBOOK);
		}
	}

	private void Update()
	{
		if (m_bEnableNextChar && (m_animGetUnit.GetCurrentAnimatorStateInfo(0).IsName("LOOP") || m_animGetUnit.GetCurrentAnimatorStateInfo(0).IsName("ON")))
		{
			m_bEnableNextChar = false;
			m_fTime = Time.time;
		}
		if (m_bCheckTimeAutoSkip && m_fTimeAutoSkip + 4f < Time.time)
		{
			m_bCheckTimeAutoSkip = false;
			OnTouchAnywhere();
		}
		if (!m_bPlayVoice && (m_animGetUnit.GetCurrentAnimatorStateInfo(0).IsName("TERM") || m_animGetUnit.GetCurrentAnimatorStateInfo(0).IsName("LOOP")))
		{
			NKCUIVoiceManager.PlayVoice(TypeToVoiceType(m_Type), m_NKMUnitTempletBase.m_UnitStrID, m_skinID);
			m_bPlayVoice = true;
		}
	}

	private void SetIllust(NKMUnitTempletBase targetUnitTempletBase, GetUnitResultData unitData)
	{
		if (m_NKCASUISpineIllust != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUISpineIllust);
		}
		m_NKCASUISpineIllust = null;
		if (targetUnitTempletBase == null)
		{
			return;
		}
		m_NKCASUISpineIllust = NKCResourceUtility.OpenSpineIllust(targetUnitTempletBase, unitData.m_SkinID);
		if (m_NKCASUISpineIllust != null)
		{
			if (targetUnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
			{
				m_NKCASUISpineIllust.SetParent(m_trRootSpineShip, worldPositionStays: false);
			}
			else
			{
				m_NKCASUISpineIllust.SetParent(m_trRootSpineIllust, worldPositionStays: false);
			}
			m_NKCASUISpineIllust.PurgeHyperCutsceneIllust();
			m_NKCASUISpineIllust.SetDefaultAnimation(targetUnitTempletBase);
			m_NKCASUISpineIllust.SetAnchoredPosition(Vector2.zero);
			m_NKCASUISpineIllust.SetIllustBackgroundEnable(bValue: false);
			m_NKCASUISpineIllust.SetSkinOption(0);
			NKCDescTemplet descTemplet = NKCDescMgr.GetDescTemplet(unitData.m_UnitID, unitData.m_SkinID);
			if (descTemplet != null)
			{
				NKCDescTemplet.NKCDescData nKCDescData = descTemplet.m_arrDescData[TypeToDescType(m_Type)];
				m_NKCASUISpineIllust.SetAnimation(nKCDescData.m_Ani, loop: false);
			}
			else
			{
				_ = targetUnitTempletBase.m_NKM_UNIT_TYPE;
				_ = 2;
				m_NKCASUISpineIllust.SetAnimation(NKCASUIUnitIllust.eAnimation.UNIT_TOUCH, loop: false);
			}
		}
	}

	private void SetBlackIllust(NKMUnitTempletBase targetUnitTempletBase, GetUnitResultData unitData)
	{
		if (m_NKCASUIBlackSpineIllust != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUIBlackSpineIllust);
		}
		m_NKCASUIBlackSpineIllust = null;
		if (targetUnitTempletBase == null)
		{
			return;
		}
		m_NKCASUIBlackSpineIllust = NKCResourceUtility.OpenSpineIllust(targetUnitTempletBase, unitData.m_SkinID);
		if (m_NKCASUIBlackSpineIllust != null)
		{
			m_NKCASUIBlackSpineIllust.PurgeHyperCutsceneIllust();
			m_NKCASUIBlackSpineIllust.SetColor(Color.black);
			m_NKCASUIBlackSpineIllust.SetIllustBackgroundEnable(unitData.m_SkinID == 0);
			m_NKCASUIBlackSpineIllust.SetSkinOption(0);
			if (targetUnitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
			{
				m_NKCASUIBlackSpineIllust.SetParent(m_rtSPINEAREA_SHIP, worldPositionStays: false);
			}
			else
			{
				m_NKCASUIBlackSpineIllust.SetParent(m_rtSPINEAREA, worldPositionStays: false);
			}
			m_NKCASUIBlackSpineIllust.SetDefaultAnimation(targetUnitTempletBase);
			m_NKCASUIBlackSpineIllust.SetAnchoredPosition(Vector2.zero);
			m_NKCASUIBlackSpineIllust.SetVFX(bSet: false);
		}
	}

	public static void AddFirstGetUnit(NKMRewardData rewardData)
	{
		if (rewardData == null)
		{
			return;
		}
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		if (armyData == null)
		{
			return;
		}
		if (rewardData.UnitDataList != null)
		{
			foreach (NKMUnitData unitData in rewardData.UnitDataList)
			{
				if (armyData.IsFirstGetUnit(unitData.m_UnitID))
				{
					AddFirstGetUnit(unitData.m_UnitID);
				}
			}
		}
		if (rewardData.OperatorList == null)
		{
			return;
		}
		foreach (NKMOperator @operator in rewardData.OperatorList)
		{
			if (armyData.IsFirstGetUnit(@operator.id))
			{
				AddFirstGetUnit(@operator.id);
			}
		}
	}

	public static void AddFirstGetUnit(int unitID)
	{
		m_setFirstGetUnit.Add(unitID);
	}

	public static bool HaveFirstGetUnit(int unitID)
	{
		return m_setFirstGetUnit.Contains(unitID);
	}

	private void ClearIllustMem()
	{
		if (m_NKCASUISpineIllust != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUISpineIllust);
			m_NKCASUISpineIllust = null;
		}
		if (m_NKCASUIBlackSpineIllust != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUIBlackSpineIllust);
			m_NKCASUIBlackSpineIllust = null;
		}
		NKCScenManager.GetScenManager().m_NKCMemoryCleaner.UnloadObjectPool();
	}

	public override void CloseInternal()
	{
		ClearIllustMem();
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		m_setFirstGetUnit.Clear();
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.CleanUp();
		}
		m_bForceHideGetUnitMark = false;
	}

	private void InvokeCallback()
	{
		m_NKCUIGRGetUnitCallBack?.Invoke();
		m_NKCUIGRGetUnitCallBack = null;
	}

	private int TypeToDescType(Type type)
	{
		if (type != Type.Get && type == Type.LimitBreak)
		{
			return 3;
		}
		return 2;
	}

	private VOICE_TYPE TypeToVoiceType(Type type)
	{
		if (type != Type.Get && type == Type.LimitBreak)
		{
			return VOICE_TYPE.VT_GROWTH_ASCEND;
		}
		return VOICE_TYPE.VT_GET;
	}

	public void SetBG()
	{
		bool flag = NKCScenManager.GetScenManager().GetGameOptionData()?.UseVideoTexture ?? false;
		NKCUtil.SetGameobjectActive(m_objFallbackBG, !flag);
		if (flag)
		{
			NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
			if (subUICameraVideoPlayer != null)
			{
				subUICameraVideoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
				subUICameraVideoPlayer.m_fMoviePlaySpeed = 1f;
				subUICameraVideoPlayer.Play("Contract_BG.mp4", bLoop: true, bPlaySound: false, VideoPlayMessageCallback);
			}
		}
	}

	private void VideoPlayMessageCallback(NKCUIComVideoPlayer.eVideoMessage message)
	{
		if (message == NKCUIComVideoPlayer.eVideoMessage.PlayFailed)
		{
			NKCUtil.SetGameobjectActive(m_objFallbackBG, bValue: true);
		}
	}

	private void SetTag(eUnitTagType type)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_RESULT_GET_UNIT_GET, type == eUnitTagType.GetUnit);
		NKCUtil.SetGameobjectActive(m_NKM_UI_RESULT_GET_UNIT_NEW, type == eUnitTagType.NewUnit);
		NKCUtil.SetGameobjectActive(m_objUnitLimitBreak, type == eUnitTagType.LimitBreak);
		NKCUtil.SetGameobjectActive(m_objUnitTranscendence, type == eUnitTagType.Transcendence);
		NKCUtil.SetGameobjectActive(m_objGetSkin, type == eUnitTagType.GetSkin);
		NKCUtil.SetGameobjectActive(m_NKM_UI_RESULT_GET_SHIP_UPGRADE, type == eUnitTagType.ShipUpgrade);
	}

	public override bool OnHotkey(HotkeyEventType hotkey)
	{
		switch (hotkey)
		{
		case HotkeyEventType.Confirm:
			OnTouchAnywhere();
			return true;
		case HotkeyEventType.ShowHotkey:
			if (m_btnSkip != null)
			{
				NKCUIComHotkeyDisplay.OpenInstance(m_btnSkip.transform, HotkeyEventType.Confirm, HotkeyEventType.Skip);
			}
			break;
		}
		return false;
	}

	public override void OnHotkeyHold(HotkeyEventType hotkey)
	{
		if (hotkey == HotkeyEventType.Skip)
		{
			OnTouchAnywhere();
		}
	}
}
