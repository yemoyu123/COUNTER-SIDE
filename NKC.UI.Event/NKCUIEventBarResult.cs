using System.Collections;
using NKM;
using NKM.Event;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Event;

public class NKCUIEventBarResult : MonoBehaviour
{
	public delegate void OnClose();

	public static NKCUIEventBarResult Instance;

	public Animator m_animator;

	public CanvasGroup m_canvasGroup;

	public GameObject m_objShake;

	public GameObject m_objStir;

	public NKCComSoundPlayer m_soundPlayer;

	public float m_fRewardPopupTimer;

	[Header("캐릭터 SD")]
	public int m_unitID;

	public float m_scale;

	public RectTransform m_shakeSDRoot;

	public RectTransform m_stirSDRoot;

	public string shakeAniName;

	public string stirAniName;

	[Header("스트레가 이벤트")]
	public Text m_lbTitle;

	public Text m_lbDesc;

	public Image m_CharacterImage;

	public Sprite[] m_CafeCharacter;

	public string[] m_TitleKey;

	public string[] m_DescKey;

	public OnClose m_onClose;

	private Coroutine m_coroutine;

	private NKCASUIUnitIllust m_NKCASUISpineIllust_Shake;

	private NKCASUIUnitIllust m_NKCASUISpineIllust_Stir;

	public static bool IsInstanceOpen
	{
		get
		{
			if (Instance != null)
			{
				return Instance.gameObject.activeSelf;
			}
			return false;
		}
	}

	public void Init()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		m_onClose = null;
	}

	public void Open(int rewardItemID)
	{
		if (rewardItemID == 0)
		{
			return;
		}
		NKMEventBarTemplet nKMEventBarTemplet = NKMEventBarTemplet.Find(rewardItemID);
		if (nKMEventBarTemplet == null)
		{
			return;
		}
		NKCUtil.SetGameobjectActive(m_objShake, nKMEventBarTemplet.Technique == ManufacturingTechnique.shake);
		NKCUtil.SetGameobjectActive(m_objStir, nKMEventBarTemplet.Technique == ManufacturingTechnique.stir);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_unitID);
		switch (nKMEventBarTemplet.Technique)
		{
		case ManufacturingTechnique.shake:
			if (m_NKCASUISpineIllust_Shake == null)
			{
				m_NKCASUISpineIllust_Shake = NKCResourceUtility.OpenSpineSD(unitTempletBase);
			}
			if (m_NKCASUISpineIllust_Shake != null)
			{
				m_NKCASUISpineIllust_Shake.SetParent(m_shakeSDRoot, worldPositionStays: false);
				RectTransform rectTransform2 = m_NKCASUISpineIllust_Shake.GetRectTransform();
				if (rectTransform2 != null)
				{
					rectTransform2.localScale = new Vector3(m_scale, m_scale, 1f);
				}
				m_NKCASUISpineIllust_Shake.SetAnimation(shakeAniName, loop: false);
			}
			break;
		case ManufacturingTechnique.stir:
			if (m_NKCASUISpineIllust_Stir == null)
			{
				m_NKCASUISpineIllust_Stir = NKCResourceUtility.OpenSpineSD(unitTempletBase);
			}
			if (m_NKCASUISpineIllust_Stir != null)
			{
				m_NKCASUISpineIllust_Stir.SetParent(m_stirSDRoot, worldPositionStays: false);
				RectTransform rectTransform = m_NKCASUISpineIllust_Stir.GetRectTransform();
				if (rectTransform != null)
				{
					rectTransform.localScale = new Vector3(m_scale, m_scale, 1f);
				}
				m_NKCASUISpineIllust_Stir.SetAnimation(stirAniName, loop: false);
			}
			break;
		}
		if (m_CafeCharacter != null)
		{
			int num = Random.Range(0, m_CafeCharacter.Length);
			NKCUtil.SetImageSprite(m_CharacterImage, m_CafeCharacter[num]);
			if (m_TitleKey != null && m_TitleKey.Length > num)
			{
				NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString(m_TitleKey[num]));
			}
			if (m_DescKey != null && m_DescKey.Length > num)
			{
				NKCUtil.SetLabelText(m_lbDesc, NKCStringTable.GetString(m_DescKey[num]));
			}
		}
		base.gameObject.SetActive(value: true);
		m_soundPlayer?.Play();
		if (m_coroutine != null)
		{
			StopCoroutine(m_coroutine);
			m_coroutine = null;
		}
		m_coroutine = StartCoroutine(IStartClose());
	}

	private IEnumerator IStartClose()
	{
		float timer = m_fRewardPopupTimer;
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			yield return null;
		}
		Close();
	}

	public void Close()
	{
		if (m_coroutine != null)
		{
			StopCoroutine(m_coroutine);
			m_coroutine = null;
		}
		if (m_onClose != null)
		{
			m_onClose();
			m_onClose = null;
		}
		if (m_NKCASUISpineIllust_Shake != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUISpineIllust_Shake);
			m_NKCASUISpineIllust_Shake = null;
		}
		if (m_NKCASUISpineIllust_Stir != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUISpineIllust_Stir);
			m_NKCASUISpineIllust_Stir = null;
		}
		base.gameObject.SetActive(value: false);
	}

	private void OnDestroy()
	{
		Instance = null;
		m_onClose = null;
		if (m_NKCASUISpineIllust_Shake != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUISpineIllust_Shake);
			m_NKCASUISpineIllust_Shake = null;
		}
		if (m_NKCASUISpineIllust_Stir != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUISpineIllust_Stir);
			m_NKCASUISpineIllust_Stir = null;
		}
	}
}
