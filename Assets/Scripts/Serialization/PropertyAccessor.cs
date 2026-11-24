using System.Reflection;

namespace Core.Serialization
{
    public class PropertyAccessor : IAccessor
    {
        PropertyInfo m_pi;

        public string Name
        {
            get
            {
                return m_pi.Name;
            }
        }

        public PropertyAccessor(PropertyInfo fi)
        {
            m_pi = fi;
        }

        public object GetValue(object obj)
        {
            return m_pi.GetGetMethod(true).Invoke(obj, null);
        }

        public void SetValue(object obj, object value)
        {
            m_pi.SetValue(obj, value, null);
        }
    }
}


