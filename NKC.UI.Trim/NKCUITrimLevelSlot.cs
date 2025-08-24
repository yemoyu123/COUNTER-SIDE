using ClientPacket.Common;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Trim;

public class NKCUITrimLevelSlot : MonoBehaviour
{
	public delegate void OnClickSlot(int trimLevel, bool isLocked);

	public NKCUIComStateButton m_csbtnButton;

	public GameObject m_objNew;

	[Header("Normal")]
	public Text m_lbTrimLevel;

	public Text m_lbScore;

	[Header("Selected")]
	public Text m_lbTrimLevelSel;

	public Text m_lbScoreSel;

	[Header("Locked")]
	public Text m_lbTrimLevelLock;

	public Text m_lbScoreLock;

	private int m_trimId;

	private int m_trimLevel;

	private NKCAssetInstanceData m_InstanceData;

	private OnClickSlot m_dOnClickSlot;

	public static NKCUITrimLevelSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_TRIM", "AB_UI_TRIM_DUNGEON_TAB");
		NKCUITrimLevelSlot nKCUITrimLevelSlot = nKCAssetInstanceData?.m_Instant.GetComponent<NKCUITrimLevelSlot>();
		if (nKCUITrimLevelSlot == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCUITrimLevelSlot  Prefab null!");
			return null;
		}
		nKCUITrimLevelSlot.m_InstanceData = nKCAssetInstanceData;
		nKCUITrimLevelSlot.Init();
		if (parent != null)
		{
			nKCUITrimLevelSlot.transform.SetParent(parent);
		}
		nKCUITrimLevelSlot.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		nKCUITrimLevelSlot.gameObject.SetActive(value: false);
		return nKCUITrimLevelSlot;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	public void Init()
	{
		NKCUtil.SetButtonClickDelegate(m_csbtnButton, OnClickButton);
		m_csbtnButton.m_bGetCallbackWhileLocked = true;
	}

	public void SetData(int trimId, int trimLevel, int clearedLevel, OnClickSlot onClickSlot)
	{
		m_dOnClickSlot = onClickSlot;
		m_trimId = trimId;
		m_trimLevel = trimLevel;
		string msg = trimLevel.ToString();
		NKCUtil.SetLabelText(m_lbTrimLevel, msg);
		NKCUtil.SetLabelText(m_lbTrimLevelSel, msg);
		NKCUtil.SetLabelText(m_lbTrimLevelLock, msg);
		NKMTrimClearData nKMTrimClearData = NKCScenManager.CurrentUserData()?.TrimData?.GetTrimClearData(trimId, trimLevel);
		NKCUtil.SetGameobjectActive(m_objNew, trimLevel == clearedLevel + 1 && nKMTrimClearData == null);
	}

	public void SetSelectedState(int selectedLevel)
	{
		m_csbtnButton?.Select(m_trimLevel == selectedLevel);
	}

	public void SetLock(bool value)
	{
		m_csbtnButton?.SetLock(value);
	}

	private void OnClickButton()
	{
		if (m_csbtnButton.m_bLock)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCStringTable.GetString("SI_DP_TRIM_MAIN_NEXT_LEVEL_TEXT"), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (m_dOnClickSlot != null)
		{
			m_dOnClickSlot(m_trimLevel, m_csbtnButton.m_bLock);
		}
	}
}
