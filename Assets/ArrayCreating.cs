using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayCreating : MonoBehaviour
{
	
	public GameObject cubePrefab;
	public GameObject cubeOchag;

	public int positionX;
	public int positionY;
	public int positionZ;
	
	public int arraySizeX;
	public int arraySizeY;
	public int arraySizeZ;
	
	public List<GameObject> cubes= new List<GameObject>();
	public List<GameObject> cubesZarazhen= new List<GameObject>();

	public float interval = 1f;

	private float timer = 0f;
	
	void Start()
	{
		for (int x = positionX; x < arraySizeX; x++)
		{
			for (int y = positionY; y < arraySizeY; y++)
			{
				for (int z = positionZ; z < arraySizeZ; z++)
				{
					Vector3 position = new Vector3(x, y, z);
					//Instantiate(cubePrefab, position, Quaternion.identity);
					cubes.Add(Instantiate(cubePrefab, position, Quaternion.identity));
				}
			}
		}
		
		foreach (GameObject cube in cubes)
		{
			Debug.Log(cube.transform.position);
		}
		
		cubesZarazhen.Add(cubeOchag);
	}
	
	

	private void Update()
	{
		timer += Time.deltaTime;

		if (timer >= interval)
		{
			UpdateDensity();
			
			timer = 0f;
		}
	}
	
	void UpdateDensity(){
		//foreach (GameObject cube in cubes)
		//{
		//	Collider[] colliders = Physics.OverlapSphere(cube.transform.position, 1f);
		//	foreach(var collider in colliders) {
		//		GameObject gameObject = collider.gameObject;
		//		gameObject.GetComponent<SwitchColor>().density += 1;
		//	}
		//}
		
		cubeOchag.GetComponent<SwitchColor>().density = 100;
		
		List<GameObject> cubesZarazhenNew= new List<GameObject>();
		
		for (int i = 0; i < cubesZarazhen.Count; i++) {
			GameObject zarazka = cubesZarazhen[i];
			
			if (zarazka != null){
				Collider[] colliders = Physics.OverlapSphere(zarazka.transform.position, 1f);
				foreach(var collider in colliders) {
					GameObject gameObject = collider.gameObject;
					
					if (gameObject.GetComponent<SwitchColor>() != null){
						if (gameObject.GetComponent<SwitchColor>().density == 0) {
							cubesZarazhen.Add(gameObject);
						}
					
						gameObject.GetComponent<SwitchColor>().density = zarazka.GetComponent<SwitchColor>().density/colliders.Length+1;
						zarazka.GetComponent<SwitchColor>().density /= colliders.Length+1;
					}
				}
			}
		}
		
		//foreach(GameObject zarazka in cubesZarazhenNew){
		//	cubesZarazhen.Add(zarazka);
		//}
		
	}
	
	//cubeOchag.GetComponent<BoxCollider>.

}
