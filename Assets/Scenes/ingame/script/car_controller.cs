using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class car_controller : MonoBehaviourPun
{
    public float move_speed = 0f;
    public float max_speed_offset = 30f;
    public float max_speed = 30f;

    //���ӷ��� �����ϴ� ����
    public float acceleration = 13f;
    //private float acceleration = 1.0f;
    public float drift_deceleration = 20f;
    public float drift_acceleration = 55f;

    
    public float drift_deceleration_scale = 0.55f;

    


    public bool is_booster = false;
    public float booster_gauge = 0f;
    public float max_booster_gauge = 20f;
    public float booster_acceleration = 10f;
    public float booster_deceleration = 30f;
    public float booster_scale = 1.3f;

    public float booster_speed_ratio = 1f;

    public float drift_speed_offset = 2f;
    public float drift_speed = 0f;
    public float max_drift_speed = 20f;

    public bool is_drift = false;
    private bool drift_delay = false;
    private bool drift_button_on = false;

    public float drift_angle = 0f;
    public float drift_max_angle = 20f;

    public float time_scale = 1f;
    

    public GameObject[] front_wheel_pivot = new GameObject[2];

    public GameObject[] front_wheel = new GameObject[2];
    public GameObject[] rear_wheel = new GameObject[2];

    GameObject car_frame;

    //���� ȸ���ӵ��� �����ϴ� ����
    public float wheel_rotation_speed = 150f;

    public float wheel_rotation_angle = 0f;
    public float car_rotation_angle = 0f;
    public float car_rotation_speed = 0f;

    public float max_angle = 40f;

    private string last_direction;

    public GameObject gamemanager;


    public bool controller_flag = false;

    void Awake()
    {

        gamemanager = GameObject.Find("gamemanager");
        front_wheel_pivot[0] = transform.Find("empty").transform.Find("Front_wheel_left_pivot").gameObject;
        front_wheel_pivot[1] = transform.Find("empty").transform.Find("Front_wheel_right_pivot").gameObject;


        front_wheel[0] = front_wheel_pivot[0].transform.Find("Front_wheel_left").gameObject;
        front_wheel[1] = front_wheel_pivot[1].transform.Find("Front_wheel_right").gameObject;
        //�޹����� ȸ������ ����
        rear_wheel[0] = transform.Find("empty").transform.Find("Rear_wheel_left").gameObject;
        rear_wheel[1] = transform.Find("empty").transform.Find("Rear_wheel_right").gameObject;


        max_speed = max_speed_offset;

        Time.timeScale = 1f;

        car_frame = transform.Find("empty").gameObject;
    }



    private void FixedUpdate()
    {
        
        //�̵�
        transform.Translate(Vector3.forward * move_speed * Time.deltaTime);

        front_wheel_pivot[1].transform.rotation = front_wheel_pivot[0].transform.rotation;
        wheel_spin();

        
        if (is_drift == true)
        {
            drift(last_direction);
        }



        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                turn("left");

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (is_drift != true)
                    {
                        is_drift = true;

                        last_direction = "left";
                        drift_button_on = true;
                        //�̰� �ѹ��� �۵��ϴ°��� �����ڸ��� �̲������°� ����
                        drift_speed += drift_speed_offset * Time.deltaTime;
                    }
                }
            }

            else if (Input.GetKey(KeyCode.RightArrow))
            {
                turn("right");

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (is_drift != true)
                    {
                        last_direction = "right";
                        is_drift = true;
                        drift_button_on = true;
                        //�̰� �ѹ��� �۵��ϴ°��� �����ڸ��� �̲������°� ����
                        drift_speed += drift_speed_offset * Time.deltaTime;
                    }
                }
            }


            else if (Input.GetKey(KeyCode.UpArrow))
            {
                restore_wheel();
            }

        }



        //��üȸ��
        if (move_speed > 0)
        {
            transform.Rotate(Vector3.up, car_rotation_speed * Time.deltaTime);
        }

        float drift_direction = 0;
        if (is_drift == true)
        {
            if (last_direction == "left")
            {
                drift_direction = -1;
            }

            else if (last_direction == "right")
            {
                drift_direction = 1;
            }
            drift_angle = drift_direction * drift_max_angle * (drift_speed / max_drift_speed);
            
            //�帮��Ʈ�� �ڵ��� �߰�ȸ��
            transform.Find("empty").gameObject.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 
                transform.rotation.eulerAngles.y + drift_angle, transform.rotation.eulerAngles.z);

            //transform.Rotate(Vector3.left, car_rotation_speed * Time.deltaTime);
        }

    }


    void Update()
    {
        if (!photonView.IsMine)
        {
            GetComponent<car_controller>().enabled = false;
            return;
        }

        controller_flag = gamemanager.GetComponent<gamemanager>().start_flag;




        wheel_rotation_angle = front_wheel_pivot[0].transform.rotation.eulerAngles.y;
        car_rotation_angle = car_frame.transform.rotation.eulerAngles.y;

        Time.timeScale = time_scale;


        
        //��Ű ������ ����
        if (Input.anyKey)
        {

            //����
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (controller_flag == true)
                {

                    booster();


                    //�ν��� ���� �ִ�ӵ� ������ ���� ����
                    if (is_booster == true)
                    {
                        //max_speed = max_speed_offset * booster_scale;

                        move_speed += booster_acceleration * Time.deltaTime;
                        if (move_speed >= max_speed)
                        {
                            move_speed = max_speed;
                        }
                    }


                    //��������
                    move_speed += acceleration * Time.deltaTime;

                    //�帮��Ʈ��
                    if (is_drift == true)
                    {
                        if (move_speed > max_speed * drift_deceleration_scale)
                        {
                            move_speed -= drift_deceleration * Time.deltaTime;
                        }

                    }
                    //�ӵ� �ִ� ����
                    if (move_speed >= max_speed)
                    {
                        move_speed = max_speed;
                    }




                    if (Input.GetKey(KeyCode.LeftControl))
                    {

                        if (is_booster == false)
                        {

                            //�ν��� ��� �÷��� ��� �ۼ��Ѱ� �����ؾ��� ���ɼ� ����
                            if (max_speed <= max_speed_offset)
                            {
                                booster_gauge = max_booster_gauge;
                                is_booster = true;
                            }
                        }

                    }
                }

            }
            //����
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                if (controller_flag == true)
                {
                    if (move_speed > 0)
                    {
                        move_speed -= acceleration * 2.0f * Time.deltaTime;
                    }
                    else
                    {
                        move_speed -= acceleration * 0.6f * Time.deltaTime;
                    }

                    
                    if(move_speed <=  max_speed_offset * -0.4f)
                    {
                        move_speed = max_speed_offset * -0.4f;
                    }
                    
                }
            }


            //�̻纸������ ����
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                turn("left");
            }

            else if (Input.GetKey(KeyCode.RightArrow))
            {
                turn("right");
            }

            else if (Input.GetKey(KeyCode.R))
            {
                if (controller_flag == true)
                {
                    //transform.position = new Vector3(0, 2, 16);
                    move_speed = 0f;
                    transform.position = GameObject.Find("gamemanager").GetComponent<gamemanager>().save_point;
                    transform.rotation = GameObject.Find("gamemanager").GetComponent<gamemanager>().save_point_rotation;

                    //transform.rotation = Quaternion.Euler(0, -90, 0);

                }
            }



        }
        else
        {   //�ӵ� �ּڰ� ����
            if (move_speed > 0)
            {
                move_speed -= acceleration * Time.deltaTime;
                if(move_speed < 0)
                {
                    move_speed = 0f;
                }
            }
            else  if (move_speed <= 0)
            {
                move_speed += acceleration * Time.deltaTime;
                if (move_speed > 0)
                {
                    move_speed = 0f;
                }
            }

            else
            {
                move_speed = 0;
            }

            restore_wheel();
        }

        

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            
            drift_delay = true;
            drift_button_on = false;
        }

        if (drift_delay == true)
        {
            //0.75�� �帮��Ʈ ������ �帮��Ʈ������ �������� �������ϴµ� ������ 0.75������ ������ ���ӽ�Ų�ٴ¶�
            drift_speed -= drift_acceleration * 0.75f * Time.deltaTime;
            if (drift_speed <= 0)
            {
                drift_speed = 0;
                is_drift = false;
                drift_delay = false;
            }
        }


        rotation_speed();
        



       
    }

    void drift(string direction)
    {
        if (drift_button_on == true)
        {
            if (drift_speed <= max_drift_speed)
            {
                drift_speed += drift_acceleration * Time.deltaTime;
                if (drift_speed >= max_drift_speed)
                {
                    drift_speed = max_drift_speed;
                }
            }
        }

        //�帮��Ʈ�� ȸ������ �ݴ�������� �з������� ���� ����ȸ���� ���������� ���ư�����
        if (direction == "left")
        {
            transform.Translate(Vector3.right * drift_speed * Time.deltaTime);
        }

        else if (direction == "right")
        {
            transform.Translate(Vector3.left * drift_speed * Time.deltaTime);
        }

    }


    public float temp = 0;
    int direction_flag = 0;

    //ȸ���ε� ���߿� ���� ���� �������ֱ��ؾ���
    void turn(string direction)
    {
        direction_flag = 0;
        //���� 315�̻�
        /*
        if (car_frame.transform.rotation.eulerAngles.y > 315 && car_frame.transform.rotation.eulerAngles.y < 360)
        {
            Debug.Log("1-1");
            if (front_wheel_pivot[0].transform.rotation.eulerAngles.y > 270 || front_wheel_pivot[0].transform.rotation.eulerAngles.y < 90)
            {
                rotation_lock = false;
                Debug.Log("1");
            }

            else
            {
                rotation_lock = true;
            }
        }


        //���� 0�̻� 45�̸�
        else if (car_frame.transform.rotation.eulerAngles.y < 45 && car_frame.transform.rotation.eulerAngles.y >= 0)
        {
            Debug.Log("2-2");
            if (front_wheel_pivot[0].transform.rotation.eulerAngles.y >315 || front_wheel_pivot[0].transform.rotation.eulerAngles.y < 90)
            {
                rotation_lock = false;
                Debug.Log("2");
            }
            else
            {
                rotation_lock = true;
            }
        }

        //���� 45�̻� 315�̸�
        else if (car_frame.transform.rotation.eulerAngles.y >= 45 && car_frame.transform.rotation.eulerAngles.y < 315)
        {
            Debug.Log("3-3");
            rotation_lock = false;
            Debug.Log("3");
        }

        else
        {
            rotation_lock = true;
            Debug.Log("4");
        }
        */

        //���Ŵ� ���ǹ�
        //if (front_wheel_pivot[0].transform.rotation.eulerAngles.y < car_frame.transform.rotation.eulerAngles.y + 40 && front_wheel_pivot[0].transform.rotation.eulerAngles.y > car_frame.transform.rotation.eulerAngles.y - 40)

        //Ʈ��� ���� ���ɸ����� ����
        if (true)
        {

            /*
            if (direction == "left")
            {
                direction_flag = -1;
                //���� ������ -45���϶��� �����ϸ� �۵���
                if (car_frame.transform.rotation.eulerAngles.y > 45 && car_frame.transform.rotation.eulerAngles.y <= 315)
                {
                    if (front_wheel_pivot[0].transform.rotation.eulerAngles.y > (car_frame.transform.rotation.eulerAngles.y - 45 + 360) % 360 && front_wheel_pivot[0].transform.rotation.eulerAngles.y < (car_frame.transform.rotation.eulerAngles.y + 45 + 360) % 360)
                    {

                        //������Ʈ�� ȸ����Ű�� �Լ�
                        front_wheel_pivot[0].transform.Rotate(Vector3.up * -wheel_rotation_speed * Time.deltaTime);

                    }
                }

                else
                {
                    if (front_wheel_pivot[0].transform.rotation.eulerAngles.y > (car_frame.transform.rotation.eulerAngles.y - 45 + 360) % 360 || front_wheel_pivot[0].transform.rotation.eulerAngles.y < (car_frame.transform.rotation.eulerAngles.y + 45 + 360) % 360)
                    {
                        //������Ʈ�� ȸ����Ű�� �Լ�
                        front_wheel_pivot[0].transform.Rotate(Vector3.up * -wheel_rotation_speed * Time.deltaTime);

                    }
                }
            }
            

            else if (direction == "right")
            {
                direction_flag = 1;

                //���� ������ 45���̻� 315�� �̸��϶�
                if (car_frame.transform.rotation.eulerAngles.y > 45 && car_frame.transform.rotation.eulerAngles.y <= 315)
                {
                    if (front_wheel_pivot[0].transform.rotation.eulerAngles.y > (car_frame.transform.rotation.eulerAngles.y - 45 + 360) % 360 && front_wheel_pivot[0].transform.rotation.eulerAngles.y < (car_frame.transform.rotation.eulerAngles.y + 45 + 360) % 360)
                    {

                        front_wheel_pivot[0].transform.Rotate(Vector3.up * wheel_rotation_speed * Time.deltaTime);

                    }
                }

                else
                {
                    if (front_wheel_pivot[0].transform.rotation.eulerAngles.y > (car_frame.transform.rotation.eulerAngles.y - 45 + 360) % 360 || front_wheel_pivot[0].transform.rotation.eulerAngles.y < (car_frame.transform.rotation.eulerAngles.y + 45 + 360) % 360)
                    {

                        front_wheel_pivot[0].transform.Rotate(Vector3.up * wheel_rotation_speed * Time.deltaTime);


                    }
                }
            }
            */


            ///////////////////////////////////////////////////////

            if (direction == "left")
            {
                direction_flag = -1;
            }

            else if (direction == "right")
            {
                direction_flag = +1;
            }

            


            //���� 315�̻�
            if (car_frame.transform.rotation.eulerAngles.y > 315 && car_frame.transform.rotation.eulerAngles.y < 360)
            {

                if (front_wheel_pivot[0].transform.rotation.eulerAngles.y <= 45)
                {
                    if (front_wheel_pivot[0].transform.rotation.eulerAngles.y + 360 >= car_frame.transform.rotation.eulerAngles.y - 
                        max_angle && front_wheel_pivot[0].transform.rotation.eulerAngles.y + 360 <= car_frame.transform.rotation.eulerAngles.y + max_angle)
                    {
                        front_wheel_pivot[0].transform.Rotate(Vector3.up * direction_flag * wheel_rotation_speed * Time.deltaTime);

                    }
                }

                else
                {
                    if (front_wheel_pivot[0].transform.rotation.eulerAngles.y + 360 >= car_frame.transform.rotation.eulerAngles.y - 
                        max_angle + 360 && front_wheel_pivot[0].transform.rotation.eulerAngles.y + 360 <= car_frame.transform.rotation.eulerAngles.y + max_angle + 360)
                    {
                        front_wheel_pivot[0].transform.Rotate(Vector3.up * direction_flag * wheel_rotation_speed * Time.deltaTime);

                    }
                }
            }


            //���� 0�̻� 45�̸�
            else if (car_frame.transform.rotation.eulerAngles.y < 45 && car_frame.transform.rotation.eulerAngles.y >= 0)
            {
                if (front_wheel_pivot[0].transform.rotation.eulerAngles.y <= 315)
                {
                    if (front_wheel_pivot[0].transform.rotation.eulerAngles.y + 360 >= car_frame.transform.rotation.eulerAngles.y - 
                        max_angle + 360 && front_wheel_pivot[0].transform.rotation.eulerAngles.y + 360 <= car_frame.transform.rotation.eulerAngles.y + max_angle + 360)
                    {
                        front_wheel_pivot[0].transform.Rotate(Vector3.up * direction_flag * wheel_rotation_speed * Time.deltaTime);
                    }
                    
                }

                else if (front_wheel_pivot[0].transform.rotation.eulerAngles.y > 315)
                {
                    if (front_wheel_pivot[0].transform.rotation.eulerAngles.y >= car_frame.transform.rotation.eulerAngles.y - 
                        max_angle + 360 && front_wheel_pivot[0].transform.rotation.eulerAngles.y <= car_frame.transform.rotation.eulerAngles.y + max_angle + 360)
                    {
                        front_wheel_pivot[0].transform.Rotate(Vector3.up * direction_flag * wheel_rotation_speed * Time.deltaTime);
                    }
                }


            }
            //���� 45�̻� 315�̸�
            else if (car_frame.transform.rotation.eulerAngles.y >= 45 && car_frame.transform.rotation.eulerAngles.y < 315)
            {
                if (front_wheel_pivot[0].transform.rotation.eulerAngles.y >= car_frame.transform.rotation.eulerAngles.y - 
                    max_angle && front_wheel_pivot[0].transform.rotation.eulerAngles.y <= car_frame.transform.rotation.eulerAngles.y + max_angle)
                {
                    front_wheel_pivot[0].transform.Rotate(Vector3.up * direction_flag * wheel_rotation_speed * Time.deltaTime);
                }
            }


        }

    }


    void restore_wheel()
    {
        //direction_flag = 0;
        /*
        if (front_wheel_pivot[0].transform.rotation.eulerAngles.y > car_frame.transform.rotation.eulerAngles.y)
        {
            if (front_wheel_pivot[0].transform.rotation.eulerAngles.y <= 315)
            {
                front_wheel_pivot[0].transform.Rotate(Vector3.up * -wheel_rotation_speed * Time.deltaTime);
                Debug.Log("1-1��");
            }

            
            else
            {
                front_wheel_pivot[0].transform.Rotate(Vector3.up * +wheel_rotation_speed * Time.deltaTime);
            Debug.Log("1-2��");
            }
            

            //direction_flag = 1;

        }

        else if(front_wheel_pivot[0].transform.rotation.eulerAngles.y < car_frame.transform.rotation.eulerAngles.y)
        {
            if (front_wheel_pivot[0].transform.rotation.eulerAngles.y <= 315)
            {
                front_wheel_pivot[0].transform.Rotate(Vector3.up * +wheel_rotation_speed * Time.deltaTime);
                Debug.Log("2-1��");
            }

            
            else
            {
                front_wheel_pivot[0].transform.Rotate(Vector3.up * -wheel_rotation_speed * Time.deltaTime);
                Debug.Log("2-2��");
            }
            
            
            //direction_flag = -1;

        }
        */

        /*
        else
        {
            front_wheel_pivot[0].transform.rotation = Quaternion.Euler(front_wheel_pivot[0].transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.x, front_wheel_pivot[0].transform.rotation.eulerAngles.z);

            direction_flag = 0;

        }
        */



        //���� 315�̻�
        if (car_frame.transform.rotation.eulerAngles.y > 315 && car_frame.transform.rotation.eulerAngles.y < 360)
        {
            //�������� ������������ Ŭ���
            if (car_frame.transform.rotation.eulerAngles.y > front_wheel_pivot[0].transform.rotation.eulerAngles.y)
            {


                //���������� �������� ũ���� ���������� 45���� �����ϰ�� 300�� �������� ���̳���
                if (front_wheel_pivot[0].transform.rotation.eulerAngles.y <= 45)
                {
                    front_wheel_pivot[0].transform.Rotate(Vector3.up * -wheel_rotation_speed * Time.deltaTime);
                }

                //���������� �������� ũ�鼭 ���� 45�̸����� �����°��
                else
                {
                    front_wheel_pivot[0].transform.Rotate(Vector3.up * +wheel_rotation_speed * Time.deltaTime);
                }

            }

            //�������� �������� �������
            else
            {
                front_wheel_pivot[0].transform.Rotate(Vector3.up * -wheel_rotation_speed * Time.deltaTime);
            }
        }

        //���� 0�̻� 45�̸�
        else if (car_frame.transform.rotation.eulerAngles.y < 45 && car_frame.transform.rotation.eulerAngles.y >= 0)
        {
            //�������� ������������ Ŭ���
            if (car_frame.transform.rotation.eulerAngles.y > front_wheel_pivot[0].transform.rotation.eulerAngles.y)
            {
                front_wheel_pivot[0].transform.Rotate(Vector3.up * +wheel_rotation_speed * Time.deltaTime);
            }


            //�������� �������� �������
            else
            {
                //�������� ũ�� ���� 45�̸��� ���� ������ �������
                if (front_wheel_pivot[0].transform.rotation.eulerAngles.y >= 315)
                {
                    front_wheel_pivot[0].transform.Rotate(Vector3.up * +wheel_rotation_speed * Time.deltaTime);
                }

                //��� ���� 45�̸��ϰ��
                else
                {
                    front_wheel_pivot[0].transform.Rotate(Vector3.up * -wheel_rotation_speed * Time.deltaTime);
                }

            }
        }
        //���� 45�̻� 315�̸�
        else if (car_frame.transform.rotation.eulerAngles.y >= 45 && car_frame.transform.rotation.eulerAngles.y < 315)
        {
            //�������� ������������ Ŭ���
            if (car_frame.transform.rotation.eulerAngles.y > front_wheel_pivot[0].transform.rotation.eulerAngles.y)
            {
                front_wheel_pivot[0].transform.Rotate(Vector3.up * +wheel_rotation_speed * Time.deltaTime);
            }

            //�������� �������� �������
            else
            {
                front_wheel_pivot[0].transform.Rotate(Vector3.up * -wheel_rotation_speed * Time.deltaTime);
            }



        }



        //car_rotation_speed = direction_flag * Mathf.Abs(car_rotation_angle - wheel_rotation_angle);
    }


    void rotation_speed()
    {
        

        //�Ʒ��� ���������� ������ ������ ���Ͽ� ȸ���ӵ��� ���ϴ°�
        float car_rotation_speed_temp = 0;

        //���� 315�̻�
        if (car_frame.transform.rotation.eulerAngles.y > 315 && car_frame.transform.rotation.eulerAngles.y < 360)
        {
            //�������� ������������ Ŭ���
            if (car_frame.transform.rotation.eulerAngles.y > front_wheel_pivot[0].transform.rotation.eulerAngles.y)
            {
                //���������� �������� ũ���� ���������� 45���� �����ϰ�� 300�� �������� ���̳���
                if (front_wheel_pivot[0].transform.rotation.eulerAngles.y <= 45)
                {
                    car_rotation_speed_temp = direction_flag * (Mathf.Abs(car_frame.transform.rotation.eulerAngles.y - 
                        front_wheel_pivot[0].transform.rotation.eulerAngles.y - 360));
                }

                //���������� �������� ũ�鼭 ���� 45�̸����� �����°��
                else
                {
                    car_rotation_speed_temp = direction_flag * (Mathf.Abs(car_frame.transform.rotation.eulerAngles.y - 
                        front_wheel_pivot[0].transform.rotation.eulerAngles.y));
                }

            }

            //�������� �������� �������
            else
            {
                car_rotation_speed_temp = direction_flag * (Mathf.Abs(front_wheel_pivot[0].transform.rotation.eulerAngles.y - 
                    car_frame.transform.rotation.eulerAngles.y));
            }
        }

        //���� 0�̻� 45�̸�
        else if (car_frame.transform.rotation.eulerAngles.y < 45 && car_frame.transform.rotation.eulerAngles.y >= 0)
        {
            //�������� ������������ Ŭ���
            if (car_frame.transform.rotation.eulerAngles.y > front_wheel_pivot[0].transform.rotation.eulerAngles.y)
            {
                car_rotation_speed_temp = direction_flag * (Mathf.Abs(car_frame.transform.rotation.eulerAngles.y - front_wheel_pivot[0].transform.rotation.eulerAngles.y));
            }


            //�������� �������� �������
            else
            {
                //�������� ũ�� ���� 45�̸��� ���� ������ �������
                if (front_wheel_pivot[0].transform.rotation.eulerAngles.y >= 315)
                {
                    car_rotation_speed_temp = direction_flag * (Mathf.Abs(front_wheel_pivot[0].transform.rotation.eulerAngles.y - car_frame.transform.rotation.eulerAngles.y - 360));
                }

                //��� ���� 45�̸��ϰ��
                else
                {
                    car_rotation_speed_temp = direction_flag * (Mathf.Abs(front_wheel_pivot[0].transform.rotation.eulerAngles.y - car_frame.transform.rotation.eulerAngles.y));
                }

            }
        }
        //���� 45�̻� 315�̸�
        else if (car_frame.transform.rotation.eulerAngles.y >= 45 && car_frame.transform.rotation.eulerAngles.y < 315)
        {
            //�������� ������������ Ŭ���
            if (car_frame.transform.rotation.eulerAngles.y > front_wheel_pivot[0].transform.rotation.eulerAngles.y)
            {
                car_rotation_speed_temp = direction_flag * (Mathf.Abs(car_frame.transform.rotation.eulerAngles.y - front_wheel_pivot[0].transform.rotation.eulerAngles.y));

            }

            //�������� �������� �������
            else
            {
                car_rotation_speed_temp = direction_flag * (Mathf.Abs(front_wheel_pivot[0].transform.rotation.eulerAngles.y - car_frame.transform.rotation.eulerAngles.y));
            }



        }

        /*
        else
        {
            car_rotation_speed_temp = 0;

        }
        */

        if (is_drift == true)
        {
            car_rotation_speed = car_rotation_speed_temp * 3;
        }

        else if (is_drift == false)
        {
            car_rotation_speed = car_rotation_speed_temp;
        }
    }
    void booster()
    {

        booster_speed_ratio = max_speed / max_speed_offset;

        if (is_booster == true)
        {
            //booster_speed_ratio = max_speed / max_speed_offset;
            booster_gauge -= 5f * Time.deltaTime;

            max_speed += booster_acceleration * Time.deltaTime;

            if(booster_gauge <= 0)
            {
                is_booster = false;
                //max_speed = max_speed_offset;
                //booster_speed_ratio = 1;
            }
        }

        else if (is_booster == false)
        {
            if(max_speed >= max_speed_offset)
            {
                max_speed -= booster_deceleration * Time.deltaTime;
                if(max_speed <= max_speed_offset)
                {
                    max_speed = max_speed_offset;
                }
            }

        }
    }

    public void remove_object()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    float spin_speed = 2000f;
    public void wheel_spin()
    {
        //front_wheel_pivot[0].transform.Rotate(Vector3.right * 5 * Time.deltaTime);
        //front_wheel_pivot[0].transform.rotation = Quaternion.Euler(front_wheel_pivot[0].transform.rotation.eulerAngles.x + spin_speed *Time.deltaTime, front_wheel_pivot[0].transform.rotation.eulerAngles.y, 0);
        spin_speed = 2000f;

        if( move_speed ==  0)
        {
            spin_speed = 0f;
        }

        front_wheel[0].transform.Rotate(Vector3.right * spin_speed * (move_speed / max_speed) * Time.deltaTime);
        front_wheel[1].transform.Rotate(Vector3.right * spin_speed * (move_speed / max_speed) * Time.deltaTime);
        rear_wheel[0].transform.Rotate(Vector3.right * spin_speed * (move_speed / max_speed) * Time.deltaTime);
        rear_wheel[1].transform.Rotate(Vector3.right * spin_speed * (move_speed / max_speed) * Time.deltaTime);



        //transform.Rotate(Vector3.right * spin_speed * Time.deltaTime);
    }
}
