using AMQP_0_9_1.Types;
using System;

namespace AMQP_0_9_1.Extensions
{
    public static class AmqpTypeExtension
    {
        public static int SizeOf<T>(this T obj) where T : IAmqpType
        {
            if (obj == null)
            {
                var instance = Activator.CreateInstance<T>() as IAmqpType;
                if (instance == null)
                {
                    throw new NotImplementedException(typeof(T).FullName);
                }
                return instance.SizeOf();
            }

            return obj.TypeSize;
        }

        #region private

        #endregion
    }
}
