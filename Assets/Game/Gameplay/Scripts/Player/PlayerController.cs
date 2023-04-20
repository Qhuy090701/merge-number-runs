﻿using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float swipeThreshold = 50f;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private bool isMovingLeft = false;
    private bool isMovingRight = false;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        // Nhân vật sẽ luôn đi thẳng với vận tốc là speed
        transform.position += Vector3.forward * speed * Time.deltaTime;

        // Khi nhấn chuột trái, lưu lại vị trí chuột hiện tại
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
        }

        // Khi giữ chuột trái, tính khoảng cách giữa vị trí bắt đầu và vị trí hiện tại
        if (Input.GetMouseButton(0))
        {
            endPosition = Input.mousePosition;
            float distance = Vector3.Distance(startPosition, endPosition);

            // Nếu khoảng cách lớn hơn swipeThreshold, xác định hướng di chuyển
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
                // Nếu khoảng cách nhỏ hơn swipeThreshold, dừng di chuyển sang trái hoặc phải
                isMovingLeft = false;
                isMovingRight = false;
            }
        }

        // Nếu đang di chuyển sang trái, di chuyển sang trái với tốc độ speed
        if (isMovingLeft)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
        // Nếu đang di chuyển sang phải, di chuyển sang phải với tốc độ speed
        else if (isMovingRight)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
    }    
}