using UnityEngine;

namespace NKC.UI;

public class NKCSwordTrainingMonster : MonoBehaviour
{
	public int iId;

	public NKCASUIUnitIllust m_illust;

	public RectTransform rtPosition;

	public Rigidbody2D rig2D;

	public bool bSpawnLeft;

	public float fMoveSpeed;

	public int iHp;

	public bool IsBoss;

	public float m_fBackPosX = 200f;

	public float m_fBackPosY = 1000f;

	public bool IsDead => iHp <= 0;

	public void Init(bool bMidBoss)
	{
		NKCSwordTraningBackgroundMonster component = base.gameObject.GetComponent<NKCSwordTraningBackgroundMonster>();
		if (null != component)
		{
			Object.Destroy(component);
		}
		BoxCollider2D boxCollider2D = base.gameObject.GetComponent<BoxCollider2D>();
		if (null == boxCollider2D)
		{
			boxCollider2D = base.gameObject.AddComponent<BoxCollider2D>();
		}
		if (null != boxCollider2D)
		{
			boxCollider2D.enabled = true;
			boxCollider2D.isTrigger = true;
			boxCollider2D.size = (bMidBoss ? new Vector2(100f, 130f) : new Vector2(100f, 80f));
			boxCollider2D.offset = (bMidBoss ? new Vector2(50f, 50f) : new Vector2(0f, 70f));
		}
		rig2D = base.gameObject.GetComponent<Rigidbody2D>();
		if (null == rig2D)
		{
			rig2D = base.gameObject.AddComponent<Rigidbody2D>();
		}
		if (null != rig2D)
		{
			rig2D.gravityScale = 0f;
			rig2D.useAutoMass = false;
			rig2D.mass = 0f;
			rig2D.interpolation = RigidbodyInterpolation2D.Interpolate;
		}
	}

	public void SetData(int id, NKCASUIUnitIllust illust, RectTransform rt, bool bLeft, float speed, int hp, float backPosX, float backPosY)
	{
		iId = id;
		m_illust = illust;
		rtPosition = rt;
		bSpawnLeft = bLeft;
		fMoveSpeed = speed;
		iHp = hp;
		IsBoss = hp > 1;
		m_fBackPosX = backPosX;
		m_fBackPosY = backPosY;
		rig2D.isKinematic = false;
		rig2D.useAutoMass = false;
		Vector2 force = (bLeft ? (Vector2.right * speed) : (Vector2.left * speed));
		rig2D?.AddForce(force, ForceMode2D.Force);
		m_illust?.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_RUN, loop: true);
		m_illust?.SetTimeScale(1f);
	}

	public void DamageReceiver(int iDamage, float fKnockBackVal)
	{
		if (!IsDead)
		{
			iHp -= iDamage;
			if (iHp <= 0)
			{
				rig2D.velocity = Vector2.zero;
				rig2D.useAutoMass = true;
				rig2D.gravityScale = 50f;
				m_illust.SetAnimation("DEATH", loop: false);
				rig2D.AddForce(new Vector2(bSpawnLeft ? (0f - m_fBackPosX) : m_fBackPosX, m_fBackPosY), ForceMode2D.Impulse);
			}
			else if (bSpawnLeft)
			{
				rtPosition.position = new Vector3(rtPosition.position.x - fKnockBackVal, rtPosition.position.y, rtPosition.position.z);
			}
			else
			{
				rtPosition.position = new Vector3(rtPosition.position.x + fKnockBackVal, rtPosition.position.y, rtPosition.position.z);
			}
		}
	}

	public void Clear()
	{
		if (m_illust != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_illust);
		}
	}
}
