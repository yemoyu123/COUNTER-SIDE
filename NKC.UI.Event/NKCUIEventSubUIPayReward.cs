using System.Text;
using NKC.Templet;
using NKM;
using NKM.Event;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventSubUIPayReward : NKCUIEventSubUIBase
{
	public Text m_lbEventDesc;

	public NKCUIComStateButton m_csbtnOpen;

	private int m_eventId;

	public override void Init()
	{
		base.Init();
		NKCUtil.SetButtonClickDelegate(m_csbtnOpen, OpenSpecialCutscene);
	}

	public override void Open(NKMEventTabTemplet tabTemplet)
	{
		m_eventId = tabTemplet.m_EventID;
		m_tabTemplet = tabTemplet;
		SetDateLimit();
	}

	public override void Refresh()
	{
	}

	private void OpenSpecialCutscene()
	{
		NKCEventPaybackTemplet nKCEventPaybackTemplet = NKCEventPaybackTemplet.Find(m_eventId);
		if (nKCEventPaybackTemplet == null)
		{
			return;
		}
		NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(nKCEventPaybackTemplet.SkinId);
		if (skinTemplet != null && !string.IsNullOrEmpty(skinTemplet.m_LoginCutin))
		{
			string key = "PAYBACK_INTRO";
			string introString = GetIntroString(nKCEventPaybackTemplet.Key, nKCEventPaybackTemplet.SkinId);
			string text = PlayerPrefs.GetString(key);
			bool flag = !text.Contains(introString);
			bool flag2 = true;
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && (NKCDefineManager.DEFINE_UNITY_EDITOR() || NKCDefineManager.DEFINE_UNITY_STANDALONE()))
			{
				flag = true;
				flag2 = false;
			}
			if (flag)
			{
				NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(skinTemplet.m_LoginCutin, skinTemplet.m_LoginCutin);
				nKMAssetName.m_BundleName = nKMAssetName.m_BundleName.ToLower();
				NKCUIEventSequence nKCUIEventSequence = NKCUIEventSequence.OpenInstance(nKMAssetName);
				if (nKCUIEventSequence != null)
				{
					if (flag2)
					{
						if (!string.IsNullOrEmpty(text))
						{
							text = text.Insert(0, ",");
						}
						text = text.Insert(0, introString.ToString());
						PlayerPrefs.SetString(key, text.ToString());
					}
					nKCUIEventSequence.Open(OpenPayRewardPopup);
					return;
				}
			}
		}
		OpenPayRewardPopup();
	}

	private string GetIntroString(int eventId, int skinId)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(eventId);
		stringBuilder.Append("_");
		stringBuilder.Append(skinId);
		return stringBuilder.ToString();
	}

	private void OpenPayRewardPopup()
	{
		NKCEventPaybackTemplet nKCEventPaybackTemplet = NKCEventPaybackTemplet.Find(m_eventId);
		if (nKCEventPaybackTemplet != null)
		{
			NKCPopupEventPayReward.OpenInstance(nKCEventPaybackTemplet.BannerPrefabId, nKCEventPaybackTemplet.BannerPrefabId, m_eventId, nKCEventPaybackTemplet.MissionTabId);
		}
	}
}
