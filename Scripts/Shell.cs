using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public Rigidbody myRigidbody;

    public float minForce;
    public float maxForce;

    float lifeTime = 4;
    float fadeTime = 2;
    private void Start()
    {
        float force = Random.Range(minForce, maxForce);
        myRigidbody.AddForce(transform.right * force);
        myRigidbody.AddTorque(Random.insideUnitSphere * force);//在单位圆内随机旋转

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifeTime);
        float percent = 0;
        float fadeSpeed = 1 / fadeTime;
        Material material = GetComponent<Renderer>().material;
        Color initialColor = material.color;

        while (percent < 1) { 
            percent += Time.deltaTime * fadeSpeed;
            material.color = Color.Lerp(initialColor, Color.clear, percent);
            yield return null;
        }

        Destroy(gameObject);
    }
}
