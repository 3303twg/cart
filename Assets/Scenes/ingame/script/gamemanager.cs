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


    //������
    public Text[] ranking_text = new Text[4]; //�̰� �ؽ�Ʈ ������Ʈ�� �ǵ��ȴ�

    //�г��� ���� ������
    public string[] ranking_list = new string[4]; //�̰� �������� ��ŷ �ؽ�Ʈ�� �� �迭�� ������� �������
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




    //�׽�Ʈ��

    public GameObject my_car;















    //�ð� ����ȭ��
    [SerializeField]
    public float time = 0f;


    //Ÿ�̸� UI ����ȭ��
    [SerializeField]
    public Text timer;


    //������ 10�ʷ� �Ұ����� �׽�Ʈ������ 5��

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

        if (stream.IsWriting) // �����͸� ������ Ŭ���̾�Ʈ
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
        else // �����͸� �޴� Ŭ���̾�Ʈ
        {

            //Ŭ���̾�Ʈ ����
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

            //������ �ʱ�ȭ
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
            Debug.Log("�̸� : " + cart_name);
        }
        */

        //���� �ε����� īƮ ���� �ҷ�����

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




        //���ε� �غ�Ϸ� ȣ��
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
            //�۾� Ȱ��ȭ
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
                if (photonView != null && photonView.Owner != null && photonView.Owner.ActorNumber == i + 1) // 1���� ������
                {


                    Debug.Log("�׽�Ʈ2");
                    gamemanager script = playerObject.GetComponent<gamemanager>();
                    if (script != null)
                    {
                        Debug.Log(playerObject.name + "�� lap_cnt :" + script.lap_cnt);
                        lap_cnt_list_data[i] = script.lap_cnt;
                        line_checked_index_list[i] = script.line_checked_index;
                        next_line_distance = script.next_line_distance; ;
                    }

                    else
                    {
                        Debug.Log("��ã��");
                    }
                }
            }

        }
        */

        //������Ŭ���̾�Ʈ�� �ƴ϶��
        /*
        if (!PhotonNetwork.IsMasterClient)
        {
            //�ð� ����ȭ
            timer.text = time.ToString("F1");
            //������ ǥ��
            wheels.text = lap_cnt + "/" + max_lap;
            //���̿� ������ ��������
            return;
        }
        */




        if (start_flag == false)
        {
            if (PhotonNetwork.IsMasterClient == true)
            {
                // ��� �÷��̾��� �� �ε��� �Ϸ�Ǿ����� Ȯ��
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

                //���ֻ��°� �ƴ϶�� �ð��� ����
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


        //�̰� ���Ŭ���̾�Ʈ�� �Ҽ��־����
        //�ð�ǥ��
        timer.text = time.ToString("F1");
        retire_cnt.text = retire_timer.ToString("F0");

        //������ ǥ��
        wheels.text = lap_cnt + "/" + max_lap;



        //���ֽ�
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






        //�ǽð� �������� Ȯ�ο�
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



    //�Ÿ� ����
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




    //��������
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

                // key���� ���� ��Ҹ� ã���鼭 �̵�
                while (j >= 0 &&
                       (lap_cnt_list_data[j] < key ||
                       (lap_cnt_list_data[j] == key &&
                       (line_checked_index_list_data[j] < line_checked_index_list_temp ||
                       (line_checked_index_list_data[j] == line_checked_index_list_temp && next_line_distance_list_data[j] >= next_line_distance_temp)))))
                {
                    if (GameObject.Find("data_box").GetComponent<data_box>().active_slot_list[j] == 1)
                    {
                        lap_cnt_list_data[j + 1] = lap_cnt_list_data[j];
                        //�������� ���� ��ü
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


    //��� Ŭ���̾�Ʈ�� �ڵ��� ��ġ������ �޾ƿ������� �Լ�
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
