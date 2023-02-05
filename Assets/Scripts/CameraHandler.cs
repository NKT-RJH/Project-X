using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : Singleton<CameraHandler>
{
	public Transform targetTransform;
	public Transform cameraTransform;
	public Transform cameraPivotTransform;
	private Vector3 cameraTransformPosition;
	private LayerMask ignoreLayers;
	private Vector3 cameraFollowVelocity = Vector3.zero;

	public float lookSpeed = 0.1f;
	public float followSpeed = 0.3f;
	public float pivotSpeed = 0.03f;

	private float targetPosition;
	private float defaultPosition;
	private float lookAngle;
	private float pivotAngle;
	public float minimumPivot = -35f;
	public float maximumPivot = 35f;

	public float cameraSphereRadius = 0.2f;
	public float cameraCollisionOffset = 0.2f;
	public float minimumCollisionOffset = 0.2f;

	protected override void Awake()
	{
		base.Awake();

		defaultPosition = cameraTransform.localPosition.z;
		ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
	}

	public void FollowTarget(float deltaTime)
	{
		Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, deltaTime / followSpeed);
		transform.position = targetPosition;

		HandleCameraCollisions(deltaTime);
	}

	public void HandleCameraRotation(float deltaTime, float mouseXInput, float mouseYInput)
	{
		lookAngle += mouseXInput * lookSpeed / deltaTime;
		pivotAngle -= mouseYInput * pivotSpeed / deltaTime;
		pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

		Vector3 rotation = Vector3.zero;
		rotation.y = lookAngle;
		Quaternion targetRotation = Quaternion.Euler(rotation);
		transform.rotation = targetRotation;

		rotation = Vector3.zero;
		rotation.x = pivotAngle;

		targetRotation = Quaternion.Euler(rotation);
		cameraPivotTransform.localRotation = targetRotation;
	}

	private void HandleCameraCollisions(float deltaTime)
	{
		targetPosition = defaultPosition;
		RaycastHit hit;
		Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
		direction.Normalize();

		if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
		{
			float distance = Vector3.Distance(cameraPivotTransform.position, hit.point);
			targetPosition = -(distance - cameraCollisionOffset);
		}

		if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
		{
			targetPosition = -minimumCollisionOffset;
		}

		cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, deltaTime / 0.2f);
		cameraTransform.localPosition = cameraTransformPosition;
	}
}