using NKC.Patcher;
using UnityEngine;

namespace NKC;

public class NKCPatcherIllustController : MonoBehaviour
{
	public NKCUIComStateButton m_buttonLeft;

	public NKCUIComStateButton m_buttonRight;

	public NKCUIComStateButton m_buttonToggleActive;

	public GameObject m_rootPatchIllustList;

	public GameObject[] m_patchIllustList;

	public int m_currentIndex;

	public int m_maxIllustCount;

	public AudioClip m_clickSound;

	public AudioSource m_audioSource;

	public AudioClip m_ambientBGM;

	private bool bActive;

	private AudioSource _audioSource
	{
		get
		{
			if (NKCPatchChecker.m_instance != null)
			{
				return NKCPatchChecker.m_instance.m_audioSource;
			}
			m_audioSource.enabled = true;
			return m_audioSource;
		}
	}

	private void Awake()
	{
		NKCUtil.SetButtonClickDelegate(m_buttonLeft, OnClickLeft);
		NKCUtil.SetButtonClickDelegate(m_buttonRight, OnClickRight);
		NKCUtil.SetButtonClickDelegate(m_buttonToggleActive, OnClickToggleActive);
		RefreshUI();
		if (m_patchIllustList != null)
		{
			m_maxIllustCount = m_patchIllustList.Length;
		}
	}

	public void RefreshUI()
	{
		NKCUtil.SetGameobjectActive(m_buttonLeft, bActive);
		NKCUtil.SetGameobjectActive(m_buttonRight, bActive);
		if (m_patchIllustList == null)
		{
			return;
		}
		for (int i = 0; i < m_patchIllustList.Length; i++)
		{
			if (i == m_currentIndex)
			{
				NKCUtil.SetGameobjectActive(m_patchIllustList[i], bValue: false);
				NKCUtil.SetGameobjectActive(m_patchIllustList[i], bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_patchIllustList[i], bValue: false);
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void OnClickLeft()
	{
		m_currentIndex--;
		if (m_currentIndex < 0)
		{
			m_currentIndex = m_maxIllustCount - 1;
		}
		PlayClick();
		RefreshUI();
	}

	public void OnClickRight()
	{
		m_currentIndex++;
		if (m_currentIndex >= m_maxIllustCount)
		{
			m_currentIndex = 0;
		}
		PlayClick();
		RefreshUI();
	}

	public void OnClickToggleActive()
	{
		bActive = !bActive;
		NKCUtil.SetGameobjectActive(m_rootPatchIllustList, bActive);
		NKCUtil.SetGameobjectActive(m_buttonLeft, bActive);
		NKCUtil.SetGameobjectActive(m_buttonRight, bActive);
		if (bActive)
		{
			RefreshUI();
		}
	}

	public void PlayClick()
	{
		if (!(_audioSource == null) && !(m_clickSound == null))
		{
			_audioSource.PlayOneShot(m_clickSound);
		}
	}
}
