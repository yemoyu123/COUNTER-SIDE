using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.HUD;

public class NKCGameHudSummonIndicator : MonoBehaviour
{
	public GameObject m_objRoot;

	public RectTransform m_rtArrow;

	public Image m_imgUnit;

	public float m_fDelayAfterStartState = 0.5f;

	public float m_fPositionOffset = 250f;

	private NKCGameClient m_NKCGameClient;

	private NKCUnitClient m_NKCUnitClient;

	private Transform m_trLeft;

	private Transform m_trRight;

	private float m_fOnTime;

	public bool Idle
	{
		get
		{
			if (!(m_fOnTime < 0f))
			{
				return !base.gameObject.activeInHierarchy;
			}
			return true;
		}
	}

	public bool SetData(NKCUnitClient targetUnit, NKCGameClient gameClient, Transform trLeft, Transform trRight)
	{
		m_fOnTime = -1f;
		if (targetUnit == null || gameClient == null)
		{
			return false;
		}
		m_NKCGameClient = gameClient;
		m_NKCUnitClient = targetUnit;
		m_trLeft = trLeft;
		m_trRight = trRight;
		Sprite sprite = NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.INVEN_ICON, targetUnit.GetUnitData());
		if (sprite == null)
		{
			return false;
		}
		NKCUtil.SetImageSprite(m_imgUnit, sprite);
		m_fOnTime = m_fDelayAfterStartState;
		SetDirection();
		base.gameObject.SetActive(value: true);
		return true;
	}

	private void Update()
	{
		if (m_NKCUnitClient == null)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		if (m_NKCUnitClient.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_PLAY)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		if (m_NKCUnitClient.GetUnitStateNow() == null || m_NKCUnitClient.GetUnitStateNow().m_NKM_UNIT_STATE_TYPE != NKM_UNIT_STATE_TYPE.NUST_START)
		{
			m_fOnTime -= Time.deltaTime;
		}
		if (m_fOnTime < 0f)
		{
			base.gameObject.SetActive(value: false);
		}
		SetDirection();
	}

	private void SetDirection()
	{
		float num = NKCCamera.GetCameraSizeNow() * NKCCamera.GetCameraAspect();
		float num2 = NKCCamera.GetPosNowX() - num;
		float num3 = NKCCamera.GetPosNowX() + num;
		float posX = m_NKCUnitClient.GetUnitSyncData().m_PosX;
		if (posX < num2)
		{
			base.transform.SetParent(m_trLeft);
			m_rtArrow.localScale = new Vector3(-1f, 1f, 1f);
			NKCUtil.SetGameobjectActive(m_objRoot, bValue: true);
		}
		else if (posX > num3)
		{
			base.transform.SetParent(m_trRight);
			m_rtArrow.localScale = Vector3.one;
			NKCUtil.SetGameobjectActive(m_objRoot, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objRoot, bValue: false);
		}
	}
}
