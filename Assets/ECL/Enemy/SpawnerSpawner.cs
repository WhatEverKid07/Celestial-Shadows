using UnityEngine;
using System.Collections.Generic;

public class RandomActivator : MonoBehaviour
{
    public List<GameObject> allObjects;
    public List<GameObject> activeObjects;
    public int numberToActivate = 3;

    private void Start()
    {
        ActivateRandomObjects();
    }

    public void ActivateRandomObjects()
    {
        activeObjects.Clear();
        List<GameObject> tempList = new List<GameObject>(allObjects);

        int count = Mathf.Min(numberToActivate, tempList.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, tempList.Count);
            GameObject selected = tempList[index];

            selected.SetActive(true);
            activeObjects.Add(selected);

            tempList.RemoveAt(index);
        }
    }
}
