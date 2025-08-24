using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;

namespace NKC.FX;

[RequireComponent(typeof(SkeletonRenderer))]
public class NKC_FX_SPINE_RENDERTEXTURE : MonoBehaviour
{
	public Material quadMaterial;

	public Camera targetCamera;

	public int maxRenderTextureSize = 1024;

	[Range(0f, 2f)]
	public float addTextureSize;

	public bool followRender = true;

	protected SkeletonRenderer skeletonRenderer;

	protected MeshRenderer meshRenderer;

	protected MeshFilter meshFilter;

	protected List<Mesh> instanceMeshList;

	public GameObject quad;

	protected MeshRenderer quadMeshRenderer;

	protected MeshFilter quadMeshFilter;

	protected Mesh quadMesh;

	protected Vector3 worldCornerNoDistortion0;

	protected Vector3 worldCornerNoDistortion1;

	protected Vector3 worldCornerNoDistortion2;

	protected Vector3 worldCornerNoDistortion3;

	protected Vector2 uvCorner0;

	protected Vector2 uvCorner1;

	protected Vector2 uvCorner2;

	protected Vector2 uvCorner3;

	public RenderTexture renderTexture;

	private CommandBuffer commandBuffer;

	private MaterialPropertyBlock propertyBlock;

	private readonly List<Material> materials = new List<Material>();

	protected Vector2Int requiredRenderTextureSize;

	protected Vector2Int allocatedRenderTextureSize;

