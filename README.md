# RoppaCorp Carpark Management System

Lecturer-friendly ASP.NET Core MVC prototype for the CET310 Software Enterprise assignment.

## What the prototype includes

- ASP.NET Core MVC UI with ASP.NET Identity login
- Role-based access for `FacilitiesManager`, `ReceptionistAdmin`, and `ITTechnician`
- Availability search by site, date/time, and bay type
- Booking create, edit, and cancel flows
- Priority bookings for facilities managers only
- Visual carpark layouts for Site A, Site B, and Site C
- Placeholder layout editor for facilities managers and IT technicians
- SQLite database seeded with the exact space totals from the assignment brief

## Demo accounts

- `manager@roppacorp.local` / `LecturerDemo1!`
- `reception@roppacorp.local` / `LecturerDemo1!`
- `tech@roppacorp.local` / `LecturerDemo1!`

## Run locally

```powershell
dotnet restore
dotnet ef database update
dotnet run
```

The app seeds roles, users, sites, spaces, layouts, and sample bookings on startup.

## Suggested lecturer demo flow

1. Log in as `reception@roppacorp.local`.
2. Search Site B visitor bays in the default demo window.
3. Create a visitor booking and show validation.
4. Open the Site B layout to show the visual change.
5. Log in as `manager@roppacorp.local` and create a priority booking.
6. Log in as `tech@roppacorp.local` and edit layout positions.
