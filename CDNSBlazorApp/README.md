# CDNS Blazor Server Application

A comprehensive .NET Core 8 Blazor Server application for managing Central Directory of National Saving (CDNS) financial instruments and data.

## Features

### 1. Authentication & Authorization
- **Admin User Seeding**: Default admin account pre-configured
  - Username: `Admin`
  - Password: `Admin`
- Role-based access control (Admin, Manager, User roles)
- JWT token-based authentication for API endpoints

### 2. Database Tables (Code First Approach)
The application includes the following tables:
- **INSTITUATION**: Institution information
- **Instrument**: Financial instruments data
- **Series**: Series information linked to instruments
- **SERIES_PATTERN**: Pattern information for series
- **SUBSCRIPTIONS**: Subscription transactions
- **PAYMENTS**: Payment transactions

### 3. Excel File Upload
- Separate upload functionality for each table
- Supports `.xlsx` and `.xls` formats
- Automatic data validation and error reporting
- Bulk import capability

### 4. RESTful API with JWT Authentication
- Secure API endpoints for all CRUD operations
- JWT token-based authentication
- Role-based authorization
- Comprehensive error handling

### 5. User Management Module
- Create, read, update, and delete users
- Assign roles to users
- Manage user status (active/inactive)
- Admin-only access

### 6. Attractive Frontend UI
- Modern, responsive design
- Intuitive dashboard
- Beautiful gradient color schemes
- Interactive cards and forms
- Mobile-friendly layout

## Technology Stack

- **.NET Core 8.0**
- **Blazor Server**
- **Entity Framework Core 8.0**
- **SQL Server**
- **ASP.NET Core Identity**
- **JWT Authentication**
- **EPPlus** (for Excel processing)

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or SQL Server instance)
- Visual Studio 2022 or VS Code

## Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd CDNSBlazorApp
```

### 2. Configure Database Connection

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CDNSBlazorAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 3. Install Dependencies

```bash
dotnet restore
```

### 4. Apply Database Migrations

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

The application will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

## Default Admin Credentials

- **Username**: Admin
- **Password**: Admin

## API Endpoints

### Authentication
- `POST /api/auth/login` - Login and get JWT token
- `POST /api/auth/register` - Register new user

### Instituations
- `GET /api/instituation` - Get all instituations
- `GET /api/instituation/{id}` - Get instituation by ID
- `POST /api/instituation` - Create new instituation (Admin/Manager)
- `PUT /api/instituation/{id}` - Update instituation (Admin/Manager)
- `DELETE /api/instituation/{id}` - Delete instituation (Admin)

### Instruments
- `GET /api/instrument` - Get all instruments
- `GET /api/instrument/{id}` - Get instrument by ID
- `POST /api/instrument` - Create new instrument (Admin/Manager)
- `PUT /api/instrument/{id}` - Update instrument (Admin/Manager)
- `DELETE /api/instrument/{id}` - Delete instrument (Admin)

### Series
- `GET /api/series` - Get all series
- `GET /api/series/{serieId}/{traNo}` - Get series by composite key
- `POST /api/series` - Create new series (Admin/Manager)
- `PUT /api/series/{serieId}/{traNo}` - Update series (Admin/Manager)
- `DELETE /api/series/{serieId}/{traNo}` - Delete series (Admin)

### Subscriptions
- `GET /api/subscription` - Get all subscriptions
- `POST /api/subscription` - Create subscription
- `PUT /api/subscription/{id}/{traNo}` - Update subscription (Admin/Manager)
- `DELETE /api/subscription/{id}/{traNo}` - Delete subscription (Admin)

### Payments
- `GET /api/payment` - Get all payments
- `POST /api/payment` - Create payment
- `PUT /api/payment/{id}/{traNo}` - Update payment (Admin/Manager)
- `DELETE /api/payment/{id}/{traNo}` - Delete payment (Admin)

### Excel Upload
- `POST /api/excelupload/instituation` - Upload instituation Excel
- `POST /api/excelupload/instrument` - Upload instrument Excel
- `POST /api/excelupload/series` - Upload series Excel
- `POST /api/excelupload/seriespattern` - Upload series pattern Excel
- `POST /api/excelupload/subscription` - Upload subscription Excel
- `POST /api/excelupload/payment` - Upload payment Excel

### User Management (Admin Only)
- `GET /api/usermanagement/users` - Get all users
- `GET /api/usermanagement/users/{id}` - Get user by ID
- `POST /api/usermanagement/users` - Create new user
- `PUT /api/usermanagement/users/{id}` - Update user
- `DELETE /api/usermanagement/users/{id}` - Delete user
- `POST /api/usermanagement/users/{id}/roles` - Assign role to user
- `DELETE /api/usermanagement/users/{id}/roles/{role}` - Remove role from user
- `GET /api/usermanagement/roles` - Get all roles
- `POST /api/usermanagement/roles` - Create new role

## Excel File Format

### Column Order for Each Table

#### INSTITUATION
1. INSTITUATION_ID
2. INSTITUATION_NAME
3. CONTACT_EMAIL
4. CREATED_AT
5. CREATED_AT_VALUE_DATE

#### Instrument
1. INSTRUMENT_ID through 26. DEBTOR_ID (see schema in code)

#### Series
1. SERIE_ID
2. SERIE_TRA_NO
3. SERIE_AMT
4. SERIE_CURRENCY
5. CD_REORG_GRP

#### SERIES_PATTERN
1. SERIE_PAT_ID
2. SERIE_PAT_TRA_NO
3. CD_PAT_TYPE
4. CD_PERIODICITY
5. CG_PERIODICITY
6. D_FIRST_PAYMENT
7. D_LAST_PAYMENT
8. AMT
9. PERCENTAGE

#### SUBSCRIPTIONS
1. SUBSCRIPTION_ID
2. SUBSCRIPTION_ID_TRA_NO
3. TR_DATE
4. RECEIVED_DATE
5. CD_TRANSACTION_TYPE
6. LOCAL_EXCHANGE_DATE
7. CU_BASE
8. RECEIPTS_AMOUNT
9. CD_EXEC_MODE

#### PAYMENTS
1. PAYMENT_ID
2. PAYMENT_TRA_NO
3. SCH_PAYMENT_DATE
4. MADE_DATE
5. RECEIVED_DATE
6. LOCAL_EXCH_RATE_DATE
7. CD_PAYMENT_MODE
8. AMOUNT
9. CU_BASE
10. CD_TRANSACTION_TYPE
11. CD_AMOUNT_DIFF

## Application Structure

```
CDNSBlazorApp/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   └── Pages/
│       ├── Login.razor
│       ├── Dashboard.razor
│       ├── ExcelUpload.razor
│       └── UserManagement.razor
├── Controllers/
│   ├── AuthController.cs
│   ├── InstituationController.cs
│   ├── InstrumentController.cs
│   ├── SeriesController.cs
│   ├── SubscriptionController.cs
│   ├── PaymentController.cs
│   ├── ExcelUploadController.cs
│   └── UserManagementController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Models/
│   ├── ApplicationUser.cs
│   ├── Instituation.cs
│   ├── Instrument.cs
│   ├── Series.cs
│   ├── SeriesPattern.cs
│   ├── Subscription.cs
│   └── Payment.cs
├── Services/
│   ├── AuthService.cs
│   └── SeedDataService.cs
├── Program.cs
└── appsettings.json
```

## Security Features

- Password hashing using ASP.NET Core Identity
- JWT token-based API authentication
- Role-based authorization
- Secure password requirements (configurable)
- HTTPS enforcement in production

## Validation

- Model validation using Data Annotations
- Required field validation
- Email format validation
- Decimal precision validation
- Date format validation
- Composite key validation

## License

This project is licensed under the MIT License.

## Support

For issues and questions, please create an issue in the repository.
