using UnityEngine;

namespace NKC;

public class NKCWarfareGameItem : MonoBehaviour
{
	public GameObject m_Ready;

	public GameObject m_Item;

	public GameObject m_Recv;

	private WARFARE_ITEM_STATE m_state;

	private int m_index = -1;

	private NKCAssetInstanceData m_instance;

	public readonly Vector3 m_PosAlone = new Vector3(-35f, -50f);

	public readonly Vector3 m_PosWithEnemy = new Vector3(0f, 0f);

	public int Index => m_index;

	public WARFARE_ITEM_STATE State => m_state;

	public static NKCWarfareGameItem GetNewInstance(Transform parent, int index)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_WARFARE", "NUM_WARFARE_ITEM");
		NKCWarfareGameItem component = nKCAssetInstanceData.m_Instant.GetComponent<NKCWarfareGameItem>();
		if (component == null)
		{
			Debug.LogError("NKCWarfareGameItem Prefab null!");
			return null;
		}
		component.m_instance = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
			component.transform.localScale = Vector3.one;
		}
		component.init(index);
		return component;
	}

	private void init(int index)
	{
		m_index = index;
		Set(WARFARE_ITEM_STATE.None, bWithEnemy: false);
	}

	public void Set(WARFARE_ITEM_STATE state, bool bWithEnemy)
	{
		m_state = state;
		base.gameObject.SetActive(state != WARFARE_ITEM_STATE.None);
		NKCUtil.SetGameobjectActive(m_Ready, state == WARFARE_ITEM_STATE.Question);
		NKCUtil.SetGameobjectActive(m_Item, state == WARFARE_ITEM_STATE.Item);
		NKCUtil.SetGameobjectActive(m_Recv, state == WARFARE_ITEM_STATE.Recv);
		SetPos(bWithEnemy);
	}

	public void SetPos(bool bWithEnemy)
	{
		GameObject obj = GetObj(m_state);
		if (obj != null)
		{
			obj.transform.localPosition = (bWithEnemy ? m_PosWithEnemy : m_PosAlone);
		}
	}

	private GameObject GetObj(WARFARE_ITEM_STATE state)
	{
		return state switch
		{
			WARFARE_ITEM_STATE.Question => m_Ready, 
			WARFARE_ITEM_STATE.Item => m_Item, 
			WARFARE_ITEM_STATE.Recv => m_Recv, 
			_ => null, 
		};
	}

	public void Close()
	{
		NKCAssetResourceManager.CloseInstance(m_instance);
		m_instance = null;
	}

	public Vector3 GetWorldPos()
	{
		return m_Item.transform.position;
	}
}
