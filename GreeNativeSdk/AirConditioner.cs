namespace GreeNativeSdk;

public class AirConditioner
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string PrivateKey { get; set; }

    public string Address { get; set; }

    public override string ToString()
    {
        return $"AirConditionerModel{{Id={Id}, Name={Name}, PrivateKey={PrivateKey}, Address={Address}}}";
    }

    public override bool Equals(object obj)
    {
        var o = obj as AirConditioner;

        if (o == null)
        {
            return false;
        }

        return Id == o.Id &&
            Name == o.Name &&
            PrivateKey == o.PrivateKey &&
            Address == o.Address;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode()
            ^ Name.GetHashCode()
            ^ PrivateKey.GetHashCode()
            ^ Address.GetHashCode();
    }
}