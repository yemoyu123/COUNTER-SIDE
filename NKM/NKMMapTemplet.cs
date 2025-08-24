using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM;

public class NKMMapTemplet : INKMTemplet
{
	public int m_MapID;

	public string m_MapStrID = "";

	public NKMAssetName m_MapAssetName = new NKMAssetName();

	public bool m_bUsePVP;

	public string m_MusicName = "";

	public string m_PVPMusicName = "";

	public string m_AmbientName = "";

	public float m_fInitPosX;

	public float m_fCamMinX;

	public float m_fCamMaxX;

	public float m_fCamMinY;

	public float m_fCamMaxY;

	public float m_fCamSize = 500f;

	public float m_fCamSizeMax = 512f;

	public float m_fMinX;

	public float m_fMaxX;

	public float m_fMinZ;

	public float m_fMaxZ;

	public List<int> m_GroupIdList = new List<int>();

	public List<NKMBloomPoint> m_listBloomPoint = new List<NKMBloomPoint>();

	public List<NKMMapLayer> m_listMapLayer = new List<NKMMapLayer>();

	public int Key => m_MapID;

	public float GetUnitMinX()
	{
		return m_fMinX + 5f;
	}

	public float GetUnitMaxX()
	{
		return m_fMaxX - 5f;
	}

