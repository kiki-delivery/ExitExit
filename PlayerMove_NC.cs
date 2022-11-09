using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMove_NC : MonoBehaviour
{

#if UNITY_EDITOR
    float blockSpeed = 15;  // 여기서 실험 할 때는 속도 15
#elif UNITY_STANDALONE_WIN
    float blockSpeed = 15; 
#elif UNITY_ANDROID
    float blockSpeed = 5; // AR에서는 5
#endif
    string trans; // 나는 무슨 모양 블럭?
    float mouseV; // getAxis 받는 변수

    public float speed = 10; // 러프 스피드
    Vector3 targetPosition; // 자동배치될 위치

    Vector3 startPosition;

    bool OnClick = false;
    bool OnCollsion = false;

    bool R1Col = false;
    bool R2Col = false;


    Vector3 yaho;

    bool yMove = false;

    Animator[] anim = new Animator[3];
    public GameObject[] model;

    AudioSource louder;

    void Start()
    {
        louder = GetComponent<AudioSource>();
        //startPosition = transform.parent.position; // 시작 위치 저장, 다시하기, 간지 연출 때 필요할꺼 같아서 일단 적어둠
        trans = transform.name; // Garo, Sero, UpDown 중 1개 저장됨 => 나는 무슨 모양 블럭?
        for (int i = 0; i < model.Length; i++)
        {
            Debug.Log(model[i].name);
            anim[i] = model[i].GetComponent<Animator>();
            anim[i].SetBool("Move", false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator OnMouse() // 클릭했을 때
    {
        //anim.SetBool("Move", true);

        OnClick = true;
        if (louder)
        {
            louder.Play();
        }
        if (transform.Find("R1") && transform.Find("R2")) // 블럭안에 있는 빈 오브젝트들, 까먹었으면 나를 불러서 다시 설명 듣기
        {
            Where(trans); // 어느 방향으로 움직여야 하는지 구함
        }

        while (Input.GetMouseButton(0)) // 클릭중이면, 이 위에는 한 번만 실행되고 이 아래로는 마우스 누르고 있으면 계속 실행되는 상태
        {
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i].SetBool("Move", true);
            }
            if (!yMove) // 블럭이 가로 모양으로 있으면 마우스 X값 받도록
            {
                mouseV = Input.GetAxis("Mouse X");
            }
            else // 그 외에는 Y값 받도록
            {
                mouseV = Input.GetAxis("Mouse Y");
            }

            SpeedStop(); // axis 값 1 안넘게 조정 후 넘김

            //anim.SetFloat("X", mouseV);

            if (R1Col) // R1 방향으로 부딪힘 = R1 방향으로는 더 가면 안된다
            {
                if (mouseV < 0) // R2 방향으로 갈 때만 움직여라
                {
                    transform.parent.position = transform.parent.position + yaho * mouseV * blockSpeed * Time.deltaTime; // yaho 방향으로 블럭 움직이기
                    //Debug.Log("R2로만 갈게요..");
                    //Debug.Log("마우스 입력 값 체크" + mouseV);
                }
            }
            else if (R2Col) // R2 방향으로 부딪힘 = R2 방향으로는 더 가면 안된다
            {
                if (mouseV > 0) // R1 방향으로 갈 때만 움직여라
                {
                    transform.parent.position = transform.parent.position + yaho * mouseV * blockSpeed * Time.deltaTime; // yaho 방향으로 블럭 움직이기
                    //Debug.Log("R1으로만 갈게요..");
                    //Debug.Log("마우스 입력 값 체크" + mouseV);
                }
            }
            else
            {
                transform.parent.position = transform.parent.position + yaho * mouseV * blockSpeed * Time.deltaTime; // yaho 방향으로 블럭 움직이기
                //Debug.Log("양 옆 어디로든 가지 like 불도저");
                //Debug.Log("마우스 입력 값 체크" + mouseV);

            }
            yield return null;
        }
    }

    void SpeedStop() // mouseAxis 속도 제한, AR에서는 1이 훌쩍 넘어가서 터치만 해도 블럭이 날아감
    {
        if (mouseV > 0.7f)
        {
            mouseV = 0.7f;
        }
        else if (mouseV < -0.7f)
        {
            mouseV = -0.7f;
        }

    }

    void Where(string whatTrans) // r1, r2 가 아니라 블럭의 기울기 기반으로 할 수 있으면 좋을듯
    {
        Vector3 plus, minus; // 

        if (whatTrans.Contains("Garo")) // 각 블럭들의 정해진 방향 지정
        {
            plus = -transform.right; // plus = r2 방향
            minus = transform.right; // minus = r1 방향
        }
        else if (whatTrans.Contains("Sero"))
        {
            plus = transform.forward;
            minus = -transform.forward;
        }
        else
        {
            plus = transform.up;
            minus = -transform.up;
        }

        Vector3 R1 = transform.Find("R1").transform.position;
        Vector3 R2 = transform.Find("R2").transform.position;

        R1 = Camera.main.WorldToScreenPoint(R1);
        R2 = Camera.main.WorldToScreenPoint(R2);

        // x, y 값 각각의 차이 저장 
        float xDis = Mathf.Abs(R1.x - R2.x); // mathf.abs = 절대 값 구하기
        float yDis = Mathf.Abs(R1.y - R2.y);

        //Debug.Log(xDis);
        //Debug.Log(yDis);


        float max = xDis > yDis ? xDis : yDis; // xDis, yDis 중에 제일 큰 값 구함 => 제일 큰 값의 모양으로 있다고 생각

        if (max == xDis) // x값 차이가 가장 크면
        {
            yMove = false; // 마우스 값은 x 기반으로 받아라
            if (R1.x > R2.x)
            {
                yaho = minus; // r1.x 가 더 크니까 r1 방향으로 가라
            }
            else
            {
                yaho = plus;
            }
            Debug.Log("x가 제일 멀어요");
            Debug.Log("R1  " + R1.x + "R2   " + R2.x);
        }
        else
        {
            yMove = true;
            if (R1.y > R2.y)
            {
                yaho = minus;
            }
            else
            {
                yaho = plus;
            }
            Debug.Log("y가 제일 멀어요");
            Debug.Log("R1  " + R1.y + "R2   " + R2.y);
        }
    }

    public void MouseUp() // 클릭 안하고 있으면 X = 클릭했다가 마우스 놨을 때
    {

        //anim.SetTrigger("Up");
        for (int i = 0; i < anim.Length; i++)
        {
            anim[i].SetBool("Move", false);
        }

        OnClick = false;
        mouseV = 0;


        float[] range = MoveRange(trans); // 이 블럭이 움직일 범위 구하기
        AutoMove(trans, range); // 가까운 곳으로 자동 배치
        while (Vector3.Distance(transform.parent.localPosition, targetPosition) > 0.1f) // 목표위치로 천천히 가고
        {
            transform.parent.localPosition = Vector3.Lerp(transform.parent.localPosition, targetPosition, 0.15f); // 여기다 time.deltatime 하면 부드러워 질거임 아마
            //Debug.Log("러프 확인");
        }
        if (Vector3.Distance(transform.parent.localPosition, targetPosition) < 0.1f) // 목표위치에 가까워 졌으면 배치
        {
            transform.parent.localPosition = targetPosition;
            //Debug.Log("순간이동 확인");
        }

    }

    float[] MoveRange(string block) // 자동으로 움직일 범위 지정, 궁금하면 부르세요, 여유 될 때 코드 줄이는 방법 생각하기
    {
        GameObject badak;
        float scale;
        if (block.Contains("Garo")) // 어떤 바닥 기준으로 삼는지
        {
            badak = GameObject.FindWithTag("GaroSeroRange");
            scale = badak.transform.localScale.x;
        }
        else if (block.Contains("Sero"))
        {
            badak = GameObject.FindWithTag("GaroSeroRange");
            scale = badak.transform.localScale.z;
        }
        else
        {
            badak = GameObject.FindWithTag("UpDownRange");
            scale = badak.transform.localScale.x;
        }
        float firstRange = scale - 1.5f; // 제일 먼 곳은 어디
        float[] range = new float[(int)scale - 1]; // 배열 크기 
        int i = 0;
        while (firstRange >= 0.5f)
        {
            range[i] = firstRange;
            firstRange = firstRange - 1f;
            i++;

        }
        return range;
    }

    private void AutoMove(string block, float[] range) // 여유 될 때 코드 줄이는 방법 생각하기, 여기도 궁금하면 부르세요, 버그있음
    {
        GameObject CollisionObject;
        if (block.Contains("Garo"))
        {
            CollisionObject = GameObject.FindWithTag("UpDownRange");
            float yaho = CollisionObject.transform.localPosition.x - transform.parent.localPosition.x;
            // Debug.Log(CollisionObject.transform.position.x - transform.parent.position.x);
            float dis = Mathf.Abs(yaho);
            // Debug.Log("거리의 절대값은" + dis);
            for (int i = 0; i < range.Length; i++)
            {
                if (dis > range[i])
                {
                    targetPosition = new Vector3(CollisionObject.transform.localPosition.x + range[i] + 0.5f, transform.parent.localPosition.y, transform.parent.localPosition.z);
                    break;
                }
            }
        }
        else if (block.Contains("Sero"))
        {
            CollisionObject = GameObject.FindWithTag("SeroMove");
            float yaho = CollisionObject.transform.localPosition.z - transform.parent.localPosition.z;
            // Debug.Log(CollisionObject.transform.position.y - transform.parent.position.y);
            float dis = Mathf.Abs(yaho);

            for (int i = 0; i < range.Length; i++)
            {
                if (dis > range[i])
                {
                    targetPosition = new Vector3(transform.parent.localPosition.x, transform.parent.localPosition.y, CollisionObject.transform.localPosition.z - (range[i] + 0.5f));
                    break;
                }
            }
        }
        else
        {
            CollisionObject = GameObject.FindWithTag("GaroSeroRange");
            float yaho = CollisionObject.transform.localPosition.y - transform.parent.localPosition.y;
            //Debug.Log(CollisionObject.transform.position.y - transform.parent.position.y);
            float dis = Mathf.Abs(yaho);

            for (int i = 0; i < range.Length; i++)
            {
                if (dis > range[i])
                {
                    targetPosition = new Vector3(transform.parent.localPosition.x, CollisionObject.transform.localPosition.y + range[i] + 0.5f, transform.parent.localPosition.z);
                    break;
                }
            }
        }
    }


    private void OnCollisionEnter(Collision collision) // 충돌했을 때(충돌 중일 때랑 다름)
    {
        for (int i = 0; i < anim.Length; i++)
        {
            anim[i].SetTrigger("Colli");
        }
        if (louder)
        {
            louder.Play();
        }

        if (mouseV > 0)
        {
            R1Col = true; // R1이 충돌했다
            //Debug.Log("R1 충돌했어요!");
        }
        else if (mouseV < 0)
        {
            R2Col = true;
            //Debug.Log("R2 충돌했어요!");
        }

        //Debug.Log(transform.parent.name + "  충돌 중 : " + collision.transform.name);
        // Debug.Log("거리는 :  " + Vector3.Distance(collision.transform.parent.position, transform.parent.position)); // 1.5 ~ 1.6


    }

    private void OnCollisionExit(Collision collision) // 충돌에서 빠져 나오면
    {
        //anim.SetBool("Colli", false);

        R1Col = false;
        R2Col = false;

    }
}
