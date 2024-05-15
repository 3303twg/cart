using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lobby_spawner_controller : MonoBehaviour
{
    // Start is called before the first frame update

    private void Start()
    {
        PhotonNetwork.Instantiate("lobbymanager", transform.position, transform.rotation);


    }

    void Update()
    {
        
    }
}