	public static NKMMapTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		NKMMapTemplet nKMMapTemplet = new NKMMapTemplet();
		cNKMLua.GetData("m_MapID", ref nKMMapTemplet.m_MapID);
		cNKMLua.GetData("m_MapStrID", ref nKMMapTemplet.m_MapStrID);
		if (!nKMMapTemplet.m_MapAssetName.LoadFromLua(cNKMLua, "m_MapAssetName"))
		{
			nKMMapTemplet.m_MapAssetName.m_BundleName = nKMMapTemplet.m_MapStrID;
			nKMMapTemplet.m_MapAssetName.m_AssetName = nKMMapTemplet.m_MapStrID;
		}
		cNKMLua.GetData("m_bUsePVP", ref nKMMapTemplet.m_bUsePVP);
		cNKMLua.GetData("m_MusicName", ref nKMMapTemplet.m_MusicName);
		nKMMapTemplet.m_PVPMusicName = "BATTLE_NORMAL_01";
		cNKMLua.GetData("m_PVPMusicName", ref nKMMapTemplet.m_PVPMusicName);
		cNKMLua.GetData("m_AmbientName", ref nKMMapTemplet.m_AmbientName);
		cNKMLua.GetData("m_fInitPosX", ref nKMMapTemplet.m_fInitPosX);
		cNKMLua.GetData("m_fCamMinX", ref nKMMapTemplet.m_fCamMinX);
		cNKMLua.GetData("m_fCamMaxX", ref nKMMapTemplet.m_fCamMaxX);
		cNKMLua.GetData("m_fCamMinY", ref nKMMapTemplet.m_fCamMinY);
		cNKMLua.GetData("m_fCamMaxY", ref nKMMapTemplet.m_fCamMaxY);
		cNKMLua.GetData("m_fCamSize", ref nKMMapTemplet.m_fCamSize);
		cNKMLua.GetData("m_fCamSizeMax", ref nKMMapTemplet.m_fCamSizeMax);
		cNKMLua.GetData("m_fMinX", ref nKMMapTemplet.m_fMinX);
		cNKMLua.GetData("m_fMaxX", ref nKMMapTemplet.m_fMaxX);
		cNKMLua.GetData("m_fMinZ", ref nKMMapTemplet.m_fMinZ);
		cNKMLua.GetData("m_fMaxZ", ref nKMMapTemplet.m_fMaxZ);
		cNKMLua.GetDataList("m_GroupIdList", out nKMMapTemplet.m_GroupIdList, nullIfEmpty: false);
		if (cNKMLua.OpenTable("m_listBloomPoint"))
		{
			int num = 1;
			while (cNKMLua.OpenTable(num))
			{
				NKMBloomPoint nKMBloomPoint = null;
				if (nKMMapTemplet.m_listBloomPoint.Count < num)
				{
					nKMBloomPoint = new NKMBloomPoint();
					nKMMapTemplet.m_listBloomPoint.Add(nKMBloomPoint);
				}
				else
				{
					nKMBloomPoint = nKMMapTemplet.m_listBloomPoint[num - 1];
				}
				nKMBloomPoint.LoadFromLUA(cNKMLua);
				num++;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		if (cNKMLua.OpenTable("m_listMapLayer"))
		{
			int num2 = 1;
			while (cNKMLua.OpenTable(num2))
			{
				NKMMapLayer nKMMapLayer = null;
				if (nKMMapTemplet.m_listMapLayer.Count < num2)
				{
					nKMMapLayer = new NKMMapLayer();
					nKMMapTemplet.m_listMapLayer.Add(nKMMapLayer);
				}
				else
				{
					nKMMapLayer = nKMMapTemplet.m_listMapLayer[num2 - 1];
				}
				nKMMapLayer.LoadFromLUA(cNKMLua);
				num2++;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		return nKMMapTemplet;
	}

	public bool IsValidLand(float fX, float fZ)
	{
		if (m_fMinX > fX || m_fMaxX < fX)
		{
			return false;
		}
		if (m_fMinZ > fZ || m_fMaxZ < fZ)
		{
			return false;
		}
		return true;
	}

	public bool IsValidLandX(float fX, bool bTeamA, float fFactor)
	{
		float num = m_fMinX;
		float num2 = m_fMaxX;
		float num3 = num2 - num;
		if (bTeamA)
		{
			num2 = num + num3 * fFactor;
		}
		else
		{
			num = num2 - num3 * fFactor;
		}
		if (num > fX || num2 < fX)
		{
			return false;
		}
		return true;
	}

	public float GetNearLandX(float fX, bool bTeamA, float fFactor, float minOffset)
	{
		float fMinX = m_fMinX;
		float fMaxX = m_fMaxX;
		if (bTeamA)
		{
			fMaxX = fMinX + (fMaxX - fMinX) * fFactor;
			fMinX += minOffset;
		}
		else
		{
			fMinX = fMaxX - (fMaxX - fMinX) * fFactor;
			fMaxX -= minOffset;
		}
		if (fMinX > fX)
		{
			return fMinX;
		}
		if (fMaxX < fX)
		{
			return fMaxX;
		}
		return fX;
	}

	public float GetMapRatePos(float fMapRate, bool bTeamA)
	{
		float num = m_fMaxX - m_fMinX;
		if (bTeamA)
		{
			return m_fMinX + num * fMapRate;
		}
		return m_fMaxX - num * fMapRate;
	}

	public float GetNearLandZ(float fZ)
	{
		if (m_fMinZ > fZ)
		{
			return m_fMinZ;
		}
		if (m_fMaxZ < fZ)
		{
			return m_fMaxZ;
		}
		return fZ;
	}

	public float GetValidLandX(bool bTeamA, float fFactor)
	{
		float fMinX = m_fMinX;
		float fMaxX = m_fMaxX;
		if (bTeamA)
		{
			return fMinX + (fMaxX - fMinX) * fFactor;
		}
		return fMaxX - (fMaxX - fMinX) * fFactor;
	}

	public float ReversePosX(float fPosX)
	{
		float num = m_fMaxX - m_fMinX;
		float num2 = m_fMinX + num * 0.5f;
		if (fPosX > num2)
		{
			return num2 - (fPosX - num2);
		}
		return num2 + (num2 - fPosX);
	}

	public float GetMapFactor(float fPosX, bool bTeamA)
	{
		float num = fPosX - m_fMinX;
		float num2 = m_fMaxX - m_fMinX;
		if (bTeamA)
		{
			return num / num2;
		}
		return 1f - num / num2;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
