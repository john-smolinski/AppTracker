import React, { useEffect } from "react";
import { useDispatch } from "react-redux";
import { fetchApplications } from "./redux/applicationsSlice";
import { fetchSources } from "./redux/sourcesSlice";
import { fetchOrganizations } from "./redux/organizataionsSlice";
import { fetchJobTitles } from "./redux/jobTitlesSlice";
import { fetchWorkEnvironments } from "./redux/workEnvironmentsSlice";
import store from "./redux/store";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Home from "./components/Pages/Home/Home";
import ApplicationsView from "./components/Pages/ApplicationsView/ApplicationsView";
import AddApplication from "./components/Pages/AddApplication/AddApplication";
import NotFound from "./components/Pages/NotFound/NotFound";
import Statistics from "./components/Pages/Statistics/Statistics";
import "./App.css";

export default function App() {
  const dispatch = useDispatch();

  useEffect(() => {
    // fetch application data on start with debug logging
    dispatch(fetchApplications()).then(() => {
      console.log("Applications fetched:", store.getState().applications);
    });
    dispatch(fetchSources()).then(() => {
      console.log("Sources fetched:", store.getState().appSources);
    });
    //
    dispatch(fetchOrganizations()).then(() => {
      console.log("Organizations fetched:", store.getState().organizations);
    });
    dispatch(fetchJobTitles()).then(() => {
      console.log("Job Titles fetched:", store.getState().jobTitles);
    });
    dispatch(fetchWorkEnvironments()).then(() => {
      console.log(
        "Work Environments fetched:",
        store.getState().workEnvironments
      );
    });
  }, [dispatch]);

  return (
    <Router>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/applications" element={<ApplicationsView />} />
        <Route path="/add" element={<AddApplication />} />
        <Route path="/statistics" element={<Statistics />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </Router>
  );
}
