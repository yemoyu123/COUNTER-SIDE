using System.Collections.Generic;
using Cs.Core.Util;
using NKC.UI.Component;
using NKM;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.Office;

public abstract class NKCOfficeFloorBase : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IDragHandler, IEndDragHandler
{
	public delegate void OnDragEvent(PointerEventData eventData);

	public NKCOfficeSelectableTile m_SelectableTile;

	public Image m_imgFloor;

	public RectTransform m_rtDecorationRoot;

	public RectTransform m_rtFunitureRoot;

	public RectTransform m_rtSelectedFunitureRoot;

	protected float m_fTileSize = 100f;

	protected int m_sizeX = 1;

	protected int m_sizeY = 1;

	private BuildingFloor m_eTarget;

	protected RectTransform m_rect;

	protected Image m_Image;

	private List<NKCOfficeFuniture> m_lstDeco;

	private OnDragEvent dOnBeginDrag;

	private OnDragEvent dOnDrag;

	private OnDragEvent dOnEndDrag;

	public RectTransform Rect
	{
		get
		{
			if (m_rect == null)
			{
				m_rect = GetComponent<RectTransform>();
			}
			return m_rect;
		}
	}

	public Image Image
	{
		get
		{
			if (m_Image == null)
			{
				m_Image = GetComponent<Image>();
			}
			return m_Image;
		}
	}

	public void Init(BuildingFloor target, Color colEmpty, Color colSelect, Color colOccupied, Color colProhibited, OnDragEvent onBeginDrag, OnDragEvent onDrag, OnDragEvent onEndDrag)
	{
		m_eTarget = target;
		m_SelectableTile?.Init(target, colEmpty, colSelect, colOccupied, colProhibited);
		dOnBeginDrag = onBeginDrag;
		dOnDrag = onDrag;
		dOnEndDrag = onEndDrag;
	}

	public void CleanUp()
	{
		if (m_lstDeco != null)
		{
			foreach (NKCOfficeFuniture item in m_lstDeco)
			{
				item.CleanUp();
			}
			m_lstDeco.Clear();
		}
		m_lstDeco = null;
		if (m_imgFloor != null)
		{
			m_imgFloor.sprite = null;
		}
	}

	public abstract bool GetFunitureInvert(NKCOfficeFunitureData funitureData);

	protected abstract bool GetFunitureInvert();

	public void SetSize(int x, int y, float tileSize, BuildingFloor target)
	{
		m_sizeX = x;
		m_sizeY = y;
		m_fTileSize = tileSize;
		m_eTarget = target;
		Rect.SetSize(new Vector2((float)x * tileSize, (float)y * tileSize));
		m_SelectableTile.SetSize(x, y, tileSize);
	}

	public Vector3 GetWorldPos(int x, int y)
	{
		Vector3 localPos = GetLocalPos(x, y);
		return Rect.TransformPoint(localPos);
	}

	public Vector3 GetWorldPos(OfficeFloorPosition pos)
	{
		return GetWorldPos(pos.x, pos.y);
	}

	public Vector3 GetWorldPos(int x, int y, int sizeX, int sizeY)
	{
		Vector3 localPos = GetLocalPos(x, y, sizeX, sizeY);
		return Rect.TransformPoint(localPos);
	}

	public Vector3 GetLocalPos((int, int) pos)
	{
		return GetLocalPos(pos.Item1, pos.Item2);
	}

	public Vector3 GetLocalPos(OfficeFloorPosition pos)
	{
		return GetLocalPos(pos.x, pos.y);
	}

	public Vector3 GetLocalPos(int x, int y)
	{
		return new Vector3((float)x * m_fTileSize + 0.5f * m_fTileSize - Rect.pivot.x * Rect.GetWidth(), (float)y * m_fTileSize + 0.5f * m_fTileSize - Rect.pivot.y * Rect.GetHeight(), 0f);
	}

	public Vector3 GetLocalPos(int x, int y, int sizeX, int sizeY)
	{
		return new Vector3(((float)x + (float)sizeX * 0.5f) * m_fTileSize - Rect.pivot.x * Rect.GetWidth(), ((float)y + (float)sizeY * 0.5f) * m_fTileSize - Rect.pivot.y * Rect.GetHeight(), 0f);
	}

