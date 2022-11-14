using System.Collections.Generic;
using LPS.DatabaseLayer.Entities;
using LPS.PlanGenerators.Configuration;

namespace LPS.Desktop.Models;

public sealed class ConfigurationDetails
{
    public ConnectionDetails ConnectionDetails { get; set; }
    public List<Department> Departments { get; set; }
    public PlanConfiguration PlanConfiguration { get; set; }
}
