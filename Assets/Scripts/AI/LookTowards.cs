using UnityEngine;

public class LookTowards : MonoBehaviour
{
    private Transform target;
    void Start()
    {
        target = Camera.main.transform;
    }
    
    void Update()
    {
        LookTowardsTarget();    
    }

    private void LookTowardsTarget()
    {
        transform.LookAt(target);
    }
}
