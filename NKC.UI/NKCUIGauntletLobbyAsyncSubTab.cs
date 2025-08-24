using UnityEngine;

namespace NKC.UI;

public class NKCUIGauntletLobbyAsyncSubTab : MonoBehaviour
{
	public delegate void OnClickSubTab(int iKey);

	private const string ASSET_BUNDLE_PATH = "ab_ui_nkm_ui_gauntlet";

	private const string ASSET_BUNDLE_FILE_NAME = "NKM_UI_GAUNTLET_ASYNC_SUBTAB_BUTTON";

	public NKCUIComStateButton m_csbtnClick;

	public NKCComText[] m_strName;

	public GameObject m_objLock;

	private OnClickSubTab m_dCallBack;

	private NKCAssetInstanceData m_InstanceData;

	private int m_iTier;

	public Animator m_Ani;

	public int Tier => m_iTier;

	public static NKCUIGauntletLobbyAsyncSubTab GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_nkm_ui_gauntlet", "NKM_UI_GAUNTLET_ASYNC_SUBTAB_BUTTON");
		NKCUIGauntletLobbyAsyncSubTab nKCUIGauntletLobbyAsyncSubTab = nKCAssetInstanceData?.m_Instant.GetComponent<NKCUIGauntletLobbyAsyncSubTab>();
		if (nKCUIGauntletLobbyAsyncSubTab == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError(string.Format("{0} Prefab null!", "NKM_UI_GAUNTLET_ASYNC_SUBTAB_BUTTON"));
			return null;
		}
		nKCUIGauntletLobbyAsyncSubTab.m_InstanceData = nKCAssetInstanceData;
		nKCUIGauntletLobbyAsyncSubTab.Init();
		if (parent != null)
		{
			nKCUIGauntletLobbyAsyncSubTab.transform.SetParent(parent);
		}
		nKCUIGauntletLobbyAsyncSubTab.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		nKCUIGauntletLobbyAsyncSubTab.gameObject.SetActive(value: false);
		return nKCUIGauntletLobbyAsyncSubTab;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	public void Init()
	{
		NKCUtil.SetBindFunction(m_csbtnClick, OnClick);
	}

	public void SetData(int _iTier, OnClickSubTab _callBack, bool bSelected)
	{
		NKCComText[] strName = m_strName;
		for (int i = 0; i < strName.Length; i++)
		{
			NKCUtil.SetLabelText(strName[i], $"Tier {_iTier}");
		}
		m_iTier = _iTier;
		m_dCallBack = _callBack;
		m_Ani.enabled = false;
		NKCUtil.SetGameobjectActive(m_objLock, NKCScenManager.CurrentUserData().m_NpcData.MaxOpenedTier < m_iTier);
		OnSelect(bSelected);
	}

	public void OnClick()
	{
		if (m_objLock.activeSelf)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_ASYNC_NPC_BLOCK_DESC);
		}
		else
		{
			m_dCallBack?.Invoke(m_iTier);
		}
	}

	public void OnSelect(bool bSel)
	{
		if (!m_objLock.activeSelf)
		{
			m_csbtnClick.Select(bSel, bForce: true);
		}
	}

	public void OnActiveEffect()
	{
		m_Ani.enabled = true;
		m_Ani.SetTrigger("PLAY");
	}
}
