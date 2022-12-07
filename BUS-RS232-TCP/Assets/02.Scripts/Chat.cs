using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Chat : MonoBehaviour
{
    public static Chat instance;
	void Awake() => instance = this;

	[Header("스크롤뷰")]
	public ScrollRect ChatScrollRect;
	public RectTransform ChatContent;
	public Text ChatText;

	[Header("인풋필드")]
	public TMP_InputField SendInput;

	[Header("Data")]
	[SerializeField] string temp;

	int count = 0;

	public void ShowMessage(string data)
	{
		ChatText.text += ChatText.text == "" ? data : "\n" + data;
		
		Fit(ChatText.GetComponent<RectTransform>());
		Fit(ChatContent);

        temp = data;

		int kkk;

        // true 면 int가 됐다.
		if (int.TryParse(data, out kkk))
		{
			if (kkk == 1)
            {
				Server.isStartList[count] = true;
				count++;
            }
        }

		if (int.TryParse(data, out kkk))
        {
			if (kkk == 4)
            {
				GameManager.Instance.startBtn = true;
				Debug.Log("dkfakdjfhakjdhfk");
			}
        }
	}

	void Fit(RectTransform Rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);
}