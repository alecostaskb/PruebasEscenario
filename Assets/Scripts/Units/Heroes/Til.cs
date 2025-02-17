public class Til : BaseHero
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        UnitName = "Til";

        CurrentAction = Action.Walking;
        ActionPoints = (int)Action.Walking;
        RemainingActionPoints = ActionPoints;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}