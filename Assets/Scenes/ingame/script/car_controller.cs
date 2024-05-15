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

    //가속력을 결정하는 변수
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

    //바퀴 회전속도를 결정하는 변수
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
        //뒷바퀴는 회전축이 없음
        rear_wheel[0] = transform.Find("empty").transform.Find("Rear_wheel_left").gameObject;
        rear_wheel[1] = transform.Find("empty").transform.Find("Rear_wheel_right").gameObject;


        max_speed = max_speed_offset;

        Time.timeScale = 1f;

        car_frame = transform.Find("empty").gameObject;
    }



    private void FixedUpdate()
    {
        
        //이동
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
                        //이건 한번만 작동하는것임 누르자마자 미끄러지는거 구현
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
                        //이건 한번만 작동하는것임 누르자마자 미끄러지는거 구현
                        drift_speed += drift_speed_offset * Time.deltaTime;
                    }
                }
            }


            else if (Input.GetKey(KeyCode.UpArrow))
            {
                restore_wheel();
            }

        }



        //차체회전
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
            
            //드리프트시 자동차 추가회전
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


        
        //앞키 누를시 가속
        if (Input.anyKey)
        {

            //전진
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (controller_flag == true)
                {

                    booster();


                    //부스터 사용시 최대속도 조절및 가속 증가
                    if (is_booster == true)
                    {
                        //max_speed = max_speed_offset * booster_scale;

                        move_speed += booster_acceleration * Time.deltaTime;
                        if (move_speed >= max_speed)
                        {
                            move_speed = max_speed;
                        }
                    }


                    //전진가속
                    move_speed += acceleration * Time.deltaTime;

                    //드리프트시
                    if (is_drift == true)
                    {
                        if (move_speed > max_speed * drift_deceleration_scale)
                        {
                            move_speed -= drift_deceleration * Time.deltaTime;
                        }

                    }
                    //속도 최댓값 고정
                    if (move_speed >= max_speed)
                    {
                        move_speed = max_speed;
                    }




                    if (Input.GetKey(KeyCode.LeftControl))
                    {

                        if (is_booster == false)
                        {

                            //부스터 사용 플래그 대신 작성한것 수정해야할 가능성 높음
                            if (max_speed <= max_speed_offset)
                            {
                                booster_gauge = max_booster_gauge;
                                is_booster = true;
                            }
                        }

                    }
                }

            }
            //후진
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


            //이사보내야함 ㅇㅇ
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
        {   //속도 최솟값 고정
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
            //0.75는 드리프트 종료후 드리프트가속을 감속으로 내려야하는데 가속을 0.75배율로 내려서 감속시킨다는뜻
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

        //드리프트는 회전방향 반대방향으로 밀려나야함 따라서 왼쪽회전시 오른쪽으로 나아가야함
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

    //회전인데 나중에 방향 따라 수정해주긴해야함
    void turn(string direction)
    {
        direction_flag = 0;
        //차량 315이상
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


        //차량 0이상 45미만
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

        //차량 45이상 315미만
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

        //락거는 조건문
        //if (front_wheel_pivot[0].transform.rotation.eulerAngles.y < car_frame.transform.rotation.eulerAngles.y + 40 && front_wheel_pivot[0].transform.rotation.eulerAngles.y > car_frame.transform.rotation.eulerAngles.y - 40)

        //트루면 꺼짐 락걸린거임 ㅇㅇ
        if (true)
        {

            /*
            if (direction == "left")
            {
                direction_flag = -1;
                //차의 각도가 -45도일때를 제외하면 작동함
                if (car_frame.transform.rotation.eulerAngles.y > 45 && car_frame.transform.rotation.eulerAngles.y <= 315)
                {
                    if (front_wheel_pivot[0].transform.rotation.eulerAngles.y > (car_frame.transform.rotation.eulerAngles.y - 45 + 360) % 360 && front_wheel_pivot[0].transform.rotation.eulerAngles.y < (car_frame.transform.rotation.eulerAngles.y + 45 + 360) % 360)
                    {

                        //오브젝트를 회전시키는 함수
                        front_wheel_pivot[0].transform.Rotate(Vector3.up * -wheel_rotation_speed * Time.deltaTime);

                    }
                }

                else
                {
                    if (front_wheel_pivot[0].transform.rotation.eulerAngles.y > (car_frame.transform.rotation.eulerAngles.y - 45 + 360) % 360 || front_wheel_pivot[0].transform.rotation.eulerAngles.y < (car_frame.transform.rotation.eulerAngles.y + 45 + 360) % 360)
                    {
                        //오브젝트를 회전시키는 함수
                        front_wheel_pivot[0].transform.Rotate(Vector3.up * -wheel_rotation_speed * Time.deltaTime);

                    }
                }
            }
            

            else if (direction == "right")
            {
                direction_flag = 1;

                //차의 각도가 45도이상 315도 미만일때
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

            


            //차량 315이상
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


            //차량 0이상 45미만
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
            //차량 45이상 315미만
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
                Debug.Log("1-1번");
            }

            
            else
            {
                front_wheel_pivot[0].transform.Rotate(Vector3.up * +wheel_rotation_speed * Time.deltaTime);
            Debug.Log("1-2번");
            }
            

            //direction_flag = 1;

        }

        else if(front_wheel_pivot[0].transform.rotation.eulerAngles.y < car_frame.transform.rotation.eulerAngles.y)
        {
            if (front_wheel_pivot[0].transform.rotation.eulerAngles.y <= 315)
            {
                front_wheel_pivot[0].transform.Rotate(Vector3.up * +wheel_rotation_speed * Time.deltaTime);
                Debug.Log("2-1번");
            }

            
            else
            {
                front_wheel_pivot[0].transform.Rotate(Vector3.up * -wheel_rotation_speed * Time.deltaTime);
                Debug.Log("2-2번");
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



        //차량 315이상
        if (car_frame.transform.rotation.eulerAngles.y > 315 && car_frame.transform.rotation.eulerAngles.y < 360)
        {
            //차각도가 바퀴각도보다 클경우
            if (car_frame.transform.rotation.eulerAngles.y > front_wheel_pivot[0].transform.rotation.eulerAngles.y)
            {


                //차량각도가 바퀴보단 크지만 바퀴각도가 45도가 이하일경우 300도 언저리의 값이나옴
                if (front_wheel_pivot[0].transform.rotation.eulerAngles.y <= 45)
                {
                    front_wheel_pivot[0].transform.Rotate(Vector3.up * -wheel_rotation_speed * Time.deltaTime);
                }

                //차량각도가 바퀴보다 크면서 값이 45미만으로 나오는경우
                else
                {
                    front_wheel_pivot[0].transform.Rotate(Vector3.up * +wheel_rotation_speed * Time.deltaTime);
                }

            }

            //차각도가 바퀴보다 작을경우
            else
            {
                front_wheel_pivot[0].transform.Rotate(Vector3.up * -wheel_rotation_speed * Time.deltaTime);
            }
        }

        //차량 0이상 45미만
        else if (car_frame.transform.rotation.eulerAngles.y < 45 && car_frame.transform.rotation.eulerAngles.y >= 0)
        {
            //차각도가 바퀴각도보다 클경우
            if (car_frame.transform.rotation.eulerAngles.y > front_wheel_pivot[0].transform.rotation.eulerAngles.y)
            {
                front_wheel_pivot[0].transform.Rotate(Vector3.up * +wheel_rotation_speed * Time.deltaTime);
            }


            //차각도가 바퀴보다 작을경우
            else
            {
                //바퀴값이 크나 값이 45미만의 값이 나오지 않을경우
                if (front_wheel_pivot[0].transform.rotation.eulerAngles.y >= 315)
                {
                    front_wheel_pivot[0].transform.Rotate(Vector3.up * +wheel_rotation_speed * Time.deltaTime);
                }

                //결과 값이 45미만일경우
                else
                {
                    front_wheel_pivot[0].transform.Rotate(Vector3.up * -wheel_rotation_speed * Time.deltaTime);
                }

            }
        }
        //차량 45이상 315미만
        else if (car_frame.transform.rotation.eulerAngles.y >= 45 && car_frame.transform.rotation.eulerAngles.y < 315)
        {
            //차각도가 바퀴각도보다 클경우
            if (car_frame.transform.rotation.eulerAngles.y > front_wheel_pivot[0].transform.rotation.eulerAngles.y)
            {
                front_wheel_pivot[0].transform.Rotate(Vector3.up * +wheel_rotation_speed * Time.deltaTime);
            }

            //차각도가 바퀴보다 작을경우
            else
            {
                front_wheel_pivot[0].transform.Rotate(Vector3.up * -wheel_rotation_speed * Time.deltaTime);
            }



        }



        //car_rotation_speed = direction_flag * Mathf.Abs(car_rotation_angle - wheel_rotation_angle);
    }


    void rotation_speed()
    {
        

        //아래는 차량에대한 바퀴의 각도를 구하여 회전속도를 구하는곳
        float car_rotation_speed_temp = 0;

        //차량 315이상
        if (car_frame.transform.rotation.eulerAngles.y > 315 && car_frame.transform.rotation.eulerAngles.y < 360)
        {
            //차각도가 바퀴각도보다 클경우
            if (car_frame.transform.rotation.eulerAngles.y > front_wheel_pivot[0].transform.rotation.eulerAngles.y)
            {
                //차량각도가 바퀴보단 크지만 바퀴각도가 45도가 이하일경우 300도 언저리의 값이나옴
                if (front_wheel_pivot[0].transform.rotation.eulerAngles.y <= 45)
                {
                    car_rotation_speed_temp = direction_flag * (Mathf.Abs(car_frame.transform.rotation.eulerAngles.y - 
                        front_wheel_pivot[0].transform.rotation.eulerAngles.y - 360));
                }

                //차량각도가 바퀴보다 크면서 값이 45미만으로 나오는경우
                else
                {
                    car_rotation_speed_temp = direction_flag * (Mathf.Abs(car_frame.transform.rotation.eulerAngles.y - 
                        front_wheel_pivot[0].transform.rotation.eulerAngles.y));
                }

            }

            //차각도가 바퀴보다 작을경우
            else
            {
                car_rotation_speed_temp = direction_flag * (Mathf.Abs(front_wheel_pivot[0].transform.rotation.eulerAngles.y - 
                    car_frame.transform.rotation.eulerAngles.y));
            }
        }

        //차량 0이상 45미만
        else if (car_frame.transform.rotation.eulerAngles.y < 45 && car_frame.transform.rotation.eulerAngles.y >= 0)
        {
            //차각도가 바퀴각도보다 클경우
            if (car_frame.transform.rotation.eulerAngles.y > front_wheel_pivot[0].transform.rotation.eulerAngles.y)
            {
                car_rotation_speed_temp = direction_flag * (Mathf.Abs(car_frame.transform.rotation.eulerAngles.y - front_wheel_pivot[0].transform.rotation.eulerAngles.y));
            }


            //차각도가 바퀴보다 작을경우
            else
            {
                //바퀴값이 크나 값이 45미만의 값이 나오지 않을경우
                if (front_wheel_pivot[0].transform.rotation.eulerAngles.y >= 315)
                {
                    car_rotation_speed_temp = direction_flag * (Mathf.Abs(front_wheel_pivot[0].transform.rotation.eulerAngles.y - car_frame.transform.rotation.eulerAngles.y - 360));
                }

                //결과 값이 45미만일경우
                else
                {
                    car_rotation_speed_temp = direction_flag * (Mathf.Abs(front_wheel_pivot[0].transform.rotation.eulerAngles.y - car_frame.transform.rotation.eulerAngles.y));
                }

            }
        }
        //차량 45이상 315미만
        else if (car_frame.transform.rotation.eulerAngles.y >= 45 && car_frame.transform.rotation.eulerAngles.y < 315)
        {
            //차각도가 바퀴각도보다 클경우
            if (car_frame.transform.rotation.eulerAngles.y > front_wheel_pivot[0].transform.rotation.eulerAngles.y)
            {
                car_rotation_speed_temp = direction_flag * (Mathf.Abs(car_frame.transform.rotation.eulerAngles.y - front_wheel_pivot[0].transform.rotation.eulerAngles.y));

            }

            //차각도가 바퀴보다 작을경우
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
