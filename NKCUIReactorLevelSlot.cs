using System.Collections.Generic;
using NKC;
using UnityEngine;

public class NKCUIReactorLevelSlot : MonoBehaviour
{
	public List<GameObject> m_lstLevelObjects;

	public void SetLevel(int level)
	{
		for (int i = 0; i < m_lstLevelObjects.Count; i++)
		{
			NKCUtil.SetGameobjectActive(m_lstLevelObjects[i], level == i);
		}
	}
}
