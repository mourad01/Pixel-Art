using System.Reflection;

namespace Core.Serialization
{
    public class FieldAccessor : IAccessor
    {
        FieldInfo m_fi;

        public string Name
        {
            get
            {
                return m_fi.Name;
            }
        }

        public FieldAccessor(FieldInfo fi)
        {
            m_fi = fi;
        }

        public object GetValue(object obj)
        {
            return m_fi.GetValue(obj);
        }

        public void SetValue(object obj, object value)
        {
            m_fi.SetValue(obj, value);
        }
    }
}


