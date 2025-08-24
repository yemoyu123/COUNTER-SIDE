using ClientPacket.Office;
using NKM;
using NKM.Templet.Office;

namespace NKC.Office;

public class NKCOfficeFunitureData
{
	public long uid;

	public int itemID;

	public BuildingFloor eTarget;

	private int m_posX;

	private int m_posY;

	public bool bInvert;

	private NKMOfficeInteriorTemplet _interiorTemplet;

	public int PosX => m_posX;

	public int PosY => m_posY;

	public NKMOfficeInteriorTemplet Templet
	{
		get
		{
			if (_interiorTemplet == null)
			{
				_interiorTemplet = NKMItemMiscTemplet.FindInterior(itemID);
			}
			return _interiorTemplet;
		}
	}

	public int SizeX
	{
		get
		{
			if (Templet.Target == InteriorTarget.Wall)
			{
				return Templet.CellX;
			}
			if (!bInvert)
			{
				return Templet.CellX;
			}
			return Templet.CellY;
		}
	}

	public int SizeY
	{
		get
		{
			if (Templet.Target == InteriorTarget.Wall)
			{
				return Templet.CellY;
			}
			if (!bInvert)
			{
				return Templet.CellY;
			}
			return Templet.CellX;
		}
	}

	public void SetPosition(int x, int y)
	{
		m_posX = x;
		m_posY = y;
	}

	public void SetPosition(BuildingFloor target, int x, int y, bool bInvert)
	{
		eTarget = target;
		m_posX = x;
		m_posY = y;
		this.bInvert = bInvert;
	}

	public void SetPosition(BuildingFloor target, int x, int y)
	{
		eTarget = target;
		m_posX = x;
		m_posY = y;
	}

	public NKCOfficeFunitureData(NKMOfficeFurniture _NKMOfficeFuniture)
	{
		uid = _NKMOfficeFuniture.uid;
		itemID = _NKMOfficeFuniture.itemId;
		m_posX = _NKMOfficeFuniture.positionX;
		m_posY = _NKMOfficeFuniture.positionY;
		bInvert = _NKMOfficeFuniture.inverted;
		eTarget = (BuildingFloor)_NKMOfficeFuniture.planeType;
	}

	public NKCOfficeFunitureData(long uid, int itemID, BuildingFloor target, int posX, int posY, bool bInvert = false)
	{
		this.uid = uid;
		this.itemID = itemID;
		SetPosition(target, posX, posY, bInvert);
	}

	public NKCOfficeFunitureData(NKCOfficeFunitureData source)
	{
		uid = source.uid;
		itemID = source.itemID;
		m_posX = source.m_posX;
		m_posY = source.m_posY;
		bInvert = source.bInvert;
		eTarget = source.eTarget;
		_interiorTemplet = source._interiorTemplet;
	}

	public NKMOfficeFurniture ToNKMOfficeFurniture()
	{
		return new NKMOfficeFurniture
		{
			uid = uid,
			itemId = itemID,
			positionX = m_posX,
			positionY = m_posY,
			inverted = bInvert,
			planeType = (OfficePlaneType)eTarget
		};
	}
}
