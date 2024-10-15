using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject bulletProjectile;
    [SerializeField] private Transform muzzle;
    [SerializeField] float shootTimeout = 0.2f;
    [SerializeField] NavMeshSurface meshSurface;

    // ��� �����ؾ���
    [SerializeField] public GameObject Player;
    [SerializeField] public GameObject Mob;

    private float shootTimeoutDelta = 0.2f;

    //�ϴ� �����ϰ� �̱�������
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
        meshSurface.BuildNavMesh();
    }

    // ���� �ݵ��ִ� ��� ��������. ������ �ִϸ��̼� �ӵ��� ������ �����ִ�.
    // ���� ��ü�� �������� �Űܹ����� ���� ������??
    // �ִϸ��̼Ǻ��ٴ� ȭ�� ��鸲�� ��� �� �ϸ�..
    public void Shoot(Vector3 target) // TODO: �� �� Ŭ�� �� �ٷ� ����Ǵ� ������� ���� �ʿ�. �ϴ� ŵ
    {
        /*shootTimeoutDelta += Time.deltaTime;
        if (shootTimeout > shootTimeoutDelta)
            return;

        shootTimeoutDelta = 0f;*/

        Vector3 aim = (target - muzzle.position).normalized;
        Instantiate(bulletProjectile, muzzle.position, Quaternion.LookRotation(aim, Vector3.up));
    }
}
