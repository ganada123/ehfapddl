using UnityEngine;
using UnityEngine.UI;
public class ScrollExample : MonoBehaviour
{
    public RectTransform content; // Contents
    public Scrollbar scrollbar; // controller

    // Values are declared as public for debugging.
    [Header("Debug")]
    public float scrollSize;
    public float scrollOffset;
    public Vector2 initialPos;

    // Start is called before the first frame update
    void Start()
    {
        // 뷰의 초기설정값 
        initialPos = content.anchoredPosition;
        // 모든 하위요소에서 RectTransform을 찾는다. 
        RectTransform[] childs = content.GetComponentsInChildren<RectTransform>();

        float total = 0;
        // 모든 하위요소의 높이 계산 
        foreach (RectTransform rect in childs)
        {
            // Content의 RectTransform은 배제
            if (content.GetInstanceID() != rect.GetInstanceID())
            {
                total += rect.rect.height;
            }
        }

        scrollOffset = total - content.rect.height;
        scrollSize = total / (childs.Length - 1);

        // set scrollbar 
        scrollbar.size = scrollSize / scrollOffset;
        scrollbar.numberOfSteps = Mathf.RoundToInt(scrollOffset / scrollSize);
        // add listener 
        scrollbar.onValueChanged.AddListener(OnScrollValueChanged);
    }

    /*
     * called when scrollbar's value changed
     */
    void OnScrollValueChanged(float _value)
    {
        float value = Mathf.Clamp(_value, 0.0f, 1.0f);

        // total 300 
        content.anchoredPosition = new Vector2(initialPos.x, initialPos.y + (value * scrollOffset));
    }
}