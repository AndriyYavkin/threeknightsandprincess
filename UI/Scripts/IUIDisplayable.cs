using Godot;

namespace Game.UI;

public interface IUIDisplayable
{
    string GetTitleUI();
    string GetNameUI();
    //Texture2D GetIconUI();
    string GetDescriptionUI();
}