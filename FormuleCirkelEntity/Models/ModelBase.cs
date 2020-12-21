namespace FormuleCirkelEntity.Models
{
    public class ModelBase
    {
        public int Id { get; set; }
    }

    public interface IArchivable
    {
        bool Archived { get; set; }
    }
}
