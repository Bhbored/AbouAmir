using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbouAmir.Convertors
{
    public class DateTimeToTimestampConverter : IFirestoreConverter<DateTime>
    {
        public object ToFirestore(DateTime value) => Timestamp.FromDateTime(value.ToUniversalTime());

        public DateTime FromFirestore(object value)
        {
            if (value is Timestamp timestamp)
            {
                return timestamp.ToDateTime();
            }
            if (value is DateTime dt)
            {
                return dt;
            }
            if (value is string str && DateTime.TryParse(str, out var parsed))
            {
                return parsed;
            }
            throw new ArgumentException("Invalid date value");
        }

    }
}
