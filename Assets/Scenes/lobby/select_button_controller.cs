using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class select_button_controller : MonoBehaviourPun
{
    public int car_index = 0;
    public int my_index = 0;

    public string cart_name;

    public GameObject[] car_list;

    public render_controller[] slot_list = new render_controller[4];

    public Vector3 position = new Vector3(0, 0, 0);
    public Quaternion rotation_value = Quaternion.Euler(0, 180, 0);

    byte group = 0;

    //public RenderTexture cart_texture;


    void Start()
    {
        car_list = GameObject.Find("lobbymanager").GetComponent<lobby_manager>().car_list;

        transform.Find("Image").transform.Find("RawImage").GetComponent<RawImage>().texture = Resources.Load<RenderTexture>("cart_room_" + car_index);
        transform.Find("Image").transform.Find("text").GetComponent<Text>().text = "car_" + car_index;

        for (int i = 0; i < slot_list.Length; i++)
        {
            slot_list[i] = GameObject.Find("slot_renderer").transform.Find("slot_" + i).GetComponent<render_controller>();
        }
        

        /*
        slot_list[0] = GameObject.Find("slot_0").GetComponent<render_controller>();
        slot_list[1] = GameObject.Find("slot_1").GetComponent<render_controller>();
        slot_list[2] = GameObject.Find("slot_2").GetComponent<render_controller>();
        slot_list[3] = GameObject.Find("slot_3").GetComponent<render_controller>();
        */
    }


    void Update()
    {
        
    }


    public void select_car()
    {


        ExitGames.Client.Photon.Hashtable user_custom_value = PhotonNetwork.LocalPlayer.CustomProperties;

        /*
        if (user_custom_value.TryGetValue("cartbody", out object cart_body))
        {
            //���⼭�� īƮ������ �ı��� īƮ�� �̸�
            cart_name = (string)cart_body;
            Debug.Log("�̸� : " + cart_name);
        }
        */

        //Debug.Log("car_list")
        //ũ�⸦ ��ȯ�ϱ⶧���� 1�� �������
        user_custom_value["cartbody"] = "car_" + (car_index);
        PhotonNetwork.LocalPlayer.SetCustomProperties(user_custom_value);
        //���� īƮ�ٵ� Ŀ���Һ������� ��ü

        //����īƮ = 1
        //�ı��ؾ��� īƮ = 0


        //���⼭�� īƮ������ 0
        /*
        for (int i = 0; i < slot_list.Length; i++)
        {
            slot_list[i].render_RPC_call(car_index);
        }
        */

        if (user_custom_value.TryGetValue("cartbody", out object cart_body))
        {
            //���⼭�� īƮ������ ���������� �����󿡼� īƮ�̸��� �Ҵ����� ����īƮ�� �̸�
            cart_name = (string)cart_body;
        }

        GameObject.Find("lobbymanager").GetComponent<lobby_manager>().slot_car_list[my_index] = cart_name;
        //slot_list[0].render_RPC_call(GameObject.Find("lobbymanager").GetComponent<lobby_manager>().slot_car_list);

        /*
        for (int i = 0; i < slot_list.Length; i++)
        {
            slot_list[i].render_RPC_call(GameObject.Find("lobbymanager").GetComponent<lobby_manager>().slot_car_list);
        }
        */

        GameObject.Find("lobbymanager").GetComponent<lobby_manager>().button_RPC_call();


        //GameObject.Find("lobbymanager").GetComponent<lobby_manager>().photonView.RPC("call_sync_car", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber - 1, GameObject.Find("lobbymanager").GetComponent<lobby_manager>().slot_car_list[PhotonNetwork.LocalPlayer.ActorNumber - 1]); 
        /*
        GameObject.Find("slot_0").GetComponent<render_controller>().render_RPC_call(my_index, car_index, cart_name);
        GameObject.Find("slot_1").GetComponent<render_controller>().render_RPC_call(my_index, car_index, cart_name);
        GameObject.Find("slot_2").GetComponent<render_controller>().render_RPC_call(my_index, car_index, cart_name);
        GameObject.Find("slot_3").GetComponent<render_controller>().render_RPC_call(my_index, car_index, cart_name);
        */

        //cart_name = "car_" + (car_index);

        //�ٵ� �ش� ������Ʈ�� ������ �ٲ�����ϴµ�
    }

}
