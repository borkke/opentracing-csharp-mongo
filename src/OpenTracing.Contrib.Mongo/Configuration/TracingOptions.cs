namespace OpenTracing.Contrib.Mongo.Configuration
{
    public class TracingOptions
    {
        public TracingOptions()
        {
            WhitelistedEvents = new string[0];
        }

        public string[] WhitelistedEvents { get; set; }
    }
}
