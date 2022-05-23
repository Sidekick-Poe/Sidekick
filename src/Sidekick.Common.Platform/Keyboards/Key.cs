using SharpHook.Native;

namespace Sidekick.Common.Platform.Keyboards
{
    public struct Key
    {
        public Key(KeyCode keyCode, string stringValue, string electronAccelerator)
        {
            KeyCode = keyCode;
            StringValue = stringValue;
            ElectronAccelerator = electronAccelerator;
        }

        public KeyCode KeyCode { get; }

        public string StringValue { get; }

        public string ElectronAccelerator { get; }
    }
}
