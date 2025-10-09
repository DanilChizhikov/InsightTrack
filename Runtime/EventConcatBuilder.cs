using System.Text;

namespace MbsCore.InsightTrack.Runtime
{
    public sealed class EventConcatBuilder
    {
        private readonly StringBuilder _builder;
        private readonly string _separator;

        public EventConcatBuilder()
        {
            _builder = new StringBuilder();
            _separator = "_";
        }

        public EventConcatBuilder AppendEvent(string eventName)
        {
            if (!string.IsNullOrEmpty(eventName))
            {
                if (!string.IsNullOrEmpty(_builder.ToString()))
                {
                    _builder.Append(_separator);
                }

                _builder.Append(eventName);
            }
            
            return this;
        }

        public string Concat() => _builder.ToString();

        public void Reset() => _builder.Clear();
    }
}