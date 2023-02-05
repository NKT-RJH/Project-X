using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance;

	protected virtual void Awake()
	{
		if(instance != null)
		{
			Destroy(gameObject);

			return;
		}

		instance = GetComponent<T>();

		DontDestroyOnLoad(gameObject);
	}
}
