using System;
using NKM;
using UnityEngine;

namespace NKC.UI;

public class NKCUIComTagConfirm : MonoBehaviour
{
	public enum TagType
	{
		OpenTag,
		ContentTag
	}

	[Serializable]
	public struct TagInfo
	{
		public TagType tagType;

		public string tagName;
	}

	public TagInfo[] tagInfo;

	public bool applyAsIgnoreTag;

	public bool setTargetObject;

	public GameObject[] objActiveWhenConfirmed;

	public GameObject[] objActiveWhenDenied;

	private void OnEnable()
	{
		bool flag = IsTagActivated();
		if (setTargetObject)
		{
			if (objActiveWhenConfirmed != null)
			{
				int num = objActiveWhenConfirmed.Length;
				for (int i = 0; i < num; i++)
				{
					NKCUtil.SetGameobjectActive(objActiveWhenConfirmed[i], flag);
				}
			}
			if (objActiveWhenDenied != null)
			{
				int num2 = objActiveWhenDenied.Length;
				for (int j = 0; j < num2; j++)
				{
					NKCUtil.SetGameobjectActive(objActiveWhenDenied[j], !flag);
				}
			}
		}
		else
		{
			base.gameObject.SetActive(flag);
		}
	}

	private bool IsTagActivated()
	{
		if (tagInfo == null)
		{
			return true;
		}
		bool flag = true;
		int num = tagInfo.Length;
		for (int i = 0; i < num; i++)
		{
			bool flag2 = true;
			switch (tagInfo[i].tagType)
			{
			case TagType.OpenTag:
				flag2 = NKMOpenTagManager.IsOpened(tagInfo[i].tagName);
				break;
			case TagType.ContentTag:
				flag2 = NKMContentsVersionManager.HasTag(tagInfo[i].tagName);
				break;
			}
			if (!setTargetObject && applyAsIgnoreTag)
			{
				flag2 = !flag2;
			}
			flag = flag && flag2;
		}
		return flag;
	}
}
