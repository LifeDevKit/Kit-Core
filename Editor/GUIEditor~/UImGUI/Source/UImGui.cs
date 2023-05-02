using System;
using System.Linq;
using ImGuiNET;
using Kit.UIMGUI.Assets;
using Kit.UIMGUI.Platform;
using Kit.UIMGUI.Renderer;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace Kit.UIMGUI
{
    [InitializeOnLoad]
	public class InitIMGUI
	{ 
		static void Initialize()
		{
			//Debug.Log("Initialized");
			EditorApplication.delayCall -= Initialize; 
			UImGui.Instance.Initialize();
		}
		static InitIMGUI()
		{ 
			//Debug.Log("Wait Initialized");
			EditorApplication.delayCall += Initialize; 
		}
	}
	[CreateAssetMenu(fileName = "UImGUI Editor Settings", menuName = "KIT/Create ImGUI Setting", order = 1)] 
	public class UImGui : ScriptableSingleton<UImGui>
	{

		public static UImGui Instance
		{
			get
			{ 
				if (_instance == null)
				{ 
					_instance = Resources.Load<UImGui>("UImGUI Editor Settings");  
				} 
				return _instance;
			}
		}

		private static UImGui _instance; 
		private IRenderer _renderer;
		private IPlatform _platform;
		private CommandBuffer _renderCommandBuffer;

		[SerializeField]
		private Camera _camera
		{
			get
			{

				Camera cam = null;
				
				
				if (SceneView.lastActiveSceneView != null)
					cam = SceneView.lastActiveSceneView.camera;
				
				if (cam == null && SceneView.lastActiveSceneView == null)
				{
					cam = SceneView.GetWindow<SceneView>()?.camera;
					
				}

				if (cam == null && SceneView.sceneViews.Count == 0)
				{
					var view = SceneView.currentDrawingSceneView;
					cam = view.camera;
				}

				return cam;
			}
		}

		[SerializeField] private RenderImGui _renderFeature;

		[SerializeField]
		private RenderType _rendererType = RenderType.Mesh;

		[SerializeField] 
		private InputType _platformType = InputType.InputManager;

		[Tooltip("Null value uses default imgui.ini file.")]
		[SerializeField]
		private IniSettingsAsset _iniSettings = null;

		[Header("Configuration")]

		[SerializeField]
		private UIOConfig _initialConfiguration = new UIOConfig
		{
			ImGuiConfig = ImGuiConfigFlags.NavEnableKeyboard | ImGuiConfigFlags.DockingEnable,

			DoubleClickTime = 0.30f,
			DoubleClickMaxDist = 6.0f,

			DragThreshold = 6.0f,

			KeyRepeatDelay = 0.250f,
			KeyRepeatRate = 0.050f,

			FontGlobalScale = 1.0f,
			FontAllowUserScaling = false,

			DisplayFramebufferScale = Vector2.one,

			MouseDrawCursor = false,
			TextCursorBlink = false,

			ResizeFromEdges = true,
			MoveFromTitleOnly = true,
			ConfigMemoryCompactTimer = 1f,
		};

		[SerializeField]
		private UnityEvent<ImGuiIOPtr> _fontCustomInitializer;

		[SerializeField]
		private FontAtlasConfigAsset _fontAtlasConfiguration = null;

		[Header("Customization")]
		[SerializeField]
		private ShaderResourcesAsset _shaders = null;

		[SerializeField]
		private StyleAsset _style = null;

		[SerializeField]
		private CursorShapesAsset _cursorShapes = null;

		[SerializeField]
		private bool _doGlobalEvents = true; // Do global/default Layout event too.

		public CommandBuffer CommandBuffer => _renderCommandBuffer;
 

		private void QueueReload()
		{
			EditorApplication.update -= QueueReload;
			DeInitialize();    
			Initialize(); 
		}
		public void Reload()
		{  
			SceneView.beforeSceneGui -= OnUpdateDruingScene;
			EditorApplication.update += QueueReload;
		}

		public void SetUserData(System.IntPtr userDataPtr)
		{
			_initialConfiguration.UserData = userDataPtr;
			ImGuiIOPtr io = ImGui.GetIO();
			_initialConfiguration.ApplyTo(io);
		}
 

	 
		public void OnApplicationQuit()
		{
			DeInitialize();
			//Debug.Log(_context);
			EditorApplication.delayCall += DelayCall; 
		}
 

		/// <summary>
		/// 에디터 인스펙터 준비시점에 호출
		/// </summary>
		public void DelayCall()
		{
			SceneView.beforeSceneGui -= OnUpdateDruingScene; 
			Initialize();
			EditorApplication.delayCall -= DelayCall;
		} 

		public void Initialize()
		{ 
			Application.quitting -= OnApplicationQuit; 
			Application.quitting += OnApplicationQuit; 



			if (UImGuiUtility.Context != null) 
				return;
			
			void Fail(string reason)
			{ 
				throw new System.Exception($"[UImGUI] Failed to start: {reason}.");
			}

			if (_camera == null)
			{	
				// Debug.Log($"[UImGUI] Stuck, 카메라가 없습니다. 씬 뷰 상태 : " +
				//           $"$마지막 씬 뷰 {SceneView.lastActiveSceneView} " +
				//           $"$현재 씬 뷰 {SceneView.currentDrawingSceneView} " +
				//           $"씬 뷰 수 ${SceneView.sceneViews.Count}");
				return;
			}

			if (_renderFeature == null && RenderUtility.IsUsingURP())
			{
				//Debug.Log("[UImGUI] Stuck, _renderFeature null");
				return;
			} 
			if (UImGuiUtility.Context == null)
			{  
				//Debug.Log("[UImGUI] Context Created");
				UImGuiUtility.Context = UImGuiUtility.CreateContext();
			}
			_renderCommandBuffer = RenderUtility.GetCommandBuffer(Constants.UImGuiCommandBuffer);

			if (RenderUtility.IsUsingURP())
			{
#if HAS_URP
				_renderFeature.Camera = _camera;
#endif
				_renderFeature.CommandBuffer = _renderCommandBuffer;
			}
			else if (!RenderUtility.IsUsingHDRP())
			{
				_camera.AddCommandBuffer(CameraEvent.AfterEverything, _renderCommandBuffer);
			}

			UImGuiUtility.SetCurrentContext(UImGuiUtility.Context);

			ImGuiIOPtr io = ImGui.GetIO();

			_initialConfiguration.ApplyTo(io);
			_style?.ApplyTo(ImGui.GetStyle());

			UImGuiUtility.Context.TextureManager.BuildFontAtlas(io, _fontAtlasConfiguration, _fontCustomInitializer);
			UImGuiUtility.Context.TextureManager.Initialize(io);

			IPlatform platform = PlatformUtility.Create(_platformType, _cursorShapes, _iniSettings);
			SetPlatform(platform, io);
			if (_platform == null)
			{
				Fail(nameof(_platform));
			}

			SetRenderer(RenderUtility.Create(_rendererType, _shaders, UImGuiUtility.Context.TextureManager), io);
			if (_renderer == null)
			{
				Fail(nameof(_renderer));
			}

			if (_doGlobalEvents)
			{
				UImGuiUtility.DoOnInitialize(this);
			} 
			
#if UNITY_EDITOR
		 
				SceneView.beforeSceneGui += OnUpdateDruingScene;
		 
#endif
		}  
		public void DeInitialize()
		{ 
			SceneView.beforeSceneGui -= OnUpdateDruingScene;
			UImGuiUtility.SetCurrentContext(UImGuiUtility.Context);
			ImGuiIOPtr io = ImGui.GetIO();

			SetRenderer(null, io);
			SetPlatform(null, io);
 
			UImGuiUtility.Context.TextureManager.Shutdown();
			UImGuiUtility.Context.TextureManager.DestroyFontAtlas(io);

			if (RenderUtility.IsUsingURP())
			{
				if (_renderFeature != null)
				{
#if HAS_URP
					_renderFeature.Camera = null;
#endif
					_renderFeature.CommandBuffer = null;
				}
			}
			else
			{
				if (_camera != null)
				{
					_camera.RemoveCommandBuffer(CameraEvent.AfterEverything, _renderCommandBuffer);
				}
			}

			if (_renderCommandBuffer != null)
			{
				RenderUtility.ReleaseCommandBuffer(_renderCommandBuffer);
			}

			_renderCommandBuffer = null;

			if (_doGlobalEvents)
			{
				UImGuiUtility.DoOnDeinitialize(this);
			} 
			
			UImGuiUtility.SetCurrentContext(null); 
#if UNITY_EDITOR
	 
				SceneView.beforeSceneGui -= OnUpdateDruingScene; 
#endif
		}

 
		private bool isEditor;

		private void OnUpdateDruingScene(SceneView view)
		{ 
			isEditor = false; 
			Update();
			isEditor = true;
		}
		
		private void Update()
		{ 
 
			if (_camera == null && UImGuiUtility.Context == null) 
				this.Reload(); 
			
			
			UImGuiUtility.SetCurrentContext(UImGuiUtility.Context);
			ImGuiIOPtr io = ImGui.GetIO();

			Constants.PrepareFrameMarker.Begin(this);
			UImGuiUtility.Context.TextureManager.PrepareFrame(io); 
			_platform.PrepareFrame(io, _camera.pixelRect);
			ImGui.NewFrame();
#if !UIMGUI_REMOVE_IMGUIZMO
			ImGuizmoNET.ImGuizmo.BeginFrame();
#endif
			Constants.PrepareFrameMarker.End();

			Constants.LayoutMarker.Begin(this);
			try
			{
				if (_doGlobalEvents)
				{
					UImGuiUtility.DoLayout(this);
				} 
			}
			finally
			{
				ImGui.Render();
				Constants.LayoutMarker.End();
			}

			Constants.DrawListMarker.Begin(this);
			_renderCommandBuffer.Clear();
			_renderer.RenderDrawLists(_renderCommandBuffer, ImGui.GetDrawData());
			Constants.DrawListMarker.End();
		}

		private void SetRenderer(IRenderer renderer, ImGuiIOPtr io)
		{
			_renderer?.Shutdown(io);
			_renderer = renderer;
			_renderer?.Initialize(io);
		}

		private void SetPlatform(IPlatform platform, ImGuiIOPtr io)
		{
			_platform?.Shutdown(io);
			_platform = platform;
			_platform?.Initialize(io, _initialConfiguration, "Unity " + _platformType.ToString());
		}
	}
}