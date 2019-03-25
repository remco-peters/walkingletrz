using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public struct MyGuard<T> where T : class {
        private readonly System.Action<T> Enable;
        private readonly System.Action<T> Disable;
        private readonly System.Func<T, bool> IsEnabled;

        public MyGuard(System.Action<T> Enable, System.Action<T> Disable, System.Func<T, bool> IsEnabled) {
            Assert.IsNotNull(Enable);
            Assert.IsNotNull(Disable);
            Assert.IsNotNull(IsEnabled);

            this.Enable = Enable;
            this.Disable = Disable;
            this.IsEnabled = IsEnabled;
        }

        public void Execute(T Context, System.Action Lambda) {
            Assert.IsNotNull(Context);
            Assert.IsNotNull(Lambda);

            // Test if we were enabled
            bool WasEnabled = IsEnabled(Context);

            // If enabled, disable
            if (WasEnabled) {
                Disable(Context);
            }

            // Check if we are actually disabled
            Assert.IsFalse(IsEnabled(Context));

            // Do the work
            Lambda();

            // Check if we are still disabled
            Assert.IsFalse(IsEnabled(Context));

            // If we were enabled before, reenable
            if (WasEnabled) {
                Enable(Context);
            }

            // Verify that the old state matches the new state
            Assert.AreEqual(WasEnabled, IsEnabled(Context));
        }
    }

// Abstract base for a single trigger/collision delegate
    public abstract class MyTriggerDelegateBase<T> where T : class {
        public abstract void Enter(object Context, T C);
        public abstract void Exit(object Context, T C);
    }

// Implementation for a single trigger/collision delegate
    public class MyTriggerDelegateImpl<S, T> : MyTriggerDelegateBase<T>
        where S : class
        where T : class {
        public event UnityAction<S, T> OnEnter;
        public event UnityAction<S, T> OnExit;

        public override void Enter(object Context, T C) {
            Assert.IsNotNull(C);
            Assert.AreEqual(Context.GetType(), typeof(S));

            if (OnEnter != null) {
                OnEnter((S)Context, C);
            }
        }

        public override void Exit(object Context, T C) {
            Assert.IsNotNull(C);
            Assert.AreEqual(Context.GetType(), typeof(S));

            if (OnExit != null) {
                OnExit((S)Context, C);
            }
        }
    }

