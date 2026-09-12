using Microsoft.AspNetCore.Mvc;

namespace MCR.API.Components
{
    public abstract class BaseViewComponent : ViewComponent
    {
        public virtual string BasePath => "~/Views/{0}/Components/{1}/Default.cshtml";
        public virtual string NamespacePrefix => "MCR.API.Components.";

        protected string ViewPath
        {
            get
            {
                string namespaceName = this.GetType().Namespace;
                string className = this.GetType().Name;
                namespaceName = namespaceName.Replace(NamespacePrefix, string.Empty);
                className = className.Replace("ViewComponent", string.Empty);
                return string.Format(BasePath, namespaceName, className);
            }
        }
    }
}
