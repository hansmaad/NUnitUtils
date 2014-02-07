using NUnit.Framework.Constraints;
using NunitUtils.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace NunitUtils.Constraints
{
    public class LikeConstraint<T> : Constraint
    {
        T expected;
        string message;
        List<string> exclude = new List<string>();
        private bool excludeRefTypes = false;

        public LikeConstraint(T expected)
        {
            this.expected = expected;
        }

        class Mismatch
        {
            public string Property { get; set; }
            public object Actual { get; set; }
            public object Expected { get; set; }
        }

        public LikeConstraint<T> Without(string propertyName)
        {
            exclude.Add(propertyName);
            return this;
        }

        public LikeConstraint<T> WithoutReferenceTypes()
        {
            excludeRefTypes = true;
            return this;
        }

        public LikeConstraint<T> Without<TProperty>(
            Expression<Func<T, TProperty>> property)
        {
            var info = ReflectionHelper.GetPropertyInfo(property);
            return Without(info.Name);
        }

        public override bool Matches(object actual)
        {
            if (expected == null)
                return actual == null;

            if (expected.Equals(actual))
                return true;

            var mismatches = GetMismatches(actual);

            if (!mismatches.Any())
                return true;

            message = CreateMessage(mismatches);

            return false;
        }

        private IEnumerable<Mismatch> GetMismatches(object actual)
        {
            var mismatches =
                 GetCommonProperties(actual)
                .Where(property => NotEqualToExpected(actual, property))
                .Select(property => CreateMismatch(actual, property));
            return mismatches;
        }

        struct PropertyAssociation
        {
            public PropertyInfo Actual { get; set; }
            public PropertyInfo Expected { get; set; }
        }

        private IEnumerable<PropertyAssociation> GetCommonProperties(object actual)
        {
            var expectedProperties = GetProperties(expected);
            var actualProperties = GetProperties(actual);
            return expectedProperties.Join(
                actualProperties,
                p => p.Name,
                p => p.Name,
                (exp, act) => new PropertyAssociation() { Actual = act, Expected = exp });
        }

        private IEnumerable<PropertyInfo> GetProperties(object obj)
        {
            var bindingAttributes = BindingFlags.Public | BindingFlags.Instance;
            return obj.GetType()
                .GetProperties(bindingAttributes)
                .Where(SatisfiesSpecification);
        }

        private bool SatisfiesSpecification(PropertyInfo property)
        {
            if (excludeRefTypes)
            {
                if (!property.PropertyType.IsValueType)
                    return false;
            }
            return !exclude.Contains(property.Name);
        }

        private bool NotEqualToExpected(object actual, PropertyAssociation properties)
        {
            var e = properties.Expected.GetValue(expected, null);
            var a = properties.Actual.GetValue(actual, null);
            if (e == null)
                return a != null;
            return !e.Equals(a);
        }

        private Mismatch CreateMismatch(object actual, PropertyAssociation properties)
        {
            return new Mismatch()
            {
                Property = properties.Expected.Name,
                Actual = properties.Actual.GetValue(actual, null),
                Expected = properties.Expected.GetValue(expected, null)
            };
        }

        private string CreateMessage(IEnumerable<Mismatch> mismatches)
        {
            return String.Join(Environment.NewLine,
                mismatches.Select(m =>
                    String.Format("{0} : expected: {1}, but was {2}",
                    m.Property, m.Expected, m.Actual)));
        }

        public string Message
        {
            get { return message; }
        }

        public override void WriteMessageTo(NUnit.Framework.Constraints.MessageWriter writer)
        {
            writer.Write(message);
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {

        }

    }


}
