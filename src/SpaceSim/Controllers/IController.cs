using System.Windows.Input;

namespace SpaceSim.Controllers
{
    interface IController
    {
        bool IsPrograde { get; }
        bool IsRetrograde { get; }

        void KeyUp(Key key);
        void KeyDown(Key key);

        void Update(double elapsedTime);
    }
}
