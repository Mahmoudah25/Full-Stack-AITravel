# AI Travel Planner — Full Stack

An AI-powered trip planning application that generates day-by-day itineraries, lets users book and pay for activities, rate their experience, and get real-time booking notifications.

**Live demo:** https://ai-travelf.vercel.app

---

## Overview

The user plans a trip by entering a destination, dates, number of days, and budget. The backend uses an AI service to generate a full itinerary — attractions, restaurants, and activities — enriched with live weather data and real place lookups. Users can book and pay for individual activities through Paymob, get notified in real time when a payment is confirmed, and rate activities afterward.

---

## Project Structure

This is a monorepo containing both applications:

```
Full-Stack-AITravel/
├── frontend/     # Vanilla HTML/CSS/JS client
└── backend/      # ASP.NET Core Web API (Clean Architecture)
```

---

## Tech Stack

### Frontend
- HTML5, CSS3, vanilla JavaScript (no framework)
- SignalR client for real-time booking notifications
- Deployed on **Vercel**

### Backend
- **ASP.NET Core Web API** (.NET 9)
- **Clean Architecture**: API → Application → Domain → Persistence / Infrastructure
- **MediatR** (CQRS pattern for commands/queries)
- **Entity Framework Core** with SQL Server
- **SignalR** for real-time notifications
- Deployed on **MonsterASP.NET**

### External Services
- **Groq** (LLM API) — itinerary generation
- **OpenWeatherMap** — weather forecasts per day
- **OpenStreetMap / Overpass API** — real place data (attractions, restaurants, etc.)
- **Paymob** — payment processing (currently EGP only)

---

## Architecture (Backend)

The backend follows Clean Architecture with four layers:

| Layer | Responsibility |
|---|---|
| `AITravelB.Domain` | Core entities (`Trip`, `Activity`, `Booking`, `Rating`) and business rules |
| `AITravelB.Application` | Use cases via MediatR commands/queries (`CreateTrip`, `GenerateItinerary`, `CreateBooking`, `InitiatePayment`, `CreateRating`) |
| `AITravelB.Infrastructure` | External service integrations (Groq, OpenWeather, OSM Places, Paymob) |
| `AITravelB.Persistence` | EF Core `DbContext`, entity configurations, migrations |
| `AITravelB.API` | Controllers, SignalR hub, DI wiring, CORS, Swagger |

---

## Features

-  **Trip creation** — destination, dates, days, budget
-  **AI-generated itineraries** — day-by-day activities with times, types, and estimated costs
-  **Live weather** per day of the trip
-  **Real places** — attractions, restaurants, and activities pulled from OpenStreetMap, each linked to Google Maps
-  **Booking & payment** — book any paid activity and pay via Paymob (opens in a new tab)
-  **Real-time notifications** — SignalR pushes booking status updates (e.g. payment confirmed) live to the client
-  **Ratings** — rate activities after a trip, with duplicate prevention per activity/email
-  **Currency-aware display** — trip costs shown in the selected currency (payment currently processed in EGP only, per the connected Paymob account)

---

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/Trips` | Create a new trip |
| GET | `/api/Trips/{id}` | Get a trip by ID |
| POST | `/api/Trips/generate-itinerary` | Generate an AI itinerary for a trip |
| POST | `/api/Bookings` | Create a booking for an activity |
| POST | `/api/Bookings/{bookingId}/initiate-payment` | Start a Paymob payment session |
| POST | `/api/webhooks/paymob` | Paymob payment confirmation webhook |
| POST | `/api/Rating` | Submit a rating for an activity |

Full interactive docs available via Swagger at `/swagger` on the deployed API.

---

## Running Locally

### Backend

1. Open `backend/AITravelB.sln` in Visual Studio.
2. Create `backend/AITravelB.API/appsettings.json` (not tracked in git) with:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "your SQL Server connection string"
     },
     "ExternalServices": {
       "AiProvider": {
         "ApiKey": "your Groq API key",
         "BaseUrl": "https://api.groq.com/openai/v1",
         "Model": "openai/gpt-oss-120b"
       },
       "OpenWeather": {
         "ApiKey": "your OpenWeatherMap API key",
         "BaseUrl": "https://api.openweathermap.org/data/2.5"
       },
       "Paymob": {
         "BaseUrl": "https://accept.paymob.com/api/",
         "ApiKey": "your Paymob API key",
         "IntegrationId": "your integration id",
         "IFrameId": "your iframe id",
         "HmacSecret": "your HMAC secret"
       }
     }
   }
   ```
3. Apply EF Core migrations:
   ```bash
   dotnet ef database update --project AITravelB.Persistence --startup-project AITravelB.API
   ```
4. Run the API (F5 or `dotnet run` from `AITravelB.API`).

### Frontend

1. Open `frontend/` with any static server (e.g. VS Code Live Server).
2. Update the API base URL in `js/trip-service.js` if your backend runs on a different address.

---

## Known Limitations

- Payments are processed in **EGP only** — the connected Paymob merchant account does not support other currencies. Trip budgets can be entered/displayed in other currencies, but bookings are settled in EGP.
- No authentication layer yet — customer email is used as a simple identifier for bookings, ratings, and notifications.

---

## Roadmap Ideas

- User authentication & accounts
- Multi-currency payment support (via conversion or multiple Paymob integrations)
- Trip editing and activity swapping after generation
- Persisted booking/payment status shown directly on the itinerary across sessions
