using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using RESTClient;
using Azure.StorageServices;

public class VoxelManager : MonoBehaviour {

	public string storageAccount;
	public string accessKey;
	public string container;

	public int dimension = 20;
	public float voxelSize = 2;
	Voxel[,,] voxels;
	public GameObject voxelPrefab;



	private StorageServiceClient client;
	private BlobService blobService;

	// Use this for initialization
	void Start () {
		if (string.IsNullOrEmpty(storageAccount) || string.IsNullOrEmpty(accessKey)){
      		Debug.Log("Storage account/access key required");
    	}else{
			client = StorageServiceClient.Create(storageAccount, accessKey);
			blobService = client.GetBlobService();
		}
		
		voxels = new Voxel[dimension,dimension,dimension];

		for(int x = 0; x < dimension; x++){
			for(int y = 0; y < dimension; y++){
				for(int z = 0; z < dimension; z++){
					Vector3 pos = new Vector3((x - (dimension/2)) * voxelSize, (y - (dimension/2)) * voxelSize, (z - (dimension/2)) * voxelSize);
					voxels[x,y,z] = Instantiate(voxelPrefab, pos, Quaternion.identity).GetComponent<Voxel>();
					voxels[x,y,z].transform.localScale = Vector3.one * voxelSize;
				}
			}

		}
		//SaveVoxelData();
		StartCoroutine(UpdateVoxels());
	}

	IEnumerator UpdateVoxels(){
		while(true){
			for(int x = 0; x < dimension; x++){
				for(int y = 0; y < dimension; y++){
					for(int z = 0; z < dimension; z++){
						voxels[x,y,z].UpdateVoxel();
					}
					
				}
				yield return new WaitForEndOfFrame();
			}
		}
		
	}

	public void HitVoxel(int x, int y, int z){
		try{
			voxels[x,y,z].value += 0.002f;
		}catch{
			// pass silently
		}
		
	}
	
	public void SaveVoxelData(){
		string voxelString = "{ \"voxels\": [ ";

		for(int x = 0; x < dimension; x++){
				for(int y = 0; y < dimension; y++){
					for(int z = 0; z < dimension; z++){
						voxelString += voxels[x,y,z]
					}
					
				}
			}



	}
}