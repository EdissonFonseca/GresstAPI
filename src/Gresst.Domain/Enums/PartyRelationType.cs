public enum PartyRelationType
{
    Customer = 1,       // Get services or products
    Supplier,       // Offer services or products
    Hauler,         // Transport goods (could be a third-party logistics provider)
    Operator,       // Execute operations (could be a third-party service provider)
    Employee,       // Employees of the company
    Owner,           // The own company 
    Driver,
    Unknown         // For any role that doesn't fit the above categories
}
