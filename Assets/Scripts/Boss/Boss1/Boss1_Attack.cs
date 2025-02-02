﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1_Attack : BossSkillMono, IBossAttack
{
    //보스 1의 공격을 처리하는 클래스입니다.
    private AttackCollider _attackCollider;

    private void Awake()
    {
        _attackCollider = transform.GetChild(0).GetComponent<AttackCollider>();
    }

    private void OnEnable()
    {
        //객체가 on되면 코루틴함수를 실행합니다.
        StartCoroutine(StartAttack());
    }

    public void Setting(float bossAtk, float factor, float range)
    {
        //데미지와 관련있는 변수를 세팅하며 콜라이더에 데미지를 설정해주는 함수입니다.
        Factor = factor;
        Atk = bossAtk;
        Range = range;

        _attackCollider.Setting(bossAtk * factor, range);
    }

    public IEnumerator StartAttack()
    {
        //일정시간이 지나면 공격상태를 on로 바꿔줍니다.
        yield return new WaitForSeconds(0.3f);

        IsAttacking = true;
        _attackCollider.gameObject.SetActive(true);
        StartCoroutine(DisableAttack());
    }

    public IEnumerator DisableAttack()
    {
        //일정시간이 지나면 공격중 상태를 false로 바꿔 끝난 상태로 변경해주는 함수입니다.
        yield return new WaitForSeconds(1.5f);

        IsAttacking = false;
        gameObject.SetActive(false);
    }
}
