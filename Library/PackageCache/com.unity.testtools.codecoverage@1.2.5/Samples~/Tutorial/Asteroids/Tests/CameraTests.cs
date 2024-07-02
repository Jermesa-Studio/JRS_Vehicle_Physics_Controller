using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class CameraTests 
{
    GameObject cameraPrefab;
    LoadSceneParameters loadSceneParameters;

#if UNITY_EDITOR
    string asteroidsScenePath;
#endif

    [SetUp]
    public void Setup()
    {
        GameManager.InitializeTestingEnvironment(true, true, true, false, false);

        loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None);

        Object asteroidsScene = ((GameObject)Resources.Load("TestsReferences")).GetComponent<TestsReferences>().asteroidsScene;
        
#if UNITY_EDITOR
        asteroidsScenePath = AssetDatabase.GetAssetPath(asteroidsScene);
#endif
        cameraPrefab = ((GameObject)Resources.Load("TestsReferences", typeof(GameObject))).GetComponent<TestsReferences>().cameraPrefab;
    }

    [Test]
    public void _01_CameraPrefabExists()
    {
        Assert.NotNull(cameraPrefab);
    }

    [Test]
    public void _02_CameraPrefabHasRequiredComponents()
    {
        Assert.IsTrue(cameraPrefab.GetComponent<Camera>().clearFlags == CameraClearFlags.Skybox);
        Assert.IsTrue(cameraPrefab.GetComponent<Camera>().orthographic);
    }
    
    [UnityTest]
    public IEnumerator _03_CameraExistsInScene()
    {
#if UNITY_EDITOR
        EditorSceneManager.LoadSceneInPlayMode(asteroidsScenePath, loadSceneParameters);
        yield return null;

        Assert.IsTrue(Object.FindObjectOfType<Camera>().name == "Camera");
#else
        yield return null;

        Assert.Pass();
#endif        

    }
}
