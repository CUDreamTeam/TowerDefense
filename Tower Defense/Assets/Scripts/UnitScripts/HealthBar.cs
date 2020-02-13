using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider = null;
    [SerializeField]
    private Image fillImage = null;

    private void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = 100;
    }

    public void SetFillColor(Color color)
    {
        fillImage.color = color;
    }

    private void Update()
    {
        Vector3 v = Camera.main.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(Camera.main.transform.position - v);
        transform.Rotate(0, 180, 0);
    }

    public void changeHealth(float percentHealth)
    {
        slider.value = percentHealth;
    }
}
