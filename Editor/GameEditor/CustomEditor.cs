using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Graphs;
using UnityEditor.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kit
{


    public interface IBomEditor
    { 
        SceneView currentActiveSceneView => SceneView.lastActiveSceneView;
        VisualElement sceneViewRootElement => currentActiveSceneView.rootVisualElement;  
        /// <summary>
        /// 씬뷰에 GUI를 그립니다.
        /// </summary>
        /// <param name="view"></param>
        void OnDuringSceneGUI(SceneView view);
    }
    /// <summary>
    /// 씬 에디터 툴을 쉽게 관리하기 위한 전략
    /// </summary>
    public abstract class CustomEditor : UnityEditor.Editor, IBomEditor
    {
        public virtual bool UseDuringScene
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Is Object Selected?
        /// </summary>
        public static bool Selected;
        public abstract void OnDuringSceneGUI(SceneView view);
        public SceneView CurrentSceneView => SceneView.lastActiveSceneView;

        protected VisualElement rootVisualElement
        {
            get
            {
                if (CurrentSceneView == null) return null;
                if (CurrentSceneView.rootVisualElement == null) return null;
                return  CurrentSceneView.rootVisualElement;
                
            }
        }

        protected VisualElement editorVisualElement;
 

        protected void Repaint()
        {
            SceneView.RepaintAll();
        }

        protected virtual VisualElement CreateEditorVisualElement()
        {
            var attributes = this.GetType().GetCustomAttribute(typeof(EditorToolAttribute), false);
            var toolbarAtt = attributes as EditorToolAttribute;  
            if (toolbarAtt == null) return new VisualElement();
 
            Debug.Log(toolbarAtt.targetType.Name);
            if (toolbarAtt.targetType == null)
            {
                return new Label("Not work");
            }
             
            
            return new VisualElement();
        }
 
        public virtual void OnEnable()
        {  
            Selected = true; 
            if(UseDuringScene)
               SceneView.duringSceneGui += OnDuringSceneGUI;
  
            if (editorVisualElement == null) 
                this.editorVisualElement = CreateEditorVisualElement();


            if (rootVisualElement != null && editorVisualElement != null)
            { 
                rootVisualElement.Add(editorVisualElement); 
            }
        }
 
        public virtual void OnDisable()
        {
            Selected = false;
            if(UseDuringScene)
               SceneView.duringSceneGui -= OnDuringSceneGUI; 
            
            if (editorVisualElement == null) 
                CreateEditorVisualElement();
            
            
            if(rootVisualElement != null&& editorVisualElement != null)
               rootVisualElement.Remove(editorVisualElement);
        }
        
        
    }
}