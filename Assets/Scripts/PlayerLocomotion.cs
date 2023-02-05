using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
	PlayerManager playerManager;

	Transform cameraObject;
	InputHandler inputHandler;
	public Vector3 moveDirection;

	[HideInInspector] public AnimatorHandler animatorHandler;

	public new Rigidbody rigidbody;
	public GameObject normalCamera;

	[Header("Ground & Air Detection Stats")]
	[SerializeField] private float groundDetectionRayStartPoint = 0.5f;
	[SerializeField] private float minimumDistanceNeededToBeginFall = 1f;
	[SerializeField] private float groundDirectionRayDistance = 0.2f;
	LayerMask ignoreForGroundCheck;
	public float inAirTimer;

	[Header("Movement Stats")]
	[SerializeField] private float walkingSpeed = 1;
	[SerializeField] private float movementSpeed = 5;
	[SerializeField] private float sprintSpeed = 7;
	[SerializeField] private float rotationSpeed = 10;
	[SerializeField] private float fallingSpeed = 45;

	private void Start()
	{
		playerManager = GetComponent<PlayerManager>();
		rigidbody = GetComponent<Rigidbody>();
		inputHandler = GetComponent<InputHandler>();
		animatorHandler = GetComponentInChildren<AnimatorHandler>();
		cameraObject = Camera.main.transform;
		animatorHandler.Intialize();

		playerManager.isGrounded = true;
		ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
	}

	#region Movement
	Vector3 normalVector;
	Vector3 targetPosition;

	private void HandleRotation(float deltaTime)
	{
		Vector3 targetDirection = Vector3.zero;
		float moveOverride = inputHandler.moveAmount;

		targetDirection = cameraObject.forward * inputHandler.vertical;
		targetDirection += cameraObject.right * inputHandler.horizontal;

		targetDirection.Normalize();
		targetDirection.y = 0;

		if (targetDirection == Vector3.zero)
		{
			targetDirection = transform.forward;
		}

		float rs = rotationSpeed;

		Quaternion tr = Quaternion.LookRotation(targetDirection);
		Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rs * deltaTime);

		transform.rotation = targetRotation;
	}

	public void HandleMovement(float deltaTime)
	{
		if (animatorHandler.animator.GetBool("IsInteracting")) return;

		if (playerManager.isInterecting) return;

		moveDirection = cameraObject.forward * inputHandler.vertical;
		moveDirection += cameraObject.right * inputHandler.horizontal;
		moveDirection.Normalize();
		moveDirection.y = 0;

		float speed = movementSpeed;

		if (inputHandler.sprintFlag)
		{
			speed = sprintSpeed;
			playerManager.isSprinting = true;
			moveDirection *= speed;
		}
		else
		{
			moveDirection *= speed;
		}
		
		Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
		print(projectedVelocity);
		rigidbody.velocity = projectedVelocity;

		animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);

		if (animatorHandler.canRotate)
		{
			HandleRotation(deltaTime);
		}
	}

	public void HandleRollingAndSprinting(float deltaTime)
	{
		if (animatorHandler.animator.GetBool("IsInteracting")) return;

		if (inputHandler.rollInput)
		{
			if (inputHandler.moveAmount > 0)
			{
				moveDirection = cameraObject.forward * inputHandler.vertical;
				moveDirection += cameraObject.right * inputHandler.horizontal;

				animatorHandler.PlayTargetAnimation("Dash", true);
				rigidbody.AddForce(transform.forward * 10);
				moveDirection.y = 0;
				Quaternion rollRoation = Quaternion.LookRotation(moveDirection);
				transform.rotation = rollRoation;
			}
		}
	}

	public void HandleFalling(float deltaTime, Vector3 moveDirection)
	{
		playerManager.isGrounded = false;
		RaycastHit hit;
		Vector3 origin = transform.position;
		origin.y += groundDetectionRayStartPoint;

		if (Physics.Raycast(origin, transform.forward, out hit, 0.4f))
		{
			moveDirection = Vector3.zero;
		}

		if (playerManager.isInAir)
		{
			rigidbody.AddForce(-Vector3.up * fallingSpeed);
			rigidbody.AddForce(moveDirection * fallingSpeed / 10f);
		}

		Vector3 dir = moveDirection;
		dir.Normalize();
		origin = origin + dir * groundDirectionRayDistance;

		targetPosition = transform.position;

		Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);

		if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall,ignoreForGroundCheck))
		{
			normalVector = hit.normal;
			Vector3 tp = hit.point;
			playerManager.isGrounded = true;
			targetPosition.y = tp.y;

			if (playerManager.isInAir)
			{
				if (inAirTimer > 0.5f)
				{
					Debug.Log("You were in the air for " + inAirTimer);
					animatorHandler.PlayTargetAnimation("Land", true);
				}
				else
				{
					animatorHandler.PlayTargetAnimation("Empty", false);
					inAirTimer = 0;
				}

				playerManager.isInAir = false;
			}
		}
		else
		{
			if (playerManager.isGrounded)
			{
				playerManager.isGrounded = false;
			}

			if (!playerManager.isInAir)
			{
				if (!playerManager.isInterecting)
				{
					animatorHandler.PlayTargetAnimation("Falling", true);
				}

				Vector3 vel = rigidbody.velocity;
				vel.Normalize();
				rigidbody.velocity = vel * (movementSpeed / 2);
				playerManager.isInAir = true;
			}
		}

		if (playerManager.isGrounded)
		{
			if (playerManager.isInterecting || inputHandler.moveAmount > 0)
			{
				transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
			}
			else
			{
				transform.position = targetPosition;
			}
		}
	}

	#endregion
}