using System;
using UnityEngine;

namespace NKC.FX;

[Obsolete("4.7a 이후로 사용하지 않습니다. 4.7c에 삭제 예정")]
public class NKC_FX_ROTATE : MonoBehaviour
{
	public GameObject[] m_FX_Rotate360;

	public GameObject[] m_FX_Rotate20;

	private Vector3 m_Vec3Temp;

	private void Start()
	{
		for (int i = 0; i < m_FX_Rotate360.Length; i++)
		{
			m_Vec3Temp.Set(0f, 0f, UnityEngine.Random.Range(0, 360));
			m_FX_Rotate360[i].transform.eulerAngles = m_Vec3Temp;
		}
		for (int j = 0; j < m_FX_Rotate20.Length; j++)
		{
			m_Vec3Temp.Set(0f, 0f, m_FX_Rotate20[j].transform.eulerAngles.z);
			m_Vec3Temp.z += UnityEngine.Random.Range(-20, 20);
			m_FX_Rotate20[j].transform.eulerAngles = m_Vec3Temp;
		}
	}
}
