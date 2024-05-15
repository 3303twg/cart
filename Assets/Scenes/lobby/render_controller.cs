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


    //�̰� ����?
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
            Debug.Log("�̸� : " + cart_name);
        }
        */

    }

    // Update is called once per frame
    void Update()
    {
        
    }



    //����ȭ�� ����
    void render_sync_car(string[] car_list)
    {
        my_index = int.Parse(Regex.Replace(gameObject.name, "[^0-9]", ""));

        Debug.Log("�� �ε����� (���԰�) = > " + my_index);
        //����ȭ�� �ϱ��Ұǵ� �������ʿ䰡 ���ݾ�?

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

        //��ũ��Ʈ ��Ȱ��ȭ
        car.GetComponent<car_controller>().enabled = false;






    }

    public void call_sync_car(string[] car_list)
    {
        //photonView.RPC("render_sync_car", RpcTarget.All, car_list);
        render_sync_car(car_list);
    }

    //car_index => ���� ��ü������ ���� �ε���
    //cart_name => ���� �ı��ؾ��� ���� �̸�



    //���� ������� ����
    void render_car(string[] car_list)
    {

        //������ ����ī����Ʈ�� �ް�
        //���� �ڽ��� �ε����� �ڵ����� �����������

        Debug.Log("������2");
        GameObject car;



        //Debug.Log("�ı��ؾ������� : " + cart_name);
        /*
        GameObject childobject = transform.Find("car").gameObject;
        Debug.Log("RPCȣ�����");
        Destroy(childobject);
        */
        Destroy(transform.Find("car").gameObject);
        Debug.Log(transform.Find("car").gameObject.name + "�� �����Ұ���");

        //�������� ������Ʈ ��ȯ
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
