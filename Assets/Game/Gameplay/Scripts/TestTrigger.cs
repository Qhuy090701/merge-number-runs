using UnityEngine;

public class TestTrigger : MonoBehaviour
{
    [SerializeField] private float speedMove = 5f;
    [SerializeField] private GameObject parent;

    private void Update()
    {
        //di chuyển a w s d
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * speedMove * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * speedMove * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * speedMove * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * speedMove * Time.deltaTime);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER_ADD))
        {
            other.transform.SetParent(parent.transform);
            float distance = parent.transform.childCount - 1; // tính khoảng cách giữa các object, trừ 1 vì object mới được thêm vào sau
            Vector3 newPosition = new Vector3(transform.position.x + Mathf.Round(distance), transform.position.y, transform.position.z);

            if (parent.transform.childCount > 1)
            {
                GameObject prevCube = parent.transform.GetChild(parent.transform.childCount - 2).gameObject;
                float prevX = prevCube.transform.position.x;
                float currX = other.transform.position.x;

                if (currX < prevX) // object mới nằm bên trái object cũ
                {
                    other.transform.SetSiblingIndex(prevCube.transform.GetSiblingIndex() + 1);
                }
                else // object mới nằm bên phải object cũ
                {
                    other.transform.SetSiblingIndex(prevCube.transform.GetSiblingIndex());
                }
            }
            else // chỉ có một object trong parent
            {
                other.transform.SetSiblingIndex(0);
            }


            other.transform.position = newPosition;
            other.gameObject.tag = Constants.TAG_PLAYER;
        }
    }
}
