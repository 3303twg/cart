using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class gamemanager : MonoBehaviourPun, IPunObservable
{


    //순위용
    public Text[] ranking_text = new Text[4]; //이건 텍스트 오브젝트임 건들면안댐

    //닉네임 적을 공간임
    public string[] ranking_list = new string[4]; //이게 실질적인 랭킹 텍스트에 들어갈 배열임 순서대로 넣으면댐
    public string nick_name;

    public int[] lap_cnt_list_data = new int[4];

    public int[] line_checked_index_list = new int[4];
    public int line_checked_index = 0;

    public float[] next_line_distance_list = new float[4];
    public float next_line_distance;

    


    public int end_line_index = 0;

    public int lap_cnt = 0;
    public int max_lap = 3;

    public GameObject[] start_point = new GameObject[4]; 




    //테스트용

    public GameObject my_car;















    //시간 동기화용
    [SerializeField]
    public float time = 0f;


    //타이머 UI 동기화용
    [SerializeField]
    public Text timer;


    //실제론 10초로 할거지만 테스트용으론 5초

    public Text retire_cnt;
    public bool end_flag = false;

    public float retire_time = 5.9999f;
    public float retire_timer;
    public bool retire_flag = false;


    public Text wheels;

    //public GameObject car_prefab;
    public GameObject camera_prefab;

    public Vector3 position = new Vector3(-10, 5, 25);
    public Quaternion rotation_value = Quaternion.Euler(0, -90, 0);

    public string cart_name;

    public string[] slot_car_list;


    public bool next_scene_flag = false;



    public bool move_scene_flag = false;

    public bool start_flag = false;

    public Vector3 save_point;
    public Quaternion save_point_rotation;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting) // 데이터를 보내는 클라이언트
        {
            stream.SendNext(PhotonNetwork.LocalPlayer.ActorNumber);

            
            stream.SendNext(time);
            stream.SendNext(retire_timer);
            

            /*stream.SendNext(lap_cnt);
            stream.SendNext(line_checked_index);
            stream.SendNext(next_line_distance);*/

            /*
            stream.SendNext(lap_cnt_list_data);
            stream.SendNext(line_checked_index_list);
            stream.SendNext(next_line_distance_list);
            */
        }
        else // 데이터를 받는 클라이언트
        {

            //클라이언트 구분
            int clientID = (int)stream.ReceiveNext();

            
            time = (float)stream.ReceiveNext();
            retire_timer = (float)stream.ReceiveNext();
            


            /*lap_cnt_list_data[clientID -1] = (int)stream.ReceiveNext();
            line_checked_index_list[clientID -1] = (int)stream.ReceiveNext();
            next_line_distance_list[clientID -1] = (float)stream.ReceiveNext();*/

            /*
            lap_cnt_list_data = (int[])stream.ReceiveNext();
            line_checked_index_list = (int[])stream.ReceiveNext();
            next_line_distance_list = (float[])stream.ReceiveNext();
            */

        }
        
    }


    public GameObject[] playerObjects;


    public void SetSceneLoaded()
    {
        var player = PhotonNetwork.LocalPlayer;
        player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "SceneLoaded", true } });
    }



    void Start()
    {

        
        ranking_list = GameObject.Find("data_box").GetComponent<data_box>().player_list;

        nick_name = PhotonNetwork.LocalPlayer.NickName;

        for (int i = 0; i < 4; i++)
        {


            ranking_text[i].text = ranking_list[i];

            //바퀴수 초기화
            lap_cnt_list_data[i] = 0;
            line_checked_index_list[i] = 0;
            next_line_distance_list[i] = 0f;

        }

        end_line_index = GameObject.Find("drive_check").transform.Find("check_line_0").GetComponent<drive_check_line>().end_index;

        
        slot_car_list = GameObject.Find("data_box").GetComponent<data_box>().slot_car_list;
        cart_name = slot_car_list[PhotonNetwork.LocalPlayer.ActorNumber - 1];

        GameObject.Find("Canvas").transform.Find("rear_camera").transform.Find("rear_camera_background").transform.Find("render").GetComponent<RawImage>().texture = Resources.Load<RenderTexture>("slot_" + (PhotonNetwork.LocalPlayer.ActorNumber - 1));


        /*
        ExitGames.Client.Photon.Hashtable user_custom_value = PhotonNetwork.LocalPlayer.CustomProperties;
            //new ExitGames.Client.Photon.Hashtable();

        if (user_custom_value.TryGetValue("cartbody", out object cart_body))
        {
            cart_name = (string)cart_body;
            Debug.Log("이름 : " + cart_name);
        }
        */

        //나의 인덱스의 카트 정보 불러오기

        if (true)
        //if (photonView.IsMine)
        {
            position = start_point[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position;

            GameObject player_car = PhotonNetwork.Instantiate(cart_name, position, rotation_value);
            player_car.name = "player_car";

            GameObject player_camera = PhotonNetwork.Instantiate(camera_prefab.name, Vector3.zero, Quaternion.identity);
            player_camera.name = "player_camera";

            GameObject player_rear_camera = PhotonNetwork.Instantiate("rear_camera", Vector3.zero, Quaternion.identity);
            player_rear_camera.name = "player_rear_camera";


            //new_camera_prefab.name = camera_prefab.name;

            /*
            GameObject new_car_prefab =
            new_car_prefab.name = car_prefab.name;
            GameObject new_camera_prefab = Instantiate(camera_prefab);
            new_camera_prefab.name = camera_prefab.name;

            */

            my_car = player_car;


        }




        //씬로딩 준비완료 호출
        SetSceneLoaded();


    }

    public float wait_time = 5f;
    public Text start_timer_text;
    public bool allClientsLoaded = false;

    // Update is called once per frame


    [PunRPC]
    void start_call()
    {
        allClientsLoaded = true;
    }

    void start_count()
    {
        wait_time -= Time.deltaTime;

        start_timer_text.text = wait_time.ToString("F0");

        if (wait_time < 3.49)
        {
            //글씨 활성화
            start_timer_text.gameObject.SetActive(true);
        }
        if (wait_time < 0.51)
        {
            start_flag = true;
            start_timer_text.gameObject.SetActive(false);
        }
    }


    void Update()
    {
        //playerObjects = GameObject.FindGameObjectsWithTag("gamemanager");

        lap_cnt_list_data[PhotonNetwork.LocalPlayer.ActorNumber - 1] = lap_cnt;
        line_checked_index_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = line_checked_index;
        next_line_distance_list[PhotonNetwork.LocalPlayer.ActorNumber - 1] = next_line_distance;

        if (move_scene_flag == false)
        {
            photonView.RPC("call_sync_data", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber - 1, lap_cnt, line_checked_index, next_line_distance);

            next_line_distance = get_distance(my_car, line_checked_index - 1);
        }





        /*
        for (int i = 0; i < 4; i++)
        {
            foreach (GameObject playerObject in playerObjects)
            {
                PhotonView photonView = playerObject.GetComponent<PhotonView>();
                if (photonView != null && photonView.Owner != null && photonView.Owner.ActorNumber == i + 1) // 1부터 시작함
                {


                    Debug.Log("테스트2");
                    gamemanager script = playerObject.GetComponent<gamemanager>();
                    if (script != null)
                    {
                        Debug.Log(playerObject.name + "의 lap_cnt :" + script.lap_cnt);
                        lap_cnt_list_data[i] = script.lap_cnt;
                        line_checked_index_list[i] = script.line_checked_index;
                        next_line_distance = script.next_line_distance; ;
                    }

                    else
                    {
                        Debug.Log("못찾음");
                    }
                }
            }

        }
        */

        //마스터클라이언트가 아니라면
        /*
        if (!PhotonNetwork.IsMasterClient)
        {
            //시간 동기화
            timer.text = time.ToString("F1");
            //바퀴수 표시
            wheels.text = lap_cnt + "/" + max_lap;
            //그이외 동작은 하지않음
            return;
        }
        */




        if (start_flag == false)
        {
            if (PhotonNetwork.IsMasterClient == true)
            {
                // 모든 플레이어의 씬 로딩이 완료되었는지 확인
                bool allLoaded = true;
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    if (!player.CustomProperties.ContainsKey("SceneLoaded") || !(bool)player.CustomProperties["SceneLoaded"])
                    {
                        allLoaded = false;
                        break;
                    }
                }

                if (allLoaded && !allClientsLoaded)
                {
                    photonView.RPC("start_call", RpcTarget.All);
                } 
            }

            if (allClientsLoaded == true)
            {
                start_count();
            }
        }

        if (start_flag == true)
        {
            if (PhotonNetwork.IsMasterClient == true)
            {

                //완주상태가 아니라면 시간초 증가
                if (lap_cnt <= max_lap)
                {
                    time += Time.deltaTime;
                }

                if (retire_flag == true)
                {
                    retire_timer -= Time.deltaTime;

                    if (retire_timer < 0)
                    {
                        if (move_scene_flag == false)
                        {
                            move_scene_flag = true;


                            /*for(int i=0; i < 4; i++)
                            {
                                if (GameObject.Find("data_box").GetComponent<data_box>().active_slot_list[i] == 1)
                                {
                                    GameObject.Find(slot_car_list[i] + "(Clone)").GetComponent<car_controller>().photonView.TransferOwnership(PhotonNetwork.MasterClient.ActorNumber);
                                    GameObject.Find(slot_car_list[i] + "(Clone)").GetComponent<car_controller>().remove_object();


                                    GameObject.Find("camera(Clone)").SetActive(true);
                                    GameObject.Find("camera(Clone)").GetComponent<camera_controller>().remove_object();

                                    GameObject.Find("rear_camera(Clone)").SetActive(true);
                                    GameObject.Find("rear_camera(Clone)").GetComponent<rear_camera_controller>().remove_object();
                                }
                            }*/

                            /*
                            if (GameObject.Find("camera(Clone)"))
                            {
                                photonView.RPC("remove_object", RpcTarget.All);
                            }
                            */



                            //PhotonNetwork.LoadLevel("lobby");

                            photonView.RPC("end_game", RpcTarget.All);
                        }
                    }
                }
            }




        }


        //이건 모든클라이언트가 할수있어야함
        //시간표기
        timer.text = time.ToString("F1");
        retire_cnt.text = retire_timer.ToString("F0");

        //바퀴수 표시
        wheels.text = lap_cnt + "/" + max_lap;



        //완주시
        if (end_flag == false)
        {
            if (lap_cnt >= max_lap)
            {
                end_flag = true;
                start_retire();

            }
        }



        /*
        if (move_scene_flag == true)
        {
            next_scene_flag = true;

            for (int i = 0; i < 4; i++)
            {
                if (GameObject.Find("data_box").GetComponent<data_box>().active_slot_list[i] == 1)
                {
                    if (end_state_list[i] == 1)
                    {

                    }

                    else
                    {
                        next_scene_flag = false;
                    }
                }
            }

            if (next_scene_flag == true)
            {
                PhotonNetwork.LoadLevel("lobby");
            }
        }
        */






        //실시간 순위변동 확인용
        rank_check();



    }


    [PunRPC]
    void retire_count()
    {
        Destroy(GameObject.Find("lobbymanager"));
        retire_cnt.gameObject.SetActive(true);

        if (PhotonNetwork.IsMasterClient == true)
        {
            if (retire_flag == false)
            {
                retire_timer = retire_time;
                retire_flag = true;
            }
        }

    }

    public void start_retire()
    {
        
        photonView.RPC("retire_count", RpcTarget.All);
    }


    [PunRPC]
    void end_game()
    {
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer.ActorNumber);

        PhotonNetwork.LoadLevel("lobby");

    }



    public void test()
    {
        photonView.RPC("end_game", RpcTarget.All);
        /*
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer.ActorNumber);
        photonView.RPC("ttest", RpcTarget.All);
        */
    }

    [PunRPC]
    void ttest()
    {

        PhotonNetwork.LoadLevel("lobby");
    }




    void rank_check()
    {
        //
        InsertionSort(lap_cnt_list_data, ranking_list, line_checked_index_list, next_line_distance_list);



        /*
        for (int i = 0; i < 4; i++)
        {


            ranking_text[i].text = ranking_list[i];
        }
        */


            /*
            if(lap_cnt == other_car.GetComponent<PhotonView>().GetComponent<gamemanager>().lap_cnt)
            {
                if(line_checked_index == other_car.GetComponent<PhotonView>().GetComponent<gamemanager>().line_checked_index)
                {

                }

            }
            */




        }



    //거리 계산용
    float get_distance(GameObject player, int line_index)
    {

        GameObject line = GameObject.Find("drive_check").transform.Find("check_line_" + line_index).gameObject;
        if (line != null)
        {
            float distance = Vector3.Distance(player.transform.position, line.transform.position);
            return distance;
        }

        else
        {
            return 200;
        }
    }




    //삽입정렬
    void InsertionSort(int[] lap_cnt_list, string[] ranking_list, int[] line_checked_index_list, float[] next_line_distance_list)
    {
        int[] lap_cnt_list_data = (int[])lap_cnt_list.Clone();
        string[] ranking_list_data = (string[])this.ranking_list.Clone();
        int[] line_checked_index_list_data = (int[])this.line_checked_index_list.Clone();
        float[] next_line_distance_list_data = (float[])this.next_line_distance_list.Clone();

        int n = lap_cnt_list_data.Length;
        for (int i = 1; i < n; ++i)
        {
            if (GameObject.Find("data_box").GetComponent<data_box>().active_slot_list[i] == 1)
            {
                int key = lap_cnt_list_data[i];
                int j = i - 1;

                string ranking_list_temp = ranking_list_data[i];
                int line_checked_index_list_temp = line_checked_index_list_data[i];
                float next_line_distance_temp = next_line_distance_list_data[i];

                // key보다 작은 요소를 찾으면서 이동
                while (j >= 0 &&
                       (lap_cnt_list_data[j] < key ||
                       (lap_cnt_list_data[j] == key &&
                       (line_checked_index_list_data[j] < line_checked_index_list_temp ||
                       (line_checked_index_list_data[j] == line_checked_index_list_temp && next_line_distance_list_data[j] >= next_line_distance_temp)))))
                {
                    if (GameObject.Find("data_box").GetComponent<data_box>().active_slot_list[j] == 1)
                    {
                        lap_cnt_list_data[j + 1] = lap_cnt_list_data[j];
                        //실질적인 순위 교체
                        ranking_list_data[j + 1] = ranking_list_data[j];
                        line_checked_index_list_data[j + 1] = line_checked_index_list_data[j];
                        next_line_distance_list_data[j + 1] = next_line_distance_list_data[j];
                        j = j - 1;
                    }
                }
                lap_cnt_list_data[j + 1] = key;
                ranking_list_data[j + 1] = ranking_list_temp;
                line_checked_index_list_data[j + 1] = line_checked_index_list_temp;
                next_line_distance_list_data[j + 1] = next_line_distance_temp;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            ranking_text[i].text = ranking_list_data[i];
        }
    }


    //모든 클라이언트의 자동차 위치정보를 받아오기위한 함수
    void collect_value()
    {

        /*
            public string nick_name;

    public int[] lap_cnt_list_data = new int[4];

    public int[] line_checked_index_list = new int[4];
    public int line_checked_index = 0;

    public float[] next_line_distance_list = new float[4];
    public float next_line_distance;


        */


        /*
        for (int i = 0; i < 4; i++)
        {
            if (GameObject.Find("data_box").GetComponent<data_box>().active_slot_list[i] == 1)
            {
                PhotonNetwork.LocalPlayer.NickName
                ranking_list[i]
            }
        }
        */
    }





    [PunRPC]
    void sync_data(int player_index, int lap_cnt_data, int line_checked_index_data, float next_line_distance_data)
    {
        lap_cnt_list_data[player_index] = lap_cnt_data;
        line_checked_index_list[player_index] = line_checked_index_data;
        next_line_distance_list[player_index] = next_line_distance_data;


    }


    [PunRPC]
    void call_sync_data(int player_index, int lap_cnt_data, int line_checked_index_data, float next_line_distance_data)
    {

        photonView.RPC("sync_data", RpcTarget.All, player_index , lap_cnt_data, line_checked_index_data, next_line_distance_data);
    }
}
