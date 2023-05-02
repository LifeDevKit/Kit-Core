using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

/*
 * 현재 방향성
 * 대부분의 게임을 섥
 */

  

public interface IEntityController
{
    void Update();
    void FixedUpdate();
};

public interface IKitEntity
{ 
    Vector3 Position { get; set; }
    Transform Root { get; }
}
public abstract class AbstractKitEntity : MonoBehaviour, IKitEntity
{ 
    public virtual Vector3 Position { get; set; }
    public virtual Transform Root
    {
        get
        {
            return this.transform;
        }
    }  
 
}
 



