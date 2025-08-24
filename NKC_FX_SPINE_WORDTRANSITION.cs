using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class NKC_FX_SPINE_WORDTRANSITION : MonoBehaviour
{
	public Material wordMat;

	public SkeletonGraphic uiGraphic;

	public string boneName;

	public Image wordImage;

	public float wordRot = -1f;

	private Skeleton _skeleton;

	private Bone targetBone;

	private float targetRot;

	private Material tempMat;

	public float rotStandard = 270f;

	private void Start()
	{
		_skeleton = uiGraphic.Skeleton;
		targetBone = _skeleton.FindBone(boneName);
		if (wordMat == null)
		{
			tempMat = wordImage.material;
			return;
		}
		tempMat = new Material(wordMat);
		wordImage.material = tempMat;
	}

	private void Update()
	{
		if (!(tempMat == null))
		{
			targetRot = targetBone.Rotation;
			if (targetRot < 360f && targetRot > rotStandard)
			{
				wordRot = -1f;
			}
			else
			{
				wordRot = targetRot;
			}
			tempMat.SetFloat("_subIntense", wordRot / rotStandard);
		}
	}
}
