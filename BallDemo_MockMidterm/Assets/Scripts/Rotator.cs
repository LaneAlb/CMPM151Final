using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

	public float partitionIndx;
	public int numPartitions;
	public float[] aveMag;
	public int numDisplayedBins;
	private Vector3 baseScale = new Vector3(0.75f, 0.75f, 0.75f); // Use this for size override if wanted
	void Start(){
		numPartitions = 8;
		aveMag = new float[numPartitions];
		partitionIndx = 0;
		numDisplayedBins = 512 / 2; //NOTE: we only display half the spectral data because the max displayable frequency is Nyquist (at half the num of bins)
	}

	void Update () {
		// rotate everything with the script
		transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);

		numPartitions = 8;
		aveMag = new float[numPartitions];
		partitionIndx = 0;
		numDisplayedBins = 512 / 2; //NOTE: we only display half the spectral data because the max displayable frequency is Nyquist (at half the num of bins)

		for (int i = 0; i < numDisplayedBins; i++) 
		{
			if(i < numDisplayedBins * (partitionIndx + 1) / numPartitions)
            {
				aveMag[(int)partitionIndx] += AudioPeer.spectrumData [i] / (512/numPartitions);
			}
			else
            {
				partitionIndx++;
				i--;
			}
		}

        //scale and bound the average magnitude.
		// values are usually close to 0, we magnify them for actual use
        for (int i = 0; i < numPartitions; i++)
        {
            aveMag[i] = 0.25f * aveMag[i] * 200;
			if(aveMag[i] < 0.3f){
				aveMag[i] = 0.3f; 
			}
			if(aveMag[i] > 4f){
				aveMag[i] = 3f;
			}
        }
		
		// sync certain pickups together
		for (int i = 0; i <= 3; i++)
		{
			if (gameObject.name == "Pickup (" + i + ")") {
				Vector3 nScale = new Vector3 (aveMag[i], aveMag[i], aveMag[i]);
				if (nScale.y < 0.3f) nScale.y = 0.3f * Random.Range(1.2f, 1.5f);
				transform.localScale = nScale;
			}
			if (gameObject.name == "Pickup (" + (i+4) + ")") {
				Vector3 nScale = new Vector3 (aveMag[i], aveMag[i], aveMag[i]);
				if (nScale.y < 0.3f) nScale.y = 0.3f * Random.Range(1.2f, 1.5f);
				transform.localScale = nScale;
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag ("Pick Up"))
			other.gameObject.SetActive (false);
	}
}

