using UnityEngine;

public class ObjectRelations : MonoBehaviour
{
    [System.Serializable]
    public struct Relation
    {
        public GameObject objectType;
        public int relationshipValue;
    }

    public Relation[] relations;

    public int GetRelationshipValue(GameObject objectType)
    {
        foreach (var relation in relations)
        {
            if (relation.objectType == objectType)
            {
                return relation.relationshipValue;
            }
        }
        return 0; // Default relationship value
    }
}