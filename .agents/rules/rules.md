# QLDSV_HQT_DBMS Workspace Coding Rules

This file defines the strict coding standards and architecture rules for developers and AI agents working on the `QLDSV_HQT_DBMS` project. All new implementations, bug fixes, and database updates must comply with these guidelines.

## Table of Contents
1. [1. Directory & Clean Architecture Structure](#1-directory-clean-architecture-structure) (Path: [src](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/src))
2. [2. No Hardcoded Constants](#2-no-hardcoded-constants) (Path: [src/QLDSV_HTC.Domain/Constants](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/src/QLDSV_HTC.Domain/Constants))
3. [3. Database Security & SQL Injection Mitigation](#3-database-security-sql-injection-mitigation) (Path: [src/Database/StoredProcedures](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/src/Database/StoredProcedures))
4. [4. Bulk Operations & Transaction Integrity](#4-bulk-operations-transaction-integrity) (Path: [src/QLDSV_HTC.Infrastructure/Repositories](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/src/QLDSV_HTC.Infrastructure/Repositories))
5. [5. Role-Based Access Control (RBAC) & CSRF Protection](#5-role-based-access-control-rbac-csrf-protection) (Path: [src/Database/Scripts](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/src/Database/Scripts))
6. [6. Build Warnings & Formatting](#6-build-warnings-formatting) (File: [Makefile](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/Makefile))
7. [7. Database Query Optimization Guidelines](#7-database-query-optimization-guidelines) (Path: [src/Database/StoredProcedures](file:///d:/CODE%20PLAYGROUND/Projects/HQTCSDL/QLDSV_HQT_DBMS/src/Database/StoredProcedures))

---

## 1. Directory & Clean Architecture structure

The codebase follows a layered clean architecture pattern:
- **`QLDSV_HTC.Domain`**: Contains core domain constants (`AppConstants`, `DbConstants`, `StoredProcedureConstants`, `RouteConstants`), entities, and enums. No dependencies on database or web technologies.
- **`QLDSV_HTC.Application`**: Contains DTOs, interfaces, and core business contracts.
- **`QLDSV_HTC.Infrastructure`**: Implementation of database repositories, database connections, and external clients.
- **`QLDSV_HTC.Web`**: MVC Web controllers, Views, Razor pages, static assets, and middleware.

---

## 2. No Hardcoded Constants

- **P0 Rule**: NEVER hardcode string literals for database columns, SQL parameters, table names, or stored procedure names directly inside controllers or repositories.
- **Remediation**:
  - Store database tables and column names inside `DbConstants.Columns`.
  - Store stored procedure names in `AppConstants.SpNames`.
  - Store stored procedure parameter names in `StoredProcedureConstants`.
  - Store route definitions in `RouteConstants`.

*Example Repository Pattern:*
```csharp
// WRONG:
new SqlParameter("@NIENKHOA", year)
MaLtc = Convert.ToInt32(row["MALTC"])

// CORRECT:
new SqlParameter(StoredProcedureConstants.GetSubjectGrades.SchoolYear, year)
MaLtc = Convert.ToInt32(row[DbConstants.Columns.Grade.CreditClassId])
```

---

## 3. Database Security & SQL Injection Mitigation

- **P0 Rule**: All database access must be fully parameterized. Direct string concatenation for building queries is strictly forbidden.
- **Stored Procedures**: Accessing the database should be done by invoking stored procedures via `BaseSqlRepository.ExecuteQueryAsync` or `ExecuteNonQueryAsync`.
- **Parameterization**: Pass values to SQL Server using `SqlParameter` objects (e.g. `new SqlParameter(parameterName, value)`).

---

## 4. Bulk Operations & Transaction Integrity

- **P0 Rule**: Bulk updates or bulk insertions (such as saving grades for a class) must NOT execute in a loop within C# repositories over separate database connections. This causes network lag (N+1 queries) and lacks transaction rollback capabilities.
- **TVP Approach (Table-Valued Parameters)**:
  - Create a User-Defined Table Type (UDT) in SQL Server:
    ```sql
    CREATE TYPE dbo.GradeEntryType AS TABLE ( ... );
    ```
  - Refactor the Stored Procedure to take the type as a `@Param dbo.GradeEntryType READONLY` parameter.
  - Construct a C# `DataTable` with identical column mapping, and pass it to the repository as a structured parameter (`SqlDbType.Structured`).
- **Explicit Transactions**: Stored procedures executing bulk operations must be wrapped inside an explicit SQL transaction block:
  ```sql
  BEGIN TRANSACTION;
  BEGIN TRY
      -- Bulk DML operations here
      COMMIT TRANSACTION;
  END TRY
  BEGIN CATCH
      IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
      THROW;
  END CATCH
  ```

---

## 5. Role-Based Access Control (RBAC) & CSRF Protection

- **Controller Security**:
  - Restrict controller access at the class level using the `[Authorize(Roles = ...)]` attribute.
  - The roles should match domain roles: `PGV` (Phòng Giáo vụ), `KHOA` (Khoa), and `SV` (Sinh viên).
  - Explicitly restrict write/modification endpoints to `PGV` and `KHOA` using `AppConstants.Groups.Faculty`.
- **Anti-CSRF**:
  - All state-modifying HTTP POST/PUT/DELETE endpoints in Web Controllers must be decorated with `[ValidateAntiForgeryToken]` (along with `@Html.AntiForgeryToken()` in views) to prevent Cross-Site Request Forgery.
- **Least Privilege DDL**:
  - Grant permissions explicitly to users/roles in `001-PhanQuyen.sql`.
  - If a user-defined table type is created, remember to grant execution and reference permissions on the type to the respective role:
    ```sql
    GRANT EXECUTE, REFERENCES ON TYPE::dbo.TypeName TO [KHOA];
    ```

---

## 6. Build Warnings & Formatting

- **P0 Rule**: The codebase must compile with **0 warnings** and **0 errors**.
- **SonarAnalyzer Compliance**:
  - **S3923**: Do not construct conditional structures (if/else) where all branches contain identical code logic. Simplify it.
  - **S101**: Follow standard PascalCase naming conventions for classes, interfaces, and methods.
  - **S1481**: Remove unused local variables or expressions immediately.
- **Code Style Checking**:
  - Always run `make format` (which executes `dotnet format --severity warn`) to run style and naming rules checks before finalizing changes.

---

## 7. Database Query Optimization Guidelines

To ensure high-performance execution plans and efficient database resource utilization, follow these query optimization rules in **descending order of priority** (Priority 1 is the most critical and has the highest optimization impact):

### 1. Priority 1: Selection and Projection Before Join
- **Rule**: Apply filter conditions (`WHERE` clauses) and select only the required columns (`SELECT col1, col2` instead of `SELECT *`) before performing table joins.
- **Why**: Minimizes the volume of intermediate data SQL Server has to process and buffer in memory during join operations.

### 2. Priority 2: Eliminate Unnecessary Joins (Connection Elimination)
- **Rule**: Remove redundant or unused `JOIN` clauses. If the query does not require columns from a joined table, do not include it. Use `EXISTS` or `IN` subqueries instead of full joins when merely validating existence.
- **Why**: Cuts down query execution plan complexity and prevents unnecessary table scans.

### 3. Priority 3: Factor Out Redundant Conditions
- **Rule**: Factor out common conditions in conditional clauses to simplify logic and enable better index matching.
- **Example**:
  - *Wrong*: `WHERE (MALOP = 'x' AND PHAI = 'nam') OR (MALOP = 'y' AND PHAI = 'nam')`
  - *Correct*: `WHERE MALOP IN ('x', 'y') AND PHAI = 'nam'`

### 4. Priority 4: Conditional Clause Ordering (Short-Circuit Evaluation)
- **Rule**:
  - In `AND` clauses: Place the condition with the **highest probability of being FALSE** first.
  - In `OR` clauses: Place the condition with the **highest probability of being TRUE** first.
- **Why**: SQL Server utilizes short-circuit evaluation. Putting high-probability failures first in `AND` blocks stops evaluation early, saving CPU cycles.

### 5. Priority 5: Index Matching and Hints
- **Rule**: Fields participating in query filter conditions (`WHERE`, `JOIN` predicates) should be ordered according to composite index key definitions. If necessary to force an optimal plan choice, explicitly specify the index hint using `WITH (INDEX = Ten_Index)`.
- **Why**: Assures SQL Server uses the appropriate non-clustered index instead of falling back to a full table scan.
