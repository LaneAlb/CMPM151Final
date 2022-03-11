using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // movement vars
    private Vector3 rotation;
    private Vector3 direction;
    private float horizontal;
    private float vertical;
    private float rayRange = 5;
    public float speed = 10.0f;
    public float rotSpeed = 120.0f;
    // stats
    public int wood = 0;
    public int stone = 0;
    public int depositWood = 0;
    public int depositStone = 0;
    private int count = 0; 
    public Text countText;

    //interal script objects
    private Collider lastSeenObject; //used to clear the outline of the last seen object by the player

	//************* Need to setup this server dictionary...
	Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog> ();
	//*************

    // Start is called before the first frame update
    void Start()
    {
        rotation = new Vector3(0, 0, 0);
        horizontal = Input.GetAxis("Horizontal");
        // from old Midterm player script
        Application.runInBackground = true; //allows unity to update when not in focus

		//************* Instantiate the OSC Handler...
	    OSCHandler.Instance.Init ();
		OSCHandler.Instance.SendMessageToClient ("pd", "/unity/trigger", "ready");
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/playseq", 1);
        //*************
    }

    void FixedUpdate() {
        //************* Routine for receiving the OSC...
		OSCHandler.Instance.UpdateLogs();
		Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog>();
		servers = OSCHandler.Instance.Servers;

		foreach (KeyValuePair<string, ServerLog> item in servers) {
			// If we have received at least one packet,
			// show the last received from the log in the Debug console
			if (item.Value.log.Count > 0) {
				int lastPacketIndex = item.Value.packets.Count - 1;

				//get address and data packet
				countText.text = item.Value.packets [lastPacketIndex].Address.ToString ();
				countText.text += item.Value.packets [lastPacketIndex].Data [0].ToString ();
			}
		}
		//*************
    }

    // Update is called once per frame
    void Update()
    {
        // PLAYER MOVEMENT
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (vertical > 0)
        {
            this.GetComponent<CharacterController>().SimpleMove(transform.forward * this.speed);
        }
        else if (vertical < 0)
        {
            this.GetComponent<CharacterController>().SimpleMove(-transform.forward * this.speed);
        }

        if (horizontal > 0)
        {
            rotation += new Vector3(0, 1, 0) * Time.deltaTime * rotSpeed;
        }
        else if (horizontal < 0)
        {
            rotation -= new Vector3(0, 1, 0) * Time.deltaTime * rotSpeed;
        }
        this.GetComponent<Transform>().eulerAngles = rotation;
        // End Player Movement

        // OSC UPDATER FOR PD SOUND EFFECTS
        sendSoundEffect(); // send PD the temp / sound effect triggers

        // For Object Pickup / Outline
        castRay();
        // Let anyone end the game with the 'esc' Key in editor or in the build
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }
    } // end Update()

    //Player interaction method. Uses raycast to detect collision with objects and allows player to interact with it.
    void castRay()
    {
        RaycastHit hitInfo = new RaycastHit();
        // if the player is currently looking at and within range to interact with an object 
        if (Physics.Raycast(this.transform.position, this.transform.forward, out hitInfo, rayRange))
        {
            //print("hit:" + hitInfo.collider);

            lastSeenObject = hitInfo.collider; //save collider to remove highlight when no longer being looked at

            //grab the outline component to highlight the object they are looking at
            var outline = hitInfo.collider.GetComponent<Outline>();
            if (outline != null)
            {        //if there's no outline component on it, nothing happens.
                outline.enabled = true; //indicates visually to the player they can interact with this object!
            }
            // if the player wishes to interact with an object in front of them
            if (Input.GetKeyDown(KeyCode.E))
            {
                // check which object and increase the proper resource value
                if (hitInfo.collider.tag == "ROCK")
                {
                    this.stone++;
                    Destroy(hitInfo.collider.gameObject);
                    StonePlayer.stoneValue += 10;
                    // OSC Updater for PD Sound Effects
                    count += 1;
		            setCountText ();
                    sendSoundEffect();
                    // Send the Client Message for Rock Pick Up
                    //Debug.Log("-------- Pick Rock ----------");
                    // trigger noise burst for pick rock
                    // remember to add "rock" into pd oscRecieve
                    OSCHandler.Instance.SendMessageToClient("pd", "/unity/rock", 1);
                }
                if (hitInfo.collider.tag == "TREE")
                {
                    this.wood++;
                    Destroy(hitInfo.collider.gameObject);
                    WoodPlayer.woodValue += 10;
                    // OSC Updater for PD Sound Effects
                    count += 1;
		            setCountText ();
                    sendSoundEffect();
                    // Send the Client Message for Tree Pick Up
                    //Debug.Log("-------- Pick Tree ----------");
                    // trigger noise burst for pick tree
                    OSCHandler.Instance.SendMessageToClient("pd", "/unity/tree", 1);
                }
                if (hitInfo.collider.tag == "DEPOSIT") //deposit resources in the cart
                {
                    this.depositWood += this.wood;
                    this.wood = 0;
                    this.depositStone += this.stone;
                    this.stone = 0;
                    WoodDeposit.woodValue += WoodPlayer.woodValue;
                    StoneDeposit.stoneValue += StonePlayer.stoneValue;
                    WoodPlayer.woodValue = 0;
                    StonePlayer.stoneValue = 0;
                    // OSC Updater for PD Sound Effects
                    count = 0;
		            setCountText ();
                    sendSoundEffect();
                    // Send the Client Message for Desposit
                    //Debug.Log("-------- Deposit ----------");
                    // trigger noise burst for deposit
                    // OSCHandler.Instance.SendMessageToClient("pd", "/unity/desposit", 1);
                }
            }
        }
        //check if the last seen object should no longer be highlighted
        else if (lastSeenObject != null)
        {
            print("lastSeenObject: " + lastSeenObject);
            var outline = lastSeenObject.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false; //remove the highlight on the previous object
            }
            lastSeenObject = null; //small optimization
        }
    }

    void setCountText()
	{
		countText.text = "Count: " + count.ToString ();

		//************* Send the message to the client...
		OSCHandler.Instance.SendMessageToClient ("pd", "/unity/trigger", count);
		//*************
	}

    void sendSoundEffect()
	{
		// change the tempo of the sequence based on how many obejcts we have picked up.
        if(count < 2)
        {
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/tempo", 500);
        }
        if (count < 4)
        {
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/tempo", 400);
        }
        else if(count < 6)
        {
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/tempo", 300);
        }
        else if (count < 8)
        {
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/tempo", 150);
        }
        else
        {
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/playseq", 0);
        } 

        // send the "completed" sound effect here
        if (count == 8)
        {
			OSCHandler.Instance.SendMessageToClient("pd", "/unity/done", 1);
		}
        // special sound effects would be here too
        // for pick up and deposit sound effect sending
        // check the "CastRay()" Function
	}

}
