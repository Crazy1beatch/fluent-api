using System;

namespace ObjectPrinting
{
    public class PrintingPropertyConfig<TOwner, TProperty>
    {
        public PrintingPropertyConfig(PrintingConfig<TOwner> printingConfig)
        {
            throw new NotImplementedException();
        }

        public PrintingConfig<TOwner> To(Func<TProperty, string> func)
        {
            throw new NotImplementedException();
        }
    }
}
