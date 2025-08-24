using UnityEngine;

namespace NKC.Util;

public static class NKCUIUtility
{
	public static Canvas FindCanvas(Transform t)
	{
		Transform transform = t;
		while (transform != null)
		{
			Canvas component = transform.GetComponent<Canvas>();
			if (component != null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return null;
	}
}
