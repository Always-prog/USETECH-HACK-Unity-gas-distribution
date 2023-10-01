using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchColor : MonoBehaviour
{
	
	Color[] colors = { Color.blue, Color.red, Color.green, Color.yellow };
	
	public float transparency;

	
    // Start is called before the first frame update
    void Start()
    {
	    Renderer rend = gameObject.GetComponent<Renderer>();
	    rend.material.color = colors[Random.Range(0, colors.Length)];
	    
	    Color oldColor = GetComponent<Renderer>().material.color;
	    Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, transparency);
	    GetComponent<Renderer>().material.SetColor("_Color", newColor);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    


}
