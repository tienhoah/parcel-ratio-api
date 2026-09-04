# Parcel Ratio Study API — project brief

> Point Claude Code at this file at the start of a session. It carries the decisions already made so they don't get re-argued.

## What this is

A small property-assessment API and map front end, built as **interview preparation** for a senior GIS developer role at Esri Canada. Their product (GAMA) is a Geographic-Assisted Mass Appraisal system: assessors value thousands of properties using GIS.

**The developer is a 7-year JavaScript/TypeScript/React developer with four days of C# behind him.** He has watched a C# course (fundamentals, OOP, LINQ, delegates, async) and written none of it. The point of this project is to change *"I haven't written C#"* into *"I built a small Web API — here's what transferred and what surprised me."*

## The one rule

**Small. Two to three days total, including the front end.** Every extra feature is time not spent rehearsing interview answers, which is what actually wins the interview. If a suggestion would add a day, it is the wrong suggestion.

**He must be able to explain every line afterwards.** That is the entire deliverable — the code is a means. So: explain before generating, go in small steps, and prefer the boring obvious version over the clever one. Do not hand over large blocks of finished code he hasn't reasoned through.

## Stack — decided, don't re-open

| Piece | Choice | Why |
|---|---|---|
| API | ASP.NET Core Web API, controllers | Their stack is C#/.NET |
| ORM | **EF Core** | Standard .NET data layer; likely what GAMA uses |
| Database | **PostgreSQL + PostGIS**, via Docker | For real spatial queries with a spatial index |
| Front end | **Angular** + **MapLibre GL** | Angular is their stack and his weak spot; MapLibre he already knows |
| Editor | Zed with the C# extension (OmniSharp) | No step debugger — use `dotnet run`, `Console.WriteLine`, curl |

**On Postgres rather than SQL Server** (which is what Esri actually uses): SQL Server is fiddly on Apple Silicon, and EF Core is the same ORM either way — only the provider package differs, and spatial types go through NetTopologySuite in both. The skill transfers and he can say so honestly.

## Build order — this sequence is deliberate

1. **JSON-file implementation first.** Get the API shape working — model, interface, service, DI, controller, endpoints — reading `parcels.json`.
2. **Then swap to EF Core + PostGIS behind the same `IParcelService`.**

Do not start with the database. Learning DI, LINQ, async and controllers at once is enough; adding Docker, EF Core and migrations on top is too much in one sitting.

The swap is also the point: the interface stays identical while the implementation changes. That becomes a real sentence — *"I started with a JSON-backed implementation and swapped it for EF Core behind the same interface, which is exactly why the interface was there."*

## Domain — enough to build it

**Assessment ratio** = assessed value ÷ sale price. Above 1.0 the property is over-assessed, below 1.0 under-assessed. Only computable for properties that recently **sold**.

**Median ratio** — is the roll systematically high or low? Target 0.90–1.10.

**COD** (coefficient of dispersion) — how *consistent* the assessments are: mean absolute deviation from the median, as a percentage of the median. Lower is better; target ≤ 15. High COD means similar houses valued inconsistently, which is the unfairness assessors get appealed over.

```
COD = 100 × ( Σ |ratio − median| / n ) / median
```

**Comparables** — the nearest recent sales to a given property. What an assessor looks at to judge whether a valuation is defensible.

**The interesting case, and it is in the seed data:** one neighbourhood (Marpole) has median ratio ≈ 0.845 with COD ≈ 2.8. *Consistent, but low* — the assessments agree with each other and sit ~15% below market. That is a real finding and it is the demo.

## Data

`parcels.json` — 52 parcels across 5 Vancouver neighbourhoods.

```json
{
  "id": "P1003",
  "address": "3042 Arbutus St",
  "neighbourhood": "Kitsilano",
  "latitude": 49.2586,
  "longitude": -123.164205,
  "landAreaSqFt": 3200,
  "buildingAreaSqFt": 1246,
  "yearBuilt": 1936,
  "bedrooms": 6,
  "assessedValue": 2040000,
  "lastSalePrice": 2289000,
  "lastSaleDate": "2025-04-21"
}
```

**30 of the 52 have a sale; 22 have `lastSalePrice: null` and `lastSaleDate: null`.** That is deliberate and it matters:

- `LastSalePrice` and `LastSaleDate` are **nullable** on the model
- **Every statistic filters to sold parcels first** — a parcel with no sale has no ratio
- The map renders them as a third state (hollow, dashed outline, "no recent sale")
- Comparables can only ever be sold parcels

## Endpoints

```
GET  /parcels                        list (support page/pageSize even if the UI doesn't use it)
GET  /parcels/{id}                   one parcel, 404 when missing
GET  /parcels/{id}/comparables       5 nearest recent sales, same neighbourhood
GET  /parcels/within?bbox=...        parcels inside a bounding box + ratio stats for them
```

`within` is the centrepiece — once on Postgres it should be a **real spatial query against a spatial index**, not a LINQ filter over everything.

**Comparables logic:** exclude the parcel itself → keep only sold parcels → same neighbourhood → order by distance → take 5. Straight-line distance is fine at this scale; a proper geodesic calculation would matter over larger areas.

## Front end — three screens, one shell

There is a published mockup. Shell = top bar, map, square-draw tool. **Only the right-hand panel changes:**

1. **All sales** — legend, median ratio, COD for everything
2. **Square drawn** — count, median, COD for the selection, the finding in a sentence, parcels listed by ratio
3. **Parcel clicked** — assessed value, last sale, ratio, four attributes, five comparables

Square draw only. No other draw modes.

**Ratio colour ramp** (validated diverging, colour-blind safe):

| Ratio | Colour |
|---|---|
| < 0.90 | `#1c5cab` |
| 0.90–0.96 | `#5598e7` |
| 0.96–1.04 | `#e8e6e0` |
| 1.04–1.10 | `#e8807f` |
| > 1.10 | `#b02c2c` |
| no sale | hollow, `#b8b6ae` dashed outline |

## Out of scope — do not suggest these

Authentication · unit or integration tests · CI · Docker Compose for the API itself · rate limiting · logging frameworks · AutoMapper · CQRS or MediatR · repository pattern on top of EF Core · Blazor · SignalR · role-based anything · roll-year and sales-window filters · attribute filters (year built, price) · PRD · histograms · trimming outliers · arm's-length flags · time-adjusted sale prices.

Several of those are real parts of a ratio study. They were deliberately cut. **Leave them cut.**

## Deploy

Azure App Service, free tier — **only after the front end works**, and drop it without regret if time runs out. Deployment was not a gap the employer flagged.

## What to capture as you go

Three observations about what surprised him in C#/.NET, in his own words. That is the actual output of this project — the sentences, not the repo. Prompt him for them at each milestone rather than at the end.
