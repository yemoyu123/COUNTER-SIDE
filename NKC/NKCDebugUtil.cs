using System.Collections.Generic;
using System.Text;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Cs.Logging;
using NKM;
using UnityEngine;

namespace NKC;

public static class NKCDebugUtil
{
	private static StringBuilder sb = new StringBuilder();

	public static void DebugDrawRect(Rect rect, Vector2 vOffset, Vector2 vEnlarge, Color color)
	{
	}

	public static void DebugDrawRect(Vector2 min, Vector2 max, Color color)
	{
		Vector3 vector = min;
		Vector3 vector2 = max;
		Vector3 vector3 = new Vector3(min.x, max.y);
		Vector3 vector4 = new Vector3(max.x, min.y);
		Debug.DrawLine(vector, vector3, color);
		Debug.DrawLine(vector3, vector2, color);
		Debug.DrawLine(vector2, vector4, color);
		Debug.DrawLine(vector4, vector, color);
	}

	public static void DebugDrawRect(Transform localTransform, Vector2 localMin, Vector2 localMax, Color color)
	{
		Vector3 vector = localTransform.TransformPoint(localMin);
		Vector3 vector2 = localTransform.TransformPoint(localMax);
		Vector3 vector3 = new Vector3(vector.x, vector2.y);
		Vector3 vector4 = new Vector3(vector2.x, vector.y);
		Debug.DrawLine(vector, vector3, color);
		Debug.DrawLine(vector3, vector2, color);
		Debug.DrawLine(vector2, vector4, color);
		Debug.DrawLine(vector4, vector, color);
	}

	public static void DebugDrawCircle(Transform rect, Vector3 center, float radius, Color color, int segments = 32)
	{
	}

	public static string GetObjectPath(GameObject go, bool StopAtClone = true)
	{
		if (go == null)
		{
			return "null";
		}
		return GetObjectPath(go.transform, StopAtClone);
	}

