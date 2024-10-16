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

    // ��� �����ؾ���
    [SerializeField] public GameObject Player;
    [SerializeField] public GameObject Mob;

    [Header("����Ʈ UI")]
    [SerializeField] private Image _hitEffectImage;
    [SerializeField] private float _fadeDuration = 1.0f;

    private Color _hitColor;

    private float shootTimeoutDelta = 0.2f;

    //�ϴ� �����ϰ� �̱�������
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
