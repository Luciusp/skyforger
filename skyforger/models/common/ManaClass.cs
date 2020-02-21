namespace skyforger.models.common
{
    public class ManaClass
    {
        public ManaClass(ManaClassEnum manaclass)
        {
            ManaClassEnum = manaclass;
        }

        public int Id { get; set; }
        public ManaClassEnum ManaClassEnum { get; set; }
    }
}