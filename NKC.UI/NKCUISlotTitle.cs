using Cs.Logging;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUISlotTitle : MonoBehaviour
{
	public delegate void OnClick(int titleId);

	public NKCUIComStateButton m_button;

	public Image m_imgTltle;

	public TMP_Text m_lbTitle;

	public GameObject m_objLock;

	public GameObject m_objTimeRoot;

	public Text m_lbTime;

	public GameObject m_objSelect;

	public GameObject m_objRedDot;

	public Transform m_FxRoot;

	[Header("\ufffdƿ\ufffd\ufffd\ufffd\ufffd\ufffd \ufffdӼ\ufffd")]
	public float m_outlineWidth;

	public float m_innerTextExpand;

	private int m_titleId;

	private OnClick m_dOnClick;

	private NKCAssetInstanceData m_NKCAssetInstanceData;

	private void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_button, OnClickSlot);
	}

	public static NKCUISlotTitle GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_INVEN_ICON", "AB_ICON_USERTITLE");
		NKCUISlotTitle component = nKCAssetInstanceData.m_Instant.GetComponent<NKCUISlotTitle>();
		if (component == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Log.Error("NKCUISlotTitle Prefab null!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUISlotTitle.cs", 49);
			return null;
		}
		component.m_NKCAssetInstanceData = nKCAssetInstanceData;
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.gameObject.SetActive(value: false);
		component.Init();
		return component;
	}

	public void SetData(int titleId, bool showEmpty, bool showLock, OnClick dOnClick = null, bool showTimeLeft = false, bool showEffect = true)
	{
		if (!NKMTitleTemplet.TitleOpenTagEnabled)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		m_titleId = titleId;
		m_dOnClick = dOnClick;
		m_button.enabled = m_dOnClick != null;
		NKCUtil.SetGameobjectActive(m_objLock, bValue: false);
		NKCUtil.SetGameobjectActive(m_objSelect, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objTimeRoot, bValue: false);
		NKCUtil.SetGameobjectActive(m_lbTitle, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgTltle, bValue: false);
		if (titleId == 0)
		{
			SetEmpty(showEmpty);
			return;
		}
		NKMTitleTemplet nKMTitleTemplet = NKMTitleTemplet.Find(titleId);
		if (nKMTitleTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_imgTltle, bValue: false);
			Log.Error($"TitleId: {titleId} not exist in LUA_TITLE_TEMPLET", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUISlotTitle.cs", 98);
			return;
		}
		if (!nKMTitleTemplet.EnableByTag)
		{
			SetEmpty(showEmpty);
			return;
		}
		NKCUtil.SetGameobjectActive(m_lbTitle, !nKMTitleTemplet.IsIMGTitle);
		if (!nKMTitleTemplet.IsIMGTitle)
		{
			NKCUtil.SetLabelText(m_lbTitle, nKMTitleTemplet.GetTitleName());
			SetTitleColor(nKMTitleTemplet.TitleColor);
			SetOutlineColor(nKMTitleTemplet.OutlineColor, m_outlineWidth);
			m_lbTitle.fontMaterial = new Material(m_lbTitle.fontMaterial);
			m_lbTitle.SetMaterialDirty();
		}
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("ab_inven_icon_usertitle", nKMTitleTemplet.TitleBG));
		NKCUtil.SetGameobjectActive(m_imgTltle, bValue: true);
		NKCUtil.SetImageSprite(m_imgTltle, orLoadAssetResource);
		DeleteEffect();
		if (showEffect && !string.IsNullOrEmpty(nKMTitleTemplet.EffectPrefab))
		{
			GameObject orLoadAssetResource2 = NKCResourceUtility.GetOrLoadAssetResource<GameObject>(new NKMAssetName("AB_INVEN_ICON_USERTITLE_FX", nKMTitleTemplet.EffectPrefab));
			if (orLoadAssetResource2 != null)
			{
				Object.Instantiate(orLoadAssetResource2, m_FxRoot);
			}
		}
		bool flag = NKMTitleTemplet.IsOwnedTitle(titleId);
		if (showLock)
		{
			NKCUtil.SetGameobjectActive(m_objLock, !flag);
		}
		if (showTimeLeft && flag)
		{
			NKCUtil.SetGameobjectActive(m_objTimeRoot, nKMTitleTemplet.IsIntervalLimited());
			if (m_objTimeRoot.gameObject.activeSelf)
			{
				string remainTime = nKMTitleTemplet.GetRemainTime(NKCScenManager.CurrentUserData());
				NKCUtil.SetLabelText(m_lbTime, remainTime);
			}
		}
	}

	public void SetSelected(bool value)
	{
		NKCUtil.SetGameobjectActive(m_objSelect, value);
	}

	private void SetEmpty(bool showEmpty)
	{
		NKCUtil.SetGameobjectActive(m_imgTltle, showEmpty);
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(NKMAssetName.ParseBundleName("ab_inven_icon_usertitle", "USERTITLE_EMPTY_R_DEFAULT"));
		NKCUtil.SetImageSprite(m_imgTltle, orLoadAssetResource);
		DeleteEffect();
	}

	private void SetTitleColor(string colorCode)
	{
		if (ColorUtility.TryParseHtmlString(colorCode, out var color))
		{
			NKCUtil.SetLabelTextColor(m_lbTitle, color);
		}
		else
		{
			NKCUtil.SetLabelTextColor(m_lbTitle, Color.white);
		}
	}

	private void SetOutlineColor(string colorCode, float outlineWidth)
	{
		if (!(m_lbTitle == null))
		{
			if (ColorUtility.TryParseHtmlString(colorCode, out var color))
			{
				m_lbTitle.outlineWidth = outlineWidth;
				m_lbTitle.outlineColor = color;
				m_lbTitle.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, Mathf.Clamp(m_innerTextExpand, 0f, 1f));
			}
			else
			{
				m_lbTitle.outlineWidth = 0f;
				m_lbTitle.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0f);
			}
		}
	}

	private void DeleteEffect()
	{
		int childCount = m_FxRoot.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Object.DestroyImmediate(m_FxRoot.GetChild(i).gameObject);
		}
	}

	private void OnClickSlot()
	{
		if (m_dOnClick != null)
		{
			m_dOnClick(m_titleId);
		}
	}

	private void OnDestroy()
	{
		NKCAssetResourceManager.CloseInstance(m_NKCAssetInstanceData);
	}
}
