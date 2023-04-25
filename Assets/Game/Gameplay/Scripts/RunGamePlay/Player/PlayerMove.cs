using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ToonyColorsPro.ShaderGenerator.Enums;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float swipeThreshold = 20f;
    [SerializeField] private float jumpForce;
    [SerializeField] private float shottime = 0.1f;
    [SerializeField] private PlayerState currentState;
    [SerializeField] private CapsuleCollider capsuleCollider;
    //[SerializeField] private Behaviour scripts;
    //[SerializeField] private bool status;

    public Transform bulletSpawnPoint;
    private float lastShotTime;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private bool hasJumped = false;
    private bool isShooting = false;
    private bool isMovingLeft = false;
    private bool isMovingRight = false;

    private void Awake()
    {
        //create capsule collider
        capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
    }
    private void Start()
    {
        //scripts.enabled = status;
        currentState = PlayerState.Idle;
        Debug.Log("idle");
        isShooting = false; // set isShooting là false để không bắn đạn khi chạy game
    }


    private enum PlayerState
    {
        Idle,
        Moving,
        Jumping,
        Lose,
        Win,
    }

    private void Update()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                IdleState();
                break;
            case PlayerState.Moving:
                MoveState();
                break;
            case PlayerState.Jumping:
                JumpState();
                break;
            case PlayerState.Win:
                Win();
                break;
            case PlayerState.Lose:
                Lose();
                break;
        }
    }


    private void IdleState()
    {
        if (Input.GetMouseButton(0))
        {
            currentState = PlayerState.Moving;
        }
    }

    private void MoveState()
    {
        Move();
        if (Input.GetMouseButtonDown(0))
        {
            currentState = PlayerState.Moving;
        }
        if (currentState == PlayerState.Moving)
        {
            ShootBullet();
        }
    }


    private void JumpState()
    {
        Move();
        if (!hasJumped)
        {
            Jump();
            hasJumped = true;
        }

        currentState = PlayerState.Moving;

    }

    private void Win()
    {

    }

    private void Lose()
    {

    }

    private void Move()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            endPosition = Input.mousePosition;
            float distance = Vector3.Distance(startPosition, endPosition);

            if (distance > swipeThreshold)
            {
                Vector3 direction = endPosition - startPosition;

                if (direction.x > 0)
                {
                    isMovingRight = true;
                    isMovingLeft = false;
                }
                else
                {
                    isMovingLeft = true;
                    isMovingRight = false;
                }
            }
            else
            {
                isMovingLeft = false;
                isMovingRight = false;
            }
        }

        if (isMovingLeft)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
        else if (isMovingRight)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
    }

    private void ShootBullet()
    {
        if(isShooting = true)
        {
            if (Time.time - lastShotTime < shottime) return;
            GameObject bullet = ObjectPool.Instance.SpawnFromPool(Constants.TAG_BULLET, bulletSpawnPoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().velocity = (transform.forward + Vector3.up * 0.5f) * (speed * 5);
            lastShotTime = Time.time;
            Debug.Log("Shoot");
        }    

    }



    private void Jump()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag(Constants.TAG_FINISH))
        {
            Debug.Log("Stop Shoot");
        }

        if(collision.CompareTag(Constants.TAG_DEADZONE))
        {
            Destroy(gameObject);
        }

        if (collision.CompareTag(Constants.TAG_GROUND))
        {
            currentState = PlayerState.Moving;
            hasJumped = false;
            isShooting = true;
        }
        if (collision.CompareTag(Constants.TAG_JUMPPOINT))
        {
            currentState = PlayerState.Jumping;
            hasJumped = false;
            isShooting = false;
            Debug.Log("Jump");
        }

        if(collision.CompareTag(Constants.TAG_TRAP))
        {
            Destroy(gameObject);
        }    

        if(collision.CompareTag(Constants.TAG_FINISH))
        {
            currentState = PlayerState.Win;
        }

        if(collision.CompareTag(Constants.TAG_COLUMN))
        {
            Destroy(gameObject);
        }

        if(collision.CompareTag(Constants.TAG_PLAYER_ADD))
        {
            //set parent
            Debug.Log("Co va cham");
            collision.transform.localPosition = new Vector3(transform.localPosition.x + 1.5f, transform.localPosition.y, transform.localPosition.z);
        }
    }
}
