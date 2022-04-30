using UnityEngine;

namespace RVModules.RVSmartAI.AutomatedTests
{
    public class DestroyWhenClose : MonoBehaviour
    {
        public Transform objectToCheckForDistance;
        public float distance;

        // Update is called once per frame
        void Update()
        {
            if(Vector3.Distance(objectToCheckForDistance.position, transform.position) < distance)
                gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }
}