// This class provides a generic Enter/Exit, as well as a per-class Enter/Exit delegate mechanism
    public class MyTriggerDelegates<T> where T : class {
        public event UnityAction<T> OnEnter;
        public event UnityAction<T> OnExit;

        protected readonly Dictionary<System.Type, MyTriggerDelegateBase<T>> Delegates = new Dictionary<System.Type, MyTriggerDelegateBase<T>>();

        public bool TryGetValue(System.Type Key, out MyTriggerDelegateBase<T> Value) {
            return Delegates.TryGetValue(Key, out Value);
        }

        public bool HasDelegates {
            get {
                return Delegates.Count != 0;
            }
        }

        public MyTriggerDelegateImpl<S, T> Get<S>() where S : MyMonoBehaviour {
            System.Type Type = typeof(S);

            MyTriggerDelegateBase<T> D;

            if (!TryGetValue(Type, out D)) {
                D = new MyTriggerDelegateImpl<S, T>();

                Delegates.Add(Type, D);
            }
            else Assert.AreEqual(D.GetType(), typeof(MyTriggerDelegateImpl<S, T>));

            return (MyTriggerDelegateImpl<S, T>)D;
        }

        private void ExecuteDelegates(GameObject GameObject, UnityAction<MyMonoBehaviour, MyTriggerDelegateBase<T>> Lambda) {
            Assert.IsNotNull(GameObject);
            Assert.IsNotNull(Lambda);

            if (!HasDelegates) {
                return; // Early out
            }

            MyMonoBehaviour[] Components = GameObject.GetComponents<MyMonoBehaviour>();

            foreach (MyMonoBehaviour Component in Components) {
                System.Type Type = Component.GetType();

                while (Type != typeof(MyMonoBehaviour)) {
                    MyTriggerDelegateBase<T> D;

                    if (TryGetValue(Type, out D)) {
                        Lambda(Component, D);
                    }

                    Type = Type.BaseType;
                }
            }
        }

        public void Enter(GameObject GameObject, T C) {
            Assert.IsNotNull(GameObject);
            Assert.IsNotNull(C);

            ExecuteDelegates(GameObject, (MyMonoBehaviour Context, MyTriggerDelegateBase<T> D) => {
                D.Enter(Context, C);
            });

            if (OnEnter != null) {
                OnEnter(C);
            }
        }

        public void Exit(GameObject GameObject, T C) {
            Assert.IsNotNull(GameObject);
            Assert.IsNotNull(C);

            ExecuteDelegates(GameObject, (MyMonoBehaviour Context, MyTriggerDelegateBase<T> D) => {
                D.Exit(Context, C);
            });

            if (OnExit != null) {
                OnExit(C);
            }
        }
    }

    public class MyMonoBehaviour : MonoBehaviour
    {
        // Wrapping a lambda with this guard ensures that the passed in GameObject is inactive during the execution of the lambda
        static readonly MyGuard<GameObject> EnsureInactiveGuard = new MyGuard<GameObject>((GameObject GameObject) => {
            GameObject.SetActive(true);
        }, (GameObject GameObject) => {
            GameObject.SetActive(false);
        }, (GameObject GameObject) => {
            return GameObject.activeSelf;
        });
    
        #region Spawning
        // These functions allow us to spawn GameObjects using an optional delegate for initialization prior to Awake and Start being called
        static public T Spawn<T>(T ComponentClass, MyMonoBehaviour ParentComponent, UnityAction<T> Initializer = null) where T : MyMonoBehaviour {
            Assert.IsNotNull(ParentComponent);

            return Spawn(ComponentClass, ParentComponent.gameObject, Initializer);
        }

        // Spawn class-restricted GameObject
        static public T Spawn<T>(T ComponentClass, GameObject Parent, UnityAction<T> Initializer = null) where T : MyMonoBehaviour {
            Assert.IsNotNull(ComponentClass);
            Assert.IsNotNull(Parent);

            T Instance = null;

            EnsureInactiveGuard.Execute(ComponentClass.gameObject, () => {
                Assert.IsFalse(ComponentClass.gameObject.activeSelf);

                Instance = Instantiate(ComponentClass, Parent.transform);

                Initializer?.Invoke(Instance);

                Instance.gameObject.SetActive(true);
            });

            return Instance;
        }

        static public T Spawn<T>(T ComponentClass, UnityAction<T> Initializer = null) where T : MyMonoBehaviour {
            Assert.IsNotNull(ComponentClass);

            T Instance = null;

            EnsureInactiveGuard.Execute(ComponentClass.gameObject, () => {
                Assert.IsFalse(ComponentClass.gameObject.activeSelf);

                Instance = Instantiate(ComponentClass);

                if (Initializer != null) {
                    Initializer(Instance);
                }

                Instance.gameObject.SetActive(true);
            });

            return Instance;
        }

        static public GameObject Spawn(GameObject ObjectClass, MyMonoBehaviour ParentComponent, UnityAction<GameObject> Initializer = null) {
            Assert.IsNotNull(ParentComponent);

            return Spawn(ObjectClass, ParentComponent.gameObject, Initializer);
        }

        // Spawn generic GameObject
        static public GameObject Spawn(GameObject ObjectClass, GameObject Parent, UnityAction<GameObject> Initializer = null) {
            Assert.IsNotNull(ObjectClass);
            Assert.IsNotNull(Parent);

            GameObject Instance = null;

            EnsureInactiveGuard.Execute(ObjectClass, () => {
                Instance = Instantiate(ObjectClass, Parent.transform);

                if (Initializer != null) {
                    Initializer(Instance);
                }

                Instance.gameObject.SetActive(true);
            });

            return Instance;
        }

        #endregion

        #region Triggers
    
        // This mechanism allows us to assign generic and class-specific delegates for collision/trigger events with guaranteed type-safety
        public readonly MyTriggerDelegates<Collision> CollisionDelegates = new MyTriggerDelegates<Collision>();
        public readonly MyTriggerDelegates<Collider> TriggerDelegates = new MyTriggerDelegates<Collider>();

        protected virtual void OnTriggerEnter(Collider c) {
            TriggerDelegates.Enter(c.gameObject, c);
        }

        protected virtual void OnTriggerExit(Collider c) {
            TriggerDelegates.Exit(c.gameObject, c);
        }

        protected virtual void OnCollisionEnter(Collision c) {
            CollisionDelegates.Enter(c.gameObject, c);
        }
        protected virtual void OnCollisionExit(Collision c) {
            CollisionDelegates.Exit(c.gameObject, c);
        }
        #endregion

        #region Coroutines
        // Wraps a coroutine in a lambda
        protected void StartCoroutine(System.Func<IEnumerator> Action) {
            Assert.IsNotNull(Action);

            StartCoroutine(Action());
        }
        #endregion
    }
}