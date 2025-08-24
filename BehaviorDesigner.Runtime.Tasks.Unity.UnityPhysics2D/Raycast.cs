using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityPhysics2D;

[TaskCategory("Unity/Physics2D")]
[TaskDescription("Casts a ray against all colliders in the scene. Returns success if a collider was hit.")]
public class Raycast : Action
{
	[Tooltip("Starts the ray at the GameObject's position. If null the originPosition will be used.")]
	public SharedGameObject originGameObject;

	[Tooltip("Starts the ray at the position. Only used if originGameObject is null.")]
	public SharedVector2 originPosition;

	[Tooltip("The direction of the ray")]
	public SharedVector2 direction;

	[Tooltip("The length of the ray. Set to -1 for infinity.")]
	public SharedFloat distance = -1f;

	[Tooltip("Selectively ignore colliders.")]
	public LayerMask layerMask = -1;

	[Tooltip("Cast the ray in world or local space. The direction is in world space if no GameObject is specified.")]
	public Space space = Space.Self;

	[SharedRequired]
	[Tooltip("Stores the hit object of the raycast.")]
	public SharedGameObject storeHitObject;

	[SharedRequired]
	[Tooltip("Stores the hit point of the raycast.")]
	public SharedVector2 storeHitPoint;

	[SharedRequired]
	[Tooltip("Stores the hit normal of the raycast.")]
	public SharedVector2 storeHitNormal;

	[SharedRequired]
	[Tooltip("Stores the hit distance of the raycast.")]
	public SharedFloat storeHitDistance;

	public override TaskStatus OnUpdate()
	{
		Vector2 vector = direction.Value;
		Vector2 origin;
		if (originGameObject.Value != null)
		{
			origin = originGameObject.Value.transform.position;
			if (space == Space.Self)
			{
				vector = originGameObject.Value.transform.TransformDirection(direction.Value);
			}
		}
		else
		{
			origin = originPosition.Value;
		}
		RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, vector, (distance.Value == -1f) ? float.PositiveInfinity : distance.Value, layerMask);
		if (raycastHit2D.collider != null)
		{
			storeHitObject.Value = raycastHit2D.collider.gameObject;
			storeHitPoint.Value = raycastHit2D.point;
			storeHitNormal.Value = raycastHit2D.normal;
			storeHitDistance.Value = raycastHit2D.distance;
			return TaskStatus.Success;
		}
		return TaskStatus.Failure;
	}

	public override void OnReset()
	{
		originGameObject = null;
		originPosition = Vector2.zero;
		direction = Vector2.zero;
		distance = -1f;
		layerMask = -1;
		space = Space.Self;
	}
}
