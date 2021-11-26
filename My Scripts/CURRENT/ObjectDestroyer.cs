using UnityEngine;
public class ObjectDestroyer : MonoBehaviour
{
    public void DestroySelfObject()
    {
        Destroy(gameObject);
    }
}
