using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyAllChildren : MonoBehaviour
{
    //recusively checks for all children within the targeted hierarchy given GameObject Parent
    public GameObject Parent;
    //TODO: add public Event callFunction; that is used to call whatever function is targeted in the script

    // Start is called before the first frame update
    void Start()
    {
        getChildren(Parent);
    }
    void getChildren(GameObject obj)
    {
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            recursiveChildCall(obj.transform.GetChild(i).gameObject);
        }
    }

    void recursiveChildCall(GameObject currentObj)
    {
        for (int i = 0; i < currentObj.transform.childCount; i++)
        {
            recursiveChildCall(currentObj.transform.GetChild(i).gameObject);
        }

        //if the currentObject is NOT an empty object (i.e. has more components than just a transform)
        if (currentObj.GetComponents<Component>().Length > 1)
        {
            //TODO: figure out how to generalize this to a public delegate "callFunction" that gets called.
            AddOutlineToObject(currentObj);
        }
    }

    public void AddOutlineToObject(GameObject obj)
    {
        var outline = obj.GetComponent<Outline>();
        if (outline == null) 
        {
            //if the object doesn't already have an outline, give it a default one
            outline = obj.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = Color.yellow;
            outline.OutlineWidth = 7.5f;
            //turn outline off by default
            outline.enabled = false;
        }
    }
}
