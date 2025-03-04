using Godot;

namespace UI;

public interface IUIDisplayable
{
    string GetTitleUI();
    string GetNameUI();
    //Texture2D GetIconUI();
    string GetDescriptionUI();
}