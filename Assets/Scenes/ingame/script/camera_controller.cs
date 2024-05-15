using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class camera_controller : MonoBehaviourPun
{

    public GameObject target;

    public float offset_rotate = 45f;
    public float camera_rotate = 45f;
    public float min_camera_rotate = 20f;

    //부스터 기준
    public float booster_camera_rotate = 15f;

    public float offset_x = 4f;
    public float distance_x = 4f;
    public float max_distance_x = 7f;

    //부스터 기준
    public float booster_distance_x = 10f;

    public float offset_y = 7f;
    public float distance_y = 7f;
    public float min_distance_y = 3.5f;

    //부스터 기준
    public float booster_distance_y = 3f;

    public float speed_ratio = 0;

    public float move_time = 2f;
    private float elapsed_time = 0f;

    public string cart_name;
    public float booster_ratio;

    public string[] slot_car_list;

    void Start()
    {
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }

        /*
         * 
        ExitGames.Client.Photon.Hashtable user_custom_value = PhotonNetwork.LocalPlayer.CustomProperties;
        //new ExitGames.Client.Photon.Hashtable();

        
        if (user_custom_value.TryGetValue("cartbody", out object cart_body))S
        {
            cart_name = (string)cart_body;
            Debug.Log("이름 : " + cart_name);
        }*/
        slot_car_list = GameObject.Find("data_box").GetComponent<data_box>().slot_car_list;
        cart_name = slot_car_list[PhotonNetwork.LocalPlayer.ActorNumber - 1];


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        target = GameObject.Find("player_car");
        booster_ratio = target.GetComponent<car_controller>().booster_speed_ratio / 2;

        Vector3 target_vector = target.transform.forward * -1;
        speed_ratio = target.GetComponent<car_controller>().move_speed / target.GetComponent<car_controller>().max_speed;
        /*
        // 경과 시간 업데이트
        elapsed_time += Time.deltaTime;

        // 보간 계수(0부터 1까지)
        float t = Mathf.Clamp01(elapsed_time / move_time);

        // 선형 보간을 사용하여 현재 위치를 목표 위치로 이동
        transform.position = Vector3.Lerp(transform.position, transform.position = target.transform.position + target_vector * (max_distance_x - offset_x) * speed_ratio, t);
        // 목표 위치에 도달하면 경과 시간 초기화
        if (t >= 1f)
        {
            elapsed_time = 0f;
            transform.position = transform.position;
        }
        */








        transform.position = target.transform.position + target_vector * (offset_x + (max_distance_x - offset_x) * speed_ratio * booster_ratio);
        transform.position += Vector3.up * (offset_y - (offset_y - min_distance_y) * speed_ratio * booster_ratio);
        transform.rotation = Quaternion.Euler(offset_rotate - (offset_rotate - min_camera_rotate) * speed_ratio * booster_ratio, target.transform.rotation.eulerAngles.y, 0);
        //transform.position = new Vector3(target.transform.position.x + offset_x, target.transform.position.y + offset_y, target.transform.position.z + offset_z);
    }


    public void remove_object()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

}
