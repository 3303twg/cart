using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class speedometer : MonoBehaviourPun
{

    public Text text;

    public int speed = 0;

    GameObject player;

    public string cart_name;
    public string[] slot_car_list;

    // Start is called before the first frame update
    void Awake()
    {

        Debug.Log("난 왜부름??");
        text.text = "0";


        slot_car_list = GameObject.Find("data_box").GetComponent<data_box>().slot_car_list;
        cart_name = slot_car_list[PhotonNetwork.LocalPlayer.ActorNumber - 1];

        /*

        ExitGames.Client.Photon.Hashtable user_custom_value = PhotonNetwork.LocalPlayer.CustomProperties;
        //new ExitGames.Client.Photon.Hashtable();

        if (user_custom_value.TryGetValue("cartbody", out object cart_body))
        {
            cart_name = (string)cart_body;
            Debug.Log("이름 : " + cart_name);
        }
        */


        player = GameObject.Find("player_car");
        


    }

    // Update is called once per frame
    void Update()
    {
        //player = GameObject.Find(cart_name + "(Clone)");
        player = GameObject.Find("player_car");
        if (player)
        {
            speed = Mathf.RoundToInt(player.GetComponent<car_controller>().move_speed * 5);


            text.text = speed.ToString();
        }
    }
}
