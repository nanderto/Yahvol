namespace Yahvol.Services
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;

    public class AdoContext
    {
        private const string AddCommand = @"INSERT INTO [dbo].[ServiceCommands] ([ReceivedDate], [SerializedCommand], [Completed], [CreatedBy], [CommandType], [EndpointId], [UniqueKey], [User]) VALUES (@CreateDate, @SerializedCommand, 0, @CreatedBy, @CommandType, @EndpointId, @UniqueKey, @User) 
                                            SELECT CAST(scope_identity() AS int)";    

        public AdoContext()
        {
            this.Connection = new SqlConnection(this.GetConnectionString());
        }

        public SqlConnection Connection { get; set; }

        public int Add(ServiceCommand serviceCommand)
        {
            try
            {
                var command = new SqlCommand(AdoContext.AddCommand, this.Connection);
                command.Parameters.Add(new SqlParameter("@CreateDate", SqlDbType.DateTime)).Value = DateTime.Now;
                command.Parameters.Add(new SqlParameter("@SerializedCommand", SqlDbType.NVarChar)).Value = serviceCommand.SerializedCommand;
                command.Parameters.Add(new SqlParameter("@ReceivedDate", SqlDbType.NVarChar)).Value = serviceCommand.ReceivedDate;
                command.Parameters.Add(new SqlParameter("@Completed", SqlDbType.Bit)).Value = serviceCommand.Completed;
                command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.NVarChar)).Value = serviceCommand.CreatedBy;
                command.Parameters.Add(new SqlParameter("@CommandType", SqlDbType.NVarChar)).Value = serviceCommand.CommandType;
                command.Parameters.Add(new SqlParameter("@EndpointId", SqlDbType.NVarChar)).Value = serviceCommand.EndpointId;
                command.Parameters.Add(new SqlParameter("@UniqueKey", SqlDbType.NVarChar)).Value = serviceCommand.UniqueKey;
                command.Parameters.Add(new SqlParameter("@User", SqlDbType.NVarChar)).Value = serviceCommand.User;

                return (int)command.ExecuteScalar();
            }
            finally
            {
                this.Connection.Close();
            }
        }

        public Subscriber AddSubscriber(int newId, Subscriber subscriber)
        {
            var command = new SqlCommand(AdoContext.AddCommand, this.Connection);
            command.Parameters.Add(new SqlParameter("@CreateDate", SqlDbType.DateTime)).Value = DateTime.Now;
            command.Parameters.Add(new SqlParameter("@SerializedCommand", SqlDbType.NVarChar)).Value = "THE STUFF";
            
            // var sqlParameter = new SqlParameter("@Identity", SqlDbType.Int) { Direction = ParameterDirection.Output };
            // command.Parameters.Add(sqlParameter);
            subscriber.Id = (int)command.ExecuteScalar();
            return subscriber;
        }

        public string GetConnectionString()
        {
            string connectionString = ConfigurationManager.ConnectionStrings[this.GetType().Name].ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString =
                    ConfigurationManager.ConnectionStrings[this.GetType().Name.Replace("Context", string.Empty)].ConnectionString;
            }

            return connectionString;
        }
    }
}
