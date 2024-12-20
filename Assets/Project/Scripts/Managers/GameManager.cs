using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject bulletProjectile;
    [SerializeField] private Transform muzzle;
    [SerializeField] float shootTimeout = 0.2f;
    [SerializeField] NavMeshSurface meshSurface;

    // 방식 변경해야함
    [SerializeField] public GameObject Player;
    [SerializeField] public GameObject Mob;

    [Header("이펙트 UI")]
    [SerializeField] private Image _hitEffectImage;
    [SerializeField] private float _fadeDuration = 1.0f;

    private Color _hitColor;

    private float shootTimeoutDelta = 0.2f;

    //일단 간단하게 싱글톤으로
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
        meshSurface.BuildNavMesh();
    }

    private void Start()
    {
        _hitColor = _hitEffectImage.color;
        _hitColor.a = 0f;
        _hitEffectImage.color = _hitColor;
    }

    // 실제 반동주는 기능 구현하자. 지금은 애니메이션 속도에 굉장히 묶여있다.
    // 에임 자체를 위쪽으로 옮겨버리면 되지 않을까??
    // 애니메이션보다는 화면 흔들림을 어떻게 잘 하면..
    public void Shoot(Vector3 target) // TODO: 한 번 클릭 시 바로 사출되는 방법으로 개선 필요. 일단 킵
    {
        /*shootTimeoutDelta += Time.deltaTime;
        if (shootTimeout > shootTimeoutDelta)
            return;

        shootTimeoutDelta = 0f;*/

        Vector3 aim = (target - muzzle.position).normalized;
        Instantiate(bulletProjectile, muzzle.position, Quaternion.LookRotation(aim, Vector3.up));
    }


    public void TakeDamageEffect()
    {
        _hitColor.a = 50f/255f;
        _hitEffectImage.color = _hitColor;
        StartCoroutine(FlashRed(_hitColor.a));
    }

    private IEnumerator FlashRed(float a)
    {
        float elapsedTime = 0f;

        while(elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            _hitColor.a = Mathf.Lerp(a, 0f, elapsedTime / _fadeDuration);
            _hitEffectImage.color = _hitColor;

            yield return null;
        }

        _hitColor.a = 0f;
        _hitEffectImage.color = _hitColor;
    }
}
