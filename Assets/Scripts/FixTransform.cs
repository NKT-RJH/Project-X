using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixTransform : MonoBehaviour
{
	private void Update()
	{
		transform.localPosition = new Vector3(0, transform.localPosition.y, 0);
		//transform.localPosition = Vector3.zero;
	}
}
