namespace Bindery.Test.TestClasses
{
    public class TestTarget
    {
        public TestTarget()
        {
            Info = new InfoContainer();
        }

        public string Text { get; set; }

        public InfoContainer Info { get; private set; }

        public class InfoContainer
        {
            public int A { get; set; }

            public int B { get; set; }
        }

        public InfoContainer GetInfo()
        {
            return Info;
        }
    }

}
