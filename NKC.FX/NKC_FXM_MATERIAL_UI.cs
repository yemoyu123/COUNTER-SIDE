using System;
using Cs.Math;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace NKC.FX;

public class NKC_FXM_MATERIAL_UI : NKC_FXM_EVALUATER
{
	[Serializable]
	public class NKC_FXM_PROPERTY
	{
		public bool Enable;

		public bool RandomValue;

		public PropertyMode Mode;

		public string PropertyName;

		public int PropertyIndex;

		public float ConstantX;

		public float ConstantY;

		public float ConstantZ;

		public float ConstantW;

		[HideInInspector]
		public float rndX;

		[HideInInspector]
		public float rndY;

		[HideInInspector]
		public float rndZ;

		[HideInInspector]
		public float rndW;

		public float TimeX;

		public float TimeY;

		public float TimeZ;

		public float TimeW;

		[HideInInspector]
		public float rndTimeX;

		[HideInInspector]
		public float rndTimeY;

		[HideInInspector]
		public float rndTimeZ;

		[HideInInspector]
		public float rndTimeW;

		public AnimationCurve CurveX;

		public AnimationCurve CurveY;

		public AnimationCurve CurveZ;

		public AnimationCurve CurveW;

		public Color Color;

		public Gradient Gradient;

		public Transform Transform;

		public Texture Texture;
	}

	public NKC_FXM_PROPERTY[] Properties;

	public MaskableGraphic Target;

	public bool SyncRenderer = true;

	public bool AdditiveCurve = true;

	public Material OriginMaterial;

	private Material drivenMaterial;

	private bool isHDR;

	private Shader shader;

	private int propertyIndex;

	private ShaderPropertyType propertyType;

	private int nameID;

	private float bufferFloat;

	private Color bufferColor;

	private Vector4 bufferVector;

	private void OnDisable()
	{
		if (init)
		{
			Target.material = OriginMaterial;
		}
	}

	private void Awake()
	{
		if (Target == null)
		{
			Target = base.gameObject.GetComponent<MaskableGraphic>();
		}
		if (Target != null)
		{
			if (Target.material != null)
			{
				if (OriginMaterial == null)
				{
					OriginMaterial = Target.material;
				}
				if (OriginMaterial != null && drivenMaterial == null)
				{
					MakeInstanceMaterial();
				}
			}
			else
			{
				Debug.LogWarning("Null Target material.");
			}
		}
		else
		{
			Debug.LogWarning("Can not found Image Component.");
		}
	}

	private void OnDestroy()
	{
		if (Properties != null)
		{
			Properties = null;
		}
		if (Target != null)
		{
			Target = null;
		}
		if (OriginMaterial != null)
		{
			OriginMaterial = null;
		}
		if (drivenMaterial != null)
		{
			drivenMaterial = null;
		}
	}

	public override void Init()
	{
		if (Target != null)
		{
			if (OriginMaterial != null)
			{
				if (drivenMaterial == null || OriginMaterial.shader != drivenMaterial.shader)
				{
					MakeInstanceMaterial();
				}
				if (drivenMaterial != null)
				{
					shader = drivenMaterial.shader;
					init = true;
				}
				else
				{
					init = false;
					Debug.LogError("Null drivenMaterial -> " + base.transform.name + " :: " + base.transform.root.name, this);
				}
			}
			else
			{
				init = false;
				Debug.LogError("Material is missing -> " + base.transform.name + " :: " + base.transform.root.name, this);
			}
		}
		else
		{
			init = false;
			Debug.LogError("Can not found Image Component. -> " + base.transform.name + " :: " + base.transform.root.name, this);
		}
	}

	public override void SetRandomValue(bool _resimulate)
	{
		if (_resimulate && RandomValue)
		{
			ExecuteProperty(_random: true, _reset: false);
		}
	}

	protected override void OnStart()
	{
		Target.material = drivenMaterial;
	}

	protected override void OnExecute(bool _render)
	{
		if (base.enabled || !(Target == null))
		{
			if (SyncRenderer)
			{
				Target.enabled = _render;
			}
			if (init)
			{
				ExecuteProperty(_random: false, !_render);
			}
		}
	}

