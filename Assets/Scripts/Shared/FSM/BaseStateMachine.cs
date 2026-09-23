using System;
using System.Collections.Generic;
using UnityEngine;

namespace BitorTools.FSM
{
    public class BaseStateMachine : MonoBehaviour
    {
        [SerializeField] private BaseState _initialState;
        private Dictionary<Type, Component> _cachedComponents;
        private void Awake()
        {
            CurrentState = _initialState;
            _cachedComponents = new Dictionary<Type, Component>();
        }

        public BaseState CurrentState { get; private set; }

        public void ChangeState(BaseState nextState)
        {
            CurrentState.ExitState(this);
            CurrentState = nextState;
            CurrentState.EnterState(this);
        }

        private void Update()
        {
            CurrentState.Execute(this);
        }

        /// <summary>
        /// Gets component while also caching the component in the state machine. If already cached, simply return the cached component.
        /// </summary>
        /// <typeparam name="T">Type of the Component to get.</typeparam>
        /// <returns></returns>
        public new T GetComponent<T>() where T : Component
        {
            if (_cachedComponents.ContainsKey(typeof(T)))
                return _cachedComponents[typeof(T)] as T;

            var component = base.GetComponent<T>();
            if (component != null)
            {
                _cachedComponents.Add(typeof(T), component);
            }
            return component;
        }

    }
}
