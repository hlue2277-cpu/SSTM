using Genesis.Logging;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqliteUtil
{
	public class TicketNotifyInfoModel
	{
		[AutoIncrement]
		public long Id { get; set; }

		public string TicketUUID { get; set; }

		public string TicketSeatId { get; set; }

		public override string ToString()
		{
			return $"({TicketUUID}，{TicketSeatId})";
		}
	}
}