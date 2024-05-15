using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;
using Photon.Pun;

public class camera_setup : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            CinemachineVirtualCamera followCam = FindObjectOfType<CinemachineVirtualCamera>();
            //followCam.gameObject.SetActive(true);

            //followCam.
            //followCam.Follow = transform;
            //followCam.LookAt = transform;
        }
        
    }

}
