using System.Collections;
using System.Reflection;
using System.Text;

namespace CameraServerTests
{
    public static class CustomAsserts
    {
        /// <summary>
        /// Class used to store the name and value of a property of an object
        /// </summary>
        private class ObjectProperty
        {
            public string Name { get; set; }
            public object? Value { get; set; }

            public ObjectProperty(string name, object? value)
            {
                Name = name;
                Value = value;
            }

            public override string ToString()
            {
                return string.Format("{0}: {1}", Name, Value == null ? "null" : Value.ToString());
            }
        }

        /// <summary>
        /// Will ensure that property names follow the forceCamelCase parameter
        /// </summary>
        private static string GetPropertyName(string name, bool forceCamelCase)
        {
            if (forceCamelCase)
                return name.Substring(0, 1).ToLower() + name.Substring(1);
            else
                return name;
        }

        /// <summary>
        /// Will get the properties of an object and return them as a dictionary where the key is the name of the property and the value is an instance of ObjectProperty
        /// </summary>
        private static Dictionary<string, ObjectProperty> GetObjectProperties(object obj, bool forceCamelCase)
        {
            PropertyInfo[] propertyInfos = obj.GetType().GetProperties().Where(p => !p.GetIndexParameters().Any()).ToArray();
            Dictionary<string, ObjectProperty> properties = new Dictionary<string, ObjectProperty>();

            foreach (PropertyInfo property in propertyInfos)
                properties.Add(GetPropertyName(property.Name, forceCamelCase), new ObjectProperty(GetPropertyName(property.Name, forceCamelCase), property.GetValue(obj, null)));

            return properties;
        }

        /// <summary>
        /// Will check if two objects have the same properties and values
        /// </summary>
        /// <returns>A boolean telling if they are equal or not. True means equal</returns>
        private static bool CheckEquals(ObjectProperty? obj1, ObjectProperty? obj2, bool forceCamelCase, string path)
        {
            if (obj1 == null && obj2 == null) // check cases where values are null
                return true;
            if (obj1 == null && obj2 != null)
                return false;
            if (obj1 != null && obj2 == null)
                return false;
            if (obj1!.Value == null && obj2!.Value == null)
                return true;
            if (obj1.Value == null && obj2!.Value != null)
                return false;
            if (obj1.Value != null && obj2!.Value == null)
                return false;

            Dictionary<string, ObjectProperty> obj1Properties = GetObjectProperties(obj1!.Value!, forceCamelCase); // get all the properties on the objects
            Dictionary<string, ObjectProperty> obj2Properties = GetObjectProperties(obj2!.Value!, forceCamelCase);

            if (obj1!.Value!.GetType() == typeof(DateTime) && obj2!.Value!.GetType() == typeof(DateTime)) // if we have a datetime we allow for some difference in the time
                return Math.Abs(((DateTime)obj1!.Value - (DateTime)obj2!.Value).TotalHours) < 3;

            if (obj1Properties.Count > 1 || obj2Properties.Count > 1) // if we have moret than one property we cann the method for checking if the objects have the same properties
            {
                string? samePropertiesResult = ObjectsHaveSameProperties(obj1!.Value!, obj2!.Value!, forceCamelCase, string.Format("{0}.{1}", path, obj1.Name)); // this will start a recursive chain to check all children
                if (samePropertiesResult != null)
                    throw new AssertFailedException(samePropertiesResult);
                return true;
            }
            else
            {
                bool result = obj1!.Value!.Equals(obj2!.Value); // we only have one property so we can just check if the values are equal (base case for recursive chain)
                return result;
            }
        }

        /// <summary>
        /// Will convert objects that implements the IList interface to a type of generic list
        /// </summary>
        /// <param name="obj">The supposed IList to convert</param>
        /// <returns>A list</returns>
        /// <exception cref="AssertFailedException">Will throw an AssertFailedException if it couldn't convert</exception>
        private static IList ConvertToList(object obj)
        {
            if (obj.GetType() == typeof(List<>)) // if it is already a list we just return it
                return (IList)obj;

            if (obj is IList)
            {
                Type? type = obj.GetType().GetElementType(); // the type of the generic list

                if (type == null) // if we didn't get the type with get element type we try to get it using generic type arguments
                {
                    Type[] typeArguments = obj.GetType().GenericTypeArguments;
                    type = typeArguments.Length > 0 ? typeArguments[0] : null;
                }

                if (type == null) // if the type is still null we have to fail here
                    throw new AssertFailedException("Failed to convert to list because the element type was missing");

                Type genericListType = typeof(List<>).MakeGenericType(type);
                object? genericList = Activator.CreateInstance(genericListType);

                if (genericList == null)
                    throw new AssertFailedException("Failed to convert to list because creating an instance of the a list returned a null reference");

                MethodInfo? addMethod = genericListType.GetMethod("Add"); // get the add method of the newly created generic type list

                if (addMethod == null)
                    throw new AssertFailedException("Failed to convert to list because the add method was missing on the created list"); // this really should not be able to happen

                foreach (object item in (IList)obj)
                {
                    addMethod.Invoke(genericList, new[] { item });
                }

                return (IList)genericList;
            }

            throw new AssertFailedException("Failed to convert to list because the object provided was not implementing the IList interface");
        }

