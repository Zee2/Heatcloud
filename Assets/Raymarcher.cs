using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raymarcher : MonoBehaviour {
	public float interval;
	public VoxelManager voxelSystem;
	float lastRayTime = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time - lastRayTime < interval)
			return;	

		lastRayTime = Time.time;
		RaycastHit hit;
		if(Physics.Raycast(transform.position, transform.forward, out hit, 1000, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore)){
			//Debug.Log("Hit " + hit.point);
			Raymarch(transform.position, hit.point - transform.position);
		}

		
	}	

	void Raymarch(Vector3 start, Vector3 vec){
		for(float x = 0; x < vec.magnitude; x += 0.5f){
			int xIndex = Mathf.RoundToInt(((start + vec.normalized * x).x + (voxelSystem.dimension/2)*voxelSystem.voxelSize) / voxelSystem.voxelSize);
			int yIndex = Mathf.RoundToInt(((start + vec.normalized * x).y + (voxelSystem.dimension/2)*voxelSystem.voxelSize) / voxelSystem.voxelSize);
			int zIndex = Mathf.RoundToInt(((start + vec.normalized * x).z + (voxelSystem.dimension/2)*voxelSystem.voxelSize) / voxelSystem.voxelSize);
			//Debug.Log("voxels: " + xIndex + ", " + yIndex + ", " + zIndex);
			voxelSystem.HitVoxel(xIndex, yIndex, zIndex);
		}
	}
}
