using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WoodPlayer : MonoBehaviour
{
    public static int woodValue = 0;
    Text wood;

    // Start is called before the first frame update
    void Start()
    {
        wood = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        wood.text = "Wood: " + woodValue;
    }
}
