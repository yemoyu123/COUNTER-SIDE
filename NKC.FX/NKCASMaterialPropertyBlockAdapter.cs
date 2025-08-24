using UnityEngine;

namespace NKC.FX;

[RequireComponent(typeof(Renderer))]
public class NKCASMaterialPropertyBlockAdapter : MonoBehaviour
{
	private Renderer m_Renderer;

	private MaterialPropertyBlock m_MPB;

	private bool m_bHasFxMPB;

	private Color m_NKCColor = Color.white;

	private Color m_FXColor = Color.white;

	public void Init(bool bApplyCleanMPB)
	{
		if (m_MPB == null)
		{
			m_MPB = new MaterialPropertyBlock();
		}
		m_NKCColor = Color.white;
		m_FXColor = Color.white;
		m_MPB.Clear();
		m_bHasFxMPB = false;
		if (m_Renderer == null)
		{
			TryGetComponent<Renderer>(out m_Renderer);
		}
		if (bApplyCleanMPB && m_Renderer != null)
		{
			m_Renderer.SetPropertyBlock(null);
		}
	}

	public void SetColorNKC(Color color)
	{
		m_NKCColor = color;
		SetMPB();
	}

	public void SetFxMaterialPropertyBlock(MaterialPropertyBlock mpb, bool bHasColor)
	{
		if (mpb == null)
		{
			m_MPB.Clear();
			m_FXColor = Color.white;
			m_bHasFxMPB = false;
		}
		else
		{
			m_bHasFxMPB = true;
			m_MPB = mpb;
			if (bHasColor)
			{
				m_FXColor = mpb.GetColor("_Color");
			}
			else
			{
				m_FXColor = Color.white;
			}
		}
		SetMPB();
	}

	private void SetMPB()
	{
		if (m_Renderer == null && !TryGetComponent<Renderer>(out m_Renderer))
		{
			return;
		}
		if (!m_bHasFxMPB)
		{
			if (m_NKCColor == Color.white)
			{
				m_Renderer.SetPropertyBlock(null);
				return;
			}
			m_MPB.SetColor("_Color", m_NKCColor);
			m_Renderer.SetPropertyBlock(m_MPB);
		}
		else
		{
			m_MPB.SetColor("_Color", m_NKCColor * m_FXColor);
			m_Renderer.SetPropertyBlock(m_MPB);
		}
	}
}
