using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TextInput : MonoBehaviour
{
    private TMP_Text myText;
    // public string initialText;
    private string currentInput;

    public AudioSource keySound;
    public AudioSource blockSound;
    public AudioSource destroySound;

    private bool isNewLineEmpty = false;

    public string[] keywords;

    public bool isPlayer = false;
    private bool canDestroy = false;

    public static Action OnTextInput;

    private void Start()
    {
        myText = GetComponent<TMP_Text>();
        myText.alignment = TextAlignmentOptions.Top;

        // myText.text = initialText;
        OnTextInput();
        if (isPlayer) CheckKeywords();
    }

    void Update()
    {
        if (isPlayer) InputText();
    }

    private void InputText()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\b' && myText.text.Length > 0)
            {
                DeleteText();
                keySound.Play();
            }
            else if (c == '\n' || c == '\r')
            {
                if (!GetComponent<PlayerController>().isJumping) return;
                if (isNewLineEmpty) return;
                currentInput += '\n';
                myText.text += currentInput;
                isNewLineEmpty = true;
                keySound.Play();
            }
            else if (char.IsLetterOrDigit(c) || c == ' ' || char.IsPunctuation(c) || char.IsSymbol(c))
            {
                currentInput += c;
                // print(WillCauseOverlap(GetSizeForText(myText.text + currentInput)));
                if (!WillCauseOverlap(GetSizeForText(myText.text + currentInput)))
                {
                    myText.text += currentInput;
                    OnTextInput();
                    isNewLineEmpty = false;
                    keySound.Play();
                }
                else blockSound.Play();
            }

            CheckKeywords();
            currentInput = "";
        }
    }

    private Vector2 newCenter;
    bool WillCauseOverlap(Vector2 newSize)
    {
        newCenter = myText.transform.position;
        return Physics2D.OverlapBox(newCenter, newSize, 0, LayerMask.GetMask("Wall"));
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireCube(newCenter, GetSizeForText(myText.text + currentInput));
    // }

    Vector2 GetSizeForText(string text)
    {
        List<GameObject> colliders = GetComponent<DynamicCollider>().colliders;
        Vector2 colliderSize = colliders[colliders.Count - 1].GetComponent<BoxCollider2D>().size;
        return new Vector2(colliderSize.x, colliderSize.y / 4);
    }

    private void CheckKeywords()
    {
        foreach (var keyword in keywords)
        {
            if (myText.text.ToUpper().Contains(keyword.ToUpper()))
            {
                ApplySpecialEffect(keyword);
            }
            else
            {
                CloseSpecialEffect(keyword);
            }
        }
    }

    private void ApplySpecialEffect(string keyword)
    {
        switch (keyword.ToUpper())
        {
            case "DESTROY":
                if (!canDestroy)
                {
                    string highlightedText =
                        HighlightKeywordInText(myText.text, keyword, ColorUtility.ToHtmlStringRGBA(Color.red));
                    myText.text = highlightedText;
                }
                canDestroy = true;
                break;
        }
    }

    private void CloseSpecialEffect(string keyword)
    {
        switch (keyword.ToUpper())
        {
            case "DESTROY":
                canDestroy = false;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (canDestroy)
        {
            if (col.collider == null) return;
            if (col.transform.GetComponent<TextInput>() == null) return;
            if (tag.Equals("Player")) destroySound.Play();
            if (!col.transform.GetComponent<TextInput>().isPlayer)
            {
                col.transform.GetComponent<TextInput>().DeleteText();
            }
        }
    }

    private void DeleteText()
    {
        // print(myText.GetParsedText());
        myText.text = myText.GetParsedText().Substring(0, myText.GetParsedText().Length - 1);
        // myText.text += currentInput;
        OnTextInput();
    }

    private string HighlightKeywordInText(string text, string keyword, string colorCode)
    {
        string openTag = $"<color=#{colorCode}>";
        string closeTag = "</color>";

        // 对于每次关键字出现的情况，插入颜色标签
        int index = text.IndexOf(keyword, StringComparison.OrdinalIgnoreCase);
        while (index != -1)
        {
            text = text.Insert(index, openTag);
            index += openTag.Length + keyword.Length; // 更新索引位置至关键字之后
            text = text.Insert(index, closeTag);
            index += closeTag.Length; // 更新索引位置至关闭标签之后
            index = text.IndexOf(keyword, index, StringComparison.OrdinalIgnoreCase); // 查找下一次关键字出现
        }

        return text;
    }
}
