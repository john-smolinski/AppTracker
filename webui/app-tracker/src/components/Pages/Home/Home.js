import React from "react";
import Menu from "../../Menu/Menu";
import Summary from "../../Features/Summary/Summary";
import ApplicationsCalendar from "../../ApplicationsCalendar";

export default function Home() {
  return (
    <div className="app">
      <Menu />
      <Summary />
      <ApplicationsCalendar />
    </div>
  );
}
