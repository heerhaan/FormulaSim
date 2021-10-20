using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels.Shared
{
    public enum BootstrapButtonType { Default = 0, Primary, Success, Info, Warning, Danger, Link }

    public class CRUDActionButtonBase
    {
        public string Text { get; set; }
        public string IconClass { get; set; }
        public string ID { get; set; }
    }

    public class CRUDActionButton : CRUDActionButtonBase
    {
        public BootstrapButtonType Type { get; set; }
        public string CallbackHandler { get; set; }
    }

    public class CRUDPulldownButton : CRUDActionButtonBase
    {
        public BootstrapButtonType Type { get; set; }
        public CRUDMenuButton[] DropdownMenu { get; set; }
    }

    public class CRUDMenuButton
    {
        public string Text { get; set; }
        public string IconClass { get; set; }
        public string CallbackHandler { get; set; }
        public string ID { get; set; }
    }

    public class FilterPulldownButton : CRUDActionButtonBase
    {
        public BootstrapButtonType Type { get; set; }
        public CRUDMenuButton[] DropdownMenu { get; set; }
    }
}
