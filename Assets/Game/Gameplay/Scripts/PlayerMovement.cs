using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float swipeThreshold = 20f;
    [SerializeField] private PlayerState currentState;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private bool isMovingLeft = false;
    private bool isMovingRight = false;

    private enum PlayerState
    {
        Idle,
        Moving,
    }
    void Start()
    {
    }

    private void Update()
    {
        //capsual.GetComponent<CapsuleCollider>().enabled = true;
        switch (currentState)
        {
            case PlayerState.Idle:
                IdleState();
                break;
            case PlayerState.Moving:
                MoveState();
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
            //ShootBullet();
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER_ADD))
        {
            foreach (Transform child in transform)
            {
                child.position = new Vector3(child.position.x, child.position.y, child.position.z);
                //debug position 
                Debug.Log("child position = " + child.position);
            }
            other.transform.SetParent(transform);
            
        }
    }
}
