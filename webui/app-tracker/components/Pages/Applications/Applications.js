import { useSelector } from "react-redux";
import { useRouter } from "next/router";
import { DataGrid, GridToolbar } from "@mui/x-data-grid";
import Menu from "@/components/Menu/Menu";
import styles from "./Applications.module.css";

export default function Applications() {
  // application data from state
  const apps = useSelector((state) => state.applications.apps);
  const router = useRouter();

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

  const handleRowDoubleClick = (params) => {
    router.push(`/applications/${params.id}`);
  };

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
      <div className={styles.applications}>
        <DataGrid
          sx={{ cursor: "pointer" }}
          rows={formattedAppData}
          columns={columns}
          pageSize={50}
          disableColumnFilter
          disableColumnSelector
          disableDensitySelector
          onRowClick={handleRowDoubleClick}
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
