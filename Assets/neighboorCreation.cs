using System.Collections;
using System.Collections.Generic;
using UnityEngine;



   public class neighboorCreation : MonoBehaviour
   {
   	
   	public GameObject parentCube;
   	public GameObject CubePrefab;
   	
   	public GameObject emptyObject;
 
   	
   	
	   void Start()
	   {

		   //Invoke("CreationProcess", 0.3f);


	   }
	   
	   void Update()
    {
	    if (Input.GetKeyDown("space"))
	    {
		    CreationProcess();
	    }
    }

	   
	   
	   void CreationProcess()
	   {
	   	
		   //	   	transform.position = new Vector3.zero;
	   	
		   // Create neighbor cubes and set their positions relative to the parent cube
		   GameObject cube1 = emptyObject;
		   cube1.transform.position = parentCube.transform.position + new Vector3(1, 0, 0);
		   Instantiate(CubePrefab,cube1.transform.position,Quaternion.identity ).GetComponent<neighboorCreation>().parentCube = gameObject;

		   GameObject cube2= emptyObject;
		   cube2.transform.position = parentCube.transform.position + new Vector3(-1, 0, 0);
		   Instantiate(CubePrefab,cube2.transform.position,Quaternion.identity ).GetComponent<neighboorCreation>().parentCube = gameObject;

		   GameObject cube3= emptyObject;
		   cube3.transform.position = parentCube.transform.position + new Vector3(0, 0, 1);
		   Instantiate(CubePrefab,cube3.transform.position,Quaternion.identity ).GetComponent<neighboorCreation>().parentCube = gameObject;

		   GameObject cube4= emptyObject;
		   cube4.transform.position = parentCube.transform.position + new Vector3(0, 0, -1);
		   Instantiate(CubePrefab,cube4.transform.position,Quaternion.identity ).GetComponent<neighboorCreation>().parentCube = gameObject;
	   	
	   	
		   Destroy(gameObject.GetComponent<neighboorCreation>(),0.1f 
		   );
	   }
   }
