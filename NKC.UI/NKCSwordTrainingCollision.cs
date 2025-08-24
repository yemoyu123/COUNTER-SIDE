using UnityEngine;

namespace NKC.UI;

public class NKCSwordTrainingCollision : MonoBehaviour
{
	public delegate void OnCrashMonster(int id, bool bLeftAttack);

	private OnCrashMonster m_Crash;

	public void Init(OnCrashMonster callBack)
	{
		m_Crash = callBack;
	}

	public void OnTriggerEnter2D(Collider2D collision)
	{
		NKCSwordTrainingMonster component = collision.GetComponent<NKCSwordTrainingMonster>();
		if (null != component)
		{
			m_Crash?.Invoke(component.iId, component.bSpawnLeft);
		}
	}
}
