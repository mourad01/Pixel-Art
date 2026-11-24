using UnityEngine;

public class Tutorial3DMoveVox : MonoBehaviour
{
    void Start()
    {
        var componentsInChildren = ((Component)transform).GetComponentsInChildren<VoxCubeItem>();
        var voxCubeItems = componentsInChildren;
        foreach (VoxCubeItem voxCubeItem in voxCubeItems)
        {
            voxCubeItem.gameObject.AddComponent<Tutorial3DMoveVoxCube>();
        }
        transform.localScale = Vector3.one;
        Object.FindObjectOfType<TutorialWindow>().transform.localScale = Vector3.one;
    }
}


