using System.Collections.Generic;
using UnityEngine;

namespace NKC.FX;

public class NKC_FX_ROOT : MonoBehaviour
{
	private List<GameObject> vfxObjects = new List<GameObject>();

	private void Awake()
	{
		if (base.transform.parent != null)
		{
			GameObject gameObject = base.transform.parent.gameObject;
			vfxObjects.Clear();
			NKC_FX_ACTIVE_ON_WORLD_SPACE[] componentsInChildren = gameObject.GetComponentsInChildren<NKC_FX_ACTIVE_ON_WORLD_SPACE>(includeInactive: true);
			if (componentsInChildren != null && componentsInChildren.Length != 0)
			{
				NKC_FX_ACTIVE_ON_WORLD_SPACE[] array = componentsInChildren;
				foreach (NKC_FX_ACTIVE_ON_WORLD_SPACE nKC_FX_ACTIVE_ON_WORLD_SPACE in array)
				{
					if (!vfxObjects.Contains(nKC_FX_ACTIVE_ON_WORLD_SPACE.gameObject))
					{
						vfxObjects.Add(nKC_FX_ACTIVE_ON_WORLD_SPACE.gameObject);
					}
				}
			}
			NKC_FX_DELAY_EXECUTER[] componentsInChildren2 = gameObject.GetComponentsInChildren<NKC_FX_DELAY_EXECUTER>(includeInactive: true);
			if (componentsInChildren2 != null && componentsInChildren2.Length != 0)
			{
				NKC_FX_DELAY_EXECUTER[] array2 = componentsInChildren2;
				foreach (NKC_FX_DELAY_EXECUTER nKC_FX_DELAY_EXECUTER in array2)
				{
					if (!vfxObjects.Contains(nKC_FX_DELAY_EXECUTER.gameObject))
					{
						vfxObjects.Add(nKC_FX_DELAY_EXECUTER.gameObject);
					}
				}
			}
			NKC_FXM_PLAYER[] componentsInChildren3 = gameObject.GetComponentsInChildren<NKC_FXM_PLAYER>(includeInactive: true);
			if (componentsInChildren3 != null && componentsInChildren3.Length != 0)
			{
				NKC_FXM_PLAYER[] array3 = componentsInChildren3;
				foreach (NKC_FXM_PLAYER nKC_FXM_PLAYER in array3)
				{
					if (!vfxObjects.Contains(nKC_FXM_PLAYER.gameObject))
					{
						vfxObjects.Add(nKC_FXM_PLAYER.gameObject);
					}
				}
			}
			vfxObjects.TrimExcess();
		}
		ChildAllDisable();
	}

	private void OnDestroy()
	{
		if (vfxObjects == null || vfxObjects.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < vfxObjects.Count; i++)
		{
			if (vfxObjects[i] != null)
			{
				Object.Destroy(vfxObjects[i], Time.maximumDeltaTime * 2f);
			}
		}
		vfxObjects.Clear();
		vfxObjects.TrimExcess();
	}

	private void OnDisable()
	{
		if (vfxObjects != null && vfxObjects.Count > 0)
		{
			for (int i = 0; i < vfxObjects.Count; i++)
			{
				if (vfxObjects[i] != null)
				{
					NKC_FX_ACTIVE_ON_WORLD_SPACE component = vfxObjects[i].GetComponent<NKC_FX_ACTIVE_ON_WORLD_SPACE>();
					if (component != null)
					{
						component.ReParent();
					}
					NKC_FX_DELAY_EXECUTER component2 = vfxObjects[i].GetComponent<NKC_FX_DELAY_EXECUTER>();
					if (component2 != null)
					{
						component2.Stop();
					}
					NKC_FXM_PLAYER component3 = vfxObjects[i].GetComponent<NKC_FXM_PLAYER>();
					if (component3 != null)
					{
						component3.Stop();
					}
				}
			}
		}
		ChildAllDisable();
	}

	public void ChildAllDisable()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			GameObject gameObject = base.transform.GetChild(i).gameObject;
			if (gameObject.activeSelf && gameObject.name != "SPINE_SkeletonGraphic")
			{
				gameObject.SetActive(value: false);
			}
		}
	}
}
