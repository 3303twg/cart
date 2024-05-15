using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class lobby_manager : MonoBehaviourPun, IPunObservable
{

    public Text button;

    public bool is_ready = false;


    public Text[] name_list = new Text[4];


    public Text[] slot_text = new Text[4];

    [SerializeField]
    public string[] text_list = new string[4];

    public string[] slot_car_list = {"car_0", "car_0", "car_0", "car_0"};
    public bool ready_flag = false;

    public int[] active_slot_list = new int[4];

    public bool is_inventory = false;

    //��� īƮ�� ������Ʈ�� ���� ����
    public GameObject[] car_list;
    public GameObject car_slot_prefab;

    public string cart_name;

    bool sync_flag = false;

    public int players = 0;

    //����Ʈ�� ������ �����Ͱ� ��ʿ���
    //�г���, �غ���� ���

    private int original_view_ID;

    public GameObject data_box;


    public string[] player_list = new string[4];
    public string nick_name;


    void player_add()
    {
        
    }

    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        /*
        if (stream.IsWriting) // �����͸� ������ Ŭ���̾�Ʈ
        {
            // �����͸� �����ϴ�.

            for (int i = 0; i < 4; i++)
            {
                stream.SendNext(text_list[i]);
            }
        }
        else // �����͸� �޴� Ŭ���̾�Ʈ
        {
            // �����͸� �ް� ������Ʈ�մϴ�.
            for (int i = 0; i < 4; i++)
            {
                text_list[i] = (string)stream.ReceiveNext();
            }
        }
        */
    }
    

    private void Start()
    {

        //transform.name = "lobbymanager(new)";

        /*
        if (GameObject.Find("lobbymanager(Clone)"))
        {
            Destroy(gameObject);
        }
        */

        /*
        PhotonView photonView = GetComponent<PhotonView>();

        original_view_ID = photonView.ViewID;

        if(original_view_ID != photonView.ViewID)
        {
            Destroy(gameObject);
        }
        for (int i = 0; i < 4; i++)
        {
            active_slot_list[i] = 0;
        }
        */


        //������ Ŀ���Һ��� ����
        ExitGames.Client.Photon.Hashtable user_custom_value = PhotonNetwork.LocalPlayer.CustomProperties;



        //ExitGames.Client.Photon.Hashtable user_custom_value = PhotonNetwork.LocalPlayer.CustomProperties;

        /*
        if (user_custom_value.TryGetValue("nickname", out object nickname_temp))
        {
            nick_name = (string)nickname_temp;
            Debug.Log("�г���: " + nick_name);
        }

        else
        {
            Debug.Log("��ã��");
        }
        */


        for (int i = 0; i < 4; i++)
        {
            name_list[i].text = "";
        }


        nick_name = PhotonNetwork.LocalPlayer.NickName;


        //�ڽ��� īƮ ���� �ѱ�
        GameObject.Find("Canvas").transform.Find("lobby_slot").transform.Find
            ("slot_" + (PhotonNetwork.LocalPlayer.ActorNumber - 1)).transform.Find
            ("slot_" + (PhotonNetwork.LocalPlayer.ActorNumber - 1) + "_background").transform.Find
            ("slot_" + (PhotonNetwork.LocalPlayer.ActorNumber - 1) + "_render").gameObject.SetActive(true);



        //�����͹ڽ��� �������
        if (GameObject.Find("data_box") != null)
        {
            for (int i = 0; i < 4; i++)
            {
                slot_car_list[i] = GameObject.Find("data_box").GetComponent<data_box>().slot_car_list[i];
                text_list[i] = GameObject.Find("data_box").GetComponent<data_box>().text_list[i];
                active_slot_list[i] = GameObject.Find("data_box").GetComponent<data_box>().active_slot_list[i];
                player_list[i] = GameObject.Find("data_box").GetComponent<data_box>().player_list[i];
            }

            user_custom_value["cartbody"] = slot_car_list[PhotonNetwork.LocalPlayer.ActorNumber - 1];
            PhotonNetwork.LocalPlayer.SetCustomProperties(user_custom_value);
            
            if (PhotonNetwork.IsMasterClient)
            {   //�����͹ڽ��� �ִٴ°��� ���������� �κ� �������� �����̹Ƿ�
                //�ټ��� Ŭ���̾�Ʈ�� ��û�� �������ɼ����־ ������ Ŭ���̾�Ʈ�� �����ؾ���
                photonView.RPC("call_lobby_sync_car", RpcTarget.MasterClient, 
                    PhotonNetwork.LocalPlayer.ActorNumber - 1, 
                    slot_car_list[PhotonNetwork.LocalPlayer.ActorNumber - 1]);
            }

        }

        else
        {
            for (int i = 0; i < 4; i++)
            {
                slot_car_list[i] = "car_0";
            }

            active_slot_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = 1;
            user_custom_value["cartbody"] = "car_0";
            PhotonNetwork.LocalPlayer.SetCustomProperties(user_custom_value);

            player_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = nick_name;

            //�ű����� Ŭ���̾�Ʈ�ϰ�쿣 ����īƮ ����ȭ�� �޾ƾ��ϴϱ� ���Ǿ��� ��û
            photonView.RPC("call_lobby_sync_car", RpcTarget.MasterClient, 
                PhotonNetwork.LocalPlayer.ActorNumber - 1, 
                slot_car_list[PhotonNetwork.LocalPlayer.ActorNumber - 1]);
        }
        

        //���� ����Ʈ���� �ڽ��� Ȱ��ȭ���� ���
        //���� ����ȭ�Ҷ� ��뿹��
        


        

        
        //DontDestroyOnLoad(gameObject);

        if (!PhotonNetwork.IsMasterClient)
        {
            text_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = "�غ�ȴ�";
            slot_text[PhotonNetwork.LocalPlayer.ActorNumber - 1].text = "�غ�ȴ�";
            //slot_car_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = "car_0"; //�⺻����
        }
        else
        {
            text_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = "����";
            slot_text[PhotonNetwork.LocalPlayer.ActorNumber - 1].text = "����";
            //slot_car_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = "car_0"; //�⺻����
        }

        

        //������� �ؽ�Ʈ ����ȭ
        photonView.RPC("call_sync_value", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber - 1, nick_name ,text_list[PhotonNetwork.LocalPlayer.ActorNumber - 1], players, active_slot_list);

        //Destroy(GameObject.Find("data_box"));

    }


    public bool room_open = false;

    private bool room_flag = false;
    void sync_text()
    {

        if (sync_flag == true)
        {
            for (int i = 0; i < players; i++)
            {
                slot_text[i].text = text_list[i];
                name_list[i].text = player_list[i];
            }

            sync_flag = false;
        }
    }
    // Update is called once per frame
    void Update()
    {

        //�� Ȱ�����¸� �����ϴ°�
        if (PhotonNetwork.IsMasterClient)
        {
            room_flag = true;
            for (int i = 0; i < 4; i++)
            {
                //�󽽷��� �������
                if (active_slot_list[i] == 1)
                {
                    //�÷��� true���� => �� ����
                }

                //�󽽷��� �������
                else
                {
                    //�÷��� ���� => �濭��
                    room_flag = false;
                }
            }

            //�÷��׸� �ΰ��� �����ϴ� ����
            // => �ϳ��θ��ϸ� ���� ����� ���µ��� �˻��� �Ǵ� Ȳ���� ��� ����

            if (room_flag == true)
            {
                room_open = false;
            }

            else
            {
                room_open = true;
            }


            // �� �ɼ� ����
            if (room_open == false)
            {
                Room room_option = PhotonNetwork.CurrentRoom;

                room_option.IsOpen = false;
                PhotonNetwork.CurrentRoom.SetCustomProperties(room_option.CustomProperties);
            }

            else if (room_open == true)
            {
                Room room_option = PhotonNetwork.CurrentRoom;

                room_option.IsOpen = true;
                PhotonNetwork.CurrentRoom.SetCustomProperties(room_option.CustomProperties);
            }
        }


        //sync_text();


        if (!PhotonNetwork.IsMasterClient)
        {
            if (is_ready == false)
            {
                button.text = "�غ�Ϸ�";
            }

            else if (is_ready == true)
            {
                button.text = "�غ������ϱ�";
            }


        }




        




    }




    public void push_button()
    {
        if (!PhotonNetwork.IsMasterClient)
        {

            if (is_ready == false)
            {
                text_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = "�غ�Ϸ�";
                is_ready = true;
            }
            else
            {
                text_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = "�غ�ȴ�";
                is_ready = false;
            }


            sync_text();
            photonView.RPC("call_sync_value", RpcTarget.MasterClient, 
                PhotonNetwork.LocalPlayer.ActorNumber - 1, nick_name, 
                text_list[PhotonNetwork.LocalPlayer.ActorNumber - 1], players);

        }

        else
        {
            
            ready_flag = false;
            for (int i = 0; i < PhotonNetwork.CountOfPlayers - 1; i++)
            {
                /*
                if (text_list[i] == "�غ�ȴ�")
                {
                    ready_flag = true;
                }
                */
            }

            if (ready_flag == false)
            {
                photonView.RPC("start_game", RpcTarget.All);
            }

            
        }
    }


    [PunRPC]
    void start_game()
    {


        //�����͹ڽ��� ������� �������ֱ�����
        if (!GameObject.Find("data_box"))
        {
            Instantiate(data_box).name = "data_box";
        }
        else
        {
            //GameObject.Find("data_box").GetComponent<data_box>().destroy_list.Clear();
        }
        
        GameObject.Find("data_box").GetComponent<data_box>().slot_car_list = slot_car_list;
        GameObject.Find("data_box").GetComponent<data_box>().text_list = text_list;
        GameObject.Find("data_box").GetComponent<data_box>().active_slot_list = active_slot_list;
        GameObject.Find("data_box").GetComponent<data_box>().player_list = player_list;



        //�� �ݱ�(�ΰ��� ���� ����)
        Room room_option = PhotonNetwork.CurrentRoom;

        room_option.IsOpen = false;
        PhotonNetwork.CurrentRoom.SetCustomProperties(room_option.CustomProperties);


        PhotonNetwork.LoadLevel("Main");

    }




    
    [PunRPC]
    void lobby_sync_car(string[] car)
    {
        slot_car_list = car;

        for (int i = 0; i < 4; i++)
        {
            Debug.Log("�κ��ũī  => " + slot_car_list[i]);
        }
    }


    [PunRPC]
    void call_lobby_sync_car(int index, string car_name)
    {
        slot_car_list[index] = car_name;

        photonView.RPC("lobby_sync_car", RpcTarget.All, slot_car_list);

        //Ÿ�ֹ̹��� �߻�

        
        photonView.RPC("button_RPC", RpcTarget.All);

        //GameObject.Find("slot_renderer").transform.Find("slot_0").GetComponent<render_controller>().call_sync_car(slot_car_list);

        //���⼭ i�� �ش� ������ �ε������� �� �ڱⰡ ������ ������ �Ű�����
        /*
        for (int i = 0; i < 4; i++)
        {
            //i�� ���� �Ƚᵵ �Ǳ��� ���Կ��� �˾Ƽ� �ڽ��� �ε����� �ľ��ϱ����
            GameObject.Find("slot_renderer").transform.Find("slot_" + i).GetComponent<render_controller>().call_sync_car(slot_car_list);
            
        }
        */



    }

    [PunRPC]
    void sync_value(string[] name_list, string[] text, int player_cnt, int[] active_players)
    {
        player_list = name_list;
        text_list = text;
        //slot_text = text;
        sync_flag = true;

        players = player_cnt;

        active_slot_list = active_players;

        for(int i = 0; i < 4; i++)
        {
            if(active_slot_list[i] == 1)
            {
                GameObject.Find("Canvas").transform.Find("lobby_slot").transform.Find
                    ("slot_" + i).transform.Find
                    ("slot_" + i + "_background").transform.Find
                    ("slot_" + i + "_render").gameObject.SetActive(true);
            }
        }

        sync_text();

    }
    


    
    [PunRPC]
    void call_sync_value(int index, string my_name, string text, int player_cnt)
    {

        //text_list[index] = "�غ�ȴ�";
        text_list[index] = text;
        player_list[index] = my_name;
        //slot_text[index].text = text;

        
        photonView.RPC("sync_value", RpcTarget.All,player_list, text_list, players, active_slot_list);

        
    }

    [PunRPC]
    void call_sync_value(int index, string my_name, string text, int player_cnt, int[] active_players)
    {

        //text_list[index] = "�غ�ȴ�";
        text_list[index] = text;
        player_list[index] = my_name;
        //slot_text[index].text = text;

        active_slot_list[index] = 1;


        players += 1;
        photonView.RPC("sync_value", RpcTarget.All, player_list, text_list, players , active_slot_list);


    }




    public void inventory()
    {

        ExitGames.Client.Photon.Hashtable user_custom_value = PhotonNetwork.LocalPlayer.CustomProperties;
        //new ExitGames.Client.Photon.Hashtable();

        if (user_custom_value.TryGetValue("cartbody", out object cart_body))
        {
            cart_name = (string)cart_body;
            //Debug.Log("�̸� : " + cart_name);
        }

        is_inventory = !is_inventory;
        GameObject.Find("Canvas").transform.Find("cart_room").gameObject.SetActive(is_inventory);

        if(is_inventory == true)
        {
            for( int i = 0; i < car_list.Length; i++)
            {
                //slot_pos = new Vector3(slot_x + i * 240, slot_y, slot_z);
                //�ӽ÷� �̷�������
                //slot_pos = new Vector3(slot_x + 600 + i * 240 , slot_y + 250, 0);

                GameObject cart_slot = Instantiate(car_slot_prefab, GameObject.Find("Canvas").transform.Find("cart_room").transform);

                cart_slot.transform.Find("Image").transform.Find("text").GetComponent<Text>().text = cart_name;

                cart_slot.GetComponent<RectTransform>().anchoredPosition = new Vector3(-400 + 250 * i, 0, 0);
                cart_slot.name = "cart_slot_" + i;

                cart_slot.GetComponent<select_button_controller>().car_index = i;
                cart_slot.GetComponent<select_button_controller>().my_index = PhotonNetwork.LocalPlayer.ActorNumber - 1;
                //cart_slot.GetComponent<select_button_controller>().cart_name = cart_name;

            }
        }

        else
        {
            GameObject.Find("Canvas").transform.Find("cart_room").gameObject.SetActive(true);
            for (int i = 0; i < car_list.Length; i++)
            {
                Destroy(GameObject.Find("Canvas").transform.Find("cart_room").transform.Find("cart_slot_" + i).gameObject);
                
            }
            GameObject.Find("Canvas").transform.Find("cart_room").gameObject.SetActive(false);

        }

    }


    [PunRPC]
    void button_RPC()
    {
        /*
        render_controller[] temp = new render_controller[4];


        for (int i = 0; i < 4; i++)
        {
            temp[i] = GameObject.Find("slot_renderer").transform.Find("slot_" + i).GetComponent<render_controller>();
        }


        for (int i = 0; i < temp.Length; i++)
        {
            temp[i].render_RPC_call(slot_car_list);
        }
        */

        for (int i = 0; i < 4; i++)
        {
            //i�� ���� �Ƚᵵ �Ǳ��� ���Կ��� �˾Ƽ� �ڽ��� �ε����� �ľ��ϱ����
            GameObject.Find("slot_renderer").transform.Find("slot_" + i).GetComponent<render_controller>().call_sync_car(slot_car_list);

        }

    }



    public void button_RPC_call()
    {
        //render_controller[] temp = new render_controller[4];
        //temp = render_slot_list;

        //��ư�� ������������ slot_car_list�� ����Ǿ�����
        //������Ŭ���̾�Ʈ���� ����ȭ ��û
        photonView.RPC("call_lobby_sync_car", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber - 1, slot_car_list[PhotonNetwork.LocalPlayer.ActorNumber - 1]);

        //photonView.RPC("button_RPC",RpcTarget.All);
    }

    /*
    public void select_car()
    {
        ExitGames.Client.Photon.Hashtable user_custom_value = PhotonNetwork.LocalPlayer.CustomProperties;

        user_custom_value["cartbody"] = "car_1";
        PhotonNetwork.LocalPlayer.SetCustomProperties(user_custom_value);
    }
    */

}
