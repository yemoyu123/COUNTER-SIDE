using System.Collections.Generic;
using ClientPacket.Community;
using ClientPacket.User;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI.Collection;

public class NKCUICollection : NKCUICollectionGeneral
{
	public delegate void OnNotify(bool bNotify);

	public delegate void OnSyncCollectingData(CollectionType type, int iCur, int iTotal);

	public delegate void OnStoryCutscen(bool bPlay);

	public const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_collection";

	public const string UI_ASSET_NAME = "NKM_UI_COLLECTION";

	private CollectionType m_eCollectionType = CollectionType.CT_NONE;

	[Header("생성 위치")]
	public RectTransform m_rtNKM_UI_COLLECTION_CONTENT;

	[Header("획득율")]
	public NKCUICollectionRate m_NKCUICollectionRate;

	[Header("알림")]
	public GameObject NKM_UI_COLLECTION_PANEL_MENU_TEAM_UP_Reddot;

	public GameObject NKM_UI_COLLECTION_PANEL_MENU_EMPLOYEE_Reddot;

	[Header("메뉴 버튼")]
	public NKCUIComToggle m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_TEAN_UP;

	public NKCUIComToggle m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_EMPLOYEE;

	public NKCUIComToggle m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_OPERATOR;

	public NKCUIComToggle m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_SHIP;

	public NKCUIComToggle m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_ALBUM;

	public NKCUIComToggle m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_STORY;

	private const string ASSET_BUNDLE_NAME_COLLECTION = "ab_ui_nkm_ui_collection";

	private const string UI_ASSET_NAME_TEAMUP = "NKM_UI_COLLECTION_TEAM_UP";

	private NKCUICollectionTeamUp m_NKCUICollectionTeamUp;

	private const string UI_ASSET_NAME_UNIT = "NKM_UI_COLLECTION_UNIT";

	private NKCUICollectionUnitList m_NKCUICollectionUnit;

	private const string UI_ASSET_NAME_OPERATOR = "NKM_UI_COLLECTION_OPERATOR";

	private NKCUICollectionOperatorList m_NKCUICollectionOperator;

	private const string UI_ASSET_NAME_SHIP = "NKM_UI_COLLECTION_SHIP";

	private NKCUICollectionUnitList m_NKCUICollectionShip;

	private const string UI_ASSET_NAME_ILLUST = "NKM_UI_COLLECTION_ILLUST";

	private NKCUICollectionIllust m_NKCUICollectionIllust;

	private const string UI_ASSET_NAME_STORY = "NKM_UI_COLLECTION_STORY";

	private NKCUICollectionStory m_NKCUICollectionStory;

	private List<NKCAssetInstanceData> m_lstAssetInstance = new List<NKCAssetInstanceData>();

	private List<int> m_lstUpdateUnitMissionUnitId = new List<int>();

	private bool m_bDataValid;

	public override string MenuName => NKCUtilString.GET_STRING_COLLECTION;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string GuideTempletID => "ARTICLE_SYSTEM_COLLETION";

