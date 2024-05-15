using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class finish_line : MonoBehaviourPun
{

    GameObject next_line;

    public int line_index = 1;


    public int end_index = 14;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {

    }


    private void OnTriggerEnter(Collider other)
    {


        if (other.gameObject.GetPhotonView().OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            next_line = GameObject.Find("drive_check").transform.Find("check_line_0").gameObject;

            next_line.GetComponent<drive_check_line>().line_index = 1;

            GameObject.Find("gamemanager").GetComponent<gamemanager>().lap_cnt += 1;
            GameObject.Find("gamemanager").GetComponent<gamemanager>().line_checked_index = 1;

            next_line.SetActive(true);

            gameObject.SetActive(false);
        }

        /*
        if (other.gameObject.GetPhotonView().OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
        
        //if (PhotonNetwork.LocalPlayer.IsLocal)
        {
            GameObject.Find("gamemanager").GetComponent<gamemanager>().wheel_cnt += 1;
        }
        */



    }
}
