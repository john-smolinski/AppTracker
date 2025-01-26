import React from "react";
import { useSelector } from "react-redux";
import {
  startOfWeek,
  addWeeks,
  eachDayOfInterval,
  format,
  getDay,
} from "date-fns";
import "./ApplicationsCalendar.css";

export default function ApplicationCalendar() {
  // access applications from Redux store
  const applications = useSelector((state) => state.applications.apps);

  // transform applications into a dictionary with dates as keys
  const contributions = applications.reduce((acc, app) => {
    const date = app.applicaitionDate;
    if (acc[date]) {
      acc[date] += 1;
    } else {
      acc[date] = 1;
    }
    return acc;
  }, {});

  // get the current week and previous 51 weeks
  const today = new Date();
  const startDate = startOfWeek(addWeeks(today, -51));
  const endDate = today; // End at the current week

  // generate all days in the last 52 weeks
  const days = eachDayOfInterval({ start: startDate, end: endDate });

  // map each day to its contribution level (0 if no data)
  const dayMap = days.map((day) => ({
    date: day,
    level: contributions[format(day, "yyyy-MM-dd")] || 0,
  }));

  // group days by week
  const weeks = [];
  let currentWeek = [];
  dayMap.forEach((day) => {
    if (getDay(day.date) === 0 && currentWeek.length) {
      weeks.push(currentWeek);
      currentWeek = [];
    }
    currentWeek.push(day);
  });
  if (currentWeek.length) weeks.push(currentWeek);

  // generate month labels
  const monthLabels = [];
  let currentMonth = null;
  weeks.forEach((week, weekIndex) => {
    const firstDayOfWeek = week[0]?.date;
    const month = format(firstDayOfWeek, "MMM");
    if (month !== currentMonth) {
      currentMonth = month;
      monthLabels.push({ weekIndex, label: month });
    }
  });

  return (
    <div>
      <h2>Calendar</h2>
      <div className="calendar">
        {/* Month Labels */}
        <div className="calendar-month-labels">
          {monthLabels.map((month) => (
            <div
              key={month.weekIndex}
              className="calendar-month-label"
              style={{ gridColumnStart: month.weekIndex + 1 }}
            >
              {month.label}
            </div>
          ))}
        </div>

        <h2 className="calendar-title">
          Total of {applications.length} applications submitted in last 52 weeks
        </h2>

        {/* Weekday Labels */}
        <div className="calendar-weekday-labels">
          {["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"].map(
            (day, index) => (
              <div key={index} className="calendar-weekday-label">
                {day}
              </div>
            )
          )}
        </div>

        {/* Calendar Grid */}
        <div className="calendar-grid">
          {weeks.map((week, weekIndex) => (
            <div key={weekIndex} className="calendar-week">
              {week.map((day) => (
                <div
                  key={day.date}
                  className={`calendar-day level-${
                    day.level < 5 ? day.level : 4
                  }`}
                  title={`${format(day.date, "MMM d, yyyy")}: ${
                    day.level
                  } applications`}
                ></div>
              ))}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
