using UnityEngine;
using System.Collections;

public class InputEvent : MonoBehaviour
{
	// delegates
	public delegate void OnObjectClickedEvent(GameObject _go, RaycastHit _hit);
	public event OnObjectClickedEvent OnObjectClicked;
	// singleton
	private static InputEvent instance;
	// instance
	public static InputEvent Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType(typeof(InputEvent)) as InputEvent;
			}
			return instance;
		}
	}


	void Update ()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 1000))
			{
				OnObjectClicked(hit.collider.gameObject, hit);
			}
		}
	}
}
