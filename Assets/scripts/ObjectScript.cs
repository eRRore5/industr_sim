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
        CalculateInteractions(transform.position); // �������� ������ ����� ����������
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

        // ������� ��� ���������� � �������
        Collider[] colliders = Physics.OverlapSphere(position, 100f);

        foreach (Collider collider in colliders)
        {
            // ��������� ����������� ��������� 
            if (collider != this.GetComponent<Collider>())
            {
                string otherTag = collider.gameObject.tag;

                // ��������� ������� ������ �������������� ��� �������� ����
                if (GameManager.instance.interactionParametersDict.ContainsKey(gameObject.tag))
                {
                    // ���������� ������� ��������������
                    foreach (InteractionParameters param in GameManager.instance.interactionParametersDict[gameObject.tag])
                    {
                        // ��������� ���������� �����
                        if (otherTag == param.interactingTag)
                        {
                            // ��������� ���������� ����� ������������
                            float distance = Vector3.Distance(position, collider.ClosestPoint(position));

                            // ���������, ��������� �� ������ � �������� ��������� ����������
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
