using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeRotation : MonoBehaviour
{
    private Transform tree;
    private float timer;

    public float speedModifier = 1f;

    private Quaternion currentTargetAngle;

    // Start is called before the first frame update
    void Start()
    {
        tree = this.gameObject.GetComponent<Transform>();
        timer = 0f;
        currentTargetAngle = Quaternion.Euler(0f,0f,0f);
    }

    // Update is called once per frame
    void Update()
    {
        // Every so often, make a new target rotation for the tree
        if(timer >= 2f / speedModifier){
            newRotation();
            timer = 0f;
        }

        // Rotates the tree towards the target angle over time by a rate given by "speedModifier"
        tree.rotation = Quaternion.RotateTowards(tree.rotation, currentTargetAngle, speedModifier * Time.deltaTime);

        timer += Time.deltaTime;
    }

    // Makes a new target rotation for the tree, without allowing it to lean too much
    private void newRotation(){
        float rotateX = Random.Range(-15f, 15f);
        float rotateZ = Random.Range(-15f, 15f);
        rotateX = Mathf.Clamp(tree.rotation.x + rotateX, -30f, 30f);
        rotateZ = Mathf.Clamp(tree.rotation.z + rotateZ, -30f, 30f);

        currentTargetAngle = Quaternion.Euler(rotateX, 0, rotateZ);
    }
}
