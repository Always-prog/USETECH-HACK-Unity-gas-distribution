using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSizeGetter : MonoBehaviour
{
	void Start()
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3 size = mesh.bounds.size;
		Debug.Log("Mesh Size: " + size);
	}
}
