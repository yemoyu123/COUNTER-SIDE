using System.Collections.Generic;
using NKM;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public class NKCTutorialReqTemplet : INKMTemplet
{
	public int EventID;

	public TutorialStep Step;

	public TutorialPoint EventPoint;

	public Dictionary<TutorialReq, string> dicReq = new Dictionary<TutorialReq, string>();

	public int Key => EventID;

	public static NKCTutorialReqTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCTutorialManager.cs", 166))
		{
			return null;
		}
		NKCTutorialReqTemplet nKCTutorialReqTemplet = new NKCTutorialReqTemplet();
		int num = (int)(1u & (lua.GetData("EventID", ref nKCTutorialReqTemplet.EventID) ? 1u : 0u) & (lua.GetData("Step", ref nKCTutorialReqTemplet.Step) ? 1u : 0u)) & (lua.GetData("EventPoint", ref nKCTutorialReqTemplet.EventPoint) ? 1 : 0);
		string rValue = string.Empty;
		if (lua.GetData("Req", out var result, TutorialReq.None))
		{
			lua.GetData("ReqValue", ref rValue);
			nKCTutorialReqTemplet.dicReq.Add(result, rValue);
		}
		rValue = string.Empty;
		if (lua.GetData("Req2", out result, TutorialReq.None))
		{
			lua.GetData("Req2Value", ref rValue);
			nKCTutorialReqTemplet.dicReq.Add(result, rValue);
		}
		if (num == 0)
		{
			Debug.LogError("NKCTutorialReqTemplet LoadFromLUA fail");
			return null;
		}
		return nKCTutorialReqTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
