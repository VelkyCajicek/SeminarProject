using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera cam;
    public float parallaxStrengthX = 0.5f;
    public float parallaxStrengthY = 0.5f;
    private Transform camTransform;
    private Vector3 lastCamPos;
    private float spriteWidth;
    private float spriteScaleX;

    private void Start()
    {
        spriteScaleX = transform.localScale.x;
        camTransform = cam.transform;
        lastCamPos = camTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        spriteWidth = texture.width / sprite.pixelsPerUnit; 
    }

    private void FixedUpdate()
    {
        Vector3 distance = camTransform.position - lastCamPos;
        transform.position += new Vector3(distance.x * -parallaxStrengthX, distance.y * -parallaxStrengthY);
        lastCamPos = camTransform.position;
        if (Mathf.Abs(camTransform.position.x - transform.position.x)>=spriteWidth * spriteScaleX)
        {
            float offset = (camTransform.position.x - transform.position.x) % (spriteWidth * spriteScaleX);
            transform.position = new Vector3(camTransform.position.x + offset, transform.position.y);
        }
    }
}
