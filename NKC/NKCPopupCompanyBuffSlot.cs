using System;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCPopupCompanyBuffSlot : MonoBehaviour
{
	public delegate void OnExpire(bool bOpen);

	public Image m_imgIcon;

	public Text m_lbTitle;

	public Text m_lbDesc;

	public Text m_lbDay;

	public Text m_lbTime;

	private NKMCompanyBuffTemplet m_NKMCompanyBuffTemplet;

	private DateTime m_expireTime;

	private NKCAssetInstanceData m_InstanceData;

	private OnExpire dOnExpire;

	private float updateInterval = 1f;

	private float deltaTime;

	public static NKCPopupCompanyBuffSlot GetNewInstance(Transform parent)
	{
		NKCAssetInstanceData nKCAssetInstanceData = NKCAssetResourceManager.OpenInstance<GameObject>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX", "NKM_UI_POPUP_EVENTBUFF_SLOT");
		NKCPopupCompanyBuffSlot component = nKCAssetInstanceData.m_Instant.GetComponent<NKCPopupCompanyBuffSlot>();
		if (component == null)
		{
			Debug.LogError("NKCPopupEventBuffSlot Prefab null!");
			return null;
		}
		if (parent != null)
		{
			component.transform.SetParent(parent);
		}
		component.m_InstanceData = nKCAssetInstanceData;
		component.transform.localPosition = new Vector3(component.transform.localPosition.x, component.transform.localPosition.y, 0f);
		component.gameObject.SetActive(value: false);
		return component;
	}

	public void DestoryInstance()
	{
		NKCAssetResourceManager.CloseInstance(m_InstanceData);
		m_InstanceData = null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void SetData(NKMCompanyBuffData buff, OnExpire onExpire)
	{
		dOnExpire = onExpire;
		m_NKMCompanyBuffTemplet = NKMTempletContainer<NKMCompanyBuffTemplet>.Find(buff.Id);
		if (m_NKMCompanyBuffTemplet == null)
		{
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetLabelText(m_lbTitle, m_NKMCompanyBuffTemplet.GetBuffName());
		NKCUtil.SetLabelText(m_lbDesc, NKCUtil.GetCompanyBuffDesc(m_NKMCompanyBuffTemplet.m_CompanyBuffID));
		NKCUtil.SetImageSprite(m_imgIcon, NKCUtil.GetCompanyBuffIconSprite(buff));
		m_expireTime = new DateTime(buff.ExpireTicks);
		SetTimeLabel();
	}

	private void SetTimeLabel()
	{
		NKMCompanyBuffSource companyBuffSource = m_NKMCompanyBuffTemplet.m_CompanyBuffSource;
		if (companyBuffSource != NKMCompanyBuffSource.ON_TIME_EVENT && companyBuffSource == NKMCompanyBuffSource.LEVEL)
		{
			if (m_NKMCompanyBuffTemplet.m_AccountLevelMax > 0)
			{
				NKCUtil.SetGameobjectActive(m_lbDay, bValue: false);
				NKCUtil.SetLabelText(m_lbTime, string.Format(NKCUtilString.GE_STRING_COMPANY_BUFFTIME_ACCOUNTLEVEL, m_NKMCompanyBuffTemplet.m_AccountLevelMax));
				NKCUtil.SetLabelTextColor(m_lbTime, Color.white);
			}
			return;
		}
		TimeSpan timeLeft = NKCSynchronizedTime.GetTimeLeft(m_expireTime);
		if (timeLeft.TotalSeconds < 0.0)
		{
			dOnExpire?.Invoke(bOpen: false);
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_lbDay, timeLeft.Days > 0);
		if (timeLeft.Days > 0)
		{
			m_lbDay.text = string.Format(NKCUtilString.GetTimeString(m_expireTime), timeLeft.Days);
		}
		if (timeLeft.TotalHours >= 1.0)
		{
			m_lbTime.text = NKCUtilString.GetTimeSpanString(timeLeft.Subtract(new TimeSpan(timeLeft.Days, 0, 0, 0)));
			NKCUtil.SetLabelTextColor(m_lbTime, Color.white);
		}
		else
		{
			m_lbTime.text = NKCUtilString.GetTimeSpanStringMS(timeLeft.Subtract(new TimeSpan(timeLeft.Days, 0, 0, 0)));
			NKCUtil.SetLabelTextColor(m_lbTime, Color.red);
		}
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime > updateInterval)
		{
			deltaTime -= updateInterval;
			SetTimeLabel();
		}
	}
}
