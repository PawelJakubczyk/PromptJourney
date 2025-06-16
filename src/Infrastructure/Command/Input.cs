namespace mjCommandGenerator;

public class Input
{
    public string Value { get; init; }
    public double Weight { get; init; }

    public Input(string value, double weight)
    {
        Value = value;
        Weight = weight;
    }
}
