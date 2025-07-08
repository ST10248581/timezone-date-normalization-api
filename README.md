# Date/Time Normalization & Timezone Safety

## 📌 Problem

In distributed systems where APIs and browsers operate in different time zones (e.g., API in UTC, browser in UTC+2), **naive DateTime and DateOnly values** can be silently misinterpreted. This leads to:

- Dates showing up a day earlier/later.
- Time shifts during round-trips.
- `new Date("yyyy-MM-dd")` being treated as midnight UTC, not local.

## ✅ Solution
This project implements a **safe, centralized timezone normalization strategy** using:

- `ZonedDateConverter`: Custom JSON converter for `DateTime` and `DateTime?` that:
  - Deserializes values assuming client time zone (from `X-Timezone` header).
  - Serializes values as UTC.
- `DateLogic`: Helper class to:
  - Set client and server time zones from config or header.
  - Convert safely between `DateOnly` and `DateTime`.
  - Return server-local equivalent of `DateTime.Now` via `GetServerDateTimeNow()`.

## 🚀 Usage

### 1. Startup Configuration
In `Program.cs`:
Set timezones at app start or per request using DateLogic.
```
var dateLogic = new DateLogic();
dateLogic.setServerTimeZone();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new ZonedDateConverter());
});
```


### 2. Setting Time Zones
```
var logic = new DateLogic();
logic.setServerTimeZone(); // from AppSettings.ServerTimeZoneId
logic.setClientTimeZone(timeZoneHeaderValue); // from "X-Timezone" header
```

### 3. Get Correct Local Time
```
var now = new DateLogic().GetServerDateTimeNow();
```

### 4. Data Models
No changes needed. Just use DateTime, DateOnly, and their nullable forms normally.

## 📝 Angular Usage Notes

### 🛑 Do not use new Date(string) on values returned by the API. It will assume UTC and apply silent timezone shifts.
### ✅ Use a custom parser like:
```
function parseDateOnly(dateStr: string): Date {
  const [y, m, d] = dateStr.split('-').map(Number);
  return new Date(y, m - 1, d); // Local time, no shift
}
```

### 🕑 With DevExtreme:

Use displayFormat and pickerType to control input/output.
Always send X-Timezone header (e.g., "Africa/Johannesburg").

### 📦 Example Header
✅ Intl.DateTimeFormat().resolvedOptions().timeZone gets the user's local IANA timezone (e.g. "Africa/Johannesburg").
```
import { HttpClient, HttpHeaders } from '@angular/common/http';

constructor(private http: HttpClient) {}

submitWithTimezone(model: any) {
  const headers = new HttpHeaders({
    'X-Timezone': Intl.DateTimeFormat().resolvedOptions().timeZone
  });

  return this.http.post('/api/your-endpoint', model, { headers }).subscribe();
}
```

## 📎 Summary
- **.NET API:** Deserialize and serialize dates with timezone awareness
- **DateLogic:** Set and manage time zones, convert safely
- **Angular:** Avoid implicit conversions, parse explicitly

 ---
made by Troy Krause
