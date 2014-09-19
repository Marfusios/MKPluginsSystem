using System;
using PluginsCore.Communication;
using PluginsCore.Utils;

namespace PluginsCore
{
    public abstract class PluginBase : MarshalByRefObject
    {
        protected PluginBase()
        { }


        public IPluginParent Parent { get; private set; }
        public string TypeFullName { get { return GetType().FullName; } }


        public void Init(IPluginParent parent)
        {
            if(parent == null)
                throw new PluginsException("PluginParent is null");

            Parent = parent;

            Init();
        }

        public Field[] GetFields()
        {
            return PluginLoader.GetFields(this);
        }

        public void SetFields(Field[] ssbFields)
        {
            PluginLoader.SetFields(this, ssbFields);
        }

        public Method[] GetMethods()
        {
            return PluginLoader.GetMethods(this);
        }

        public Parameter ExecuteMethod(Method method)
        {
            return PluginExecuter.ExecuteMethod(this, method);
        }


        #region FOR OVERRIDE

        protected abstract void Init();
        public abstract void Close();

        #endregion


        #region FOR CHILDREN

        public virtual void LogMessage(object message)
        {
            if (Parent != null)
            {
                Parent.LogMessage(this, message);
            }
        }

        #endregion


        #region NULL Object

        public static PluginBase Null = PluginNull.Instance;

        private class PluginNull : PluginBase
        {
            public static PluginNull Instance
            {
                get { return PluginNullCreator.instance; }
            }

            private PluginNull()
            {
            }

            protected override void Init()
            {
                throw new PluginsException("Null Plugin object cannot be initialized");
            }

            public override void Close()
            {
                throw new PluginsException("Null Plugin object cannot be closed");
            }

            #region THREAD SAFE SINGLETON

            private class PluginNullCreator
            {
                static PluginNullCreator()
                {
                }

                internal static readonly PluginNull instance = new PluginNull();
            }

            #endregion
        }

        #endregion
    }

    
}
