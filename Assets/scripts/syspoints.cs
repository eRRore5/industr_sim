using System.Collections.Generic;
using UnityEngine;

public class syspoints : MonoBehaviour
{
    private int totalScore = 0;
    // Метод для расчета очков
    public void CalculateScore(List<ObjectScript> placedObjects)
    {
        foreach (ObjectScript objScript in placedObjects)
        {
            int scoreToAdd = CalculateScoreForObject(objScript, placedObjects);
            totalScore += scoreToAdd;
        }
        Debug.Log("Total Score: " + totalScore);
        // Здесь вы можете обновить ваш UI для отображения общего счета
    }

    // Метод для расчета очков для отдельного объекта
    private int CalculateScoreForObject(ObjectScript objScript, List<ObjectScript> placedObjects)
    {
        int score = 0;
        foreach (ObjectScript otherObj in placedObjects)
        {
            if (otherObj != objScript)
            {
                float distance = Vector3.Distance(objScript.transform.position, otherObj.transform.position);
                // Примерная логика расчета очков
                if (distance <= 5f)
                {
                    score += 2; // Добавляем 2 очка, если расстояние меньше или равно 5
                }
                else if (distance <= 7f)
                {
                    score -= 2; // Вычитаем 2 очка, если расстояние меньше или равно 7
                }
            }
        }
        return score;
    }
}
