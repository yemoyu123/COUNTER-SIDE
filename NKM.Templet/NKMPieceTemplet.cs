using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMPieceTemplet : INKMTemplet
{
	public int m_PieceId;

	public int m_PieceGetUintId;

	public int m_PieceReqFirst;

	public int m_PieceReq;

	public string m_OpenTag;

	public int Key => m_PieceId;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public static NKMPieceTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPieceTemplet.cs", 21))
		{
			return null;
		}
		NKMPieceTemplet nKMPieceTemplet = new NKMPieceTemplet();
		int num = (int)(1u & (cNKMLua.GetData("m_PieceID", ref nKMPieceTemplet.m_PieceId) ? 1u : 0u) & (cNKMLua.GetData("m_PieceGetUnitID", ref nKMPieceTemplet.m_PieceGetUintId) ? 1u : 0u) & (cNKMLua.GetData("m_PieceReq_First", ref nKMPieceTemplet.m_PieceReqFirst) ? 1u : 0u)) & (cNKMLua.GetData("m_PieceReq", ref nKMPieceTemplet.m_PieceReq) ? 1 : 0);
		if (NKMContentsVersionManager.HasDFChangeTagType(DataFormatChangeTagType.OPEN_TAG_PIECE))
		{
			cNKMLua.GetData("m_OpenTag", ref nKMPieceTemplet.m_OpenTag);
		}
		if (num == 0)
		{
			return null;
		}
		return nKMPieceTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (m_PieceReqFirst <= 0)
		{
			Log.ErrorAndExit($"pieceReqFirst is empty, pieceId: {m_PieceId}, value: {m_PieceReqFirst}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPieceTemplet.cs", 48);
		}
		if (m_PieceReq <= 0)
		{
			Log.ErrorAndExit($"pieceReq is empty, pieceId: {m_PieceId}, value: {m_PieceReq}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPieceTemplet.cs", 53);
		}
		if (NKMItemManager.GetItemMiscTempletByID(m_PieceId) == null)
		{
			Log.ErrorAndExit($"piece id is invalid, pieceId: {m_PieceId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPieceTemplet.cs", 58);
		}
		NKMUnitTempletBase nKMUnitTempletBase = NKMTempletContainer<NKMUnitTempletBase>.Find(m_PieceGetUintId);
		if (nKMUnitTempletBase == null)
		{
			Log.ErrorAndExit($"unit id is invalid, pieceId: {m_PieceId} pieceGetUnitId: {m_PieceGetUintId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPieceTemplet.cs", 64);
		}
		if (NKMContentsVersionManager.HasDFChangeTagType(DataFormatChangeTagType.OPEN_TAG_PIECE) && EnableByTag && !nKMUnitTempletBase.CollectionEnableByTag)
		{
			Log.ErrorAndExit($"Unit BasicTag is closed, pieceId: {m_PieceId} pieceGetUnitId: {m_PieceGetUintId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPieceTemplet.cs", 71);
		}
		if (m_PieceReqFirst <= m_PieceReq)
		{
			Log.ErrorAndExit($"m_PieceReqFirt({m_PieceReqFirst}) mubst be bigger than m_PieceReq({m_PieceReq}), pieceId: {m_PieceId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPieceTemplet.cs", 77);
		}
	}

	public NKM_ERROR_CODE CanExchange(NKMUserData userData)
	{
		long num = (userData.m_ArmyData.IsCollectedUnit(m_PieceGetUintId) ? m_PieceReq : m_PieceReqFirst);
		if (userData.m_InventoryData.GetCountMiscItem(m_PieceId) < num)
		{
			return NKM_ERROR_CODE.NEC_FAIL_ITEM_INSUFFICIENT_COUNT;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}
}
