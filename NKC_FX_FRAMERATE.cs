using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NKC_FX_FRAMERATE : MonoBehaviour
{
	public Text FPS;

	private float m_fFPSTime;

	private StringBuilder m_FPSText = new StringBuilder();

	private void Update()
	{
		m_fFPSTime += (Time.deltaTime - m_fFPSTime) * 0.1f;
		if (Time.timeScale > 0f)
		{
			FPSView(m_fFPSTime);
		}
	}

	private void FPSView(float _time)
	{
		float num = _time * 1000f;
		float num2 = 1f / _time;
		m_FPSText.Remove(0, m_FPSText.Length);
		m_FPSText.AppendFormat("{0:0.0} ms ({1:0.} fps)", num, num2);
		FPS.text = m_FPSText.ToString();
	}
}
