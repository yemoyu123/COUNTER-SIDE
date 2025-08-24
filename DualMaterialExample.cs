using NextGenSprites;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DualMaterialExample : MonoBehaviour
{
	public string MaterialName = "Awesome Dual Material";

	public Material FirstMaterial;

	public Material SecondMaterial;

	private DualMaterial _dualMat;

	private SpriteRenderer _sRenderer;

	private void Awake()
	{
		_sRenderer = GetComponent<SpriteRenderer>();
		_dualMat = new DualMaterial(FirstMaterial, SecondMaterial, MaterialName);
		_sRenderer.material = _dualMat.FusedMaterial;
	}

	public void SetMaterialLerp(float target)
	{
		_dualMat.Lerp = target;
	}
}
