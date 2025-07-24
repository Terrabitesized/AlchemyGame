using UnityEngine;

public class AtkDespawn : MonoBehaviour
{
    [SerializeField] float despawnTime;    
    

    void Start()
    {
        Destroy(this.gameObject, despawnTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