	private Vector2Int TEXTURE_SIZE;

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		meshFilter = GetComponent<MeshFilter>();
		skeletonRenderer = GetComponent<SkeletonRenderer>();
		if (targetCamera == null)
		{
			targetCamera = Camera.main;
		}
		commandBuffer = new CommandBuffer();
		propertyBlock = new MaterialPropertyBlock();
		TEXTURE_SIZE = new Vector2Int(maxRenderTextureSize, maxRenderTextureSize);
		if (quad == null)
		{
			CreateQuad();
		}
		else
		{
			quadMeshRenderer = quad.GetComponent<MeshRenderer>();
			quadMeshFilter = quad.GetComponent<MeshFilter>();
		}
		quadMesh = new Mesh();
		quadMesh.MarkDynamic();
		quadMesh.name = "RenderTexture Quad";
		quadMesh.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
		if (base.isActiveAndEnabled)
		{
			Debug.LogError("\ufffd\ufffd\ufffd\ufffdƮ \ufffd\u02f8\ufffd : \ufffd\ufffd\ufffd\ufffd \ufffdؽ\ufffd\ufffdİ\ufffd ó\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\u05bd\ufffd\ufffdϴ\ufffd");
		}
	}

	private void LateUpdate()
	{
		RenderOntoQuad(skeletonRenderer);
	}

	private void OnEnable()
	{
		if ((bool)quadMeshRenderer)
		{
			quadMeshRenderer.gameObject.SetActive(value: true);
			meshRenderer.forceRenderingOff = true;
		}
	}

	private void OnDisable()
	{
		meshRenderer.forceRenderingOff = false;
		if ((bool)renderTexture)
		{
			RenderTexture.ReleaseTemporary(renderTexture);
			allocatedRenderTextureSize = Vector2Int.one;
		}
		if ((bool)quadMeshRenderer)
		{
			quadMeshRenderer.gameObject.SetActive(value: false);
		}
	}

	public void CreateQuad()
	{
		if (quad != null)
		{
			UnityEngine.Object.DestroyImmediate(quad);
		}
		quad = new GameObject(base.name + " RenderTexture", typeof(MeshRenderer), typeof(MeshFilter));
		quad.transform.SetParent(base.transform.parent, worldPositionStays: false);
		quadMeshRenderer = quad.GetComponent<MeshRenderer>();
		quadMeshFilter = quad.GetComponent<MeshFilter>();
		if (quadMaterial != null)
		{
			quadMeshRenderer.material = new Material(quadMaterial);
		}
		else
		{
			quadMeshRenderer.material = new Material(Shader.Find("StudioBside/Custom/BlurShader"));
		}
	}

	private void RenderOntoQuad(SkeletonRenderer skeletonRenderer)
	{
		PrepareForMesh();
		RenderToRenderTexture();
		AssignAtQuad();
	}

	protected void PrepareForMesh()
	{
		Bounds bounds = meshFilter.sharedMesh.bounds;
		Vector3 vector = new Vector3(addTextureSize, addTextureSize, 0f);
		Vector3 position = bounds.min - vector;
		Vector3 position2 = bounds.max + vector;
		Vector3 position3 = new Vector3(position.x, position2.y, position.z);
		Vector3 position4 = new Vector3(position2.x, position.y, position2.z);
		Vector3 position5 = base.transform.TransformPoint(position);
		Vector3 position6 = base.transform.TransformPoint(position3);
		Vector3 position7 = base.transform.TransformPoint(position4);
		Vector3 position8 = base.transform.TransformPoint(position2);
		Vector3 vector2 = targetCamera.WorldToScreenPoint(position5);
		Vector3 vector3 = targetCamera.WorldToScreenPoint(position6);
		Vector3 vector4 = targetCamera.WorldToScreenPoint(position7);
		Vector3 vector5 = targetCamera.WorldToScreenPoint(position8);
		float z = (vector2.z + vector3.z + vector4.z + vector5.z) / 4f;
		vector2.z = (vector3.z = (vector4.z = (vector5.z = z)));
		worldCornerNoDistortion0 = targetCamera.ScreenToWorldPoint(vector2);
		worldCornerNoDistortion1 = targetCamera.ScreenToWorldPoint(vector3);
		worldCornerNoDistortion2 = targetCamera.ScreenToWorldPoint(vector4);
		worldCornerNoDistortion3 = targetCamera.ScreenToWorldPoint(vector5);
		Vector3 vector6 = Vector3.Min(vector2, Vector3.Min(vector3, Vector3.Min(vector4, vector5)));
		Vector3 vector7 = Vector3.Max(vector2, Vector3.Max(vector3, Vector3.Max(vector4, vector5)));
		vector6.x = Mathf.Floor(vector6.x);
		vector6.y = Mathf.Floor(vector6.y);
		vector7.x = Mathf.Ceil(vector7.x);
		vector7.y = Mathf.Ceil(vector7.y);
		uvCorner0 = InverseLerp(vector6, vector7, vector2);
		uvCorner1 = InverseLerp(vector6, vector7, vector3);
		uvCorner2 = InverseLerp(vector6, vector7, vector4);
		uvCorner3 = InverseLerp(vector6, vector7, vector5);
		requiredRenderTextureSize = new Vector2Int(Math.Min(maxRenderTextureSize, Math.Abs((int)vector7.x - (int)vector6.x)), Math.Min(maxRenderTextureSize, Math.Abs((int)vector7.y - (int)vector6.y)));
		PrepareRenderTexture();
		PrepareCommandBuffer(targetCamera, vector6, vector7);
	}

	protected Vector2 InverseLerp(Vector2 a, Vector2 b, Vector2 value)
	{
		return new Vector2((value.x - a.x) / (b.x - a.x), (value.y - a.y) / (b.y - a.y));
	}

	protected void PrepareRenderTexture()
	{
		if (TEXTURE_SIZE != allocatedRenderTextureSize)
		{
			if ((bool)renderTexture)
			{
				RenderTexture.ReleaseTemporary(renderTexture);
			}
			renderTexture = RenderTexture.GetTemporary(TEXTURE_SIZE.x, TEXTURE_SIZE.y);
			allocatedRenderTextureSize = TEXTURE_SIZE;
		}
		else if (!renderTexture)
		{
			renderTexture = RenderTexture.GetTemporary(TEXTURE_SIZE.x, TEXTURE_SIZE.y);
			allocatedRenderTextureSize = TEXTURE_SIZE;
		}
	}

	protected void PrepareCommandBuffer(Camera targetCamera, Vector3 screenSpaceMin, Vector3 screenSpaceMax)
	{
		commandBuffer.Clear();
		commandBuffer.SetRenderTarget(renderTexture);
		commandBuffer.ClearRenderTarget(clearDepth: true, clearColor: true, Color.clear);
		commandBuffer.SetProjectionMatrix(targetCamera.projectionMatrix);
		commandBuffer.SetViewMatrix(targetCamera.worldToCameraMatrix);
		Vector2 size = targetCamera.pixelRect.size;
		if ((float)maxRenderTextureSize < Mathf.Abs(screenSpaceMax.y - screenSpaceMin.y))
		{
			size.y *= (float)maxRenderTextureSize / Mathf.Abs(screenSpaceMax.y - screenSpaceMin.y);
		}
		if ((float)maxRenderTextureSize < Mathf.Abs(screenSpaceMax.x - screenSpaceMin.x))
		{
			size.x *= (float)maxRenderTextureSize / Mathf.Abs(screenSpaceMax.x - screenSpaceMin.x);
		}
		Vector3 vector = screenSpaceMin;
		if ((float)maxRenderTextureSize < Mathf.Abs(screenSpaceMax.y - screenSpaceMin.y))
		{
			vector.y *= (float)maxRenderTextureSize / Mathf.Abs(screenSpaceMax.y - screenSpaceMin.y);
		}
		if ((float)maxRenderTextureSize < Mathf.Abs(screenSpaceMax.x - screenSpaceMin.x))
		{
			vector.x *= (float)maxRenderTextureSize / Mathf.Abs(screenSpaceMax.x - screenSpaceMin.x);
		}
		commandBuffer.SetViewport(new Rect(-vector, size));
	}

	protected void RenderToRenderTexture()
	{
		meshRenderer.GetPropertyBlock(propertyBlock);
		meshRenderer.GetSharedMaterials(materials);
		for (int i = 0; i < materials.Count; i++)
		{
			commandBuffer.DrawMesh(meshFilter.sharedMesh, base.transform.localToWorldMatrix, materials[i], meshRenderer.subMeshStartIndex + i, -1, propertyBlock);
		}
		Graphics.ExecuteCommandBuffer(commandBuffer);
	}

	protected void AssignAtQuad()
	{
		Transform transform = quadMeshRenderer.transform;
		if (followRender)
		{
			transform.position = base.transform.position;
			transform.rotation = base.transform.rotation;
			transform.localScale = base.transform.localScale;
		}
		Vector3 vector = transform.InverseTransformPoint(worldCornerNoDistortion0);
		Vector3 vector2 = transform.InverseTransformPoint(worldCornerNoDistortion1);
		Vector3 vector3 = transform.InverseTransformPoint(worldCornerNoDistortion2);
		Vector3 vector4 = transform.InverseTransformPoint(worldCornerNoDistortion3);
		Vector3[] vertices = new Vector3[4] { vector, vector2, vector3, vector4 };
		quadMesh.vertices = vertices;
		int[] triangles = new int[6] { 0, 2, 1, 2, 3, 1 };
		quadMesh.triangles = triangles;
		Vector3[] normals = new Vector3[4]
		{
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward,
			-Vector3.forward
		};
		quadMesh.normals = normals;
		float num = (float)requiredRenderTextureSize.x / (float)allocatedRenderTextureSize.x;
		float num2 = (float)requiredRenderTextureSize.y / (float)allocatedRenderTextureSize.y;
		Vector2[] uv = new Vector2[4]
		{
			new Vector2(uvCorner0.x * num, uvCorner0.y * num2),
			new Vector2(uvCorner1.x * num, uvCorner1.y * num2),
			new Vector2(uvCorner2.x * num, uvCorner2.y * num2),
			new Vector2(uvCorner3.x * num, uvCorner3.y * num2)
		};
		quadMeshRenderer.material.SetFloat("_MAXU", num);
		quadMeshRenderer.material.SetFloat("_MAXV", num2);
		quadMesh.uv = uv;
		quadMeshFilter.mesh = quadMesh;
		quadMeshRenderer.material.mainTexture = renderTexture;
	}
}
