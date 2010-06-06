﻿namespace Caliburn.Micro
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;

    public static class ViewLocator
    {
        public static readonly object DefaultContext = new object();
        static Assembly[] AssembliesToInspect = new[] { Assembly.GetExecutingAssembly() };
        const string View = "View";
        const string Model = "Model";

        public static void Initialize(Assembly[] assembliesToInspect)
        {
            AssembliesToInspect = assembliesToInspect; 
        }

        public static UIElement Locate(object viewModel, object context = null)
        {
            var viewAware = viewModel as IViewAware;
            if(viewAware != null)
            {
                var existing = viewAware.GetView(context ?? DefaultContext);
                if (existing != null)
                    return (UIElement)existing;
            }

            var viewTypeName = viewModel.GetType().FullName.Replace(Model, string.Empty);

            if (context != null)
            {
                viewTypeName = viewTypeName.Remove(viewTypeName.Length - View.Length, View.Length);
                viewTypeName = viewTypeName + "." + context;
            }

            var viewType = (from assmebly in AssembliesToInspect
                            from type in assmebly.GetExportedTypes()
                            where type.FullName == viewTypeName
                            select type).FirstOrDefault();

            return viewType == null ? null : GetOrCreateViewFromType(viewType);
        }

        static UIElement GetOrCreateViewFromType(Type type)
        {
            var view = IoC.GetAllInstances(type)
                .FirstOrDefault() as UIElement;

            if (view != null)
                return view;

            if (type.IsInterface || type.IsAbstract || !typeof(UIElement).IsAssignableFrom(type))
                throw new Exception(string.Format("Cannot instantiate {0}.", type.FullName));

            return (UIElement)Activator.CreateInstance(type);
        }
    }
}