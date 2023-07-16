using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBullet : MonoBehaviour
{

    public GameObject bulletPrefab;  //子弹预制体
    void Update()
    {
        Shoot();
    }
    float time = Time.deltaTime
    void Shoot()
    {
        if (Input.GetMouseButtonDown(0))  //如果按下鼠标左键，生成预制体
        {
            Instantiate(bulletPrefab, transform.position, transform.rotation);  //生成预制体
        }
    }
    
}
