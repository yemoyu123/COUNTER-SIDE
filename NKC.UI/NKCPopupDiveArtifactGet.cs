using System.Collections.Generic;
using NKC.FX;
using NKC.UI.Option;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupDiveArtifactGet : NKCUIBase
{
	public delegate void dOnCloseCallBack();

	public delegate void dOnEffectExplode();

	public delegate void dOnEffectDestSetting();

	public const string ASSET_BUNDLE_NAME = "AB_UI_NKM_UI_WORLD_MAP_DIVE";

	public const string UI_ASSET_NAME = "NKM_UI_DIVE_ARTIFACT_POPUP";

	public RectTransform m_rtContents;

	public List<NKCPopupDiveArtifactGetSlot> m_lstNKCPopupDiveArtifactGetSlot;

	public NKCUIComStateButton m_csbtnSkip;

	public Image m_imgReturnItemIcon;

	public Text m_lbReturnDesc;

	public Animator m_amtorPopup;

	public Transform m_trEffectDest;

	public NKC_FXM_EVENT m_NKC_FXM_EVENT_Explode;

	public NKC_FXM_EVENT m_NKC_FXM_EVENT_DestSetting;

	private bool m_bAuto;

	private float m_fElapsedTime;

	private bool m_bOpenSkipPopup;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private dOnEffectExplode m_dOnEffectExplode;

	private dOnEffectDestSetting m_dOnEffectDestSetting;

	private dOnCloseCallBack m_dOnCloseCallBack;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => "PopupDiveArtifactGet";

	public void InvalidAuto()
	{
		m_bAuto = false;
	}

	private void OnCloseCallBack()
	{
		if (m_dOnCloseCallBack != null)
		{
			m_dOnCloseCallBack();
		}
		m_dOnCloseCallBack = null;
	}

	public void PlayOutroAni(int index)
	{
		m_amtorPopup.Play("OUTRO_" + index);
		NKCSoundManager.PlaySound("FX_UI_TITLE_IN_TEST", 1f, 0f, 0f);
	}

	private void OnEffectExplode()
	{
		NKCSoundManager.PlaySound("FX_UI_ARTIFACT_GET_02", 1f, 0f, 0f);
		if (m_dOnEffectExplode != null)
		{
			m_dOnEffectExplode();
		}
	}

	private void OnEffectDestSetting()
	{
		NKCSoundManager.PlaySound("FX_UI_ARTIFACT_GET_01", 1f, 0f, 0f);
		if (m_dOnEffectDestSetting != null)
		{
			m_dOnEffectDestSetting();
		}
	}

	public void SetEffectDestPos(Vector3 pos)
	{
		m_trEffectDest.position = pos;
	}

	public void InitUI(dOnEffectExplode _dOnEffectExplode, dOnEffectDestSetting _dOnEffectDestSetting)
	{
		m_dOnEffectExplode = _dOnEffectExplode;
		m_dOnEffectDestSetting = _dOnEffectDestSetting;
		for (int i = 0; i < m_lstNKCPopupDiveArtifactGetSlot.Count; i++)
		{
			m_lstNKCPopupDiveArtifactGetSlot[i].InitUI(i);
		}
		m_csbtnSkip.PointerClick.RemoveAllListeners();
		m_csbtnSkip.PointerClick.AddListener(OnClickSkip);
		m_NKC_FXM_EVENT_Explode.Evt.AddListener(OnEffectExplode);
		m_NKC_FXM_EVENT_DestSetting.Evt.AddListener(OnEffectDestSetting);
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void Open(List<int> lstArtifact, bool bAuto, dOnCloseCallBack _dOnCloseCallBack = null)
	{
		if (lstArtifact == null || lstArtifact.Count == 0)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_DIVE_ARTIFACT_ALREADY_FULL);
			OnClickSkip_();
			return;
		}
		NKCUIGameOption.CheckInstanceAndClose();
		m_dOnCloseCallBack = _dOnCloseCallBack;
		if (m_rtContents != null)
		{
			m_rtContents.localPosition = new Vector3(0f, m_rtContents.localPosition.y, m_rtContents.localPosition.z);
		}
		bool flag = false;
		for (int i = 0; i < m_lstNKCPopupDiveArtifactGetSlot.Count; i++)
		{
			if (m_lstNKCPopupDiveArtifactGetSlot[i] == null)
			{
				continue;
			}
			if (i < lstArtifact.Count)
			{
				NKMDiveArtifactTemplet nKMDiveArtifactTemplet = NKMDiveArtifactTemplet.Find(lstArtifact[i]);
				m_lstNKCPopupDiveArtifactGetSlot[i].SetData(nKMDiveArtifactTemplet);
				if (nKMDiveArtifactTemplet != null)
				{
					flag |= nKMDiveArtifactTemplet.RewardId > 0;
				}
			}
			else
			{
				m_lstNKCPopupDiveArtifactGetSlot[i].SetData(null);
			}
		}
		NKCUtil.SetGameobjectActive(m_lbReturnDesc, flag);
		m_bAuto = bAuto;
		m_fElapsedTime = 0f;
		m_bOpenSkipPopup = false;
		if (m_NKCUIOpenAnimator != null)
		{
			m_NKCUIOpenAnimator.PlayOpenAni();
		}
		UIOpened();
	}

	private void Update()
	{
		if (!base.IsOpen)
		{
			return;
		}
		m_NKCUIOpenAnimator.Update();
		if (!m_bAuto)
		{
			return;
		}
		m_fElapsedTime += Time.deltaTime;
		if (!(m_fElapsedTime >= 3f / NKCClientConst.DiveAutoSpeed) || NKMPopUpBox.IsOpenedWaitBox() || m_bOpenSkipPopup)
		{
			return;
		}
		NKMDiveGameData diveGameData = NKCScenManager.CurrentUserData().m_DiveGameData;
		if (diveGameData != null)
		{
			if (diveGameData.Player.PlayerBase.ReservedArtifacts.Count > 0 && m_lstNKCPopupDiveArtifactGetSlot.Count > 0)
			{
				bool flag = true;
				for (int i = 0; i < diveGameData.Player.PlayerBase.ReservedArtifacts.Count; i++)
				{
					if (NKMDiveArtifactTemplet.Find(diveGameData.Player.PlayerBase.ReservedArtifacts[i]) == null)
					{
						flag = false;
						break;
					}
				}
				int num = -1;
				if (flag)
				{
					num = NKMRandom.Range(0, diveGameData.Player.PlayerBase.ReservedArtifacts.Count);
				}
				else
				{
					int num2 = 0;
					for (int j = 0; j < diveGameData.Player.PlayerBase.ReservedArtifacts.Count; j++)
					{
						NKMDiveArtifactTemplet.Find(diveGameData.Player.PlayerBase.ReservedArtifacts[j]);
					}
					num = num2;
				}
				if (num < m_lstNKCPopupDiveArtifactGetSlot.Count && num >= 0)
				{
					m_lstNKCPopupDiveArtifactGetSlot[num].OnClickSelect();
				}
			}
			else
			{
				OnClickSkip_();
			}
		}
		m_bAuto = false;
	}

	public void OnClickSkip()
	{
		m_bOpenSkipPopup = true;
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_DIVE_ARTIFACT_GET_SKIP_CHECK_REQ, OnClickSkip_, delegate
		{
			m_bOpenSkipPopup = false;
		});
	}

	public void OnClickSkip_()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_DIVE().GetDiveGame()
			.SetLastSelectedArtifactSlotIndex(-1);
		NKCPacketSender.Send_NKMPacket_DIVE_SELECT_ARTIFACT_REQ(0);
		m_bAuto = false;
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
		m_bAuto = false;
		OnCloseCallBack();
	}

	public override void OnBackButton()
	{
	}
}
