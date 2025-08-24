using System.Collections.Generic;
using NKC.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component;

[RequireComponent(typeof(Image))]
public class NKCOfficeSelectableTile : MonoBehaviour
{
	public enum TileState : short
	{
		Empty,
		Selected,
		Occupied,
		Prohibited
	}

	public float m_fBorderWidth = 2f;

	private Color m_colEmpty;

	private Color m_colSelect;

	private Color m_colOccupied;

	private Color m_colProhibited;

	[Header("Materials")]
	public Material m_mat;

	private BuildingFloor m_eTarget;

	private int m_sizeX;

	private int m_sizeY;

	private float m_tileSize;

	private Image _image;

	private RectTransform _imageRectTransform;

	private Texture2D m_texTile;

	private Image m_image
	{
		get
		{
			if (_image == null)
			{
				_image = GetComponent<Image>();
			}
			return _image;
		}
	}

	private RectTransform m_imageRectTransform
	{
		get
		{
			if (_imageRectTransform == null)
			{
				_imageRectTransform = m_image.GetComponent<RectTransform>();
			}
			return _imageRectTransform;
		}
	}

	public void Init(BuildingFloor target, Color colEmpty, Color colSelect, Color colOccupied, Color colProhibited)
	{
		m_eTarget = target;
		m_colEmpty = colEmpty;
		m_colSelect = colSelect;
		m_colOccupied = colOccupied;
		m_colProhibited = colProhibited;
	}

	public void SetSize(int x, int y, float tileSize)
	{
		m_sizeX = x;
		m_sizeY = y;
		m_tileSize = tileSize;
		m_texTile = new Texture2D(x, y, TextureFormat.ARGB32, mipChain: false);
		m_texTile.wrapMode = TextureWrapMode.Clamp;
		m_texTile.filterMode = FilterMode.Point;
		m_texTile.anisoLevel = 0;
		m_image.material = new Material(m_mat);
		m_image.material.SetFloat("_InvRectSizeX", 1f / ((float)x * tileSize));
		m_image.material.SetFloat("_InvRectSizeY", 1f / ((float)y * tileSize));
	}

	public bool UpdateSelectionTile(NKCOfficeFunitureData selectedFunitureData, NKCOfficeRoomData roomData)
	{
		Color[,] colorGrid;
		bool result = MakeSelectionGrid(selectedFunitureData, roomData, out colorGrid);
		UpdateTileStatus(colorGrid);
		return result;
	}

	public void UpdateSelectionTileForAI(long[,] tileState)
	{
		Color[,] array = new Color[tileState.GetLength(0), tileState.GetLength(1)];
		for (int i = 0; i < tileState.GetLength(0); i++)
		{
			for (int j = 0; j < tileState.GetLength(1); j++)
			{
				array[i, j] = ((tileState[i, j] == 0L) ? m_colEmpty : m_colOccupied);
			}
		}
		UpdateTileStatus(array);
	}

	private bool MakeSelectionGrid(NKCOfficeFunitureData selectedFunitureData, NKCOfficeRoomData roomData, out Color[,] colorGrid)
	{
		bool result = true;
		TileState[,] array = new TileState[m_sizeX, m_sizeY];
		for (int i = 0; i < m_sizeX; i++)
		{
			for (int j = 0; j < m_sizeY; j++)
			{
				array[i, j] = TileState.Empty;
			}
		}
		foreach (KeyValuePair<long, NKCOfficeFunitureData> item in roomData.m_dicFuniture)
		{
			if (selectedFunitureData != null && selectedFunitureData.uid == item.Key)
			{
				continue;
			}
			NKCOfficeFunitureData value = item.Value;
			if (value.eTarget != m_eTarget)
			{
				continue;
			}
			for (int k = value.PosX; k < value.PosX + value.SizeX; k++)
			{
				for (int l = value.PosY; l < value.PosY + value.SizeY; l++)
				{
					array[k, l] = TileState.Occupied;
				}
			}
		}
		if (selectedFunitureData != null && selectedFunitureData.eTarget == m_eTarget)
		{
			for (int m = selectedFunitureData.PosX; m < selectedFunitureData.PosX + selectedFunitureData.SizeX; m++)
			{
				if (m < 0 || m >= m_sizeX)
				{
					continue;
				}
				for (int n = selectedFunitureData.PosY; n < selectedFunitureData.PosY + selectedFunitureData.SizeY; n++)
				{
					if (n >= 0 && n < m_sizeY)
					{
						TileState tileState = array[m, n];
						if ((uint)tileState <= 1u || (uint)(tileState - 2) > 1u)
						{
							array[m, n] = TileState.Selected;
							continue;
						}
						array[m, n] = TileState.Prohibited;
						result = false;
					}
				}
			}
		}
		colorGrid = new Color[m_sizeX, m_sizeY];
		for (int num = 0; num < m_sizeX; num++)
		{
			for (int num2 = 0; num2 < m_sizeY; num2++)
			{
				colorGrid[num, num2] = GetColor(array[num, num2]);
			}
		}
		return result;
	}

	private Color GetColor(TileState state)
	{
		return state switch
		{
			TileState.Occupied => m_colOccupied, 
			TileState.Prohibited => m_colProhibited, 
			TileState.Selected => m_colSelect, 
			_ => m_colEmpty, 
		};
	}

	private void UpdateTileStatus(Color[,] tileState)
	{
		if (m_texTile == null)
		{
			return;
		}
		for (int i = 0; i < m_sizeX; i++)
		{
			for (int j = 0; j < m_sizeY; j++)
			{
				m_texTile.SetPixel(i, j, tileState[i, j]);
			}
		}
		m_texTile.Apply();
		m_image.material.SetTexture("_ColorTex", m_texTile);
	}

	private void UpdateLine()
	{
	}
}
