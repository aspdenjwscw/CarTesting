using UnityEngine;

public class SettingMenu : MonoBehaviour
{
    public MenuButtons menuButtons;
    public GameObject buttonsParent;

    void Awake()
    {
        //Make a list of button childs using Get Component
        //Foreach(button child ) 
        // get the buttons child textMeshPro using Get Component
        //Change the buttons textMeshPro to information from persistant memory. Get stuff from Memory

        //Set a default incase there is nothing in persistant memory.
        //Make it update the key in other classes where the menu button saves to persistant memory --> MenuButtons.cs Script


    }

    //Button Saver Function
    //Maybe combine with Button Changer Function
    //Get the button that was pressed, and what key was pressed after, and adding the change into persistant memory

    //Button Changer Function
    //This can change what shows on the buttons. It will be sent information of the button 
    //and what the button has had a change, and update that button based on what is changed in memory




    //Make a function to send the cs file to MenuButtons, or just manually make a public variable in MenuButtons
    


}
