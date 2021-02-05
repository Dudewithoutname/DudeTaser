using Rocket.API;

namespace DudeTaser
{
    public class TaserConfig : IRocketPluginConfiguration
    {
        public uint TaserId;
        public float TasedTime;

        public void LoadDefaults()
        {
            TaserId = 63026;
            TasedTime = 5f;
        }
    }
}
