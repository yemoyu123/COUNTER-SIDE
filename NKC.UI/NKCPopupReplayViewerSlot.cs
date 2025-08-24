using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupReplayViewerSlot : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	public delegate void OnClickSlot(DirectoryInfo dirInfo, FileInfo fileInfo);

	public Text fileNameText;

	public Text fileDateText;

	public Image slotImage;

	public Color idleColor;

	public Color cursorOverColor;

	private OnClickSlot dOnClickSlot;

	private NKCAssetInstanceData m_InstanceData;

	private DirectoryInfo dirInfo;

	private FileInfo fileInfo;

	private float clickTime;

	private void Init()
	{
	}

	public static NKCPopupReplayViewerSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_REPLAY_VIEWER_SLOT");
		NKCPopupReplayViewerSlot nKCPopupReplayViewerSlot = nKCAssetInstanceData?.m_Instant.GetComponent<NKCPopupReplayViewerSlot>();
		if (nKCPopupReplayViewerSlot == null)
		{
			NKCAssetResourceManager.CloseInstance(nKCAssetInstanceData);
			Debug.LogError("NKCPopupReplayViewerSlot Prefab null!");
			return null;
		}
		nKCPopupReplayViewerSlot.m_InstanceData = nKCAssetInstanceData;
		nKCPopupReplayViewerSlot.Init();
		if (parent != null)
		{
			nKCPopupReplayViewerSlot.transform.SetParent(parent);
		}
		nKCPopupReplayViewerSlot.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		nKCPopupReplayViewerSlot.gameObject.SetActive(value: false);
		return nKCPopupReplayViewerSlot;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		Object.Destroy(base.gameObject);
	}

	public void SetData(DirectoryInfo _dirInfo, FileInfo _fileInfo, OnClickSlot _onClickSlot)
	{
		base.gameObject.SetActive(value: true);
		dirInfo = _dirInfo;
		fileInfo = _fileInfo;
		if (dirInfo != null)
		{
			NKCUtil.SetLabelText(fileNameText, "[" + dirInfo.Name + "]");
			NKCUtil.SetLabelText(fileDateText, $"{dirInfo.LastWriteTime.Year}-{dirInfo.LastWriteTime.Month:00}-{dirInfo.LastWriteTime.Day:00} {dirInfo.LastWriteTime.Hour:00}:{dirInfo.LastWriteTime.Minute:00}");
		}
		else if (fileInfo != null)
		{
			NKCUtil.SetLabelText(fileNameText, fileInfo.Name);
			NKCUtil.SetLabelText(fileDateText, $"{fileInfo.LastWriteTime.Year}-{fileInfo.LastWriteTime.Month:00}-{fileInfo.LastWriteTime.Day:00} {fileInfo.LastWriteTime.Hour:00}:{fileInfo.LastWriteTime.Minute:00}");
		}
		else if (_onClickSlot != null)
		{
			NKCUtil.SetLabelText(fileNameText, "...");
			NKCUtil.SetLabelText(fileDateText, "");
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
		slotImage.color = idleColor;
		dOnClickSlot = _onClickSlot;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.clickTime - clickTime < 0.5f && dOnClickSlot != null)
		{
			dOnClickSlot(dirInfo, fileInfo);
		}
		slotImage.color = cursorOverColor;
		clickTime = eventData.clickTime;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		slotImage.color = cursorOverColor;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		slotImage.color = idleColor;
	}
}
