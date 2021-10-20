using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormuleCirkelEntity.ViewModels.Shared
{
    public class CRUDVariables
    {
        public const string LoadDetailsFunction = "loadDetailView";
        public const string AttachDefaultActionsFunction = "attachDefaultListActions";

        public const string ListContentContainerID = "crud-list-content";
        public const string ListFilterContainerID = "crud-list-filter-content";
        public const string ListFooterContainerID = "crud-list-footer-content";

        public const string DetailItemTemplateObject = "detailItemTemplate";

        public const string BodyContentContainerID = "crud-body-content";
        public const string BodyFooterContainerID = "crud-body-footer";

        public const string LoadEditorContentFunction = "loadContentAndPostprocess";

        public const string ListItemClass = "crud-list-item";

        public bool HasFilter => !string.IsNullOrEmpty(FilterContainerID);
        public bool HasActions => ActionButtons != null && ActionButtons.Any();
        public bool HasCallbackHandler => !string.IsNullOrEmpty(OnLoadedCallbackHandler);
        public bool HasDetailsCallbackHandler => !string.IsNullOrEmpty(OnDetailsCallbackHandler);

        #region list actions and filters
        /// <summary>
        /// Contains a key-value collection of action name (key) and javascript function (value) pairs
        /// for display for action button. The value-part will be javascript-evaluated on activation.
        /// </summary>
        public CRUDActionButtonBase[] ActionButtons { get; set; }

        /// <summary>
        /// Container name that holds the filter container controls
        /// </summary>
        public string FilterContainerID { get; set; }
        #endregion

        #region layout
        private string additionalControlsHeight;
        public string AdditionalControlsHeight
        {
            get => HasAdditionalControls ? additionalControlsHeight ?? "4.5em" : null;
            set => additionalControlsHeight = value;
        }
        public string AdditionalControlsContainer { get; set; }
        public bool HasAdditionalControls => !string.IsNullOrEmpty(AdditionalControlsContainer);
        #endregion

        #region mechanism
        public string DataURL { get; set; }
        public string IDProperty { get; set; }

        public string ListItemTemplate { get; set; }
        public string DetailItemTemplate { get; set; }
        public string DetailEmptyTemplate { get; set; }

        public string OnLoadedCallbackHandler { get; set; }
        public string OnDetailsCallbackHandler { get; set; }

        public bool DisableAutolist { get; set; }
        #endregion
    }
}
