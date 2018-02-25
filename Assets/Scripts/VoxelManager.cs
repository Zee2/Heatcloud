﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

using System;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using RESTClient;
using Azure.StorageServices;

public class VoxelManager : MonoBehaviour {
	public Text pointsText; 
	public Text statusText;
	public string storageAccount;
	public string accessKey;
	public string container;
	public string filename;

	public int dimension = 20;
	public float voxelSize = 2;
	Voxel[,,] voxels;
	public GameObject voxelPrefab;

	public int points = 0;

	private StorageServiceClient client;
	private BlobService blobService;

	// Use this for initialization
	void Start () {
		statusText.text = "";
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
			voxels[x,y,z].UpdateVoxel();
			points++;
		}catch{
			// pass silently
		}
		
	}

	public void TriggerSave(){
		StartCoroutine(SaveVoxelData());
	}

	
	public IEnumerator SaveVoxelData(){
		bool isFirst = true;

		using(MemoryStream mStream = new MemoryStream()){
			for(int x = 0; x < dimension; x++){
				for(int y = 0; y < dimension; y++){
					for(int z = 0; z < dimension; z++){
						if(!isFirst){
							mStream.Write(Encoding.ASCII.GetBytes(","), 0, 1);
						}
						isFirst = false;
						byte[] value = Encoding.ASCII.GetBytes(voxels[x,y,z].value.ToString("G3"));
						mStream.Write(value, 0, value.Length);
					}
					
				}
				statusText.text = "Processing voxel CSV, please wait";
				yield return new WaitForEndOfFrame();
			}
			StartCoroutine(blobService.PutTextBlob(PutTextBlobComplete, Encoding.ASCII.GetString(mStream.ToArray()), container, filename));
		}
		
	}

	public void LoadVoxelData(){
		statusText.text = "Loading voxel data from Azure...";
		string resourcePath = container + "/" + filename;
    	StartCoroutine(blobService.GetTextBlob(GetTextBlobComplete, resourcePath));
	}

	private void PutTextBlobComplete(RestResponse response){
		if(response.IsError){
			Debug.Log("error: " + response.Content);
			return;
		}else{
			statusText.text = "Put blob status:\n" + response.StatusCode;
		}
	}

	private void GetTextBlobComplete(RestResponse response){
		if (response.IsError){
      		statusText.text = response.ErrorMessage + " Error getting blob:" + response.Content;
			return;
		}


    	StartCoroutine(ProcessIncomingVoxels(response.Content));
	}

	IEnumerator ProcessIncomingVoxels(string content){
		string[] strings = content.Split(',');
		int index = 0;
		for(int x = 0; x < dimension; x++){
			for(int y = 0; y < dimension; y++){
				for(int z = 0; z < dimension; z++){
					voxels[x,y,z].value = float.Parse(strings[index]);
					index++;
				}
					
			}
			statusText.text = "Processing voxel CSV, please wait";
			yield return new WaitForEndOfFrame();
		}
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.Space)){
			StartCoroutine(SaveVoxelData());
		}
		pointsText.text = points.ToString();
	}
}