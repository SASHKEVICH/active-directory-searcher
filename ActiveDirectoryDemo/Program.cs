using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ActiveDirectoryDemo
{
    class Program
    {
        private static Unit _currentUnit;
        static void Main(string[] args)
        {   
            var searchingProps = new string[]
            {
                "cn", "description", "department", "ou", "objectClass", "mobile", "mail", "st", "l", "streetaddress", "title" 
            };
            var path = "LDAP://OU=Департамент разработки,OU=БН ПО,OU=Томск,OU=Элком\\+,DC=elcom,DC=local";
            var entry = new DirectoryEntry(path);
            var searcher = new DirectorySearcher(entry, "(!(objectClass=group))", searchingProps);
            var smh = searcher.FindAll();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var serializer = new JsonSerializer();
            var units = new List<string>();
            using (StreamWriter unitsFile = File.CreateText(@"C:\Users\student\ldapJson\units.json"))
            {
                for (var i = 0; i < smh.Count; i++)
                {
                    SearchResult result = smh[i];
                    var dirEntry = result.GetDirectoryEntry();
                    ResultPropertyCollection myResultPropColl = result.Properties;
                    Person person;

                    if (IsUnit(myResultPropColl))
                    {
                        if (_currentUnit != null && _currentUnit.Entries.Any())
                        {
                            units.Add(_currentUnit.Name);
                            using StreamWriter file =
                                File.CreateText(@$"C:\Users\student\ldapJson\{_currentUnit.Name.Replace(" ", "_")}.json");
                            var jsonUnit = JsonConvert.SerializeObject(_currentUnit);
                            file.Write(jsonUnit);
                        }
                        _currentUnit = ReadPropsFromUnit(myResultPropColl);
                    }
                    else
                    {
                        person = ReadPropsFromPerson(myResultPropColl);
                        if (person.Name == null) continue;

                        _currentUnit.Entries.Add(person);
                    }
                }
                serializer.Serialize(unitsFile, units);
            }
            

            smh.Dispose();

            Console.ReadKey();
        }

        private static bool IsUnit(ResultPropertyCollection collection)
        {
            var isUnit = false;
            foreach (string myKey in collection.PropertyNames)
            {
                if (myKey == "adspath") continue;
                foreach (string prop in collection[myKey])
                {
                    if (prop.Equals("top") || prop.Equals("user") || prop.Equals("person")) continue;

                    if (prop.Equals("organizationalUnit") && myKey == "objectclass")
                    {
                        return true;
                    }
                }
            }

            return isUnit;
        }

        private static Unit ReadPropsFromUnit(ResultPropertyCollection collection)
        {
            var unit = new Unit();
            foreach (string prop in collection.PropertyNames)
            {
                if (prop == "adspath") continue;
                foreach (string propValue in collection[prop])
                {
                    if (propValue.Equals("top") ||
                        propValue.Equals("user") ||
                        propValue.Equals("person") ||
                        propValue.Equals("organizationalUnit") ||
                        propValue.Equals("organizationalPerson")) continue;

                    if (prop == "ou")
                    {
                        unit.Name = propValue;
                    }
                }
            }
            return unit;
        }

        private static Person ReadPropsFromPerson(ResultPropertyCollection collection)
        {
            var person = new Person();
            foreach (string prop in collection.PropertyNames)
            {
                if (prop == "adspath" || prop == "objectClass" || prop == "ou") continue;
                foreach (string propValue in collection[prop])
                {
                    if (prop == "description") person.FullName = propValue;
                    else if (prop == "department") person.Department = propValue;
                    else if (prop == "cn") person.Name = propValue;
                    else if (prop == "mobile") person.MobilePhone = propValue;
                    else if (prop == "mail") person.Email = propValue;
                    else if (prop == "st") person.Title = propValue;
                    else if (prop == "l") person.City = propValue;
                    else if (prop == "title") person.JobTitle = propValue;
                    else if (prop == "streetaddress") person.Address = propValue;
                }
            }
            return person;
        }
    }
}
