using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHandler : MonoBehaviour
{
	private PlayerManager playerManager;
	private PlayerLocomotion playerLocomotion;

	public Animator animator;
	private InputHandler inputHandler;
	private int vertical;
	private int horizontal;
	public bool canRotate;

	public void Intialize()
	{
		playerManager = GetComponentInParent<PlayerManager>();
		playerLocomotion = GetComponent<PlayerLocomotion>();
		animator = GetComponentInChildren<Animator>();
		inputHandler = GetComponentInParent<InputHandler>();
		vertical = Animator.StringToHash("Vertical");
		horizontal = Animator.StringToHash("Horizontal");
	}

	public void UpdateAnimatorValues(float vertiaclMovement, float horizontalMovement, bool isSprinting)
	{
		#region Vertical
		float v = 0;

		if (vertiaclMovement > 0 && vertiaclMovement < 0.55f)
		{
			v = 0.5f;
		}
		else if (vertiaclMovement > 0.55f)
		{
			v = 1;
		}
		else if (vertiaclMovement < 0 && vertiaclMovement > -0.55f)
		{
			v = -0.5f;
		}
		else if (vertiaclMovement < -0.55f)
		{
			v = -1;
		}
		else
		{
			v = 0;
		}
		#endregion

		#region Horizontal
		float h = 0;

		if (horizontalMovement > 0 && horizontalMovement < 0.55f)
		{
			h = 0.5f;
		}
		else if (horizontalMovement > 0.55f)
		{
			h = 1;
		}
		else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
		{
			h = -0.5f;
		}
		else if (horizontalMovement < -0.55f)
		{
			h = -1;
		}
		else
		{
			h = 0;
		}
		#endregion

		if (isSprinting && vertiaclMovement > 0)
		{
			v = 2;
			h = horizontalMovement;
		}

		animator.SetFloat(vertical, v, 0.1f, Time.deltaTime);
		animator.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
	}

	public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
	{
		animator.applyRootMotion = isInteracting;
		animator.SetBool("IsInteracting", isInteracting);
		animator.CrossFade(targetAnimation, 0.2f);
	}

	public void CanRotate()
	{
		canRotate = true;
	}

	public void StopRotation()
	{
		canRotate = false;
	}

	private void OnAnimatorMove()
	{
		if (playerManager.isInterecting == false) return;

		playerLocomotion.rigidbody.drag = 0;
		Vector3 deltaPosition = animator.deltaPosition;
		deltaPosition.y = 0;
		Vector3 velocity = deltaPosition / Time.deltaTime;
		playerLocomotion.rigidbody.velocity = velocity;
	}
}
