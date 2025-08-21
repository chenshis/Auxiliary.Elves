using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auxiliary.Elves.Infrastructure.Config.Snowflake
{
    public class IdWorker
    {
        public const long Twepoch = 1288834974657L;
        private const int WorkerIdBits = 3;
        private const int DatacenterIdBits = 3;
        private const int SequenceBits = 8;
        private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);
        private const int WorkerIdShift = SequenceBits;
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
        public const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;

        private static readonly IdWorker Instance; 

        private static readonly object _lock = new object();
        private long _lastTimestamp = -1L;

        static IdWorker()
        {
            int initDatacenterId;
            int initWorkerId;
            try
            {
                initWorkerId = Math.Abs((Environment.MachineName.GetHashCode() % 7)) + 1;
                initDatacenterId = Math.Abs((Environment.UserDomainName.GetHashCode() % 7)) + 1;
            }
            catch
            {
                initWorkerId = new Random().Next(1, 7);
                initDatacenterId = new Random().Next(1, 7);
            }

            if (initWorkerId > initDatacenterId) initWorkerId = initDatacenterId;
            Instance = new IdWorker(initWorkerId, initDatacenterId);
        }

        public IdWorker(long workerId, long datacenterId, long sequence = 0L)
        {
            if (workerId > MaxWorkerId || workerId < 0)
                throw new ArgumentException(string.Format("worker Id 必须大于0，且不能大于MaxWorkerId： {0}", MaxWorkerId));

            if (datacenterId > MaxDatacenterId || datacenterId < 0)
                throw new ArgumentException(string.Format("region Id 必须大于0，且不能大于MaxWorkerId： {0}", MaxDatacenterId));

            WorkerId = workerId;
            DatacenterId = datacenterId;
            Sequence = sequence;
        }

        public long WorkerId { get; protected set; }
        public long DatacenterId { get; protected set; }

        public long Sequence { get; internal set; }

        public static long NewDefaultId => Instance.NextId();

        public virtual long NextId()
        {
            lock (_lock)
            {
                var timestamp = TimeGen();
                if (timestamp < _lastTimestamp)
                    throw new Exception(string.Format("时间戳必须大于上一次生成ID的时间戳.  拒绝为{0}毫秒生成id", _lastTimestamp - timestamp));

               
                if (_lastTimestamp == timestamp)
                {
                    Sequence = (Sequence + 1) & SequenceMask;
                    if (Sequence == 0)
                        timestamp = TilNextMillis(_lastTimestamp);
                }
                else
                {
                    Sequence = new Random(Guid.NewGuid().GetHashCode()).Next(255);
                }

                _lastTimestamp = timestamp;
                var idWorker = ((timestamp - Twepoch) << TimestampLeftShift)
                              | (DatacenterId << DatacenterIdShift)
                              | (WorkerId << WorkerIdShift)
                              | Sequence;
                return idWorker;
            }
        }

        protected virtual long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp) timestamp = TimeGen();
            return timestamp;
        }

        protected virtual long TimeGen()
        {
            return TimeExtensions.CurrentTimeMillis();
        }
    }
}
