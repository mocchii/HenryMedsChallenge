# README

## How to run
Restore the database from db/HenryMeds.bak
Open HenryMedsApp.sln
```
Built on .net8, using visual studio 2022.
Used SQL server and entityframeworks core.

I didn't feel like I had enough time, but I wanted to create 2 more tables. One table with clients and one table with providers. I would've set these two as foreign keys to the two existing tables.
I went in with the idea of maybe having a changable interval. The logic where per provider they might want to have a different appointment duration. Ie. one more dedicated specialist might want to reserve 30 minutes or 90 minutes a session etc. I also didn't get time to build out a validation for overlapping provider schedules on the same day. But this imagined we would have actually have a put endpoint to update the existing schedule for providers.

In terms of business aspect, I believe it's fully functional. I wrote one test case but it doesn't cover too much.
There is a hosted service running in the background, which sets the active flag to false for bookings that have not been reserved within the 30 minutes. I wanted to create a controller to control the on/off state for the service but didn't get to it.

The tables are
ClientBooking Table
ScheduleId
ClientId
ProviderId
StartDate
EndDate
IsReserved
Active
CreateDate

ProviderSchedule Table
ProviderScheduleId
ProviderId
StartDate
EndDate
Active
Interval
```
