using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PredictionManager : Singleton<PredictionManager>
{

    /// ----------- Variables------------- 

    public GameObject wallObstaclesRoot;
    public GameObject obstaclesRoot;
    public GameObject pixelsRoot;

    List<GameObject> dummyObstacles;
    List<GameObject> dummyPixels;

    [Space]
    public GameObject ball;
    GameObject dummyBall;

    [Space]
    public int maxIterations;

    Scene currentScene;
    Scene predictionScene;

    PhysicsScene currentPhysicsScene;
    PhysicsScene predictionPhysicsScene;

    LineRenderer lineRenderer;


    /// ----------- Start Function------------- 
    void Start()
    {
        Physics.autoSimulation = false;

        currentScene = SceneManager.GetActiveScene();
        currentPhysicsScene = currentScene.GetPhysicsScene();

        CreateSceneParameters parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        predictionScene = SceneManager.CreateScene("Prediction", parameters);
        predictionPhysicsScene = predictionScene.GetPhysicsScene();
        lineRenderer = GetComponent<LineRenderer>();
        dummyObstacles = new List<GameObject>();
        dummyPixels = new List<GameObject>();
        CopyWallObstacles();
    }

    /// ----------- Event Functions------------- 
    void FixedUpdate()
    {
        if (currentPhysicsScene.IsValid())
        {
            currentPhysicsScene.Simulate(Time.fixedDeltaTime);
        }
    }

    /// ----------- Functions-------------
    ///
    ///Copy Wall Obstacles To Predict Scene 
    public void CopyWallObstacles()
    {
        foreach (Transform t in wallObstaclesRoot.transform)
        {
            if (t.gameObject.GetComponent<Collider>() != null)
            {
                GameObject fakeT = Instantiate(t.gameObject);
                fakeT.transform.position = t.position;
                fakeT.transform.rotation = t.rotation;
                Renderer fakeR = fakeT.GetComponent<Renderer>();
                if (fakeR)
                {
                    fakeR.enabled = false;
                }
                SceneManager.MoveGameObjectToScene(fakeT, predictionScene);
            }
        }
    }

    ///Copy Obstacles To Predict Scene 
    public void CopyAllObstacles()
    {
        foreach (Transform t in obstaclesRoot.transform)
        {
            if (t.gameObject.GetComponent<Collider>() != null)
            {
                GameObject fakeT = Instantiate(t.gameObject);
                fakeT.transform.position = t.position;
                fakeT.transform.rotation = t.rotation;
                Renderer fakeR = fakeT.GetComponent<Renderer>();
                if (fakeR)
                {
                    fakeR.enabled = false;
                }
                SceneManager.MoveGameObjectToScene(fakeT, predictionScene);
                dummyObstacles.Add(fakeT);
            }
        }
    }

    ///Copy PixelObjects To Predict Scene 
    public void CopyAllPixels()
    {
        foreach (Transform t in pixelsRoot.transform)
        {
            if (t.gameObject.GetComponent<Collider>() != null)
            {
                GameObject fakeT = Instantiate(t.gameObject);
                fakeT.GetComponent<Collider>().isTrigger = true;
                fakeT.transform.position = t.position;
                fakeT.transform.rotation = t.rotation;
                Renderer fakeR = fakeT.GetComponent<Renderer>();
                if (fakeR)
                {
                    fakeR.enabled = false;
                }
                SceneManager.MoveGameObjectToScene(fakeT, predictionScene);
                dummyPixels.Add(fakeT);
            }
        }
    }
    ///Destroy obstacles and pixels from Predict Scene 
    public void KillAll()
    {
        foreach (var o in dummyObstacles)
        {
            Destroy(o);
        }
        dummyObstacles.Clear();

        foreach (var o in dummyPixels)
        {
            Destroy(o);
        }
        dummyPixels.Clear();
    }
    ///LineRenderer Clear 
    public void DeletePredict()
    {
        lineRenderer.positionCount = 0;
    }

    /// Linerenderer object  renderer by the ball position and force   
    public void Predict(GameObject ball, Vector3 force)
    {
        if (currentPhysicsScene.IsValid() && predictionPhysicsScene.IsValid())
        {
            if (dummyBall == null)
            {
                dummyBall = Instantiate(ball);
                SceneManager.MoveGameObjectToScene(dummyBall, predictionScene);
            }
            dummyBall.GetComponent<Renderer>().enabled = false;
            dummyBall.transform.position = ball.transform.position;
            dummyBall.GetComponent<Collider>().enabled = true;
            dummyBall.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            lineRenderer.positionCount = 0;
            lineRenderer.positionCount = maxIterations;
            lineRenderer.SetPosition(0, ball.transform.position);
            for (int i = 1; i < maxIterations; i++)
            {
                predictionPhysicsScene.Simulate(Time.fixedDeltaTime);
                lineRenderer.SetPosition(i, dummyBall.transform.position);
            }

            Destroy(dummyBall);
        }
    }
}
