using System;
using System.Text;
using NKC.Loading;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUILoadingScreen : MonoBehaviour
{
	public GameObject m_objBigLoadingScreen;

	public Text m_lbTip;

	public Text m_lbProgress;

	public Text m_lbProgressCount;

	public Image m_imgProgress;

	public GameObject m_objDefaultLoadingScreen;

	public Image m_imgFull;

	public Image m_imgCartoon;

	public GameObject m_rootCartoon;

	public void Init()
	{
		base.gameObject.SetActive(value: false);
		NKCUtil.SetGameobjectActive(m_objBigLoadingScreen, bValue: false);
	}

	public void ShowMainLoadingUI(NKM_GAME_TYPE gameType, int contentValue = 0)
	{
		ShowMainLoadingUI(NKCLoadingScreenManager.GetGameContentsType(gameType), contentValue);
	}

	public void ShowMainLoadingUI(NKCLoadingScreenManager.eGameContentsType contentType = NKCLoadingScreenManager.eGameContentsType.DEFAULT, int contentValue = 0, int dungeonID = 0)
	{
		if (base.gameObject.activeSelf)
		{
			return;
		}
		Tuple<NKCLoadingScreenManager.NKCLoadingImgTemplet, string> loadingScreen = NKCLoadingScreenManager.GetLoadingScreen(contentType, contentValue, dungeonID);
		NKCLoadingScreenManager.NKCLoadingImgTemplet item = loadingScreen.Item1;
		if (item == null)
		{
			NKCUtil.SetGameobjectActive(m_objDefaultLoadingScreen, bValue: true);
			NKCUtil.SetGameobjectActive(m_rootCartoon, bValue: false);
			NKCUtil.SetGameobjectActive(m_imgFull, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objDefaultLoadingScreen, bValue: false);
			Sprite sprite = LoadSprite(item.m_ImgAssetName);
			if (sprite != null)
			{
				switch (item.m_eImgType)
				{
				case NKCLoadingScreenManager.NKCLoadingImgTemplet.eImgType.FULL:
					NKCUtil.SetGameobjectActive(m_imgFull, bValue: true);
					NKCUtil.SetGameobjectActive(m_rootCartoon, bValue: false);
					NKCUtil.SetImageSprite(m_imgFull, sprite);
					break;
				case NKCLoadingScreenManager.NKCLoadingImgTemplet.eImgType.CARTOON:
					NKCUtil.SetGameobjectActive(m_imgFull, bValue: false);
					NKCUtil.SetGameobjectActive(m_rootCartoon, bValue: true);
					NKCUtil.SetImageSprite(m_imgCartoon, sprite);
					break;
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objDefaultLoadingScreen, bValue: true);
				NKCUtil.SetGameobjectActive(m_rootCartoon, bValue: false);
				NKCUtil.SetGameobjectActive(m_imgFull, bValue: false);
			}
		}
		if (string.IsNullOrEmpty(loadingScreen.Item2))
		{
			NKCUtil.SetLabelText(m_lbTip, "");
		}
		else
		{
			NKCUtil.SetLabelText(m_lbTip, NKCStringTable.GetString(loadingScreen.Item2));
		}
		NKCUtil.SetGameobjectActive(m_objBigLoadingScreen, bValue: true);
		base.gameObject.SetActive(value: true);
	}

	public void CloseLoadingUI()
	{
		base.gameObject.SetActive(value: false);
		NKCUtil.SetImageSprite(m_imgFull, null);
		NKCUtil.SetImageSprite(m_imgCartoon, null);
	}

	public void SetLoadingProgress(float fProgress)
	{
		StringBuilder builder = NKMString.GetBuilder();
		builder.AppendFormat("{0}%", (int)(fProgress * 100f));
		NKCUtil.SetLabelText(m_lbProgress, NKCUtilString.GET_STRING_ATTACK_PREPARING);
		NKCUtil.SetLabelText(m_lbProgressCount, builder.ToString());
		m_imgProgress.fillAmount = fProgress;
	}

	public void SetWaitOpponent()
	{
		NKCUtil.SetLabelText(m_lbProgress, NKCUtilString.GET_STRING_ATTACK_WAITING_OPPONENT);
		NKCUtil.SetLabelText(m_lbProgressCount, "100%");
		m_imgProgress.fillAmount = 1f;
	}

	private Sprite LoadSprite(string assetName)
	{
		NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName("ab_loading", assetName);
		if (NKCAssetResourceManager.IsBundleExists(nKMAssetName.m_BundleName, nKMAssetName.m_AssetName))
		{
			return NKCResourceUtility.GetOrLoadAssetResource<Sprite>(nKMAssetName);
		}
		return null;
	}
}
