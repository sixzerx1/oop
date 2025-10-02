public class CarFactory : VehicleFactory
{
    public override IVehicle Create() => new Car();
}