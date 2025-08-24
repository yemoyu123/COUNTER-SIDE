using UnityEngine;
using UnityEngine.Events;

namespace NKC.UI;

public class NKCSwordTrainingWeapon : MonoBehaviour
{
	public delegate void OnKill(bool bBoss);

	private UnityAction m_dAttackFailCallBack;

	private CircleCollider2D m_Collider;

	private OnKill m_OnKill;

	private int m_iDamage = 1;

	private float m_fKnockBackValue;

	private RectTransform m_rtLeftAttckSocket;

	private RectTransform m_rtRightAttckSocket;

	private Transform m_trWeaponFX;

	public bool m_IsAttacking;

	private bool bHit;

	private Transform m_trFx;

	public void Init(Transform weaponFX, OnKill callBack, UnityAction attFailCallBack, float radius, RectTransform leftSocket, RectTransform rightSocket)
	{
		m_dAttackFailCallBack = attFailCallBack;
		m_trWeaponFX = weaponFX;
		m_OnKill = callBack;
		m_Collider = base.gameObject.GetComponent<CircleCollider2D>();
		if (null == m_Collider)
		{
			m_Collider = base.gameObject.AddComponent<CircleCollider2D>();
		}
		m_rtLeftAttckSocket = leftSocket;
		m_rtRightAttckSocket = rightSocket;
		m_Collider.radius = radius;
		m_Collider.isTrigger = true;
		m_Collider.enabled = false;
		m_IsAttacking = false;
	}

	public void SetHitFX(Transform fx)
	{
		m_trFx = fx;
	}

	public void SetData(int damage, float knockBackVal)
	{
		m_iDamage = damage;
		m_fKnockBackValue = knockBackVal;
	}

	public void OnAttack(bool bActive, bool bLeftAttack)
	{
		m_IsAttacking = bActive;
		if (m_IsAttacking)
		{
			bHit = false;
			m_Collider.enabled = true;
			RectTransform component = m_trWeaponFX.GetComponent<RectTransform>();
			if (bLeftAttack)
			{
				m_trWeaponFX.SetParent(m_rtLeftAttckSocket, worldPositionStays: false);
				m_trWeaponFX.SetPositionAndRotation(m_rtLeftAttckSocket.position, new Quaternion(0f, 180f, 0f, 0f));
				if (null != component)
				{
					Vector2 anchoredPosition = component.anchoredPosition + new Vector2(40f, 0f);
					component.anchoredPosition = anchoredPosition;
				}
			}
			else
			{
				m_trWeaponFX.SetParent(m_rtRightAttckSocket, worldPositionStays: false);
				m_trWeaponFX.SetPositionAndRotation(m_rtRightAttckSocket.position, new Quaternion(0f, 0f, 0f, 0f));
				if (null != component)
				{
					Vector2 anchoredPosition2 = component.anchoredPosition + new Vector2(-40f, 0f);
					component.anchoredPosition = anchoredPosition2;
				}
			}
			return;
		}
		m_Collider.enabled = false;
		if (!bHit)
		{
			m_dAttackFailCallBack?.Invoke();
			return;
		}
		NKCUtil.SetGameobjectActive(m_trFx.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_trFx.gameObject, bValue: true);
		if (bLeftAttack)
		{
			m_trFx.SetParent(m_rtLeftAttckSocket, worldPositionStays: false);
		}
		else
		{
			m_trFx.SetParent(m_rtRightAttckSocket, worldPositionStays: false);
		}
	}

	public void OnTriggerEnter2D(Collider2D collision)
	{
		NKCSwordTrainingMonster component = collision.GetComponent<NKCSwordTrainingMonster>();
		if (null != component && !component.IsDead)
		{
			NKCSoundManager.PlaySound("FX_COMBAT_MONSTER_BEAST_VOICE_02", 1f, 0f, 0f);
			bHit = true;
			component.DamageReceiver(m_iDamage, m_fKnockBackValue);
			if (component.IsDead)
			{
				m_OnKill?.Invoke(component.IsBoss);
			}
		}
	}

	public void Deactive()
	{
		if (null != m_Collider)
		{
			m_Collider.enabled = false;
		}
	}
}
