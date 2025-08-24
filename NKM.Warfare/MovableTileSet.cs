using NKM.Templet;

namespace NKM.Warfare;

public sealed class MovableTileSet
{
	private static readonly TileVector Origin = new TileVector(2, 2);

	public const int MAX_WIDTH = 5;

	private static readonly MovableTileSet[] Preset = new MovableTileSet[4]
	{
		new MovableTileSet(new byte[5, 5]
		{
			{ 0, 0, 0, 0, 0 },
			{ 0, 1, 1, 1, 0 },
			{ 0, 1, 0, 1, 0 },
			{ 0, 1, 1, 1, 0 },
			{ 0, 0, 0, 0, 0 }
		}),
		new MovableTileSet(new byte[5, 5]
		{
			{ 0, 0, 0, 0, 0 },
			{ 1, 1, 1, 1, 1 },
			{ 1, 1, 0, 1, 1 },
			{ 1, 1, 1, 1, 1 },
			{ 0, 0, 0, 0, 0 }
		}),
		new MovableTileSet(new byte[5, 5]
		{
			{ 0, 0, 1, 0, 0 },
			{ 0, 1, 1, 1, 0 },
			{ 1, 1, 0, 1, 1 },
			{ 0, 1, 1, 1, 0 },
			{ 0, 0, 1, 0, 0 }
		}),
		new MovableTileSet(new byte[5, 5]
		{
			{ 1, 0, 0, 0, 1 },
			{ 0, 1, 1, 1, 0 },
			{ 0, 1, 0, 1, 0 },
			{ 0, 1, 1, 1, 0 },
			{ 1, 0, 0, 0, 1 }
		})
	};

	public bool this[int x, int y] => TileSet[x, y] > 0;

	public byte[,] TileSet { get; }

	private MovableTileSet(byte[,] tileSet)
	{
		TileSet = tileSet;
	}

	public static MovableTileSet GetTileSet(NKM_UNIT_STYLE_TYPE styleType)
	{
		return styleType switch
		{
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_HEAVY => Preset[1], 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_CRUISER => Preset[2], 
			NKM_UNIT_STYLE_TYPE.NUST_SHIP_SPECIAL => Preset[3], 
			_ => Preset[0], 
		};
	}

	public bool IsMovable(in TilePosition from, in TilePosition to)
	{
		TileVector tileVector = Origin + (to - from);
		if (tileVector.X < 0 || tileVector.X >= TileSet.GetLength(0) || tileVector.Y < 0 || tileVector.Y >= TileSet.GetLength(1))
		{
			return false;
		}
		return TileSet[tileVector.Y, tileVector.X] > 0;
	}
}
