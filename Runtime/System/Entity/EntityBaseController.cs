using System;
using UnityEngine; 

[RequireComponent(typeof(KitEntity))]
public abstract class EntityBaseController : MonoBehaviour
{
    public KitEntity entity;
    
    public void Awake()
    {
        entity ??= GetComponent<KitEntity>(); 
    }  
}