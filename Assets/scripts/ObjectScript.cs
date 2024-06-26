using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    public Material cantPlaceMaterial;
    public GameObject objectToBePlaced;
    private int buildingLayer;
    public bool _canPlace = false;
    private Renderer objectsRenderer;
    private Bounds objectBounds;
    public bool _collidingWithBuilding = false;
    private bool _objectPlaced = false;
    private Material originalMaterial;
    public int potentialPoints = 0;

    void Start()
    {
        buildingLayer = LayerMask.NameToLayer("building");
        objectsRenderer = GetComponent<Renderer>();
        objectBounds = objectsRenderer.bounds;
        originalMaterial = objectsRenderer.material;
    }

    void Update()
    {
        SetObjectMaterial();
    }

    private void SetObjectMaterial()
    {
        if (_canPlace && !_collidingWithBuilding || _objectPlaced)
        {
            objectsRenderer.material = originalMaterial;
        }
        else
        {
            objectsRenderer.material = cantPlaceMaterial;
        }
    }

    public void SetCanPlace(float normalValue)
    {
        if (normalValue == 1)
        {
            _canPlace = true;
        }
        else
        {
            _canPlace = false;
        }
    }

    public bool CanPlace()
    {
        return _canPlace;
    }

    public void SetObjectPlaced()
    {
        _objectPlaced = true;
        CalculateInteractions(transform.position); // Вызываем расчет после размещения
    }

    public void PlaceObject(Vector3 place, Quaternion rotation)
    {
        if (_canPlace && !_collidingWithBuilding)
        {
            GameObject createdObject = Instantiate(objectToBePlaced, place, rotation);
            ObjectScript createdObjectScript = createdObject.GetComponent<ObjectScript>();
            createdObjectScript.SetCanPlace(1f);
            createdObjectScript.SetObjectPlaced();
            createdObjectScript.originalMaterial = originalMaterial;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == buildingLayer)
        {
            _collidingWithBuilding = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == buildingLayer)
        {
            _collidingWithBuilding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == buildingLayer)
        {
            _collidingWithBuilding = false;
        }
    }

    public void CalculateInteractions(Vector3 position)
    {
        potentialPoints = 0;

        // Находим все коллайдеры в радиусе
        Collider[] colliders = Physics.OverlapSphere(position, 100f);

        foreach (Collider collider in colliders)
        {
            // Исключаем собственный коллайдер 
            if (collider != this.GetComponent<Collider>())
            {
                string otherTag = collider.gameObject.tag;

                // Проверяем наличие правил взаимодействия для текущего тега
                if (GameManager.instance.interactionParametersDict.ContainsKey(gameObject.tag))
                {
                    // Перебираем правила взаимодействия
                    foreach (InteractionParameters param in GameManager.instance.interactionParametersDict[gameObject.tag])
                    {
                        // Проверяем совпадение тегов
                        if (otherTag == param.interactingTag)
                        {
                            // Вычисляем расстояние между коллайдерами
                            float distance = Vector3.Distance(position, collider.ClosestPoint(position));

                            // Проверяем, находится ли объект в пределах заданного расстояния
                            if (distance <= param.distanceThreshold)
                            {
                                potentialPoints += param.pointsToAdd;
                            }
                        }
                    }
                }
            }
        }

        GameManager.instance.UpdatePointsUI(potentialPoints);
    }
}
