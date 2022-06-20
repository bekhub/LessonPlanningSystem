namespace LPS.PlanGenerators.Enums;

public enum SubgroupMode
{
    /// <summary>
    /// No subgroups
    /// </summary>
    Mode0 = 0,
    /// <summary>
    /// One teacher – one lab 
    /// </summary>
    Mode1 = 1,
    /// <summary>
    /// One teacher – more than one lab
    /// </summary>
    Mode2 = 2,
    /// <summary>
    /// More than one teacher – one lab
    /// </summary>
    Mode3 = 3,
    /// <summary>
    /// More than one teacher – more than one lab
    /// </summary>
    Mode4 = 4,
    /// <summary>
    /// Joint lesson for more than one departments (Theory and Practice)
    /// </summary>
    Mode5 = 5,
    /// <summary>
    /// Joint lesson for more than one departments (Theory)
    /// </summary>
    Mode6 = 6,
}
