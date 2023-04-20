using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 2.5f;

    void Start()
    {
        StartCoroutine(DisableAfterTime());
    }

    private IEnumerator DisableAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        // Trả lại đạn vào pool khi hết thời gian sống
        ObjectPool.Instance.ReturnToPool(Constants.TAG_BULLET, gameObject);
    }
}