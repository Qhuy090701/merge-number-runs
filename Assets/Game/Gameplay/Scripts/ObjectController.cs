using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    public GameObject player;  // Đối tượng người chơi

    public List<GameObject> objects;  // Danh sách các đối tượng

    void Start()
    {
        // Khởi tạo vị trí ban đầu của các đối tượng
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].transform.position = new Vector3(i * 2, 0, 0);
        }
    }

    void Update()
    {
        // Di chuyển người chơi theo hướng thẳng
        player.transform.Translate(Vector3.forward * Time.deltaTime);

        // Kiểm tra va chạm giữa người chơi và các đối tượng
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] != null && IsColliding(player, objects[i]))
            {
                // Xử lý va chạm giữa người chơi và đối tượng
                if (objects[i].transform.position.x > player.transform.position.x)
                {
                    // Nếu đối tượng nằm bên phải của người chơi
                    AddObjectToRight(player, objects[i], i + 1);
                }
                else
                {
                    // Nếu đối tượng nằm bên trái của người chơi
                    AddObjectToLeft(player, objects[i], i - 1);
                }
            }
        }
    }

    // Hàm kiểm tra va chạm giữa hai đối tượng
    bool IsColliding(GameObject object1, GameObject object2)
    {
        Collider[] colliders = Physics.OverlapBox(object1.transform.position, new Vector3(1, 1, 1));
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject == object2)
            {
                return true;
            }
        }
        return false;
    }

    // Hàm thêm đối tượng vào bên phải
    void AddObjectToRight(GameObject player, GameObject objectToMove, int index)
    {
        if (index >= objects.Count)
        {
            // Nếu đối tượng cần thêm nằm bên phải nhất của danh sách
            objects.Add(objectToMove);
        }
        else
        {
            // Di chuyển các đối tượng phía bên phải của đối tượng cần thêm
            for (int i = index; i < objects.Count; i++)
            {
                objects[i].transform.position += new Vector3(2, 0, 0);
            }
            // Thêm đối tượng vào danh sách
            objects.Insert(index, objectToMove);
        }
        objectToMove.transform.position = player.transform.position + new Vector3(2, 0, 0);
    }

    // Hàm thêm đối tượng vào bên trái
    void AddObjectToLeft(GameObject player, GameObject objectToMove, int index)
    {
        if (index < 0)
        {
            // Nếu đối tượng cần thêm nằm bên trái nhất của danh sách
            objects.Insert(0, objectToMove);
            // Di chuyển các đối tượng phía bên phải của đối tượng cần thêm
            for (int i = 1; i < objects.Count; i++)
            {
                objects[i].transform.position += new Vector3(2, 0, 0);
            }
        }
        else
        {
            // Di chuyển các đối tượng phía bên phải của đối tượng cần thêm
            for (int i = index; i < objects.Count; i++)
            {
                objects[i].transform.position += new Vector3(2, 0, 0);
            }
            // Thêm đối tượng vào danh sách
            objects.Insert(index + 1, objectToMove);
        }
        // Di chuyển đối tượng cần thêm vào bên trái của người chơi
        objectToMove.transform.position = player.transform.position + new Vector3(-2, 0, 0);
    }
}