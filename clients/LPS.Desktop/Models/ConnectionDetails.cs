using System;
using System.Runtime.Serialization;

namespace LPS.Desktop.Models;

[DataContract]
public record ConnectionDetails
{
    [DataMember]
    public string Server { get; }
    [DataMember]
    public string Database { get; }
    [DataMember]
    public string User { get; }
    [DataMember]
    public string Password { get; }
    [DataMember]
    public string? MysqlVersion { get; set; }
    
    public ConnectionDetails(string server, string database, string user, string password, string mysqlVersion)
    {
        Server = server;
        Database = database;
        User = user;
        Password = password;
        MysqlVersion = mysqlVersion;
    }
    
    public string GetConnectionString()
    {
        return $"server={Server.Trim()};database={Database.Trim()};user={User.Trim()};password={Password.Trim()}";
    }

    public Version GetMysqlVersion()
    {
        return Version.Parse(MysqlVersion!);
    }

    public override string ToString()
    {
        return $"{GetConnectionString()};mysql_version={MysqlVersion}";
    }
}
