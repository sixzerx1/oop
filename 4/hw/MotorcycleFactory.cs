public class MotorcycleFactory : VehicleFactory
{
    public override IVehicle Create() => new Motorcycle();
}