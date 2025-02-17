public class Dasha : BaseHero
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        UnitName = "Dasha";

        CurrentAction = Action.Running;
        ActionPoints = (int)Action.Running;
        RemainingActionPoints = ActionPoints;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}