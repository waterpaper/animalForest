﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SingleConversationUI : MonoBehaviour
{
    //초기 세팅상태인지 의미하는 변수입니다.
    public bool isSetting = false;

    //현재 문장리스르를 저장하는 리스트입니다.
    public List<int> singleConversationList;

    public void OnEnable()
    {
        if (!isSetting)
        {
            isSetting = true;
            return;
        }
        //캐릭터와 맞는 이미지로 교체합니다.
        transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = DataManager.instance.GetIconData(IconDataKind.IconDataKind_Character, PlayerManager.instance.Kind);
    }
    
    public void OkayButton()
    {
        //확인 버튼 클릭시 현재 리스트를 제거하고 맞는 설정의 문구를 불러오거나 종료합니다.
        if(singleConversationList.Count >= 0)  singleConversationList.RemoveAt(0);

        UIManager.instance.UISetting(UiKind.UIKind_SingleConversationPauseUI);

        if (singleConversationList.Count == 0) return;

        SingleConversationTable temp = DataManager.instance.GetTableData<SingleConversationTable>(TableDataKind.TableDataKind_SingleConversation, singleConversationList[0]);

        if (temp == null) return;

        if (temp.Type == 0)
            UIManager.instance.UISetting(UiKind.UIKind_SingleConversationUI);
        else if (temp.Type == 1)
            UIManager.instance.UISetting(UiKind.UIKind_SingleConversationPauseUI);

        TextChange(temp);
    }

    public void Setting(List<int> singleConversationListTemp, bool clearLIst)
    {
        if (clearLIst == true)
        {
            singleConversationList.Clear();
        }

        singleConversationListTemp.ForEach((listIndex) => {
            singleConversationList.Add(listIndex);
        });

        SingleConversationTable temp = DataManager.instance.GetTableData<SingleConversationTable>(TableDataKind.TableDataKind_SingleConversation ,singleConversationList[0]);

        if (temp == null) return;

        TextChange(temp);
    }

    public void TextChange(SingleConversationTable temp)
    {
        transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = temp.Text;
    }
}
