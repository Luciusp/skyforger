namespace skyforger.models.spells
{
    public class MaterialComponent
    {
        public MaterialComponent()
        {
            
        }

        public MaterialComponent(string component)
        {
            Component = component;
        }
        public int Id { get; set; }
        public string Component { get; set; }
    }
}