	public static string GetObjectPath(Transform t, bool StopAtClone = true)
	{
		if (t == null)
		{
			return "null";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(t.name);
		Transform parent = t.parent;
		while (parent != null)
		{
			stringBuilder.Insert(0, parent.name + "/");
			if (StopAtClone && parent.name.EndsWith("(Clone)"))
			{
				break;
			}
			parent = parent.parent;
		}
		return stringBuilder.ToString();
	}

	public static string ToDebugString<T>(IEnumerable<T> target)
	{
		if (target == null)
		{
			return "null";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(target.GetType().ToString());
		stringBuilder.Append("[");
		bool flag = false;
		foreach (T item in target)
		{
			stringBuilder.Append(item.ToString());
			stringBuilder.Append(", ");
			flag = true;
		}
		if (flag)
		{
			stringBuilder.Remove(stringBuilder.Length - 2, 2);
		}
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public static void LogBehaivorTree(BehaviorTree behaivor)
	{
		if (behaivor == null)
		{
			return;
		}
		Cs.Logging.Log.Info("Behaivor name : " + behaivor.BehaviorName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCDebugUtil.cs", 144);
		if (behaivor.GetAllVariables() == null)
		{
			Cs.Logging.Log.Info("Variables not initialized, or have no variables", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCDebugUtil.cs", 148);
		}
		else
		{
			Cs.Logging.Log.Info("Behaivor Variables", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCDebugUtil.cs", 152);
			foreach (SharedVariable allVariable in behaivor.GetAllVariables())
			{
				if (allVariable == null)
				{
					Cs.Logging.Log.Error("null variable found!!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCDebugUtil.cs", 157);
					continue;
				}
				string text = allVariable.GetType().ToString();
				Cs.Logging.Log.Info("Variable " + allVariable.Name + "(" + text + ") : " + allVariable.ToString(), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCDebugUtil.cs", 161);
			}
		}
		Cs.Logging.Log.Info("Behaivor nodes", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCDebugUtil.cs", 165);
		foreach (Task item in behaivor.FindTasks<Task>())
		{
			string text2 = item.GetType().ToString();
			if (text2.Contains("UnknownTask"))
			{
				Cs.Logging.Log.Error($"Task ID : {item.ID}, Type : {text2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCDebugUtil.cs", 172);
			}
			else
			{
				Cs.Logging.Log.Info($"Task ID : {item.ID}, Type : {text2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCDebugUtil.cs", 176);
			}
		}
	}

	public static Color GetAttackBoxColor(NKMEventAttack cNKMEventAttack, Color defaultcolor)
	{
		if (cNKMEventAttack.m_bTrueDamage)
		{
			return Color.white;
		}
		if (cNKMEventAttack.m_bForceCritical)
		{
			return Color.red;
		}
		if (cNKMEventAttack.m_bCleanHit)
		{
			return Color.green;
		}
		if (cNKMEventAttack.m_bNoCritical)
		{
			return Color.yellow;
		}
		return defaultcolor;
	}

	public static string BuildAttackText(NKMEventAttack cNKMEventAttack)
	{
		sb.Clear();
		bool bAppend = false;
		NKMDamageTemplet nKMDamageTemplet = null;
		if (!string.IsNullOrEmpty(cNKMEventAttack.m_DamageTempletName))
		{
			sb.Append("<size=30>");
			sb.Append(cNKMEventAttack.m_DamageTempletName);
			nKMDamageTemplet = NKMDamageManager.GetTempletByStrID(cNKMEventAttack.m_DamageTempletName);
			if (nKMDamageTemplet != null)
			{
				sb.Append('(');
				if (nKMDamageTemplet.m_DamageTempletBase.m_AtkFactorStat != NKM_STAT_TYPE.NST_ATK)
				{
					sb.Append(nKMDamageTemplet.m_DamageTempletBase.m_AtkFactorStat);
					sb.Append(" ");
				}
				sb.Append(nKMDamageTemplet.m_DamageTempletBase.m_fAtkFactor);
				if (nKMDamageTemplet.m_DamageTempletBase.m_fAtkMaxHPRateFactor > 0f)
				{
					sb.Append("·");
					sb.Append("MaxHP ");
					sb.Append(nKMDamageTemplet.m_DamageTempletBase.m_fAtkMaxHPRateFactor);
				}
				if (nKMDamageTemplet.m_DamageTempletBase.m_fAtkHPRateFactor > 0f)
				{
					sb.Append("·");
					sb.Append("HP ");
					sb.Append(nKMDamageTemplet.m_DamageTempletBase.m_fAtkHPRateFactor);
				}
				sb.Append(')');
			}
			sb.AppendLine("</size>");
			if (nKMDamageTemplet.m_ApplyStatusEffect != NKM_UNIT_STATUS_EFFECT.NUSE_NONE)
			{
				sb.Append(nKMDamageTemplet.m_ApplyStatusEffect);
				sb.Append('(');
				sb.Append(nKMDamageTemplet.m_fApplyStatusTime);
				sb.Append(')');
				bAppend = true;
			}
		}
		ApplyBool("m_bTrueDamage", cNKMEventAttack.m_bTrueDamage);
		ApplyBool("m_bForceHit", cNKMEventAttack.m_bForceHit);
		ApplyBool("m_bCleanHit", cNKMEventAttack.m_bCleanHit);
		ApplyBool("m_bForceCritical", cNKMEventAttack.m_bForceCritical);
		ApplyBool("m_bNoCritical", cNKMEventAttack.m_bNoCritical);
		ApplyBool("m_bDamageSpeedDependRight", cNKMEventAttack.m_bDamageSpeedDependRight);
		if (cNKMEventAttack.m_fGetAgroTime > 0f)
		{
			if (bAppend)
			{
				sb.Append("·");
			}
			else
			{
				bAppend = true;
			}
			sb.Append("m_fGetAgroTime = ");
			sb.Append(cNKMEventAttack.m_fGetAgroTime);
		}
		if (nKMDamageTemplet != null && nKMDamageTemplet.m_ExtraHitDamageTemplet != null)
		{
			NKMDamageTemplet extraHitDamageTemplet = nKMDamageTemplet.m_ExtraHitDamageTemplet;
			sb.AppendLine();
			sb.Append("ExtraHitDT");
			sb.Append('[');
			sb.Append(extraHitDamageTemplet.m_ExtraHitCountRange.m_Min);
			if (extraHitDamageTemplet.m_ExtraHitCountRange.m_Max > extraHitDamageTemplet.m_ExtraHitCountRange.m_Min)
			{
				sb.Append('-');
				sb.Append(extraHitDamageTemplet.m_ExtraHitCountRange.m_Max);
			}
			sb.Append("] : ");
			sb.Append(nKMDamageTemplet.m_ExtraHitDamageTempletID);
			sb.Append('(');
			if (nKMDamageTemplet.m_DamageTempletBase.m_AtkFactorStat != NKM_STAT_TYPE.NST_ATK)
			{
				sb.Append(nKMDamageTemplet.m_DamageTempletBase.m_AtkFactorStat);
				sb.Append(" ");
			}
			sb.Append(extraHitDamageTemplet.m_DamageTempletBase.m_fAtkFactor);
			if (extraHitDamageTemplet.m_DamageTempletBase.m_fAtkMaxHPRateFactor > 0f)
			{
				sb.Append("·");
				sb.Append("MaxHP ");
				sb.Append(extraHitDamageTemplet.m_DamageTempletBase.m_fAtkMaxHPRateFactor);
			}
			if (extraHitDamageTemplet.m_DamageTempletBase.m_fAtkHPRateFactor > 0f)
			{
				sb.Append("·");
				sb.Append("HP ");
				sb.Append(extraHitDamageTemplet.m_DamageTempletBase.m_fAtkHPRateFactor);
			}
			sb.Append(')');
			if (nKMDamageTemplet.m_ExtraHitAttribute != null)
			{
				sb.AppendLine();
				bAppend = false;
				ApplyBool("m_bTrueDamage", nKMDamageTemplet.m_ExtraHitAttribute.m_bTrueDamage);
				ApplyBool("m_bForceCritical", nKMDamageTemplet.m_ExtraHitAttribute.m_bForceCritical);
				ApplyBool("m_bNoCritical", nKMDamageTemplet.m_ExtraHitAttribute.m_bNoCritical);
			}
		}
		return sb.ToString();
		void ApplyBool(string name, bool value)
		{
			if (value)
			{
				if (bAppend)
				{
					sb.Append("·");
				}
				else
				{
					bAppend = true;
				}
				sb.Append(name);
			}
		}
	}
}
