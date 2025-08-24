using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UiParticles;

[RequireComponent(typeof(ParticleSystem))]
public class UiParticles : MaskableGraphic
{
	[SerializeField]
	[FormerlySerializedAs("m_ParticleSystem")]
	private ParticleSystem m_ParticleSystem;

	[FormerlySerializedAs("m_ParticleSystemRenderer")]
	private ParticleSystemRenderer m_ParticleSystemRenderer;

	[FormerlySerializedAs("m_RenderMode")]
	[SerializeField]
	[Tooltip("Render mode of particles")]
	private UiParticleRenderMode m_RenderMode;

	[FormerlySerializedAs("m_StretchedSpeedScale")]
	[SerializeField]
	[Tooltip("Speed Scale for streched billboards")]
	private float m_StretchedSpeedScale = 1f;

	[FormerlySerializedAs("m_StretchedLenghScale")]
	[SerializeField]
	[Tooltip("Speed Scale for streched billboards")]
	private float m_StretchedLenghScale = 1f;

	private int numParticlesAlive;

	private ParticleSystem.Particle[] m_Particles;

	private List<Vector4> customDataList = new List<Vector4>();

	private Vector2 bufferVector2 = new Vector2(0f, 0f);

	private Vector3 bufferVector3 = new Vector3(0f, 0f, 0f);

	private Vector3 bufferVelocity = new Vector3(0f, 0f, 0f);

	private Vector3 bufferRotation = new Vector3(0f, 0f, 0f);

	private Vector3 bufferSize = new Vector3(0f, 0f, 0f);

	private int numTilesX;

	private int numTilesY;

	private readonly float zeroNaN = 1E-12f;

	private ParticleSystem.TextureSheetAnimationModule textureAnimator;

	private ParticleSystem.MinMaxCurve frameOverTime;

	private ParticleSystem.MinMaxCurve startFrame;

	public ParticleSystem ParticleSystem
	{
		get
		{
			return m_ParticleSystem;
		}
		set
		{
			if (SetPropertyUtility.SetClass(ref m_ParticleSystem, value))
			{
				SetAllDirty();
			}
		}
	}

	public ParticleSystemRenderer ParticleSystemRenderer
	{
		get
		{
			return m_ParticleSystemRenderer;
		}
		set
		{
			if (SetPropertyUtility.SetClass(ref m_ParticleSystemRenderer, value))
			{
				SetAllDirty();
			}
		}
	}

	public override Texture mainTexture
	{
		get
		{
			if (material != null && material.mainTexture != null)
			{
				return material.mainTexture;
			}
			return Graphic.s_WhiteTexture;
		}
	}

	public UiParticleRenderMode RenderMode
	{
		get
		{
			return m_RenderMode;
		}
		set
		{
			if (SetPropertyUtility.SetStruct(ref m_RenderMode, value))
			{
				SetAllDirty();
			}
		}
	}

	protected override void Awake()
	{
		ParticleSystem component = GetComponent<ParticleSystem>();
		ParticleSystemRenderer component2 = GetComponent<ParticleSystemRenderer>();
		if (m_Material == null)
		{
			m_Material = component2.sharedMaterial;
		}
		if (component2.renderMode == ParticleSystemRenderMode.Stretch)
		{
			RenderMode = UiParticleRenderMode.StreachedBillboard;
		}
		ParticleSystem = component;
		ParticleSystemRenderer = component2;
		Initialize();
	}

	public void Initialize()
	{
		if (ParticleSystemRenderer != null && ParticleSystemRenderer.enabled)
		{
			ParticleSystemRenderer.enabled = false;
		}
		raycastTarget = false;
		textureAnimator = ParticleSystem.textureSheetAnimation;
		frameOverTime = textureAnimator.frameOverTime;
		startFrame = textureAnimator.startFrame;
		SetAllDirty();
	}

	public override void SetMaterialDirty()
	{
		base.SetMaterialDirty();
		if (ParticleSystemRenderer != null)
		{
			ParticleSystemRenderer.sharedMaterial = m_Material;
		}
	}

	protected override void OnPopulateMesh(VertexHelper toFill)
	{
		if (base.isActiveAndEnabled)
		{
			if (ParticleSystem == null)
			{
				base.OnPopulateMesh(toFill);
			}
			else
			{
				GenerateParticlesBillboards(toFill);
			}
		}
	}

