using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.UIElements;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;
using UnityEngine.Windows;

public class render_controller : MonoBehaviourPun
{
    //public int index = 0;

    public Vector3 position = new Vector3(0, 0, 0);

    public Quaternion rotation_value = Quaternion.Euler(0, 180, 0);


    //이거 뭐임?
    public GameObject[] controller_list = new GameObject[4];

    public int my_index;
    //public string cart_name;
    void Start()
    {


        my_index = int.Parse(Regex.Replace(gameObject.name, "[^0-9]", ""));

        for (int i = 0; i < 4; i++)
        {
            controller_list[i] = GameObject.Find("slot_" + i);
        }

        /*
        ExitGames.Client.Photon.Hashtable user_custom_value = PhotonNetwork.LocalPlayer.CustomProperties;
        //new ExitGames.Client.Photon.Hashtable();

        if (user_custom_value.TryGetValue("cartbody", out object cart_body))
        {
            cart_name = (string)cart_body;
            Debug.Log("이름 : " + cart_name);
        }
        */

    }

    // Update is called once per frame
    void Update()
    {
        
    }



    //동기화용 렌더
    void render_sync_car(string[] car_list)
    {
        my_index = int.Parse(Regex.Replace(gameObject.name, "[^0-9]", ""));

        Debug.Log("내 인덱스값 (슬롯값) = > " + my_index);
        //동기화를 하긴할건데 나는할필요가 없잖아?

        GameObject car;

        string[] test = GameObject.Find("lobbymanager").GetComponent<lobby_manager>().slot_car_list;

        Destroy(transform.Find("car").gameObject);

        /*
        if (car_list[slot_index] == null)
        {
            transform.Find("car").gameObject.SetActive(false);
        }*/


        if (car_list[my_index] == null)
        {
            car = Instantiate(Resources.Load<GameObject>("car_0"), position, rotation_value);
        }
        else
        {
            car = Instantiate(Resources.Load<GameObject>(test[my_index]), position, rotation_value);
        }
        //car.name = "car_" + car_index;

        car.GetComponent<Rigidbody>().useGravity = false;
        car.GetComponent<Rigidbody>().isKinematic = true;

        car.transform.SetParent(transform);
        car.transform.localPosition = position;

        car.name = "car";

        //스크립트 비활성화
        car.GetComponent<car_controller>().enabled = false;






    }

    public void call_sync_car(string[] car_list)
    {
        //photonView.RPC("render_sync_car", RpcTarget.All, car_list);
        render_sync_car(car_list);
    }

    //car_index => 내가 교체예정인 차의 인덱스
    //cart_name => 내가 파괴해야할 차의 이름



    //차량 변경시의 렌더
    void render_car(string[] car_list)
    {

        //이제는 슬롯카리스트를 받고
        //그중 자신의 인덱스의 자동차로 변경해줘야함

        Debug.Log("렌더콜2");
        GameObject car;



        //Debug.Log("파괴해야할차량 : " + cart_name);
        /*
        GameObject childobject = transform.Find("car").gameObject;
        Debug.Log("RPC호출받음");
        Destroy(childobject);
        */
        Destroy(transform.Find("car").gameObject);
        Debug.Log(transform.Find("car").gameObject.name + "를 삭제할것임");

        //렌더링할 오브젝트 소환
        /*
        if (photonView.IsMine == true)
        {
            car = PhotonNetwork.Instantiate("car_" + car_index, position, rotation_value);
        }
        else
        {
            car = transform.Find("car_" + car_index).gameObject;
        }*/


        car = Instantiate(Resources.Load<GameObject>(car_list[my_index]), position, rotation_value, transform);
        //car.name = "car_" + car_index;

        car.GetComponent<Rigidbody>().useGravity = false;
        car.GetComponent<Rigidbody>().isKinematic = true;

        car.transform.SetParent(transform);
        car.transform.localPosition = position;
        car.name = "car";

    }


    public void render_RPC_call(string[] car_list)
    {
        //photonView.RPC("render_car", RpcTarget.All , car_list);

        render_car(car_list);
    }
}
