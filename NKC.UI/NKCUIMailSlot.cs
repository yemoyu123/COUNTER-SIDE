using System;
using System.Collections.Generic;
using NKC.Publisher;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIMailSlot : MonoBehaviour
{
	public delegate void OnReceive(long index);

	public delegate void OnOpen(NKMPostData postData);

	public Image m_imgMailType;

	public Text m_lbTitle;

	public Text m_lbContent;

	public Text m_lbDate;

	public List<NKCUISlot> m_lstSlot;

	public GameObject m_objTimeLeft;

	public Text m_lbTimeLeft;

	public GameObject m_objTimeLeftShort;

	public Text m_lbTimeLeftShort;

	public NKCUIComButton m_cbtnReceive;

	public NKCUIComStateButton m_cbtnOpen;

	public Sprite m_spMailTypeNormal;

	public Sprite m_spMailTypeAnnouncement;

	public Sprite m_spMailTypeImportant;

	private NKMPostData m_PostData;

	private OnReceive dOnReceive;

	private OnOpen dOnOpen;

	public long Index { get; private set; }

	public void Init()
	{
		foreach (NKCUISlot item in m_lstSlot)
		{
			item.Init();
		}
		m_cbtnOpen.PointerClick.AddListener(OnBtnOpen);
	}

	public void SetData(NKMPostData postData, OnReceive onReceive, OnOpen onOpen)
	{
		m_PostData = postData;
		Index = postData.postIndex;
		dOnReceive = onReceive;
		dOnOpen = onOpen;
		SetIcon(postData);
		string origin = NKCStringTable.GetString(postData.title, bSkipErrorCheck: true);
		m_lbTitle.text = NKCPublisherModule.Localization.GetTranslationIfJson(origin);
		m_lbContent.text = NKCUtilString.GetFinalMailContents(postData.contents);
		m_lbContent.text = NKCUtil.LabelLongTextCut(m_lbContent);
		SetSlot(postData.items);
		m_lbDate.text = NKMTime.UTCtoLocal(postData.sendDate).ToString("yyyy-MM-dd");
		SetTimeLeft(postData);
	}

	private void SetIcon(NKMPostData postData)
	{
		if (postData.PostTemplet == null)
		{
			Debug.Log($"[NKCUIMailSlot] postTemplet is null. postId:{postData.postId}");
			return;
		}
		if (!postData.PostTemplet.AllowReceiveAll)
		{
			NKCUtil.SetImageSprite(m_imgMailType, m_spMailTypeImportant);
			return;
		}
		switch (postData.PostTemplet.PostType)
		{
		case NKM_POST_TYPE.NORMAL:
			m_imgMailType.sprite = m_spMailTypeNormal;
			break;
		case NKM_POST_TYPE.ANNOUNCEMENT:
			m_imgMailType.sprite = m_spMailTypeAnnouncement;
			break;
		default:
			m_imgMailType.sprite = m_spMailTypeNormal;
			Debug.Log("Undefined mail type");
			break;
		}
	}

	private void SetSlot(List<NKMRewardInfo> lstPostItem)
	{
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			NKCUISlot nKCUISlot = m_lstSlot[i];
			if (i < lstPostItem.Count)
			{
				NKMRewardInfo nKMRewardInfo = lstPostItem[i];
				bool flag = IsSlotVisible(nKMRewardInfo.rewardType, nKMRewardInfo.ID, nKMRewardInfo.Count);
				NKCUtil.SetGameobjectActive(nKCUISlot, flag);
				if (flag)
				{
					nKCUISlot.SetData(NKCUISlot.SlotData.MakePostItemData(nKMRewardInfo), bEnableLayoutElement: false);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(nKCUISlot, bValue: false);
			}
		}
	}

	private void SetTimeLeft(NKMPostData postData)
	{
		if (postData == null)
		{
			NKCUtil.SetGameobjectActive(m_objTimeLeft, bValue: false);
			NKCUtil.SetGameobjectActive(m_objTimeLeftShort, bValue: false);
			return;
		}
		if (postData.expirationDate >= NKMConst.Post.UnlimitedExpirationUtcDate)
		{
			NKCUtil.SetGameobjectActive(m_objTimeLeft, bValue: true);
			NKCUtil.SetGameobjectActive(m_objTimeLeftShort, bValue: false);
			m_lbTimeLeft.text = NKCUtilString.GET_STRING_TIME_NO_LIMIT;
			return;
		}
		TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(postData.expirationDate);
		if (timeLeft.TotalDays >= 1.0)
		{
			NKCUtil.SetGameobjectActive(m_objTimeLeft, bValue: true);
			NKCUtil.SetGameobjectActive(m_objTimeLeftShort, bValue: false);
			m_lbTimeLeft.text = string.Format(NKCUtilString.GET_STRING_TIME_DAY_HOUR_TWO_PARAM, timeLeft.Days, timeLeft.Hours);
		}
		else if (timeLeft.TotalHours >= 1.0)
		{
			NKCUtil.SetGameobjectActive(m_objTimeLeft, bValue: true);
			NKCUtil.SetGameobjectActive(m_objTimeLeftShort, bValue: false);
			m_lbTimeLeft.text = string.Format(NKCUtilString.GET_STRING_TIME_HOUR_MINUTE_TWO_PARAM, timeLeft.Hours, timeLeft.Minutes);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objTimeLeft, bValue: false);
			NKCUtil.SetGameobjectActive(m_objTimeLeftShort, bValue: true);
			m_lbTimeLeftShort.text = string.Format(NKCUtilString.GET_STRING_TIME_MINUTE_ONE_PARAM, timeLeft.Minutes);
		}
	}

	private void OnBtnOpen()
	{
		if (dOnOpen != null)
		{
			dOnOpen((m_PostData != null) ? m_PostData : null);
		}
	}

	public void UpdateTime()
	{
		if (m_PostData.expirationDate < NKMConst.Post.UnlimitedExpirationUtcDate)
		{
			SetTimeLeft(m_PostData);
		}
	}

	public void OnBtnReceive()
	{
		if (dOnReceive != null)
		{
			dOnReceive((m_PostData != null) ? m_PostData.postIndex : (-1));
		}
	}

	public static bool IsSlotVisible(NKM_REWARD_TYPE rewardType, int rewardID, int count)
	{
		switch (rewardType)
		{
		case NKM_REWARD_TYPE.RT_MISC:
			if (NKMItemManager.GetItemMiscTempletByID(rewardID) == null)
			{
				return false;
			}
			if (rewardID == 0 || count == 0)
			{
				return false;
			}
			break;
		case NKM_REWARD_TYPE.RT_EQUIP:
			if (rewardID == 0)
			{
				return false;
			}
			if (NKMItemManager.GetEquipTemplet(rewardID) == null)
			{
				return false;
			}
			break;
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_SHIP:
		case NKM_REWARD_TYPE.RT_OPERATOR:
			if (rewardID == 0)
			{
				return false;
			}
			if (NKMUnitManager.GetUnitTempletBase(rewardID) == null)
			{
				return false;
			}
			break;
		case NKM_REWARD_TYPE.RT_NONE:
			return false;
		default:
			if (count == 0)
			{
				return false;
			}
			break;
		}
		return true;
	}
}
