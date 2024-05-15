using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    public float spin_speed = 10f;

    public float turn_speed = 45f;
    void Update()

    {
        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + spin_speed * Time.deltaTime, transform.rotation.eulerAngles.y, 0);

        transform.Rotate(Vector3.up * turn_speed * Time.deltaTime);
        transform.Rotate(Vector3.right * spin_speed * Time.deltaTime);
        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);

       // transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x + spin_speed * Time.deltaTime, transform.rotation.eulerAngles.y, 0, Space.Self);
    }
}
