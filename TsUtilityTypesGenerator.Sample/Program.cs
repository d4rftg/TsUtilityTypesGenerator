using TsUtilityTypesGenerator.Attributes;

namespace TsUtilityTypesGenerator.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var pickA = new PickA { A = 100 };
        }
    }

    public class PickBase
    {
        public int A { get; set; }
        public string B { get; set; } = null!;
    }

    [Pick(typeof(PickBase), "A")]
    public partial class PickA
    {

    }

}
