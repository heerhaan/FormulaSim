using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
