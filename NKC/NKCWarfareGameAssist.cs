using NKC.FX;
using UnityEngine;

namespace NKC;

public class NKCWarfareGameAssist : MonoBehaviour
{
	public Transform m_Start;

	public Transform m_Target;

	private NKCAssetInstanceData m_instance;

	public static NKCWarfareGameAssist GetNewInstance(Transform parent, Vector3 start, Vector3 target)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_FX_PF_CMN", "AB_FX_PF_ARC_DISTANCE_WARFARE");
		NKCWarfareGameAssist component = nKCAssetInstanceData.m_Instant.GetComponent<NKCWarfareGameAssist>();
		if (component == null)
		{
			Debug.LogError("NKCWarfareGameAssist Prefab null!");
			return null;
		}
		component.m_instance = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
			component.transform.localScale = Vector3.one;
		}
		component.Init(start, target);
		component.GetComponent<NKC_FXM_PLAYER>().Restart();
		return component;
	}

	private void Init(Vector3 start, Vector3 target)
	{
		m_Start.localPosition = start;
		m_Target.localPosition = target;
	}

	public void Close()
	{
		NKCAssetResourceManager.CloseInstance(m_instance);
	}
}
