using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{

    
    
	//private float timer = 0f;
	//private float delay = 0.1f;
	//private bool isTriggerEnabled = false;

	//private void Update()
	//{
	//	// Increment the timer
	//	timer += Time.deltaTime;

	//	if (timer >= delay && !isTriggerEnabled)
	//	{
	//		// Enable the trigger
	//		GetComponent<Collider>().isTrigger = true;
	//		isTriggerEnabled = true;
	//		Destroy(gameObject.GetComponent<TriggerScript>());
	//	}
	//}
    
    
    
	void OnTriggerEnter(Collider other)
	{

		Destroy(other.gameObject);
		
	}

}
