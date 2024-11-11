using Legacy.Database;

public class PlayerTutorial
{
    public ushort hard_tutorial_state;
    public int menu_tutorial_state;
    public DatabaseDictionary<ushort> tutorials_steps;

    public void Read(PlayerProfileInstance player)
    {
        hard_tutorial_state = player.tutorial.hard_tutorial_state;
        menu_tutorial_state = player.tutorial.menu_tutorial_state;
        tutorials_steps = player.tutorial.tutorials_steps;
    }
}