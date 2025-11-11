# Gresst - Waste Management API

A comprehensive REST API for waste management built with .NET 8, C#, Entity Framework Core, and Clean Architecture.

## Features

### Core Operations (11 Required Operations)
1. **Generate Waste** - Create waste records
2. **Collect Waste** - Collection operations
3. **Transport Waste** - Transportation with vehicles and routes
4. **Receive Waste** - Receiving at facilities
5. **Transform Waste** - Convert, decompose, or group wastes
6. **Store Temporarily** - Short-term storage
7. **Store Permanently** - Long-term storage
8. **Sell Waste** - Commercial transactions
9. **Deliver to Third Party** - Transfer to external parties
10. **Dispose Waste** - Final disposal
11. **Classify Waste** - Categorization and reclassification

### Key Features
- **Complete Traceability** - Track every operation on each waste item
- **Multitenant Support** - Isolated data per account
- **Inventory Management** - Real-time balance tracking
- **Waste Bank** - Publish wastes for reuse/recycling
- **Service Requests** - Request services between companies/people
- **Work Orders** - Plan and execute operations
- **Certificates** - Generate certificates for operations
- **Licenses** - Manage operating licenses
- **International Classifications** - Support for UN, LER, Y-Code, A-Code
- **Dynamic Storage Hierarchy** - Flexible location structure
- **Route Planning** - Plan collection and transport routes

## Architecture

Clean Architecture with 4 layers:

```
Gresst.API/
├── src/
│   ├── Gresst.Domain/          # Entities, Enums, Interfaces
│   ├── Gresst.Application/     # DTOs, Services, Business Logic
│   ├── Gresst.Infrastructure/  # DbContext, Repositories, Data Access
│   └── Gresst.API/             # Controllers, Configuration, Entry Point
```

## Domain Model

### Main Entities (22)
- **Person** - Actors with capabilities (Generator, Collector, Transporter, etc.)
- **Waste** - Individual waste items with complete tracking
- **WasteType** - Waste classifications
- **Classification** - International codes (UN, LER, Y, A)
- **Facility** - Treatment plants, disposal sites, storage facilities
- **Location** - Dynamic hierarchical storage structure
- **Management** - Records of all operations
- **Order** - Work orders
- **Request** - Service requests
- **Certificate** - Operation certificates
- **License** - Operating licenses
- **Vehicle** - Transport vehicles
- **Balance** - Real-time inventory
- **Adjustment** - Inventory adjustments
- **WasteTransformation** - Transformation records
- **Treatment** - Treatment types
- **Material** - Material composition
- **Packaging** - Container types
- **Route** - Collection/transport routes
- **User** - System users
- **Rate** - Service pricing

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full version)
- Visual Studio 2022 or VS Code

### Installation

1. Clone the repository
```bash
git clone <repository-url>
cd API
```

2. Restore packages
```bash
dotnet restore
```

3. Update connection string in `appsettings.json`
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GreesstDb;Trusted_Connection=true"
}
```

4. Create database migration
```bash
cd src/Gresst.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../Gresst.API
dotnet ef database update --startup-project ../Gresst.API
```

5. Run the API
```bash
cd ../Gresst.API
dotnet run
```

6. Open Swagger UI
Navigate to: `https://localhost:5001` or `http://localhost:5000`

## API Endpoints

### Waste Management
- `POST /api/management/generate` - Generate waste
- `POST /api/management/collect` - Collect waste
- `POST /api/management/transport` - Transport waste
- `POST /api/management/receive` - Receive waste
- `POST /api/management/transform` - Transform waste
- `POST /api/management/store` - Store waste (temporary/permanent)
- `POST /api/management/sell` - Sell waste
- `POST /api/management/deliver` - Deliver to third party
- `POST /api/management/dispose` - Dispose waste
- `POST /api/management/classify` - Classify waste
- `GET /api/management/waste/{wasteId}/history` - Get waste history

### Waste CRUD
- `GET /api/waste` - Get all wastes
- `GET /api/waste/{id}` - Get waste by ID
- `GET /api/waste/generator/{generatorId}` - Get by generator
- `GET /api/waste/status/{status}` - Get by status
- `GET /api/waste/bank` - Get waste bank (available for reuse)
- `POST /api/waste` - Create waste
- `PUT /api/waste/{id}` - Update waste
- `DELETE /api/waste/{id}` - Delete waste
- `POST /api/waste/{id}/publish-to-bank` - Publish to waste bank
- `POST /api/waste/{id}/remove-from-bank` - Remove from waste bank

### Inventory
- `GET /api/inventory` - Get inventory (with filters)
- `GET /api/inventory/balance` - Get specific balance

## Multitenant Support

The API supports multitenancy through the `AccountId` field. Pass the account ID via:
- Header: `X-Account-Id: <guid>`
- Claim: `AccountId` in JWT token

All queries are automatically filtered by the current account.

## Example Usage

### Generate Waste
```bash
POST /api/management/generate
Content-Type: application/json
X-Account-Id: 12345678-1234-1234-1234-123456789012

{
  "code": "WASTE-001",
  "description": "Industrial plastic waste",
  "wasteTypeId": "...",
  "quantity": 100,
  "unit": 1,
  "generatorId": "...",
  "isHazardous": false
}
```

### Collect and Transport
```bash
POST /api/management/collect
{
  "wasteId": "...",
  "quantity": 100,
  "collectorId": "...",
  "vehicleId": "...",
  "notes": "Collected from facility A"
}

POST /api/management/transport
{
  "wasteId": "...",
  "quantity": 100,
  "transporterId": "...",
  "vehicleId": "...",
  "originFacilityId": "...",
  "destinationFacilityId": "..."
}
```

### Get Inventory
```bash
GET /api/inventory?personId=...&facilityId=...
```

### Waste Bank
```bash
GET /api/waste/bank

POST /api/waste/{id}/publish-to-bank
{
  "description": "Good quality plastic for recycling",
  "price": 150.00
}
```

## Technologies

- **.NET 8** - Latest .NET framework
- **Entity Framework Core 8** - ORM
- **SQL Server** - Database
- **Swagger/OpenAPI** - API documentation
- **Clean Architecture** - Separation of concerns
- **Repository Pattern** - Data access abstraction
- **Unit of Work** - Transaction management

## Contributing

This is a complete implementation following Clean Architecture principles and waste management best practices including Basel Convention and EU Waste Framework Directive.

## License

MIT License

