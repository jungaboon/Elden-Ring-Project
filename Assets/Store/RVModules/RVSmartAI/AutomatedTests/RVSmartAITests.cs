using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace RVModules.RVSmartAI.AutomatedTests
{
    public class RVSmartAITests
    {
        // WARNING SUPER IMPORTANT
        // because smartAiUpdater is singleton implementation, its not destroyed between tests, so it's config/values are only used from first test!!!

        [UnityTest]
        public IEnumerator FleeBehaviourTest()
        {
            Time.timeScale = 10;
            SceneManager.LoadScene("FleeTest", LoadSceneMode.Single);
            yield return new WaitForSecondsRealtime(.1f);
            Transform fleeChar = GameObject.Find("Flee").transform;
            Transform stationaryChar = GameObject.Find("Character").transform;
            yield return new WaitForSecondsRealtime(.3f);
            Assert.True(Vector3.Distance(fleeChar.position, stationaryChar.position) > 2f);
        }

        [UnityTest]
        public IEnumerator FollowBehaviourTest()
        {
            Time.timeScale = 10;
            SceneManager.LoadScene("FollowTest", LoadSceneMode.Single);
            yield return new WaitForSecondsRealtime(.1f);
            Transform follower = GameObject.Find("Follower").transform;
            yield return new WaitForSecondsRealtime(.3f);
            Assert.True(follower.position.x > 1.2f);
        }
        
        [UnityTest]
        public IEnumerator PatrolBehaviourTest()
        {
            Time.timeScale = 10;
            SceneManager.LoadScene("WaypointsTest", LoadSceneMode.Single);
            yield return new WaitForSecondsRealtime(.9f);
            Assert.True(Object.FindObjectsOfType<DestroyWhenClose>().Length == 0);
        }
    }
}