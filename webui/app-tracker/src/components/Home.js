import React from "react";
import Menu from "./Menu";
import Summary from "./Summary";
import ApplicationCalendar from "./ApplicationCalendar";

export default function Home() {
  return (
    <div className="app">
      <Menu />
      <Summary />
      <ApplicationCalendar />
    </div>
  );
}
