using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
	public float horizontal;
	public float vertical;
	public float moveAmount;
	public float mouseX;
	public float mouseY;

	public bool sprintFlag;
	//public bool rollFlag;
	

	PlayerControls inputActions;

	Vector2 movementInput;
	Vector2 cameraInput;
	public bool sprintInput;

	public bool rollInput;

	private void OnEnable()
	{
		if (inputActions == null)
		{
			inputActions = new PlayerControls();
			inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
			inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
			inputActions.PlayerMovement.Sprint.performed += s => sprintInput = s.ReadValue<float>() > 0;
			inputActions.PlayerAction.Roll.performed += d => rollInput = d.ReadValue<float>() > 0;
		}

		inputActions.Enable();
	}

	private void OnDisable()
	{
		inputActions.Disable();
	}

	public void TickInput(float deltaTime)
	{
		MoveInput(deltaTime);
		//HandleRollInput(deltaTime);
	}

	private void MoveInput(float deltaTime)
	{
		horizontal = movementInput.x;
		vertical = movementInput.y;
		moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
		mouseX = cameraInput.x;
		mouseY = cameraInput.y;
	}

	//private void HandleRollInput(float deltaTime)
	//{
	//	//rollInput = inputActions.PlayerAction.Roll.phase == UnityEngine.InputSystem.InputActionPhase.Started;

	//	if (rollInput)
	//	{
	//		rollFlag = true;
	//	}
	//}
}
