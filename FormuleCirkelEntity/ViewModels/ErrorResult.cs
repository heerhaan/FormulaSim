using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels
{
    public class ErrorResult
    {
        public ErrorResult(string code, string message)
            => (Code, Message) = (code, message);

        public string Code { get; set; }
        public string Message { get; set; }
    }
}
