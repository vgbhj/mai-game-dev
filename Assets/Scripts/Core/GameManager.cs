using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TerrainGenerator terrainGen;
    public ObstacleSpawner obstacleSpawner;
    public UpgradeSpawner upgradeSpawner;
    public PathValidator pathValidator;

    public Transform robot;
    public Transform finishPoint;

    public GameObject winScreen;
    public GameObject loseScreen;

    private bool gameOver = false;

    void Start()
    {
        GenerateLevel();

        var energy = robot.GetComponent<EnergySystem>();
        energy.OnEnergyDepleted += OnLose;
    }

    void GenerateLevel()
    {
        int seed = Random.Range(0, 99999);
        int attempts = 0;

        do
        {
            terrainGen.Generate(seed + attempts);
            PlaceStartAndFinish();
            attempts++;
        }
        while (!pathValidator.HasPath(robot.position, finishPoint.position)
               && attempts < 20);

        obstacleSpawner.Spawn();
        upgradeSpawner.Spawn();
    }

    void PlaceStartAndFinish()
    {
        Terrain t = terrainGen.terrain;
        TerrainData td = t.terrainData;

        Vector3 startPos = t.transform.position + new Vector3(20, 0, 20);
        startPos.y = t.SampleHeight(startPos) + 1f;
        robot.position = startPos;

        Vector3 endPos = t.transform.position + new Vector3(
            td.size.x - 20, 0, td.size.z - 20
        );
        endPos.y = t.SampleHeight(endPos) + 1f;
        finishPoint.position = endPos;
    }

    void OnLose()
    {
        if (gameOver) return;
        gameOver = true;
        loseScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnWin()
    {
        if (gameOver) return;
        gameOver = true;
        winScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
