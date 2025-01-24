import React from "react";
import { useSelector } from "react-redux";
import { DataGrid } from "@mui/x-data-grid";
import Menu from "../../Menu/Menu";
import "../../../App.css";
import "./ApplicationsView.css";

export default function ApplicationsView() {
  // application data from state
  const apps = useSelector((state) => state.applications.apps);

  // reformat the application data in a way that is easier to bind to the DataGrid
  const formattedAppData = apps.map((item) => ({
    id: item.id,
    applicaitionDate: item.applicaitionDate,
    source: item.source.name,
    organization: item.organization.name,
    jobTitle: item.jobTitle.name,
    workEnvironment: item.workEnvironment.name,
  }));

  // map the columns
  const columns = [
    { field: "id", headerName: "Id", width: 50 },
    { field: "applicaitionDate", headerName: "Date", width: 100 },
    { field: "source", headerName: "Source", width: 200 },
    { field: "organization", headerName: "Organization", width: 200 },
    { field: "jobTitle", headerName: "Job Title", width: 200 },
    { field: "workEnvironment", headerName: "Office", width: 200 },
  ];

  return (
    <div className="app">
      <Menu />
      <h1>Applications Submitted</h1>
      <div className="applications">
        <DataGrid rows={formattedAppData} columns={columns} pageSize={50} />
      </div>
    </div>
  );
}