	private void InitParticlesBuffer()
	{
		if (m_Particles == null || m_Particles.Length < ParticleSystem.main.maxParticles)
		{
			m_Particles = new ParticleSystem.Particle[ParticleSystem.main.maxParticles];
		}
	}

	private void GenerateParticlesBillboards(VertexHelper vh)
	{
		InitParticlesBuffer();
		numParticlesAlive = ParticleSystem.GetParticles(m_Particles);
		if (ParticleSystem.textureSheetAnimation.enabled)
		{
			textureAnimator = ParticleSystem.textureSheetAnimation;
			numTilesX = textureAnimator.numTilesX;
			numTilesY = textureAnimator.numTilesY;
			startFrame = textureAnimator.startFrame;
		}
		vh.Clear();
		ParticleSystem.GetCustomParticleData(customDataList, ParticleSystemCustomData.Custom1);
		for (int i = 0; i < numParticlesAlive; i++)
		{
			DrawParticleBillboard(m_Particles[i], vh, customDataList[i]);
		}
	}

	private void DrawParticleBillboard(ParticleSystem.Particle particle, VertexHelper vh, Vector4 customdataStream)
	{
		Vector3 vector = particle.position;
		Quaternion rotation = Quaternion.Euler(particle.rotation3D);
		if (ParticleSystem.main.simulationSpace == ParticleSystemSimulationSpace.World)
		{
			vector = base.rectTransform.InverseTransformPoint(vector);
		}
		float num = particle.startLifetime - particle.remainingLifetime;
		float num2 = num / particle.startLifetime + zeroNaN;
		Vector3 zero = Vector3.zero;
		zero = particle.GetCurrentSize3D(ParticleSystem);
		if (m_RenderMode == UiParticleRenderMode.StreachedBillboard)
		{
			GetStrechedBillboardsSizeAndRotation(particle, num2, ref zero, ref rotation);
		}
		bufferVector3.x = (0f - zero.x) * 0.5f;
		bufferVector3.y = zero.y * 0.5f;
		Vector3 vector2 = bufferVector3;
		bufferVector3.x = zero.x * 0.5f;
		bufferVector3.y = zero.y * 0.5f;
		Vector3 vector3 = bufferVector3;
		bufferVector3.x = zero.x * 0.5f;
		bufferVector3.y = (0f - zero.y) * 0.5f;
		Vector3 vector4 = bufferVector3;
		bufferVector3.x = (0f - zero.x) * 0.5f;
		bufferVector3.y = (0f - zero.y) * 0.5f;
		Vector3 vector5 = bufferVector3;
		vector2 = rotation * vector2 + vector;
		vector3 = rotation * vector3 + vector;
		vector4 = rotation * vector4 + vector;
		vector5 = rotation * vector5 + vector;
		Color32 currentColor = particle.GetCurrentColor(ParticleSystem);
		Color color = currentColor;
		if (Mathf.FloorToInt(num2) == 1)
		{
			color.a = 0f;
			return;
		}
		int currentVertCount = vh.currentVertCount;
		if (!ParticleSystem.textureSheetAnimation.enabled)
		{
			bufferVector2.x = 0f;
			bufferVector2.y = 0f;
			vh.AddVert(vector5, currentColor, bufferVector2, customdataStream, Vector3.zero, Vector4.zero);
			bufferVector2.x = 0f;
			bufferVector2.y = 1f;
			vh.AddVert(vector2, currentColor, bufferVector2, customdataStream, Vector3.zero, Vector4.zero);
			bufferVector2.x = 1f;
			bufferVector2.y = 1f;
			vh.AddVert(vector3, currentColor, bufferVector2, customdataStream, Vector3.zero, Vector4.zero);
			bufferVector2.x = 1f;
			bufferVector2.y = 0f;
			vh.AddVert(vector4, currentColor, bufferVector2, customdataStream, Vector3.zero, Vector4.zero);
		}
		else
		{
			float num3 = particle.startLifetime / (float)textureAnimator.cycleCount + zeroNaN;
			float time = num % num3 / num3;
			int num4 = numTilesY * numTilesX;
			float num5 = startFrame.constant;
			Random.InitState((int)particle.randomSeed);
			switch (startFrame.mode)
			{
			case ParticleSystemCurveMode.Constant:
				num5 = startFrame.constant;
				break;
			case ParticleSystemCurveMode.TwoConstants:
				num5 = Random.Range(startFrame.constantMin, startFrame.constantMax);
				break;
			}
			switch (frameOverTime.mode)
			{
			case ParticleSystemCurveMode.Constant:
				num5 += frameOverTime.constant;
				break;
			case ParticleSystemCurveMode.Curve:
				num5 += frameOverTime.Evaluate(time);
				break;
			case ParticleSystemCurveMode.TwoCurves:
				num5 += frameOverTime.Evaluate(time);
				break;
			case ParticleSystemCurveMode.TwoConstants:
				num5 = Random.Range(frameOverTime.constantMin, frameOverTime.constantMax);
				break;
			}
			num5 = Mathf.Repeat(num5, 1f);
			float num6 = 0f;
			switch (textureAnimator.animation)
			{
			case ParticleSystemAnimationType.WholeSheet:
				num6 = Mathf.Clamp(Mathf.Floor(num5 * (float)num4), 0f, num4);
				break;
			case ParticleSystemAnimationType.SingleRow:
			{
				num6 = Mathf.Clamp(Mathf.Floor(num5 * (float)numTilesX), 0f, numTilesX);
				int num7 = textureAnimator.rowIndex;
				if (textureAnimator.rowMode == ParticleSystemAnimationRowMode.Random)
				{
					num7 = Random.Range(0, numTilesY);
				}
				num6 += (float)(num7 * numTilesX);
				break;
			}
			}
			int num8 = 1;
			int num9 = 1;
			if (!numTilesX.Equals(0))
			{
				num8 = (int)num6 % numTilesX;
				num9 = (int)num6 / numTilesX;
			}
			float num10 = 1f / ((float)numTilesX + zeroNaN);
			float num11 = 1f / ((float)numTilesY + zeroNaN);
			num9 = numTilesY - 1 - num9;
			float num12 = (float)num8 * num10;
			float num13 = (float)num9 * num11;
			float x = num12 + num10;
			float y = num13 + num11;
			bufferVector2.x = num12;
			bufferVector2.y = num13;
			vh.AddVert(vector5, currentColor, bufferVector2, customdataStream, Vector3.zero, Vector4.zero);
			bufferVector2.x = num12;
			bufferVector2.y = y;
			vh.AddVert(vector2, currentColor, bufferVector2, customdataStream, Vector3.zero, Vector4.zero);
			bufferVector2.x = x;
			bufferVector2.y = y;
			vh.AddVert(vector3, currentColor, bufferVector2, customdataStream, Vector3.zero, Vector4.zero);
			bufferVector2.x = x;
			bufferVector2.y = num13;
			vh.AddVert(vector4, currentColor, bufferVector2, customdataStream, Vector3.zero, Vector4.zero);
		}
		vh.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
		vh.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
	}

