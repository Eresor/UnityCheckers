using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameSetupView : MonoBehaviour {

    public Slider NumRowsSlider;

    public Text NumRowsText;

    public Slider CheckersRowsSlider;

    public Text CheckersRowsText;

    public Toggle P1Human;

    public Toggle P2Human;

    static private readonly string numberOfRowsText = "Number of rows: ";

    static private readonly string numberOfCheckersText = "Rows of checkers: ";
    // Use this for initialization
    void Start () {

        NumRowsText.text = numberOfRowsText + (int)NumRowsSlider.value;
        NumRowsSlider.onValueChanged.AddListener(delegate { BoardSliderValueChanged(); });

        CheckersRowsText.text = numberOfCheckersText + (int)CheckersRowsSlider.value;
        CheckersRowsSlider.onValueChanged.AddListener(delegate { CheckersSliderChanged(); });

    }
	
	// Update is called once per frame
    public void BoardSliderValueChanged()
    {
        if(NumRowsSlider.value%2!=0)
        {
            NumRowsSlider.value += 1;
        }
        NumRowsText.text = numberOfRowsText + (int)NumRowsSlider.value;
    }

    public void CheckersSliderChanged()
    {
        CheckersRowsText.text = numberOfCheckersText + (int)CheckersRowsSlider.value;
    }
}
