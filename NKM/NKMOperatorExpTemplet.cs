using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMOperatorExpTemplet : INKMTemplet
{
	private NKM_UNIT_GRADE m_NKM_UNIT_GRADE;

	private List<NKMOperatorExpData> datas = new List<NKMOperatorExpData>();

	public static int MaxLevel { get; private set; }

	public int Key => (int)m_NKM_UNIT_GRADE;

	public List<NKMOperatorExpData> values => datas;

	public NKMOperatorExpTemplet(NKM_UNIT_GRADE m_NKM_UNIT_GRADE, List<NKMOperatorExpData> datas)
	{
		if (datas == null || datas.Count == 0)
		{
			Log.ErrorAndExit($"Invalid data list. UnitGrade:{m_NKM_UNIT_GRADE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMOperatorExpTemplet.cs", 19);
			return;
		}
		this.m_NKM_UNIT_GRADE = m_NKM_UNIT_GRADE;
		this.datas.AddRange(datas);
		MaxLevel = Math.Max(MaxLevel, datas.Max((NKMOperatorExpData e) => e.m_iLevel));
	}

	public static NKMOperatorExpTemplet Find(NKM_UNIT_GRADE key)
	{
		return NKMTempletContainer<NKMOperatorExpTemplet>.Find((int)key);
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
