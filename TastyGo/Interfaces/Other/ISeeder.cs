namespace TastyGo.Interfaces.Other
{
    public interface ISeeder
    { 
            Task up();
            Task down();
            string Description();
    }
}
