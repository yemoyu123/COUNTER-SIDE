using UnityEngine;

namespace NKC.FX;

public class NKC_FX_PTC_SELECTED_SEED : MonoBehaviour
{
	public bool UseRandomSeed;

	public int[] m_Seeds;

	public ParticleSystem[] m_PS;

	private void OnDestroy()
	{
		if (m_PS != null)
		{
			m_PS = null;
		}
	}

	private void OnDisable()
	{
		ReSeed();
	}

	private void ReSeed()
	{
		if (UseRandomSeed)
		{
			if (m_PS.Length == 0)
			{
				return;
			}
			uint randomSeed = (uint)Random.Range(-2147483647, int.MaxValue);
			for (int i = 0; i < m_PS.Length; i++)
			{
				if (m_PS[i] != null && !m_PS[i].isPlaying)
				{
					m_PS[i].randomSeed = randomSeed;
				}
			}
		}
		else
		{
			if (m_Seeds.Length == 0)
			{
				return;
			}
			int num = 0;
			int maxExclusive = m_Seeds.Length;
			num = Random.Range(0, maxExclusive);
			if (m_PS.Length == 0)
			{
				return;
			}
			for (int j = 0; j < m_PS.Length; j++)
			{
				if (m_PS[j] != null && !m_PS[j].isPlaying)
				{
					m_PS[j].randomSeed = (uint)m_Seeds[num];
				}
			}
		}
	}
}
