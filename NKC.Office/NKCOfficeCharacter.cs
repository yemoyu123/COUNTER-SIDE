using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using ClientPacket.Common;
using Cs.Logging;
using NKC.Templet;
using NKC.Templet.Office;
using NKC.UI;
using NKC.UI.Component;
using NKC.UI.Component.Office;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.Office;

public class NKCOfficeCharacter : MonoBehaviour, INKCAnimationActor, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private enum State
	{
		NONE,
		AI,
		WaitGrab,
		Grab
	}

	public delegate bool OnClick();

	public Animator m_animator;

	public Transform m_trSDParent;

	public GameObject m_objShadow;

	public NKCUIComOfficeLoyalty m_comLoyalty;

	public NKCUIComOfficeFriendInfo m_comFriendInfo;

	public NKCUIComCharacterEmotion m_comEmotion;

	public bool m_bCanTouch = true;

	public bool m_bCanGrab = true;

	public Graphic m_gpTouchTarget;

	protected BehaviorTree BT;

	protected NKCOfficeBuildingBase m_OfficeBuilding;

	private State m_eState;

	private OnClick dOnClick;

	private NKCAnimationInstance m_animInstance;

	private NKCAnimationInstance m_emotionAnimInstance;

	private Queue<NKCAnimationInstance> m_qAnimInstances = new Queue<NKCAnimationInstance>();

	private List<NKCAnimationInstance> m_lstFinishedInstances = new List<NKCAnimationInstance>();

	protected NKCASUIUnitIllust m_SDIllust;

	protected NKMUnitData m_UnitData;

	protected long m_FriendUID;

	private const string UNIT_OFFICE_SD_BUNDLENAME = "AB_UNIT_OFFICE_SD";

	private const string UNIT_OFFICE_SD_ASSETNAME = "UNIT_OFFICE_SD";

	private bool m_bTakeHeartSent;

	public float GrabYOffset = 20f;

	public float GRAB_WAIT_TIME = 0.2f;

	private float m_fGrabWaitTime;

	private Vector2 m_touchPos = Vector2.zero;

	private Vector3 m_touchLocalOffset = Vector3.zero;

	public float grabUITime = 0.5f;

	private float m_fInteractionCheckInterval;

	private float m_fFurnitureInteractionCooltime;

	private float m_fUnitInteractionCooltime;

	private float m_fSoloInteractionCooltime;

	private bool m_bRectWorldValid;

	private Rect m_rectWorld;

	private List<NKCOfficeUnitInteractionTemplet> m_lstUnitInteractionCache;

	private List<NKCOfficeUnitInteractionTemplet> m_lstSoloInteractionCache;

	Animator INKCAnimationActor.Animator => m_animator;

	Transform INKCAnimationActor.SDParent => m_trSDParent;

	Transform INKCAnimationActor.Transform => base.transform;

	public NKCOfficeBuildingBase OfficeBuilding => m_OfficeBuilding;

	private NKCOfficeFloorBase Floor => m_OfficeBuilding.m_Floor;

	private long[,] FloorMap => m_OfficeBuilding?.FloorMap;

	public int UnitID { get; private set; }

	public int SkinID { get; private set; }

	public bool PlayingInteractionAnimation { get; private set; }

	public NKCOfficeFuniture CurrentInteractionTargetFurniture { get; private set; }

	public NKCOfficeFurnitureInteractionTemplet CurrentFurnitureInteractionTemplet { get; private set; }

	public NKCOfficeUnitInteractionTemplet CurrentUnitInteractionTemplet { get; private set; }

	public NKCOfficeCharacter CurrentUnitInteractionTarget { get; private set; }

	public Vector3 CurrentUnitInteractionPosition { get; private set; }

	public bool CurrentUnitInteractionIsMainActor { get; private set; }

	public List<NKCOfficeUnitInteractionTemplet> UnitInteractionCache
	{
		get
		{
			if (m_lstUnitInteractionCache == null)
			{
				BuildInteractionCache();
			}
			return m_lstUnitInteractionCache;
		}
	}

	public List<NKCOfficeUnitInteractionTemplet> SoloInteractionCache
	{
		get
		{
			if (m_lstSoloInteractionCache == null)
			{
				BuildInteractionCache();
			}
			return m_lstSoloInteractionCache;
		}
	}

	public static NKCOfficeCharacter GetInstance(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return null;
		}
		return GetInstance(unitData.m_UnitID, unitData.m_SkinID);
	}

	public static NKCOfficeCharacter GetInstance(int unitID, int skinID)
	{
		NKCOfficeCharacterTemplet nKCOfficeCharacterTemplet = NKCOfficeCharacterTemplet.Find(unitID, skinID);
		NKMAssetName cNKMAssetName = ((nKCOfficeCharacterTemplet == null || string.IsNullOrEmpty(nKCOfficeCharacterTemplet.PrefabAsset)) ? new NKMAssetName("AB_UNIT_OFFICE_SD", "UNIT_OFFICE_SD") : new NKMAssetName("AB_UNIT_OFFICE_SD", nKCOfficeCharacterTemplet.PrefabAsset));
		GameObject gameObject = Object.Instantiate(NKCResourceUtility.GetOrLoadAssetResource<GameObject>(cNKMAssetName));
		NKCOfficeCharacter component = gameObject.GetComponent<NKCOfficeCharacter>();
		if (component == null)
		{
			Debug.LogError("NKCUIOfficeCharacter loadprefab failed!");
			Object.DestroyImmediate(gameObject);
			return null;
		}
		component.SetSpineIllust(NKCResourceUtility.OpenSpineSD(unitID, skinID), bSetParent: true);
		if (component.m_SDIllust != null)
		{
			component.m_SDIllust.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE);
			component.m_SDIllust.GetRectTransform().localPosition = Vector3.zero;
			component.m_SDIllust.GetRectTransform().pivot = new Vector2(0.5f, 0.5f);
			component.m_SDIllust.GetRectTransform().anchorMin = new Vector2(0.5f, 0.5f);
			component.m_SDIllust.GetRectTransform().anchorMax = new Vector2(0.5f, 0.5f);
			component.m_SDIllust.GetRectTransform().SetWidth(100f);
			component.m_SDIllust.GetRectTransform().SetHeight(100f);
			component.transform.rotation = Quaternion.identity;
			return component;
		}
		Debug.LogError($"SD Illust load failed!! unitID {unitID}, skinID {skinID}");
		Object.DestroyImmediate(gameObject);
		return null;
	}

	public static NKCOfficeCharacter GetInstance(NKMAssetName sdAssetName)
	{
		GameObject gameObject = Object.Instantiate(NKCResourceUtility.GetOrLoadAssetResource<GameObject>(new NKMAssetName("AB_UNIT_OFFICE_SD", "UNIT_OFFICE_SD")));
		NKCOfficeCharacter component = gameObject.GetComponent<NKCOfficeCharacter>();
		if (component == null)
		{
			Debug.LogError("NKCUIOfficeCharacter loadprefab failed!");
			Object.DestroyImmediate(gameObject);
			return null;
		}
		component.SetSpineIllust(NKCResourceUtility.OpenSpineSD(sdAssetName.m_BundleName, sdAssetName.m_BundleName), bSetParent: true);
		if (component.m_SDIllust != null)
		{
			component.m_SDIllust.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE);
			component.m_SDIllust.GetRectTransform().localPosition = Vector3.zero;
		}
		component.transform.rotation = Quaternion.identity;
		return component;
	}

	public void Init(NKCOfficeBuildingBase officeBuilding, NKMUnitData unitData)
	{
		CommonInit(officeBuilding, unitData.m_UnitID, unitData.m_SkinID);
		m_UnitData = unitData;
		NKCUtil.SetGameobjectActive(m_comLoyalty, unitData != null);
		if (unitData != null && m_comLoyalty != null)
		{
			m_comLoyalty.SetData(m_UnitData);
		}
		NKCUtil.SetGameobjectActive(m_comFriendInfo, bValue: false);
		m_FriendUID = 0L;
	}

	public void Init(NKCOfficeBuildingBase officeBuilding, NKMUserProfileData profileData)
	{
		CommonInit(officeBuilding, profileData.commonProfile.mainUnitId, profileData.commonProfile.mainUnitSkinId);
		NKCUtil.SetGameobjectActive(m_comLoyalty, bValue: false);
		NKCUtil.SetGameobjectActive(m_comFriendInfo, bValue: true);
		m_FriendUID = profileData.commonProfile.userUid;
		m_comFriendInfo.SetData(profileData);
	}

	public void Init(NKCOfficeBuildingBase officeBuilding, int unitID, int skinID)
	{
		CommonInit(officeBuilding, unitID, skinID);
		NKCUtil.SetGameobjectActive(m_comLoyalty, bValue: false);
		NKCUtil.SetGameobjectActive(m_comFriendInfo, bValue: false);
		m_FriendUID = 0L;
	}

	private void CommonInit(NKCOfficeBuildingBase officeBuilding, int unitID, int skinID)
	{
		UnitID = unitID;
		SkinID = skinID;
		m_bTakeHeartSent = false;
		m_OfficeBuilding = officeBuilding;
		BT = GetComponent<BehaviorTree>();
		if (BT == null)
		{
			Debug.LogWarning("Office SD : BT Not found. Using Default BT");
			BT = base.gameObject.AddComponent<BehaviorTree>();
			BT.StartWhenEnabled = false;
		}
		else
		{
			BT.StartWhenEnabled = false;
		}
		BT.DisableBehavior();
		NKCOfficeCharacterTemplet nKCOfficeCharacterTemplet = NKCOfficeCharacterTemplet.Find(unitID, skinID);
		if (nKCOfficeCharacterTemplet != null && !string.IsNullOrEmpty(nKCOfficeCharacterTemplet.BTAsset))
		{
			ExternalBehavior externalBehavior = LoadBT("ab_ui_office_bt", nKCOfficeCharacterTemplet.BTAsset);
			if (externalBehavior != null)
			{
				BT.ExternalBehavior = externalBehavior;
				Log.Info("[NKCOfficeCharacter] External Behaivor " + nKCOfficeCharacterTemplet.BTAsset + " loaded", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Office/NKCOfficeCharacter.cs", 248);
				NKCDebugUtil.LogBehaivorTree(BT);
			}
			else
			{
				Log.Warn("[NKCOfficeCharacter] External Behaivor " + nKCOfficeCharacterTemplet.BTAsset + " load failed, using default", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Office/NKCOfficeCharacter.cs", 255);
				externalBehavior = LoadBT("ab_ui_office_bt", "OFFICE_BT_DEFAULT");
				BT.ExternalBehavior = externalBehavior;
			}
		}
		else
		{
			ExternalBehavior externalBehavior2 = LoadBT("ab_ui_office_bt", "OFFICE_BT_DEFAULT");
			BT.ExternalBehavior = externalBehavior2;
		}
		AddCommonBTVariable();
		ApplyBTVariables(nKCOfficeCharacterTemplet);
		BT.RestartWhenComplete = true;
		BT.OnBehaviorRestart += OnBTRestart;
		base.transform.SetParent(officeBuilding.trActorRoot);
		if (m_comEmotion != null)
		{
			m_comEmotion.Init();
		}
		m_lstUnitInteractionCache = null;
		m_lstSoloInteractionCache = null;
		SetCheckIntervalCooltime();
		SetShadow(value: true);
	}

	private ExternalBehavior LoadBT(string bundleName, string assetName)
	{
		ExternalBehavior orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<ExternalBehavior>(bundleName, assetName);
		if (orLoadAssetResource == null)
		{
			return null;
		}
		ExternalBehavior externalBehavior = Object.Instantiate(orLoadAssetResource);
		externalBehavior.Init();
		return externalBehavior;
	}

	private void AddCommonBTVariable()
	{
		if (BT.GetVariable("GrabEmotion") == null)
		{
			BT.SetVariable("GrabEmotion", new SharedString());
		}
	}

	private void ApplyBTVariables(NKCOfficeCharacterTemplet templet)
	{
		if (templet == null || string.IsNullOrEmpty(templet.Variables))
		{
			return;
		}
		foreach (KeyValuePair<string, string> item in NKCUtil.ParseStringTable(templet.Variables))
		{
			string key = item.Key;
			string value = item.Value;
			SharedVariable variable = BT.GetVariable(key);
			if (variable == null)
			{
				Debug.LogError("BT variable not found! name : " + key);
				continue;
			}
			Debug.Log("Set Variable <" + key + "> : " + value);
			if (variable is SharedString)
			{
				BT.SetVariableValue(key, value.Trim());
			}
			else if (variable is SharedInt)
			{
				if (int.TryParse(value, out var result))
				{
					BT.SetVariableValue(key, result);
					continue;
				}
				Debug.LogError($"[OfficeCharacterTemplet] {templet.Type} {templet.ID} BT Variable parse failed : {key} not followed by int(param {value}).");
			}
			else if (variable is SharedBool)
			{
				if (bool.TryParse(value, out var result2))
				{
					BT.SetVariableValue(key, result2);
					continue;
				}
				Debug.LogError($"[OfficeCharacterTemplet] {templet.Type} {templet.ID} BT Variable parse failed : {key} not followed by bool(param {value}).");
			}
			else if (variable is SharedFloat)
			{
				if (float.TryParse(value, out var result3))
				{
					BT.SetVariableValue(key, result3);
					continue;
				}
				Debug.LogError($"OfficeCharacterTemplet : {templet.Type} {templet.ID} BT Variable parse failed : {key} not followed by float(param {value}).");
			}
			else if (variable is BTSharedNKCValue)
			{
				BTSharedNKCValue bTSharedNKCValue = variable as BTSharedNKCValue;
				if (bTSharedNKCValue.TryParse(value))
				{
					BT.SetVariable(key, bTSharedNKCValue);
					continue;
				}
				Debug.LogError($"OfficeCharacterTemplet : {templet.Type} {templet.ID} BT Variable parse {key} failed(param {value}).");
			}
			else if (variable is SharedUInt)
			{
				if (uint.TryParse(value, out var result4))
				{
					BT.SetVariableValue(key, result4);
					continue;
				}
				Debug.LogError($"OfficeCharacterTemplet : {templet.Type} {templet.ID} BT Variable parse failed : {key} not followed by uint(param {value}).");
			}
			else
			{
				Debug.LogError($"OfficeCharacterTemplet : {templet.Type} {templet.ID} BT Variable parse failed - Unexpected type");
			}
		}
	}

	public void SetEnableExtraUI(bool value)
	{
		if (value)
		{
			NKCUtil.SetGameobjectActive(m_comLoyalty, m_UnitData != null);
			if (m_UnitData != null && m_comLoyalty != null)
			{
				m_comLoyalty.SetData(m_UnitData);
			}
			NKCUtil.SetGameobjectActive(m_comFriendInfo, m_FriendUID > 0);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_comLoyalty, bValue: false);
			NKCUtil.SetGameobjectActive(m_comFriendInfo, bValue: false);
		}
	}

	public void OnUnitUpdated(NKMUnitData unitData)
	{
		m_UnitData = unitData;
		if (m_comLoyalty != null)
		{
			m_comLoyalty.SetData(m_UnitData);
		}
	}

	public void OnUnitTakeHeart(NKMUnitData unitData)
	{
		m_bTakeHeartSent = false;
		m_UnitData = unitData;
		if (m_comLoyalty != null)
		{
			m_comLoyalty.SetData(m_UnitData);
			m_comLoyalty.PlayTakeHeartEffect();
		}
		SetState(State.AI, bForce: true);
	}

	public void Cleanup()
	{
		UnregisterInteraction();
		CleanupAnimInstances();
		CleanupCharacter();
	}

	private void CleanupCharacter()
	{
		if (m_SDIllust != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_SDIllust);
		}
		m_SDIllust = null;
	}

	protected void OnBTRestart(Behavior behavior)
	{
		CleanupAnimInstances();
	}

	public void StopAllAnimInstances()
	{
		m_qAnimInstances.Clear();
		if (m_animInstance != null)
		{
			m_lstFinishedInstances.Add(m_animInstance);
		}
		if (m_emotionAnimInstance != null)
		{
			m_lstFinishedInstances.Add(m_emotionAnimInstance);
		}
		m_animInstance = null;
		m_emotionAnimInstance = null;
		SetShadow(value: true);
		CleanupFinishedInstances();
	}

	private void CleanupAnimInstances()
	{
		m_qAnimInstances.Clear();
		if (m_animInstance != null)
		{
			m_animInstance.RemoveEffect();
			m_animInstance = null;
		}
		if (m_emotionAnimInstance != null)
		{
			m_emotionAnimInstance.RemoveEffect();
			m_emotionAnimInstance = null;
		}
		CleanupFinishedInstances();
	}

	private void CleanupFinishedInstances()
	{
		foreach (NKCAnimationInstance lstFinishedInstance in m_lstFinishedInstances)
		{
			lstFinishedInstance?.RemoveEffect();
		}
		m_lstFinishedInstances.Clear();
	}

	private void SetCheckIntervalCooltime()
	{
		m_fInteractionCheckInterval = Random.Range(1, 2);
	}

	public void SetFurnitureInteractionCooltime()
	{
		m_fFurnitureInteractionCooltime = Random.Range(NKMCommonConst.Office.OfficeInteraction.ActInteriorCoolTime, NKMCommonConst.Office.OfficeInteraction.ActInteriorCoolTime * 1.5f);
		SetCheckIntervalCooltime();
	}

	public void ResetUnitInteractionCooltime()
	{
		m_fUnitInteractionCooltime = 0f;
	}

	public void SetUnitInteractionCooltime()
	{
		m_fUnitInteractionCooltime = Random.Range(NKMCommonConst.Office.OfficeInteraction.ActUnitCoolTime, NKMCommonConst.Office.OfficeInteraction.ActUnitCoolTime * 1.5f);
		SetCheckIntervalCooltime();
	}

	public void SetSoloInteractionCooltime()
	{
		m_fSoloInteractionCooltime = Random.Range(NKMCommonConst.Office.OfficeInteraction.ActSoloCoolTime, NKMCommonConst.Office.OfficeInteraction.ActSoloCoolTime * 1.5f);
		SetCheckIntervalCooltime();
	}

	protected virtual void Update()
	{
		m_bRectWorldValid = false;
		m_fInteractionCheckInterval -= Time.deltaTime;
		m_fFurnitureInteractionCooltime -= Time.deltaTime;
		m_fUnitInteractionCooltime -= Time.deltaTime;
		m_fSoloInteractionCooltime -= Time.deltaTime;
		switch (m_eState)
		{
		case State.AI:
			if (m_animInstance != null)
			{
				if (m_animInstance.IsFinished())
				{
					m_lstFinishedInstances.Add(m_animInstance);
					m_animInstance = null;
				}
				else
				{
					m_animInstance.Update(Time.deltaTime);
				}
			}
			if (m_emotionAnimInstance != null)
			{
				if (m_emotionAnimInstance.IsFinished())
				{
					m_lstFinishedInstances.Add(m_emotionAnimInstance);
					m_emotionAnimInstance = null;
				}
				else
				{
					m_emotionAnimInstance.Update(Time.deltaTime);
				}
			}
			if (m_animInstance == null && m_qAnimInstances.Count > 0)
			{
				m_animInstance = m_qAnimInstances.Dequeue();
			}
			if (!HasInteractionTarget() && m_fInteractionCheckInterval <= 0f)
			{
				SetCheckIntervalCooltime();
				CheckFurnitureInteraction();
				CheckUnitInteraction();
				CheckSoloInteraction();
			}
			break;
		case State.WaitGrab:
			if (Input.touchCount > 1)
			{
				SetState(State.AI);
			}
			else if (m_fGrabWaitTime < GRAB_WAIT_TIME)
			{
				m_fGrabWaitTime += Time.deltaTime;
			}
			else if (m_bCanGrab)
			{
				if (!NKCUIHoldLoading.IsOpen)
				{
					NKCUIHoldLoading.Instance.Open(m_touchPos, grabUITime);
				}
				else if (!NKCUIHoldLoading.Instance.IsPlaying())
				{
					SetState(State.Grab);
				}
			}
			break;
		}
	}

	public void EnqueueAnimation(List<NKCAnimationEventTemplet> lstAnimEvent)
	{
		EnqueueAnimation(new NKCAnimationInstance(this, m_OfficeBuilding.transform, lstAnimEvent, base.transform.localPosition, base.transform.localPosition));
	}

	public void EnqueueAnimation(NKCAnimationInstance instance)
	{
		m_qAnimInstances.Enqueue(instance);
	}

	private void PlayEmotionAnimation(List<NKCAnimationEventTemplet> lstAnim)
	{
		PlayEmotionAnimation(new NKCAnimationInstance(this, m_OfficeBuilding.transform, lstAnim, base.transform.localPosition, base.transform.localPosition));
	}

	private void PlayEmotionAnimation(NKCAnimationInstance instance)
	{
		if (m_emotionAnimInstance != null)
		{
			m_lstFinishedInstances.Add(m_emotionAnimInstance);
			m_emotionAnimInstance = null;
		}
		m_emotionAnimInstance = instance;
	}

	public bool PlayAnimCompleted()
	{
		if (m_qAnimInstances.Count == 0)
		{
			return m_animInstance == null;
		}
		return false;
	}

	private void ClearHolding()
	{
		if (NKCUIHoldLoading.IsOpen)
		{
			NKCUIHoldLoading.Instance.Close();
		}
		m_fGrabWaitTime = 0f;
	}

	public void StartAI()
	{
		SetState(State.AI, bForce: true);
	}

	private void SetState(State state, bool bForce = false)
	{
		if (!bForce && state == m_eState)
		{
			return;
		}
		ClearHolding();
		bool flag = false;
		bool flag2 = false;
		if (state != State.AI)
		{
			if (PlayingInteractionAnimation)
			{
				UnregisterInteraction();
			}
			StopAllAnimInstances();
			BT.DisableBehavior();
			if (m_comEmotion != null)
			{
				m_comEmotion.Stop();
			}
		}
		if (m_eState == State.Grab)
		{
			base.transform.SetParent(m_OfficeBuilding.trActorRoot, worldPositionStays: true);
			PlayEmotion(NKCUIComCharacterEmotion.Type.NONE);
			flag2 = true;
		}
		switch (state)
		{
		case State.AI:
			BT.EnableBehavior();
			BT.Start();
			break;
		case State.WaitGrab:
			PlaySpineAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE, loop: true, 1f, bDefaultAnim: false);
			m_fGrabWaitTime = 0f;
			break;
		case State.Grab:
		{
			PlaySpineAnimation(NKCASUIUnitIllust.eAnimation.SD_DRAG, loop: true, 1f, bDefaultAnim: false);
			string bTValue = GetBTValue("GrabEmotion", "");
			if (string.IsNullOrEmpty(bTValue))
			{
				PlayEmotion(NKCUIComCharacterEmotion.Type.Sweat);
			}
			else
			{
				PlayEmotion(bTValue);
			}
			base.transform.SetParent(Floor.m_rtSelectedFunitureRoot, worldPositionStays: true);
			flag = true;
			UnregisterInteraction();
			break;
		}
		}
		m_eState = state;
		if (flag)
		{
			m_OfficeBuilding.OnCharacterBeginDrag(this);
		}
		else if (flag2)
		{
			m_OfficeBuilding.OnCharacterEndDrag(this);
		}
	}

	public Vector3 GetLocalPos((int, int) pos, bool bRandomize = true)
	{
		return GetLocalPos(pos.Item1, pos.Item2, bRandomize);
	}

	public Vector3 GetLocalPos(OfficeFloorPosition pos, bool bRandomize = true)
	{
		return GetLocalPos(pos.x, pos.y, bRandomize);
	}

	public Vector3 GetLocalPos(int x, int y, bool bRandomize = true)
	{
		Vector3 localPos = Floor.GetLocalPos(x, y);
		if (bRandomize)
		{
			localPos.x += NKMRandom.Range((0f - m_OfficeBuilding.TileSize) * 0.25f, m_OfficeBuilding.TileSize * 0.25f);
			localPos.y += NKMRandom.Range((0f - m_OfficeBuilding.TileSize) * 0.25f, m_OfficeBuilding.TileSize * 0.25f);
		}
		return localPos;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!m_bTakeHeartSent && m_eState == State.AI)
		{
			SetState(State.WaitGrab);
			m_touchPos = eventData.position;
			Vector3 localPosFromScreenPos = Floor.GetLocalPosFromScreenPos(m_touchPos);
			m_touchLocalOffset = base.transform.localPosition - localPosFromScreenPos;
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (m_eState == State.WaitGrab && m_fGrabWaitTime < GRAB_WAIT_TIME && !OnTouchAction() && m_bCanTouch)
		{
			PlayTouchAnimation();
			PlayTouchVoice();
		}
		SetState(State.AI);
	}

	protected virtual bool OnTouchAction()
	{
		if (m_bTakeHeartSent)
		{
			return true;
		}
		if (m_UnitData != null && m_UnitData.CheckOfficeRoomHeartFull())
		{
			PlayTakeHeartAnimation();
			PlayTakeHeartVoice();
			m_bTakeHeartSent = true;
			NKCPacketSender.Send_NKMPacket_OFFICE_TAKE_HEART_REQ(m_UnitData.m_UnitUID);
			return true;
		}
		if (m_UnitData == null && m_FriendUID > 0)
		{
			NKCPacketSender.Send_NKMPacket_USER_PROFILE_INFO_REQ(m_FriendUID, NKM_DECK_TYPE.NDT_NORMAL);
			return true;
		}
		if (dOnClick != null && dOnClick())
		{
			return true;
		}
		return false;
	}

	protected virtual void PlayTouchVoice()
	{
		if (m_UnitData != null)
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_TOUCH, m_UnitData, bIgnoreShowNormalAfterLifeTimeOption: false, bShowCaption: true);
		}
	}

	private void PlayTouchAnimation()
	{
		EnqueueSimpleAni(NKCASUIUnitIllust.eAnimation.SD_TOUCH, bNow: false);
	}

	private void PlayTakeHeartVoice()
	{
		_ = m_UnitData;
	}

	private void PlayTakeHeartAnimation()
	{
		EnqueueSimpleAni(NKCASUIUnitIllust.eAnimation.SD_WIN, bNow: true);
	}

	public void EnqueueSimpleAni(string animName, bool bNow, bool bInvert)
	{
		if (bNow)
		{
			StopAllAnimInstances();
		}
		float animationTime = m_SDIllust.GetAnimationTime(animName);
		string aniEventStrID = "SimpleAni" + animName;
		List<NKCAnimationEventTemplet> list = new List<NKCAnimationEventTemplet>();
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = aniEventStrID,
			m_StartTime = 0f,
			m_AniEventType = AnimationEventType.INVERT_MODEL_X,
			m_BoolValue = bInvert
		});
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = aniEventStrID,
			m_StartTime = 0f,
			m_AniEventType = AnimationEventType.ANIMATION_NAME_SPINE,
			m_StrValue = animName,
			m_FloatValue = 1f,
			m_BoolValue = false
		});
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = aniEventStrID,
			m_StartTime = 0f,
			m_AniEventType = AnimationEventType.SET_MOVE_SPEED,
			m_FloatValue = 0f
		});
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = aniEventStrID,
			m_StartTime = 0f,
			m_AniEventType = AnimationEventType.SET_POSITION,
			m_FloatValue = 0f
		});
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = aniEventStrID,
			m_StartTime = animationTime,
			m_AniEventType = AnimationEventType.SET_POSITION,
			m_FloatValue = 1f
		});
		NKCAnimationInstance instance = new NKCAnimationInstance(this, m_OfficeBuilding.transform, list, base.transform.localPosition, base.transform.localPosition);
		EnqueueAnimation(instance);
	}

	private void EnqueueSimpleAni(NKCASUIUnitIllust.eAnimation animation, bool bNow)
	{
		if (bNow)
		{
			StopAllAnimInstances();
		}
		float animationTime = m_SDIllust.GetAnimationTime(animation);
		string aniEventStrID = "SimpleAni" + animation;
		List<NKCAnimationEventTemplet> list = new List<NKCAnimationEventTemplet>();
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = aniEventStrID,
			m_StartTime = 0f,
			m_AniEventType = AnimationEventType.ANIMATION_SPINE,
			m_StrValue = animation.ToString(),
			m_FloatValue = 1f,
			m_BoolValue = false
		});
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = aniEventStrID,
			m_StartTime = 0f,
			m_AniEventType = AnimationEventType.SET_MOVE_SPEED,
			m_FloatValue = 0f
		});
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = aniEventStrID,
			m_StartTime = 0f,
			m_AniEventType = AnimationEventType.SET_POSITION,
			m_FloatValue = 0f
		});
		list.Add(new NKCAnimationEventTemplet
		{
			m_AniEventStrID = aniEventStrID,
			m_StartTime = animationTime,
			m_AniEventType = AnimationEventType.SET_POSITION,
			m_FloatValue = 1f
		});
		NKCAnimationInstance instance = new NKCAnimationInstance(this, m_OfficeBuilding.transform, list, base.transform.localPosition, base.transform.localPosition);
		EnqueueAnimation(instance);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (m_eState == State.Grab)
		{
			Vector3 localPos = Floor.GetLocalPosFromScreenPos(eventData.position) + m_touchLocalOffset;
			localPos = Floor.Rect.ClampLocalPos(localPos);
			base.transform.localPosition = localPos;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		SetState(State.AI);
	}

	private void DebugDrawRoute()
	{
		foreach (NKCAnimationInstance lstFinishedInstance in m_lstFinishedInstances)
		{
			lstFinishedInstance?.DrawDebugLine(Color.gray);
		}
		foreach (NKCAnimationInstance qAnimInstance in m_qAnimInstances)
		{
			qAnimInstance.DrawDebugLine(Color.green);
		}
	}

	public void SetSpineIllust(NKCASUIUnitIllust illust, bool bSetParent)
	{
		if (illust != null)
		{
			if (bSetParent)
			{
				illust.SetParent(m_trSDParent, worldPositionStays: false);
			}
			m_SDIllust = illust;
		}
	}

	public NKCASUIUnitIllust GetSpineIllust()
	{
		return m_SDIllust;
	}

	public void PlaySpineAnimation(string name, bool loop, float timeScale)
	{
		if (m_SDIllust != null)
		{
			m_SDIllust.SetAnimation(name, loop, 0, bForceRestart: true, 0f, bReturnDefault: false);
			m_SDIllust.SetTimeScale(timeScale);
			m_SDIllust.InvalidateWorldRect();
		}
	}

	public void PlaySpineAnimation(NKCASUIUnitIllust.eAnimation eAnim, bool loop, float timeScale, bool bDefaultAnim)
	{
		if (m_SDIllust != null)
		{
			if (bDefaultAnim)
			{
				m_SDIllust.SetDefaultAnimation(eAnim);
			}
			else
			{
				m_SDIllust.SetAnimation(eAnim, loop, 0, bForceRestart: true, 0f, bReturnDefault: false);
			}
			m_SDIllust.SetTimeScale(timeScale);
			m_SDIllust.InvalidateWorldRect();
		}
	}

	public bool IsSpineAnimationFinished(NKCASUIUnitIllust.eAnimation eAnim)
	{
		if (m_SDIllust == null)
		{
			return true;
		}
		string animationName = NKCASUIUnitIllust.GetAnimationName(eAnim);
		return IsSpineAnimationFinished(animationName);
	}

	public bool IsSpineAnimationFinished(string name)
	{
		if (m_SDIllust == null)
		{
			return true;
		}
		if (m_SDIllust.GetCurrentAnimationName() == name && m_SDIllust.GetAnimationTime(name) > m_SDIllust.GetCurrentAnimationTime())
		{
			return false;
		}
		return true;
	}

	public Vector3 GetBonePosition(string name)
	{
		if (m_SDIllust == null)
		{
			return Vector3.zero;
		}
		return m_SDIllust.GetBoneWorldPosition(name);
	}

	public bool CanPlaySpineAnimation(string name)
	{
		if (m_SDIllust != null)
		{
			return m_SDIllust.HasAnimation(name);
		}
		return false;
	}

	public bool CanPlaySpineAnimation(NKCASUIUnitIllust.eAnimation eAnim)
	{
		if (m_SDIllust != null)
		{
			return m_SDIllust.HasAnimation(eAnim);
		}
		return false;
	}

	public void SetEnableTouch(bool value)
	{
		if (m_gpTouchTarget != null)
		{
			m_gpTouchTarget.raycastTarget = value;
		}
	}

	public float GetAnimTime(string animName)
	{
		if (m_SDIllust != null)
		{
			return m_SDIllust.GetAnimationTime(animName);
		}
		return 0f;
	}

	public float GetAnimTime(NKCASUIUnitIllust.eAnimation anim)
	{
		if (m_SDIllust != null)
		{
			return m_SDIllust.GetAnimationTime(anim);
		}
		return 0f;
	}

	public string GetCurrentAnimName(int trackIndex = 0)
	{
		if (m_SDIllust != null)
		{
			m_SDIllust.GetCurrentAnimationName(trackIndex);
		}
		return "";
	}

	public void SetOnClick(OnClick onClick)
	{
		dOnClick = onClick;
	}

	public void SetShadow(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objShadow, value);
	}

	public T GetBTValue<T>(string valueName, T defaultValue)
	{
		if (BT == null)
		{
			return defaultValue;
		}
		object value = BT.GetVariable(valueName).GetValue();
		if (value is T)
		{
			return (T)value;
		}
		return defaultValue;
	}

	public Rect GetWorldRect()
	{
		if (!m_bRectWorldValid)
		{
			Rect rectWorld = ((m_SDIllust != null) ? m_SDIllust.GetWorldRect() : ((!(m_gpTouchTarget != null)) ? new Rect(base.transform.position.x, base.transform.position.y, 141f, 315f) : m_gpTouchTarget.GetComponent<RectTransform>().GetWorldRect()));
			if (m_comLoyalty != null && m_comLoyalty.gameObject.activeInHierarchy)
			{
				RectTransform component = m_comLoyalty.GetComponent<RectTransform>();
				if (component != null)
				{
					Rect worldRect = component.GetWorldRect();
					rectWorld.yMax = Mathf.Max(rectWorld.yMax, worldRect.yMax);
				}
			}
			else if (m_comFriendInfo != null && m_comFriendInfo.gameObject.activeInHierarchy)
			{
				RectTransform component2 = m_comFriendInfo.GetComponent<RectTransform>();
				if (component2 != null)
				{
					Rect worldRect2 = component2.GetWorldRect();
					rectWorld.xMin = Mathf.Min(rectWorld.xMin, worldRect2.xMin);
					rectWorld.xMax = Mathf.Max(rectWorld.xMax, worldRect2.xMax);
					rectWorld.yMax = Mathf.Max(rectWorld.yMax, worldRect2.yMax);
				}
			}
			m_bRectWorldValid = true;
			m_rectWorld = rectWorld;
		}
		return m_rectWorld;
	}

	private void BuildInteractionCache()
	{
		List<NKCOfficeUnitInteractionTemplet> interactionTempletList = NKCOfficeUnitInteractionTemplet.GetInteractionTempletList(this);
		if (interactionTempletList == null)
		{
			m_lstUnitInteractionCache = new List<NKCOfficeUnitInteractionTemplet>();
			m_lstSoloInteractionCache = new List<NKCOfficeUnitInteractionTemplet>();
		}
		m_lstUnitInteractionCache = interactionTempletList.FindAll((NKCOfficeUnitInteractionTemplet x) => !x.IsSoloAction);
		m_lstSoloInteractionCache = interactionTempletList.FindAll((NKCOfficeUnitInteractionTemplet x) => x.IsSoloAction);
	}

	public bool RegisterInteraction(NKCOfficeFuniture furniture, NKCOfficeFurnitureInteractionTemplet templet)
	{
		if (templet == null)
		{
			return false;
		}
		List<NKCAnimationEventTemplet> lstAnim = NKCAnimationEventManager.Find(templet.UnitAni);
		if (NKCAnimationEventManager.IsEmotionOnly(lstAnim))
		{
			PlayEmotionAnimation(lstAnim);
		}
		else
		{
			BT.DisableBehavior();
			StopAllAnimInstances();
			CurrentInteractionTargetFurniture = furniture;
			CurrentFurnitureInteractionTemplet = templet;
			StartAI();
			if (templet.eActType == NKCOfficeFurnitureInteractionTemplet.ActType.Common)
			{
				furniture.RegisterInteractionCharacter(this);
			}
		}
		if (templet.eActType == NKCOfficeFurnitureInteractionTemplet.ActType.Common)
		{
			SetFurnitureInteractionCooltime();
		}
		return true;
	}

	public bool RegisterInteraction(NKCOfficeUnitInteractionTemplet soloTemplet)
	{
		if (soloTemplet == null)
		{
			return false;
		}
		if (!soloTemplet.IsSoloAction)
		{
			Debug.LogError("Logic Error!");
			return false;
		}
		List<NKCAnimationEventTemplet> lstAnim = NKCAnimationEventManager.Find(soloTemplet.ActorAni);
		if (NKCAnimationEventManager.IsEmotionOnly(lstAnim))
		{
			PlayEmotionAnimation(lstAnim);
		}
		else
		{
			BT.DisableBehavior();
			StopAllAnimInstances();
			CurrentUnitInteractionTemplet = soloTemplet;
			StartAI();
		}
		SetSoloInteractionCooltime();
		return true;
	}

	public bool RegisterInteraction(NKCOfficeUnitInteractionTemplet templet, NKCOfficeCharacter targetCharacter, bool IsMainActor, Vector3 actionPosition)
	{
		if (templet == null)
		{
			return false;
		}
		List<NKCAnimationEventTemplet> lstAnim = NKCAnimationEventManager.Find(IsMainActor ? templet.ActorAni : templet.TargetAni);
		bool flag = NKCAnimationEventManager.IsEmotionOnly(lstAnim);
		if (!templet.AlignUnit && flag)
		{
			PlayEmotionAnimation(lstAnim);
		}
		else
		{
			BT.DisableBehavior();
			StopAllAnimInstances();
			CurrentUnitInteractionTemplet = templet;
			CurrentUnitInteractionTarget = targetCharacter;
			CurrentUnitInteractionIsMainActor = IsMainActor;
			CurrentUnitInteractionPosition = actionPosition;
			StartAI();
		}
		SetUnitInteractionCooltime();
		return true;
	}

	public void UnregisterInteraction()
	{
		if (PlayingInteractionAnimation)
		{
			StopAllAnimInstances();
		}
		if (CurrentInteractionTargetFurniture != null)
		{
			CurrentInteractionTargetFurniture.CleanupInteraction();
			SetFurnitureInteractionCooltime();
		}
		if (CurrentUnitInteractionTemplet != null)
		{
			if (CurrentUnitInteractionTemplet.IsSoloAction)
			{
				SetSoloInteractionCooltime();
			}
			else
			{
				SetUnitInteractionCooltime();
			}
		}
		base.transform.SetParent(m_OfficeBuilding.trActorRoot);
		base.transform.localScale = Vector3.one;
		base.transform.rotation = Quaternion.identity;
		Vector3 localPosition = m_OfficeBuilding.m_Floor.m_rtFunitureRoot.ProjectPointToPlane(base.transform.position);
		base.transform.localPosition = localPosition;
		PlayingInteractionAnimation = false;
		CurrentInteractionTargetFurniture = null;
		CurrentFurnitureInteractionTemplet = null;
		CurrentUnitInteractionTemplet = null;
		CurrentUnitInteractionTarget = null;
		SetShadow(value: true);
	}

	public bool IsUnitInteractTargetable()
	{
		if (m_eState != State.AI)
		{
			return false;
		}
		if (m_fUnitInteractionCooltime > 0f)
		{
			return false;
		}
		if (HasInteractionTarget())
		{
			return false;
		}
		return true;
	}

	public bool HasInteractionTarget()
	{
		if (CurrentInteractionTargetFurniture != null && CurrentFurnitureInteractionTemplet != null)
		{
			return true;
		}
		if (CurrentUnitInteractionTemplet != null)
		{
			return true;
		}
		return false;
	}

	public void SetPlayingInteractionAnimation(bool value)
	{
		PlayingInteractionAnimation = value;
		if (value && CurrentInteractionTargetFurniture != null && CurrentFurnitureInteractionTemplet.eActType == NKCOfficeFurnitureInteractionTemplet.ActType.Common)
		{
			GameObject interactionPoint = CurrentInteractionTargetFurniture.GetInteractionPoint();
			if (interactionPoint != null)
			{
				base.transform.SetParent(interactionPoint.transform);
			}
			CurrentInteractionTargetFurniture.InvalidateWorldRect();
		}
	}

	public Vector3 GetInteractionPosition()
	{
		if (CurrentFurnitureInteractionTemplet != null)
		{
			if (CurrentFurnitureInteractionTemplet.eActType == NKCOfficeFurnitureInteractionTemplet.ActType.Reaction)
			{
				return base.transform.localPosition;
			}
			if (CurrentInteractionTargetFurniture != null)
			{
				GameObject interactionPoint = CurrentInteractionTargetFurniture.GetInteractionPoint();
				if (interactionPoint != null)
				{
					return m_OfficeBuilding.m_Floor.Rect.InverseTransformPoint(interactionPoint.transform.position);
				}
				return base.transform.localPosition;
			}
		}
		if (CurrentUnitInteractionTemplet != null)
		{
			if (CurrentUnitInteractionTemplet.IsSoloAction)
			{
				return base.transform.localPosition;
			}
			return CurrentUnitInteractionPosition;
		}
		return base.transform.localPosition;
	}

	private void CheckFurnitureInteraction()
	{
		if (!(m_fFurnitureInteractionCooltime > 0f) && !HasInteractionTarget() && Random.Range(0, 100) < NKMCommonConst.Office.OfficeInteraction.ActInteriorRatePercent)
		{
			NKCOfficeFuniture nKCOfficeFuniture = m_OfficeBuilding.FindInteractableInterior(this);
			if (!(nKCOfficeFuniture == null))
			{
				NKCOfficeManager.PlayInteraction(this, nKCOfficeFuniture);
			}
		}
	}

	private void CheckUnitInteraction()
	{
		if (!HasInteractionTarget() && !(m_fUnitInteractionCooltime > 0f) && Random.Range(0, 100) < NKMCommonConst.Office.OfficeInteraction.ActUnitRatePercent)
		{
			NKCOfficeCharacter nKCOfficeCharacter = m_OfficeBuilding.FindInteractableCharacter(this);
			if (!(nKCOfficeCharacter == null))
			{
				NKCOfficeManager.PlayInteraction(this, nKCOfficeCharacter);
			}
		}
	}

	private void CheckSoloInteraction()
	{
		if (!HasInteractionTarget() && !(m_fSoloInteractionCooltime > 0f) && Random.Range(0, 100) < NKMCommonConst.Office.OfficeInteraction.ActSoloRatePercent && SoloInteractionCache.Count != 0)
		{
			NKCOfficeUnitInteractionTemplet soloTemplet = SoloInteractionCache[Random.Range(0, SoloInteractionCache.Count)];
			RegisterInteraction(soloTemplet);
		}
	}

	public void PlayEmotion(string animName, float speed = 1f)
	{
		if (m_comEmotion != null)
		{
			m_comEmotion.Play(animName, speed);
		}
	}

	public void PlayEmotion(NKCUIComCharacterEmotion.Type type, float speed = 1f)
	{
		if (m_comEmotion != null)
		{
			m_comEmotion.Play(type, speed);
		}
	}
}
