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

    //모든 카트의 오브젝트를 담을 변수
    public GameObject[] car_list;
    public GameObject car_slot_prefab;

    public string cart_name;

    bool sync_flag = false;

    public int players = 0;

    //리스트에 유저의 데이터가 몇개필요함
    //닉네임, 준비상태 등등

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
        if (stream.IsWriting) // 데이터를 보내는 클라이언트
        {
            // 데이터를 보냅니다.

            for (int i = 0; i < 4; i++)
            {
                stream.SendNext(text_list[i]);
            }
        }
        else // 데이터를 받는 클라이언트
        {
            // 데이터를 받고 업데이트합니다.
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


        //유저의 커스텀변수 설정
        ExitGames.Client.Photon.Hashtable user_custom_value = PhotonNetwork.LocalPlayer.CustomProperties;



        //ExitGames.Client.Photon.Hashtable user_custom_value = PhotonNetwork.LocalPlayer.CustomProperties;

        /*
        if (user_custom_value.TryGetValue("nickname", out object nickname_temp))
        {
            nick_name = (string)nickname_temp;
            Debug.Log("닉네임: " + nick_name);
        }

        else
        {
            Debug.Log("못찾음");
        }
        */


        for (int i = 0; i < 4; i++)
        {
            name_list[i].text = "";
        }


        nick_name = PhotonNetwork.LocalPlayer.NickName;


        //자신의 카트 렌더 켜기
        GameObject.Find("Canvas").transform.Find("lobby_slot").transform.Find
            ("slot_" + (PhotonNetwork.LocalPlayer.ActorNumber - 1)).transform.Find
            ("slot_" + (PhotonNetwork.LocalPlayer.ActorNumber - 1) + "_background").transform.Find
            ("slot_" + (PhotonNetwork.LocalPlayer.ActorNumber - 1) + "_render").gameObject.SetActive(true);



        //데이터박스가 있을경우
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
            {   //데이터박스가 있다는것은 게임종료후 로비에 재입장한 상태이므로
                //다수의 클라이언트가 요청을 보낼가능성이있어서 마스터 클라이언트만 동작해야함
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

            //신규입장 클라이언트일경우엔 렌더카트 동기화를 받아야하니까 조건없이 요청
            photonView.RPC("call_lobby_sync_car", RpcTarget.MasterClient, 
                PhotonNetwork.LocalPlayer.ActorNumber - 1, 
                slot_car_list[PhotonNetwork.LocalPlayer.ActorNumber - 1]);
        }
        

        //슬롯 리스트에서 자신의 활성화상태 등록
        //추후 동기화할때 사용예정
        


        

        
        //DontDestroyOnLoad(gameObject);

        if (!PhotonNetwork.IsMasterClient)
        {
            text_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = "준비안댐";
            slot_text[PhotonNetwork.LocalPlayer.ActorNumber - 1].text = "준비안댐";
            //slot_car_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = "car_0"; //기본차량
        }
        else
        {
            text_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = "방장";
            slot_text[PhotonNetwork.LocalPlayer.ActorNumber - 1].text = "방장";
            //slot_car_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = "car_0"; //기본차량
        }

        

        //레디상태 텍스트 동기화
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

        //방 활성상태를 결정하는곳
        if (PhotonNetwork.IsMasterClient)
        {
            room_flag = true;
            for (int i = 0; i < 4; i++)
            {
                //빈슬롯이 없을경우
                if (active_slot_list[i] == 1)
                {
                    //플래그 true상태 => 방 닫힘
                }

                //빈슬롯이 있을경우
                else
                {
                    //플래그 해제 => 방열림
                    room_flag = false;
                }
            }

            //플래그를 두개로 관리하는 이유
            // => 하나로만하면 가끔 빈방이 없는데도 검색이 되는 황당한 경우 방지

            if (room_flag == true)
            {
                room_open = false;
            }

            else
            {
                room_open = true;
            }


            // 룸 옵션 설정
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
                button.text = "준비완료";
            }

            else if (is_ready == true)
            {
                button.text = "준비해제하기";
            }


        }




        




    }




    public void push_button()
    {
        if (!PhotonNetwork.IsMasterClient)
        {

            if (is_ready == false)
            {
                text_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = "준비완료";
                is_ready = true;
            }
            else
            {
                text_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = "준비안댐";
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
                if (text_list[i] == "준비안댐")
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


        //데이터박스가 없을경우 생성해주기위함
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



        //룸 닫기(인게임 난입 방지)
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
            Debug.Log("로비싱크카  => " + slot_car_list[i]);
        }
    }


    [PunRPC]
    void call_lobby_sync_car(int index, string car_name)
    {
        slot_car_list[index] = car_name;

        photonView.RPC("lobby_sync_car", RpcTarget.All, slot_car_list);

        //타이밍문제 발생

        
        photonView.RPC("button_RPC", RpcTarget.All);

        //GameObject.Find("slot_renderer").transform.Find("slot_0").GetComponent<render_controller>().call_sync_car(slot_car_list);

        //여기서 i는 해당 슬롯의 인덱스값임 즉 자기가 누군지 구별할 매개변수
        /*
        for (int i = 0; i < 4; i++)
        {
            //i는 이제 안써도 되긴해 슬롯에서 알아서 자신의 인덱스를 파악하기로함
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

        //text_list[index] = "준비안댐";
        text_list[index] = text;
        player_list[index] = my_name;
        //slot_text[index].text = text;

        
        photonView.RPC("sync_value", RpcTarget.All,player_list, text_list, players, active_slot_list);

        
    }

    [PunRPC]
    void call_sync_value(int index, string my_name, string text, int player_cnt, int[] active_players)
    {

        //text_list[index] = "준비안댐";
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
            //Debug.Log("이름 : " + cart_name);
        }

        is_inventory = !is_inventory;
        GameObject.Find("Canvas").transform.Find("cart_room").gameObject.SetActive(is_inventory);

        if(is_inventory == true)
        {
            for( int i = 0; i < car_list.Length; i++)
            {
                //slot_pos = new Vector3(slot_x + i * 240, slot_y, slot_z);
                //임시로 이렇게하자
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
            //i는 이제 안써도 되긴해 슬롯에서 알아서 자신의 인덱스를 파악하기로함
            GameObject.Find("slot_renderer").transform.Find("slot_" + i).GetComponent<render_controller>().call_sync_car(slot_car_list);

        }

    }



    public void button_RPC_call()
    {
        //render_controller[] temp = new render_controller[4];
        //temp = render_slot_list;

        //버튼을 누른시점에서 slot_car_list가 변경되었으니
        //마스터클라이언트에게 동기화 요청
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
