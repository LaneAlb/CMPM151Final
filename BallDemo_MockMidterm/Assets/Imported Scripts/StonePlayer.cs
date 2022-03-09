using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StonePlayer : MonoBehaviour
{
    public static int stoneValue = 0;
    Text stone;

    // Start is called before the first frame update
    void Start()
    {
        stone = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        stone.text = "Stone: " + stoneValue;
    }
}
