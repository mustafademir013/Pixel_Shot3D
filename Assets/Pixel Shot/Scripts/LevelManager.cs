using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// Level Data Class for Serilization
[System.Serializable]
class LevelData
{
    public List<ItemData> obstacleDatas;
    public List<ItemData> pixelDatas;
}
/// Level Data Struct for Serilization
[System.Serializable]
struct ItemData
{
    public int itemIndex;
    public Vector3 position;
    public Quaternion rotation;
}
/// Check for LevelLoad
public enum LevelMode { UNLOAD, LOAD }

public class LevelManager : Singleton<LevelManager>
{
    public GameObject wallObstacleRoot;
    [Space]
    public List<GameObject> obstaclePrefabs;
    [Space]
    public GameObject obstaclesRoot;
    [Space]
    public List<GameObject> pixelPrefabs;
    [Space]
    public GameObject pixelsRoot;
    [Space]

    public GameObject ball;
    public GameObject ballSpawnArea;

    //Which level load
    [HideInInspector]
    public int levelIndex;

    [Header("Level Design")]
    public bool levelDesignMenuActive = false;
    public Button saveLevelButton;
    public Text inputFieldText;

    //For manage input manager 
    LevelMode levelMode = LevelMode.UNLOAD;

    /// ----------- Start Function------------- 

    void Start()
    {
        saveLevelButton.gameObject.SetActive(levelDesignMenuActive);
        inputFieldText.transform.parent.
            gameObject.SetActive(levelDesignMenuActive);
        levelIndex = GetLevelIndex();
        if (!levelDesignMenuActive)
            LoadLevel();
    }

    /// ----------- Event Function------------- 
    private void Update()
    {
        if (levelMode == LevelMode.LOAD)
        {
            if (pixelsRoot.transform.childCount == 0)
            {
                levelMode = LevelMode.UNLOAD;
                PredictionManager.instance.KillAll();
                DeleteGameOjects();
                levelIndex++;
                if (levelIndex == 3)
                {
                    levelIndex = 0;
                }
                LoadLevel();
            }
        }
    }
    /// -----------Functions-------------
    ///

    void DeleteGameOjects()
    {
        foreach (var item in GameObject.FindGameObjectsWithTag(Tags.BALL))
            Destroy(item);
        foreach (var item in GameObject.FindGameObjectsWithTag(Tags.OBSTACLE))
            Destroy(item);
        foreach (var item in GameObject.FindGameObjectsWithTag(Tags.PIXELOBJECT))
            Destroy(item);
    }


    public LevelMode GetLevelMode()
    {
        return levelMode;
    }
    public void SaveLevelIndex(int index)
    {
        PlayerPrefs.SetInt(Tags.LEVELINDEX, index);
    }

    int GetLevelIndex()
    {
        return PlayerPrefs.GetInt(Tags.LEVELINDEX, 0);
    }

    LevelData GetLevelData(int levelIndex)
    {
        var textAsset = Resources.Load<TextAsset>(levelIndex.ToString());
        LevelData levelData = JsonUtility.FromJson<LevelData>(textAsset.ToString());
        return levelData;
    }
    void SaveLevelData(LevelData levelData, string name)
    {
        string jsonPath = Path.Combine(Application.dataPath, "Resources/" + name + ".json");
        string json = JsonUtility.ToJson(levelData, true);
        File.WriteAllText(jsonPath, json);
    }

    ///Save level Data for game design mode 
    public void SaveLevelData()
    {
        List<ItemData> obstacleDatas = new List<ItemData>();

        foreach (Transform tr in obstaclesRoot.transform)
        {
            int index = 0; ;
            ItemData itemData;
            itemData.itemIndex = index;
            itemData.position = tr.position;
            itemData.rotation = tr.rotation;
            obstacleDatas.Add(itemData);
        }
        List<ItemData> pixelDatas = new List<ItemData>();

        foreach (Transform tr in pixelsRoot.transform)
        {
            int index = 0;
            ItemData itemData;
            itemData.itemIndex = index;
            itemData.position = tr.position;
            itemData.rotation = tr.rotation;
            pixelDatas.Add(itemData);
        }

        LevelData levelData = new LevelData
        {
            obstacleDatas = obstacleDatas,
            pixelDatas = pixelDatas
        };
        if (inputFieldText.text != "")
            SaveLevelData(levelData, inputFieldText.text);

    }
    //Level load from json 
    public void LoadLevel()
    {
        LevelData levelData = GetLevelData(levelIndex);
        CreateObstacles(levelData.obstacleDatas);
        CreatePixels(levelData.pixelDatas);
        CreateBall();
        levelMode = LevelMode.LOAD;
        StartCoroutine(CopyAllObjectToPredictionScene());

    }
    void CreateObstacles(List<ItemData> list)
    {
        foreach (var item in list)
        {
            GameObject obj = obstaclePrefabs[item.itemIndex];
            GameObject o = Instantiate(obj, item.position, item.rotation);
            o.transform.SetParent(obstaclesRoot.transform);
        }
    }
    void CreatePixels(List<ItemData> list)
    {
        foreach (var item in list)
        {
            GameObject obj = pixelPrefabs[item.itemIndex];
            GameObject o = Instantiate(obj, item.position, item.rotation);
            o.transform.SetParent(pixelsRoot.transform);
        }

    }
    public void CreateBall()
    {
        StartCoroutine(Ball());
    }
    IEnumerator Ball()
    {
        yield return new WaitForSeconds(0.3f);
        BallController.SpawnBall(ballSpawnArea.transform.position, ball);
    }
    IEnumerator CopyAllObjectToPredictionScene()
    {
        yield return new WaitForSeconds(1);
        PredictionManager.instance.CopyAllObstacles();
        PredictionManager.instance.CopyAllPixels();
    }




}