        /// <summary>
        /// Will count the times that an object appears in a list
        /// </summary>
        /// <returns>The amount of times an object appears in a list</returns>
        private static int GetOccurencesOfEqualObjectInList(object needle, IList haystack, bool forceCamelCase, string path)
        {
            int result = 0;

            foreach (object item in haystack)
            {
                string? assertMessage = ObjectsHaveSameProperties(needle, item, forceCamelCase, path);

                if (assertMessage == null)
                    result++;
            }

            return result;
        }

        /// <summary>
        /// Will make sure that two lists have the same content by checking how many times a object is found in both lists and making sure that count is the same
        /// </summary>
        private static void AssertListsHaveSameContent(IList expected, IList actual, bool forceCamelCase, string path)
        {
            if (expected.Count != actual.Count) // this should actually never happen because the capacity is checked before this in when all properties are checked
                throw new AssertFailedException("Found lists in expected and actual but they have different lengths so they can't be equal"); // but let's check it to be sure

            foreach (object item in expected)
            {
                int occurencesInExpected = GetOccurencesOfEqualObjectInList(item, expected, forceCamelCase, path);
                int occurencesInActual = GetOccurencesOfEqualObjectInList(item, actual, forceCamelCase, path);

                if (occurencesInExpected != occurencesInActual)
                {
                    StringBuilder detailedInformation = new StringBuilder("There is an object that looks like this: { "); // let's create detailed information
                    foreach (KeyValuePair<string, ObjectProperty> valuePair in GetObjectProperties(item, forceCamelCase)) // about what object was actually different
                        detailedInformation.Append(string.Format("{0}, ", valuePair.Value.ToString()));                   // for the two lists
                    detailedInformation.Length = detailedInformation.Length - 2;
                    detailedInformation.Append(" }. ");
                    detailedInformation.Append(string.Format("It appears {0} in the expected list and {1} times in the actual list.", occurencesInExpected, occurencesInActual));

                    throw new AssertFailedException(string.Format("Lists of ({0}) are not the same in expected and actual. Detailed information: {1}", item.ToString(), detailedInformation.ToString()));
                }
            }
        }

        /// <summary>
        /// Asserts that two objects have the same properties and if the properties have children, it asserts that those have the same properties as well. Will convert all types that implements IList to generic lists so that an array and lists with the same items will be counted as the same
        /// </summary>
        /// <param name="assert">The assert that this is called on, not actually used for anything, just to get nice syntax, making this an extension method</param>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="forceCamelCase">If it should force all properties to start with a lower case letter to look the same as most json is formatted</param>
        /// <exception cref="AssertFailedException">If the assertion fails it will throw an assert failed exception</exception>
        public static void ObjectsHaveSameProperties(this Assert assert, object expected, object actual, bool forceCamelCase = true)
        {
            string? result = ObjectsHaveSameProperties(expected, actual, forceCamelCase, "");

            if (result != null)
                throw new AssertFailedException(result);
        }

        /// <summary>
        /// Will do the actual work of checking wether or not objects have the same properties, the public method with the same name is just to get it as an extension method.
        /// This won't throw an exception but will instead return the string that will go into the exception.
        /// This is so that this method can be used internally without having to use error handling as a part of the planned flow.
        /// Specifically it is used when comparing lists, when checking how many times an object is present in a list.
        /// The path parameter is used to be able to write more detailed error messages about where in an object it failed
        /// </summary>
        /// <returns>A string containing the assertion error message or a null reference if the assertion was a success</returns>
        private static string? ObjectsHaveSameProperties(object expected, object actual, bool forceCamelCase, string path)
        {
            if (expected is IList || actual is IList)
            {
                if (expected is IList) // if is a type of list or array we convert it to the basic generic list so it doesn't matter what type of list it is
                    expected = ConvertToList(expected);
                else
                    return "The actual value is a type of list but the expected is not"; // both need to be a type of list, else they are not equal

                if (actual is IList)
                    actual = ConvertToList(actual);
                else
                    return "The expected value is a type of list but the actual is not";
            }

            Dictionary<string, ObjectProperty> expectedProperties = GetObjectProperties(expected, forceCamelCase); // get all the properties on the object
            Dictionary<string, ObjectProperty> actualProperties = GetObjectProperties(actual, forceCamelCase);

            foreach (string key in expectedProperties.Keys) // check that all expected properties are present in actual object
            {
                if (!actualProperties.ContainsKey(key))
                    return string.Format("Expected property {0} not found in actual object", key);
            }

            foreach (string key in actualProperties.Keys) // check that all actual properties are present in expected object
            {
                if (!expectedProperties.ContainsKey(key))
                    return string.Format("Actual property {0} not found in expected object", key);
            }

            foreach (string key in actualProperties.Keys) // check that all properties match
            {
                if (!CheckEquals(expectedProperties[key], actualProperties[key], forceCamelCase, path))
                    return string.Format("Property {0} does not match. Expected: {1}, Actual: {2}. At path: {3}", key, expectedProperties[key], actualProperties[key], path);
            }

            if (expected is IList || actual is IList)
                AssertListsHaveSameContent((IList)expected, (IList)actual, forceCamelCase, path);

            return null;
        }
    }
}
