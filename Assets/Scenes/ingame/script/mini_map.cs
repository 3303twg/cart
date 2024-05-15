using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mini_map : MonoBehaviour
{
    public GameObject top_camera;

    public GameObject target;

    public GameObject player_symbol;



    public float offset_x;
    public float offset_y = 380;
    public float offset_z;

    public string cart_name;
    public string[] slot_car_list;


    void Start()
    {

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

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        target = GameObject.Find("player_car");

        if (target)
        {
            top_camera.transform.position = target.transform.position;
            top_camera.transform.position += Vector3.up * offset_y;
            top_camera.transform.rotation = Quaternion.Euler(90, target.transform.rotation.eulerAngles.y, 0);



            player_symbol.transform.position = target.transform.position;
            player_symbol.transform.position += Vector3.up * 260;
            player_symbol.transform.rotation = Quaternion.Euler(0, target.transform.rotation.eulerAngles.y, 0);

        }


    }
}
