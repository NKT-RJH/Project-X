using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
	private InputHandler inputHandler;
	private Animator animator;
	private CameraHandler cameraHandler;
	private PlayerLocomotion playerLocomotion;

	public bool isInterecting;
	[Header("PlayerFlags")]
	public bool isSprinting;
	public bool isInAir;
	public bool isGrounded;

	private void Awake()
	{
		cameraHandler = CameraHandler.instance;
	}

	private void Start()
	{
		inputHandler = GetComponent<InputHandler>();
		animator = GetComponentInChildren<Animator>();
		playerLocomotion = GetComponent<PlayerLocomotion>();
	}

	private void Update()
	{
		isInterecting = animator.GetBool("IsInteracting");

		inputHandler.TickInput(Time.deltaTime);
		playerLocomotion.HandleMovement(Time.deltaTime);
		playerLocomotion.HandleRollingAndSprinting(Time.deltaTime);
		playerLocomotion.HandleFalling(Time.deltaTime,playerLocomotion.moveDirection);
	}

	private void FixedUpdate()
	{
		if (cameraHandler == null) return;

		cameraHandler.FollowTarget(Time.fixedDeltaTime);
		cameraHandler.HandleCameraRotation(Time.fixedDeltaTime, inputHandler.mouseX, inputHandler.mouseY);
	}

	private void LateUpdate()
	{
		inputHandler.sprintFlag = false;
		isSprinting = inputHandler.sprintInput;

		if (isInAir)
		{
			playerLocomotion.inAirTimer += Time.deltaTime;
		}
	}
}
