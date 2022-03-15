using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseUpdater : MonoBehaviour
{
    public List<GameObject> houses;
    public GameObject Player;
    private int wood;
    private int stone;
    
    void Start()
    {
        // get the children and put into an array
        wood = stone = 0;
    }

    void Update()
    {
        wood = Player.GetComponent<Player>().depositWood;
        stone = Player.GetComponent<Player>().depositStone;
        if(houses[0].activeSelf && wood + stone >= 2){
            houses[0].SetActive(false);
            houses[1].SetActive(true);
        }
        if(houses[1].activeSelf && wood + stone >= 4){
            houses[1].SetActive(false);
            houses[2].SetActive(true);
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/done", 1);
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/playseq", 0);
        }
    }
}
