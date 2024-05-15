using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drive_check_line : MonoBehaviour
{


    public GameObject next_line;

    public int line_index = 1;


    public int end_index = 13;

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("gamemanager");

        end_index = 13;
        if (line_index < end_index)
        {
            next_line = GameObject.Find("drive_check").transform.Find("check_line_" + line_index).gameObject;

            next_line.GetComponent<drive_check_line>().line_index = line_index + 1;
        }


        //마지막 체크 라인일경우
        else
        {
            next_line = GameObject.Find("drive_check").transform.Find("finish_line").gameObject;
        }



    }

    // Update is called once per frameS
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetPhotonView().OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            player.GetComponent<gamemanager>().line_checked_index += 1;

            player.GetComponent<gamemanager>().save_point = transform.position;
            player.GetComponent<gamemanager>().save_point_rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y - 90, transform.rotation.eulerAngles.z);
            
            next_line.SetActive(true);


            gameObject.SetActive(false);
        }

    }

}
