using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
	public GameObject objectPrefab;
	public float spawnInterval = 1f;

	private float timer = 0f;

	private void Update()
	{
		timer += Time.deltaTime;

		if (timer >= spawnInterval)
		{
			Instantiate(objectPrefab, transform.position, transform.rotation);
			timer = 0f;
		}
	}
}
