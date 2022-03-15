using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindAnimation : MonoBehaviour {

	public float partitionIndx;
	public int numPartitions;
	public float[] aveMag;
	public int numDisplayedBins;
	private Vector3 baseScale = new Vector3(0.75f, 0.75f, 0.75f); // Use this for size override if wanted
	
	public ParticleSystem ps;
	Vector3 slant = new Vector3(60, 0, 0);
	Vector3 original = new Vector3(90, 0, 0);
	void Start(){
		ps = GetComponent<ParticleSystem>();

		numPartitions = 8;
		aveMag = new float[numPartitions];
		partitionIndx = 0;
		numDisplayedBins = 512 / 2; //NOTE: we only display half the spectral data because the max displayable frequency is Nyquist (at half the num of bins)
	}

	void Update () {
		var main = ps.main;
		var emission = ps.emission;

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
            aveMag[i] = 0.5f * aveMag[i] * 1000;
        }
		
		for (int i = 0; i < 7; i++)
		{
			Debug.Log(aveMag[i]);
			if (aveMag[i] >= 0.75)
			{
				main.startSpeed = 60;
				ps.transform.eulerAngles = slant;
				emission.rateOverTime = 1400;
			}
			else
			{
				main.startSpeed = 20;
				ps.transform.eulerAngles = original;
				emission.rateOverTime = 700;
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag ("Pick Up"))
			other.gameObject.SetActive (false);
	}
}

