

using System;
using System.Collections.Generic;
using UnityEngine;

public class UniAndroidQueryPermission : MonoBehaviour
{
    public static Queue<AndroidPermission> permissionQueue = new Queue<AndroidPermission>();
    public static Action allPermissionRequested; 

    public static void RequestPermissions(List<AndroidPermission> listPermission)
    {
        foreach (AndroidPermission androidPermission in listPermission)
        {
            permissionQueue.Enqueue(androidPermission);
        }
        allPermissionRequested = delegate
        {
        };
        RequestPermission();
    }

    static void RequestPermission()
    {
        if (permissionQueue.Count > 0)
        {
            var permission = permissionQueue.Dequeue();
            UniAndroidPermission.RequestPermission(permission, new Action(RequestPermission), new Action(RequestPermission));
        }
        else
        {
            allPermissionRequested();
        }
    }
}


