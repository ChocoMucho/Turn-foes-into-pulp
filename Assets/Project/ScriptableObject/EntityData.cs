using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity Data", menuName = "Scriptable Object/Entity Data")]
public class EntityData : ScriptableObject
{
    [SerializeField] private float hp;
    [SerializeField] private float damage;

    public float Hp { get { return hp; } }
    public float Damage { get { return damage; } }
}
