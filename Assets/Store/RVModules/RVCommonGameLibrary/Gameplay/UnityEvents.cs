// Created by Ronis Vision. All rights reserved
// 05.03.2021.

using System;
using UnityEngine;
using UnityEngine.Events;

namespace RVModules.RVCommonGameLibrary.Gameplay
{
        
    [Serializable] public class StringUnityEvent : UnityEvent<string>
    {
    }
    
    [Serializable] public class IntUnityEvent : UnityEvent<int>
    {
    }
    
    [Serializable] public class FloatUnityEvent : UnityEvent<float>
    {
    }
    
    [Serializable] public class Vector2UnityEvent : UnityEvent<Vector2>
    {
    }
    
    [Serializable] public class Vector3UnityEvent : UnityEvent<Vector3>
    {
    }
    
    [Serializable] public class GameObjectUnityEvent : UnityEvent<GameObject>
    {
    }

    [Serializable] public class TransformUnityEvent : UnityEvent<Transform>
    {
    }

    [Serializable] public class BehaviourUnityEvent : UnityEvent<Behaviour>
    {
    }

    [Serializable] public class ComponentUnityEvent : UnityEvent<Component>
    {
    }

    [Serializable] public class ColliderUnityEvent : UnityEvent<Collider>
    {
    }
    
    [Serializable] public class CollisionUnityEvent : UnityEvent<Collision>
    {
    }

    [Serializable] public class AudioSourceUnityEvent : UnityEvent<AudioSource>
    {
    }
}