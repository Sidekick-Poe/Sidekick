using SharpHook.Native;

namespace Sidekick.Common.Platform.Keyboards
{
    public struct Key
    {
        public Key(KeyCode keyCode, string stringValue)
        {
            KeyCode = keyCode;
            StringValue = stringValue;
        }

        public KeyCode KeyCode { get; }

        public string StringValue { get; }
    }
}
