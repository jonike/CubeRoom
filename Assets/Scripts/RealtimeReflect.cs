using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealtimeReflect : MonoBehaviour {

	public Cubemap cubmap;
	public Camera cam;
	private Material curMat;

	// Use this for initialization
	void Start () {
		InvokeRepeating("UpdateChange", 1, 0.1f);
		Renderer renderer = GetComponent<Renderer>();
    	curMat = renderer.sharedMaterial; 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void UpdateChange () {  
    	// cam.transform.rotation = Quaternion.identity;  
    	cam.RenderToCubemap(cubmap);  
  
    	curMat.SetTexture("_Cubemap", cubmap);  
	}  
}
