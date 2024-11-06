using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum Monster
{
    Bug,
    Golem
}

public class SpawnManager : Singleton<SpawnManager> // TODO: �̱��� ����
{
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private List<GameObject> _spawnMonsters;
    public int SpawnCount { get; private set; }
    public int KillCount { get; private set; }
    

    public override void Init()
    {
        base.Init();
        SpawnCount = 0;
        KillCount = 0;
    }


    private void SpawnMonster(int monsterAmount, Monster monster) // ������, ����, 
    {
        SufflePoints();

        for (int i = 0; i < monsterAmount; ++i)
        {

        }
    }


    public void IncreaseKillCount()
    {
        ++KillCount;
        if(SpawnCount > 0 && SpawnCount == KillCount)

    }

    // 1. ���� ����
    // 1-1. �ϴ� ���۾����� ���� ���� �������� ť�� �־���� -> ���߿� ������ ��� ���� ����Ʈ ���� ���ָ� ��
    // 2. �ð� ���� ������Ʈ ���� (Ǯ�� ����) 
    // 2-1. �����ϸ� 10����, 60�� �Ŀ� 15����, 60�� �Ŀ� ���� 1����
    //
    //3. ���� ��, ų �� ���� (ų �� �ø��� �޼��� �����)

}
