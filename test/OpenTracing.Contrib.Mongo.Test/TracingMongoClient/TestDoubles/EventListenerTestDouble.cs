namespace OpenTracing.Contrib.Mongo.Test.TracingMongoClient.TestDoubles
{
    public class EventListenerTestDouble
    {
        public int Counter;

        public void StartEventHandler()
        {
            Counter++;
        }
    }
}
