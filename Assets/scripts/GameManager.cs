using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class InteractionParameters
{
    public string interactingTag;
    public float distanceThreshold;
    public int pointsToAdd;

public InteractionParameters(string tag, float distance, int points)
    {
        interactingTag = tag;
        distanceThreshold = distance;
        pointsToAdd = points;
    }
}

public class GameManager : MonoBehaviour
{
    public Text pointsText;
    public Text potentialPointsText;
    public Text scoreText; // Добавим текст для отображения оценки
    public Button finishButton; // Кнопка для завершения игры
    public Button exitButton; // Кнопка для выхода в главное меню

private int currentPoints = 0;
    public LayerMask groundLayer;
    public Material canPlaceMaterial;
    public Material cantPlaceMaterial;

    public GameObject[] objectPrefabs;
    public Button[] modelButtons;

    public float rotationSpeed = 5f;
    private Vector3 rotationStartPosition;

    public static GameManager instance;

    public Dictionary<string, List<InteractionParameters>> interactionParametersDict;
    public Dictionary<string, int> buildingCounts; // Количество зданий каждого типа

    private GameObject currentObjectInstance;
    private ObjectScript currentObjectScript;
    private Collider currentObjectCollider;
    private int currentPrefabIndex = 0;
    private float currentRotation = 0f;

    private bool isMenuActive = false; // Для отслеживания состояния меню

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        interactionParametersDict = new Dictionary<string, List<InteractionParameters>>();
        buildingCounts = new Dictionary<string, int>(); // Инициализация словаря

        pointsText.text = "Points: " + currentPoints;
        potentialPointsText.text = "Potential Points: 0";

        // Правила взаимодействия
        List<InteractionParameters> coffeeParams = new List<InteractionParameters>();
        coffeeParams.Add(new InteractionParameters("fire", 20f, -5));
        coffeeParams.Add(new InteractionParameters("water", 20f, 5));
        coffeeParams.Add(new InteractionParameters("coffee", 20f, 2));
        interactionParametersDict.Add("coffee", coffeeParams);

        List<InteractionParameters> fireParams = new List<InteractionParameters>();
        fireParams.Add(new InteractionParameters("fire", 20f, -10));
        fireParams.Add(new InteractionParameters("water", 20f, 5));
        fireParams.Add(new InteractionParameters("coffee", 20f, -5));
        interactionParametersDict.Add("fire", fireParams);

        List<InteractionParameters> waterParams = new List<InteractionParameters>();
        waterParams.Add(new InteractionParameters("fire", 20f, 5));
        waterParams.Add(new InteractionParameters("water", 20f, 10));
        waterParams.Add(new InteractionParameters("coffee", 20f, -5));
        interactionParametersDict.Add("water", waterParams);

        // Инициализация количества зданий
        buildingCounts["coffee"] = 7; // Пример количества
        buildingCounts["fire"] = 5;   // Пример количества
        buildingCounts["water"] = 15;

        // Настройка кнопок
        for (int i = 0; i < modelButtons.Length; i++)
        {
            int index = i;
            modelButtons[i].onClick.AddListener(() => ChangePrefab(index));
        }

        // Настройка кнопки "Завершить"
        finishButton.onClick.AddListener(ShowScore);

        // Настройка кнопки "Выход"
        exitButton.onClick.AddListener(ExitToMainMenu);
        exitButton.gameObject.SetActive(false); // Скрыть кнопку по умолчанию

