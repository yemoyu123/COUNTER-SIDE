using System;

namespace UnityEngine.UI.Extensions;

[ExecuteInEditMode]
[RequireComponent(typeof(CanvasRenderer), typeof(ParticleSystem))]
[AddComponentMenu("UI/Effects/Extensions/UIParticleSystem")]
public class UIParticleSystem : MaskableGraphic
{
	[Tooltip("Having this enabled run the system in LateUpdate rather than in Update making it faster but less precise (more clunky)")]
	public bool fixedTime = true;

	[Tooltip("Enables 3d rotation for the particles")]
	public bool use3dRotation;

	private Transform _transform;

	private ParticleSystem pSystem;

	private ParticleSystem.Particle[] particles;

	private UIVertex[] _quad = new UIVertex[4];

	private Vector4 imageUV = Vector4.zero;

	private ParticleSystem.TextureSheetAnimationModule textureSheetAnimation;

	private int textureSheetAnimationFrames;

	private Vector2 textureSheetAnimationFrameSize;

	private ParticleSystemRenderer pRenderer;

	private bool isInitialised;

	private Material currentMaterial;

	private Texture currentTexture;

	private ParticleSystem.MainModule mainModule;

	public override Texture mainTexture => currentTexture;

	protected bool Initialize()
	{
		if (_transform == null)
		{
			_transform = base.transform;
		}
		if (pSystem == null)
		{
			pSystem = GetComponent<ParticleSystem>();
			if (pSystem == null)
			{
				return false;
			}
			mainModule = pSystem.main;
			if (pSystem.main.maxParticles > 14000)
			{
				mainModule.maxParticles = 14000;
			}
			pRenderer = pSystem.GetComponent<ParticleSystemRenderer>();
			if (pRenderer != null)
			{
				pRenderer.enabled = false;
			}
			if (material == null)
			{
				Shader shaderInstance = ShaderLibrary.GetShaderInstance("UI Extensions/Particles/Additive");
				if ((bool)shaderInstance)
				{
					material = new Material(shaderInstance);
				}
			}
			currentMaterial = material;
			if ((bool)currentMaterial && currentMaterial.HasProperty("_MainTex"))
			{
				currentTexture = currentMaterial.mainTexture;
				if (currentTexture == null)
				{
					currentTexture = Texture2D.whiteTexture;
				}
			}
			material = currentMaterial;
			mainModule.scalingMode = ParticleSystemScalingMode.Hierarchy;
			particles = null;
		}
		if (particles == null)
		{
			particles = new ParticleSystem.Particle[pSystem.main.maxParticles];
		}
		imageUV = new Vector4(0f, 0f, 1f, 1f);
		textureSheetAnimation = pSystem.textureSheetAnimation;
		textureSheetAnimationFrames = 0;
		textureSheetAnimationFrameSize = Vector2.zero;
		if (textureSheetAnimation.enabled)
		{
			textureSheetAnimationFrames = textureSheetAnimation.numTilesX * textureSheetAnimation.numTilesY;
			textureSheetAnimationFrameSize = new Vector2(1f / (float)textureSheetAnimation.numTilesX, 1f / (float)textureSheetAnimation.numTilesY);
		}
		return true;
	}

