var calendar;

function initializeFullCalendar() {
  var ID = document.getElementById("calendar");

  calendar = new FullCalendar.Calendar(ID, {
    initialView: "dayGridMonth",
    headerToolbar: {
      left: "prev,next",
      center: "title",
      right: "newBtn today",
    },
    events: [],
    eventClick: function (info) {
      if (info.event.id.length > 5) {
        window.location.href = `/view-journal/${info.event.id}`;
      }
    },
    customButtons: {
      newBtn: {
        text: "New Journal",
        click: function () {
          window.location.href = "/add-journal";
        },
      },
    },
    height: 750,
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
      end: e.end,
      allDay: true,
      color: e.color,
    });
  });
}

window.initializeFullCalendar = initializeFullCalendar;
window.createCalendarEvents = createCalendarEvents;
