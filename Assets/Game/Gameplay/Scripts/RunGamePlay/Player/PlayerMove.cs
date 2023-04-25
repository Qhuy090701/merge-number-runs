using Funzilla;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ToonyColorsPro.ShaderGenerator.Enums;
using static UnityEditor.PlayerSettings;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float speedMove = 5f;
    [SerializeField] private float jumpForce;
    [SerializeField] private float shottime = 0.1f;
    protected PlayerState currentState;
    public   GameObject player;

    public Transform bulletSpawnPoint;
    private float lastShotTime;

    private bool hasJumped = false;
    private bool isShooting = false;

    private void Start()
    {
        currentState = PlayerState.Idle;
        isShooting = false;
    }


    protected enum PlayerState
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
        //if (Input.GetMouseButton(0))
        //{
            currentState = PlayerState.Moving;
        //}
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
        transform.position += Vector3.forward * speedMove * Time.deltaTime;
    }

    private void ShootBullet()
    {
        if(isShooting = true)
        {
            if (Time.time - lastShotTime < shottime) return;
            GameObject bullet = ObjectPool.Instance.SpawnFromPool(Constants.TAG_BULLET, bulletSpawnPoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().velocity = (transform.forward + Vector3.up * 0.5f) * (speedMove * 5);
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
            collision.transform.SetParent(player.transform);
            int childCount = player.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = player.transform.GetChild(i); 
                //debug ra số i khi va chạm
                Debug.Log(i);
                Vector3 childPos = child.position;
                collision.transform.position = new Vector3(transform.position.x + (1.55f*i), transform.position.y, transform.position.z);
                collision.gameObject.tag = Constants.TAG_PLAYER;
            }
        }
    }
}
