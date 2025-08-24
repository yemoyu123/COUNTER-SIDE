using System.Collections.Generic;

namespace NKM;

public class SquareGrid
{
	public static readonly NKMAStarSearchLocation[] DIRS = new NKMAStarSearchLocation[8]
	{
		new NKMAStarSearchLocation(1, 0),
		new NKMAStarSearchLocation(0, -1),
		new NKMAStarSearchLocation(-1, 0),
		new NKMAStarSearchLocation(0, 1),
		new NKMAStarSearchLocation(1, 1),
		new NKMAStarSearchLocation(-1, 1),
		new NKMAStarSearchLocation(1, -1),
		new NKMAStarSearchLocation(-1, -1)
	};

	public static readonly NKMAStarSearchLocation[] DIRS_Cross = new NKMAStarSearchLocation[4]
	{
		new NKMAStarSearchLocation(1, 0),
		new NKMAStarSearchLocation(0, -1),
		new NKMAStarSearchLocation(-1, 0),
		new NKMAStarSearchLocation(0, 1)
	};

	public bool bCrossMoveType;

	public int x;

	public int y;

	public int width;

	public int height;

	public NKM_ASTAR_SEARCH_TILE_TYPE[,] tiles;

	public SquareGrid(int x, int y, int width, int height)
	{
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	}

	public bool InBounds(NKMAStarSearchLocation id)
	{
		if (x <= id.x && id.x < width && y <= id.y)
		{
			return id.y < height;
		}
		return false;
	}

	public bool Passable(NKMAStarSearchLocation id)
	{
		return tiles[id.x, id.y] < NKM_ASTAR_SEARCH_TILE_TYPE.NASTT_WALL;
	}

	public float Cost(NKMAStarSearchLocation a, NKMAStarSearchLocation b)
	{
		return (float)tiles[b.x, b.y];
	}

	public IEnumerable<NKMAStarSearchLocation> Neighbors(NKMAStarSearchLocation id)
	{
		NKMAStarSearchLocation[] dIRS;
		if (!bCrossMoveType)
		{
			dIRS = DIRS;
			foreach (NKMAStarSearchLocation nKMAStarSearchLocation in dIRS)
			{
				NKMAStarSearchLocation nKMAStarSearchLocation2 = new NKMAStarSearchLocation(id.x + nKMAStarSearchLocation.x, id.y + nKMAStarSearchLocation.y);
				if (InBounds(nKMAStarSearchLocation2) && Passable(nKMAStarSearchLocation2))
				{
					yield return nKMAStarSearchLocation2;
				}
			}
			yield break;
		}
		dIRS = DIRS_Cross;
		foreach (NKMAStarSearchLocation nKMAStarSearchLocation3 in dIRS)
		{
			NKMAStarSearchLocation nKMAStarSearchLocation4 = new NKMAStarSearchLocation(id.x + nKMAStarSearchLocation3.x, id.y + nKMAStarSearchLocation3.y);
			if (InBounds(nKMAStarSearchLocation4) && Passable(nKMAStarSearchLocation4))
			{
				yield return nKMAStarSearchLocation4;
			}
		}
	}
}
