using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum Monster
{
    Bug,
    Golem
}

public class SpawnManager : Singleton<SpawnManager> // TODO: 싱글톤 적용
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


    private void SpawnMonster(int monsterAmount, Monster monster) // 마리수, 종류, 
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

    // 1. 스폰 지역
    // 1-1. 일단 수작업으로 스폰 지역 만들어놓고 큐에 넣어놓기 -> 나중에 예전에 썼던 스폰 포인트 생성 해주면 됨
    // 2. 시간 별로 오브젝트 생성 (풀링 아직) 
    // 2-1. 시작하면 10마리, 60초 후에 15마리, 60초 후에 보스 1마리
    //
    //3. 생성 수, 킬 수 저장 (킬 수 올리는 메서드 만들기)

}
