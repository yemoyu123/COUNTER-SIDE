using System;
using System.Collections;
using DG.Tweening;
using NKM;
using NKM.Templet;
using UnityEngine;

namespace NKC.UI;

public class NKCUIPointExchangeTransition : NKCUIBase
{
	[Serializable]
	public struct TransInfo
	{
		public enum TransType
		{
			SLIDE_FROM_RIGHT,
			SLIDE_FROM_TOP
		}

		public TransType transType;

		public float popupValue;
	}

	public TransInfo m_transInfo;

	public RectTransform m_rtTransitionRoot;

	public RectTransform m_rtTransitionScreen;

	public Animator m_animator;

	[Header("\ufffdڵ\ufffd\ufffd \ufffd۵\ufffd\ufffdϴ\ufffd Ʈ\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public float m_duration;

	public bool m_enableCodeAnimation;

	public override string MenuName => "Point Exchange Transition";

	public override eMenutype eUIType => eMenutype.Overlay;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public static NKCUIPointExchangeTransition MakeInstance(string bundleName, string assetName)
	{
		NKCUIPointExchangeTransition instance = NKCUIManager.OpenNewInstance<NKCUIPointExchangeTransition>(bundleName, assetName, NKCUIManager.eUIBaseRect.UIFrontCommon, null).GetInstance<NKCUIPointExchangeTransition>();
		instance.InitUI();
		return instance;
	}

	public static NKCUIPointExchangeTransition MakeInstance(NKMPointExchangeTemplet pointExchangeTemplet = null)
	{
		if (pointExchangeTemplet == null)
		{
			pointExchangeTemplet = NKMPointExchangeTemplet.GetByTime(NKCSynchronizedTime.ServiceTime);
		}
		if (pointExchangeTemplet == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(pointExchangeTemplet.PrefabId))
		{
			return null;
		}
		NKMAssetName nKMAssetName = NKMAssetName.ParseBundleName(pointExchangeTemplet.PrefabId, pointExchangeTemplet.PrefabId);
		return MakeInstance(nKMAssetName.m_BundleName, nKMAssetName.m_AssetName + "_TRANS");
	}

	private void InitUI()
	{
		base.gameObject.SetActive(value: false);
	}

	public void Open()
	{
		base.gameObject.SetActive(value: true);
		StartCoroutine(IOpeningTransition());
		if (!base.IsOpen)
		{
			UIOpened();
		}
	}

	private IEnumerator IOpeningTransition()
	{
		if (m_enableCodeAnimation)
		{
			float num = NKCUIManager.GetUIFrontCanvasScaler().referenceResolution.x / NKCUIManager.GetUIFrontCanvasScaler().referenceResolution.y;
			float num2 = m_rtTransitionRoot.rect.width / m_rtTransitionRoot.rect.height;
			float num3 = m_rtTransitionScreen.rect.width - m_rtTransitionRoot.rect.width;
			if (num2 >= num)
			{
				Vector3 position = m_rtTransitionScreen.transform.position;
				position.x = m_rtTransitionRoot.rect.width + num3;
				m_rtTransitionScreen.transform.position = position;
			}
			else
			{
				float num4 = NKCUIManager.GetUIFrontCanvasScaler().referenceResolution.y / m_rtTransitionRoot.rect.height;
				Vector3 position2 = m_rtTransitionScreen.transform.position;
				position2.x = m_rtTransitionRoot.rect.width * num4 + num3;
				m_rtTransitionScreen.transform.position = position2;
			}
			m_rtTransitionScreen.DOMoveX(0f - m_rtTransitionRoot.rect.width - num3, m_duration).SetEase(Ease.Linear).OnComplete(base.Close);
		}
		TransInfo.TransType transType = m_transInfo.transType;
		if (transType != TransInfo.TransType.SLIDE_FROM_RIGHT)
		{
			if (transType == TransInfo.TransType.SLIDE_FROM_TOP)
			{
				while (m_rtTransitionScreen.transform.position.y > m_transInfo.popupValue)
				{
					yield return null;
				}
			}
		}
		else
		{
			while (m_rtTransitionScreen.transform.position.x > m_transInfo.popupValue)
			{
				yield return null;
			}
		}
		NKCPopupPointExchange.Instance.Open();
		NKCPopupPointExchange.Instance.PlayMusic();
		while (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			yield return null;
		}
		Close();
	}

	public override void CloseInternal()
	{
		m_animator.Rebind();
		base.gameObject.SetActive(value: false);
	}
}
