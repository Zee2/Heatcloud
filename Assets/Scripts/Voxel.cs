using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Voxel : MonoBehaviour {
	[SerializeField]
	public float value = 0;

	public Material thisMaterial;
	MeshRenderer meshRenderer;
	// Use this for initialization
	void Awake () {
		meshRenderer = gameObject.GetComponent<MeshRenderer>();
		thisMaterial = meshRenderer.material;
		UpdateVoxel();
	}
	
	public void UpdateVoxel () {
		if(value < 0.01f){
			meshRenderer.enabled = false;
		}else{
			meshRenderer.enabled = true;
		}
		Color lastColor = thisMaterial.GetColor("_TintColor");
		thisMaterial.SetColor("_TintColor", new Color(lastColor.r, lastColor.g, lastColor.b, value));
	}
}