	public OfficeFloorPosition GetCellPosFromScreenPos(Vector2 inputScreenPos, int funitureSizeX, int funitureSizeY)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(Rect, inputScreenPos, NKCCamera.GetCamera(), out var localPoint);
		OfficeFloorPosition result = default(OfficeFloorPosition);
		result.x = (int)((localPoint.x + Rect.pivot.x * Rect.GetWidth()) / m_fTileSize - (float)funitureSizeX * 0.5f);
		result.y = (int)((localPoint.y + Rect.pivot.y * Rect.GetHeight()) / m_fTileSize - (float)funitureSizeY * 0.5f);
		result.x = result.x.Clamp(0, m_sizeX - funitureSizeX);
		result.y = result.y.Clamp(0, m_sizeY - funitureSizeY);
		return result;
	}

	public Vector3 GetLocalPosFromScreenPos(Vector2 inputScreenPos)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(Rect, inputScreenPos, NKCCamera.GetCamera(), out var localPoint);
		return localPoint;
	}

	public bool IsContainsScreenPoint(Vector2 inputScreenPos)
	{
		return RectTransformUtility.RectangleContainsScreenPoint(Rect, inputScreenPos, NKCCamera.GetCamera());
	}

	public Vector3 GetLocalPos(NKCOfficeFunitureData funitureData)
	{
		return GetLocalPos(funitureData.PosX, funitureData.PosY, funitureData.SizeX, funitureData.SizeY);
	}

	public void ShowSelectionTile(bool value)
	{
		NKCUtil.SetGameobjectActive(m_SelectableTile, value);
	}

	public bool IsInBound(OfficeFloorPosition pos)
	{
		if (pos.x < 0)
		{
			return false;
		}
		if (pos.y < 0)
		{
			return false;
		}
		if (pos.x >= m_sizeX)
		{
			return false;
		}
		if (pos.y >= m_sizeY)
		{
			return false;
		}
		return true;
	}

	public bool UpdateSelectionTile(NKCOfficeFunitureData selectionData, NKCOfficeRoomData roomData)
	{
		if (m_SelectableTile != null)
		{
			return m_SelectableTile.UpdateSelectionTile(selectionData, roomData);
		}
		return true;
	}

	public void SetDecoration(NKMOfficeInteriorTemplet templet)
	{
		if (m_imgFloor != null)
		{
			m_imgFloor.enabled = templet.IsTexture;
		}
		NKCUtil.SetGameobjectActive(m_rtDecorationRoot, !templet.IsTexture);
		CleanUp();
		if (templet.IsTexture)
		{
			if (!(m_imgFloor == null))
			{
				NKMAssetName cNKMAssetName = NKMAssetName.ParseBundleName(templet.PrefabName, templet.PrefabName);
				m_imgFloor.sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(cNKMAssetName);
				m_imgFloor.type = Image.Type.Tiled;
			}
		}
		else
		{
			if (m_rtDecorationRoot == null)
			{
				return;
			}
			if (m_lstDeco == null)
			{
				m_lstDeco = new List<NKCOfficeFuniture>();
			}
			for (int i = 0; i < m_sizeX; i += templet.CellX)
			{
				for (int j = 0; j < m_sizeY; j += templet.CellY)
				{
					NKCOfficeFuniture instance = NKCOfficeFuniture.GetInstance(-1L, templet, m_fTileSize, GetFunitureInvert(), m_rtDecorationRoot);
					if (instance == null)
					{
						Debug.LogError($"Decoration {templet.PrefabName} not found! id : {templet.m_ItemMiscID}");
						return;
					}
					m_lstDeco.Add(instance);
					instance.dOnBeginDragFuniture = OnBeginDrag;
					instance.dOnDragFuniture = OnDrag;
					instance.dOnEndDragFuniture = OnEndDrag;
					int a = i + templet.CellX - m_sizeX;
					a = Mathf.Max(a, 0);
					int a2 = j + templet.CellY - m_sizeY;
					a2 = Mathf.Max(a2, 0);
					if (a > 0 || a2 > 0)
					{
						instance.Resize(templet, a, a2);
						instance.transform.localPosition = GetLocalPos(i - a, j - a2, templet.CellX, templet.CellY);
					}
					else
					{
						instance.transform.localPosition = GetLocalPos(i, j, templet.CellX, templet.CellY);
					}
				}
			}
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		dOnBeginDrag?.Invoke(eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		dOnDrag?.Invoke(eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		dOnEndDrag?.Invoke(eventData);
	}
}
