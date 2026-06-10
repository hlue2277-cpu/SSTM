using System.Windows.Markup;

namespace Genesis.UI
{
    public class DefaultViewLocator:IViewLocator
    {
        private readonly string[] _viewLocations = new string[]
		{
			"Views\\{1}\\{0}.xaml",
			"Views\\{0}.xaml"
		};

        object IViewLocator.Locate(object view)
        {
            object result;
            if (view is IViewModel)
            {
                result = ((IViewLocator)this).Locate(view as IViewModel);
            }
            else
            {
                result = null;
            }
            return result;
        }

        object IViewLocator.Locate(IViewModel viewModel)
        {
            return this.InternalLocate(viewModel, this._viewLocations, typeof(IViewModel).Name.Substring(1), "View");
        }

        protected virtual object InternalLocate(object model, string[] searchLocations, string modelPostfix, string viewPostfix)
        {
            object element = null;
            System.Type modelType = model.GetType();
            string modelName = modelType.Name;
            string viewName = modelName.Replace(modelPostfix, viewPostfix);
            for (int i = 0; i < searchLocations.Length; i++)
            {
                string location = searchLocations[i];
                string fileName = string.Format(location, viewName, modelType.Assembly.GetName().Name);
                if (System.IO.File.Exists(fileName))
                {
                    using (System.IO.FileStream stream = System.IO.File.OpenRead(fileName))
                    {
                        element = XamlReader.Load(stream);
                        break;
                    }
                }
            }
            if (null == element)
            {
                string elementTypeName = modelType.FullName.Replace(modelPostfix, viewPostfix);
                System.Type elementType = modelType.Assembly.GetType(elementTypeName);
                if (null != elementType)
                {
                    element = System.Activator.CreateInstance(elementType);
                }
                else
                {
                    System.IO.Stream stream2 = modelType.Assembly.GetManifestResourceStream(elementTypeName + ".xaml");
                    if (null != stream2)
                    {
                        using (stream2)
                        {
                            element = XamlReader.Load(stream2);
                        }
                    }
                }
            }
            return element;
        }
    }
}
