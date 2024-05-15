using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class data_box : MonoBehaviour
{

    public string[] slot_car_list;

    public string[] text_list;

    public int[] active_slot_list;

    public string[] player_list;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
