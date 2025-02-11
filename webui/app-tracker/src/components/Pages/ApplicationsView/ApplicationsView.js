import React from "react";
import { useSelector } from "react-redux";
import { DataGrid, GridToolbar } from "@mui/x-data-grid";
import Menu from "../../Menu/Menu";
import "../../../App.css";
import "./ApplicationsView.css";

export default function ApplicationsView() {
  // application data from state
  const apps = useSelector((state) => state.applications.apps);

  // reformat the application data in a way that is easier to bind to the DataGrid
  const formattedAppData = apps.map((item) => ({
    id: item.id,
    applicationDate: item.applicationDate,
    source: item.source.name,
    organization: item.organization.name,
    jobTitle: item.jobTitle.name,
    workEnvironment: item.workEnvironment.name,
    city: item.city,
    state: item.state,
  }));

  // map the columns
  const columns = [
    { field: "id", headerName: "Id", width: 50 },
    { field: "applicationDate", headerName: "Date", width: 100 },
    { field: "source", headerName: "Source", width: 200 },
    { field: "organization", headerName: "Organization", width: 200 },
    { field: "jobTitle", headerName: "Job Title", width: 200 },
    { field: "workEnvironment", headerName: "Office", width: 100 },
    { field: "city", headerName: "City", width: 200 },
    { field: "state", headerName: "State", width: 100 },
  ];

  return (
    <div className="app">
      <Menu />
      <h2>Applications Submitted</h2>
      <div className="applications">
        <DataGrid
          rows={formattedAppData}
          columns={columns}
          pageSize={50}
          disableColumnFilter
          disableColumnSelector
          disableDensitySelector
          slots={{ toolbar: GridToolbar }}
          slotProps={{
            toolbar: {
              showQuickFilter: true,
              printOptions: { disableToolbarButton: true },
              csvOptions: { disableToolbarButton: true },
            },
          }}
        />
      </div>
    </div>
  );
}
