using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rear_camera_controller : MonoBehaviourPun
{
    public string cart_name;

    public GameObject target;


    public float offset_x = 5f;
    public float offset_y = 3f;
    public float offset_z = 0f;


    public string[] slot_car_list;

    void Start()
    {

        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }


        slot_car_list = GameObject.Find("data_box").GetComponent<data_box>().slot_car_list;
        cart_name = slot_car_list[PhotonNetwork.LocalPlayer.ActorNumber - 1];

        /*
        ExitGames.Client.Photon.Hashtable user_custom_value = PhotonNetwork.LocalPlayer.CustomProperties;
        //new ExitGames.Client.Photon.Hashtable();

        if (user_custom_value.TryGetValue("cartbody", out object cart_body))
        {
            cart_name = (string)cart_body;
            Debug.Log("¿Ã∏ß : " + cart_name);
        }
        */


        target = GameObject.Find("player_car");

        GetComponent<Camera>().targetTexture = Resources.Load<RenderTexture>("slot_" + (PhotonNetwork.LocalPlayer.ActorNumber - 1));
    }

    // Update is called once per frame
    void Update()
    {
        //target = GameObject.Find(cart_name + "(Clone)");
        Vector3 target_vector = target.transform.forward * -1;


        

        transform.position = target.transform.position;
        transform.position = target.transform.position + target_vector * offset_x;
        //transform.position += Vector3.forward * (offset_x * -1);
        transform.position += Vector3.up * (offset_y * +1);
        transform.rotation = Quaternion.Euler(target.transform.rotation.eulerAngles.x + 15 , target.transform.rotation.eulerAngles.y + 180, target.transform.rotation.eulerAngles.z);

    }



    public void remove_object()
    {
        /*
        if (photonView.IsMine)
        {
            GameObject.Find("gamemanager").GetComponent<gamemanager>().end_state_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = 1;
            PhotonNetwork.Destroy(gameObject);
        }
        */
    }

}

