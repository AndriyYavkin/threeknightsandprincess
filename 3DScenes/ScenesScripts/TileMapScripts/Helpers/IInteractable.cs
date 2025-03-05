using Game.HelperCharacters;

namespace Game.ScenesHelper;

/// <summary>
/// Defines if Node can be Interacted with: can be entered, picked up, used ...
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Called when the Node is interacted with.
    /// </summary>
    /// <param name="character">The character interacting with the Node.</param>
    void OnInteract(CharacterHeroTemplate character);
}