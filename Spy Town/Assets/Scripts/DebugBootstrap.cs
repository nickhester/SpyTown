using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DebugBootstrap : MonoBehaviour
{
	
	void Start ()
	{
	
	}
	
	void Update ()
	{
	
	}

	public void StartGame()
	{
		SceneManager.LoadScene("TestScene");
	}
}
