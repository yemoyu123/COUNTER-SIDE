using System.Collections;
using NKC.Publisher;
using UnityEngine;

namespace NKC.Patcher;

public class NKCLogo : MonoBehaviour
{
	public GameObject m_NPUF_BSIDE_LOGO;

	public GameObject m_NPUF_GAMEBEANS_LOGO;

	public GameObject m_NPUF_ZLONG_LOGO;

	public GameObject m_NPUF_ZLONG_LOGO3;

	public GameObject m_NPUF_BSIDE_PC_GAME_GRADE;

	private float m_fLogoTime = 1.5f;

	public static bool s_bLogoPlayed;

	private bool isInit;

	public void Init()
	{
		if (!isInit)
		{
			InitUI();
			isInit = true;
		}
	}

	private void InitUI()
	{
		NKCUtil.SetGameobjectActive(m_NPUF_BSIDE_LOGO, bValue: false);
		NKCUtil.SetGameobjectActive(m_NPUF_GAMEBEANS_LOGO, bValue: false);
		NKCUtil.SetGameobjectActive(m_NPUF_ZLONG_LOGO, bValue: false);
		NKCUtil.SetGameobjectActive(m_NPUF_ZLONG_LOGO3, bValue: false);
		NKCUtil.SetGameobjectActive(m_NPUF_BSIDE_PC_GAME_GRADE, bValue: false);
	}

	public IEnumerator DisplayLogo()
	{
		if (s_bLogoPlayed)
		{
			yield break;
		}
		if (NKCDefineManager.DEFINE_IOS())
		{
			yield return new WaitForSeconds(1f);
		}
		switch (NKCPublisherModule.PublisherType)
		{
		case NKCPublisherModule.ePublisherType.SB_Gamebase:
		case NKCPublisherModule.ePublisherType.STEAM:
			yield return ProcessLogo(m_NPUF_BSIDE_LOGO);
			break;
		case NKCPublisherModule.ePublisherType.Zlong:
			if (NKCDefineManager.DEFINE_ZLONG_SEA())
			{
				yield return ProcessLogo(m_NPUF_ZLONG_LOGO);
			}
			else if (NKCDefineManager.DEFINE_ZLONG_CHN())
			{
				yield return ProcessLogo(m_NPUF_ZLONG_LOGO3);
			}
			else
			{
				yield return ProcessLogo(m_NPUF_GAMEBEANS_LOGO);
			}
			break;
		default:
			yield return ProcessLogo(m_NPUF_BSIDE_LOGO);
			break;
		}
		s_bLogoPlayed = true;
	}

	public IEnumerator DisplayGameGrade()
	{
		yield return ProcessLogo(m_NPUF_BSIDE_PC_GAME_GRADE, 3f);
	}

	private IEnumerator ProcessLogo(GameObject obj, float fSpecialLogoTime = -1f)
	{
		if (!(obj == null))
		{
			obj.SetActive(value: true);
			obj.transform.localPosition = Vector3.zero;
			if (fSpecialLogoTime != -1f)
			{
				yield return new WaitForSeconds(fSpecialLogoTime);
			}
			else
			{
				yield return new WaitForSeconds(m_fLogoTime);
			}
			obj.SetActive(value: false);
			if (NKCPublisherModule.PublisherType == NKCPublisherModule.ePublisherType.Zlong)
			{
				Object.Destroy(obj);
			}
		}
	}
}
