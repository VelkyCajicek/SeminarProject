using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public GameObject mainCamera;
    float spriteLength;
    float startPos;
    public float parallaxSpeed=0.1f;
    // Start is called before the first frame update
    void Start()
    {
        spriteLength = GetComponent<SpriteRenderer>().bounds.size.x;
        startPos = transform.position.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //float temp = mainCamera.transform.position.x * (1 - parallaxSpeed);
        float dist = mainCamera.transform.position.x * parallaxSpeed;
        transform.position = new Vector3(startPos+dist,transform.position.y, transform.position.z);
        if (Mathf.Abs(Mathf.Abs(startPos) - Mathf.Abs(transform.position.x))>spriteLength)
        {
            startPos = mainCamera.transform.position.x;
        }
    }
}