	public override void OnBackButton()
	{
		if (NKCUICutScenPlayer.IsInstanceOpen && NKCUICutScenPlayer.Instance.IsPlaying())
		{
			NKCUICutScenPlayer.Instance.StopWithCallBack();
			return;
		}
		base.OnBackButton();
		NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME, bForce: false);
	}

	public override void Hide()
	{
		base.Hide();
	}

	public override void UnHide()
	{
		if (!m_bDataValid)
		{
			m_bDataValid = true;
		}
		base.UnHide();
		if (m_lstUpdateUnitMissionUnitId.Count > 0)
		{
			if (m_NKCUICollectionUnit != null && m_NKCUICollectionUnit.gameObject.activeSelf)
			{
				m_NKCUICollectionUnit.UpdateCollectionMissionRate(m_lstUpdateUnitMissionUnitId);
			}
			m_lstUpdateUnitMissionUnitId.Clear();
		}
		NKCUtil.SetGameobjectActive(NKM_UI_COLLECTION_PANEL_MENU_EMPLOYEE_Reddot, NKCUnitMissionManager.HasRewardEnableMission());
		if (m_NKCUICollectionUnit != null && m_NKCUICollectionUnit.gameObject.activeSelf)
		{
			m_NKCUICollectionUnit.CheckRewardToggle();
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		CloseAllInstance();
	}

	public override NKM_SHORTCUT_TYPE GetShortcutType()
	{
		return m_eCollectionType switch
		{
			CollectionType.CT_TEAM_UP => NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_TEAMUP, 
			CollectionType.CT_UNIT => NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_UNIT, 
			CollectionType.CT_SHIP => NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_SHIP, 
			CollectionType.CT_OPERATOR => NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_OPERATOR, 
			CollectionType.CT_ILLUST => NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_ILLUST, 
			CollectionType.CT_STORY => NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION_STORY, 
			_ => NKM_SHORTCUT_TYPE.SHORTCUT_COLLECTION, 
		};
	}

	public override void Init()
	{
		InitButton();
		m_NKCUICollectionRate.Init();
		NKCUtil.SetGameobjectActive(m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_OPERATOR, !NKCOperatorUtil.IsHide());
	}

	private void InitButton()
	{
		if (null != m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_TEAN_UP)
		{
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_TEAN_UP.OnValueChanged.RemoveAllListeners();
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_TEAN_UP.OnValueChanged.AddListener(delegate
			{
				ChangeState(CollectionType.CT_TEAM_UP);
			});
		}
		if (null != m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_EMPLOYEE)
		{
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_EMPLOYEE.OnValueChanged.RemoveAllListeners();
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_EMPLOYEE.OnValueChanged.AddListener(delegate
			{
				ChangeState(CollectionType.CT_UNIT);
			});
		}
		if (null != m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_OPERATOR)
		{
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_OPERATOR.OnValueChanged.RemoveAllListeners();
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_OPERATOR.OnValueChanged.AddListener(delegate
			{
				ChangeState(CollectionType.CT_OPERATOR);
			});
		}
		if (null != m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_SHIP)
		{
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_SHIP.OnValueChanged.RemoveAllListeners();
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_SHIP.OnValueChanged.AddListener(delegate
			{
				ChangeState(CollectionType.CT_SHIP);
			});
		}
		if (null != m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_ALBUM)
		{
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_ALBUM.OnValueChanged.RemoveAllListeners();
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_ALBUM.OnValueChanged.AddListener(delegate
			{
				ChangeState(CollectionType.CT_ILLUST);
			});
		}
		if (null != m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_STORY)
		{
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_STORY.OnValueChanged.RemoveAllListeners();
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_STORY.OnValueChanged.AddListener(delegate
			{
				ChangeState(CollectionType.CT_STORY);
			});
		}
	}

	public override void Open(CollectionType reserveType = CollectionType.CT_NONE, string reserveUnitStrID = "")
	{
		m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_TEAN_UP.Select(bSelect: true);
		UIOpened();
		if (!NKCOperatorUtil.IsActive() && reserveType == CollectionType.CT_OPERATOR)
		{
			reserveType = CollectionType.CT_NONE;
		}
		if ((uint)(reserveType - -1) > 1u && (uint)(reserveType - 1) <= 4u)
		{
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_EMPLOYEE.Select(reserveType == CollectionType.CT_UNIT);
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_OPERATOR.Select(reserveType == CollectionType.CT_OPERATOR);
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_SHIP.Select(reserveType == CollectionType.CT_SHIP);
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_ALBUM.Select(reserveType == CollectionType.CT_ILLUST);
			m_ctbtn_NKM_UI_COLLECTION_PANEL_MENU_STORY.Select(reserveType == CollectionType.CT_STORY);
			ChangeState(reserveType);
		}
		if (!string.IsNullOrEmpty(reserveUnitStrID))
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(reserveUnitStrID);
			if (unitTempletBase != null)
			{
				switch (unitTempletBase.m_NKM_UNIT_TYPE)
				{
				default:
					NKCUICollectionUnitInfo.CheckInstanceAndOpen(NKCUtil.MakeDummyUnit(unitTempletBase.m_UnitID, 100, 3), null);
					break;
				case NKM_UNIT_TYPE.NUT_SHIP:
					NKCUICollectionShipInfo.CheckInstanceAndOpen(NKCUtil.MakeDummyUnit(unitTempletBase.m_UnitID, 100, 3), NKMDeckIndex.None);
					break;
				case NKM_UNIT_TYPE.NUT_OPERATOR:
					NKCUICollectionOperatorInfo.Instance.Open(NKCOperatorUtil.GetDummyOperator(unitTempletBase, bSetMaximum: true));
					break;
				}
			}
		}
		NKCUtil.SetGameobjectActive(NKM_UI_COLLECTION_PANEL_MENU_EMPLOYEE_Reddot, NKCUnitMissionManager.HasRewardEnableMission());
	}

	private void CloseAllInstance()
	{
		if (null != m_NKCUICollectionUnit)
		{
			m_NKCUICollectionUnit.Clear();
			m_NKCUICollectionUnit = null;
		}
		if (null != m_NKCUICollectionShip)
		{
			m_NKCUICollectionShip.Clear();
			m_NKCUICollectionShip = null;
		}
		if (null != m_NKCUICollectionOperator)
		{
			m_NKCUICollectionOperator.Clear();
			m_NKCUICollectionOperator = null;
		}
		if (null != m_NKCUICollectionTeamUp)
		{
			m_NKCUICollectionTeamUp.Clear();
			m_NKCUICollectionTeamUp = null;
		}
		if (null != m_NKCUICollectionIllust)
		{
			m_NKCUICollectionIllust.Clear();
			m_NKCUICollectionIllust = null;
		}
		if (null != m_NKCUICollectionStory)
		{
			m_NKCUICollectionStory.Clear();
			m_NKCUICollectionStory = null;
		}
		if (m_lstUpdateUnitMissionUnitId != null)
		{
			m_lstUpdateUnitMissionUnitId.Clear();
			m_lstUpdateUnitMissionUnitId = null;
		}
		for (int i = 0; i < m_lstAssetInstance.Count; i++)
		{
			m_lstAssetInstance[i].Unload();
		}
		m_eCollectionType = CollectionType.CT_NONE;
		Object.Destroy(base.gameObject);
		NKCAssetResourceManager.CloseResource("ab_ui_nkm_ui_collection", "NKM_UI_COLLECTION");
	}

	private void ChangeState(CollectionType type)
	{
		if (m_eCollectionType == type)
		{
			Debug.Log("already loaded type " + type);
			return;
		}
		PreUIUnHide();
		OpenUI(type);
		m_eCollectionType = type;
	}

	private void OpenUI(CollectionType type)
	{
		switch (type)
		{
		case CollectionType.CT_TEAM_UP:
			if (null == m_NKCUICollectionTeamUp)
			{
				NKCAssetInstanceData AssetInstance6 = null;
				LoadInstance(ref AssetInstance6, ref m_NKCUICollectionTeamUp, "ab_ui_nkm_ui_collection", "NKM_UI_COLLECTION_TEAM_UP");
				m_NKCUICollectionTeamUp.Init(SyncCollectingData, NotifyTeamup);
				m_NKCUICollectionTeamUp.gameObject.transform.SetParent(m_rtNKM_UI_COLLECTION_CONTENT, worldPositionStays: false);
				m_lstAssetInstance.Add(AssetInstance6);
			}
			NKCUtil.SetGameobjectActive(m_NKCUICollectionTeamUp.gameObject, bValue: true);
			m_NKCUICollectionTeamUp.Open();
			break;
		case CollectionType.CT_UNIT:
			if (null == m_NKCUICollectionUnit)
			{
				NKCAssetInstanceData AssetInstance2 = null;
				LoadInstance(ref AssetInstance2, ref m_NKCUICollectionUnit, "ab_ui_nkm_ui_collection", "NKM_UI_COLLECTION_UNIT");
				m_NKCUICollectionUnit.Init(SyncCollectingData);
				m_NKCUICollectionUnit.gameObject.transform.SetParent(m_rtNKM_UI_COLLECTION_CONTENT, worldPositionStays: false);
				m_lstAssetInstance.Add(AssetInstance2);
			}
			NKCUtil.SetGameobjectActive(m_NKCUICollectionUnit.gameObject, bValue: true);
			m_NKCUICollectionUnit.Open();
			break;
		case CollectionType.CT_OPERATOR:
			if (null == m_NKCUICollectionOperator)
			{
				NKCAssetInstanceData AssetInstance5 = null;
				LoadInstance(ref AssetInstance5, ref m_NKCUICollectionOperator, "ab_ui_nkm_ui_collection", "NKM_UI_COLLECTION_OPERATOR");
				m_NKCUICollectionOperator.Init(SyncCollectingData);
				m_NKCUICollectionOperator.gameObject.transform.SetParent(m_rtNKM_UI_COLLECTION_CONTENT, worldPositionStays: false);
				m_lstAssetInstance.Add(AssetInstance5);
			}
			NKCUtil.SetGameobjectActive(m_NKCUICollectionOperator.gameObject, bValue: true);
			m_NKCUICollectionOperator.Open();
			break;
		case CollectionType.CT_SHIP:
			if (null == m_NKCUICollectionShip)
			{
				NKCAssetInstanceData AssetInstance3 = null;
				LoadInstance(ref AssetInstance3, ref m_NKCUICollectionShip, "ab_ui_nkm_ui_collection", "NKM_UI_COLLECTION_SHIP");
				m_NKCUICollectionShip.Init(SyncCollectingData);
				m_NKCUICollectionShip.gameObject.transform.SetParent(m_rtNKM_UI_COLLECTION_CONTENT, worldPositionStays: false);
				m_lstAssetInstance.Add(AssetInstance3);
			}
			NKCUtil.SetGameobjectActive(m_NKCUICollectionShip.gameObject, bValue: true);
			m_NKCUICollectionShip.Open();
			break;
		case CollectionType.CT_ILLUST:
			if (null == m_NKCUICollectionIllust)
			{
				NKCAssetInstanceData AssetInstance4 = null;
				LoadInstance(ref AssetInstance4, ref m_NKCUICollectionIllust, "ab_ui_nkm_ui_collection", "NKM_UI_COLLECTION_ILLUST");
				m_NKCUICollectionIllust.Init(SyncCollectingData);
				m_NKCUICollectionIllust.gameObject.transform.SetParent(m_rtNKM_UI_COLLECTION_CONTENT, worldPositionStays: false);
				m_lstAssetInstance.Add(AssetInstance4);
			}
			NKCUtil.SetGameobjectActive(m_NKCUICollectionIllust.gameObject, bValue: true);
			m_NKCUICollectionIllust.Open();
			break;
		case CollectionType.CT_STORY:
			if (null == m_NKCUICollectionStory)
			{
				NKCAssetInstanceData AssetInstance = null;
				LoadInstance(ref AssetInstance, ref m_NKCUICollectionStory, "ab_ui_nkm_ui_collection", "NKM_UI_COLLECTION_STORY");
				m_NKCUICollectionStory.Init(SyncCollectingData, StoryCutscen);
				m_NKCUICollectionStory.gameObject.transform.SetParent(m_rtNKM_UI_COLLECTION_CONTENT, worldPositionStays: false);
				SetupScrollRects(m_NKCUICollectionStory.gameObject);
				m_lstAssetInstance.Add(AssetInstance);
			}
			NKCUtil.SetGameobjectActive(m_NKCUICollectionStory.gameObject, bValue: true);
			m_NKCUICollectionStory.Open();
			break;
		}
	}

	public void NotifyTeamup(bool bNotify)
	{
		if (NKM_UI_COLLECTION_PANEL_MENU_TEAM_UP_Reddot != null)
		{
			NKCUtil.SetGameobjectActive(NKM_UI_COLLECTION_PANEL_MENU_TEAM_UP_Reddot, bNotify);
		}
	}

	public void SyncCollectingData(CollectionType type, int iCur, int iTotal)
	{
		m_NKCUICollectionRate.SetData(type, iCur, iTotal);
	}

	private void LoadInstance<T>(ref NKCAssetInstanceData AssetInstance, ref T script, string AssetBundleName, string AssetName)
	{
		AssetInstance = NKCAssetResourceManager.OpenInstance<GameObject>(AssetBundleName, AssetName);
		if (AssetInstance.m_Instant != null)
		{
			AssetInstance.m_Instant.transform.SetParent(base.gameObject.transform, worldPositionStays: false);
			script = AssetInstance.m_Instant.GetComponent<T>();
			return;
		}
		Debug.LogError("Load Faile " + typeof(T).ToString() + ", path" + AssetBundleName.ToString() + ", name" + AssetName.ToString());
	}

	private void PreUIUnHide()
	{
		if (m_eCollectionType != CollectionType.CT_NONE)
		{
			switch (m_eCollectionType)
			{
			case CollectionType.CT_TEAM_UP:
				NKCUtil.SetGameobjectActive(m_NKCUICollectionTeamUp.gameObject, bValue: false);
				break;
			case CollectionType.CT_UNIT:
				NKCUtil.SetGameobjectActive(m_NKCUICollectionUnit.gameObject, bValue: false);
				break;
			case CollectionType.CT_SHIP:
				NKCUtil.SetGameobjectActive(m_NKCUICollectionShip.gameObject, bValue: false);
				break;
			case CollectionType.CT_OPERATOR:
				NKCUtil.SetGameobjectActive(m_NKCUICollectionOperator.gameObject, bValue: false);
				break;
			case CollectionType.CT_ILLUST:
				NKCUtil.SetGameobjectActive(m_NKCUICollectionIllust.gameObject, bValue: false);
				break;
			case CollectionType.CT_STORY:
				NKCUtil.SetGameobjectActive(m_NKCUICollectionStory.gameObject, bValue: false);
				break;
			}
		}
	}

	public override void OnRecvReviewTagVoteCancelAck(NKMPacket_UNIT_REVIEW_TAG_VOTE_CANCEL_ACK sPacket)
	{
		NKCUICollectionUnitInfo.Instance.OnRecvReviewTagVoteCancelAck(sPacket);
	}

	public override void OnRecvReviewTagVoteAck(NKMPacket_UNIT_REVIEW_TAG_VOTE_ACK sPacket)
	{
		NKCUICollectionUnitInfo.Instance.OnRecvReviewTagVoteAck(sPacket);
	}

	public override void OnRecvReviewTagListAck(NKMPacket_UNIT_REVIEW_TAG_LIST_ACK sPacket)
	{
		NKCUICollectionUnitInfo.Instance.OnRecvReviewTagListAck(sPacket);
	}

	public override void OnRecvTeamCollectionRewardAck(NKMPacket_TEAM_COLLECTION_REWARD_ACK sPacket)
	{
		m_NKCUICollectionTeamUp.OnRecvTeamCollectionRewardAck(sPacket);
	}

	public override void OnRecvUnitMissionReward(int unitId)
	{
		m_lstUpdateUnitMissionUnitId.Add(unitId);
	}

	public override void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
		if (eEventType == NKMUserData.eChangeNotifyType.Add)
		{
			if (m_bHide)
			{
				m_bDataValid = false;
			}
			else
			{
				UpdateCollection();
			}
		}
	}

	private void UpdateCollection()
	{
		switch (m_eCollectionType)
		{
		case CollectionType.CT_TEAM_UP:
			m_NKCUICollectionTeamUp.Open();
			break;
		case CollectionType.CT_UNIT:
			m_NKCUICollectionUnit.Open();
			break;
		case CollectionType.CT_SHIP:
			m_NKCUICollectionShip.Open();
			break;
		case CollectionType.CT_STORY:
			m_NKCUICollectionStory.Open();
			break;
		case CollectionType.CT_OPERATOR:
			m_NKCUICollectionOperator.Open();
			break;
		default:
			Debug.Log("Can not fount collection type : " + m_eCollectionType);
			break;
		}
	}

	public void StoryCutscen(bool bPlay)
	{
	}
}
