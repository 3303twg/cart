using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finish_line_spawner : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject finish_line;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {

        finish_line.SetActive(true);


        gameObject.SetActive(false);
        
        Debug.Log("√‚πﬂ");
    }



}
