using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;

public class WindowTest : MonoBehaviour {

	// Use this for initialization

	public GameObject left, right;
	GameObject composite;
	public Material myMaterial;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI() {
		if (GUI.Button(new Rect(0, 150, 50, 20), "窗户")) {  
            Test();
        }
	}

	void Test() {
		Mesh m = CSG.Subtract(left, right);

		composite = new GameObject();
		composite.AddComponent<MeshFilter>().sharedMesh = m;
		composite.AddComponent<MeshRenderer>().sharedMaterial = myMaterial;

		if(left) GameObject.Destroy(left);
		if(right) GameObject.Destroy(right);
	}
}
