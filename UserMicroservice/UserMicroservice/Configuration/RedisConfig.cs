using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMicroservice.Configuration
{
    public static class RedisConfig
    {
		public const bool IgnoreLongTests = true;

		public static string SingleHost
		{
			get { return "redis-api"; }
		}
		public static readonly string[] MasterHosts = new[] { "redis-api" };
		public static readonly string[] SlaveHosts = new[] { "redis-api" };

		public const int RedisPort = 6379;

		public static string SingleHostConnectionString
		{
			get
			{
				return SingleHost + ":" + RedisPort;
			}
		}

		public static BasicRedisClientManager BasicClientManger
		{
			get
			{
				return new BasicRedisClientManager(new[] {
					SingleHostConnectionString
				});
			}
		}
	}
}
