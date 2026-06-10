using Genesis;
using Genesis.Logging;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqliteUtil
{
	public class TicketNotifyInfoDao : IEntityDao
	{
		private ILogger logger;

		public TicketNotifyInfoDao(ILogger logger)
		{
			this.logger = logger;
			Init();
		}

		private bool Init()
		{
			bool result = false;

			using (var conn = DbConnectionFactory.GetLocalDbConnection())
			{
				try
				{
					conn.CreateTableIfNotExists<TicketNotifyInfoModel>();
					result = true;
					logger.Information("初始化TicketNotifyInfo表成功");
				}
				catch (Exception ex)
				{
					logger.Error(ex, $"初始化TicketNotifyInfo表失败。");
				}
			}

			return result;
		}

		public List<TicketNotifyInfoModel> SelectAll()
		{
			List<TicketNotifyInfoModel> list = new List<TicketNotifyInfoModel>();

			using (var conn = DbConnectionFactory.GetLocalDbConnection())
			{
				list = conn.Select<TicketNotifyInfoModel>();

				conn.Close();
			}

			return list;
		}

		public bool Insert(TicketNotifyInfoModel info)
		{
			bool result = false;

			if (info != null)
			{
				using (var conn = DbConnectionFactory.GetLocalDbConnection())
				{
					conn.Insert<TicketNotifyInfoModel>(info);
					conn.Close();
				}
			}

			return result;
		}

		public bool Delete(TicketNotifyInfoModel info)
		{
			bool result = false;

			if (info != null)
			{
				using (var conn = DbConnectionFactory.GetLocalDbConnection())
				{
					conn.Delete<TicketNotifyInfoModel>(info);
					conn.Close();
				}
			}

			return result;
		}

		public bool Delete(List<TicketNotifyInfoModel> infos)
		{
			bool result = false;

			if (infos == null || infos.Count == 0)
			{
				return result;
			}

			using (var conn = DbConnectionFactory.GetLocalDbConnection())
			{

				using (var trans = conn.BeginTransaction())
				{
					try
					{
						foreach (var info in infos)
						{
							conn.Delete<TicketNotifyInfoModel>(info);
						}

						trans.Commit();
					}
					catch (Exception ex)
					{
						logger.Error(ex);
						trans.Rollback();
						result = false;
					}
				}

				conn.Close();
			}

			return result;
		}
	}
}