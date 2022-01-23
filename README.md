# MeetingScheduler
This API is used to schedule daily meeting times according to set conditions. It exposes endpoints to set meeting times, get meeting times and get free time slots

## Specs
- A meeting can’t be longer than 2 hours and must happen at either 00/30min timestamp
with minimum increments of 30min.
- The phone booth’s operating hours are between 9am-5pm GMT. A meeting cannot occur
outside of this time.
- A meeting can only be scheduled for a present or future date/time.
- If time ranges are not passed in for List APIs, assume start time is now, and end time to
be end-of-day-hour.
- Timestamp is passed in using ISO-8601 format, e.g. 2021-07-05T09:00:00Z.
- MeetingId should be a unique server-side generated identifier with low-probability of
key-collision.
- Assume all time ranges passed in are within a day’s operating-hour limit. e.g., a user
can’t create meetings/view time availability that span multiple days, but only for the same
day at most.
- For data storage, use an in-memory data structure. Don’t use an external data store.
- No need to store meetings that have already occurred on any prior day. e.g. if today is
08/30, there won’t be user queries for meetings/availability for any time happening on
08/29 or prior. This way we can limit the size of the data stored.


## API breakdown

| Action        | Route          | Resource  |
| ------------- |:-------------:| :----------:|
| **GET**     | `/ListMeetings/` | Get all created meetings |
| **POST**      | `/CreateMeeting/`      |   Create meeting |
| **GET** | `/ListAvailableSlots`    |    Get available meeting slots


## Built Using
* [.NET Core](https://dotnet.microsoft.com/en-us/learn/aspnet/what-is-aspnet-core/) - .NET Core
* [Swagger](https://swagger.io/) - API documentation
* [XUnit](https://xunit.net/) - Test framework
* [EFCore In-Memory DB](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/) - In-memory database

## Version
This project is currently in Version 1

## license
This project is licensed under the MIT License - see the [LICENSE](./LICENSE) file for details
