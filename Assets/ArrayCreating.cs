using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayCreating : MonoBehaviour
{
	
	public GameObject cubePrefab;


	public int positionX;
	public int positionY;
	public int positionZ;
	
	public int arraySizeX;
	public int arraySizeY;
	public int arraySizeZ;

	
	void Start()
	{
		for (int x = positionX; x < arraySizeX; x++)
		{
			for (int y = positionY; y < arraySizeY; y++)
			{
				for (int z = positionZ; z < arraySizeZ; z++)
				{
					Vector3 position = new Vector3(x, y, z);
					Instantiate(cubePrefab, position, Quaternion.identity);
				}
			}
		}
	}

}
