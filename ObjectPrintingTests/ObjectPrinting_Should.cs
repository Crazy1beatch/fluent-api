﻿using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using ObjectPrinting;
using ObjectPrinting.Extensions;


namespace ObjectPrintingTests
{
    public class ObjectPrinting_Should
    {
        private Person person;
        [SetUp]
        public void CreateDefaultPerson() =>
            person = new Person { Name = "Alex", Age = 19, Surname = "Vasilyev", Weight = 70.3 };

        [Test]
        public void ExcludeByType()
        {
            const string notExcepted = nameof(Person.Name) + " = ";
            var result = ObjectPrinter.For<Person>()
                .ExcludeProperty<string>()
                .PrintToString(person);

            result.Should().NotContain(notExcepted);
        }

        [Test]
        public void ExcludeProperty()
        {
            var notExcepted = nameof(Person.Id) + " = " + person.Id + Environment.NewLine;
            var result = ObjectPrinter.For<Person>()
                .ExcludeProperty(x => x.Id)
                .PrintToString(person);

            result.Should().NotContain(notExcepted);
        }

        [Test]
        public void ProvideCustomSerialization_ForType()
        {
            var heightExcepted = nameof(Person.Height) + " = " + person.Height.ToString("f0") + Environment.NewLine;
            var weightExcepted = nameof(Person.Weight) + " = " + person.Weight.ToString("f0") + Environment.NewLine;
            var result = ObjectPrinter.For<Person>()
                .ChangeSerializationFor<double>()
                .To(x => x.ToString("f0"))
                .PrintToString(person);

            result.Should().Contain(heightExcepted).And.Contain(weightExcepted);
        }

        [Test]
        public void ProvideCustomSerialization_ForProperty()
        {
            var excepted = nameof(Person.Weight) + " = " + person.Weight.ToString("f0") + Environment.NewLine;
            var result = ObjectPrinter.For<Person>()
                .ChangeSerializationFor(t => t.Weight)
                .To(x => x.ToString("f0"))
                .PrintToString(person);

            result.Should().Contain(excepted);
        }

        [Test]
        public void ProvideCulture_ForNumberTypes()
        {
            var excepted = nameof(Person.Weight) + " = " +
                           person.Weight.ToString(CultureInfo.CurrentCulture) + Environment.NewLine;
            var result = ObjectPrinter.For<Person>()
                .ChangeSerializationFor<double>()
                .To(CultureInfo.CurrentCulture)
                .PrintToString(person);

            result.Should().Contain(excepted);
        }

        [Test]
        public void ProvideTrimForStrings()
        {
            const int length = 5;
            var excepted = nameof(Person.Surname) + " = " + person.Surname[..length] + Environment.NewLine;
            var result = ObjectPrinter.For<Person>()
                .ChangeSerializationFor<string>()
                .ToTrimmedLength(length)
                .PrintToString(person);

            result.Should().Contain(excepted);
        }


        [Test]
        public void Work_WhenReferenceCycles()
        {
            const string excepted = "Cycle, object was already serialized";
            person.Parents = [person];
            person.Friends = [person];
            var result = ObjectPrinter.For<Person>().PrintToString(person);

            result.Should().Contain(excepted);
        }

        [Test]
        public void Print_WhenArray()
        {
            const string excepted = "Person\r\n\tParents = Person[] {\r\n\t\tPerson" +
                                    "\r\n\t\t\tParents = null\r\n\t\tPerson\r\n\t\t\tParents = null\r\n\t}\r\n";
            person.Parents = [new Person(), new Person()];
            var result = ObjectPrinter.For<Person>()
                .ExcludeProperty(t => t.SomeDictionary)
                .ExcludeProperty(t => t.Friends)
                .ExcludeProperty<Guid>()
                .ExcludeProperty<double>()
                .ExcludeProperty<bool>()
                .ExcludeProperty<string>()
                .ExcludeProperty<int>()
                .PrintToString(person);

            result.Should().Be(excepted);
        }

        [Test]
        public void Print_WhenList()
        {
            const string excepted = "Person\r\n\tFriends = List`1 {\r\n\t\tPerson\r\n\t\t\tFriends = null" +
                                    "\r\n\t\tPerson\r\n\t\t\tFriends = null\r\n\t}\r\n";
            person.Friends = [new Person(), new Person()];

            var result = ObjectPrinter.For<Person>()
                .ExcludeProperty(t => t.SomeDictionary)
                .ExcludeProperty(t => t.Parents)
                .ExcludeProperty<Guid>()
                .ExcludeProperty<double>()
                .ExcludeProperty<bool>()
                .ExcludeProperty<string>()
                .ExcludeProperty<int>()
                .PrintToString(person);

            result.Should().Be(excepted);
        }

        [Test]
        public void Print_WhenDictionaries()
        {
            const string expected = "Person\r\n\tSomeDictionary = Dictionary`2 " +
                                    "{\r\n\t\t1 = aboba\r\n\t\t2 = biba\r\n\t}\r\n";
            person.SomeDictionary = new Dictionary<int, string>
            {
                { 1, "aboba" },
                { 2, "biba" }
            };
            var result = ObjectPrinter.For<Person>()
                .ExcludeProperty(t => t.Parents)
                .ExcludeProperty(t => t.Friends)
                .ExcludeProperty<Guid>()
                .ExcludeProperty<double>()
                .ExcludeProperty<bool>()
                .ExcludeProperty<string>()
                .ExcludeProperty<int>()
                .PrintToString(person);
            result.Should().Be(expected);
        }
    }
}