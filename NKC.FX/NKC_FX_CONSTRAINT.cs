using NKC.UI;
using UnityEngine;
using UnityEngine.Animations;

namespace NKC.FX;

[ExecuteAlways]
public class NKC_FX_CONSTRAINT : MonoBehaviour
{
	public enum SourceType
	{
		Root,
		Parent,
		FrontCanvas,
		MidCanvas,
		MainCamera
	}

	public SourceType Source;

	public float Weight;

	private bool init;

	public ScaleConstraint scaleConstraint;

	private ConstraintSource constraintSource;

	private void OnDestroy()
	{
		if (scaleConstraint != null)
		{
			scaleConstraint = null;
		}
	}

	private void OnEnable()
	{
		Initialize();
		if (init)
		{
			SetConstraint();
		}
	}

	public void Initialize()
	{
		switch (Source)
		{
		case SourceType.Root:
			constraintSource.sourceTransform = base.transform.root;
			break;
		case SourceType.Parent:
			constraintSource.sourceTransform = base.transform.parent;
			break;
		case SourceType.FrontCanvas:
			constraintSource.sourceTransform = NKCUIManager.rectFrontCanvas;
			break;
		case SourceType.MidCanvas:
			constraintSource.sourceTransform = NKCUIManager.rectMidCanvas;
			break;
		case SourceType.MainCamera:
			constraintSource.sourceTransform = Camera.main.transform;
			break;
		default:
			constraintSource.sourceTransform = base.transform.root;
			break;
		}
		if (constraintSource.sourceTransform != null)
		{
			constraintSource.weight = Weight;
			init = true;
		}
		else
		{
			init = false;
			Debug.LogWarning("Can not found sourceTransform.", base.gameObject);
		}
	}

	private void SetConstraint()
	{
		if (scaleConstraint == null)
		{
			scaleConstraint = GetComponent<ScaleConstraint>();
		}
		if (scaleConstraint != null)
		{
			scaleConstraint.weight = 1f;
			if (scaleConstraint.sourceCount > 0)
			{
				scaleConstraint.SetSource(0, constraintSource);
			}
			else
			{
				scaleConstraint.AddSource(constraintSource);
			}
			if (!scaleConstraint.constraintActive)
			{
				scaleConstraint.constraintActive = true;
			}
		}
		else
		{
			Debug.LogWarning("Null Constraint.", base.gameObject);
		}
	}
}
