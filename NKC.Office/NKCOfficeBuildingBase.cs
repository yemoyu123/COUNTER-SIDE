using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Office;
using NKC.Templet;
using NKC.UI.Component;
using NKM;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NKC.Office;

public abstract class NKCOfficeBuildingBase : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
	protected struct FloorRect
	{
		public int x;

		public int y;

		public int sizeX;

		public int sizeY;

		public override string ToString()
		{
			return $"({x},{y}) [{sizeX},{sizeY}]";
		}
	}

	public const float BASE_TILE_SIZE = 100f;

	public NKCOfficeFloor m_Floor;

	public NKCOfficeFloor m_FloorTile;

	public NKCOfficeWall m_LeftWall;

	public NKCOfficeWall m_RightWall;

	public RectTransform m_rtBackgroundRoot;

	protected GameObject m_objBackground;

	protected RectTransform m_rtBackground;

	public NKCOfficeSelectableTile m_SelectionForAIDebug;

	protected Dictionary<long, NKCOfficeCharacter> m_dicCharacter = new Dictionary<long, NKCOfficeCharacter>();

	public int m_SizeX = 16;

	public int m_SizeY = 16;

	public int m_wallHeight = 6;

	public float m_fTileSize = 100f;

	[Header("BG 고정 Size. 수정하지 마세요.")]
	public float m_fBGWidth = 5708f;

	[Header("Camera")]
	public float m_fCameraZPos = -676f;

	public Vector2 m_vCameraOverflowRange = new Vector2(300f, 200f);

	private bool m_bEnableDrag = true;

	protected float m_fCameraOffset;

	protected Rect m_rectWorld;

	protected Rect m_rectCamMoveRange;

	protected Rect m_rectCamLimit;

	protected NKCOfficeFuniture.OnClickFuniture dOnSelectFuniture;

	private long sdCount;

	private Vector2 m_vTotalMove;

	private Vector2 m_vCamPosBefore;

	private bool m_bDragging;

	[Header("스크롤 관련")]
	public float m_fScrollSensibility = 1f;

	public float m_fCamReturnRate = 0.2f;

	public float m_fPinchZoomRate = 0.5f;

	public float m_fKeyboardMoveSpeed = 10f;

	public float m_fScrollZoomSensibility = 100f;

	protected long[,] m_FloorMap;

	private List<Transform> m_lstTransformBuffer = new List<Transform>();

	private Vector3 BottomPoint;

	private Vector3[] buffer = new Vector3[4];

	protected IEnumerable<NKCOfficeFloorBase> OfficeFloors
	{
		get
		{
			if (m_Floor != null)
			{
				yield return m_Floor;
			}
			if (m_FloorTile != null)
			{
				yield return m_FloorTile;
			}
			if (m_LeftWall != null)
			{
				yield return m_LeftWall;
			}
			if (m_RightWall != null)
			{
				yield return m_RightWall;
			}
		}
	}

	public Transform trActorRoot => m_Floor.m_rtFunitureRoot;

	protected Transform trFloorFunitureRoot => m_Floor.m_rtFunitureRoot;

	public float m_fBGHeight => m_fBGWidth * 1080f / 1920f;

	private float MaxOrthoSize
	{
		get
		{
			Vector2 vector = new Vector2(m_fBGWidth * 0.5f - m_vCameraOverflowRange.x - Mathf.Abs(m_fCameraOffset), m_fBGHeight * 0.5f - m_vCameraOverflowRange.y);
			float num = vector.x / vector.y;
			float num2 = (float)Screen.width / (float)Screen.height;
			if (num2 > num)
			{
				return Mathf.Min(vector.x / num2, Screen.height);
			}
			return Mathf.Min(vector.y, Screen.height);
		}
	}

	private float MinOrthoSize => (float)Screen.height * 0.25f;

	public long[,] FloorMap => m_FloorMap;

	public float TileSize => m_fTileSize;

	public bool AIMapEnable
	{
		get
		{
			return m_SelectionForAIDebug.gameObject.activeSelf;
		}
		set
		{
			NKCUtil.SetGameobjectActive(m_SelectionForAIDebug, value);
			if (value)
			{
				m_SelectionForAIDebug.SetSize(m_SizeX, m_SizeY, m_fTileSize);
				m_SelectionForAIDebug.UpdateSelectionTileForAI(m_FloorMap);
			}
		}
	}

	public void SetEnableDrag(bool bSet)
	{
		m_bEnableDrag = bSet;
	}

	public virtual void Init(NKCOfficeFuniture.OnClickFuniture onSelectFuniture)
	{
		if (m_SelectionForAIDebug != null)
		{
			m_SelectionForAIDebug.Init(BuildingFloor.Floor, Color.white * 0.4f, Color.white * 0.4f, Color.yellow * 0.4f, Color.red * 0.4f);
			NKCUtil.SetGameobjectActive(m_SelectionForAIDebug, bValue: false);
		}
	}

	public void SetRoomSize(int x, int y, int wallheight, float tilesize)
	{
		m_SizeX = x;
		m_SizeY = y;
		m_wallHeight = wallheight;
		m_fTileSize = tilesize;
		m_Floor.SetSize(x, y, tilesize, BuildingFloor.Floor);
		m_FloorTile.SetSize(x, y, tilesize, BuildingFloor.Tile);
		m_LeftWall.SetSize(x, wallheight, tilesize, BuildingFloor.LeftWall);
		m_RightWall.SetSize(y, wallheight, tilesize, BuildingFloor.RightWall);
		if (Application.isPlaying && NKCCamera.GetCamera() != null)
		{
			CalculateRoomSize();
		}
	}

	protected virtual void Update()
	{
		ProcessCameraUpdate();
		SortFloorObjects();
		if (NKCDefineManager.DEFINE_UNITY_EDITOR())
		{
			NKCDebugUtil.DebugDrawRect(m_rectWorld, Vector2.zero, Vector2.zero, Color.green);
			NKCDebugUtil.DebugDrawRect(m_rectCamLimit, Vector2.zero, -m_vCameraOverflowRange, Color.yellow);
			NKCDebugUtil.DebugDrawRect(m_rectCamMoveRange, Vector2.zero, Vector2.zero, Color.red);
			NKCDebugUtil.DebugDrawRect(m_rectCamLimit, Vector2.zero, Vector2.zero, Color.blue);
		}
	}

	public virtual void CleanUp()
	{
		CleanupCharacters(bCleanupNPC: true);
		if (m_objBackground != null)
		{
			Object.Destroy(m_objBackground);
			m_objBackground = null;
		}
	}

	public long TestAddSDCharacter(string assetName)
	{
		long num = sdCount;
		sdCount++;
		NKCOfficeCharacter instance = NKCOfficeCharacter.GetInstance(NKMAssetName.ParseBundleName(assetName, assetName));
		instance.Init(this, 0, 0);
		m_dicCharacter.Add(num, instance);
		OfficeFloorPosition pos = new OfficeFloorPosition(NKMRandom.Range(0, m_SizeX), NKMRandom.Range(0, m_SizeY));
		pos = FindNearestEmptyCell(pos);
		instance.transform.localPosition = m_Floor.GetLocalPos(pos);
		instance.StartAI();
		return num;
	}

	public virtual void UpdateSDCharacters(List<long> lstUnitUID, List<NKMUserProfileData> lstFriends)
	{
		SetSDCharacters(lstUnitUID, lstFriends);
	}

	protected void SetSDCharacters(List<long> lstUnitUID, List<NKMUserProfileData> lstFriends)
	{
		if (lstUnitUID == null)
		{
			lstUnitUID = new List<long>();
		}
		if (lstFriends == null)
		{
			lstFriends = new List<NKMUserProfileData>();
		}
		if (lstUnitUID.Count + lstFriends.Count == 0)
		{
			CleanupCharacters(bCleanupNPC: false);
			return;
		}
		List<long> list = new List<long>();
		foreach (long uid in m_dicCharacter.Keys)
		{
			if (!lstUnitUID.Contains(uid) && !lstFriends.Exists((NKMUserProfileData x) => uid == x.commonProfile.userUid))
			{
				list.Add(uid);
			}
		}
		foreach (long item in list)
		{
			RemoveSDCharacter(item);
		}
		for (int num = 0; num < lstUnitUID.Count; num++)
		{
			NKMUnitData unitOrTrophyFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitOrTrophyFromUID(lstUnitUID[num]);
			if (m_dicCharacter.TryGetValue(lstUnitUID[num], out var value))
			{
				value.OnUnitUpdated(unitOrTrophyFromUID);
			}
			else
			{
				AddSDCharacter(unitOrTrophyFromUID);
			}
		}
		foreach (NKMUserProfileData lstFriend in lstFriends)
		{
			if (!m_dicCharacter.TryGetValue(lstFriend.commonProfile.userUid, out var _))
			{
				NKCOfficeCharacter nKCOfficeCharacter = AddSDCharacter(lstFriend);
				if (nKCOfficeCharacter != null)
				{
					nKCOfficeCharacter.m_bCanGrab = false;
					nKCOfficeCharacter.m_bCanTouch = false;
				}
			}
		}
	}

	protected void SetSDCharacters(List<long> lstUnitUID, long userUid)
	{
		if (lstUnitUID == null || lstUnitUID.Count == 0)
		{
			CleanupCharacters(bCleanupNPC: false);
			return;
		}
		List<long> list = new List<long>();
		foreach (long key in m_dicCharacter.Keys)
		{
			if (!lstUnitUID.Contains(key))
			{
				list.Add(key);
			}
		}
		foreach (long item in list)
		{
			RemoveSDCharacter(item);
		}
		for (int i = 0; i < lstUnitUID.Count; i++)
		{
			NKMOfficeUnitData friendUnit = NKCScenManager.CurrentUserData().OfficeData.GetFriendUnit(userUid, lstUnitUID[i]);
			if (friendUnit != null && friendUnit.unitId != 0 && !m_dicCharacter.ContainsKey(lstUnitUID[i]))
			{
				NKCOfficeCharacter nKCOfficeCharacter = AddSDCharacter(friendUnit);
				nKCOfficeCharacter.m_bCanGrab = false;
				nKCOfficeCharacter.m_bCanTouch = false;
			}
		}
	}

	public NKCOfficeCharacter AddSDCharacter(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return null;
		}
		NKCOfficeCharacter instance = NKCOfficeCharacter.GetInstance(unitData);
		instance.Init(this, unitData);
		m_dicCharacter.Add(unitData.m_UnitUID, instance);
		OfficeFloorPosition pos = new OfficeFloorPosition(NKMRandom.Range(0, m_SizeX), NKMRandom.Range(0, m_SizeY));
		pos = FindNearestEmptyCell(pos);
		instance.transform.localPosition = m_Floor.GetLocalPos(pos);
		instance.StartAI();
		return instance;
	}

	public NKCOfficeCharacter AddSDCharacter(NKMOfficeUnitData unitData)
	{
		return AddSDCharacter(unitData.unitUid, unitData.unitId, unitData.skinId);
	}

	public NKCOfficeCharacter AddSDCharacter(long unitUID, int unitID, int skinID)
	{
		NKCOfficeCharacter instance = NKCOfficeCharacter.GetInstance(unitID, skinID);
		instance.Init(this, unitID, skinID);
		m_dicCharacter.Add(unitUID, instance);
		OfficeFloorPosition pos = new OfficeFloorPosition(NKMRandom.Range(0, m_SizeX), NKMRandom.Range(0, m_SizeY));
		pos = FindNearestEmptyCell(pos);
		instance.transform.localPosition = m_Floor.GetLocalPos(pos);
		instance.StartAI();
		return instance;
	}

	public NKCOfficeCharacter AddSDCharacter(NKMUserProfileData profileData)
	{
		NKCOfficeCharacter instance = NKCOfficeCharacter.GetInstance(profileData.commonProfile.mainUnitId, profileData.commonProfile.mainUnitSkinId);
		if (instance == null)
		{
			return null;
		}
		instance.Init(this, profileData);
		m_dicCharacter.Add(profileData.commonProfile.userUid, instance);
		OfficeFloorPosition pos = new OfficeFloorPosition(NKMRandom.Range(0, m_SizeX), NKMRandom.Range(0, m_SizeY));
		pos = FindNearestEmptyCell(pos);
		instance.transform.localPosition = m_Floor.GetLocalPos(pos);
		instance.StartAI();
		return instance;
	}

	private void RemoveSDCharacter(long unitUID)
	{
		if (m_dicCharacter.TryGetValue(unitUID, out var value))
		{
			value.Cleanup();
			Object.Destroy(value.gameObject);
			m_dicCharacter.Remove(unitUID);
		}
	}

	public void OnUnitUpdated(NKMUnitData unitData)
	{
		if (unitData != null && m_dicCharacter.TryGetValue(unitData.m_UnitUID, out var value))
		{
			value.OnUnitUpdated(unitData);
		}
	}

	public void OnUnitTakeHeart(NKMUnitData unitData)
	{
		if (unitData != null && m_dicCharacter.TryGetValue(unitData.m_UnitUID, out var value))
		{
			value.OnUnitTakeHeart(unitData);
		}
	}

	public void SetEnableUnitTouch(bool value)
	{
		foreach (NKCOfficeCharacter value2 in m_dicCharacter.Values)
		{
			value2.SetEnableTouch(value);
		}
	}

	public void SetEnableUnitExtraUI(bool value)
	{
		foreach (NKCOfficeCharacter value2 in m_dicCharacter.Values)
		{
			value2.SetEnableExtraUI(value);
		}
	}

	protected virtual void CleanupCharacters(bool bCleanupNPC)
	{
		foreach (KeyValuePair<long, NKCOfficeCharacter> item in m_dicCharacter)
		{
			item.Value.Cleanup();
			Object.Destroy(item.Value.gameObject);
		}
		m_dicCharacter.Clear();
	}

	public virtual void OnBeginDrag(PointerEventData data)
	{
		if (m_bEnableDrag)
		{
			m_vTotalMove = Vector2.zero;
			m_bDragging = true;
			m_vCamPosBefore.x = NKCCamera.GetPosNowX();
			m_vCamPosBefore.y = NKCCamera.GetPosNowY();
		}
	}

	public virtual void OnDrag(PointerEventData pointData)
	{
		if (m_bEnableDrag && m_bDragging)
		{
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				Zoom(pointData.delta.x + pointData.delta.y);
			}
			else if (NKCScenManager.GetScenManager().GetHasPinch())
			{
				Zoom(NKCScenManager.GetScenManager().GetPinchDeltaMagnitude() * (float)Screen.height * m_fPinchZoomRate);
			}
			else
			{
				MoveCamera(pointData.delta * m_fScrollSensibility);
			}
		}
	}

	private void MoveCamera(Vector2 delta)
	{
		float num = NKCCamera.GetCamera().orthographicSize / (float)Screen.height;
		delta *= num;
		m_vTotalMove -= delta;
		Vector2 camPos = m_vCamPosBefore + m_vTotalMove;
		Vector2 cameraOverstretch = GetCameraOverstretch(camPos);
		if (cameraOverstretch.x > 0f)
		{
			camPos.x = m_rectCamMoveRange.xMax + Rubber(cameraOverstretch.x, m_vCameraOverflowRange.x);
		}
		else if (cameraOverstretch.x < 0f)
		{
			camPos.x = m_rectCamMoveRange.xMin + Rubber(cameraOverstretch.x, m_vCameraOverflowRange.x);
		}
		if (cameraOverstretch.y > 0f)
		{
			camPos.y = m_rectCamMoveRange.yMax + Rubber(cameraOverstretch.y, m_vCameraOverflowRange.y);
		}
		else if (cameraOverstretch.y < 0f)
		{
			camPos.y = m_rectCamMoveRange.yMin + Rubber(cameraOverstretch.y, m_vCameraOverflowRange.y);
		}
		NKCCamera.SetPos(camPos.x, camPos.y, -1f, bTrackingStop: true, bForce: true);
	}

	private float Rubber(float currentValue, float Limit)
	{
		float num = Mathf.Abs(currentValue);
		return Limit * num / (Limit + num) * Mathf.Sign(currentValue);
	}

	private Vector2 GetCameraOverstretch(Vector2 camPos)
	{
		Vector2 zero = Vector2.zero;
		if (camPos.x < m_rectCamMoveRange.xMin)
		{
			zero.x = camPos.x - m_rectCamMoveRange.xMin;
		}
		else if (camPos.x > m_rectCamMoveRange.xMax)
		{
			zero.x = camPos.x - m_rectCamMoveRange.xMax;
		}
		if (camPos.y < m_rectCamMoveRange.yMin)
		{
			zero.y = camPos.y - m_rectCamMoveRange.yMin;
		}
		else if (camPos.y > m_rectCamMoveRange.yMax)
		{
			zero.y = camPos.y - m_rectCamMoveRange.yMax;
		}
		return zero;
	}

	public virtual void OnEndDrag(PointerEventData data)
	{
		m_bDragging = false;
	}

	private void CamReturnStep()
	{
		Vector2 vector = default(Vector2);
		vector.x = NKCCamera.GetPosNowX();
		vector.y = NKCCamera.GetPosNowY();
		if (m_rectCamMoveRange.xMax < vector.x)
		{
			vector.x = Mathf.Lerp(vector.x, m_rectCamMoveRange.xMax, m_fCamReturnRate);
			if (vector.x - m_rectCamMoveRange.xMax < 0.001f)
			{
				vector.x = m_rectCamMoveRange.xMax;
			}
		}
		else if (m_rectCamMoveRange.xMin > vector.x)
		{
			vector.x = Mathf.Lerp(vector.x, m_rectCamMoveRange.xMin, m_fCamReturnRate);
			if (m_rectCamMoveRange.xMin - vector.x < 0.001f)
			{
				vector.x = m_rectCamMoveRange.xMin;
			}
		}
		if (m_rectCamMoveRange.yMax < vector.y)
		{
			vector.y = Mathf.Lerp(vector.y, m_rectCamMoveRange.yMax, m_fCamReturnRate);
			if (vector.y - m_rectCamMoveRange.yMax < 0.001f)
			{
				vector.y = m_rectCamMoveRange.yMax;
			}
		}
		else if (m_rectCamMoveRange.yMin > vector.y)
		{
			vector.y = Mathf.Lerp(vector.y, m_rectCamMoveRange.yMin, m_fCamReturnRate);
			if (m_rectCamMoveRange.yMin - vector.y < 0.001f)
			{
				vector.y = m_rectCamMoveRange.yMin;
			}
		}
		NKCCamera.SetPos(vector.x, vector.y, -1f, bTrackingStop: true, bForce: true);
	}

	protected void ProcessCameraUpdate()
	{
		Vector2 vector = NKCInputManager.GetMoveVector() * m_fKeyboardMoveSpeed;
		if (vector != Vector2.zero)
		{
			m_vTotalMove = Vector2.zero;
			m_vCamPosBefore.x = NKCCamera.GetPosNowX();
			m_vCamPosBefore.y = NKCCamera.GetPosNowY();
			MoveCamera(-vector);
		}
		else if (!m_bDragging)
		{
			CamReturnStep();
		}
		if (NKCInputManager.IsHotkeyPressed(HotkeyEventType.Plus))
		{
			Zoom(m_fKeyboardMoveSpeed);
		}
		else if (NKCInputManager.IsHotkeyPressed(HotkeyEventType.Minus))
		{
			Zoom(0f - m_fKeyboardMoveSpeed);
		}
	}

	public virtual void OnScroll(PointerEventData eventData)
	{
		Zoom(eventData.scrollDelta.y * m_fScrollZoomSensibility);
	}

	public void SetCamera()
	{
		NKCCamera.GetCamera().orthographic = true;
		SetDefaultCam();
	}

	public void Zoom(float zoomDelta)
	{
		NKCCamera.GetCamera().orthographicSize = Mathf.Clamp(NKCCamera.GetCamera().orthographicSize - zoomDelta, MinOrthoSize, MaxOrthoSize);
		CalcuateCamMoveRange();
		Vector2 vector = default(Vector2);
		vector.x = NKCCamera.GetPosNowX();
		vector.y = NKCCamera.GetPosNowY();
		if (vector.x > m_rectCamLimit.xMax)
		{
			vector.x = m_rectCamLimit.xMax;
		}
		else if (vector.x < m_rectCamLimit.xMin)
		{
			vector.x = m_rectCamLimit.xMin;
		}
		if (vector.y > m_rectCamLimit.yMax)
		{
			vector.y = m_rectCamLimit.yMax;
		}
		else if (vector.y < m_rectCamLimit.yMin)
		{
			vector.y = m_rectCamLimit.yMin;
		}
		NKCCamera.SetPos(vector.x, vector.y);
	}

	public void SetCameraOffset(float offset)
	{
		m_fCameraOffset = offset;
		SetBackgroundSize();
	}

	private void SetDefaultCam()
	{
		NKCCamera.GetCamera().orthographicSize = MaxOrthoSize;
		CalcuateCamMoveRange();
		NKCCamera.SetPos(m_rectWorld.center.x - m_fCameraOffset, m_rectWorld.center.y);
	}

	public void CalculateRoomSize()
	{
		Rect worldRect = m_Floor.Rect.GetWorldRect();
		Rect worldRect2 = m_LeftWall.Rect.GetWorldRect();
		m_rectWorld = Rect.MinMaxRect(worldRect.xMin, worldRect.yMin, worldRect.xMax, worldRect2.yMax);
		CalcuateCamMoveRange();
	}

	private void CalcuateCamMoveRange()
	{
		m_rectCamLimit = Rect.MinMaxRect((0f - m_fBGWidth) * 0.5f - m_fCameraOffset, (0f - m_fBGHeight) * 0.5f, m_fBGWidth * 0.5f - m_fCameraOffset, m_fBGHeight * 0.5f);
		float num = (float)Screen.width * NKCCamera.GetCamera().orthographicSize / (float)Screen.height;
		float orthographicSize = NKCCamera.GetCamera().orthographicSize;
		Vector2 vector = new Vector2(m_vCameraOverflowRange.x + num, m_vCameraOverflowRange.y + orthographicSize);
		float num2 = (0f - m_fBGWidth) * 0.5f + vector.x - m_fCameraOffset;
		float num3 = (0f - m_fBGHeight) * 0.5f + vector.y;
		float num4 = m_fBGWidth * 0.5f - vector.x - m_fCameraOffset;
		float num5 = m_fBGHeight * 0.5f - vector.y;
		if (num5 < num3)
		{
			num5 = 0f;
			num3 = 0f;
		}
		if (num4 < num2)
		{
			num4 = 0f - m_fCameraOffset;
			num2 = 0f - m_fCameraOffset;
		}
		m_rectCamMoveRange = Rect.MinMaxRect(num2, num3, num4, num5);
	}

	protected void SetBackground(NKCOfficeRoomData roomData)
	{
		if (roomData == null)
		{
			Debug.LogError("roomData null!");
		}
		NKMOfficeInteriorTemplet background = NKMItemMiscTemplet.FindInterior(roomData.m_BackgroundID);
		SetBackground(background);
	}

	protected void SetBackground(NKMOfficeInteriorTemplet templet)
	{
		if (templet == null)
		{
			Debug.LogError("templet null!");
			return;
		}
		if (templet.InteriorCategory != InteriorCategory.DECO || templet.Target != InteriorTarget.Background)
		{
			Debug.LogError("Wrong type!");
			return;
		}
		NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(templet.PrefabName, templet.PrefabName);
		SetBackground(nKMAssetName.m_BundleName, nKMAssetName.m_BundleName);
	}

	protected void SetBackground(string bundleName, string assetName)
	{
		if (m_objBackground != null)
		{
			Object.Destroy(m_objBackground);
			m_objBackground = null;
		}
		NKCAssetResourceData nKCAssetResourceData = NKCAssetResourceManager.OpenResource<GameObject>(bundleName, assetName);
		if (nKCAssetResourceData != null && nKCAssetResourceData.GetAsset<GameObject>() != null)
		{
			m_objBackground = Object.Instantiate(nKCAssetResourceData.GetAsset<GameObject>());
			m_objBackground.transform.SetParent(m_rtBackgroundRoot);
			Transform transform = m_objBackground.transform.Find("Stretch/Background");
			if (transform != null)
			{
				m_rtBackground = transform.GetComponent<RectTransform>();
				EventTrigger eventTrigger = transform.GetComponent<EventTrigger>();
				if (eventTrigger == null)
				{
					eventTrigger = transform.gameObject.AddComponent<EventTrigger>();
				}
				eventTrigger.triggers.Clear();
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.BeginDrag;
				entry.callback.AddListener(delegate(BaseEventData eventData)
				{
					PointerEventData data = eventData as PointerEventData;
					OnBeginDrag(data);
				});
				eventTrigger.triggers.Add(entry);
				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.Drag;
				entry.callback.AddListener(delegate(BaseEventData eventData)
				{
					PointerEventData pointData = eventData as PointerEventData;
					OnDrag(pointData);
				});
				eventTrigger.triggers.Add(entry);
				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.EndDrag;
				entry.callback.AddListener(delegate(BaseEventData eventData)
				{
					PointerEventData data = eventData as PointerEventData;
					OnEndDrag(data);
				});
				eventTrigger.triggers.Add(entry);
				entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.Scroll;
				entry.callback.AddListener(delegate(BaseEventData eventData)
				{
					PointerEventData eventData2 = eventData as PointerEventData;
					OnScroll(eventData2);
				});
				eventTrigger.triggers.Add(entry);
			}
		}
		if (nKCAssetResourceData != null)
		{
			NKCAssetResourceManager.CloseResource(nKCAssetResourceData);
		}
		SetBackgroundSize();
	}

	protected void SetBackgroundSize()
	{
		if (m_rtBackground != null)
		{
			m_rtBackground.localScale = Vector3.one;
			m_rtBackground.SetSize(new Vector2(m_fBGWidth, m_fBGHeight));
		}
		if (m_objBackground != null)
		{
			m_objBackground.transform.localPosition = new Vector3(0f - m_fCameraOffset, 0f, 0f);
			m_objBackground.transform.localScale = Vector3.one;
		}
	}

	protected abstract void UpdateFloorMap();

	protected FloorRect CalculateFloorRect(RectTransform rect)
	{
		float width = rect.GetWidth();
		float height = rect.GetHeight();
		FloorRect result = new FloorRect
		{
			sizeX = Mathf.RoundToInt(width / m_fTileSize),
			sizeY = Mathf.RoundToInt(height / m_fTileSize)
		};
		OfficeFloorPosition officeFloorPosition = CalculateFloorPosition(rect.localPosition, result.sizeX, result.sizeY);
		result.x = officeFloorPosition.x;
		result.y = officeFloorPosition.y;
		return result;
	}

	public OfficeFloorPosition CalculateFloorPosition(Vector3 localPos, bool bClamp)
	{
		return CalculateFloorPosition(localPos, 1, 1, bClamp);
	}

	public OfficeFloorPosition CalculateFloorPosition(Vector3 localPos, int sizeX = 1, int sizeY = 1, bool bClamp = false)
	{
		OfficeFloorPosition result = new OfficeFloorPosition
		{
			x = Mathf.RoundToInt(localPos.x / m_fTileSize + m_Floor.m_rtFunitureRoot.pivot.x * (float)m_SizeX - (float)sizeX * 0.5f),
			y = Mathf.RoundToInt(localPos.y / m_fTileSize + m_Floor.m_rtFunitureRoot.pivot.y * (float)m_SizeY - (float)sizeY * 0.5f)
		};
		if (bClamp)
		{
			result.x = Mathf.Clamp(result.x, 0, m_SizeX - 1);
			result.y = Mathf.Clamp(result.y, 0, m_SizeY - 1);
		}
		return result;
	}

	public bool IsPositionUnblocked(OfficeFloorPosition pos)
	{
		if (m_Floor.IsInBound(pos))
		{
			return FloorMap[pos.x, pos.y] == 0;
		}
		return false;
	}

	public OfficeFloorPosition FindNearestEmptyCell(OfficeFloorPosition pos)
	{
		if (FloorMap == null)
		{
			return pos;
		}
		if (FloorMap[pos.x, pos.y] == 0L)
		{
			return pos;
		}
		Queue<OfficeFloorPosition> queue = new Queue<OfficeFloorPosition>();
		HashSet<OfficeFloorPosition> hashSet = new HashSet<OfficeFloorPosition>();
		queue.Enqueue(pos);
		int length = FloorMap.GetLength(0);
		int length2 = FloorMap.GetLength(1);
		while (queue.Count > 0)
		{
			OfficeFloorPosition officeFloorPosition = queue.Dequeue();
			if (officeFloorPosition.x >= 0 && officeFloorPosition.x < length && officeFloorPosition.y >= 0 && officeFloorPosition.y < length2 && !hashSet.Contains(officeFloorPosition))
			{
				if (FloorMap[officeFloorPosition.x, officeFloorPosition.y] == 0L)
				{
					return officeFloorPosition;
				}
				hashSet.Add(officeFloorPosition);
				queue.Enqueue(officeFloorPosition + new OfficeFloorPosition(0, 1));
				queue.Enqueue(officeFloorPosition + new OfficeFloorPosition(1, 0));
				queue.Enqueue(officeFloorPosition + new OfficeFloorPosition(0, -1));
				queue.Enqueue(officeFloorPosition + new OfficeFloorPosition(-1, 0));
			}
		}
		return pos;
	}

	protected void SortFloorObjects()
	{
		BottomPoint = BottomMostPoint();
		NKCTopologicalSort<Transform> nKCTopologicalSort = new NKCTopologicalSort<Transform>(GetRelation);
		m_lstTransformBuffer.Clear();
		foreach (Transform item in trFloorFunitureRoot)
		{
			m_lstTransformBuffer.Add(item);
		}
		List<Transform> list = nKCTopologicalSort.DoSort(m_lstTransformBuffer);
		if (list == null)
		{
			return;
		}
		foreach (Transform item2 in list)
		{
			item2.SetAsLastSibling();
		}
	}

	protected (bool, int) GetRelation(Transform a, Transform b)
	{
		return (!ISApart(a, b), FullComparer(a, b));
	}

	private bool ISApart(Transform a, Transform b)
	{
		NKCOfficeFuniture component = a.GetComponent<NKCOfficeFuniture>();
		NKCOfficeFuniture component2 = b.GetComponent<NKCOfficeFuniture>();
		if (component != null && component2 != null)
		{
			return IsFurnitureApart(component, component2);
		}
		if (component != null && component2 == null)
		{
			return IsFurniturePointApart(component, b);
		}
		if (component == null && component2 != null)
		{
			return IsFurniturePointApart(component2, a);
		}
		NKCOfficeCharacter component3 = a.GetComponent<NKCOfficeCharacter>();
		NKCOfficeCharacter component4 = b.GetComponent<NKCOfficeCharacter>();
		if (component3 != null && component4 != null)
		{
			Rect worldRect = component3.GetWorldRect();
			Rect worldRect2 = component4.GetWorldRect();
			return !worldRect.Overlaps(worldRect2);
		}
		return true;
	}

	private bool IsFurniturePointApart(NKCOfficeFuniture funA, Transform b)
	{
		NKCOfficeCharacter component = b.GetComponent<NKCOfficeCharacter>();
		if (component != null)
		{
			Rect worldRect = funA.GetWorldRect(bFurnitureOnly: false);
			Rect worldRect2 = component.GetWorldRect();
			return !worldRect.Overlaps(worldRect2);
		}
		funA.GetWorldInfo(out var zMin, out var zMax, out var xMinPos, out var xMaxPos);
		if (zMin > b.position.z || b.position.z > zMax)
		{
			return true;
		}
		if (xMinPos.x > b.position.x || b.position.x > xMaxPos.x)
		{
			return true;
		}
		return false;
	}

	private bool IsFurnitureApart(NKCOfficeFuniture a, NKCOfficeFuniture b)
	{
		Rect worldRect = a.GetWorldRect(bFurnitureOnly: false);
		Rect worldRect2 = b.GetWorldRect(bFurnitureOnly: false);
		return !worldRect.Overlaps(worldRect2);
	}

	protected int FurnitureComparer(NKCOfficeFuniture a, NKCOfficeFuniture b)
	{
		(float, float) zMinMax = a.GetZMinMax();
		(float, float) zMinMax2 = b.GetZMinMax();
		if (zMinMax.Item1 > zMinMax2.Item2)
		{
			return -1;
		}
		if (zMinMax2.Item1 > zMinMax.Item2)
		{
			return 1;
		}
		(Vector3, Vector3) horizonalLine = a.GetHorizonalLine();
		(Vector3, Vector3) horizonalLine2 = b.GetHorizonalLine();
		int num = CompareFurnitureLines(horizonalLine, horizonalLine2);
		if (num != 0)
		{
			return num;
		}
		float squaredDistanceFromPoint = GetSquaredDistanceFromPoint(a, BottomPoint);
		return GetSquaredDistanceFromPoint(b, BottomPoint).CompareTo(squaredDistanceFromPoint);
	}

	protected int SimpleComparer(Transform a, Transform b)
	{
		return b.position.z.CompareTo(a.position.z);
	}

	protected int FullComparer(Transform a, Transform b)
	{
		NKCOfficeFuniture component = a.GetComponent<NKCOfficeFuniture>();
		NKCOfficeFuniture component2 = b.GetComponent<NKCOfficeFuniture>();
		if (component != null && component2 != null)
		{
			return FurnitureComparer(component, component2);
		}
		if (component != null && component2 == null)
		{
			if (GetPointIsBehindFurniture(b.position, component, BottomPoint))
			{
				return 1;
			}
			return -1;
		}
		if (component == null && component2 != null)
		{
			if (GetPointIsBehindFurniture(a.position, component2, BottomPoint))
			{
				return -1;
			}
			return 1;
		}
		return b.position.z.CompareTo(a.position.z);
	}

	private int CompareFurnitureLines((Vector3, Vector3) lineA, (Vector3, Vector3) lineB)
	{
		float num = Mathf.Max(lineA.Item1.x, lineB.Item1.x);
		float num2 = Mathf.Min(lineA.Item2.x, lineB.Item2.x);
		if (num >= num2)
		{
			float value = Mathf.Min(lineA.Item1.y, lineA.Item2.y);
			return Mathf.Min(lineB.Item1.y, lineB.Item2.y).CompareTo(value);
		}
		Vector3 vector = lineA.Item2 - lineA.Item1;
		float num3 = vector.y / vector.x;
		float num4 = lineA.Item1.y - num3 * lineA.Item1.x;
		Vector3 vector2 = lineB.Item2 - lineB.Item1;
		float num5 = vector2.y / vector2.x;
		float num6 = lineB.Item1.y - num5 * lineB.Item1.x;
		float a = num3 * num2 + num4;
		float b = num3 * num + num4;
		float a2 = num5 * num2 + num6;
		float b2 = num5 * num + num6;
		float value2 = Mathf.Min(a, b);
		return Mathf.Min(a2, b2).CompareTo(value2);
	}

	private bool GetPointIsBehindFurniture(Vector3 P, NKCOfficeFuniture furniture, Vector3 BasePoint)
	{
		(Vector3, Vector3) horizonalLine = furniture.GetHorizonalLine();
		return GetPointIsBehindLine(P, horizonalLine.Item1, horizonalLine.Item2, BasePoint);
	}

	private bool GetPointIsBehindLine(Vector3 P, Vector3 L1, Vector3 L2, Vector3 Base)
	{
		Vector3 vector = L2 - L1;
		Vector3 vector2 = P + vector;
		float num = LineToPointDistanceSquared(L1, L2, Base, Color.yellow);
		Debug.DrawLine(vector2, P, Color.cyan);
		return LineToPointDistanceSquared(P, vector2, Base, Color.green) > num;
	}

	private float LineToPointDistanceSquared(Vector3 P1, Vector3 P2, Vector3 P, Color debugColor)
	{
		Vector3 lhs = P - P1;
		Vector3 rhs = P2 - P1;
		float num = Vector3.Dot(lhs, rhs);
		float sqrMagnitude = rhs.sqrMagnitude;
		if (sqrMagnitude == 0f)
		{
			return (P - P1).sqrMagnitude;
		}
		float num2 = num / sqrMagnitude;
		Vector3 vector = P1 + num2 * (P2 - P1);
		Vector3 vector2 = P - vector;
		Debug.DrawLine(P, vector, debugColor);
		return vector2.sqrMagnitude;
	}

	private float GetSquaredDistanceFromPoint(NKCOfficeFuniture furniture, Vector3 P)
	{
		(Vector3, Vector3) horizonalLine = furniture.GetHorizonalLine();
		return SegmentToPointDistanceSquared(horizonalLine.Item1, horizonalLine.Item2, P);
	}

	protected float SegmentToPointDistanceSquared(Vector3 P1, Vector3 P2, Vector3 P)
	{
		Vector3 lhs = P - P1;
		Vector3 rhs = P2 - P1;
		float num = Vector3.Dot(lhs, rhs);
		float sqrMagnitude = rhs.sqrMagnitude;
		if (sqrMagnitude == 0f)
		{
			return (P - P1).sqrMagnitude;
		}
		float num2 = num / sqrMagnitude;
		Vector3 vector = ((num2 < 0f) ? P1 : ((!(num2 > 1f)) ? (P1 + num2 * (P2 - P1)) : P2));
		return (P - vector).sqrMagnitude;
	}

	protected Vector3 BottomMostPoint()
	{
		m_Floor.Rect.GetWorldCorners(buffer);
		return buffer[0];
	}

	public NKCOfficeCharacter GetCharacter(int unitID)
	{
		foreach (KeyValuePair<long, NKCOfficeCharacter> item in m_dicCharacter)
		{
			NKCOfficeCharacter value = item.Value;
			if (value.UnitID == unitID)
			{
				return value;
			}
		}
		return null;
	}

	public IEnumerable<NKCOfficeCharacter> GetCharacterEnumerator()
	{
		return m_dicCharacter.Values;
	}

	public IEnumerable<NKCOfficeCharacter> GetCharactersInRange(Vector3 worldpos, float range)
	{
		float rangeSqr = range * range;
		foreach (NKCOfficeCharacter value in m_dicCharacter.Values)
		{
			if ((value.transform.position - worldpos).sqrMagnitude <= rangeSqr)
			{
				yield return value;
			}
		}
	}

	public virtual void OnCharacterBeginDrag(NKCOfficeCharacter character)
	{
	}

	public virtual void OnCharacterEndDrag(NKCOfficeCharacter character)
	{
	}

	public virtual NKCOfficeFuniture FindInteractableInterior(NKCOfficeCharacter character)
	{
		return null;
	}

	public virtual NKCOfficeCharacter FindInteractableCharacter(NKCOfficeCharacter character)
	{
		return null;
	}

	public bool CalcInteractionPos(NKCOfficeCharacter actor, NKCOfficeCharacter target, out Vector3 actorPos, out Vector3 targetPos)
	{
		Vector3 vector = (actor.transform.localPosition + target.transform.localPosition) * 0.5f;
		Vector3 vector2 = vector + new Vector3(0f - m_fTileSize, m_fTileSize, 0f);
		Vector3 vector3 = vector + new Vector3(m_fTileSize, 0f - m_fTileSize + 1f, 0f);
		Vector3[] array = new Vector3[3] { vector, vector2, vector3 };
		foreach (Vector3 localPos in array)
		{
			OfficeFloorPosition pos = CalculateFloorPosition(localPos);
			if (!IsPositionUnblocked(pos))
			{
				actorPos = actor.transform.localPosition;
				targetPos = target.transform.localPosition;
				return false;
			}
		}
		if (actor.transform.position.x <= target.transform.position.x)
		{
			actorPos = vector2;
			targetPos = vector3;
		}
		else
		{
			actorPos = vector3;
			targetPos = vector2;
		}
		return true;
	}

	public void OnPartyFinished(NKCOfficePartyTemplet partyTemplet)
	{
		if (partyTemplet == null)
		{
			return;
		}
		foreach (NKCOfficeCharacter value in m_dicCharacter.Values)
		{
			List<string> list = new List<string>();
			foreach (string item in partyTemplet.PartyEndAni)
			{
				List<NKCAnimationEventTemplet> lstAnim = NKCAnimationEventManager.Find(item);
				if (NKCAnimationEventManager.CanPlayAnimEvent(value, lstAnim))
				{
					list.Add(item);
				}
			}
			if (list.Count > 0)
			{
				List<NKCAnimationEventTemplet> lstAnimEvent = NKCAnimationEventManager.Find(list[Random.Range(0, list.Count)]);
				value.StopAllAnimInstances();
				value.EnqueueAnimation(lstAnimEvent);
			}
		}
	}
}
