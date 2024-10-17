
public class NPCSpriteController : CharacterSpriteController
{
    public int skinColor;

    private void Start()
    {
        HeroEditor.instance.SetCharacterSpriteProperties((CharacterSpriteController)this, 2, 2, 2, 2);
    }
}
