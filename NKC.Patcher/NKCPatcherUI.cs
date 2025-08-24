using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.Patcher;

public class NKCPatcherUI : MonoBehaviour
{
	[SerializeField]
	public Slider sdProgressBar;

	[SerializeField]
	private TMP_Text lbNoticeText;

	[SerializeField]
	private TMP_Text lbProgressText;

	[SerializeField]
	private TMP_Text lbVersionCode;

	[SerializeField]
	private TMP_Text lbAppVersion;

	[SerializeField]
	private TMP_Text lbProtocolVersion;

	[SerializeField]
	private Text lbCanDownloadBackground;

	[SerializeField]
	private GameObject m_touchToStart;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd÷\ufffd\ufffd\ufffd \ufffd\ufffd \ufffdɶ\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd")]
	public GameObject m_objFallbackBG;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffdἶ \ufffd\u02fb\ufffd \ufffd\ufffdư")]
	public NKCUIComToggle m_ctglIntegrityCheck;

	[Header("\ufffd\ufffd\ufffd \ufffd\ufffdġ")]
	public EventTrigger evtBackground;

	private bool m_bBGTouch;

	private float ProgressBarValue
	{
		get
		{
			if (!(sdProgressBar != null))
			{
				return 0f;
			}
			return sdProgressBar.value;
		}
		set
		{
			if (sdProgressBar != null)
			{
				sdProgressBar.value = value;
			}
		}
	}

	private void Awake()
	{
		NKCUtil.SetLabelText(lbAppVersion, NKCUtilString.GetAppVersionText());
		NKCUtil.SetLabelText(lbProtocolVersion, NKCUtilString.GetProtocolVersionText());
		if (PlayerPrefsContainer.GetBoolean("PatchIntegrityCheck"))
		{
			NKCUtil.SetGameobjectActive(m_ctglIntegrityCheck, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_ctglIntegrityCheck, bValue: true);
			m_ctglIntegrityCheck.OnValueChanged.RemoveAllListeners();
			m_ctglIntegrityCheck.OnValueChanged.AddListener(delegate(bool v)
			{
				PlayerPrefsContainer.Set("PatchIntegrityCheck", v);
			});
		}
		if (evtBackground != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				m_bBGTouch = true;
			});
			evtBackground.triggers.Add(entry);
		}
		NKCUtil.SetGameobjectActive(evtBackground, bValue: false);
	}

	private void Start()
	{
		RectTransform componentInChildren = base.gameObject.GetComponentInChildren<RectTransform>();
		Vector3 localPosition = componentInChildren.localPosition;
		localPosition.Set(0f, 0f, 0f);
		componentInChildren.localPosition = localPosition;
		NKCUtil.SetLabelText(lbVersionCode, NKCConnectionInfo.s_ServerType);
		NKCUtil.SetLabelText(lbProgressText, "");
	}

	public void SetIntegrityCheckProgress()
	{
		NKCPatchDownloader.Instance.onIntegrityCheckProgress = OnIntegrityCheckProgress;
		void OnIntegrityCheckProgress(int fileCount, int totalCount)
		{
			if (totalCount != 0)
			{
				float num = (float)fileCount / (float)totalCount;
				NKCUtil.SetLabelText(lbNoticeText, string.Format("{0} ({1:0.00%})", NKCStringTable.GetString("SI_DP_PATCHER_INTEGRITY_CHECK"), num));
			}
		}
	}

	public void SetActive(bool active)
	{
		NKCUtil.SetGameobjectActive(this, active);
	}

	public void SetProgressText(string str)
	{
		NKCUtil.SetLabelText(lbNoticeText, str);
	}

	public void Progress()
	{
		if (!(lbNoticeText == null))
		{
			NKCUtil.SetLabelText(lbNoticeText, lbNoticeText.text + ".");
		}
	}

	public void SetActiveBackGround(bool active)
	{
		NKCUtil.SetGameobjectActive(lbCanDownloadBackground, active);
	}

	public void Set_lbCanDownloadBackground(string str)
	{
		NKCUtil.SetLabelText(lbCanDownloadBackground, str);
	}

	public bool BackGroundTextIsNull()
	{
		return lbCanDownloadBackground == null;
	}

	public void SetForTouchWait()
	{
		NKCUtil.SetGameobjectActive(evtBackground, bValue: true);
		NKCUtil.SetGameobjectActive(sdProgressBar, bValue: false);
		NKCUtil.SetGameobjectActive(lbNoticeText, bValue: false);
		NKCUtil.SetGameobjectActive(lbProgressText, bValue: false);
	}

	public IEnumerator WaitForTouch()
	{
		float fTime = 0f;
		while (!m_bBGTouch)
		{
			lbCanDownloadBackground.color = new Color(1f, 1f, 1f, (Mathf.Cos(fTime * 3f) + 1f) * 0.5f);
			fTime += Time.unscaledDeltaTime;
			yield return null;
		}
	}

	public void OnFileDownloadProgressTotal(long currentByte, long maxByte)
	{
		float progressBarValue = (float)currentByte / (float)maxByte;
		ProgressBarValue = progressBarValue;
		NKCUtil.SetLabelText(lbProgressText, $"{ProgressBarValue:0.00%}");
	}

	public void OnInvalidPatcherVideoPlayer(bool active)
	{
		NKCUtil.SetGameobjectActive(m_objFallbackBG, active);
	}
}
