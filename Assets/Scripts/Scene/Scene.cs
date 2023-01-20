using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace NoMansLand.Scene
{
    [System.Serializable]
    public class SceneContext
    {
        //comps

        public SceneInput Input;
        public SceneCamera Camera;
        public SceneUI UI;
        public ScenePostProcessing PostProcessing;
        public ItemDatabase ItemDatabase;
        public GameConfig Config;
        
        //Gameplay

        [HideInInspector]
        public NetworkTeams Teams;
        
        [HideInInspector]
        public Gameplay Gameplay;
        
        [HideInInspector]
        public NetworkRunner Runner;
        
        //Player

        [HideInInspector]
        public PlayerRef LocalPlayerRef;
        
        [HideInInspector] 
        public PlayerRef ObservedPlayerRef;
        
        [HideInInspector] 
        public NetworkPlayer ObservedPlayer;
    }
    
    public class Scene : MonoBehaviour
    {
        public bool ContextReady { get; private set; }
        public bool IsActive { get; private set; }
        
        public SceneContext Context => context;
        
        [SerializeField] private SceneContext context;

        private bool _initialized;

        private List<SceneComponent> _sceneComponents = new(16);
        
        public T GetService<T>() where T : SceneComponent
        {
            for (int i = 0, count = _sceneComponents.Count; i < count; i++)
            {
                if (_sceneComponents[i] is T service)
                    return service;
            }

            return null;
        }

        protected IEnumerator Start()
        {
            if (_initialized == false)
            {
                yield break;
            }

            if (IsActive)
            {
                yield break;
            }
            
            
        //    AddService(Context.UI);

            yield return Activate();
        }
        

        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            
            ContextReady = true;
            
            CollectServices();
            OnInitialize();

            _initialized = true;
        }

        protected virtual void Update()
        {
            if (IsActive == false)
            {
                return;
            }

            OnTick();
        }
        
        public IEnumerator Activate()
        {
            if (!_initialized)
            {
                yield break;
            }

            yield return OnActivate();

            IsActive = true;
        }

        protected virtual void LateUpdate()
        {
            if (IsActive == false)
            {
                return;
            }

            OnLateTick();
        }
        
        protected virtual void CollectServices()
        {
            var services = GetComponentsInChildren<SceneComponent>(true);

            foreach (var service in services)
            {
                AddService(service);
            }
        }
        
        protected virtual IEnumerator OnActivate()
        {
            for (int i = 0; i < _sceneComponents.Count; i++)
            {
                _sceneComponents[i].Activate();
            }

            yield break;
        }
        
        protected virtual void OnDeactivate()
        {
            for (int i = 0; i < _sceneComponents.Count; i++)
            {
                _sceneComponents[i].DeActivate();
            }
        }
        
        protected virtual void OnInitialize()
        {
            for (int i = 0; i < _sceneComponents.Count; i++)
            {
                _sceneComponents[i].Init(this, Context);
            }
        }
        
        protected virtual void OnTick()
        {
            for (int i = 0, count = _sceneComponents.Count; i < count; i++)
            {
                _sceneComponents[i].Tick();
            }
        }

        protected virtual void OnLateTick()
        {
            for (int i = 0, count = _sceneComponents.Count; i < count; i++)
            {
                _sceneComponents[i].LateTick();
            }
        }
        
        protected void AddService(SceneComponent service)
        {
            if (service == null)
            {
                Debug.LogError($"Missing service");
                return;
            }

            if (_sceneComponents.Contains(service))
            {
                Debug.LogError($"Service {service.gameObject.name} already added.");
                return;
            }

            _sceneComponents.Add(service);

            if (_initialized)
            {
                service.Init(this, Context);
            }

            if (IsActive == true)
            {
                service.Activate();
            }
        }
    }
}
