using System;
using System.Data.SqlTypes;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        [Test]
        public void Demo()
        {
            var person = new Person { Name = "Alex", Age = 19 };

            var printer = ObjectPrinter.For<Person>()
                .ExcludeProperty<int>()
                .ChangeSerializationFor<double>()
                .To(x => Math.Round(x).ToString(CultureInfo.InvariantCulture));
                //1. Исключить из сериализации свойства определенного типа
                //2. Указать альтернативный способ сериализации для определенного типа
                //3. Для числовых типов указать культуру
                //4. Настроить сериализацию конкретного свойства
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                //6. Исключить из сериализации конкретного свойства
            
            var s1 = printer.PrintToString(person);

            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию        
            //8. ...с конфигурированием
        }

        [Test]
        public void ExcludeHeight()
        {
            var person = new Person { Name = "Alex", Age = 19 };

            var printer = ObjectPrinter.For<Person>()
                .ExcludeProperty(x => x.Height);
            var s1 = printer.PrintToString(person);
            s1.Should().NotContain("Height");
        }
        
        [Test]
        public void ExcludeType()
        {
            var person = new Person { Name = "Alex", Age = 19 };

            var printer = ObjectPrinter.For<Person>()
                .ExcludeProperty<int>();
            var s1 = printer.PrintToString(person);
            s1.Should().NotContain("Age");
        }
        
        
    }
}