        CreateObjectInstance();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, groundLayer))
        {
            if (currentObjectInstance != null)
            {
                currentObjectInstance.SetActive(true);
                currentObjectInstance.transform.position = hit.point;

                // Вращение объекта
                if (Input.GetMouseButton(1))
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        rotationStartPosition = hit.point;
                    }
                    Cursor.lockState = CursorLockMode.Locked;
                    float mouseXDelta = Input.GetAxis("Mouse X");
                    currentRotation += mouseXDelta * rotationSpeed;

                    Vector3 offset = new Vector3(-3, 0, -3);

                    currentObjectInstance.transform.position = rotationStartPosition + offset;
                    currentObjectInstance.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    Cursor.lockState = CursorLockMode.None;
                }

                // Проверка возможности размещения и расчет потенциальных очков
                if (currentObjectScript != null)
                {
                    if (CanPlaceObject(hit.point, hit.normal))
                    {
                        currentObjectScript.SetCanPlace(1f);
                        currentObjectScript.CalculateInteractions(hit.point); // Расчет при перемещении
                    }
                    else
                    {
                        currentObjectScript.SetCanPlace(0f);
                        currentObjectScript.potentialPoints = 0; // Сбрасываем, если нельзя разместить
                        UpdatePointsUI(0); // Обновляем UI
                    }
                }
            }

            // Размещение объекта
            if (Input.GetMouseButtonDown(0))
            {
                PlaceObject(hit.point, Quaternion.identity);
            }
        }
        else
        {
            if (currentObjectInstance != null)
            {
                currentObjectInstance.SetActive(false);
                currentObjectScript.potentialPoints = 0; // Сбрасываем, если вне зоны размещения
                UpdatePointsUI(0); // Обновляем UI
            }
        }

        // Обработка нажатия клавиши ESC для показа/скрытия меню
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    void ChangePrefab(int prefabIndex)
    {
        currentPrefabIndex = prefabIndex;
        DestroyObjectInstance();
        CreateObjectInstance();
    }

    void PlaceObject(Vector3 place, Quaternion rotation)
    {
        if (currentObjectScript.CanPlace())
        {
            Quaternion finalRotation = Quaternion.Euler(0, currentRotation, 0);
            currentObjectScript.PlaceObject(place, finalRotation);
            AddPoints(currentObjectScript.potentialPoints);
            currentObjectScript.potentialPoints = 0; // Обнуляем potentialPoints после добавления
        }
    }

    bool CanPlaceObject(Vector3 place, Vector3 surfaceNormal)
    {
        LayerMask buildingLayer = LayerMask.GetMask("building");

        Collider[] colliders = Physics.OverlapSphere(place, 0.1f, buildingLayer);

        if (colliders.Length > 0)
        {
            return false;
        }

        if (Physics.Raycast(place, -surfaceNormal, out RaycastHit hit, 100f, ~buildingLayer) &&
            Mathf.Abs(surfaceNormal.x) < 0.01f && Mathf.Abs(surfaceNormal.z) < 0.01f)
        {
            return !currentObjectScript._collidingWithBuilding;
        }

        return false;
    }

    void CreateObjectInstance()
    {
        currentObjectInstance = Instantiate(objectPrefabs[currentPrefabIndex]);
        currentObjectScript = currentObjectInstance.GetComponent<ObjectScript>();
        currentObjectCollider = currentObjectInstance.GetComponent<Collider>();
    }

    void DestroyObjectInstance()
    {
        if (currentObjectInstance != null)
        {
            Destroy(currentObjectInstance);
            currentObjectScript = null;
            currentObjectCollider = null;
        }
    }

    public void UpdatePointsUI(int potentialPoints)
    {
        potentialPointsText.text = "Potential Points: " + potentialPoints;
    }

    public void AddPoints(int points)
    {
        currentPoints += points;
        pointsText.text = "Points: " + currentPoints;
    }

    // Метод для расчета максимальных очков
    public int CalculateMaxPoints()
    {
        int maxPoints = 0;

        foreach (var buildingCount in buildingCounts)
        {
            string buildingTag = buildingCount.Key;
            int count = buildingCount.Value;

            if (interactionParametersDict.ContainsKey(buildingTag))
            {
                foreach (InteractionParameters param in interactionParametersDict[buildingTag])
                {
                    // Максимальные очки для одного здания умножаем на количество зданий этого типа
                    maxPoints += param.pointsToAdd * count;
                }
            }
        }

        return maxPoints;
    }

    // Метод для отображения оценки
    public void ShowScore()
    {
        int maxPoints = CalculateMaxPoints();
        float scorePercentage = (float)currentPoints / maxPoints * 100f;
        scorePercentage = Mathf.Clamp(scorePercentage, 0f, 100f); // Ограничиваем от 0 до 100
        scoreText.text = "Score: " + scorePercentage.ToString("F2") + "%";
    }

    // Метод для выхода в главное меню
    public void ExitToMainMenu()
    {
        // Добавьте здесь код для загрузки главного меню вашей игры
        // Например:
        // SceneManager.LoadScene("MainMenu");
    }

    // Метод для показа/скрытия меню
    void ToggleMenu()
    {
        isMenuActive = !isMenuActive;
        exitButton.gameObject.SetActive(isMenuActive);
    }
}