	private void GetStrechedBillboardsSizeAndRotation(ParticleSystem.Particle particle, float timeAlive01, ref Vector3 size3D, ref Quaternion rotation)
	{
		if (ParticleSystem.velocityOverLifetime.enabled)
		{
			bufferVelocity.x = ParticleSystem.velocityOverLifetime.x.Evaluate(timeAlive01);
			bufferVelocity.y = ParticleSystem.velocityOverLifetime.y.Evaluate(timeAlive01);
			bufferVelocity.z = ParticleSystem.velocityOverLifetime.z.Evaluate(timeAlive01);
		}
		Vector3 vector = particle.velocity + bufferVelocity;
		float num = Vector3.Angle(vector, Vector3.up);
		int num2 = ((vector.x < 0f) ? 1 : (-1));
		bufferRotation.Set(0f, 0f, num * (float)num2);
		rotation = Quaternion.Euler(bufferRotation);
		size3D.y *= m_StretchedLenghScale;
		bufferSize.Set(0f, m_StretchedSpeedScale * vector.magnitude, 0f);
		size3D += bufferSize;
	}

	private void LateUpdate()
	{
		if (base.isActiveAndEnabled && ParticleSystem != null && ParticleSystem.isPlaying)
		{
			SetVerticesDirty();
		}
	}
}
