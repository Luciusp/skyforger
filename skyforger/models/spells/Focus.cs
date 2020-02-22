namespace skyforger.models.spells
{
    public class Focus
    {
        public Focus()
        {
            
        }

        public Focus(string focus)
        {
            FocusName = focus;
        }

        public int Id { get; set; }
        public string FocusName { get; set; }
    }
}