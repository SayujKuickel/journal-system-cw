var calendar;

function initializeFullCalendar() {
  var ID = document.getElementById("calendar");

  calendar = new FullCalendar.Calendar(ID, {
    initialView: "dayGridMonth",
    headerToolbar: {
      left: "prev,next today",
      center: "title",
      right: "",
    },
    events: [],
    dateClick: function (info) {
      if (window.DotNet && DotNet.invokeMethodAsync) {
        DotNet.invokeMethodAsync(
          "JournalApplication",
          "NavigateToDate",
          info.dateStr,
        );
      }
    },
    eventClick: function (info) {
      if (window.DotNet && DotNet.invokeMethodAsync) {
        DotNet.invokeMethodAsync(
          "JournalApplication",
          "NavigateToEntry",
          info.event.id,
        );
      }
    },
    height: 600,
  });

  calendar.render();
}

function createCalendarEvents(events) {
  if (!calendar) return;

  calendar.removeAllEvents();

  events.forEach((e) => {
    calendar.addEvent({
      id: e.id,
      title: e.title,
      start: e.start,
      allDay: true,
      color: e.color,
    });
  });
}

window.initializeFullCalendar = initializeFullCalendar;
window.createCalendarEvents = createCalendarEvents;
