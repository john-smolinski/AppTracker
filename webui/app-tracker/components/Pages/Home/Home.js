import React from "react";
import Menu from "@/components/Menu/Menu";
import Summary from "@/components/Features/Summary/Summary";
import ApplicationsCalendar from "@/components/Features/ApplicationsCalendar/ApplicationsCalendar";

export default function Home() {
  return (
    <div className="app">
      <Menu />
      <Summary />
      <ApplicationsCalendar />
    </div>
  );
}
