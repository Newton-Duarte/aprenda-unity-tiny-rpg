using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    [SerializeField] GameObject shadowPrefab;
    [SerializeField] Transform shadowPosition;
    [SerializeField] float minScale;
    [SerializeField] float maxScale;

    public LightSource[] lightSources;
    public List<Transform> shadows;
    public List<Transform> lights;

    void OnEnable()
    {
        updateLightsList();
    }

    public void updateLightsList()
    {
        if (lightSources.Length > 0)
        {
            foreach(Transform t in shadows)
            {
                Destroy(t.gameObject);
            }

            shadows.Clear();
            lights.Clear();
        }

        lightSources = FindObjectsOfType(typeof(LightSource)) as LightSource[];

        for (int i = 0; i < lightSources.Length; i++)
        {
            lights.Add(lightSources[i].transform);
            GameObject tempShadow = Instantiate(shadowPrefab, shadowPosition.position, transform.localRotation, transform);
            shadows.Add(tempShadow.transform);
        }
    }

    void LateUpdate()
    {
        for (int i = 0; i < lightSources.Length; i++)
        {
            Vector3 direction = lights[i].transform.position - transform.position;
            shadows[i].up = direction * -1;

            float distance = Vector3.Distance(transform.position, lights[i].transform.position);

            if (distance < minScale) { distance = minScale; }
            else if (distance > maxScale) { distance = maxScale; }

            shadows[i].localScale = new Vector3(shadows[i].localScale.x, distance, shadows[i].localScale.z);
        }
    }
}
