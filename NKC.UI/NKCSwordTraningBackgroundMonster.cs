using UnityEngine;

namespace NKC.UI;

internal class NKCSwordTraningBackgroundMonster : MonoBehaviour
{
	public NKCASUIUnitIllust m_illust;

	private RectTransform m_rtPosition;

	private bool m_bMoveLeft;

	private float m_fMoveSpeed;

	private bool m_IsDead;

	public Rigidbody2D rig2D;

	private RectTransform m_rtLeftLimit;

	private RectTransform m_rtRightLimit;

	public void Init()
	{
		rig2D = base.gameObject.GetComponent<Rigidbody2D>();
		if (null == rig2D)
		{
			rig2D = base.gameObject.AddComponent<Rigidbody2D>();
		}
		BoxCollider2D component = base.gameObject.GetComponent<BoxCollider2D>();
		if (null != component)
		{
			Object.Destroy(component);
		}
		NKCSwordTrainingMonster component2 = base.gameObject.GetComponent<NKCSwordTrainingMonster>();
		if (null != component2)
		{
			Object.Destroy(component2);
		}
		rig2D.gravityScale = 0f;
		rig2D.useAutoMass = false;
		rig2D.mass = 0f;
		rig2D.interpolation = RigidbodyInterpolation2D.Interpolate;
	}

	public void SetData(NKCASUIUnitIllust illust, RectTransform rt, bool moveLeft, float speed, RectTransform leftLimit, RectTransform rightLimit)
	{
		m_illust = illust;
		m_rtPosition = rt;
		m_bMoveLeft = moveLeft;
		m_fMoveSpeed = speed;
		m_rtLeftLimit = leftLimit;
		m_rtRightLimit = rightLimit;
		m_IsDead = false;
		m_rtPosition.localScale = Vector3.one;
		m_rtPosition.position = (moveLeft ? m_rtRightLimit.position : m_rtLeftLimit.position);
		m_illust?.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_RUN, loop: true);
		m_illust.SetTimeScale(1f);
		UpdateMove();
	}

	private void UpdateMove()
	{
		if (m_bMoveLeft)
		{
			m_rtPosition.SetPositionAndRotation(m_rtPosition.position, new Quaternion(0f, 180f, 0f, 0f));
		}
		else
		{
			m_rtPosition.SetPositionAndRotation(m_rtPosition.position, new Quaternion(0f, 0f, 0f, 0f));
		}
		Vector2 force = ((!m_bMoveLeft) ? (Vector2.right * m_fMoveSpeed) : (Vector2.left * m_fMoveSpeed));
		rig2D.AddForce(force, ForceMode2D.Force);
	}

	private void Update()
	{
		if (!m_IsDead)
		{
			if (m_bMoveLeft && m_rtPosition.position.x < m_rtLeftLimit.position.x)
			{
				MoveTurn();
			}
			else if (!m_bMoveLeft && m_rtPosition.position.x > m_rtRightLimit.position.x)
			{
				MoveTurn();
			}
		}
	}

	public void MoveTurn()
	{
		rig2D.velocity = Vector2.zero;
		m_bMoveLeft = !m_bMoveLeft;
		UpdateMove();
	}

	public void SetDead()
	{
		m_IsDead = true;
	}

	public void Clear()
	{
		if (m_illust != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_illust);
		}
	}
}
