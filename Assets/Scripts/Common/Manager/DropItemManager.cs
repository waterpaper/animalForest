﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemManager : SingletonMonoBehaviour<DropItemManager>
{
    //드랍 아이템박스 프리팹입니다.
    public GameObject dropItemBoxPrefab;
    //드랍 아이템 박스를 저장할 위치
    public GameObject dropItemBoxPoolListLocation;
    //드랍 아이템 박스를 미리 선언해두는 리스트입니다
    public List<GameObject> dropItemBoxPoolList;
    //미리 선언할 드랍 아이템 박스의 갯수입니다.
    public int maxDropItem;

    void Awake()
    {
        if (instance != this)
        {
            Destroy(this.gameObject);
        }

        //상자를 on, off하지 않기 위해 미리 세팅합니다
        for (int i = 0; i < maxDropItem; i++)
        {
            var obj = Instantiate<GameObject>(dropItemBoxPrefab, dropItemBoxPoolListLocation.transform);
            obj.name = dropItemBoxPrefab.name + i.ToString("00");
            //비활성화
            obj.SetActive(false);

            dropItemBoxPoolList.Add(obj);
        }
    }

    public void DropItemSetting(int enemyID, Transform enemyTransform)
    {
        //몬스터가 드랍하는 상자를 세팅하는 함수입니다.
        EnemyDropItemTable dropItemTableTemp;
        List<Item> dropItemListTemp = new List<Item>();
        int dropMoney = 0;

        for (int i = 0; i < maxDropItem; i++)
        {
            if (dropItemBoxPoolList[i].activeSelf == true) continue;

            dropItemTableTemp = DataManager.instance.GetTableData<EnemyDropItemTable>(TableDataKind.TableDataKind_EnemyDropItem, enemyID);

            if (dropItemTableTemp != null)
            {
                dropMoney = Random.Range(dropItemTableTemp.DropMoneyMin, dropItemTableTemp.DropMoneyMax);
                float randIdx = Random.Range(0, 100);

                for (int j = 0; j < dropItemTableTemp.EnemyDropItemDataList.Count; j++)
                {

                    if (randIdx < dropItemTableTemp.EnemyDropItemDataList[j].DropItemPercent)
                    {
                        Item itemTemp = DataManager.instance.ItemSetting(dropItemTableTemp.EnemyDropItemDataList[j].DropItemId);

                        if (itemTemp == null) continue;
                        dropItemListTemp.Add(itemTemp);
                    }

                }

                dropItemBoxPoolList[i].GetComponent<DropItem>().ItemSetting(dropMoney, dropItemListTemp);
                dropItemBoxPoolList[i].transform.position = enemyTransform.position;

                dropItemBoxPoolList[i].SetActive(true);

                break;
            }
        }
    }
}
