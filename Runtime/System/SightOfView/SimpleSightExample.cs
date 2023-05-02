using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Kit
{


    public interface ISightObserver
    {
        IReadOnlyList<ISightableObject> FindSight();
    }
    public interface ISightableObject
    {
        void OnSighted();
        void OnUnSight();
    }
 
    public class SimpleSightExample : MonoBehaviour 
    {  
        public List<ISightableObject> sighted = new List<ISightableObject>();
        
    }
}
