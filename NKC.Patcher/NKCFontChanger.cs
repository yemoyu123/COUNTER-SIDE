using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NKC.Patcher;

public class NKCFontChanger : MonoBehaviour
{
	[Serializable]
	public class LocalizedFont
	{
		public NKM_NATIONAL_CODE m_nationCode;

		public Font m_mainFont;

		public TMP_FontAsset m_mainTMPFont;
	}

	public List<LocalizedFont> m_listLocalizedFont = new List<LocalizedFont>();

	public void ChagneAllMainFont(NKM_NATIONAL_CODE nationCode)
	{
		if (nationCode != NKM_NATIONAL_CODE.NNC_END && m_listLocalizedFont != null)
		{
			LocalizedFont localizedFont = m_listLocalizedFont.Find((LocalizedFont x) => x.m_nationCode == nationCode);
			if (localizedFont != null)
			{
				ChangeFontInScene(localizedFont.m_mainFont, localizedFont.m_mainTMPFont, "MainFont", "TmpFont");
			}
		}
	}

	public void ChangeFontInScene(Font font, TMP_FontAsset tmpFont, string fontName, string tmpFontName)
	{
		GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
		if (rootGameObjects == null)
		{
			return;
		}
		GameObject[] array = rootGameObjects;
		foreach (GameObject gameObject in array)
		{
			if (gameObject == null)
			{
				continue;
			}
			if (font != null)
			{
				Text[] componentsInChildren = gameObject.GetComponentsInChildren<Text>(includeInactive: true);
				if (componentsInChildren != null)
				{
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						if (!(componentsInChildren[j] == null) && !(componentsInChildren[j].font == null) && !(componentsInChildren[j].font.name != fontName))
						{
							componentsInChildren[j].font = font;
						}
					}
				}
			}
			if (!(tmpFont != null))
			{
				continue;
			}
			TextMeshProUGUI[] componentsInChildren2 = gameObject.GetComponentsInChildren<TextMeshProUGUI>(includeInactive: true);
			if (componentsInChildren2 == null)
			{
				continue;
			}
			for (int k = 0; k < componentsInChildren2.Length; k++)
			{
				if (!(componentsInChildren2[k] == null) && !(componentsInChildren2[k].font == null) && !(componentsInChildren2[k].font.name != tmpFontName))
				{
					componentsInChildren2[k].font = tmpFont;
				}
			}
		}
	}
}
