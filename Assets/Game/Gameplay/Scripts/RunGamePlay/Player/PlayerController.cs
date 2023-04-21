using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float swipeThreshold = 50f;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform startGamePoint;

    public List<GameObject> characters = new List<GameObject>();

    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    private float lastShotTime;

    private Vector3 startPosition;
    private Vector3 endPosition;

    public bool isShooting = false;
    public bool isGround = false;
    private bool isMovingLeft = false;
    private bool isMovingRight = false;


    private void Start()
    {

    }
    private void Update()
    {
        Move();
        RayCastCheck();
    }

    void FixedUpdate()
    {
       // Jump();
    }

    private void Move()
    {
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
    void ShootBullet()
    {
        if (isShooting = true)
        {
            // Nếu chưa đủ thời gian giữa các lần bắn, thoát khỏi hàm
            if (Time.time - lastShotTime < 0.5f) return;
            GameObject bullet = ObjectPool.Instance.SpawnFromPool(Constants.TAG_BULLET, bulletSpawnPoint.position, Quaternion.identity);
            // Bắn đạn với vận tốc của player + 2f
            bullet.GetComponent<Rigidbody>().velocity = (transform.forward + Vector3.up * 0.5f) * (speed + 2f);
            // Lưu lại thời điểm bắn viên đạn cuối cùng
            lastShotTime = Time.time;
            
            Debug.Log("Shoot");
        }
    }

    public void Jump()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        //}

        GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Debug.Log("Jump thành công");
    }

    public void RayCastCheck()
    {
        Vector3 startPos = transform.position;

        // Xác định hướng của raycast
        Vector3 rayDirection = Vector3.down;

        // Độ dài của raycast
        float rayLength = 1.0f;

        // Tạo raycast
        RaycastHit hit;
        if (Physics.Raycast(startPos, rayDirection, out hit, rayLength))
        {
            // Kiểm tra xem raycast đã va chạm với mặt đất hay không
            if (hit.collider.CompareTag("Ground"))
            {
                // Xử lý khi va chạm với mặt đất
                Debug.Log("Hit the ground!");
                ShootBullet();
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_FINISH))
        {
            speed = 0;
        }

        if(other.CompareTag(Constants.TAG_PLAYER_ADD))
        {
            Debug.Log("Va Cham");
        }    

        if(other.CompareTag(Constants.TAG_JUMPPOINT))
        {
            Jump();
            Debug.Log("jump ");
        }    

        if(other.CompareTag(Constants.TAG_TRAP))
        {
            Destroy(gameObject);
        }
    }
}