	protected override void Awake()
	{
		base.Awake();
		if (!Initialize())
		{
			base.enabled = false;
		}
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (!isInitialised && !pSystem.main.playOnAwake)
		{
			pSystem.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmittingAndClear);
			isInitialised = true;
		}
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		Vector2 zero3 = Vector2.zero;
		int num = pSystem.GetParticles(particles);
		for (int i = 0; i < num; i++)
		{
			ParticleSystem.Particle particle = particles[i];
			Vector2 vector = ((mainModule.simulationSpace == ParticleSystemSimulationSpace.Local) ? particle.position : _transform.InverseTransformPoint(particle.position));
			float num2 = (0f - particle.rotation) * ((float)Math.PI / 180f);
			float f = num2 + (float)Math.PI / 2f;
			Color32 currentColor = particle.GetCurrentColor(pSystem);
			float num3 = particle.GetCurrentSize(pSystem) * 0.5f;
			if (mainModule.scalingMode == ParticleSystemScalingMode.Shape)
			{
				vector /= base.canvas.scaleFactor;
			}
			Vector4 vector2 = imageUV;
			if (textureSheetAnimation.enabled)
			{
				float num4 = 1f - particle.remainingLifetime / particle.startLifetime;
				if (textureSheetAnimation.frameOverTime.curveMin != null)
				{
					num4 = textureSheetAnimation.frameOverTime.curveMin.Evaluate(1f - particle.remainingLifetime / particle.startLifetime);
				}
				else if (textureSheetAnimation.frameOverTime.curve != null)
				{
					num4 = textureSheetAnimation.frameOverTime.curve.Evaluate(1f - particle.remainingLifetime / particle.startLifetime);
				}
				else if (textureSheetAnimation.frameOverTime.constant > 0f)
				{
					num4 = textureSheetAnimation.frameOverTime.constant - particle.remainingLifetime / particle.startLifetime;
				}
				num4 = Mathf.Repeat(num4 * (float)textureSheetAnimation.cycleCount, 1f);
				int num5 = 0;
				switch (textureSheetAnimation.animation)
				{
				case ParticleSystemAnimationType.WholeSheet:
					num5 = Mathf.FloorToInt(num4 * (float)textureSheetAnimationFrames);
					break;
				case ParticleSystemAnimationType.SingleRow:
				{
					num5 = Mathf.FloorToInt(num4 * (float)textureSheetAnimation.numTilesX);
					int num6 = textureSheetAnimation.rowIndex;
					if (textureSheetAnimation.rowMode == ParticleSystemAnimationRowMode.Random)
					{
						num6 = Mathf.Abs((int)particle.randomSeed % textureSheetAnimation.numTilesY);
					}
					num5 += num6 * textureSheetAnimation.numTilesX;
					break;
				}
				}
				num5 %= textureSheetAnimationFrames;
				vector2.x = (float)(num5 % textureSheetAnimation.numTilesX) * textureSheetAnimationFrameSize.x;
				vector2.y = 1f - (float)(num5 / textureSheetAnimation.numTilesX + 1) * textureSheetAnimationFrameSize.y;
				vector2.z = vector2.x + textureSheetAnimationFrameSize.x;
				vector2.w = vector2.y + textureSheetAnimationFrameSize.y;
			}
			zero.x = vector2.x;
			zero.y = vector2.y;
			_quad[0] = UIVertex.simpleVert;
			_quad[0].color = currentColor;
			_quad[0].uv0 = zero;
			zero.x = vector2.x;
			zero.y = vector2.w;
			_quad[1] = UIVertex.simpleVert;
			_quad[1].color = currentColor;
			_quad[1].uv0 = zero;
			zero.x = vector2.z;
			zero.y = vector2.w;
			_quad[2] = UIVertex.simpleVert;
			_quad[2].color = currentColor;
			_quad[2].uv0 = zero;
			zero.x = vector2.z;
			zero.y = vector2.y;
			_quad[3] = UIVertex.simpleVert;
			_quad[3].color = currentColor;
			_quad[3].uv0 = zero;
			if (num2 == 0f)
			{
				zero2.x = vector.x - num3;
				zero2.y = vector.y - num3;
				zero3.x = vector.x + num3;
				zero3.y = vector.y + num3;
				zero.x = zero2.x;
				zero.y = zero2.y;
				_quad[0].position = zero;
				zero.x = zero2.x;
				zero.y = zero3.y;
				_quad[1].position = zero;
				zero.x = zero3.x;
				zero.y = zero3.y;
				_quad[2].position = zero;
				zero.x = zero3.x;
				zero.y = zero2.y;
				_quad[3].position = zero;
			}
			else if (use3dRotation)
			{
				Vector3 vector3 = ((mainModule.simulationSpace == ParticleSystemSimulationSpace.Local) ? particle.position : _transform.InverseTransformPoint(particle.position));
				if (mainModule.scalingMode == ParticleSystemScalingMode.Shape)
				{
					vector /= base.canvas.scaleFactor;
				}
				Vector3[] array = new Vector3[4]
				{
					new Vector3(0f - num3, 0f - num3, 0f),
					new Vector3(0f - num3, num3, 0f),
					new Vector3(num3, num3, 0f),
					new Vector3(num3, 0f - num3, 0f)
				};
				Quaternion quaternion = Quaternion.Euler(particle.rotation3D);
				_quad[0].position = vector3 + quaternion * array[0];
				_quad[1].position = vector3 + quaternion * array[1];
				_quad[2].position = vector3 + quaternion * array[2];
				_quad[3].position = vector3 + quaternion * array[3];
			}
			else
			{
				Vector2 vector4 = new Vector2(Mathf.Cos(num2), Mathf.Sin(num2)) * num3;
				Vector2 vector5 = new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * num3;
				_quad[0].position = vector - vector4 - vector5;
				_quad[1].position = vector - vector4 + vector5;
				_quad[2].position = vector + vector4 + vector5;
				_quad[3].position = vector + vector4 - vector5;
			}
			vh.AddUIVertexQuad(_quad);
		}
	}

	private void Update()
	{
		if (!fixedTime && Application.isPlaying)
		{
			pSystem.Simulate(Time.unscaledDeltaTime, withChildren: false, restart: false, fixedTimeStep: true);
			SetAllDirty();
			if ((currentMaterial != null && currentTexture != currentMaterial.mainTexture) || (material != null && currentMaterial != null && material.shader != currentMaterial.shader))
			{
				pSystem = null;
				Initialize();
			}
		}
	}

	private void LateUpdate()
	{
		if (!Application.isPlaying)
		{
			SetAllDirty();
		}
		else if (fixedTime)
		{
			pSystem.Simulate(Time.unscaledDeltaTime, withChildren: false, restart: false, fixedTimeStep: true);
			SetAllDirty();
			if ((currentMaterial != null && currentTexture != currentMaterial.mainTexture) || (material != null && currentMaterial != null && material.shader != currentMaterial.shader))
			{
				pSystem = null;
				Initialize();
			}
		}
		if (!(material == currentMaterial))
		{
			pSystem = null;
			Initialize();
		}
	}

	protected override void OnDestroy()
	{
		currentMaterial = null;
		currentTexture = null;
	}

	public void StartParticleEmission()
	{
		pSystem.time = 0f;
		pSystem.Play();
	}

	public void StopParticleEmission()
	{
		pSystem.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	public void PauseParticleEmission()
	{
		pSystem.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmitting);
	}
}