	private void ExecuteProperty(bool _random, bool _reset)
	{
		if (Properties == null || Properties.Length == 0)
		{
			return;
		}
		for (int i = 0; i < Properties.Length; i++)
		{
			if (!Properties[i].Enable)
			{
				continue;
			}
			if (OriginMaterial.HasProperty(Properties[i].PropertyName))
			{
				nameID = Shader.PropertyToID(Properties[i].PropertyName);
				switch (Properties[i].Mode)
				{
				case PropertyMode.Constant:
					if (_random && Properties[i].RandomValue)
					{
						Properties[i].ConstantX = UnityEngine.Random.Range(Properties[i].rndX, Properties[i].rndY);
					}
					bufferFloat = Properties[i].ConstantX;
					if (!SetFloatProperty(Properties[i].PropertyName, bufferFloat) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.Curve:
					if (_random && Properties[i].RandomValue)
					{
						Properties[i].ConstantX = UnityEngine.Random.Range(Properties[i].rndX, Properties[i].rndY);
					}
					bufferFloat = Evaluate(Properties[i].CurveX) * Properties[i].ConstantX;
					if (!SetFloatProperty(Properties[i].PropertyName, bufferFloat) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.Vector:
					bufferVector.Set(Properties[i].ConstantX, Properties[i].ConstantY, Properties[i].ConstantZ, Properties[i].ConstantW);
					if (!SetVectorProperty(Properties[i].PropertyName, bufferVector) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.VectorXY:
					if (_random && Properties[i].RandomValue)
					{
						Properties[i].ConstantX = UnityEngine.Random.Range(Properties[i].rndX, Properties[i].rndY);
						Properties[i].ConstantY = UnityEngine.Random.Range(Properties[i].rndZ, Properties[i].rndW);
					}
					bufferVector.Set(Properties[i].ConstantX, Properties[i].ConstantY, drivenMaterial.GetVector(nameID).z, drivenMaterial.GetVector(nameID).w);
					if (!SetVectorProperty(Properties[i].PropertyName, bufferVector) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.VectorXYCurve:
					if (_random && Properties[i].RandomValue)
					{
						Properties[i].ConstantX = UnityEngine.Random.Range(Properties[i].rndX, Properties[i].rndY);
						Properties[i].ConstantY = UnityEngine.Random.Range(Properties[i].rndZ, Properties[i].rndW);
					}
					if (AdditiveCurve)
					{
						bufferVector.Set(drivenMaterial.GetVector(nameID).x + Evaluate(Properties[i].CurveX) * Properties[i].ConstantX, drivenMaterial.GetVector(nameID).y + Evaluate(Properties[i].CurveY) * Properties[i].ConstantY, drivenMaterial.GetVector(nameID).z, drivenMaterial.GetVector(nameID).w);
					}
					else
					{
						bufferVector.Set(Evaluate(Properties[i].CurveX) * Properties[i].ConstantX, Evaluate(Properties[i].CurveY) * Properties[i].ConstantY, drivenMaterial.GetVector(nameID).z, drivenMaterial.GetVector(nameID).w);
					}
					if (!SetVectorProperty(Properties[i].PropertyName, bufferVector) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.VectorZW:
					if (_random && Properties[i].RandomValue)
					{
						Properties[i].ConstantZ = UnityEngine.Random.Range(Properties[i].rndX, Properties[i].rndY);
						Properties[i].ConstantW = UnityEngine.Random.Range(Properties[i].rndZ, Properties[i].rndW);
					}
					bufferVector.Set(drivenMaterial.GetVector(nameID).x, drivenMaterial.GetVector(nameID).y, Properties[i].ConstantZ, Properties[i].ConstantW);
					if (!SetVectorProperty(Properties[i].PropertyName, bufferVector) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.VectorZWCurve:
					if (_random && Properties[i].RandomValue)
					{
						Properties[i].ConstantZ = UnityEngine.Random.Range(Properties[i].rndX, Properties[i].rndY);
						Properties[i].ConstantW = UnityEngine.Random.Range(Properties[i].rndZ, Properties[i].rndW);
					}
					if (AdditiveCurve)
					{
						bufferVector.Set(drivenMaterial.GetVector(nameID).x, drivenMaterial.GetVector(nameID).y, drivenMaterial.GetVector(nameID).z + Evaluate(Properties[i].CurveZ) * Properties[i].ConstantZ, drivenMaterial.GetVector(nameID).w + Evaluate(Properties[i].CurveW) * Properties[i].ConstantW);
					}
					else
					{
						bufferVector.Set(drivenMaterial.GetVector(nameID).x, drivenMaterial.GetVector(nameID).y, Evaluate(Properties[i].CurveZ) * Properties[i].ConstantZ, Evaluate(Properties[i].CurveW) * Properties[i].ConstantW);
					}
					if (!SetVectorProperty(Properties[i].PropertyName, bufferVector) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.SheetIndex:
					bufferVector = AnimateTextureSheet((int)Properties[i].ConstantX, (int)Properties[i].ConstantZ, (int)Properties[i].ConstantW);
					if (!SetVectorProperty(Properties[i].PropertyName, bufferVector) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.SheetAnimation:
					bufferVector = AnimateTextureSheet((int)Evaluate(Properties[i].CurveX), (int)Properties[i].ConstantZ, (int)Properties[i].ConstantW);
					if (!SetVectorProperty(Properties[i].PropertyName, bufferVector) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.TiledSheetAnimation:
					bufferVector = AnimateTextureSheet((int)Evaluate(Properties[i].CurveX), (int)Properties[i].ConstantZ, (int)Properties[i].ConstantW, Properties[i].ConstantX, Properties[i].ConstantY);
					if (!SetVectorProperty(Properties[i].PropertyName, bufferVector) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.Color:
					if (_random && Properties[i].RandomValue)
					{
						Properties[i].Color = Properties[i].Gradient.Evaluate(UnityEngine.Random.value);
					}
					bufferColor = Properties[i].Color;
					if (!SetColorProperty(Properties[i].PropertyName, bufferColor) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.Gradient:
					bufferColor = Evaluate(Properties[i].Gradient);
					if (!SetColorProperty(Properties[i].PropertyName, bufferColor) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.HSV:
					bufferColor = Color.HSVToRGB(Evaluate(Properties[i].CurveX), Evaluate(Properties[i].CurveY), Evaluate(Properties[i].CurveZ));
					if (!SetColorProperty(Properties[i].PropertyName, bufferColor) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.Deltatime:
					bufferFloat = deltaTime;
					if (!SetFloatProperty(Properties[i].PropertyName, bufferFloat) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.Position:
					if (Properties[i].Transform != null)
					{
						if (!SetVectorProperty(Properties[i].PropertyName, bufferVector) && Application.isPlaying)
						{
							Properties[i].Enable = false;
						}
						break;
					}
					if (!SetVectorProperty(Properties[i].PropertyName, Vector4.zero) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					Debug.LogWarning("Null Transform", base.gameObject);
					break;
				case PropertyMode.Rotation:
					if (Properties[i].Transform != null)
					{
						if (!SetVectorProperty(Properties[i].PropertyName, bufferVector) && Application.isPlaying)
						{
							Properties[i].Enable = false;
						}
						break;
					}
					if (!SetVectorProperty(Properties[i].PropertyName, Vector4.zero) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					Debug.LogWarning("Null Transform", base.gameObject);
					break;
				case PropertyMode.Scale:
					if (Properties[i].Transform != null)
					{
						if (!SetVectorProperty(Properties[i].PropertyName, bufferVector) && Application.isPlaying)
						{
							Properties[i].Enable = false;
						}
						break;
					}
					if (!SetVectorProperty(Properties[i].PropertyName, Vector4.zero) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					Debug.LogWarning("Null Transform", base.gameObject);
					break;
				case PropertyMode.Boolean:
					bufferFloat = Properties[i].ConstantX;
					if (!SetFloatProperty(Properties[i].PropertyName, bufferFloat) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.Slider:
					bufferFloat = Properties[i].ConstantX;
					if (!SetFloatProperty(Properties[i].PropertyName, bufferFloat) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.RGBAChannel:
					bufferFloat = Properties[i].ConstantX;
					if (!SetFloatProperty(Properties[i].PropertyName, bufferFloat) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.UVChannel:
					bufferFloat = Properties[i].ConstantX;
					if (!SetFloatProperty(Properties[i].PropertyName, bufferFloat) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.BlendMode:
					bufferFloat = Properties[i].ConstantX;
					if (!SetFloatProperty(Properties[i].PropertyName, bufferFloat) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.Texture:
					if (Properties[i].Texture == null)
					{
						Properties[i].Texture = Texture2D.whiteTexture;
					}
					if (!SetTextureProperty(Properties[i].PropertyName, Properties[i].Texture) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.TimeConstant:
					if (_random && Properties[i].RandomValue)
					{
						Properties[i].TimeX = UnityEngine.Random.Range(Properties[i].rndTimeX, Properties[i].rndTimeY);
					}
					if (!Properties[i].TimeX.IsNearlyZero())
					{
						Properties[i].ConstantX = AnimateTime(Properties[i].ConstantX, Properties[i].TimeX, 0.1f, 360);
					}
					if (_reset || Properties[i].TimeX.IsNearlyZero())
					{
						Properties[i].ConstantX = 0f;
					}
					bufferFloat = Properties[i].ConstantX;
					if (!SetFloatProperty(Properties[i].PropertyName, bufferFloat) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.TimeVector:
					if (!Properties[i].TimeX.IsNearlyZero())
					{
						Properties[i].ConstantX = AnimateTime(Properties[i].ConstantX, Properties[i].TimeX, 0.1f, 10);
					}
					if (_reset || Properties[i].TimeX.IsNearlyZero())
					{
						Properties[i].ConstantX = 0f;
					}
					if (!Properties[i].TimeY.IsNearlyZero())
					{
						Properties[i].ConstantY = AnimateTime(Properties[i].ConstantY, Properties[i].TimeY, 0.1f, 10);
					}
					if (_reset || Properties[i].TimeY.IsNearlyZero())
					{
						Properties[i].ConstantY = 0f;
					}
					if (!Properties[i].TimeZ.IsNearlyZero())
					{
						Properties[i].ConstantZ = AnimateTime(Properties[i].ConstantZ, Properties[i].TimeZ, 0.1f, 10);
					}
					if (_reset || Properties[i].TimeZ.IsNearlyZero())
					{
						Properties[i].ConstantZ = 0f;
					}
					if (!Properties[i].TimeW.IsNearlyZero())
					{
						Properties[i].ConstantW = AnimateTime(Properties[i].ConstantW, Properties[i].TimeW, 0.1f, 10);
					}
					if (_reset || Properties[i].TimeW.IsNearlyZero())
					{
						Properties[i].ConstantW = 0f;
					}
					bufferVector.Set(drivenMaterial.GetVector(nameID).x + Properties[i].ConstantX, drivenMaterial.GetVector(nameID).y + Properties[i].ConstantY, drivenMaterial.GetVector(nameID).z + Properties[i].ConstantZ, drivenMaterial.GetVector(nameID).w + Properties[i].ConstantW);
					if (!SetVectorProperty(Properties[i].PropertyName, bufferVector) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.TimeVectorXY:
					if (_random && Properties[i].RandomValue)
					{
						Properties[i].TimeX = UnityEngine.Random.Range(Properties[i].rndTimeX, Properties[i].rndTimeY);
						Properties[i].TimeY = UnityEngine.Random.Range(Properties[i].rndTimeZ, Properties[i].rndTimeW);
					}
					if (!Properties[i].TimeX.IsNearlyZero())
					{
						Properties[i].ConstantX = AnimateTime(Properties[i].ConstantX, Properties[i].TimeX, 0.1f, 10);
					}
					if (_reset || Properties[i].TimeX.IsNearlyZero())
					{
						Properties[i].ConstantX = 0f;
					}
					if (!Properties[i].TimeY.IsNearlyZero())
					{
						Properties[i].ConstantY = AnimateTime(Properties[i].ConstantY, Properties[i].TimeY, 0.1f, 10);
					}
					if (_reset || Properties[i].TimeY.IsNearlyZero())
					{
						Properties[i].ConstantY = 0f;
					}
					bufferVector.Set(drivenMaterial.GetVector(nameID).x + Properties[i].ConstantX, drivenMaterial.GetVector(nameID).y + Properties[i].ConstantY, drivenMaterial.GetVector(nameID).z, drivenMaterial.GetVector(nameID).w);
					if (!SetVectorProperty(Properties[i].PropertyName, bufferVector) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.TimeVectorZW:
					if (_random && Properties[i].RandomValue)
					{
						Properties[i].TimeZ = UnityEngine.Random.Range(Properties[i].rndTimeX, Properties[i].rndTimeY);
						Properties[i].TimeW = UnityEngine.Random.Range(Properties[i].rndTimeZ, Properties[i].rndTimeW);
					}
					if (!Properties[i].TimeZ.IsNearlyZero())
					{
						Properties[i].ConstantZ = AnimateTime(Properties[i].ConstantZ, Properties[i].TimeZ, 0.1f, 10);
					}
					if (_reset || Properties[i].TimeZ.IsNearlyZero())
					{
						Properties[i].ConstantZ = 0f;
					}
					if (!Properties[i].TimeW.IsNearlyZero())
					{
						Properties[i].ConstantW = AnimateTime(Properties[i].ConstantW, Properties[i].TimeW, 0.1f, 10);
					}
					if (_reset || Properties[i].TimeW.IsNearlyZero())
					{
						Properties[i].ConstantW = 0f;
					}
					bufferVector.Set(drivenMaterial.GetVector(nameID).x, drivenMaterial.GetVector(nameID).y, drivenMaterial.GetVector(nameID).z + Properties[i].ConstantZ, drivenMaterial.GetVector(nameID).w + Properties[i].ConstantW);
					if (!SetVectorProperty(Properties[i].PropertyName, bufferVector) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.TimeVectorXYCurve:
					if (_random && Properties[i].RandomValue)
					{
						Properties[i].TimeX = UnityEngine.Random.Range(Properties[i].rndX, Properties[i].rndY);
						Properties[i].TimeY = UnityEngine.Random.Range(Properties[i].rndZ, Properties[i].rndW);
					}
					if (!Properties[i].TimeX.IsNearlyZero())
					{
						Properties[i].ConstantX = AnimateTime(Properties[i].ConstantX, Evaluate(Properties[i].CurveX) * Properties[i].TimeX, 0.1f, 10);
					}
					if (_reset || Properties[i].TimeX.IsNearlyZero())
					{
						Properties[i].ConstantX = 0f;
					}
					if (!Properties[i].TimeY.IsNearlyZero())
					{
						Properties[i].ConstantY = AnimateTime(Properties[i].ConstantY, Evaluate(Properties[i].CurveY) * Properties[i].TimeY, 0.1f, 10);
					}
					if (_reset || Properties[i].TimeY.IsNearlyZero())
					{
						Properties[i].ConstantY = 0f;
					}
					bufferVector.Set(drivenMaterial.GetVector(nameID).x + Properties[i].ConstantX, drivenMaterial.GetVector(nameID).y + Properties[i].ConstantY, drivenMaterial.GetVector(nameID).z, drivenMaterial.GetVector(nameID).w);
					if (!SetVectorProperty(Properties[i].PropertyName, bufferVector) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				case PropertyMode.TimeVectorZWCurve:
					if (_random && Properties[i].RandomValue)
					{
						Properties[i].TimeZ = UnityEngine.Random.Range(Properties[i].rndTimeX, Properties[i].rndTimeY);
						Properties[i].TimeW = UnityEngine.Random.Range(Properties[i].rndTimeZ, Properties[i].rndTimeW);
					}
					if (!Properties[i].TimeZ.IsNearlyZero())
					{
						Properties[i].ConstantZ = AnimateTime(Properties[i].ConstantZ, Evaluate(Properties[i].CurveZ) * Properties[i].TimeZ, 0.1f, 10);
					}
					if (_reset || Properties[i].TimeZ.IsNearlyZero())
					{
						Properties[i].ConstantZ = 0f;
					}
					if (!Properties[i].TimeW.IsNearlyZero())
					{
						Properties[i].ConstantW = AnimateTime(Properties[i].ConstantW, Evaluate(Properties[i].CurveW) * Properties[i].TimeW, 0.1f, 10);
					}
					if (_reset || Properties[i].TimeW.IsNearlyZero())
					{
						Properties[i].ConstantW = 0f;
					}
					bufferVector.Set(drivenMaterial.GetVector(nameID).x, drivenMaterial.GetVector(nameID).y, drivenMaterial.GetVector(nameID).z + Properties[i].ConstantZ, drivenMaterial.GetVector(nameID).w + Properties[i].ConstantW);
					if (!SetVectorProperty(Properties[i].PropertyName, bufferVector) && Application.isPlaying)
					{
						Properties[i].Enable = false;
					}
					break;
				}
			}
			else
			{
				Debug.LogWarning("Property does not exist : " + Properties[i].PropertyName, this);
			}
		}
		if (_reset)
		{
			Target.material = OriginMaterial;
		}
	}

	private bool SetVectorProperty(string _name, Vector4 _value)
	{
		bool result = true;
		int num = shader.FindPropertyIndex(_name);
		propertyType = shader.GetPropertyType(num);
		if (propertyType == ShaderPropertyType.Vector)
		{
			drivenMaterial.SetVector(_name, _value);
		}
		else
		{
			result = false;
			Debug.LogWarning($"Vector 타입만 넣을 수 있습니다, ({_name} : {propertyType})", base.gameObject);
		}
		return result;
	}

	private bool SetFloatProperty(string _name, float _value)
	{
		bool result = true;
		int num = shader.FindPropertyIndex(_name);
		propertyType = shader.GetPropertyType(num);
		if (propertyType == ShaderPropertyType.Float || propertyType == ShaderPropertyType.Range)
		{
			drivenMaterial.SetFloat(_name, _value);
		}
		else
		{
			result = false;
			Debug.LogWarning($"Float 또는 Range 타입만 넣을 수 있습니다, ({_name} : {propertyType})", base.gameObject);
		}
		return result;
	}

	private bool SetTextureProperty(string _name, Texture _value)
	{
		bool result = true;
		int num = shader.FindPropertyIndex(_name);
		propertyType = shader.GetPropertyType(num);
		if (propertyType == ShaderPropertyType.Texture)
		{
			drivenMaterial.SetTexture(_name, _value);
		}
		else
		{
			result = false;
			Debug.LogWarning($"Texture 타입만 넣을 수 있습니다, ({_name} : {propertyType})", base.gameObject);
		}
		return result;
	}

	private bool SetColorProperty(string _name, Color _value)
	{
		bool result = true;
		int num = shader.FindPropertyIndex(_name);
		propertyType = shader.GetPropertyType(num);
		if (propertyType == ShaderPropertyType.Color)
		{
			if (isHDR)
			{
				drivenMaterial.SetVector(_name, _value);
			}
			else
			{
				drivenMaterial.SetColor(_name, _value);
			}
		}
		else
		{
			result = false;
			Debug.LogWarning($"Color 타입만 넣을 수 있습니다, ({_name} : {propertyType})", base.gameObject);
		}
		return result;
	}

	private void MakeInstanceMaterial()
	{
		if (OriginMaterial == null)
		{
			Debug.LogWarning("Null OriginMaterial, Can not instance", base.gameObject);
			return;
		}
		if (drivenMaterial == null)
		{
			drivenMaterial = new Material(OriginMaterial);
			drivenMaterial.name = OriginMaterial.name + "(" + drivenMaterial.GetInstanceID() + ")";
			return;
		}
		string text = OriginMaterial.shader.name;
		string value = drivenMaterial.shader.name;
		if (!text.Equals(value))
		{
			Material mat = drivenMaterial;
			drivenMaterial = new Material(OriginMaterial);
			drivenMaterial.name = OriginMaterial.name + "(" + drivenMaterial.GetInstanceID() + ")";
			drivenMaterial.CopyPropertiesFromMaterial(mat);
		}
	}

	private void ResetImage()
	{
		if (Target != null && OriginMaterial != null)
		{
			Target.material = OriginMaterial;
		}
	}

	private float AnimateTime(float _data, float _value, float _rate, int _limit)
	{
		float num = _data;
		num += deltaTime * _rate * _value;
		if (num > (float)_limit)
		{
			num -= (float)_limit;
		}
		else if (num < (float)(-_limit))
		{
			num += (float)_limit;
		}
		return num;
	}

	private Vector4 AnimateTextureSheet(int _count, int _scaleX, int _scaleY)
	{
		Vector4 result = default(Vector4);
		if (_scaleX != 0 && _scaleY != 0)
		{
			result.x = 1f / (float)_scaleX;
			result.y = 1f / (float)_scaleY;
			int num = _count / _scaleX;
			result.z = (float)_count / (float)_scaleX - (float)num;
			result.w = 1f - 1f / (float)_scaleY - (float)num / (float)_scaleY;
		}
		return result;
	}

	private Vector4 AnimateTextureSheet(int _count, int _scaleX, int _scaleY, float _tileX, float _tileY)
	{
		Vector4 result = default(Vector4);
		if (_scaleX != 0 && _scaleY != 0)
		{
			result.x = 1f / (float)_scaleX;
			result.y = 1f / (float)_scaleY;
			int num = _count / _scaleX;
			result.z = (float)_count / (float)_scaleX - (float)num;
			result.w = 1f - 1f / (float)_scaleY - (float)num / (float)_scaleY;
			result.x *= _tileX;
			result.y *= _tileY;
		}
		return result;
	}
}
